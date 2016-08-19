<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigureWorkBoxCollection.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ConfigureWorkBoxCollection" DynamicMasterPageFile="~masterurl/default.master" %>

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

    function WorkBoxFramework_triggerNextConfigurationStep() {
        var link = document.getElementById(wbf__id_of_hidden_submit_link);
        link.click();
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
.wbf-admin-page { padding: 10px }


td.wbf-configuation-step-image-table-cell 
{
    width: 32px;
    height: 32px;
    vertical-align: top;
}

td.wbf-configuation-step-name-table-cell 
{
    width: 150px !important;    
    font-weight: bold;  
}

td.wbf-configuation-step-status-table-cell 
{
    width: 150px !important;  
    font-weight: bold;  
}

td.wbf-configuation-step-feedback-table-cell 
{
    font-weight: normal;  
}


</style>

<div class="wbf-admin-page">

<h2>Configure Work Box Collection</h2>

<p>
This admin page will help do the initial configuration of a work box collection.</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Name of 'Work Boxes In Collection' list</div>
<div>
<p>
This is the list that holds all of the metadata for each of the work boxes in this list. If you pick an existing list then the required WBF columns will be added to this list.</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="WorkBoxesInCollectionListName" Columns="50" runat="server" />
<asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="WorkBoxesInCollectionListName" ErrorMessage="You must provide a name for this list" runat="server"/>

</div>

</td>
</tr>

<!--
<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Default Records Type</div>
<div>
<p>
(NOT IMPLEMENTED!!)
To do the initial setup please give the first, or default, records type for work boxes in this collection.
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="DefaultRecordsType" Columns="50" runat="server" />

</div>

</td>
</tr>
-->


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Configuration Steps</div>
<div>
<p>
These are the key configuration steps that will be checked.
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<asp:LinkButton ID="HiddenSubmitLink" Text="Reload" OnClick="DoNextConfigStep" runat="server" style="display:none;" />

<asp:UpdatePanel ID="ConfigurationStepsPanel" runat="server" UpdateMode="Always">

    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="DoInitialConfigStep" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="HiddenSubmitLink" EventName="Click" />
    </Triggers>

    <ContentTemplate>

<asp:HiddenField ID="NextConfigurationStep" runat="server" />

<asp:PlaceHolder ID="ConfigurationSteps" runat="server" />

</ContentTemplate>
</asp:UpdatePanel>

</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="DoInitialConfigStep" runat="server" Text="Do Initial Setup" OnClick="DoInitialConfigStep_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelButton_OnClick"/>

</td>
</tr>


</table>

</div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Configure Work Box Collection
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Configure Work Box Collection
</asp:Content>
