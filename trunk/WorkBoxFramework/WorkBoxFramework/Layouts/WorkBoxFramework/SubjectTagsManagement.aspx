<%@ Assembly Name="WorkBoxFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4554acfc19d83350" %>
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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectTagsManagement.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.SubjectTagsManagement" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <SharePoint:CssRegistration ID="WBFCssRegistration"
      name="WorkBoxFramework/css/WBF.css" 
      After="corev4.css"
      runat="server"
    />

    <SharePoint:ScriptLink ID="JqueryScriptRegistration"
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

     <style type="text/css">
        BODY #s4-leftpanel { width: 0 !important; display:none; }
        .s4-ca { margin-left: 0 !important; }
        .wbf-metadata-title { padding: 5px; font-weight: bold; }
        .wbf-metadata-details { padding: 2px; margin-left: 15px; }
        .wbf-details-panel { padding: 2px; }
        td.wbf-management-title-panel { padding: 2px; border: 1px solid gray; background-color: #ebf3ff; text-align: center; height: 25px; }
        td.wbf-management-panel { padding: 2px; border: 1px solid gray; background-color: #ebf3ff; width:100%;}
        td.wbf-management-selector-panel { padding: 2px; padding-left:10px; border: 1px solid gray; background-color: #fff; height:25px; max-height:25px; }
        td.wbf-management-details-title-panel {  text-align:center; background-color: #e0e0e0;  }
        .wbf-management-title { }
        td.ms-authoringcontrols { border-left: 1px solid gray; }
        td.ms-authoringcontrols td { border: 0px; }
        td.ms-descriptiontext { width: 150px; }
        td.ms-inputformcontrols { width: auto; min-width: 375px; }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="wbf-application-page">
        <table cellspacing="6">
            <tr>
                <td colspan="2" class="wbf-management-title-panel">
                    <h1 class="wbf-management-title">
                        Subject Tag Management</h1>
                </td>
            </tr>
            <tr>
                <td valign="top" width="100px" class="wbf-management-selector-panel">
                    <div>
                        <h3>
                            Select Subject Tag</h3>
                        <SharePoint:SPRememberScroll ID="MyTreeViewRememberScroll" runat="server" onscroll="javascript:_spRecordScrollPositions(this);"
                            Style="overflow: auto; height: 600px; min-height:350px; width: 300px;" CssClass="sp-scroll">
                            <SharePoint:SPTreeView ID="tvAllSubjectTags" UseInternalDataBindings="false"
                                runat="server" ShowLines="true" ExpandDepth="1" SelectedNodeStyle-CssClass="ms-tvselected"
                                NodeStyle-CssClass="ms-navitem"
                                NodeStyle-HorizontalPadding="0" NodeStyle-VerticalPadding="0" NodeStyle-ImageUrl="/_layouts/Images/EMMTerm.png"
                                SkipLinkText="" NodeIndent="20" ExpandImageUrl="/_layouts/images/tvplus.gif"
                                CollapseImageUrl="/_layouts/images/tvminus.gif" NoExpandImageUrl="/_layouts/images/tvblank.gif" />
                        </SharePoint:SPRememberScroll>
                    </div>
                </td>
                <td valign="top" class="wbf-management-panel">
                    <asp:UpdatePanel ID="ShowSelectionPanel" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tvAllSubjectTags" EventName="SelectedNodeChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div>
                                <table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="ms-sectionline" height="1" colspan="2">
                                            <img src="/_layouts/images/blank.gif" width='1' height='1' alt="" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="wbf-management-details-title-panel">
                                            <h2>
                                                Subject Tag Details</h2>
                                        </td>
                                    </tr>

                                    <!-- Permission Groups -->
                                    <wssuc:InputFormSection
	                                    id="PermissionGroupsSection"
	                                    title="Teams with permission"
	                                    Description="Teams that have permissions to edit this tag and create new child tags"
	                                    runat="server"
	                                    >
	                                    <Template_InputFormControls>
		                                    <wssuc:InputFormControl runat="server">
			                                    <Template_Control>
                                                        <div class="wbf-details-panel">
                                                            <Taxonomy:TaxonomyWebTaggingControl ID="taxTeams" ControlMode="display" runat="server" />
                                                            <%--<SharePoint:PeopleEditor ID="ppPermissionGroups" runat="server" CssClass="ms-long" SelectionSet="SPGroup" MultiSelect="true" />--%>
				                                        </div>
                                                        <%--<div class="wbf-details-panel">
                                                            <em>Select the groups that can create and edit terms within this subject tag</em>
				                                        </div>--%>
			                                    </Template_Control>
		                                    </wssuc:InputFormControl>
	                                    </Template_InputFormControls>
                                    </wssuc:InputFormSection>

                                    <!-- Page Content -->
                                    <wssuc:InputFormSection
	                                    id="PageContentSection"
	                                    title="Page Content"
	                                    Description=""
	                                    runat="server"
	                                    >
	                                    <Template_InputFormControls>
		                                    <wssuc:InputFormControl runat="server">
			                                    <Template_Control>
                                                        <div class="wbf-details-panel">
                                                            <asp:literal ID="litPageContent" text="" runat="server" />
				                                        </div>
			                                    </Template_Control>
		                                    </wssuc:InputFormControl>
	                                    </Template_InputFormControls>
                                    </wssuc:InputFormSection>

                                    <!-- Internal Contact -->
                                    <wssuc:InputFormSection
	                                    id="InternalContactSection"
	                                    title="Internal Contact"
	                                    Description=""
	                                    runat="server"
	                                    >
	                                    <Template_InputFormControls>
		                                    <wssuc:InputFormControl runat="server">
			                                    <Template_Control>
                                                        <div class="wbf-details-panel">
                                                            <asp:literal ID="litInternalContact" text="" runat="server" />
				                                        </div>
			                                    </Template_Control>
		                                    </wssuc:InputFormControl>
	                                    </Template_InputFormControls>
                                    </wssuc:InputFormSection>

                                    <!-- External Contact -->
                                    <wssuc:InputFormSection
	                                    id="ExternalContactSection"
	                                    title="External Contact"
	                                    Description=""
	                                    runat="server"
	                                    >
	                                    <Template_InputFormControls>
		                                    <wssuc:InputFormControl runat="server">
			                                    <Template_Control>
                                                        <div class="wbf-details-panel">
                                                            <asp:literal ID="litExternalContact" text="" runat="server" />
				                                        </div>
			                                    </Template_Control>
		                                    </wssuc:InputFormControl>
	                                    </Template_InputFormControls>
                                    </wssuc:InputFormSection>


                                    <!-- Buttons Section -->
                                    <wssuc:ButtonSection runat="server" ShowStandardCancelButton="false">
	                                    <Template_Buttons>
                                            <asp:Label text="" id="lblPageMessage" runat="server" EnableViewState="false" />
		                                    <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" 
                                                OnClick="btnSave_Click" Text="Save Changes" id="btnSave" 
                                                accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
		                                    <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" 
                                                OnClick="btnCancel_Click" Text="Cancel Changes" id="btnCancel" 
                                                accesskey="<%$Resources:wss,cancelbutton_accesskey%>"/>
	                                    </Template_Buttons>
                                    </wssuc:ButtonSection>


                                </table>
                                
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        $(function () {
            //sp-scroll
            $(window).resize(function () {
                console.log("resized");
                $spremem = $(".sp-scroll");
                console.log($(".wbf-management-selector-panel").height());
            });
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
WBF - Subject Tag Management
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
WBF - Subject Tag Management
</asp:Content>
