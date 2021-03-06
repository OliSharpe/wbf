﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoveTeamOrIndividual.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.RemoveTeamOrIndividual" DynamicMasterPageFile="~masterurl/default.master" %>

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

<div class="wbf-dialog">

<h2 class="wbf-dialog-title"><asp:Label ID="DialogTitle" runat="server" /></h2>

<div class="wbf-dialog-message">
<asp:Label ID="AreYouSureText" runat="server" />
</div>

<p style="padding-left: 20px;">
<asp:Label ID="NameOfTeamOrIndividual" runat="server" />
</p>

<asp:HiddenField ID="TeamOrIndividual" runat="server" />
<asp:HiddenField ID="GUIDOfTeamToRemove" runat="server" />
<asp:HiddenField ID="LoginNameOfUserToRemove" runat="server" />
<asp:HiddenField ID="InvolvedOrVisiting" runat="server" />

<table class="wbf-dialog-just-buttons">

<tr>
<td class="wbf-buttons-panel">
    <asp:Button ID="RemoveButton" runat="server" Text="Remove"  OnClick="removeButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>
</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Remove Team or Individual
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Remove Team or Individual
</asp:Content>
