<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditLinkedCalendarSettings.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.EditLinkedCalendarSettings" DynamicMasterPageFile="~masterurl/default.master" %>

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
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<h2>Edit Linked Calendar Settings</h2>
<p>
The following settings will link events in this calendar with work boxes.
</p>

<asp:HiddenField ID="ListGUID" runat="server" />

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Work Box Collection URL</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b><asp:TextBox ID="WorkBoxCollectionURL" runat="server" Text=""></asp:TextBox></b>

</td>
</tr>

<tr>
<td valign="top">
<b>Default Template Title</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="DefaultTemplateTitle" runat="server" Text=""></asp:TextBox>

</td>
</tr>

<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>


<asp:Label ID="EventReceivers" runat="server" Text=""></asp:Label>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Edit Linked Calendar Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Edit Linked Calendar Settings
</asp:Content>
