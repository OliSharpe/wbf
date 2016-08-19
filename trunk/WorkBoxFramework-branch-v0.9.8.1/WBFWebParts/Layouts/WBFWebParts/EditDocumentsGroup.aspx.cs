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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using WorkBoxFramework;

namespace WBFWebParts.Layouts.WBFWebParts
{
    public partial class EditDocumentGroup : WBDialogPageBase
    {
        private int numOfDocs = 0;
        private List<TableRow> allRows = new List<TableRow>();

        private String newSubjectTagsUIControlValue = "";

        private SPSite extranetRecordsSite = null;
        private SPWeb extranetRecordsWeb = null;
        private SPList extranetRecordsLibrary = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String allDetails = Request.QueryString["CurrentDetails"];

                string[] details = allDetails.Split(',');

                if (details.Length != 5)
                {
                    WBLogging.Debug("The details sent to this page have the wrong structure: " + allDetails);
                    ErrorMessage.Text = "There was a problem with the data sent to this page.";
                    return;
                }

                EditTitle.Text = WBUtils.PutBackDelimiterCharacters(details[0]);
                EditDescription.Text = WBUtils.PutBackDelimiterCharacters(details[1]);
                newSubjectTagsUIControlValue = WBUtils.PutBackDelimiterCharacters(details[2]);
                EditCoverage.Text = WBUtils.PutBackDelimiterCharacters(details[3]);
                DocumentsDetails.Value = WBUtils.PutBackDelimiterCharacters(details[4]);
            }

            List<String> documentsDetailsList = new List<String>();

            string currentDetails = DocumentsDetails.Value.WBxTrim();

            if (!String.IsNullOrEmpty(currentDetails))
            {
                documentsDetailsList = new List<String>(currentDetails.Split(';'));
            }

            if (IsPostBack)
            {
                if (!String.IsNullOrEmpty(ReplaceRowIndex.Value))
                {
                    int rowIndex = Convert.ToInt32(ReplaceRowIndex.Value);
                    ReplaceRowIndex.Value = "";

                    if (rowIndex == -1)
                    {
                        documentsDetailsList.Add(ReplacementDetails.Value);
                    }
                    else
                    {
                        documentsDetailsList.RemoveAt(rowIndex);
                        documentsDetailsList.Insert(rowIndex, ReplacementDetails.Value);
                    }

                    DocumentsDetails.Value = String.Join(";", documentsDetailsList.ToArray());
                }


                if (!String.IsNullOrEmpty(DeleteRowIndex.Value))
                {
                    int rowIndex = Convert.ToInt32(DeleteRowIndex.Value);
                    DeleteRowIndex.Value = "";

                    documentsDetailsList.RemoveAt(rowIndex);

                    DocumentsDetails.Value = String.Join(";", documentsDetailsList.ToArray());
                }
            }

