using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Administration;
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

        #endregion


        #region Constructors

        private WBFarm _farm;
        private WBRecordsLibraries _libraries;

        public WBRecordsManager()
        {
            WBLogging.Debug("In WBRecordsManager() constructor");

            _farm = WBFarm.Local;
            _libraries = new WBRecordsLibraries(this);

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

            process.AddExtraMetadataIfMissing(WBColumn.DatePublished, DateTime.Now);
            if (SPContext.Current != null)
            {
                process.AddExtraMetadataIfMissing(WBColumn.PublishedBy, SPContext.Current.Web.CurrentUser);
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

                process.WorkBox.GenerateFilename(recordsType, document.Item);

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
                newRecord = Libraries.DeclareNewRecord(feedback, document, recordToReplace, process.ReplaceAction, process.ExtraMetadata);
            }
            catch (Exception e)
            {
                feedback.Failed("Something went wrong with the publishing process", e);
                WBLogging.Debug("Something went wrong with the publishing process");

                process.CurrentItemFailed();
                return process;
            }


            WBLogging.Debug("WBRecordsManager.PublishDocument(): Declared new record");
            feedback.Success();
            process.CurrentItemSucceeded();

            if (newRecord != null)
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


                if (String.IsNullOrEmpty(process.WebteamEmailAlertMessage))
                {
                    process.WebteamEmailAlertMessage = "One or more documents have been published that should be put on a web page.\n\nThe web page url is: " + process.WebPageURL + "\n\n" + publishedByString + "\n" + approvedByString + "\n\nThe documents are: \n\n";
                }

                if (String.IsNullOrEmpty(process.IAOEmailAlertMessage))
                {
                    process.IAOEmailAlertMessage = "One or more documents have been published by a team for which you are the assigned IAO.\n\n" + publishedByString + "\n" + approvedByString + "\n\nThe documents are: \n\n";
                }

                String functionalAreaString = "";
                if (newRecord.FunctionalArea.Count > 0)
                {
                    functionalAreaString = newRecord.FunctionalArea[0].FullPath;
                }

                process.WebteamEmailAlertMessage += newRecord.ProtectedMasterRecord.Name + "\n(" + newRecord.ProtectiveZone + "): " + functionalAreaString + "/" + newRecord.RecordsType.FullPath + "\n";
                process.IAOEmailAlertMessage += newRecord.ProtectedMasterRecord.Name + "\n(" + newRecord.ProtectiveZone + "): " + functionalAreaString + "/" + newRecord.RecordsType.FullPath + "\n";

                if (process.PublishMode != WBPublishingProcess.PUBLISH_MODE__ALL_TOGETHER || !process.HasMoreDocumentsToPublish)
                {
                    if (!String.IsNullOrEmpty(process.WebPageURL))
                    {
                        WBLogging.Debug("WBRecordsManager.PublishDocument(): Webteam Email Alert Message: " + process.WebteamEmailAlertMessage);

                        WBUtils.SendEmail(Libraries.ProtectedMasterLibrary.Web, WBFarm.Local.PublicDocumentEmailAlertsTo, "New documents published for a web page", process.WebteamEmailAlertMessage, false);
                    }
                    process.WebteamEmailAlertMessage = null;

                    WBLogging.Debug("WBRecordsManager.PublishDocument(): Webteam Email Alert Message: " + process.WebteamEmailAlertMessage);

                    SPUser teamsIAO = Libraries.ProtectedMasterLibrary.Web.WBxEnsureUserOrNull(process.OwningTeamsIAOAtTimeOfPublishing);
                    if (teamsIAO != null)
                    {
                        WBUtils.SendEmail(Libraries.ProtectedMasterLibrary.Web, teamsIAO.Email, "New documents published for which you are IAO", process.IAOEmailAlertMessage, false);
                    }
                    process.IAOEmailAlertMessage = null;
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
            _fileTypeInfo[fileType] = fileTypeInfo;

            return fileTypeInfo;
        }

        internal bool AllowPublishToPublicOfFileTypes(IEnumerable<String> fileTypes)
        {
            bool publicAllowed = true;
            foreach (String fileType in fileTypes)
            {
                SPListItem fileTypeInfoItem = GetFileTypeInfo(fileType);

                if (!fileTypeInfoItem.WBxGetAsBool(WBColumn.CanPublishToPublic))
                {
                    publicAllowed = false;
                }
            }

            return publicAllowed;
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
    }
}
