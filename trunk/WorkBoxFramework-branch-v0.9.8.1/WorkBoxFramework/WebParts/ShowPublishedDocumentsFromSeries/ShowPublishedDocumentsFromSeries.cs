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
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ShowPublishedDocumentsFromSeries
{
    [ToolboxItemAttribute(false)]
    public class ShowPublishedDocumentsFromSeries : WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Series Tag")]
        [WebDescription("Only include documents that have this series tag.")]
        [System.ComponentModel.Category("Configuration")]
        public String SeriesTag { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Reference ID")]
        [WebDescription("Only include documents if they have this Reference ID.")]
        [System.ComponentModel.Category("Configuration")]
        public String ReferenceID { get; set; }


        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Number of Documents")]
        [WebDescription("Maximum number of documents to show.")]
        [System.ComponentModel.Category("Configuration")]
        public int MaxNumDocuments { get; set; }

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
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            WBTaxonomy seriesTags = WBTaxonomy.GetSeriesTags(SPContext.Current.Site);

            WBTerm seriesTag = null;

            if (SeriesTag != null && SeriesTag != "")
            {
                Term seriesTagTerm = seriesTags.GetSelectedTermByPath(SeriesTag);
                if (seriesTagTerm != null)
                    seriesTag = new WBTerm(seriesTags, seriesTagTerm);
            }

            WBQuery query = new WBQuery();

            //query.AddEqualsFilter(WBColumn.RecordsType, workBox.RecordsType);

            if (seriesTag != null)
                query.AddEqualsFilter(WBColumn.SeriesTag, seriesTag);

            if (ReferenceID != null && ReferenceID != "")
                query.AddEqualsFilter(WBColumn.ReferenceID, ReferenceID);

            //            if (FilterByOwningTeam) 
            //            query.AddEqualsFilter(WBColumn.OwningTeam, workBox.OwningTeam);


            if (sortColumn != null) WBLogging.Debug("Sorting in Render with sortColumn: " + sortColumn.DisplayName);
            else
            {
                WBLogging.Debug("SortColumn was null - so sorting by declared record date.");

                sortColumn = WBColumn.DeclaredRecord;
                ascending = false;

            }

            if (sortColumn != null)
            {
                query.OrderBy(sortColumn, ascending);
            }

            query.AddViewColumn(WBColumn.Name);
            query.AddViewColumn(WBColumn.FileTypeIcon);
            query.AddViewColumn(WBColumn.EncodedAbsoluteURL);            
            query.AddViewColumn(WBColumn.ReferenceDate);
            query.AddViewColumn(WBColumn.ProtectiveZone);
            query.AddViewColumn(WBColumn.DeclaredRecord);

            WBFarm farm = WBFarm.Local;

            using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl);

                    DataTable dataTable = recordsLibrary.WBxGetDataTable(site, query, MaxNumDocuments);

                    gridView.DataSource = dataTable;

                    gridView.Columns.Clear();
                    gridView.Columns.Add(WBUtils.DynamicIconTemplateField(WBColumn.FileTypeIcon, WBColumn.EncodedAbsoluteURL));
                    gridView.Columns.Add(WBUtils.HyperLinkField(WBColumn.Name, WBColumn.EncodedAbsoluteURL, sortColumn, ascending));
                    gridView.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));
                    gridView.Columns.Add(WBUtils.BoundField(WBColumn.ProtectiveZone, sortColumn, ascending));
                    gridView.Columns.Add(WBUtils.BoundField(WBColumn.DeclaredRecord, sortColumn, ascending));

                    gridView.DataBind();
                }
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
            String sortExpression = ViewState["SortExpression"] as String;

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
