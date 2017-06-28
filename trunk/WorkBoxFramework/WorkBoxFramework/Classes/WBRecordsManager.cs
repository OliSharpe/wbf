using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework
{
    public class WBRecordsManager : IDisposable
    {
        #region Constants

        public const String REPLACING_ACTION__NOTHING = "Nothing";
        public const String REPLACING_ACTION__ARCHIVE = "Archive";

        public const String FILE_TYPES_LIST_NAME = "FileTypes";
        public const String CHECK_BOXES_LIST_NAME = "CheckBoxes";
        public const String FILE_TYPES_LIST_TITLE = "File Types";
        public const String CHECK_BOXES_LIST_TITLE = "Check Boxes";

        public const string VIEW_MODE__NEW = "New";
        public const string VIEW_MODE__REPLACE = "Replace";
        public const string VIEW_MODE__BROWSE_FOLDERS = "Browse Folders";
        public const string VIEW_MODE__BROWSE_DOCUMENTS = "Browse Documents";

        #endregion


        #region Constructors

        private WBFarm _farm;
        private WBRecordsLibraries _libraries;

        private String _callingUserLogin = null;

        public WBRecordsManager(String callingUserLogin)
        {
            WBLogging.Debug("In WBRecordsManager() constructor");

            _farm = WBFarm.Local;
            _libraries = new WBRecordsLibraries(this);
            _callingUserLogin = callingUserLogin;

            WBLogging.Debug("Finished WBRecordsManager() constructor");
        }

        #endregion

        #region Properties

        public WBRecordsLibraries Libraries
        {
            get { return _libraries; }
        }

        public WBTaxonomy RecordsTypesTaxonomy { get { return Libraries.ProtectedMasterLibrary.RecordsTypesTaxonomy; } }
        public WBTaxonomy TeamsTaxonomy { get { return Libraries.ProtectedMasterLibrary.TeamsTaxonomy; } }
        public WBTaxonomy SeriesTagsTaxonomy { get { return Libraries.ProtectedMasterLibrary.SeriesTagsTaxonomy; } }
        public WBTaxonomy SubjectTagsTaxonomy { get { return Libraries.ProtectedMasterLibrary.SubjectTagsTaxonomy; } }
        public WBTaxonomy FunctionalAreasTaxonomy { get { return Libraries.ProtectedMasterLibrary.FunctionalAreasTaxonomy; } }
            
        #endregion


        #region Methods

        /*
        public WBPublishingProcess PublishDocument(String documentURL)
        {
            WBTaskFeedback feedback = null;

            using (WorkBox workBox = new WorkBox(documentURL))
            {
                feedback = PublishDocument(workBox, documentURL);
            }

            return feedback;
        }

        public WBPublishingProcess PublishDocument(String documentURL, String replacingRecordID, String replacingAction)
        {
            WBTaskFeedback feedback = null;

            using (WorkBox workBox = new WorkBox(documentURL))
            {
                feedback = PublishDocument(workBox, documentURL, replacingRecordID, replacingAction);
            }

            return feedback;
        }

        public WBPublishingProcess PublishDocument(WorkBox workBox, String documentURL)
        {
            return PublishDocument(workBox, documentURL, null, null);
        }

        public WBPublishingProcess PublishDocument(WorkBox workBox, String documentURL, String replacingRecordID, String replacingAction)
        {
            SPListItem item = workBox.Web.GetListItem(documentURL);
            if (item == null)
            {
                WBTaskFeedback feedback = new WBTaskFeedback(WBTaskFeedback.TASK_TYPE__PUBLISH, documentURL);
                feedback.Failed("Couldn't find document to publish with URL: " + documentURL);
                return feedback;
            }

            return PublishDocument(workBox, new WBDocument(item), replacingRecordID, replacingAction, new WBItem());
        }

         */
 
        public WBPublishingProcess PublishDocument(WorkBox workBox, WBDocument document)
        {
            throw new NotImplementedException("This method is no longer being used!!");
         //   return PublishDocument(workBox, document, null, null, new WBItem());
        }
        
        public WBPublishingProcess PublishDocument(WBPublishingProcess process)
        {
            SPListItem currentItem = process.CurrentItem;
            WBDocument document = new WBDocument(process.WorkBox, currentItem);
            
            WBTaskFeedback feedback = new WBTaskFeedback(WBTaskFeedback.TASK_TYPE__PUBLISH, process.CurrentItemID);
            feedback.PrettyName = document.Name;

            process.LastTaskFeedback = feedback;

            // Just check that the IAO at time of publishing is captured:
            process.AddExtraMetadata(WBColumn.IAOAtTimeOfPublishing, process.OwningTeamsIAOAtTimeOfPublishing);

            if (SPContext.Current != null)
            {
                process.AddExtraMetadataIfMissing(WBColumn.PublishedBy, SPContext.Current.Web.CurrentUser);
            }
            process.AddExtraMetadataIfMissing(WBColumn.DatePublished, DateTime.Now);
            if (process.ProtectiveZone != WBRecordsType.PROTECTIVE_ZONE__PROTECTED)
            {
                // If the document is going on the public or public extranet zones then let's set a review date for 2 years from now:
                process.AddExtraMetadataIfMissing(WBColumn.ReviewDate, DateTime.Now.AddYears(2));
            }

            if (process.RecordsTypeTaxonomy == null)
            {
                WBLogging.Debug("Yeah - the process.RecordsTypeTaxonomy == null !! ");
            }
            else
            {
                WBLogging.Debug("No - the process.RecordsTypeTaxonomy was NOT  null !! ");
            }

            try
            {
                // Setting the various keys metadata values on the document to be published:
                WBRecordsType recordsType = new WBRecordsType(process.RecordsTypeTaxonomy, process.RecordsTypeUIControlValue);
                document.RecordsType = recordsType;
                document.FunctionalArea = new WBTermCollection<WBTerm>(process.FunctionalAreasTaxonomy, process.FunctionalAreaUIControlValue);
                document.SubjectTags = new WBTermCollection<WBSubjectTag>(process.SubjectTagsTaxonomy, process.SubjectTagsUIControlValue);
                document.OwningTeam = new WBTeam(process.TeamsTaxonomy, process.OwningTeamUIControlValue);
                document.InvolvedTeams = new WBTermCollection<WBTeam>(process.TeamsTaxonomy, process.InvolvedTeamsUIControlValue);
                document.ProtectiveZone = process.ProtectiveZone;
                document.Title = process.CurrentShortTitle;

                WBLogging.Debug("Set document.Title = " + document.Title);

                document.Update();
                document.Reload();

                process.WorkBox.GenerateAndSetFilename(recordsType, document);

                document.Update();
                document.Reload();

                process.ReloadCurrentItem();
            }
            catch (Exception e)
            {
                feedback.Failed("It was not possible to save the metadata to the document before publishing it", e);
                WBLogging.Debug("It was not possible to save the metadata to the document before publishing it");

                process.CurrentItemFailed();
                return process;
            }

            WBLogging.Debug("Starting WBRecordsManager.PublishDocument()");

            if (!document.IsSPListItem) {
                feedback.Failed("You can currently only publish SPListItem backed WBDocument objects");
                WBLogging.Debug("WBRecordsManager.PublishDocument(): WBDocument wasn't a list item");

                process.CurrentItemFailed();
                return process;
            }

            WBRecord recordToReplace = null;

            if (!String.IsNullOrEmpty(process.ToReplaceRecordID))
            {
                WBLogging.Debug("WBRecordsManager.PublishDocument(): Replacing record with id: " + process.ToReplaceRecordID);
                recordToReplace = Libraries.GetRecordByID(process.ToReplaceRecordID);

                if (recordToReplace == null)
                {
                    feedback.Failed("Couldn't find the record that is meant to be replaced with Record ID = " + process.ToReplaceRecordID);
                    WBLogging.Debug("WBRecordsManager.PublishDocument(): Couldn't find the record that is meant to be replaced with Record ID = " + process.ToReplaceRecordID);

                    process.CurrentItemFailed();
                    return process;
                }

            }


            WBLogging.Debug("WBRecordsManager.PublishDocument(): About to declare new record");
            
            WBRecord newRecord = null;
            try
            {
                newRecord = Libraries.DeclareNewRecord(feedback, _callingUserLogin, document, recordToReplace, process.ReplaceAction, process.ExtraMetadata);
            }
            catch (Exception e)
            {
                feedback.AddFeedback("Something went wrong with first attempt to publish document");
                feedback.AddException(e);

                WBLogging.RecordsTypes.Unexpected("Something went wrong with first attempt to publish document", e);
            }

            if (newRecord == null)
            {
                WBLogging.RecordsTypes.Unexpected("Making a second attempt to publish document");

                try
                {
                    newRecord = Libraries.DeclareNewRecord(feedback, _callingUserLogin, document, recordToReplace, process.ReplaceAction, process.ExtraMetadata);
                }
                catch (Exception e)
                {
                    feedback.Failed("Something went wrong with the second attempt to publish document", e);
                    WBLogging.RecordsTypes.Unexpected("Something went wrong with the second attempt to publish document", e);

                    process.CurrentItemFailed();
                    return process;
                }
            }

            WBLogging.Debug("WBRecordsManager.PublishDocument(): Declared new record");
            feedback.Success();
            process.CurrentItemSucceeded();

            if (newRecord != null && newRecord.ProtectiveZone != WBRecordsType.PROTECTIVE_ZONE__PROTECTED)
            {
                String documentType = GetDocumentType(newRecord.ProtectedMasterRecord);
                bool needsEmailToIAONow = (documentType == WBColumn.DOCUMENT_TYPE__SPREADSHEET);
                bool needsEmailToWebteamNow = !String.IsNullOrEmpty(process.WebPageURL);

                if (needsEmailToIAONow || needsEmailToWebteamNow)
                {
                    //WBLogging.Debug("WBRecordsManager.PublishDocument(): process.WebPageURL has been set - so creating or updating alert email");

                    SPUser publisehdByUser = newRecord.ProtectedMasterRecord[WBColumn.PublishedBy] as SPUser;
                    String publishedByString = "Published by: <unknown>";
                    if (publisehdByUser != null)
                    {
                        publishedByString = "Published by: " + publisehdByUser.Name;
                    }

                    List<SPUser> approvedByUsers = newRecord.ProtectedMasterRecord[WBColumn.PublishingApprovedBy] as List<SPUser>;
                    String approvedByString = "Approved by: <unknown>";
                    if (approvedByUsers != null)
                    {
                        approvedByString = "Approved by: " + approvedByUsers.WBxToPrettyString();
                    }

                    if (needsEmailToWebteamNow && String.IsNullOrEmpty(process.WebteamEmailAlertMessage))
                    {
                        process.WebteamEmailAlertMessage = @"<p>Dear Webteam,</p>

<p>One or more documents have been published to the Public Records Library that should be put on a web page.</p>

<p>Web page URL: " + process.WebPageURL + @"</p>

<p>Please find details of the published documents below.</p>
 
<p>" + publishedByString + "<br/>\n" + approvedByString + "</p>\n\n<p><b>Published Documents:</b></p>\n\n";
                    }

                    if (needsEmailToIAONow && String.IsNullOrEmpty(process.IAOEmailAlertMessage))
                    {
                        process.IAOEmailAlertMessage = @"<p>Dear Information Asset Owner,</p>

<p>An Excel document has been published to the Public Records Library by a member of your team.</p>

<p>As the responsible Information Asset Owner for this document, please find details of the publication below along with a link.</p>
 
<p>" + publishedByString + "<br/>\n" + approvedByString + "</p>\n\n<p><b>Published Documents:</b></p>\n\n";
                    }

                    String functionalAreaString = "";
                    if (newRecord.FunctionalArea.Count > 0)
                    {
                        functionalAreaString = newRecord.FunctionalArea[0].FullPath;
                    }

                    if (needsEmailToWebteamNow) process.WebteamEmailAlertMessage += "<p><a href=\"" + newRecord.ProtectedMasterRecord.AbsoluteURL + "\">" + newRecord.ProtectedMasterRecord.Name + "</a><br/>\n(" + newRecord.ProtectiveZone + "): " + functionalAreaString + "/" + newRecord.RecordsType.FullPath + "</p>\n";
                    if (needsEmailToIAONow) process.IAOEmailAlertMessage += "<p><a href=\"" + newRecord.ProtectedMasterRecord.AbsoluteURL + "\">" + newRecord.ProtectedMasterRecord.Name + "</a><br/>\nLocation: (" + newRecord.ProtectiveZone + "): " + functionalAreaString + "/" + newRecord.RecordsType.FullPath + "</p>\n";

                    if (process.PublishMode != WBPublishingProcess.PUBLISH_MODE__ALL_TOGETHER || !process.HasMoreDocumentsToPublish)
                    {
                        if (needsEmailToWebteamNow)
                        {
                            WBLogging.Debug("WBRecordsManager.PublishDocument(): Webteam Email Alert Message: " + process.WebteamEmailAlertMessage);

                            StringDictionary headers = new StringDictionary();

                            headers.Add("to", WBFarm.Local.PublicWebsiteTeamEmail);
                            headers.Add("cc", WBFarm.Local.PublicDocumentEmailAlertsTo);
                            headers.Add("content-type", "text/html");
                            headers.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);
                            headers.Add("subject", "New documents published for a web page");

                            WBUtils.SendEmail(Libraries.ProtectedMasterLibrary.Web, headers, process.WebteamEmailAlertMessage);
                        }
                        process.WebteamEmailAlertMessage = null;

                        WBLogging.Debug("WBRecordsManager.PublishDocument(): Webteam Email Alert Message: " + process.WebteamEmailAlertMessage);

                        if (needsEmailToIAONow)
                        {
                            StringDictionary headers = new StringDictionary();

                            SPUser teamsIAO = Libraries.ProtectedMasterLibrary.Web.WBxEnsureUserOrNull(process.OwningTeamsIAOAtTimeOfPublishing);
                            if (teamsIAO != null)
                            {
                                headers.Add("to", teamsIAO.Email);
                                headers.Add("cc", WBFarm.Local.PublicDocumentEmailAlertsTo);
                            }
                            else
                            {
                                headers.Add("to", WBFarm.Local.PublicDocumentEmailAlertsTo);
                            }

                            headers.Add("content-type", "text/html");
                            headers.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);
                            headers.Add("subject", "New Excel documents published for which you are IAO");
                            
                            WBUtils.SendEmail(Libraries.ProtectedMasterLibrary.Web, headers, process.IAOEmailAlertMessage);
                            process.IAOEmailAlertMessage = null;
                        }
                    }
                }

            }
            else
            {
                WBLogging.Debug("WBRecordsManager.PublishDocument(): Either publishing failed - or web page URL was not set");
            }


            return process;
        }

        /*
        public String PrettyNameForFileType(String fileType)
        {
            switch (fileType)
            {
                case "pdf": return "PDF Document";
                case "doc": return "Word Document (1997-2003)";
                case "docx": return "Word Document";
                case "xls": return "Excel Document (1997-2003)";
                case "xlsx": return "Excel Document";
                case "ppt": return "PowerPoint Presentation (1997-2003)";
                case "pptx": return "PowerPoint Presentation";
                case "txt": return "Text Document";
                default:
                    return "<Unknown File Type>";
            }
        }
         */ 

        public void Dispose()
        {
            _libraries.Dispose();
        }

        #endregion


        private Dictionary<String, SPListItem> _fileTypeInfo = new Dictionary<String, SPListItem>();
        private SPList _fileTypesList = null;
        internal SPListItem GetFileTypeInfo(String fileType)
        {
            if (_fileTypeInfo.ContainsKey(fileType)) return _fileTypeInfo[fileType];

            if (_fileTypesList == null)
            {
                _fileTypesList = this.Libraries.ProtectedMasterLibrary.Web.Lists.TryGetList(FILE_TYPES_LIST_TITLE);
            }

            SPListItem fileTypeInfo = WBUtils.FindItemByColumn(Libraries.ProtectedMasterLibrary.Site, _fileTypesList, WBColumn.FileTypeExtension, fileType);

            if (fileTypeInfo != null)
            {
                _fileTypeInfo[fileType] = fileTypeInfo;
            }

            return fileTypeInfo;
        }

        internal String GetDocumentType(WBDocument document)
        {
            SPListItem fileTypeInfo = GetFileTypeInfo(document.FileType);
            return fileTypeInfo.WBxGetAsString(WBColumn.DocumentType);
        }

        internal List<String> GetFileTypesDisallowedFromBeingPublishedToPublic(IEnumerable<String> fileTypes)
        {
            List<String> disallowedFileTypes = new List<string>();

            foreach (String fileType in fileTypes)
            {
                SPListItem fileTypeInfoItem = GetFileTypeInfo(fileType);

                if (fileTypeInfoItem == null)
                {
                    disallowedFileTypes.Add(fileType);
                }
                else
                {
                    if (!fileTypeInfoItem.WBxGetAsBool(WBColumn.CanPublishToPublic))
                    {
                        disallowedFileTypes.Add(fileType);
                    }
                    else
                    {
                        WBTermCollection<WBTeam> teams = fileTypeInfoItem.WBxGetMultiTermColumn<WBTeam>(this.TeamsTaxonomy, WBColumn.OnlyTeamsCanPublishToPublic);
                        if (teams != null && teams.Count > 0)
                        {
                            WBLogging.RecordsTypes.Verbose("Found this many teams: " + teams.Count);

                            // OK so if we're here then a team restriction has been set for this file type:
                            SPUser callingUser = null;

                            SPWeb web = null;
                            SPSite site = null;

                            if (SPContext.Current != null)
                            {
                                web = SPContext.Current.Web;
                                site = SPContext.Current.Site;
                            }

                            if (web == null)
                            {
                                WBLogging.RecordsTypes.Verbose("SPContext.Current.Web was null !");
                                web = this.Libraries.ProtectedMasterLibrary.Web;
                                site = this.Libraries.ProtectedMasterLibrary.Site;
                            }

                            bool isAMemberOfAtLeastOneTeam = false;
                            if (web == null)
                            {
                                WBLogging.RecordsTypes.Verbose("this.Libraries.ProtectedMasterLibrary.Web was null !");
                            }
                            else
                            {
                                WBLogging.RecordsTypes.Verbose("We have an SPWeb with web.Url = " + web.Url);

                                if (!String.IsNullOrEmpty(this._callingUserLogin))
                                {
                                    callingUser = web.WBxEnsureUserOrNull(this._callingUserLogin);
                                }

                                if (callingUser == null)
                                {
                                    WBLogging.RecordsTypes.Verbose("Wasn't able to find calling user: " + this._callingUserLogin);
                                }
                                else
                                {
                                    WBLogging.RecordsTypes.Verbose("Found calling user: " + callingUser.Email);
                                }


                                if (callingUser != null)
                                {
                                    foreach (WBTeam team in teams)
                                    {
                                        if (team.IsUserTeamMember(callingUser, site))
                                        {
                                            WBLogging.RecordsTypes.Verbose("Calling user is a member of team : " + team.Name);
                                            isAMemberOfAtLeastOneTeam = true;
                                            break;
                                        }
                                        else
                                        {
                                            WBLogging.RecordsTypes.Verbose("Calling user is NOT a member of team : " + team.Name);
                                        }
                                    }
                                }

                            }

                            if (!isAMemberOfAtLeastOneTeam)
                            {
                                disallowedFileTypes.Add(fileType + " (you're not a member of an allowed team)");
                            }

                        }
                    }
                }

            }

            return disallowedFileTypes;
        }

        internal bool AllowBulkPublishOfFileTypes(IEnumerable<String> fileTypes)
        {
            bool bulkPublishAllowed = true;
            foreach (String fileType in fileTypes)
            {
                SPListItem fileTypeInfoItem = GetFileTypeInfo(fileType);

                if (!fileTypeInfoItem.WBxGetAsBool(WBColumn.CanBulkPublish))
                {
                    bulkPublishAllowed = false;
                }
            }

            return bulkPublishAllowed;
        }

        internal bool AllowBulkPublishToPublicOfFileTypes(IEnumerable<String> fileTypes)
        {
            bool bulkPublishToPublicAllowed = true;
            foreach (String fileType in fileTypes)
            {
                SPListItem fileTypeInfoItem = GetFileTypeInfo(fileType);

                if (!fileTypeInfoItem.WBxGetAsBool(WBColumn.CanBulkPublishToPublic))
                {
                    bulkPublishToPublicAllowed = false;
                }
            }

            return bulkPublishToPublicAllowed;
        }

        internal Dictionary<String, String> GetCheckBoxDetailsForDocumentType(String documentType)
        {
            WBQuery query = new WBQuery();
            query.AddEqualsFilter(WBColumn.DocumentType, documentType);
            query.AddEqualsFilter(WBColumn.UseCheckBox, true);
            query.OrderByAscending(WBColumn.Order);

            SPList checkBoxDetailsList = Libraries.ProtectedMasterLibrary.Web.Lists.TryGetList(CHECK_BOXES_LIST_TITLE);
            SPListItemCollection items = checkBoxDetailsList.WBxGetItems(Libraries.ProtectedMasterLibrary.Site, query);

            Dictionary<String, String> checkBoxDetails = new Dictionary<String, String>();
            foreach (SPListItem item in items)
            {
                checkBoxDetails.Add(item.WBxGetAsString(WBColumn.CheckBoxCode), item.WBxGetAsString(WBColumn.CheckBoxText));
            }

            return checkBoxDetails;
        }

        internal Dictionary<string, string> GetChecklistTextMap()
        {
            SPList checkBoxDetailsList = Libraries.ProtectedMasterLibrary.Web.Lists.TryGetList(CHECK_BOXES_LIST_TITLE);
            SPListItemCollection items = checkBoxDetailsList.Items;

            Dictionary<String, String> checklistTextMap = new Dictionary<String, String>();
            foreach (SPListItem item in items)
            {
                checklistTextMap.Add(item.WBxGetAsString(WBColumn.CheckBoxCode), item.WBxGetAsString(WBColumn.CheckBoxText));
            }

            return checklistTextMap;
        }



        // This should be a farm variable!
        internal const int _weeksBetweenReviewDateAndAutoArchival = 4;
        internal WBQuery GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(WBTeam team, int weekInFuture, bool limitToJustOneWeek)
        {
            WBQuery query = new WBQuery();

            if (team != null) query.AddEqualsFilter(WBColumn.OwningTeam, team);
            query.AddEqualsFilter(WBColumn.LiveOrArchived, WBColumn.LIVE_OR_ARCHIVED__LIVE);
            query.AddEqualsFilter(WBColumn.ProtectiveZone, WBRecordsType.PROTECTIVE_ZONE__PUBLIC);
            query.AddEqualsFilter(WBColumn.RecordSeriesStatus, WBColumn.RECORD_SERIES_STATUS__LATEST);

            query.OrderByAscending(WBColumn.ReviewDate);

            if (limitToJustOneWeek && weekInFuture > 1)
            {
                query.AddFilter(WBColumn.ReviewDate, WBQueryClause.Comparators.GreaterThanEquals, DateTime.Now.AddDays((-_weeksBetweenReviewDateAndAutoArchival + weekInFuture-1)*7));
            }

            query.AddFilter(WBColumn.ReviewDate, WBQueryClause.Comparators.LessThan, DateTime.Now.AddDays((-_weeksBetweenReviewDateAndAutoArchival + weekInFuture)*7));
            query.RecursiveAll = true;

            return query;
        }

        internal WBQuery GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(WBTeam team, int weekInFuture)
        {
            return GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(team, weekInFuture, true);
        }

        internal WBQuery GetQueryForTeamsPublicRecordsToArchive(WBTeam team)
        {
            return GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(team, 0, false);
        }

        internal WBQuery GetQueryForAllPublicRecordsToArchiveInFutureWeek(int weeksBeforeBeingArchived)
        {
            return GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(null, weeksBeforeBeingArchived);
        }

        internal WBQuery GetQueryForTeamsPublicRecordsToReview(WBTeam team)
        {
            // We can use the 'in next weeks' query generator - with the week set as the maximum gap between review and archive!
            return GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(team, _weeksBetweenReviewDateAndAutoArchival, false);
        }

        internal WBQuery GetQueryForAllPublicRecordsToReview()
        {
            return GetQueryForTeamsPublicRecordsToReview(null);
        }

        // This should be a farm variable!
        internal const int _daysToIncludeNewlyPublishedPublicDocsInEmailAlerts = 14;
        internal WBQuery GetQueryForNewlyPublishedPublicDocsThatNeedEmailAlert()
        {
            WBQuery query = new WBQuery();

            query.AddEqualsFilter(WBColumn.LiveOrArchived, WBColumn.LIVE_OR_ARCHIVED__LIVE);
            query.AddEqualsFilter(WBColumn.ProtectiveZone, WBRecordsType.PROTECTIVE_ZONE__PUBLIC);
            query.AddEqualsFilter(WBColumn.RecordSeriesStatus, WBColumn.RECORD_SERIES_STATUS__LATEST);
            query.AddIsNullFilter(WBColumn.SentNewlyPublishedAlert);
            query.AddFilter(WBColumn.DatePublished, WBQueryClause.Comparators.GreaterThan, DateTime.Now.AddDays(-_daysToIncludeNewlyPublishedPublicDocsInEmailAlerts));
            query.RecursiveAll = true;

            return query;
        }

        internal void PopulateWithFunctionalAreas(WBLocationTreeState treeState, TreeNodeCollection treeNodeCollection, String viewMode, WBTermCollection<WBTerm> teamFunctionalAreas)
        {
            bool expandNodes = true;
            if (teamFunctionalAreas.Count > 2) {
                expandNodes = false;
            }

            List<WBTerm> sortedTerms = new List<WBTerm>();
            foreach (WBTerm term in teamFunctionalAreas) sortedTerms.Add(term);
            sortedTerms = sortedTerms.OrderBy(o => o.Name).ToList();

            foreach (WBTerm functionalArea in sortedTerms)
            {
                SPFolder folder = null;

                if (viewMode != VIEW_MODE__NEW)
                {
                    folder = this.Libraries.GetMasterFolderByPath(functionalArea.Name);

                    if (folder == null)
                    {
                        WBLogging.Debug("Couldn't find folder for functional area: " + functionalArea.Name);
                        continue;
                    }
                }
                else
                {
                    WBLogging.Debug("View mode = " + viewMode);
                }


                WBFunctionalAreaTreeNode functionalAreaTreeNode = new WBFunctionalAreaTreeNode(functionalArea, folder);
                TreeNode node = functionalAreaTreeNode.AsTreeNode();

                node.Expanded = expandNodes;
                node.PopulateOnDemand = false;
                node.SelectAction = TreeNodeSelectAction.Expand;

                treeNodeCollection.Add(node);

                WBTaxonomy recordsTypes = this.RecordsTypesTaxonomy;
                TermCollection terms = recordsTypes.TermSet.Terms;

                PopulateWithRecordsTypes(treeState, node.ChildNodes, viewMode, folder, functionalArea, recordsTypes, terms);
            }          
        }

        internal void PopulateWithRecordsTypes(WBLocationTreeState treeState, TreeNodeCollection treeNodeCollection, String viewMode, SPFolder parentFolder, WBTerm functionalArea, WBTaxonomy recordsTypesTaxonomy, TermCollection recordsTypeTerms)
        {
            List<Term> sortedTerms = new List<Term>();
            foreach (Term term in recordsTypeTerms) sortedTerms.Add(term);
            sortedTerms = sortedTerms.OrderBy(o => o.Name).ToList();

            foreach (Term term in sortedTerms)
            {
                WBRecordsType recordsType = new WBRecordsType(recordsTypesTaxonomy, term);

                bool protectiveZoneOK = true;
                if (!String.IsNullOrEmpty(treeState.MinimumProtectiveZone))
                {
                    protectiveZoneOK = (recordsType.IsZoneAtLeastMinimum(treeState.MinimumProtectiveZone));
                }

                if (recordsType.BranchCanHaveDocuments() && recordsType.IsRelevantToFunctionalArea(functionalArea) && protectiveZoneOK)
                {
                    SPFolder folder = null;
                    if (viewMode != VIEW_MODE__NEW && parentFolder != null)
                    {
                        folder = parentFolder.WBxGetSubFolder(recordsType.Name);
                        if (folder == null) WBLogging.Debug("Did not find folder for: " + recordsType.Name);
                    }

                    if (viewMode == VIEW_MODE__NEW || folder != null)
                    {
                        WBRecordsTypeTreeNode recordsTypeTreeNode = new WBRecordsTypeTreeNode(functionalArea, recordsType, folder);
                        TreeNode node = recordsTypeTreeNode.AsTreeNode();

                        if (recordsType.Term.TermsCount > 0 || viewMode != VIEW_MODE__NEW)
                        {
                            if (viewMode == VIEW_MODE__BROWSE_FOLDERS && recordsType.Term.TermsCount == 0)
                            {
                                node.SelectAction = TreeNodeSelectAction.Select;
                            }
                            else
                            {
                                node.SelectAction = TreeNodeSelectAction.Expand;
                            }
                            node.Expanded = false;
                            node.PopulateOnDemand = true;
                        } else {
                            node.SelectAction = TreeNodeSelectAction.Select;
                            node.Expanded = true;
                            node.PopulateOnDemand = false;
                        }

                        treeNodeCollection.Add(node);

                    }
                }
            }            
        }

        internal void PopulateWithSubFolders(WBLocationTreeState treeState, TreeNodeCollection treeNodeCollection, String viewMode, SPFolder parentFolder)
        {
            SPFolderCollection subFolders = parentFolder.SubFolders;

            if (subFolders.Count > 0)
            {
                List<SPFolder> folders = new List<SPFolder>();
                foreach (SPFolder folder in subFolders) folders.Add(folder);
                folders = folders.OrderBy(o => o.Name).ToList();

                foreach (SPFolder folder in folders)
                {
                    WBFolderTreeNode folderNode = new WBFolderTreeNode(folder);
                    TreeNode node = folderNode.AsTreeNode();

                    if (folder.SubFolders.Count > 0 || viewMode == VIEW_MODE__REPLACE)
                    {
                        node.Expanded = false;
                        node.PopulateOnDemand = true;
                        if (viewMode == VIEW_MODE__BROWSE_FOLDERS)
                        {
                            node.SelectAction = TreeNodeSelectAction.Select;
                        }
                        else
                        {
                            node.SelectAction = TreeNodeSelectAction.Expand;
                        }
                    }
                    else
                    {
                        node.Expanded = true;
                        node.PopulateOnDemand = false;
                        node.SelectAction = TreeNodeSelectAction.Select;
                    }

                    treeNodeCollection.Add(node);
                }
            }
            else
            {
                if (viewMode == VIEW_MODE__REPLACE)
                {
                    PopulateWithDocuments(treeState, treeNodeCollection, viewMode, parentFolder);
                }
            }
        }

        internal void PopulateWithDocuments(WBLocationTreeState treeState, TreeNodeCollection treeNodeCollection, String viewMode, SPFolder folder)
        {
            WBLogging.Debug("In PopulateWithDocuments()");

            SPListItemCollection items = GetItemsRecursive(folder);

            WBLogging.Debug("In PopulateWithDocuments() items.Count = " + items.Count);

            List<SPListItem> sortedItems = new List<SPListItem>();
            foreach (SPListItem item in items) sortedItems.Add(item);
            sortedItems = sortedItems.OrderBy(o => o.Name).ToList();

            foreach (SPListItem item in items)
            {
                if (ItemCanBePicked(treeState, item))
                {
                    TreeNode node = new TreeNode();

                    node.Text = item.Name;
                    node.Value = item.Name;
                    node.Expanded = true;
                    node.PopulateOnDemand = false;
                    node.SelectAction = TreeNodeSelectAction.Select;


                    node.ImageUrl = SPUtility.ConcatUrls("/_layouts/images/",
                        SPUtility.MapToIcon(treeState.Web,
                        SPUtility.ConcatUrls(treeState.Web.Url, node.Text), "", IconSize.Size16));

                    // No need to add this to the tree state as we'll never come looking for it:
                    treeNodeCollection.Add(node);
                }
            }
        }

        private bool ItemCanBePicked(WBLocationTreeState treeState, SPListItem item)
        {
            if (item == null) return false;

            if (String.IsNullOrEmpty(item.WBxGetAsString(WBColumn.RecordID))) return false;
            if (item.WBxGetAsString(WBColumn.LiveOrArchived) == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED) return false;

            String recordSeriesStatus = item.WBxGetAsString(WBColumn.RecordSeriesStatus);
            if (recordSeriesStatus != "Latest" && !String.IsNullOrEmpty(recordSeriesStatus)) return false;

            String itemProtectiveZone = item.WBxGetAsString(WBColumn.ProtectiveZone);
            if (itemProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC) return true;

            if (treeState.MinimumProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET && itemProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET) return true;

            if (treeState.MinimumProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PROTECTED) return true;

            return false;
        }

        public static SPListItemCollection GetItemsRecursive(SPFolder folder)
        {
            SPList list = folder.ParentWeb.Lists[folder.ParentListId];
            SPQuery query = new SPQuery();
            query.Folder = folder;                        //set folder for seaching;
            query.ViewAttributes = "Scope=\"Recursive\""; //set recursive mode for items seaching;
            return list.GetItems(query);
        }

        internal WBFolderTreeNode GetFolderTreeNode(String path)
        {
            List<String> steps = WBUtils.GetPathStepsFromNormalisedPath(path);
            String normalisedPath = String.Join("/", steps.ToArray());

            SPFolder folder =  this.Libraries.GetMasterFolderByPath(normalisedPath);

            WBLogging.Debug("In GetFolderTreeNode(): steps.Count = " + steps.Count);
            if (folder == null)
            {
                WBLogging.Debug("In GetFolderTreeNode(): folder is NULL");
            }
            else
            {
                WBLogging.Debug("In GetFolderTreeNode(): folder = " + folder.Name);
            }

            if (steps.Count == 0) return null;

            WBTerm functionalArea = this.FunctionalAreasTaxonomy.GetSelectedWBTermByPath(steps[0]);

            if (steps.Count == 1)
            {
                return new WBFunctionalAreaTreeNode(functionalArea, folder);
            }

            // Now let's remove the functional area bit from the list of steps so that we just have a potential records type path:
            steps.RemoveAt(0);

            Term deepestRecordsTypeTerm = this.RecordsTypesTaxonomy.GetDeepestTermBySteps(steps);
            if (deepestRecordsTypeTerm == null) return null;

            String deepestRecordsTypeLocationPath = functionalArea.Name + "/" + deepestRecordsTypeTerm.WBxFullPath();
            if (deepestRecordsTypeLocationPath == normalisedPath)
            {
                // OK so the path was to a records type - not a folder below the records type:
                return new WBRecordsTypeTreeNode(functionalArea, new WBRecordsType(this.RecordsTypesTaxonomy, deepestRecordsTypeTerm), folder);
            }

            // Otherwise - if we've got here then the path is to an actual folder within the master records library:
            return new WBFolderTreeNode(folder);
        }

        internal void PopulateTreeNode(WBLocationTreeState treeState, TreeNode node, String viewMode)
        {
            WBLogging.Debug("Looking for WBFolderTreeNode with path: " + node.ValuePath);

            WBFolderTreeNode folderTreeNode = this.GetFolderTreeNode(node.ValuePath);

            if (folderTreeNode == null)
            {
                WBLogging.Debug("Did not find WBFolderTreeNode at: " + node.ValuePath);
                return;
            }

            if (folderTreeNode is WBRecordsTypeTreeNode)
            {
                WBLogging.Debug("Expanding a records type node: " + node.Text);

                WBRecordsTypeTreeNode recordsTypeNode = (WBRecordsTypeTreeNode)folderTreeNode;
                WBRecordsType recordsType = recordsTypeNode.RecordsType;
                TermCollection childTerms = recordsType.Term.Terms;
                if (childTerms.Count > 0)
                {
                    PopulateWithRecordsTypes(treeState, node.ChildNodes, viewMode, recordsTypeNode.Folder, recordsTypeNode.FunctionalArea, recordsType.Taxonomy, childTerms);
                }
                else
                {
                    if (viewMode != VIEW_MODE__NEW)
                    {
                        PopulateWithSubFolders(treeState, node.ChildNodes, viewMode, recordsTypeNode.Folder);
                    }
                }
            }
            else if (folderTreeNode is WBFolderTreeNode)
            {
                WBLogging.Debug("Expanding a folder node: " + node.Text);

                // You shouldn't be here if the view mode was NEW !
                PopulateWithSubFolders(treeState, node.ChildNodes, viewMode, folderTreeNode.Folder);
            }
            else
            {
                WBLogging.Debug("NOT expanding an unrecognised node: " + node.Text + " of type: " + node.GetType());
            }
        }

        internal String GetSelectedPath(HttpRequest request)
        {
            String eventArgument = request.Params["__EVENTARGUMENT"];

            if (String.IsNullOrEmpty(eventArgument))
            {
                eventArgument = request.Params["HiddenSelectedPath"];
            }

            if (!String.IsNullOrEmpty(eventArgument) && eventArgument[0] == 's')
            {
                String selectedPath = eventArgument.Substring(1);
                selectedPath = selectedPath.Replace("\\", "/");
                return selectedPath;
            }
            else
            {
                return null;
            }
        }
    }
}
