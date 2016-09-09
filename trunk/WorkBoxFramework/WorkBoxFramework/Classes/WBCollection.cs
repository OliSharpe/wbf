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
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{

    public class WBCollection : IDisposable
    {

        #region Constants

        private const string COLLECTION_PROPERTY__SYSTEM_ADMIN_TEAMS = "wbf__collection__system_admin_teams";
        private const string COLLECTION_PROPERTY__BUSINESS_ADMIN_TEAMS = "wbf__collection__business_admin_teams";

        private const string COLLECTION_PROPERTY__WORK_BOXES_LIST_NAME = "wbf__collection__work_boxes_list_name";
        private const string COLLECTION_PROPERTY__WORK_BOXES_LIST_EVENT_RECEIVERS_ADDED = "wbf__collection__work_boxes_list_event_receivers_added";

        private const string COLLECTION_PROPERTY__UNIQUE_ID_PREFIX = "wbf__collection__unique_id_prefix";
        private const string COLLECTION_PROPERTY__GENERATE_UNIQUE_IDS = "wbf__collection__generate_unique_ids";
        private const string COLLECTION_PROPERTY__NUMBER_OF_DIGITS_IN_IDS = "wbf__collection__number_of_digits_in_ids";
        private const string COLLECTION_PROPERTY__INITIAL_ID_OFFSET = "wbf__collection__initial_id_offset";
        
        private const string COLLECTION_PROPERTY__CAN_ANYONE_CREATE = "wbf__collection__can_anyone_create";
        private const string COLLECTION_PROPERTY__CAN_OWNER_EDIT_PROPERTIES = "wbf__collection__can_owner_edit_properties";
        private const string COLLECTION_PROPERTY__CAN_OWNER_CHANGE_OWNER = "wbf__collection__can_owner_change_owner";
        private const string COLLECTION_PROPERTY__CAN_OWNER_CLOSE = "wbf__collection__can_owner_close";
        private const string COLLECTION_PROPERTY__CAN_OWNER_REOPEN = "wbf__collection__can_owner_reopen";

        private const string COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_SYSTEM_ADMIN = "wbf__collection__open_permission_level_for_system_admin";
        private const string COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_BUSINESS_ADMIN = "wbf__collection__open_permission_level_for_business_admin";
        private const string COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_OWNER = "wbf__collection__open_permission_level_for_owner";
        private const string COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_INVOLVED = "wbf__collection__open_permission_level_for_involved";
        private const string COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_VISITORS = "wbf__collection__open_permission_level_for_visitors";
        private const string COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_EVERYONE = "wbf__collection__open_permission_level_for_everyone";

        private const string COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_SYSTEM_ADMIN = "wbf__collection__closed_permission_level_for_system_admin";
        private const string COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_BUSINESS_ADMIN = "wbf__collection__closed_permission_level_for_business_admin";
        private const string COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_OWNER = "wbf__collection__closed_permission_level_for_owner";
        private const string COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_INVOLVED = "wbf__collection__closed_permission_level_for_involved";
        private const string COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_VISITORS = "wbf__collection__closed_permission_level_for_visitors";
        private const string COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_EVERYONE = "wbf__collection__closed_permission_level_for_everyone";

        private const string COLLECTION_PROPERTY__USE_FOLDER_ACCESS_GROUPS_PATTERN = "wbf__collection__use_folder_access_groups_pattern";
        private const string COLLECTION_PROPERTY__FOLDER_ACCESS_GROUPS_PREFIX = "wbf__collection__folder_access_groups_prefix";
        private const string COLLECTION_PROPERTY__FOLDER_ACCESS_GROUPS_FOLDER_NAMES = "wbf__collection__folder_access_groups_folder_names";
        private const string COLLECTION_PROPERTY__FOLDER_ACCESS_GROUP_PERMISSION_LEVEL = "wbf__collection__folder_access_group_permission_level";
        private const string COLLECTION_PROPERTY__ALL_FOLDERS_ACCESS_GROUP_PERMISSION_LEVEL = "wbf__collection__all_folders_access_group_permission_level";


        private const string COLLECTION_PROPERTY__URL_FOR_NEW_WORK_BOX_DIALOG = "wbf__collection__url_for_new_dialog";
        private const string COLLECTION_PROPERTY__CREATE_NEW_WORK_BOX_TEXT = "wbf__collection__create_new_work_box_text";
        private const string COLLECTION_PROPERTY__DEFAULT_OWNING_TEAM = "wbf__collection__default_owning_team";

        private const string COLLECTION_PROPERTY__DIALOG_DETAILS_FORMAT = "wbf__collection__dialog__{0}";


        private const string COLLECTION_PROPERTY__USES_LINKED_CALENDARS = "wbf__collection__uses_linked_calendars";


        private const bool DEFAULT__CAN_ANYONE_CREATE = true;
        private const bool DEFAULT__CAN_OWNER_EDIT_PROPERTIES = true;
        private const bool DEFAULT__CAN_OWNER_CHANGE_OWNER = true;
        private const bool DEFAULT__CAN_OWNER_CLOSE = true;
        private const bool DEFAULT__CAN_OWNER_REOPEN = true;

        internal const string PERMISSION_LEVEL__WORK_BOX_SYSTEM_ADMIN = "Work Box System Admin";
        internal const string PERMISSION_LEVEL__WORK_BOX_BUSINESS_ADMIN = "Work Box Business Admin";
        internal const string PERMISSION_LEVEL__WORK_BOX_OWNER = "Work Box Owner";
        internal const string PERMISSION_LEVEL__WORK_BOX_INVOLVED = "Work Box Involved";
        internal const string PERMISSION_LEVEL__WORK_BOX_VISITOR = "Work Box Visitor";
        internal const string PERMISSION_LEVEL__WORK_BOX_CLOSED = "Work Box Closed";
        internal const string PERMISSION_LEVEL__WORK_BOX_METADATA_EDITOR = "Work Box Metadata Editor";

        private const string DEFAULT__OPEN_PERMISION_LEVEL_FOR_SYSTEM_ADMIN = PERMISSION_LEVEL__WORK_BOX_SYSTEM_ADMIN;
        private const string DEFAULT__OPEN_PERMISION_LEVEL_FOR_BUSINESS_ADMIN = PERMISSION_LEVEL__WORK_BOX_BUSINESS_ADMIN;
        private const string DEFAULT__OPEN_PERMISION_LEVEL_FOR_OWNER = PERMISSION_LEVEL__WORK_BOX_OWNER;
        private const string DEFAULT__OPEN_PERMISION_LEVEL_FOR_INVOLVED = PERMISSION_LEVEL__WORK_BOX_INVOLVED;
        private const string DEFAULT__OPEN_PERMISION_LEVEL_FOR_VISITORS = PERMISSION_LEVEL__WORK_BOX_VISITOR;
        private const string DEFAULT__OPEN_PERMISION_LEVEL_FOR_EVERYONE = "";

        private const string DEFAULT__CLOSED_PERMISION_LEVEL_FOR_SYSTEM_ADMIN = PERMISSION_LEVEL__WORK_BOX_CLOSED;
        private const string DEFAULT__CLOSED_PERMISION_LEVEL_FOR_BUSINESS_ADMIN = PERMISSION_LEVEL__WORK_BOX_CLOSED;
        private const string DEFAULT__CLOSED_PERMISION_LEVEL_FOR_OWNER = PERMISSION_LEVEL__WORK_BOX_CLOSED;
        private const string DEFAULT__CLOSED_PERMISION_LEVEL_FOR_INVOLVED = PERMISSION_LEVEL__WORK_BOX_CLOSED;
        private const string DEFAULT__CLOSED_PERMISION_LEVEL_FOR_VISITORS = PERMISSION_LEVEL__WORK_BOX_CLOSED;
        private const string DEFAULT__CLOSED_PERMISION_LEVEL_FOR_EVERYONE = "";

        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_SYSTEM_ADMIN =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.AddListItems |
                                                                SPBasePermissions.EditListItems |
                                                                SPBasePermissions.DeleteListItems |
                                                                SPBasePermissions.ApproveItems |
                                                                SPBasePermissions.OpenItems |
                                                                SPBasePermissions.ViewVersions |
                                                                SPBasePermissions.DeleteVersions |
                                                                SPBasePermissions.CancelCheckout |
                                                                SPBasePermissions.ManagePersonalViews |
                                                                SPBasePermissions.ManageLists |
                                                                SPBasePermissions.ViewFormPages |
                                                                SPBasePermissions.Open |
                                                                SPBasePermissions.ViewPages |
                                                                SPBasePermissions.AddAndCustomizePages |
                                                                SPBasePermissions.ApplyThemeAndBorder |
                                                                SPBasePermissions.ApplyStyleSheets |
                                                                SPBasePermissions.ViewUsageData |
                                                                SPBasePermissions.ManagePermissions |
                                                                SPBasePermissions.BrowseDirectories |
                                                                SPBasePermissions.BrowseUserInfo |
                                                                SPBasePermissions.ManageWeb |
                                                                SPBasePermissions.UseClientIntegration |
                                                                SPBasePermissions.UseRemoteAPIs |
                                                                SPBasePermissions.ManageAlerts |
                                                                SPBasePermissions.CreateAlerts |
                                                                SPBasePermissions.EditMyUserInfo |
                                                                SPBasePermissions.EnumeratePermissions;


        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_BUSINESS_ADMIN =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.AddListItems |
                                                                SPBasePermissions.EditListItems |
                                                                SPBasePermissions.DeleteListItems |
                                                                SPBasePermissions.ApproveItems |
                                                                SPBasePermissions.OpenItems |
                                                                SPBasePermissions.ViewVersions |
                                                                SPBasePermissions.DeleteVersions |
                                                                SPBasePermissions.CancelCheckout |
                                                                SPBasePermissions.ManagePersonalViews |
                                                                SPBasePermissions.ManageLists |
                                                                SPBasePermissions.ViewFormPages |
                                                                SPBasePermissions.Open |
                                                                SPBasePermissions.ViewPages |
                                                                SPBasePermissions.AddAndCustomizePages |
                                                                SPBasePermissions.ViewUsageData |
                                                                SPBasePermissions.BrowseDirectories |
                                                                SPBasePermissions.BrowseUserInfo |
                                                                SPBasePermissions.UseClientIntegration |
                                                                SPBasePermissions.UseRemoteAPIs |
                                                                SPBasePermissions.ManageAlerts |
                                                                SPBasePermissions.CreateAlerts |
                                                                SPBasePermissions.EditMyUserInfo |
                                                                SPBasePermissions.EnumeratePermissions;

        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_OWNER =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.AddListItems |
                                                                SPBasePermissions.EditListItems |
                                                                SPBasePermissions.DeleteListItems |
                                                                SPBasePermissions.ApproveItems |
                                                                SPBasePermissions.OpenItems |
                                                                SPBasePermissions.ViewVersions |
                                                                SPBasePermissions.DeleteVersions |
                                                                SPBasePermissions.CancelCheckout |
                                                                SPBasePermissions.ManagePersonalViews |
                                                                SPBasePermissions.ManageLists |
                                                                SPBasePermissions.ViewFormPages |
                                                                SPBasePermissions.Open |
                                                                SPBasePermissions.ViewPages |
                                                                SPBasePermissions.AddAndCustomizePages |
                                                                SPBasePermissions.ViewUsageData |
                                                                SPBasePermissions.BrowseDirectories |
                                                                SPBasePermissions.BrowseUserInfo |
                                                                SPBasePermissions.UseClientIntegration |
                                                                SPBasePermissions.UseRemoteAPIs |
                                                                SPBasePermissions.ManageAlerts |
                                                                SPBasePermissions.CreateAlerts |
                                                                SPBasePermissions.EditMyUserInfo |
                                                                SPBasePermissions.EnumeratePermissions;

        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_INVOLVED =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.AddListItems |
                                                                SPBasePermissions.EditListItems |
                                                                SPBasePermissions.OpenItems |
                                                                SPBasePermissions.ViewVersions |
                                                                SPBasePermissions.ViewFormPages |
                                                                SPBasePermissions.Open |
                                                                SPBasePermissions.ViewPages |
                                                                SPBasePermissions.BrowseDirectories |
                                                                SPBasePermissions.BrowseUserInfo |
                                                                SPBasePermissions.UseClientIntegration |
                                                                SPBasePermissions.UseRemoteAPIs |
                                                                SPBasePermissions.CreateAlerts |
                                                                SPBasePermissions.EditMyUserInfo;

        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_VISITOR =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.OpenItems |
                                                                SPBasePermissions.ViewFormPages |
                                                                SPBasePermissions.Open |
                                                                SPBasePermissions.ViewPages |
                                                                SPBasePermissions.BrowseDirectories |
                                                                SPBasePermissions.BrowseUserInfo |
                                                                SPBasePermissions.UseClientIntegration |
                                                                SPBasePermissions.UseRemoteAPIs |
                                                                SPBasePermissions.CreateAlerts;


        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_CLOSED =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.OpenItems |
                                                                SPBasePermissions.ViewFormPages |
                                                                SPBasePermissions.Open |
                                                                SPBasePermissions.ViewPages |
                                                                SPBasePermissions.BrowseDirectories |
                                                                SPBasePermissions.BrowseUserInfo |
                                                                SPBasePermissions.UseClientIntegration |
                                                                SPBasePermissions.UseRemoteAPIs |
                                                                SPBasePermissions.CreateAlerts;


        internal const SPBasePermissions BASE_PERMISSIONS__WORK_BOX_METADATA_EDITOR =
                                                                SPBasePermissions.ViewListItems |
                                                                SPBasePermissions.AddListItems |
                                                                SPBasePermissions.EditListItems |
                                                                SPBasePermissions.Open;
                                                                                    


        private const string DEFAULT_URL__NEW_WORK_BOX_DIALOG = "/_layouts/WorkBoxFramework/NewWorkBox.aspx?workBoxCollectionUrl=[CollectionURL]&recordsTypeGUID=[RecordsTypeGUID]&owningTeamGUID=[TeamGUID]&relatedWorkBoxURL=[RelatedWorkBoxURL]&relationType=[RelationType]";
        private const string DEFAULT__CREATE_NEW_WORK_BOX_TEXT = "Create New Work Box";

        private const string DEFAULT_URL__VIEW_PROPERTIES_DIALOG = "~WorkBoxCollection/Lists/[AllWorkBoxesListName]/DispForm.aspx?ID=[ID]&IsDlg=1";        private const string DEFAULT_URL__EDIT_PROPERTIES_DIALOG = "~WorkBoxCollection/Lists/[AllWorkBoxesListName]/EditForm.aspx?ID=[ID]&IsDlg=1";

        private const string DEFAULT_URL__VIEW_ALL_INVOLVED_DIALOG = "~WorkBox/_layouts/WorkBoxFramework/ViewAllInvolved.aspx";
        private const string DEFAULT_URL__INVITE_TEAM_DIALOG = "~WorkBox/_layouts/WorkBoxFramework/InviteTeamsWorkBoxDialog.aspx";
        private const string DEFAULT_URL__INVITE_INDIVIDUAL_DIALOG = "~WorkBox/_layouts/WorkBoxFramework/GenericOKPage.aspx?pageTitle=Not%20Implemented%20Yet&pageText=Not%20Implemented%20Yet";
        private const string DEFAULT_URL__CHANGE_OWNER_DIALOG = "~WorkBox/_layouts/WorkBoxFramework/ChangeWorkBoxOwner.aspx";

        private const string DEFAULT_URL__CLOSE_DIALOG = "~WorkBox/_layouts/WorkBoxFramework/CloseWorkBoxDialog.aspx";
        private const string DEFAULT_URL__REOPEN_DIALOG = "~WorkBox/_layouts/WorkBoxFramework/ReOpenWorkBoxDialog.aspx";

        #endregion

        #region Private Variables

        private bool _siteNeedsDisposing = false;
        private bool _webNeedsDisposing = false;

        #endregion

        #region Constructors

        public WBCollection(String workBoxCollectionURL)
        {
            _url = workBoxCollectionURL;

            _site = null;
            _siteNeedsDisposing = false;

            _web = null;
            _webNeedsDisposing = false;
        }

        public WBCollection(SPSite site, Guid workBoxCollectionWebGuid)
        {
            _site = site;
            _siteNeedsDisposing = false;

            _web = site.OpenWeb(workBoxCollectionWebGuid);
            _webNeedsDisposing = true;
        }

        public WBCollection(SPContext context)
        {
            _site = context.Site;
            _siteNeedsDisposing = false;

            _web = context.Web;
            _webNeedsDisposing = false;
        }

        public WBCollection(SPSite site, SPWeb web)
        {
            _site = site;
            _siteNeedsDisposing = false;

            _web = web;
            _webNeedsDisposing = false;
        }

        public WBCollection(SPListItem item)
        {
            _web = item.ParentList.ParentWeb;
            _webNeedsDisposing = true;

            _site = _web.Site;
            _siteNeedsDisposing = true;
        }
        #endregion

        #region static methods

        public static bool IsWebAWBCollection(SPWeb web)
        {
            return (web.AllProperties.ContainsKey(WBCollection.COLLECTION_PROPERTY__WORK_BOXES_LIST_NAME)
                && !web.AllProperties[WBCollection.COLLECTION_PROPERTY__WORK_BOXES_LIST_NAME].Equals(""));
        }

        #endregion


        #region Configuration Steps

        private const String CONFIG_STEP__SITE_COLUMNS = "Site Columns";
        private const String CONFIG_STEP__CONTENT_TYPES = "Content Types";
        private const String CONFIG_STEP__PERMISSIONS_LEVELS = "Permissions Levels";
        private const String CONFIG_STEP__WORK_BOXES_LIST = "Work Boxes List";
        private const String CONFIG_STEP__TEMPLATES_LIST = "Templates List";
        private const String CONFIG_STEP__ADD_TEMPLATES_LOOKUP_COLUMN = "Add Templates Lookup Column";
        private const String CONFIG_STEP__ATTACH_EVENT_RECEIVERS = "Attach Event Receivers";
        private const String CONFIG_STEP__SETUP_DEFAULT_RECORDS_TYPE = "Setup Default Records Type";
        private const String CONFIG_STEP__SETUP_DEFAULT_TEMPLATE = "Setup Default Template";
        private const String CONFIG_STEP__SYNC_TEAMS_TIMER_TASK = "Sync Teams Timer Task";

        internal static String[] ConfigurationStepsNames = { 
                                                  CONFIG_STEP__SITE_COLUMNS, 
                                                  CONFIG_STEP__CONTENT_TYPES, 
                                                  CONFIG_STEP__PERMISSIONS_LEVELS,
                                                  CONFIG_STEP__WORK_BOXES_LIST, 
                                                  CONFIG_STEP__TEMPLATES_LIST,
                                                  CONFIG_STEP__ADD_TEMPLATES_LOOKUP_COLUMN,
                                                  CONFIG_STEP__ATTACH_EVENT_RECEIVERS,
                                                  CONFIG_STEP__SETUP_DEFAULT_RECORDS_TYPE,
                                                  CONFIG_STEP__SETUP_DEFAULT_TEMPLATE,
                                                  CONFIG_STEP__SYNC_TEAMS_TIMER_TASK
                                              };

        internal WBTaskFeedback DoConfigurationStep(String stepName)
        {
            WBTaskFeedback feedback = new WBTaskFeedback(stepName);

            SPWeb siteCollectionRootWeb = Site.RootWeb;
            try
            {
                switch (stepName)
                {
                    case CONFIG_STEP__SITE_COLUMNS:
                        {
                            WBFarm.Local.CreateOrCheckWBFSiteColumns(feedback, Site, siteCollectionRootWeb);
                            break;
                        }
                    case CONFIG_STEP__CONTENT_TYPES:
                        {
                            WBFarm.Local.CreateOrCheckWBCSiteContentTypes(feedback, Site, siteCollectionRootWeb);
                            break;
                        }
                    case CONFIG_STEP__PERMISSIONS_LEVELS:
                        {
                            CreateOrCheckWBCPermissionLevels(feedback);
                            break;
                        }
                    case CONFIG_STEP__WORK_BOXES_LIST:
                        {
                            CreateOrCheckWorkBoxesList(feedback, Site, siteCollectionRootWeb, Web);
                            break;
                        }
                    case CONFIG_STEP__TEMPLATES_LIST:
                        {
                            CreateOrCheckTemplatesList(feedback, Site, siteCollectionRootWeb, Web);
                            break;
                        }
                    case CONFIG_STEP__ADD_TEMPLATES_LOOKUP_COLUMN:
                        {
                            CreateOrCheckLookupColumn(feedback);
                            break;
                        }
                    case CONFIG_STEP__ATTACH_EVENT_RECEIVERS:
                        {
                            if (AttachEventReceivers())
                            {
                                feedback.Created("Attached event receivers");
                            }
                            else
                            {
                                feedback.Failed("Failed to attach event receivers");
                            }
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                feedback.Failed("Something went wrong with step: " + stepName, exception);
            }

            if (siteCollectionRootWeb != Web && (SPContext.Current == null || siteCollectionRootWeb != SPContext.Current.Web))
            {
                siteCollectionRootWeb.Dispose();
            }


            int thisStep = Array.IndexOf(ConfigurationStepsNames, stepName);

            if (thisStep >= 0 && thisStep < ConfigurationStepsNames.Length - 1)
            {
                feedback.NextTaskName = ConfigurationStepsNames[thisStep + 1];
            }
            else
            {
                feedback.NextTaskName = "";
            }

            return feedback;
        }

        internal void CreateOrCheckWBCPermissionLevels(WBTaskFeedback feedback)
        {

            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_SYSTEM_ADMIN, "For system admins of the whole work box collection.", BASE_PERMISSIONS__WORK_BOX_SYSTEM_ADMIN);
            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_BUSINESS_ADMIN, "For business admins of the whole work box collection.", BASE_PERMISSIONS__WORK_BOX_BUSINESS_ADMIN);
            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_OWNER, "For owners of particular work boxes.", BASE_PERMISSIONS__WORK_BOX_OWNER);
            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_INVOLVED, "For those involved with particular work boxes.", BASE_PERMISSIONS__WORK_BOX_INVOLVED);
            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_VISITOR, "For those visiting particular work boxes.", BASE_PERMISSIONS__WORK_BOX_VISITOR);
            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_CLOSED, "For closed work boxes.", BASE_PERMISSIONS__WORK_BOX_CLOSED);
            WBUtils.CreateOrCheckPermissionLevel(feedback, Site, PERMISSION_LEVEL__WORK_BOX_METADATA_EDITOR, "To enable users to edit work box metadata.", BASE_PERMISSIONS__WORK_BOX_METADATA_EDITOR);

        }

        internal void CreateOrCheckWorkBoxesList(WBTaskFeedback feedback, SPSite site, SPWeb rootWeb, SPWeb web)
        {
            bool createdList = WBUtils.CreateOrCheckListUsingContentType(feedback, rootWeb, web, ListName, WorkBox.CONTENT_TYPE__WORK_BOX_METADATA_ITEM);

            if (!createdList)
            {
                // OK so we're not creating the list - but let's check that it has all of the columns that it ought to have:
                WBUtils.CreateOrCheckCustomList(feedback, rootWeb, web, ListName, WBFarm.WBCMetadataItemFields);
            }
            else
            {
                // If we did create the list then let's also add it to the quick launch nav area:
                WBUtils.CheckOrCreateQuickLaunchNav(feedback, web, "Lists", ListName, List.DefaultViewUrl);

                // Let's also add some columns to the default view:
                WBColumn[] wbcListDefaultViewColumns = { 
                                                            // WBColumn.Title,
                                                            WBColumn.WorkBoxStatus,
                                                            WBColumn.WorkBoxStatusChangeRequest,
                                                            WBColumn.WorkBoxLink,
                                                            WBColumn.RecordsType
                                                        };

                SPView view = List.Views[0];
                WBUtils.AddColumnsToView(feedback, view, wbcListDefaultViewColumns);
            }

        }

        internal void CreateOrCheckTemplatesList(WBTaskFeedback feedback, SPSite site, SPWeb rootWeb, SPWeb web)
        {
            bool createdList = WBUtils.CreateOrCheckListUsingContentType(feedback, rootWeb, web, WorkBox.LIST_NAME__WORK_BOX_TEMPLATES, WorkBox.CONTENT_TYPE__WORK_BOX_TEMPLATES_ITEM);

            if (createdList)
            {
                // If we did create the list then let's also add it to the quick launch nav area:
                WBUtils.CheckOrCreateQuickLaunchNav(feedback, web, "Lists", TemplatesList.Title, TemplatesList.DefaultViewUrl);

                // Let's also add some columns to the default view:
                WBColumn[] templatesListDefaultViewColumns = { 
                                                                  // WBColumn.Title,
                                                                  WBColumn.RecordsType,
                                                                  WBColumn.WorkBoxTemplateStatus,
                                                                  WBColumn.WorkBoxTemplateTitle,
                                                                  WBColumn.WorkBoxTemplateName
                                                              };

                SPView view = TemplatesList.Views[0];
                WBUtils.AddColumnsToView(feedback, view, templatesListDefaultViewColumns);
            }

        }

        internal void CreateOrCheckLookupColumn(WBTaskFeedback feedback)
        {
            SPFieldLookup lookupField = null;

            if (TemplatesList == null)
            {
                feedback.Failed("You can't add the CreateOrCheckLookupColumn method without first creating the templates list");
                return;
            }

            if (List == null)
            {
                feedback.Failed("You can't add the CreateOrCheckLookupColumn method without first creating the WBC list");
                return;
            }

            if (List.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE))
            {
                feedback.Checked("Found Work Box Template field in WBC list: " + List.Title);
                return;
            }

            if (Web.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE))
            {
                feedback.Checked("Found site level column called: " + WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE);
                lookupField = Web.Fields[WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE] as SPFieldLookup;
            }
            else
            {
                Web.Fields.AddLookup(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE, TemplatesList.ID, Web.ID, false);
                lookupField = Web.Fields[WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE] as SPFieldLookup;
                lookupField.AllowMultipleValues = false;
                lookupField.LookupField = WBColumn.Title.DisplayName;
                lookupField.Group = WorkBox.SITE_COLUMNS_GROUP_NAME;
                lookupField.Update();
                feedback.Created("Created site level column called: " + lookupField.Title);
            }

            SPContentType defaultContentType = List.ContentTypes[0];
            SPFieldLink fieldLink = new SPFieldLink(lookupField);
            defaultContentType.FieldLinks.Add(fieldLink);
            defaultContentType.Update();
            List.Update();

            feedback.Created("Added site level column called: " + lookupField.Title + " to content type " + defaultContentType.Name + " in list called: " + List.Title);
            
            // And finally try to add the new lookup column to the default view:
            WBColumn[] lookupColumnAsList = { WBColumn.WorkBoxTemplate };
            SPView view = List.Views[0];
            WBUtils.AddColumnsToView(feedback, view, lookupColumnAsList);
        }


        #endregion

        #region Object Properties

        private SPWeb _web = null;
        public SPWeb Web 
        { 
            get 
            {
                if (_web == null) loadSiteAndWeb();
                return _web; 
            }         
        }

        private SPSite _site = null;
        public SPSite Site 
        { 
            get 
            {
                if (_site == null) loadSiteAndWeb();
                return _site; 
            } 
        }

        private void loadSiteAndWeb()
        {
            _site = new SPSite(_url);
            _siteNeedsDisposing = true;

            _web = _site.OpenWeb();
            _webNeedsDisposing = true;
        }

        private string _url;
        public String Url
        {
            get
            {
                if (_url == null)
                {
                    _url = Web.Url;
                }
                return _url;
            }
        }

        private SPList _list = null;
        public SPList List
        {
            get
            {
                if (_list == null)
                {
                    string listName = ListName;

                    if (listName == "")
                    {
                        WBLogging.WorkBoxCollections.Unexpected("Error finding: listName = " + listName);
                    }
                    else
                    {
                        _list = this.Web.Lists.TryGetList(listName);
                        if (_list == null)
                        {
                            WBLogging.WorkBoxCollections.Unexpected("Couldn't find the list with Name = " + listName);
                        }
                    }
                }

                return _list;
            }
            private set { _list = value; }
        }

        private SPList _templatesList = null;
        public SPList TemplatesList
        {
            get
            {
                if (_templatesList == null)
                {
                    _templatesList = this.Web.Lists[WorkBox.LIST_NAME__WORK_BOX_TEMPLATES];
                    if (_templatesList == null)
                    {
                        WBUtils.shouldThrowError("Couldn't find the list of work box templates.");
                    }
                }

                return _templatesList;
            }
        }

        public bool DisposeRequired { get { return (_siteNeedsDisposing || _webNeedsDisposing); } }

        #endregion

        #region WBCollection Metadata Properties

        private WBTermCollection<WBTeam> _systemAdminTeams = null;
        public WBTermCollection<WBTeam> SystemAdminTeams
        {
            get 
            {
                if (_systemAdminTeams == null)
                {
                    string value = Web.WBxGetProperty(WBCollection.COLLECTION_PROPERTY__SYSTEM_ADMIN_TEAMS);

                    WBTaxonomy teams = WBTaxonomy.GetTeams(this.Site);
                    _systemAdminTeams = new WBTermCollection<WBTeam>(teams, value);
                }
                return _systemAdminTeams;
            }

            set
            {
                _systemAdminTeams = value;
                Web.WBxSetProperty(WBCollection.COLLECTION_PROPERTY__SYSTEM_ADMIN_TEAMS, value.UIControlValue);
            }
        }

        private WBTermCollection<WBTeam> _businessAdminTeams = null;
        public WBTermCollection<WBTeam> BusinessAdminTeams
        {
            get
            {
                if (_businessAdminTeams == null)
                {
                    string value = Web.WBxGetProperty(WBCollection.COLLECTION_PROPERTY__BUSINESS_ADMIN_TEAMS);

                    WBTaxonomy teams = WBTaxonomy.GetTeams(this.Site);
                    _businessAdminTeams = new WBTermCollection<WBTeam>(teams, value);
                }
                return _businessAdminTeams;
            }

            set
            {
                _businessAdminTeams = value;
                Web.WBxSetProperty(WBCollection.COLLECTION_PROPERTY__BUSINESS_ADMIN_TEAMS, value.UIControlValue);
            }
        }

        public String ListName
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__WORK_BOXES_LIST_NAME); }
            set 
            {
                String newListName = value.WBxTrim();
                String oldListName = Web.WBxGetProperty(COLLECTION_PROPERTY__WORK_BOXES_LIST_NAME);
                Web.WBxSetProperty(COLLECTION_PROPERTY__WORK_BOXES_LIST_NAME, newListName); 

                if (!newListName.Equals(oldListName))                        
                {                            
                    // OK so we have a new list name to add our event receiver to:
                            
                    if (!oldListName.Equals("") && EventReceiversAdded)
                    {
                                
                        // OK so we have an out of date event receiver to remove first:
                        SPList oldList = Web.Lists.TryGetList(oldListName);

                        if (oldList != null)
                        {
                            for (int i = 0; i < oldList.EventReceivers.Count; i++)
                            {
                                if (oldList.EventReceivers[i].Name != null)
                                {
                                    if (oldList.EventReceivers[i].Name == WorkBox.WORK_BOXES_LIST_EVENT_RECEIVER__ITEM_ADDED || oldList.EventReceivers[i].Name == WorkBox.WORK_BOXES_LIST_EVENT_RECEIVER__ITEM_UPDATED)
                                    {
                                        oldList.EventReceivers[i].Delete();

                                        i = -1;
                                    }
                                }
                            }
                        }

                        EventReceiversAdded = false;                            
                    }
                            
                    if (!newListName.Equals(""))                            
                    {
                        AttachEventReceivers();
                    }
                }
            }
        }

        internal bool AttachEventReceivers()
        {
            if (List != null)
            {
                string assemblyName = "WorkBoxFramework, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=4554acfc19d83350";
                string className = "WorkBoxFramework.WorkBoxMetaDataItemChangeEventReceiver";

                SPEventReceiverDefinition itemAddedEventReceiver = List.EventReceivers.Add();
                itemAddedEventReceiver.Name = WorkBox.WORK_BOXES_LIST_EVENT_RECEIVER__ITEM_ADDED;
                itemAddedEventReceiver.Type = SPEventReceiverType.ItemAdded;
                itemAddedEventReceiver.SequenceNumber = 1000;
                itemAddedEventReceiver.Assembly = assemblyName;
                itemAddedEventReceiver.Class = className;
                itemAddedEventReceiver.Update();

                SPEventReceiverDefinition itemUpdatedEventReceiver = List.EventReceivers.Add();
                itemUpdatedEventReceiver.Name = WorkBox.WORK_BOXES_LIST_EVENT_RECEIVER__ITEM_UPDATED;
                itemUpdatedEventReceiver.Type = SPEventReceiverType.ItemUpdated;
                itemUpdatedEventReceiver.SequenceNumber = 1000;
                itemUpdatedEventReceiver.Assembly = assemblyName;
                itemUpdatedEventReceiver.Class = className;
                itemUpdatedEventReceiver.Update();

                EventReceiversAdded = true;
            }
            else
            {
                EventReceiversAdded = false;
            }

            return EventReceiversAdded;
        }

        public bool EventReceiversAdded
        {
            get { return Web.WBxGetBoolProperty(COLLECTION_PROPERTY__WORK_BOXES_LIST_EVENT_RECEIVERS_ADDED); }
            private set { Web.WBxSetBoolProperty(COLLECTION_PROPERTY__WORK_BOXES_LIST_EVENT_RECEIVERS_ADDED, value); }
        }

        public String UniqueIDPrefix
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__UNIQUE_ID_PREFIX); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__UNIQUE_ID_PREFIX, value); }
        }

        public bool GenerateUniqueIDs
        {
            get { return Web.WBxGetBoolProperty(COLLECTION_PROPERTY__GENERATE_UNIQUE_IDS); }
            set { Web.WBxSetBoolProperty(COLLECTION_PROPERTY__GENERATE_UNIQUE_IDS, value); }
        }

        public int NumberOfDigitsInIDs
        {
            get { return Web.WBxGetIntProperty(COLLECTION_PROPERTY__NUMBER_OF_DIGITS_IN_IDS); }
            set { Web.WBxSetIntProperty(COLLECTION_PROPERTY__NUMBER_OF_DIGITS_IN_IDS, value); }
        }

        public int InitialIDOffset
        {
            get { return Web.WBxGetIntProperty(COLLECTION_PROPERTY__INITIAL_ID_OFFSET); }
            set { Web.WBxSetIntProperty(COLLECTION_PROPERTY__INITIAL_ID_OFFSET, value); }
        }

        public bool CanAnyoneCreate
        {
            get { return Web.WBxGetBoolPropertyOrDefault(COLLECTION_PROPERTY__CAN_ANYONE_CREATE, DEFAULT__CAN_ANYONE_CREATE); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CAN_ANYONE_CREATE, value); }
        }

        public bool CanOwnerEditProperties
        {
            get { return Web.WBxGetBoolPropertyOrDefault(COLLECTION_PROPERTY__CAN_OWNER_EDIT_PROPERTIES, DEFAULT__CAN_OWNER_EDIT_PROPERTIES); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CAN_OWNER_EDIT_PROPERTIES, value); }
        }


        public bool CanOwnerChangeOwner
        {
            get { return Web.WBxGetBoolPropertyOrDefault(COLLECTION_PROPERTY__CAN_OWNER_CHANGE_OWNER, DEFAULT__CAN_OWNER_CHANGE_OWNER); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CAN_OWNER_CHANGE_OWNER, value); }
        }

        public bool CanOwnerClose
        {
            get { return Web.WBxGetBoolPropertyOrDefault(COLLECTION_PROPERTY__CAN_OWNER_CLOSE, DEFAULT__CAN_OWNER_CLOSE); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CAN_OWNER_CLOSE, value); }
        }

        public bool CanOwnerReOpen
        {
            get { return Web.WBxGetBoolPropertyOrDefault(COLLECTION_PROPERTY__CAN_OWNER_REOPEN, DEFAULT__CAN_OWNER_REOPEN); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CAN_OWNER_REOPEN, value); }
        }


        public String OpenPermissionLevelForSystemAdmin
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_SYSTEM_ADMIN, DEFAULT__OPEN_PERMISION_LEVEL_FOR_SYSTEM_ADMIN); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_SYSTEM_ADMIN, value); }
        }

        public String OpenPermissionLevelForBusinessAdmin
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_BUSINESS_ADMIN, DEFAULT__OPEN_PERMISION_LEVEL_FOR_BUSINESS_ADMIN); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_BUSINESS_ADMIN, value); }
        }

        public String OpenPermissionLevelForOwner
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_OWNER, DEFAULT__OPEN_PERMISION_LEVEL_FOR_OWNER); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_OWNER, value); }
        }

        public String OpenPermissionLevelForInvolved
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_INVOLVED, DEFAULT__OPEN_PERMISION_LEVEL_FOR_INVOLVED); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_INVOLVED, value); }
        }

        public String OpenPermissionLevelForVisitors
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_VISITORS, DEFAULT__OPEN_PERMISION_LEVEL_FOR_VISITORS); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_VISITORS, value); }
        }

        public String OpenPermissionLevelForEveryone
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_EVERYONE, DEFAULT__OPEN_PERMISION_LEVEL_FOR_EVERYONE); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__OPEN_PERMISION_LEVEL_FOR_EVERYONE, value); }
        }


        public String ClosedPermissionLevelForSystemAdmin
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_SYSTEM_ADMIN, DEFAULT__CLOSED_PERMISION_LEVEL_FOR_SYSTEM_ADMIN); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_SYSTEM_ADMIN, value); }
        }

        public String ClosedPermissionLevelForBusinessAdmin
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_BUSINESS_ADMIN, DEFAULT__CLOSED_PERMISION_LEVEL_FOR_BUSINESS_ADMIN); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_BUSINESS_ADMIN, value); }
        }

        public String ClosedPermissionLevelForOwner
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_OWNER, DEFAULT__CLOSED_PERMISION_LEVEL_FOR_OWNER); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_OWNER, value); }
        }

        public String ClosedPermissionLevelForInvolved
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_INVOLVED, DEFAULT__CLOSED_PERMISION_LEVEL_FOR_INVOLVED); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_INVOLVED, value); }
        }

        public String ClosedPermissionLevelForVisitors
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_VISITORS, DEFAULT__CLOSED_PERMISION_LEVEL_FOR_VISITORS); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_VISITORS, value); }
        }

        public String ClosedPermissionLevelForEveryone
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_EVERYONE, DEFAULT__CLOSED_PERMISION_LEVEL_FOR_EVERYONE); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CLOSED_PERMISION_LEVEL_FOR_EVERYONE, value); }
        }


        public bool UseFolderAccessGroupsPattern
        {
            get { return Web.WBxGetBoolProperty(COLLECTION_PROPERTY__USE_FOLDER_ACCESS_GROUPS_PATTERN); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__USE_FOLDER_ACCESS_GROUPS_PATTERN, value); }
        }

        public String FolderAccessGroupsPrefix
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__FOLDER_ACCESS_GROUPS_PREFIX); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__FOLDER_ACCESS_GROUPS_PREFIX, value); }
        }

        public String FolderAccessGroupsFolderNames
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__FOLDER_ACCESS_GROUPS_FOLDER_NAMES); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__FOLDER_ACCESS_GROUPS_FOLDER_NAMES, value); }
        }

        public String FolderAccessGroupPermissionLevel
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__FOLDER_ACCESS_GROUP_PERMISSION_LEVEL); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__FOLDER_ACCESS_GROUP_PERMISSION_LEVEL, value); }
        }

        public String AllFoldersAccessGroupPermissionLevel
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__ALL_FOLDERS_ACCESS_GROUP_PERMISSION_LEVEL); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__ALL_FOLDERS_ACCESS_GROUP_PERMISSION_LEVEL, value); }
        }



        public String UrlForNewWorkBoxDialog
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__URL_FOR_NEW_WORK_BOX_DIALOG, DEFAULT_URL__NEW_WORK_BOX_DIALOG); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__URL_FOR_NEW_WORK_BOX_DIALOG, value); } 
        }

        public String CreateNewWorkBoxText
        {
            get { return Web.WBxGetPropertyOrDefault(COLLECTION_PROPERTY__CREATE_NEW_WORK_BOX_TEXT, DEFAULT__CREATE_NEW_WORK_BOX_TEXT); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__CREATE_NEW_WORK_BOX_TEXT, value); }
        }

        public String DefaultOwningTeamUIControlValue
        {
            get { return Web.WBxGetProperty(COLLECTION_PROPERTY__DEFAULT_OWNING_TEAM); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__DEFAULT_OWNING_TEAM, value); }
        }

        public WBTeam DefaultOwningTeam
        {
            get
            {
                if (DefaultOwningTeamUIControlValue != "")
                {
                    WBTaxonomy teams = WBTaxonomy.GetTeams(this.Site);
                    return new WBTeam(teams, DefaultOwningTeamUIControlValue);
                }
                else
                {
                    WBTermCollection<WBTeam> admins = this.BusinessAdminTeams;
                    WBTeam team = null;
                    if (admins.Count > 0)
                    {
                        team = admins[0];
                    }
                    else
                    {
                        WBTermCollection<WBTeam> sysadmins = this.SystemAdminTeams;
                        if (sysadmins.Count > 0)
                        {
                            team = sysadmins[0];
                        }
                    }
                    return team;
                }

            }
        }


        public bool UsesLinkedCalendars
        {
            get { return Web.WBxGetBoolProperty(COLLECTION_PROPERTY__USES_LINKED_CALENDARS); }
            set { Web.WBxSetProperty(COLLECTION_PROPERTY__USES_LINKED_CALENDARS, value); }
        }


        #endregion

        #region Methods

        public WBAction GetAction(String actionKey)
        {
            WBAction action = new WBAction(actionKey);
            action.SetFromPropertyValue(Web.WBxGetProperty(action.PropertyKey));
            return action;
        }

        public void SetAction(WBAction action)
        {
            Web.WBxSetProperty(action.PropertyKey, action.PropertyValue);
        }


        public WBTemplate GetTypeByID(int id)
        {
            return new WBTemplate(this, id);
        }

        public void Dispose()
        {
            if (_web != null && _webNeedsDisposing) _web.Dispose();
            _web = null;

            if (_site != null && _siteNeedsDisposing) _site.Dispose();
            _site = null;
        }

        public void Update()
        {
            if (_web != null) _web.Update();
        }


        public bool HealthCheckOK
        {
            get { return true; }
        }
        /*
        private bool itemHasCorrectColumns(SPListItem workBoxMetaDataItem)
        {
            return (
                workBoxMetaDataItem != null
                &&
                workBoxMetaDataItem.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_STATUS_CHANGE_REQUEST)
                &&
                workBoxMetaDataItem.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_STATUS)
                &&
                workBoxMetaDataItem.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_ERROR_MESSAGE)
                &&
                workBoxMetaDataItem.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_LINK)
                &&
                workBoxMetaDataItem.Fields.ContainsField(WorkBox.COLUMN_NAME__WORK_BOX_GUID)
                );
        }
        */

        public List<WorkBox> GetWorkBoxes(WBQuery query)
        {
            SPListItemCollection items = List.WBxGetItems(Site, query);

            List<WorkBox> workBoxes = new List<WorkBox>();
            foreach (SPListItem item in items)
            {
                workBoxes.Add(new WorkBox(this, item));
            }

            return workBoxes;
        }

        public DataTable Query(WBQuery query)
        {
            // Maybe should be using: SPSiteDataQuery  class as the basis for this instead of GetItems()

            //return List.GetItems(query.AsSPQuery(Site)).GetDataTable();
            return List.WBxGetDataTable(Site, query);
        }

        public SPListItemCollection QueryFilteredBy(WBTeam team, WBRecordsType recordsType, bool includeRecordsTypeDescendants)
        {
            SPQuery query = Site.WBxMakeCAMLQueryFilterBy(team, recordsType, includeRecordsTypeDescendants);

//            if (query == null) return SPListItemCollection.;

            return List.GetItems(query);
        }

        public SPListItemCollection QueryFilteredBy(WBRecordsType recordsType, String status, bool includeRecordsTypeDescendants)
        {
            SPQuery query = Site.WBxMakeCAMLQueryFilterBy(recordsType, status, includeRecordsTypeDescendants);

            //            if (query == null) return SPListItemCollection.;

            return List.GetItems(query);
        }

        public SPListItemCollection QueryFilteredByStatus(String status)
        {
            SPQuery query = new SPQuery();
            query.Query = "<Where>" + WBUtils.MakeCAMLClauseFilterBy(WorkBox.COLUMN_NAME__WORK_BOX_STATUS, "Text", status) + "</Where>";

            return List.GetItems(query);
        }

        public String GetUrlForNewDialog(WBRecordsType recordsType, WBTeam team)
        {
            return GetUrlForNewDialog(this.Web.Url, recordsType.Id.ToString(), team.Id.ToString(), "", "");
        }

        public String GetUrlForNewDialog(WorkBox relatedWorkBox, String relationType)
        {
            return GetUrlForNewDialog(this.Web.Url, relatedWorkBox.RecordsType.Id.ToString(), "", relatedWorkBox.Url, relationType);
        }

        private String GetUrlForNewDialog(String workBoxCollectionURL, String recordsTypeGUID, String owningTeamGUID, String relatedWorkBoxURL, String relationType)
        {
            string url = this.UrlForNewWorkBoxDialog;

            url = url.Replace("~WorkBoxCollection", workBoxCollectionURL);
            url = url.Replace("[CollectionURL]", workBoxCollectionURL);
            url = url.Replace("[RecordsTypeGUID]", recordsTypeGUID);
            url = url.Replace("[TeamGUID]", owningTeamGUID);
            url = url.Replace("[RelatedWorkBoxURL]", relatedWorkBoxURL);
            url = url.Replace("[RelationType]", relationType);

            return url;
        }


        #endregion

        #region Static Helper Methods

        public static String makePropertyFromList(List<WBCollection> list)
        {
            if (list == null || list.Count == 0) return "";

            List<String> parts = new List<String>();
            foreach (WBCollection collection in list)
            {
                parts.Add(collection.Url);
            }
            return string.Join(";", parts.ToArray());
        }

        public static List<WBCollection> makeListFromProperty(String value)
        {
            List<WBCollection> list = new List<WBCollection>();

            if (value != null && value != "")
            {
                string[] parts = value.Split(';');

                foreach (string part in parts)
                {
                    list.Add(new WBCollection(part));
                }
            }

            return list;
        }

        #endregion


        #region Find and Create Methods

        public WorkBox FindOrCreateNewByLocalID(String localID)
        {
            WorkBox workBox = FindByLocalID(localID);

            if (workBox == null)
            {
                workBox = RequestNewWorkBox("", localID);
            }

            return workBox;
        }

        public WorkBox FindByLocalID(String localID)
        {
            string queryString = "<Where><Eq><FieldRef Name='" + WorkBox.COLUMN_NAME__WORK_BOX_LOCAL_ID + "'/><Value Type='Text'>" + localID + "</Value></Eq></Where>";
            SPQuery query = new SPQuery();
            query.Query = queryString;

            SPListItemCollection items = List.GetItems(query);

            if (items.Count == 0) return null;
            if (items.Count > 1) WBUtils.shouldThrowError("There should only be one work box with a given local ID !!");

            SPListItem item = items[0];

            return new WorkBox(this, item);
        }


        public WBTemplate DefaultTemplate()
        {
                // For the moment just going to return the first work box template found:
                foreach (SPListItem item in TemplatesList.Items)
                {
                    WBTemplate template = new WBTemplate(this, item);
                    if (template.IsActive) return template;
                }

                throw new Exception("Couldn't find a default work box template!");
                //return null;
        }

        public WBTemplate DefaultTemplate(WBRecordsType recordsType)
        {
            WBTemplate foundAnyActive = null;

            foreach (SPListItem item in TemplatesList.Items)
            {
                WBTemplate template = new WBTemplate(this, item);

                if (template.Status == WorkBox.WORK_BOX_TEMPLATE_STATUS__ACTIVE_DEFAULT)
                    return template;

                if (template.IsActive)
                {
                    foundAnyActive = template;
                }
            }

            return foundAnyActive;
        }


        public List<WBTemplate> ActiveTemplates(WBRecordsType recordsType)
        {
            List<WBTemplate> templates = new List<WBTemplate>();

            foreach (SPListItem item in TemplatesList.Items)
            {
                WBTemplate template = new WBTemplate(this, item);

                if (template.IsActive)
                {
                    // We can pass in null as we're not going to be using anything except the ID:
                    WBRecordsType templateRecordsType = template.RecordsType(null);

                    if (templateRecordsType.Id.Equals(recordsType.Id))
                    {
                        templates.Add(template);
                    }
                }
            }

            return templates;
        }



        public WorkBox RequestNewEventWorkBox(String calendarURL, Guid calendarGuid, int eventID, String shortTitle, String description, DateTime eventDate, DateTime endDate, WBTeam owningTeam, WBTermCollection<WBTeam> involvedTeams, String templateTitle)
        {
            WBLogging.WorkBoxCollections.Unexpected("In: RequestNewEventWorkBox()");

            SPListItem foundTemplateItem = WBUtils.FindItemByColumn(this.Site, this.TemplatesList, WBColumn.Title, templateTitle);

            WBTemplate template = this.DefaultTemplate();
            if (foundTemplateItem == null)
            {
                WBLogging.WorkBoxCollections.Unexpected("Could not find a template with the title: " + templateTitle + " so just using the default template!!");
            }
            else
            {
                template = new WBTemplate(this, foundTemplateItem);
            }
            
            Hashtable extraBits = new Hashtable();
            extraBits["EventDate"] = eventDate;
            extraBits["ReferenceDate"] = eventDate;
            extraBits["EndDate"] = endDate;
            extraBits["WorkBoxLinkedCalendars"] = calendarURL + "|" + calendarGuid + "|" + eventID;

            return RequestNewWorkBox(shortTitle, "", template, owningTeam, involvedTeams, extraBits);
        }


        public WorkBox RequestNewWorkBox()
        {
            return RequestNewWorkBox("", "", null, null, null, null);
        }

        public WorkBox RequestNewWorkBox(String shortTitle)
        {
            return RequestNewWorkBox(shortTitle, "", null, null, null, null);
        }

        public WorkBox RequestNewWorkBox(String shortTitle, String localID)
        {
            return RequestNewWorkBox(shortTitle, localID, null, null, null, null);
        }

        public WorkBox RequestNewWorkBox(String shortTitle, String localID, WBTemplate type, WBTeam owningTeam, WBTermCollection<WBTeam> involvedTeams)
        {
            return RequestNewWorkBox(shortTitle, localID, type, owningTeam, involvedTeams, null);
        }

        public WorkBox RequestNewWorkBox(String shortTitle, String localID, WBTemplate template, WBTeam owningTeam, WBTermCollection<WBTeam> involvedTeams, Hashtable extraRequiredColumnValues)
        {
            WBLogging.WorkBoxCollections.Unexpected("In: RequestNewWorkBox()");

            // First let's make sure that we have a template to be using:
            if (template == null) template = DefaultTemplate();

            WorkBox newWorkBox = template.GetPrecreatedWorkBoxIfAny(shortTitle);
            SPListItem newItem = null;
            bool usingPrecreatedWorkBox = false;
            if (newWorkBox != null)
            {
                usingPrecreatedWorkBox = true;
                newItem = newWorkBox.Item;
            }

            using (EventsFiringDisabledScope noevents = new EventsFiringDisabledScope())
            {
                if (newItem == null)
                {
                    newItem = List.AddItem();
                    newWorkBox = new WorkBox(this, newItem);
                }

                /* First we set the required items: */
                newWorkBox.Template = template;

                newWorkBox.RecordsType = template.RecordsType(newWorkBox.RecordsTypes);
                //newItem[WorkBox.COLUMN_NAME__RECORDS_TYPE] = template.Item[WorkBox.COLUMN_NAME__RECORDS_TYPE];

                if (extraRequiredColumnValues != null)
                {
                    foreach (DictionaryEntry entry in extraRequiredColumnValues)
                    {
                        switch (entry.Key.WBxToString())
                        {
                            case  WorkBox.COLUMN_NAME__FUNCTIONAL_AREA:
                                {
                                    newItem.WBxSetMultiTermColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, entry.Value.WBxToString());
                                    break;
                                }

                            case WorkBox.COLUMN_NAME__SERIES_TAG:
                                {
                                    newItem.WBxSetSingleTermColumn(WorkBox.COLUMN_NAME__SERIES_TAG, entry.Value.WBxToString());
                                    break;
                                }

                            default: 
                                {
                                    WBLogging.WorkBoxCollections.Unexpected("Setting extra bit: " + entry.Key.WBxToString());
                                    WBLogging.WorkBoxCollections.Unexpected("To have value: " + entry.Value.WBxToString());
                                    newItem[entry.Key.WBxToString()] = entry.Value;
                                    break;
                                }
                        }
                    }
                }

                newWorkBox.FirstUseOfWorkBox = true;

                /* Then do an initial update to ensure that the item is assigned an ID: */
                newWorkBox.Update();

                newWorkBox.ShortTitle = shortTitle.WBxTrim(); // This WBxTrim is as much to check for null as to trim the title passed in.


                if (!usingPrecreatedWorkBox) 
                {
                    if (localID == null || localID == "")
                    {
                        newWorkBox.GenerateLocalID();
                    }
                    else 
                    { 
                        newWorkBox.SetLocalID(localID); 
                    }
                    newWorkBox.GenerateUniqueID();
                }

                if (owningTeam == null) owningTeam = DefaultOwningTeam;
                newWorkBox.OwningTeam = owningTeam;

                if (involvedTeams != null) newWorkBox.InvolvedTeams = involvedTeams;

                if (String.IsNullOrEmpty(newWorkBox.Status))
                {
                    newWorkBox.Status = WorkBox.WORK_BOX_STATUS__REQUESTED;
                }
                newWorkBox.GenerateTitle();
                newWorkBox.Update();

                return newWorkBox;
            }
        }

        #endregion
    }
}

