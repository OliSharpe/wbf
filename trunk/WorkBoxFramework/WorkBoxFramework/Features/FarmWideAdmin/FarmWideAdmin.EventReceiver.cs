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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace WorkBoxFramework.Features.FarmWideAdmin
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("96d1bed3-7f98-4f2d-9c22-94071204a75f")]
    public class FarmWideAdminEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            WBUtils.logMessage("Activating Farm Admin Feature");
            RegisterLoggingService(properties);
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            UnRegisterLoggingService(properties);
        }

        static void RegisterLoggingService(SPFeatureReceiverProperties properties)
        {
            // SPFarm farm = properties.Feature.Parent as SPFarm;
            SPFarm farm = SPFarm.Local;

            if (farm != null)
            {
                WBLogging service = WBLogging.Local;

                if (service == null)
                {
                    service = new WBLogging();

                    service.Update();

                    if (service.Status != SPObjectStatus.Online)
                        service.Provision();

                }
            }
            else
            {
                WBUtils.logMessage("Farm was null!!");
            }

        }


        static void UnRegisterLoggingService(SPFeatureReceiverProperties properties)
        {
            SPFarm farm = SPFarm.Local;

            if (farm != null)
            {
                WBLogging service = WBLogging.Local;

                if (service != null)
                    service.Delete();
            }
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
