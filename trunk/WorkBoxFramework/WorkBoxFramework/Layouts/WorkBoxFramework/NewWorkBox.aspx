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

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFjQueryScriptRegistration"
        name="WorkBoxFramework/jquery-1.7.2.min.js"
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

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Functional Area<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<% if (functionalAreaFieldIsEditable)
   { %>
<div class="wbf-field-value">
<Taxonomy:TaxonomyWebTaggingControl ID="FunctionalAreaField" ControlMode="display" runat="server" />
</div>
<% }
   else
   { %>
<div class="wbf-field-read-only-value">
<asp:Label ID="ReadOnlyFunctionalAreaField" runat="server" />
</div>
<% } %>

<div class="wbf-field-error">
<asp:Label ID="FunctionalAreaFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Records Type<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="RecordsType" runat="server" />
</div>

<div class="wbf-field-error">
<asp:Label ID="RecordsTypeFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Work Box Naming Convention</div>
</td>
<td class="wbf-field-value-panel">

    <div class="wbf-field-read-only-value">
    <asp:Label ID="WorkBoxNamingConvention" runat="server"></asp:Label>
    </div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Work Box Template<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<% if (onlyOneWorkBoxTemplate)
   { %>
<div class="wbf-field-read-only-value">
    <asp:Label ID="WorkBoxTemplate" runat="server"></asp:Label>
    </div>
    <div class="wbf-field-error">
<asp:Label ID="NoTemplatesError" runat="server" Text="" ForeColor="Red"/>
</div>
    <asp:HiddenField ID="WorkBoxTemplateID" runat="server"></asp:HiddenField>

<% }
   else
   { %>
    <div class="wbf-field-value">
    <asp:DropDownList ID="WorkBoxTemplates" runat="server"></asp:DropDownList>
    </div>
<% } %>

</td>
</tr>

<% if (showShortTitle)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="ShortTitleTitle" runat="server"/></div>
<div class="wbf-field-description">
<asp:Label ID="ShortTitleDescription" runat="server"/>
</div>
</td>
<td class="wbf-field-value-panel" valign="top">
    <asp:TextBox ID="WorkBoxShortTitle" runat="server" />
<div class="wbf-field-error">
<asp:Label ID="WorkBoxShortTitleMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>




<% if (showReferenceID)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="ReferenceIDTitle" runat="server"/></div>
<div class="wbf-field-description">
<asp:Label ID="ReferenceIDDescription" runat="server"/>
</div>
</td>
<td class="wbf-field-value-panel" valign="top">
    <div class="wbf-field-value">
    <asp:TextBox ID="ReferenceID" runat="server" />
    </div>
<div class="wbf-field-error">
<asp:Label ID="ReferenceIDMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>

<% if (showReferenceDate)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="ReferenceDateTitle" runat="server"/></div>
<div class="wbf-field-description">
<asp:Label ID="ReferenceDateDescription" runat="server"/>
</div>
</td>
<td class="wbf-field-value-panel" valign="top">
    <div class="wbf-field-value">
    <SharePoint:DateTimeControl ID="ReferenceDate" runat="server" />
    </div>
<div class="wbf-field-error">
<asp:Label ID="ReferenceDateMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>

<% if (showSeriesTag)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="SeriesTagTitle" runat="server"/></div>
<div class="wbf-field-description">
<asp:Label ID="SeriesTagDescription" runat="server"/>
</div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value">
<asp:DropDownList ID="SeriesTagDropDownList" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="SeriesTagFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
 <% } %>

 
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Owning Team</div>
<div class="wbf-field-description">
The team who will own this work box.
</div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-read-only-value">
<asp:Label ID="OwningTeamField" ControlMode="display" runat="server" />
</div>

<asp:HiddenField ID="OwningTeamUIControlValue" runat="server" />
<div class="wbf-field-error">
<asp:Label ID="OwningTeamFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Involved Teams</div>
<div class="wbf-field-description">
The teams who can collaborate in this work box.
</div>
</td>
<td class="wbf-field-value-panel" valign="top">
    <div class="wbf-field-value">
<Taxonomy:TaxonomyWebTaggingControl ID="InvolvedTeamsField" ControlMode="display" runat="server" />
</div>
<div class="wbf-field-error">
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

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
New Work Box: Required Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
New Work Box: Required Metadata
</asp:Content>
