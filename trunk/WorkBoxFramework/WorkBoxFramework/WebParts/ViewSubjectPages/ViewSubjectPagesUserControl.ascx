<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSubjectPagesUserControl.ascx.cs" Inherits="WorkBoxFramework.ViewSubjectPages.ViewSubjectPagesUserControl" %>

<div class="wbf-view-subject-pages">

<h3 class="wbf-view-subject-pages-title">
<asp:Literal ID="PageName" runat="server" />
</h3>

<div class="wbf-a-to-z-letters">
<% if (showAtoZ) { %>
<a href="?Letter=A">A</a>
&nbsp;
<a href="?Letter=B">B</a>
&nbsp;
<a href="?Letter=C">C</a>
&nbsp;
<a href="?Letter=D">D</a>
&nbsp;
<a href="?Letter=E">E</a>
&nbsp;
<a href="?Letter=F">F</a>
&nbsp;
<a href="?Letter=G">G</a>
&nbsp;
<a href="?Letter=H">H</a>
&nbsp;
<a href="?Letter=I">I</a>
&nbsp;
<a href="?Letter=J">J</a>
&nbsp;
<a href="?Letter=K">K</a>
&nbsp;
<a href="?Letter=L">L</a>
&nbsp;
<a href="?Letter=M">M</a>
&nbsp;
<a href="?Letter=N">N</a>
&nbsp;
<a href="?Letter=O">O</a>
&nbsp;
<a href="?Letter=P">P</a>
&nbsp;
<a href="?Letter=Q">Q</a>
&nbsp;
<a href="?Letter=R">R</a>
&nbsp;
<a href="?Letter=S">S</a>
&nbsp;
<a href="?Letter=T">T</a>
&nbsp;
<a href="?Letter=U">U</a>
&nbsp;
<a href="?Letter=V">V</a>
&nbsp;
<a href="?Letter=W">W</a>
&nbsp;
<a href="?Letter=X">X</a>
&nbsp;
<a href="?Letter=Y">Y</a>
&nbsp;
<a href="?Letter=Z">Z</a>
<% } %>
</div>

<div>
<asp:Label ID="PageSubjectTagDescription" runat="server" />
</div>

<div class="wbf-list-of-subjects">
<asp:Literal ID="TableOfChildSubjects" runat="server" />
</div>

<asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Always">
    <Triggers>
    </Triggers>
    <ContentTemplate>

<div class="wbf-show-documents-for-subject">

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