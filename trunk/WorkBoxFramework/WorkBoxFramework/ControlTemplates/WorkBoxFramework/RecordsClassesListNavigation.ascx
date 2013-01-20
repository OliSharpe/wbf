<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="System.Collections.Generic" %> 
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Import Namespace="Microsoft.SharePoint.Taxonomy" %> 
<%@ Import Namespace="WorkBoxFramework" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecordsClassesListNavigation.ascx.cs" Inherits="WorkBoxFramework.ControlTemplates.WorkBoxFramework.TeamSiteNavigation" %>


<% if (RecordsClasses != null)
   { 
%>

<h3 class="wbf-leftpanel-header <%=AdditionalCSSStyle %>"><%= RecordsGroup %></h3>

<div class="wbf-records-classes-list <%=AdditionalCSSStyle %>">

<% foreach (Hashtable recordsClassDetails in RecordsClasses) { %>

<div class="wbf-records-class">
<div class="wbf-records-class-header <%=recordsClassDetails[SELECTED_CLASS_CSS_STYLE] %>">
<span class="wbf-records-class <%=recordsClassDetails[SELECTED_CLASS_CSS_STYLE] %>">
<a href="<%=recordsClassDetails[LINK_URL] %>" class="wbf-link" onclick="<%=recordsClassDetails[ON_CLICK_COMMAND] %>"><%=recordsClassDetails[LINK_TEXT] %></a>
</span>
</div>

<div id="<%=recordsClassDetails[UNIQUE_TOGGLE_ID] %>" class="wbf-records-types <%=recordsClassDetails[SELECTED_CLASS_CSS_STYLE] %>">

<%      
    List<Hashtable> recordsTypes = recordsClassDetails[RECORDS_TYPES] as List<Hashtable>;
    if (recordsTypes == null) recordsTypes = new List<Hashtable>();
    foreach (Hashtable recordsTypeDetails in recordsTypes)
    { 
%>

<div class="wbf-records-type <%=recordsTypeDetails[SELECTED_TYPE_CSS_STYLE] %>  <%=recordsTypeDetails[ADDITIONAL_TYPE_CSS_STYLE] %> ">
<span class="wbf-records-type">
<a href="<%=recordsTypeDetails[LINK_URL] %>" class="wbf-link <%=recordsTypeDetails[SELECTED_TYPE_CSS_STYLE] %>" onclick="<%=recordsTypeDetails[ON_CLICK_COMMAND] %>"><%=recordsTypeDetails[LINK_TEXT]%></a>
</span>
</div>
<%    
    }
%>

</div>
</div>
<% 
    }     
%>
</div>


<%
    }
   else
   {
%>
<%=NotSetupText %>           
<%       
   }
%>