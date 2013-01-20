<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteToTeamWithEmail.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.InviteToTeamWithEmail" DynamicMasterPageFile="~masterurl/default.master" %>

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

<asp:Label ID="ErrorText" ForeColor="Red"  runat="server" />

<p>
Select the individuals you want to invite to this team.
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Team Name</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<b>
<asp:Label ID="TeamName" runat="server" Text=""></asp:Label>
</b>

</td>
</tr>


<tr>
<td valign="top">
<b>Select Individuals</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

			<SharePoint:PeopleEditor id="IndividualsToInviteControl" runat="server"
				SelectionSet="User"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "true"
				/>


</td>
</tr>

<tr>
<td valign="top">
<b>Invite as:</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">
<div>
<asp:CheckBox ID="InviteAsOwner" Text="Team owner (can manage team membership)" runat="server"/>
</div>
<div>
<asp:CheckBox ID="InviteAsMember" Text="Team member (can use team's work boxes)" runat="server"/>
</div>



</td>
</tr>



<tr>
<td valign="top">
<b>Send invite email?</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">
<div>
<asp:CheckBox ID="SendInviteEmail" Text="Send invite email?" runat="server" />
</div>
<div>
<asp:CheckBox ID="SendAsOne" Text="Send one email to everyone?" runat="server" />
</div>
<div>
<asp:CheckBox ID="CCToYou" Text="CC email(s) to yourself?" runat="server" />
</div>

</td>
</tr>

<tr>
<td valign="top">
<b>Subject of invite email</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="EmailSubject" Text="" Columns="50" runat="server" />

</td>
</tr>

<tr>
<td valign="top">
<b>Text of invite email</b>
<p></p>
<p>Template tags:<br />
[TEAM_NAME]<br />
[TEAM_SITE_URL]<br />
[ROLE_WITHIN_TEAM]<br />
[USER_NAME]
</p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:TextBox ID="EmailBody" TextMode="MultiLine" Rows="8" Columns="50" Text="" runat="server" />

<asp:HiddenField ID="OtherEmailSubject" runat="server" />
<asp:HiddenField ID="OtherEmailBody" runat="server" />

</td>
</tr>

<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="InviteButton" runat="server" Text="Invite"  OnClick="inviteButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Invite To Team
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Invite To Team
</asp:Content>
