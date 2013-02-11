<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultInviteEmailTemplates.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.DefaultInviteEmailTemplates" DynamicMasterPageFile="~masterurl/default.master" %>

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
td.wbf-create-new-title { padding: 6px; }
div.wbf-create-new-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom: 4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
.wbf-admin-page { padding: 10px }
</style>

<div class="wbf-admin-page">

<h2>Configure the default invitation email templates</h2>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Invite Involved Email Subject</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="InvolvedSubject" Columns="50" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Invite Involved Email Body</div>
<div>
<p></p>
<p>Template tags:<br />
[WORK_BOX_TITLE]<br />
[WORK_BOX_URL]<br />
[USER_NAME]
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="InvolvedBody" TextMode="MultiLine" Rows="8" Columns="50" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Invite Visiting Email Subject</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="VisitingSubject" Columns="50" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Invite Visiting Email Body</div>
<div>
<p></p>
<p>Template tags:<br />
[WORK_BOX_TITLE]<br />
[WORK_BOX_URL]<br />
[USER_NAME]
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="VisitingBody" TextMode="MultiLine" Rows="8" Columns="50" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Invite To Team Email Subject</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="ToTeamSubject" Columns="50" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Invite To Team Email Body</div>
<div>
<p></p>
<p>Template tags:<br />
[TEAM_NAME]<br />
[TEAM_SITE_URL]<br />
[ROLE_WITHIN_TEAM]<br />
[USER_NAME]
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="ToTeamBody" TextMode="MultiLine" Rows="8" Columns="50" runat="server" />
</div>

</td>
</tr>




<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="SaveButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_OnClick"/>

</td>
</tr>


</table>

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Default Invite Email Templates
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Default Invite Email Templates
</asp:Content>
