<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Search" Namespace="Microsoft.Office.Server.Search.WebControls" Assembly="Microsoft.Office.Server.Search, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkBoxExplorerUserControl.ascx.cs" Inherits="WorkBoxFramework.WorkBoxExplorer.WorkBoxExplorerUserControl" %>

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

   <SharePoint:ScriptLink ID="WBFScriptRegistration"
        name="WorkBoxFramework/WorkBoxFramework.js"
        language="javascript"
        localizable="false"
        runat="server"
     />

<script type="text/javascript">

    $("#wbf-right-tabs").insertAfter("#wbf-right-tabs-go-here");

    var wbf__id_of_hidden_submit_link = "<%= HiddenSubmitLink.ClientID %>";
    var wbf__id_of_hidden_records_type_guid_field = "<%= HiddenRecordsTypeGUIDField.ClientID %>";

    var wbf__id_of_hidden_view_style_field = "<%= HiddenViewStyleField.ClientID %>";

    var wbf__is_details_view_style = <%= IsDetailsViewStyle %>;

    var wbf__request_has_query_keywords = (WorkBoxFramework_getParameterByName("k") != "");

    function showResultsIfDoingQuery() {
        if (wbf__request_has_query_keywords) {
            // OK so a query is being done on this request so let's show the results div:
            showSearchResultsView();
        }   
    }

    function repositionWBExplorerTabs() {
        var top = $("#wbf-site-title").offset().top;
        var left = $("#wbf-wb-explorer-tabs-column").offset().left;
        
        $("#wbf-wb-explorer-tabs").offset({ top: top, left: left });
    }

    function toggleViewStyle() {
        if (wbf__is_details_view_style) {
            showIconsView();
        } else {
            showDetailsView();
        }
    }

    function showDetailsView() {
        $("#wbf-wb-explorer-icon-view").hide();
        $("#wbf-wb-explorer-details-view").show();
        $("#wbf-wb-explorer-search-results").hide();
        $("#wbf-wb-explorer-refinement-panel").hide();
        $("#wbf-image-for-changing-view-style").attr("src", "/_layouts/images/WorkBoxFramework/icons-view-32.png");
        $("#wbf-image-for-changing-view-style").attr("title", "Change to icons view");
        $("#wbf-filter-icon-list-item").show();
        $("#wbf-columns-icon-list-item").show();
        $("#" + wbf__id_of_hidden_view_style_field).val("Details View");
        wbf__is_details_view_style = true;
    }

    function showIconsView() {
        $("#wbf-wb-explorer-details-view").hide();
        $("#wbf-wb-explorer-icon-view").show();
        $("#wbf-wb-explorer-search-results").hide();
        $("#wbf-wb-explorer-refinement-panel").hide();
        $("#wbf-image-for-changing-view-style").attr("src", "/_layouts/images/WorkBoxFramework/details-view-32.png");
        $("#wbf-image-for-changing-view-style").attr("title", "Change to details view");
        $("#wbf-filter-icon-list-item").show();
        $("#wbf-columns-icon-list-item").hide();
        $("#" + wbf__id_of_hidden_view_style_field).val("Icons View");
        wbf__is_details_view_style = false;
    }

    function showSearchResultsView() {
        $("#wbf-wb-explorer-icon-view").hide();
        $("#wbf-wb-explorer-details-view").hide();
        $("#wbf-wb-explorer-search-results").show();
        $("#wbf-wb-explorer-refinement-panel").show();
        $("#wbf-filter-icon-list-item").hide();
        $("#wbf-columns-icon-list-item").hide();
    }

    function aspPanelHasUpdated() {
        $("#wbf-wb-explorer-tabs").tabs({
            collapsible: true,
            active: false
        });

        // Make sure that the right view is showing:
        if (wbf__is_details_view_style) {
            showDetailsView();
        } else {
            showIconsView();
        }

        repositionWBExplorerTabs();

        $("#wbf-wb-explorer-tabs > ul > li > a > img").tooltip();
        $("#wbf-wb-explorer-tabs > ul > li > a > img").click(function() {
            $(this).tooltip("close");
        });

        $("#teamSearch").blur(function () {
            if ($(this).val() == "") { $(this).val("Search this team's work boxes"); }
        }).focus(function () {
            if ($(this).val() == "Search this team's work boxes") { $(this).val(""); }
        });

    }

