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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using WorkBoxFramework;

namespace WBFWebParts.PickedDocumentsGroup
{
    public partial class PickedDocumentsGroupUserControl : UserControl
    {
        protected PickedDocumentsGroup webPart = default(PickedDocumentsGroup);

        public String WebPartUniqueID = "";
        public bool InEditMode = false;
        public bool DocumentsToView = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as PickedDocumentsGroup;

            SPWebPartManager webPartManager = (SPWebPartManager)WebPartManager.GetCurrentWebPartManager(this.Page);

            Guid WebPartGuid = webPartManager.GetStorageKey(webPart);
            WebPartUniqueID = WebPartGuid.ToString().Replace("-", String.Empty); ;

            //EditRelatedDocumentsButton.OnClientClick = "WorkBoxFramework_editRelatedDocuments(WBF_EditDialogCallback" + WebPartUniqueID + ", \"" + webPart.PickedDocumentsDetails + "\"); return false;";


            String[] detailsToSave = new String[5];

            detailsToSave[0] = WBUtils.ReplaceDelimiterCharacters(webPart.Title);
            detailsToSave[1] = WBUtils.ReplaceDelimiterCharacters(webPart.DocumentsGroupDescription);
            detailsToSave[2] = WBUtils.ReplaceDelimiterCharacters(webPart.DocumentsGroupSubjectTags);
            detailsToSave[3] = WBUtils.ReplaceDelimiterCharacters(webPart.DocumentsGroupCoverage);
            detailsToSave[4] = WBUtils.ReplaceDelimiterCharacters(webPart.PickedDocumentsDetails.WBxTrim());

            String currentDetails = String.Join(",", detailsToSave);

            WBLogging.Debug("Current details: " + currentDetails);

            if (IsPostBack)
            {
                if (NeedToSave.Value == "true")
                {
                    WBLogging.Debug("Trying to save value: " + NewDocumentsGroupDetails.Value);

                    string[] newDetails = NewDocumentsGroupDetails.Value.WBxTrim().Split(',');

                    if (newDetails.Length != 5)
                    {
                        WBLogging.Debug("The details sent to this page have the wrong structure: " + NewDocumentsGroupDetails.Value);
                        Description.Text = "(the web part has not yet been edited).";
                        return;
                    }


                    webPart.Title = WBUtils.PutBackDelimiterCharacters(newDetails[0]);
                    webPart.DocumentsGroupDescription = WBUtils.PutBackDelimiterCharacters(newDetails[1]);
                    webPart.DocumentsGroupSubjectTags = WBUtils.PutBackDelimiterCharacters(newDetails[2]);
                    webPart.DocumentsGroupCoverage = WBUtils.PutBackDelimiterCharacters(newDetails[3]);
                    webPart.PickedDocumentsDetails = WBUtils.PutBackDelimiterCharacters(newDetails[4]);

                    webPartManager.SaveChanges(WebPartGuid);

                    SPContext.Current.File.Update();
                    SPContext.Current.Web.Update();

                    currentDetails = NewDocumentsGroupDetails.Value.WBxTrim();

                    WBLogging.Debug("New current details: " + currentDetails);

                }
            }

            if (!String.IsNullOrEmpty(currentDetails) && !currentDetails.Contains(","))
            {
                WBLogging.Generic.Unexpected("The PickRelatedDocuments web part had an odd value: " + currentDetails);
                currentDetails = "";
            }

