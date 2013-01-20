#region Copyright and License

// Copyright (c) Islington Council 2010-2013
// Author: Oli Sharpe  (oli@gometa.co.uk)
//
// This file is part of the Work Box Framework.
//
// The Work Box Framework is free software: you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public License as  
// published by the Free Software Foundation, either version 2.1 of the 
// License, or (at your option) any later version.
//
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocPickWorkBoxFolder : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {
                ListGUID.Value = Request.QueryString["ListGUID"];
                ItemID.Value = Request.QueryString["ItemID"];

                // The following variable has its name due to a strange compliation error with the name 'DestinationType' 
                TheDestinationType.Value = Request.QueryString["DestinationType"];
                DestinationURL.Value = Request.QueryString["DestinationURL"];
                DestinationTitle.Text = Request.QueryString["DestinationTitle"];

                if (!string.IsNullOrEmpty(ListGUID.Value))
                {
                    Guid sourceListGuid = new Guid(ListGUID.Value);
                    SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)WorkBox.Web.Lists[sourceListGuid];

                    SPListItem sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));

                    if (sourceDocAsItem != null)
                    {
                        SourceDocIcon.AlternateText = "Icon of document being publishing out.";
                        SourceDocIcon.ImageUrl = SPUtility.ConcatUrls("/_layouts/images/",
                                                    SPUtility.MapToIcon(WorkBox.Web,
                                                    SPUtility.ConcatUrls(WorkBox.Web.Url, sourceDocAsItem.Url), "", IconSize.Size32));
                    }
                }

                using (WorkBox workBox = new WorkBox(DestinationURL.Value))
                {

                    SPFolder rootFolder = workBox.DocumentLibrary.RootFolder;

                    TreeViewFolderCollection collection = new TreeViewFolderCollection(rootFolder, "(root)");

                    WorkBoxFolders.DataSource = collection;
                    WorkBoxFolders.DataBind();
                }


                SelectedFolderPath.Text = "/";
            }


            // Now do a check that we do at this stage have the basic details of the document:
            if (ListGUID.Value == null || ListGUID.Value == "")
            {
                errorMessage += "ListGUID hasn't been set. ";
            }

            if (ItemID.Value == null || ItemID.Value == "")
            {
                errorMessage += "ItemID hasn't been set. ";
            }

            if (TheDestinationType.Value == null || TheDestinationType.Value == "")
            {
                errorMessage += "DestinationType hasn't been set. ";
            }

            if (errorMessage.Length > 0)
            {
                ErrorMessageLabel.Text = errorMessage;
                return;
            }


        }

        protected void WorkBoxFolders_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (WorkBoxFolders.SelectedNode != null)
            {
                string selectedPath = WorkBoxFolders.SelectedNode.ValuePath;
                if (string.IsNullOrEmpty(selectedPath)) selectedPath = "/";

                selectedPath = selectedPath.Replace("(root)", "");
                if (string.IsNullOrEmpty(selectedPath)) selectedPath = "/";

                SelectedFolderPath.Text = selectedPath;
            }
        }


        private void GoToMetadataPage(String destinationType, String destinationTitle, String destinationUrl)
        {
            string listGuid = ListGUID.Value;
            string itemID = ItemID.Value;

            string redirectUrl = "WorkBoxFramework/PublishDocDialogRequiredMetadataPage.aspx?ListGUID=" + listGuid + "&ItemID=" + itemID + "&DestinationURL=" + destinationUrl + "&DestinationTitle=" + destinationTitle + "&DestinationType=" + destinationType + "&SelectedFolderPath=" + SelectedFolderPath.Text;

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void nextButton_OnClick(object sender, EventArgs e)
        {
//            WorkBox caseFileWorkBox = WorkBox.Collection.FindByLocalID(LocalID.Text.Trim());

            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX,
            DestinationTitle.Text.Trim(),
            DestinationURL.Value.Trim());

            /*
            if (caseFileWorkBox != null)
            {
            }
            else
            {
                goToGenericOKPage("Counldn't find case file work box", "It was not possible to find the case file work box with local ID = " + LocalID.Text);
            }
             * */
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }


    }
}
