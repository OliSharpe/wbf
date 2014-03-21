<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewClipboard.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ViewClipboard" DynamicMasterPageFile="~masterurl/default.master" %>

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

.wbf-clipboard h2 { padding: 0px; margin: 0px }

.wbf-clipboard td { padding-left: 10px; }

.wbf-clipboard-items { padding: 10px; }

.wbf-clipboard-item { padding-top: 8px; margin-left: 20px; }

.wbf-clipboard-from-work-box { padding-top: 15px; }

.wbf-clipboard-header { padding-top: 10px; padding-left: 10px; }

</style>

<div class="wbf-clipboard">

<div class="wbf-clipboard-header">
<table cellpadding="0" cellspacing="0" border="0">
<tr>
<td rowspan="2"><img src="/_layouts/images/pastehh.png" /></td>
<td><h2>Your Work Box Clipboard</h2></td>
</tr>
<tr>
<td><asp:Label ID="CutOrCopiedText" runat="server" /></td>
</tr>
</table>
</div>

<asp:Literal ID="JustPastedText" runat="server" />
<asp:HiddenField ID="NeedsRefreshOnReturn" runat="server" />

<asp:Literal ID="ItemsOnClipboard" runat="server" />

<div>

    <asp:Button ID="clearButton" runat="server" Text="Clear All"  OnClick="clearAllButton_OnClick"/>
&nbsp;
    <asp:Button ID="closeButton" runat="server" Text="Close"  OnClick="closeButton_OnClick"/>

</div>

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
View Clipboard
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
View Clipboard
</asp:Content>