            if ((SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                  || (webPartManager.DisplayMode == WebPartManager.EditDisplayMode))
            {
                EditPanel.Visible = true;
                InEditMode = true;
                EditDocumentsGroupButton.OnClientClick = "WorkBoxFramework_editDocumentsGroup(WBF_EditDialogCallback" + WebPartUniqueID + ", \"" + currentDetails + "\"); return false;";
            }
            else
            {
                EditPanel.Visible = false;
                EditDocumentsGroupButton.OnClientClick = "";
            }



            WBLogging.Debug("PickedDocumentsGroup currentDetails: " + currentDetails);


            Title.Text = webPart.Title;
            Description.Text = WBUtils.MaybeAddParagraphTags(webPart.DocumentsGroupDescription);

            WBTermCollection<WBTerm> allSubjects = new WBTermCollection<WBTerm>(null, webPart.DocumentsGroupSubjectTags);
            SubjectTags.Text = allSubjects.Names();
            Coverage.Text = webPart.DocumentsGroupCoverage;


            if (String.IsNullOrEmpty(webPart.PickedDocumentsDetails))
            {
                DocumentsList.Text = "<ul><li>(No documents picked)</li></ul>";
                DocumentsToView = false;
                return;
            }

            String extranetRecordsLibraryURL = WBFarm.Local.PublicExtranetRecordsLibraryUrl;

            SPSite extranetRecordsSite = null;
            SPWeb extranetRecordsWeb = null;
            SPList extranetRecordsLibrary = null;

            try
            {
                string[] documentsDetailsArray = webPart.PickedDocumentsDetails.Split(';');

                String publicRecordsLibraryURL = WBFWebPartsUtils.GetRecordsLibraryURL(SPContext.Current.Site);

                using (SPSite publicRecordsSite = new SPSite(publicRecordsLibraryURL))
                using (SPWeb publicRecordsWeb = publicRecordsSite.OpenWeb())
                {
                    SPList publicRecordsLibrary = publicRecordsWeb.GetList(publicRecordsLibraryURL);

                    String html = "<ul>\n";

                    foreach (string documentDetails in documentsDetailsArray)
                    {
                        string[] documentDetailsArray = documentDetails.Split('|');

                        if (documentDetailsArray.Length != 4)
                        {
                            WBLogging.Generic.Unexpected("Badly formatted document details in PickRelatedDocuments web part: " + currentDetails + " - Ignoring these details");
                            continue;
                        }

                        string zone = documentDetailsArray[0];
                        string recordID = documentDetailsArray[1];
                        string sourceID = documentDetailsArray[2];
                        string filename = documentDetailsArray[3];

                        SPListItem item = null;

                        if (zone == "Public Extranet")
                        {
                            if (extranetRecordsLibrary == null)
                            {
                                extranetRecordsSite = new SPSite(extranetRecordsLibraryURL);
                                extranetRecordsWeb = extranetRecordsSite.OpenWeb();
                                extranetRecordsLibrary = extranetRecordsWeb.GetList(extranetRecordsLibraryURL);
                            }

                            item = WBFWebPartsUtils.GetRecord(extranetRecordsSite, extranetRecordsWeb, extranetRecordsLibrary, zone, recordID);
                        }
                        else
                        {
                            item = WBFWebPartsUtils.GetRecord(publicRecordsSite, publicRecordsWeb, publicRecordsLibrary, zone, recordID);
                        }

                        if (item == null)
                        {
                            if (InEditMode)
                            {
                                html += "<li><i>(Could not find document)</i></li>";
                            }
                        }
                        else
                        {

                            DocumentsToView = true;

                            string title = item.WBxGetAsString(WBColumn.Title);
                            if (String.IsNullOrEmpty(title))
                                title = Path.GetFileNameWithoutExtension(item.Name);

                            string extension = Path.GetExtension(item.Name).Replace(".", "").ToUpper();

                            string additionalText = "";
                            long fileLength = (item.File.Length / 1024);
                            additionalText = ", " + fileLength + "KB";

                            if (item.WBxHasValue(WBColumn.ReferenceDate))
                            {
                                DateTime referenceDate = (DateTime)item.WBxGet(WBColumn.ReferenceDate);
                                string referenceDateString = string.Format("{0}-{1}-{2}",
                                    referenceDate.Year.ToString("D4"),
                                     referenceDate.Month.ToString("D2"),
                                    referenceDate.Day.ToString("D2"));

                                additionalText += ", " + referenceDateString;
                            }

                            html += String.Format("<li><img src=\"{0}\" alt=\"{1}\"/><a target=\"_blank\" href=\"{2}\">{1}</a> <span>({3})</span></li>",
                            WBUtils.DocumentIcon16(item.WBxGetAsString(WBColumn.Name)),
                            title,
                            item.WBxGetAsString(WBColumn.EncodedAbsoluteURL),
                            extension + additionalText);
                        }
                    }

                    html += "</ul>\n";

                    DocumentsList.Text = html;


                }
            }
            catch (Exception exception)
            {
                if (InEditMode)
                {
                    DocumentsList.Text = "An error occurred: " + exception.Message;
                }
                else
                {
                    DocumentsToView = false;
                }
            }
            finally
            {
                if (extranetRecordsWeb != null) extranetRecordsWeb.Dispose();
                if (extranetRecordsSite != null) extranetRecordsSite.Dispose();
            }


        }

        private String makeJavascriptForEditing()
        {
            StringBuilder script = new StringBuilder();

            script.Append("not done yet");


            return script.ToString();
        }
    }
}
