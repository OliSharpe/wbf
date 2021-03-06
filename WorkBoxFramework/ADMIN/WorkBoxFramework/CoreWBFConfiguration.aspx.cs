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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class CoreWBFConfiguration : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WBFarm farm = WBFarm.Local;

                FarmInstance.DataSource = WBFarm.GetFarmInstances();
                FarmInstance.DataBind();
                FarmInstance.WBxSafeSetSelectedValue(farm.FarmInstance);

                ProtectedRecordsLibraryURL.Text = farm.ProtectedRecordsLibraryUrl;
                PublicRecordsLibraryURL.Text = farm.PublicRecordsLibraryUrl;
                PublicExtranetRecordsLibraryURL.Text = farm.PublicExtranetRecordsLibraryUrl;

                TeamSitesSiteCollectionURL.Text = farm.TeamSitesSiteCollectionUrl;

                WBLogging.Debug("Got here");

                WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);

                if (teams == null) WBLogging.Debug("teams was null");

                WBTeam systemAdminTeam = farm.SystemAdminTeam(teams);

                if (systemAdminTeam != null)
                {
                    SystemAdminTeam.Text = systemAdminTeam.FullPath;
                }
                else
                {
                    WBLogging.Debug("systemAdminTeam was null");
                    SystemAdminTeam.Text = "";
                }
   //             SystemAdminTeam.Text = farm.SystemAdminTeamGUIDString;

                OpenWorkBoxesCachedDetailsListURL.Text = farm.OpenWorkBoxesCachedDetailsListUrl;

                RecordsManagersGroupName.Text = farm.RecordsManagersGroupName;
                RecordsSystemAdminGroupName.Text = farm.RecordsSystemAdminGroupName;

                TimerJobsManagementSiteURL.Text = farm.TimerJobsManagementSiteUrl;
                TimerJobsServerName.Text = farm.TimerJobsServerName;

                AllWorkBoxCollections.Text = farm.AllWorkBoxCollectionsPropertyValue;

                TermStoreName.Text = farm.TermStoreName;
                TermStoreGroupName.Text = farm.TermStoreGroupName;

                DocumentContentTypeName.Text = farm.WorkBoxDocumentContentTypeName;
                RecordContentTypeName.Text = farm.WorkBoxRecordContentTypeName;

                SendErrorReportsTo.Text = farm.SendErrorReportEmailsTo;
            }
        }


        protected void okButton_OnClick(object sender, EventArgs e)
        {
            WBFarm farm = WBFarm.Local;

            farm.FarmInstance = FarmInstance.SelectedValue;

            farm.ProtectedRecordsLibraryUrl = ProtectedRecordsLibraryURL.Text;
            farm.PublicRecordsLibraryUrl = PublicRecordsLibraryURL.Text;
            farm.PublicExtranetRecordsLibraryUrl = PublicExtranetRecordsLibraryURL.Text;

            farm.TeamSitesSiteCollectionUrl = TeamSitesSiteCollectionURL.Text;

            //farm.SystemAdminTeamGUIDString = SystemAdminTeam.Text;

            
            if (!String.IsNullOrEmpty(SystemAdminTeam.Text))
            {
                WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);
                WBTeam systemAdminTeam = teams.GetSelectedTeam(SystemAdminTeam.Text);
                if (systemAdminTeam != null)
                {
                    farm.SystemAdminTeamGUIDString = systemAdminTeam.Id.ToString();
                }
                else
                {
                    farm.SystemAdminTeamGUIDString = "";
                }
            }
            else
            {
                farm.SystemAdminTeamGUIDString = "";
            }
            

            farm.OpenWorkBoxesCachedDetailsListUrl = OpenWorkBoxesCachedDetailsListURL.Text;

            farm.RecordsManagersGroupName = RecordsManagersGroupName.Text;
            farm.RecordsSystemAdminGroupName = RecordsSystemAdminGroupName.Text;

            farm.TimerJobsManagementSiteUrl = TimerJobsManagementSiteURL.Text;
            farm.TimerJobsServerName = TimerJobsServerName.Text;

            farm.AllWorkBoxCollectionsPropertyValue = AllWorkBoxCollections.Text;

            farm.TermStoreName = TermStoreName.Text;
            farm.TermStoreGroupName = TermStoreGroupName.Text;

            farm.WorkBoxDocumentContentTypeName = DocumentContentTypeName.Text;
            farm.WorkBoxRecordContentTypeName = RecordContentTypeName.Text;

            farm.SendErrorReportEmailsTo = SendErrorReportsTo.Text;

            farm.Update();

            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

    }
}
