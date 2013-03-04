﻿#region Copyright and License

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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace WorkBoxFramework.TeamDetails
{
    public partial class TeamDetailsUserControl : UserControl
    {
        public bool userIsTeamOwner = false;

        private int countPawnsOnPage = 0;

        protected TeamDetails webPart = default(TeamDetails);

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as TeamDetails;

            if (!IsPostBack)
            {
                WBTeam team = WBTeam.getFromTeamSite(SPContext.Current);

                if (team == null)
                {
                    // then the SPContext is NOT on a team site 
                    ListOfTeamOwners.Text = "<i>(This web part is only for use in team sites)</i>";
                    ListOfTeamMembers.Text = "<i>(This web part is only for use in team sites)</i>";
                }
                else
                {
                    if (team.IsCurrentUserTeamOwner()) userIsTeamOwner = true;
                    
                    ListOfTeamOwners.Text = generateTable(team, team.OwnersGroupName, "Owner", "Team Owners");
                    ListOfTeamMembers.Text = generateTable(team, team.MembersGroupName, "Member", "Team Members");
                }
            }
        }

        private String generateTable(WBTeam team, String groupName, String groupType, String title)
        {
            string html = "";
            SPGroup group = SPContext.Current.Site.RootWeb.WBxGetGroupOrNull(groupName);

            if (group == null)
            {
                // Then either the owners group name has not been defined for this team, or the group doesn’t exist for some reason!
                html += "<i>(The " + groupType + " group name has not been defined for this team, or the group doesn’t exist for some reason)</i>";
            }
            else
            {
                // 
                if (group.OnlyAllowMembersViewMembership && !group.ContainsCurrentUser) return "";

                html += "<h3>" + title + ":</h3>\n";

                // OK so now we have the SPGroup for the team’s owners group. 
                // Now we can iterate through the SPUser-s in this group … or whatever else we want to do with it, e.g.:

                List<String> teamEmails = new List<String>();

                html += "<table cellpadding='5'><tr><td><ul>";
                foreach (SPUser user in group.Users)
                {
                    html += "<li>" + user.WBxToHTML(Context); //renderUser(user, SPContext.Current.Site.RootWeb);

                    if (userIsTeamOwner)
                    {
                        string actionURL = "RemoveFromTeam.aspx?userLogin=" + user.LoginName.Replace("\\", "\\\\") + "&role=" + groupType;

                        html += " <a href=\"javascript: WorkBoxFramework_relativeCommandAction('" + actionURL + "', 400, 200); \">(remove)</a>";
                    }

                    html += "</li>";

                    if (!String.IsNullOrEmpty(user.Email) && !teamEmails.Contains(user.Email))
                    {
                        teamEmails.Add(user.Email);
                    }
                }

                html += "</ul></td></tr>\n";
                html += "</table>\n";

                if (webPart.ShowMailToLinks)
                {
                    html += "<div class='wbf-mail-to-team'><a href='mailto:" + String.Join(",", teamEmails.ToArray()) + "'>Email all " + title.ToLower() + "</div>";
                }
            }

            return html;
        }

        private String renderUser(SPUser user)
        {
            string html = "<span class=\"ms-imnSpan\">\n";
            html += "<a class=\"ms-imnlink\" onclick=\"IMNImageOnClick(event);return false;\" href=\"javascript:;\"/>\n";
            html += "<img name=\"imnmark\" width=\"12\" height=\"12\" title=\"\" class=\"ms-imnImg\" id=\"imn_220,type=smtp\" alt=\"Available\" src=\"/_layouts/images/imnon.png\" border=\"0\" complete=\"complete\" sip=\"" + user.Email + "\"/>\n";

            html += "<a onclick=\"GoToLink(this);return false;\" href=\"/_layouts/userdisp.aspx?ID=" + user.ID + "\">" + user.Name + "</a></span>\n";

            return html;
        }

        // Based on ideas picked up from: 
        // http://blogs.msdn.com/b/uksharepoint/archive/2010/05/07/office-communicator-integration-presence-in-a-custom-webpart-for-sharepoint-2010.aspx
        private String renderUser(SPUser user, SPWeb rootWeb)
        {
            countPawnsOnPage++;

            SPListItem userListItem = rootWeb.SiteUserInfoList.GetItemById(user.ID);
            string sipAddress = userListItem.WBxGetColumnAsString("SipAddress");

            string id = "WBF_PresenceLink_" + countPawnsOnPage;

            // return the html for this user
            return String.Concat(
            "<span id\""
            , id
            , "\">"
            , "<img border=\"0\" height=\"12\" src=\"/_layouts/images/imnhdr.gif\" onload=\"WorkBoxFramework__add_user_presence('"
            , id
            , "','"
            , sipAddress
            , "', this)\" ShowOfflinePawn=\"1\" style=\"padding-right: 3px;\" id=\"PresencePawn"
            , sipAddress
            , "\" alt=\"Presence pawn for "
            , sipAddress
            , "\"/>"
            , "<a href=\""
            , rootWeb.Url
            , "/_layouts/userdisp.aspx?ID="
            , user.ID
            , "\" id=\"ProfileLink"
            , sipAddress
            , "\">"
            , user.Name
            , "</a></span>"
            );
        }


    }
}
