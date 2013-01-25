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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PasteFromClipboard : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                using (SPLongOperation longOperation = new SPLongOperation(this.Page))
                {
                    longOperation.LeadingHTML = "Pasting items from your work box clipboard";
                    longOperation.TrailingHTML = "If you are pasting a lot of items this might take some time.";

                    longOperation.Begin();


                    String folderPath = Request.QueryString["RootFolder"];
                    if (String.IsNullOrEmpty(folderPath))
                    {
                        folderPath = "";
                    }

                    String docLibraryFolderPath = WorkBox.Web.ServerRelativeUrl + "/" + WorkBox.DocumentLibrary.RootFolder.Url;

//                    WBLogging.Debug("Root folder was: " + folderPath + "     in : " + WorkBox.DocumentLibrary.RootFolder.Url);
  //                  WBLogging.Debug("docLibraryFolderPath =  " + docLibraryFolderPath);

                    folderPath = folderPath.Replace(docLibraryFolderPath, "");

//                    WBLogging.Debug("Now using folder path: " + folderPath);


                    SPFolder folder = WorkBox.DocumentLibrary.RootFolder.WBxGetFolderPath(folderPath);

                    WBUser user = new WBUser(WorkBox.Web.CurrentUser);

                    Dictionary<String, List<int>> clipboard = user.GetClipboard(WorkBox);

                    WorkBox.Web.AllowUnsafeUpdates = true;

                    foreach (String workBoxURL in clipboard.Keys)
                    {
                        List<int> ids = clipboard[workBoxURL];

                        using (WorkBox clipboardWorkBox = new WorkBox(workBoxURL))
                        {
                            SPDocumentLibrary documents = clipboardWorkBox.DocumentLibrary;

                            foreach (int id in ids)
                            {
                                SPListItem item = documents.GetItemById(id);

                                folder.WBxCopyIntoFolder(item);
                            }
                        }
                    }

                    WorkBox.Web.AllowUnsafeUpdates = false;


                    
                    string okPageUrl = "WorkBoxFramework/ViewClipboard.aspx";
                    string queryString = "justPasted=True";

                    longOperation.End(okPageUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
                }


            }


        }

        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            this.returnFromDialogOKAndRefresh();
        }


    }
}
