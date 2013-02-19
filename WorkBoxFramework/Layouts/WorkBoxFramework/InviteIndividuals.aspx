﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteIndividuals.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.InviteIndividuals" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<p>
Select the individuals you want to be involved with this work box.
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Work Box Title</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:Label ID="WorkBoxTitle" runat="server" Text=""></asp:Label>

</td>
</tr>


<tr>
<td valign="top">
<b>Involved Individuals</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

			<SharePoint:PeopleEditor id="InvolvedIndividualsControl" runat="server"
				SelectionSet="User"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "true"
				/>


</td>
</tr>

<tr>
<td valign="top">
<b>Visiting Individuals</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

			<SharePoint:PeopleEditor id="VisitingIndividualsControl" runat="server"
				SelectionSet="User"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "true"
				/>


</td>
</tr>



<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Invite Individual
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Invite Individual
</asp:Content>