</script>

<style type="text/css">
.wbf-filter-selected { font-weight: bold; }

table.wbf-dialog-form
{
    width: 300px;
    min-width: 300px;
}

.wbf-field-name-panel 
{
    width: 50px; 
}

.wbf-field-value-panel 
{ 
    width: 150px; 
}

#wbf-columns .wbf-field-name-panel 
{
    width: 150px; 
}

#wbf-columns .wbf-field-value-panel 
{ 
    width: 50px; 
}

.ui-tooltip {
	padding: 8px;
	position: absolute;
	z-index: 9999;
	max-width: 300px;
	-webkit-box-shadow: 0 0 5px #aaa;
	box-shadow: 0 0 5px #aaa;
	border: 1px solid #aaaaaa;
	background: #ffffff;
	color: #222222;
	font-family: Verdana,Arial,sans-serif;
	font-size: 1.1em;
}


td.ms-vb2 
{
    padding-top: 0px;
    padding-bottom: 4px;
    vertical-align: middle;
}

td.ms-vb2 img { 
  vertical-align: middle;
}

.ms-rte-layoutszone-inner 
{
    padding: 0px;
    overflow: visible;
}

h3.lbi-council-wide-business {
    background-color: #007229;
}

h3.lbi-team-admin {
    background-color: #009ACF;
}

div.wbf-wb-explorer-update-panel
{
   font-size: 10pt !important;
}

div#wbf-wb-explorer-tabs 
{
   position: relative;
   top: -68px;
}

#wbf-wb-explorer-tabs .ms-rte-wpbox 
{
    color: #3b4f65 !important;
    font-size: 10pt !important;
}

#wbf-wb-explorer-tabs p 
{
    font-size: 10pt !important;
    padding-left: 15px;
}

div.wbf-records-type-tree-nav a 
{
    padding: 0px;
}

div.wbf-records-type-tree-nav 
{
    padding: 6px;
}

.wbf-wb-explorer-gridview 
{
    font-size: 10pt !important;
}

h3.wbf-tab-dialog-header 
{
    border-bottom-width: 2px;
    border-bottom-style: solid;
    border-bottom-color: #fff;
    padding: 8px 5px 12px 10px;
    margin: 0;
    font-weight: bold !important;
    font-size: 14pt !important;
    background-color: #fff;
    margin-top: 10px;
    color: #581E54;
}

h4.wbf-tab-dialog-sub-header 
{
    border-bottom-width: 2px;
    border-bottom-style: solid;
    border-bottom-color: #fff;
    padding: 4px 5px 4px 10px;
    margin: 0;
    font-weight: bold !important;
    font-size: 10pt !important;
    background-color: #fff;
    color: #581E54;
}

.wbf-tabs-dialog 
{
    border: 1px solid grey;
}

#wbf-add-new div h3.wbf-leftpanel-header 
{
    font-size: 1.1em;
}

#teamSearch, #searchAgain 
{
    color: #555;
    width: 200px;
}

#wbf-wb-explorer-refinement-panel .ms-searchref-main 
{
    background-image: none !important;
}

p.srch-Metadata1 
{
    margin: 0px 0px 20px 24px !important;
}


</style>

<div style="display: none;" class="wbf-hidden-submit-button">

<asp:LinkButton ID="HiddenSubmitLink" Text="Reload" OnClick="HiddenSubmitLink_OnClick" runat="server" />
<asp:HiddenField ID="HiddenRecordsTypeGUIDField" Value="" runat="server" />
<asp:HiddenField ID="HiddenViewStyleField" runat="server" />

</div>

<div class="wbf-wb-explorer-update-panel">
<asp:UpdatePanel ID="ShowSelectionPanel" runat="server" UpdateMode="Always">

    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="HiddenSubmitLink" EventName="Click" />
    </Triggers>

    <ContentTemplate>
