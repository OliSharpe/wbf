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

            WBLogging.Debug("In WBRecordsManager() about to setup taxonomies");
            RecordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(_libraries.ProtectedMasterLibrary.Site);
            TeamsTaxonomy = WBTaxonomy.GetTeams(RecordsTypesTaxonomy);
            SeriesTagsTaxonomy = WBTaxonomy.GetSeriesTags(RecordsTypesTaxonomy);
            SubjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(RecordsTypesTaxonomy);
            FunctionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(RecordsTypesTaxonomy);

            WBLogging.Debug("Finished WBRecordsManager() constructor");
        }

        #endregion

        #region Properties

        public WBRecordsLibraries Libraries
        {
            get { return _libraries; }
        }

        public WBTaxonomy RecordsTypesTaxonomy { get; private set; }
        public WBTaxonomy TeamsTaxonomy { get; private set; }
        public WBTaxonomy SeriesTagsTaxonomy { get; private set; }
        public WBTaxonomy SubjectTagsTaxonomy { get; private set; }
        public WBTaxonomy FunctionalAreasTaxonomy { get; private set; }
            
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

                document.Update();
                document.Reload();

                process.WorkBox.GenerateFilename(recordsType, currentItem);

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

            try
            {
                WBRecord newRecord = Libraries.DeclareNewRecord(feedback, document, recordToReplace, process.ReplaceAction, new WBItem());
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
            return process;
        }

        public bool AllowBulkPublishingOfFileType(String fileType)
        {
            if (fileType == "pdf") return true;
            return false;
        }

        public bool AllowPublishingOfFileType(String fileType)
        {
            switch (fileType)
            {
                case "pdf":
                case "doc":
                case "docx":
                case "xls":
                case "xlsx":
                case "ppt":
                case "pptx":
                case "txt":
                    return true;
                default:
                    return false;
            }
        }

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

        public void Dispose()
        {
            _libraries.Dispose();
        }

        #endregion

    }
}
