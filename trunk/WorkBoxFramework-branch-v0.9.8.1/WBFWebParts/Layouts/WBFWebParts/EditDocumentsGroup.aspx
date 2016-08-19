<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDocumentsGroup.aspx.cs" Inherits="WBFWebParts.Layouts.WBFWebParts.EditDocumentGroup" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<script type="text/javascript">
    function WBF_DeleteRow(rowIndex, filename) {

        if (confirm("Are you sure you wish to remove row " + (rowIndex + 1) + " that has file: " + filename)) {
            var deleteRowIndex = document.getElementById("<%= DeleteRowIndex.ClientID %>");
            deleteRowIndex.value = rowIndex;

            document.forms['aspnetForm'].submit();
        }

    }


    function WBF_PickDialogCallback(dialogResult, returnValue) {
        if (dialogResult == SP.UI.DialogResult.OK) {

            var parts = returnValue.split('#');


            //alert("Callback to update row: " + parts[0] + " with details: " + parts[1]);


            var replaceRowIndex = document.getElementById("<%= ReplaceRowIndex.ClientID %>");
            replaceRowIndex.value = parts[0];


            var replacementDetails = document.getElementById("<%= ReplacementDetails.ClientID %>");
            replacementDetails.value = parts[1];

            //alert("Got the return value: " + returnValue);

            document.forms['aspnetForm'].submit();
        }

    }

    function WorkBoxFramework_pickADocument(rowIndex, library) {

        var urlValue = L_Menu_BaseUrl + '/_layouts/WBFWebParts/PublishedDocumentPicker.aspx?RowIndex=' + rowIndex + "&Library=" + library; //?CurrentDetails=' + currentRelatedDocumentsDetails; //recordsTypeUIControlValue=' + currentRecordsTypeUIControlValue;

        var options = {
            url: urlValue,
            tite: 'Edit Related Documents List',
            allowMaximize: false,
            showClose: true,
            width: 900,
            height: 650,
            dialogReturnValueCallback: WBF_PickDialogCallback
        };

        SP.UI.ModalDialog.showModalDialog(options);
    }
</script>


</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<h3>Edit a document group</h3>

<asp:Label ID="ErrorMessage" runat="server" />

<table cellpadding="5px" cellspacing="0">

<tr>
<td>
<b>Title</b>
</td>
<td>
<asp:TextBox ID="EditTitle" runat="server" />
</td>
</tr>

<tr>
<td>
<b>Description</b>
</td>
<td>
<asp:TextBox ID="EditDescription" Columns="40" Rows="4" TextMode="MultiLine" runat="server" />
</td>
</tr>

<tr>
<td>
<b>Subject</b>
</td>
<td>
<Taxonomy:TaxonomyWebTaggingControl ID="SubjectTagsField" ControlMode="display" runat="server" />
</td>
</tr>

<tr>
<td>
<b>Coverage</b>
</td>
<td>
<asp:TextBox ID="EditCoverage" runat="server" />
</td>
</tr>

<tr>
<td>
<b>Resources</b>
</td>
<td>

<asp:PlaceHolder ID="EditDocumentsTable" runat="server" />


<div>&nbsp;</div>

<div>
    <asp:Button ID="AddNewPublicDocumentButton" UseSubmitBehavior="false" runat="server" Text="Add new public document"  OnClientClick="WorkBoxFramework_pickADocument(-1, 'Public'); return false;"/>
    <asp:Button ID="AddNewExtranetDocumentButton" UseSubmitBehavior="false" runat="server" Text="Add new extranet document"  OnClientClick="WorkBoxFramework_pickADocument(-1, 'Extranet'); return false;"/>
</div>


</td>
</tr>


</table>


<div>&nbsp;</div>

<div>
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</div>

<asp:HiddenField ID="DocumentsDetails" runat="server" />
<asp:HiddenField ID="NumberOfDocuments" runat="server" />

<asp:HiddenField ID="DeleteRowIndex" runat="server" />
<asp:HiddenField ID="ReplaceRowIndex" runat="server" />
<asp:HiddenField ID="ReplacementDetails" runat="server" />



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Edit Document Group
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Edit Document Group
</asp:Content>
