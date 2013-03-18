<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewWorkBox.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.NewWorkBox" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
<style type="text/css">
td.wbf-records-type { border: 0px; }
td.wbf-metadata-title-panel { width: 300px; padding: 8px; border-top:solid 1px grey; vertical-align: top; }
td.wbf-metadata-value-panel { width: 405px; padding: 8px; border-top:solid 1px grey; vertical-align: top; background-color: #f1f1f2;  }
td.wbf-buttons-panel { border-top:solid 1px grey; text-align: center; vertical-align: top; }
.wbf-metadata-title { font-weight: bold; padding-bottom: 2px; }
.wbf-metadata-description { font-weight: normal; padding: 2px; }
.wbf-metadata-read-only-value { font-weight: bold; padding: 2px; }
.wbf-metadata-error { font-weight: normal; padding: 0px; color: Red; }
td.wbf-create-new-title { padding: 6px; }
div.wbf-create-new-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom: 4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
</style>

<table cellpadding="0" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle">
<img src="/_layouts/images/WorkBoxFramework/work-box-48.png" alt="Work Box Icon"/>
</td>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-create-new-title">
<asp:Label ID="CreateNewWorkBoxText" runat="server" />
</div>
<div>
You must enter the following metadata details for your new Work Box.
</div>
</td>
</tr>
</table>

<table width="100%" cellpadding="5" cellspacing="0">


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Functional Area</div>
</td>
<td class="wbf-metadata-value-panel">

<% if (functionalAreaFieldIsEditable)
   { %>
<Taxonomy:TaxonomyWebTaggingControl ID="FunctionalAreaField" ControlMode="display" runat="server" />
<% }
   else
   { %>
<div class="wbf-metadata-read-only-value">
<asp:Label ID="ReadOnlyFunctionalAreaField" runat="server" />
</div>
<% } %>

<div class="wbf-metadata-error">
<asp:Label ID="FunctionalAreaFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Records Type</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:Label ID="RecordsType" runat="server" />
</div>

<div class="wbf-metadata-error">
<asp:Label ID="RecordsTypeFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Work Box Naming Convention</div>
</td>
<td class="wbf-metadata-value-panel">

    <div class="wbf-metadata-read-only-value">
    <asp:Label ID="WorkBoxNamingConvention" runat="server"></asp:Label>
    </div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Work Box Template</div>
</td>
<td class="wbf-metadata-value-panel">

<% if (onlyOneWorkBoxTemplate)
   { %>
<div class="wbf-metadata-read-only-value">
    <asp:Label ID="WorkBoxTemplate" runat="server"></asp:Label>
    </div>
    <div class="wbf-metadata-error">
<asp:Label ID="NoTemplatesError" runat="server" Text="" ForeColor="Red"/>
</div>
    <asp:HiddenField ID="WorkBoxTemplateID" runat="server"></asp:HiddenField>

<% }
   else
   { %>
    <asp:DropDownList ID="WorkBoxTemplates" runat="server"></asp:DropDownList>
<% } %>

</td>
</tr>

<% if (showShortTitle)
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title"><asp:Label ID="ShortTitleTitle" runat="server"/></div>
<div class="wbf-metadata-description">
<asp:Label ID="ShortTitleDescription" runat="server"/>
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">
    <asp:TextBox ID="WorkBoxShortTitle" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="WorkBoxShortTitleMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>




<% if (showReferenceID)
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title"><asp:Label ID="ReferenceIDTitle" runat="server"/></div>
<div class="wbf-metadata-description">
<asp:Label ID="ReferenceIDDescription" runat="server"/>
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">
    <asp:TextBox ID="ReferenceID" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="ReferenceIDMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>

<% if (showReferenceDate)
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title"><asp:Label ID="ReferenceDateTitle" runat="server"/></div>
<div class="wbf-metadata-description">
<asp:Label ID="ReferenceDateDescription" runat="server"/>
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">
    <SharePoint:DateTimeControl ID="ReferenceDate" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="ReferenceDateMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>

<% if (showSeriesTag)
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title"><asp:Label ID="SeriesTagTitle" runat="server"/></div>
<div class="wbf-metadata-description">
<asp:Label ID="SeriesTagDescription" runat="server"/>
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">
<asp:DropDownList ID="SeriesTagDropDownList" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="SeriesTagFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
 <% } %>

 
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Owning Team</div>
<div class="wbf-metadata-description">
The team who will own this work box.
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">

<div class="wbf-metadata-read-only-value">
<asp:Label ID="OwningTeamField" ControlMode="display" runat="server" />
</div>

<asp:HiddenField ID="OwningTeamUIControlValue" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="OwningTeamFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Involved Teams</div>
<div class="wbf-metadata-description">
The teams who can collaborate in this work box.
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="InvolvedTeamsField" ControlMode="display" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="InvolvedTeamsFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>



<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="CreateNewButton" runat="server" Text="Create New"  OnClick="createNewButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>

</td>
</tr>

</table>

    <asp:HiddenField ID="WorkBoxCollectionUrl" runat="server" />
    <asp:HiddenField ID="RecordsTypeGUID" runat="server" />

    <asp:HiddenField ID="RelatedWorkBoxUrl" runat="server" />
    <asp:HiddenField ID="RelationType" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
New Work Box: Required Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
New Work Box: Required Metadata
</asp:Content>
