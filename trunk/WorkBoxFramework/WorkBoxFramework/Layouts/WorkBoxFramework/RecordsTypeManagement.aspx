<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="LinkSection" src="/_controltemplates/LinkSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/ButtonSection.ascx" %> 
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecordsTypeManagement.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.RecordsTypeManagement" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFScriptRegistration"
        name="WorkBoxFramework/WorkBoxFramework.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

<style type="text/css">
BODY #s4-leftpanel { width: 0 !important; }
.s4-ca { margin-left: 0 !important; }
.wbf-metadata-title { padding: 5px; font-weight: bold; }
.wbf-metadata-details { padding: 2px; margin-left: 15px; }
.wbf-details-panel { padding: 2px; }
td.wbf-management-title-panel { padding: 2px; border: 1px solid gray; background-color: #ebf3ff; text-align: center; }
td.wbf-management-panel { padding: 2px; border: 1px solid gray; background-color: #ebf3ff; }
td.wbf-management-selector-panel { padding: 2px; padding-left:10px; border: 1px solid gray; background-color: #fff; }
td.wbf-management-details-title-panel {  text-align:center; background-color: #e0e0e0;  }
.wbf-management-title { }
td.ms-authoringcontrols { border-left: 1px solid gray; }
td.ms-authoringcontrols td { border: 0px; }
</style>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderLeftNavBar" style="display:none" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server"> 

<div class="wbf-application-page">
   		<table cellspacing="6">
   			<tr>	
   				<td colspan="2" class="wbf-management-title-panel">
			   		<h1 class="wbf-management-title">Records Type Management</h1>
   				</td>
   			</tr>
   			<tr>
   				<td valign="top" width="100px" class="wbf-management-selector-panel">
                <div>
   					<h3>Select Records Type</h3>

                    <SharePoint:SPRememberScroll
      id="MyTreeViewRememberScroll"
      runat="server" onscroll="javascript:_spRecordScrollPositions(this);"
      Style="overflow: auto;height: 700px;width: 300px; ">
  <SharePoint:SPTreeView
        id="AllRecordsTypesTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="1"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="AllRecordsTypesTreeView_SelectedNodeChanged"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"
        ExpandImageUrl="/_layouts/images/tvplus.gif"
        CollapseImageUrl="/_layouts/images/tvminus.gif"
        NoExpandImageUrl="/_layouts/images/tvblank.gif" />
</SharePoint:SPRememberScroll>

</div>
   				</td>
   				<td valign="top" class="wbf-management-panel">
                       <asp:UpdatePanel ID="ShowSelectionPanel" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="AllRecordsTypesTreeView" EventName="SelectedNodeChanged" />
                            </Triggers>
                            <ContentTemplate>

                    <div>

	<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">

        <tr>
			<td class="ms-sectionline" height="1" colspan="2"><img src="/_layouts/images/blank.gif" width='1' height='1' alt="" /></td>
		</tr>
        <tr>
			<td colspan="2" class="wbf-management-details-title-panel">
                            <h2>Records Type Details</h2>                   
            </td>
		</tr>


<!-- Records Type Name Section -->
<wssuc:InputFormSection
	id="RecordsTypeNameSection"
	title="Records Type Name"
	Description=""
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <asp:TextBox ID="RecordsTypeName" runat="server" Columns="50"></asp:TextBox>
				    </div>
                    <div class="wbf-details-panel">
                            Last Modified: <asp:Label ID="LastModfiedDate" runat="server" />
				    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Records Type Description Section -->
<wssuc:InputFormSection
	id="RecordsTypeDescriptionSection"
	title="Records Type Description"
	Description="Enter some scope notes here to disinguish this records type from others. In certain contexts this description will be displayed to users."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <asp:TextBox ID="RecordsTypeDescription" runat="server" Columns="40" Rows="4" TextMode="MultiLine"></asp:TextBox>
				    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Records Type Description Section -->
<wssuc:InputFormSection
	id="RecordsTypeFunctionalArea"
	title="Records Type Functional Area"
	Description="If this field is left blank then the functional area(s) of the records will be taken from the work box owning team. The 'allow other' tick box only applies if a default has been set."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                        Default functional area:
                    </div>
                    <div class="wbf-details-panel">
                            <Taxonomy:TaxonomyWebTaggingControl ID="DefaultFunctionalArea" ControlMode="display" runat="server" />
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:CheckBox ID="AllowOtherFunctionalAreas" runat="server"/>
                            Allow other functional areas?
                            </span>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


        <tr>
			<td class="ms-sectionline" height="1" colspan="2"><img src="/_layouts/images/blank.gif" width='1' height='1' alt="" /></td>
		</tr>
        <tr>
			<td colspan="2" style=" text-align:center; background-color: #e0e0e0; ">
                            <h3>Work Box Records</h3>
            </td>
		</tr>

<!-- Creating New Work Boxes -->
<wssuc:InputFormSection
	id="CreatingNewWorkBoxes"
	title="Creating New Work Boxes"
	Description="Should it be possible to create work boxes of this records type?"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:CheckBox ID="AllowWorkBoxRecords" runat="server"/>
                            Allow work box records?
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            Who can create new work boxes of this records type?
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:DropDownList ID="WhoCanCreateNewWorkBoxes" runat="server"/>
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:TextBox ID="CreateNewWorkBoxText" runat="server"/>
                            Create new work box text
                            </span>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Records Type Work Box Colleciton URL Section -->
<wssuc:InputFormSection
	id="WorkBoxCollecitonURLSection"
	title="Work Box Colleciton URL"
	Description="Enter the URL for the work box collection that holds work boxes of this records type. If a given records type does not have a specified URL then it will use the URL specified for its parent records type."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="WorkBoxCollectionURLProperty" runat="server" Columns="50"></asp:TextBox>
						</td>
                    </tr>
                    <tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="WorkBoxCollectionURL" runat="server" Columns="50"></asp:Label>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Work Box ID -->
<wssuc:InputFormSection
	id="WorkBoxIDSection"
	title="Work Box ID"
	Description="Specify how the work boxes of this type should be uniquely identified."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>

                <div class="wbf-details-panel">
                    <span>
                        <asp:TextBox ID="WorkBoxUniqueIDPrefix" runat="server"></asp:TextBox>
                        Set the Unique ID Prefix
                    </span>
                </div>
                <div class="wbf-details-panel">
                    <span>
                        <asp:DropDownList ID="WorkBoxLocalIDSource" runat="server"></asp:DropDownList>
                        Select the Local ID source
                    </span>
                </div>
                <div class="wbf-details-panel">
                    <span>
                        <asp:TextBox ID="WorkBoxGeneratedLocalIDOffset" runat="server" Columns="10"></asp:TextBox>
                        Offset for generated Local IDs
                    </span>
                </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Work Box Metadata -->
<wssuc:InputFormSection
	id="WorkBoxMetadataSection"
	title="Work Box Metadata Requirements"
	Description="Which of the following metadata should be required, optional or hidden?."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>

                    <div class="wbf-metadata-title">
                            Short Title
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="WorkBoxShortTitleRequirement" runat="server"/>
                            on the new work box form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the new work box form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="WorkBoxShortTitleDescription" runat="server" columns="55"/>
                    </div>


                    <div class="wbf-metadata-title">
                            Reference ID
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="WorkBoxReferenceIDRequirement" runat="server"/>
                            on the new work box form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the new work box form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="WorkBoxReferenceIDDescription" runat="server" columns="55"/>
                    </div>


                    <div class="wbf-metadata-title">
                            <hr />
                            Reference Date
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="WorkBoxReferenceDateRequirement" runat="server"/>
                            on the new work box form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the new work box form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="WorkBoxReferenceDateDescription" runat="server" columns="55"/>
                    </div>
                    <div class="wbf-metadata-title">
                            <hr />
                            Series Tag
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="WorkBoxSeriesTagRequirement" runat="server"/>
                            on the new work box form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            Select the parent term of the series tags to use:
                    </div>
                    <div class="wbf-metadata-details">
                            <Taxonomy:TaxonomyWebTaggingControl ID="WorkBoxSeriesTagParentTerm" ControlMode="display" runat="server" />
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:CheckBox ID="WorkBoxSeriesTagAllowNewTerms" runat="server"/>
                            Allow users to create new series tags?
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the new work box form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="WorkBoxSeriesTagDescription" runat="server" columns="55"/>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Work Box Naming -->
<wssuc:InputFormSection
	id="WorkBoxNamingSection"
	title="Work Box Naming Convention"
	Description="Specify how the work boxes of this type should be named."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            Select a work box naming convention: 
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:DropDownList ID="WorkBoxNamingConventions" runat="server"></asp:DropDownList>
                            </span>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<wssuc:InputFormSection
	id="AutoCloseSection"
	title="Work Box Auto Close Rule"
	Description="Enter the number of days after which the work box will be closed if it hasn't been modified. (0 or blank means never auto close)"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            Select the auto close trigger date:
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:DropDownList ID="AutoCloseTriggerDate" runat="server"></asp:DropDownList>
                            </span>
                    </div>
                    <div class="wbf-details-panel">
				<table border="0" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                        Auto close after:
                        </td>
                        <td width="40" align="right">
                            <asp:TextBox ID="AutoCloseTimeScalar" runat="server" Columns="4"/>
                        </td>
						<td valign="top" align="left" >
                            <asp:DropDownList ID="AutoCloseTimeUnits" runat="server"></asp:DropDownList>
						</td>
                    </tr>
				</table>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Records Type Retention -->
<wssuc:InputFormSection
	id="RetentionSection"
	title="Work Box Retention Period"
	Description="Enter the time period for which work boxes should be kept after closure."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            Select the retention trigger date:
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:DropDownList ID="RetentionTriggerDate" runat="server"></asp:DropDownList>
                            </span>
                    </div>
                    <div class="wbf-details-panel">
				<table border="0" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                        Delete after:
                        </td>
                        <td width="40" align="right">
                            <asp:TextBox ID="RetentionScalar" runat="server" Columns="4"/>
                        </td>
						<td valign="top" align="left" >
                            <asp:DropDownList ID="RetentionUnits" runat="server"></asp:DropDownList>
						</td>
                    </tr>
				</table>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>





<!-- Publishing Out Section -->
<wssuc:InputFormSection
	id="PublishingOutSection"
	title="Publishing Out from Work Box"
	Description="Configure the details of default publishing out behaviour for work boxes of this records type."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:CheckBox ID="AllowPublishingOut" runat="server"/>
                            Allow publishing out of documents?
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:DropDownList ID="MinimumPublishingOutProtectiveZone" runat="server"/>
                            Minimum publishing out protective zone?
                            </span>
                    </div>
                    <div class="wbf-details-panel" style=" display:none; ">
                            <span>
                            <asp:CheckBox ID="GenerateFilenames" runat="server"/>
                            Generate filenames using document naming convention?
                            </span>
                    </div>
                    <div class="wbf-details-panel" style=" display:none; ">
                            <span>
                            <asp:CheckBox ID="UseDefaults" runat="server"/>
                            Use defaults when publishing out?
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            Default document records type if different from this records type:
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <Taxonomy:TaxonomyWebTaggingControl ID="DefaultRecordsType" ControlMode="display" runat="server" />
                            </span>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>




<!-- Cached Details Section -->
<wssuc:InputFormSection
	id="CachedDetailsSection"
	title="Cache Details"
	Description="Configure whether or not to keep cached details of work boxes of this type."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:CheckBox ID="CacheDetailsForOpenWorkBoxes" runat="server"/>
                            Cache details for open Work Boxes?
                            </span>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>




        <tr>
			<td class="ms-sectionline" height="1" colspan="2"><img src="/_layouts/images/blank.gif" width='1' height='1' alt="" /></td>
		</tr>
        <tr>
			<td colspan="2" style=" text-align:center; background-color: #e0e0e0; ">
                            <h3>Document Records</h3>
            </td>
		</tr>

        <!-- Allow Document Records -->
<wssuc:InputFormSection
	id="AllowDocumentRecordsSection"
	title="Allow Document Records"
	Description="Should it be possible to create document records of this records type?"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td align="left" colspan="2">
                            <span>
                            <asp:CheckBox ID="AllowDocumentRecords" runat="server"/>
                            Allow document records?
                            </span>
						</td>
                    </tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Metadata Requirements -->
<wssuc:InputFormSection
	id="DocumentMetadataRequirementsSection"
	title="Document Metadata Requirements"
	Description="When a document is published out to the records center, and thereby declared as a record, what metadata must it have?"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-metadata-title">
                            Protective Zone
                    </div>
                    <div class="wbf-metadata-details">
                        <span>
                            <asp:DropDownList ID="ProtectiveZones" runat="server"/>
                            What is the minimum protective zone?
                        </span>
                    </div>
                    <div class="wbf-metadata-title">
                            <hr />
                            Reference ID
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="DocumentReferenceIDRequirement" runat="server"/>
                            on the publish out form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the publish out form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="DocumentReferenceIDDescription" runat="server" columns="55"/>
                    </div>
                    <div class="wbf-metadata-title">
                            <hr />
                            Reference Date
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="DocumentReferenceDateRequirement" runat="server"/>
                            on the publish out form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="DocumentReferenceDateSource" runat="server"/>
                            Set date using this default source
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the publish out form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="DocumentReferenceDateDescription" runat="server" columns="55"/>
                    </div>
                    <div class="wbf-metadata-title">
                            <hr />
                            Series Tag
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="DocumentSeriesTagRequirement" runat="server"/>
                            on the publish out form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            Select the parent term of the series tags to use:
                    </div>
                    <div class="wbf-metadata-details">
                            <Taxonomy:TaxonomyWebTaggingControl ID="DocumentSeriesTagParentTerm" ControlMode="display" runat="server" />
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:CheckBox ID="DocumentSeriesTagAllowNewTerms" runat="server"/>
                            Allow users to create new series tags?
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the publish out form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="DocumentSeriesTagDescription" runat="server" columns="55"/>
                    </div>
                    <div class="wbf-metadata-title">
                            <hr />
                            Scan Date
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            <asp:DropDownList ID="DocumentScanDateRequirement" runat="server"/>
                            on the publish out form.
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                            <span>
                            Description on the publish out form:
                            </span>
                    </div>
                    <div class="wbf-metadata-details">
                        <asp:TextBox ID="DocumentScanDateDescription" runat="server" columns="55"/>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>





<!-- Records Type Retention -->
<wssuc:InputFormSection
	id="DocumentNamingConventionSection"
	title="Document Naming Convention"
	Description="Enter the naming convention to be used by documents of this records type."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <span>
                            Select a document naming convention:
                            </span>
                    </div>
                    <div class="wbf-details-panel">
                            <span>
                            <asp:DropDownList ID="DocumentNamingConvention" runat="server"></asp:DropDownList>
                            </span>
                    </div>
                    <div class="wbf-details-panel" style=" display:none; ">
                            <span>
                            <asp:CheckBox ID="EnforceDocumentNamingConvention" runat="server"/>
                            Enforce the document naming convention?
                            </span>
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Records Type Retention -->
<wssuc:InputFormSection
	id="FilingRules"
	title="Filing Rules"
	Description="Set the filing rules that will be used to create folder parts when documents are published to the records library."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td width="50" align="right">
                            <asp:DropDownList ID="FilingRuleLevel1" runat="server"></asp:DropDownList>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                        Level 1 filing rule.
						</td>
                    </tr>
					<tr>
                        <td width="50" align="right">
                            <asp:DropDownList ID="FilingRuleLevel2" runat="server"></asp:DropDownList>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                        Level 2 filing rule.
						</td>
                    </tr>
					<tr>
                        <td width="50" align="right">
                            <asp:DropDownList ID="FilingRuleLevel3" runat="server"></asp:DropDownList>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                        Level 3 filing rule.
						</td>
                    </tr>
					<tr>
                        <td width="50" align="right">
                            <asp:DropDownList ID="FilingRuleLevel4" runat="server"></asp:DropDownList>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                        Level 4 filing rule.
						</td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Records Type Retention -->
<wssuc:InputFormSection
	id="DocumentRetentionSection"
	title="Individual Documents Retention Period"
	Description="Click on the link to go to set the retention period for individual documents of this type stored in the records center."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <asp:Literal ID="LinkToRecordsCenterConfig" runat="server"/>
                        </td>
                    </tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Buttons Section -->
<wssuc:ButtonSection runat="server" ShowStandardCancelButton="false">
	<Template_Buttons>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="saveButton_OnClick" Text="Save Changes" id="saveButton" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="cancelButton_OnClick" Text="Cancel Changes" id="cancelButton" accesskey="<%$Resources:wss,cancelbutton_accesskey%>"/>
	</Template_Buttons>
</wssuc:ButtonSection>


	</table> 

                    </div>


                            </ContentTemplate>

                       </asp:UpdatePanel>
   				</td>
   			</tr>
   		</table>

</div>




</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Records Type Management
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Records Type Management
</asp:Content>
