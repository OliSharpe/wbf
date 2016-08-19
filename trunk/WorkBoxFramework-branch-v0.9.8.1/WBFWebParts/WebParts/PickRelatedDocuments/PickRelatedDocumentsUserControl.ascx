<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickRelatedDocumentsUserControl.ascx.cs" Inherits="WBFWebParts.PickRelatedDocuments.PickRelatedDocumentsUserControl" %>

<% if (InEditMode)
   { %>
<script type="text/javascript">
    function WBF_EditDialogCallback<%=WebPartUniqueID %>(dialogResult, returnValue) {
        if (dialogResult == SP.UI.DialogResult.OK) {


            var newRelatedDocumentsDetails = document.getElementById("<%=NewRelatedDocumentsDetails.ClientID %>");
            newRelatedDocumentsDetails.value = returnValue;


            var needToSave = document.getElementById("<%=NeedToSave.ClientID %>");
            needToSave.value = "true";

            //alert("Got the return value: " + returnValue);

            document.forms['aspnetForm'].submit();
        }

    }

    function WorkBoxFramework_editRelatedDocuments(callbackFunction, currentRelatedDocumentsDetails) {

        var urlValue = L_Menu_BaseUrl + '/_layouts/WBFWebParts/EditRelatedDocuments.aspx?CurrentDetails=' + currentRelatedDocumentsDetails; //recordsTypeUIControlValue=' + currentRecordsTypeUIControlValue;

        var options = {
            url: urlValue,
            tite: 'Edit Related Documents List',
            allowMaximize: false,
            showClose: true,
            width: 850,
            height: 400,
            dialogReturnValueCallback: callbackFunction
        };

        SP.UI.ModalDialog.showModalDialog(options);
    }
</script>
<% } %>

<% if (DocumentsToView || InEditMode || showDescription) { %>

<div class="relatedContentsMain">

<asp:Panel Id="DisplayPanel" runat="server">

<% if (showDescription)
   { %>
<div class="wbf-related-documents-description">
<asp:Literal ID="Description" runat="server" />
</div>
<% } %>


<% if (DocumentsToView || InEditMode) { %>
<div class="wbf-related-documents-list">

<asp:Literal ID="DocumentList" runat="server" />

</div>
<% } %>

</asp:Panel>

<% if (InEditMode) { %>
<asp:Panel Id="EditPanel" runat="server">

<asp:Button ID="EditRelatedDocumentsButton" UseSubmitBehavior="false" runat="server" Text="Edit" />
<asp:HiddenField ID="NewRelatedDocumentsDetails" runat="server" />
<asp:HiddenField ID="NeedToSave" runat="server" Value="false"/>

</asp:Panel>
<% } %>
<div>&nbsp;</div>

</div>

<% } %>