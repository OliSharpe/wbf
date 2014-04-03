<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WBFUser.ascx.cs" Inherits="WorkBoxFramework.ControlTemplates.WorkBoxFramework.WBFUser" %>

<style type="text/css">
    .wbf-user, .wbf-user * {
        box-sizing: border-box;   
    }
    .wbf-user {
        display: table;
        padding: 5px;
        border: 1px solid #dcdcdc;
    }
    .wbf-user-info, .wbf-user-photo  {
        display: table-cell;
        vertical-align:top;
    }
    .wbf-user-photo {
        width: 110px;
    }
    .wbf-user-dept, .wbf-user-name, .wbf-user-phone, .wbf-user-email {
        white-space:nowrap;
        width: 100%;
        padding-bottom: 3px;
    }
</style>

<%--Could have potentially looked at the People Results template for this (def in 2013), but for simplicity...--%>
<div class="wbf-user">
    <div class="wbf-user-photo">
        <asp:Image ImageUrl="" ID="imgUserPhoto" runat="server" style="width:100px; height: 100px; font-size: 0.7em;" AlternateText="Profile Picture" />
    </div>
    <div class="wbf-user-info">
        <div class="wbf-user-name">
            <asp:Label Text="" ID="lblName" runat="server" />
        </div>
        <div class="wbf-user-dept">
            <asp:Label Text="" ID="lblDept" runat="server" />
        </div>
        <div class="wbf-user-phone">
            <asp:Label Text="" ID="lblPhone" runat="server" />
        </div>
        <div class="wbf-user-email">
            <asp:HyperLink ID="hlEmail" NavigateUrl="" runat="server" />
        </div>
    </div>
</div>