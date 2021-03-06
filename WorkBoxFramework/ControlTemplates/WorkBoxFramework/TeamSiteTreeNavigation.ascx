﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeamSiteTreeNavigation.ascx.cs" Inherits="WorkBoxFramework.ControlTemplates.WorkBoxFramework.TeamSiteTreeNavigation" %>

<script type="text/javascript">
    var wbf__spweb_url = '<%=SPContext.Current.Web.Url %>';

    $(document).ready(function () {
        $("#<%=RecordsTypeTreeView.ClientID  %> a.ms-navitem").click(function () {
            $("a.ms-navitem").css("font-weight", "normal");
            $(this).css("font-weight", "bold");
            return true;
        });
    });

</script>

<div class="wbf-records-type-tree-nav">

<SharePoint:SPTreeView
        id="RecordsTypeTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

<% if (NotSetupText != null & NotSetupText != "") { %>
<p><%=NotSetupText %></p>           
<% } %>

        </div>