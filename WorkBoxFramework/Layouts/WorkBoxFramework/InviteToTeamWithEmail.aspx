﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteToTeamWithEmail.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.InviteToTeamWithEmail" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFjQueryScriptRegistration"
        name="WorkBoxFramework/jquery-1.11.3.min.js"
        language="javascript"
        localizable="false"
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
<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorText" ForeColor="Red"  runat="server" />
</div>

<div class="wbf-dialog-message">
Select the individuals you want to invite to this team.
</div>

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Team Name</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="TeamName" runat="server" Text=""></asp:Label>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Select Individuals<span class="wbf-required-asterisk">*</span></div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
			<SharePoint:PeopleEditor id="IndividualsToInviteControl" runat="server"
				SelectionSet="User"
				ValidatorEnabled="true"
				AllowEmpty = "true"
				MultiSelect = "true"
				/>
</div>
<div class="wbf-field-error">
<asp:Label ID="IndividualsToInviteFieldMessage" runat="server" Text="" ForeColor="Red"/>
</div>


</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Invite as:</div>
</td>
<td class="wbf-field-value-panel">
<div class="wbf-field-value">
<div>
<asp:CheckBox ID="InviteAsOwner" Text="Team owner (can manage team membership)" runat="server"/>
</div>
<div>
<asp:CheckBox ID="InviteAsMember" Text="Team member (can use team's work boxes)" runat="server"/>
</div>
</div>


</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Send invite email?</div>
</td>
<td class="wbf-field-value-panel">
<div class="wbf-field-value">
<div>
<asp:CheckBox ID="SendInviteEmail" Text="Send invite email?" runat="server" />
</div>
<div>
<asp:CheckBox ID="SendAsOne" Text="Send one email to everyone?" runat="server" />
</div>
<div>
<asp:CheckBox ID="CCToYou" Text="CC email(s) to yourself?" runat="server" />
</div>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Subject of invite email</div
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="EmailSubject" Text="" Columns="50" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Text of invite email</div>
<div class="wbf-field-description">
<p>Template tags:<br />
[TEAM_NAME]<br />
[TEAM_SITE_URL]<br />
[ROLE_WITHIN_TEAM]<br />
[USER_NAME]
</p>
</div>

</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="EmailBody" TextMode="MultiLine" Rows="8" Columns="50" Text="" runat="server" />
</div>

<asp:HiddenField ID="OtherEmailSubject" runat="server" />
<asp:HiddenField ID="OtherEmailBody" runat="server" />

</td>
</tr>

<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="InviteButton" runat="server" Text="Invite"  OnClick="inviteButton_OnClick" />
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="cancelButton_OnClick"/>

</td>
</tr>


</table>
</div>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Invite To Team
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Invite To Team
</asp:Content>
