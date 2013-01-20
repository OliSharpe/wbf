<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestDocumentPicker.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.TestDocumentPicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

<script type="text/javascript">

    function WorkBoxFramework_pickAPublishedDocumentCallback(dialogResult, returnValue) {

        if (dialogResult == SP.UI.DialogResult.OK) {

            if (returnValue == null || returnValue == "") {
                return;
            }

            var pickedSpan = document.getElementById('PickedDocument');

            pickedSpan.innerHTML = returnValue;
        }
    }
</script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<h3>Test Document Picker</h3>

<a href="#" onclick="javascript: WorkBoxFramework_pickAPublishedDocument(WorkBoxFramework_pickAPublishedDocumentCallback , 'Public');">Pick a document</a>


<p>
<b>Picked document: <span id="PickedDocument"><i>(none yet)</i></span></b>
</p>



<p>

<a href="#" onclick="javascript: createNewDocumentWithProgID('http://workboxportals/projects/15/Project-ffff/Project%20ffff%20-%20fffff%20-%20Documents/Forms/template.dotx', 'http://workboxportals/projects/15/Project-ffff/Project%20ffff%20-%20fffff%20-%20Documents', 'SharePoint.OpenDocuments', false);"> Test this out!!</a>
</p>


<div>
<h3>Test UserProfile.ModifyUserProfilePicture Method</h3>
<asp:TextBox ID="FileToLoad" runat="server" />
<asp:Button ID="UploadPicture" runat="server" OnClick="UploadPicture_OnClick" />

</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
