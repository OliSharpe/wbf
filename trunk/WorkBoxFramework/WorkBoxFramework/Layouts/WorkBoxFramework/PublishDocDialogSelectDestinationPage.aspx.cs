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
    public partial class PublishDocDialogSelectDestinationPage : WorkBoxDialogPageBase
    {
        public bool userCanPublishToPublic = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            SPGroup publishersGroup = WorkBox.OwningTeam.PublishersGroup(SPContext.Current.Site);
            if (publishersGroup != null)
            {
                if (publishersGroup.ContainsCurrentUser)
                {
                    userCanPublishToPublic = true;
                }
            }

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

        private void GoToMetadataPage(String destinationType, String destinationTitle, String destinationUrl)
        {
            string listGuid = ListGUID.Value;
            string itemID = ItemID.Value;

            string redirectUrl = "WorkBoxFramework/PublishDocDialogRequiredMetadataPage.aspx?ListGUID=" + listGuid + "&ItemID=" + itemID + "&DestinationURL=" + destinationUrl + "&DestinationTitle=" + destinationTitle + "&DestinationType=" + destinationType;

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        /* not being used at the moment:
        protected void nextButton_OnClick(object sender, EventArgs e)
        {
            // Now let's go to the second page of the publish dialog:
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__USER_DEFINED_DESTINATION,
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__USER_DEFINED_DESTINATION, 
                DestinationURL.Value.Trim());
        }
         */ 

        protected void PublicWebSiteButton_onClick(object sender, EventArgs e)
        {
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE,
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE,
                "");
        }

        protected void PublicExtranetButton_onClick(object sender, EventArgs e)
        {
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_EXTRANET,
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_EXTRANET,
                "");
        }

        /* Not being used at the moment ... to be discussed.
        protected void izziIntranetButton_onClick(object sender, EventArgs e)
        {
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__IZZI_INTRANET,
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__IZZI_INTRANET,
                "");
        }
        */

        protected void RecordsLibraryButton_onClick(object sender, EventArgs e)
        {
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__RECORDS_LIBRARY,
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__RECORDS_LIBRARY,
                "");
        }

        protected void WorkBoxButton_onClick(object sender, EventArgs e)
        {
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX,
                DestinationTitle.Value.Trim(),
                DestinationURL.Value.Trim());
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }

    }
}