<div class="wbf-wb-explorer-updating-panel">

<table border="0" cellpadding="0" cellspacing="0" width="100%">
<tr>
<td valign="top">

<div id="wbf-wb-explorer-details-view" style="display: none;" >
<div class="wbf-wb-explorer-gridview">
    <SharePoint:SPGridView runat="server" ID="SelectedWorkBoxes" AutoGenerateColumns="false">
        <EmptyDataTemplate>
            <span class="wbf-wb-explorer-no-work-boxes"><%= NoWorkBoxesText %></span>                                    
        </EmptyDataTemplate>
    </SharePoint:SPGridView>        

</div>
</div>


<div id="wbf-wb-explorer-icon-view" style="display: none;" >

<asp:Literal ID="IconViewLiteral" runat="server" />

</div>


<div id="wbf-wb-explorer-search-results" style="display: none;" >

  <table style="margin:0px; padding:0px;">

<tr>
<td colspan="2">

<input class="searchBox" type="text" name="searchAgain" id="searchAgain" value="Search this team's work boxes" accesskey="T"
            onkeydown="WorkBoxFramework__search__KeyDown(event, '', 'searchAgain', '<%=RefinementByOwningTeam %>', '<%=SearchScope %>')" />
<input class="submitSearch" type="button" value="Search" onclick="WorkBoxFramework__doRefinedSearch('', 'searchAgain', '<%=RefinementByOwningTeam %>', '<%=SearchScope %>')" /><br />

</td>
</tr>
</table>


<Search:SearchStatsWebPart ID="SearchStatsWebPart" runat="server" />

<Search:CoreResultsWebPart ID="CoreResultsWebPart" runat="server" />

<Search:SearchPagingWebPart ID="SearchPagingWebPart" runat="server" />

</div>


</td>

<td id="wbf-wb-explorer-tabs-column" width="305px" valign="top">
  
