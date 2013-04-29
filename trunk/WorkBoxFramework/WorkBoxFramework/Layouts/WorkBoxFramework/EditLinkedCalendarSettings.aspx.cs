using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class EditLinkedCalendarSettings : LayoutsPageBase
    {
        SPList list = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ListGUID.Value = Request.QueryString["List"];
            }

            if (!String.IsNullOrEmpty(ListGUID.Value))
            {
                list = SPContext.Current.Web.Lists[new Guid(ListGUID.Value)];

                string html = "<ul>";
                for (int i = 0; i < list.EventReceivers.Count; i++)
                {
                    html += "<li>" + list.EventReceivers[i].Name + "   |   " + list.EventReceivers[i].Class + "</li>";
                }
                html += "</ul>";

                EventReceivers.Text = html;

            }
            else
            {
                EventReceivers.Text = "<i>Couldn't find the list</i>";
            }

            if (!IsPostBack && list != null)
            {
                WorkBoxCollectionURL.Text = list.WBxGetProperty(WorkBox.LIST_PROPERTY__LINKED_CALENDAR__WORK_BOX_COLLECTION);
                DefaultTemplateTitle.Text = list.WBxGetProperty(WorkBox.LIST_PROPERTY__LINKED_CALENDAR__DEFAULT_TEMPLATE_TITLE);
            }
        }


        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            if (list != null)
            {
                list.WBxSetProperty(WorkBox.LIST_PROPERTY__LINKED_CALENDAR__WORK_BOX_COLLECTION, WorkBoxCollectionURL.Text);
                list.WBxSetProperty(WorkBox.LIST_PROPERTY__LINKED_CALENDAR__DEFAULT_TEMPLATE_TITLE, DefaultTemplateTitle.Text);

                list.RootFolder.Update();
                list.Update();
                SPContext.Current.Web.Update();

                RemoveEventReceivers(list);
                AddEventReceivers(list);

            }
            else
            {
                WBLogging.Generic.Unexpected("This list was nulL");
            }

            SPUtility.Redirect("listedit.aspx?List=" + ListGUID.Value, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("listedit.aspx?List=" + ListGUID.Value, SPRedirectFlags.RelativeToLayoutsPage, Context);
        }


        private void RemoveEventReceivers(SPList calendar)
        {
            for (int i = 0; i < calendar.EventReceivers.Count; i++)
            {
                if (calendar.EventReceivers[i].Name != null)
                {
                    if (calendar.EventReceivers[i].Name == WorkBox.LINKED_CALENDAR_EVENT_RECEIVER__ITEM_ADDED
                        || calendar.EventReceivers[i].Name == WorkBox.LINKED_CALENDAR_EVENT_RECEIVER__ITEM_UPDATED
                        || calendar.EventReceivers[i].Name == WorkBox.LINKED_CALENDAR_EVENT_RECEIVER__ITEM_DELETING)
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
            string className = "WorkBoxFramework.WBLinkedCalendarUpdatesEventReceiver";

            SPEventReceiverDefinition itemAddedEventReceiver = calendar.EventReceivers.Add();
            itemAddedEventReceiver.Name = WorkBox.LINKED_CALENDAR_EVENT_RECEIVER__ITEM_ADDED;
            itemAddedEventReceiver.Type = SPEventReceiverType.ItemAdded;
            itemAddedEventReceiver.SequenceNumber = 1000;
            itemAddedEventReceiver.Assembly = assemblyName;
            itemAddedEventReceiver.Class = className;
            itemAddedEventReceiver.Update();

            SPEventReceiverDefinition itemUpdatedEventReceiver = calendar.EventReceivers.Add();
            itemUpdatedEventReceiver.Name = WorkBox.LINKED_CALENDAR_EVENT_RECEIVER__ITEM_UPDATED;
            itemUpdatedEventReceiver.Type = SPEventReceiverType.ItemUpdated;
            itemUpdatedEventReceiver.SequenceNumber = 1000;
            itemUpdatedEventReceiver.Assembly = assemblyName;
            itemUpdatedEventReceiver.Class = className;
            itemUpdatedEventReceiver.Update();

            SPEventReceiverDefinition itemDeletedEventReceiver = calendar.EventReceivers.Add();
            itemDeletedEventReceiver.Name = WorkBox.LINKED_CALENDAR_EVENT_RECEIVER__ITEM_DELETING;
            itemDeletedEventReceiver.Type = SPEventReceiverType.ItemDeleting;
            itemDeletedEventReceiver.SequenceNumber = 1000;
            itemDeletedEventReceiver.Assembly = assemblyName;
            itemDeletedEventReceiver.Class = className;
            itemDeletedEventReceiver.Update();

        }


    }
}
