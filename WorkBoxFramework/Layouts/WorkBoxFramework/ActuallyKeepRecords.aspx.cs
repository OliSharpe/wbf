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
    public partial class ActuallyKeepRecords : WorkBoxDialogPageBase
    {
        private int indexOfNextRecordToKeep = 0;
        String[] recordIDs = null;
        Dictionary<String, String> mappedFilenames = new Dictionary<string, string>();
        List<String> allFilenamesToKeep = new List<String>();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                using (WBRecordsManager manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName))
                {
                    AllRecordIDsToKeep.Value = Request.QueryString["AllRecordIDsToKeep"];
                    ReasonToKeepRecords.Value = Request.QueryString["ReasonToKeepRecords"];

                    recordIDs = AllRecordIDsToKeep.Value.Split(',');
                    foreach (String recordID in recordIDs)
                    {

                        WBDocument record = manager.Libraries.ProtectedMasterLibrary.GetDocumentByID(recordID);
                        allFilenamesToKeep.Add(record.Name);
                    }

                    AllRecordFilenamesToKeep.Value = String.Join(",", allFilenamesToKeep.ToArray());

                    WBLogging.Debug("AllRecordIDsToKeep.Value = " + AllRecordIDsToKeep.Value);
                    WBLogging.Debug("AllRecordFilenamesToKeep.Value = " + AllRecordFilenamesToKeep.Value);

                    indexOfNextRecordToKeep = 0;
                    NextRecordToKeep.Text = "" + indexOfNextRecordToKeep;
                }
            }
            else
            {
                indexOfNextRecordToKeep = NextRecordToKeep.Text.WBxToInt();
                recordIDs = AllRecordIDsToKeep.Value.Split(',');
            }

            WBLogging.Debug("recordIDs.Length = " + recordIDs.Length);

            if (indexOfNextRecordToKeep < recordIDs.Length)
            {
                String[] filenames = AllRecordFilenamesToKeep.Value.Split(',');

                WBLogging.Debug("filenames.Length = " + filenames.Length);

                for (int i = 0; i < recordIDs.Length && i < filenames.Length; i++)
                {
                    mappedFilenames.Add(i.ToString(), filenames[i]);
                }

                RecordKeepingProgress.WBxCreateTasksTable(mappedFilenames.Keys, mappedFilenames);

                Image image = (Image)RecordKeepingProgress.WBxFindNestedControlByID(RecordKeepingProgress.WBxMakeControlID(indexOfNextRecordToKeep.ToString(), "image"));
                if (image != null)
                {
                    image.ImageUrl = "/_layouts/images/WorkBoxFramework/processing-task-32.gif";
                }

                WBLogging.Debug("Finished");
            }
        }


        protected void KeepNextDocument(object sender, EventArgs e)
        {
            WBLogging.Debug("Attempting to keep the next document with index: " + indexOfNextRecordToKeep + " and filename: " + mappedFilenames[indexOfNextRecordToKeep.ToString()]);

            String callingUserLogin = SPContext.Current.Web.CurrentUser.LoginName;
            WBTaskFeedback feedback = new WBTaskFeedback(indexOfNextRecordToKeep.ToString());
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager(callingUserLogin))
                    {
                        WBRecord record = elevatedManager.Libraries.GetRecordByID(recordIDs[indexOfNextRecordToKeep]);
                        record[WBColumn.ReviewDate] = DateTime.Now.AddYears(2);
                        record.Update(callingUserLogin, ReasonToKeepRecords.Value);

                        feedback.Success("Kept successfully");
                    }
                });
            }
            catch (Exception exception)
            {
                feedback.Failed("Keeping failed", exception);
            }

            WBLogging.Debug("Kept the document");

            RecordKeepingProgress.WBxUpdateTask(feedback);

            indexOfNextRecordToKeep++;

            NextRecordToKeep.Text = "" + indexOfNextRecordToKeep;

            if (indexOfNextRecordToKeep < recordIDs.Length)
            {
                Image image = (Image)RecordKeepingProgress.WBxFindNestedControlByID(RecordKeepingProgress.WBxMakeControlID(indexOfNextRecordToKeep.ToString(), "image"));
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/processing-task-32.gif";

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TriggerNextStepFunction", "WorkBoxFramework_triggerKeepNextDocument();", true);
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
