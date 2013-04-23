﻿#region Copyright and License

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
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ViewClipboard : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["justPasted"] == "True")
                {
                    JustPastedText.Text = "<div><b><i>You have just pasted these items:</i></b></div>";
                    NeedsRefreshOnReturn.Value = "True";
                }
                else
                {
                    NeedsRefreshOnReturn.Value = "False";
                }

                RenderClipboard();
            }
        }

        private void RenderClipboard()
        {
            WBUser user = new WBUser(WorkBox.Web.CurrentUser);

            CutOrCopiedText.Text = user.RenderClipboardAction(SPContext.Current.Site);

            ItemsOnClipboard.Text = user.RenderClipboardItems(SPContext.Current.Site);
        }


        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            if (NeedsRefreshOnReturn.Value == "True")
            {
                CloseDialogAndRefresh();
            }
            else
            {
                CloseDialogWithOK(" ");
            }
        }

        protected void clearAllButton_OnClick(object sender, EventArgs e)
        {
            WBUser user = new WBUser(WorkBox.Web.CurrentUser);

            WorkBox.Web.AllowUnsafeUpdates = true;
            user.ClearClipboard(SPContext.Current.Site);
            WorkBox.Web.AllowUnsafeUpdates = false;

            JustPastedText.Text = "";
//            RenderClipboard();

            CloseDialogAndRefresh();
        }


    }
}
