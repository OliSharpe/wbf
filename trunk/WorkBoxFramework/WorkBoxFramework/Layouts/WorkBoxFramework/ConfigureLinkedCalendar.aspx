<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigureLinkedCalendar.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ConfigureLinkedCalendar" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<h2>Configure Linked Calendar</h2>

<asp:Hidden ID="CalendarListGUID" runat="server"></asp:Hidden>
<asp:Label ID="ErrorMessage" runat="server"></asp:Label>


<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Calendar URL</b>
</td>
<td class="ms-authoringcontrols" valign="top">

<b><asp:Label ID="CalendarURL" runat="server" Text=""></asp:Label></b>

</td>
</tr>

<tr>
<td valign="top">
<b>Linked Work Box Collection</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="LinkedCalendarWorkBoxCollection" runat="server" Text=""></asp:TextBox>

</td>
</tr>

<tr>
<td valign="top">
<b>Default Work Box Template</b>
<p>The title of the work box template to use for new events created in this calendar</p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="LinkedCalendarDefaultWorkBoxTemplate" runat="server" Text=""></asp:TextBox>

</td>
</tr>

<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="SaveButton" runat="server" Text="Update Link"  OnClick="saveButton_OnClick" />
&nbsp;
    <asp:Button ID="RemoveButton" runat="server" Text="Remove Link"  OnClick="removeButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Configure Linked Calendar
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Configure Linked Calendar
</asp:Content>
