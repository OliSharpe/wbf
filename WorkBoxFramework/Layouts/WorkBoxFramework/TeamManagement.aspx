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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeamManagement.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.TeamManagement" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFjQueryScriptRegistration"
        name="WorkBoxFramework/jquery-1.11.3.min.js"
        language="javascript"
        localizable="false"
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

<script type="text/javascript">

    $(document).ready(function () {
        $("#<%=AllTeamsTreeView.ClientID  %> a.ms-navitem").click(function () {
            WorkBoxFramework_clearPeopleEditors();
            return true;
        });
    });

</script>


<div class="wbf-application-page">
   		<table cellspacing="6">
   			<tr>	
   				<td colspan="2" class="wbf-management-title-panel">
			   		<h1>Team Management</h1>
   				</td>
   			</tr>
   			<tr>
   				<td valign="top" width="100px" class="wbf-management-selector-panel">
   					<h3>Select Team</h3>

                    <SharePoint:SPRememberScroll
      id="MyTreeViewRememberScroll"
      runat="server" onscroll="javascript:_spRecordScrollPositions(this);"
      Style="overflow: auto;height: 700px;width: 300px; ">
  <SharePoint:SPTreeView
        id="AllTeamsTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="1"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="AllTeamsTreeView_SelectedNodeChanged"
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

   				</td>
   				<td valign="top" class="wbf-management-panel">

                <asp:UpdatePanel ID="ShowSelectionPanel" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="AllTeamsTreeView" EventName="SelectedNodeChanged" />
                            </Triggers>
                            <ContentTemplate>

                    <div>
       <table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">

        <tr>
			<td class="ms-sectionline" height="1" colspan="2"><img src="/_layouts/images/blank.gif" width='1' height='1' alt="" /></td>
		</tr>
        <tr>
			<td colspan="2" class="wbf-management-details-title-panel">
                    <h2>Team Details</h2>
            </td>
		</tr>

<!-- Team Name Section -->
<wssuc:InputFormSection
	id="TeamNameSection"
	title="Team Name"
	Description=""
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="TeamName" runat="server" Columns="50"></asp:TextBox>
						</td>
					</tr>
                    <tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamGUID" runat="server" Columns="50"></asp:Label>
						</td>
					</tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Team Acronym Section -->
<wssuc:InputFormSection
	id="TeamAcronymSection"
	title="Team Acronym"
	Description="Enter the short name or acronym for the team that can be used in work box or document names."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="TeamAcronym" runat="server" Columns="20"></asp:TextBox>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Team Functional Areas Section -->
<wssuc:InputFormSection
	id="TeamFunctionalArea"
	title="Team's Functional Area"
	Description="If this field is left blank then this team will inherit its functional area(s) from it's parent team."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
                    <div class="wbf-details-panel">
                            <Taxonomy:TaxonomyWebTaggingControl ID="TeamFunctionalAreas" ControlMode="display" runat="server" />
                    </div>
                    <div class="wbf-details-panel">
                            <asp:Label ID="InheritedFunctionalAreas" runat="server" />
                    </div>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Team's Site URL Section -->
<wssuc:InputFormSection
	id="TeamsSiteURLSection"
	title="Team's Site URL"
	Description="Enter the URL for the team's site."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="TeamsSiteURL" runat="server" Columns="50"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamsSiteGUID" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Team Manager User Section -->
<wssuc:InputFormSection
	id="TeamManagerUserSection"
	title="Team Manager"
	Description="Select the name of the manager for the team (optional)"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<SharePoint:PeopleEditor id="TeamManager" runat="server"
				SelectionSet="User"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "false"
				/>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Team Owners SharePoint User Groups Section -->
<wssuc:InputFormSection
	id="TeamOwnersUserGroupSection"
	title="Team Owners SharePoint User Group"
	Description="Select the name of the SharePoint user group that defines who are the owners of this team."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<SharePoint:PeopleEditor id="TeamOwnersSharePointUserGroup" runat="server"
				SelectionSet="SPGroup"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "false"
				/>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Team Members SharePoint User Groups Section -->
<wssuc:InputFormSection
	id="TeamMembersUserGroupSection"
	title="Team Members SharePoint User Group"
	Description="Select the name of the SharePoint user group that defines the membership of this team."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<SharePoint:PeopleEditor id="TeamMembersSharePointUserGroup" runat="server"
				SelectionSet="SPGroup"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "false"
				/>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Team Publishers SharePoint User Groups Section -->
<wssuc:InputFormSection
	id="TeamPublishersUserGroupSection"
	title="Team Publishers SharePoint User Group"
	Description="Select the name of the SharePoint user group that defines who from this team can publish to the public. If this selection is left blank then no-one can publish from this team's work boxes to the public websites."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<SharePoint:PeopleEditor id="TeamPublishersSharePointUserGroup" runat="server"
				SelectionSet="SPGroup"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "false"
				/>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Team Records Types List URL Section -->
<wssuc:InputFormSection
	id="TeamRecordsTypesListUrlSection"
	title="Records Types List URL"
	Description="Enter the URL fo the configuration list that determines what records types should be displayed on this team's site."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="RecordsTypesListUrl" columns=70 runat="server"></asp:TextBox>
						</td>
                    </tr>

                    <!--
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="CommonActivitiesListUrl" columns=70 runat="server"></asp:TextBox>
						</td>
                    </tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="FunctionalActivitiesListUrl" columns=70 runat="server"></asp:TextBox>
						</td>
                    </tr>
                    -->

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
Team Management
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Team Management
</asp:Content>
