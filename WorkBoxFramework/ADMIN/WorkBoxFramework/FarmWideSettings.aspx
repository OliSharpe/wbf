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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FarmWideSettings.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.FarmWideSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<div class="wbf-admin-page">

<h1>Farm Wide Settings</h1>

	<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">


<!-- Farm Instance Section -->
<wssuc:InputFormSection
	id="EmailMailToSettings"
	title="MailTo Settings"
	description="This setting determines how the WBF uses the mailto tag for lists of email addresses"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Use mailto links</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:CheckBox ID="UseMailToLinks" runat="server" Checked="true" />
                        </td>
                    </tr>

					<tr>
                        <td>
                            <b><nobr>Character limit for mailto links</nobr></b>
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="CharacterLimitForMailToLinks" runat="server" columns="20"/>
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
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="OKButton_OnClick" Text="<%$Resources:wss,multipages_okbutton_text%>" id="OKButton" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" CausesValidation="False" OnClick="CancelButton_OnClick" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="CancelButton" accesskey="<%$Resources:wss,cancelbutton_accesskey%>"/>
	</Template_Buttons>
</wssuc:ButtonSection>

	</table> 

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Farm Wide Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Farm Wide Settings
</asp:Content>
