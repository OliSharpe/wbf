<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishDocDialogPickLocation.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.PublishDocDialogPickLocation" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
</div>

<table class="ms-propertysheet" width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>New Records Type</b>
<p>
Select from the taxonomy of records types.
</p>
</td>
<td class="ms-authoringcontrols" valign="top">

The tree will go here.

<br />
RecordsType = <asp:Label ID="RecordsTypeUIControlValue" runat="server"/>
<br />
FunctionAreas = <asp:Label ID="FunctionalAreasUIControlValue" runat="server"/>


<asp:Label ID="RecordsTypeFieldMessage" runat="server" Text="" ForeColor="Red"/>
</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
<p>
        <asp:Button ID="Select" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Save" OnClick="selectButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

</div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Pick Location In Records Library
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Pick Location In Records Library
</asp:Content>
