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
    public partial class ConfigureWorkBoxCollection : LayoutsPageBase
    {
        private WBCollection collection = null; 

        protected void Page_Init(object sender, EventArgs e)
        {
            ConfigurationSteps.WBxCreateTasksTable(WBCollection.ConfigurationStepsNames);
            if (String.IsNullOrEmpty(NextConfigurationStep.Value)) NextConfigurationStep.Value = WBCollection.ConfigurationStepsNames[0];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            collection = new WBCollection(SPContext.Current);

            if (!IsPostBack)
            {
                WorkBoxesInCollectionListName.Text = collection.ListName;
            }
        }

        protected void DoInitialConfigStep_OnClick(object sender, EventArgs e)
        {
            if (WorkBoxesInCollectionListName.Text != collection.ListName)
            {
                collection.ListName = WorkBoxesInCollectionListName.Text;
                collection.Update();
            }

            // Then start the configuration steps:
            doNextConfigurationStep();
        }


        protected void DoNextConfigStep(object sender, EventArgs e)
        {
            doNextConfigurationStep();
        }

        private void doNextConfigurationStep()
        {
            WBTaskFeedback feedback = collection.DoConfigurationStep(NextConfigurationStep.Value);

            ConfigurationSteps.WBxUpdateTask(feedback);

            if (!String.IsNullOrEmpty(feedback.NextTaskName))
            {
                NextConfigurationStep.Value = feedback.NextTaskName;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TriggerNextStepFunction", "WorkBoxFramework_triggerNextConfigurationStep();", true);
            }
            else
            {
                CancelButton.Text = "Done";
            }
        }


        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

    }
}
