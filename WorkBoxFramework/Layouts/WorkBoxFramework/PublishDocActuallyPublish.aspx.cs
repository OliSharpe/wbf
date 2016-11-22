using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Newtonsoft.Json;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocActuallyPublish : WorkBoxDialogPageBase
    {
        private WBPublishingProcess process = null;
        WBRecordsManager manager = null;

        protected void Page_Init(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            manager = new WBRecordsManager();

            if (!IsPostBack)
            {
                process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(Request.QueryString["PublishingProcessJSON"]);
                process.WorkBox = WorkBox;

                WBLogging.Debug("Created the WBProcessObject");

                PublishingProcessJSON.Text = WBUtils.SerializeToCompressedJSONForURI(process);

                WBLogging.Debug("Serialized the WBProcessObject to hidden field");
            }
            else
            {
                WBLogging.Debug("About to deserialise: " + PublishingProcessJSON.Text);

                process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(PublishingProcessJSON.Text);
                process.WorkBox = WorkBox;
            }

            if (process.HasMoreDocumentsToPublish)
            {
                if (process.PublishMode == WBPublishingProcess.PUBLISH_MODE__ALL_TOGETHER)
                {
                    DocumentPublishingProgress.WBxCreateTasksTable(process.ItemIDs, process.MappedFilenames);
                }
                else
                {
                    List<String> oneItem = new List<String>();
                    oneItem.Add(process.CurrentItemID);

                    Dictionary<String, String> oneMapping = new Dictionary<String, String>();
                    oneMapping.Add(process.CurrentItemID, process.CurrentItem.Name);

                    DocumentPublishingProgress.WBxCreateTasksTable(oneItem, oneMapping);
                }

                Image image = (Image)DocumentPublishingProgress.WBxFindNestedControlByID(DocumentPublishingProgress.WBxMakeControlID(process.CurrentItemID, "image"));
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/processing-task-32.gif";
            }

        }


        protected void PublishNextDocument(object sender, EventArgs e)
        {
            WBLogging.Debug("Attempting to publishg the next document " + process.CurrentItemID);
            
            process = manager.PublishDocument(process);

            WBLogging.Debug("Published the document");

            DocumentPublishingProgress.WBxUpdateTask(process.LastTaskFeedback);

            PublishingProcessJSON.Text = WBUtils.SerializeToCompressedJSONForURI(process);

            WBLogging.Debug("Serialized to: " + PublishingProcessJSON.Text);

            if (process.HasMoreDocumentsToPublish && process.PublishMode == WBPublishingProcess.PUBLISH_MODE__ALL_TOGETHER)
            {

                Image image = (Image)DocumentPublishingProgress.WBxFindNestedControlByID(DocumentPublishingProgress.WBxMakeControlID(process.CurrentItemID, "image"));
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/processing-task-32.gif";

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TriggerNextStepFunction", "WorkBoxFramework_triggerPublishNextDocument();", true);
            }
            else
            {
                if (process.HasMoreDocumentsToPublish)
                {
                    WBLogging.Debug("Trying to set button text to Publish next doc");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ChangeDoneButtonTextFunction", "WorkBoxFramework_finishedProcessing('Publish Next Document');", true);
                }
                else
                {
                    WBLogging.Debug("Trying to set button text to done");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ChangeDoneButtonTextFunction", "WorkBoxFramework_finishedProcessing('Done');", true);
                }                
            }
        }


        protected void DoneButton_OnClick(object sender, EventArgs e)
        {
            if (process.HasMoreDocumentsToPublish)
            {
                if (process.PublishMode == WBPublishingProcess.PUBLISH_MODE__ALL_TOGETHER)
                {
                    // OK so if we're here then someone clicked on it when it said 'Stop' ... so let's stop:
                    CloseDialogAndRefresh();
                }
                else
                {
                    // If we're here then someone wants to go on to publish the next document
                    string redirectUrl = "WorkBoxFramework/PublishDocRequiredMetadata.aspx?PublishingProcessJSON=" + WBUtils.SerializeToCompressedJSONForURI(process);
                    SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
                }
            }
            else
            {
                // If we're here then the publishing is done - so we can close the dialog:
                CloseDialogAndRefresh();
            }
        }

    }
}
