<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoveTeamOrIndividual.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.RemoveTeamOrIndividual" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


<h2><asp:Label ID="DialogTitle" runat="server" /></h2>

<p>
<asp:Label ID="AreYouSureText" runat="server" />
</p>

<p style="padding-left: 20px;">
<asp:Label ID="NameOfTeamOrIndividual" runat="server" />
</p>

<asp:HiddenField ID="TeamOrIndividual" runat="server" />
<asp:HiddenField ID="GUIDOfTeamToRemove" runat="server" />
<asp:HiddenField ID="LoginNameOfUserToRemove" runat="server" />
<asp:HiddenField ID="InvolvedOrVisiting" runat="server" />

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td align="center" valign="top">
    <asp:Button ID="RemoveButton" runat="server" Text="Remove"  OnClick="removeButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Remove Team or Individual
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Remove Team or Individual
</asp:Content>
