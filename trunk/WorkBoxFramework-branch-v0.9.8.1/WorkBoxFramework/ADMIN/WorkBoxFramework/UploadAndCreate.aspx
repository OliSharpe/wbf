<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadAndCreate.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.UploadAndCreate" DynamicMasterPageFile="~masterurl/default.master" %>

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
Select the details of the upload and create process:
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Work Box Collection URL</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="WorkBoxCollectionURL" runat="server" Text=""></asp:TextBox>

</td>
</tr>

<tr>
<td valign="top">
<b>Upload Control File on Server</b>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="ControlFile" runat="server" Text=""></asp:TextBox>

</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="UploadAndCreateButton" runat="server" Text="Upload and Create"  OnClick="uploadAndCreateButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>

<asp:Label ID="ProgressReport" runat="server" Text=""></asp:Label>

<hr />

<h2>Migrate izzi pages:</h2>
<div>
Migration List: <asp:TextBox ID="WebPageMigrationList" runat="server" />
</div>
<div>
User name: <asp:TextBox ID="UserName" runat="server" />
</div>
<div>
Password: <asp:TextBox ID="UserPassword" TextMode="password" runat="server" />
</div>
<div>
<asp:Button ID="MigratePages" runat="server" Text="Migrate Pages" OnClick="MigratePages_OnClick"/>
</div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Upload Documents and Create Work Boxes
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Upload Documents and Create Work Boxes
</asp:Content>
