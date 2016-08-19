<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="wbf" Namespace="WBFWebParts.ControlTemplates.WBFWebParts" Assembly="WBFWebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3b249a3da591438f" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WhereIsRecordBeingUsed.aspx.cs" Inherits="WBFWebParts.Layouts.WBFWebParts.WhereIsRecordBeingUsed" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<h2>Where is this record being used?</h2>

<table>
<tbody>
<tr><td colspan='2'>Looking for record with:</td></tr>
<tr><td>Record ID</td><td><%= recordID %></td></tr>
<tr><td>Title</td><td><%= recordTitle %></td></tr>
<tr><td>Name</td><td><%= recordName %></td></tr>
<tr><td>URL</td><td><%= recordURL %></td></tr>
<tr><td>URL fragment</td><td><%= recordURLToSearchFor%></td></tr>
</tbody>
</table>

<h3 id="just-searched-title">Just Searched:</h3>
<p id="just-searched"></p>
<p><span id="total-searched-text">Total searched so far: </span><span id="searched-count">0</span></p>

<h3>Record was found here:</h3>
<p><span id="total-found-text">Total found so far: </span><span id="found-count">0</span></p>
<table id="results-table" cellpadding="5">
<tbody>
<tr><th>Page URL</th><th>In Web Part</th><th>In Page Content</th></tr>
</tbody>
</table>

<h3>Processing Errors</h3>
<p><span id="total-errors-text">Total errors so far: </span><span id="errors-count">0</span></p>
<table id="error-table" cellpadding="5">
<tbody>
<tr><th>Page URL</th><th>Error Message</th></tr>
</tbody>
</table>

<!-- Loading in jQuery in case it's not loaded already -->
<script type="text/javascript" src="/_layouts/workboxframework/jquery-1.11.3.min.js"></script>

<script type="text/javascript">
    var searched = 0;
    var found = 0;
    var errors = 0;

    function finishedSearch() {
        $('#just-searched-title').text("Search Complete");
        $('#just-searched').text("");

        $('#total-searched-text').text("Total searched: ");
        $('#total-found-text').text("Total found: ");
        $('#total-errors-text').text("Total errors: ");
        
    }

    function justSearched(pageURL) {
        $('#just-searched').text(pageURL);

        searched++;
        $('#searched-count').text(searched);
    }

    function foundUsage(pageURL, inWebPart, inPageContent) {
        justSearched(pageURL);
        $('#results-table > tbody:last').append('<tr><td><a href=\"' + pageURL + '\">' + pageURL + '</a></td><td align=\"center\">' + inWebPart + '</td><td align=\"center\">' + inPageContent + '</td></tr>');

        found++;
        $('#found-count').text(found);
    }

    function errorProcessingPage(pageURL, errorMessage) {
        justSearched(pageURL);
        $('#error-table > tbody:last').append('<tr><td><a href=\"' + pageURL + '\">' + pageURL + '</a></td><td>' + errorMessage + '</td></tr>');

        errors++;
        $('#errors-count').text(errors);
    }

</script>


<!-- And now the control that outputs the on-going search results -->
<wbf:FindWhereRecordIsBeingUsed runat="server"/>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Where is record being used?
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Where is record being used?
</asp:Content>
