using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace WorkBoxFramework.Features.TeamSiteCalendarUpdates
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("18e178c6-9d60-445b-933a-4f53e0c3f8ac")]
    public class TeamSiteCalendarUpdatesEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public const String TEAM_SITE_CALENDAR__ADDITIONS = "Team Site Calendar Additions";
        public const String TEAM_SITE_CALENDAR__UPDATES = "Team Site Calendar Updates";
        public const String TEAM_SITE_CALENDAR__DELETIONS = "Team Site Calendar Deletions";
        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            RemoveEventReceivers(properties);
            AddEventReceivers(properties);

        }

        private void RemoveEventReceivers(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            SPList calendar = web.Lists["Calendar"];

            for (int i = 0; i < calendar.EventReceivers.Count; i++)                                
            {                            
                if (calendar.EventReceivers[i].Name != null)                            
                {
                    if (calendar.EventReceivers[i].Name == TEAM_SITE_CALENDAR__ADDITIONS 
                        || calendar.EventReceivers[i].Name == TEAM_SITE_CALENDAR__UPDATES
                        || calendar.EventReceivers[i].Name == TEAM_SITE_CALENDAR__DELETIONS)                                
                    {                                    
                        calendar.EventReceivers[i].Delete();                                    
                        i = -1;                                
                    }                            
                }                        
            }
        }
                
        private void AddEventReceivers(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            SPList calendar = web.Lists["Calendar"];

            string assemblyName = "WorkBoxFramework, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=4554acfc19d83350";
            string className = "WorkBoxFramework.WBTeamSiteCalendarChangeEventReceiver.WBTeamSiteCalendarChangeEventReceiver";

            SPEventReceiverDefinition itemAddedEventReceiver = calendar.EventReceivers.Add();
            itemAddedEventReceiver.Name = TEAM_SITE_CALENDAR__ADDITIONS;
            itemAddedEventReceiver.Type = SPEventReceiverType.ItemAdded;
            itemAddedEventReceiver.SequenceNumber = 1000;
            itemAddedEventReceiver.Assembly = assemblyName;
            itemAddedEventReceiver.Class = className;
            itemAddedEventReceiver.Update();

            SPEventReceiverDefinition itemUpdatedEventReceiver = calendar.EventReceivers.Add();
            itemUpdatedEventReceiver.Name = TEAM_SITE_CALENDAR__UPDATES;
            itemUpdatedEventReceiver.Type = SPEventReceiverType.ItemUpdated;
            itemUpdatedEventReceiver.SequenceNumber = 1000;
            itemUpdatedEventReceiver.Assembly = assemblyName;
            itemUpdatedEventReceiver.Class = className;
            itemUpdatedEventReceiver.Update();

            SPEventReceiverDefinition itemDeletedEventReceiver = calendar.EventReceivers.Add();
            itemDeletedEventReceiver.Name = TEAM_SITE_CALENDAR__DELETIONS;
            itemDeletedEventReceiver.Type = SPEventReceiverType.ItemDeleting;
            itemDeletedEventReceiver.SequenceNumber = 1000;
            itemDeletedEventReceiver.Assembly = assemblyName;
            itemDeletedEventReceiver.Class = className;
            itemDeletedEventReceiver.Update();

        }

        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            RemoveEventReceivers(properties);
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
