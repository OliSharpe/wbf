﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class AddAllManagersReports : WBDialogPageBase
    {
        WBTeam team = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            team = WBTeam.GetFromTeamSite(SPContext.Current);

            if (team == null)
            {
                AreYouSureText.Text = "You should only be using this form when on a team site.";
                AddButton.Enabled = false;
                return;
            }

            if (!team.IsCurrentUserTeamOwnerOrSystemAdmin())
            {
                AreYouSureText.Text = "Only team owners can add members to a team.";
                AddButton.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                AreYouSureText.Text = "Are you sure you want to add all of the managers reports (all staff below manager) as members of this team?";
            }

        }

        protected void addButton_OnClick(object sender, EventArgs e)
        {
            team.AddAllManagersReports();

            CloseDialogAndRefresh();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Adding was cancelled");
        }

    }
}
