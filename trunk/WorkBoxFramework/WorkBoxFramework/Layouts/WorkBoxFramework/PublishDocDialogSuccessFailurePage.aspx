<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocDialogSuccessFailurePage.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocDialogSuccessFailurePage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

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
<asp:Label ID="SuccessFailureTitle" runat="server" />
</div>
<div>
<asp:Label ID="SuccessFailureMessage" runat="server" />
</div>
</td>
</tr>
</table>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
<%= title %>
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
<%= title %>
</asp:Content>
