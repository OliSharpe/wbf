<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewRecordsLibrary.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ViewRecordsLibrary" DynamicMasterPageFile="~masterurl/default.master" %>

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

<style type="text/css">
BODY #s4-leftpanel { width: 0 !important; }
.s4-ca { margin-left: 0 !important; }
</style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderLeftNavBar" style="display:none" runat="server">
</asp:Content>


<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

   		<table width="100%" cellspacing="10" cellpadding="0" class="wbf-view-records-library">
   			<tr>	
   				<td colspan="2">
			   		<h1>Our Published Documents</h1>
   				</td>
   			</tr>
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

                    <div>
                    <h3><asp:Label ID="SelectedRecordsType" runat="server" /></h3>

                    <p>
                    <asp:Label ID="SelectedRecordsTypeDescription" runat="server" Text="Please select a records type from the tree to the left." />
                    </p>

                                <SharePoint:SPGridView runat="server" ID="ShowResults" AutoGenerateColumns="false">
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



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
View Records Library
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
View Records Library
</asp:Content>
