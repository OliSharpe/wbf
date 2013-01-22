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
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

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
        protected override void CreateChildControls()
        {

            Literal literal = new Literal();
            string html = getScriptCode();

            SPSite _site = SPContext.Current.Site;
            SPServiceContext _serviceContext = SPServiceContext.GetContext(_site);
            UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
            UserProfile profile = _profileManager.GetUserProfile(true);

            UserProfileValueCollection myFavouriteWorkBoxesPropertyValue = profile[WorkBox.USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES];

            if (myFavouriteWorkBoxesPropertyValue.Value != null)
            {
                string[] myFavouriteWorkBoxes = myFavouriteWorkBoxesPropertyValue.Value.ToString().Split(';');

                if (myFavouriteWorkBoxes.Length > 0)
                {
                    html += "<table cellpadding='5'>";
                    foreach (string recentWorkBox in myFavouriteWorkBoxes)
                    {
                        string[] details = recentWorkBox.Split('|');

                        string guidString = details[2];
                        if (details.Length == 4)
                            guidString = details[3];

                        html += "<tr><td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td><td><a href='";
                        html += details[1];
                        html += "'>" + details[0] + "</a></td>";
                        html += "<td><a href='#' onclick='javascript: RemoveFavourite_commandAction(\"" + details[0] + "\", \"" + guidString + "\");'>remove</a></td>";
                            html += "</tr>";
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