﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAllInvolved.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ViewAllInvolved" DynamicMasterPageFile="~masterurl/default.master" %>

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

<asp:Panel ID="JavascriptPanel" runat="server">
<script type="text/javascript">
    function removeTeam(involvedOrVisiting, teamGUID) {
        WorkBoxFramework_commandAction("<%= SPContext.Current.Web.Url %>/_layouts/WorkBoxFramework/RemoveTeamOrIndividual.aspx?TeamOrIndividual=Team&InvolvedOrVisiting=" + involvedOrVisiting + "&GUIDOfTeamToRemove=" + teamGUID, 0, 0);
    }

    function removeIndividual(involvedOrVisiting, loginName) {
        WorkBoxFramework_commandAction("<%= SPContext.Current.Web.Url %>/_layouts/WorkBoxFramework/RemoveTeamOrIndividual.aspx?TeamOrIndividual=Individual&InvolvedOrVisiting=" + involvedOrVisiting + "&LoginNameOfUserToRemove=" + loginName, 0, 0);
    }
</script>
</asp:Panel>

    <asp:Literal ID="GeneratedViewOfAllInvolved" runat="server"></asp:Literal>

    <div class="wbf-buttons-panel">
        <asp:Button ID="RefreshTeams" runat="server" Text="Refresh Team Membership"  OnClick="refreshTeams_OnClick"/>
    &nbsp;
        <asp:Button ID="CloseDialog" runat="server" Text="Close"  OnClick="close_OnClick"/>
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
View All Involved
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
View All Involved
</asp:Content>