<% if (!InEditMode)
   { %>

<div id="wbf-wb-explorer-tabs">
  <ul>
    <li><a href="#" onclick="toggleViewStyle();" id="wbf-anchor-for-changing-view-style"><img id="wbf-image-for-changing-view-style" src="/_layouts/images/WorkBoxFramework/details-view-32.png" title="Change to details view" alt="Change to details view"/></a></li>
    <li id="wbf-filter-icon-list-item"><a href="#wbf-filter-by"><img src="/_layouts/images/WorkBoxFramework/filter-icon-32.png" title="Ordering and filtering of view" alt="Ordering and filtering of view"/></a></li>
    <li id="wbf-columns-icon-list-item"><a href="#wbf-columns"><img src="/_layouts/images/WorkBoxFramework/columns-icon-32.png" title="Choose which columns to display" alt="Choose which columns to display"/></a></li>
    <li><a href="#wbf-search-work-boxes" onclick="showResultsIfDoingQuery();"><img src="/_layouts/images/WorkBoxFramework/search-icon-32.png" title="Search team's work boxes" alt="Search team's work boxes"/></a></li>
    <li><a href="#wbf-add-new"><img src="/_layouts/images/WorkBoxFramework/plus-icon-32.png" title="Create a new work box" alt="Create a new work box"/></a></li>
  </ul>

  
  <div id="wbf-filter-by" class="wbf-tabs-dialog">
  
      <h3 class="wbf-tab-dialog-header">Ordering and filtering</h3>

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<h4 class="wbf-tab-dialog-sub-header">Order by:</h4>
</td>
<td class="wbf-field-name-panel">
</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Column:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:DropDownList ID="OrderBy" CssClass="wbf-drop-down" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Direction:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:DropDownList ID="AscendingDescendingChoice" CssClass="wbf-drop-down" runat="server" />
</div>

</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
<h4 class="wbf-tab-dialog-sub-header">Filter by:</h4>
</td>
<td class="wbf-field-name-panel">
</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Status:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:DropDownList ID="StatusFilter" CssClass="wbf-drop-down" runat="server" />
</div>

<div class="wbf-field-error">
<asp:Label ID="StatusFilterError" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Involvement:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:DropDownList ID="InvolvementFilter" CssClass="wbf-drop-down" runat="server" />
</div>

<div class="wbf-field-error">
<asp:Label ID="InvolvementFilterError" runat="server" Text="" ForeColor="Red"/>
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Records Type:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<div class="wbf-records-type-selected">
<asp:Label ID="RecordsTypeSelected" Text="" runat="server" />
</div>

<div class="wbf-records-type-description">
    <asp:Label ID="RecordsTypeDescription" Text="" runat="server" />
</div>

<a href="#" onclick="javascript: WorkBoxFramework_triggerWebPartUpdate(''); ">Remove</a> &nbsp; | &nbsp; <a href="#" onclick="javascript: $('#wbf-select-records-type').toggle(); ">Change</a>
</div>

</td>
</tr>

<tr>
<td colspan="2">

<div id="wbf-select-records-type" style="display: none;">

<h4 class="wbf-tab-dialog-sub-header">Select which records type to filter on:</h4>


                          <div class="lbi-team-admin">
                            <h3 class="wbf-leftpanel-header lbi-team-admin">
                                Team admin</h3>

    <div class="wbf-records-type-tree-nav">

      <SharePoint:SPTreeView
        id="TeamAdminRecordsTypesFilter"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

        <% if (NotSetupText != null & NotSetupText != "")
           { %>
<p><%=NotSetupText%></p>           
<% } %>

    </div>

                        </div>
                        <div class="lbi-our-work">
                            <h3 class="wbf-leftpanel-header lbi-our-work">
                                Our work</h3>

    <div class="wbf-records-type-tree-nav">

      <SharePoint:SPTreeView
        id="OurWorkRecordsTypesFilter"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

        <% if (NotSetupText != null & NotSetupText != "")
           { %>
<p><%=NotSetupText%></p>           
<% } %>

    </div>

                        </div>
                        <div class="lbi-council-wide-business">
                            <h3 class="wbf-leftpanel-header lbi-council-wide-business">
                                Council-wide business</h3>


    <div class="wbf-records-type-tree-nav">

      <SharePoint:SPTreeView
        id="CouncilWideRecordsTypesFilter"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

        <% if (NotSetupText != null & NotSetupText != "")
           { %>
<p><%=NotSetupText%></p>           
<% } %>

    </div>

                        </div>
</div>

</td>
</tr>


<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="UpdateViewFromFilters" runat="server" Text="Update View"  OnClick="UpdateView_OnClick"/>
</td>
</tr>


</table>

  </div>
  
  <div id="wbf-columns" class="wbf-tabs-dialog">

    <h3 class="wbf-tab-dialog-header">Select columns to view</h3>

<table class="wbf-dialog-form">

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Title:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="TitleCheckBox" Checked="True" Enabled="false" runat="server" /> (always required)
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Work Box Status:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="StatusCheckBox" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Records Type:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="RecordsTypeCheckBox" runat="server" />
</div>

</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Last Modified (approx):</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="LastModifiedCheckBox" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Last Visited (approx):</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="LastVisitedCheckBox" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Date Created:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="DateCreatedCheckBox" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Reference Date:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="ReferenceDateCheckBox" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Reference ID:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="ReferenceIDCheckBox" runat="server" />
</div>

</td>
</tr>



<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Owning Team:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="OwningTeamCheckBox" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Involved Teams:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="InvolvedTeamsCheckBox" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Visiting Teams:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="VisitingTeamsCheckBox" runat="server" />
</div>

</td>
</tr>


<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Involved Individuals:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="InvolvedIndividualsCheckBox" runat="server" />
</div>

</td>
</tr>

<tr>
<td class="wbf-field-name-panel">
<div class="wbf-field-name">Visiting Individuals:</div>
</td>
<td class="wbf-field-value-panel">

<div class="wbf-field-value">
<asp:CheckBox ID="VisitingIndividualsCheckBox" runat="server" />
</div>


</td>
</tr>



<tr>
<td colspan="2" class="wbf-buttons-panel">
    <asp:Button ID="UpdateViewFromColumns" runat="server" Text="Update View"  OnClick="UpdateView_OnClick"/>
</td>
</tr>



</table>
  </div>

  <div id="wbf-search-work-boxes" class="wbf-tabs-dialog">
  
  <h3 class="wbf-tab-dialog-header">Search team's work boxes</h3>

  <table class="wbf-dialog-form" style="margin:0px; padding:0px;">

<tr>
<td class="wbf-field-name-panel" colspan="2">

<input class="searchBox" type="text" name="teamSearch" id="teamSearch" value="Search this team's work boxes" accesskey="T"
            onkeydown="WorkBoxFramework__search__KeyDown(event, '', 'teamSearch', '<%=RefinementByOwningTeam %>', '<%=SearchScope %>')" />
<input class="submitSearch" type="button" value="Search" onclick="WorkBoxFramework__doRefinedSearch('', 'teamSearch', '<%=RefinementByOwningTeam %>', '<%=SearchScope %>')" /><br />

</td>
</tr>
</table>

<div id="wbf-wb-explorer-refinement-panel" style="display: none;" >

<Search:RefinementWebPart ID="RefinementWebPart" runat="server" />

</div>

  </div>


  <div id="wbf-add-new" class="wbf-tabs-dialog">

  <h3 class="wbf-tab-dialog-header">Create new work boxes</h3>

  <table class="wbf-dialog-form" style="margin:0px; padding:0px;">

<tr>
<td class="wbf-field-name-panel" colspan="2">
  <p>
    Select which type of work box to create:
  </p>
</td>
</tr>
</table>


                          <div class="lbi-team-admin">
                            <h3 class="wbf-leftpanel-header lbi-team-admin">
                                Team admin</h3>

    <div class="wbf-records-type-tree-nav">

      <SharePoint:SPTreeView
        id="TeamAdminRecordsTypesTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

        <% if (NotSetupText != null & NotSetupText != "")
           { %>
<p><%=NotSetupText%></p>           
<% } %>

    </div>

                        </div>
                        <div class="lbi-our-work">
                            <h3 class="wbf-leftpanel-header lbi-our-work">
                                Our work</h3>

    <div class="wbf-records-type-tree-nav">

      <SharePoint:SPTreeView
        id="OurWorkRecordsTypesTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

        <% if (NotSetupText != null & NotSetupText != "")
           { %>
<p><%=NotSetupText%></p>           
<% } %>

    </div>

                        </div>
                        <div class="lbi-council-wide-business">
                            <h3 class="wbf-leftpanel-header lbi-council-wide-business">
                                Council-wide business</h3>


    <div class="wbf-records-type-tree-nav">

      <SharePoint:SPTreeView
        id="CouncilWideRecordsTypesTreeView"
        UseInternalDataBindings="false"
        runat="server"
        ShowLines="true"
        SelectedNodeStyle-CssClass="ms-tvselected"
        NodeStyle-CssClass="ms-navitem"
        NodeStyle-HorizontalPadding="0"
        NodeStyle-VerticalPadding="0"
        NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
        SkipLinkText=""
        NodeIndent="20"/>

        <% if (NotSetupText != null & NotSetupText != "")
           { %>
<p><%=NotSetupText%></p>           
<% } %>

    </div>

                        </div>


  </div>

</div>

<!-- Closing bracket for the 'if not in edit mode' conditional -->
<% } %>

</td>
</tr>

</table>

</div>

    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    aspPanelHasUpdated();

    if (wbf__request_has_query_keywords) {
        $("#searchAgain").val(WorkBoxFramework_getParameterByName("k"));
        $("#teamSearch").val(WorkBoxFramework_getParameterByName("k"));
        $("#wbf-wb-explorer-tabs").tabs({ active: 3 });
        showSearchResultsView();
    }

    $(function () {
        repositionWBExplorerTabs();
    });

    $(window).resize(function () {
        repositionWBExplorerTabs();
    });

</script>

</div>

