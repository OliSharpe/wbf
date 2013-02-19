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
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    /// <summary>
    /// The WorkBox class encapsulates the key data and actions for a Work Box. In particular it provides a single 
    /// place to interact with both the metadata list item for the work box and the SPWeb site for the work box. 
    /// </summary>
    public class WorkBox : IDisposable
    {

        #region Constants

        public const string JAVASCRIPT_VAR__URL_FOR_NEW_DIALOG                  = "wbf__javascript_var__url_for_new_dialog";

        public const string JAVASCRIPT_VAR__URL_FOR_VIEW_PROPERTIES_DIALOG      = "wbf__javascript_var__url_for_view_properties_dialog";
        public const string JAVASCRIPT_VAR__URL_FOR_EDIT_PROPERTIES_DIALOG      = "wbf__javascript_var__url_for_edit_properties_dialog";

        public const string JAVASCRIPT_VAR__URL_FOR_VIEW_ALL_INVOLVED_DIALOG    = "wbf__javascript_var__url_for_view_all_involved_dialog";
        public const string JAVASCRIPT_VAR__URL_FOR_INVITE_TEAM_DIALOG          = "wbf__javascript_var__url_for_invite_team_dialog";
        public const string JAVASCRIPT_VAR__URL_FOR_INVITE_INDIVIDUAL_DIALOG    = "wbf__javascript_var__url_for_invite_individual_dialog";
        public const string JAVASCRIPT_VAR__URL_FOR_CHANGE_OWNER_DIALOG         = "wbf__javascript_var__url_for_change_owner_dialog";

        public const string JAVASCRIPT_VAR__URL_FOR_CLOSE_DIALOG                = "wbf__javascript_var__url_for_close_dialog";
        public const string JAVASCRIPT_VAR__URL_FOR_REOPEN_DIALOG               = "wbf__javascript_var__url_for_reopen_dialog";


        public const string JAVASCRIPT_VAR__ENABLE_VIEW_PROPERTIES_BUTTON       = "wbf__javascript_var__enable_view_properties_button";
        public const string JAVASCRIPT_VAR__ENABLE_EDIT_PROPERTIES_BUTTON       = "wbf__javascript_var__enable_edit_properties_button";

        public const string JAVASCRIPT_VAR__ENABLE_VIEW_ALL_INVOLVED_BUTTON     = "wbf__javascript_var__enable_view_all_involved_button";
        public const string JAVASCRIPT_VAR__ENABLE_INVITE_TEAM_BUTTON           = "wbf__javascript_var__enable_invite_team_button";
        public const string JAVASCRIPT_VAR__ENABLE_INVITE_INDIVIDUAL_BUTTON     = "wbf__javascript_var__enable_invite_individual_button";
        public const string JAVASCRIPT_VAR__ENABLE_CHANGE_OWNER_BUTTON          = "wbf__javascript_var__enable_change_owner_button";

        public const string JAVASCRIPT_VAR__ENABLE_CLOSE_BUTTON                 = "wbf__javascript_var__enable_close_button";
        public const string JAVASCRIPT_VAR__ENABLE_REOPEN_BUTTON                = "wbf__javascript_var__enable_reopen_button";

 
        public const string WORK_BOXES_LIST_EVENT_RECEIVER__ITEM_ADDED          = "wbf__work_box_list_event_receiver__ItemAdded";
        public const string WORK_BOXES_LIST_EVENT_RECEIVER__ITEM_UPDATED        = "wbf__work_box_list_event_receiver__ItemUpdated";

        public const Int32 LOCALE_ID_ENGLISH = 1033;

        public const string WORK_BOX_PROPERTY__COLLECTION_WEB_GUID      = "wbf__work_box__collection_web_guid";
        public const string WORK_BOX_PROPERTY__COLLECTION_LIST_GUID     = "wbf__work_box__collection_list_guid";
        public const string WORK_BOX_PROPERTY__METADATA_ITEM_ID         = "wbf__work_box__metadata_item_id";
        public const string WORK_BOX_PROPERTY__DOCUMENT_LIBRARY_GUID    = "wbf__work_box__document_library_guid";


        public const string COLUMN_NAME__WORK_BOX_STATUS = "WorkBoxStatus";
        public const string COLUMN_NAME__WORK_BOX_STATUS_CHANGE_REQUEST = "WorkBoxStatusChangeRequest";
        public const string COLUMN_NAME__WORK_BOX_ERROR_MESSAGE = "WorkBoxErrorMessage";
        public const string COLUMN_NAME__WORK_BOX_LOCAL_ID = "WorkBoxLocalID";
        public const string COLUMN_NAME__WORK_BOX_UNIQUE_ID = "WorkBoxUniqueID";
        public const string COLUMN_NAME__WORK_BOX_SHORT_TITLE = "WorkBoxShortTitle";
        public const string COLUMN_NAME__WORK_BOX_LINK = "WorkBoxLink";
        public const string COLUMN_NAME__WORK_BOX_GUID = "WorkBoxGUID";
        public const string COLUMN_NAME__WORK_BOX_URL = "WorkBoxURL";
        public const string COLUMN_NAME__WORK_BOX_TEMPLATE = "WorkBoxTemplate";
        public const string COLUMN_NAME__WORK_BOX_DATE_CREATED = "WorkBoxDateCreated";
        public const string COLUMN_NAME__WORK_BOX_DATE_DELETED = "WorkBoxDateDeleted";
        public const string COLUMN_NAME__WORK_BOX_DATE_LAST_OPENED = "WorkBoxDateLastOpened";
        public const string COLUMN_NAME__WORK_BOX_DATE_LAST_CLOSED = "WorkBoxDateLastClosed";
        public const string COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED = "WorkBoxDateLastModified";
        public const string COLUMN_NAME__WORK_BOX_RETENTION_END_DATE = "WorkBoxRetentionEndDate";
        public const string COLUMN_NAME__WORK_BOX_AUDIT_LOG = "WorkBoxAuditLog";

        public const string COLUMN_NAME__WORK_BOX_CACHED_LIST_ITEM_ID = "WorkBoxCachedListItemID";
        public const string COLUMN_NAME__WORK_BOX_DATE_LAST_VISITED = "WorkBoxDateLastVisited";
        
        public const string LIST_NAME__WORK_BOX_TEMPLATES = "Work Box Templates";
        public const string COLUMN_NAME__WORK_BOX_TEMPLATE_NAME = "WorkBoxTemplateName";
        public const string COLUMN_NAME__WORK_BOX_TEMPLATE_TITLE = "WorkBoxTemplateTitle";
        public const string COLUMN_NAME__WORK_BOX_TEMPLATE_STATUS = "WorkBoxTemplateStatus";
        public const string COLUMN_NAME__WORK_BOX_DOCUMENT_TEMPLATES = "WorkBoxDocumentTemplates";
        public const string COLUMN_NAME__WORK_BOX_INVITE_INVOLVED_EMAIL_SUBJECT = "WorkBoxInviteInvolvedEmailSubject";
        public const string COLUMN_NAME__WORK_BOX_INVITE_INVOLVED_EMAIL_BODY = "WorkBoxInviteInvolvedEmailBody";
        public const string COLUMN_NAME__WORK_BOX_INVITE_VISITING_EMAIL_SUBJECT = "WorkBoxInviteVisitingEmailSubject";
        public const string COLUMN_NAME__WORK_BOX_INVITE_VISITING_EMAIL_BODY = "WorkBoxInviteVisitingEmailBody";
        public const string COLUMN_NAME__WORK_BOX_TEMPLATE_USE_FOLDER_PATTERN = "WorkBoxTemplateUseFolderPattern";

        public const string WORK_BOX_TEMPLATE_STATUS__ACTIVE = "Active";
        public const string WORK_BOX_TEMPLATE_STATUS__ACTIVE_DEFAULT = "Active (default)";
        public const string WORK_BOX_TEMPLATE_STATUS__DISABLED = "Disabled";
        
        public const string LIST_NAME__LINKED_WORK_BOXES = "Linked Work Boxes";


        public const string REQUEST_WORK_BOX_STATUS_CHANGE__CREATE = "Create";
        public const string REQUEST_WORK_BOX_STATUS_CHANGE__OPEN = "Open";
        public const string REQUEST_WORK_BOX_STATUS_CHANGE__CLOSE = "Close";
        public const string REQUEST_WORK_BOX_STATUS_CHANGE__ARCHIVE = "Archive";
        public const string REQUEST_WORK_BOX_STATUS_CHANGE__DELETE = "Delete";
        public const string REQUEST_WORK_BOX_STATUS_CHANGE__REAPPLY_PERMISSIONS = "Reapply Permissions";
        public const string REQUEST_WORK_BOX_STATUS_CHANGE__DONE = "";
          

        public const string WORK_BOX_STATUS__REQUESTED = "Requested";
        public const string WORK_BOX_STATUS__CREATING = "Creating";
        public const string WORK_BOX_STATUS__CREATED = "Created";
        public const string WORK_BOX_STATUS__OPENING = "Opening";
        public const string WORK_BOX_STATUS__OPEN = "Open";
        public const string WORK_BOX_STATUS__CLOSING = "Closing";
        public const string WORK_BOX_STATUS__CLOSED = "Closed";
        public const string WORK_BOX_STATUS__ARCHIVING = "Archiving";
        public const string WORK_BOX_STATUS__ARCHIVED = "Archived";
        public const string WORK_BOX_STATUS__DELETING = "Deleting";
        public const string WORK_BOX_STATUS__DELETED = "Deleted";
        public const string WORK_BOX_STATUS__ERROR = "Error";


        public const string COLUMN_NAME__RECORDS_TYPE           = "Records Type";
        public const string COLUMN_NAME__OWNING_TEAM            = "Owning Team";
        public const string COLUMN_NAME__OWNING_INDIVIDUAL      = "Owning Individual";
        public const string COLUMN_NAME__INVOLVED_TEAMS         = "Involved Teams";
        public const string COLUMN_NAME__INVOLVED_INDIVIDUALS   = "Involved Individuals";
        public const string COLUMN_NAME__VISITING_TEAMS         = "Visiting Teams";
        public const string COLUMN_NAME__VISITING_INDIVIDUALS   = "Visiting Individuals";

        public const string COLUMN_NAME__FUNCTIONAL_AREA = "Functional Area";
        public const string COLUMN_NAME__PROTECTIVE_ZONE = "Protective Zone";
        public const string COLUMN_NAME__SERIES_TAG = "Series Tag";
        public const string COLUMN_NAME__SUBJECT_TAGS = "Subject Tags";
        public const string COLUMN_NAME__REFERENCE_DATE = "Reference Date";
        public const string COLUMN_NAME__REFERENCE_ID = "Reference ID";
        public const string COLUMN_NAME__ORIGINAL_FILENAME = "Original Filename";
        public const string COLUMN_NAME__SCAN_DATE = "Scan Date";


        public const string TERM_STORE_NAME = "Managed Metadata Service";
        //public const string TERM_STORE_NAME = "Connection to: Managed Metadata Service";

        public const string TERM_STORE_GROUP_NAME = "Islington Council";

        public const string TERM_SET_NAME__RECORDS_TYPES = "Records Types";
        public const string TERM_SET_NAME__TEAMS = "Teams";
        public const string TERM_SET_NAME__FUNCTIONAL_AREAS = "Functional Areas";
        public const string TERM_SET_NAME__SERIES_TAGS = "Series Tags";
        public const string TERM_SET_NAME__SUBJECT_TAGS = "Subject Tags";


        public const string USER_PROFILE_PROPERTY__WORK_BOX_LAST_VISITED_GUID = "WorkBoxLastVisitedGuid";
        public const string USER_PROFILE_PROPERTY__MY_RECENTLY_VISITED_WORK_BOXES = "MyRecentlyVisitedWorkBoxes";
        public const string USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES = "MyFavouriteWorkBoxes";
        public const string USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD = "MyWorkBoxClipboard";
        public const string USER_PROFILE_PROPERTY__MY_UNPROTECTED_WORK_BOX_URL = "MyUnprotectedWorkBoxUrl";        

        public const string PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE = "Public Web Site";
        public const string PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_EXTRANET = "Public Extranet";
        public const string PUBLISHING_OUT_DESTINATION_TYPE__IZZI_INTRANET = "izzi Intranet";
        public const string PUBLISHING_OUT_DESTINATION_TYPE__RECORDS_LIBRARY = "Records Library";
        public const string PUBLISHING_OUT_DESTINATION_TYPE__WORK_BOX = "Work Box";
        public const string PUBLISHING_OUT_DESTINATION_TYPE__USER_DEFINED_DESTINATION = "User Defined Destination";

        // To be deleted soon:
        public const string TEAM_SITE_PROPERTY__TERM_GUID = "wbf__team_site__term_guid";


        public const string ICON_16_IMAGE_URL = "/_layouts/images/WorkBoxFramework/work-box-16.png";
        public const string ICON_32_IMAGE_URL = "/_layouts/images/WorkBoxFramework/work-box-32.png";
        public const string ICON_48_IMAGE_URL = "/_layouts/images/WorkBoxFramework/work-box-48.png";


        public const string RELATION_TYPE__DYNAMIC = "Dynamic";
        public const string RELATION_TYPE__CHILD = "Child";
        public const string RELATION_TYPE__PARENT = "Parent";
        public const string RELATION_TYPE__MANUAL_LINK = "Manual Link";


        public const string WORK_BOX_DOCUMENT_CONTENT_TYPE = "Islington Document";
        public const string WORK_BOX_RECORD_CONTENT_TYPE = "Islington Record";


        #endregion


        #region Private Variables

        private bool _updateMustRedoPermissions = false;

        private WBTaxonomy _teams;
        public WBTaxonomy Teams
        {
            get 
            { 
                if (_teams == null)
                {
                    _teams = WBTaxonomy.GetTeams(Site);
                }
                return _teams;
            }
        }

        private WBTaxonomy _recordsTypes;
        public WBTaxonomy RecordsTypes
        {
            get 
            { 
                if (_recordsTypes == null)
                {
                    _recordsTypes = WBTaxonomy.GetRecordsTypes(Site);
                }
                return _recordsTypes;
            }
        }


        #endregion

        #region Constructors and Factories

        private bool _webNeedsDisposing = true;
        private bool _siteNeedsDisposing = true;

        /// <summary>
        /// If the SPWeb of the given SPContext is a work box then an appropriate WorkBox object is returned
        /// otherwise null is returned. You should use this method to potentially create WorkBox objects when
        /// you are unsure whether or not the SPWeb object that the user is on is a work box or not.         
        /// </summary>
        /// <param name="context">The SPContext whose SPWeb will be checked and conditionally wrapped in a WorkBox object.</param>
        /// <returns>A WorkBox object if the SPWeb of the given SPContext is a work box or null otherwise.</returns>
        public static WorkBox GetIfWorkBox(SPContext context)
        {
            if (isWebAWorkBox(context.Web))
            {
                return new WorkBox(context);
            }
            else
            {
                return null;
            }
        }

        public WorkBox(SPContext context)
        {
            _item = null;
            _collection = null;

            _web = context.Web;
            _webNeedsDisposing = false;

            _site = context.Site;
            _siteNeedsDisposing = false;

            _useable = true;
        }

        public WorkBox(WBCollection collection, SPListItem item)
        {
            _item = item;
            _collection = collection;
            _collectionNeedsDisposing = false;

            _web = null;
            _webNeedsDisposing = false;

            _site = collection.Site;
            _siteNeedsDisposing = false;

            _useable = true;
        }

        public WorkBox(WBCollection collection, int listItemID)
        {
            _item = collection.List.GetItemById(listItemID);
            _collection = collection;
            _collectionNeedsDisposing = false;

            _web = null;
            _webNeedsDisposing = false;

            _site = collection.Site;
            _siteNeedsDisposing = false;

            _useable = true;
        }

        public WorkBox(String workBoxURL)
        {
            _item = null;
            _collection = null;

            _site = new SPSite(workBoxURL);
            _siteNeedsDisposing = true;

            _web = _site.OpenWeb();
            _webNeedsDisposing = true;

            if (!isWebAWorkBox(_web)) throw new Exception("You can only use the WorkBox(String workBoxURL) constructor when you know you are using a genuine work box URL");

            _useable = true;
        }

        #endregion

        #region Properties

        private bool _useable = false;
        public bool Usable { get { return _useable; } }

        private SPSite _site = null;
        public SPSite Site { get { return _site; } }

        private SPWeb _web = null;
        public SPWeb Web
        {
            get
            {
                if (!_useable) WBUtils.shouldThrowError("(From WorkBox.Web) The WorkBox object is not in a usable state, it's probably been Disposed().");
                if (_web == null) 
                {
                    if (_item == null) WBUtils.shouldThrowError("(From WorkBox.Web) _web and _item shouldn't both be null!!");

                    WBLogging.WorkBoxes.Verbose("In WorkBox.Web : getting Web where _web == null - so loading a new SPWeb");

                    if (WebExists)
                    {
                        string guidString = _item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_GUID);
                        if (guidString == "") WBUtils.shouldThrowError("(From WorkBox.Web) the GUID for the work box web was blank");

                        if (this.Site == null) WBUtils.shouldThrowError("(From WorkBox.Web) the Site for the work box web was null");

                        _web = this.Site.OpenWeb(new Guid(guidString));
                        _webNeedsDisposing = true;
                    }
                }
                return _web;
            }
            private set 
            {
                WBLogging.WorkBoxes.Verbose("In WorkBox.Web : setting Web with value.Title = " + value.Title);

                _web = value; 
            }
        }

        private bool _collectionNeedsDisposing = false;
        private WBCollection _collection = null;
        public WBCollection Collection
        {
            get
            {
                if (!_useable) WBUtils.shouldThrowError("(From WorkBox.Collection) The WorkBox object is not in a usable state, it's probably been Disposed().");
                if (_collection == null)
                {
                    string collectionWebGuidString = Web.WBxGetProperty(WORK_BOX_PROPERTY__COLLECTION_WEB_GUID);

                    if (collectionWebGuidString == "")
                    {
                        WBUtils.shouldThrowError("Not sure why we've got here");
                    }
                    else
                    {
                        _collection = new WBCollection(this.Site, new Guid(collectionWebGuidString));

                        _collectionNeedsDisposing = true;
                    }
                }
                return _collection;
            }
        }

        private SPListItem _item = null;
        public SPListItem Item
        {
            get
            {
                if (!_useable) WBUtils.shouldThrowError("(From WorkBox.Item) The WorkBox object is not in a usable state, it's probably been Disposed().");
                if (_item == null)
                {
                    if (_web == null) WBUtils.shouldThrowError("(From Item) _item and _web shouldn't both be null!!");

                    string itemIDString = _web.WBxGetProperty(WORK_BOX_PROPERTY__METADATA_ITEM_ID);

                    if (itemIDString == "")
                    {
                        WBLogging.WorkBoxes.Verbose("Error finding parent metadata item: workBoxMetadataItemID = " + itemIDString);
                    }
                    else
                    {
                        _item = this.Collection.List.GetItemById(int.Parse(itemIDString));
                        if (_item == null)
                        {
                            WBLogging.WorkBoxes.Verbose("Couldn't find the workBoxMetadataItem with ID = " + itemIDString);
                        }

                    }
                }

                return _item;
            }
        }

        public bool WebExists
        {
            get
            {
                if (_web != null) return true;
                if (HasBeenCreated && !HasBeenDeleted) return true;
                return false;
            }
        }

        public String CollectionWebGUIDString
        {
            get { return Web.WBxGetProperty(WORK_BOX_PROPERTY__COLLECTION_WEB_GUID); }
            private set { Web.WBxSetProperty(WORK_BOX_PROPERTY__COLLECTION_WEB_GUID, value); }
        }

        public String CollectionListGUIDString
        {
            get { return Web.WBxGetProperty(WORK_BOX_PROPERTY__COLLECTION_LIST_GUID); }
            private set { Web.WBxSetProperty(WORK_BOX_PROPERTY__COLLECTION_LIST_GUID, value); }
        }

        public int MetadataItemID
        {
            get { return Web.WBxGetIntProperty(WORK_BOX_PROPERTY__METADATA_ITEM_ID); }
            private set { Web.WBxSetIntProperty(WORK_BOX_PROPERTY__METADATA_ITEM_ID, value); }
        }

        public String DocumentLibraryGUIDString
        {
            get { return Web.WBxGetProperty(WORK_BOX_PROPERTY__DOCUMENT_LIBRARY_GUID); }
            private set { Web.WBxSetProperty(WORK_BOX_PROPERTY__DOCUMENT_LIBRARY_GUID, value); }
        }

        public SPList LinkedWorkBoxesList
        {
            get
            {
                SPList list = Web.Lists.TryGetList(LIST_NAME__LINKED_WORK_BOXES);

                if (list == null)
                {
                    list = createLinkedWorkBoxesList();
                }

                return list;
            }
        }



