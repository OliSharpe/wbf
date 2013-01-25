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
                        ReferenceDate.SelectedDate = WorkBox.ReferenceDate;
                    }
                }
            }
        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            WorkBox.ShortTitle = WorkBoxShortTitle.Text;            
            WorkBox.Web.Title = WorkBoxPrettyTitle.Text;
            WorkBox.GenerateTitle();

            if (showReferenceID)
            {
                WorkBox.ReferenceID = ReferenceID.Text;
            }

            if (showReferenceDate)
            {
                if (!ReferenceDate.IsDateEmpty)
                {
                    WorkBox.ReferenceDate = ReferenceDate.SelectedDate;
                }
            }

            WorkBox.Update();
            returnFromDialogOK("");
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }

    }
}
