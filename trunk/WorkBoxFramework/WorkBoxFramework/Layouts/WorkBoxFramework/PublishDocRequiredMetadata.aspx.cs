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
using Newtonsoft.Json;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocRequiredMetadata : WorkBoxDialogPageBase
    {

        public WBPublishingProcess process = null;


        WBRecordsManager manager = null;
        WBRecord recordBeingReplaced = null;

        WBRecordsType documentRecordsType = null;
        WBTerm documentFunctionalArea = null;
        //SPListItem sourceDocAsItem = null;
        //SPFile sourceFile = null;
        string destinationType = "";

        protected bool functionalAreaFieldIsEditable = false;
        protected bool showReferenceID = false;
        protected bool showReferenceDate = false;
        protected bool showSeriesTag = false;
        protected bool showSubjectTags = true;
        protected bool showScanDate = false;

        protected bool showWebPageURL = false;

        protected bool showPublishAllButton = false;

        
        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Debug("In Page_Load for the public doc metadata dialog");

            manager = new WBRecordsManager();

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {

                process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(Request.QueryString["PublishingProcessJSON"]);
                process.WorkBox = WorkBox;

                WBLogging.Debug("Created the WBProcessObject");

                PublishingProcessJSON.Value = WBUtils.SerializeToCompressedJSONForURI(process);

                WBLogging.Debug("Serialized the WBProcessObject to hidden field");

                NewRadioButton.Checked = true;
                NewOrReplace.Text = "New";

                pageRenderingRequired = true;
            }
            else
            {
                process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(PublishingProcessJSON.Value.WBxTrim());
                process.WorkBox = WorkBox;

                CaptureChanges();

                // By default we should not be rendering the page on a post back call
                pageRenderingRequired = false;
            }

            if (errorMessage.Length > 0)
            {
                ErrorMessageLabel.Text = errorMessage;
                return;
            }

            // Let's clear out all of the error messages text fields:
            ErrorMessageLabel.Text = "";
            ReferenceIDMessage.Text = "";
            ReferenceDateMessage.Text = "";
            SeriesTagFieldMessage.Text = "";
            ScanDateMessage.Text = "";
            OwningTeamFieldMessage.Text = "";
            InvolvedTeamsFieldMessage.Text = "";
            PublishingLocationError.Text = "";
            ShortTitleError.Text = "";

            if (IsPostBack)
            {
                // If this is a post back - then let's check if the records type has been modified:
                if (!String.IsNullOrEmpty(UpdatedPublishingProcessJSON.Value))
                {
                    WBLogging.Generic.Unexpected("The returned value was: " + UpdatedPublishingProcessJSON.Value);

                    process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(UpdatedPublishingProcessJSON.Value.WBxTrim());
                    process.WorkBox = WorkBox;

                    CaptureChanges();

                    // Now set the title and subject tags from the record that is going to be replaced:
                    process.CurrentShortTitle = process.ToReplaceShortTitle;
                    process.SubjectTagsUIControlValue = process.ToReplaceSubjectTagsUIControlValue;

                    // Now blanking this hidden field so that it doesn't trigger a recapture each time!
                    UpdatedPublishingProcessJSON.Value = "";

                    pageRenderingRequired = true;

                }
                else
                {
                    // Otherwise we are in a normal post back call.
                    pageRenderingRequired = false;
                }
            }


            destinationType = process.ProtectiveZone;

            // Now load up some of the basic details:
            if (String.IsNullOrEmpty(process.RecordsTypeUIControlValue))
            {
                showReferenceID = false;
                showReferenceDate = false;
                showSubjectTags = true; 
                showSeriesTag = false;
                showScanDate = false;
            }
            else
            {
                documentRecordsType = new WBRecordsType(process.RecordsTypeTaxonomy, process.RecordsTypeUIControlValue);

                // Which of the metadata fields are being used in the form (or will need to be processed in any postback) :
                showReferenceID = documentRecordsType.DocumentReferenceIDRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
                showReferenceDate = documentRecordsType.DocumentReferenceDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
                showSubjectTags = true; // documentRecordsType.DocumentSubjectTagsRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
                showSeriesTag = documentRecordsType.DocumentSeriesTagRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
                showScanDate = documentRecordsType.DocumentScanDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            }

            if (!String.IsNullOrEmpty(process.FunctionalAreaUIControlValue))
            {
                documentFunctionalArea = new WBTerm(process.FunctionalAreasTaxonomy, process.FunctionalAreaUIControlValue);
            }
            else
            {
                documentFunctionalArea = null;
            }


            if (pageRenderingRequired)
            {
                WBLogging.Debug("In Page_Load calling RenderPage()");
                RenderPage();
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

        private void RenderPage()
        {
            if (process == null)
            {
                WBLogging.Debug("process == null");
                return;
            }

            DocumentsBeingPublished.Text = process.GetStandardHTMLTableRows();

            SPListItem currentItem = process.CurrentItem;

            EditShortTitle.Text = process.CurrentShortTitle; 
            ShortTitle.Text = process.CurrentShortTitle;

            WBLogging.Debug("Passed title / name");

            SelectLocationButton.OnClientClick = "WorkBoxFramework_pickANewLocation(WorkBoxFramework_PublishDoc_pickedANewLocation, '" + process.FunctionalAreaUIControlValue + "', '" + process.RecordsTypeUIControlValue + "'); return false;";


            if (!String.IsNullOrEmpty(process.ToReplaceRecordID) && process.ReplaceAction != WBPublishingProcess.REPLACE_ACTION__CREATE_NEW_SERIES)
            {
                recordBeingReplaced = manager.Libraries.GetRecordByID(process.ToReplaceRecordID);
                if (recordBeingReplaced == null)
                {
                    ErrorMessageLabel.Text = "Could not find the record that is meant to be replaced. Supposedly it has RecordID = " + process.ToReplaceRecordID;
                    return;
                }
            }

            if (recordBeingReplaced == null)
            {
                if (documentFunctionalArea != null && documentRecordsType != null)
                {
                    LocationPath.Text = documentFunctionalArea.Name + " / " + documentRecordsType.FullPath.Replace("/", " / ");
                }
                else
                {
                    LocationPath.Text = "<none>";
                }
            }
            else
            {
                LocationPath.Text = process.ToReplaceRecordPath;
            }

            if (recordBeingReplaced == null || process.ReplaceAction == WBPublishingProcess.REPLACE_ACTION__CREATE_NEW_SERIES)
            {
                WBLogging.Debug("Setting buttons etc for NEW");

                NewRadioButton.Checked = true;
                ReplaceRadioButton.Checked = false;
                LeaveOnIzziCheckBox.Enabled = true; // Otherwise the surrounding span tag is disabled too! - we'll disable with jQuery!
                SelectLocationButton.Text = "Choose Location";
                PublishAll.Enabled = true;

                NewOrReplace.Text = "New";
            }
            else
            {
                WBLogging.Debug("Setting buttons etc for REPLACE");

                NewRadioButton.Checked = false;
                ReplaceRadioButton.Checked = true;
                LeaveOnIzziCheckBox.Enabled = true;
                if (process.ReplaceAction == WBPublishingProcess.REPLACE_ACTION__LEAVE_ON_IZZI)
                {
                    LeaveOnIzziCheckBox.Checked = true;
                }
                else
                {
                    LeaveOnIzziCheckBox.Checked = false;
                }
                SelectLocationButton.Text = "Choose Document";
                PublishAll.Enabled = false;

                NewOrReplace.Text = "Replace";
            }

            WBLogging.Debug("Just before protective zone stage");

            TheProtectiveZone.Text = process.ProtectiveZone;

            if (showSubjectTags)
            {
                if (documentRecordsType == null || documentRecordsType.IsDocumentSubjectTagsRequired)
                {
                    SubjectTagsTitle.Text = "Subject Tags" + WBConstant.REQUIRED_ASTERISK;
                }
                else
                {
                    SubjectTagsTitle.Text = "Subject Tags (optional)";
                }

                if (documentRecordsType != null)
                {
                    SubjectTagsDescription.Text = documentRecordsType.DocumentSubjectTagsDescription;
                }

                process.SubjectTagsTaxonomy.InitialiseTaxonomyControl(SubjectTagsField, WorkBox.COLUMN_NAME__SUBJECT_TAGS, true);
                SubjectTagsField.Text = process.SubjectTagsUIControlValue;
            }

            /*
            if (showReferenceID)
            {
                if (documentRecordsType.IsDocumentReferenceIDRequired)
                {
                    ReferenceIDTitle.Text = "Reference ID" + WBConstant.REQUIRED_ASTERISK;
                }
                else
                {
                    ReferenceIDTitle.Text = "Reference ID (optional)";
                }
                ReferenceIDDescription.Text = documentRecordsType.DocumentReferenceIDDescription;
                ReferenceID.Text = sourceDocAsItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID);
            }

            if (showReferenceDate)
            {
                if (documentRecordsType.IsDocumentReferenceDateRequired)
                {
                    ReferenceDateTitle.Text = "Reference Date" + WBConstant.REQUIRED_ASTERISK;
                }
                else
                {
                    ReferenceDateTitle.Text = "Reference Date (optional)";
                }
                ReferenceDateDescription.Text = documentRecordsType.DocumentReferenceDateDescription;
                if (sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__REFERENCE_DATE))
                {
                    ReferenceDate.SelectedDate = (DateTime)sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE];
                }
                else
                {
                    ReferenceDate.SelectedDate = DateTime.Now;
                }
            }

            if (showSeriesTag)
            {
                if (documentRecordsType.IsDocumentSeriesTagRequired)
                {
                    SeriesTagTitle.Text = "Series Tag" + WBConstant.REQUIRED_ASTERISK;
                }
                else
                {
                    SeriesTagTitle.Text = "Series Tag (optional)";
                }
                SeriesTagDescription.Text = documentRecordsType.DocumentSeriesTagDescription;

                SeriesTagDropDownList.DataSource = GetSeriesTagDataSource(documentRecordsType.DocumentSeriesTagParentTerm(process.SeriesTagsTaxonomy));
                SeriesTagDropDownList.DataTextField = "SeriesTagTermName";
                SeriesTagDropDownList.DataValueField = "SeriesTagTermUIControlValue";
                SeriesTagDropDownList.DataBind();

                if (sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__SERIES_TAG))
                {
                    SeriesTagDropDownList.SelectedValue = sourceDocAsItem.WBxGetSingleTermColumn<WBTerm>(process.SeriesTagsTaxonomy, WorkBox.COLUMN_NAME__SERIES_TAG).UIControlValue;
                }
            }

            if (showScanDate)
            {
                if (documentRecordsType.IsDocumentScanDateRequired)
                {
                    ScanDateTitle.Text = "Scan Date" + WBConstant.REQUIRED_ASTERISK;
                }
                else
                {
                    ScanDateTitle.Text = "Scan Date (optional)";
                }
                ScanDateDescription.Text = documentRecordsType.DocumentScanDateDescription;
                if (sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__SCAN_DATE))
                {
                    ScanDate.SelectedDate = (DateTime)sourceDocAsItem[WorkBox.COLUMN_NAME__SCAN_DATE];
                }
            }
            */

            WBLogging.Debug("Just owning team");

            process.TeamsTaxonomy.InitialiseTaxonomyControl(OwningTeamField, WorkBox.COLUMN_NAME__OWNING_TEAM, false);
            OwningTeamField.Text = process.OwningTeamUIControlValue;

            WBLogging.Debug("Just involved team");

            process.TeamsTaxonomy.InitialiseTaxonomyControl(InvolvedTeamsField, WorkBox.COLUMN_NAME__INVOLVED_TEAMS, true);
            InvolvedTeamsField.Text = process.InvolvedTeamsWithoutOwningTeamAsUIControlValue;

            if (process.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
            {
                WebPageURL.Text = process.WebPageURL;
                showWebPageURL = true;
            }
            else
            {
                WebPageURL.Text = "";
                process.WebPageURL = "";
                showWebPageURL = false;
            }


            if (process.CountStillToPublish > 1)
            {
                if (process.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PROTECTED)
                {
                    showPublishAllButton = manager.AllowBulkPublishOfFileTypes(process.DifferentFileTypesStillToPublish);
                }
                else
                {
                    showPublishAllButton = manager.AllowBulkPublishToPublicOfFileTypes(process.DifferentFileTypesStillToPublish);
                }
            }
            else
            {
                showPublishAllButton = false;
            }


            WBLogging.Debug("Just before serialization");

            // Lastly we're going to capture the state of the publishing process:
            PublishingProcessJSON.Value = WBUtils.SerializeToCompressedJSONForURI(process);
        }


        private Hashtable CheckMetadataOK()
        {
            Hashtable metadataProblems = new Hashtable();

            if (OwningTeamField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__OWNING_TEAM, "You must enter the owning team.");

            if (ShortTitle.Text.Equals("")) metadataProblems.Add(WBColumn.WorkBoxShortTitle.InternalName, "You must enter a short title.");


            if (String.IsNullOrEmpty(process.FunctionalAreaUIControlValue) || String.IsNullOrEmpty(process.RecordsTypeUIControlValue)) {
                metadataProblems.Add("PublishingLocation", "You must pick either a location or replacement document");
            }

            return metadataProblems;
        }


        private Hashtable checkMetadataState()
        {
            Hashtable metadataProblems = new Hashtable();

            if (OwningTeamField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__OWNING_TEAM, "You must enter the owning team.");

            if (InvolvedTeamsField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, "You must enter at least one involved team.");

            if (false) // RecordsType.Text.Equals(""))
            {
                metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "You must enter a records type for this document.");
            }
            else
            {
                // So here we'll load up the actual records type so that we can check what other metadata is required:
                documentRecordsType = new WBRecordsType(process.RecordsTypeTaxonomy, process.RecordsTypeUIControlValue);

                if (documentRecordsType != null)
                {
                    if (!documentRecordsType.AllowDocumentRecords)
                    {
                        metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "You cannot publish documents of this records type. Please choose another.");
                    }


                    if (documentRecordsType.IsFunctionalAreaEditable)
                    {
                        if (false) //FunctionalAreaField.Text == "")
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, "The functional area must be set.");
                        }
                    }

                    bool userCanPublishToPublic = false;
                    SPGroup publishersGroup = WorkBox.OwningTeam.PublishersGroup(SPContext.Current.Site);
                    if (publishersGroup != null)
                    {
                        if (publishersGroup.ContainsCurrentUser)
                        {
                            userCanPublishToPublic = true;
                        }
                    }

                    if (userCanPublishToPublic)
                    {
                        if (!documentRecordsType.IsZoneAtLeastMinimum(TheProtectiveZone.Text))
                        {
                            if (TheProtectiveZone.Text == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                            {
                                metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "You can only publish to the public extranet zone if the records type has that zone explicitly set. This records type has the minimum zone set as: " + documentRecordsType.DocumentMinimumProtectiveZone);
                            }
                            else
                            {
                                metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "The selected protective zone does not meet the minimum requirement for this records type of: " + documentRecordsType.DocumentMinimumProtectiveZone);
                            }
                        }
                    }
                    else
                    {
                        if (TheProtectiveZone.Text != WBRecordsType.PROTECTIVE_ZONE__PROTECTED)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "In this work box you only have permissions to publish to the internal records library.");
                        }
                    }

                    if (documentRecordsType.IsDocumentReferenceIDRequired)
                    {
                        if (ReferenceID.Text.Equals(""))
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__REFERENCE_ID, "You must enter a reference ID for this records type.");
                        }
                    }

                    if (documentRecordsType.IsDocumentReferenceDateRequired)
                    {
                        if (ReferenceDate.IsDateEmpty)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__REFERENCE_DATE, "You must enter a reference date for this records type.");
                        }
                    }

                    if (documentRecordsType.IsDocumentSeriesTagRequired)
                    {
                        if (SeriesTagDropDownList.SelectedValue.Equals(""))
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__SERIES_TAG, "You must select a series tag for this records type.");
                        }
                    }

                    if (documentRecordsType.IsDocumentScanDateRequired)
                    {
                        if (ScanDate.IsDateEmpty)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__SCAN_DATE, "You must enter a scan date for this records type.");
                        }
                    }
                }
                else
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "Could not find this records type.");
                }
            }

            if (destinationType.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE)
                && !TheProtectiveZone.Text.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC))
            {
                if (!metadataProblems.ContainsKey(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE))
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "Only documents marked as 'Public' can be published to the Public Web Site");
                }

            }

            return metadataProblems;
        }

        protected void CaptureChanges()
        {

            if (NewRadioButton.Checked)
            {
                process.ReplaceAction = WBPublishingProcess.REPLACE_ACTION__CREATE_NEW_SERIES;
            }
            else
            {
                if (LeaveOnIzziCheckBox.Checked)
                {
                    process.ReplaceAction = WBPublishingProcess.REPLACE_ACTION__LEAVE_ON_IZZI;
                }
                else
                {
                    process.ReplaceAction = WBPublishingProcess.REPLACE_ACTION__ARCHIVE_FROM_IZZI;
                }
            }

            if (EditShortTitle.Text != ShortTitle.Text)
            {
                process.CurrentShortTitle = EditShortTitle.Text;
            }


            process.SubjectTagsUIControlValue = SubjectTagsField.Text;

            WBLogging.Debug("Captured subject tags to be: " + SubjectTagsField.Text);

            if (process.OwningTeamUIControlValue != OwningTeamField.Text)
            {
                // OK so the owning team has changed we need to change the owning team and the associated IAO:
                process.OwningTeamUIControlValue = OwningTeamField.Text;

                WBTeam owningTeam = new WBTeam(process.TeamsTaxonomy, process.OwningTeamUIControlValue);
                process.OwningTeamsIAOAtTimeOfPublishing = owningTeam.InformationAssetOwnerLogin;
                process.AddExtraMetadata(WBColumn.IAOAtTimeOfPublishing, process.OwningTeamsIAOAtTimeOfPublishing);
            }

            process.InvolvedTeamsWithoutOwningTeamAsUIControlValue = InvolvedTeamsField.Text;

            process.WebPageURL = WebPageURL.Text;
            if (!String.IsNullOrEmpty(process.WebPageURL))
            {
                process.AddExtraMetadata(WBColumn.IntendedWebPageURL, process.WebPageURL);
            }
        }


        protected void publishButton_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("In publishButton_OnClick()");
            MaybeGoToNextPage();
        }

        protected void publishAllButton_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("In publishAllButton_OnClick()");
            process.PublishMode = WBPublishingProcess.PUBLISH_MODE__ALL_TOGETHER;

            MaybeGoToNextPage();
        }

        protected void MaybeGoToNextPage() {

            // There should be no reason to call this here now
            // CaptureChanges();

            Hashtable metadataProblems = CheckMetadataOK();

            string protectiveZone = "";


            if (metadataProblems.Count > 0)
            {
                String errorMessage = ErrorMessageLabel.Text;

                // Have to give these somewhere to be be shown!
                errorMessage += metadataProblems[WorkBox.COLUMN_NAME__RECORDS_TYPE].WBxToString();
                errorMessage += metadataProblems[WorkBox.COLUMN_NAME__FUNCTIONAL_AREA].WBxToString();
                errorMessage += metadataProblems[WorkBox.COLUMN_NAME__PROTECTIVE_ZONE].WBxToString();
                errorMessage += metadataProblems[WorkBox.COLUMN_NAME__SUBJECT_TAGS].WBxToString();

                ErrorMessageLabel.Text = errorMessage;

                ReferenceIDMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__REFERENCE_ID].WBxToString(); ;
                ReferenceDateMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__REFERENCE_DATE].WBxToString(); ;
                SeriesTagFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__SERIES_TAG].WBxToString();
                ScanDateMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__SCAN_DATE].WBxToString();

                OwningTeamFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__OWNING_TEAM].WBxToString();
                InvolvedTeamsFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__INVOLVED_TEAMS].WBxToString();

                ShortTitleError.Text = metadataProblems[WBColumn.WorkBoxShortTitle.InternalName].WBxToString();
                PublishingLocationError.Text = metadataProblems["PublishingLocation"].WBxToString();


                pageRenderingRequired = true;
            }
            else
            {
                pageRenderingRequired = false;
            }

            if (pageRenderingRequired)
            {
                WBLogging.Debug("In publishButton_OnClick(): Page render required - not publishing at this point");
                RenderPage();
            }
            else
            {
                WBLogging.Debug("In publishButton_OnClick(): No page render required - so moving to publish");
                GoToNextPage();
            }
        }



        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }

        DataView GetSeriesTagDataSource(WBTerm seriesTagsParentTerm)
        {
            WBTaxonomy seriesTagsTaxonomy = seriesTagsParentTerm.Taxonomy;

            // Create a table to store data for the DropDownList control.
            DataTable dataTable = new DataTable();

            // Define the columns of the table.
            dataTable.Columns.Add(new DataColumn("SeriesTagTermName", typeof(String)));
            dataTable.Columns.Add(new DataColumn("SeriesTagTermUIControlValue", typeof(String)));

            // First add a blank data row:
            dataTable.Rows.Add(CreateRow("", "", dataTable));

            // Then add all of the terms under the parent term:
            foreach (Term childTerm in seriesTagsParentTerm.Term.Terms)
            {
                WBTerm child = new WBTerm(seriesTagsTaxonomy, childTerm);
                dataTable.Rows.Add(CreateRow(child.Name, child.UIControlValue, dataTable));
            }

            // Create a DataView from the DataTable to act as the data source
            // for the DropDownList control.
            DataView dataView = new DataView(dataTable);
            return dataView;
        }


        DataRow CreateRow(String Text, String Value, DataTable dataTable)
        {
            DataRow dataRow = dataTable.NewRow();

            dataRow[0] = Text;
            dataRow[1] = Value;

            return dataRow;

        }


        private void GoToNextPage()
        {
            string redirectUrl; 

            if (process.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PROTECTED)
            {
                redirectUrl = "WorkBoxFramework/PublishDocActuallyPublish.aspx?PublishingProcessJSON=" + WBUtils.SerializeToCompressedJSONForURI(process);
            }
            else
            {
                redirectUrl = "WorkBoxFramework/PublishDocSelfApprove.aspx?PublishingProcessJSON=" + WBUtils.SerializeToCompressedJSONForURI(process);
            }

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }






    }
}
