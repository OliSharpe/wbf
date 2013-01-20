<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewClipboard.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ViewClipboard" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<style type="text/css">

.wbf-clipboard { padding: 10px; }

.wbf-clipboard-item { padding-top: 8px; margin-left: 20px; }

</style>

<h2>Your Work Box Clipboard</h2>

<asp:Literal ID="JustPastedText" runat="server" />
<asp:HiddenField ID="NeedsRefreshOnReturn" runat="server" />

<asp:Literal ID="ItemsOnClipboard" runat="server" />

<div>

    <asp:Button ID="clearButton" runat="server" Text="Clear All"  OnClick="clearAllButton_OnClick"/>
&nbsp;
    <asp:Button ID="closeButton" runat="server" Text="Close"  OnClick="closeButton_OnClick"/>

</div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
View Clipboard
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
View Clipboard
</asp:Content>
