<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="LinkSection" src="/_controltemplates/LinkSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/ButtonSection.ascx" %> 
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FarmWideAdmin.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.FarmWideAdmin" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<h1>Farm Wide Admin of Work Box Framework</h1>

	<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">


<!-- Farm Instance Section -->
<wssuc:InputFormSection
	id="FarmInstanceSection"
	title="Farm Instance"
	description="This setting let's the Work Box Framework know where it is running."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:DropDownList ID="FarmInstance" runat="server" columns="50"/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Records Center Section -->
<wssuc:InputFormSection
	id="RecordsCenterSection"
	title="Records Center"
	description="Settings for the farm's records center."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Protected Records Library URL</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="ProtectedRecordsLibraryURL" runat="server" columns="50"/>
                        </td>
                    </tr>

					<tr>
                        <td>
                            <b><nobr>Public Records Library URL</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="PublicRecordsLibraryURL" runat="server" columns="50"/>
                        </td>
                    </tr>

					<tr>
                        <td>
                            <b><nobr>Public Extranet Records Library URL</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="PublicExtranetRecordsLibraryURL" runat="server" columns="50"/>
                        </td>
                    </tr>


				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Team Sites Section -->
<wssuc:InputFormSection
	id="TeamSitesSection"
	title="Team Sites"
	description="All work box collections in the farm should be registered here."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Site Collection URL</nobr></b><br />
                            The url for the site collection which is hosting the team sites.
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="TeamSitesSiteCollectionURL" runat="server" columns="50"/>
                        </td>
                    </tr>

					<tr>
                        <td>
                            <b><nobr>Open Work Boxes Cached Details</nobr></b><br />
                            The url for the list that holds the cached details of all open work boxes.
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="OpenWorkBoxesCachedDetailsListURL" runat="server" columns="50"/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Records Managers Section -->
<wssuc:InputFormSection
	id="RecordsManagersSection"
	title="Records Managers Groups"
	description="These groups are used to control who can perform administrative functions on the records library."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Records Managers Group Name</nobr></b><br />
                            Only members of this SharePoint group will be able to edit some of the metadata of a published record.
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="RecordsManagersGroupName" runat="server" columns="50"/>
                        </td>
                    </tr>

					<tr>
                        <td>
                            <b><nobr>Records System Admin Group Name</nobr></b><br />
                            Only members of this SharePoint group will be able to use the context menu option to delete records from the library.
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="RecordsSystemAdminGroupName" runat="server" columns="50"/>
                        </td>
                    </tr>



				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Team Sites Section -->
<wssuc:InputFormSection
	id="TimerJobsManagement"
	title="Timer Jobs Management"
	description="The site on which the details of the various Work Box Framework timer jobs are managed and reported on."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Management Site URL</nobr></b><br />
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="TimerJobsManagementSiteURL" runat="server" columns="50"/>
                        </td>
                    </tr>
					<tr>
                        <td>
                            <b><nobr>Server Name</nobr></b><br />
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="TimerJobsServerName" runat="server" columns="50"/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Work Box Collections Section -->
<wssuc:InputFormSection
	id="WorkBoxCollectionsSection"
	title="Work Box Collections"
	description="All work box collections in the farm should be registered here."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Work Box Collections</nobr></b><br />
                            A semi-colon ';' delimted list of the URLs of all work box collections.
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="AllWorkBoxCollections" runat="server" columns="50"/>
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





</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Farm Wide Admin
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Farm Wide Admin
</asp:Content>
