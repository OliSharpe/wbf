<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditBlockButtonsDetails.aspx.cs" Inherits="WBFExtraWebParts.Layouts.WBFExtraWebParts.EditBlockButtonsDetails" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

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

    <SharePoint:CssRegistration ID="WBFColorPickerCssRegistration"
      name="WBFExtraWebParts/css/colpick.css" 
      After="corev4.css"
      runat="server"
    />

<script type="text/javascript">
    function WBF_DeleteButton(buttonIndex, title) {

        if (confirm("Are you sure you wish to remove button " + (buttonIndex + 1) + " that has title: " + title)) {
            var deleteButtonIndex = document.getElementById("<%= DeleteButtonIndex.ClientID %>");
            deleteButtonIndex.value = buttonIndex;

            document.forms['aspnetForm'].submit();
        }

    }

    function WBF_checkPreviewButtonHeights() {
        var height = $("#<%= EditHeight.ClientID %>").val();
        WBF_checkBlockButtonsHeights("preview", height);
    }

    /* This jQuery links up the width and height buttons to update the preview buttons */
    $(function () {
        $("#<%= EditWidth.ClientID %>").change(function () {
            var width = $(this).val();
            if (width.indexOf("px") == -1) { width = width + "px"; $(this).val(width); }

            var index = 0;
            for (index=0; index < <%=NumberOfButtons %>; index++) {
                $("#wbf-block-button-preview-" + index).css('width', width);
            }
            
            var height = $("#<%= EditHeight.ClientID %>").val();
            WBF_checkBlockButtonsHeights("preview", height);
        });
        $("#<%= EditHeight.ClientID %>").change(function () {
            var height = $(this).val();
            if (height.indexOf("px") == -1) { height = height + "px"; $(this).val(height); }

            var index = 0;
            for (index=0; index < <%=NumberOfButtons %>; index++) {
                $("#wbf-block-button-preview-" + index).css('height', height);
            }
            WBF_checkBlockButtonsHeights("preview", height);
        });
    });
</script>


<style type="text/css">

<%= CSSExtraStyles %>

</style>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<!-- Note that this has to be placed here to ensure that it comes after the ScriptLink to jQuery -->
<script src="/_layouts/WBFExtraWebParts/colpick.js" type="text/javascript"></script>

<h2 class="block-buttons-edit-page-title">Edit Block Buttons</h2>

<asp:Label ID="ErrorMessage" runat="server" />

<table cellpadding="5px" cellspacing="0">

<tr>
<td>
<b>Width (for all buttons)</b>
</td>
<td align="left">
<asp:TextBox ID="EditWidth" runat="server"  Columns="10"/>
</td>
<td></td>
</tr>

<tr>
<td>
<b>Height (for all buttons)</b>
</td>
<td align="left">
<asp:TextBox ID="EditHeight" runat="server" Columns="10" />
</td>
<td>
<div id="wbf-dynamic-buttons-height" style="color:red;"></div>
</td>
</tr>

</table>


<div>&nbsp;</div>

<asp:PlaceHolder ID="EditBlockButtonsTable" runat="server" />

<script type="text/javascript">
    WBF_checkPreviewButtonHeights(); 
</script>

<div>&nbsp;</div>

<div>
    <asp:Button ID="AddNewBlockButtonButton" runat="server" Text="Add new button"  OnClick="AddNewBlockButtonButton_OnClick"/>
&nbsp;
    <span class="block-buttons-help-text">(You can use // to force text onto a new line for the <b>Title</b> and <b>Extra Text</b> fields)</span>
</div>

<div>&nbsp;</div>

<div>
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick"/>
&nbsp;
    <asp:Button ID="RefreshPreview" runat="server" Text="Refresh Preview"  OnClick="refreshButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>
</div>

<asp:HiddenField ID="BlockButtonsDetails" runat="server" />
<asp:HiddenField ID="HiddenCSSExtraClass" runat="server" />
<asp:HiddenField ID="HiddenCSSExtraStyles" runat="server" />
<asp:HiddenField ID="NumberOfBlockButtons" runat="server" />

<asp:HiddenField ID="DeleteButtonIndex" runat="server" />




<div>&nbsp;</div>

<h2 class="block-buttons-edit-page-title">Preview Block Buttons</h2>

<div>&nbsp;</div>

<table class="block-button-table <%= CSSExtraClass %>" cellpadding="0" cellspacing="0" border="0" align="center">
<tr>

<asp:Literal ID="BlockButtons" runat="server" />

</tr>
</table>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Edit Block Buttons
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Edit Block Buttons
</asp:Content>
