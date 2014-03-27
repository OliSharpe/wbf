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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.MyRecentWorkBoxes
{
    [ToolboxItemAttribute(false)]
    public class MyRecentWorkBoxes : WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Number To Show")]
        [WebDescription("How many recent work boxes should be listed?")]
        [System.ComponentModel.Category("Configuration")]
        public int NumberToShow { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Unique Prefix to Filter")]
        [WebDescription("Which unique prefix should be filtered from results?")]
        [System.ComponentModel.Category("Configuration")]
        public String UniquePrefixToFilter { get; set; }

        protected override void CreateChildControls()
        {
            
            Literal literal = new Literal();
            string html = "<style type=\"text/css\">\n tr.wbf-extra-recent-items {display:none;}\n</style>\n\n";

            // Now let's check or set the last visited Guid:
            SPSite _site = SPContext.Current.Site;
            SPServiceContext _serviceContext = SPServiceContext.GetContext(_site);
            UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
            UserProfile profile = _profileManager.GetUserProfile(true);

            UserProfileValueCollection workBoxesRecentlyVisited = profile[WorkBox.USER_PROFILE_PROPERTY__MY_RECENTLY_VISITED_WORK_BOXES];         

            // If the NumberToShow value isn't set or is set zero or negative then fix the web part to show 5 items:
            if (NumberToShow <= 0) NumberToShow = 5;

            if (workBoxesRecentlyVisited.Value != null)
            {
                string[] recentWorkBoxes = workBoxesRecentlyVisited.Value.ToString().Split(';');

                if (recentWorkBoxes.Length > 0)
                {
                    html += "<table cellpadding='5' width='100%'>";
                    int count = 0;
                    bool hasExtraItems = false;
                    String cssClass = "";

                    foreach (string recentWorkBox in recentWorkBoxes)
                    {
                        string[] details = recentWorkBox.Split('|');

                        String workBoxTitle = details[0];
                        String workBoxUrl = details[1];
                        String workBoxUniqueID = details[2];

                        // We're going to skip any work box whose title matches the unique prefix being filtered:
                        if (!String.IsNullOrEmpty(UniquePrefixToFilter))
                        {
                            if (workBoxUniqueID.StartsWith(UniquePrefixToFilter)) continue;
                        }

                        count++;
                        if (count > NumberToShow)
                        {
                            cssClass = " class='wbf-extra-recent-items'";
                            hasExtraItems = true;
                        }

                        html += "<tr" + cssClass + "><td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td><td><a href='";
                        html += workBoxUrl;
                        html += "'>" + workBoxTitle + "</a></td></tr>";

                    }

                    if (hasExtraItems)
                    {
                        html += "<tr class=\"wbf-show-more-recent-link\"><td colspan='2' align='right'><a href='#' onclick='javascript: $(\".wbf-extra-recent-items\").show(); $(\".wbf-show-more-recent-link\").hide(); '>More recent work boxes ...</a></td></tr>";
                        html += "<tr class=\"wbf-extra-recent-items\"><td colspan='2' align='right'><a href='#' onclick='javascript: $(\".wbf-extra-recent-items\").hide(); $(\".wbf-show-more-recent-link\").show(); '>Fewer recent work boxes</a></td></tr>";
                    }

                    html += "</table>";

                }
                else
                {
                    html += "<i>(No recently visited work boxes)</i>";
                }
            }
            else
            {
                html += "<i>(No recently visited work boxes)</i>";
            }


            literal.Text = html;

            this.Controls.Add(literal);
        }

    }
}
