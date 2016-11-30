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
using WorkBoxFramework;

namespace WBFWebParts.Layouts.WBFWebParts
{
    public partial class PublishedDocumentPicker : WBDialogPageBase
    {

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
            if (!IsPostBack)
            {
                CallingRowIndex.Value = Request.QueryString["RowIndex"];
                SelectedDocumentName.Text = "(none selected yet)";
            }

            SelectedView = VIEW_BY_FUNCTION_THEN_TYPE;

            recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            teamsTaxonomy = WBTaxonomy.GetTeams(recordsTypesTaxonomy);
            functionalAreaTaxonomy = WBTaxonomy.GetFunctionalAreas(recordsTypesTaxonomy);
            subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(recordsTypesTaxonomy);

            RefreshBrowsableTreeView();

            ShowResults.AllowSorting = true;
            ShowResults.Sorting += new GridViewSortEventHandler(ShowResults_Sorting);

            ShowResults.AllowPaging = true;
            ShowResults.PageIndexChanging += new GridViewPageEventHandler(ShowResults_PageIndexChanging);
            ShowResults.PagerSettings.Mode = PagerButtons.Numeric;
            ShowResults.PagerSettings.Position = PagerPosition.Bottom;
            ShowResults.PagerSettings.PageButtonCount = 50;
            ShowResults.PagerSettings.Visible = true;
            ShowResults.PageSize = 20;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            ShowResults.PagerTemplate = null;



        }


        protected void PickDocument_OnClick(object sender, EventArgs e)
        {
            // If nothing has been selected then 'Save' will behave like a cancel:
            if (String.IsNullOrEmpty(SelectedDocumentDetails.Value))
            {
                returnFromDialogCancel("");
            }
            else
            {
                returnFromDialogOK(CallingRowIndex.Value + "#" + SelectedDocumentDetails.Value);
            }

        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
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
        }


        private TreeNode AddFunctionBranch(Term functionTerm)
        {
            TreeNode functionNode = new TreeNode(functionTerm.Name, functionTerm.Name, "/_layouts/Images/FOLDER.GIF");
            //WBLogging.Debug("Adding the funciton node: " + functionTerm.Name);
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

            //WBLogging.Debug("Adding the subject node: " + subjectTerm.Name);
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
            //WBLogging.Debug("Adding the records type node: " + recordsType.Name);

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


        /*
        protected void ViewSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedViewTitle.Text = "Browse " + webPart.RecordsLibraryView; // ViewSelector.SelectedValue;
            // SelectedView = ViewSelector.SelectedValue;

            SelectedNodePath = "";

            RefreshBrowsableTreeView();
            RefreshBoundData();

        }
         */ 

        protected void FilterByProtectiveZone_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ProtectiveZoneFilter = FilterByProtectiveZone.SelectedValue;

            RefreshBoundData();
        }




        private void RefreshBoundData()
        {
            if (SelectedNodePath != "")
            {
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
                                WBLogging.Debug("Did not find subject with path: " + SelectedNodePath);
                                return;
                            }
                            break;
                        }


