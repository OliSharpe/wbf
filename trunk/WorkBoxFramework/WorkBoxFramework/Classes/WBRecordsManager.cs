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


        public WBTaskFeedback PublishDocument(String documentURL)
        {
            WBTaskFeedback feedback = null;

            using (WorkBox workBox = new WorkBox(documentURL))
            {
                feedback = PublishDocument(workBox, documentURL);
            }

            return feedback;
        }

        public WBTaskFeedback PublishDocument(String documentURL, String replacingRecordID, String replacingAction)
        {
            WBTaskFeedback feedback = null;

            using (WorkBox workBox = new WorkBox(documentURL))
            {
                feedback = PublishDocument(workBox, documentURL, replacingRecordID, replacingAction);
            }

            return feedback;
        }

        public WBTaskFeedback PublishDocument(WorkBox workBox, String documentURL)
        {
            return PublishDocument(workBox, documentURL, null, null);
        }


        public WBTaskFeedback PublishDocument(WorkBox workBox, String documentURL, String replacingRecordID, String replacingAction)
        {
            SPListItem item = workBox.Web.GetListItem(documentURL);
            if (item == null)
            {
                WBTaskFeedback feedback = new WBTaskFeedback(WBTaskFeedback.TASK_TYPE__PUBLISH, documentURL);
                feedback.Failed("Couldn't find document to publish with URL: " + documentURL);
                return feedback;
            }

            return PublishDocument(workBox, new WBDocument(item), replacingRecordID, replacingAction);
        }
        
        public WBTaskFeedback PublishDocument(WorkBox workBox, WBDocument document)
        {
            return PublishDocument(workBox, document, null, null);
        }

        public WBTaskFeedback PublishDocument(WorkBox workBox, WBDocument document, String replacingRecordID, String replacingAction)
        {
            WBTaskFeedback feedback = new WBTaskFeedback(WBTaskFeedback.TASK_TYPE__PUBLISH, document.AbsoluteURL);

            WBLogging.Debug("Starting WBRecordsManager.PublishDocument()");

            if (!document.IsSPListItem) {
                feedback.Failed("You can currently only publish SPListItem backed WBDocument objects");
                WBLogging.Debug("WBRecordsManager.PublishDocument(): WBDocument wasn't a list item");
                return feedback;
            }

            WBRecord recordToReplace = null;

            if (!String.IsNullOrEmpty(replacingRecordID))
            {
                WBLogging.Debug("WBRecordsManager.PublishDocument(): Replacing record with id: " + replacingRecordID);
                recordToReplace = Libraries.GetRecordByID(replacingRecordID);

                if (recordToReplace == null)
                {
                    feedback.Failed("Couldn't find the record that is meant to be replaced with Record ID = " + replacingRecordID);
                    WBLogging.Debug("WBRecordsManager.PublishDocument(): Couldn't find the record that is meant to be replaced with Record ID = " + replacingRecordID);
                    return feedback;
                }

            }


            WBLogging.Debug("WBRecordsManager.PublishDocument(): About to declare new record");

            WBRecord newRecord = Libraries.DeclareNewRecord(feedback, document, recordToReplace, replacingAction);

            WBLogging.Debug("WBRecordsManager.PublishDocument(): Declared new record");

            return feedback;
        }

        #endregion 


        public void Dispose()
        {
            _libraries.Dispose();
        }

    }
}
