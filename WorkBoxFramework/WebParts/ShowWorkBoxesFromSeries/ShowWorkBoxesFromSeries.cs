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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.ShowWorkBoxesFromSeries
{
    [ToolboxItemAttribute(false)]
    public class ShowWorkBoxesFromSeries : WebPart
    {

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Filter By Same Series Tag")]
        [WebDescription("Only include Work Boxes if they have the same Series Tag as this work box.")]
        [System.ComponentModel.Category("Configuration")]
        public bool FilterBySeriesTag { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Filter By Same Reference ID")]
        [WebDescription("Only include Work Boxes if they have the same Reference ID as this work box.")]
        [System.ComponentModel.Category("Configuration")]
        public bool FilterByReferenceID { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Filter By Same Owning Team")]
        [WebDescription("Only include Work Boxes if they have the same Owning Team as this work box.")]
        [System.ComponentModel.Category("Configuration")]
        public bool FilterByOwningTeam { get; set; }


        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show Create New Link")]
        [WebDescription("Should the web part show a link to create a new work box of the same records type as this work box?")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowCreateNewLink { get; set; }

        private Literal errorLiteral = null;
        private SPGridView gridView = null;
        private Literal createNewLink = null;
        private WBColumn sortColumn = null;
        private bool ascending = false;

        protected override void CreateChildControls()
        {
            errorLiteral = new Literal();
            this.Controls.Add(errorLiteral);

            gridView = new SPGridView();
            gridView.AutoGenerateColumns = false;

            gridView.AllowSorting = true;
            gridView.Sorting += new GridViewSortEventHandler(gridView_Sorting);

            gridView.AllowPaging = true;
            gridView.PageIndex = 0;
            gridView.PageIndexChanging += new GridViewPageEventHandler(gridView_PageIndexChanging);
            gridView.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            gridView.PagerSettings.Position = PagerPosition.Bottom;
            gridView.PagerSettings.PageButtonCount = 10;
            gridView.PagerSettings.Visible = true;
            gridView.PageSize = 10;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            gridView.PagerTemplate = null;


            Panel gridPanel = new Panel();
            gridPanel.Controls.Add(gridView);
            this.Controls.Add(gridPanel);

            if (ShowCreateNewLink)
            {
                Panel createNewPanel = new Panel();
                createNewLink = new Literal();

                createNewPanel.Controls.Add(createNewLink);

                this.Controls.Add(createNewPanel);
            }

        }

        protected override void Render(HtmlTextWriter writer)
        {
            WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);

            if (workBox == null) errorLiteral.Text = "<i>(You can only use this web part in a work box)</i>";
            else
            {
                WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
                WBTaxonomy seriesTags = WBTaxonomy.GetSeriesTags(recordsTypes);

                WBTerm thisSeries = workBox.SeriesTag(seriesTags);

                WBQuery query = new WBQuery();

                query.AddEqualsFilter(WBColumn.RecordsType, workBox.RecordsType);

                if (FilterBySeriesTag)
                    query.AddEqualsFilter(WBColumn.SeriesTag, workBox.SeriesTag(seriesTags));

                if (FilterByReferenceID)
                    query.AddEqualsFilter(WBColumn.ReferenceID, workBox.ReferenceID);

                //            if (FilterByOwningTeam) 
                //            query.AddEqualsFilter(WBColumn.OwningTeam, workBox.OwningTeam);


                if (sortColumn != null) WBLogging.Debug("Sorting in Render with sortColumn: " + sortColumn.DisplayName);
                else WBLogging.Debug("Sorting Render - sortColumn was null");

                if (sortColumn != null)
                {
                    query.OrderBy(sortColumn, ascending);
                }

                query.AddViewColumn(WBColumn.Title);
                query.AddViewColumn(WBColumn.WorkBoxURL);
                query.AddViewColumn(WBColumn.ReferenceDate);
                query.AddViewColumn(WBColumn.WorkBoxStatus);

                WBColumn testIfIsThisWorkBox = new WBColumn("IfIsThisWorkBox", WBColumn.DataTypes.VirtualConditional);
                testIfIsThisWorkBox.TestColumnInternalName = WBColumn.Title.InternalName;
                testIfIsThisWorkBox.TestColumnValue = workBox.Title;
                testIfIsThisWorkBox.ValueIfEqual = "===>";

                query.AddViewColumn(testIfIsThisWorkBox);

                DataTable dataTable = workBox.Collection.Query(query);

                gridView.DataSource = new DataView(dataTable);

                BoundField testIfIsThisWorkBoxField = WBUtils.BoundField(testIfIsThisWorkBox, HorizontalAlign.Center, sortColumn, ascending);
                testIfIsThisWorkBoxField.HeaderText = "     ";

                gridView.Columns.Add(testIfIsThisWorkBoxField);
                gridView.Columns.Add(WBUtils.FixedIconTemplateField(WorkBox.ICON_16_IMAGE_URL, WBColumn.WorkBoxURL));
                gridView.Columns.Add(WBUtils.HyperLinkField(WBColumn.Title, WBColumn.WorkBoxURL, sortColumn, ascending));
                gridView.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));
                gridView.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxStatus, HorizontalAlign.Center, sortColumn, ascending));

                gridView.DataBind();


                if (ShowCreateNewLink)
                {
                    string createNewText = workBox.RecordsType.CreateNewWorkBoxText;
                    string createNewURL = workBox.Collection.GetUrlForNewDialog(workBox, WorkBox.RELATION_TYPE__DYNAMIC);

                    createNewLink.Text = "<a href=\"#\" onclick=\"javascript: WorkBoxFramework_commandAction('" + createNewURL + "', 600, 500); \">" + createNewText + "</a>";
                }

                workBox.Dispose();
            }

            base.Render(writer);
        }

        void gridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            WBLogging.Debug("In gridView_PageIndexChanging - not sure if there's anything that needs to be done!");

            gridView.PageIndex = e.NewPageIndex;

            checkSortState();
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

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
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
            gridView.PageIndex = 0;
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
