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
using Microsoft.SharePoint.Utilities;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class EditWorkBoxProperties : WorkBoxDialogPageBase
    {
        protected bool showReferenceID = true;
        protected bool showReferenceDate = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            WBRecordsType recordsType = WorkBox.RecordsType;
            if (recordsType.WorkBoxReferenceIDRequirement == WBRecordsType.METADATA_REQUIREMENT__HIDDEN)
            {
                showReferenceID = false;
            }

            if (recordsType.WorkBoxReferenceDateRequirement == WBRecordsType.METADATA_REQUIREMENT__HIDDEN)
            {
                showReferenceDate = false;
            }

            if (!IsPostBack)
            {
                WorkBoxTitle.Text = WorkBox.Title;
                OwningTeam.Text = WorkBox.OwningTeam.Name;
                FunctionalArea.Text = WorkBox.FunctionalArea(WBTaxonomy.GetFunctionalAreas(WorkBox.RecordsTypes)).Names();
                RecordsType.Text = recordsType.FullPath;
                WorkBoxTemplate.Text = WorkBox.Template.Title;
                WorkBoxStatus.Text = WorkBox.Status;
                WorkBoxURL.Text = WorkBox.Url;
                WorkBoxShortTitle.Text = WorkBox.ShortTitle;
                WorkBoxPrettyTitle.Text = WorkBox.Web.Title;

                WorkBoxShortTitle.Focus();

                if (showReferenceID)
                {
                    ReferenceID.Text = WorkBox.ReferenceID;
                }

                if (showReferenceDate)
                {
                    if (WorkBox.ReferenceDateHasValue)
                    {
                        ReferenceDate.SelectedDate = WorkBox.ReferenceDate;
                    }
                }
            }
        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                bool digestOK = WorkBox.Web.ValidateFormDigest();

                if (digestOK)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite elevatedSite = new SPSite(WorkBox.Site.ID))
                        using (SPWeb elevatedWorkBoxWeb = elevatedSite.OpenWeb(WorkBox.Web.ID))
                        {
                            elevatedSite.AllowUnsafeUpdates = true;
                            elevatedWorkBoxWeb.AllowUnsafeUpdates = true;

                            WorkBox elevatedWorkBox = new WorkBox(elevatedSite, elevatedWorkBoxWeb);
                            elevatedWorkBox.ShortTitle = WorkBoxShortTitle.Text;
                            elevatedWorkBox.Web.Title = WorkBoxPrettyTitle.Text;
                            elevatedWorkBox.GenerateTitle();

                            if (showReferenceID)
                            {
                                elevatedWorkBox.ReferenceID = ReferenceID.Text;
                            }

                            if (showReferenceDate)
                            {
                                if (!ReferenceDate.IsDateEmpty)
                                {
                                    elevatedWorkBox.ReferenceDate = ReferenceDate.SelectedDate;
                                }
                            }

                            elevatedWorkBox.Update();
                        }
                    });
                }            
            }
            catch (Exception exception)
            {
                WBUtils.SendErrorReport(SPContext.Current.Web, "Exception in EditWorkBoxPropertise.saveButton_OnClick()", "Something went wrong when saving: " + exception.Message + " ... " + exception.StackTrace);
                throw new NotImplementedException("Something went wrong when saving the properties changes");
            }

            WBFarm farm = WBFarm.Local;
            String cachedDetailsListUrl = farm.OpenWorkBoxesCachedDetailsListUrl;

            if (!String.IsNullOrEmpty(cachedDetailsListUrl))
            {
                using (SPSite cacheSite = new SPSite(cachedDetailsListUrl))
                using (SPWeb cacheWeb = cacheSite.OpenWeb())
                {
                    SPList cacheList = cacheWeb.GetList(cachedDetailsListUrl);

                    SPServiceContext serviceContext = SPServiceContext.GetContext(cacheSite);
                    UserProfileManager profileManager = new UserProfileManager(serviceContext);

                    // Get the current user's user profile:
                    UserProfile profile = profileManager.GetUserProfile(true);

                    // We're using the 'now' plus one hour ticks as we're not really looking to update the last modified dates of other work boxes.
                    WBUser.CheckLastModifiedDatesAndTitlesOfRecentWorkBoxes(cacheSite, cacheList, profile, DateTime.Now.AddHours(1).Ticks);

                    WBUser.CheckTitlesOfFavouriteWorkBoxes(cacheSite, cacheList, profile);
                }
            }

            returnFromDialogOK("");
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }

    }
}
