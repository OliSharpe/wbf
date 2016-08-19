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
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocJustToCaseFile : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["selectedItemsIDsString"] != null && Request.QueryString["selectedListGUID"] != null)
                {
                    string selectedListGUID = Request.QueryString["selectedListGUID"];
                    string[] selectedItemsIDs = Request.QueryString["selectedItemsIDsString"].ToString().Split('|');

                    WBUtils.logMessage("The list GUID was: " + selectedListGUID);
                    selectedListGUID = selectedListGUID.Substring(1, selectedListGUID.Length - 2).ToLower();

                    Guid sourceListGuid = new Guid(selectedListGUID);

                    ListGUID.Value = sourceListGuid.ToString();
                    ItemID.Value = selectedItemsIDs[1].ToString();

                    WBUtils.logMessage("The ListGUID was: " + ListGUID.Value);
                    WBUtils.logMessage("The ItemID was: " + ItemID.Value);

                    SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)WorkBox.Web.Lists[sourceListGuid];
                    SPListItem sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(ItemID.Value));

                    SourceDocFileName.Text = sourceDocAsItem.Name;

                    SourceDocIcon.AlternateText = "Icon of document being publishing out.";
                    SourceDocIcon.ImageUrl = SPUtility.ConcatUrls("/_layouts/images/",
                                                SPUtility.MapToIcon(WorkBox.Web,
                                                SPUtility.ConcatUrls(WorkBox.Web.Url, sourceDocAsItem.Url), "", IconSize.Size32));

                    //                    foreach (SPList list in SPContext.Current.Web.Lists)
                    //                  {
                    //                    WBUtils.logMessage("Found list name = " + list.Title + " list ID = " + list.ID);
                    //              }

                }
                else
                {
                    ErrorMessageLabel.Text = "There was an error with the passed through values";
                }
            }

        }

        private void GoToNextPage(String destinationType, String destinationTitle, String destinationUrl)
        {
            string listGuid = ListGUID.Value;
            string itemID = ItemID.Value;

            string redirectUrl = "WorkBoxFramework/PublishDocPickWorkBoxFolder.aspx?ListGUID=" + listGuid + "&ItemID=" + itemID + "&DestinationURL=" + destinationUrl + "&DestinationTitle=" + destinationTitle + "&DestinationType=" + destinationType;

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void WorkBoxButton_onClick(object sender, EventArgs e)
        {
            WorkBox caseFileWorkBox = WorkBox.Collection.FindByLocalID(LocalID.Text.Trim());

            if (caseFileWorkBox != null)
            {
                GoToNextPage(
                    WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX,
                    caseFileWorkBox.Title,
                    caseFileWorkBox.Url);
            }
            else
            {
                GoToGenericOKPage("Counldn't find case file work box", "It was not possible to find the case file work box with local ID = " + LocalID.Text);
            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }

    }
}
