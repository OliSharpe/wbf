<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocDialogRequiredMetadataPage.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocDialogRequiredMetadataPage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">


<script type="text/javascript">
    function WorkBoxFramework_PublishDoc_pickedANewRecordsType(dialogResult, returnValue) {

        if (dialogResult == SP.UI.DialogResult.OK) {


            var newRecordsType = document.getElementById("<%=NewRecordsTypeUIControlValue.ClientID %>");
            newRecordsType.value = returnValue;

            document.forms['aspnetForm'].submit();
        }

    }
</script>

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
div.wbf-publish-out-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom:4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
</style>

<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle">
<asp:Image ID="SourceDocIcon" runat="server" />
</td>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-publish-out-title">
Publish Document to: <asp:Label ID="DestinationTitle" runat="server" />
</div>
<div>
You must enter the following metadata for the document
</div>
</td>
</tr>
</table>


<asp:HiddenField ID="RecordsTypeUIControlValue" runat="server"/>
<asp:HiddenField ID="NewRecordsTypeUIControlValue" runat="server" Value="" />

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
<table width="100%" cellpadding="0" cellspacing="0">
<tr>
<td align="left" class="wbf-records-type">
<div class="wbf-metadata-read-only-value">
<asp:Label ID="RecordsType" runat="server" />
</div>
</td>
<td align="right" class="wbf-records-type">
<asp:Button ID="PickRecordsTypeButton" UseSubmitBehavior="false" runat="server" Text="Change" />
</td>
</tr>
</table>

<div class="wbf-metadata-error">
<asp:Label ID="RecordsTypeFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>


<% if (generatingFilename)
   { %>
<tr>
    <td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Published File Name</div>
        <div class="wbf-metadata-description">Generated with file naming convention</div>
    </td>
    <td class="wbf-metadata-value-panel">
        <div class="wbf-metadata-read-only-value">
            <asp:Label ID="ReadOnlyNameField" runat="server"></asp:Label>
        </div>
        <div class="wbf-metadata-description">
            <asp:Label ID="DocumentFileNamingConvention" runat="server"></asp:Label>
        </div>
    </td>
</tr>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Original File Name</div>
</td>
<td class="wbf-metadata-value-panel">
<div class="wbf-metadata-read-only-value">    <asp:Label ID="OriginalFileName" runat="server"></asp:Label></div>
</td>
</tr>
<% }
   else
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">File Name</div>
</td>
<td class="wbf-metadata-value-panel">
    <asp:TextBox ID="NameField" runat="server"></asp:TextBox>
</td>
</tr>
<% } %>



<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Short Title</div>
<div class="wbf-metadata-description">Give a short, meaningful title.</div>
</td>
<td class="wbf-metadata-value-panel">

    <asp:TextBox ID="TitleField" runat="server"></asp:TextBox>
    <div class="wbf-metadata-error">
    <asp:RequiredFieldValidator
        ID="RequiredFieldValidator2" runat="server" 
        ErrorMessage="You must enter a value for the Title"
        ControlToValidate="TitleField">        
    </asp:RequiredFieldValidator>
    </div>
</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Protective Zone</div>
<div class="wbf-metadata-description">What is the most permissive zone that this document could be held in?</div>
</td>
<td class="wbf-metadata-value-panel">

    <asp:DropDownList ID="ProtectiveZone" runat="server"></asp:DropDownList>
<div class="wbf-metadata-error">
<asp:Label ID="ProtectiveZoneMessage" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>


<% if (showSubjectTags)
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title"><asp:Label ID="SubjectTagsTitle" runat="server"/></div>
<div class="wbf-metadata-description">
<asp:Label ID="SubjectTagsDescription" runat="server"/>
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">
<Taxonomy:TaxonomyWebTaggingControl ID="SubjectTagsField" ControlMode="display" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="SubjectTagsError" runat="server" Text="" ForeColor="Red"/>
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

 <% if (showScanDate)
   { %>
<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title"><asp:Label ID="ScanDateTitle" runat="server"/></div>
<div class="wbf-metadata-description">
<asp:Label ID="ScanDateDescription" runat="server"/>
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">
    <SharePoint:DateTimeControl ID="ScanDate" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="ScanDateMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>
<% } %>


<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Owning Team</div>
<div class="wbf-metadata-description">
A record of the team who owned this document when it was created.
</div>
</td>
<td class="wbf-metadata-value-panel" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="OwningTeamField" ControlMode="display" runat="server" />
<div class="wbf-metadata-error">
<asp:Label ID="OwningTeamFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>



<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Involved Teams</div>
<div class="wbf-metadata-description">
A record of the teams that were involved with the creation of this document.
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
<p>
        <asp:Button ID="Publish" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Publish" OnClick="publishButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

    <asp:HiddenField ID="ListGUID" runat="server" />
    <asp:HiddenField ID="ItemID" runat="server" />
    <asp:HiddenField ID="TheDestinationType" runat="server" />
    <asp:HiddenField ID="DestinationURL" runat="server" />


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Publish Document: Required Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Publish Document
</asp:Content>
