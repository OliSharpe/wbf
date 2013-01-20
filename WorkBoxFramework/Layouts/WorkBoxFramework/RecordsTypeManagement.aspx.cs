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
using System.Text;
using System.Reflection;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class RecordsTypeManagement : LayoutsPageBase
    {
        WBTaxonomy recordsTypes;
        WBTaxonomy seriesTags;
        WBTaxonomy functionalAreas;

        //protected String popupMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            SPSite site = SPContext.Current.Site;
            recordsTypes = WBTaxonomy.GetRecordsTypes(site);
            seriesTags = WBTaxonomy.GetSeriesTags(recordsTypes);
            functionalAreas = WBTaxonomy.GetFunctionalAreas(recordsTypes);

            WBUtils.logMessage("Records Types object has been recreated");

            functionalAreas.InitialiseTaxonomyControl(DefaultFunctionalArea, "Select Default Functional Area", false, false, this);

            recordsTypes.InitialiseTaxonomyControl(DefaultRecordsType, "Select Default Publishing Out Records Type", false, false, this);

            seriesTags.InitialiseTaxonomyControl(DocumentSeriesTagParentTerm, "Select the Series Tag Parent", false, true, this);
            seriesTags.InitialiseTaxonomyControl(WorkBoxSeriesTagParentTerm, "Select the Series Tag Parent", false, true, this);

            if (!IsPostBack)
            {
                TreeViewTermCollection collection = new TreeViewTermCollection(recordsTypes.TermSet);

                // Bind the data source to your collection
                AllRecordsTypesTreeView.DataSource = collection;
                AllRecordsTypesTreeView.DataBind();
            }
        }

        protected void updatePanelWithRecordsTypeDetails(WBRecordsType recordsType)
        {
            RecordsTypeName.Text = recordsType.Name;
            LastModfiedDate.Text = String.Format("{0:d/M/yyyy HH:mm:ss}", recordsType.Term.LastModifiedDate); 

            RecordsTypeDescription.Text = recordsType.Description;
            DefaultFunctionalArea.Text = recordsType.DefaultFunctionalAreaUIControlValue;
            AllowOtherFunctionalAreas.Checked = recordsType.AllowOtherFunctionalAreas;


            // First the work box details:
            AllowWorkBoxRecords.Checked = recordsType.AllowWorkBoxRecords;
            WhoCanCreateNewWorkBoxes.DataSource = WBRecordsType.getWhoCanCreateOptions();
            WhoCanCreateNewWorkBoxes.DataBind();
            WhoCanCreateNewWorkBoxes.WBxSafeSetSelectedValue(recordsType.WhoCanCreateNewWorkBoxes);

            CreateNewWorkBoxText.Text = recordsType.CreateNewWorkBoxText;

            WorkBoxUniqueIDPrefix.Text = recordsType.WorkBoxUniqueIDPrefix;

            WorkBoxLocalIDSource.DataSource = WBRecordsType.getWorkBoxLocalIDSources();
            WorkBoxLocalIDSource.DataBind();
            WorkBoxLocalIDSource.WBxSafeSetSelectedValue(recordsType.WorkBoxLocalIDSource);

            WorkBoxGeneratedLocalIDOffset.Text = recordsType.WorkBoxGeneratedLocalIDOffset.ToString();

            WorkBoxShortTitleRequirement.DataSource = WBRecordsType.getRequirementOptions();
            WorkBoxShortTitleRequirement.DataBind();
            WorkBoxShortTitleRequirement.WBxSafeSetSelectedValue(recordsType.WorkBoxShortTitleRequirement);

            WorkBoxShortTitleDescription.Text = recordsType.WorkBoxShortTitleDescription;


            WorkBoxReferenceIDRequirement.DataSource = WBRecordsType.getRequirementOptions();
            WorkBoxReferenceIDRequirement.DataBind();
            WorkBoxReferenceIDRequirement.WBxSafeSetSelectedValue(recordsType.WorkBoxReferenceIDRequirement);

            WorkBoxReferenceIDDescription.Text = recordsType.WorkBoxReferenceIDDescription;

            WorkBoxReferenceDateRequirement.DataSource = WBRecordsType.getRequirementOptions();
            WorkBoxReferenceDateRequirement.DataBind();
            WorkBoxReferenceDateRequirement.WBxSafeSetSelectedValue(recordsType.WorkBoxReferenceDateRequirement);

            WorkBoxReferenceDateDescription.Text = recordsType.WorkBoxReferenceDateDescription;

            WorkBoxSeriesTagRequirement.DataSource = WBRecordsType.getRequirementOptions();
            WorkBoxSeriesTagRequirement.DataBind();
            WorkBoxSeriesTagRequirement.WBxSafeSetSelectedValue(recordsType.WorkBoxSeriesTagRequirement);

            WorkBoxSeriesTagParentTerm.Text = recordsType.WorkBoxSeriesTagParentTermUIControlValue;
            WorkBoxSeriesTagAllowNewTerms.Checked = recordsType.WorkBoxSeriesTagAllowNewTerms;
            WorkBoxSeriesTagDescription.Text = recordsType.WorkBoxSeriesTagDescription;


            WorkBoxNamingConventions.DataSource = WBRecordsType.getWorkBoxNamingConventions();
            WorkBoxNamingConventions.DataBind();
            WorkBoxNamingConventions.WBxSafeSetSelectedValue(recordsType.WorkBoxNamingConvention);

            AutoCloseTriggerDate.DataSource = WBRecordsType.getAutoCloseTriggerDates();
            AutoCloseTriggerDate.DataBind();
            AutoCloseTriggerDate.WBxSafeSetSelectedValue(recordsType.AutoCloseTriggerDate);

            AutoCloseTimeUnits.DataSource = WBRecordsType.getAutoCloseUnits();
            AutoCloseTimeUnits.DataBind();
            AutoCloseTimeUnits.WBxSafeSetSelectedValue(recordsType.AutoCloseTimeUnit);

            AutoCloseTimeScalar.Text = recordsType.AutoCloseTimeScalarAsString;


            RetentionTriggerDate.DataSource = WBRecordsType.getRetentionTriggerDates();
            RetentionTriggerDate.DataBind();
            RetentionTriggerDate.WBxSafeSetSelectedValue(recordsType.RetentionTriggerDate);

            RetentionUnits.DataSource = WBRecordsType.getRetentionUnits();
            RetentionUnits.DataBind();

            RetentionUnits.WBxSafeSetSelectedValue(recordsType.RetentionTimeUnit);
            RetentionScalar.Text = recordsType.RetentionTimeScalarAsString;

            AllowPublishingOut.Checked = recordsType.AllowPublishingOut;
            MinimumPublishingOutProtectiveZone.DataSource = WBRecordsType.getProtectiveZones();
            MinimumPublishingOutProtectiveZone.DataBind();
            MinimumPublishingOutProtectiveZone.WBxSafeSetSelectedValue(recordsType.MinimumPublishingOutProtectiveZone);

            GenerateFilenames.Checked = recordsType.GeneratePublishOutFilenames;
            UseDefaults.Checked = recordsType.UseDefaultsWhenPublishingOut;

            DefaultRecordsType.Text = recordsType.DefaultPublishingOutRecordsTypeUIControlValue;

            WorkBoxCollectionURLProperty.Text = recordsType.WorkBoxCollectionUrlProperty;
            WorkBoxCollectionURL.Text = recordsType.WorkBoxCollectionUrl;

            CacheDetailsForOpenWorkBoxes.Checked = recordsType.CacheDetailsForOpenWorkBoxes;


            // Now the document details:
            AllowDocumentRecords.Checked = recordsType.AllowDocumentRecords;

            ProtectiveZones.DataSource = WBRecordsType.getProtectiveZones();
            ProtectiveZones.DataBind();
            ProtectiveZones.WBxSafeSetSelectedValue(recordsType.DocumentMinimumProtectiveZone);

            DocumentReferenceIDRequirement.DataSource = WBRecordsType.getRequirementOptions();
            DocumentReferenceIDRequirement.DataBind();
            DocumentReferenceIDRequirement.WBxSafeSetSelectedValue(recordsType.DocumentReferenceIDRequirement);

            DocumentReferenceIDDescription.Text = recordsType.DocumentReferenceIDDescription;

            DocumentReferenceDateRequirement.DataSource = WBRecordsType.getRequirementOptions();
            DocumentReferenceDateRequirement.DataBind();
            DocumentReferenceDateRequirement.WBxSafeSetSelectedValue(recordsType.DocumentReferenceDateRequirement);

            DocumentReferenceDateSource.DataSource = WBRecordsType.getReferenceDateSources();
            DocumentReferenceDateSource.DataBind();
            DocumentReferenceDateSource.WBxSafeSetSelectedValue(recordsType.DocumentReferenceDateSource);

            DocumentReferenceDateDescription.Text = recordsType.DocumentReferenceDateDescription;

            DocumentSeriesTagRequirement.DataSource = WBRecordsType.getRequirementOptions();
            DocumentSeriesTagRequirement.DataBind();
            DocumentSeriesTagRequirement.WBxSafeSetSelectedValue(recordsType.DocumentSeriesTagRequirement);

            DocumentSeriesTagParentTerm.Text = recordsType.DocumentSeriesTagParentTermUIControlValue;
            DocumentSeriesTagAllowNewTerms.Checked = recordsType.DocumentSeriesTagAllowNewTerms;

            DocumentSeriesTagDescription.Text = recordsType.DocumentSeriesTagDescription;

            DocumentScanDateRequirement.DataSource = WBRecordsType.getRequirementOptions();
            DocumentScanDateRequirement.DataBind();
            DocumentScanDateRequirement.WBxSafeSetSelectedValue(recordsType.DocumentScanDateRequirement);

            DocumentScanDateDescription.Text = recordsType.DocumentScanDateDescription;


            DocumentNamingConvention.DataSource = WBRecordsType.getDocumentNamingConventions();
            DocumentNamingConvention.DataBind();
            DocumentNamingConvention.WBxSafeSetSelectedValue(recordsType.DocumentNamingConvention);

            EnforceDocumentNamingConvention.Checked = recordsType.EnforceDocumentNamingConvention;

            FilingRuleLevel1.DataSource = WBRecordsType.getFilingRules();
            FilingRuleLevel1.DataBind();
            FilingRuleLevel1.WBxSafeSetSelectedValue(recordsType.FilingRuleLevel1);

            FilingRuleLevel2.DataSource = WBRecordsType.getFilingRules();
            FilingRuleLevel2.DataBind();
            FilingRuleLevel2.WBxSafeSetSelectedValue(recordsType.FilingRuleLevel2);

            FilingRuleLevel3.DataSource = WBRecordsType.getFilingRules();
            FilingRuleLevel3.DataBind();
            FilingRuleLevel3.WBxSafeSetSelectedValue(recordsType.FilingRuleLevel3);

            FilingRuleLevel4.DataSource = WBRecordsType.getFilingRules();
            FilingRuleLevel4.DataBind();
            FilingRuleLevel4.WBxSafeSetSelectedValue(recordsType.FilingRuleLevel4);


            WBFarm farm = WBFarm.Local;

            using (SPSite recordsSite = new SPSite(farm.ProtectedRecordsLibraryUrl))
            using (SPWeb recordsWeb = recordsSite.OpenWeb())
            {
                string link = farm.ProtectedRecordsLibraryUrl + "/_layouts/expirationconfig.aspx?RootFolder=";

                SPList library = recordsWeb.GetList(farm.ProtectedRecordsLibraryUrl);


                string rootFolder = library.RootFolder.ServerRelativeUrl + recordsType.FullPath;
                link += Uri.EscapeDataString(rootFolder);
                link += "&List=" + library.ID.WBxToString();

                LinkToRecordsCenterConfig.Text = "<a href=\"" + link + "\">Configure document retention</a>";                
            }

        }

        private void resetPanelToSelectedTermValues()
        {
            WBRecordsType selectedRecordsType = recordsTypes.GetSelectedRecordsType(AllRecordsTypesTreeView.SelectedNode.ValuePath);

            updatePanelWithRecordsTypeDetails(selectedRecordsType);
        }

        protected void AllRecordsTypesTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (AllRecordsTypesTreeView.SelectedNode != null)
            {
                resetPanelToSelectedTermValues();
            }
        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            WBRecordsType selectedRecordsType = recordsTypes.GetSelectedRecordsType(AllRecordsTypesTreeView.SelectedNode.ValuePath);

            selectedRecordsType.Name = RecordsTypeName.Text;
            selectedRecordsType.Description = RecordsTypeDescription.Text;
            selectedRecordsType.DefaultFunctionalAreaUIControlValue = DefaultFunctionalArea.Text;
            selectedRecordsType.AllowOtherFunctionalAreas = AllowOtherFunctionalAreas.Checked;


            selectedRecordsType.AllowWorkBoxRecords = AllowWorkBoxRecords.Checked;
            selectedRecordsType.WhoCanCreateNewWorkBoxes = WhoCanCreateNewWorkBoxes.SelectedValue;
            selectedRecordsType.CreateNewWorkBoxText = CreateNewWorkBoxText.Text;

            selectedRecordsType.WorkBoxCollectionUrlProperty = WorkBoxCollectionURLProperty.Text;

            selectedRecordsType.WorkBoxUniqueIDPrefix = WorkBoxUniqueIDPrefix.Text;
            selectedRecordsType.WorkBoxLocalIDSource = WorkBoxLocalIDSource.SelectedValue;
            selectedRecordsType.WorkBoxGeneratedLocalIDOffset = Convert.ToInt32(WorkBoxGeneratedLocalIDOffset.Text);

            selectedRecordsType.WorkBoxShortTitleRequirement = WorkBoxShortTitleRequirement.SelectedValue;
            selectedRecordsType.WorkBoxShortTitleDescription = WorkBoxShortTitleDescription.Text;

            selectedRecordsType.WorkBoxReferenceIDRequirement = WorkBoxReferenceIDRequirement.SelectedValue;
            selectedRecordsType.WorkBoxReferenceIDDescription = WorkBoxReferenceIDDescription.Text;

            selectedRecordsType.WorkBoxReferenceDateRequirement = WorkBoxReferenceDateRequirement.SelectedValue;
            selectedRecordsType.WorkBoxReferenceDateDescription = WorkBoxReferenceDateDescription.Text;

            selectedRecordsType.WorkBoxSeriesTagRequirement = WorkBoxSeriesTagRequirement.SelectedValue;
            selectedRecordsType.WorkBoxSeriesTagParentTermUIControlValue = WorkBoxSeriesTagParentTerm.Text;
            selectedRecordsType.WorkBoxSeriesTagAllowNewTerms = WorkBoxSeriesTagAllowNewTerms.Checked;
            selectedRecordsType.WorkBoxSeriesTagDescription = WorkBoxSeriesTagDescription.Text;

            
            selectedRecordsType.WorkBoxNamingConvention = WorkBoxNamingConventions.SelectedValue;

            selectedRecordsType.AutoCloseTriggerDate = AutoCloseTriggerDate.SelectedValue;
            selectedRecordsType.AutoCloseTimeUnit = AutoCloseTimeUnits.SelectedValue;
            selectedRecordsType.AutoCloseTimeScalarAsString = AutoCloseTimeScalar.Text;

            selectedRecordsType.RetentionTriggerDate = RetentionTriggerDate.SelectedValue;
            selectedRecordsType.RetentionTimeUnit = RetentionUnits.SelectedValue;
            selectedRecordsType.RetentionTimeScalarAsString = RetentionScalar.Text;

            selectedRecordsType.AllowPublishingOut = AllowPublishingOut.Checked;
            selectedRecordsType.MinimumPublishingOutProtectiveZone = MinimumPublishingOutProtectiveZone.SelectedValue;

            selectedRecordsType.GeneratePublishOutFilenames = GenerateFilenames.Checked;
            selectedRecordsType.UseDefaultsWhenPublishingOut = UseDefaults.Checked;
            selectedRecordsType.DefaultPublishingOutRecordsTypeUIControlValue = DefaultRecordsType.Text;

            selectedRecordsType.CacheDetailsForOpenWorkBoxes = CacheDetailsForOpenWorkBoxes.Checked;


            selectedRecordsType.AllowDocumentRecords = AllowDocumentRecords.Checked; 
            selectedRecordsType.DocumentMinimumProtectiveZone = ProtectiveZones.SelectedValue;

            selectedRecordsType.DocumentReferenceIDRequirement = DocumentReferenceIDRequirement.SelectedValue;
            selectedRecordsType.DocumentReferenceIDDescription = DocumentReferenceIDDescription.Text;

            selectedRecordsType.DocumentReferenceDateRequirement = DocumentReferenceDateRequirement.SelectedValue;
            selectedRecordsType.DocumentReferenceDateSource = DocumentReferenceDateSource.SelectedValue;
            selectedRecordsType.DocumentReferenceDateDescription = DocumentReferenceDateDescription.Text;

            selectedRecordsType.DocumentSeriesTagRequirement = DocumentSeriesTagRequirement.SelectedValue;
            selectedRecordsType.DocumentSeriesTagParentTermUIControlValue = DocumentSeriesTagParentTerm.Text;
            selectedRecordsType.DocumentSeriesTagAllowNewTerms = DocumentSeriesTagAllowNewTerms.Checked;
            selectedRecordsType.DocumentSeriesTagDescription = DocumentSeriesTagDescription.Text;

            selectedRecordsType.DocumentScanDateRequirement = DocumentScanDateRequirement.SelectedValue;
            selectedRecordsType.DocumentScanDateDescription = DocumentScanDateDescription.Text;


            selectedRecordsType.DocumentNamingConvention = DocumentNamingConvention.SelectedValue;
            selectedRecordsType.EnforceDocumentNamingConvention = EnforceDocumentNamingConvention.Checked;

            selectedRecordsType.FilingRuleLevel1 = FilingRuleLevel1.SelectedValue;
            selectedRecordsType.FilingRuleLevel2 = FilingRuleLevel2.SelectedValue;
            selectedRecordsType.FilingRuleLevel3 = FilingRuleLevel3.SelectedValue;
            selectedRecordsType.FilingRuleLevel4 = FilingRuleLevel4.SelectedValue;


            selectedRecordsType.Update();
            updatePanelWithRecordsTypeDetails(selectedRecordsType);
            popupMessageOnUpdate("Changes saved OK.");
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            resetPanelToSelectedTermValues();

            popupMessageOnUpdate("Changes cancelled.");
        }


        private void popupMessageOnUpdate(String message)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "PopupMessage", String.Format("alert('{0}');", message), true);
        }

    }
}
