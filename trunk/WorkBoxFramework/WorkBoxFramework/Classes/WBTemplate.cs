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

        #endregion
    }
}
