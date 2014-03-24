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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using WorkBoxFramework;

namespace WorkBoxFramework.RedirectToMyWorkBox
{
    public partial class RedirectToMyWorkBoxUserControl : UserControl
    {
        public bool redirecting = false;
        public String redirectToUrl = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["NoRedirect"]))
            {
                redirecting = false;
                return;
            }

            WBUser currentUser = new WBUser(SPContext.Current.Web);

            String myWBUrl = currentUser.GetUrlToMyUnprotectedWorkBox(SPContext.Current.Site);

            if (!String.IsNullOrEmpty(myWBUrl))
            {
                redirecting = true;
                redirectToUrl = myWBUrl;
            }
        }
    }
}
