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
    public partial class PublishDocDialogRequiredMetadataPage : WorkBoxDialogPageBase
    {

        protected List<SPListItem> ListItems;
        WBTaxonomy recordsTypeTaxonomy = null;
        WBTaxonomy teamsTaxonomy = null;
        WBTaxonomy seriesTagsTaxonomy = null;
        WBTaxonomy subjectTagsTaxonomy = null;
        WBTaxonomy functionalAreasTaxonomy = null;



        WBRecordsType documentRecordsType = null;
        SPListItem sourceDocAsItem = null;
        SPFile sourceFile = null;
        string destinationType = "";

        protected bool generatingFilename = false;
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

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {
                ListGUID.Value = Request.QueryString["ListGUID"];
                ItemID.Value = Request.QueryString["ItemID"];

                // The following variable has its name due to a strange compliation error with the name 'DestinationType' 
                TheDestinationType.Value = Request.QueryString["DestinationType"];
                DestinationURL.Value = Request.QueryString["DestinationURL"];
                DestinationTitle.Text = Request.QueryString["DestinationTitle"] + " (" + Request.QueryString["DestinationType"] + ")";

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
            RecordsTypeFieldMessage.Text = "";
            FunctionalAreaFieldMessage.Text = "";
            ProtectiveZoneMessage.Text = "";
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

            generatingFilename = WorkBox.RecordsType.GeneratePublishOutFilenames;

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


                // If this is a post back - then let's check if the records type has been modified:
                if (NewRecordsTypeUIControlValue.Value != "")
                {
                    WBLogging.Generic.Unexpected("The returned value was: " + NewRecordsTypeUIControlValue.Value);

                    WBRecordsType oldRecordsType = sourceDocAsItem.WBxGetSingleTermColumn<WBRecordsType>(recordsTypeTaxonomy, WorkBox.COLUMN_NAME__RECORDS_TYPE);
                    WBRecordsType newRecordsType = new WBRecordsType(recordsTypeTaxonomy, NewRecordsTypeUIControlValue.Value);

                    RecordsTypeUIControlValue.Value = NewRecordsTypeUIControlValue.Value;
                    RecordsType.Text = newRecordsType.Name;
                    pageRenderingRequired = true;

                    sourceDocAsItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__RECORDS_TYPE, NewRecordsTypeUIControlValue.Value);
                    sourceDocAsItem.WBxSet(WBColumn.Title, this.TitleField.Text);

                    //if (generatingFilename)
                    //{
                    WorkBox.GenerateFilename(newRecordsType, sourceDocAsItem);
                    //}

                    // If either the old or new records type have an uneditable functional area, then we'll update it to the new default area.
                    if (!oldRecordsType.IsFunctionalAreaEditable || !newRecordsType.IsFunctionalAreaEditable)
                    {
                        WBLogging.Debug("Setting the functional area as it's not editable: " + newRecordsType.DefaultFunctionalAreaUIControlValue);
                        sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, newRecordsType.DefaultFunctionalAreaUIControlValue);
                    }
                    else
                    {
                        WBLogging.Debug("Saving the current functional area selection: " + this.FunctionalAreaField.Text);
                        sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, this.FunctionalAreaField.Text);
                    }


                    sourceDocAsItem.Update();

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

        private void renderPage()
        {
            // OK, so now we're finally in a position to load up the values of the page fields:

            SourceDocIcon.AlternateText = "Icon of document being publishing out.";
            SourceDocIcon.ImageUrl = WBUtils.DocumentIcon32(sourceDocAsItem.Url);

            TitleField.Text = sourceFile.Title;

            ReadOnlyNameField.Text = sourceDocAsItem.Name;
            NameField.Text = sourceDocAsItem.Name;
            OriginalFileName.Text = sourceDocAsItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__ORIGINAL_FILENAME);

            DocumentFileNamingConvention.Text = documentRecordsType.DocumentNamingConvention.Replace("<", "&lt;").Replace(">", "&gt;");

            RecordsTypeUIControlValue.Value = documentRecordsType.UIControlValue;
            PickRecordsTypeButton.OnClientClick = "WorkBoxFramework_pickANewRecordsType(WorkBoxFramework_PublishDoc_pickedANewRecordsType, '" + documentRecordsType.UIControlValue + "'); return false;";
            RecordsType.Text = documentRecordsType.FullPath.Replace("/", " / ");


            WBTermCollection<WBTerm> functionalAreas = sourceDocAsItem.WBxGetMultiTermColumn<WBTerm>(functionalAreasTaxonomy, WorkBox.COLUMN_NAME__FUNCTIONAL_AREA);
            functionalAreaFieldIsEditable = documentRecordsType.IsFunctionalAreaEditable;
            if (functionalAreaFieldIsEditable)
            {
                functionalAreasTaxonomy.InitialiseTaxonomyControl(FunctionalAreaField, WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, false, false, this);

                String functionalAreaValue = functionalAreas.UIControlValue;
                if (functionalAreaValue.Contains(";"))
                {
                    string[] allFunctionalValues = functionalAreaValue.Split(';');
                    functionalAreaValue = allFunctionalValues[0];
                }

                FunctionalAreaField.Text = functionalAreaValue;
            }
            else
            {
                ReadOnlyFunctionalAreaField.Text = functionalAreas.Names();
            }

            ProtectiveZone.DataSource = WBRecordsType.getProtectiveZones();
            ProtectiveZone.DataBind();

            if (destinationType.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE))
            {
                WBLogging.Generic.Verbose("In PUBLIC: The destination type was: " + destinationType);                
                ProtectiveZone.SelectedValue = WBRecordsType.PROTECTIVE_ZONE__PUBLIC;
            }
            else if (destinationType.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_EXTRANET))
            {
                WBLogging.Generic.Verbose("In PUBLIC EXTRANET: The destination type was: " + destinationType);                
                ProtectiveZone.SelectedValue = WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET;
            }
            else
            {
                WBLogging.Generic.Verbose("The destination type was: " + destinationType);                
                string currentZone = sourceDocAsItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE);

                if (currentZone == "") currentZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;
                ProtectiveZone.SelectedValue = currentZone;
            }

            if (showSubjectTags)
            {
                if (documentRecordsType.IsDocumentSubjectTagsRequired)
                {
                    SubjectTagsTitle.Text = "Subject Tags";
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
                    ReferenceIDTitle.Text = "Reference ID";
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
                    ReferenceDateTitle.Text = "Reference Date";
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
                    SeriesTagTitle.Text = "Series Tag";
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
                    ScanDateTitle.Text = "Scan Date";
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

            if (RecordsType.Text.Equals(""))
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
                        if (FunctionalAreaField.Text == "")
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, "The functional area must be set.");
                        }
                    }

                    if (!documentRecordsType.IsZoneAtLeastMinimum(ProtectiveZone.Text))
                    {
                        if (ProtectiveZone.Text == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "You can only publish to the public extranet zone if the records type has that zone explicitly set. This records type has the minimum zone set as: " + documentRecordsType.DocumentMinimumProtectiveZone);
                        }
                        else
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "The selected protective zone does not meet the minimum requirement for this records type of: " + documentRecordsType.DocumentMinimumProtectiveZone);
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
                && !ProtectiveZone.SelectedValue.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC))
            {
                if (!metadataProblems.ContainsKey(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE))
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "Only documents marked as 'Public' can be published to the Public Web Site");
                }

            }

            return metadataProblems;
        }

        protected void publishButton_OnClick(object sender, EventArgs e)
        {
            Hashtable metadataProblems = checkMetadataState();

            string protectiveZone = "";


            if (metadataProblems.Count > 0)
            {
                RecordsTypeFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__RECORDS_TYPE].WBxToString();

                FunctionalAreaFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__FUNCTIONAL_AREA].WBxToString();
                ProtectiveZoneMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__PROTECTIVE_ZONE].WBxToString();

 //               SubjectTagsMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__SERIES_TAG].WBxToString();
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
                renderPage();
            }
            else
            {
                // The event should only be processed if there is no other need to render the page again

                // First let's update the item with the new metadata values submitted:
                SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)SPContext.Current.Web.Lists[new Guid(ListGUID.Value)];
                SPListItem sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));

                WBDocument document = new WBDocument(sourceDocAsItem);

                //document.Name = 

