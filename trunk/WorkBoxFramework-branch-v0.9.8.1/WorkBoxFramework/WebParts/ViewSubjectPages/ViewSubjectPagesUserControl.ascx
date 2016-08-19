<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="wbf" TagName="WBFUser" src="/_controltemplates/WorkBoxFramework/WBFUser.ascx" %> 
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSubjectPagesUserControl.ascx.cs" Inherits="WorkBoxFramework.ViewSubjectPages.ViewSubjectPagesUserControl" %>

<SharePoint:CssRegistration ID="wbfCssSectionTags" Name="WorkBoxFramework/css/SubjectTags.css" After="corev4.css" runat="server" />
<SharePoint:ScriptLink ID="WBFScriptRegistration" name="/_layouts/WorkBoxFramework/SubjectTags.js" language="javascript" localizable="false" runat="server" OnDemand="false" />

<style type="text/css">

</style>

<div class="wbf-view-subject-pages">

<h3 class="wbf-view-subject-pages-title">
<asp:Literal ID="PageName" runat="server" />
<%if (canEditOrCreate){ %>
<span class="wbf-view-subject-pages-controls">
    <asp:Button Text="New" ID="btnNewSubjectTag" runat="server" CssClass="wbf-btn-new-tag" ToolTip="Add New Subject Tag" OnClientClick="Tags.ShowDialog(this);return false;" />
    <asp:Button Text="Edit" ID="btnEditSubjectTag" runat="server" CssClass="wbf-btn-edit-tag" ToolTip="Edit Subject Tag" OnClientClick="Tags.ShowDialog(this, 2);return false;" />
</span>
<%} %>
</h3>

<div class="wbf-a-to-z-letters">
<% if (showAtoZ) {
    for (char l = 'A'; l <= 'Z'; l++)
    {%>
    <a href="?Letter=<%=l%>"><%=l%></a>
    <%}
} %>
</div>

<asp:UpdatePanel runat="server" ID="udpPageContent" UpdateMode="Always">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnUpdTrick" />
    </Triggers>
    <ContentTemplate>

<div class="wbf-subjects-page-content">
<asp:Label ID="PageSubjectTagDescription" runat="server" />
</div>

<div class="wbf-subjects-contacts">
    <div class="wbf-subjects-internal-contact">
        <asp:Panel runat="server" CssClass="wbf-tbl" ID="panInternalContact" Visible="false">
            <h3 class="wbf-subject-tag-section-head">Internal Contact</h3>
            <wbf:WBFUser runat="server" id="wbfInternalContact" />
            <asp:Literal Text="" ID="litInternalContact" runat="server" /> 
        </asp:Panel>
    </div>
    <div class="wbf-subjects-external-contact">
        <asp:Panel runat="server" CssClass="wbf-tbl" ID="panExternalContact" Visible="false">
            <h3 class="wbf-subject-tag-section-head">Additional Information</h3>
            <asp:Literal runat="server" ID="litExternalContact"></asp:Literal>
        </asp:Panel>
    </div>
</div>

<div class="wbf-list-of-subjects">
<asp:Literal ID="TableOfChildSubjects" runat="server" />
</div>

        

    </ContentTemplate>
</asp:UpdatePanel>
<asp:Button style="display:none;" id="btnUpdTrick" runat="server" />
<script type="text/javascript">Tags.setPB('<%= btnUpdTrick.ClientID %>');</script>
<asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Always">
    <Triggers>
    </Triggers>
    <ContentTemplate>

<div class="wbf-show-documents-for-subject">
<%-- Header is made visible only when required --%>
<h3 class="wbf-subject-tag-section-head" id="h3RelatedDocs" runat="server" style="display:none;">Related Documents</h3>

<% if (showFilters)
   { %>
<div class="wbf-documents-selection-filters">
<asp:LinkButton ID="FilterLiveStatus" runat="server" Text="Live" OnClick="FilterLiveStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterArchivedStatus" runat="server" Text="Archived" OnClick="FilterArchivedStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterAllStatus" runat="server" Text="All" OnClick="FilterAllStatus_OnClick"/>
</div>
<% } %>

    <SharePoint:SPGridView runat="server" ID="DocumentsForSubject" AutoGenerateColumns="false">
        <EmptyDataTemplate>
        </EmptyDataTemplate>
    </SharePoint:SPGridView>        

    <asp:Label ID="DynamicNoDocumentsMessage" runat="server"/>
</div>

    </ContentTemplate>
</asp:UpdatePanel>

</div>