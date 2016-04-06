<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlockButtonsUserControl.ascx.cs" Inherits="WBFExtraWebParts.BlockButtons.BlockButtonsUserControl" %>

    <SharePoint:ScriptLink ID="WBFjQueryScriptRegistration"
        name="WorkBoxFramework/jquery-1.11.3.min.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

   <SharePoint:ScriptLink ID="WBFExtraWebPartsScriptRegistration"
        name="WBFExtraWebParts/WBFExtraWebParts.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

    <SharePoint:CssRegistration ID="WBFBlockButtonsCssRegistration"
      name="WBFExtraWebParts/css/BlockButtons.css" 
      After="corev4.css"
      runat="server"
    />


<style type="text/css">
<%= CSSExtraStyles %>
</style>

<script type="text/javascript">
    function WBF_EditDialogCallback<%=WebPartUniqueID %>(dialogResult, returnValue) {
        if (dialogResult == SP.UI.DialogResult.OK) {

            var blockButtonsDetails = document.getElementById("<%=BlockButtonsDetails.ClientID %>");
            blockButtonsDetails.value = returnValue;

            var needToSave = document.getElementById("<%=NeedToSave.ClientID %>");
            needToSave.value = "true";

            WBF_UpdateBlockButtons("<%=WebPartUniqueID %>", returnValue); 

            // document.forms['aspnetForm'].submit();
        }
    }
</script>

<% if (InEditMode) { %>
<table cellpadding="0" cellspacing="0" border="0" align="center">
<tbody>
<tr>
<td>
<% } %>

<div id="wbf-block-buttons-data-<%=WebPartUniqueID %>" data-block-button-details="" display="none"/>
<table id="wbf-block-buttons-table-<%=WebPartUniqueID %>" class="block-button-table <%= CSSExtraClass %>" cellpadding="0" cellspacing="0" border="0" align="center">
<tbody>
<tr>

<asp:Literal ID="BlockButtons" runat="server" />

</tr>
</tbody>
</table>

<% if (InEditMode) { %>
</td>
<td>
<asp:Button ID="EditBlockButtonsButton" UseSubmitBehavior="false" runat="server" Text="Edit Buttons" />
<asp:HiddenField ID="BlockButtonsDetails" runat="server" />
<asp:HiddenField ID="NeedToSave" runat="server" Value="false"/>
</td>
</tr>
</tbody>
</table>
<% } %>
