<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkBoxPicker.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.WorkBoxPicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

<script type="text/javascript">
    function WorkBoxFramework_WorkBoxPicker_pickWorkBox(workBoxURL, workBoxTitle) {
            window.frameElement.commonModalDialogClose(SP.UI.DialogResult.OK, workBoxURL + ";" + workBoxTitle);
    }
</script>


</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<p>
Pick a Work Box
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>My Recent Work Boxes</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:Literal ID="RecentWorkBoxes" runat="server" Text=""></asp:Literal>

</td>
</tr>

<tr>
<td valign="top">
<b>My Favourite Work Boxes</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:Literal ID="FavouriteWorkBoxes" runat="server" Text=""></asp:Literal>

</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick" />

</td>
</tr>


</table>




</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Work Box Picker
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Work Box Picker
</asp:Content>
