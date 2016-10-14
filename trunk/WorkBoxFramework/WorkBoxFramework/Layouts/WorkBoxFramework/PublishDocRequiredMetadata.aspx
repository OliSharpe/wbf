<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocRequiredMetadata.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocRequiredMetadata" DynamicMasterPageFile="~masterurl/default.master" %>

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
    function WorkBoxFramework_PublishDoc_pickedANewLocation(dialogResult, returnValue) {

        if (dialogResult == SP.UI.DialogResult.OK) {

            var updatedPublishingProcess = document.getElementById("<%=UpdatedPublishingProcessJSON.ClientID %>");
            updatedPublishingProcess.value = returnValue;

            document.forms['aspnetForm'].submit();
        }
    }


    function WorkBoxFramework_pickANewLocation(callbackFunction, currentFunctionalAreasUIControlValue, currentRecordsTypeUIControlValue) {

        var publishingProcessJSON = document.getElementById("<%=PublishingProcessJSON.ClientID %>");
        var newOrReplace = $('#<% =NewOrReplace.ClientID %>').text();
        var archiveOrLeave = "Archive";
        if ($("#<% =LeaveOnIzziCheckBox.ClientID %>").is(':checked')) archiveOrLeave = "Leave";

        var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/PublishDocDialogPickLocation.aspx'
            + '?PublishingProcessJSON=' + publishingProcessJSON.value
            + '&NewOrReplace=' + newOrReplace
            + '&ArchiveOrLeave=' + archiveOrLeave;

        var options = {
            url: urlValue,
            title: 'Pick Location in Records Library',
            allowMaximize: false,
            showClose: true,
            width: 600,
            height: 700,
            dialogReturnValueCallback: callbackFunction
        };

        SP.UI.ModalDialog.showModalDialog(options);
    }

    function WBF_editShortTitle() {
        $("#wbf-show-short-title").hide();
        $("#wbf-edit-short-title").show();
    }

    function WBF_maybeDisablePublishButtons() {
        var location = $("#<%=LocationPath.ClientID %>").text();

        if (location == "") {
            $("#<%=Publish.ClientID %>").attr("disabled", false);
            $("#<%=PublishAll.ClientID %>").attr("disabled", false);
        } else {
            if ($("#<%=NewOrReplace.ClientID %>").text() == "New") {
                $("#<%=PublishAll.ClientID %>").attr("disabled", false);
            } else {
                $("#<%=PublishAll.ClientID %>").attr("disabled", true);
            }
        }
    } 

</script>

</asp:Content>


<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
</div>

<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-publish-out-title">
Publish Document(s) to:  <asp:Label ID="TheProtectiveZone" runat="server" /> Library
</div>
<div>
You must enter the following metadata for the document(s)
</div>
</td>
</tr>
</table>


<asp:HiddenField ID="PublishingProcessJSON" runat="server" />

<asp:HiddenField ID="UpdatedPublishingProcessJSON" runat="server" />

<asp:Label ID="NewOrReplace" runat="server" style="display: none;"/>

<table class="wbf-dialog-form">

<asp:Literal ID="DocumentsBeingPublished" runat="server" />


<tr>
<td class="wbf-field-name-panel" colspan="2">

<div class="wbf-publishing-replace-options">

<asp:RadioButton id="ReplaceRadioButton" GroupName="NewOrReplaceRadios" Value="Replace"
             Text="" runat="server"/><b>Replace</b> an existing document

</div>

<div  class="wbf-publishing-replace-options" style="padding-left: 30px;">
<asp:CheckBox id="LeaveOnIzziCheckBox" runat="server"/> Viewable on izzi search
</div>

<div class="wbf-publishing-replace-options">
<asp:RadioButton id="NewRadioButton" GroupName="NewOrReplaceRadios" Value="New"
             Text="" runat="server"/>Publish a <b>new</b> document
</div>
<div class="wbf-publishing-location-button">
<asp:Button ID="SelectLocationButton" UseSubmitBehavior="false" runat="server" Text="Select Location" />
</div>

<div class="wbf-publishing-choosen-location">
<span id="locationType" style="font-weight: bold; ">Publishing Location</span>: <span class="wbf-publishing-choosen-location"><asp:Label ID="LocationPath" runat="server"></asp:Label></span>
</div>
<div class="wbf-field-error">
<asp:Label ID="PublishingLocationError" runat="server" Text="" ForeColor="Red"/>
</div>
</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Short Title<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<div id="wbf-show-short-title" class="wbf-field-value">
    <asp:Label ID="ShortTitle" runat="server"></asp:Label> | <a href='#' onclick='WBF_editShortTitle();'>edit</a>
</div>

<div id="wbf-edit-short-title" class="wbf-field-value" style=" display:none; ">
    <asp:TextBox ID="EditShortTitle" runat="server"></asp:TextBox>
</div>
<div class="wbf-field-error">
<asp:Label ID="ShortTitleError" runat="server" Text="" ForeColor="Red"/>
</div>

