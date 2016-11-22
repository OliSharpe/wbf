using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ActuallyArchiveRecords : WorkBoxDialogPageBase
    {
        private int indexOfNextRecordToArchive = 0;
        String[] recordIDs = null;
        Dictionary<String, String> mappedFilenames = new Dictionary<string, string>();
        List<String> allFilenamesToArchive = new List<String>();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                using (WBRecordsManager manager = new WBRecordsManager())
                {
                    AllRecordIDsToArchive.Value = Request.QueryString["AllRecordIDsToArchive"];

                    recordIDs = AllRecordIDsToArchive.Value.Split(',');
                    foreach (String recordID in recordIDs)
                    {

                        WBDocument record = manager.Libraries.ProtectedMasterLibrary.GetDocumentByID(recordID);
                        allFilenamesToArchive.Add(record.Name);
                    }

                    AllRecordFilenamesToArchive.Value = String.Join(",", allFilenamesToArchive.ToArray());

                    WBLogging.Debug("AllRecordIDsToArchive.Value = " + AllRecordIDsToArchive.Value);
                    WBLogging.Debug("AllRecordFilenamesToArchive.Value = " + AllRecordFilenamesToArchive.Value);

                    indexOfNextRecordToArchive = 0;
                    NextRecordToArchive.Text = "" + indexOfNextRecordToArchive;
                }
            }
            else
            {
                indexOfNextRecordToArchive = NextRecordToArchive.Text.WBxToInt();
                recordIDs = AllRecordIDsToArchive.Value.Split(',');
            }

            WBLogging.Debug("recordIDs.Length = " + recordIDs.Length);

            if (indexOfNextRecordToArchive < recordIDs.Length)
            {
                String[] filenames = AllRecordFilenamesToArchive.Value.Split(',');

                WBLogging.Debug("filenames.Length = " + filenames.Length);

                for (int i = 0; i < recordIDs.Length && i < filenames.Length; i++)
                {
                    mappedFilenames.Add(i.ToString(), filenames[i]);
                }

                RecordArchivingProgress.WBxCreateTasksTable(mappedFilenames.Keys, mappedFilenames);

                Image image = (Image)RecordArchivingProgress.WBxFindNestedControlByID(RecordArchivingProgress.WBxMakeControlID(indexOfNextRecordToArchive.ToString(), "image"));
                if (image != null)
                {
                    image.ImageUrl = "/_layouts/images/WorkBoxFramework/processing-task-32.gif";
                }

                WBLogging.Debug("Finished");
            }
        }


        protected void ArchiveNextDocument(object sender, EventArgs e)
        {
            WBLogging.Debug("Attempting to archive the next document with index: " + indexOfNextRecordToArchive + " and filename: " + mappedFilenames[indexOfNextRecordToArchive.ToString()]);

            WBTaskFeedback feedback = new WBTaskFeedback(indexOfNextRecordToArchive.ToString());
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager())
                    {
                        WBRecord record = elevatedManager.Libraries.GetRecordByID(recordIDs[indexOfNextRecordToArchive]);
                        record.LiveOrArchived = WBColumn.LIVE_OR_ARCHIVED__ARCHIVED;
                        record.Update();

                        feedback.Success("Archived successfully");
                    }
                });
            }
            catch (Exception exception)
            {
                feedback.Failed("Archiving failed", exception);
            }

            WBLogging.Debug("Archived the document");

            RecordArchivingProgress.WBxUpdateTask(feedback);

            indexOfNextRecordToArchive++;

            NextRecordToArchive.Text = "" + indexOfNextRecordToArchive;

            if (indexOfNextRecordToArchive < recordIDs.Length)
            {
                Image image = (Image)RecordArchivingProgress.WBxFindNestedControlByID(RecordArchivingProgress.WBxMakeControlID(indexOfNextRecordToArchive.ToString(), "image"));
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/processing-task-32.gif";

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TriggerNextStepFunction", "WorkBoxFramework_triggerArchiveNextDocument();", true);
            }
            else
            {
                WBLogging.Debug("Trying to set button text to done");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ChangeDoneButtonTextFunction", "WorkBoxFramework_finishedProcessing('Done');", true);
            }

        }


        protected void DoneButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogAndRefresh();
        }
    }
}
