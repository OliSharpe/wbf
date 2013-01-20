<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoveWorkBoxFromFavourites.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.RemoveWorkBoxFromFavourites" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<p>
    <asp:Label ID="Message" runat="server" Text="Label"></asp:Label>
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="removeFromFavouritesButton" runat="server" Text="Remove From Favourites"  OnClick="removeFromFavouritesButton_OnClick"/>
    &nbsp;
    <asp:Button ID="cancel" runat="server" Text="Cancel"  OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>

    <asp:HiddenField ID="WorkBoxTitle" runat="server" />
    <asp:HiddenField ID="WorkBoxGuid" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Remove Work Box From Favourites
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Remove Work Box From Favourites
</asp:Content>
