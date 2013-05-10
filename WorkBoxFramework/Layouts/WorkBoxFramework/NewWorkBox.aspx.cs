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
using System.Data;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Taxonomy;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class NewWorkBox : WBDialogPageBase
    {
        WBTaxonomy recordsTypes = null;
        WBTaxonomy teams = null;
        WBTaxonomy functionalAreas = null;
        WBTaxonomy seriesTags = null;

        WorkBox relatedWorkBox = null;

        WBRecordsType workBoxRecordsType = null;

        protected bool functionalAreaFieldIsEditable = true;
        protected bool showShortTitle = true;
        protected bool showReferenceID = true;
        protected bool showReferenceDate = true;
        protected bool showSeriesTag = true;
        protected bool onlyOneWorkBoxTemplate = true;

        private bool pageRendered = false;
        private String seriesTagInitialValue = "";
        private String functionalAreaInitialValue = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string relatedWorkBoxUrl = "";
            string recordsTypeGUIDString = "";

            // Set the reference date control to match the locally set locale:
            ReferenceDate.LocaleId = SPContext.Current.Web.Locale.LCID;
            
            recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            teams = WBTaxonomy.GetTeams(recordsTypes);
            seriesTags = WBTaxonomy.GetSeriesTags(recordsTypes);
            functionalAreas = WBTaxonomy.GetFunctionalAreas(recordsTypes);

            if (!IsPostBack)
            {
                WorkBoxCollectionUrl.Value = Request.QueryString["workBoxCollectionUrl"];
                pageRenderingRequired = true;

                recordsTypeGUIDString = Request.QueryString["recordsTypeGUID"];
                RecordsTypeGUID.Value = recordsTypeGUIDString;

                if (recordsTypeGUIDString != "")
                    workBoxRecordsType = recordsTypes.GetRecordsType(new Guid(recordsTypeGUIDString));
                else
                    WBLogging.Generic.Unexpected("The records type GUID appears to be blank in initial call to page!");


                // Now to setup some default intiail values:

                ReferenceDate.SelectedDate = DateTime.Now;

                functionalAreaFieldIsEditable = workBoxRecordsType.IsFunctionalAreaEditable;

                string owningTeamGuidString = Request.QueryString["owningTeamGUID"];
                WBTeam owningTeam = null;

                if (owningTeamGuidString != null && !owningTeamGuidString.Equals(""))
                {

                    owningTeam = teams.GetTeam(new Guid(owningTeamGuidString));

                    OwningTeamField.Text = owningTeam.Name; //  UIControlValue;
                    OwningTeamUIControlValue.Value = owningTeam.UIControlValue;

                    InvolvedTeamsField.Text = owningTeam.UIControlValue;
                }

                relatedWorkBoxUrl = Request.QueryString["relatedWorkBoxURL"];
                RelatedWorkBoxUrl.Value = relatedWorkBoxUrl;
                RelationType.Value = Request.QueryString["relationType"];
                if (relatedWorkBoxUrl != null && relatedWorkBoxUrl != "")
                {
                    relatedWorkBox = new WorkBox(relatedWorkBoxUrl);

                    ReferenceID.Text = relatedWorkBox.ReferenceID;

                    WBTerm seriesTag = relatedWorkBox.SeriesTag(seriesTags);
                    if (seriesTag != null) 
                    {
                        seriesTagInitialValue = seriesTag.UIControlValue;
                    }

                    owningTeam = relatedWorkBox.OwningTeam;
                    OwningTeamField.Text = owningTeam.Name; //  UIControlValue;
                    OwningTeamUIControlValue.Value = owningTeam.UIControlValue;
                    InvolvedTeamsField.Text = relatedWorkBox.InvolvedTeams.UIControlValue;
                }


                // Now let's setup the initial value for the functional area:
                if (functionalAreaFieldIsEditable)
                {
                    functionalAreaInitialValue = workBoxRecordsType.DefaultFunctionalAreaUIControlValue;

                    if (functionalAreaInitialValue == null || functionalAreaInitialValue == "")
                    {
                        if (owningTeam != null)
                            functionalAreaInitialValue = owningTeam.InheritedFunctionalAreaUIControlValue;
                    }
                }
                else
                {
                    functionalAreaInitialValue = workBoxRecordsType.DefaultFunctionalAreaUIControlValue;
                }


            }
            else
            {
                WBUtils.logMessage("In the postback with workBoxCollectionUrl = " + WorkBoxCollectionUrl.Value);
                pageRenderingRequired = false;

                relatedWorkBoxUrl = RelatedWorkBoxUrl.Value;

                recordsTypeGUIDString = RecordsTypeGUID.Value;

                if (recordsTypeGUIDString != "")
                    workBoxRecordsType = recordsTypes.GetRecordsType(new Guid(recordsTypeGUIDString));
                else
                    WBLogging.Generic.Unexpected("The records type GUID appears to be blank in postback!");
            }



            if (pageRenderingRequired)
            {
                renderPage();
            }
        }

        private void renderPage()
        {
            if (pageRendered) return;

            WBLogging.Generic.Unexpected("Rendering 'New Work Box' page");

            WBCollection collection = new WBCollection(WorkBoxCollectionUrl.Value);

            CreateNewWorkBoxText.Text = workBoxRecordsType.CreateNewWorkBoxText;

            RecordsType.Text = workBoxRecordsType.FullPath.Replace("/", " / ");

            string namingConvention = workBoxRecordsType.WorkBoxNamingConvention;
            string uniqueIDprefix = workBoxRecordsType.WorkBoxUniqueIDPrefix;
            if (uniqueIDprefix != "") 
            {
                namingConvention = namingConvention.Replace("<Unique ID Prefix>", uniqueIDprefix);
                namingConvention = namingConvention.Replace("<UID Prefix>", uniqueIDprefix);
            }

            WorkBoxNamingConvention.Text = namingConvention.Replace("<", "&lt;").Replace(">", "&gt;");

            DataView templates = GetTemplatesDataSource(collection, workBoxRecordsType);

            if (templates.Count == 0)
            {
                NoTemplatesError.Text = "There are no templates for this work box records type!";
                onlyOneWorkBoxTemplate = true;
            }
            else
            {
                if (templates.Count == 1)
                {
                    DataRow theTemplate = templates.Table.Rows[0];

                    onlyOneWorkBoxTemplate = true;
                    WorkBoxTemplate.Text = theTemplate["WorkBoxTemplateTextField"].WBxToString();
                    WorkBoxTemplateID.Value = theTemplate["WorkBoxTemplateValueField"].WBxToString();
                }
                else
                {
                    onlyOneWorkBoxTemplate = false;
                    WorkBoxTemplates.DataSource = templates;
                    WorkBoxTemplates.DataTextField = "WorkBoxTemplateTextField";
                    WorkBoxTemplates.DataValueField = "WorkBoxTemplateValueField";
                    WorkBoxTemplates.DataBind();

                    WBTemplate defaultTemplate = collection.DefaultTemplate(workBoxRecordsType);
                    if (defaultTemplate != null) 
                    {
                        WorkBoxTemplates.SelectedValue = defaultTemplate.ID.ToString();
                    }

                    // We're going to use the fact that this hidden field is blank to assume
                    // that there was more than one value:
                    WorkBoxTemplateID.Value = "";
                }
            }

            if (functionalAreaFieldIsEditable)
            {
                functionalAreas.InitialiseTaxonomyControl(FunctionalAreaField, WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, true, true, null);

                if (!IsPostBack && functionalAreaInitialValue != null)
                {
                    FunctionalAreaField.Text = functionalAreaInitialValue;                    
                }
            }
            else
            {
                ReadOnlyFunctionalAreaField.Text = workBoxRecordsType.DefaultFunctionalArea(functionalAreas).Names();
            }


            // teams.InitialiseTaxonomyControl(OwningTeamField, WorkBox.COLUMN_NAME__OWNING_TEAM, false);

            teams.InitialiseTaxonomyControl(InvolvedTeamsField, WorkBox.COLUMN_NAME__INVOLVED_TEAMS, true);

            showShortTitle = workBoxRecordsType.WorkBoxShortTitleRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            if (showShortTitle)
            {
                if (workBoxRecordsType.IsWorkBoxShortTitleRequired)
                {
                    ShortTitleTitle.Text = "Short Title";
                }
                else
                {
                    ShortTitleTitle.Text = "Short Title (optional)";
                }
                ShortTitleDescription.Text = workBoxRecordsType.WorkBoxShortTitleDescription;
            }


            showReferenceID = workBoxRecordsType.WorkBoxReferenceIDRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            if (showReferenceID)
            {
                if (workBoxRecordsType.IsWorkBoxReferenceIDRequired)
                {
                    ReferenceIDTitle.Text = "Reference ID";
                }
                else
                {
                    ReferenceIDTitle.Text = "Reference ID (optional)";
                }
                ReferenceIDDescription.Text = workBoxRecordsType.WorkBoxReferenceIDDescription;
                //                    ReferenceID.Text = sourceDocAsItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID);
            }

            showReferenceDate = workBoxRecordsType.WorkBoxReferenceDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            if (showReferenceDate)
            {
                // Setting the local to a value that should make the date format DD/MM/YYYY
                // ReferenceDate.LocaleId = 2057;

                if (workBoxRecordsType.IsWorkBoxReferenceDateRequired)
                {
                    ReferenceDateTitle.Text = "Reference Date";
                }
                else
                {
                    ReferenceDateTitle.Text = "Reference Date (optional)";
                }
                ReferenceDateDescription.Text = workBoxRecordsType.WorkBoxReferenceDateDescription;
                /*
                if (sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__REFERENCE_DATE))
                {
                    ReferenceDate.SelectedDate = (DateTime)sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE];
                }
                else
                {
                }
                 * */
            }

            showSeriesTag = workBoxRecordsType.WorkBoxSeriesTagRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN;
            if (showSeriesTag)
            {
                if (workBoxRecordsType.IsWorkBoxSeriesTagRequired)
                {
                    SeriesTagTitle.Text = "Series Tag";
                }
                else
                {
                    SeriesTagTitle.Text = "Series Tag (optional)";
                }
                SeriesTagDescription.Text = workBoxRecordsType.DocumentSeriesTagDescription;

                SeriesTagDropDownList.DataSource = GetSeriesTagsDataSource(workBoxRecordsType.WorkBoxSeriesTagParentTerm(seriesTags));
                SeriesTagDropDownList.DataTextField = "SeriesTagTermName";
                SeriesTagDropDownList.DataValueField = "SeriesTagTermUIControlValue";
                SeriesTagDropDownList.DataBind();

                if (!IsPostBack && seriesTagInitialValue != "")
                {
                    if (SeriesTagDropDownList.Items.FindByValue(seriesTagInitialValue) != null)
                    {
                        SeriesTagDropDownList.SelectedValue = seriesTagInitialValue;
                    }
                }

            }


            ErrorMessageLabel.Text = errorMessage;

            if (relatedWorkBox != null) relatedWorkBox.Dispose();

            collection.Dispose();

            pageRendered = true;
        }

        private Hashtable checkMetadataState()
        {
            Hashtable metadataProblems = new Hashtable();

            // if (OwningTeamField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__OWNING_TEAM, "You must enter the owning team.");

            if (InvolvedTeamsField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, "You must enter at least one involved team.");

            if (RecordsType.Text.Equals(""))
            {
                metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "You must enter a records type for this document.");
            }
            else
            {
                if (workBoxRecordsType != null)
                {

                    if (workBoxRecordsType.IsFunctionalAreaEditable)
                    {
                        if (FunctionalAreaField.Text == "")
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, "The functional area must be set.");
                        }
                    }

                    if (workBoxRecordsType.IsWorkBoxShortTitleRequired)
                    {
                        if (WorkBoxShortTitle.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__WORK_BOX_SHORT_TITLE, "You must enter a short title.");
                    }

                    if (workBoxRecordsType.IsWorkBoxReferenceIDRequired)
                    {
                        if (ReferenceID.Text.Equals(""))
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__REFERENCE_ID, "You must enter a reference ID for this records type.");
                        }
                    }

                    if (workBoxRecordsType.IsWorkBoxReferenceDateRequired)
                    {
                        if (ReferenceDate.IsDateEmpty)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__REFERENCE_DATE, "You must enter a reference date for this records type.");
                        }
                    }

                    if (workBoxRecordsType.IsWorkBoxSeriesTagRequired)
                    {
                        if (SeriesTagDropDownList.SelectedValue.Equals(""))
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__SERIES_TAG, "You must select a series tag for this records type.");
                        }
                    }

                }
                else
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "Could not find this records type.");
                }
            }

            return metadataProblems;
        }


        protected void createNewButton_OnClick(object sender, EventArgs e)
        {
            Hashtable metadataProblems = checkMetadataState();

            if (metadataProblems.Count > 0)
            {
                RecordsTypeFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__RECORDS_TYPE].WBxToString();

                FunctionalAreaFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__FUNCTIONAL_AREA].WBxToString();

                WorkBoxShortTitleMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__WORK_BOX_SHORT_TITLE].WBxToString();

                ReferenceIDMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__REFERENCE_ID].WBxToString(); ;
                ReferenceDateMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__REFERENCE_DATE].WBxToString(); ;
                SeriesTagFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__SERIES_TAG].WBxToString();

                OwningTeamFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__OWNING_TEAM].WBxToString();
                InvolvedTeamsFieldMessage.Text = metadataProblems[WorkBox.COLUMN_NAME__INVOLVED_TEAMS].WBxToString();

                pageRenderingRequired = true;
            }
            else
            {
                pageRenderingRequired = false;
            }

            // The event should only be processed if there is no other need to render the page again
            if (pageRenderingRequired)
            {
                renderPage();
            }
            else
            {
                WBCollection collection = new WBCollection(WorkBoxCollectionUrl.Value);

                collection.Web.AllowUnsafeUpdates = true;

                WBUtils.logMessage("OK so we've set to allow unsafe updates of the WorkBoxCollectionWeb");

                string selectedWorkBoxTemplateValue = WorkBoxTemplateID.Value;
                if (selectedWorkBoxTemplateValue == "")
                {
                    selectedWorkBoxTemplateValue = WorkBoxTemplates.SelectedValue;
                }

                int templateID = Convert.ToInt32(selectedWorkBoxTemplateValue);

                WBTemplate template = collection.GetTypeByID(templateID);


                WBTeam owningTeam = new WBTeam(teams, OwningTeamUIControlValue.Value);
                WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(teams, InvolvedTeamsField.Text);

                Hashtable extraValues = null;
                extraValues = new Hashtable();

                if (ReferenceID.Text != "")
                {
                    extraValues.Add(WorkBox.COLUMN_NAME__REFERENCE_ID, ReferenceID.Text);
                }

                if (!ReferenceDate.IsDateEmpty)
                {
                    extraValues.Add(WorkBox.COLUMN_NAME__REFERENCE_DATE, ReferenceDate.SelectedDate);
                }

                if (SeriesTagDropDownList.SelectedValue != "")
                {
                    extraValues.Add(WorkBox.COLUMN_NAME__SERIES_TAG, SeriesTagDropDownList.SelectedValue);
                }

                if (functionalAreaFieldIsEditable)
                {
                    extraValues.Add(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, FunctionalAreaField.Text);
                }
                else
                {
                    extraValues.Add(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, workBoxRecordsType.DefaultFunctionalArea(functionalAreas).UIControlValue);
                }

                WBLogging.Generic.Unexpected("Owning team has values: " + owningTeam.Name + " " + owningTeam.Id);
                WorkBox newWorkBox = collection.RequestNewWorkBox(WorkBoxShortTitle.Text, "", template, owningTeam, involvedTeams, extraValues);

                if (newWorkBox == null)
                {
                    string pageTitle = Uri.EscapeDataString("Failed to create new work box");
                    string pageText = Uri.EscapeDataString("Your request to create a new work box was not successful.");

                    string redirectUrl = "WorkBoxFramework/GenericOKPage.aspx";
                    string queryString = "pageTitle=" + pageTitle + "&pageText=" + pageText;

                    SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
                }


                collection.Web.AllowUnsafeUpdates = false;

                using (SPLongOperation longOperation = new SPLongOperation(this.Page))
                {                   
                    longOperation.LeadingHTML = "Creating your new work box.";
                    longOperation.TrailingHTML = "Please wait while the work box is being created.";

                    longOperation.Begin();


                    newWorkBox.Open("Requested via NewWorkBox.aspx.");

                    if (relatedWorkBox != null)
                    {
                        switch (RelationType.Value)
                        {
                            case WorkBox.RELATION_TYPE__DYNAMIC:
                                break;
                            case WorkBox.RELATION_TYPE__MANUAL_LINK:
                                {
                                    relatedWorkBox.LinkToWorkBox(newWorkBox, WorkBox.RELATION_TYPE__MANUAL_LINK);
                                    break;
                                }
                            case WorkBox.RELATION_TYPE__CHILD:
                                {
                                    relatedWorkBox.LinkToWorkBox(newWorkBox, WorkBox.RELATION_TYPE__CHILD);
                                    newWorkBox.LinkToWorkBox(relatedWorkBox, WorkBox.RELATION_TYPE__PARENT);
                                    break;
                                }
                            default:
                                {
                                    WBUtils.shouldThrowError("Did not recognise the relation type: " + RelationType.Value);
                                    break;
                                }
                        }

                        relatedWorkBox.Dispose();
                    }
                    collection.Dispose();

                    string html = "<h1>Successfully created</h1><p>Your new work box has been successfully created.</p>";

                    html += String.Format("<p>Go to your new work box: <a href=\"#\" onclick=\"javascript: dialogReturnOKAndRedirect('{0}');\">{1}</a></p>",
                        newWorkBox.Url,
                        newWorkBox.Title);


                    string pageTitle = Uri.EscapeDataString("Created new work box");
                    string pageText = Uri.EscapeDataString(html);

                    string okPageUrl = "WorkBoxFramework/GenericOKPage.aspx";

                    string refreshQueryString = "?recordsTypeGUID=" + newWorkBox.RecordsType.Id.ToString();

                    newWorkBox.Dispose();

                    string queryString = "pageTitle=" + pageTitle + "&pageText=" + pageText + "&refreshQueryString=" + refreshQueryString;

                    longOperation.End(okPageUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
                }
            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Creation of new work box cancelled");
        }

        DataView GetTemplatesDataSource(WBCollection collection, WBRecordsType recordsType)
        {

            // Create a table to store data for the DropDownList control.
            DataTable dataTable = new DataTable();

            // Define the columns of the table.
            dataTable.Columns.Add(new DataColumn("WorkBoxTemplateTextField", typeof(String)));
            dataTable.Columns.Add(new DataColumn("WorkBoxTemplateValueField", typeof(String)));

            List<WBTemplate> templates = collection.ActiveTemplates(recordsType);

            foreach (WBTemplate template in templates)
            {
                dataTable.Rows.Add(CreateRow(template.Title, template.ID.ToString(), dataTable));
            }

            // Create a DataView from the DataTable to act as the data source
            // for the DropDownList control.
            DataView dataView = new DataView(dataTable);
            return dataView;
        }

        DataView GetSeriesTagsDataSource(WBTerm seriesTagsParentTerm)
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
                if (childTerm.IsAvailableForTagging)
                {
                    WBTerm child = new WBTerm(seriesTagsTaxonomy, childTerm);
                    dataTable.Rows.Add(CreateRow(child.Name, child.UIControlValue, dataTable));
                }
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
