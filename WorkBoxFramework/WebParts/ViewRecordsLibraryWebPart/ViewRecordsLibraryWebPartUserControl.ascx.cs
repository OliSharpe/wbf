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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ViewRecordsLibraryWebPart
{
    public partial class ViewRecordsLibraryWebPartUserControl : UserControl
    {
        protected ViewRecordsLibraryWebPart webPart = default(ViewRecordsLibraryWebPart);

        private const String VIEW_BY_RECORDS_TYPE = "By Records Type";

        private const String VIEW_BY_FUNCTION_THEN_TYPE = "By Function then Type";
        private const String VIEW_BY_SUBJECT = "By Subject";
        private const String VIEW_BY_FILING_PATH = "By Filing Path";

        
        private WBColumn sortColumn = null;
        private bool ascending = false;

        public String[] LibraryViews = {VIEW_BY_FUNCTION_THEN_TYPE,  VIEW_BY_RECORDS_TYPE, VIEW_BY_SUBJECT, VIEW_BY_FILING_PATH };
        public List<String> ProtectiveZoneFilterOptions;

        public WBTaxonomy recordsTypesTaxonomy = null;
        protected WBTaxonomy teamsTaxonomy = null;
        protected WBTaxonomy functionalAreaTaxonomy = null;
        protected WBTaxonomy subjectTagsTaxonomy = null;

        bool currentUserIsRecordsManager = false;

        public bool showFilters = false;


        protected void Page_Load(object sender, EventArgs e)
        {

            webPart = this.Parent as ViewRecordsLibraryWebPart;

            recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            teamsTaxonomy = WBTaxonomy.GetTeams(recordsTypesTaxonomy);
            functionalAreaTaxonomy = WBTaxonomy.GetFunctionalAreas(recordsTypesTaxonomy);
            subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(recordsTypesTaxonomy);


            if (!IsPostBack)
            {
//                ViewSelector.DataSource = LibraryViews;
  //              ViewSelector.DataBind();

                ProtectiveZoneFilterOptions = WBRecordsType.getProtectiveZones();
                ProtectiveZoneFilterOptions.Insert(0, "");

                FilterByProtectiveZone.DataSource = ProtectiveZoneFilterOptions;
                FilterByProtectiveZone.DataBind();

                string selectedView = webPart.RecordsLibraryView;
                if (String.IsNullOrEmpty(selectedView))
                    selectedView = VIEW_BY_RECORDS_TYPE;

                SelectedViewTitle.Text = selectedView; 
                SelectedView = selectedView; 
            }

            RefreshBrowsableTreeView();

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



        }

        private void RefreshBrowsableTreeView()
        {
            switch (SelectedView)
            {
                case VIEW_BY_RECORDS_TYPE:
                    {
//                        TreeViewTermCollection collection = new TreeViewTermCollection();
  //                      collection.Add(new TreeViewTerm(recordsTypesTaxonomy.TermSet));

    //                    BrowsableTreeView.DataSource = collection;
      //                  BrowsableTreeView.DataBind();
                        
                        foreach (Term term in recordsTypesTaxonomy.TermSet.Terms)
                        {
                            if (term.IsAvailableForTagging)
                            {
                                WBRecordsType recordsType = new WBRecordsType(recordsTypesTaxonomy, term);

                                if (recordsType.AllowDocumentRecords)
                                {
                                    BrowsableTreeView.Nodes.Add(AddRecordsTypeBranch(null, recordsType));
                                }
                            }
                        }

                        break;
                    }

                case VIEW_BY_FUNCTION_THEN_TYPE:
                    {
                        //WBTaxonomy functionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(recordsTypesTaxonomy);

                        //BrowsableTreeView.DataSource = null;


                        foreach (Term term in functionalAreaTaxonomy.TermSet.Terms)
                        {
                            if (term.IsAvailableForTagging)
                            {
                                BrowsableTreeView.Nodes.Add(AddFunctionBranch(term));
                            }
                        }
                        break;
                    }

                case VIEW_BY_SUBJECT:
                    {
                        foreach (Term term in subjectTagsTaxonomy.TermSet.Terms)
                        {
                            if (term.IsAvailableForTagging)
                            {
                                BrowsableTreeView.Nodes.Add(AddSubjectBranch(term));
                            }
                        }

                        break;
                    }

            }


            
            /*
            TreeNode rootNode = new TreeNode("Test", "Test", "/_layouts/Images/EMMTerm.png");

            rootNode.ChildNodes.Add(new TreeNode("Child", "Child", "/_layouts/Images/FOLDER.GIF"));
            rootNode.ChildNodes.Add(new TreeNode("Child2", "Child2", "/_layouts/Images/FOLDER.GIF"));
            rootNode.ChildNodes.Add(new TreeNode("Child3", "Child3", "/_layouts/Images/EMMTerm.png"));

            PickRecordsTypeTreeView.Nodes.Add(rootNode);
            */




        }

        private TreeNode AddFunctionBranch(Term functionTerm)
        {
            TreeNode functionNode = new TreeNode(functionTerm.Name, functionTerm.Name, "/_layouts/Images/FOLDER.GIF");
            //WBLogging.Generic.Verbose("Adding the funciton node: " + functionTerm.Name);
            foreach (Term recordsGrouping in recordsTypesTaxonomy.TermSet.Terms)
            {
                WBRecordsType recordsType = new WBRecordsType(recordsTypesTaxonomy, recordsGrouping);

                if (recordsGrouping.IsAvailableForTagging == false) continue;
                if (!recordsType.AllowDocumentRecords) continue;

                if (string.IsNullOrEmpty(recordsType.DefaultFunctionalAreaUIControlValue) || recordsType.DefaultFunctionalAreaUIControlValue.Contains(functionTerm.Id.ToString()))
                {
                    functionNode.ChildNodes.Add(AddRecordsTypeBranch(functionTerm, recordsType));
                }
            }

            

            return functionNode;
        }

        private TreeNode AddSubjectBranch(Term subjectTerm)
        {
            TreeNode subjectNode = new TreeNode(subjectTerm.Name, subjectTerm.Name, "/_layouts/Images/EMMTerm.png");

            //WBLogging.Generic.Verbose("Adding the subject node: " + subjectTerm.Name);
            foreach (Term childSubject in subjectTerm.Terms)
            {
                if (childSubject.IsAvailableForTagging)
                    subjectNode.ChildNodes.Add(AddSubjectBranch(childSubject));
            }

            if (subjectNode.ChildNodes.Count == 0)
            {
            //    subjectNode.PopulateOnDemand = true;
            }

            return subjectNode;
        }


        private TreeNode AddRecordsTypeBranch(Term functionTerm, WBRecordsType recordsType)
        {
            TreeNode recordsTypeNode = new TreeNode(recordsType.Name, recordsType.Name, "/_layouts/Images/EMMTerm.png");
            //WBLogging.Generic.Verbose("Adding the records type node: " + recordsType.Name);
 
            foreach (Term childRecordsTypeTerm in recordsType.Term.Terms)
            {
                if (!childRecordsTypeTerm.IsAvailableForTagging) continue;

                WBRecordsType childRecordsType = new WBRecordsType(recordsTypesTaxonomy, childRecordsTypeTerm);
                if (!childRecordsType.AllowDocumentRecords) continue;

                if (functionTerm == null || string.IsNullOrEmpty(childRecordsType.DefaultFunctionalAreaUIControlValue) || recordsType.DefaultFunctionalAreaUIControlValue.Contains(functionTerm.Id.ToString()))
                {
                    recordsTypeNode.ChildNodes.Add(AddRecordsTypeBranch(functionTerm, childRecordsType));
                }
            }

            return recordsTypeNode;
        }


        private String SelectedNodePath
        {
            get { return ViewState["WBF_SelectedNodePath"].WBxToString(); }
            set { ViewState["WBF_SelectedNodePath"] = value; }
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


        protected void BrowsableTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (BrowsableTreeView.SelectedNode != null)
            {
                //Let's add a new node:

//                BrowsableTreeView.SelectedNode.ChildNodes.Add(new TreeNode("Test", "Test", "/_layouts/Images/EMMTerm.png"));

                SelectedNodePath = BrowsableTreeView.SelectedNode.ValuePath;
            }
            else
            {
                SelectedNodePath = "";
            }

            RefreshBoundData();
        }


        protected void BrowsableTreeView_PopulateNode(object sender, TreeNodeEventArgs e)
        {

            TreeNode newNode = new TreeNode("Test", "Test", "/_layouts/Images/EMMTerm.png");
            newNode.PopulateOnDemand = true;

            e.Node.ChildNodes.Add(newNode);

            if (BrowsableTreeView.SelectedNode != null)
            {
                SelectedNodePath = BrowsableTreeView.SelectedNode.ValuePath;
            }
            else
            {
                SelectedNodePath = "";
            }

            RefreshBoundData();
        }



        protected void ViewSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedViewTitle.Text = "Browse " + webPart.RecordsLibraryView; // ViewSelector.SelectedValue;
            // SelectedView = ViewSelector.SelectedValue;

            SelectedNodePath = "";

            RefreshBrowsableTreeView();
            RefreshBoundData();

        }

        protected void FilterByProtectiveZone_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ProtectiveZoneFilter = FilterByProtectiveZone.SelectedValue;

            RefreshBoundData();
        }




        private void RefreshBoundData()
        {
            if (SelectedNodePath != "")
            {
                SPGroup rmManagersGroup = SPContext.Current.Web.WBxGetGroupOrNull(WBFarm.Local.RecordsManagersGroupName);

                if (rmManagersGroup != null)
                {
                    if (rmManagersGroup.ContainsCurrentUser)
                    {
                        currentUserIsRecordsManager = true;
                        WBLogging.Debug("Set the current user as being a records manager.");
                    }
                    else
                    {
                        WBLogging.Debug("The current user is not a part of the RM Managers group called: " + WBFarm.Local.RecordsManagersGroupName);
                    }
                }
                else
                {
                    WBLogging.Debug("Could not find the RM Managers group called: " + WBFarm.Local.RecordsManagersGroupName);
                }



                SelectedRecordsType.Text = SelectedNodePath.Replace("Records Types/", "").Replace("/", " / ");

                WBRecordsType recordsType = null;
                WBTerm functionalArea = null;
                WBTerm subjectTag = null;

                switch (SelectedView)
                {
                    case VIEW_BY_RECORDS_TYPE:
                        {
                            recordsType = recordsTypesTaxonomy.GetSelectedRecordsType(SelectedNodePath);
                            SelectedRecordsTypeDescription.Text = recordsType.Description;
                            break;
                        }

                    case VIEW_BY_FUNCTION_THEN_TYPE:
                        {
                            string[] parts = SelectedNodePath.Split('/');
                            if (parts.Length<3) break;

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
                            SelectedRecordsTypeDescription.Text = recordsType.Description;
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
                                WBLogging.Generic.Verbose("Did not find subject with path: " + SelectedNodePath);
                                return;
                            }
                            break;
                        }


                    default: return;

                }


                WBTeam team = WBTeam.getFromTeamSite(teamsTaxonomy, SPContext.Current.Web);

                WBFarm farm = WBFarm.Local;

                using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        WBQuery query = new WBQuery();

                        if (recordsType == null && functionalArea == null)
                        {
                            query.LogicallyCannotHaveResults = true;
                        }
                        else
                        {

                            if (SelectedView == VIEW_BY_FUNCTION_THEN_TYPE)
                            {
                                query.FilterByFolderPath = SelectedNodePath;
                            }
                            else
                            {
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
                            }

                            query.AddClause(new WBQueryClause(WBColumn.ContentType, WBQueryClause.Comparators.Equals, WorkBox.WORK_BOX_RECORD_CONTENT_TYPE_NAME));


                            if (subjectTag != null)
                            {
                                WBQueryClause subjectTagClause = new WBQueryClause(WBColumn.SubjectTags, WBQueryClause.Comparators.Equals, subjectTag);
                                subjectTagClause.UseDescendants = false;
                                query.AddClause(subjectTagClause);
                            }

                            if (team != null)
                            {
                                query.AddEqualsFilter(WBColumn.InvolvedTeams, team);
                            }

                            if (!String.IsNullOrEmpty(webPart.FilterByProtectiveZone))
                            {
                                query.AddEqualsFilter(WBColumn.ProtectiveZone, webPart.FilterByProtectiveZone);
                            }
                        }

                        showFilters = true;

                        string statusFilter = SelectedLiveOrArchivedStatusFilter;
                        if (statusFilter == null || statusFilter == "") statusFilter = "All"; // WBColumn.LIVE_OR_ARCHIVED__LIVE;
                        if (statusFilter != "All")
                        {
                            query.AddEqualsFilter(WBColumn.LiveOrArchived, statusFilter);
                        }

                        FilterLiveStatus.CssClass = "wbf-unselected-filter";
                        FilterArchivedStatus.CssClass = "wbf-unselected-filter";
                        FilterAllStatus.CssClass = "wbf-unselected-filter";

                        if (statusFilter == WBColumn.LIVE_OR_ARCHIVED__LIVE) FilterLiveStatus.CssClass = "wbf-selected-filter";
                        if (statusFilter == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED) FilterArchivedStatus.CssClass = "wbf-selected-filter";
                        if (statusFilter == "All") FilterAllStatus.CssClass = "wbf-selected-filter";


                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.TitleOrName);
                        query.AddViewColumn(WBColumn.FileSize);
                        query.AddViewColumn(WBColumn.FileTypeIcon);
                        query.AddViewColumn(WBColumn.FileType);
                        query.AddViewColumn(WBColumn.DisplayFileSize);
                        query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
                        query.AddViewColumn(WBColumn.FunctionalArea);
                        query.AddViewColumn(WBColumn.OwningTeam);
                        query.AddViewColumn(WBColumn.ReferenceDate);
                        query.AddViewColumn(WBColumn.ReferenceID);
                        query.AddViewColumn(WBColumn.SeriesTag);
                        query.AddViewColumn(WBColumn.ProtectiveZone);
                        query.AddViewColumn(WBColumn.RecordID);
                        query.AddViewColumn(WBColumn.LiveOrArchived);
                        


                        if (SelectedView != VIEW_BY_SUBJECT)
                        {
                            query.AddViewColumn(WBColumn.SubjectTags);
                        }
                        else
                        {
                            query.AddViewColumn(WBColumn.RecordsType);
                        }

                        if (sortColumn != null)
                            query.OrderBy(sortColumn, ascending);





                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        DataTable dataTable = recordsLibrary.WBxGetDataTable(site, query);

                        ShowResults.DataSource = dataTable;

                        ShowResults.Columns.Clear();
                        ShowResults.Columns.Add(WBUtils.DynamicIconTemplateField(WBColumn.FileTypeIcon, WBColumn.EncodedAbsoluteURL));
                        ShowResults.Columns.Add(WBUtils.HyperLinkField(WBColumn.TitleOrName, WBColumn.EncodedAbsoluteURL, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FileType, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.DisplayFileSize, sortColumn, ascending));
