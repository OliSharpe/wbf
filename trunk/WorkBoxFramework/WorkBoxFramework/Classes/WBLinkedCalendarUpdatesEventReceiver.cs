using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;


namespace WorkBoxFramework
{
    public class WBLinkedCalendarUpdatesEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            WBLogging.Teams.Unexpected("In WBLinkedCalendarUpdatesEventReceiver(): Requesting a new team event");

            if (properties.List == null)
            {
                WBLogging.Teams.Unexpected("The properties.List value was null!");
                return;
            }
            else
            {
                WBLogging.Teams.Unexpected("Calendar item added to list: " + properties.List.DefaultViewUrl);
            }

            String workBoxCollectionURL = properties.List.WBxGetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__WORK_BOX_COLLECTION);
            String defaultTemplateTitle = properties.List.WBxGetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__DEFAULT_TEMPLATE_TITLE);

            if (String.IsNullOrEmpty(workBoxCollectionURL) || String.IsNullOrEmpty(defaultTemplateTitle))
            {
                WBLogging.Teams.Unexpected("The linked calendar configuration properties were blank: " + workBoxCollectionURL + " | " + defaultTemplateTitle);
                return;
            }

            using (WBCollection collection = new WBCollection(workBoxCollectionURL))
            using (SPSite teamsSite = new SPSite(properties.WebUrl))
            using (SPWeb teamsWeb = teamsSite.OpenWeb())
            {
                WBTaxonomy teams = WBTaxonomy.GetTeams(collection.Site);
                WBTeam team = WBTeam.GetFromTeamSite(teams, teamsWeb);

                if (team == null)
                {
                    WBLogging.Teams.Unexpected("Didn't find a team for this calender creation event!!!");
                }
                else
                {
                    WBLogging.Teams.Unexpected("Found team: " + team.Name + " | " + team.TeamSiteUrl);
                }


                DateTime eventDate = DateTime.Now;
                if (properties.ListItem["EventDate"] == null)
                {
                    if (properties.AfterProperties["EventDate"] == null)
                    {
                        WBLogging.Teams.Unexpected("Both ListItem and AfterProperties have null for 'EventDate' !!");
                    }
                    else
                    {
                        eventDate = (DateTime)properties.AfterProperties["EventDate"];
                    }
                }
                else
                {
                    eventDate = (DateTime)properties.ListItem["EventDate"];
                }

                DateTime endDate = DateTime.Now.AddHours(1);
                if (properties.ListItem["EndDate"] == null)
                {
                    if (properties.AfterProperties["EndDate"] == null)
                    {
                        WBLogging.Teams.Unexpected("Both ListItem and AfterProperties have null for 'EndDate' !!");
                    }
                    else
                    {
                        endDate = (DateTime)properties.AfterProperties["EndDate"];
                    }
                }
                else
                {
                    endDate = (DateTime)properties.ListItem["EndDate"];
                }

                WBLogging.Teams.Unexpected(" Start End times are: " + eventDate + " and " + endDate);

                String title = properties.ListItem["Title"].WBxToString();

                WBLogging.Teams.Unexpected(" Title is: " + title);

                String description = "Changed"; // properties.ListItem["Description"].WBxToString();

                WBLogging.Teams.Unexpected(" description is: " + description);

                WorkBox workBox = collection.RequestNewEventWorkBox(
                    properties.List.DefaultViewUrl,
                    properties.List.ID,
                    properties.ListItemId,
                    title,
                    description,
                    eventDate,
                    endDate,
                    team,
                    null,
                    defaultTemplateTitle);

                workBox.Open("Opening work box triggered by new event in a calendar.");

                workBox.Dispose();
            }

            base.ItemAdded(properties);
        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            String workBoxURL = properties.ListItem.WBxGetAsString(WBColumn.WorkBoxURL);

            if (!String.IsNullOrEmpty(workBoxURL))
            {
                using (WorkBox workBox = new WorkBox(workBoxURL))
                {
                    workBox.ReferenceDate = (DateTime)properties.ListItem["EventDate"];

                    if (workBox.Item.WBxColumnExists("EventDate"))
                    {
                        workBox.Item["EventDate"] = properties.ListItem["EventDate"];
                    }

                    if (workBox.Item.WBxColumnExists("EndDate"))
                    {
                        workBox.Item["EndDate"] = properties.ListItem["EndDate"];
                    }

                    workBox.JustUpdate();
                    workBox.UpdateCachedDetails();
                }
            }

            base.ItemUpdated(properties);
        }

        /// <summary>
        /// An item was deleted.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            String workBoxURL = properties.ListItem.WBxGetAsString(WBColumn.WorkBoxURL);

            if (!String.IsNullOrEmpty(workBoxURL))
            {
                using (WorkBox workBox = new WorkBox(workBoxURL))
                {
                    // We're doing this like this in order trigger an asynchronous closure of the work box:
                    workBox.Item.WBxSet(WBColumn.WorkBoxStatusChangeRequest, "Close");
                    workBox.JustUpdate();
                }
            }

            base.ItemDeleted(properties);
        }


    }
}
