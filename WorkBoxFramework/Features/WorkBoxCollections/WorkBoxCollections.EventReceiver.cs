using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace WorkBoxFramework.Features.WorkBoxCollections
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("5ecd6b2f-ac1a-4830-b8b5-83bd2ef83ba9")]
    public class WorkBoxCollectionsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            WBLogging.Generic.HighLevel("WorkBoxCollectionsEventReceiver.FeatureActivated(): Activating the WBF Work Box Collections feature");

            SPSite site = properties.Feature.Parent as SPSite;

            WBFarm.Local.InitialWBCollectionSetup(site);

            WBLogging.Generic.HighLevel("WorkBoxCollectionsEventReceiver.FeatureActivated(): Activating the WBF Work Box Collections feature");
        }
        

        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


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
