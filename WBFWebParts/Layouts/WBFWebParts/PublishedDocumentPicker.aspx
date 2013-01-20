<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishedDocumentPicker.aspx.cs" Inherits="WBFWebParts.Layouts.WBFWebParts.PublishedDocumentPicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

   		<table width="100%" cellspacing="10" cellpadding="0" class="wbf-view-records-library">
        <tr>
        <td colspan="2">

        <h2>Select a published document</h2>
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

                <div>
    <asp:Button ID="PickDocument" runat="server" Text="Save"  OnClick="PickDocument_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</div>

<div>&nbsp;</div>

                
                       <asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Always">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="BrowsableTreeView" EventName="SelectedNodeChanged" />
                            </Triggers>
                            <ContentTemplate>

<div>
<span style=" font-weight: bold; font-size: 10pt" >
Selected document: <asp:Label ID="SelectedDocumentName" runat="server" />
</span>
<asp:HiddenField ID="SelectedDocumentDetails" runat="server" />
</div>

<div>&nbsp;</div>



                    <div>
                    <h3><asp:Label ID="SelectedRecordsType" runat="server" /></h3>

                    <p>
                    <asp:Label ID="SelectedRecordsTypeDescription" runat="server" Text="Please select a records type from the tree to the left." />
                    </p>

                                <SharePoint:SPGridView runat="server" ID="ShowResults" AutoGenerateColumns="false"  OnRowCommand="ShowResults_RowCommand">
                                  <EmptyDataTemplate>
                                    <i>No results</i>                                    
                                  </EmptyDataTemplate>
                                </SharePoint:SPGridView>        
                    </div>






                            </ContentTemplate>

                       </asp:UpdatePanel>
   				</td>
   			</tr>
   		</table>



<asp:HiddenField ID="CallingRowIndex" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Published Document Picker
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Published Document Picker
</asp:Content>
