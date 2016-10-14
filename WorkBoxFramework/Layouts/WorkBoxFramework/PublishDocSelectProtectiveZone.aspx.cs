﻿#region Copyright and License

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
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Newtonsoft.Json;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocSelectProtectiveZone : WorkBoxDialogPageBase
    {
        public bool userCanPublishToPublic = false;

        private WBPublishingProcess process = null;

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

                    WBLogging.Debug("Before creating the WBProcessObject");

                    process = new WBPublishingProcess(WorkBox, selectedListGUID, selectedItemsIDs);

                    WBLogging.Debug("Created the WBProcessObject");
                    
                    PublishingProcessJSON.Value = JsonConvert.SerializeObject(process);

                    String html = "";

                    WBLogging.Debug("Created the WBProcessObject and serialised " + PublishingProcessJSON.Value);

                    if (process.ItemIDs.Count == 0)
                    {
                        WBLogging.Debug("process.ItemIDs.Count == 0");
                        html += "<i>No documents selected!</i>";
                    }
                    else
                    {
                        html += "<table cellpadding='0px' cellspacing='5px'>";

                        foreach (String itemID in process.ItemIDs)
                        {
                            String filename = process.MappedFilenames[itemID];

                            WBLogging.Debug("list through item with name: " + filename);
                            html += "<tr><td align='center'><img src='" + WBUtils.DocumentIcon16(filename) + "' alt='Icon for file " + filename + "'/></td><td align='left'>" + filename + "</td></tr>\n";
                        }

                        html += "</table>";
                    }

                    DocumentsBeingPublished.Text = html;

                }
                else
                {
                    ErrorMessageLabel.Text = "There was an error with the passed through values";
                }
            }
            else
            {
                process = JsonConvert.DeserializeObject<WBPublishingProcess>(PublishingProcessJSON.Value);
                process.WorkBox = WorkBox;
            }
        }

        private void GoToMetadataPage()
        {
            string redirectUrl = "WorkBoxFramework/PublishDocRequiredMetadata.aspx?PublishingProcessJSON=" + JsonConvert.SerializeObject(process);
            //string redirectUrl = "WorkBoxFramework/PublishDocActuallyPublish.aspx?PublishingProcessJSON=" + JsonConvert.SerializeObject(process);

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
            process.ProtectiveZone = WBRecordsType.PROTECTIVE_ZONE__PUBLIC;

            GoToMetadataPage();
        }

        protected void PublicExtranetButton_onClick(object sender, EventArgs e)
        {
            process.ProtectiveZone = WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET;

            GoToMetadataPage();
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
            process.ProtectiveZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;

            GoToMetadataPage();
        }

        /*
        protected void WorkBoxButton_onClick(object sender, EventArgs e)
        {
            GoToMetadataPage(
                WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX,
                DestinationTitle.Value.Trim(),
                DestinationURL.Value.Trim());
        }
         */ 

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }

    }
}