<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeTeamManager.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.ChangeTeamManager" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<style type="text/css">
td 
{
border-top:solid 1px grey;
}
</style>

<h2>Change Team Manager</h2>
<asp:Label ID="ErrorText" ForeColor="Red"  runat="server" />

<p>
Select the name of the new manager.
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Team Name</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:Label ID="TeamName" runat="server" Text=""></asp:Label>

</td>
</tr>


<tr>
<td valign="top">
<b>Select new team manager</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

			<SharePoint:PeopleEditor id="NewTeamManager" runat="server"
				SelectionSet="User"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "false"
				/>

<div>
                    <asp:RequiredFieldValidator ID="NewTeamManagerValidator" runat="server" ErrorMessage="You must enter a new team manager or click cancel."
            ControlToValidate = "NewTeamManager"></asp:RequiredFieldValidator>
</div>

</td>
</tr>

<tr>
<td align="center" valign="top" colspan="2">
    <asp:Button ID="ChangeButton" runat="server" Text="Change Team Manager"  OnClick="changeButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" OnClick="cancelButton_OnClick"/>
</td>
</tr>


</table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Change Team Manager
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Change Team Manager
</asp:Content>
