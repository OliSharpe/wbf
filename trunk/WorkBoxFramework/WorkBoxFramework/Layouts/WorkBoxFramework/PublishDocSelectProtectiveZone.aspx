<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocSelectProtectiveZone.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocSelectProtectiveZone" DynamicMasterPageFile="~masterurl/default.master" %>

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
<style type="text/css">
td.wbf-records-type { border: 0px; }
td.wbf-metadata-title-panel { width: 300px; padding: 8px; border-top:solid 1px grey; vertical-align: top; }
td.wbf-metadata-value-panel { width: 405px; padding: 8px; border-top:solid 1px grey; vertical-align: middle; background-color: #f1f1f2;  }
td.wbf-buttons-panel { border-top:solid 1px grey; text-align: center; vertical-align: top; }
.wbf-metadata-title { font-weight: bold; padding-bottom: 2px; }
.wbf-metadata-description { font-weight: normal; padding: 2px; }
.wbf-metadata-read-only-value { font-weight: bold; padding: 2px; }
.wbf-metadata-error { font-weight: normal; padding: 0px; color: Red; }
div.wbf-publish-out-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom:4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
</style>

<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>

<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-publish-out-title">
Select Publish Out Destination for Document(s)
</div>
<div style="text-align: center; padding-left: 20px; ">
<asp:Literal ID="DocumentsBeingPublished" runat="server" />
</div>
</td>
</tr>
</table>


<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-title-panel">
<b>Either</b> publish to the <b>Public Web Site</b>
<p>These documents will be available for use on public facing web sites and will appear in the public library on the main council web site.</p>
</td>
<td class="wbf-metadata-value-panel" valign="middle" align="center">
 
<% if (userCanPublishToPublic)
   { %>
    <asp:Button ID="PublicWebSiteButton" runat="server" Text="Public Website" OnClick="PublicWebSiteButton_onClick" />
    <div class="wbf-metadata-error">
    <asp:Label ID="PublicNotAllowedMessage" runat="server" />
    </div>
    <% }
   else
   { %>

   <span style=" color: Red; font-style: italic; ">You do not have permission to publish to the public zone. Please contact your team owner if you think you should have this right.</span> 

    <% } %>
</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
<b>Or</b> publish to the <b>Public Extranet</b>
<p>These documents will be available for use on public facing extranet sites.</p>
</td>
<td class="wbf-metadata-value-panel" valign="middle" align="center">
 
<% if (userCanPublishToPublic)
   { %>
    <asp:Button ID="PublicExtranetButton" runat="server" Text="Public Extranet" OnClick="PublicExtranetButton_onClick" />
    <div class="wbf-metadata-error">
    <asp:Label ID="PublicExtranetNotAllowedMessage" runat="server" />
    </div>
    <% }
   else
   { %>

   <span style=" color: Red; font-style: italic; ">As above you need permission to publish to the public zone in order to publish to the public extranet zone. Please contact your team owner if you think you should have this right.</span> 

    <% } %>
</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<b>Or</b> publish just to <b>Records Library</b>
<p>These documents will not be available for use on public facing web sites.</p>
</td>
<td class="wbf-metadata-value-panel" valign="middle" align="center">
    <asp:Button ID="RecordsLibraryButton" runat="server" Text="Records Library" OnClick="RecordsLibraryButton_onClick" />
</td>
</tr>

<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>


    <asp:HiddenField ID="PublishingProcessJSON" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Publish Document: Select Protective Zone
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Publish Document
</asp:Content>
