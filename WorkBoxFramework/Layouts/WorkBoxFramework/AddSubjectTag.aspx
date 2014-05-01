<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubjectTag.aspx.cs" Inherits="WorkBoxFramework.Layouts.WorkBoxFramework.AddSubjectTag" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:CssRegistration ID="wbfCssSectionTags" Name="WorkBoxFramework/css/SubjectTags.css" After="corev4.css" runat="server" />
    <script src="jquery-1.7.2.min.js" type="text/javascript"></script>
    <SharePoint:ScriptLink ID="subjectTagsScriptLink" Name="/_layouts/WorkBoxFramework/SubjectTags.js" runat="server" OnDemand="false" Localizable="false" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


<div class="wbf-add-tag-wrapper">
    <%--<h2>Add Subject Tag</h2>--%>

    <table class="ms-formtable wbf-subject-tag-form" cellpadding="0" cellspacing="0">
        <tbody>
            <tr>
                <td class="ms-formlabel" valign="top">
                    <h3 class="ms-standardheader"><%= CreateNew ? "Create tag in" : "Edit tag <span style='color:red'>*</span>" %></h3>
                    <%if (!CreateNew)
                      {%>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ErrorMessage="Required" ControlToValidate="txtEdit_CurrentTagName" Display="Dynamic" InitialValue="" ForeColor="Red" runat="server" />
                    <%} %>
                </td>
                <td class="ms-formbody" valign="top">
                    <h4 class="wbf-form-lbl">
                        <asp:Label Text="" ID="lblMMSPath" runat="server"/>
                        <% if (!CreateNew)
                           { %>
                           <asp:TextBox runat="server" ID="txtEdit_CurrentTagName" style="padding: 4px; border:1px solid #828790;" CssClass="wbf-text"></asp:TextBox>
                        <%} %>
                    </h4>
                    <%= CreateNew ? "<em>Your new tag will be created as a child of this location</em>" : ""%>
                </td>
            </tr>
            <tr style='<%= CreateNew ? "" : "display: none;" %>'>
                <td class="ms-formlabel" valign="top">
                    <h3 class="ms-standardheader">Subject Tag Name <span style='color:red'>*</span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" ErrorMessage="Required" ControlToValidate="txtTagName" Display="Static" InitialValue="" ForeColor="Red" runat="server" /></h3>
                </td>
                <td class="ms-formbody" valign="top">
                    <span>
                        <SharePoint:InputFormTextBox ID="txtTagName" RichText="false" TextMode="SingleLine" MaxLength="255" CssClass="ms-long ms-spellcheck-true" runat="server" style="padding: 4px; border:1px solid #828790;" />
                    </span>
                    <br />
                    <em>The name you want to give your new tag</em>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" valign="top">
                    <h3 class="ms-standardheader">Description</h3>
                </td>
                <td class="ms-formbody" valign="top">
                    <span>
                        <PublishingWebControls:HtmlEditor runat="server" ID="htmlDescription" />
                    </span>
                    <em>Give a full description of the tag</em>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" valign="top">
                    <h3 class="ms-standardheader">Internal Contact</h3>
                </td>
                <td class="ms-formbody" valign="top">
                    <span>
                        <SharePoint:PeopleEditor ID="ppInternalContact" runat="server" CssClass="ms-long" SelectionSet="User" MultiSelect="false" />
                    </span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" valign="top">
                    <h3 class="ms-standardheader">Additional Information</h3>
                </td>
                <td class="ms-formbody" valign="top">
                    <span>
                        <PublishingWebControls:HtmlEditor runat="server" ID="htmlExternalContact" />
                    </span>
                    <%= CreateNew ? "<em>Use the template provided to populate the details of the external contact.</em>" : ""%>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblValidationMessage" runat="server"></asp:Label>
                </td>
            </tr>
        </tbody>
    </table>

    <!-- Dialog Toolbar -->
    <table class="ms-formtoolbar" cellpadding="2" cellspacing="0" border="0" width="100%" style="margin-top:3px;">
        <tbody>
            <tr>
                <td width="99%" class="ms-toolbar" nowrap="nowrap">
                    <img src="/_layouts/images/blank.gif" width="1" height="18" alt="">
                </td>
                <td class="ms-toolbar" nowrap="nowrap">
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tbody>
                            <tr>
                                <td align="right" width="100%" nowrap="nowrap">
                                    <asp:Button Text="Save" ID="btnAdd" runat="server" OnClick="addButton_OnClick" ToolTip="Save" Font-Size="12px" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td class="ms-separator">
                    &nbsp;
                </td>
                <td class="ms-toolbar" nowrap="nowrap">
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tbody>
                            <tr>
                                <td align="right" width="100%" nowrap="nowrap">
                                    <asp:Button Text="Cancel" runat="server" ID="btnCancel" OnClientClick="Tags.CloseDialog(this);return false;" ToolTip="Cancel without saving" Font-Size="12px" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    </div>

    <!--
    

    -->

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Add Subject Tag Term
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Add Subject Tag Term
</asp:Content>