            RenderPage();
        }


        private void RenderPage()
        {
            WBTaxonomy subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(SPContext.Current.Site);
            subjectTagsTaxonomy.InitialiseTaxonomyControl(SubjectTagsField, WorkBox.COLUMN_NAME__SUBJECT_TAGS, true);

            if (!String.IsNullOrEmpty(newSubjectTagsUIControlValue)) 
            {
                SubjectTagsField.Text = newSubjectTagsUIControlValue;
            }

            EditDocumentsTable.Controls.Clear();

            List<String> documentsDetailsList = new List<String>();

            string currentDetails = DocumentsDetails.Value.WBxTrim();

            WBLogging.Debug("Building the table with current details: " + currentDetails);

            if (!String.IsNullOrEmpty(currentDetails))
            {
                documentsDetailsList = new List<String>(currentDetails.Split(';'));
            }

            numOfDocs = documentsDetailsList.Count;

            Table table = new Table();
            table.Width = Unit.Percentage(100);

            TableRow headers = new TableRow();
            headers.WBxAddTableHeaderCell("Title");
            headers.WBxAddTableHeaderCell("Filename");
            headers.WBxAddTableHeaderCell("File Type");
            //            headers.WBxAddTableHeaderCell("Else");

            table.Rows.Add(headers);

            String recordsLibraryURL = WBFWebPartsUtils.GetRecordsLibraryURL(SPContext.Current.Site);
            using (SPSite site = new SPSite(recordsLibraryURL))
            using (SPWeb web = site.OpenWeb())
            {
                SPList library = web.GetList(recordsLibraryURL);



                int index = 0;
                foreach (String details in documentsDetailsList)
                {
                    TableRow row = CreateEditableTableRow(site, web, library, index, details);

                    allRows.Add(row);

                    table.Rows.Add(row);
                    index++;
                }


                if (extranetRecordsWeb != null) extranetRecordsWeb.Dispose();
                if (extranetRecordsSite != null) extranetRecordsSite.Dispose();

            }
            EditDocumentsTable.Controls.Add(table);
        }


        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            String[] detailsToSave = new String[5];

            detailsToSave[0] = WBUtils.ReplaceDelimiterCharacters(EditTitle.Text);
            detailsToSave[1] = WBUtils.ReplaceDelimiterCharacters(EditDescription.Text);
            detailsToSave[2] = WBUtils.ReplaceDelimiterCharacters(SubjectTagsField.Text);
            detailsToSave[3] = WBUtils.ReplaceDelimiterCharacters(EditCoverage.Text);
            detailsToSave[4] = WBUtils.ReplaceDelimiterCharacters(DocumentsDetails.Value);

            returnFromDialogOK(String.Join(",", detailsToSave));
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }


        public TableRow CreateEditableTableRow(SPSite site, SPWeb web, SPList library, int index, String details)
        {
            TableRow row = new TableRow();
            row.ID = MakeControlID(index, "row");
            row.CssClass = "wbf-edit-action-row";

            string[] parts = details.Split('|');

            string zone = parts[0];
            string recordID = parts[1];

            SPListItem recordItem = null;

            if (zone == "Public Extranet")
            {
                if (extranetRecordsLibrary == null)
                {
                    string extranetRecordsLibraryURL = WBFWebPartsUtils.GetExtranetLibraryURL(SPContext.Current.Site);
                    extranetRecordsSite = new SPSite(extranetRecordsLibraryURL);
                    extranetRecordsWeb = extranetRecordsSite.OpenWeb();
                    extranetRecordsLibrary = extranetRecordsWeb.GetList(extranetRecordsLibraryURL);
                }

                recordItem = WBFWebPartsUtils.GetRecord(extranetRecordsSite, extranetRecordsWeb, extranetRecordsLibrary, zone, recordID);
            }
            else
            {
                recordItem = WBFWebPartsUtils.GetRecord(site, web, library, zone, recordID);
            }


            if (recordItem == null) return row;

            String displayName = recordItem.WBxGetAsString(WBColumn.Title);
            if (String.IsNullOrEmpty(displayName)) displayName = recordItem.WBxGetAsString(WBColumn.Name);

            string extension = Path.GetExtension(recordItem.Name).Replace(".", "").ToUpper();

            Image image = new Image();
            image.ImageUrl = WBUtils.DocumentIcon16(recordItem.Name);
            image.Width = Unit.Pixel(16);
            image.Height = Unit.Pixel(16);
            row.WBxAddInTableCell(image);


            Label label = new Label();
            label.Text = recordItem.WBxGetAsString(WBColumn.Title);
            row.WBxAddInTableCell(label);

            label = new Label();
            label.Text = recordItem.WBxGetAsString(WBColumn.Name);
            row.WBxAddInTableCell(label);


            Label extensionLabel = new Label();
            extensionLabel.Text = extension;
            row.WBxAddInTableCell(extensionLabel);


            Button upButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "UpButton"));
            upButton.Text = "/\\";
            upButton.CommandName = "Up";
            upButton.CommandArgument = index.ToString();
            upButton.Command += new CommandEventHandler(upButton_OnClick);

            Button downButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "DownButton"));
            downButton.Text = "\\/";
            downButton.CommandName = "Down";
            downButton.CommandArgument = index.ToString();
            downButton.Command += new CommandEventHandler(downButton_OnClick);


            Button replaceButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "ReplaceButton"));
            replaceButton.Text = "Replace";
            replaceButton.OnClientClick = "WorkBoxFramework_pickADocument(" + index + "); return false;";

            Button removeButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "RemoveButton"));
            removeButton.Text = "Remove";
            removeButton.OnClientClick = "WBF_DeleteRow(" + index + ",\"" + recordItem.Name + "\"); return false;";

            HiddenField documentDetails = (HiddenField)row.WBxAddWithIDInTableCell(new HiddenField(), MakeControlID(index, "DocumentDetails"));


            if (!IsPostBack)
            {
                documentDetails.Value = details;
            }

            return row;
        }


        protected void upButton_OnClick(object sender, CommandEventArgs e)
        {

            string currentDetails = DocumentsDetails.Value.WBxTrim();

            List<String> documentsDetailsList = new List<String>();

            if (!String.IsNullOrEmpty(currentDetails))
            {
                documentsDetailsList = new List<String>(currentDetails.Split(';'));
            }

            if (!String.IsNullOrEmpty(e.CommandArgument.WBxToString()))
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.WBxToString());


                if (rowIndex > 0)
                {
                    String valueToMove = documentsDetailsList[rowIndex];

                    documentsDetailsList.RemoveAt(rowIndex);
                    rowIndex--;
                    documentsDetailsList.Insert(rowIndex, valueToMove);

                    DocumentsDetails.Value = String.Join(";", documentsDetailsList.ToArray());
                }
            }

            RenderPage();
        }


        protected void downButton_OnClick(object sender, CommandEventArgs e)
        {

            string currentDetails = DocumentsDetails.Value.WBxTrim();

            WBLogging.Debug("Current details : " + currentDetails);

            List<String> documentsDetailsList = new List<String>();

            if (!String.IsNullOrEmpty(currentDetails))
            {
                documentsDetailsList = new List<String>(currentDetails.Split(';'));
            }

            if (!String.IsNullOrEmpty(e.CommandArgument.WBxToString()))
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.WBxToString());

                WBLogging.Debug("rowIndex = " + rowIndex);

                if (rowIndex < documentsDetailsList.Count - 1)
                {
                    String valueToMove = documentsDetailsList[rowIndex];

                    documentsDetailsList.RemoveAt(rowIndex);
                    rowIndex++;
                    documentsDetailsList.Insert(rowIndex, valueToMove);

                    DocumentsDetails.Value = String.Join(";", documentsDetailsList.ToArray());


                    WBLogging.Debug("Set current DocumentsDetails.Value =: " + DocumentsDetails.Value);

                }
            }

            RenderPage();
        }



        public String MakeControlID(int index, String innerName)
        {
            return this.WBxMakeControlID(index.ToString(), innerName);
        }


    }
}
