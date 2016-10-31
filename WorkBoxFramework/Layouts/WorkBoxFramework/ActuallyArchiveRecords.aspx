<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActuallyArchiveRecords.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ActuallyArchiveRecords" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

   <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFjQueryScriptRegistration"
        name="WorkBoxFramework/jquery-1.11.3.min.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

    <SharePoint:ScriptLink ID="WBFScriptRegistration"
        name="WorkBoxFramework/WorkBoxFramework.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

<script type="text/javascript">

    var wbf__id_of_hidden_submit_link = "<%= HiddenSubmitLink.ClientID %>";
    var wbf__id_of_done_button = "<%= DoneButton.ClientID %>";

    function WorkBoxFramework_triggerArchiveNextDocument() {
        // Then trigger the AJAX call for archiving the next document:
        var link = document.getElementById(wbf__id_of_hidden_submit_link);
        link.click();
    }

    function WorkBoxFramework_finishedProcessing(buttonText) {
        //alert("In finished processing function: " + buttonText);
        $('#' + wbf__id_of_done_button).val(buttonText);
    }

</script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<style type="text/css">
td.wbf-records-type { border: 0px; }
td.wbf-metadata-title-panel { width: 300px; padding: 8px; border-top:solid 1px grey; vertical-align: top; }
td.wbf-metadata-value-panel { width: 405px; padding: 8px; border-top:solid 1px grey; vertical-align: top; background-color: #f1f1f2;  }
td.wbf-buttons-panel { border-top:solid 1px grey; text-align: center; vertical-align: top; }
.wbf-metadata-title { font-weight: bold; padding-bottom: 2px; }
.wbf-metadata-description { font-weight: normal; padding: 2px; }
.wbf-metadata-read-only-value { font-weight: bold; padding: 2px; }
.wbf-metadata-error { font-weight: normal; padding: 0px; color: Red; }
td.wbf-create-new-title { padding: 6px; }
div.wbf-create-new-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom: 4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
.wbf-admin-page { padding: 10px; }


td.wbf-task-image-table-cell 
{
    width: 32px;
    height: 32px;
    vertical-align: top;
}

td.wbf-task-name-table-cell 
{
    min-width: 300px !important;    
    font-weight: bold;  
}

td.wbf-task-status-table-cell 
{
    width: 150px !important;  
    font-weight: bold;  
}

td.wbf-task-feedback-table-cell 
{
    font-weight: normal;  
    font-size: 80%;
}


</style>

<div class="wbf-admin-page">

<h2>Archiving the Records</h2>

<p>
The records(s) selected will now be archived</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-value-panel" colspan="2">

<asp:LinkButton ID="HiddenSubmitLink" Text="Archive Next" OnClick="ArchiveNextDocument" runat="server" style="display:none;" />
<asp:HiddenField ID="AllRecordIDsToArchive" runat="server" />
<asp:HiddenField ID="AllRecordFilenamesToArchive" runat="server" />

<asp:UpdatePanel ID="ArchivingRecordsPanel" runat="server" UpdateMode="Always">

    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="HiddenSubmitLink" EventName="Click" />
    </Triggers>

    <ContentTemplate>

<div style="min-width: 800px !important; min-height: 250px;">
<asp:PlaceHolder ID="RecordArchivingProgress" runat="server" />

</div>

<asp:Label ID="NextRecordToArchive" runat="server" style="display: none; "/>

</ContentTemplate>
</asp:UpdatePanel>

</td>
</tr>

<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="DoneButton" runat="server" Text="Stop" CausesValidation="False" OnClick="DoneButton_OnClick"/>
</td>
</tr>

</table>


</div>

<script type="text/javascript">
    WorkBoxFramework_triggerArchiveNextDocument();
</script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Archiving Records
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Archiving Records
</asp:Content>
