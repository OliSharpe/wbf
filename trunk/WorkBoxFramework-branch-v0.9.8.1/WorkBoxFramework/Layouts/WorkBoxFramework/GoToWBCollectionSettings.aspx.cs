#region Copyright and License

// Copyright (c) Islington Council 2010-2014
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
    public partial class GoToWBCollectionSettings : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String redirectionURL = "";

            String settingsPage = Request.QueryString["SettingsPage"];
            String returnUrl = Request.QueryString["ReturnUrl"];
            if (String.IsNullOrEmpty(settingsPage))
            {
                settingsPage = "WorkBoxCollectionSettingsPage.aspx";
            }

            // By default we will assume that we're either on the WBC or even that we are creating the WBC for the first time:
            redirectionURL = SPContext.Current.Web.Url + "/_layouts/WorkBoxFramework/" + settingsPage;

            // But if we are on a work box then we'll need to redirect to the wb collection's URL:
            WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);
            if (workBox != null)
            {
                redirectionURL = workBox.Collection.Url + "/_layouts/WorkBoxFramework/" + settingsPage;

                // This will dispose of the WBCollection object too.
                workBox.Dispose();
            }
            else
            {
                // Maybe we're on one of the container sites so the parent site will be the WBCollection:
                SPWeb parent = SPContext.Current.Web.ParentWeb;
                if (parent != null)
                {
                    if (WBCollection.IsWebAWBCollection(parent))
                    {
                        redirectionURL = parent.Url + "/_layouts/WorkBoxFramework/" + settingsPage;
                    }

                    parent.Dispose();
                }
            }

            redirectionURL = redirectionURL + "?ReturnUrl=" + returnUrl;

            SPUtility.Redirect(redirectionURL, SPRedirectFlags.Static, Context);
        }
    }
}
