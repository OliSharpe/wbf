<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListWorkBoxesWebPartUserControl.ascx.cs" Inherits="WorkBoxFramework.TeamsWorkBoxesWebPart.TeamsWorkBoxesWebPartUserControl" %>

<asp:Literal ID="WebPartContent" runat="server"></asp:Literal>

<script type="text/javascript">
    function createNewWorkBox(teamsTermGuid, encodedWorkBoxCollectionUrl) {

        // I don't really like this solution - but it works!
        var urlValue = L_Menu_BaseUrl + "/_layouts/WorkBoxFramework/NewWorkBox.aspx?teamsTermGuid=" + teamsTermGuid + "&workBoxCollectionUrl=" + encodedWorkBoxCollectionUrl;
//        var urlValue = workBoxCollectionRoot + '_layouts/WorkBoxFramework/NewWorkBox.aspx' + parameters;


        var options = {
            url: urlValue,
            tite: 'New Work Box Dialog',
            allowMaximize: false,
            showClose: false,
            width: 600,
            height: 500,
            dialogReturnValueCallback: WorkBoxFramework_callback
        };

        SP.UI.ModalDialog.showModalDialog(options);
    }

</script>

<% if (showCreateNewLink)
   { %>
<p style="border: 5 white;">
<a href="#" onclick="javascript: createNewWorkBox('<%=teamsTermGuid %>', '<%=encodedWorkBoxCollectionUrl %>');"><%= createNewLinkText%></a>
</p>
<% } %>

