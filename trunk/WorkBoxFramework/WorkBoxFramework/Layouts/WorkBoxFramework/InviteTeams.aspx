<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteTeams.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.InviteTeams" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<asp:Label ID="ErrorMessageLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<script type="text/javascript">

    $("#<%= controlID %>").focusout(function () { alert("Focus out fired"); });

</script>

<p>
Invite teams to be involved with the Work Box.
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Involved Teams</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="InvolvedTeamsField" ControlMode="display" runat="server" />
<br />
<asp:Label ID="InvolvedTeamsFieldMessage" runat="server" Text="" ForeColor="Red"/>

</td>
</tr>


<tr>
<td valign="top">
<b>Visiting Teams</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="VisitingTeamsField" ControlMode="display" runat="server" />
<br />
<asp:Label ID="VisitingTeamsFieldMessage" runat="server" Text="" ForeColor="Red"/>

</td>
</tr>


<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="saveButton_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Invite Teams to Work Box
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Invite Teams to Work Box
</asp:Content>
