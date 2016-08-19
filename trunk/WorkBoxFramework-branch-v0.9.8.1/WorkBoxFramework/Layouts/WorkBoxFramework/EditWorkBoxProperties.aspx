<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditWorkBoxProperties.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.EditWorkBoxProperties" DynamicMasterPageFile="~masterurl/default.master" %>

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
<div class="wbf-dialog-error">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
</div>

<h2 class="wbf-dialog-title">Edit Work Box Properties</h2>

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box Title</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="WorkBoxTitle" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Owning Team</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="OwningTeam" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Functional Area</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="FunctionalArea" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Records Type</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="RecordsType" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box Template</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="WorkBoxTemplate" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box Status</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="WorkBoxStatus" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box URL</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-value">
<asp:Label ID="WorkBoxURL" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box Short Title<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="WorkBoxShortTitle" runat="server" Text=""></asp:TextBox>
</div>
<div class="wbf-field-error">
    <asp:RequiredFieldValidator ID="WorkBoxShortTitleValidator" runat="server" ErrorMessage="You must enter a short title."
            ControlToValidate="WorkBoxShortTitle"></asp:RequiredFieldValidator>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Title In Work Box<span class="wbf-required-asterisk">*</span></div>
<div class="wbf-field-description">
This title will only be visible in the work box itself.
</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="WorkBoxPrettyTitle" runat="server" Text="" Columns="50"></asp:TextBox>
</div>

<div class="wbf-field-error">
    <asp:RequiredFieldValidator ID="WorkBoxPrettyTitleValidator" runat="server" ErrorMessage="You must enter a title for inside the work box."
            ControlToValidate = "WorkBoxPrettyTitle"></asp:RequiredFieldValidator>
</div>


</td>
</tr>

<% if (showReferenceID)
   { %>
<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Reference ID</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="ReferenceID" runat="server" Text=""></asp:TextBox>
</div>

</td>
</tr>

<% } %>

<% if (showReferenceDate)
   { %>
<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Reference Date</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<SharePoint:DateTimeControl ID="ReferenceDate" runat="server" />
</div>

</td>
</tr>

<% } %>


<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>
</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Edit Work Box Properties
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Edit Work Box Properties
</asp:Content>
