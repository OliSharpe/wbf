<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeamDetailsUserControl.ascx.cs" Inherits="WorkBoxFramework.TeamDetails.TeamDetailsUserControl" %>

<style type="text/css">
.wbf-team-details-webpart h3 
{
    color: #650260;
    font-family: arial, helvetica, Sans-Serif;
    font-size: 16px;
    font-weight: bold;
}
</style>

<script type="text/javascript">

    // There's probably a nicer way to do this ....
    var wbf__user_presence_sips = new Object();
    var wbf__user_presence_elements = new Object();
    var wbf__user_presence_ids = new Array();

    function WBF_team_details__add_user_presence(id, sip, element) {
        wbf__user_presence_sips[id] = sip;
        wbf__user_presence_elements[id] = element;
    }

    function WBF_team_details__do_user_presence() {

        for (var id in wbf__user_presence_ids) {

            var sip = wbf__user_presence_sips[id];
            var element = wbf__user_presence_elements[id];

            IMNRC(sip, element);
        }
    }

    // We want to run this function when the page has finished loading:
    _spBodyOnLoadFunctionNames.push("WBF_team_details__do_user_presence");   

</script>

<div class="wbf-team-details-webpart">

<asp:Literal ID="ListOfTeamOwners" runat="server" />

<asp:Literal ID="ListOfTeamMembers" runat="server" />

<% if (userIsTeamOwner) { %>

<h3>Manage Team:</h3>
<ul>
<li><a href="javascript: WorkBoxFramework_relativeCommandAction('InviteToTeamWithEmail.aspx', 660, 500); ">Invite user to team</a></li>
</ul>

<% } %>

</div>