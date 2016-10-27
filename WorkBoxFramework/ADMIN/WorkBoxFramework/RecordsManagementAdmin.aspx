<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecordsManagementAdmin.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.RecordsManagementAdmin" DynamicMasterPageFile="~masterurl/default.master" %>

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

<div class="wbf-admin-page">

<h2>Records Management Admin</h2>

<p>
The work box framework (WBF) has various features that help with the management of documents as records of business. 
This admin page is a central place to configure some of the key locations on this farm involved with such records management.
</p>

<table class="wbf-dialog-form">


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Public Document Email Alerts To</div>
<div class="wbf-field-description">
Where should the alert emails for new public documents be sent to?
</div>
</td>
<td class="wbf-field-value-panel">
<div class="wbf-field-value">
<asp:TextBox ID="PublicDocumentEmailAlertsTo" runat="server" Columns="55" />
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Subject Tags' Records Routings</div>
<div class="wbf-field-description">
Here you can configure which root subject tags will be used to determine additional routing for public and extranet document records.
</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Literal ID="SubjectTagsRecordsRoutings" runat="server" />
</div>

</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="SaveButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_OnClick"/>

</td>
</tr>

</table>

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Records Management Admin
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Records Management Admin
</asp:Content>