//                if (!generatingFilename)
  //              {
    //                sourceDocAsItem["Name"] = NameField.Text;
      //          }



                document.Title = TitleField.Text;
                //sourceDocAsItem["Title"] = TitleField.Text;

                if (documentRecordsType.IsFunctionalAreaEditable)
                {     
                    document[WBColumn.FunctionalArea] = FunctionalAreaField.Text;
                    sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, FunctionalAreaField.Text);
                }

                //document.FunctionalArea = sourceDocAsItem.WBxGetMultiTermColumn<WBTerm>(functionalAreasTaxonomy, WBColumn.FunctionalArea.DisplayName);


                protectiveZone = ProtectiveZone.SelectedValue;
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
                        }
                    }
                }
                else
                {
                    WBRecordsType recordsType = new WBRecordsType(recordsTypeTaxonomy, document[WBColumn.RecordsType] as String);

                    try
                    {
                        recordsType.PublishDocument(document, sourceFile.OpenBinaryStream());

                        
                        string fullClassPath = WBUtils.NormalisePath(document.FunctionalArea.Names() + "/" + recordsType.FullPath);

                        successMessage += "<tr><td>Published out to location:</td><td>" + fullClassPath + "</td></tr>\n";


                        if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                        {
                            successMessage += "<tr><td>To public records library</td><td><a href=\"http://stagingweb/publicrecords\">Our public library</a></td></tr>\n";
                        }

                        if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                        {
                            successMessage += "<tr><td>To public extranet records library</td><td><a href=\"http://stagingextranets/records\">Our public extranet library</a></td></tr>\n";
                        }

                        successMessage += "<tr><td>To internal records library</td><td><a href=\"http://sp.izzi/library/Pages/ViewByFunctionThenType.aspx\">Our internal library</a></td></tr>\n";

                    }
                    catch (Exception exception)
                    {
                        errorMessage = "An error occurred when trying to publish: " + exception.Message;
                        WBLogging.Generic.Unexpected(exception);
                    }

                }

