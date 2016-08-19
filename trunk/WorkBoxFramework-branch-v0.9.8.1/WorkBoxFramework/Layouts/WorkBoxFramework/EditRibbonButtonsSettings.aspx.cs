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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class EditRibbonButtonsSettings : LayoutsPageBase
    {
        Dictionary<String, WBAction> actions;

        protected void Page_Load(object sender, EventArgs e)
        {
            actions = new Dictionary<String, WBAction>();
            List<String> actionKeys = WBAction.GetKeysForEditableRibbonTabButtons();
            foreach (string key in actionKeys)
            {
                actions.Add(key, new WBAction(key));
            }

            Table table = new Table();
            table.Width = Unit.Percentage(100);

            TableRow headers = new TableRow();
            headers.WBxAddTableHeaderCell("Label");
            headers.WBxAddTableHeaderCell("Icon");
            headers.WBxAddTableHeaderCell("Is Enabled?");
            headers.WBxAddTableHeaderCell("Allow Owners?");
            headers.WBxAddTableHeaderCell("Allow Involved?");
            headers.WBxAddTableHeaderCell("Allow Visitors?");
            headers.WBxAddTableHeaderCell("Is Modal?");
            headers.WBxAddTableHeaderCell("Show Close?");
            headers.WBxAddTableHeaderCell("Allow Maximise?");
            headers.WBxAddTableHeaderCell("Action URL");
            headers.WBxAddTableHeaderCell("Width");
            headers.WBxAddTableHeaderCell("Height");
            headers.WBxAddTableHeaderCell("Revert To Defaults?");

            table.Rows.Add(headers);

            foreach (WBAction action in actions.Values)
            {
                table.Rows.Add(action.CreateEditableTableRow());
            }

            EditActionsTable.Controls.Add(table);

            if (!IsPostBack)
            {
                using (WBCollection collection = new WBCollection(SPContext.Current))
                {
                    foreach (WBAction action in actions.Values)
                    {
                        string propertyValue = collection.Web.WBxGetProperty(action.PropertyKey);
                        action.SetFromPropertyValue(propertyValue);

                        action.SetControlValues();
                    }
                }

                ReturnUrl.Value = Request.QueryString["ReturnUrl"];
            }

        }

        protected void SaveChangesButton_OnClick(object sender, EventArgs e)
        {
            using (WBCollection collection = new WBCollection(SPContext.Current))
            {
                foreach (WBAction action in actions.Values)
                {
                    action.CaptureControlValues();

                    collection.SetAction(action);
                    // collection.Web.WBxSetProperty(action.PropertyKey, action.PropertyValue);
                }

                collection.Update();
            }

            if (String.IsNullOrEmpty(ReturnUrl.Value))
            {
                SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
            } else {
                SPUtility.Redirect(ReturnUrl.Value, SPRedirectFlags.Static, Context);
            }

        }


        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ReturnUrl.Value))
            {
                SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
            }
            else
            {
                SPUtility.Redirect(ReturnUrl.Value, SPRedirectFlags.Static, Context);
            }
        }




    }
}
