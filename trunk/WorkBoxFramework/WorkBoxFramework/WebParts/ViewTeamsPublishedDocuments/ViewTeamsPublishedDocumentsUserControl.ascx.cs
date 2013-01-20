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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ViewTeamsPublishedDocuments
{
    public partial class ViewTeamsPublishedDocumentsUserControl : UserControl
    {
        protected ViewTeamsPublishedDocuments webPart = default(ViewTeamsPublishedDocuments);

        private const String VIEW_BY_RECORDS_TYPE = "By Records Type";

        private const String VIEW_BY_FUNCTION_THEN_TYPE = "By Function then Type";
        private const String VIEW_BY_SUBJECT = "By Subject";
        private const String VIEW_BY_FILING_PATH = "By Filing Path";


        private WBColumn sortColumn = null;
        private bool ascending = false;

        public WBTaxonomy recordsTypesTaxonomy = null;
        protected WBTaxonomy teamsTaxonomy = null;
        protected WBTaxonomy functionalAreaTaxonomy = null;
        protected WBTaxonomy subjectTagsTaxonomy = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as ViewTeamsPublishedDocuments;

            recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            teamsTaxonomy = WBTaxonomy.GetTeams(recordsTypesTaxonomy);
            functionalAreaTaxonomy = WBTaxonomy.GetFunctionalAreas(recordsTypesTaxonomy);
            subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(recordsTypesTaxonomy);

            //RefreshBrowsableTreeView();

            ShowResults.AllowSorting = true;
            ShowResults.Sorting += new GridViewSortEventHandler(ShowResults_Sorting);

            ShowResults.AllowPaging = true;
            ShowResults.PageIndexChanging += new GridViewPageEventHandler(ShowResults_PageIndexChanging);
            ShowResults.PagerSettings.Mode = PagerButtons.Numeric;
            ShowResults.PagerSettings.Position = PagerPosition.Bottom;
            ShowResults.PagerSettings.PageButtonCount = 50;
            ShowResults.PagerSettings.Visible = true;
            ShowResults.PageSize = 50;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            ShowResults.PagerTemplate = null;


            if (!IsPostBack)
            {
                //                ViewSelector.DataSource = LibraryViews;
                //              ViewSelector.DataBind();

                //ProtectiveZoneFilterOptions = WBRecordsType.getProtectiveZones();
                // ProtectiveZoneFilterOptions.Insert(0, "");

                // FilterByProtectiveZone.DataSource = ProtectiveZoneFilterOptions;
                // FilterByProtectiveZone.DataBind();

                SelectedView = VIEW_BY_FUNCTION_THEN_TYPE;

                RefreshBoundData();
            }



        }


        private String SelectedView
        {
            get { return ViewState["WBF_SelectedView"].WBxToString(); }
            set { ViewState["WBF_SelectedView"] = value; }
        }

        private String ProtectiveZoneFilter
        {
            get { return ViewState["WBF_ProtectiveZoneFilter"].WBxToString(); }
            set { ViewState["WBF_ProtectiveZoneFilter"] = value; }
        }








        private void RefreshBoundData()
        {
            //if (SelectedNodePath != "")
           // {
             //   SelectedRecordsType.Text = SelectedNodePath.Replace("Records Types/", "").Replace("/", " / ");

                WBRecordsType recordsType = null;
                WBTerm functionalArea = null;
                WBTerm subjectTag = null;

            /*
                switch (SelectedView)
                {
                    case VIEW_BY_RECORDS_TYPE:
                        {
                            recordsType = recordsTypesTaxonomy.GetSelectedRecordsType(SelectedNodePath);
                           // SelectedRecordsTypeDescription.Text = recordsType.Description;
                            break;
                        }

                    case VIEW_BY_FUNCTION_THEN_TYPE:
                        {
                            string[] parts = SelectedNodePath.Split('/');
                            if (parts.Length < 3) return;

                            string functionPath = parts[0];
                            List<String> partsList = new List<String>(parts);
                            partsList.RemoveAt(0);
                            string recordsTypePath = String.Join("/", partsList.ToArray());

                            Term functionalAreaTerm = functionalAreaTaxonomy.GetSelectedTermByPath(functionPath);
                            if (functionalAreaTerm != null)
                            {
                                functionalArea = new WBTerm(functionalAreaTaxonomy, functionalAreaTerm);
                            }

                            recordsType = recordsTypesTaxonomy.GetSelectedRecordsType(recordsTypePath);
                           // SelectedRecordsTypeDescription.Text = recordsType.Description;
                            break;
                        }

                    case VIEW_BY_SUBJECT:
                        {
                            Term subjectTagsTerm = subjectTagsTaxonomy.GetSelectedTermByPath(SelectedNodePath);
                            if (subjectTagsTerm != null)
                            {
                                subjectTag = new WBTerm(subjectTagsTaxonomy, subjectTagsTerm);
                                SelectedRecordsTypeDescription.Text = subjectTag.Description;
                            }

                            if (subjectTag == null)
                            {
                                WBLogging.Debug("Did not find subject with path: " + SelectedNodePath);
                                return;
                            }
                            break;
                        }


                    default: return;

                }
            */

                WBTeam team = null;
            
               if (!String.IsNullOrEmpty(webPart.FilterByOwningTeam)) 
               {
                    team = teamsTaxonomy.GetSelectedTeam(webPart.FilterByOwningTeam);
               }

               if (team == null) 
               {
                   team = WBTeam.getFromTeamSite(teamsTaxonomy, SPContext.Current.Web);
               }
            
               if (team == null)
               {
                   WBUtils.shouldThrowError("There was no team configured - so we'll jus stop");
                   return;
               }
                

                WBFarm farm = WBFarm.Local;

                using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        WBQuery query = new WBQuery();

                        if (recordsType != null)
                        {
                            WBQueryClause recordsTypeClause = new WBQueryClause(WBColumn.RecordsType, WBQueryClause.Comparators.Equals, recordsType);
                            recordsTypeClause.UseDescendants = true;
                            query.AddClause(recordsTypeClause);
                        }

                        if (functionalArea != null)
                        {
                            WBQueryClause functionalAreaClause = new WBQueryClause(WBColumn.FunctionalArea, WBQueryClause.Comparators.Equals, functionalArea);
                            functionalAreaClause.UseDescendants = true;
                            query.AddClause(functionalAreaClause);
                        }

                        if (subjectTag != null)
                        {
                            WBQueryClause subjectTagClause = new WBQueryClause(WBColumn.SubjectTags, WBQueryClause.Comparators.Equals, subjectTag);
                            subjectTagClause.UseDescendants = false;
                            query.AddClause(subjectTagClause);
                        }

                        if (team != null)
                        {
                            query.AddEqualsFilter(WBColumn.OwningTeam, team);
                        }

                        //if (!String.IsNullOrEmpty(webPart.FilterByProtectiveZone))
                       // {
                         //   query.AddEqualsFilter(WBColumn.ProtectiveZone, webPart.FilterByProtectiveZone);
                        //}


                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.TitleOrName);
//                        query.AddViewColumn(WBColumn.FileSize);
                        query.AddViewColumn(WBColumn.FileTypeIcon);
  //                      query.AddViewColumn(WBColumn.FileType);
    //                    query.AddViewColumn(WBColumn.DisplayFileSize);
                        query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
      //                  query.AddViewColumn(WBColumn.FunctionalArea);
        //                query.AddViewColumn(WBColumn.OwningTeam);
                        query.AddViewColumn(WBColumn.ReferenceDate);
                        query.AddViewColumn(WBColumn.DeclaredRecord);
                        query.AddViewColumn(WBColumn.SeriesTag);
            //            query.AddViewColumn(WBColumn.ProtectiveZone);

                        if (SelectedView != VIEW_BY_SUBJECT)
                        {
                            query.AddViewColumn(WBColumn.SubjectTags);
                        }
                        else
                        {
                            query.AddViewColumn(WBColumn.RecordsType);
                        }

                        if (sortColumn == null) {
                            sortColumn = WBColumn.DeclaredRecord;
                            ascending = false;
                        }

                        if (sortColumn != null)
                            query.OrderBy(sortColumn, ascending);

                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        DataTable dataTable = recordsLibrary.WBxGetDataTable(site, query);

                        ShowResults.DataSource = dataTable;

                        ShowResults.Columns.Clear();
                        ShowResults.Columns.Add(WBUtils.DynamicIconTemplateField(WBColumn.FileTypeIcon, WBColumn.EncodedAbsoluteURL));
                        ShowResults.Columns.Add(WBUtils.HyperLinkField(WBColumn.TitleOrName, WBColumn.EncodedAbsoluteURL, sortColumn, ascending));
                       // ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FileType, sortColumn, ascending));
                      //  ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.DisplayFileSize, sortColumn, ascending));
                        //                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));
                        //                      ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
                        //ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
                       // ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                      //  ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ProtectiveZone, sortColumn, ascending));

                        /*
                        if (SelectedView != VIEW_BY_SUBJECT)
                        {
                            ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SubjectTags, sortColumn, ascending));
                        }
                        else
                        {
                            ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, sortColumn, ascending));
                        }
                        */

                        ShowResults.DataBind();

                    }
                }
            /*
            }
            else
            {
                WBUtils.logMessage("SelectedNodePath was empty");
            }
             */ 

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
