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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBFWebPartsConfig.aspx.cs" Inherits="WBFWebParts.Layouts.WBFWebParts.WBFWebPartsConfig" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div class="wbf-admin-page" style="padding: 5px; ">

<h1>WBF Web Parts Configuration</h1>
<p>
Configure the features of the <b>Related Documents</b> and <b>Documents Group</b> web parts.
</p>

	<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">


<!-- Farm Instance Section -->
<wssuc:InputFormSection
	id="UsePublicOrProtectedLibrarySection"
	title="Use Public Or Protected Library"
	description="On public facing websites you must use the public library"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:DropDownList ID="RecordsLibraryToUse" runat="server" columns="50"/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>



<!-- Records Center Section -->
<wssuc:InputFormSection
	id="UseExtranetLibrarySection"
	title="Use Extranets Library"
	description="Should users also have the option to pick records from the extranet library?"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Use Extranet Library?</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:CheckBox ID="UseExtranetLibrary" runat="server" columns="50"/>
                        </td>
                    </tr>

				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Records Center Section -->
<wssuc:InputFormSection
	id="FeaturesToShowSection"
	title="Web Part Features To Show"
	description="Which of these features should be shown"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Show File Icons</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:CheckBox ID="ShowFileIcons" runat="server" columns="50"/>
                        </td>
                    </tr>
					<tr>
                        <td>
                            <b><nobr>Show KB File Size</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:CheckBox ID="ShowKBFileSize" runat="server" columns="50"/>
                        </td>
                    </tr>
					<tr>
                        <td>
                            <b><nobr>Show Description Field</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:CheckBox ID="ShowDescription" runat="server" columns="50"/>
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
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" CausesValidation="False" OnClick="cancelButton_OnClick" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="cancelButton" accesskey="<%$Resources:wss,cancelbutton_accesskey%>"/>
	</Template_Buttons>
</wssuc:ButtonSection>

	</table> 

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
WBF Web Parts Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
WBF Web Parts Configuration
</asp:Content>
