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

namespace WorkBoxFramework.TeamSiteWorkBoxes
{
    public partial class TeamSiteWorkBoxesUserControl : UserControl
    {
        public String NoWorkBoxesText = "Your team have no work boxes of the selected type.";
        private WBColumn sortColumn = null;
        private bool ascending = false;

        protected TeamSiteWorkBoxes webPart = default(TeamSiteWorkBoxes);

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as TeamSiteWorkBoxes;

            SelectedWorkBoxes.AllowSorting = true;
            SelectedWorkBoxes.Sorting += new GridViewSortEventHandler(SelectedWorkBoxes_Sorting);

            SelectedWorkBoxes.AllowPaging = true;
            SelectedWorkBoxes.PageIndexChanging += new GridViewPageEventHandler(SelectedWorkBoxes_PageIndexChanging);
            SelectedWorkBoxes.PagerSettings.Mode = PagerButtons.Numeric;
            SelectedWorkBoxes.PagerSettings.Position = PagerPosition.Bottom;
            SelectedWorkBoxes.PagerSettings.PageButtonCount = 30;
            SelectedWorkBoxes.PagerSettings.Visible = true;
            SelectedWorkBoxes.PageSize = 30;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            SelectedWorkBoxes.PagerTemplate = null;

            if (!IsPostBack)
            {
                String guidString = Request.QueryString["recordsTypeGUID"];

                if (!String.IsNullOrEmpty(guidString))
                {
                    SelectedRecordsTypeGUID = guidString;
                    SelectedWorkBoxView = VIEW__SELECTED_RECORDS_TYPE;
                    ViewState["SortExpression"] = "";
                }
                else if (!String.IsNullOrEmpty(webPart.InitialRecordsType))
                {
                    SelectedWorkBoxView = VIEW__SELECTED_RECORDS_TYPE;
                    ViewState["SortExpression"] = "";
                }

                


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


        protected void ViewRecentlyCreated_OnClick(object sender, EventArgs e)
        {
            SelectedWorkBoxView = VIEW__RECENTLY_CREATED;
            SelectedWorkBoxStatusFilter = "Open";

            SelectedRecordsTypeGUID = "";

            SetSortColumn(WBColumn.WorkBoxDateCreated);
            GridViewSortDirection = SortDirection.Descending;

            RefreshBoundData();
        }

        protected void ViewRecentlyModified_OnClick(object sender, EventArgs e)
        {
            SelectedWorkBoxView = VIEW__RECENTLY_MODIFIED;
            SelectedWorkBoxStatusFilter = "Open";

            SelectedRecordsTypeGUID = "";

            SetSortColumn(WBColumn.WorkBoxDateLastModified);
            GridViewSortDirection = SortDirection.Descending;

            RefreshBoundData();
        }

        protected void ViewRecentlyVisited_OnClick(object sender, EventArgs e)
        {
            SelectedWorkBoxView = VIEW__RECENTLY_VISITED;
            SelectedWorkBoxStatusFilter = "Open";

            SelectedRecordsTypeGUID = "";

            SetSortColumn(WBColumn.WorkBoxDateLastVisited);
            GridViewSortDirection = SortDirection.Descending;

            RefreshBoundData();
        }



        protected void FilterByOwns_OnClick(object sender, EventArgs e)
        {
            SelectedInvolvementFilter = FILTER_INVOLVEMENT__OWNS;
            RefreshBoundData();
        }

        protected void FilterByInvolved_OnClick(object sender, EventArgs e)
        {
            SelectedInvolvementFilter = FILTER_INVOLVEMENT__INVOLVED;
            RefreshBoundData();
        }

        protected void FilterByVisiting_OnClick(object sender, EventArgs e)
        {
            SelectedInvolvementFilter = FILTER_INVOLVEMENT__VISITING;
            RefreshBoundData();
        }

        protected void HiddenSubmitLink_OnClick(object sender, EventArgs e)
        {
            String guidString = HiddenRecordsTypeGUIDField.Value;

            ViewState["SortExpression"] = "";

            if (guidString != null && guidString != "")
            {
                SelectedRecordsTypeGUID = guidString;
                SelectedWorkBoxView = VIEW__SELECTED_RECORDS_TYPE;

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
            get
            {
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

        private const String VIEW__RECENTLY_CREATED = "Recently Created";
        private const String VIEW__RECENTLY_MODIFIED = "Recently Modified";
        private const String VIEW__RECENTLY_VISITED = "Recently Visited";
        private const String VIEW__SELECTED_RECORDS_TYPE = "Selected Records Type";

        private String SelectedWorkBoxView
        {
            get
            {
                string currentView = ViewState["WBF_SelectedWorkBoxView"].WBxToString();
                if (currentView == "")
                {
                    currentView = VIEW__RECENTLY_CREATED;
                    ViewState["WBF_SelectedWorkBoxView"] = currentView;

                    SetSortColumn(WBColumn.WorkBoxDateCreated);
                    GridViewSortDirection = SortDirection.Descending;
                }
                return currentView;
            }
            set { ViewState["WBF_SelectedWorkBoxView"] = value; }
        }

        private const String FILTER_INVOLVEMENT__OWNS = "Owns";
        private const String FILTER_INVOLVEMENT__INVOLVED = "Involved";
        private const String FILTER_INVOLVEMENT__VISITING = "Visiting";

        private String SelectedInvolvementFilter
        {
            get
            {
                string currentFilter = ViewState["WBF_SelectedInvolvementFilter"].WBxToString();
                if (currentFilter == "")
                {
                    currentFilter = FILTER_INVOLVEMENT__OWNS;
                    ViewState["WBF_SelectedInvolvementFilter"] = currentFilter;
                }
                return currentFilter;
            }
            set { ViewState["WBF_SelectedInvolvementFilter"] = value; }
        }



        private void RefreshBoundData()
        {
            if (SelectedWorkBoxView == VIEW__SELECTED_RECORDS_TYPE)
            {
                FilterOpenStatus.Enabled = true;
                FilterClosedStatus.Enabled = true;
                FilterAllStatus.Enabled = true;
            }
            else
            {
                FilterOpenStatus.Enabled = false;
                FilterClosedStatus.Enabled = false;
                FilterAllStatus.Enabled = false;
            }

            WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            WBTaxonomy teams = WBTaxonomy.GetTeams(recordsTypes);

            WBQuery query = new WBQuery();

            query.AddViewColumn(WBColumn.Title);
            query.AddViewColumn(WBColumn.WorkBoxURL);
            //query.AddViewColumn(WBColumn.OwningTeam);
            //                        query.AddViewColumn(WBColumn.FunctionalArea);
            //query.AddViewColumn(WBColumn.ReferenceDate);
            //query.AddViewColumn(WBColumn.ReferenceID);
            //query.AddViewColumn(WBColumn.SeriesTag);
            //                    query.AddViewColumn(WBColumn.InvolvedTeams);
            query.AddViewColumn(WBColumn.WorkBoxStatus);

            checkSortState();
            if (sortColumn != null)
                query.OrderBy(sortColumn, ascending);

            SelectedWorkBoxes.Columns.Clear();
            SelectedWorkBoxes.Columns.Add(WBUtils.FixedIconTemplateField(WorkBox.ICON_16_IMAGE_URL, WBColumn.WorkBoxURL));
            SelectedWorkBoxes.Columns.Add(WBUtils.HyperLinkField(WBColumn.Title, WBColumn.WorkBoxURL, sortColumn, ascending));
            //                    SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
            //                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
            // SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, HorizontalAlign.Center, sortColumn, ascending));
            //                  SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
            //                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
            //    ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.InvolvedTeams, sortColumn, ascending));

            switch (SelectedWorkBoxView)
            {
                case VIEW__RECENTLY_CREATED:
                    {
                        query.AddViewColumn(WBColumn.WorkBoxDateCreated);
                        //query.AddViewColumn(WBColumn.RecordsType);
                        SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxDateCreated, HorizontalAlign.Center, sortColumn, ascending));
                        //SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, HorizontalAlign.Center, sortColumn, ascending));

                        break;
                    }
                case VIEW__RECENTLY_MODIFIED:
                    {
                        query.AddViewColumn(WBColumn.WorkBoxDateLastModified);
                        //query.AddViewColumn(WBColumn.RecordsType);
                        SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxDateLastModified, HorizontalAlign.Center, sortColumn, ascending));
                        //SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, HorizontalAlign.Center, sortColumn, ascending));

                        break;
                    }
                case VIEW__RECENTLY_VISITED:
                    {
                        query.AddViewColumn(WBColumn.WorkBoxDateLastVisited);
                        //query.AddViewColumn(WBColumn.RecordsType);
                        SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxDateLastVisited, HorizontalAlign.Center, sortColumn, ascending));
                        //SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, HorizontalAlign.Center, sortColumn, ascending));

                        break;
                    }

