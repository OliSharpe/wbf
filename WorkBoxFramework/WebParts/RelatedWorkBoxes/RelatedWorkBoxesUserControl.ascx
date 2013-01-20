<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedWorkBoxesUserControl.ascx.cs" Inherits="WorkBoxFramework.RelatedWorkBoxes.RelatedWorkBoxesUserControl" %>

<script type="text/javascript">
    function WorkBoxFramework_PublishDoc_pickedAWorkBox(dialogResult, returnValue) {

        if (dialogResult == SP.UI.DialogResult.OK) {

            var values = returnValue.split(";");

            var workBoxURL = values[0];
            var workBoxTitle = values[1];

            var displaySelected = document.getElementById("selectedWorkBoxTitle");
            displaySelected.innerText = workBoxTitle;

            var destinationURL = document.getElementById("<%=DestinationURL.ClientID %>");
            destinationURL.value = workBoxURL;

            var destinationTitle = document.getElementById("<%=DestinationTitle.ClientID %>");
            destinationTitle.value = workBoxTitle;

            var destinationType = document.getElementById("<%=DestinationType.ClientID %>");
            destinationType.value = "Work Box";

        }

    }
</script>


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



<!--
<tr>
<td class="wbf-metadata-title-panel">
<b>Or</b> publish to a <b>Work Box</b>
<p></p>
</td>
<td class="wbf-metadata-value-panel" valign="middle"  align="center" width="200px">

<div style="padding: 5px;">
<span id="selectedWorkBoxTitle">(none selected)</span>
</div>
<div style="padding: 5px;">
<asp:LinkButton ID="SelectAWorkBox" runat="server" Text="Select a Work Box" OnClientClick="WorkBoxFramework_pickAWorkBox(WorkBoxFramework_PublishDoc_pickedAWorkBox); return false;" />
</div>
<div>
    <asp:Button ID="PublishToWorkBox" runat="server" Text="Work Box" OnClick="WorkBoxButton_onClick" />
</div>


</td>
</tr>
-->