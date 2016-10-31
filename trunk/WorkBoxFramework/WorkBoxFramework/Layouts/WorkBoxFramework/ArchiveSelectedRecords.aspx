<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArchiveSelectedRecords.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ArchiveSelectedRecords" DynamicMasterPageFile="~masterurl/default.master" %>

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
Archive Record(s)
</div>
<div>
You have selected to archive the following records.
</div>
</td>
</tr>
</table>

<asp:HiddenField ID="SelectedRecords" runat="server" />
<asp:HiddenField ID="AllRecordIDsToArchive" runat="server" />
<asp:HiddenField ID="AllRecordFilenamesToArchive" runat="server" />

<table class="wbf-dialog-form">

<asp:Literal ID="RecordsBeingArchived" runat="server" />

<tr>
<td class="wbf-field-name-panel">
    <div class="wbf-field-name">Reason for archiving<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-value">
<asp:TextBox ID="ArchiveReason" runat="server" Text="" TextMode="multiline" Rows="4" Columns="50"></asp:TextBox>
</div>
<div class="wbf-field-error">

<asp:RequiredFieldValidator ID="ArchiveReasonValidator" ControlToValidate="ArchiveReason" ErrorMessage="You must provide a reason for archiving these documents." runat="server"/>
</div>
<div class="wbf-field-description">
Please provide a reason for why these records are being achived.
</div>

</td>
</tr>

<tr>
<td colspan="2" class="wbf-buttons-panel">
<p>
        <asp:Button ID="ArchiveAll" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Archive All" OnClick="archiveAllButton_OnClick" />

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
Archive Selected Records
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Archive Selected Records
</asp:Content>
