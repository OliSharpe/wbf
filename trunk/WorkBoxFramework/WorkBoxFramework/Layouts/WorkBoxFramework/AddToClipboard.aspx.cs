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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class AddToClipboard : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                if (Request.QueryString["selectedItemsIDsString"] != null && Request.QueryString["selectedListGUID"] != null)
                {
                    string selectedListGUID = Request.QueryString["selectedListGUID"];
                    string[] selectedItemsIDs = Request.QueryString["selectedItemsIDsString"].ToString().Split('|');

                    WBUtils.logMessage("The list GUID was: " + selectedListGUID);
                    selectedListGUID = selectedListGUID.Substring(1, selectedListGUID.Length - 2).ToLower();

                    Guid sourceListGuid = new Guid(selectedListGUID);

                    //ListGUID.Value = sourceListGuid.ToString();
                    //ItemID.Value = selectedItemsIDs[1].ToString();

                    //WBUtils.logMessage("The ListGUID was: " + ListGUID.Value);
                    //WBUtils.logMessage("The ItemID was: " + ItemID.Value);

                    SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)WorkBox.Web.Lists[sourceListGuid];

                    WBUser user = new WBUser(WorkBox.Web.CurrentUser);

                    WorkBox.Web.AllowUnsafeUpdates = true;
                    user.AddToClipboard(WorkBox, selectedItemsIDs, true);
                    WorkBox.Web.AllowUnsafeUpdates = false;

                    RenderClipboard();
                }
                else
                {
                    ItemsOnClipboard.Text = "There was an error with the passed through values";
                }
            }


        }

        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogOK(" ");
        }


        private void RenderClipboard()
        {
            WBUser user = new WBUser(WorkBox.Web.CurrentUser);
            Dictionary<String, List<int>> clipboard = user.GetClipboard(WorkBox);

            ItemsOnClipboard.Text = WBUser.RenderClipboard(clipboard); 
        }


    }
}
