<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRecordsLibraryWebPartUserControl.ascx.cs" Inherits="WorkBoxFramework.ViewRecordsLibraryWebPart.ViewRecordsLibraryWebPartUserControl" %>

<asp:UpdatePanel ID="RefreshWebPart" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="FilterByProtectiveZone" EventName="SelectedIndexChanged" />
    </Triggers>
    <ContentTemplate>


   		<table width="100%" cellspacing="10" cellpadding="0" class="wbf-view-records-library">
        <tr>
        <td colspan="2">

        <!--
        Filter by Protective Zone:
        <asp:DropDownList ID="FilterByProtectiveZone" runat="server" OnSelectedIndexChanged="FilterByProtectiveZone_OnSelectedIndexChanged"  AutoPostBack="true"/>
        -->

        </td>
        
        </tr>

   			<tr>
   				<td valign="top" width="225px">
   					<h3><asp:Label ID="SelectedViewTitle" runat="server"/></h3>

  <SharePoint:SPTreeView
        id="BrowsableTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="0"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="BrowsableTreeView_SelectedNodeChanged"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"
        AutoPostBack="True"
        PopulateOnDemand="true"
        OnTreeNodePopulate="BrowsableTreeView_PopulateNode"/>

   				</td>
   				<td valign="top">
                    <asp:UpdateProgress ID="ShowSelectionProgress" runat="server" DisplayAfter="1000">
                  <ProgressTemplate>
                  
                  <div style="padding-top: 5px; padding-bottom: 10px;">
                  
                  Getting new results ......
                  
                  </div>

                  </ProgressTemplate>
                  </asp:UpdateProgress>

                       <asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="BrowsableTreeView" EventName="SelectedNodeChanged" />
                            </Triggers>

                            <ContentTemplate>

                    <div id="ShowSelectionUpdatePanelDiv">
                    <h3><asp:Label ID="SelectedRecordsType" runat="server" /></h3>

                    <p>
                    <asp:Label ID="SelectedRecordsTypeDescription" runat="server" Text="Please select a records type from the tree to the left." />
                    </p>

<% if (showFilters)
   { %>
<div class="wbf-documents-selection-filters">
<asp:LinkButton ID="FilterLiveStatus" runat="server" Text="Live" OnClick="FilterLiveStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterArchivedStatus" runat="server" Text="Archived" OnClick="FilterArchivedStatus_OnClick"/>&nbsp;|&nbsp;<asp:LinkButton ID="FilterAllStatus" runat="server" Text="All" OnClick="FilterAllStatus_OnClick"/>
</div>
<% } %>


                                <SharePoint:SPGridView runat="server" ID="ShowResults" AutoGenerateColumns="false">
                                  <EmptyDataTemplate>
                                    <i>(No documents of this type)</i>                                    
                                  </EmptyDataTemplate>
                                </SharePoint:SPGridView>        
                    </div>






                            </ContentTemplate>

                       </asp:UpdatePanel>
   				</td>
   			</tr>
   		</table>


    </ContentTemplate>
</asp:UpdatePanel>


