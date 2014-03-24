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

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class CheckConfiguration : LayoutsPageBase
    {
        public bool doingInitialSetup = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WBFarm farm = WBFarm.Local;

                if (!String.IsNullOrEmpty(farm.TermStoreName)) {
                    try
                    {
                        WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);

                        if (teams == null) WBLogging.Debug("teams was null");

                        WBTeam systemAdminTeam = farm.SystemAdminTeam(teams);

                        if (systemAdminTeam != null)
                        {
                            AdminTeamSiteURL.Text = systemAdminTeam.TeamSiteUrl;
                        }
                    }
                    catch (Exception exception)
                    {
                        WBLogging.Generic.HighLevel("Couldn't find the current admin team site URL: " + exception.Message);
                    }
                }

                TimerJobsServerName.Text = farm.TimerJobsServerName;

                TermStoreName.Text = farm.TermStoreName;
                TermStoreGroupName.Text = farm.TermStoreGroupName;

                DocumentContentTypeName.Text = farm.WorkBoxDocumentContentTypeName;
                RecordContentTypeName.Text = farm.WorkBoxRecordContentTypeName;

            }
        }

        protected void DoInitialSetup_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("AdminTeamSiteURL: " + AdminTeamSiteURL.Text);
            WBLogging.Debug("TimerJobsServerName: " + TimerJobsServerName.Text);
            WBLogging.Debug("TimerJobsServerName to upper: " + TimerJobsServerName.Text.ToUpper());

            WBFarm farm = WBFarm.Local;

            farm.TermStoreName = TermStoreName.Text;
            farm.TermStoreGroupName = TermStoreGroupName.Text;

            farm.WorkBoxDocumentContentTypeName = DocumentContentTypeName.Text;
            farm.WorkBoxRecordContentTypeName = RecordContentTypeName.Text;

            farm.Update();

            WBFarm.Local.InitialFarmSetup(SPContext.Current.Site, AdminTeamSiteURL.Text, TimerJobsServerName.Text.ToUpper());

            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }



        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

    }
}
