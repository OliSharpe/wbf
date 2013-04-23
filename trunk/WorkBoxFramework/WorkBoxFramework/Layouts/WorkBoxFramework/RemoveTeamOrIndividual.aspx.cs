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
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class RemoveTeamOrIndividual : WorkBoxDialogPageBase
    {
        WBTaxonomy teams = null;
        WBTeam team = null;
        SPUser user = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            DialogTitle.Text = "An Error Occurred";

            if (!WorkBox.CurrentUserIsOwner() && !WorkBox.CurrentUserIsBusinessAdmin() && !WorkBox.CurrentUserIsSystemAdmin())
            {
                AreYouSureText.Text = "Only owners or admin can remove teams or individuals from having access to a work box.";
                RemoveButton.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                TeamOrIndividual.Value = Request.QueryString["TeamOrIndividual"];
                InvolvedOrVisiting.Value = Request.QueryString["InvolvedOrVisiting"];

                if (TeamOrIndividual.Value == "Team")
                {
                    GUIDOfTeamToRemove.Value = Request.QueryString["GUIDOfTeamToRemove"];
                }

                if (TeamOrIndividual.Value == "Individual")
                {
                    LoginNameOfUserToRemove.Value = Request.QueryString["LoginNameOfUserToRemove"];
                }
            }

            if (String.IsNullOrEmpty(TeamOrIndividual.Value))
            {
                AreYouSureText.Text = "Error in the parameters sent to this dialog.";
                return;
            }

            AreYouSureText.Text = "Odd error in the parameters sent to this dialog.";

            if (TeamOrIndividual.Value == "Team" && !String.IsNullOrEmpty(GUIDOfTeamToRemove.Value))
            {
                teams = WBTaxonomy.GetTeams(SPContext.Current.Site);

                team = teams.GetTeam(new Guid(GUIDOfTeamToRemove.Value));

                if (!IsPostBack)
                {
                    if (InvolvedOrVisiting.Value == "Involved")
                    {
                        DialogTitle.Text = "Remove Involved Team";
                        AreYouSureText.Text = "Are you sure you want to remove the following team from being involved with this work box?";
                        NameOfTeamOrIndividual.Text = "Removing involved team: <b>" + team.Name + "</b>";
                    }
                    else
                    {
                        DialogTitle.Text = "Remove Visiting Team";
                        AreYouSureText.Text = "Are you sure you want to remove the following team from being able to visit this work box?";
                        NameOfTeamOrIndividual.Text = "Removing visiting team: <b>" + team.Name + "</b>";
                    }
                }
            }

            if (TeamOrIndividual.Value == "Individual" && !String.IsNullOrEmpty(LoginNameOfUserToRemove.Value))
            {
                user = WorkBox.Web.WBxEnsureUserOrNull(LoginNameOfUserToRemove.Value);

                if (!IsPostBack)
                {
                    if (InvolvedOrVisiting.Value == "Involved")
                    {
                        DialogTitle.Text = "Remove Involved Individual";
                        AreYouSureText.Text = "Are you sure you want to remove the following individual from being involved with this work box?";
                        NameOfTeamOrIndividual.Text = "Removing involved individual: <b>" + user.Name + "</b>";
                    }
                    else
                    {
                        DialogTitle.Text = "Remove Visiting Individual";
                        AreYouSureText.Text = "Are you sure you want to remove the following individual from being able to visit this work box?";
                        NameOfTeamOrIndividual.Text = "Removing visiting individual: <b>" + user.Name + "</b>";
                    }
                }
            }

            if (!IsPostBack)
            {
                DisposeWorkBox();
            }
        }

        protected void removeButton_OnClick(object sender, EventArgs e)
        {
            AreYouSureText.Text = "Something went wrong when trying to remove the indiviual or team.";

            if (TeamOrIndividual.Value == "Team" && team != null)
            {
                if (InvolvedOrVisiting.Value == "Involved")
                {
                    WBTermCollection<WBTeam> involvedTeams = WorkBox.InvolvedTeams;
                    involvedTeams.Remove(team);

                    WorkBox.InvolvedTeams = involvedTeams;
                    WorkBox.AuditLogEntry("Removed team", "No longer involved: " + team.Name);
                    WorkBox.Update();
                }
                else
                {
                    WBTermCollection<WBTeam> visitingTeams = WorkBox.VisitingTeams;

                    visitingTeams.Remove(team);

                    WorkBox.VisitingTeams = visitingTeams;
                    WorkBox.AuditLogEntry("Removed team", "No longer visiting: " + team.Name);
                    WorkBox.Update();
                }

                CloseDialogAndRefresh();
            }

            if (TeamOrIndividual.Value == "Individual" && user != null)
            {
                if (InvolvedOrVisiting.Value == "Involved")
                {
                    List<SPUser> involvedUsers = WorkBox.InvolvedIndividuals;

                    involvedUsers = WBUtils.RemoveUser(involvedUsers, user);

                    WorkBox.InvolvedIndividuals = involvedUsers;
                    WorkBox.AuditLogEntry("Removed individual", "No longer involved: " + user.Name);
                    WorkBox.Update();
                }
                else
                {
                    List<SPUser> visitingUsers = WorkBox.VisitingIndividuals;
                    visitingUsers = WBUtils.RemoveUser(visitingUsers, user);
                    WorkBox.VisitingIndividuals = visitingUsers;
                    WorkBox.AuditLogEntry("Removed individual", "No longer visiting: " + user.Name);
                    WorkBox.Update();
                }

                CloseDialogAndRefresh();
            }

            DisposeWorkBox();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            CloseDialogWithCancel("Inviting of teams was cancelled");
        }

    }
}
