<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSubjectTagRouting.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.EditSubjectTagRouting" DynamicMasterPageFile="~masterurl/default.master" %>

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

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name"><nobr>Subject Tag</nobr></div>
<div class="wbf-field-description">
Which root subject tag is the trigger for this routing rule?
</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<Taxonomy:TaxonomyWebTaggingControl ID="SubjectTag" ControlMode="display" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name"><nobr>Public Documents Library</nobr></div>
<div class="wbf-field-description">
Where should public document records be routed to?
</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:TextBox ID="PublicDocumentsLibrary" runat="server" columns="50"/>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name"><nobr>Extranet Documents Library</nobr></div>
<div class="wbf-field-description">
Where should password protected extranet document records be routed to?
</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:TextBox ID="ExtranetDocumentsLibrary" runat="server" columns="50"/>
</div>

</td>
</tr>


<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>

<asp:HiddenField ID="RouteIndex" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Edit Subject Tag Records Routing
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Edit Subject Tag Records Routing
</asp:Content>