<div class="wbf-field-description">
You can use spaces and capitals. You should remove hyphens/underscores, version no’s, “DRAFT”/”FINAL”, etc.
</div>

<div class="wbf-field-description">
It’s recommended you don’t change this when you’re replacing an existing document.
</div>

</td>
</tr>

<% if (showSubjectTags)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="SubjectTagsTitle" runat="server"/></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value wbf-taxonomy-control">
<Taxonomy:TaxonomyWebTaggingControl ID="SubjectTagsField" ControlMode="display" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="SubjectTagsError" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
<asp:Label ID="SubjectTagsDescription" runat="server"/>
</div>
</td>
</tr>
<% } %>



<% if (showReferenceID)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="ReferenceIDTitle" runat="server"/></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value">
    <asp:TextBox ID="ReferenceID" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="ReferenceIDMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
<asp:Label ID="ReferenceIDDescription" runat="server"/>
</div>
</td>
</tr>
<% } %>

<% if (showReferenceDate)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="ReferenceDateTitle" runat="server"/></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value">
    <SharePoint:DateTimeControl ID="ReferenceDate" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="ReferenceDateMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
<asp:Label ID="ReferenceDateDescription" runat="server"/>
</div>
</td>
</tr>
<% } %>

<% if (showSeriesTag)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="SeriesTagTitle" runat="server"/></div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-value">
<asp:DropDownList ID="SeriesTagDropDownList" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="SeriesTagFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
<asp:Label ID="SeriesTagDescription" runat="server"/>
</div>
</td>
</tr>
 <% } %>

 <% if (showScanDate)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="ScanDateTitle" runat="server"/></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value">
    <SharePoint:DateTimeControl ID="ScanDate" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="ScanDateMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
<asp:Label ID="ScanDateDescription" runat="server"/>
</div>
</td>
</tr>
<% } %>


<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Owning Team<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value wbf-taxonomy-control">
<Taxonomy:TaxonomyWebTaggingControl ID="OwningTeamField" ControlMode="display" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="OwningTeamFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
The team responsible for this document.
</div>
</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Involved Teams</div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-value wbf-taxonomy-control">
<Taxonomy:TaxonomyWebTaggingControl ID="InvolvedTeamsField" ControlMode="display" runat="server" />
</div>
<div class="wbf-field-error">
<asp:Label ID="InvolvedTeamsFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
Other teams that were involved with the creation of this document.
</div>
</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name">Web Page URL</div>
</td>
<td class="wbf-field-value-panel" valign="top">

<div class="wbf-field-value">
<asp:TextBox ID="WebPageURL" runat="server" Columns="60" />
</div>
<div class="wbf-field-error">
<asp:Label ID="WebPageURLMessage" runat="server" Text="" ForeColor="Red"/>
</div>
<div class="wbf-field-description">
If this document needs to be shown on a webpage please provide the pages full URL (address) here.
</div>
</td>
</tr>


<tr>
<td colspan="2" class="wbf-buttons-panel">
<p>
        <asp:Button ID="Publish" UseSubmitBehavior="false" runat="server" Text="Publish" OnClick="publishButton_OnClick" />

<% if (process.AllowBulkPublishAllTogether)
   { %>
        &nbsp;

        <asp:Button ID="PublishAll" UseSubmitBehavior="false" runat="server" Text="Publish All" OnClick="publishAllButton_OnClick" />
<%} %>

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

</div>

<script>
    $(function () {

        var selectLocationButton = $('#<%=SelectLocationButton.ClientID %>');

        if ($("#<%=NewOrReplace.ClientID %>").text() == "New") {
            $("#<%=LeaveOnIzziCheckBox.ClientID %>").attr("disabled", true);
        }

        $('input:radio[name="ctl00$PlaceHolderMain$NewOrReplaceRadios"]').change(function () {
            if ($(this).val() == 'New') {
                selectLocationButton.val("Choose Location");
                $("#<%=NewOrReplace.ClientID %>").text("New");
                $("#locationType").text("Publishing location");
//                $("#<%=PublishAll.ClientID %>").attr("disabled", false);
                $("#<%=LeaveOnIzziCheckBox.ClientID %>").attr("disabled", true);
            } else {
                selectLocationButton.val("Choose Document");
                $("#<%=NewOrReplace.ClientID %>").text("Replace");
                $("#locationType").text("Document to replace");
//                $("#<%=PublishAll.ClientID %>").attr("disabled", true);
                $("#<%=LeaveOnIzziCheckBox.ClientID %>").attr("disabled", false);
            }

            WBF_maybeDisablePublishButtons();
        });

        WBF_maybeDisablePublishButtons();

    });

    // Hopefully this will resize the modal correctly. 
    // Found with thanks in answers here: https://social.msdn.microsoft.com/Forums/sharepoint/en-US/ddd6ce37-b289-47d5-92ad-067b2c9ee4fd/resizing-an-open-dialog-as-its-contents-change
//    var currentModal = SP.UI.ModalDialog.get_childDialog();
  //  currentModal.$$d_autoSize();
</script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Publish Document: Required Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Publish Document
</asp:Content>
