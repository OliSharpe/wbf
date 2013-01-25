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
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class WorkBoxPicker : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string html = "";
                int count = 0;


                /*
                 *  First add the recent work boxes:
                 */

                SPSite _site = SPContext.Current.Site;
                SPServiceContext _serviceContext = SPServiceContext.GetContext(_site);
                UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
                UserProfile profile = _profileManager.GetUserProfile(true);

                UserProfileValueCollection workBoxesRecentlyVisited = profile[WorkBox.USER_PROFILE_PROPERTY__MY_RECENTLY_VISITED_WORK_BOXES];

                if (workBoxesRecentlyVisited.Value != null)
                {
                    string[] recentWorkBoxes = workBoxesRecentlyVisited.Value.ToString().Split(';');

                    if (recentWorkBoxes.Length > 0)
                    {
                        html += "<table cellpadding='5'>";
                        count = 0;
                        foreach (string recentWorkBox in recentWorkBoxes)
                        {
                            string[] details = recentWorkBox.Split('|');

                            string workBoxTitle = details[0];
                            string workBoxURL = details[1];

                            html += string.Format("<tr><td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td><td><a href='#' onclick='javascript: WorkBoxFramework_WorkBoxPicker_pickWorkBox(\"{0}\", \"{1}\");'>{1}</a></td></tr>\n", workBoxURL, workBoxTitle);

                            count++;
                            if (count >= 10) break;
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


                RecentWorkBoxes.Text = html;


                html = "";


                /*
                 *  Now add the favourite work boxes: 
                 */

                UserProfileValueCollection myFavouriteWorkBoxesPropertyValue = profile[WorkBox.USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES];

                if (myFavouriteWorkBoxesPropertyValue.Value != null)
                {
                    string[] myFavouriteWorkBoxes = myFavouriteWorkBoxesPropertyValue.Value.ToString().Split(';');

                    if (myFavouriteWorkBoxes.Length > 0)
                    {
                        html += "<table cellpadding='5'>";
                        count = 0;
                        foreach (string recentWorkBox in myFavouriteWorkBoxes)
                        {
                            string[] details = recentWorkBox.Split('|');

                            string workBoxTitle = details[0];
                            string workBoxURL = details[1];

                            html += string.Format("<tr><td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td><td><a href='#' onclick='javascript: WorkBoxFramework_WorkBoxPicker_pickWorkBox(\"{0}\", \"{1}\");'>{1}</a></td></tr>\n", workBoxURL, workBoxTitle);

                            count++;
                            if (count >= 10) break;
                        }
                        html += "</table>";
                    }
                    else
                    {
                        html += "<i>(No favourite work boxes)</i>";
                    }
                }
                else
                {
                    html += "<i>(No favourite work boxes)</i>";
                }


                FavouriteWorkBoxes.Text = html;




            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            returnFromDialogCancel("Picking of a work box was cancelled.");
        }
    }
}
