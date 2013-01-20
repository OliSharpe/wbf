<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishedDocumentPicker.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishedDocumentPicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

<script type="text/javascript">
    function WorkBoxFramework_pickPublishedDocument(documentURL, documentTitle) {
        window.frameElement.commonModalDialogClose(SP.UI.DialogResult.OK, documentURL + ";" + documentTitle);
    }
</script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<style type="text/css">
td.wbf-records-type { border: 0px; }
td.wbf-metadata-title-panel { width: 250px; padding: 8px; border-top:solid 1px grey; vertical-align: top; }
td.wbf-metadata-value-panel { width: 405px; padding: 8px; border-top:solid 1px grey; vertical-align: top; background-color: #f1f1f2;  }
td.wbf-buttons-panel { border-top:solid 1px grey; text-align: center; vertical-align: top; }
.wbf-metadata-title { font-weight: bold; padding-bottom: 2px; }
.wbf-metadata-description { font-weight: normal; padding: 2px; }
.wbf-metadata-read-only-value { font-weight: bold; padding: 2px; }
.wbf-metadata-error { font-weight: normal; padding: 0px; color: Red; }
td.wbf-create-new-title { padding: 6px; }
div.wbf-create-new-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom: 4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
</style>


   		<table width="100%" cellspacing="2" cellpadding="0" class="wbf-view-records-library">
   			<tr>	
   				<td colspan="2">
			   		<h1>Pick A Published Document</h1>
   				</td>
   			</tr>
   			<tr>	
   				<td class="wbf-metadata-title-panel">
			   		<h3>Your recently published documents:</h3>
   				</td>
                <td class="wbf-metadata-value-panel">
                <div>
                <img src="/_layouts/images/icdocx.png" alt="Document"/> <a href="#" onclick="javascript: WorkBoxFramework_pickPublishedDocument('/fake1.docx', 'Fake 1 doc');" >Fake example document 1</a>
                </div>
                <div>
                <img src="/_layouts/images/icdocx.png" alt="Document"/> <a href="#" onclick="javascript: WorkBoxFramework_pickPublishedDocument('/fake2.docx', 'Fake 2 doc');" >Fake example document 2</a>
                </div>
                <div>
                <img src="/_layouts/images/icdocx.png" alt="Document"/> <a href="#" onclick="javascript: WorkBoxFramework_pickPublishedDocument('/fake3.docx', 'Fake 3 doc');" >Fake example document 3</a>
                </div>
                <div>
                <img src="/_layouts/images/icdocx.png" alt="Document"/> <a href="#" onclick="javascript: WorkBoxFramework_pickPublishedDocument('/fake4.docx', 'Fake 4 doc');" >Fake example document 4</a>
                </div>
                <div>
                <img src="/_layouts/images/icdocx.png" alt="Document"/> <a href="#" onclick="javascript: WorkBoxFramework_pickPublishedDocument('/fake5.docx', 'Fake 5 doc');" >Fake example document 5</a>
                </div>
                </td>
   			</tr>
   			<tr>
   				<td valign="top" width="225px" class="wbf-metadata-title-panel">
   					<h3>Browse by Records Type:</h3>

                    <SharePoint:SPRememberScroll
      id="MyTreeViewRememberScroll"
      runat="server" onscroll="javascript:_spRecordScrollPositions(this);"
      Style="overflow: auto;height: 400px;width: 250px; ">
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
</SharePoint:SPRememberScroll>

   				</td>
   				<td valign="top" class="wbf-metadata-value-panel">
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
Published Document Picker
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Published Document Picker
</asp:Content>
