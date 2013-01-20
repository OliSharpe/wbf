<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteTeamsWithEmail.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.InviteTeamsWithEmail" DynamicMasterPageFile="~masterurl/default.master" %>

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
    function getSelectedValue(radioButtonList) {
        for (var i = 0; i < radioButtonList.rows.length; ++i) {
            if (radioButtonList.rows[i].cells[0].firstChild.checked) {
                return radioButtonList.rows[i].cells[0].firstChild.value;
            }
        }

        return "";
    }

    // Doing this the hard way as there is no client side event to catch the change event:
    function toggleEmailText(radioButtonList) {

        var currentlySelectedValueField = $("#<%= CurrentlySelectedValue.ClientID %>");

        var currentlySelectedValue = getSelectedValue(radioButtonList);

        // So we'll toggle the email details if the selected value has changed:
        if (currentlySelectedValueField.val() != currentlySelectedValue) {

            currentlySelectedValueField.val(currentlySelectedValue);

            var subjectTB = $("#<%= EmailSubject.ClientID %>");
            var bodyTB = $("#<%= EmailBody.ClientID %>");

            var otherSubjectHF = $("#<%= OtherEmailSubject.ClientID %>");
            var otherBodyHF = $("#<%= OtherEmailBody.ClientID %>");

            var currentSubject = subjectTB.val();
            var currentBody = bodyTB.val();

            subjectTB.val(otherSubjectHF.val());
            bodyTB.val(otherBodyHF.val());

            otherSubjectHF.val(currentSubject);
            otherBodyHF.val(currentBody);
        }
    }
</script>

<asp:Label ID="ErrorText" ForeColor="Red"  runat="server" />

<p>
Select the teams you want to invite to this work box.
</p>

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td valign="top">
<b>Work Box Title</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:Label ID="WorkBoxTitle" runat="server" Text=""></asp:Label>

</td>
</tr>


<tr>
<td valign="top">
<b>Select Teams</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<Taxonomy:TaxonomyWebTaggingControl ID="TeamsToInviteControl" ControlMode="display" runat="server" />
<br />
<asp:Label ID="InvolvedTeamsFieldMessage" runat="server" Text="" ForeColor="Red"/>


</td>
</tr>

<tr>
<td valign="top">
<b>Invite as:</b>
<p></p>
</td>
<td class="ms-authoringcontrols" valign="top">

<asp:RadioButtonList ID="InviteType" runat="server" onclick="javascript: toggleEmailText(this);">

<asp:ListItem Text="Involved team (read, add and edit work box content)" Value="Involved"/>

<asp:ListItem Text="Visiting team (only read work box content)" Value="Visiting"/>

</asp:RadioButtonList>

<asp:HiddenField ID="CurrentlySelectedValue" runat="server" />


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
[WORK_BOX_TITLE]<br />
[WORK_BOX_URL]<br />
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
Invite Teams
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Invite Teams
</asp:Content>
