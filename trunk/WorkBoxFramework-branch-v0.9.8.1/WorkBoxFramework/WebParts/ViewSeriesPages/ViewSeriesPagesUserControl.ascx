<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSeriesPagesUserControl.ascx.cs" Inherits="WorkBoxFramework.ViewSeriesPages.ViewSeriesPagesUserControl" %>

<style type="text/css">
table.seriesTags { border-collapse: separate; border-spacing: 10px; }
td.seriesTags a { color: Blue; font-weight: bold; font-size: 12pt; padding: 10px;  }
td.seriesTags a.link { color: Blue; font-weight: bold; font-size: 12pt; padding: 10px;  }
tr.seriesTags { color: Blue; font-weight: bold; font-size: 12pt; padding: 10px;  }

</style>

<h1 class="article-title">
<asp:Label ID="PageSeriesTagName" runat="server" />
</h1>

<div>
<asp:Label ID="PageSeriesTagDescription" runat="server" />
</div>

<div>
<asp:Literal ID="TableOfChildTerms" runat="server" />
</div>