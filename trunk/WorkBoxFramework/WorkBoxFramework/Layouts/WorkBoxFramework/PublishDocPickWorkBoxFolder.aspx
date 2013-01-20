<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocPickWorkBoxFolder.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocPickWorkBoxFolder" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
<style type="text/css">
 
td.wbf-records-type { border: 0px; }
td.wbf-metadata-title-panel { width: 300px; padding: 8px; border-top:solid 1px grey; vertical-align: top; }
td.wbf-metadata-value-panel { width: 405px; padding: 8px; border-top:solid 1px grey; vertical-align: top; background-color: #f1f1f2;  }
td.wbf-buttons-panel { border-top:solid 1px grey; text-align: center; vertical-align: top; }
.wbf-metadata-title { font-weight: bold; padding-bottom: 2px; }
.wbf-metadata-description { font-weight: normal; padding: 2px; }
.wbf-metadata-read-only-value { font-weight: bold; padding: 2px; }
.wbf-metadata-error { font-weight: normal; padding: 0px; color: Red; }
div.wbf-publish-out-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom:4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
</style>

<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle">
<asp:Image ID="SourceDocIcon" runat="server" />
</td>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-publish-out-title">
Publish Document to: <asp:Label ID="DestinationTitle" runat="server" />
</div>
<div>
Select the folder into which to publish the document
</div>
</td>
</tr>
</table>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Select Folder</div>
<div class="wbf-metadata-description">Pick the folder in the work box into which you would like to publish the document.</div>
</td>
<td class="wbf-metadata-value-panel">

  <SharePoint:SPTreeView
        id="WorkBoxFolders"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="1"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="WorkBoxFolders_SelectedNodeChanged"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/FOLDER.GIF"
        SkipLinkText=""
        NodeIndent="20"
        ExpandImageUrl="/_layouts/images/tvplus.gif"
        CollapseImageUrl="/_layouts/images/tvminus.gif"
        NoExpandImageUrl="/_layouts/images/tvblank.gif" />

</td>
</tr>

<tr>
<td class="wbf-metadata-title-panel">
        <div class="wbf-metadata-title">Selected Folder Path</div>
<div class="wbf-metadata-description"></div>
</td>
<td class="wbf-metadata-value-panel">


<asp:UpdatePanel ID="ShowSelectionPanel" runat="server">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="WorkBoxFolders" EventName="SelectedNodeChanged" />
    </Triggers>
    <ContentTemplate>

<div>

Selected folder: <asp:Label ID="SelectedFolderPath" runat="server" />

</div>

    </ContentTemplate>
</asp:UpdatePanel>


</td>
</tr>


<tr>
<td colspan="2" class="wbf-buttons-panel">
<p>
        <asp:Button ID="Next" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Publish" OnClick="nextButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

    <asp:HiddenField ID="ListGUID" runat="server" />
    <asp:HiddenField ID="ItemID" runat="server" />
    <asp:HiddenField ID="TheDestinationType" runat="server" />
    <asp:HiddenField ID="DestinationURL" runat="server" />


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Publish To Work Box - Pick Folder
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Publish To Work Box - Pick Folder
</asp:Content>
