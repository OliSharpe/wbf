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

                    WBLogging.Generic.Unexpected("Root folder was: " + folderPath + "     in : " + WorkBox.DocumentLibrary.RootFolder.Url);
                    WBLogging.Generic.Unexpected("docLibraryFolderPath =  " + docLibraryFolderPath);

                    folderPath = folderPath.Replace(docLibraryFolderPath, "");

                    WBLogging.Generic.Unexpected("Now using folder path: " + folderPath);


                    WBUser user = new WBUser(WorkBox.Web.CurrentUser);

                    String clipboardAction = user.PasteClipboard(WorkBox, folderPath);

                    /*
                    String justReturnOK = "Pasted items are still on clipboard to be copied again.";
                    if (clipboardAction == WBUser.CLIPBOARD_ACTION__CUT)
                    {
                        justReturnOK = "Pasted items removed from original location and clipboard.";
                    }
                    */

                    string okPageUrl = "WorkBoxFramework/GenericOKPage.aspx";
                    string queryString = "justRefreshOK=True";

                    longOperation.End(okPageUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
                }


            }


        }

        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogAndRefresh();
        }


    }
}
