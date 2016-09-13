#region Copyright and License

// Copyright (c) Islington Council 2010-2016
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
using System.IO;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocSelfApprove : WorkBoxDialogPageBase
    {
        protected List<SPListItem> ListItems;

        WBRecordsManager manager = null;
        WBRecord recordBeingReplaced = null;

        SPListItem sourceDocAsItem = null;
        SPFile sourceFile = null;
        string destinationType = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Generic.Verbose("In Page_Load for the public doc metadata dialog");

            manager = new WBRecordsManager();

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {
                ListGUID.Value = Request.QueryString["ListGUID"];
                ItemID.Value = Request.QueryString["ItemID"];

                // The following variable has its name due to a strange compliation error with the name 'DestinationType' 
                TheDestinationType.Value = Request.QueryString["DestinationType"];
                DestinationURL.Value = Request.QueryString["DestinationURL"];
                DestinationTitle.Text = Request.QueryString["DestinationTitle"];

                ToReplaceRecordID.Value = Request.QueryString["ToReplaceRecordID"];
                ToReplaceRecordPath.Value = Request.QueryString["ToReplaceRecordPath"];
                NewOrReplace.Value = Request.QueryString["NewOrReplace"];
                ReplacementAction.Value = Request.QueryString["ReplacementAction"];

                WBLogging.Debug("NewOrReplace = " + NewOrReplace.Value);
                WBLogging.Debug("ReplacementAction = " + ReplacementAction.Value);
                WBLogging.Debug("ToReplaceRecordID = " + ToReplaceRecordID.Value);
                WBLogging.Debug("ToReplaceRecordPath = " + ToReplaceRecordPath.Value);

                WBLogging.Generic.Verbose("DestinationType = " + TheDestinationType.Value);
                WBLogging.Generic.Verbose("DestinationURL = " + DestinationURL.Value);
            }

            // Now do a check that we do at this stage have the basic details of the document:
            if (ListGUID.Value == null || ListGUID.Value == "")
            {
                errorMessage += "ListGUID hasn't been set. ";
            }

            if (ItemID.Value == null || ItemID.Value == "")
            {
                errorMessage += "ItemID hasn't been set. ";
            }

            if (TheDestinationType.Value == null || TheDestinationType.Value == "")
            {
                errorMessage += "DestinationType hasn't been set. ";
            }

            if (errorMessage.Length > 0)
            {
                ErrorMessageLabel.Text = errorMessage;
                return;
            }

            // Let's clear out all of the error messages text fields:
            ErrorMessageLabel.Text = "";


            //OK so we have the basic identity information for the document being published out so let's get the document item:

            Guid sourceListGuid = new Guid(ListGUID.Value);
            SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)WorkBox.Web.Lists[sourceListGuid];

            sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));
            sourceFile = sourceDocAsItem.File;

            WBDocument sourceDocument = new WBDocument(WorkBox, sourceDocAsItem);

            if (!IsPostBack)
            {
                SourceDocIcon.AlternateText = "Icon of document being publishing out.";
                SourceDocIcon.ImageUrl = WBUtils.DocumentIcon32(sourceDocAsItem.Url);

                ReadOnlyNameField.Text = sourceDocAsItem.Name;

                 DocumentType.Text = manager.PrettyNameForFileType(sourceDocument.FileType);
                WBLogging.Debug("The file type of the record is: " + DocumentType.Text);
            }

        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (manager != null)
            {
                manager.Dispose();
                manager = null;
            }
        }


        protected void publishButton_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("In publishButton_OnClick()");

                WBLogging.Debug("In publishButton_OnClick(): No page render required - so moving to publish");

                // The event should only be processed if there is no other need to render the page again

                // First let's update the item with the new metadata values submitted:
                SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)SPContext.Current.Web.Lists[new Guid(ListGUID.Value)];
                SPListItem sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));

                WBDocument document = new WBDocument(WorkBox, sourceDocAsItem);

                /*
                 * 
                 *   OK So now we actually publish out the document:
                 * 
                 */

                WBItem selfApprovalMetadata = new WBItem();

                selfApprovalMetadata[WBColumn.PublishingApprovalChecklist] = "Test value";
                selfApprovalMetadata[WBColumn.PublishingApprovedBy] = PublishingApprovedBy.WBxGetMultiResolvedUsers(SPContext.Current.Web);
                selfApprovalMetadata[WBColumn.PublishingApprovalStatement] = PublishingApprovalStatement.Text;

                SPUser currentUser = SPContext.Current.Web.CurrentUser;
                WBLogging.Debug("Current user: " + currentUser);
                selfApprovalMetadata[WBColumn.PublishedBy] = currentUser;
                selfApprovalMetadata[WBColumn.DatePublished] = DateTime.Now;

                SPFile sourceFile = sourceDocAsItem.File;
                string errorMessage = "";

                string successMessage = "<h3>Successfully Published Out</h3> <table cellpadding='5'>";

                    try
                    {
                        WBLogging.Debug("In publishButton_OnClick(): About to try to publish replacing: " + ToReplaceRecordID.Value + " with action: " + ReplacementAction.Value);

                        manager.PublishDocument(WorkBox, document, ToReplaceRecordID.Value, ReplacementAction.Value, selfApprovalMetadata);

                        WBLogging.Debug("In publishButton_OnClick(): Should have finished the publishing");

                        WBRecord record = manager.Libraries.GetRecordByID(document.RecordID);

                        WBLogging.Debug("In publishButton_OnClick():got the record object");

                        //recordsType.PublishDocument(document, sourceFile.OpenBinaryStream());

                        string fullClassPath = record.ProtectedMasterRecord.LibraryRelativePath; //  WBUtils.NormalisePath(document.FunctionalArea.Names() + "/" + recordsType.FullPath);

                        successMessage += "<tr><td>Published out to location:</td><td>" + fullClassPath + "</td></tr>\n";

                        /*
                        if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                        {
                            successMessage += "<tr><td>To public records library</td><td><a href=\"http://stagingweb/publicrecords\">Our public library</a></td></tr>\n";
                        }

                        if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                        {
                            successMessage += "<tr><td>To public extranet records library</td><td><a href=\"http://stagingextranets/records\">Our public extranet library</a></td></tr>\n";
                        }

                        successMessage += "<tr><td>To internal records library</td><td><a href=\"http://sp.izzi/library/Pages/ViewByFunctionThenType.aspx\">Our internal library</a></td></tr>\n";
                        */
                    }
                    catch (Exception exception)
                    {
                        errorMessage = "An error occurred when trying to publish: " + exception.Message;
                        WBLogging.Generic.Unexpected(exception);
                    }

                successMessage += "</table>";

                if (errorMessage == "")
                {
                    //returnFromDialogOKAndRefresh();
                    GoToGenericOKPage("Publishing Out Success", successMessage);
                }
                else
                {
                    GoToGenericOKPage("Publishing Out Error", errorMessage);

                    //returnFromDialogOK("An error occurred during publishing: " + errorMessage);
                }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }

    }
}
