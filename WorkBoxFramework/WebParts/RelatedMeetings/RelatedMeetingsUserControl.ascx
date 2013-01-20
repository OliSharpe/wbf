<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedMeetingsUserControl.ascx.cs" Inherits="WorkBoxFramework.RelatedMeetings.RelatedMeetingsUserControl" %>

<div>
                                <SharePoint:SPGridView runat="server" ID="RelatedMeetings" AutoGenerateColumns="false">
                                  <Columns>
                                    <asp:ImageField HeaderText="" DataImageUrlField = "Icon" DataImageUrlFormatString = "{0}" />
                                    <asp:BoundField HeaderText="Meeting Date" 
                                            DataField="Reference Date" 
                                            SortExpression="Reference Date" 
                                            DataFormatString="{0:dd/MM/yyyy}" htmlencode="false">
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:HyperLinkField HeaderText="Title"
                                                        DataTextField="Title"
                                                        DataNavigateUrlFormatString="{0}"
                                                        DataNavigateUrlFields="URL" />

                                  </Columns>
                                  <EmptyDataTemplate>
                                    <i>No related meetings</i>                                    
                                  </EmptyDataTemplate>
                                </SharePoint:SPGridView>     
                                </div>

                                <asp:Literal ID="CreateNewMeetingLink" runat="server"></asp:Literal>