//        private bool _needsUpdating = false;
//        public bool NeedsUpdating { get { return _needsUpdating; } }

        #endregion

        #region Work Box Metadata Properties

        public String Status
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_STATUS); }
            set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_STATUS, value); }
        }

        public bool IsInErrorStatus { get { return Status.Equals(WorkBox.WORK_BOX_STATUS__ERROR); } }

        public bool IsOpen { get { return (Status.Equals(WORK_BOX_STATUS__OPEN)); } }

        public bool IsClosed 
        {
            get { return (Status.Equals(WORK_BOX_STATUS__CLOSED) || Status.Equals(WORK_BOX_STATUS__ARCHIVED)); } 
        }

        public bool IsArchived
        {
            get
            {
                throw new NotImplementedException();
                // return (Status.Equals(WORK_BOX_STATUS__ARCHIVED));
            }
        }

        public bool IsDeleted { get { return (Status.Equals(WORK_BOX_STATUS__DELETED)); } }

        public String StatusChangeRequest
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_STATUS_CHANGE_REQUEST); }
            set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_STATUS_CHANGE_REQUEST, value); }
        }

        public String ShortTitle
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_SHORT_TITLE); }
            set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_SHORT_TITLE, value); }
        }

        public String Title
        {
            get { return Item.WBxGetColumnAsString("Title"); }
            set { Item.WBxSetColumnAsString("Title", value); }
        }

        public String ErrorMessage
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_ERROR_MESSAGE); }
            set 
            { 
                if (value != null && value != "") 
                {
                    Status = WorkBox.WORK_BOX_STATUS__ERROR;
                    StatusChangeRequest = WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__DONE;
                }
                Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_ERROR_MESSAGE, value); 
            }
        }

        public String LocalIDAsString
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_LOCAL_ID); }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_LOCAL_ID, value); }
        }

        public String UniqueID
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_UNIQUE_ID); }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_UNIQUE_ID, value); }
        }

        // Not sure about this one yet!!
        public String LinkUIControlValue
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_LINK); }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_LINK, value); }
        }

        public String GUIDString
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_GUID); }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_GUID, value); }
        }

        public String Url
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_URL); }
            private set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_URL, value); }
        }

        private WBRecordsType _recordsType;
        public WBRecordsType RecordsType
        {
            get
            {
                if (_recordsType == null)
                {
                    string recordsTypeUIControlValue = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__RECORDS_TYPE);
                    if (recordsTypeUIControlValue == "") return null;
                    _recordsType = new WBRecordsType(RecordsTypes, recordsTypeUIControlValue);
                }
                return _recordsType;
            }
            set
            {
                Item.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__RECORDS_TYPE, value);
                _recordsType = value;
                _updateMustRedoPermissions = true;
            }

        }


        private WBTeam _owningTeam;
        public WBTeam OwningTeam
        {
            get
            {
                if (_owningTeam == null)
                {
                    string owningTeamUIControlValue = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__OWNING_TEAM);
                    if (owningTeamUIControlValue == "") return null;
                    _owningTeam = new WBTeam(Teams, owningTeamUIControlValue);
                }
                return _owningTeam;
            }
            set
            {
                Item.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__OWNING_TEAM, value);
                _owningTeam = value;
                _updateMustRedoPermissions = true;

                // Now to update the functional area of this work box according to the new owning team:
                Item.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, _owningTeam.InheritedFunctionalAreaUIControlValue);
            }

        }

        private WBTermCollection<WBTeam> _involvedTeams;
        public WBTermCollection<WBTeam> InvolvedTeams
        {
            get
            {
                if (_involvedTeams == null)
                {
                    _involvedTeams = Item.WBxGetMultiTermColumn<WBTeam>(Teams, WorkBox.COLUMN_NAME__INVOLVED_TEAMS);
                }
                return _involvedTeams;
            }
            set
            {
                Item.WBxSetMultiTermColumn<WBTeam>(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, value);
                _involvedTeams = value;
                _updateMustRedoPermissions = true;
            }

        }

        private List<SPUser> _involvedIndividuals = null;
        public List<SPUser> InvolvedIndividuals
        {
            get 
            {
                if (_involvedIndividuals == null)
                {
                    _involvedIndividuals = Item.WBxGetMultiUserColumn(WorkBox.COLUMN_NAME__INVOLVED_INDIVIDUALS);
                }

                return _involvedIndividuals;                
            }
            set { 
                Item.WBxSetMultiUserColumn(Web, WorkBox.COLUMN_NAME__INVOLVED_INDIVIDUALS, value);
                _involvedIndividuals = value;
                _updateMustRedoPermissions = true;
            } 
        }

        private WBTermCollection<WBTeam> _visitingTeams;
        public WBTermCollection<WBTeam> VisitingTeams
        {
            get
            {
                if (_visitingTeams == null)
                {
                    _visitingTeams = Item.WBxGetMultiTermColumn<WBTeam>(Teams, WorkBox.COLUMN_NAME__VISITING_TEAMS);
                }
                return _visitingTeams;
            }
            set
            {
                Item.WBxSetMultiTermColumn<WBTeam>(WorkBox.COLUMN_NAME__VISITING_TEAMS, value);
                _visitingTeams = value;
                _updateMustRedoPermissions = true;
            }

        }

        private List<SPUser> _visitingIndividuals = null;
        public List<SPUser> VisitingIndividuals
        {
            get
            {
                if (_visitingIndividuals == null)
                {
                    _visitingIndividuals = Item.WBxGetMultiUserColumn(WorkBox.COLUMN_NAME__VISITING_INDIVIDUALS);
                }

                return _visitingIndividuals;
            }
            set
            {
                Item.WBxSetMultiUserColumn(Web, WorkBox.COLUMN_NAME__VISITING_INDIVIDUALS, value);
                _visitingIndividuals = value;
                _updateMustRedoPermissions = true;
            }
        }


        private WBTemplate _template = null;
        public WBTemplate Template
        {
            get 
            {
                if (_template == null)
                {
                    string typeUIControlValue = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE);
                    if (typeUIControlValue != "")
                    {
                        SPFieldLookupValue lookupValue = new SPFieldLookupValue(typeUIControlValue);
                        _template = new WBTemplate(Collection, lookupValue.LookupId);
                    }

                }
                return _template;
            }
            set 
            { 
                _template = value;
                SPFieldLookupValue lookupValue = new SPFieldLookupValue(_template.ID, _template.Title);
                Item[WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE] = lookupValue;
            }
        }

        public SPDocumentLibrary DocumentTemplates
        {
            get
            {
                if (Template == null) return null;
                return Template.DocumentTemplates;
            }
        }

        // Not sure how well this basic implementation will work
        public DateTime DateCreated
        {
            get { return (DateTime)Item[COLUMN_NAME__WORK_BOX_DATE_CREATED]; } 
            set { Item[COLUMN_NAME__WORK_BOX_DATE_CREATED] = value; } 
        }

        public bool HasBeenCreated { get { return Item.WBxColumnHasValue(COLUMN_NAME__WORK_BOX_DATE_CREATED); } } 

        public DateTime DateDeleted
        {
            get { return (DateTime)Item[COLUMN_NAME__WORK_BOX_DATE_DELETED]; } 
            set { Item[COLUMN_NAME__WORK_BOX_DATE_DELETED] = value; } 
        }

        public bool HasBeenDeleted { get { return Item.WBxColumnHasValue(COLUMN_NAME__WORK_BOX_DATE_DELETED); } } 

        public DateTime DateLastOpened
        {
            get { return (DateTime)Item[COLUMN_NAME__WORK_BOX_DATE_LAST_OPENED]; } 
            set { Item[COLUMN_NAME__WORK_BOX_DATE_LAST_OPENED] = value; } 
        }

        public bool HasBeenOpened { get { return Item.WBxColumnHasValue(COLUMN_NAME__WORK_BOX_DATE_LAST_OPENED); } }
        
        public DateTime DateLastClosed
        {
            get { return (DateTime)Item[COLUMN_NAME__WORK_BOX_DATE_LAST_CLOSED]; }
            set { Item[COLUMN_NAME__WORK_BOX_DATE_LAST_CLOSED] = value; }
        }
        
        public DateTime DateLastModified
        {
            get { return (DateTime)Item[COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED]; }
            set { Item[COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED] = value; }
        }


        public bool HasBeenClosed { get { return Item.WBxColumnHasValue(COLUMN_NAME__WORK_BOX_DATE_LAST_CLOSED); } }


        public bool HasRetentionEndDate
        {
            get { return Item.WBxColumnHasValue(COLUMN_NAME__WORK_BOX_RETENTION_END_DATE); }
        }

        public DateTime RetentionEndDate
        {
            get { return (DateTime)Item[COLUMN_NAME__WORK_BOX_RETENTION_END_DATE]; }
            private set { Item[COLUMN_NAME__WORK_BOX_RETENTION_END_DATE] = value; }
        }

        private void ResetRetentionEndDate()
        {
            Item[COLUMN_NAME__WORK_BOX_RETENTION_END_DATE] = null;
        }


        public List<WBAuditLogEntry> AuditLog
        {
            get { return WBAuditLogEntry.CreateListOfEntries(Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_AUDIT_LOG)); }
        }

        public bool ReferenceDateHasValue
        {
               get {
                   return Item.WBxColumnHasValue(COLUMN_NAME__REFERENCE_DATE);
                }
        }
        
        public DateTime ReferenceDate
        {
            get { 
                if (Item.WBxColumnHasValue(COLUMN_NAME__REFERENCE_DATE))
                    return (DateTime)Item[COLUMN_NAME__REFERENCE_DATE];
                return DateTime.Now;
            }
            set { Item[COLUMN_NAME__REFERENCE_DATE] = value; }
        }


        public String ReferenceID
        {
            get { return Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID); }
            set { Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID, value); }
        }

        public int CachedListItemID
        {
            get { return Item.WBxGetColumnAsInt(WorkBox.COLUMN_NAME__WORK_BOX_CACHED_LIST_ITEM_ID, -1); }
            set {
                if (value == -1)
                {
                    Item[WorkBox.COLUMN_NAME__WORK_BOX_CACHED_LIST_ITEM_ID] = null;
                }
                else
                {
                    Item[WorkBox.COLUMN_NAME__WORK_BOX_CACHED_LIST_ITEM_ID] = value;
                }
            }
        }


        #endregion

        #region Properties for Work Box Buttons and Dialogs
        private string _urlForViewPropertiesDialog = null;
        public String UrlForViewPropertiesDialog
        {
            get
            {
                if (!EnableViewPropertiesButton) return "";
                if (_urlForViewPropertiesDialog == null)
                {
                    _urlForViewPropertiesDialog = processDialogUrl(Collection.UrlForViewPropertiesDialog);
                }
                return _urlForViewPropertiesDialog;
            }
        }

        public bool EnableViewPropertiesButton
        {
            get { return true; }
        }

        private string _urlForEditPropertiesDialog = null;
        public String UrlForEditPropertiesDialog
        {
            get
            {
                if (!EnableEditPropertiesButton) return "";
                if (_urlForEditPropertiesDialog == null)
                {
                    _urlForEditPropertiesDialog = processDialogUrl(Collection.UrlForEditPropertiesDialog);
                }
                return _urlForEditPropertiesDialog;
            }
        }

        public bool EnableEditPropertiesButton
        {
            get
            {
                if (!IsOpen) return false;

                SPUser currentUser = Web.CurrentUser;

                if (currentUser == null) return false;

                if (CurrentUserIsBusinessAdmin() || CurrentUserIsSystemAdmin()) return true;

                if (CurrentUserIsOwner() && Collection.CanOwnerEditProperties) return true;

                return false;
            }
        }



        private string _urlForViewAllInvolvedDialog = null;
        public String UrlForViewAllInvolvedDialog
        {
            get
            {
                if (!EnableViewAllInvolvedButton) return "";
                if (_urlForViewAllInvolvedDialog == null)
                {
                    _urlForViewAllInvolvedDialog = processDialogUrl(Collection.UrlForViewAllInvolvedDialog);
                }
                return _urlForViewAllInvolvedDialog;
            }
        }

        public bool EnableViewAllInvolvedButton
        {
            get { return true; }
        }

        private string _urlForInviteTeamDialog = null;
        public String UrlForInviteTeamDialog
        {
            get
            {
                if (!EnableInviteTeamButton) return "";
                if (_urlForInviteTeamDialog == null)
                {
                    _urlForInviteTeamDialog = processDialogUrl(Collection.UrlForInviteTeamDialog);
                }
                return _urlForInviteTeamDialog;
            }
        }

        public bool EnableInviteTeamButton
        {
            get
            {
                if (!IsOpen) return false;

                SPUser currentUser = Web.CurrentUser;

                if (currentUser == null) return false;

                if (CurrentUserIsBusinessAdmin() || CurrentUserIsSystemAdmin() || CurrentUserIsOwner()) return true;

                return false;
            }
        }

        private string _urlForInviteIndividualDialog = null;
        public String UrlForInviteIndividualDialog
        {
            get
            {
                if (!EnableInviteIndividualButton) return "";
                if (_urlForInviteIndividualDialog == null)
                {
                    _urlForInviteIndividualDialog = processDialogUrl(Collection.UrlForInviteIndividualDialog);
                }
                return _urlForInviteIndividualDialog;
            }
        }

        public bool EnableInviteIndividualButton
        {
            get 
            {
                if (!IsOpen) return false;

                SPUser currentUser = Web.CurrentUser;

                if (currentUser == null) return false;

                if (CurrentUserIsBusinessAdmin() || CurrentUserIsSystemAdmin() || CurrentUserIsOwner()) return true;

                return false;
            }
        }

        private string _urlForChangeOwnerDialog = null;
        public String UrlForChangeOwnerDialog
        {
            get
            {
                if (!EnableChangeOwnerButton) return "";
                if (_urlForChangeOwnerDialog == null)
                {
                    _urlForChangeOwnerDialog = processDialogUrl(Collection.UrlForChangeOwnerDialog);
                }
                return _urlForChangeOwnerDialog;
            }
        }

        public bool EnableChangeOwnerButton
        {
            get 
            {
                if (!IsOpen) return false;

                SPUser currentUser = Web.CurrentUser;

                if (currentUser == null) return false;

                if (CurrentUserIsBusinessAdmin() || CurrentUserIsSystemAdmin()) return true;

                if (CurrentUserIsOwner() && Collection.CanOwnerChangeOwner) return true;

                return false;
            }
        }

        private string _urlForCloseDialog = null;
        public String UrlForCloseDialog
        {
            get
            {
                if (_urlForCloseDialog == null)
                {
                    _urlForCloseDialog = processDialogUrl(Collection.UrlForCloseDialog);
                }
                return _urlForCloseDialog;
            }
        }

        public bool EnableCloseButton
        {
            get {
                if (UrlForCloseDialog == "") return false;

                if (!IsOpen) return false;

                SPUser currentUser = Web.CurrentUser;

                if (currentUser == null) return false;

                if (CurrentUserIsBusinessAdmin() || CurrentUserIsSystemAdmin()) return true;

                if (CurrentUserIsOwner() && Collection.CanOwnerClose) return true; 

                return false; 
            }
        }

        private string _urlForReOpenDialog = null;
        public String UrlForReOpenDialog
        {
            get
            {
                if (_urlForReOpenDialog == null)
                {
                    _urlForReOpenDialog = processDialogUrl(Collection.UrlForReOpenDialog);
                }
                return _urlForReOpenDialog;
            }
        }

        public bool EnableReOpenButton
        {
            get
            {
                if (UrlForReOpenDialog == "") return false;

                if (Status != WORK_BOX_STATUS__CLOSED) return false;

                SPUser currentUser = Web.CurrentUser;

                if (currentUser == null) return false;

                if (CurrentUserIsBusinessAdmin() || CurrentUserIsSystemAdmin()) return true;

                if (CurrentUserIsOwner() && Collection.CanOwnerReOpen) return true;

                return false;
            }
        }


        #endregion

        #region Static Methods
        public static bool isWebAWorkBox(SPWeb web)
        {
            return (web.AllProperties.ContainsKey(WorkBox.WORK_BOX_PROPERTY__METADATA_ITEM_ID)
                && !web.AllProperties[WorkBox.WORK_BOX_PROPERTY__METADATA_ITEM_ID].Equals(""));
        }
        #endregion

        #region Private Methods

        private String processDialogUrl(String url)
        {
            return url.WBxReplaceTokens(this);
        }

        /*
        private String processDialogUrlFromWBCollection(String collectionPropertyKey)
        {
            String urlTemplate = WBUtils.safeGetPropertyAsString(this.Collection.Web, collectionPropertyKey);
            return processDialogUrl(urlTemplate);
        }
         */
        #endregion

        #region Public Methods

        public WBAction GetAction(String actionKey)
        {
            WBAction action = Collection.GetAction(actionKey);

            action.SpecialiseForCurrentContext(this);

            return action;
        }

        public Dictionary<String, WBAction> GetAllActions()
        {
            List<String> actionKeys = WBAction.GetKeysForEditableRibbonTabButtons();

            Dictionary<String, WBAction> allActions = new Dictionary<String, WBAction>();

            foreach (String actionKey in actionKeys)
            {
                WBAction action = this.GetAction(actionKey);
                allActions.Add(actionKey, action);
            }

            return allActions;
        }


        internal void JustUpdate()
        {
            if (_item != null)
            {
                _item.Update();
                WBLogging.WorkBoxes.Verbose("In WorkBox.JustUpdate() Done the _item update");
            }
            else
            {
                WBLogging.WorkBoxes.Verbose("In WorkBox.JustUpdate() _item was null");
            }

            if (_web != null)
            {
                _web.Update();
                WBLogging.WorkBoxes.Verbose("In WorkBox.JustUpdate() Done the _web update:");
            }
            else
            {
                WBLogging.WorkBoxes.Verbose("In WorkBox.JustUpdate() _web was null");
            }
        }


        internal int UpdateCachedDetails(SPList cachedDetailsList)
        {
            int cachedListItemID = CachedListItemID;

            SPListItem cachedItem = null;                
            if (cachedListItemID >= 0)
            {                    
                try                    
                {
                    cachedItem = cachedDetailsList.GetItemById(cachedListItemID);                    
                }                    
                catch (Exception exception)                
                {                
                    // So the list item ID appear to be out of date:                    
                    cachedListItemID = -1;                    
                    cachedItem = null;                    
                }
            }
                        
            // Check if we're meant to be cacheing the details of this work box:
            if (!IsOpen || !RecordsType.CacheDetailsForOpenWorkBoxes)                         
            {
                // If we've found a previous cached item we should delete it:
                if (cachedItem != null) cachedItem.Delete();

                // and now the list item ID should be set back to the 'non-value' of -1:
                return -1;
            }

            if (cachedItem == null)
            {
                cachedItem = cachedDetailsList.AddItem();
            }


            WBColumn[] columnsToSet = { 
                                        WBColumn.Title,
                                        WBColumn.WorkBoxStatus,
                                        WBColumn.WorkBoxURL,
                                        WBColumn.RecordsType, 
                                        WBColumn.FunctionalArea, 
                                        WBColumn.SubjectTags,
                                        WBColumn.SeriesTag,
                                        WBColumn.ReferenceID,
                                        WBColumn.ReferenceDate,
                                        WBColumn.OwningTeam,
                                        WBColumn.InvolvedTeams,
                                        WBColumn.VisitingTeams,
                                        WBColumn.WorkBoxDateCreated,
                                        WBColumn.WorkBoxDateLastModified,
                                        WBColumn.WorkBoxDateLastVisited,
                                        WBColumn.WorkBoxGUID
                                      };

            cachedItem.WBxSetFrom(Item, columnsToSet);

            cachedItem.Update();

            return cachedItem.ID;
        }

        internal int UpdateCachedDetails()
        {
            String cachedListUrl = WBFarm.Local.OpenWorkBoxesCachedDetailsListUrl;

            if (String.IsNullOrEmpty(cachedListUrl)) return -1;

            int cachedListItemID = CachedListItemID;

            if (cachedListItemID == -1 && ((!IsOpen || !RecordsType.CacheDetailsForOpenWorkBoxes))) return -1;

            bool digestOK = Collection.Web.ValidateFormDigest();            

            if (digestOK)            
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite elevatedCacheSite = new SPSite(cachedListUrl))
                    using (SPWeb elevatedCacheWeb = elevatedCacheSite.OpenWeb())
                    {
                        elevatedCacheWeb.AllowUnsafeUpdates = true;
                        SPList cachedDetailsList = elevatedCacheWeb.GetList(cachedListUrl);

                        cachedListItemID = UpdateCachedDetails(cachedDetailsList);

                        elevatedCacheWeb.AllowUnsafeUpdates = false;
                    }
                });
            }

            return cachedListItemID;
        }

        internal void RecentlyVisited(SPList cachedDetailsList, long ticksWhenVisited)
        {
            WBLogging.Debug("Call to RecentlyVisited for work box: " + Title);                

            long currentLastVisitedTicks = 0;
            if (Item.WBxHasValue(WBColumn.WorkBoxDateLastVisited))
            {
                DateTime currentLastVisited = (DateTime)Item.WBxGet(WBColumn.WorkBoxDateLastVisited);
                currentLastVisitedTicks = currentLastVisited.Ticks;
            }

            // If the work box more recently visited than is currently recorded then we'll update the details:
            if (ticksWhenVisited > currentLastVisitedTicks)
            {
                Item.WBxSet(WBColumn.WorkBoxDateLastVisited, new DateTime(ticksWhenVisited));
                if (_web != null)
                {
                    // We're going to update this value as often as we can - but it wont always be up to date.
                    Item[COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED] = _web.LastItemModifiedDate;
                }

                CachedListItemID = UpdateCachedDetails(cachedDetailsList);

                Item.Update();
            }
        }

        public void Update()
        {
            if (_item != null)
            {
                checkOwnersAreAlsoInvolved();

                WBLogging.WorkBoxes.Verbose("In WorkBox.Update(): Checked that owners are involved - now about to do the update:");

                if (_web != null)
                {
                    // We're going to update this value as often as we can - but it wont always be up to date.
                    Item[COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED] = _web.LastItemModifiedDate;
                }

                CachedListItemID = UpdateCachedDetails();

                _item.Update();

                WBLogging.WorkBoxes.Verbose("In WorkBox.Update() done the item update");

            }

            if (_web != null)
            {

                if (_updateMustRedoPermissions == true)
                {
                    WBLogging.WorkBoxes.Verbose("In WorkBox.Update() Found that we need to do a full re-do of the permissions!");

                    ReapplyPermissions();

                    WBLogging.WorkBoxes.Verbose("In WorkBox.Update() Finished re-doing the permissions.");
                }

                WBLogging.WorkBoxes.Verbose("In WorkBox.Update() About to do the web update:");
                _web.Update();
                WBLogging.WorkBoxes.Verbose("In WorkBox.Update() Done the web update:");
            }
            else
            {
                WBLogging.WorkBoxes.Verbose("In WorkBox.Update() _web was null");
            }
        }

        public void Dispose()
        {
            if (_web != null && _webNeedsDisposing) _web.Dispose();
            _web = null;

            if (_collection != null && _collectionNeedsDisposing) _collection.Dispose();
            _collection = null;

            if (_site != null && _siteNeedsDisposing) _site.Dispose();
            _site = null;

            _useable = false;
        }


        public void ClearStatusChangeRequest()
        {
            StatusChangeRequest = "";
            Item.Update();
        }

        public void SetStatusNow(String newStatus)
        {
            // We're just literally going to call an update on the item to ensure that the status change is saved:
            Status = newStatus;
            StatusChangeRequest = "";
            Item.Web.AllowUnsafeUpdates = true;
            Item.Update();
        }

        public void AddToErrorMessage(String extraErrorText) 
        {
            WBLogging.WorkBoxes.Verbose("Adding Error Message to work box: " + extraErrorText);

            ErrorMessage = ErrorMessage + "\n\n NEXT ERROR MESSAGE: \n" + extraErrorText;

            AuditLogEntry("Error occurred", extraErrorText);
        }

        /// <summary>
        /// This method is used to update the status of the work box, potentially closing it or deleting
        /// it as appropriate.
        /// </summary>
        public void UpdateStatus()
        {
            WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Updating status for work box: " + Title + " Current Status: " + Status);
            string initialStatus = Status;

            if (Status == WorkBox.WORK_BOX_STATUS__OPEN)
            {
                if (WebExists)
                {
                    Item[COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED] = Web.LastItemModifiedDate;
                }

                int unmodifiedTimeScalar = this.RecordsType.AutoCloseTimeScalar;

                WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): The records type is: " + RecordsType.Name);
                WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): The Unmodified Days Before Closing setting is: " + unmodifiedTimeScalar);
                

                if (unmodifiedTimeScalar > 0)
                {

                    DateTime triggerDate = DateTime.Now;
                    switch (this.RecordsType.AutoCloseTriggerDate)  
                    {
                        case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__LAST_MODIFIED_DATE:
                            {
                                triggerDate = this.DateLastModified;
                                break;
                            }

                        case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__REFERENCE_DATE:
                            {
                                triggerDate = this.ReferenceDate;
                                break;
                            }

                        case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__DATE_CREATED:
                            {
                                triggerDate = this.DateCreated;
                                break;
                            }

                        default:
                            {
                                WBLogging.WorkBoxes.Unexpected("WorkBox.UpdateStatus(): Trigger date has been set to value not yet implemented: " + this.RecordsType.AutoCloseTriggerDate);
                                return;
                            }
                    }

                    switch (this.RecordsType.AutoCloseTimeUnit)
                    {
                        case WBRecordsType.AUTO_CLOSE_TIME_UNIT__YEARS:
                            {
                                if (this.DateLastModified.AddYears(unmodifiedTimeScalar) < DateTime.Now)
                                {
                                    WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Auto-closing work box: " + Title);
                                    this.Close("Auto-closed because this work box has been unmodified for more than " + unmodifiedTimeScalar + " years.");
                                }
                                break;
                            }

                        case WBRecordsType.AUTO_CLOSE_TIME_UNIT__MONTHS:
                            {
                                if (this.DateLastModified.AddMonths(unmodifiedTimeScalar) < DateTime.Now)
                                {
                                    WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Auto-closing work box: " + Title);
                                    this.Close("Auto-closed because this work box has been unmodified for more than " + unmodifiedTimeScalar + " months.");
                                }
                                break;
                            }

                        case WBRecordsType.AUTO_CLOSE_TIME_UNIT__DAYS:
                            {
                                if (this.DateLastModified.AddDays(unmodifiedTimeScalar) < DateTime.Now)
                                {
                                    WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Auto-closing work box: " + Title);
                                    this.Close("Auto-closed because this work box has been unmodified for more than " + unmodifiedTimeScalar + " days.");
                                }
                                break;
                            }
                        case WBRecordsType.AUTO_CLOSE_TIME_UNIT__HOURS:
                            {
                                if (this.DateLastModified.AddHours(unmodifiedTimeScalar) < DateTime.Now)
                                {
                                    WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Auto-closing work box: " + Title);
                                    this.Close("Auto-closed because this work box has been unmodified for more than " + unmodifiedTimeScalar + " hours.");
                                }
                                break;
                            }
                        case WBRecordsType.AUTO_CLOSE_TIME_UNIT__MINUTES:
                            {
                                if (this.DateLastModified.AddMinutes(unmodifiedTimeScalar) < DateTime.Now)
                                {
                                    WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Auto-closing work box: " + Title);
                                    this.Close("Auto-closed because this work box has been unmodified for more than " + unmodifiedTimeScalar + " minutes.");
                                }
                                break;
                            }
                    }

                }
            }

            if (Status == WorkBox.WORK_BOX_STATUS__CLOSED)
            {
                if (HasRetentionEndDate)
                {
                    if (RetentionEndDate < DateTime.Now)
                    {
                        WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Auto-deleting work box: " + Title);
                        this.Delete("Auto-deleted because the retention end date had passed");
                    }
                }
            }

            Update();

            WBLogging.WorkBoxes.HighLevel("WorkBox.UpdateStatus(): Finished updating status: " + initialStatus + " -> " + Status);
        }

        public void ReapplyPermissions()
        {
            Web.AllowUnsafeUpdates = true;
            if (IsOpen) ApplyPermissionsForOpenStatus();
            else ApplyPermissionsForClosedStatus();
            _updateMustRedoPermissions = false;
        }

        // This method actually creates the sub-site for the work box
        public bool Create()
        {
            return Create(null);
        }

        public bool Create(String auditComment)
        {
            using (EventsFiringDisabledScope noevents = new EventsFiringDisabledScope())
            {
                Collection.Web.AllowUnsafeUpdates = true;

                // Make sure that any update events fired don't re-trigger the create event:
                StatusChangeRequest = "";

                if (HasBeenCreated)
                {
                    AddToErrorMessage("This work box has been created already so it cannot be created again.");
                    return false;
                }

                // This action request can only be made just after the item was requested:
                if (Status != "" && !Status.Equals(WorkBox.WORK_BOX_STATUS__REQUESTED))
                {
                    AddToErrorMessage("You can only 'create' a work box that is new or that has status 'requested'. This work box is in the status: " + Status);
                    return false;
                }

                SetStatusNow(WorkBox.WORK_BOX_STATUS__CREATING);

                WBLogging.WorkBoxes.Verbose("Trying to create a work box: " + Title);

                // Before we do anything we need to set the Work Box's template type:
                if (this.Template == null) this.Template = Collection.DefaultTemplate();

                // And then set the records type based on the template:
                this.RecordsType = Template.RecordsType(this.RecordsTypes);

                //this.RecordsType = Type.RecordsType;
                //Item[WorkBox.COLUMN_NAME__RECORDS_TYPE] = Template.Item[WorkBox.COLUMN_NAME__RECORDS_TYPE];


                // We'll only try to generate the ID if it's not already set:
                if (Collection.GenerateUniqueIDs && UniqueID == "")
                {
                    if (LocalIDAsString == "") GenerateLocalID();
                    GenerateUniqueID();
                }

                GenerateTitle();

                WBLogging.WorkBoxes.Verbose("Generated title is: " + Title);

                String workBoxWebSiteTitle = Title;

                WBRecordsType recordsTypeForName = Template.Item.WBxGetSingleTermColumn<WBRecordsType>(null, WBColumn.RecordsType.DisplayName);
                if (recordsTypeForName != null && recordsTypeForName.Name.Contains("Team meetings"))
                {
                    string referenceDateString = string.Format("({0}-{1}-{2})",
                            ReferenceDate.Year.ToString("D4"),
                            ReferenceDate.Month.ToString("D2"),
                            ReferenceDate.Day.ToString("D2"));

                    String teamName = "";
                            if (this.OwningTeam != null && !String.IsNullOrEmpty(this.OwningTeam.Name))
                            {
                                teamName = this.OwningTeam.Name + " ";
                            }

                    string seriesTag = "";

                    if (this.SeriesTag(null) != null)
                    {
                        seriesTag = SeriesTag(null).Name + " ";
                    }

                    workBoxWebSiteTitle = teamName + GetUniqueIDPrefix() + " " + seriesTag + referenceDateString;

                    WBLogging.WorkBoxes.Verbose("Creating a different site name for a team meeting:" + workBoxWebSiteTitle);
                }


                int prefixLength = Collection.UniqueIDPrefix.Length;
                int idLength = UniqueID.Length;

                if (idLength < prefixLength + 4) WBUtils.shouldThrowError("The unique ID is too short - probably wasn't set correctly: " + UniqueID);

                string dividerSiteName = GenerateDividerSiteName();

                if (this.Template.Template != null)
                {
                    WBLogging.WorkBoxes.Verbose("Using template name: " + this.Template.TemplateName);

                    string relativeUrlForNewWorkBoxSite = UniqueID;
                    string absoluteUrlForNewWorkBoxSite = Collection.Web.Url + "/" + dividerSiteName + "/" + relativeUrlForNewWorkBoxSite;
                    WBLogging.WorkBoxes.Verbose("New work box site's absolute URL will be: " + absoluteUrlForNewWorkBoxSite);

                    bool digestOK = Collection.Web.ValidateFormDigest();            
                    WBLogging.WorkBoxes.Verbose("The FormDigest validation value when refreshing teams was: " + digestOK);

                    if (digestOK)            
                    {                
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (SPSite elevatedSite = new SPSite(Collection.Site.ID))
                            using (SPWeb elevatedCollectionWeb = elevatedSite.OpenWeb(Collection.Web.ID))
                            {
                                elevatedSite.AllowUnsafeUpdates = true;
                                elevatedCollectionWeb.AllowUnsafeUpdates = true;

                                SPWeb dividerWeb;

                                if (elevatedCollectionWeb.Webs[dividerSiteName].Exists)
                                {
                                    dividerWeb = elevatedCollectionWeb.Webs[dividerSiteName];
                                    WBLogging.WorkBoxes.Verbose("Found the divider subsite: " + dividerSiteName);
                                }
                                else
                                {
                                    WBLogging.WorkBoxes.Verbose("Creating the divider subsite: " + dividerSiteName);
                                    //Note that the STS#1 template is for a blank site
                                    dividerWeb = elevatedCollectionWeb.Webs.Add(dividerSiteName, "Work Box Container " + dividerSiteName, "Please ignore this web site.", Convert.ToUInt32(WorkBox.LOCALE_ID_ENGLISH), "STS#1", false, false);
                                }

                                if (!dividerWeb.Webs[relativeUrlForNewWorkBoxSite].Exists)
                                {
                                    dividerWeb.AllowUnsafeUpdates = true;
                                    Web = dividerWeb.Webs.Add(relativeUrlForNewWorkBoxSite, workBoxWebSiteTitle, "", Convert.ToUInt32(WorkBox.LOCALE_ID_ENGLISH), Template.Template, false, false);
                                    //dividerWeb.AllowUnsafeUpdates = false;

                                    Web.AllowUnsafeUpdates = true;

                                    this.LinkUIControlValue = absoluteUrlForNewWorkBoxSite + ", Go to work box";
                                    this.GUIDString = Web.ID.ToString();
                                    this.Url = absoluteUrlForNewWorkBoxSite;
                                    this.DateCreated = DateTime.Today;

                                    // Let's make sure that the newly created list has the right information to link back to it's own metadata item:
                                    this.CollectionWebGUIDString = Collection.Web.ID.ToString();
                                    this.CollectionListGUIDString = Collection.List.ID.ToString();
                                    this.MetadataItemID = this.Item.ID;

                                    WBLogging.WorkBoxes.Verbose("Set all of the info to link work box web back to metadata item");

                                    SPDocumentLibrary documentLibrary = (SPDocumentLibrary)Web.Lists["Documents"];

                                    DocumentLibraryGUIDString = documentLibrary.ID.WBxToString();

                                    documentLibrary.BrowserFileHandling = SPBrowserFileHandling.Permissive;
                                    documentLibrary.Update();

                                    /* The documents library is now going to remain being called 'Documents'
                                    string documentsRootFolderName = Title + " - Documents";

                                    SPFolder rootFolder = documentLibrary.RootFolder;

                                    rootFolder.MoveTo(documentsRootFolderName);
                                    rootFolder.Update();
                                    */

                                    JustUpdate();
                                    //Web.AllowUnsafeUpdates = false;
                                }
                                else
                                {
                                    AddToErrorMessage("There is a conflict with the URL of the new work box: " + absoluteUrlForNewWorkBoxSite);
                                }

                            }
                        });

                    }
                    else
                    {
                        WBUtils.shouldThrowError("The form digest was not valid when trying to create - not sure why??");
                    }

                }
                else
                {
                    AddToErrorMessage("There doesn't appear to be a site template configured for this work box portal");
                }

            }
            // This last set of changes we will do outside of the 'no-events' scope in order
            // to fire a update change event that can be used by workflows etc.
            if (!IsInErrorStatus)
            {
                Status = WORK_BOX_STATUS__CREATED;
                AuditLogEntry("Work Box Created", auditComment);
                _updateMustRedoPermissions = false;
            }

            Web.AllowUnsafeUpdates = true;

            Web.RootFolder.WelcomePage = this.DocumentLibrary.RootFolder.Url; 
            Web.RootFolder.Update();


            Update();

            Collection.Web.AllowUnsafeUpdates = false;

            return IsInErrorStatus;
        }

        private String GenerateDividerSiteName()
        {
            return (Item.ID % 500).ToString("D000");
        }

        public void GenerateLocalID()
        {
            WBLogging.WorkBoxes.HighLevel("WorkBox.GenerateLocalID(): Generating local id for the work box with item ID = " + Item.ID);

            string localID = "";

            switch (RecordsType.WorkBoxLocalIDSource)
            {
                case WBRecordsType.LOCAL_ID_SOURCE__GENERATE_LOCAL_ID:
                    {
                        int offsetValue = Collection.InitialIDOffset;
                        int numberOfDigits = Collection.NumberOfDigitsInIDs;
                        if (numberOfDigits == 0)
                        {
                            numberOfDigits = WBRecordsType.NUMBER_OF_DIGITS_IN_GENERATED_LOCAL_IDS;
                        }
                        
                        int localIDValue = Item.ID + offsetValue;

                        localID = localIDValue.ToString("D" + numberOfDigits.ToString());
                        break;
                    }
                case WBRecordsType.LOCAL_ID_SOURCE__USE_REFERENCE_ID:
                    {
                        localID = ReferenceID;
                        break;
                    }

                case WBRecordsType.LOCAL_ID_SOURCE__USE_CURRENT_USER_LOGIN_NAME:
                    {
                        localID = SPContext.Current.Web.CurrentUser.LoginName.Replace(" ", "_");
                        break;
                    }
            }

            WBLogging.WorkBoxes.HighLevel("WorkBox.GenerateLocalID(): Generated local id: " + localID);
            SetLocalID(localID);
        }

        private String GetUniqueIDPrefix()
        {
            string prefix = RecordsType.WorkBoxUniqueIDPrefix;
            if (prefix == "") return Collection.UniqueIDPrefix;
            return prefix;
        }

        public void GenerateTitle()
        {
            // Start with a backup value:
            string generatedTitle = UniqueID;

            string referenceDateString = string.Format("({0}-{1}-{2})",
                            ReferenceDate.Year.ToString("D4"),
                            ReferenceDate.Month.ToString("D2"),
                            ReferenceDate.Day.ToString("D2"));
           
            switch (RecordsType.WorkBoxNamingConvention)
            {
                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_TITLE:
                    {
                        String teamAcronym = "";
                        if ( this.OwningTeam != null &&  !String.IsNullOrEmpty(this.OwningTeam.Acronym))
                        {
                            teamAcronym = this.OwningTeam.Acronym + " ";
                        }

                        string shortTitle = "";
                        if (!String.IsNullOrEmpty(ShortTitle))
                        {
                            shortTitle = " - " + ShortTitle;
                        }

                        generatedTitle = teamAcronym + GetUniqueIDPrefix() + shortTitle;

                        break;
                    }


                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__TEAM_TITLE:
                    {
                        String teamAcronym = "";
                        if (this.OwningTeam != null && !String.IsNullOrEmpty(this.OwningTeam.Acronym))
                        {
                            teamAcronym = this.OwningTeam.Acronym + " ";
                        }

                        string shortTitle = LocalIDAsString;
                        if (!String.IsNullOrEmpty(ShortTitle)) shortTitle = ShortTitle;

                        generatedTitle = teamAcronym + shortTitle;

                        break;

                    }

                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_OPTIONAL_SERIES_DATE:
                    {
                        String teamAcronym = "";
                        if (this.OwningTeam != null && !String.IsNullOrEmpty(this.OwningTeam.Acronym))
                        {
                            teamAcronym = this.OwningTeam.Acronym + " ";
                        }

                        string seriesTag = "";

                        if (this.SeriesTag(null) != null)
                        {
                           seriesTag  = SeriesTag(null).Name + " ";
                        }

                        generatedTitle = teamAcronym + GetUniqueIDPrefix() + " " + seriesTag + referenceDateString;

                        break;

                    }

                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_TITLE:
                    {
                        string shortTitle = LocalIDAsString;
                        if (!String.IsNullOrEmpty(ShortTitle)) shortTitle = ShortTitle;

                        generatedTitle = GetUniqueIDPrefix() + " - " + shortTitle;

                        break;
                    }


                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_LOCALID_TITLE:
                    {
                        generatedTitle = GetUniqueIDPrefix() + " " + LocalIDAsString;

                        if (ShortTitle != "")
                        {
                            generatedTitle += " - " + ShortTitle;
                        }
                        break;
                    }
                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_REFERENCEID_TITLE:
                    {
                        generatedTitle = GetUniqueIDPrefix() + " " + ReferenceID;

                        if (ShortTitle != "")
                        {
                            generatedTitle += " - " + ShortTitle;
                        }
                        break;
                    }
                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_DATE_OLD:
                    {
                        generatedTitle = GetUniqueIDPrefix() + " " + referenceDateString;
                        break;
                    }
                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_DATE_TITLE_OLD:
                    {
                        generatedTitle = GetUniqueIDPrefix() + " " + referenceDateString;

                        if (ShortTitle != "")
                        {
                            generatedTitle += " - " + ShortTitle;
                        }
                        break;
                    }
                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_DATE_REFERENCEID_TITLE_OLD:
                    {
                        generatedTitle = GetUniqueIDPrefix() + " " + referenceDateString + " " + ReferenceID;

                        if (ShortTitle != "")
                        {
                            generatedTitle += " - " + ShortTitle;
                        }
                        break;
                    }

                case WBRecordsType.WORK_BOX_NAMING_CONVENTION__PREFIX_SERIES_REFERENCEID_TITLE_OLD:
                    {
                        // We can pass in null because we're only retrieving the name from the UIControlValue
                        string seriesTag = SeriesTag(null).Name;

                        generatedTitle = GetUniqueIDPrefix() + " " + seriesTag + " " + ReferenceID;

                        if (ShortTitle != "")
                        {
                            generatedTitle += " - " + ShortTitle;
                        }
                        break;
                    }


            }

            


            Title = generatedTitle;
            WBLogging.WorkBoxes.Verbose("The work box naming convention used was: " + RecordsType.WorkBoxNamingConvention);
            WBLogging.WorkBoxes.Verbose("The work box generated title is: " + generatedTitle);
        }

        public void SetLocalID(string localID)
        {
            if (LocalIDAsString == "")
            {
                LocalIDAsString = localID;
            }
            else
            {
                throw new Exception("You cannot set the local ID when it already has a value: " + LocalIDAsString);
            }
        }

        /// <summary>
        /// The unique ID is created from the records types unique ID and the local ID string. 
        /// </summary>
        /// <remarks>
        /// A hyphen is used to delimit the end of the unique ID and then all spaces within the local ID are also replaced by hyphens. Not only
        /// does thia make it easy to retrieve the prefix from the unique ID but it also makes the whole unique ID suitable as an email address.
        /// </remarks>
        public void GenerateUniqueID()
        {
            UniqueID = GetUniqueIDPrefix() + "-" + LocalIDAsString.Replace(' ', '-');
        }

        public bool Open()
        {
            return Open(null);
        }

        public bool Open(String auditComment)
        {
            using (EventsFiringDisabledScope noevents = new EventsFiringDisabledScope())
            {
                // Make sure that any later update events fired don't re-trigger the open event:
                StatusChangeRequest = "";

                if (HasBeenDeleted)
                {
                    AddToErrorMessage("You cannot open again a work box that has been deleted.");
                    return false;
                }

                if (IsOpen)
                {
                    AddToErrorMessage("You cannot open again a work box that is already open.");
                    return false;
                }

                // If the work box hasn't even been created - then let's do that first:
                if (!HasBeenCreated)
                {
                    Create();
                }

                Web.AllowUnsafeUpdates = true;

                SetStatusNow(WORK_BOX_STATUS__OPENING);

                if (!IsInErrorStatus)
                {
                    if (this.OwningTeam == null) this.OwningTeam = Collection.DefaultOwningTeam;

                    ApplyPermissionsForOpenStatus();
                }
            }

            // This last set of changes we will do outside of the 'no-events' scope in order
            // to fire a update change event that can be used by workflows etc.
            if (!IsInErrorStatus)
            {
                Status = WORK_BOX_STATUS__OPEN;
                DateLastOpened = DateTime.Today;
                ResetRetentionEndDate();
                AuditLogEntry("Work Box Opened", auditComment);
                _updateMustRedoPermissions = false;
            }
            Update();

            Web.AllowUnsafeUpdates = false;

            return IsInErrorStatus;
        }

        internal void RefreshTeams()
        {
            bool digestOK = Web.ValidateFormDigest();
            WBLogging.WorkBoxes.Verbose("The FormDigest validation value when refreshing teams was: " + digestOK);

            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite elevatedSite = new SPSite(Site.ID))
                    {
//                        using (SPWeb elevatedWeb = elevatedSite.OpenWeb(Web.ID))
 //                       {

                            foreach (WBTeam team in Collection.SystemAdminTeams)
                            {
                                team.SyncSPGroup(elevatedSite);
                            }

                            foreach (WBTeam team in Collection.BusinessAdminTeams)
                            {
                                team.SyncSPGroup(elevatedSite);
                            }

                            OwningTeam.SyncSPGroup(elevatedSite);

                            foreach (WBTeam team in InvolvedTeams)
                            {
                                team.SyncSPGroup(elevatedSite);
                            }
   //                     }
                    }
                });

            }
            else
            {
                WBUtils.shouldThrowError("The form digest was not valid when trying to open - not sure why??");
            }


        }

        private void ApplyPermissionsForOpenStatus()
        {
            bool digestOK = Web.ValidateFormDigest();
            WBLogging.WorkBoxes.Verbose("The FormDigest validation value when opening was: " + digestOK);

            Guid workBoxGuid = Web.ID;

            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite elevatedSite = new SPSite(Site.ID))
                    {
                        using (SPWeb elevatedWeb = elevatedSite.OpenWeb(Web.ID))
                        {
                            bool previousState = elevatedWeb.AllowUnsafeUpdates;
                            elevatedWeb.AllowUnsafeUpdates = true;
                            elevatedSite.AllowUnsafeUpdates = true;

                            elevatedWeb.BreakRoleInheritance(false, false);
                            elevatedWeb.WBxRemoveAllPermissionBindings();
                            elevatedWeb.Update();

                            foreach (WBTeam team in Collection.SystemAdminTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.OpenPermissionLevelForSystemAdmin);
                            }

                            foreach (WBTeam team in Collection.BusinessAdminTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.OpenPermissionLevelForBusinessAdmin);
                            }

                            elevatedWeb.WBxAssignTeamWithRole(elevatedSite, OwningTeam, Collection.OpenPermissionLevelForOwner);

                            foreach (WBTeam team in InvolvedTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.OpenPermissionLevelForInvolved);
                            }

                            foreach (SPUser user in InvolvedIndividuals)
                            {
                                elevatedWeb.WBxAssignADNameWithRole(user.LoginName, Collection.OpenPermissionLevelForInvolved);
                            }

                            foreach (WBTeam team in VisitingTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.OpenPermissionLevelForVisitors);
                            }

                            foreach (SPUser user in VisitingIndividuals)
                            {
                                elevatedWeb.WBxAssignADNameWithRole(user.LoginName, Collection.OpenPermissionLevelForVisitors);
                            }

                            elevatedWeb.WBxAssignADNameWithRole(WBConstant.AD_GROUP__ALL_AUTHENTICATED_USERS, Collection.OpenPermissionLevelForEveryone);

                            if (Collection.UseFolderAccessGroupsPattern && Template.UseFolderGroupAccessPattern)
                            {
                                string[] folderNames = Collection.FolderAccessGroupsFolderNames.Split(';');

                                SPDocumentLibrary documents = (SPDocumentLibrary)elevatedWeb.Lists[new Guid(this.DocumentLibraryGUIDString)];
                                SPFolder rootFolder = documents.RootFolder;

                                foreach (string folderName in folderNames)
                                {
                                    SPFolder folder = rootFolder.WBxGetOrCreateSubFolder(folderName);

                                    string groupName = Collection.FolderAccessGroupsPrefix + " - " + folderName;
                                    string allFoldersGroupName = Collection.FolderAccessGroupsPrefix + " - All Folders";

                                    folder.Item.BreakRoleInheritance(true);
                                    folder.Item.WBxRemoveAllPermissionBindings();
                                    folder.Item.WBxAssignGroupWithRole(elevatedWeb, groupName, Collection.FolderAccessGroupPermissionLevel);
                                    folder.Item.WBxAssignGroupWithRole(elevatedWeb, allFoldersGroupName, Collection.AllFoldersAccessGroupPermissionLevel);
                                }
                            }


                            elevatedWeb.Update();
                            elevatedWeb.AllowUnsafeUpdates = previousState;
                        }
                    }
                });
            }
            else
            {
                WBUtils.shouldThrowError("The form digest was not valid when trying to open - not sure why??");
            }

            if (_webNeedsDisposing && _web != null) _web.Dispose();

            _web = Site.OpenWeb(workBoxGuid);
            _webNeedsDisposing = true;

        }

        public bool Close()
        {
            return Close(null);
        }

        public bool Close(String auditComment)
        {
            using (EventsFiringDisabledScope noevents = new EventsFiringDisabledScope())
            {
                // Make sure that any later update events fired don't re-trigger the close event:
                StatusChangeRequest = "";

                if (!IsOpen)
                {
                    AddToErrorMessage("You can only close work boxes that are in the 'open' status. This work box is in the status: " + Status);
                    return false;
                }

                WBLogging.WorkBoxes.Verbose("Setting status now to 'closing'");

                SetStatusNow(WORK_BOX_STATUS__CLOSING);

                WBLogging.WorkBoxes.Verbose("About to set the web site permission to closed:");

                ApplyPermissionsForClosedStatus();

                WBLogging.WorkBoxes.Verbose("Finished setting the web site permission to closed:");

            }

            // This last set of changes we will do outside of the 'no-events' scope in order
            // to fire a update change event that can be used by workflows etc.
            if (!IsInErrorStatus)
            {
                Status = WORK_BOX_STATUS__CLOSED;
                DateLastClosed = DateTime.Today;
                RetentionEndDate = calculateRetentionEndDate(DateTime.Now);
                AuditLogEntry("Work Box Closed", auditComment);
            }

            Update();

            return IsInErrorStatus;
        }

        private DateTime calculateRetentionEndDate(DateTime triggerDate)
        {
            WBRecordsType recordsType = this.RecordsType;

            // Just put here a default value:
            if (recordsType == null) return WBRecordsType.getPermanentDate();

            int scalar = recordsType.RetentionTimeScalar;

            switch (recordsType.RetentionTimeUnit)
            {
                case WBRecordsType.RETENTION_TIME_UNIT__PERMANENT:
                    return WBRecordsType.getPermanentDate();
                case WBRecordsType.RETENTION_TIME_UNIT__YEARS:
                    return triggerDate.AddYears(scalar);
                case WBRecordsType.RETENTION_TIME_UNIT__DAYS:
                    return triggerDate.AddDays(scalar);
                case WBRecordsType.RETENTION_TIME_UNIT__MONTHS:
                    return triggerDate.AddMonths(scalar);
                case WBRecordsType.RETENTION_TIME_UNIT__HOURS:
                    return triggerDate.AddHours(scalar);
                case WBRecordsType.RETENTION_TIME_UNIT__MINUTES:
                    return triggerDate.AddMinutes(scalar);
            }

            return WBRecordsType.getPermanentDate();
        }

        internal DateTime calculateAutoCloseDate()
        {
            WBRecordsType recordsType = this.RecordsType;

            // Just put here a default value:
            if (recordsType == null) return WBRecordsType.getPermanentDate();

            DateTime triggerDate = DateTime.Now;

            switch (recordsType.AutoCloseTriggerDate)
            {
                case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__NONE:
                    return WBRecordsType.getPermanentDate();
                case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__LAST_MODIFIED_DATE:
                    {
                        triggerDate = this.DateLastModified;
                        break;
                    }
                case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__REFERENCE_DATE:
                    {
                        triggerDate = this.ReferenceDate;
                        break;
                    }
                case WBRecordsType.AUTO_CLOSE_TRIGGER_DATE__DATE_CREATED:
                    {
                        triggerDate = this.DateCreated;
                        break;
                    }
            }                

            int scalar = recordsType.AutoCloseTimeScalar;

            switch (recordsType.AutoCloseTimeUnit)
            {
                case WBRecordsType.AUTO_CLOSE_TIME_UNIT__NONE:
                    return WBRecordsType.getPermanentDate();
                case WBRecordsType.AUTO_CLOSE_TIME_UNIT__YEARS:
                    return triggerDate.AddYears(scalar);
                case WBRecordsType.AUTO_CLOSE_TIME_UNIT__DAYS:
                    return triggerDate.AddDays(scalar);
                case WBRecordsType.AUTO_CLOSE_TIME_UNIT__MONTHS:
                    return triggerDate.AddMonths(scalar);
                case WBRecordsType.AUTO_CLOSE_TIME_UNIT__HOURS:
                    return triggerDate.AddHours(scalar);
                case WBRecordsType.AUTO_CLOSE_TIME_UNIT__MINUTES:
                    return triggerDate.AddMinutes(scalar);
            }

            return WBRecordsType.getPermanentDate();
        }


        private void ApplyPermissionsForClosedStatus()
        {

            bool digestOK = Web.ValidateFormDigest();
            WBLogging.WorkBoxes.Verbose("The FormDigest validation value when closing was: " + digestOK);

            Guid workBoxGuid = Web.ID;

            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite elevatedSite = new SPSite(Site.ID))
                    {
                        using (SPWeb elevatedWeb = elevatedSite.OpenWeb(Web.ID))
                        {
                            bool previousState = elevatedWeb.AllowUnsafeUpdates;
                            elevatedSite.AllowUnsafeUpdates = true;
                            elevatedWeb.AllowUnsafeUpdates = true;
                            elevatedWeb.BreakRoleInheritance(false, false);

                            elevatedWeb.WBxRemoveAllPermissionBindings();
                            elevatedWeb.Update();

                            foreach (WBTeam team in Collection.SystemAdminTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.ClosedPermissionLevelForSystemAdmin);
                            }

                            foreach (WBTeam team in Collection.BusinessAdminTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.ClosedPermissionLevelForBusinessAdmin);
                            }

                            elevatedWeb.WBxAssignTeamWithRole(elevatedSite, OwningTeam, Collection.ClosedPermissionLevelForOwner);

                            foreach (WBTeam team in InvolvedTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.ClosedPermissionLevelForInvolved);
                            }

                            foreach (SPUser user in InvolvedIndividuals)
                            {
                                elevatedWeb.WBxAssignADNameWithRole(user.LoginName, Collection.ClosedPermissionLevelForInvolved);
                            }

                            foreach (WBTeam team in VisitingTeams)
                            {
                                elevatedWeb.WBxAssignTeamWithRole(elevatedSite, team, Collection.ClosedPermissionLevelForVisitors);
                            }

                            foreach (SPUser user in VisitingIndividuals)
                            {
                                elevatedWeb.WBxAssignADNameWithRole(user.LoginName, Collection.ClosedPermissionLevelForVisitors);
                            }

                            elevatedWeb.WBxAssignADNameWithRole(WBConstant.AD_GROUP__ALL_AUTHENTICATED_USERS, Collection.ClosedPermissionLevelForEveryone);

                            if (Collection.UseFolderAccessGroupsPattern && Template.UseFolderGroupAccessPattern)
                            {
                                string[] folderNames = Collection.FolderAccessGroupsFolderNames.Split(';');

                                SPDocumentLibrary documents = (SPDocumentLibrary)elevatedWeb.Lists[new Guid(this.DocumentLibraryGUIDString)];
                                SPFolder rootFolder = documents.RootFolder;

                                foreach (string folderName in folderNames)
                                {
                                    SPFolder folder = rootFolder.WBxGetOrCreateSubFolder(folderName);

                                    folder.Item.ResetRoleInheritance();
                                }
                            }


                            elevatedWeb.Update();
                            elevatedWeb.AllowUnsafeUpdates = previousState;
                        }
                    }
                });
            }
            else
            {
                WBUtils.shouldThrowError("The form digest was not valid when closing - not sure why??");
            }


            // This is in order to re-load the SPWeb object cleanly after the permissions changes:
            if (_webNeedsDisposing && _web != null) _web.Dispose();

            _web = Site.OpenWeb(workBoxGuid);
            _webNeedsDisposing = true;

        }


        public bool Archive()
        {
            // For the moment we'll throw this exception because it really isn't implemented.
            throw new NotImplementedException();
/*
            // Make sure that any update events fired don't re-trigger the archive event:
            StatusChangeRequest = "";

            if (!Status.Equals(WORK_BOX_STATUS__CLOSED))
            {
                AddToErrorMessage("You can only archive work boxes that are in the 'closed' status. This work box is in the status: " + Status);
                return false;
            }

            Status = WORK_BOX_STATUS__ARCHIVED;
            Update();

            return IsInErrorStatus;
*/ 
        }

        public bool Delete()
        {
            return Delete(null);
        }

        public bool Delete(String auditComment)
        {
            using (EventsFiringDisabledScope noevents = new EventsFiringDisabledScope())
            {
                // Make sure that any update events fired don't re-trigger the delete event:
                StatusChangeRequest = "";

                if (!Status.Equals(WORK_BOX_STATUS__CLOSED) && !Status.Equals(WORK_BOX_STATUS__ARCHIVED))
                {
                    AddToErrorMessage("You can only delete work boxes that are in the 'closed' or 'archived' status. This work box is in the status: " + Status);
                    return false;
                }

                if (this.GUIDString == "")
                {
                    AddToErrorMessage("There doesn't appear to be a GUID defined for the work box, suggesting it hasn't been created - so can't be deleted: " + Title);
                    return false;
                }

                SetStatusNow(WORK_BOX_STATUS__DELETING);

                WBLogging.WorkBoxes.Verbose("Trying to delete a work box: " + Title);

                try
                {
                    Web.Delete();
                    if (_webNeedsDisposing) _web.Dispose();
                    _web = null;

                }
                catch (Exception Ex)
                {
                    AddToErrorMessage("An error occurred while trying to delete the work box: " + Ex.Message);
                }
            }

            // This last set of changes we will do outside of the 'no-events' scope in order
            // to fire a update change event that can be used by workflows etc.
            if (!IsInErrorStatus)
            {
                LinkUIControlValue = "";
                Status = WORK_BOX_STATUS__DELETED;
                DateDeleted = DateTime.Today;
                AuditLogEntry("Work Box Deleted", auditComment);
            }
            Update();

            return IsInErrorStatus;
        }

        private void checkOwnersAreAlsoInvolved()
        {
            WBLogging.WorkBoxes.Verbose("Checking owning team is also involved");

            TaxonomyFieldValue owningTeamFieldValue = Item[WorkBox.COLUMN_NAME__OWNING_TEAM] as TaxonomyFieldValue;

            if (owningTeamFieldValue != null)
            {
                WBLogging.WorkBoxes.Verbose("Found owning teams field value - so we can check if it's involved.");


                TaxonomyField involvedTeamsField = Item.Fields[WorkBox.COLUMN_NAME__INVOLVED_TEAMS] as TaxonomyField;

                WBLogging.WorkBoxes.Verbose("Found field - new release");

                TaxonomyFieldValueCollection involvedTeamsFieldValueCollection = null;

                if (Item[WorkBox.COLUMN_NAME__INVOLVED_TEAMS] == null)
                {
                    WBLogging.WorkBoxes.Verbose("Was null!!");
                    involvedTeamsFieldValueCollection = new TaxonomyFieldValueCollection(involvedTeamsField);
                }
                else
                {
                    WBLogging.WorkBoxes.Verbose("Found involved teams field value");
                    involvedTeamsFieldValueCollection = Item[involvedTeamsField.InternalName] as TaxonomyFieldValueCollection;
                }


                if (!involvedTeamsFieldValueCollection.ToString().Contains(owningTeamFieldValue.TermGuid))
                {
                    WBLogging.WorkBoxes.Verbose("Couldn't find it so adding it.");
                    involvedTeamsFieldValueCollection.Add(owningTeamFieldValue);

                    involvedTeamsField.SetFieldValue(Item, involvedTeamsFieldValueCollection);

                    _updateMustRedoPermissions = true;
                }
                WBLogging.WorkBoxes.Verbose("Checked owning team is also involved");
            }
            else
            {
                WBLogging.WorkBoxes.Verbose("Owning team is currently null - so cannot be checked if it's also invovled!");
            }
        }

        public void ClearErrorsAndResetStatus()
        {
            ErrorMessage = "";
            Status = WORK_BOX_STATUS__REQUESTED;
            if (HasBeenCreated) Status = WORK_BOX_STATUS__CREATED;
            if (HasBeenOpened) Status = WORK_BOX_STATUS__OPEN;
            if (HasBeenClosed && (DateTime.Compare(DateLastOpened, DateLastClosed) > 0))
            {
                Status = WORK_BOX_STATUS__CLOSED;
            }
            if (HasBeenDeleted) Status = WORK_BOX_STATUS__DELETED;
            _updateMustRedoPermissions = true;

            JustUpdate();
        }



        public void AuditLogEntry(String title)
        {
            AuditLogEntry(title, null);
        }

        // We're looking for the one called 'Documents' 
        private SPDocumentLibrary _documentLibrary;
        public SPDocumentLibrary DocumentLibrary
        {
            get
            {
                if (_documentLibrary == null)
                {
                    if (!HasBeenCreated) return null;

                    string guidString = DocumentLibraryGUIDString;

                    if (guidString != "")
                    {
                        _documentLibrary = (SPDocumentLibrary)Web.Lists[new Guid(guidString)];
                    }
                    else
                    {
                        // So as a backup we'll just try to pull a library that has the title 'Documents'
                        foreach (SPList list in Web.Lists)
                        {
                            if (list.WBxIsDocumentLibrary())
                            {
                                if (list.Title == "Documents")
                                {
                                    _documentLibrary = (SPDocumentLibrary)list;
                                    return _documentLibrary;
                                }
                            }
                        }
                    }

                }
                return _documentLibrary;
            }
        }

        private SPUser getCurrentUserOrLastModifiedByUser()
        {
            SPUser currentUser = null;

            if (SPContext.Current != null)
            {
                currentUser = SPContext.Current.Web.CurrentUser;
            }
            else
            {
                string colvalue = Item.WBxGetColumnAsString("Modified By");

                string[] split = colvalue.Split('#');

                string userLogin = null;
                if (split.Length == 2)
                {
                    userLogin = split[1];
                }


                if (userLogin != null && userLogin != "")
                {
                    WBLogging.WorkBoxes.Verbose("Found modified by user called: " + userLogin);

                    using (SPWeb web = Item.ParentList.ParentWeb)
                    {
                        currentUser = web.WBxEnsureUserOrNull(userLogin);
                    }
                }
                else
                {
                    WBUtils.shouldThrowError("Couldn't find a current users!");

                }

            }
            return currentUser;
        }

        public void AuditLogEntry(String title, String comment)
        {
            WBAuditLogEntry logEntry = new WBAuditLogEntry(getCurrentUserOrLastModifiedByUser(), title, comment);

            WBLogging.WorkBoxes.Verbose("Adding an audit log entry: \n\n " + logEntry);

            string auditLog = Item.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_AUDIT_LOG);

            if (auditLog == "") 
            {
                auditLog = logEntry.ToString();
            }
            else 
            { 
                auditLog += ";" + logEntry.ToString(); 
            }

            Item.WBxSetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_AUDIT_LOG, auditLog); 
        }



        #endregion


        public WBTermCollection<WBTerm> FunctionalArea(WBTaxonomy functionalAreas)
        {
            return Item.WBxGetMultiTermColumn<WBTerm>(functionalAreas, WorkBox.COLUMN_NAME__FUNCTIONAL_AREA);
        }

        public WBTerm SeriesTag(WBTaxonomy seriesTags)
        {
            return Item.WBxGetSingleTermColumn<WBTerm>(seriesTags, WorkBox.COLUMN_NAME__SERIES_TAG);
        }

        private SPList createLinkedWorkBoxesList()
        {
            // throw new NotImplementedException();
            return null;
        }


        public void ApplyPublishOutDefaults(SPListItem sourceDocAsItem)
        {
            bool updateRequired = false;

            WBRecordsType documentRecordsType = null;

            TaxonomyFieldValue recordsTypeFieldValue = sourceDocAsItem[WorkBox.COLUMN_NAME__RECORDS_TYPE] as TaxonomyFieldValue;
            string recordsTypeUIControlValue = recordsTypeFieldValue.WBxUIControlValue();
            if (recordsTypeUIControlValue != "")
            {
                documentRecordsType = new WBRecordsType(this.RecordsTypes, recordsTypeUIControlValue);
            }


            if (true) // RecordsType.UseDefaultsWhenPublishingOut)
            {
                // First we'll double check that this item is the correct content type:
                SPContentType workBoxDocumentType = sourceDocAsItem.ParentList.ContentTypes[WORK_BOX_DOCUMENT_CONTENT_TYPE];
                sourceDocAsItem["ContentTypeId"] = workBoxDocumentType.Id;
                sourceDocAsItem.Update();

                if (recordsTypeUIControlValue == "" || documentRecordsType == null)
                {
                    documentRecordsType = RecordsType.DefaultPublishingOutRecordsType;
                    sourceDocAsItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__RECORDS_TYPE, documentRecordsType);
                    updateRequired = true;
                }


                if (documentRecordsType.IsFunctionalAreaEditable)
                {
                    if (!sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA))
                    {
                        sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, this.OwningTeam.InheritedFunctionalAreaUIControlValue);
                        updateRequired = true;
                    }
                }
                else
                {
                    sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, documentRecordsType.DefaultFunctionalAreaUIControlValue);
                    updateRequired = true;
                }

                if (ReferenceID != "" && !sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__REFERENCE_ID))
                {
                    sourceDocAsItem.WBxSetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID, ReferenceID);
                    updateRequired = true;
                }

                TaxonomyFieldValue owningTeamValue = sourceDocAsItem[WorkBox.COLUMN_NAME__OWNING_TEAM] as TaxonomyFieldValue;
                if (owningTeamValue.WBxUIControlValue() == "")
                {
                    sourceDocAsItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__OWNING_TEAM, OwningTeam);
                    updateRequired = true;
                }

                TaxonomyFieldValueCollection involvedTeamsValues = sourceDocAsItem[WorkBox.COLUMN_NAME__INVOLVED_TEAMS] as TaxonomyFieldValueCollection;
                if (involvedTeamsValues.WBxUIControlValue() == "")
                {
                    sourceDocAsItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, InvolvedTeams);
                    updateRequired = true;
                }
            }

            // Changing this so that the source value is set even if another date is already set
            //if (!sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__REFERENCE_DATE))
            //{
            switch (documentRecordsType.DocumentReferenceDateSource)
                {
                    case WBRecordsType.DOCUMENT_REFERENCE_DATE_SOURCE__PUBLISH_OUT_DATE:
                        {
                            sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE] = DateTime.Now;
                            updateRequired = true;
                            break;
                        }
                    case WBRecordsType.DOCUMENT_REFERENCE_DATE_SOURCE__WORK_BOX_REFERENCE_DATE:
                        {
                            sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE] = this.ReferenceDate;
                            updateRequired = true;
                            break;
                        }
                }

            //}

            if (documentRecordsType != null)
            {
                if (RecordsType.GeneratePublishOutFilenames)
                {
                    // OK so the document naming convention hasn't yet been applied, so let's apply it:
                    GenerateFilename(documentRecordsType, sourceDocAsItem);
                    updateRequired = true;
                }

            }

            if (updateRequired)
            {
                sourceDocAsItem.Update();
            }

        }

        internal void GenerateFilename(WBRecordsType documentRecordsType, SPListItem sourceDocAsItem)
        {
            if (documentRecordsType == null)
            {
                WBLogging.WorkBoxes.Verbose("The documentRecordsType was null!");
                return;
            }

            if (sourceDocAsItem == null)
            {
                WBLogging.WorkBoxes.Verbose("The sourceDocAsItem was null!");
                return;
            }

            string filename = sourceDocAsItem.Name;

            string extension = Path.GetExtension(filename);
            string justName = Path.GetFileNameWithoutExtension(filename);

            string name = documentRecordsType.GenerateCorrectDocumentName(this, sourceDocAsItem);

            if (name == null || name == "")
            {
                WBLogging.Debug("In WorkBox.GenerateFilename(): The return from documentRecordsType.GenerateCorrectDocumentName was empty!");
                return;
            }

            string newFilename = name + extension;

            // We only want to change the name if it's genuinely new. We'll assume any difference here comes from (x) unique endings:
            if (!justName.Contains(name))
            {
                // If we're changing the name then we'd better check that this file name is unique:
                newFilename = sourceDocAsItem.Web.WBxMakeFilenameUnique(sourceDocAsItem.File.ParentFolder, newFilename);

                sourceDocAsItem["Name"] = newFilename;
            }

            WBLogging.WorkBoxes.Verbose("The generated name was: " + newFilename);

            if (!sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__ORIGINAL_FILENAME))
            {
                sourceDocAsItem.WBxSetColumnAsString(WorkBox.COLUMN_NAME__ORIGINAL_FILENAME, filename);
            }
            string title = sourceDocAsItem.Title.WBxTrim();
            if (title == "")
            {
                sourceDocAsItem["Title"] = justName;
            }

        }

        public String MakeFilenameUnique(SPFolder folder, String suggestedName)
        {
            string fileNamePart = Path.GetFileNameWithoutExtension(suggestedName);
            string extension = Path.GetExtension(suggestedName);

            WBLogging.WorkBoxes.Verbose(string.Format("Trying to make the name unique: {0}    {1}", fileNamePart, extension));
            WBLogging.WorkBoxes.Verbose(string.Format("Suggested name: {0}    ", suggestedName));

            int count = 0;
            while (FileExists(folder, suggestedName))
            {
                count++;
                suggestedName = fileNamePart + " (" + count + ")" + extension;

                WBLogging.WorkBoxes.Verbose(string.Format("New suggested name: {0}    ", suggestedName));

                if (count > 1000) throw new Exception("You are trying to create more than 1000 files with the same name in the same folder!");
            }

            return suggestedName;
        }

        public bool FileExists(SPFolder folder, String suggestedName)
        {
            string fullPath = folder.Url + "/" + suggestedName;

            WBLogging.WorkBoxes.Verbose("About to GetFile : " + fullPath);
            SPFile file = Web.GetFile(fullPath);
            return file.Exists;
        }

        public bool CurrentUserIsOwner()
        {
            if (OwningTeam == null) return false;
            return OwningTeam.IsCurrentUserTeamMember();
        }

        public bool CurrentUserIsInvolved()
        {
            if (CurrentUserIsOwner()) return true;
            if (InvolvedTeams == null) return false;
            foreach (WBTeam team in InvolvedTeams)
            {
                if (team.IsCurrentUserTeamMember()) return true;
            }

            if (InvolvedIndividuals == null) return false;
            foreach (SPUser user in InvolvedIndividuals) 
            {
                if (Web.CurrentUser.Equals(user)) return true;
            }

            return false;
        }

        public bool CurrentUserCanVisit()
        {
            if (CurrentUserIsInvolved()) return true;

            // Obviously this is NOT a real implementation!!!
            return false;
        }


        public bool CurrentUserIsBusinessAdmin()
        {
            if (Collection.BusinessAdminTeams == null) return false;
            foreach (WBTeam team in Collection.BusinessAdminTeams)
            {
                if (team.IsCurrentUserTeamMember()) return true;
            }

            return false;
        }

        public bool CurrentUserIsSystemAdmin()
        {
            if (Collection.SystemAdminTeams == null) return false;
            foreach (WBTeam team in Collection.SystemAdminTeams)
            {
                if (team.IsCurrentUserTeamMember()) return true;
            }

            return false;
        }


        public void LinkToWorkBox(WorkBox otherWorkBox, String relationType)
        {
            if (LinkedWorkBoxesList == null)
            {
                WBUtils.shouldThrowError("Maybe should throw error - or some message as can't add linked work box due to missing linked work boxes list");
                return;
            }

            SPListItem item = LinkedWorkBoxesList.Items.Add();
            item["Title"] = otherWorkBox.Title;
            item[WorkBox.COLUMN_NAME__WORK_BOX_URL] = otherWorkBox.Url;
            item[WorkBox.COLUMN_NAME__WORK_BOX_UNIQUE_ID] = otherWorkBox.UniqueID;
            item[WorkBox.COLUMN_NAME__WORK_BOX_GUID] = otherWorkBox.GUIDString;
            item.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__RECORDS_TYPE, otherWorkBox.RecordsType);

            if (otherWorkBox.ReferenceDateHasValue)
                item[WorkBox.COLUMN_NAME__REFERENCE_DATE] = otherWorkBox.ReferenceDate;

            item.Update();
        }
    }
}
