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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBTemplate
    {
        #region Constructors
        public WBTemplate(WBCollection collection, int id)
        {
            _collection = collection;
            _id = id;
            _item = collection.TemplatesList.GetItemById(id);
        }

        public WBTemplate(WBCollection collection, SPListItem item)
        {
            _collection = collection;
            _id = item.ID;
            _item = item;
        }

        #endregion


        #region Properties

        private WBCollection _collection = null;
        public WBCollection Collection { get { return _collection; } }

        private int _id = -1;
        public int ID { get { return _id; } } 

        private SPListItem _item = null;
        public SPListItem Item { get { return _item; } }


        public String Title
        {
            get { return Item.Title; }
        }


        public String TemplateTitle
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_TITLE); }
            set 
            {
                if (!TemplateTitle.Equals(value))
                {
                    // OK so this really is a new title:
                    Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_TITLE, value);
                    TemplateName = "";
                    Update();

                    // So any existing template object is not necessarily right:
                    _template = null;
                }
            }
        }

        public String TemplateName
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_NAME); }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_NAME, value); }
        }

        public String Status
        {
            get 
            { 
                string status = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_STATUS);
                if (status == "") status = WorkBox.WORK_BOX_TEMPLATE_STATUS__DISABLED;
                return status;
            }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_STATUS, value); }
        }

        public bool UseFolderGroupAccessPattern
        {
            get
            {
                return Item.WBxGetColumnAsBool(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE_USE_FOLDER_PATTERN);
            }
        }


        public bool IsActive
        {
            get { return this.Status == WorkBox.WORK_BOX_TEMPLATE_STATUS__ACTIVE_DEFAULT
                            || this.Status == WorkBox.WORK_BOX_TEMPLATE_STATUS__ACTIVE; } 
        }

        private SPWebTemplate _template = null;
        public SPWebTemplate Template
        {
            get 
            {
                if (_template == null)
                {
                    if (TemplateName == "")
                    {
                        _template = Collection.Site.WBxGetWebTemplateByTitle(TemplateTitle);
                        TemplateName = _template.Name;
                        Update();
                    }
                    else
                    {
                        _template = Collection.Site.WBxGetWebTemplateByName(TemplateName);
                    }

                }
                return _template;
            }
        }

        public SPDocumentLibrary DocumentTemplates
        {
            get
            {
                if (Collection == null)
                {
                    WBLogging.Debug("In WBTemplate.DocumentTemplates: Collection was null");
                    return null;
                }

                string templatesLibraryName = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_DOCUMENT_TEMPLATES);

                if (String.IsNullOrEmpty(templatesLibraryName))
                {
                    WBLogging.Debug("In WBTemplate.DocumentTemplates: templatesLibraryName was: " + templatesLibraryName);

                    return null;
                }

                SPList templatesLibraryList = Collection.Web.Lists.TryGetList(templatesLibraryName);

                if (templatesLibraryList == null)
                {
                    WBLogging.Debug("In WBTemplate.DocumentTemplates: templatesLibraryList was null even though name was: " + templatesLibraryName);

                    return null;
                }

                return (SPDocumentLibrary)templatesLibraryList;
            }
        }


        #endregion

        #region Methods

        public WBRecordsType RecordsType(WBTaxonomy recordsTypes)
        {
            return _item.WBxGetSingleTermColumn<WBRecordsType>(recordsTypes, WorkBox.COLUMN_NAME__RECORDS_TYPE);
        }


        private void Update()
        {
            Item.Update();
        }

        public SPFieldLookupValue AsLookupFieldValue
        {
            get 
            {
                return new SPFieldLookupValue(this.ID, this.Title);
            }
        }

        public String InviteInvolvedUserEmailSubject
        {
            get
            {
                String subject = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_INVITE_INVOLVED_EMAIL_SUBJECT);

                if (String.IsNullOrEmpty(subject))
                {
                    subject = WBFarm.Local.InviteInvolvedDefaultEmailSubject;
                }

                return subject;
            }
        }

        public String InviteInvolvedUserEmailBody
        {
            get
            {
                String subject = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_INVITE_INVOLVED_EMAIL_BODY);

                if (String.IsNullOrEmpty(subject))
                {
                    subject = WBFarm.Local.InviteInvolvedDefaultEmailBody;
                }

                return subject;
            }
        }


        public String InviteVisitingUserEmailSubject
        {
            get
            {
                String subject = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_INVITE_VISITING_EMAIL_SUBJECT);

                if (String.IsNullOrEmpty(subject))
                {
                    subject = WBFarm.Local.InviteVisitingDefaultEmailSubject;
                }

                return subject;
            }
        }

        public String InviteVisitingUserEmailBody
        {
            get
            {
                String subject = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_INVITE_VISITING_EMAIL_BODY);

                if (String.IsNullOrEmpty(subject))
                {
                    subject = WBFarm.Local.InviteVisitingDefaultEmailBody;
                }

                return subject;
            }
        }

        public void PrecreateWorkBoxes()
        {
            int totalToHavePrecreated = Item.WBxGetColumnAsInt(WBColumn.PrecreateWorkBoxes, -1);

            // Is this template configured to precreate?
            if (totalToHavePrecreated <= 0) return;

            // Next let's just check that both of the lists for the precreation process are configured:
            String precreatedWorkBoxesListName = Item.WBxGetAsString(WBColumn.PrecreatedWorkBoxesList);
            if (String.IsNullOrEmpty(precreatedWorkBoxesListName)) return;

            String requestPrecreatedWorkBoxListName = Item.WBxGetAsString(WBColumn.RequestPrecreatedWorkBoxList);
            if (String.IsNullOrEmpty(requestPrecreatedWorkBoxListName)) return;

            bool previousWebAllowUnsafeUpdates = Collection.Web.AllowUnsafeUpdates;
            Collection.Web.AllowUnsafeUpdates = true;

            try
            {
                SPList precreatedWorkBoxesList = Collection.Web.Lists[precreatedWorkBoxesListName];
                SPList requestPrecreatedWorkBoxList = Collection.Web.Lists[requestPrecreatedWorkBoxListName];

                // We only need to bring the two list's IDs into sync if there are no remaining precreated work boxes waiting to be used
                // if there are still waiting precreated work boxes, then we should be able to assume that the lists are still in sync
                if (precreatedWorkBoxesList.ItemCount == 0)
                {
                    MakeSureListsAreInSync(precreatedWorkBoxesList, requestPrecreatedWorkBoxList);
                }

                int safety = 0;
                int safetyCutOut = 1000;

                int countPrecreated = precreatedWorkBoxesList.ItemCount;

                WBLogging.Debug("Current count of precreated: " + countPrecreated);
                WBLogging.Debug("Total target of precreated: " + totalToHavePrecreated);


                using (EventsFiringDisabledScope noevents = new EventsFiringDisabledScope())
                {
                    while (countPrecreated < totalToHavePrecreated && safety < safetyCutOut)
                    {
                        safety++;

                        SPListItem newItem = Collection.List.AddItem();
                        WorkBox newWorkBox = new WorkBox(Collection, newItem);
                        newWorkBox.Template = this;

                        // This update ensures that at the item is assigned an ID:
                        newItem.Update();

                        newWorkBox.Create("Precreated work box");

                        Collection.Web.AllowUnsafeUpdates = true;

                        SPListItem precreatedWorkBoxesListItem = precreatedWorkBoxesList.AddItem();
                        precreatedWorkBoxesListItem.WBxSet(WBColumn.WorkBoxListID, newItem.ID);
                        precreatedWorkBoxesListItem.WBxSet(WBColumn.Title, "Precreated work box: " + newWorkBox.Url);
                        precreatedWorkBoxesListItem.Update();

                        // We need to do this as precreatedWorkBoxesList.ItemCount does not get updated in the loop:
                        countPrecreated++;
                    }
                }

                if (safety >= safetyCutOut)
                {
                    throw new NotImplementedException("The precreation of work boxes loop appears to be out of control for: " + TemplateTitle);
                }

            }
            catch (Exception exception)
            {
                WBUtils.SendErrorReport(this.Collection.Web, "Work Box Precreation Error", "Something went wrong when trying to precreate a work box for: " + this.TemplateTitle + " Exception: " + exception.Message + " \n\n " + exception.StackTrace);
            }

            Collection.Web.AllowUnsafeUpdates = previousWebAllowUnsafeUpdates;
        }

        /// <summary>
        /// The IDs of these two lists need to be in sync for the precreation process to work correctly.
        /// </summary>
        /// <param name="precreatedWorkBoxesList"></param>
        /// <param name="requestPrecreatedWorkBoxList"></param>
        private void MakeSureListsAreInSync(SPList precreatedWorkBoxesList, SPList requestPrecreatedWorkBoxList)
        {
            List<SPListItem> itemsToDelete = new List<SPListItem>();

            SPListItem requestNextItem = requestPrecreatedWorkBoxList.AddItem();
            requestNextItem.Update();
            itemsToDelete.Add(requestNextItem);

            int safety = 0;
            int safetyCutOut = 1000;
            int nextPrecreatedListID = -1;

            while (nextPrecreatedListID < requestNextItem.ID && safety < safetyCutOut)
            {
                safety++;

                SPListItem nextItem = precreatedWorkBoxesList.AddItem();
                nextItem.Update();
                nextPrecreatedListID = nextItem.ID;
                itemsToDelete.Add(nextItem);
            }

            int nextRequestListID = requestNextItem.ID;

            // Just in case the lists are out of sync the other way around:
            while (nextRequestListID < nextPrecreatedListID && safety < safetyCutOut)
            {
                safety++;

                SPListItem nextItem = requestPrecreatedWorkBoxList.AddItem();
                nextItem.Update();
                nextRequestListID = nextItem.ID;
                itemsToDelete.Add(nextItem);
            }


            if (safety >= safetyCutOut)
            {
                WBUtils.SendErrorReport(this.Collection.Web, "Work Box Precreation Error", "The safety cutout was exceeded when trying to synchronise the two precreate lists for template: " + TemplateTitle);
                throw new NotImplementedException("The safety cutout was exceeded when trying to synchronise the two precreate lists for template: " + TemplateTitle);
            }

            foreach (SPListItem item in itemsToDelete)
            {
                item.Delete();
            }

            precreatedWorkBoxesList.Update();
            requestPrecreatedWorkBoxList.Update();

            // So, now the 'next ID' value for both of these two lists is the same.
        }

        public WorkBox GetPrecreatedWorkBoxIfAny(String requestTitle)
        {
            if (String.IsNullOrEmpty(requestTitle)) requestTitle = "Request for precreated work box";

            int precreating = Item.WBxGetColumnAsInt(WBColumn.PrecreateWorkBoxes, -1);
            if (precreating <= 0) return null;

            String precreatedWorkBoxesListName = Item.WBxGetAsString(WBColumn.PrecreatedWorkBoxesList);
            if (String.IsNullOrEmpty(precreatedWorkBoxesListName)) return null;

            String requestPrecreatedWorkBoxListName = Item.WBxGetAsString(WBColumn.RequestPrecreatedWorkBoxList);
            if (String.IsNullOrEmpty(requestPrecreatedWorkBoxListName)) return null;

            WorkBox workBox = null;

            try 
            {
                SPList precreatedWorkBoxesList = Collection.Web.Lists[precreatedWorkBoxesListName];

                if (precreatedWorkBoxesList.ItemCount == 0) return null;

                // OK so if we've got here that's because there are at least some existing precreated work boxes.
                // So, it should be the case that creating a new item in the RequestPrecreatedWorkBox list will get
                // us an ID that matches one of the precreated work boxes.

                SPList requestPrecreatedWorkBoxList = Collection.Web.Lists[requestPrecreatedWorkBoxListName];

                SPListItem requestItem = requestPrecreatedWorkBoxList.AddItem();
                requestItem.WBxSet(WBColumn.Title, requestTitle);
                requestItem.Update();

                SPListItem precreatedWorkBoxListItem = precreatedWorkBoxesList.GetItemById(requestItem.ID);

                SPListItem workBoxItem = Collection.List.GetItemById(precreatedWorkBoxListItem.WBxGetColumnAsInt(WBColumn.WorkBoxListID, -1));

                workBox = new WorkBox(Collection, workBoxItem);
                workBox.FirstUseOfWorkBox = true;

                precreatedWorkBoxListItem.Delete();
            }
            catch (Exception exception)
            {
                WBUtils.SendErrorReport(this.Collection.Web, "Work Box Precreation Error", "Something went wrong when trying to find a precreated work box for: " + this.TemplateTitle + " Exception: " + exception.Message + " \n\n " + exception.StackTrace);
                workBox = null;
            }


            return workBox;
        }

        #endregion
    }
}
