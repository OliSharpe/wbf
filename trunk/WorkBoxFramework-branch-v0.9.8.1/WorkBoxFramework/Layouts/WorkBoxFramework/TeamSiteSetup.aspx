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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeamSiteSetup.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.TeamSiteSetup" DynamicMasterPageFile="~masterurl/default.master" %>

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

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<div class="wbf-application-page">

<h1>Team Site Setup</h1>

	<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">

<!-- Team Name Section -->
<wssuc:InputFormSection
	id="TeamNameSection"
	title="Team Name"
	description="Changing the name here will change both the Team Site's title and the Team's term name."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="TeamName" Columns="40" runat="server" />
                        </td>
                                                    

                    </tr>

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="TeamAcronym" runat="server" Columns="10"></asp:TextBox> Team Achronym
                        </td>
                    </tr>


				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Team Term Section -->
<wssuc:InputFormSection
	id="TeamTermSection"
	title="Team Term"
	description="Make the connection between this team site and the 'team term' that represents the team."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <b>Either:</b> pick an existing term to define the team.
                        </td>
                    </tr>

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <Taxonomy:TaxonomyWebTaggingControl ID="TeamTerm" ControlMode="display" runat="server" />
                        </td>
                    </tr>

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <b>Or:</b> pick the parent term under which to automatically <b><em>create</em></b> a new term that will represent this team. The name of the new term will be set as this team site's title.
                        </td>
                    </tr>

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <Taxonomy:TaxonomyWebTaggingControl ID="ParentTeamTerm" ControlMode="display" runat="server" />
                        </td>
                    </tr>


					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamTermStatus" runat="server" Text="" ForeColor="Red"/>
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

<!-- Team Manager Section -->
<wssuc:InputFormSection
	id="TeamManagerSharePointUserSection"
	title="Team Manager"
	description="Specify which user is the team manager (if any)."
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

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamManagerMessage" runat="server" Text=""/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>




<!-- Team Owners Group Section -->
<wssuc:InputFormSection
	id="TeamOwnersSharePointUserGroupSection"
	title="Team Owners SharePoint Group"
	description="Specify which SharePoint group defines the owners of this team."
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

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamOwnersMessage" runat="server" Text=""/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Team Membership Group Section -->
<wssuc:InputFormSection
	id="TeamMembersSharePointUserGroupSection"
	title="Team Members SharePoint Group"
	description="Specify which SharePoint group defines the membership of this team."
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

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamMembersMessage" runat="server" Text=""/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Team Membership Group Section -->
<wssuc:InputFormSection
	id="TeamPublishersUserGroupSection"
	title="Team Publishers SharePoint User Group"
	description="Select the name of the SharePoint user group that defines who from this team can publish to the public. If this selection is left blank then no-one can publish from this team's work boxes to the public websites."
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

					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:Label ID="TeamPublishersMessage" runat="server" Text=""/>
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
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>




<!-- Buttons Section -->
<wssuc:ButtonSection runat="server" ShowStandardCancelButton="false">
	<Template_Buttons>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="okButton_OnClick" Text="<%$Resources:wss,multipages_okbutton_text%>" id="okButton" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="cancelButton_OnClick" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="cancelButton" accesskey="<%$Resources:wss,cancelbutton_accesskey%>"/>
	</Template_Buttons>
</wssuc:ButtonSection>


	</table> 

</div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Team Site Setup
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Team Site Setup
</asp:Content>
