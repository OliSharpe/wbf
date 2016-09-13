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

            var values = returnValue.split("@");

            var newFunctionalAreas = document.getElementById("<%=NewFunctionalAreasUIControlValue.ClientID %>");
            newFunctionalAreas.value = values[0];

            var newRecordsType = document.getElementById("<%=NewRecordsTypeUIControlValue.ClientID %>");
            newRecordsType.value = values[1];

            var toReplaceRecordID = document.getElementById("<%=ToReplaceRecordID.ClientID %>");
            toReplaceRecordID.value = values[2];

            var toReplaceRecordPath = document.getElementById("<%=ToReplaceRecordPath.ClientID %>");
            toReplaceRecordPath.value = values[3];

            document.forms['aspnetForm'].submit();
        }
    }


    function WorkBoxFramework_pickANewLocation(callbackFunction, currentFunctionalAreasUIControlValue, currentRecordsTypeUIControlValue) {

        var listGUID = document.getElementById("<%=ListGUID.ClientID %>");
        var itemID = document.getElementById("<%=ItemID.ClientID %>");
        var destinationTitle = document.getElementById("<%=DestinationTitle.ClientID %>");
        var destinationType = document.getElementById("<%=TheDestinationType.ClientID %>");
        var newOrReplace = document.getElementById("<%=NewOrReplace.ClientID %>");
        var protectiveZone = document.getElementById("<%=ProtectiveZone.ClientID %>");

        var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/PublishDocDialogPickLocation.aspx' 
            + '?FunctionalAreasUIControlValue=' + currentFunctionalAreasUIControlValue
            + '&RecordsTypeUIControlValue=' + currentRecordsTypeUIControlValue
            + "&NewOrReplace=" + $(newOrReplace).text()
            + "&ListGUID=" + listGUID.value
            + "&ItemID=" + itemID.value
            + "&DestinationTitle=" + destinationTitle.value
            + "&DestinationType=" + destinationType.value
            + "&ProtectiveZone=" + protectiveZone.value;

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



</script>

</asp:Content>


<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
</div>

<table cellpadding="8" cellspacing="0" class="wbf-title-table">
<tr>
<td valign="middle">
<asp:Image ID="SourceDocIcon" runat="server" />
</td>
<td valign="middle" class="wbf-create-new-title">
<div class="wbf-publish-out-title">
Publish Document to: <asp:Label ID="DestinationTitle" runat="server" />
</div>
<div>
You must enter the following metadata for the document
</div>
</td>
</tr>
</table>


<asp:HiddenField ID="ListGUID" runat="server" />
<asp:HiddenField ID="ItemID" runat="server" />
<asp:HiddenField ID="TheDestinationType" runat="server" />
<asp:HiddenField ID="DestinationURL" runat="server" />

<asp:HiddenField ID="RecordsTypeUIControlValue" runat="server"/>
<asp:HiddenField ID="NewRecordsTypeUIControlValue" runat="server" Value="" />

<asp:HiddenField ID="FunctionalAreasUIControlValue" runat="server"/>
<asp:HiddenField ID="NewFunctionalAreasUIControlValue" runat="server" Value="" />

<asp:HiddenField ID="ToReplaceRecordID" runat="server" Value="" />
<asp:HiddenField ID="ToReplaceRecordPath" runat="server" Value="" />

<asp:HiddenField ID="ProtectiveZone" runat="server"/>

<asp:Label ID="NewOrReplace" runat="server"/>

<table class="wbf-dialog-form">


<tr>
    <td class="wbf-field-name-panel">
        <div class="wbf-field-name">Publishing Document</div>
    </td>
    <td class="wbf-field-value-panel">
        <div class="wbf-field-read-only-title">
            <asp:Label ID="ReadOnlyNameField" runat="server"></asp:Label>
        </div>
        <div class="wbf-field-read-only-value">
            <asp:Label ID="OriginalFileName" runat="server"></asp:Label>
        </div>
    </td>
</tr>


<tr>
<td class="wbf-field-name-panel" colspan="2">

<div>
<asp:RadioButton id="ReplaceRadioButton" GroupName="NewOrReplaceRadios" Value="Replace"
             Text="I want to replace existing document" runat="server"/>
</div>
<div>
<!--                     
AutoPostBack="True"
OnSelectedIndexChanged="Selection_Change"
-->

<span>
I want to 
<asp:DropDownList id="ReplacementActions"
                    runat="server">

                  <asp:ListItem Value="Archive">Archive</asp:ListItem>
                  <asp:ListItem Value="Retire">Retire</asp:ListItem>

               </asp:DropDownList>
the document being replaced.
</span>

</div>

<div>
<asp:RadioButton id="NewRadioButton" GroupName="NewOrReplaceRadios" Value="New"
             Text="I want to publish a new document" runat="server"/>
</div>
<div>
<asp:Button ID="SelectLocationButton" UseSubmitBehavior="false" runat="server" Text="Select Location" />
</div>

<div>
<span>Publishing Location:</span> <span><asp:Label ID="LocationPath" runat="server"></asp:Label></span>
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
</td>
</tr>

<% if (showSubjectTags)
   { %>
<tr>
<td class="wbf-field-name-panel">
        <div class="wbf-field-name"><asp:Label ID="SubjectTagsTitle" runat="server"/></div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value">
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
        <div class="wbf-field-name">Owning Team</div>
</td>
<td class="wbf-field-value-panel" valign="top">
<div class="wbf-field-value">
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

<div class="wbf-field-value">
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
<td colspan="2" class="wbf-buttons-panel">
<p>
        <asp:Button ID="Publish" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Next" OnClick="publishButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

</div>

<script>
    $(function () {

        var selectLocationButton = $('#<%=SelectLocationButton.ClientID %>');

        $('input:radio[name="ctl00$PlaceHolderMain$NewOrReplaceRadios"]').change(function () {
            if ($(this).val() == 'New') {
                selectLocationButton.val("Choose Location");
                $("#<%=NewOrReplace.ClientID %>").text("New");
            } else {
                selectLocationButton.val("Choose Document");
                $("#<%=NewOrReplace.ClientID %>").text("Replace");
            }
        });
    });
</script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Publish Document: Required Metadata
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Publish Document
</asp:Content>