//                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));
  //                      ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
                        //ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ProtectiveZone, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.LiveOrArchived, sortColumn, ascending));


                        if (SelectedView != VIEW_BY_SUBJECT)
                        {
                            ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SubjectTags, sortColumn, ascending));
                        }
                        else
                        {
                            ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, sortColumn, ascending));
                        }

                        if (this.currentUserIsRecordsManager)
                        {
                            List<WBColumn> valueColumns = new List<WBColumn>();
                            valueColumns.Add(WBColumn.RecordID);

                            String formatString = SPContext.Current.Web.Url + "/_layouts/WorkBoxFramework/UpdateRecordsMetadata.aspx?RecordID={0}";

                            formatString = "<a href=\"javascript: WorkBoxFramework_commandAction('" + formatString + "', 800, 600); \">(edit metadata)</a>";

                            ShowResults.Columns.Add(WBUtils.FormatStringTemplateField(formatString, valueColumns));
                            WBLogging.Debug("Added the (edit metadata) show column.");
                        }
                        else
                        {
                            WBLogging.Debug("Did not add the (edit metadata) show column.");
                        }


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
            WBLogging.Generic.Verbose("In gridView_PageIndexChanging - not sure if there's anything that needs to be done!");

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
            WBLogging.Generic.Verbose("In gridView_Sorting with e.SortExpression = " + e.SortExpression);

            string sortExpression = e.SortExpression;
            ViewState["SortExpression"] = sortExpression;

            sortColumn = WBColumn.GetKnownColumnByInternalName(sortExpression);

            if (GridViewSortDirection == SortDirection.Ascending)
            {
                GridViewSortDirection = SortDirection.Descending;
                ascending = false;
                WBLogging.Generic.Verbose("In gridView_Sorting setting to descending");
            }
            else
            {
                GridViewSortDirection = SortDirection.Ascending;
                ascending = true;
                WBLogging.Generic.Verbose("In gridView_Sorting setting to ascending");
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



        protected void FilterLiveStatus_OnClick(object sender, EventArgs e)
        {
            SelectedLiveOrArchivedStatusFilter = WBColumn.LIVE_OR_ARCHIVED__LIVE;
            RefreshBoundData();
        }

        protected void FilterArchivedStatus_OnClick(object sender, EventArgs e)
        {
            SelectedLiveOrArchivedStatusFilter = WBColumn.LIVE_OR_ARCHIVED__ARCHIVED;
            RefreshBoundData();
        }

        protected void FilterAllStatus_OnClick(object sender, EventArgs e)
        {
            SelectedLiveOrArchivedStatusFilter = "All";
            RefreshBoundData();
        }

        private String SelectedLiveOrArchivedStatusFilter
        {
            get
            {
                string currentStatusFilter = ViewState["WBF_SelectedLiveOrArchivedStatusFilter"].WBxToString();
                if (String.IsNullOrEmpty(currentStatusFilter))
                {
                    currentStatusFilter = Request.QueryString["LiveOrArchived"];

                    if (String.IsNullOrEmpty(currentStatusFilter))
                    {
                        currentStatusFilter = "All"; // WBColumn.LIVE_OR_ARCHIVED__LIVE;
                    }

                    ViewState["WBF_SelectedLiveOrArchivedStatusFilter"] = currentStatusFilter;
                }
                return currentStatusFilter;
            }
            set { ViewState["WBF_SelectedLiveOrArchivedStatusFilter"] = value; }
        }

    }
}
