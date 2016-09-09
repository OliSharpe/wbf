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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ViewRecordsLibrary : LayoutsPageBase
    {
        private WBColumn sortColumn = null;
        private bool ascending = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            TreeViewTermCollection collection = new TreeViewTermCollection();
            collection.Add(new TreeViewTerm(recordsTypes.TermSet));

            PickRecordsTypeTreeView.DataSource = collection;
            PickRecordsTypeTreeView.DataBind();


            ShowResults.AllowSorting = true;
            ShowResults.Sorting += new GridViewSortEventHandler(ShowResults_Sorting);

            ShowResults.AllowPaging = true;
            ShowResults.PageIndexChanging += new GridViewPageEventHandler(ShowResults_PageIndexChanging);
            ShowResults.PagerSettings.Mode = PagerButtons.Numeric;
            ShowResults.PagerSettings.Position = PagerPosition.Bottom;
            ShowResults.PagerSettings.PageButtonCount = 10;
            ShowResults.PagerSettings.Visible = true;
            ShowResults.PageSize = 10;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            ShowResults.PagerTemplate = null;


        }

        private String SelectedNodePath
        {
            get { return ViewState["WBF_SelectedNodePath"].WBxToString(); }
            set { ViewState["WBF_SelectedNodePath"] = value; } 
        }

        protected void PickRecordsTypeTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (PickRecordsTypeTreeView.SelectedNode != null)
            {
                SelectedNodePath = PickRecordsTypeTreeView.SelectedNode.ValuePath;
            }
            else
            {
                SelectedNodePath = "";
            }

            RefreshBoundData();
        }


        private void RefreshBoundData()
        {
            if (SelectedNodePath != "")
            {
                SelectedRecordsType.Text = SelectedNodePath.Replace("Records Types/", "").Replace("/", " / ");
                        
                WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);                     
                WBTaxonomy teams = WBTaxonomy.GetTeams(recordsTypes);

                WBRecordsType recordsType = recordsTypes.GetSelectedRecordsType(SelectedNodePath);
                SelectedRecordsTypeDescription.Text = recordsType.Description;

                WBTeam team = WBTeam.GetFromTeamSite(teams, SPContext.Current.Web);

                WBFarm farm = WBFarm.Local;

                using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        WBQuery query = new WBQuery();

                        WBQueryClause recordsTypeClause = new WBQueryClause(WBColumn.RecordsType, WBQueryClause.Comparators.Equals, recordsType);
                        recordsTypeClause.UseDescendants = true;
                        query.AddClause(recordsTypeClause);

                        if (team != null)
                        {
                            query.AddEqualsFilter(WBColumn.InvolvedTeams, team);
                        }

                        string protectiveZoneFilter = Request.QueryString["ProtectiveZone"];
                        if (protectiveZoneFilter != null && protectiveZoneFilter != "")
                        {
                            query.AddEqualsFilter(WBColumn.ProtectiveZone, protectiveZoneFilter);
                        }


                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.FileTypeIcon);
                        query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
                        query.AddViewColumn(WBColumn.FunctionalArea);
                        query.AddViewColumn(WBColumn.OwningTeam);
                        query.AddViewColumn(WBColumn.ReferenceDate);
                        query.AddViewColumn(WBColumn.ReferenceID);
                        query.AddViewColumn(WBColumn.SeriesTag);
                        query.AddViewColumn(WBColumn.ProtectiveZone);

                        if (sortColumn != null) 
                            query.OrderBy(sortColumn, ascending);

                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl);

                        DataTable dataTable = recordsLibrary.WBxGetDataTable(site, query); 

                        ShowResults.DataSource = dataTable;

                        ShowResults.Columns.Clear();
                        ShowResults.Columns.Add(WBUtils.DynamicIconTemplateField(WBColumn.FileTypeIcon, WBColumn.EncodedAbsoluteURL));
                        ShowResults.Columns.Add(WBUtils.HyperLinkField(WBColumn.Name, WBColumn.EncodedAbsoluteURL, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ProtectiveZone, sortColumn, ascending));


                        ShowResults.DataBind();

                    }
                }
            }
            else
            {
                WBUtils.logMessage("SelectedNodePath was empty");
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


        private void addWorkBoxResultsToCombinedData(SPListItemCollection workBoxResults, DataTable combinedData)
        {
            if (workBoxResults == null) return;

            foreach (SPListItem item in workBoxResults)
            {
                combinedData.Rows.Add("/_layouts/images/WorkBoxFramework/work-box-16.png", item.Title, item[WorkBox.COLUMN_NAME__WORK_BOX_URL]);
            }
        }

        private DataTable createCombinedDataTable()
        {
            DataTable table = new DataTable();


            table.Columns.Add("Icon", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("URL", typeof(string));

            return table;
        }




        void ShowResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            WBLogging.Debug("In gridView_PageIndexChanging - not sure if there's anything that needs to be done!");

            ShowResults.PageIndex = e.NewPageIndex;

            checkSortState();
            RefreshBoundData();
        }


        private void checkSortState()
        {
            String sortExpression = ViewState["SortExpression"].WBxToString();

            sortColumn = WBColumn.GetKnownColumnByInternalName(sortExpression);

            if (GridViewSortDirection == SortDirection.Ascending)
                ascending = true;
            else
                ascending = false;

        }

        protected void ShowResults_Sorting(object sender, GridViewSortEventArgs e)
        {
            WBLogging.Debug("In gridView_Sorting with e.SortExpression = " + e.SortExpression);

            string sortExpression = e.SortExpression;
            ViewState["SortExpression"] = sortExpression;

            sortColumn = WBColumn.GetKnownColumnByInternalName(sortExpression);

            if (GridViewSortDirection == SortDirection.Ascending)
            {
                GridViewSortDirection = SortDirection.Descending;
                ascending = false;
                WBLogging.Debug("In gridView_Sorting setting to descending");
            }
            else
            {
                GridViewSortDirection = SortDirection.Ascending;
                ascending = true;
                WBLogging.Debug("In gridView_Sorting setting to ascending");
            }

            // If we're re-sorting the data let's start back on page 0:
            ShowResults.PageIndex = 0;

            RefreshBoundData();
        }

        private SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Descending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }


    }
}
