using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class MailToLinkReplacement : WBDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WBFarm farm = WBFarm.Local;

                if (farm.UseMailToLinks)
                {
                    ExplanationMessage.Text = "You are seeing this dialog because the number of email addresses is too long. Please copy and paste these email addresses into your email client if you wish to email all of these users.";
                }
                else
                {
                    ExplanationMessage.Text = "You are seeing this dialog because the use of the mailto link by the work box framework is disabled for this farm.";
                }

                EmailTo.Text = Request.QueryString["to"];
                EmailSubject.Text = Request.QueryString["subject"];
                EmailBody.Text = Request.QueryString["body"];
            }
        }

        protected void CloseButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithOK();
        }

    }
}
