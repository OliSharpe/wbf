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

    function WorkBoxFramework_archiveSelectedRecords() {

        var selectedRecords = $('#wbf-list-of-records-selected').text();

        if (selectedRecords && selectedRecords != "") {
            var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/ArchiveSelectedRecords.aspx'
            + '?SelectedRecords=' + selectedRecords;

            var options = {
                url: urlValue,
                title: 'Archive Record(s)',
                allowMaximize: false,
                showClose: true,
                dialogReturnValueCallback: WorkBoxFramework_callback
            };

            SP.UI.ModalDialog.showModalDialog(options);
        } else {
            alert("You have not selected any records");
        }
    }



    function WBF_add_record_id(recordID) {
        var soFarString = $('#wbf-list-of-records-selected').text();

        var soFarArray = [];
        if (soFarString && soFarString != "") soFarArray = soFarString.split('_');

        soFarArray.push(recordID);
        $('#wbf-list-of-records-selected').text(soFarArray.join('_'));


        //var archiveRecordsButton = document.getElementById("<%=ArchiveRecordsButton.ClientID %>"); 
    }

    function WBF_remove_record_id(recordID) {
        var soFarString = $('#wbf-list-of-records-selected').text();

        var soFarArray = [];
        if (soFarString && soFarString != "") soFarArray = soFarString.split('_');

        // OK so this is crude - but it should work everywhere!
        for(var i = soFarArray.length - 1; i >= 0; i--) {
          if(soFarArray[i] == recordID) {
            soFarArray.splice(i, 1);
          }
        }
        $('#wbf-list-of-records-selected').text(soFarArray.join('_'));
    }


    function WBF_checkbox_changed(event) {
        event = event || window.event;
        var target = event.target || event.srcElement;

        var checkbox = $(target);
        var recordID = checkbox.data('record-id');

        if (checkbox.prop('checked')) {
            WBF_add_record_id(recordID);
        } else {
            WBF_remove_record_id(recordID);
        }
    }

    function WBF_add_checkbox_change_function() {
        
        $('.wbf-our-records-check-boxes').change(WBF_checkbox_changed());
        alert('attempted to connect');
    }

    //Sys.Application.add_load(WBF_add_checkbox_change_function); 
    //

    </script>
<div style="display:none;">
<div id="wbf-list-of-records-selected"></div>
</div>

<table borders="1" cellpadding="20" cellspacing="0">

<tr>
<td colspan="2">
<div style="display:none;">
<span>
Search: 
<asp:TextBox ID="SearchBox" runat="server" Enabled="false"/>
<asp:Button ID="DoSearch" Text="Go" OnClick="DoSearch_Click" Width="70px" runat="server" Enabled="false"/>
</span>
</div>

</td>
</tr>


<tr>
<td>
</td>
<td>
<asp:Button ID="ArchiveRecordsButton" Text="Archive Records" runat="server" OnClientClick="WorkBoxFramework_archiveSelectedRecords(); return false;" UseSubmitBehavior="false" />

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