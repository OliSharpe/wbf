#region Copyright and License

// Copyright (c) Islington Council 2010-2015
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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class CheckConfiguration : LayoutsPageBase
    {
        public bool doingInitialSetup = true;

        protected void Page_Init(object sender, EventArgs e)
        {
            ConfigurationSteps.WBxCreateConfigurationStepsTable(WBFarm.ConfigurationStepsNames);
            if (String.IsNullOrEmpty(NextConfigurationStep.Value)) NextConfigurationStep.Value = WBFarm.ConfigurationStepsNames[0];
        }

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

                if (!String.IsNullOrEmpty(farm.TimerJobsManagementSiteUrl))
                {
                    DoInitialConfigStep.Text = "Check Configuration";
                }
            }
        }

        protected void DoInitialConfigStep_OnClick(object sender, EventArgs e)
        {
            // First save the values set within the form to the farm object:
            WBFarm farm = WBFarm.Local;

            farm.SystemAdminTeamSiteUrl = AdminTeamSiteURL.Text;
            farm.TimerJobsServerName = TimerJobsServerName.Text.ToUpper();

            farm.TermStoreName = TermStoreName.Text;
            farm.TermStoreGroupName = TermStoreGroupName.Text;

            farm.WorkBoxDocumentContentTypeName = DocumentContentTypeName.Text;
            farm.WorkBoxRecordContentTypeName = RecordContentTypeName.Text;

            farm.Update();

            WBLogging.Config.Unexpected("farm.SystemAdminTeamSiteUrl = " + farm.SystemAdminTeamSiteUrl);

            // Then start the configuration steps:
            doNextConfigurationStep();

            // old code:
            // WBFarm.Local.InitialFarmSetup(SPContext.Current.Site, AdminTeamSiteURL.Text, TimerJobsServerName.Text.ToUpper());
        }


        protected void DoNextConfigStep(object sender, EventArgs e)
        {
            doNextConfigurationStep();
        }

        private void doNextConfigurationStep()
        {
            WBConfigStepFeedback feedback = WBFarm.Local.DoConfigurationStep(NextConfigurationStep.Value);

            ConfigurationSteps.WBxUpdateConfigurationStep(feedback);

            if (!String.IsNullOrEmpty(feedback.NextStepName))
            {
                NextConfigurationStep.Value = feedback.NextStepName;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TriggerNextStepFunction", "WorkBoxFramework_triggerNextConfigurationStep();", true);
            }
            else
            {
                CancelButton.Text = "Done";
            }
        }


        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

    }
}
