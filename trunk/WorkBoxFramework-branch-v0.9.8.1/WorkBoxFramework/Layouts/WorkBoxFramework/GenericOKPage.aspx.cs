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

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class GenericOKPage : WBDialogPageBase
    {
        public string pageTitle = "";
        public string pageText = "";
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String justReturnOK = Request.QueryString["justReturnOK"];
                if (!String.IsNullOrEmpty(justReturnOK))
                {
                    returnFromDialogOK(justReturnOK);
                    return;
                }

                String justReturnError = Request.QueryString["justReturnError"];
                if (!String.IsNullOrEmpty(justReturnError))
                {
                    returnFromDialogError(justReturnError);
                    return;
                }


                String refreshQueryString = Request.QueryString["refreshQueryString"];
                if (String.IsNullOrEmpty(refreshQueryString)) refreshQueryString = String.Empty;

                String justRefreshOK = Request.QueryString["justRefreshOK"];
                if (!String.IsNullOrEmpty(justRefreshOK))
                {
                    CloseDialogAndRefresh(refreshQueryString);
                    return;
                }


                RefreshQueryString.Value = refreshQueryString;

                pageTitle = Request.QueryString["pageTitle"];
                pageText = Request.QueryString["pageText"];

                okButton.Focus();

            }
        }

        protected void okButton_OnClick(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(RefreshQueryString.Value)) 
            {
                CloseDialogAndRefresh();
            }
            else
            {
                CloseDialogAndRefresh(RefreshQueryString.Value);
            }
        }

    }
}
