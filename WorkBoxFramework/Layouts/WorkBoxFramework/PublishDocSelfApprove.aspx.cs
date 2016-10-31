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
using System.Linq;
using System.IO;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Taxonomy;
using Newtonsoft.Json;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocSelfApprove : WorkBoxDialogPageBase
    {
        private WBPublishingProcess process = null;

        WBRecordsManager manager = null;
        WBRecord recordBeingReplaced = null;

        SPListItem sourceDocAsItem = null;
        SPFile sourceFile = null;
        string destinationType = "";

        SPListItem fileTypeInfo = null;
       

        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Generic.Verbose("In Page_Load for the self approval dialog");

            manager = new WBRecordsManager();

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {
                process = JsonConvert.DeserializeObject<WBPublishingProcess>(Request.QueryString["PublishingProcessJSON"]);
                process.WorkBox = WorkBox;

//                WBLogging.Debug("Created the WBProcessObject");

                PublishingProcessJSON.Value = JsonConvert.SerializeObject(process);

   //             WBLogging.Debug("Serialized the WBProcessObject to hidden field");

            }
            else
            {
                process = JsonConvert.DeserializeObject<WBPublishingProcess>(PublishingProcessJSON.Value);
                process.WorkBox = WorkBox;
            }





            // Let's clear out all of the error messages text fields:
            ErrorMessageLabel.Text = "";


            //OK so we have the basic identity information for the document being published out so let's get the document item:

            Guid sourceListGuid = new Guid(process.ListGUID);
            SPDocumentLibrary sourceDocLib = (SPDocumentLibrary)WorkBox.Web.Lists[sourceListGuid];

            sourceDocAsItem = sourceDocLib.GetItemById(int.Parse(process.CurrentItemID));
            sourceFile = sourceDocAsItem.File;

            WBDocument sourceDocument = new WBDocument(WorkBox, sourceDocAsItem);

            fileTypeInfo = manager.GetFileTypeInfo(sourceDocument.FileType);

            if (fileTypeInfo != null) 
            {
                Dictionary<String, String> checkBoxDetails = manager.GetCheckBoxDetailsForDocumentType(fileTypeInfo.WBxGetAsString(WBColumn.DocumentType));                
                foreach (String checkBoxCode in checkBoxDetails.Keys)
                {
                    CheckBoxes.Controls.Add(CreateCheckBoxDiv(checkBoxCode, checkBoxDetails[checkBoxCode]));
                }

                CheckBoxesCodes.Value = String.Join(";", checkBoxDetails.Keys.ToArray<string>());
            }

            if (!IsPostBack)
            {

                DocumentsBeingPublished.Text = process.GetStandardHTMLTableRows();

                String typeText = null;

                if (fileTypeInfo != null)
                {
                    typeText = fileTypeInfo.WBxGetAsString(WBColumn.DocumentType) + " (" + fileTypeInfo.WBxGetAsString(WBColumn.FileTypePrettyName) + ")";
                }
                if (String.IsNullOrEmpty(typeText)) typeText = sourceDocument.FileType + " " + sourceDocument.Name;
                DocumentType.Text = typeText;
                WBLogging.Debug("The file type of the record is: " + typeText);

                IAO.Text = "Bill Smith";
            }

        }

        private Control CreateCheckBoxDiv(String id, String text)
        {
            Panel div = new Panel();
            div.CssClass ="wbf-field-value";

            CheckBox checkBox = new CheckBox();
            checkBox.ID = id;
            checkBox.CssClass = "wbf-self-approval-check-box";

            Literal checkBoxText = new Literal();
            checkBoxText.Text = text;

            div.Controls.Add(checkBox);
            div.Controls.Add(checkBoxText);

            return div;
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (manager != null)
            {
                manager.Dispose();
                manager = null;
            }
        }

        private Hashtable CheckMetadataOK()
        {
            Hashtable metadataProblems = new Hashtable();

            List<SPUser> newUsers = PublishingApprovedBy.WBxGetMultiResolvedUsers(SPContext.Current.Web);

            if (newUsers.Count == 0) metadataProblems.Add("PublishingApprovedBy", "You must enter at least one person who approved publication.");

            String[] checkBoxesCodes = CheckBoxesCodes.Value.Split(';');

            bool allChecked = true;
            foreach (String code in checkBoxesCodes)
            {
                CheckBox checkBox = (CheckBox)CheckBoxes.WBxFindNestedControlByID(code);
                if (!checkBox.Checked) allChecked = false;
            }

            if (!allChecked)
            {
                metadataProblems.Add("CheckList", "You have to read and tick all check boxes");
            }

            return metadataProblems;
        }



        protected void publishButton_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("In publishButton_OnClick()");

            Hashtable metadataProblems = CheckMetadataOK();

            if (metadataProblems.Count > 0)
            {

                PublishingApprovedByError.Text = metadataProblems["PublishingApprovedBy"].WBxToString();
                CheckListError.Text = metadataProblems["CheckList"].WBxToString();

                WBLogging.Debug("In publishButton_OnClick(): Page render required - not publishing at this point");
                ReRenderPage();
            }
            else
            {

                Dictionary<String, String> selfApproveDetails = new Dictionary<string, string>();

                List<SPUser> approvedBy = PublishingApprovedBy.WBxGetMultiResolvedUsers(SPContext.Current.Web);

                selfApproveDetails.Add(WBColumn.PublishingApprovedBy.InternalName, approvedBy.WBxToString());
                selfApproveDetails.Add(WBColumn.PublishingApprovalChecklist.InternalName, CheckBoxesCodes.Value);

                process.SelfApprovalDictionary = selfApproveDetails;

                WBLogging.Debug("In publishButton_OnClick(): No page render required - so moving to GoToPublishPage");
                GoToPublishPage();
            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Publishing of document was cancelled");
        }

        private void ReRenderPage()
        {
            // For the moment I think there is nothing to do here
        }

        private void GoToPublishPage()
        {
            string redirectUrl = "WorkBoxFramework/PublishDocActuallyPublish.aspx?PublishingProcessJSON=" + JsonConvert.SerializeObject(process);

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }


    }
}
