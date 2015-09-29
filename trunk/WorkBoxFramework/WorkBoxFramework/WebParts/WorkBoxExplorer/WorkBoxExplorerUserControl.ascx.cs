#region Copyright and License

// Copyright (c) Islington Council 2010-2015
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
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.WorkBoxExplorer
{
    public partial class WorkBoxExplorerUserControl : UserControl
    {
        public String NoWorkBoxesText = "There are no work boxes of the selected type.";
        public String RefinementByOwningTeam = "";
        public String SearchScope = "Work%20Box%20Explorer";
        public bool InEditMode = false;

        private WBColumn sortColumn = null;
        private bool ascending = false;

        public String RecordsGroup;
        public String AdditionalCSSStyle;
        public String NotSetupText = "";

        public SPList ConfigurationList = null;

        public WBTeam Team;

        protected WorkBoxExplorer webPart = default(WorkBoxExplorer);

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as WorkBoxExplorer;

            SPWebPartManager webPartManager = (SPWebPartManager)WebPartManager.GetCurrentWebPartManager(this.Page);
            if ((SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                || (webPartManager.DisplayMode == WebPartManager.EditDisplayMode))
            {
                InEditMode = true;
            }

            SearchScope = WBUtils.UrlDataEncode(webPart.SearchScope);

            SelectedWorkBoxes.AllowSorting = true;
            SelectedWorkBoxes.Sorting += new GridViewSortEventHandler(SelectedWorkBoxes_Sorting);

            SelectedWorkBoxes.AllowPaging = true;
            SelectedWorkBoxes.PageIndexChanging += new GridViewPageEventHandler(SelectedWorkBoxes_PageIndexChanging);
            SelectedWorkBoxes.PagerSettings.Mode = PagerButtons.Numeric;
            SelectedWorkBoxes.PagerSettings.Position = PagerPosition.Bottom;
            SelectedWorkBoxes.PagerSettings.PageButtonCount = 50;
            SelectedWorkBoxes.PagerSettings.Visible = true;
            SelectedWorkBoxes.PageSize = 30;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            SelectedWorkBoxes.PagerTemplate = null;

            CoreResultsWebPart.UseLocationVisualization = false;
            CoreResultsWebPart.PropertiesToRetrieve = string.Empty;
            CoreResultsWebPart.SelectColumns = @"<Columns>  <Column Name=""WorkId""/>  <Column Name=""Rank""/>  <Column Name=""Title""/>  <Column Name=""Author""/>  <Column Name=""Size""/>  <Column Name=""Path""/>  <Column Name=""Description""/>  <Column Name=""Write""/>  <Column Name=""SiteName""/>  <Column Name=""CollapsingStatus""/>  <Column Name=""HitHighlightedSummary""/>  <Column Name=""HitHighlightedProperties""/>  <Column Name=""ContentClass""/>  <Column Name=""IsDocument""/>  <Column Name=""PictureThumbnailURL""/>  <Column Name=""PopularSocialTags""/>  <Column Name=""PictureWidth""/>  <Column Name=""PictureHeight""/>  <Column Name=""DatePictureTaken""/>  <Column Name=""ServerRedirectedURL""/>  <Column Name=""SiteTitle""/>  <Column Name=""SPWebURL""/>  <Column Name=""OwningTeam""/>  </Columns>";
            CoreResultsWebPart.XslLink = "/Style Library/WBF/wb-explorer-search-results.xslt";

            //CoreResultsWebPart.DisplayAlertMeLink = true;
            //CoreResultsWebPart.AllowConnect = false;


            SPWeb web = SPContext.Current.Web;
            SPSite site = SPContext.Current.Site;

            WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            string teamGUIDString = "";
            Team = WBTeam.GetFromTeamSite(SPContext.Current);
            if (Team == null) return;

            // RefinementByOwningTeam = "owningteam%3D%22%23" + (Team.Id.ToString().Replace(" ", "%20").Replace("#", "%23").Replace("-", "%2D")) + "%22";
            // RefinementByOwningTeam = "owningteam%3D%22" + (Team.Name.ToString().Replace(" ", "%20").Replace("#", "%23").Replace("-", "%2D")) + "%22";
            RefinementByOwningTeam = WBUtils.UrlDataEncode("owningteam=\"" + Team.Name + "\"");

            teamGUIDString = WBExtensions.WBxToString(Team.Id);
            string recordsTypesListUrl = Team.RecordsTypesListUrl;

            if (recordsTypesListUrl == null || recordsTypesListUrl == "")
            {
                //recordsTypesListUrl = web.Url + "/Lists/Configure%20Teams%20Records%20Classes";
                NotSetupText = "(<i>The team has no records types list setup yet.</i>)";
                return;
            }

            // urm ... this is a real mess - a hidden field and a view state - it's a mashup mess!!
            String selectedRecordsTypeGUID = Request.QueryString["recordsTypeGUID"];
            if (String.IsNullOrEmpty(selectedRecordsTypeGUID))
            {
                String guidString = HiddenRecordsTypeGUIDField.Value;
                if (guidString != null && guidString != "")
                {
                    SelectedRecordsTypeGUID = guidString;
                }
            }
            else
            {
                SelectedRecordsTypeGUID = selectedRecordsTypeGUID;
                HiddenRecordsTypeGUIDField.Value = selectedRecordsTypeGUID;
            }



            using (SPWeb configWeb = site.OpenWeb(recordsTypesListUrl))
            {
                ConfigurationList = configWeb.GetList(recordsTypesListUrl);
                if (ConfigurationList != null)
                {
                    if (!ConfigurationList.Fields.ContainsField("Records Class"))
                    {
                        ConfigurationList = null;
                        NotSetupText = "(<i>The configuration list " + recordsTypesListUrl + " has no 'Records Class' column.</i>)";
                    }
                }
                else
                {
                    NotSetupText = "(<i>The configuration list " + recordsTypesListUrl + " was not set up correctly or does not exist.</i>)";
                }

                if (ConfigurationList != null)
                {

                    TeamAdminRecordsTypesTreeView.Nodes.Clear();
                    OurWorkRecordsTypesTreeView.Nodes.Clear();
                    CouncilWideRecordsTypesTreeView.Nodes.Clear();

                    TeamAdminRecordsTypesFilter.Nodes.Clear();
                    OurWorkRecordsTypesFilter.Nodes.Clear();
                    CouncilWideRecordsTypesFilter.Nodes.Clear();

                    foreach (SPListItem item in ConfigurationList.Items)
                    {
                        try
                        {

                            WBRecordsType recordsClass = new WBRecordsType(recordsTypesTaxonomy, WBExtensions.WBxGetColumnAsString(item, "Records Class"));
                            TreeNode createNewNodes = createNodes(recordsClass, Team, false);
                            TreeNode forFilteringNodes = createNodes(recordsClass, Team, true);

                            string groupName = item.WBxGetColumnAsString("Records Group");
                            if (groupName.Equals("Team admin"))
                            {
                                addNodesToTreeView(TeamAdminRecordsTypesTreeView, createNewNodes, selectedRecordsTypeGUID);
                                addNodesToTreeView(TeamAdminRecordsTypesFilter, forFilteringNodes, selectedRecordsTypeGUID);
                            }

                            if (groupName.Equals("Our work"))
                            {
                                addNodesToTreeView(OurWorkRecordsTypesTreeView, createNewNodes, selectedRecordsTypeGUID);
                                addNodesToTreeView(OurWorkRecordsTypesFilter, forFilteringNodes, selectedRecordsTypeGUID);
                            }

                            if (groupName.Equals("Council-wide business"))
                            {
                                addNodesToTreeView(CouncilWideRecordsTypesTreeView, createNewNodes, selectedRecordsTypeGUID);
                                addNodesToTreeView(CouncilWideRecordsTypesFilter, forFilteringNodes, selectedRecordsTypeGUID);
                            }

                        }
                        catch (Exception exception)
                        {
                            WBUtils.logMessage("The error message was: " + exception.Message);
                        }
                    }
                }
            }


            if (!IsPostBack)
            {
                List<String> ascendingDescendingOptions = new List<String>();
                ascendingDescendingOptions.Add("Ascending");
                ascendingDescendingOptions.Add("Descending");

                AscendingDescendingChoice.DataSource = ascendingDescendingOptions;
                AscendingDescendingChoice.DataBind();

                SetSortColumn(WBColumn.WorkBoxDateLastModified);
                GridViewSortDirection = SortDirection.Descending;

                SelectedViewStyle = VIEW_STYLE__ICONS;
                StatusCheckBox.Checked = false;
                RecordsTypeCheckBox.Checked = true;
                LastModifiedCheckBox.Checked = true;
                DateCreatedCheckBox.Checked = true;


                List<String> statusOptions = new List<String>();
                statusOptions.Add("Open");
                statusOptions.Add("Closed");
                statusOptions.Add("Deleted");
                statusOptions.Add("Any");

                StatusFilter.DataSource = statusOptions;
                StatusFilter.DataBind();

                StatusFilter.WBxSafeSetSelectedValue("Any");

                List<String> involvementOptions = new List<String>();
                involvementOptions.Add(FILTER_INVOLVEMENT__OWNS);
                involvementOptions.Add(FILTER_INVOLVEMENT__INVOLVED);
                involvementOptions.Add(FILTER_INVOLVEMENT__VISITING);

                InvolvementFilter.DataSource = involvementOptions;
                InvolvementFilter.DataBind();


                WBLogging.Debug("Not in post back so setting guid value to be: " + SelectedRecordsTypeGUID);
                RefreshBoundData();
            }

        }

        private void addNodesToTreeView(SPTreeView treeView, TreeNode nodes, String selectedRecordsTypeGUID)
        {
            treeView.Nodes.Add(nodes);
            treeView.CollapseAll();
            expandByRecordsTypeGUID(treeView.Nodes, selectedRecordsTypeGUID);
        }

        private TreeNode createNodes(WBRecordsType recordsType, WBTeam owningTeam, bool forFiltering)
        {
            String commandURL = "";

            if (forFiltering)
            {

                commandURL = "javascript: WorkBoxFramework_triggerWebPartUpdate('" + recordsType.Id.ToString() + "'); ";
            }
            else
            {
                commandURL = "javascript: WorkBoxFramework_relativeCommandAction('NewWorkBox.aspx?workBoxCollectionUrl=" + recordsType.WorkBoxCollectionUrl + "&recordsTypeGUID=" + recordsType.Id.ToString() + "&owningTeamGUID=" + owningTeam.Id.ToString() + "', 0, 0); ";
            }

            TreeNode node = new TreeNode();
            node.Text = recordsType.Name;
            node.NavigateUrl = commandURL;
            node.Value = recordsType.Id.WBxToString();

            Dictionary<String, TreeNode> allNodes = new Dictionary<String, TreeNode>();

            foreach (Term term in recordsType.Term.Terms)
            {
                WBRecordsType childRecordsType = new WBRecordsType(recordsType.Taxonomy, term);

                if (term.IsAvailableForTagging && childRecordsType.AllowWorkBoxRecords)
                {
                    TreeNode childNode = createNodes(childRecordsType, owningTeam, forFiltering);
                    allNodes.Add(childNode.Text, childNode);
                }
            }

            List<String> names = new List<String>(allNodes.Keys);
            names.Sort();

            foreach (String name in names)
            {
                node.ChildNodes.Add(allNodes[name]);
            }

            return node;
        }

        private void expandByRecordsTypeGUID(TreeNodeCollection nodes, String recordsTypeGUID)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Value == recordsTypeGUID)
                {
                    WBLogging.Debug("Found the node to expand: " + node.Text);
                    expandNodeAndParents(node);
                    return;
                }

                expandByRecordsTypeGUID(node.ChildNodes, recordsTypeGUID);
            }
        }

        private void expandNodeAndParents(TreeNode node)
        {
            node.Expand();
            if (node.Parent != null) expandNodeAndParents(node.Parent);
        }

        protected void HiddenSubmitLink_OnClick(object sender, EventArgs e)
        {
            String guidString = HiddenRecordsTypeGUIDField.Value;

            if (guidString != null && guidString != "")
            {
                SelectedRecordsTypeGUID = guidString;
            }
            else
            {
                SelectedRecordsTypeGUID = "";
                WBLogging.Generic.Unexpected("HiddenSubmitLink_OnClick: Had a submit with not set GUID value.");
            }

            CaptureFormData();

            checkSortState();

            RefreshBoundData();
        }

        private void CaptureFormData()
        {
            ViewState["SortExpression"] = OrderBy.SelectedValue;
            if (AscendingDescendingChoice.SelectedValue.Equals("Ascending"))
                GridViewSortDirection = SortDirection.Ascending;
            else
                GridViewSortDirection = SortDirection.Descending;
        }

        protected void UpdateView_OnClick(object sender, EventArgs e)
        {
            CaptureFormData();

            checkSortState();

            RefreshBoundData();
        }


        protected void OrderBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            CaptureFormData();

            checkSortState();

            RefreshBoundData();
        }

        protected void StatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            CaptureFormData();

            checkSortState();

            RefreshBoundData();
        }

        protected void InvolvementFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            CaptureFormData();

            checkSortState();

            RefreshBoundData();
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
                return StatusFilter.SelectedValue;
            }
            set
            {
                StatusFilter.WBxSafeSetSelectedValue(value);
            }
        }

        private const String FILTER_INVOLVEMENT__OWNS = "Owns";
        private const String FILTER_INVOLVEMENT__INVOLVED = "Involved";
        private const String FILTER_INVOLVEMENT__VISITING = "Visiting";

        private String SelectedInvolvementFilter
        {
            get
            {
                return InvolvementFilter.SelectedValue;
            }
            set
            {
                InvolvementFilter.WBxSafeSetSelectedValue(value);
            }
        }

        private const String VIEW_STYLE__ICONS = "Icons View";
        private const String VIEW_STYLE__DETAILS = "Details View";

        public String SelectedViewStyle
        {
            get
            {

                string currentViewStyle = HiddenViewStyleField.Value;
                if (String.IsNullOrEmpty(currentViewStyle))
                {
                    currentViewStyle = VIEW_STYLE__ICONS;
                    HiddenViewStyleField.Value = currentViewStyle;
                }
                return currentViewStyle;
            }
            set
            {
                HiddenViewStyleField.Value = value;
            }
        }

        public String IsDetailsViewStyle { get { return VIEW_STYLE__DETAILS.Equals(SelectedViewStyle).ToString().ToLower(); } }


        private void RefreshBoundData()
        {
            if (IsPostBack) ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TriggerUpdateFunction", "aspPanelHasUpdated();", true);

            WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            WBTaxonomy teams = WBTaxonomy.GetTeams(recordsTypes);

            WBQuery query = new WBQuery();

            query.AddViewColumn(WBColumn.Title);
            query.AddViewColumn(WBColumn.WorkBoxURL);

            //                        query.AddViewColumn(WBColumn.FunctionalArea);
            //query.AddViewColumn(WBColumn.ReferenceDate);
            //query.AddViewColumn(WBColumn.ReferenceID);
            //query.AddViewColumn(WBColumn.SeriesTag);
            //                    query.AddViewColumn(WBColumn.InvolvedTeams);


            //List<String> orderByColumnOptions = new List<String>();
            //orderByColumnOptions.Add("Title");

            checkSortState();
            if (sortColumn != null)
                query.OrderBy(sortColumn, ascending);


            SelectedWorkBoxes.Columns.Clear();

            SelectedWorkBoxes.Columns.Add(WBUtils.StatusIconTemplateField("24"));
            SelectedWorkBoxes.Columns.Add(WBUtils.HyperLinkField(WBColumn.Title, WBColumn.WorkBoxURL, sortColumn, ascending, ""));

            OrderBy.Items.Clear();
            addColumnAsOption(OrderBy, WBColumn.Title);

            //                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
            // SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, HorizontalAlign.Center, sortColumn, ascending));
            //                  SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
            //                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));


            query.AddViewColumn(WBColumn.WorkBoxStatus);
            if (StatusCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxStatus, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.WorkBoxStatus);
            }
            string statusFilter = SelectedWorkBoxStatusFilter;
            if (statusFilter != "Any")
            {
                query.AddEqualsFilter(WBColumn.WorkBoxStatus, statusFilter);
            }


            query.AddViewColumn(WBColumn.RecordsType);
            if (RecordsTypeCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.RecordsType);
            }

            if (LastModifiedCheckBox.Checked)
            {
                query.AddViewColumn(WBColumn.WorkBoxDateLastModified);
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxDateLastModified, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.WorkBoxDateLastModified);
            }

            if (LastVisitedCheckBox.Checked)
            {
                query.AddViewColumn(WBColumn.WorkBoxDateLastVisited);
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxDateLastVisited, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.WorkBoxDateLastVisited);
            }

            if (DateCreatedCheckBox.Checked)
            {
                query.AddViewColumn(WBColumn.WorkBoxDateCreated);
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.WorkBoxDateCreated, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.WorkBoxDateCreated);
            }

            if (ReferenceDateCheckBox.Checked)
            {
                query.AddViewColumn(WBColumn.ReferenceDate);
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.ReferenceDate);
            }

            if (ReferenceIDCheckBox.Checked)
            {
                query.AddViewColumn(WBColumn.ReferenceID);
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, HorizontalAlign.Center, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.ReferenceID);
            }


            query.AddViewColumn(WBColumn.OwningTeam);
            if (OwningTeamCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.OwningTeam);
            }

            query.AddViewColumn(WBColumn.InvolvedTeams);
            if (InvolvedTeamsCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.InvolvedTeams, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.InvolvedTeams);
            }

            query.AddViewColumn(WBColumn.VisitingTeams);
            if (VisitingTeamsCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.VisitingTeams, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.VisitingTeams);
            }

            query.AddViewColumn(WBColumn.InvolvedIndividuals);
            if (InvolvedIndividualsCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.InvolvedIndividuals, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.InvolvedIndividuals);
            }

            query.AddViewColumn(WBColumn.VisitingIndividuals);
            if (VisitingIndividualsCheckBox.Checked)
            {
                SelectedWorkBoxes.Columns.Add(WBUtils.BoundField(WBColumn.VisitingIndividuals, sortColumn, ascending));
                addColumnAsOption(OrderBy, WBColumn.VisitingIndividuals);
            }

            //OrderBy.DataSource = orderByColumnOptions;
            //OrderBy.DataBind();

            OrderBy.WBxSafeSetSelectedValue(ViewState["SortExpression"] as String);


            WBRecordsType recordsType = null;
            WBTeam team = WBTeam.GetFromTeamSite(teams, SPContext.Current.Web);

            WBLogging.Generic.Unexpected("SelectedInvolvementFilter = " + SelectedInvolvementFilter);

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

            if (SelectedRecordsTypeGUID != "")
            {
                recordsType = recordsTypes.GetRecordsType(new Guid(SelectedRecordsTypeGUID));
            }

            if (recordsType != null)
            {
                String recordsTypePath = recordsType.FullPath;
                //recordsTypePath = recordsTypePath.Substring(1, recordsTypePath.Length - 1);
                recordsTypePath = recordsTypePath.Replace("/", " / ");

                RecordsTypeSelected.Text = recordsTypePath;
                RecordsTypeDescription.Text = recordsType.Description;

                WBQueryClause recordsTypeClause = new WBQueryClause(WBColumn.RecordsType, WBQueryClause.Comparators.Equals, recordsType);
                recordsTypeClause.UseDescendants = false;
                query.AddClause(recordsTypeClause);


            }
            else
            {
                RecordsTypeSelected.Text = "No records type selected";
                RecordsTypeDescription.Text = "";
            }

            String cachedDetailsListUrl = WBFarm.Local.OpenWorkBoxesCachedDetailsListUrl;

            // OK so this is a general 'recent' query
            using (SPWeb cacheWeb = SPContext.Current.Site.OpenWeb(cachedDetailsListUrl))
            {
                SPList cacheList = cacheWeb.GetList(cachedDetailsListUrl);

                SPListItemCollection items = cacheList.WBxGetItems(SPContext.Current.Site, query);


                if (items.Count > 0)
                {
                    StringBuilder html = new StringBuilder();
                    foreach (SPListItem item in items)
                    {
                        String status = item.WBxGetAsString(WBColumn.WorkBoxStatus);

                        html.Append("<div class='wbf-icons-view-icon-panel'><div class='wbf-icons-view-icon'><a href='");
                        html.Append(item.WBxGetAsString(WBColumn.WorkBoxURL)).Append("'>").Append("<img src='").Append(WBUtils.StatusIconImageURL(status, "64")).Append("' alt='Work box icon for: ").Append(item.WBxGetAsString(WBColumn.Title).Replace('\'', ' ')).Append("' />").Append("</a></div><div class='wbf-icons-view-label'><a href='");
                        html.Append(item.WBxGetAsString(WBColumn.WorkBoxURL)).Append("'>").Append(item.WBxGetAsString(WBColumn.Title)).Append("</a></div></div>\n\n");
                    }

                    IconViewLiteral.Text = html.ToString();
                }
                else
                {
                    IconViewLiteral.Text = "<p>" + NoWorkBoxesText + "</p>";
                }

                DataTable dataTable = cacheList.WBxGetDataTable(SPContext.Current.Site, query);

                SelectedWorkBoxes.DataSource = dataTable;
                SelectedWorkBoxes.DataBind();
            }

        }

        private void addColumnAsOption(DropDownList dropDownList, WBColumn column)
        {
            dropDownList.Items.Add(new ListItem(column.PrettyName, column.InternalName));
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
            OrderBy.WBxSafeSetSelectedValue(column.InternalName);
        }

        protected void SelectedWorkBoxes_Sorting(object sender, GridViewSortEventArgs e)
        {
            WBLogging.Debug("In SelectedWorkBoxes_Sorting with e.SortExpression = " + e.SortExpression);

            string sortExpression = e.SortExpression;
            ViewState["SortExpression"] = sortExpression;

            sortColumn = WBColumn.GetKnownColumnByInternalName(sortExpression);
            OrderBy.WBxSafeSetSelectedValue(sortExpression);

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
                    AscendingDescendingChoice.WBxSafeSetSelectedValue("Ascending");
                    ascending = true;
                }
                else
                {
                    AscendingDescendingChoice.WBxSafeSetSelectedValue("Descending");
                    ascending = false;
                }
            }
        }

    }
}
