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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace WorkBoxFramework.RelatedMeetings
{
    public partial class RelatedMeetingsUserControl : UserControl
    {
        protected RelatedMeetings webPart = default(RelatedMeetings);

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as RelatedMeetings;

            if (webPart.MeetingsWorkBoxCollectionURL == null || webPart.MeetingsWorkBoxCollectionURL == "")
            {
                CreateNewMeetingLink.Text = "(Web part not configured yet)";
                return;
            }

            using (WorkBox workBox = new WorkBox(SPContext.Current))
            {
                if (workBox.LinkedWorkBoxesList == null)
                {
                    CreateNewMeetingLink.Text = "(Work Box doesn't have a related items list)";
                    return;
                }

                DataTable dataTable = createDataTable();
                addRelatedWorkBoxesToDataTable(workBox.LinkedWorkBoxesList, dataTable);

                RelatedMeetings.DataSource = dataTable;
                RelatedMeetings.DataBind();

                using (WBCollection collection = new WBCollection(webPart.MeetingsWorkBoxCollectionURL))
                {
                    string createNewURL = collection.GetUrlForNewDialog(workBox, WorkBox.RELATION_TYPE__CHILD);
                    string createNewText = "Create New Meeting"; // collection.CreateNewWorkBoxText;

                    CreateNewMeetingLink.Text = "<a href=\"#\" onclick=\"javascript: WorkBoxFramework_commandAction('" + createNewURL + "', 600, 500);\">" + createNewText + "</a>";
                }
            }
        }


        private void addRelatedWorkBoxesToDataTable(SPList relatedWorkBoxesList, DataTable dataTable)
        {
            if (relatedWorkBoxesList == null) return;

            foreach (SPListItem item in relatedWorkBoxesList.Items)
            {
                dataTable.Rows.Add("/_layouts/images/WorkBoxFramework/work-box-16.png",
                    item["Title"],
                    item[WorkBox.COLUMN_NAME__WORK_BOX_URL],
                    item[WorkBox.COLUMN_NAME__REFERENCE_DATE]);
            }
        }

        private DataTable createDataTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Icon", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("URL", typeof(string));
            table.Columns.Add(WorkBox.COLUMN_NAME__REFERENCE_DATE, typeof(DateTime));

            return table;
        }

    }
}
