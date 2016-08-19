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
    public partial class RemoveWorkBoxFromFavourites : WBDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                WorkBoxTitle.Value = Request.QueryString["workBoxTitle"]; ;
                WorkBoxGuid.Value = Request.QueryString["workBoxGuid"]; ;

                Message.Text = "Are you sure you want to remove the work box called '" + WorkBoxTitle.Value + "' from your list of favourite work boxes?";

                cancel.Focus();
            }


        }



        protected void removeFromFavouritesButton_OnClick(object sender, EventArgs e)
        {
            SPSite site = SPContext.Current.Site;
            SPServiceContext serviceContext = SPServiceContext.GetContext(site);
            UserProfileManager profileManager = new UserProfileManager(serviceContext);
            UserProfile profile = profileManager.GetUserProfile(true);

            UserProfileValueCollection myFavouriteWorkBoxesPropertyValue = profile[WorkBox.USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES];

            string myFavouriteWorkBoxesString = "";


            if (myFavouriteWorkBoxesPropertyValue.Value != null)
            {
                myFavouriteWorkBoxesString = myFavouriteWorkBoxesPropertyValue.Value.ToString();
            }

            string guidStringToRemove = WorkBoxGuid.Value;

            List<String> updatedFavouritesList = new List<String>();
            string[] favouriteWorkBoxes = myFavouriteWorkBoxesString.Split(';');
            foreach (string favouriteWorkBox in favouriteWorkBoxes)
            {
                    if (!favouriteWorkBox.Contains(guidStringToRemove))
                    {
                        updatedFavouritesList.Add(favouriteWorkBox);
                    }
            }

            myFavouriteWorkBoxesPropertyValue.Value = String.Join(";", updatedFavouritesList.ToArray());

            site.AllowUnsafeUpdates = true;
            profile.Commit();
            site.AllowUnsafeUpdates = false;

            CloseDialogAndRefresh();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithCancel("Removal from favourites was cancelled.");
        }


    }
}
