<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewRecordSeriesDetails.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ViewRecordSeriesDetails" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
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

table.wbf-record-series-details .wbf-record-series-summary-issue-odd
{
    background: #EEE;
    font-weight: bold;
    text-align:center;
    border-top: 2px solid #ccc;
}

table.wbf-record-series-details .wbf-record-series-summary-issue-even
{
    background: #DDD;
    font-weight: bold;
    text-align:center;
    border-top: 2px solid #ccc;
}

table.wbf-record-series-details .wbf-record-series-summary-detail
{
    background: #FFF;
    text-align:center;
    border-top: 2px solid #ccc;
}

table.wbf-record-series-details .wbf-record-series-detail-left
{
    text-align:left;
}

table.wbf-record-series-details .wbf-record-series-detail-right
{
    text-align:right;
}


table.wbf-record-series-details .wbf-record-series-details-panel-cell
{
    padding: 0px;
}

table.wbf-record-series-details .wbf-old-metadata
{
    color: #DDD;
}

table.wbf-record-series-details-panel
{
    border: 0px;
    border-spacing: 0px;
    padding: 0px;
    margin: 0px;
}

table.wbf-record-series-details-panel .wbf-record-series-detail-even
{
    background: #FFF;
    padding: 5px;
}

table.wbf-record-series-details-panel .wbf-record-series-detail-odd
{
    background: #EEE;
    padding: 5px;
}

table.wbf-record-series-details-panel .wbf-record-series-detail-title
{
    font-weight: bold;
    width: 170px;
}

</style>

<script>

    function WBF_toggleByID(toggleID) {

        var currently = $("#wbf-more-or-less-" + toggleID).text();

        if (currently.indexOf("more") > -1) {
            $(".wbf-record-details").hide();
            $(".wbf-more-or-less").html("<nobr>more &gt;</nobr>");
            $("#wbf-more-or-less-" + toggleID).html("<nobr>less &lt;</nobr>");
            $("#wbf-record-details-" + toggleID).show();
        } else {
            $("#wbf-more-or-less-" + toggleID).html("<nobr>more &gt;</nobr>");
            $("#wbf-record-details-" + toggleID).hide();
        }

    }

    function WBF_toggleChecklist(toggleID) {
        $("#wbf-checklist-" + toggleID).toggle();
    }

    function WBF_edit_records_metadata(recordID) {
        var editCommandString = "EditRecordsMetadata.aspx?RecordID=" + recordID;
        WorkBoxFramework_relativeCommandAction(editCommandString, 800, 600);
    }

</script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">



<asp:Literal ID="ViewRecordSeriesTable" runat="server" />


<div style="text-align: center; ">
        <asp:Button ID="Close" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Close" OnClick="closeButton_OnClick" />
</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
View Record Series Details
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
View Record Series Details
</asp:Content>
