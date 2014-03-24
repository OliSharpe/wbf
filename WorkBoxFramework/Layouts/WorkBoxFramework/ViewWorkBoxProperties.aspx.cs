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
    public partial class ViewWorkBoxProperties : WorkBoxDialogPageBase
    {
        protected bool showReferenceID = false;
        protected bool showReferenceDate = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            WBRecordsType recordsType = WorkBox.RecordsType;

            if (recordsType.WorkBoxReferenceIDRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN)
            {
                showReferenceID = true;
            }

            if (recordsType.WorkBoxReferenceDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN)
            {
                showReferenceDate = true;
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

                if (showReferenceID)
                {
                    ReferenceID.Text = WorkBox.ReferenceID;
                }

                if (showReferenceDate)
                {
                    if (WorkBox.ReferenceDateHasValue)
                    {
                        ReferenceDate.Text = WorkBox.ReferenceDate.ToShortDateString();
                    }
                }

                WBAction editAction = WorkBox.GetAction(WBAction.ACTION_KEY__EDIT_PROPERTIES);
                EditButton.Enabled = editAction.IsEnabled;

            }
        }

        protected void editButton_OnClick(object sender, EventArgs e)
        {
            WBAction editAction = WorkBox.GetAction(WBAction.ACTION_KEY__EDIT_PROPERTIES);

            if (editAction.IsEnabled)
            {
                SPUtility.Redirect(editAction.ActionUrl, SPRedirectFlags.Trusted, Context);
            }
            else
            {
                ErrorMessageLabel.Text = "You don't have permission to edit the work box properties";
            }

            //WBLogging.Debug("Got an action URL for edit page as being: " + editAction.ActionUrl);

        }

        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogOK("");
        }

    }
}
