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
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class AddWorkBoxToFavourites : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                SPServiceContext _serviceContext = SPServiceContext.GetContext(WorkBox.Site);
                UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
                UserProfile profile = _profileManager.GetUserProfile(true);

                UserProfileValueCollection myFavouriteWorkBoxesPropertyValue = profile[WorkBox.USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES];

                string myFavouriteWorkBoxesString = "";

                if (myFavouriteWorkBoxesPropertyValue.Value != null)
                {
                    myFavouriteWorkBoxesString = myFavouriteWorkBoxesPropertyValue.Value.ToString();

                }

                if (myFavouriteWorkBoxesString.Contains(WorkBox.Web.ID.ToString()))
                {
                    // The user's favourites already contains this work box - so do nothing.
                    Message.Text = "The work box is already one of your favourites.";
                }
                else
                {
                    if (myFavouriteWorkBoxesString.Length > 0)
                    {
                        myFavouriteWorkBoxesString += ";";
                    }
                    myFavouriteWorkBoxesString += WorkBox.Web.Title + "|" + WorkBox.Web.Url + "|" + WorkBox.UniqueID + "|" + WorkBox.Web.ID.ToString();

                    myFavouriteWorkBoxesPropertyValue.Value = myFavouriteWorkBoxesString;
                    WorkBox.Web.AllowUnsafeUpdates = true;
                    profile.Commit();
                    WorkBox.Web.AllowUnsafeUpdates = false;

                    Message.Text = "The work box has been added to your favourites.";
                }

                okButton.Focus();
            }
        }

        protected void okButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogOK(" ");
        }

    }
}
