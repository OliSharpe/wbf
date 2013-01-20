<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DisplaySelectedTeamRecordsUserControl.ascx.cs" Inherits="WorkBoxFramework.DisplaySelectedTeamRecords.DisplaySelectedTeamRecordsUserControl" %>


<div class="wbf-selected-team-records-web-part">
<div class="wbf-view-selected-records-type-info">
<asp:Literal ID="InformationText" runat="server" />
</div>
<div class="wbf-selected-team-records">
                                <SharePoint:SPGridView runat="server" ID="ShowCombinedResults" AutoGenerateColumns="false">
                                  <Columns>
                                    <asp:ImageField HeaderText="" DataImageUrlField = "Icon" DataImageUrlFormatString = "{0}" />
                                    <asp:HyperLinkField HeaderText="Title"
                                                        DataTextField="Title"
                                                        DataNavigateUrlFormatString="{0}"
                                                        DataNavigateUrlFields="URL" />
                                    <asp:BoundField HeaderText="Records Type" 
                                            DataField="RecordsType" 
                                            SortExpression="RecordsType" 
                                            >
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Date Created" 
                                            DataField="WorkBoxDateCreated" 
                                            SortExpression="WorkBoxDateCreated" 
                                            DataFormatString="{0:dd/MM/yyyy}" htmlencode="false">
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Status" 
                                            DataField="WorkBoxStatus" 
                                            SortExpression="WorkBoxStatus" 
                                            >
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>


                                  </Columns>
                                  <EmptyDataTemplate>
                                    <i>No results</i>                                    
                                  </EmptyDataTemplate>
                                </SharePoint:SPGridView>     
</div>
<div class="wbf-create-new-work-box-link">

<asp:Literal ID="CreateNewWorkBoxLink" runat="server"></asp:Literal>

</div>


</div>

                                <!--
                                <div>
<asp:Label ID="WorkBoxCollectionQuery" runat="server" Text="Label"></asp:Label>
                                </div>
<div>
<asp:Label ID="RecordsLibraryQuery" runat="server" Text="Label"></asp:Label>
</div>
-->
