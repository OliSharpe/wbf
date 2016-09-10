using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocDialogPickLocation : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FunctionalAreasUIControlValue.Text = Request.QueryString["FunctionalAreasUIControlValue"];
                RecordsTypeUIControlValue.Text = Request.QueryString["RecordsTypeUIControlValue"];
            }

        }

        protected void selectButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogOK(FunctionalAreasUIControlValue.Text + "@" + RecordsTypeUIControlValue.Text);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }

    }
}
