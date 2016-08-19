<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="LinkSection" src="/_controltemplates/LinkSection.ascx" %> 
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/ButtonSection.ascx" %> 
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkBoxCollectionSettingsPage.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.WorkBoxPortalSettingsPage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

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

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">


    <!-- Work Box Collection Administrator User Groups Section -->
<wssuc:InputFormSection
	id="WorkBoxCollectionAdministratorsSection"
	title="Work Box Collection Administrators"
	Description="Select the SharePoint user groups that need to have administrators rights over this work box collection."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<Taxonomy:TaxonomyWebTaggingControl  ID="SystemAdminTeams" ControlMode="display" runat="server"/>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							System Administrator Teams
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<Taxonomy:TaxonomyWebTaggingControl  ID="BusinessAdminTeams" ControlMode="display" runat="server"/>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Business Administrator Teams
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>




<!-- Name Of All Work Boxes List Section -->
<wssuc:InputFormSection
	id="NameOfAllWorkBoxesListSection"
	title="Name of the 'Work Boxes in Collection' list" 
	Description="Enter the name of the list that contains the definitive list of all the work boxes in the collection."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:TextBox ID="NameOfAllWorkBoxesList" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							<asp:Label ID="EventReceiverStatus" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Work Box Collection Unique ID Section -->
<wssuc:InputFormSection
	id="WorkBoxCollectionUniqueIdSection"
	title="Work Box Collection Unique IDs"
	Description="Enter the details of how unique IDs should be managed for this work box collection."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:TextBox ID="WorkBoxCollectionUniqueIdPrefix" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Unique ID Prefix (e.g. 'FOI')
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="right" width="50">
                           <asp:CheckBox ID="GenerateUniqueIds" runat="server"></asp:CheckBox> 
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Generate Unique IDs
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                           <asp:TextBox ID="NumberOfDigitsInIds" runat="server"></asp:TextBox> 
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Number of Digits in IDs
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                           <asp:TextBox ID="InitialIdOffset" runat="server"></asp:TextBox> 
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Initial ID Offset
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Who can create and close -->
<wssuc:InputFormSection
	id="ExtraPermissions"
	title="Extra Permissions"
	Description="Set the extra permissions behaviour for all work boxes in this work box collection."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:CheckBox ID="CanAnyoneCreate" runat="server"></asp:CheckBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Can anyone create?
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>

<!-- Set Permission Levels -->
<wssuc:InputFormSection
	id="SetPermissionLevels"
	title="Set Permission Levels"
	Description="Enter the permission levels to use for various roles."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" colspan="3">
                            <h3 class="ms-standardheader ms-inputformheader">
                            Open Permission Levels:
                            </h3>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							System Admin
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="SysadminOpen" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Business Admin
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="AdminOpen" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Owner(s)
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="OwnerOpen" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Involved
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="InvolvedOpen" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Visitors
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="VisitorsOpen" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Everyone
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="EveryoneOpen" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" colspan="3">
                            <h3 class="ms-standardheader ms-inputformheader">
                            Closed Permission Levels:
                            </h3>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							System Admin
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="SysadminClosed" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Business Admin
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="AdminClosed" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Owner(s)
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="OwnerClosed" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Involved
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="InvolvedClosed" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Visitors
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="VisitorsClosed" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td nowrap="nowrap" class="ms-authoringcontrols"  align="left">
							Everyone
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td class="ms-authoringcontrols" valign="top" align="left" >
                            <asp:TextBox ID="EveryoneClosed" runat="server"></asp:TextBox>
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Work Box Collection Folder Access Groups Pattern Section -->
<wssuc:InputFormSection
	id="FolderAccessGroupPattern"
	title="Use Folder Access Groups Pattern"
	Description="Should the top level folders in the work boxes use the folder access groups pattern?"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:CheckBox ID="UseFolderAccessGroupsPattern" runat="server"></asp:CheckBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Use folder access groups pattern?
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:TextBox ID="FolderAccessGroupsPrefix" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Folder Access Groups' Prefix
                            <br />(Group names are of the form: [Prefix] - [Folder Name]  )
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:TextBox ID="FolderAccessGroupsFolderNames" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Folder Access Groups' Folder Names 
                            <br />(separate folder names using semi-colons ';' )
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="FolderAccessGroupPermissionLevel" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Folder Access Group Permission Level
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
                            <asp:TextBox ID="AllFoldersAccessGroupPermissionLevel" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							All Folders Access Group Permission Level
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>






<!-- Work Box Collection Dialog Forms Section -->
<wssuc:InputFormSection
	id="CreateNewWorkBoxAction"
	title="Create New Work Box Action"
	Description="Enter the URL and link text for the 'create new' action."
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:TextBox ID="NewWorkBoxDialogUrl" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							New Work Box Dialog URL
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:TextBox ID="CreateNewWorkBoxText" runat="server"></asp:TextBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Create New Work Box Text
						</td>
					</tr>
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="left" width="50">
			<Taxonomy:TaxonomyWebTaggingControl  ID="DefaultOwningTeam" ControlMode="display" runat="server"/>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Default Owning Team
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>


<!-- Work Box Collection Uses Linked Calendars Section -->
<wssuc:InputFormSection
	id="UsesLinkedCalendarsSection"
	title="Uses Linked Calendars"
	Description="Does this work box collection use linked calendars?"
	runat="server"
	>
	<Template_InputFormControls>
		<wssuc:InputFormControl runat="server">
			<Template_Control>
				<table border="0" width="100%" cellspacing="0" cellpadding="2">
					<tr>
						<td class="ms-authoringcontrols" valign="top" align="center" width="50">
                            <asp:CheckBox ID="UsesLinkedCalendars" runat="server"></asp:CheckBox>
						</td>
						<td class="ms-authoringcontrols" width="10"></td>
						<td nowrap="nowrap" class="ms-authoringcontrols" width="100%">
							Uses Linked Calendars?
						</td>
					</tr>
				</table>
			</Template_Control>
		</wssuc:InputFormControl>
	</Template_InputFormControls>
</wssuc:InputFormSection>








<!-- Buttons Section -->
<wssuc:ButtonSection runat="server" ShowStandardCancelButton="false">
	<Template_Buttons>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="okButton_OnClick" Text="<%$Resources:wss,multipages_okbutton_text%>" id="okButton" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
		<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="cancelButton_OnClick" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="cancelButton" accesskey="<%$Resources:wss,cancelbutton_accesskey%>"/>
	</Template_Buttons>
</wssuc:ButtonSection>


	</table> 

<p>                           
<asp:Label ID="CurrentProperties" runat="server" Text=""></asp:Label> 
</p>

<asp:HiddenField ID="ReturnUrl" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Work Box Portal Settings Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Work Box Portal Settings Page
</asp:Content>
