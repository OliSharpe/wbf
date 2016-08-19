<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MigrationAdmin.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.MigrationAdmin" DynamicMasterPageFile="~masterurl/default.master" %>

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
</style>

<h2>Administration for Migration Timer Job:</h2>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Migration Type</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:DropDownList ID="MigrationType" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Migration Source System</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="MigrationSourceSystem" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Migration Control List</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<span style="width: 50px; ">List URL:</span>
<asp:TextBox ID="MigrationControlListUrl" runat="server" />
</div>

<div class="wbf-metadata-read-only-value">
<span style="width: 50px; ">List View:</span>
<asp:TextBox ID="MigrationControlListView" runat="server" />  
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Migration Mapping List</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<span style="width: 50px; ">List URL:</span>
<asp:TextBox ID="MigrationMappingListUrl" runat="server" />
</div>

<div class="wbf-metadata-read-only-value">
<span style="width: 50px; ">List View:</span>
<asp:TextBox ID="MigrationMappingListView" runat="server" />  
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Migration Additional Subjects List</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<span style="width: 50px; ">List URL:</span>
<asp:TextBox ID="MigrationSubjectsListUrl" runat="server" />
</div>

<div class="wbf-metadata-read-only-value">
<span style="width: 50px; ">List View:</span>
<asp:TextBox ID="MigrationSubjectsListView" runat="server" />  
</div>

</td>
</tr>



<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Items Per Migration Cycle</div>
The timer job that runs the migration will try to migrate this many items each time that it runs.
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="ItemsPerCycle" runat="server" />
</div>

<div class="wbf-metadata-error">
<asp:Label ID="ItemsPerCycleError" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>

<tr>
<td colspan="2" class="wbf-metadata-value-panel">
&nbsp;
</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">User Name</div>
The user name for remote access to authenticated content.
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="UserName" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Password</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="UserPassword" TextMode="password" runat="server" />
</div>

</td>
</tr>

<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="UpdateButton" runat="server" Text="Update"  OnClick="UpdateButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_OnClick"/>

</td>
</tr>


</table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Migration Admin
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Migration Admin
</asp:Content>
