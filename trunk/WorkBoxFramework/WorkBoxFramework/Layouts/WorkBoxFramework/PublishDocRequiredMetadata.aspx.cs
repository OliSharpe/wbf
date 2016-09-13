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
    public partial class PublishDocRequiredMetadata : WorkBoxDialogPageBase
    {

        protected List<SPListItem> ListItems;
        WBTaxonomy recordsTypeTaxonomy = null;
        WBTaxonomy teamsTaxonomy = null;
        WBTaxonomy seriesTagsTaxonomy = null;
        WBTaxonomy subjectTagsTaxonomy = null;
        WBTaxonomy functionalAreasTaxonomy = null;

        WBRecordsManager manager = null;
        WBRecord recordBeingReplaced = null;

        WBRecordsType documentRecordsType = null;
        SPListItem sourceDocAsItem = null;
        SPFile sourceFile = null;
        string destinationType = "";

        protected bool functionalAreaFieldIsEditable = false;
        protected bool showReferenceID = false;
        protected bool showReferenceDate = false;
        protected bool showSeriesTag = false;
        protected bool showSubjectTags = true;
        protected bool showScanDate = false;

        
        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Generic.Verbose("In Page_Load for the public doc metadata dialog");

            // Creating the taxonomy objects for later use:
            recordsTypeTaxonomy = WBTaxonomy.GetRecordsTypes(WorkBox.Site);
            teamsTaxonomy = WBTaxonomy.GetTeams(recordsTypeTaxonomy);
            seriesTagsTaxonomy = WBTaxonomy.GetSeriesTags(recordsTypeTaxonomy);
            subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(recordsTypeTaxonomy);
            functionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(recordsTypeTaxonomy);

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

                WBLogging.Generic.Verbose("DestinationType = " + TheDestinationType.Value);
                WBLogging.Generic.Verbose("DestinationURL = " + DestinationURL.Value);

                NewRadioButton.Checked = true;
                NewOrReplace.Text = "New";
                ReplacementActions.SelectedIndex = 0;
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
            ReferenceIDMessage.Text = "";
            ReferenceDateMessage.Text = "";
            SeriesTagFieldMessage.Text = "";
            ScanDateMessage.Text = "";
            OwningTeamFieldMessage.Text = "";
            InvolvedTeamsFieldMessage.Text = "";


            //OK so we have the basic identity information for the document being published out so let's get the document item:

            Guid sourceListGuid = new Guid(ListGUID.Value);
            SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)WorkBox.Web.Lists[sourceListGuid];

            sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));
            sourceFile = sourceDocAsItem.File;

            // Now, if this is the first time we might need to load up the default metadata values for the document:
            if (!IsPostBack)
            {
                WorkBox.Web.AllowUnsafeUpdates = true;
                WorkBox.ApplyPublishOutDefaults(sourceDocAsItem);
                WorkBox.Web.AllowUnsafeUpdates = false;

                // Let's now re-load the item as it's name may have changed:
                sourceDocAsItem = null;
                sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));
                sourceFile = sourceDocAsItem.File;
                pageRenderingRequired = true;
            }
            else
            {
                WBLogging.Debug("Setting the subject tags: " + SubjectTagsField.Text);
                sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__SUBJECT_TAGS, SubjectTagsField.Text);

                if (!String.IsNullOrEmpty(ToReplaceRecordID.Value))
                {
                    recordBeingReplaced = manager.Libraries.GetRecordByID(ToReplaceRecordID.Value);
                    if (recordBeingReplaced == null)
                    {
                        ErrorMessageLabel.Text = "Could not find the record that is meant to be replaced. Supposedly it has RecordID = " + ToReplaceRecordID.Value;
                        return;
                    }
                }

                // If this is a post back - then let's check if the records type has been modified:
                if (NewRecordsTypeUIControlValue.Value != "")
                {
                    WBLogging.Generic.Unexpected("The returned value was: " + NewRecordsTypeUIControlValue.Value);

                    WBRecordsType oldRecordsType = sourceDocAsItem.WBxGetSingleTermColumn<WBRecordsType>(recordsTypeTaxonomy, WorkBox.COLUMN_NAME__RECORDS_TYPE);
                    WBRecordsType newRecordsType = new WBRecordsType(recordsTypeTaxonomy, NewRecordsTypeUIControlValue.Value);

                    RecordsTypeUIControlValue.Value = NewRecordsTypeUIControlValue.Value;
                    FunctionalAreasUIControlValue.Value = NewFunctionalAreasUIControlValue.Value;
                    pageRenderingRequired = true;

                    WBDocument document = CaptureAsDocument(sourceDocAsItem, newRecordsType);
                    document.Update();

                    // Let's now re-load the item as it's name may have changed:
                    sourceDocAsItem = null;
                    sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));
                    sourceFile = sourceDocAsItem.File;
                }
                else
                {
                    // Otherwise we are in a normal post back call.
                    pageRenderingRequired = false;
                }
            }



            // Now load up some of the basic details:
            documentRecordsType = sourceDocAsItem.WBxGetSingleTermColumn<WBRecordsType>(recordsTypeTaxonomy, WorkBox.COLUMN_NAME__RECORDS_TYPE);

            destinationType = TheDestinationType.Value;

            // Which of the metadata fields are being used in the form (or will need to be processed in any postback) :
            showReferenceID = documentRecordsType.DocumentReferenceIDRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showReferenceDate = documentRecordsType.DocumentReferenceDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showSubjectTags = true; // documentRecordsType.DocumentSubjectTagsRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showSeriesTag = documentRecordsType.DocumentSeriesTagRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showScanDate = documentRecordsType.DocumentScanDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;

            if (pageRenderingRequired)
            {
                renderPage();
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

        private void renderPage()
        {
            // OK, so now we're finally in a position to load up the values of the page fields:

            SourceDocIcon.AlternateText = "Icon of document being publishing out.";
            SourceDocIcon.ImageUrl = WBUtils.DocumentIcon32(sourceDocAsItem.Url);

            EditShortTitle.Text = sourceFile.Title;
            ShortTitle.Text = sourceFile.Title;

            ReadOnlyNameField.Text = sourceDocAsItem.Name;
 //           NameField.Text = sourceDocAsItem.Name;
            OriginalFileName.Text = sourceDocAsItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__ORIGINAL_FILENAME);

//            DocumentFileNamingConvention.Text = documentRecordsType.DocumentNamingConvention.Replace("<", "&lt;").Replace(">", "&gt;");


            WBTermCollection<WBTerm> functionalAreas = sourceDocAsItem.WBxGetMultiTermColumn<WBTerm>(functionalAreasTaxonomy, WorkBox.COLUMN_NAME__FUNCTIONAL_AREA);
            String functionalAreasUIControlValue = functionalAreas.UIControlValue;

            FunctionalAreasUIControlValue.Value = functionalAreasUIControlValue;


            RecordsTypeUIControlValue.Value = documentRecordsType.UIControlValue;
            SelectLocationButton.OnClientClick = "WorkBoxFramework_pickANewLocation(WorkBoxFramework_PublishDoc_pickedANewLocation, '" + functionalAreasUIControlValue + "', '" + documentRecordsType.UIControlValue + "'); return false;";

            if (recordBeingReplaced == null)
            {
                LocationPath.Text = functionalAreas[0].Name + " / " + documentRecordsType.FullPath.Replace("/", " / ");
            }
            else
            {
                LocationPath.Text = ToReplaceRecordPath.Value;
            }

            if (recordBeingReplaced == null && NewOrReplace.Text == "New")
            {
                WBLogging.Debug("Setting buttons etc for NEW");

                NewRadioButton.Checked = true;
                ReplaceRadioButton.Checked = false;
                SelectLocationButton.Text = "Choose Location";
            }
            else
            {
                WBLogging.Debug("Setting buttons etc for REPLACE");

                NewRadioButton.Checked = false;
                ReplaceRadioButton.Checked = true;
                SelectLocationButton.Text = "Choose Document";

                NewOrReplace.Text = "replace";
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

            String selectedZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;
            if (userCanPublishToPublic)
            {
                if (destinationType.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE))
                {
                    WBLogging.Generic.Verbose("In PUBLIC: The destination type was: " + destinationType);
                    selectedZone = WBRecordsType.PROTECTIVE_ZONE__PUBLIC;
                }
                else if (destinationType.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_EXTRANET))
                {
                    WBLogging.Generic.Verbose("In PUBLIC EXTRANET: The destination type was: " + destinationType);
                    selectedZone = WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET;
                }
                else
                {
                    WBLogging.Generic.Verbose("The destination type was: " + destinationType);
                    selectedZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;
                }
            }
            else
            {
                selectedZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;
            }


            ProtectiveZone.Value = selectedZone;


            if (showSubjectTags)
            {
                if (documentRecordsType.IsDocumentSubjectTagsRequired)
                {
                    SubjectTagsTitle.Text = "Subject Tags" + WBConstant.REQUIRED_ASTERISK;
                }
                else
                {
                    SubjectTagsTitle.Text = "Subject Tags (optional)";
                }
                SubjectTagsDescription.Text = documentRecordsType.DocumentSubjectTagsDescription;

                subjectTagsTaxonomy.InitialiseTaxonomyControl(SubjectTagsField, WorkBox.COLUMN_NAME__SUBJECT_TAGS, true, true, this);
                WBTermCollection<WBTerm> subjectTags = sourceDocAsItem.WBxGetMultiTermColumn<WBTerm>(subjectTagsTaxonomy, WorkBox.COLUMN_NAME__SUBJECT_TAGS);
                SubjectTagsField.Text = subjectTags.WBxToString();
            }


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

                SeriesTagDropDownList.DataSource = GetSeriesTagDataSource(documentRecordsType.DocumentSeriesTagParentTerm(seriesTagsTaxonomy));
                SeriesTagDropDownList.DataTextField = "SeriesTagTermName";
                SeriesTagDropDownList.DataValueField = "SeriesTagTermUIControlValue";
                SeriesTagDropDownList.DataBind();

                if (sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__SERIES_TAG))
                {
                    SeriesTagDropDownList.SelectedValue = sourceDocAsItem.WBxGetSingleTermColumn<WBTerm>(seriesTagsTaxonomy, WorkBox.COLUMN_NAME__SERIES_TAG).UIControlValue;
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

            teamsTaxonomy.InitialiseTaxonomyControl(OwningTeamField, WorkBox.COLUMN_NAME__OWNING_TEAM, false);
            TaxonomyFieldValue owningTeamValue = sourceDocAsItem[WorkBox.COLUMN_NAME__OWNING_TEAM] as TaxonomyFieldValue;
            OwningTeamField.Text = owningTeamValue.WBxUIControlValue();

            teamsTaxonomy.InitialiseTaxonomyControl(InvolvedTeamsField, WorkBox.COLUMN_NAME__INVOLVED_TEAMS, true);
            TaxonomyFieldValueCollection involvedTeamsValues = sourceDocAsItem[WorkBox.COLUMN_NAME__INVOLVED_TEAMS] as TaxonomyFieldValueCollection;
            InvolvedTeamsField.Text = involvedTeamsValues.WBxUIControlValue();

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
                documentRecordsType = new WBRecordsType(recordsTypeTaxonomy, RecordsTypeUIControlValue.Value);

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
                        if (!documentRecordsType.IsZoneAtLeastMinimum(ProtectiveZone.Value))
                        {
                            if (ProtectiveZone.Value == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
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
                        if (ProtectiveZone.Value != WBRecordsType.PROTECTIVE_ZONE__PROTECTED)
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
                && !ProtectiveZone.Value.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC))
            {
                if (!metadataProblems.ContainsKey(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE))
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "Only documents marked as 'Public' can be published to the Public Web Site");
                }

            }

            return metadataProblems;
        }

        protected WBDocument CaptureAsDocument(SPListItem sourceDocAsItem, WBRecordsType documentRecordsType)
        {
            WBDocument document = new WBDocument(WorkBox, sourceDocAsItem);

            // Which of the metadata fields are being used by the active records type?
            showReferenceID = documentRecordsType.DocumentReferenceIDRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showReferenceDate = documentRecordsType.DocumentReferenceDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showSubjectTags = true; // documentRecordsType.DocumentSubjectTagsRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showSeriesTag = documentRecordsType.DocumentSeriesTagRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            showScanDate = documentRecordsType.DocumentScanDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;


            //document.Name = 

            //                if (!generatingFilename)
            //              {
            //                sourceDocAsItem["Name"] = NameField.Text;
            //          }


            if (EditShortTitle.Text != ShortTitle.Text)
            {
                document.Title = EditShortTitle.Text;
            }
            //sourceDocAsItem["Title"] = TitleField.Text;

            if (documentRecordsType.IsFunctionalAreaEditable)
            {
                document[WBColumn.FunctionalArea] = FunctionalAreasUIControlValue.Value;
                sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, FunctionalAreasUIControlValue.Value);
            }

            //document.FunctionalArea = sourceDocAsItem.WBxGetMultiTermColumn<WBTerm>(functionalAreasTaxonomy, WBColumn.FunctionalArea.DisplayName);


            String protectiveZone = ProtectiveZone.Value;
            document.ProtectiveZone = protectiveZone;
            //sourceDocAsItem.WBxSetColumnAsString(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, protectiveZone);

            // Now to save the current value of the Records Type field:
            document[WBColumn.RecordsType] = RecordsTypeUIControlValue.Value;
            //sourceDocAsItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__RECORDS_TYPE, RecordsTypeUIControlValue.Value);

            if (showSubjectTags)
            {
                WBLogging.Debug("Setting subject tags to be: " + SubjectTagsField.Text);
                document[WBColumn.SubjectTags] = SubjectTagsField.Text;
            }
            else
            {
                WBLogging.Debug("NOT !!! Setting subject tags to be: " + SubjectTagsField.Text);
            }


            if (showReferenceID)
            {
                document.ReferenceID = ReferenceID.Text;
                //sourceDocAsItem.WBxSetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID, ReferenceID.Text);
            }

            if (showReferenceDate)
            {
                document.ReferenceDate = ReferenceDate.SelectedDate;
                // sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE] = ReferenceDate.SelectedDate;
            }

            if (showSeriesTag)
            {
                document[WBColumn.SeriesTag] = SeriesTagDropDownList.SelectedValue;
                //sourceDocAsItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__SERIES_TAG, SeriesTagDropDownList.SelectedValue);
            }

            if (showScanDate)
            {
                document.ScanDate = ScanDate.SelectedDate;
                //sourceDocAsItem[WorkBox.COLUMN_NAME__SCAN_DATE] = ScanDate.SelectedDate;
            }


            //sourceDocAsItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__OWNING_TEAM, OwningTeamField.Text);
            //sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, InvolvedTeamsField.Text);

            document[WBColumn.OwningTeam] = OwningTeamField.Text;
            document[WBColumn.InvolvedTeams] = InvolvedTeamsField.Text;
            document.CheckOwningTeamIsAlsoInvolved();

            if (String.IsNullOrEmpty(document.OriginalFilename))
            {
                document.OriginalFilename = sourceDocAsItem.Name;
            }

            WorkBox.GenerateFilename(documentRecordsType, sourceDocAsItem);

            /*
            if (WorkBox.RecordsType.GeneratePublishOutFilenames)
            {
                WorkBox.GenerateFilename(documentRecordsType, sourceDocAsItem);
            }

            sourceDocAsItem.Update();
             */

            return document;
        }

        protected void publishButton_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("In publishButton_OnClick()");

            Hashtable metadataProblems = checkMetadataState();

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

                pageRenderingRequired = true;
            }
            else
            {
                pageRenderingRequired = false;
            }

            if (pageRenderingRequired)
            {
                WBLogging.Debug("In publishButton_OnClick(): Page render required - not publishing at this point");
                renderPage();
            }
            else
            {
                WBLogging.Debug("In publishButton_OnClick(): No page render required - so moving to publish");

                // The event should only be processed if there is no other need to render the page again

                // First let's update the item with the new metadata values submitted:
                SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)SPContext.Current.Web.Lists[new Guid(ListGUID.Value)];
                SPListItem sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));

                WBDocument document = CaptureAsDocument(sourceDocAsItem, documentRecordsType);

                document.Update();

                /*
                 * 
                 *   OK So now we actually publish out the document:
                 * 
                 */


                SPFile sourceFile = sourceDocAsItem.File;
                string errorMessage = "";

                string successMessage = "<h3>Successfully Published Out</h3> <table cellpadding='5'>";
                if (TheDestinationType.Value.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX))
                {
                    using (WorkBox workBox = new WorkBox(DestinationURL.Value))
                    {
                        string selectedFolderPath = Request.QueryString["SelectedFolderPath"];
                        if (string.IsNullOrEmpty(selectedFolderPath))
                        {
                            selectedFolderPath = "/";
                        }

                        string destinationRootFolderUrl = DestinationURL.Value + "/" + workBox.DocumentLibrary.RootFolder.Url + selectedFolderPath;

                        errorMessage = sourceFile.WBxCopyTo(destinationRootFolderUrl, new List<String>());

                        if (errorMessage == "")
                        {
                            successMessage += "<tr><td>Filename</td><td><b>" + sourceFile.Name + "</b></td></tr><tr><td>Published out to:</td><td><a href=\"" + destinationRootFolderUrl + "\"><b>" + destinationRootFolderUrl + "</b></a></td></tr>";
                            successMessage += "</table>";
                            GoToGenericOKPage("Publishing Out Success", successMessage);
                        }
                        else
                        {
                            GoToGenericOKPage("Publishing Out Error", errorMessage);
                        }
                    }
                }
                else
                {
                    GoToApprovalPage();
                }
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


        private void GoToApprovalPage()
        {
            String destinationType = TheDestinationType.Value;
            String destinationTitle = DestinationTitle.Text;
            String destinationUrl = DestinationURL.Value;
            string listGuid = ListGUID.Value;
            string itemID = ItemID.Value;

            string redirectUrl = "WorkBoxFramework/PublishDocSelfApprove.aspx?"
                + "ListGUID=" + listGuid
                + "&ItemID=" + itemID
                + "&DestinationURL=" + destinationUrl
                + "&DestinationTitle=" + destinationTitle
                + "&DestinationType=" + destinationType
                + "&ToReplaceRecordID=" + ToReplaceRecordID.Value
                + "&ToReplaceRecordPath=" + ToReplaceRecordPath.Value
                + "&NewOrReplace=" + NewOrReplace.Text
                + "&ReplacementAction=" + ReplacementActions.SelectedValue;

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }






    }
}
