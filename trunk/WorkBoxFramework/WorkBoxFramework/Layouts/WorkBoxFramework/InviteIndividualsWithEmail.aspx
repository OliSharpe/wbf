<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteIndividualsWithEmail.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.InviteIndividualsWithEmail" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="WBFjQueryScriptRegistration"
        name="WorkBoxFramework/jquery-1.7.2.min.js"
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

<div class="wbf-dialog">
<div class="wbf-dialog-error">
<asp:Label ID="ErrorText" ForeColor="Red"  runat="server" />
</div>

<div class="wbf-dialog-message">
Select the individuals you want to invite to this work box.
</div>

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box Title</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-read-only-title">
<asp:Label ID="WorkBoxTitle" runat="server" Text=""></asp:Label>
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
				AllowEmpty = "false"
				MultiSelect = "true"
				/>

</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Invite as:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:RadioButtonList ID="InviteType" runat="server" onclick="javascript: toggleEmailText(this);">

<asp:ListItem Text="Involved individual (read, add and edit work box content)" Value="Involved"/>

<asp:ListItem Text="Visiting individual (only read work box content)" Value="Visiting"/>

</asp:RadioButtonList>

<asp:HiddenField ID="CurrentlySelectedValue" runat="server" />
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
<div class="wbf-field-name">Subject of invite email</div>
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
[WORK_BOX_TITLE]<br />
[WORK_BOX_URL]<br />
[USER_NAME]
</p>
</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:TextBox ID="EmailBody" TextMode="MultiLine" Rows="8" Columns="50" Text="" runat="server" />

<asp:HiddenField ID="OtherEmailSubject" runat="server" />
<asp:HiddenField ID="OtherEmailBody" runat="server" />
</div>

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
Invite Individuals
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Invite Individuals
</asp:Content>
