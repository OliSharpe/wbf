using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ChangeTeamManager : WBDialogPageBase
    {
        WBTeam team = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            team = WBTeam.GetFromTeamSite(SPContext.Current);

            if (team == null)
            {
                ErrorText.Text = "You should only be using this form when on a team site.";
                ChangeButton.Enabled = false;
                return;
            }

            if (!team.IsCurrentUserTeamManagerOrSystemAdmin())
            {
                ErrorText.Text = "Only team the team manager can change the team manager.";
                ChangeButton.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                TeamName.Text = team.Name;
            }

        }

        protected void changeButton_OnClick(object sender, EventArgs e)
        {
            team.SetManager(SPContext.Current.Site, NewTeamManager.WBxGetSingleResolvedUser(SPContext.Current.Web));

            team.Update();

            CloseDialogAndRefresh();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithCancel("Adding was cancelled");
        }

    }
}
