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

<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<asp:Panel ID="AccessDeniedPanel" runat="server" Visible="false">
<h2>Access Denied</h2>
<p>
You are not a member of the records management group therefore you do not have permission to perform this action.
</p>
</asp:Panel>

<asp:Panel ID="UpdateRecordsMetadataPanel" runat="server">

<p>
As a records manager you have the right to modify the following metadata fields.
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Record's Filename</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b>
<asp:Label ID="Filename" runat="server" Text=""></asp:Label>
</b>

</td>
</tr>

<tr>
<td valign="top">
<b>Record's Title</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b>
<asp:Label ID="Title" runat="server" Text=""></asp:Label>
</b>

</td>
</tr>

<tr>
<td valign="top">
<b>Functional Area</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b>
<asp:Label ID="FunctionalArea" runat="server" Text=""></asp:Label>
</b>

</td>
</tr>

<tr>
<td valign="top">
<b>Records Type</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b>
<asp:Label ID="RecordsType" runat="server" Text=""></asp:Label>
</b>

</td>
</tr>


<tr>
<td valign="top">
<b>Unique Record ID</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b>
<asp:Label ID="RecordID" runat="server" Text=""></asp:Label>
</b>

</td>
</tr>

<tr>
<td valign="top">
<b>Update Live / Archived:</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:DropDownList ID="LiveOrArchived" runat="server" />

</td>
</tr>


<tr>
<td valign="top">
<b>Update Protective Zone:</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:DropDownList ID="ProtectiveZone" runat="server" />

</td>
</tr>


<tr>
<td valign="top">
<b>Update Subject Tags:</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="SubjectTags" ControlMode="display" runat="server" />
<br />
<asp:Label ID="SubjectTagsErrorMessage" runat="server" Text="" ForeColor="Red"/>

</td>
</tr>



<tr>
<td valign="top">
<b>Reason for change</b>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="ReasonForChange" TextMode="MultiLine" Rows="4" Columns="50" runat="server" />
<div>
<asp:RequiredFieldValidator ControlToValidate="ReasonForChange" ErrorMessage="You must provide a reason for making this change." runat="server"/>
</div>
</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
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

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Update Record's Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Update Record's Metadata
</asp:Content>
