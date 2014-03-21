<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecordsTypePicker.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.RecordsTypePicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFScriptRegistration"
        name="WorkBoxFramework/WorkBoxFramework.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


<table class="ms-propertysheet" width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>New Records Type</b>
<p>
Select from the taxonomy of records types.
</p>
</td>
<td class="ms-authoringcontrols" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="RecordsTypeField" ControlMode="display" runat="server" />

<br />
<asp:Label ID="RecordsTypeFieldMessage" runat="server" Text="" ForeColor="Red"/>
</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
<p>
        <asp:Button ID="Save" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Save" OnClick="saveButton_OnClick" />

        &nbsp;

        <asp:Button ID="Cancel" UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="Cancel" OnClick="cancelButton_OnClick"
            CausesValidation="False"/>
</p>
</td>
</tr>

</table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Records Type Picker
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Records Type Picker
</asp:Content>
