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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace WBFWebParts.CentralListOfLinks
{
    public partial class CentralListOfLinksUserControl : UserControl
    {
        protected CentralListOfLinks webPart = default(CentralListOfLinks);

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as CentralListOfLinks;

            // Read contents from SharePoint list and display links
            string strHTML = "";

            SPSite oSite = null;
            SPWeb oWeb = null;
            bool mustDisposeOfWeb = false;
            bool mustDisposeOfSite = false;

            if (String.IsNullOrEmpty(webPart.CentralWebSiteURL))
            {
                oSite = SPContext.Current.Site;
                oWeb = oSite.RootWeb;
                if (oWeb != SPContext.Current.Web)
                {
                    mustDisposeOfWeb = true;
                }
            } else {
                oSite = new SPSite(webPart.CentralWebSiteURL);
                mustDisposeOfSite = true;

                oWeb = oSite.OpenWeb();
                mustDisposeOfWeb = true;
            }

            String listName = "ListOfLinks";
            if (!String.IsNullOrEmpty(webPart.CentralListName))
            {
                listName = webPart.CentralListName;
            }

            try
            {
                SPList List = oWeb.Lists[listName];
                string strQuery = "<OrderBy><FieldRef Name='Order' /></OrderBy>";

                SPQuery oQuery = new SPQuery();
                // Set up filter criteria selected by end user
                oQuery.Query = string.Format(strQuery);
                SPListItemCollection listItems = List.GetItems(oQuery);
                strHTML = strHTML + "<ul>";

                foreach (SPListItem membitem in listItems)
                {
                    strHTML = strHTML + "<li><a href='" + membitem["PageLink"] + "' target='_self'>" + membitem["Title"] + "</a></li>";
                }

                strHTML = strHTML + "</ul>";
            }
            catch (Exception exception)
            {
                litWebPartText.Text = "<i>This web part is not configured correctly</i>";
            }

            litWebPartText.Text = strHTML;

            if (mustDisposeOfWeb) oWeb.Dispose();
            if (mustDisposeOfSite) oSite.Dispose();
        }
    }
}
