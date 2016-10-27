<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocDialogPickLocation.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocDialogPickLocation" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <Sharepoint:ScriptLink ID="SP2010ModalDialogs" 
        Name="sp.ui.dialog.js" 
        Localizable="false" 
        runat="server"
     />


</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
</div>


<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
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

<table class="wbf-dialog-form">

<asp:Literal ID="DocumentsBeingPublished" runat="server" />

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Select Folder</div>
<div class="wbf-field-description">Pick the folder in the work box into which you would like to publish the document.</div>
</td>
<td class="wbf-field-value-panel">

<div id="" style="overflow:scroll; height:400px; width: 400px; border: 1px solid #ccc; ">

  <SharePoint:SPTreeView
        id="LibraryLocations"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="1"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="LibraryLocations_SelectedNodeChanged"
        OnTreeNodeDataBound="LibraryLocations_Bound"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/FOLDER.GIF"
        SkipLinkText=""
        NodeIndent="20"
        ExpandImageUrl="/_layouts/images/tvplus.gif"
        CollapseImageUrl="/_layouts/images/tvminus.gif"
        NoExpandImageUrl="/_layouts/images/tvblank.gif" />

</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Selected Folder Path</div>
<div class="wbf-field-description"></div>
</td>
<td class="wbf-field-value-panel">

<asp:UpdatePanel ID="ShowSelectionPanel" runat="server">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="LibraryLocations" EventName="SelectedNodeChanged" />
    </Triggers>
    <ContentTemplate>

<div>

Selected folder: <asp:Label ID="SelectedFolderPath" runat="server" />

</div>

<div style="display: none; ">
    <asp:Label ID="SelectedRecordID" runat="server" />
</div>

<asp:HiddenField ID="PublishingProcessJSON" runat="server" />

    </ContentTemplate>
</asp:UpdatePanel>

</td>
</tr>


<tr>
<td colspan="2" class="wbf-buttons-panel">
<p>
        <asp:Button ID="Select" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Select" OnClick="selectButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

</div>

<script type="text/javascript">
    // Hopefully this will resize the modal correctly. 
    // Found with thanks in answers here: https://social.msdn.microsoft.com/Forums/sharepoint/en-US/ddd6ce37-b289-47d5-92ad-067b2c9ee4fd/resizing-an-open-dialog-as-its-contents-change
    var currentModal = SP.UI.ModalDialog.get_childDialog();
    currentModal.$$d_autoSize();
</script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Pick Location In Records Library
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Pick Location In Records Library
</asp:Content>
