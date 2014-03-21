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
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;


namespace WorkBoxFramework.DisplaySelectedTeamRecords
{
    public partial class DisplaySelectedTeamRecordsUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the selected parameters:
            string recordsTypeGUID = Request.Params["recordsTypeGUID"];
            string selectedWorkBoxCollectionURL = Request.Params["workBoxCollectionURL"];
            string includeDocumentsFlag = Request.Params["includeDocuments"];

            WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);
            WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(teams);

            WBTeam team = WBTeam.GetFromTeamSite(teams, SPContext.Current);

            if (recordsTypeGUID == null
                || recordsTypeGUID == ""
                || selectedWorkBoxCollectionURL == null
                || selectedWorkBoxCollectionURL == ""
                || team == null
                )
            {
                InformationText.Text = "<span>Make a selection from the left to see work boxes of that type.</span>";
            }
            else
            {


            Guid teamsTermGuid = team.Id;
            string teamsGUID = team.Id.ToString();

            // Process the parameters:
            bool includeDocumentRecords = false;
            if (includeDocumentsFlag != null && includeDocumentsFlag != "")
            {
                includeDocumentRecords = includeDocumentsFlag.Equals(true.ToString());
            }

            WBRecordsType recordsType = null;
            Guid selectedRecordsTypeTermGUID = new Guid(recordsTypeGUID);
            recordsType = recordsTypes.GetRecordsType(selectedRecordsTypeTermGUID);

            string infoText = "<div class='wbf-view-selected-records-type'>You have selected to view: <span 'wbf-records-type-name'>" + recordsType.Name + "</span></div>\n";

            if (recordsType.Description != "")
            {
                infoText += "<div class='wbf-records-type-description'>" + recordsType.Description + "</div>";
            }

            InformationText.Text = infoText;


            WBUtils.logMessage("Found the records type info: " + recordsType.Name);

                DataTable combinedData = createCombinedDataTable();

                if (includeDocumentRecords)
                {
                    WBUtils.logMessage("Records Library IS being included in search");

                    WBFarm farm = WBFarm.Local;

               //     SPListItemCollection docResults = getResultsForList(farm.RecordsCenterUrl, farm.RecordsCenterRecordsLibraryName, team, recordsType); 

                 //   addDocResultsToCombinedData(farm.RecordsCenterUrl, docResults, combinedData);
                }
                else
                {
                    WBUtils.logMessage("Records Library is not being included in search");
                }

                if (selectedWorkBoxCollectionURL != "")
                {
                    using (WBCollection collection = new WBCollection(selectedWorkBoxCollectionURL))
                    {

                        WBUtils.logMessage("A work box colleciton IS being included in search: " + selectedWorkBoxCollectionURL);
                        SPListItemCollection workBoxResults = collection.QueryFilteredBy(team, recordsType, true);

                        WBUtils.logMessage("Got back from query this num of results: " + workBoxResults.Count);

                        addWorkBoxResultsToCombinedData(collection, workBoxResults, combinedData);

                        if (recordsType.CanCurrentUserCreateWorkBoxForTeam(collection, team))
                        {
                            string createNewURL = collection.GetUrlForNewDialog(recordsType, team);
                            string createNewText = recordsType.CreateNewWorkBoxText;

                            CreateNewWorkBoxLink.Text = "<a href=\"#\" onclick=\"javascript: WorkBoxFramework_commandAction('" + createNewURL + "', 730, 800);\">" + createNewText + "</a>";
                        }
                    }
                }
                else
                {
                    WBUtils.logMessage("No work box colleciton is being included in search");
                }

                ShowCombinedResults.DataSource = combinedData;
                ShowCombinedResults.DataBind();

            }
        }

        private void addDocResultsToCombinedData(String urlPrefix, SPListItemCollection docResults, DataTable combinedData)
        {
            if (docResults == null) return;

            foreach (SPListItem item in docResults)
            {
                combinedData.Rows.Add("/_layouts/images/icdocx.png", item.Name, urlPrefix + item.Url);
            }
        }

        private void addWorkBoxResultsToCombinedData(WBCollection collection, SPListItemCollection workBoxResults, DataTable combinedData)
        {
            if (workBoxResults == null) return;

            foreach (SPListItem item in workBoxResults)
            {
                using (WorkBox workBox = new WorkBox(collection, item))
                {
                    if (workBox.Status == WorkBox.WORK_BOX_STATUS__OPEN)
                    {
                        combinedData.Rows.Add("/_layouts/images/WorkBoxFramework/work-box-16.png", workBox.Title, workBox.Url, workBox.RecordsType.Name, workBox.DateCreated, workBox.Status);
                    }
                }
            }
        }


        private DataTable createCombinedDataTable()
        {
            DataTable table = new DataTable();


            table.Columns.Add("Icon", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("URL", typeof(string));
            table.Columns.Add("RecordsType", typeof(string));
            table.Columns.Add("WorkBoxDateCreated", typeof(DateTime));
            table.Columns.Add("WorkBoxStatus", typeof(string));

            return table;
        }

        private SPListItemCollection getResultsForList(string webURL, string listName, WBTeam team, WBRecordsType recordsType)
        {
            WBUtils.logMessage("Getting results for" +  listName + " within " + webURL);
            using (SPSite site = new SPSite(webURL))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[listName];

                    SPQuery query = site.WBxMakeCAMLQueryFilterBy(team, recordsType, true);

                    return list.GetItems(query);
                }
            }

        }

        private SPListItemCollection getResultsForWorkBoxCollection(string workBoxCollectionURL, WBTeam team, WBRecordsType recordsType)
        {
            WBUtils.logMessage("Getting results for WBCollection: " + workBoxCollectionURL);

            using (WBCollection collection = new WBCollection(workBoxCollectionURL))
            {
                return collection.QueryFilteredBy(team, recordsType, true);
            }

        }



    }
}
