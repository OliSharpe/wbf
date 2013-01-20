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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace WorkBoxFramework.Features.ErrorPages
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("d3654747-a709-4859-aa03-30c61a7bd5be")]
    public class ErrorPagesEventReceiver : SPFeatureReceiver
    {
        const string customAccessDeniedPage = "/_layouts/WorkBoxFramework/AccessDenied.html";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            SPSite site = properties.Feature.Parent as SPSite;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite _site = new SPSite(site.ID, SPUserToken.SystemAccount))
                {
                    SPWebApplication webApp = _site.WebApplication;
                    if (null != webApp)
                    {
                        webApp.FileNotFoundPage = "WBF404Page.html";

                        if (webApp.UpdateMappedPage(SPWebApplication.SPCustomPage.AccessDenied, customAccessDeniedPage))
                        {
                            webApp.Update();
                        }
                    }
                }
            });


        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite _site = new SPSite(site.ID, SPUserToken.SystemAccount))
                {
                    SPWebApplication webApp = _site.WebApplication;
                    if (null != webApp)
                    {
                        webApp.FileNotFoundPage = null;

                        if (webApp.UpdateMappedPage(SPWebApplication.SPCustomPage.AccessDenied, null))
                        {
                            webApp.Update();
                        }
                    }
                }
            });
        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
