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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class WorkBoxPortalSettingsPage : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControls();
        }


        void PopulateControls()
        {

            if (!IsPostBack)
            {

                WBCollection collection = new WBCollection(SPContext.Current);

                WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);

                teams.InitialiseTaxonomyControl(SystemAdminTeams, "Select the System Admin Teams", true);
                SystemAdminTeams.Text = collection.SystemAdminTeams.UIControlValue;

                teams.InitialiseTaxonomyControl(BusinessAdminTeams, "Select the Business Admin Teams", true);
                BusinessAdminTeams.Text = collection.BusinessAdminTeams.UIControlValue;

                NameOfAllWorkBoxesList.Text = collection.ListName;

                if (collection.EventReceiversAdded)
                {
                    EventReceiverStatus.Text = "<i>(Event receiver <b>has</b> been added)</i>";
                }

                GenerateUniqueIds.Checked = collection.GenerateUniqueIDs;
                WorkBoxCollectionUniqueIdPrefix.Text = collection.UniqueIDPrefix;
                NumberOfDigitsInIds.Text = collection.NumberOfDigitsInIDs.ToString();
                InitialIdOffset.Text = collection.InitialIDOffset.ToString();

                CanAnyoneCreate.Checked = collection.CanAnyoneCreate;

                SysadminOpen.Text = collection.OpenPermissionLevelForSystemAdmin;
                AdminOpen.Text = collection.OpenPermissionLevelForBusinessAdmin;
                OwnerOpen.Text = collection.OpenPermissionLevelForOwner;
                InvolvedOpen.Text = collection.OpenPermissionLevelForInvolved;
                VisitorsOpen.Text = collection.OpenPermissionLevelForVisitors;
                EveryoneOpen.Text = collection.OpenPermissionLevelForEveryone;

                SysadminClosed.Text = collection.ClosedPermissionLevelForSystemAdmin;
                AdminClosed.Text = collection.ClosedPermissionLevelForBusinessAdmin;
                OwnerClosed.Text = collection.ClosedPermissionLevelForOwner;
                InvolvedClosed.Text = collection.ClosedPermissionLevelForInvolved;
                VisitorsClosed.Text = collection.ClosedPermissionLevelForVisitors;
                EveryoneClosed.Text = collection.ClosedPermissionLevelForEveryone;

                UseFolderAccessGroupsPattern.Checked = collection.UseFolderAccessGroupsPattern; 
                FolderAccessGroupsPrefix.Text = collection.FolderAccessGroupsPrefix;
                FolderAccessGroupsFolderNames.Text = collection.FolderAccessGroupsFolderNames;
                FolderAccessGroupPermissionLevel.Text = collection.FolderAccessGroupPermissionLevel;
                AllFoldersAccessGroupPermissionLevel.Text = collection.AllFoldersAccessGroupPermissionLevel;

                NewWorkBoxDialogUrl.Text = collection.UrlForNewWorkBoxDialog;
                CreateNewWorkBoxText.Text = collection.CreateNewWorkBoxText;

                teams.InitialiseTaxonomyControl(DefaultOwningTeam, "Select the Default Owning Team", false);
                DefaultOwningTeam.Text = collection.DefaultOwningTeamUIControlValue;

            }
        }


        protected void okButton_OnClick(object sender, EventArgs e)
        {
            String errorMessage = "No error occurred";

            try
            {
                WBCollection collection = new WBCollection(SPContext.Current);

                WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);

                collection.SystemAdminTeams = new WBTermCollection<WBTeam>(teams, SystemAdminTeams.Text);
                collection.BusinessAdminTeams = new WBTermCollection<WBTeam>(teams, BusinessAdminTeams.Text);

                // OK so first make sure that the event receiver is attached to the correct list
                collection.ListName = NameOfAllWorkBoxesList.Text;

                // OK so now to save the various other settings values:

                collection.UniqueIDPrefix = WorkBoxCollectionUniqueIdPrefix.Text;
                collection.GenerateUniqueIDs = GenerateUniqueIds.Checked;

                collection.NumberOfDigitsInIDs = Convert.ToInt32(NumberOfDigitsInIds.Text);
                collection.InitialIDOffset = Convert.ToInt32(InitialIdOffset.Text);
                
                collection.CanAnyoneCreate = CanAnyoneCreate.Checked;

                collection.OpenPermissionLevelForSystemAdmin = SysadminOpen.Text;
                collection.OpenPermissionLevelForBusinessAdmin = AdminOpen.Text;
                collection.OpenPermissionLevelForOwner = OwnerOpen.Text;
                collection.OpenPermissionLevelForInvolved = InvolvedOpen.Text;
                collection.OpenPermissionLevelForVisitors = VisitorsOpen.Text;
                collection.OpenPermissionLevelForEveryone = EveryoneOpen.Text;

                collection.ClosedPermissionLevelForSystemAdmin = SysadminClosed.Text;
                collection.ClosedPermissionLevelForBusinessAdmin = AdminClosed.Text;
                collection.ClosedPermissionLevelForOwner = OwnerClosed.Text;
                collection.ClosedPermissionLevelForInvolved = InvolvedClosed.Text;
                collection.ClosedPermissionLevelForVisitors = VisitorsClosed.Text;
                collection.ClosedPermissionLevelForEveryone = EveryoneClosed.Text;

                collection.UseFolderAccessGroupsPattern = UseFolderAccessGroupsPattern.Checked;
                collection.FolderAccessGroupsPrefix = FolderAccessGroupsPrefix.Text;
                collection.FolderAccessGroupsFolderNames = FolderAccessGroupsFolderNames.Text;
                collection.FolderAccessGroupPermissionLevel = FolderAccessGroupPermissionLevel.Text;
                collection.AllFoldersAccessGroupPermissionLevel = AllFoldersAccessGroupPermissionLevel.Text;
                

                collection.UrlForNewWorkBoxDialog = NewWorkBoxDialogUrl.Text;
                collection.CreateNewWorkBoxText = CreateNewWorkBoxText.Text;

                collection.DefaultOwningTeamUIControlValue = DefaultOwningTeam.Text;
                
                collection.Update();

            }
            catch (Exception exception)
            {
                WBLogging.WorkBoxCollections.Unexpected(exception.StackTrace);
                SPUtility.TransferToErrorPage("An exception occurred : " + exception.StackTrace, "Return to site settings page", "/_layouts/settings.aspx");
            }

            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }
    }
}
