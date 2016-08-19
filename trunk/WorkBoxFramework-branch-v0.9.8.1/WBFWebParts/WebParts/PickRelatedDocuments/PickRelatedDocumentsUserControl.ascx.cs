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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using WorkBoxFramework;

namespace WBFWebParts.PickRelatedDocuments
{
    public partial class PickRelatedDocumentsUserControl : UserControl
    {
        protected PickRelatedDocuments webPart = default(PickRelatedDocuments);

        public String WebPartUniqueID = "";
        public bool InEditMode = false;
        public bool DocumentsToView = false;
        public bool showDescription = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as PickRelatedDocuments;

            SPWebPartManager webPartManager = (SPWebPartManager)WebPartManager.GetCurrentWebPartManager(this.Page);

            Guid WebPartGuid = webPartManager.GetStorageKey(webPart);            

            WebPartUniqueID = WebPartGuid.ToString().Replace("-", String.Empty); ;

            //EditRelatedDocumentsButton.OnClientClick = "WorkBoxFramework_editRelatedDocuments(WBF_EditDialogCallback" + WebPartUniqueID + ", \"" + webPart.PickedDocumentsDetails + "\"); return false;";

            String[] detailsToSave = new String[3];

            detailsToSave[0] = WBUtils.ReplaceDelimiterCharacters(webPart.Title);
            detailsToSave[1] = WBUtils.ReplaceDelimiterCharacters(webPart.RelatedDocumentsDescription);
            detailsToSave[2] = WBUtils.ReplaceDelimiterCharacters(webPart.PickedDocumentsDetails.WBxTrim());

            String currentDetails = String.Join(",", detailsToSave);
            String pickedDocumentsDetails = webPart.PickedDocumentsDetails.WBxTrim();


            if (IsPostBack)
            {
                if (NeedToSave.Value == "true")
                {

                    string[] newDetails = NewRelatedDocumentsDetails.Value.WBxTrim().Split(',');

                    if (newDetails.Length != 3)
                    {
                        WBLogging.Debug("The details sent to this page have the wrong structure: " + NewRelatedDocumentsDetails.Value);
                        Description.Text = "(the web part has not yet been edited).";
                        return;
                    }

                    if (WBFWebPartsUtils.ShowDescription(SPContext.Current.Site))
                    {
                        webPart.Title = WBUtils.PutBackDelimiterCharacters(newDetails[0]);
                        webPart.RelatedDocumentsDescription = WBUtils.PutBackDelimiterCharacters(newDetails[1]);
                    }
                    webPart.PickedDocumentsDetails = WBUtils.PutBackDelimiterCharacters(newDetails[2]);

                    webPartManager.SaveChanges(WebPartGuid);

                    SPContext.Current.File.Update();
                    SPContext.Current.Web.Update();

                    currentDetails = NewRelatedDocumentsDetails.Value.WBxTrim();
                    pickedDocumentsDetails = webPart.PickedDocumentsDetails;

                }
            }

            if (!String.IsNullOrEmpty(currentDetails) && !currentDetails.Contains(","))
            {
                WBLogging.Generic.Unexpected("The PickRelatedDocuments web part had an odd value: " + currentDetails);
                currentDetails = "";
            }

            Description.Text = WBUtils.MaybeAddParagraphTags(webPart.RelatedDocumentsDescription);
            if (!String.IsNullOrEmpty(Description.Text) && WBFWebPartsUtils.ShowDescription(SPContext.Current.Site))
            {
                showDescription = true;
            }


            if ((SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                  || (webPartManager.DisplayMode == WebPartManager.EditDisplayMode))
            {
                EditPanel.Visible = true;
                InEditMode = true;
                EditRelatedDocumentsButton.OnClientClick = "WorkBoxFramework_editRelatedDocuments(WBF_EditDialogCallback" + WebPartUniqueID + ", \"" + stripDownDetailsForEditing(currentDetails) + "\"); return false;";
            }
            else
            {
                EditPanel.Visible = false;
                EditRelatedDocumentsButton.OnClientClick = "";
            }

            

            WBLogging.Debug("PickRelatedDocuments currentDetails: " + currentDetails);


            if (String.IsNullOrEmpty(pickedDocumentsDetails))
            {
                DocumentList.Text = "<ul><li>(No documents picked)</li></ul>";
                DocumentsToView = false;
                return;
            }

            try
            {
                string[] documentsDetailsArray = pickedDocumentsDetails.Split(';');

                String recordsLibraryURL = WBFWebPartsUtils.GetRecordsLibraryURL(SPContext.Current.Site);

                using (SPSite site = new SPSite(recordsLibraryURL))
                using (SPWeb web = site.OpenWeb())
                {
                    SPList library = web.GetList(recordsLibraryURL);

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

                        SPListItem item = WBFWebPartsUtils.GetRecord(site, web, library, zone, recordID);

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

                            if (WBFWebPartsUtils.ShowKBFileSize(SPContext.Current.Site))
                            {
                                long fileLength = (item.File.Length / 1024);
                                additionalText = ", " + fileLength + "KB";
                            }

                            if (WBFWebPartsUtils.ShowFileIcons(SPContext.Current.Site))
                            {
                                title = String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"wbf-picked-doc-image\"/> {1}",
                                    WBUtils.DocumentIcon16(item.Name),
                                    title);
                            }

                            html += "<li><a target=\"_blank\" href=\"" + item.WBxGetAsString(WBColumn.EncodedAbsoluteURL) + "\">" + title + "</a> <span>(" + extension + additionalText + ")</span></li>";
                        }
                    }

                    html += "</ul>\n";

                    DocumentList.Text = html;


                }
            }
            catch (Exception exception)
            {
                if (InEditMode)
                {
                    DocumentList.Text = "An error occurred: " + exception.Message;
                }
                else
                {
                    DocumentsToView = false;
                }
            }





        }

        private String stripDownDetailsForEditing(String details)
        {
            WBLogging.Debug("Stripping down details from: " + details);

            string[] parts = details.Split(',');

            String docDetails = WBUtils.PutBackDelimiterCharacters(parts[2]);

            if (!String.IsNullOrEmpty(docDetails) && docDetails.Contains("|"))
            {
                string[] documentDetails = docDetails.Split(';');

                List<String> newDocumentDetails = new List<String>();

                foreach (String oneDocDetails in documentDetails)
                {
                    string[] oneDocParts = oneDocDetails.Split('|');

                    newDocumentDetails.Add(oneDocParts[0] + "|" + oneDocParts[1]);
                }

                parts[2] = WBUtils.ReplaceDelimiterCharacters(String.Join(";", newDocumentDetails.ToArray()));
            }


            return String.Join(",", parts);
        }

        private String makeJavascriptForEditing()
        {
            StringBuilder script = new StringBuilder();

            script.Append("not done yet");


            return script.ToString();
        }
    }
}