/*
                WBFarm farm = WBFarm.Local;
                string destinationRootFolderUrl = farm.ProtectedRecordsLibraryUrl;
                List<String> filingPath = null;


                filingPath = documentRecordsType.FilingPathForItem(sourceDocAsItem);

                string filingPathString = string.Join("/", filingPath.ToArray());

                WBLogging.Generic.Verbose("The file is: " + sourceFile.Url);
                WBLogging.Generic.Verbose("The destination is: " + destinationRootFolderUrl);
                WBLogging.Generic.Verbose("The destination filing path is: " + filingPathString);


                string errorMessage = sourceFile.WBxCopyTo(destinationRootFolderUrl, filingPath);

                if (errorMessage == "")
                {
                    successMessage += "<tr><td>Filename</td><td><b>" + sourceFile.Name + "</b></td></tr><tr><td>Published out to:</td><td><a href=\"" + destinationRootFolderUrl + "\"><b>" + destinationRootFolderUrl + "</b></a></td></tr><tr><td>Filing path:</td><td><a href=\"" + destinationRootFolderUrl + "/" + filingPathString + "\"><b>" + filingPathString + "</b></td></tr>";
                }

                WBLogging.Generic.Verbose("Protective zone was set to be: " + protectiveZone);


                if (!TheDestinationType.Value.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX)
                    && protectiveZone.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC))
                {
                    // OK so we're going to copy this to the public library as well:
                WBLogging.Generic.Verbose("The file is: " + sourceFile.Url);
                WBLogging.Generic.Verbose("The destination is: " + farm.PublicRecordsLibraryUrl);
                WBLogging.Generic.Verbose("The destination filing path is: " + filingPathString);

                    string errorMessagePublic = sourceFile.WBxCopyTo(farm.PublicRecordsLibraryUrl, filingPath);

                    if (errorMessagePublic == "")
                    {
                        successMessage += "<tr><td colspan='2'><b>And also published to the public library.</b></td></tr>";
                    }
                }

                if (!TheDestinationType.Value.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX)
                    && protectiveZone.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET))
                {
                    // OK so we're going to copy this to the public extranet library as well:
                    WBLogging.Generic.Verbose("The file is: " + sourceFile.Url);
                    WBLogging.Generic.Verbose("The destination is: " + farm.PublicExtranetRecordsLibraryUrl);
                    WBLogging.Generic.Verbose("The destination filing path is: " + filingPathString);

                    string errorMessagePublicExtranet = sourceFile.WBxCopyTo(farm.PublicExtranetRecordsLibraryUrl, filingPath);

                    if (errorMessagePublicExtranet == "")
                    {
                        successMessage += "<tr><td colspan='2'><b>And also published to the public extranet library.</b></td></tr>";
                    }
                }
                */
                successMessage += "</table>";

                if (errorMessage == "")
                {
                    //returnFromDialogOKAndRefresh();
                    goToGenericOKPage("Publishing Out Success", successMessage);
                }
                else
                {
                    goToGenericOKPage("Publishing Out Error", errorMessage);

                    //returnFromDialogOK("An error occurred during publishing: " + errorMessage);
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






    }
}
