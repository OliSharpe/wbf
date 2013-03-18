<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckConfiguration.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.CheckConfiguration" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<style type="text/css">
td.wbf-records-type { border: 0px; }
td.wbf-metadata-title-panel { width: 300px; padding: 8px; border-top:solid 1px grey; vertical-align: top; }
td.wbf-metadata-value-panel { width: 405px; padding: 8px; border-top:solid 1px grey; vertical-align: top; background-color: #f1f1f2;  }
td.wbf-buttons-panel { border-top:solid 1px grey; text-align: center; vertical-align: top; }
.wbf-metadata-title { font-weight: bold; padding-bottom: 2px; }
.wbf-metadata-description { font-weight: normal; padding: 2px; }
.wbf-metadata-read-only-value { font-weight: bold; padding: 2px; }
.wbf-metadata-error { font-weight: normal; padding: 0px; color: Red; }
td.wbf-create-new-title { padding: 6px; }
div.wbf-create-new-title { font-weight: bold; font-size: 16px; vertical-align: top; padding-bottom: 4px; }
table.wbf-title-table { padding: 6px 0px 12px 10px; }
.wbf-admin-page { padding: 10px }
</style>

<div class="wbf-admin-page">

<h2>Check Work Box Framework Configuration</h2>

<p>
The work box framework (WBF) depends on various components to be configured correctly. This page will help
both with the initial setup and (<b>one day!</b>) with on-going checking of the configuration.
</p>


<% if (doingInitialSetup)
   { %>

<h3>Initial Farm Setup</h3>

<p>
The initial farm setup will ensure that the basic setup for the Work Box Framework is in place. In particular it will:
</p>
<ul>
<li>Create necessary user profile properties</li>
<li>Create necessary managed metadata term sets</li>
<li>Setup the 'team sites' site collection</li>
<li>Setup the timer jobs lists in the system admin team site.</li>
<li>Setup the timer jobs</li>
</ul> 

<asp:Label ID="InitialSetupError" Text="" ForeColor="Red" />

<table width="100%" cellpadding="5" cellspacing="0">

<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">URL for system admin team site</div>
<div>
<p>
The site collection hosting this site will be used as the host site collection for all SharePoint groups that will be synchronised to other site collections.
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="AdminTeamSiteURL" Columns="50" runat="server" />
<asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="AdminTeamSiteURL" ErrorMessage="You must provide a URL for the admin team site" runat="server"/>

</div>

</td>
</tr>


<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Server to run timer jobs</div>
<div>
<p>
Give the name of the server on which you want the WBF timer jobs to be run.
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

<div class="wbf-metadata-read-only-value">
<asp:TextBox ID="TimerJobsServerName" Columns="50" runat="server" />
<asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="TimerJobsServerName" ErrorMessage="You must provide a server name" runat="server"/>

</div>

</td>
</tr>



<tr>
<td class="wbf-metadata-title-panel">
<div class="wbf-metadata-title">Managed Metadata Term Store</div>
<div>
<p>
Edit the name of the term store and group in which the WBF term sets are held if these are going to be different from the default.
</p>
</div>
</td>
<td class="wbf-metadata-value-panel">

				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
                        <td>
                            <b><nobr>Term Store Name</nobr></b><br />
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="TermStoreName" runat="server" columns="50"/>
                            <div>
                                <asp:RequiredFieldValidator ID="TermStoreNameValidator" runat="server" ErrorMessage="The WBF requires a term store name to function."
            ControlToValidate = "TermStoreName"></asp:RequiredFieldValidator>
                            </div>
                        </td>
                    </tr>
					<tr>
                        <td>
                            <b><nobr>Term Store Group Name</nobr></b><br />
                        </td>
						<td class="ms-authoringcontrols" valign="top" align="left">
                            <asp:TextBox ID="TermStoreGroupName" runat="server" columns="50"/>
                            <div>
                                <asp:RequiredFieldValidator ID="TermStoreGroupNameValidator" runat="server" ErrorMessage="The WBF requires a term store group name to function."
            ControlToValidate = "TermStoreGroupName"></asp:RequiredFieldValidator>
                            </div>
                        </td>
                    </tr>

				</table>

</td>
</tr>



<tr>
<td colspan="2" align="center" valign="top">
    <asp:Button ID="DoInitialSetup" runat="server" Text="Do Initial Setup"  OnClick="DoInitialSetup_OnClick"/>
&nbsp;
    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelButton_OnClick"/>

</td>
</tr>


</table>

<% } %>

</div>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Check WBF Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Check WBF Configuration
</asp:Content>
