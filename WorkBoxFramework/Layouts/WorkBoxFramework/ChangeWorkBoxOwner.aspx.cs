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

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ChangeWorkBoxOwner : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
                if (OwningTeamField.Text.Equals(""))
                {
                    OwningTeamFieldMessage.Text = "You must select a new owning team.";
                    validValues = false;
                }

                if (validValues) pageRenderingRequired = false;
            }

            checkForErrors();

            if (pageRenderingRequired)
            {

                WorkBoxTitle.Text = WorkBox.Title;

                WorkBox.Teams.InitialiseTaxonomyControl(OwningTeamField, "Select New Owning Team", false);

                if (!IsPostBack)
                {
                    OwningTeamField.Text = WorkBox.OwningTeam.UIControlValue;
                }

                ErrorMessageLabel.Text = errorMessage;
                DisposeWorkBox();
            }

        }

        protected void changeOwnerButton_OnClick(object sender, EventArgs e)
        {
            // The event should only be processed if there is no other need to render the page again
            if (!pageRenderingRequired)
            {
                // Now to save the current value of the Involved Teams field:

                WorkBox.OwningTeam = new WBTeam(WorkBox.Teams, OwningTeamField.Text);

                WorkBox.AuditLogEntry("Changed owner", "Owning team: " + WorkBox.OwningTeam.Name);
                WorkBox.Update();

                DisposeWorkBox();

                CloseDialogAndRefresh();
            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            CloseDialogWithCancel("Inviting of teams was cancelled");
        }

    }
}
