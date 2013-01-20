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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ViewWorkBoxLibrary : LayoutsPageBase
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



                WBTeam team = WBTeam.getFromTeamSite(teams, SPContext.Current.Web);

                WBFarm farm = WBFarm.Local;

                using (WBCollection collection = new WBCollection(recordsType.WorkBoxCollectionUrl))
                {
//                    using (SPWeb web = site.OpenWeb())
  //                  {

                        WBQuery query = new WBQuery();

                        WBQueryClause recordsTypeClause = new WBQueryClause(WBColumn.RecordsType, WBQueryClause.Comparators.Equals, recordsType);
                        recordsTypeClause.UseDescendants = true;
                        query.AddClause(recordsTypeClause);

                        if (team != null)
                        {
                            query.AddEqualsFilter(WBColumn.InvolvedTeams, team);
                        }

                        string statusFilter = Request.QueryString["Status"];
                        if (statusFilter != null && statusFilter != "")
                        {
                            query.AddEqualsFilter(WBColumn.WorkBoxStatus, statusFilter);
                        }

                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.WorkBoxURL);
                        query.AddViewColumn(WBColumn.OwningTeam);
                        //                        query.AddViewColumn(WBColumn.FunctionalArea);
                        query.AddViewColumn(WBColumn.ReferenceDate);
                        query.AddViewColumn(WBColumn.ReferenceID);
                        query.AddViewColumn(WBColumn.SeriesTag);
    //                    query.AddViewColumn(WBColumn.InvolvedTeams);
                        query.AddViewColumn(WBColumn.WorkBoxStatus);

                        if (sortColumn != null)
                            query.OrderBy(sortColumn, ascending);

                        DataTable dataTable = collection.Query(query);

                        ShowResults.DataSource = dataTable;

                        ShowResults.Columns.Clear();
                        ShowResults.Columns.Add(WBUtils.FixedIconTemplateField(WorkBox.ICON_16_IMAGE_URL, WBColumn.WorkBoxURL));
                        ShowResults.Columns.Add(WBUtils.HyperLinkField(WBColumn.Title, WBColumn.WorkBoxURL, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                        //                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, HorizontalAlign.Center, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
                    //    ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.InvolvedTeams, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxStatus, sortColumn, ascending));


                        ShowResults.DataBind();

                   // }
                }
            }
            else
            {
                WBUtils.logMessage("SelectedNodePath was empty");
            }

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
            String sortExpression = ViewState["SortExpression"] as String;

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
