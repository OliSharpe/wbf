using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.SearchOrBrowseOurRecords
{
    public partial class SearchOrBrowseOurRecordsUserControl : UserControl
    {
        WBRecordsManager manager = null;
        WorkBox workBox = null;
        WBTeam team = null;
        WBTaxonomy teamsTaxonomy = null;
        WBTaxonomy functionalAreasTaxonomy = null;
        WBLocationTreeState treeState = null;

        String selectedPath = null;

        private WBColumn sortColumn = null;
        private bool ascending = false;

        bool masterLibraryHasVersions = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName);

            if (WorkBox.IsWebAWorkBox(SPContext.Current.Web))
            {
                workBox = new WorkBox(SPContext.Current);
                team = workBox.OwningTeam;
                functionalAreasTaxonomy = workBox.FunctionalAreasTaxonomy;
            }
            else
            {
                team = WBTeam.GetFromTeamSite(SPContext.Current);
                if (team != null)
                {
                    teamsTaxonomy = team.Taxonomy;
                    functionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(teamsTaxonomy);
                }
            }

            if (team == null)
            {
                WBLogging.Debug("Couldn't find a suitable team !!");
                return;
            }

            masterLibraryHasVersions = manager.Libraries.ProtectedMasterLibrary.List.EnableVersioning;

            RecordsLibraryFolders.TreeNodePopulate += new TreeNodeEventHandler(RecordsLibraryFolders_TreeNodePopulate);
            // RecordsLibraryFolders.SelectedNodeChanged += new EventHandler(RecordsLibraryFolders_SelectedNodeChanged);

            RecordsLibraryFolders.PopulateNodesFromClient = true;
            RecordsLibraryFolders.EnableClientScript = true;

            treeState = new WBLocationTreeState(SPContext.Current.Web, WBRecordsManager.VIEW_MODE__BROWSE_FOLDERS, WBRecordsType.PROTECTIVE_ZONE__PUBLIC);

            if (!IsPostBack)
            {
                WBTermCollection<WBTerm> functionalAreas = team.FunctionalArea(functionalAreasTaxonomy);

                ViewState["SortColumn"] = WBColumn.DatePublished.InternalName;
                ViewState["SortDirection"] = "Descending";

                /*
                TreeViewLocationCollection collection = new TreeViewLocationCollection(manager, , "", functionalAreas);

                RecordsLibraryFolders.DataSource = collection;
                RecordsLibraryFolders.DataBind();
                */

                manager.PopulateWithFunctionalAreas(treeState, RecordsLibraryFolders.Nodes, WBRecordsManager.VIEW_MODE__BROWSE_FOLDERS, functionalAreas);
            }
            else
            {
                SetSelectedPath();
                if (!String.IsNullOrEmpty(selectedPath))
                {
                    ProcessSelection(selectedPath);
                }
            }
        }

        protected void SetSelectedPath()
        {
            String eventArgument = Request.Params["__EVENTARGUMENT"];

            if (!String.IsNullOrEmpty(eventArgument) && eventArgument[0] == 's')
            {
                selectedPath = eventArgument.Substring(1);
                selectedPath = selectedPath.Replace("\\", "/");
            }
            else
            {
                selectedPath = HiddenSelectedPath.Value;
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (manager != null)
            {
                manager.Dispose();
                manager = null;
            }

            if (workBox != null)
            {
                workBox.Dispose();
                workBox = null;
            }
        }

        protected void HiddenSubmitLink_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug(" HiddenSelectedPath = " + HiddenSelectedPath.Value);
            WBLogging.Debug(" HiddenSortColumn = " + HiddenSortColumn.Value);
            WBLogging.Debug(" HiddenSortDirection = " + HiddenSortDirection.Value);

        }

        protected void DoSearch_Click(object sender, EventArgs e)
        {
            string strQuery = "";
            strQuery = "<OrderBy><FieldRef Name='Title' Ascending='FALSE' /></OrderBy>";
            List<string> conditions = new List<string>();

            String filter = "";

            String searchText = SearchBox.Text;

            if (searchText != "")
            {
                filter = "<Or><Contains><FieldRef Name='BaseName'/><Value Type='Text'>" + searchText + "</Value></Contains><Contains><FieldRef Name='Title'/><Value Type='Text'>" + searchText + "</Value></Contains></Or>";
            }


            if (!String.IsNullOrEmpty(filter))
            {
                strQuery = strQuery + "<Where>" + filter + "</Where>";
            }

            SPList List = manager.Libraries.ProtectedMasterLibrary.List;
            SPQuery query = new SPQuery();

            query.Query = string.Format(strQuery);

            WBLogging.Debug("The query filter being used: \n" + query.Query);

            SPFolder protectedLibraryRootFolder = manager.Libraries.ProtectedMasterLibrary.List.RootFolder;

            /*
            WBTerm functionalArea = workBox.OwningTeam.FunctionalArea(workBox.FunctionalAreasTaxonomy)[0];

            WBLogging.Debug("Looking for folder: \n" + functionalArea.Name);

            SPFolder functionalAreaFolder = protectedLibraryRootFolder.WBxGetFolderPath(functionalArea.Name);

            if (functionalAreaFolder == null) WBLogging.Debug("functionalAreaFolder == null");
            else
            {
                WBLogging.Debug("Adding folder filter to query of: " + functionalAreaFolder.Name);
            //    query.Folder = functionalAreaFolder;
            }
            */


            SPListItemCollection items = List.GetItems(query);

            WBLogging.Debug("Found items: " + items.Count);

            RenderFoundRecords(items);

        }

        /*
        private String GetSelectedFolderPath()
        {
            string selectedPath = RecordsLibraryFolders.SelectedNode.ValuePath;
            if (string.IsNullOrEmpty(selectedPath)) selectedPath = "/";
            return selectedPath;
        }
         */ 

        protected void RecordsLibraryFolders_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            WBLogging.Debug("Call to RecordsLibraryFolders_TreeNodePopulate");

            manager.PopulateTreeNode(treeState, e.Node, treeState.ViewMode);
        }

        /*
        protected void RecordsLibraryFolders_SelectedNodeChanged(object sender, EventArgs e)
        {
            WBLogging.Debug("Call to RecordsLibraryFolders_SelectedNodeChanged");

            String selectedPath = manager.GetSelectedPath(Request);
            if (!String.IsNullOrEmpty(selectedPath))
            {
                ProcessSelection(selectedPath);
            }
        }
         */ 



        protected void ProcessSelection(String selectedPath)
        {
            if (!String.IsNullOrEmpty(selectedPath))
            {

                if (!String.IsNullOrEmpty(HiddenSortColumn.Value))
                {
                    ViewState["SortColumn"] = HiddenSortColumn.Value;
                    ViewState["SortDirection"] = HiddenSortDirection.Value; 
                }

                // Now for the bit where the path is analysed to pick out the selected functional area and the records type:

                String[] pathSteps = selectedPath.Split('/');

                // We're only interested in selections of 3rd level 'folders' that are: functional area / records type / records type  ... or below.
                if (pathSteps.Length < 3) return;

                WBTerm functionalArea = manager.FunctionalAreasTaxonomy.GetSelectedWBTermByPath(pathSteps[0]);
                if (functionalArea == null)
                {
                    WBLogging.Debug("The functional area part of the selected path came back null: " + selectedPath);
                    return;
                }

                Term recordsTypeTerm = manager.RecordsTypesTaxonomy.GetOrCreateSelectedTermByPath(pathSteps[1] + "/" + pathSteps[2]);
                if (recordsTypeTerm == null)
                {
                    WBLogging.Debug("The records type part of the selected path came back null: " + selectedPath);
                    return;
                }
                WBRecordsType recordsType = new WBRecordsType(manager.RecordsTypesTaxonomy, recordsTypeTerm);

                RenderRecordsLibraryFoldersSelection(selectedPath);
            }
        }

        private void RenderRecordsLibraryFoldersSelection(String selectedPath)
        {
            SPListItemCollection items = GetRecordsInFolder(selectedPath);

            RenderFoundRecords(items);
        }

        private String RenderColumnTitle(String title, WBColumn column)
        {
            String directionArrow = "";
            String newDirectionParameter = "Ascending";

            if (ViewState["SortColumn"].WBxToString() == column.InternalName)
            {
                if (ViewState["SortDirection"].WBxToString() == "Descending")
                {
                    directionArrow = " <nobr>\\/</nobr>";
                    newDirectionParameter = "Ascending";
                }
                else
                {
                    directionArrow = " <nobr>/\\</nobr>";
                    newDirectionParameter = "Descending";
                }
            }

            String javascript = "WBF_sort_our_records('" + selectedPath + "', '" + column.InternalName + "', '" + newDirectionParameter + "'); "; 

            String html = "<a href=\"#\" onclick=\"" + javascript + "\">" + title + directionArrow + "</a>";

            return html;
        }


        private void RenderFoundRecords(SPListItemCollection items)
        {
            if (items == null)
            {
                FoundRecords.Text = "<i>items was null!</i>";
                return;
            }

            if (items.Count == 0)
            {
                FoundRecords.Text = "<i>No records found</i>";
                return;
            }

            String html = "<table class='wbf-record-series-details'>\n";

            html += "<tr>"
+ "<th class='wbf-record-series-odd'></th>"
+ "<th class='wbf-record-series-odd'>" + RenderColumnTitle("Title", WBColumn.Title) + "</th>"
+ "<th class='wbf-record-series-even'>" + RenderColumnTitle("Filename", WBColumn.Name) + "</th>"
+ "<th class='wbf-record-series-odd'>Version</th>"
+ "<th class='wbf-record-series-even'>" + RenderColumnTitle("Protective Zone", WBColumn.ProtectiveZone) + "</th>"
+ "<th class='wbf-record-series-odd'>" + RenderColumnTitle("Published Date", WBColumn.DatePublished) + "</th>"
+ "<th class='wbf-record-series-even'>" + RenderColumnTitle("Published By", WBColumn.PublishedBy) + "</th>"
+ "<th class='wbf-record-series-even'></th>"
+ "</tr>\n";

            int countViewableItems = 0;

            foreach (SPListItem item in items)
            {
                if (ItemCanBePicked(item))
                {
                    countViewableItems++;

                    WBDocument document = new WBDocument(manager.Libraries.ProtectedMasterLibrary, item);
                    document.CheckAndFixMetadataForRecord();

                    String publishedDateString = "";
                    if (document.Item.WBxHasValue(WBColumn.DatePublished))
                    {
                        publishedDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.DatePublished]);
                    }
                    if (publishedDateString == "" && document.Item.WBxHasValue(WBColumn.Modified))
                    {
                        publishedDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.Modified]);
                    }

                    String publishedByString = "<unknown>";
                    SPUser publishedBy = document.GetSingleUserColumn(WBColumn.PublishedBy);

                    if (publishedBy != null)
                    {
                        publishedByString = publishedBy.Name;
                    }
                    else
                    {
                        // If the published by column isn't set then we'll use the author column as a backup value:
                        publishedBy = document.GetSingleUserColumn(WBColumn.Author);
                        if (publishedBy != null)
                        {
                            publishedByString = publishedBy.Name;
                        }
                    }

                    long fileLength = (document.Item.File.Length / 1024);
                    if (fileLength == 0) fileLength = 1;
                    String fileLengthString = "" + fileLength + " KB";

                    String version = document.RecordSeriesIssue.WBxTrim();
                    if (String.IsNullOrEmpty(version))
                    {
                        version = "1";
                    }

                    if (masterLibraryHasVersions)
                    {
                        version += "." + (item.Versions.Count - 1).ToString();
                    }

                    html += "<tr>"
                        + "<td class='wbf-record-series-summary-detail'><input type='checkbox' class='wbf-our-records-check-boxes' data-record-id='" + document.RecordSeriesID + "x" + document.RecordID + "' onclick=\"WBF_checkbox_changed(event);\"/></td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.Title + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.Filename + "</td>"
                        + "<td class='wbf-record-series-summary-detail wbf-centre'>" + version + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.ProtectiveZone + "</td>"
                        + "<td class='wbf-record-series-summary-detail wbf-centre'>" + publishedDateString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + publishedByString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'><a href='#' onclick='WorkBoxFramework_viewRecordSeriesDetails(\"" + document.RecordSeriesID + "\", \"" + document.RecordID + "\");'>view details</a></td>"
                        + "</tr>";
                }
            }

            if (countViewableItems == 0)
            {
                FoundRecords.Text = "<i>No suitable records found</i><!-- number of unsuitable records = " + items.Count + " -->";
                return;
            }

            html += "\n</table>";

            FoundRecords.Text = html;

            // This should attach the right function to the checkboxes
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AttachChangeListeners", "$(function () { WBF_add_checkbox_change_function(); });", true);            
        }

        public SPListItemCollection GetRecordsInFolder(String folderPath)
        {
            WBColumn sortColumn = WBColumn.GetKnownColumnByInternalName(ViewState["SortColumn"].WBxToString());

            WBQuery workBoxQuery = new WBQuery();
            workBoxQuery.FilterByFolderPath = folderPath;

            if (sortColumn != null)
            {
                bool ascending = true;
                if (ViewState["SortDirection"].WBxToString() == "Descending") ascending = false;

                workBoxQuery.OrderBy(sortColumn, ascending);
            }

            workBoxQuery.RecursiveAll = true;

            SPList list = manager.Libraries.ProtectedMasterLibrary.List;

            return list.WBxGetItems(manager.Libraries.ProtectedMasterLibrary.Site, workBoxQuery);
        }

        private bool ItemCanBePicked(SPListItem item)
        {
            if (item == null) return false;

            if (String.IsNullOrEmpty(item.WBxGetAsString(WBColumn.RecordID))) return false;
            if (item.WBxGetAsString(WBColumn.LiveOrArchived) == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED) return false;

            String recordSeriesStatus = item.WBxGetAsString(WBColumn.RecordSeriesStatus);
            if (recordSeriesStatus == "Latest" || String.IsNullOrEmpty(recordSeriesStatus)) return true;

            return false;
        }

    }
}
