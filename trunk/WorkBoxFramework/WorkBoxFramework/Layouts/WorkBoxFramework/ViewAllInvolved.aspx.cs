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
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ViewAllInvolved : WorkBoxDialogPageBase
    {
        bool currentUserCanRemoveIndividuals = false;
        bool currentUserCanRemoveTeams = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                generateTableOfAllInvolved();
            }
        }

        private void generateTableOfAllInvolved()
        {
            WBAction inviteIndividualsAction = WorkBox.GetAction(WBAction.ACTION_KEY__INVITE_INDIVIDUALS);
            if (inviteIndividualsAction.IsEnabled)
            {
                currentUserCanRemoveIndividuals = true;
            }

            WBAction inviteTeamsAction = WorkBox.GetAction(WBAction.ACTION_KEY__INVITE_TEAMS);
            if (inviteTeamsAction.IsEnabled)
            {
                currentUserCanRemoveTeams = true;
            }

            SPServiceContext serviceContext = SPServiceContext.GetContext(SPContext.Current.Site);
            UserProfileManager profileManager = new UserProfileManager(serviceContext);

            Dictionary<String, String> headers = new Dictionary<String, String>();
            headers.Add("body", "%0D%0A%0D%0A%0D%0AWork Box Title: " + WorkBox.Title + "%0D%0AWork Box URL: " + WorkBox.Url);

            string html = "<p>Users Involved with <b>" + WorkBox.Title + "</b></p>\n";

            html += "<table class=\"wbf-dialog-form\">\n";
           
            if (WorkBox.OwningTeam != null)
            {
                html += "<tr><td class=\"wbf-field-name-panel\"><b>Owning Team:</b><ul><li>";

                html += WBUtils.GenerateLinkToEmailGroup("Email work box owners", WorkBox.GetAllOwners(SPContext.Current.Site).WBxToEmails(), headers);

                html += "</li></ul></td><td class=\"wbf-field-value-panel\">\n";

                html += renderTeamAsFieldSet(profileManager, SPContext.Current.Site, WorkBox.OwningTeam);

                html += "</td></tr>\n";
            }


            html += "<tr><td class=\"wbf-field-name-panel\"><b>Involved Teams:</b><ul><li>";

            html += WBUtils.GenerateLinkToEmailGroup("Email all involved with work box", WorkBox.GetAllInvolved(SPContext.Current.Site).WBxToEmails(), headers);

            html += "</li></ul></td><td class=\"wbf-field-value-panel\">\n";

            if (WorkBox.InvolvedTeams != null && WorkBox.OwningTeam != null)
            {
                foreach (WBTeam involved in WorkBox.InvolvedTeams)
                {
                    if (involved.Id.Equals(WorkBox.OwningTeam.Id)) continue;

                    html += renderTeamAsFieldSet(profileManager, SPContext.Current.Site, involved, "Involved");
                }
            }

            html += "</td></tr>\n";

            html += "<tr><td class=\"wbf-field-name-panel\"><b>Involved Individuals:</b></td><td class=\"wbf-field-value-panel\"><ul>\n";

            if (WorkBox.InvolvedIndividuals != null)
            {
                foreach (SPUser user in WorkBox.InvolvedIndividuals)
                {
                    html += "<li>" + renderUser(profileManager, user, "Involved") + "</li>";
                }
            }

            html += "</ul>";
            html += "</td></tr>\n";


            html += "<tr><td class=\"wbf-field-name-panel\"><b>Visiting Teams:</b><ul><li>";

            html += WBUtils.GenerateLinkToEmailGroup("Email everyone who can visit the work box", WorkBox.GetAllWhoCanVisit(SPContext.Current.Site).WBxToEmails(), headers);

            html += "</li></ul></td><td class=\"wbf-field-value-panel\">\n";

            if (WorkBox.VisitingTeams != null)
            {
                foreach (WBTeam visiting in WorkBox.VisitingTeams)
                {
                    html += renderTeamAsFieldSet(profileManager, SPContext.Current.Site, visiting, "Visiting");
                }
            }

            html += "</td></tr>\n";

            html += "<tr><td class=\"wbf-field-name-panel\"><b>Visiting Individuals:</b></td><td class=\"wbf-field-value-panel\"><ul>\n";

            if (WorkBox.VisitingIndividuals != null)
            {
                foreach (SPUser user in WorkBox.VisitingIndividuals)
                {
                    html += "<li>" + renderUser(profileManager, user, "Visiting") + "</li>";
                }
            }

            html += "</ul>";
            html += "</td></tr>\n";


            html += "</table>\n";

            GeneratedViewOfAllInvolved.Text = html;

        }
        private String renderTeamAsFieldSet(UserProfileManager profileManager, SPSite site, WBTeam team)
        {
            return renderTeamAsFieldSet(profileManager, site, team, "");
        }

        private String renderTeamAsFieldSet(UserProfileManager profileManager, SPSite site, WBTeam team, String involvedOrVisiting)
        {
            SPGroup group = team.MembersGroup(site);

            string html = "<fieldset><legend><a href=\"#\" onclick=\"javascript: dialogReturnOKAndRedirect('" + team.TeamSiteUrl + "');\">" + team.Name + "</a>";

            if (!String.IsNullOrEmpty(involvedOrVisiting) && currentUserCanRemoveTeams)
            {
                html += " <a href=\"javascript: removeTeam('" + involvedOrVisiting + "','" + team.Id.ToString() + "');\">(remove team)</a>";
            }

            html += "</legend><ul>\n";

            if (group == null)
            {
                html += "<i>(no user group defined for this team)</i>";
            }
            else
            {
                foreach (SPUser user in group.Users)
                {
                    html += "<li>" + renderUser(profileManager, user, "") + "</li>\n";
                }
            }


            html += "</ul></fieldset>\n";
            
            return html;
        }

        private String renderUser(UserProfileManager profileManager, SPUser user)
        {
            return renderUser(profileManager, user, "");
        }

        private String renderUser(UserProfileManager profileManager, SPUser user, String involvedOrVisiting)
        {
            string html = "<span class=\"wbf-view-involved-user\">\n";

            html += user.WBxToHTML(profileManager, Context);

            if (!String.IsNullOrEmpty(involvedOrVisiting) && currentUserCanRemoveIndividuals)
            {
                html += " <a href=\"javascript: removeIndividual('" + involvedOrVisiting + "','" + user.LoginName.Replace("\\", "\\\\") + "');\">(remove individual)</a>";
            }

            html += "</span>\n";

            return html;
        }

        protected void refreshTeams_OnClick(object sender, EventArgs e)
        {
            WorkBox.RefreshTeams();

            generateTableOfAllInvolved();
            DisposeWorkBox();
        }


        protected void close_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            returnFromDialogOK("  ");
        }

    }
}
