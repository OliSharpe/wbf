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
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class InviteTeams : WorkBoxDialogPageBase
    {
        public String controlID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            WBUtils.logMessage("In Page_Load for the public doc metadata dialog");

            if (IsPostBack)
            {
                if (InvolvedTeamsField.Text.Equals(""))
                {
                    InvolvedTeamsFieldMessage.Text = "You must enter at least one involved team.";
                    validValues = false;
                }

                if (validValues) pageRenderingRequired = false;
            }

            checkForErrors();

            if (pageRenderingRequired)
            {

                WorkBox.Teams.InitialiseTaxonomyControl(InvolvedTeamsField, WorkBox.COLUMN_NAME__INVOLVED_TEAMS, true);
                WorkBox.Teams.InitialiseTaxonomyControl(VisitingTeamsField, WorkBox.COLUMN_NAME__VISITING_TEAMS, true);

                controlID = InvolvedTeamsField.ClientID;

                if (!IsPostBack)
                {
                    InvolvedTeamsField.Text = WorkBox.InvolvedTeams.UIControlValue;
                    VisitingTeamsField.Text = WorkBox.VisitingTeams.UIControlValue;
                }

                ErrorMessageLabel.Text = errorMessage;
                DisposeWorkBox();
            }
                 
        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            // The event should only be processed if there is no other need to render the page again
            if (!pageRenderingRequired)
            {
                // Now to save the current value of the Involved Teams field:

                WorkBox.InvolvedTeams = new WBTermCollection<WBTeam>(WorkBox.Teams, InvolvedTeamsField.Text);
                WorkBox.VisitingTeams = new WBTermCollection<WBTeam>(WorkBox.Teams, VisitingTeamsField.Text);

                WorkBox.Update();

                DisposeWorkBox();

                //returnFromDialogOK("The involved teams have been updated");
                returnFromDialogOKAndRefresh();
            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            returnFromDialogCancel("Inviting of teams was cancelled");
        }


    }
}