                    default: return;

                }


                WBTeam team = WBTeam.GetFromTeamSite(teamsTaxonomy, SPContext.Current.Web);

                String recordsLibraryURL = WBFWebPartsUtils.GetRecordsLibraryURL(SPContext.Current.Site);

                if (Request.QueryString["Library"] == "Extranet")
                {
                    recordsLibraryURL = WBFWebPartsUtils.GetExtranetLibraryURL(SPContext.Current.Site); 
                }

                using (SPSite site = new SPSite(recordsLibraryURL))
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
                            query.AddEqualsFilter(WBColumn.InvolvedTeams, team);
                        }

                        //                        string protectiveZoneFilter = "Public"; // Request.QueryString["ProtectiveZone"];
                        //                      if (protectiveZoneFilter != null && protectiveZoneFilter != "")
                        //                    {
                        //                      query.AddEqualsFilter(WBColumn.ProtectiveZone, protectiveZoneFilter);
                        //                }


                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.FileSize);
                        query.AddViewColumn(WBColumn.FileTypeIcon);
                        query.AddViewColumn(WBColumn.FileType);
                        query.AddViewColumn(WBColumn.TitleOrName);
                        query.AddViewColumn(WBColumn.DisplayFileSize);
                        query.AddViewColumn(WBColumn.RecordID);
                        query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
                        query.AddViewColumn(WBColumn.ReferenceDate);
                        query.AddViewColumn(WBColumn.ReferenceID);
                        query.AddViewColumn(WBColumn.ProtectiveZone);
                        query.AddViewColumn(WBColumn.DeclaredRecord);

                        if (SelectedView != VIEW_BY_SUBJECT)
                        {
                            query.AddViewColumn(WBColumn.SubjectTags);
                        }
                        else
                        {
                            query.AddViewColumn(WBColumn.RecordsType);
                        }

                        if (sortColumn == null)
                        {
                            sortColumn = WBColumn.DeclaredRecord;
                            ascending = false;
                        }

                        if (sortColumn != null)
                            query.OrderBy(sortColumn, ascending);

                        SPList recordsLibrary = web.GetList(recordsLibraryURL); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        DataTable dataTable = recordsLibrary.WBxGetDataTable(site, query);

                        ShowResults.DataSource = dataTable;

                        ShowResults.Columns.Clear();

                        ButtonField buttonField = new ButtonField();
                        buttonField.Text = "Select";
                        buttonField.CommandName = "Select Document";
                        ShowResults.Columns.Add(buttonField);

                        ShowResults.Columns.Add(WBUtils.DynamicIconTemplateField(WBColumn.FileTypeIcon, WBColumn.EncodedAbsoluteURL));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.Title, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.Name, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.DeclaredRecord, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ProtectiveZone, sortColumn, ascending));
                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.RecordID, sortColumn, ascending));

//                        BoundField test = WBUtils.BoundField(WBColumn.ServerURL, sortColumn, ascending);
  //                      test.Hid

//                        ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.FunctionalArea, sortColumn, ascending));
                        //ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.OwningTeam, sortColumn, ascending));
                      //  ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));
                      //  ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceID, sortColumn, ascending));
                        //ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SeriesTag, sortColumn, ascending));
                        
                        //if (SelectedView != VIEW_BY_SUBJECT)
                       // {
                       //     ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.SubjectTags, sortColumn, ascending));
                       // }
                        //else
                       // {
                        //    ShowResults.Columns.Add(WBUtils.BoundField(WBColumn.RecordsType, sortColumn, ascending));
                       // }



                        ShowResults.DataBind();

                    }
                }
            }

            else
            {
                WBUtils.logMessage("SelectedNodePath was empty");
            }

        }


        public void ShowResults_RowCommand(Object sender, GridViewCommandEventArgs e)
        {

            WBLogging.Debug("In ShowResults_RowCommand");

            // If multiple ButtonField column fields are used, use the
            // CommandName property to determine which button was clicked.
           // if (e.CommandName == "Select Documents")
           // {

                // Convert the row index stored in the CommandArgument
                // property to an Integer.
                int index = Convert.ToInt32(e.CommandArgument);

                // Get the last name of the selected author from the appropriate
                // cell in the GridView control.
                GridViewRow selectedRow = ShowResults.Rows[index];
                TableCell recordIDCell = selectedRow.Cells[6];
                string recordID = recordIDCell.Text;

                TableCell protectizeZoneCell = selectedRow.Cells[5];
                string protectiveZone = protectizeZoneCell.Text;

                TableCell nameCell = selectedRow.Cells[3];
                string selectedName = nameCell.Text.WBxTrim().Replace(";","-");

                WBLogging.Debug("Index was: " + index + " recordID = " + recordID);

                string pickedDocumentDetails = String.Format("{0}|{1}|{2}|{3}",
                    protectiveZone, recordID, "Ignore", selectedName
                    );

                WBLogging.Debug("Picked documents details are being set to: " + pickedDocumentDetails);

                SelectedDocumentDetails.Value = pickedDocumentDetails;
                SelectedDocumentName.Text = selectedName;

                //returnFromDialogOK(CallingRowIndex.Value + "#" + pickedDocumentDetails);
               
           // }

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
