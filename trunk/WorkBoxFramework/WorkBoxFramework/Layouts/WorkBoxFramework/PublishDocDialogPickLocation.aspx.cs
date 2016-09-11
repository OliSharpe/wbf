#region Copyright and License

// Copyright (c) Islington Council 2010-2016
// Author: Oli Sharpe  (oli@gometa.co.uk)
//
// This file is part of the Work Box Framework.
//
// The Work Box Framework is free software: you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public License as  
// published by the Free Software Foundation, either version 2.1 of the 
// License, or (at your option) any later version.
//
// The Work Box Framework (WBF) is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with the WBF.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocDialogPickLocation : WorkBoxDialogPageBase
    {
        WBRecordsManager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            manager = new WBRecordsManager();

            if (!IsPostBack)
            {
                FunctionalAreasUIControlValue.Text = Request.QueryString["FunctionalAreasUIControlValue"];
                RecordsTypeUIControlValue.Text = Request.QueryString["RecordsTypeUIControlValue"];
                NewOrReplace.Text = Request.QueryString["NewOrReplace"];
                ProtectiveZone.Text = Request.QueryString["ProtectiveZone"];

                ListGUID.Value = Request.QueryString["ListGUID"];
                ItemID.Value = Request.QueryString["ItemID"];

                // The following variable has its name due to a strange compliation error with the name 'DestinationType' 
                TheDestinationType.Value = Request.QueryString["DestinationType"];
                DestinationURL.Value = Request.QueryString["DestinationURL"];
                DestinationTitle.Text = Request.QueryString["DestinationTitle"];

                WBRecordsLibrary masterLibrary = manager.Libraries.ProtectedMasterLibrary;



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


                SPFolder rootFolder = masterLibrary.List.RootFolder;

                WBTerm functionalArea = new WBTerm(manager.FunctionalAreasTaxonomy, FunctionalAreasUIControlValue.Text);
                TreeViewLocationCollection collection = new TreeViewLocationCollection(manager, NewOrReplace.Text, WBRecordsType.PROTECTIVE_ZONE__PROTECTED, functionalArea);

                WorkBoxFolders.DataSource = collection;
                WorkBoxFolders.DataBind();

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

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (manager != null)
            {
                manager.Dispose();
                manager = null;
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

                // Now for the bit where the path is analysed to pick out the selected functional area and the records type:

                String[] pathSteps = selectedPath.Split('/');

                // We're only interested in selections of 3rd level 'folders' that are: functional area / records type / records type  ... or below.
                if (pathSteps.Length < 3) return;

                WBTerm functionalArea = manager.FunctionalAreasTaxonomy.GetSelectedWBTermByPath(pathSteps[0]);
                if (functionalArea == null)
                {
                    WBLogging.Debug("The functional area part of the selected path came back null: " + selectedPath);
                    return;
                }

                Term recordsTypeTerm = manager.RecordsTypesTaxonomy.GetOrCreateSelectedTermByPath(pathSteps[1] + "/" + pathSteps[2]);
                if (recordsTypeTerm == null)
                {
                    WBLogging.Debug("The records type part of the selected path came back null: " + selectedPath);
                    return;
                }
                WBRecordsType recordsType = new WBRecordsType(manager.RecordsTypesTaxonomy, recordsTypeTerm);


                FunctionalAreasUIControlValue.Text = functionalArea.UIControlValue;
                RecordsTypeUIControlValue.Text = recordsType.UIControlValue;


                // Finally let's see if there is a specific record being selected as well:
                if (NewOrReplace.Text == "Replace")
                {
                    WBRecord record = manager.Libraries.GetRecordByPath(selectedPath);

                    SelectedRecordID.Text = record.RecordID;
                }

            }
        }


        private void GoToMetadataPage(String destinationType, String destinationTitle, String destinationUrl)
        {
            string listGuid = ListGUID.Value;
            string itemID = ItemID.Value;

            string redirectUrl = "WorkBoxFramework/PublishDocDialogRequiredMetadataPage.aspx?ListGUID=" + listGuid + "&ItemID=" + itemID + "&DestinationURL=" + destinationUrl + "&DestinationTitle=" + destinationTitle + "&DestinationType=" + destinationType + "&SelectedFolderPath=" + SelectedFolderPath.Text;

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void selectButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogOK(FunctionalAreasUIControlValue.Text + "@" + RecordsTypeUIControlValue.Text + "@" + SelectedRecordID.Text + "@" + SelectedFolderPath.Text);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }

    }
}
