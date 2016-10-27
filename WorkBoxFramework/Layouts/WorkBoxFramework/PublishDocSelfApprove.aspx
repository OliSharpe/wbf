<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocSelfApprove.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocSelfApprove" DynamicMasterPageFile="~masterurl/default.master" %>

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

</asp:Content>


<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
</div>

<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-publish-out-title">
Self Approval Stage
</div>
<div>
This is a self-approval stage to ensure you have taken the document through all the appropriate checks and approvals. Once submitted all content within this document will be made viewable by the public.
</div>
</td>
</tr>
</table>

<asp:HiddenField ID="PublishingProcessJSON" runat="server" />

<table class="wbf-dialog-form">

<asp:Literal ID="DocumentsBeingPublished" runat="server" />

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Document Type</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
    <asp:Label ID="DocumentType" runat="server"></asp:Label>
</div>
</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Checklist<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-description">
        This record will be available to the public, please tick to confirm these checks have been completed.
</div>
<div class="wbf-field-value">
Have you:
</div>
<asp:HiddenField ID="CheckBoxesCodes" runat="server" />
<asp:PlaceHolder ID="CheckBoxes" runat="server" />

<div class="wbf-field-error">
<asp:Literal ID="CheckListError" runat="server" Text="" />
</div>


</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Information Asset Owner</div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-read-only-title">
<asp:Label ID="IAO" runat="server" />
</div>
<div class="wbf-field-description">
This is the person who is ultimately responsible for the correct handling of this information.
</div>
</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Approved By<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-value">
			<SharePoint:PeopleEditor id="PublishingApprovedBy" runat="server"
				SelectionSet="User"
				ValidatorEnabled="false"
				AllowEmpty = "true"
				MultiSelect = "true"
				/>
</div>
<div class="wbf-field-error">
<asp:Label ID="PublishingApprovedByError" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
Please enter the name of the person approving the publication of this document.
</div>
</td>
</tr>

<!--
<tr>
<td class="wbf-field-name-panel">
    <div class="wbf-field-name">Approval Statement</div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-value">
<asp:TextBox ID="PublishingApprovalStatement" runat="server" Text="" TextMode="multiline" Rows="4" Columns="50"></asp:TextBox>
</div>
<div class="wbf-field-error">

<asp:Label ID="PublishingApprovalStatementError" runat="server" />

<div class="wbf-field-description">
Please confirm the reason for publishing this record to the public records library
</div>

</div>
</td>
</tr>
-->


<tr>
<td colspan="2" class="wbf-buttons-panel">
<p>
        <asp:Button ID="ApproveAndPublish" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Approve and Publish" OnClick="publishButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Publishing Self Approval
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Publishing Self Approval
</asp:Content>
