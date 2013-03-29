using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ConfigureLinkedCalendar : WorkBoxDialogPageBase
    {
        SPList calendar = null;

        public const String TEAM_SITE_CALENDAR__ADDITIONS = "Team Site Calendar Additions";
        public const String TEAM_SITE_CALENDAR__UPDATES = "Team Site Calendar Updates";
        public const String TEAM_SITE_CALENDAR__DELETIONS = "Team Site Calendar Deletions";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CalendarListGUID.Value = Request.QueryString["ListId"];
            }

            if (String.IsNullOrEmpty(CalendarListGUID.Value))
            {
                ErrorMessage.Text = "Couldn't find the list Id for the calendar";
                return;
            }

            calendar = SPContext.Current.Web.Lists[new Guid(CalendarListGUID.Value.Trim())];

            if (!IsPostBack)
            {
                CalendarURL.Text = calendar.DefaultViewUrl;
                LinkedCalendarWorkBoxCollection.Text = calendar.WBxGetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__WORK_BOX_COLLECTION);
                LinkedCalendarDefaultWorkBoxTemplate.Text = calendar.WBxGetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__DEFAULT_TEMPLATE_TITLE);
            }
        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            if (calendar != null)
            {
                calendar.WBxSetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__WORK_BOX_COLLECTION, LinkedCalendarWorkBoxCollection.Text);
                calendar.WBxSetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__DEFAULT_TEMPLATE_TITLE, LinkedCalendarDefaultWorkBoxTemplate.Text);
                calendar.Update();
            }

            returnFromDialogCancel("");
        }

        protected void removeButton_OnClick(object sender, EventArgs e)
        {
            if (calendar != null)
            {
                calendar.WBxSetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__WORK_BOX_COLLECTION, "");
                calendar.WBxSetProperty(WorkBox.LINKED_CALENDAR_PROPERTY__DEFAULT_TEMPLATE_TITLE, "");
                calendar.Update();
            }

            returnFromDialogCancel("");
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }


        private void RemoveEventReceivers(SPList calendar)
        {
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

        private void AddEventReceivers(SPList calendar)
        {
            string assemblyName = "WorkBoxFramework, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=4554acfc19d83350";
            string className = "WorkBoxFramework.WBTeamSiteCalendarChangeEventReceiver";

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

    }
}