                case VIEW__SELECTED_RECORDS_TYPE:
                    {
                        query.AddViewColumn(WBColumn.ReferenceDate);

                        SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, HorizontalAlign.Center, sortColumn, ascending));

                        break;
                    }
            }

            SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxStatus, HorizontalAlign.Center, sortColumn, ascending));


            WBRecordsType recordsType = null;
            WBTeam team = WBTeam.getFromTeamSite(teams, SPContext.Current.Web);

            if (team != null)
            {
                switch (SelectedInvolvementFilter)
                {
                    case FILTER_INVOLVEMENT__OWNS:
                        {
                            WBLogging.Debug("Adding a filter for OwningTeam");
                            query.AddEqualsFilter(WBColumn.OwningTeam, team);
                            break;
                        }
                    case FILTER_INVOLVEMENT__INVOLVED:
                        {
                            WBLogging.Debug("Adding a filter for InvolvedTeams");
                            query.AddEqualsFilter(WBColumn.InvolvedTeams, team);
                            break;
                        }
                    case FILTER_INVOLVEMENT__VISITING:
                        {
                            WBLogging.Debug("Adding a filter for VisitingTeams");
                            query.AddEqualsFilter(WBColumn.VisitingTeams, team);
                            break;
                        }
                }
            }

            if (SelectedWorkBoxView == VIEW__SELECTED_RECORDS_TYPE)
            {
                if (SelectedRecordsTypeGUID != "")
                {
                    recordsType = recordsTypes.GetRecordsType(new Guid(SelectedRecordsTypeGUID));
                }
                else if (!IsPostBack && !String.IsNullOrEmpty(webPart.InitialRecordsType))
                {
                    string initialRecordsTypePath = webPart.InitialRecordsType.Replace(" / ", "/").Trim();
                    recordsType = recordsTypes.GetSelectedRecordsType(initialRecordsTypePath);
                }

                if (recordsType != null)
                {
                    String recordsTypePath = recordsType.FullPath;
                    //recordsTypePath = recordsTypePath.Substring(1, recordsTypePath.Length - 1);
                    recordsTypePath = recordsTypePath.Replace("/", " / ");

                    SelectionTitle.Text = recordsTypePath;
                    SelectionDescription.Text = recordsType.Description;

                    WBQueryClause recordsTypeClause = new WBQueryClause(WBColumn.RecordsType, WBQueryClause.Comparators.Equals, recordsType);
                    recordsTypeClause.UseDescendants = false;
                    query.AddClause(recordsTypeClause);

                    string statusFilter = SelectedWorkBoxStatusFilter;
                    if (statusFilter != "All")
                    {
                        query.AddEqualsFilter(WBColumn.WorkBoxStatus, statusFilter);
                    }


                    using (WBCollection collection = new WBCollection(recordsType.WorkBoxCollectionUrl))
                    {

                        DataTable dataTable = collection.Query(query);

                        SelectedWorkBoxes.DataSource = dataTable;
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

                    }

                }
                else
                {
                    WBUtils.logMessage("SelectedRecordsTypeGUID was empty");
                }
            }
            else
            {

                String cachedDetailsListUrl = WBFarm.Local.OpenWorkBoxesCachedDetailsListUrl;

                // OK so this is a general 'recent' query
                using (SPWeb cacheWeb = SPContext.Current.Site.OpenWeb(cachedDetailsListUrl))
                {
                    SPList cacheList = cacheWeb.GetList(cachedDetailsListUrl);

                    DataTable dataTable = cacheList.WBxGetDataTable(SPContext.Current.Site, query);

                    SelectedWorkBoxes.DataSource = dataTable;
                    SelectedWorkBoxes.DataBind();

                    CreateNewWorkBoxLink.Text = "";

                    SelectionTitle.Text = SelectedWorkBoxView + " Work Boxes";
                    SelectionDescription.Text = "Select a category from left hand tree navigation to list work boxes of that type."; 
                }
            }


            // OK so now to check that the right filters are highlighted:
            ViewRecentlyCreated.CssClass = "";
            ViewRecentlyModified.CssClass = "";
            ViewRecentlyVisited.CssClass = "";

            if (SelectedWorkBoxView == VIEW__RECENTLY_CREATED) ViewRecentlyCreated.CssClass = "wbf-filter-selected";
            if (SelectedWorkBoxView == VIEW__RECENTLY_MODIFIED) ViewRecentlyModified.CssClass = "wbf-filter-selected";
            if (SelectedWorkBoxView == VIEW__RECENTLY_VISITED) ViewRecentlyVisited.CssClass = "wbf-filter-selected";

            FilterByOwns.CssClass = "";
            FilterByInvolved.CssClass = "";
            FilterByVisiting.CssClass = "";

            if (SelectedInvolvementFilter == FILTER_INVOLVEMENT__OWNS) FilterByOwns.CssClass = "wbf-filter-selected";
            if (SelectedInvolvementFilter == FILTER_INVOLVEMENT__INVOLVED) FilterByInvolved.CssClass = "wbf-filter-selected";
            if (SelectedInvolvementFilter == FILTER_INVOLVEMENT__VISITING) FilterByVisiting.CssClass = "wbf-filter-selected";

            FilterOpenStatus.CssClass = "";
            FilterClosedStatus.CssClass = "";
            FilterAllStatus.CssClass = "";

            if (SelectedWorkBoxStatusFilter == "Open") FilterOpenStatus.CssClass = "wbf-filter-selected";
            if (SelectedWorkBoxStatusFilter == "Closed") FilterClosedStatus.CssClass = "wbf-filter-selected";
            if (SelectedWorkBoxStatusFilter == "All") FilterAllStatus.CssClass = "wbf-filter-selected";

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
            String sortExpression = ViewState["SortExpression"] as String;

            sortColumn = WBColumn.GetKnownColumnByInternalName(sortExpression);

            if (GridViewSortDirection == SortDirection.Ascending)
                ascending = true;
            else
                ascending = false;
        }

        // Urm... this should be a property surely!! :)
        protected void SetSortColumn(WBColumn column)
        {
            ViewState["SortExpression"] = column.InternalName;
            sortColumn = column;
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
            set 
            { 
                ViewState["sortDirection"] = value;
                if (value == SortDirection.Ascending)
                {
                    ascending = true;
                }
                else
                {
                    ascending = false;
                }
            }
        }

    }
}
