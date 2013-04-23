#region Copyright and License

// Copyright (c) Islington Council 2010-2013
// Author: Oli Sharpe  (oli@gometa.co.uk)
//
// This file is part of the Work Box Framework.
//
// The Work Box Framework is free software: you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public License as  
// published by the Free Software Foundation, either version 2.1 of the 
// License, or (at your option) any later version.
//
// The Work Box Framework (WBF) is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with the WBF.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class RemoveFromTeam : WBDialogPageBase
    {
        WBTeam team = null;        

        protected void Page_Load(object sender, EventArgs e)
        {
            team = WBTeam.getFromTeamSite(SPContext.Current);

            if (team == null)
            {
                AreYouSureText.Text = "You should only be using this form when on a team site.";
                RemoveButton.Enabled = false;
                return;
            }

            if (!team.IsCurrentUserTeamOwnerOrSystemAdmin())
            {
                AreYouSureText.Text = "Only team owners can remove users from a team.";
                RemoveButton.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                LoginNameOfUserToRemove.Value = Request.QueryString["userLogin"];
                RoleToRemove.Value = Request.QueryString["role"];

                if (String.IsNullOrEmpty(LoginNameOfUserToRemove.Value) || String.IsNullOrEmpty(RoleToRemove.Value))
                {
                    AreYouSureText.Text = "Error in the parameters sent to this dialog.";
                    RemoveButton.Enabled = false;
                    return;
                }

                SPUser userToRemove = SPContext.Current.Web.WBxEnsureUserOrNull(LoginNameOfUserToRemove.Value);

                if (userToRemove == null)
                {
                    AreYouSureText.Text = "Cannot find the user that is to be removed: " + LoginNameOfUserToRemove.Value;
                    RemoveButton.Enabled = false;
                    return;
                }
                else
                {
                    AreYouSureText.Text = "Are you sure you want to remove the following user:";
                    NameOfIndividual.Text = RoleToRemove.Value + ": " + userToRemove.Name;
                }
            }

        }

        protected void removeButton_OnClick(object sender, EventArgs e)
        {
            AreYouSureText.Text = "Something went wrong when trying to remove the user from the team";

            SPUser userToRemove = SPContext.Current.Web.WBxEnsureUserOrNull(LoginNameOfUserToRemove.Value);
            if (userToRemove == null) return;

            if (RoleToRemove.Value == "Owner")
            {
                team.RemoveOwner(SPContext.Current.Site, userToRemove);
            } 
            else
            {
                team.RemoveMember(SPContext.Current.Site, userToRemove);
            }

            CloseDialogAndRefresh();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithCancel("Inviting of teams was cancelled");
        }

    }
}
