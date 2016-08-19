<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateRecordsMetadata.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.UpdateRecordsMetadata" DynamicMasterPageFile="~masterurl/default.master" %>

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

<div class="wbf-dialog">

<asp:Panel ID="AccessDeniedPanel" runat="server" Visible="false">
<h2 class="wbf-dialog-title">Access Denied</h2>
<div class="wbf-dialog-message">
You are not a member of the records management group therefore you do not have permission to perform this action.
</div>
</asp:Panel>

<asp:Panel ID="UpdateRecordsMetadataPanel" runat="server">

<div class="wbf-dialog-message">
As a records manager you have the right to modify the following metadata fields.
</div>

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Record's Filename</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="Filename" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Record's Title</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="Title" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Functional Area</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="FunctionalArea" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Records Type</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="RecordsType" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Unique Record ID</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="RecordID" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Update Live / Archived:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:DropDownList ID="LiveOrArchived" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Update Protective Zone:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:DropDownList ID="ProtectiveZone" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Update Subject Tags:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<Taxonomy:TaxonomyWebTaggingControl ID="SubjectTags" ControlMode="display" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="SubjectTagsErrorMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Reason for change<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="ReasonForChange" TextMode="MultiLine" Rows="4" Columns="50" runat="server" />
</div>
<div class="wbf-field-error">
<asp:RequiredFieldValidator ControlToValidate="ReasonForChange" ErrorMessage="You must provide a reason for making this change." runat="server"/>
</div>
</td>
</tr>


<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="UpdateButton" runat="server" Text="Update Record"  OnClick="updateButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>

<asp:HiddenField ID="OnRecordsLibrary" runat="server" />
<asp:HiddenField ID="ListID" runat="server" />
<asp:HiddenField ID="ItemID" runat="server" />

</asp:Panel>
</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Update Record's Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Update Record's Metadata
</asp:Content>
