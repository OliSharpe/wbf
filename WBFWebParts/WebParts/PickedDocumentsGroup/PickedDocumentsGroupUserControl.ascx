<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickedDocumentsGroupUserControl.ascx.cs" Inherits="WBFWebParts.PickedDocumentsGroup.PickedDocumentsGroupUserControl" %>


<% if (InEditMode)
   { %>
<script type="text/javascript">
    function WBF_EditDialogCallback<%=WebPartUniqueID %>(dialogResult, returnValue) {
        if (dialogResult == SP.UI.DialogResult.OK) {


            var newDocumentsGroupDetails = document.getElementById("<%=NewDocumentsGroupDetails.ClientID %>");
            newDocumentsGroupDetails.value = returnValue;


            var needToSave = document.getElementById("<%=NeedToSave.ClientID %>");
            needToSave.value = "true";

            //alert("Got the return value: " + returnValue);

            document.forms['aspnetForm'].submit();
        }

    }

    function WorkBoxFramework_editDocumentsGroup(callbackFunction, currentDocumentsGroupDetails) {

        var urlValue = L_Menu_BaseUrl + '/_layouts/WBFWebParts/EditDocumentsGroup.aspx?CurrentDetails=' + currentDocumentsGroupDetails; 

        var options = {
            url: urlValue,
            tite: 'Edit Documents Group List',
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

<div class="wbf-documents-group-main">

<asp:Panel Id="DisplayPanel" runat="server">

<div class="wbf-documents-group-title">
<asp:Label ID="Title" runat="server" />
</div>

<table>

<tr>
<td class="wbf-label">
Description
</td>
<td class="wbf-value">
<asp:Literal ID="Description" runat="server" />
</td>
</tr>

<tr>
<td class="wbf-label">
Subject
</td>
<td class="wbf-value">
<asp:Label ID="SubjectTags" runat="server" />
</td>
</tr>

<tr>
<td class="wbf-label">
Coverage
</td>
<td class="wbf-value">
<asp:Label ID="Coverage" runat="server" />
</td>
</tr>

<tr>
<td class="wbf-label">
Resources
</td>
<td class="wbf-value wbf-documents-list">

<asp:Literal ID="DocumentsList" runat="server" />

</td>
</tr>


</table>

</asp:Panel>

<asp:Panel Id="EditPanel" runat="server">

<asp:Button ID="EditDocumentsGroupButton" UseSubmitBehavior="false" runat="server" Text="Edit" />
<asp:HiddenField ID="NewDocumentsGroupDetails" runat="server" />
<asp:HiddenField ID="NeedToSave" runat="server" Value="false"/>

</asp:Panel><div>&nbsp;</div>

</div>
