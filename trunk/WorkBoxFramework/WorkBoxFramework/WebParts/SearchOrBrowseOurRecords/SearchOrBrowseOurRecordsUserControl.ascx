<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchOrBrowseOurRecordsUserControl.ascx.cs" Inherits="WorkBoxFramework.SearchOrBrowseOurRecords.SearchOrBrowseOurRecordsUserControl" %>

<style>

table.wbf-record-series-details 
{
    border: 0px;
    border-spacing: 0px;
    padding: 5px;
}

table.wbf-record-series-details th
{
    font-weight: bold;
    padding: 5px;
}

table.wbf-record-series-details td
{
    padding: 5px;
}

table.wbf-record-series-details .wbf-record-series-odd
{
    background: #EEE;
    text-align:center;
}

table.wbf-record-series-details .wbf-record-series-even
{
    background: #CCC;
    text-align:center;
}

table.wbf-record-series-details .wbf-record-series-summary-issue
{
    background: #EEE;
    font-weight: bold;
    text-align:center;
    border-top: 1px solid #ccc;
}

table.wbf-record-series-details .wbf-record-series-summary-detail
{
    background: #FFF;
    text-align:left;
    border-top: 1px solid #ccc;
}

.wbf-centre 
{
    text-align:center;
}

</style>

<script>
    function WorkBoxFramework_viewRecordSeriesDetails(seriesID, recordID) {

        var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/ViewRecordSeriesDetails.aspx?'
            + '&RecordSeriesID=' + seriesID
            + "&RecordID=" + recordID;

        var options = {
            url: urlValue,
            title: 'View Record Series Details',
            allowMaximize: false,
            showClose: true,
            width: 600,
            height: 700,
            dialogReturnValueCallback: WorkBoxFramework_callback
        };

        SP.UI.ModalDialog.showModalDialog(options);
    }
    </script>


<table borders="1" cellpadding="20" cellspacing="0">

<tr>
<td colspan="2">
<div>
<span>
Search: 
<asp:TextBox ID="SearchBox" runat="server" Enabled="false"/>
<asp:Button ID="DoSearch" Text="Go" OnClick="DoSearch_Click" Width="70px" runat="server" Enabled="false"/>
</span>
</div>

</td>
</tr>


<tr>
<td valign="top">
<!-- Tree panel -->

  <SharePoint:SPTreeView
        id="RecordsLibraryFolders"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        ExpandDepth="1"
        SelectedNodeStyle-CssClass="ms-tvselected"
        OnSelectedNodeChanged="RecordsLibraryFolders_SelectedNodeChanged"
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


<td valign="top">
<!-- View panel -->


<asp:UpdatePanel ID="ShowSelectionPanel" runat="server">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="RecordsLibraryFolders" EventName="SelectedNodeChanged" />
    </Triggers>
    <ContentTemplate>

<div>

<asp:Literal ID="FoundRecords" runat="server" />

</div>

    </ContentTemplate>
</asp:UpdatePanel>



</td>


</tr>

</table>