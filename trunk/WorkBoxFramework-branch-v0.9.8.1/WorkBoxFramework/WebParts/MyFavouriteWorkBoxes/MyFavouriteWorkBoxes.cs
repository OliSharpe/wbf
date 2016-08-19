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

namespace WorkBoxFramework.MyFavouriteWorkBoxes
{
    [ToolboxItemAttribute(false)]
    public class MyFavouriteWorkBoxes : WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Number To Show")]
        [WebDescription("How many favourite work boxes should be listed?")]
        [System.ComponentModel.Category("Configuration")]
        public int NumberToShow { get; set; }

        protected override void CreateChildControls()
        {

            Literal literal = new Literal();
            string html = "";


            try
            {
                html += "<style type=\"text/css\">\n tr.wbf-extra-favourites-items {display:none;}\n</style>\n\n" + getScriptCode();

                SPSite _site = SPContext.Current.Site;
                SPServiceContext _serviceContext = SPServiceContext.GetContext(_site);
                UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
                UserProfile profile = _profileManager.GetUserProfile(true);

                UserProfileValueCollection myFavouriteWorkBoxesPropertyValue = profile[WorkBox.USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES];

                // If the NumberToShow value isn't set or is set zero or negative then fix the web part to show 5 items:
                if (NumberToShow <= 0) NumberToShow = 5;

                if (myFavouriteWorkBoxesPropertyValue.Value != null)
                {
                    string[] myFavouriteWorkBoxes = myFavouriteWorkBoxesPropertyValue.Value.ToString().Split(';');

                    // We actually want to display the most recently added favourite first even though it'll be last in the list so:
                    Array.Reverse(myFavouriteWorkBoxes);

                    if (myFavouriteWorkBoxes.Length > 0)
                    {
                        html += "<table cellpadding='5' width='100%'>";
                        int count = 0;
                        bool hasExtraItems = false;
                        String cssClass = "";

                        foreach (string workBoxLinkDetails in myFavouriteWorkBoxes)
                        {
                            WBLink workBoxLink = new WBLink(workBoxLinkDetails);
                            if (!workBoxLink.IsOK) continue;

                            count++;

                            if (count > NumberToShow)
                            {
                                cssClass = " class='wbf-extra-favourites-items'";
                                hasExtraItems = true;
                            }

                            /*
                            string[] details = recentWorkBoxDetails.Split('|');

                            string guidString = details[2];
                            if (details.Length == 4)
                                guidString = details[3];

                            html += "<tr" + cssClass + "><td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td><td><a href='";
                            html += details[1];
                            html += "'>" + details[0] + "</a></td>";

                            String command = "RemoveWorkBoxFromFavourites.aspx?workBoxTitle=" + HttpUtility.UrlEncode(details[0]) + "&workBoxGuid=" + guidString;

                            html += "<td><a href='#' onclick='javascript: WorkBoxFramework_relativeCommandAction(\"" + command + "\", 0, 0);'>remove</a></td>";
                            html += "</tr>";
                            */

                            html += "<tr" + cssClass + "><td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td><td><a href='";
                            html += workBoxLink.URL;
                            html += "'>" + workBoxLink.Title + "</a></td>";

                            String command = "RemoveWorkBoxFromFavourites.aspx?workBoxTitle=" + HttpUtility.UrlEncode(workBoxLink.Title) + "&workBoxGuid=" + workBoxLink.SPWebGUID;

                            html += "<td><a href='#' onclick='javascript: WorkBoxFramework_relativeCommandAction(\"" + command + "\", 0, 0);'>remove</a></td>";
                            html += "</tr>";


                        }

                        if (hasExtraItems)
                        {
                            html += "<tr class=\"wbf-show-more-favourites-link\"><td colspan='3' align='right'><a href='#' onclick='javascript: $(\".wbf-extra-favourites-items\").show(); $(\".wbf-show-more-favourites-link\").hide(); '>More favourite work boxes ...</a></td></tr>";
                            html += "<tr class=\"wbf-extra-favourites-items\"><td colspan='3' align='right'><a href='#' onclick='javascript: $(\".wbf-extra-favourites-items\").hide(); $(\".wbf-show-more-favourites-link\").show(); '>Fewer favourite work boxes</a></td></tr>";
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
            }
            catch (Exception e)
            {
                html += "<i>(An error occurred)</i> \n\n <!-- \n Exception was: " + e.WBxFlatten() + " \n\n -->";
            }            

            literal.Text = html;

            this.Controls.Add(literal);
        }

        private string getScriptCode()
        {
            string scriptCode = "<script type=\"text/javascript\">";

            scriptCode += "function RemoveFavourite_callback(dialogResult, returnValue) {";

    scriptCode += "if (dialogResult == SP.UI.DialogResult.OK) {";

            scriptCode += "location.reload(true);";
            scriptCode += "return;";
        scriptCode += "}";

    scriptCode += "if (dialogResult == SP.UI.DialogResult.cancel) {";

        scriptCode += "this.statusId = SP.UI";
            scriptCode += ".Status";
            scriptCode += ".addStatus(\"Action Cancelled\", returnValue, true);";

        scriptCode += "SP.UI.Status.setStatusPriColor(this.statusId, \"blue\");";

    scriptCode += "setTimeout(RemoveFavourite_removeStatus, 5000);";
            
            scriptCode += "}";


scriptCode += "}";

scriptCode += "function RemoveFavourite_removeStatus() {";
    scriptCode += "SP.UI.Status.removeAllStatus(true);";

   scriptCode += "statusId = '';";
scriptCode += "}";

scriptCode += "function RemoveFavourite_commandAction(workBoxTitle, workBoxGuid) {";                        

    scriptCode += "var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/RemoveWorkBoxFromFavourites.aspx?";

            scriptCode += "workBoxTitle=' + workBoxTitle + '&workBoxGuid=' + workBoxGuid;";

     scriptCode += "var options = {";
        scriptCode += " url: urlValue,";
         scriptCode += "tite: 'Work Box Dialog',";
        scriptCode += " allowMaximize: false,";
         scriptCode += "showClose: false,";
         scriptCode += "width: 300,";
        scriptCode += "height: 200,";
        scriptCode += "dialogReturnValueCallback: RemoveFavourite_callback";
     scriptCode += "};";

    scriptCode += " SP.UI.ModalDialog.showModalDialog(options); ";
 scriptCode += "}";

            
            scriptCode += "</script>";

            return scriptCode;
        }
    }
}
