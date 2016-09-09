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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace WorkBoxFramework.TeamSiteTreeSelection
{
    public partial class TeamSiteTreeSelectionUserControl : UserControl
    {
        public String NoWorkBoxesText = "Your team have no work boxes of the selected type.";
        private WBColumn sortColumn = null;
        private bool ascending = false;

        protected void Page_Load(object sender, EventArgs e)
        {


            SelectedWorkBoxes.AllowSorting = true;
            SelectedWorkBoxes.Sorting += new GridViewSortEventHandler(SelectedWorkBoxes_Sorting);

            SelectedWorkBoxes.AllowPaging = true;
            SelectedWorkBoxes.PageIndexChanging += new GridViewPageEventHandler(SelectedWorkBoxes_PageIndexChanging);
            SelectedWorkBoxes.PagerSettings.Mode = PagerButtons.Numeric;
            SelectedWorkBoxes.PagerSettings.Position = PagerPosition.Bottom;
            SelectedWorkBoxes.PagerSettings.PageButtonCount = 10;
            SelectedWorkBoxes.PagerSettings.Visible = true;
            SelectedWorkBoxes.PageSize = 10;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            SelectedWorkBoxes.PagerTemplate = null;

            if (!IsPostBack)
            {

                SelectedRecordsTypeGUID = Request.QueryString["recordsTypeGUID"];
                WBLogging.Debug("Not in post back so setting guid value to be: " + SelectedRecordsTypeGUID);
                RefreshBoundData();
            }

        }


        protected void FilterOpenStatus_OnClick(object sender, EventArgs e)
        {
            SelectedWorkBoxStatusFilter = "Open";
            RefreshBoundData();
        }

        protected void FilterClosedStatus_OnClick(object sender, EventArgs e)
        {
            SelectedWorkBoxStatusFilter = "Closed";
            RefreshBoundData();
        }

        protected void FilterAllStatus_OnClick(object sender, EventArgs e)
        {
            SelectedWorkBoxStatusFilter = "All";
            RefreshBoundData();
        }


        protected void HiddenSubmitLink_OnClick(object sender, EventArgs e)
        {
            String guidString = HiddenRecordsTypeGUIDField.Value;
            if (guidString != null && guidString != "")
            {
                SelectedRecordsTypeGUID = guidString;

                RefreshBoundData();
            }
            else
            {
                SelectedRecordsTypeGUID = "";
                WBLogging.Generic.Unexpected("HiddenSubmitLink_OnClick: Had a submit with not set GUID value.");
            }

        }


        private String SelectedRecordsTypeGUID
        {
            get { return ViewState["WBF_SelectedRecordsTypeGUID"].WBxToString(); }
            set { ViewState["WBF_SelectedRecordsTypeGUID"] = value; }
        }

        private String SelectedWorkBoxStatusFilter
        {
            get { 
                string currentStatusFilter = ViewState["WBF_SelectedWorkBoxStatusFilter"].WBxToString();
                if (currentStatusFilter == "")
                {
                    currentStatusFilter = "Open";
                    ViewState["WBF_SelectedWorkBoxStatusFilter"] = currentStatusFilter;
                }
                return currentStatusFilter;
            }
            set { ViewState["WBF_SelectedWorkBoxStatusFilter"] = value; }
        }


        
        private void RefreshBoundData()
        {
            if (!String.IsNullOrEmpty(SelectedRecordsTypeGUID))
            {
                WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
                WBTaxonomy teams = WBTaxonomy.GetTeams(recordsTypes);

                WBRecordsType recordsType = recordsTypes.GetRecordsType(new Guid(SelectedRecordsTypeGUID));

                String recordsTypePath = recordsType.FullPath;
                //recordsTypePath = recordsTypePath.Substring(1, recordsTypePath.Length - 1);
                recordsTypePath = recordsTypePath.Replace("/", " / ");

                SelectionTitle.Text = recordsTypePath;
                SelectionDescription.Text = recordsType.Description;

                WBTeam team = WBTeam.GetFromTeamSite(teams, SPContext.Current.Web);

                WBFarm farm = WBFarm.Local;

                using (WBCollection collection = new WBCollection(recordsType.WorkBoxCollectionUrl))
                {
                    //                    using (SPWeb web = site.OpenWeb())
                    //                  {

                    WBQuery query = new WBQuery();

                    WBQueryClause recordsTypeClause = new WBQueryClause(WBColumn.RecordsType, WBQueryClause.Comparators.Equals, recordsType);
                    recordsTypeClause.UseDescendants = false;
                    query.AddClause(recordsTypeClause);

                    if (team != null)
                    {
                        query.AddEqualsFilter(WBColumn.InvolvedTeams, team);
                    }

                    string statusFilter = SelectedWorkBoxStatusFilter;
                    if (statusFilter == null || statusFilter == "") statusFilter = "Open";
                    if (statusFilter != "All")
                    {
                        query.AddEqualsFilter(WBColumn.WorkBoxStatus, statusFilter);
                    }

                    FilterOpenStatus.CssClass = "wbf-unselected-filter";
                    FilterClosedStatus.CssClass = "wbf-unselected-filter";
                    FilterAllStatus.CssClass = "wbf-unselected-filter";

                    if (statusFilter == "Open") FilterOpenStatus.CssClass = "wbf-selected-filter";
                    if (statusFilter == "Closed") FilterClosedStatus.CssClass = "wbf-selected-filter";
                    if (statusFilter == "All") FilterAllStatus.CssClass = "wbf-selected-filter";


                    query.AddViewColumn(WBColumn.Title);
                    query.AddViewColumn(WBColumn.WorkBoxURL);
                    //query.AddViewColumn(WBColumn.OwningTeam);
                    //                        query.AddViewColumn(WBColumn.FunctionalArea);
                    query.AddViewColumn(WBColumn.ReferenceDate);
                    //query.AddViewColumn(WBColumn.ReferenceID);
                    //query.AddViewColumn(WBColumn.SeriesTag);
                    //                    query.AddViewColumn(WBColumn.InvolvedTeams);
                    query.AddViewColumn(WBColumn.WorkBoxStatus);

                    if (sortColumn != null)
                        query.OrderBy(sortColumn, ascending);

                    DataTable dataTable = collection.Query(query);

                    SelectedWorkBoxes.DataSource = dataTable;

                    SelectedWorkBoxes.Columns.Clear();
                    SelectedWorkBoxes.Columns.Add(WBUtils.FixedIconTemplateField(WorkBox.ICON_16_IMAGE_URL, WBColumn.WorkBoxURL));
                    SelectedWorkBoxes.Columns.Add(WBUtils.HyperLinkField(WBColumn.Title, WBColumn.WorkBoxURL, sortColumn, ascending));
//                    SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                    //                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
                    SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, HorizontalAlign.Center, sortColumn, ascending));
  //                  SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
    //                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
                    //    ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.InvolvedTeams, sortColumn, ascending));
                    SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxStatus, sortColumn, ascending));


                    SelectedWorkBoxes.DataBind();



                    if (recordsType.CanCurrentUserCreateWorkBoxForTeam(collection, team))
                    {
                        string createNewURL = collection.GetUrlForNewDialog(recordsType, team);
                        string createNewText = recordsType.CreateNewWorkBoxText;

                        CreateNewWorkBoxLink.Text = "<a href=\"#\" onclick=\"javascript: WorkBoxFramework_commandAction('" + createNewURL + "', 730, 800);\">" + createNewText + "</a>";
                    }
                    else
                    {
                        CreateNewWorkBoxLink.Text = "";
                    }



                    // }
                }
            }
            else
            {
                WBUtils.logMessage("SelectedRecordsTypeGUID was empty");
            }

        }

        void SelectedWorkBoxes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            WBLogging.Debug("In SelectedWorkBoxes_PageIndexChanging - not sure if there's anything that needs to be done!");

            SelectedWorkBoxes.PageIndex = e.NewPageIndex;

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


        protected void SelectedWorkBoxes_Sorting(object sender, GridViewSortEventArgs e)
        {
            WBLogging.Debug("In SelectedWorkBoxes_Sorting with e.SortExpression = " + e.SortExpression);

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
            SelectedWorkBoxes.PageIndex = 0;

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
