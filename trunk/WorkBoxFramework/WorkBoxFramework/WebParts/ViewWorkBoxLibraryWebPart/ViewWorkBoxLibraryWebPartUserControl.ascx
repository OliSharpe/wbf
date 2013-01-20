<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewWorkBoxLibraryWebPartUserControl.ascx.cs" Inherits="WorkBoxFramework.ViewWorkBoxLibraryWebPart.ViewWorkBoxLibraryWebPartUserControl" %>

<div class="wbf-view-work-box-library">

   		<table width="100%" cellspacing="10" cellpadding="0" class="wbf-view-records-library">
   			<tr>
   				<td valign="top" width="225px">
   					<h3>Select a Records Type:</h3>

  <SharePoint:SPTreeView
        id="PickRecordsTypeTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="1"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="PickRecordsTypeTreeView_SelectedNodeChanged"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

   				</td>
   				<td valign="top">
                       <asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Always">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="PickRecordsTypeTreeView" EventName="SelectedNodeChanged" />
                            </Triggers>
                            <ContentTemplate>

                    <div class="wbf-view-selected-records-type">

                    <h3><asp:Label ID="SelectedRecordsType" runat="server" /></h3>

                    <p>
                    <asp:Label ID="SelectedRecordsTypeDescription" runat="server" Text="Please select a records type from the tree to the left." />
                    </p>

<% if (showStatusFilter)
   { %>
<div class="wbf-work-box-status-filters">
<asp:LinkButton ID="FilterOpenStatus" runat="server" Text="Open" OnClick="FilterOpenStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterClosedStatus" runat="server" Text="Closed" OnClick="FilterClosedStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterAllStatus" runat="server" Text="All" OnClick="FilterAllStatus_OnClick"/>
</div>
<% } %>

<div class="wbf-show-results-grid">
                                <SharePoint:SPGridView runat="server" ID="ShowResults" AutoGenerateColumns="false">
                                  <EmptyDataTemplate>
                                    <i>No results</i>                                    
                                  </EmptyDataTemplate>
                                </SharePoint:SPGridView>        
</div>

</div>

                            </ContentTemplate>

                       </asp:UpdatePanel>
   				</td>
   			</tr>
   		</table>

</div>