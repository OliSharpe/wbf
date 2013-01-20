<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeamSiteTreeSelectionUserControl.ascx.cs" Inherits="WorkBoxFramework.TeamSiteTreeSelection.TeamSiteTreeSelectionUserControl" %>

<script type="text/javascript">

    var wbf__id_of_hidden_submit_link = "<%= HiddenSubmitLink.ClientID %>";
    var wbf__id_of_hidden_records_type_guid_field = "<%= HiddenRecordsTypeGUIDField.ClientID %>";

</script>

<div style="display: none;" class="wbf-hidden-submit-button">

<asp:LinkButton ID="HiddenSubmitLink" Text="Reload" OnClick="HiddenSubmitLink_OnClick" runat="server" />
<asp:HiddenField ID="HiddenRecordsTypeGUIDField" Value="" runat="server" />

</div>

<div class="wbf-tree-selection-update-panel">
<asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Always">

    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="HiddenSubmitLink" EventName="Click" />
    </Triggers>

    <ContentTemplate>
<div class="wbf-tree-selection-updating-panel">

<div class="wbf-tree-selection-title">
<asp:Label ID="SelectionTitle" Text="Select Work Box Type" runat="server" />
</div>

<div class="wbf-tree-selection-description">
    <asp:Label ID="SelectionDescription" Text="Select a category from left hand tree navigation to list work boxes of that type." runat="server" />
</div>

<div class="wbf-tree-selection-filters">
<asp:LinkButton ID="FilterOpenStatus" runat="server" Text="Open" OnClick="FilterOpenStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterClosedStatus" runat="server" Text="Closed" OnClick="FilterClosedStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterAllStatus" runat="server" Text="All" OnClick="FilterAllStatus_OnClick"/>
</div>

<div class="wbf-tree-selection-gridview">
    <SharePoint:SPGridView runat="server" ID="SelectedWorkBoxes" AutoGenerateColumns="false">
        <EmptyDataTemplate>
            <span class="wbf-tree-selection-no-work-boxes"><%= NoWorkBoxesText %></span>                                    
        </EmptyDataTemplate>
    </SharePoint:SPGridView>        

</div>

<div class="wbf-create-new-work-box-link">

<asp:Literal ID="CreateNewWorkBoxLink" runat="server"></asp:Literal>

</div>

</div>

    </ContentTemplate>
</asp:UpdatePanel>
</div>