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
using System.Collections.Specialized;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework
{
    /// <summary>
    /// This class is used to hold farm wide settings for the Work Box Framework. The values can be edited via Central Administration.
    /// </summary>
    public class WBFarm
    {
        private const string FARM_PROPERTY__FARM_INSTANCE = "wbf__farm__farm_instance";

        private const string FARM_PROPERTY__RECORDS_CENTER_URL = "wbf__farm__records_center_url";
        private const string FARM_PROPERTY__RECORDS_CENTER_RECORDS_LIBRARY_NAME = "wbf__farm__records_center_records_library_name";
        private const string FARM_PROPERTY__RECORDS_CENTER_DROP_OFF_URL = "wbf__farm__records_center_drop_off_url";

        private const string FARM_PROPERTY__PROTECTED_RECORDS_LIBRARY_URL = "wbf__farm__protected_records_library_url";
        private const string FARM_PROPERTY__PUBLIC_RECORDS_LIBRARY_URL = "wbf__farm__public_records_library_url";
        private const string FARM_PROPERTY__PUBLIC_EXTRANET_RECORDS_LIBRARY_URL = "wbf__farm__public_extranet_records_library_url";


        private const string FARM_PROPERTY__TEAM_SITES_SITE_COLLECTION_URL = "wbf__farm__team_sites_site_collection_url";

        private const string FARM_PROPERTY__SYSTEM_ADMIN_TEAM_GUID = "wbf__farm__system_admin_team_guid";

        private const string FARM_PROPERTY__OPEN_WORK_BOXES_CACHED_DETAILS_LIST_URL = "wbf__farm__open_work_boxes_cached_details_list_url";
        private const string FARM_PROPERTY__TICKS_WHEN_LAST_UPDATED_RECENTLY_VISITED = "wbf__farm__ticks_when_last_updated_recently_visited";


        private const string FARM_PROPERTY__RECORDS_MANAGERS_GROUP_NAME = "wbf__farm__records_managers_group_name";
        private const string FARM_PROPERTY__RECORDS_SYSTEM_ADMIN_GROUP_NAME = "wbf__farm__records_system_admin_group_name";


//        private const string FARM_PROPERTY__TIMER_JOB_WEB_APPLICATION = "wbf__farm__timer_job_web_application";
        private const string FARM_PROPERTY__TIMER_JOBS_MANAGEMENT_SITE_URL = "wbf__farm__timer_jobs_management_url";
        private const string FARM_PROPERTY__TIMER_JOBS_SERVER_NAME = "wbf__farm__timer_jobs_server_name";



        private const string FARM_PROPERTY__TERM_STORE_NAME = "wbf__farm__term_store_name";
        private const string FARM_PROPERTY__TERM_STORE_GROUP_NAME = "wbf__farm__term_store_group_name";

        private const string FARM_PROPERTY__WORK_BOX_DOCUMENT_CONTENT_TYPE_NAME = "wbf__farm__work_box_document_content_type_name";
        private const string FARM_PROPERTY__WORK_BOX_RECORD_CONTENT_TYPE_NAME = "wbf__farm__work_box_record_content_type_name";

        private const string FARM_PROPERTY__INVITE_INVOLVED_DEFAULT_EMAIL_SUBJECT = "wbf__farm__invite_involved_default_email_subject";
        private const string FARM_PROPERTY__INVITE_INVOLVED_DEFAULT_EMAIL_BODY = "wbf__farm__invite_involved_default_email_body";
        private const string FARM_PROPERTY__INVITE_VISITING_DEFAULT_EMAIL_SUBJECT = "wbf__farm__invite_visiting_default_email_subject";
        private const string FARM_PROPERTY__INVITE_VISITING_DEFAULT_EMAIL_BODY = "wbf__farm__invite_visiting_default_email_body";

        private const string FARM_PROPERTY__INVITE_TO_TEAM_DEFAULT_EMAIL_SUBJECT = "wbf__farm__invite_to_team_default_email_subject";
        private const string FARM_PROPERTY__INVITE_TO_TEAM_DEFAULT_EMAIL_BODY = "wbf__farm__invite_to_team_default_email_body";


        private const string FARM_PROPERTY__ALL_WORK_BOX_COLLECTIONS = "wbf__farm__all_work_box_collections";

        private const string FARM_PROPERTY__MIGRATION_TYPE = "wbf__farm__migration_type";
        private const string FARM_PROPERTY__MIGRATION_SOURCE_SYSTEM = "wbf__farm__migration_source_system";
        private const string FARM_PROPERTY__MIGRATION_CONTROL_LIST_URL = "wbf__farm__migration_control_list_url";
        private const string FARM_PROPERTY__MIGRATION_CONTROL_LIST_VIEW = "wbf__farm__migration_control_list_view";
        private const string FARM_PROPERTY__MIGRATION_MAPPING_LIST_URL = "wbf__farm__migration_mapping_list_url";
        private const string FARM_PROPERTY__MIGRATION_MAPPING_LIST_VIEW = "wbf__farm__migration_mapping_list_view";

        private const string FARM_PROPERTY__MIGRATION_SUBJECTS_LIST_URL = "wbf__farm__migration_subjects_list_url";
        private const string FARM_PROPERTY__MIGRATION_SUBJECTS_LIST_VIEW = "wbf__farm__migration_subjects_list_view";

        private const string FARM_PROPERTY__MIGRATION_ITEMS_PER_CYCLE = "wbf__farm__migration_items_per_cycle";

        private const string FARM_PROPERTY__MIGRATION_USER_NAME = "wbf__farm__migration_user_name";
        private const string FARM_PROPERTY__MIGRATION_PASSWORD = "wbf__farm__migration_password";

        internal const string MIGRATION_TYPE__NONE = "None";
        internal const string MIGRATION_TYPE__MIGRATE_IZZI_PAGES = "Migrate izzi Pages";
        internal const string MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_LIBRARY = "Migrate Documents To Records Library";
        internal const string MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_WORK_BOXES = "Migrate Documents To Work Boxes";

        public const string FARM_INSTANCE__PRODUCTION_FARM = "Production Farm";
        public const string FARM_INSTANCE__DEVELOPMENT_FARM = "Development Farm";
        public const string FARM_INSTANCE__PROTECTED_INTERNAL_FARM = "Protected Internal Farm";
        public const string FARM_INSTANCE__PUBLIC_EXTERNAL_FARM = "Public External Farm";

        #region Constructors

        private SPFarm _farm;
        private WBFarm()
        {
            _farm = SPFarm.Local;
        }
        
        public static WBFarm Local
        {
            get { return new WBFarm(); }
        }

        #endregion

        #region Properties

        public SPFarm SPFarm
        {
            get { return _farm; }
        }

        public String FarmInstance
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__FARM_INSTANCE); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__FARM_INSTANCE, value); }
        }

        /*
        public String RecordsCenterUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__RECORDS_CENTER_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__RECORDS_CENTER_URL, value); }
        }

        public String RecordsCenterRecordsLibraryName
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__RECORDS_CENTER_RECORDS_LIBRARY_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__RECORDS_CENTER_RECORDS_LIBRARY_NAME, value); }
        }

        public String RecordsCenterDropOffUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__RECORDS_CENTER_DROP_OFF_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__RECORDS_CENTER_DROP_OFF_URL, value); }
        }
        */

        public String ProtectedRecordsLibraryUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__PROTECTED_RECORDS_LIBRARY_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__PROTECTED_RECORDS_LIBRARY_URL, value); }
        }

        public String PublicRecordsLibraryUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__PUBLIC_RECORDS_LIBRARY_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__PUBLIC_RECORDS_LIBRARY_URL, value); }
        }

        public String PublicExtranetRecordsLibraryUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__PUBLIC_EXTRANET_RECORDS_LIBRARY_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__PUBLIC_EXTRANET_RECORDS_LIBRARY_URL, value); }
        }

        public String OpenWorkBoxesCachedDetailsListUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__OPEN_WORK_BOXES_CACHED_DETAILS_LIST_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__OPEN_WORK_BOXES_CACHED_DETAILS_LIST_URL, value); }
        }

        public long TicksWhenLastUpdatedRecentlyVisited
        {
            get { 
                string ticksString = _farm.WBxGetProperty(FARM_PROPERTY__TICKS_WHEN_LAST_UPDATED_RECENTLY_VISITED);
                if (String.IsNullOrEmpty(ticksString)) return 0;
                return Convert.ToInt64(ticksString);
            }
            set 
            { 
                _farm.WBxSetProperty(FARM_PROPERTY__TICKS_WHEN_LAST_UPDATED_RECENTLY_VISITED, value); 
            }
        }

        
        /// <summary>
        /// The Team Sites site collection is where all of the master SPGroups are defined for the various
        /// teams. All teams must therefore be created and managed on this site collection. 
        /// </summary>
        /// <remarks>
        /// The The SPGroups and their members are automatically copied from this site collection to 
        /// other site collections participating in the Work Box Framework (WBF). The WBF feature for Team Management
        /// should only be activated on this site collection.
        /// </remarks>      
        public String TeamSitesSiteCollectionUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__TEAM_SITES_SITE_COLLECTION_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__TEAM_SITES_SITE_COLLECTION_URL, value); }
        }
        
        public String SystemAdminTeamGUIDString
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__SYSTEM_ADMIN_TEAM_GUID); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__SYSTEM_ADMIN_TEAM_GUID, value); }
        }
        
        public String RecordsManagersGroupName
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__RECORDS_MANAGERS_GROUP_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__RECORDS_MANAGERS_GROUP_NAME, value); }
        }

        public String RecordsSystemAdminGroupName
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__RECORDS_SYSTEM_ADMIN_GROUP_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__RECORDS_SYSTEM_ADMIN_GROUP_NAME, value); }
        }


        /// <summary>
        /// This is the URL for the site on which the various daily timer jobs details are managed and reported on.
        /// </summary>
        public String TimerJobsManagementSiteUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__TIMER_JOBS_MANAGEMENT_SITE_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__TIMER_JOBS_MANAGEMENT_SITE_URL, value); }
        }

        /// <summary>
        /// The name of the server on which the timer jobs will be run.
        /// </summary>
        public String TimerJobsServerName
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__TIMER_JOBS_SERVER_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__TIMER_JOBS_SERVER_NAME, value); }
        }


        /// <summary>
        /// The name of the managed metadata term store that will be used.
        /// </summary>
        public String TermStoreName
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__TERM_STORE_NAME, WorkBox.TERM_STORE_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__TERM_STORE_NAME, value); }
        }

        /// <summary>
        /// The name of the managed metadata term store group that will be used.
        /// </summary>
        public String TermStoreGroupName
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__TERM_STORE_GROUP_NAME, WorkBox.TERM_STORE_GROUP_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__TERM_STORE_GROUP_NAME, value); }
        }

        public String WorkBoxDocumentContentTypeName
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__WORK_BOX_DOCUMENT_CONTENT_TYPE_NAME, WorkBox.WORK_BOX_DOCUMENT_CONTENT_TYPE_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__WORK_BOX_DOCUMENT_CONTENT_TYPE_NAME, value); }
        }

        public String WorkBoxRecordContentTypeName
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__WORK_BOX_RECORD_CONTENT_TYPE_NAME, WorkBox.WORK_BOX_RECORD_CONTENT_TYPE_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__WORK_BOX_RECORD_CONTENT_TYPE_NAME, value); }
        }

        public String InviteInvolvedDefaultEmailSubject
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__INVITE_INVOLVED_DEFAULT_EMAIL_SUBJECT,  "You have been invited to be involved with a work box"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__INVITE_INVOLVED_DEFAULT_EMAIL_SUBJECT, value); }
        }

        public String InviteInvolvedDefaultEmailBody
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__INVITE_INVOLVED_DEFAULT_EMAIL_BODY, "You have been invited to be involved with the work box: [WORK_BOX_TITLE].\n\nYou can get involved with the work box here: [WORK_BOX_URL]"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__INVITE_INVOLVED_DEFAULT_EMAIL_BODY, value); }
        }

        public String InviteVisitingDefaultEmailSubject
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__INVITE_VISITING_DEFAULT_EMAIL_SUBJECT, "You have been invited to be a visitor to a work box"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__INVITE_VISITING_DEFAULT_EMAIL_SUBJECT, value); }
        }

        public String InviteVisitingDefaultEmailBody
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__INVITE_VISITING_DEFAULT_EMAIL_BODY, "You have been invited to be a visitor to the work box: [WORK_BOX_TITLE].\n\nYou can visit the work box here: [WORK_BOX_URL]"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__INVITE_VISITING_DEFAULT_EMAIL_BODY, value); }
        }

        public String InviteToTeamDefaultEmailSubject
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__INVITE_TO_TEAM_DEFAULT_EMAIL_SUBJECT, "You have been invited to a team"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__INVITE_TO_TEAM_DEFAULT_EMAIL_SUBJECT, value); }
        }

        public String InviteToTeamDefaultEmailBody
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__INVITE_TO_TEAM_DEFAULT_EMAIL_BODY, "You have been invited to be a [ROLE_WITHIN_TEAM] of the team [TEAM_NAME]. \n\nYou can visit the team's site here: [TEAM_SITE_URL]"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__INVITE_TO_TEAM_DEFAULT_EMAIL_BODY, value); }
        }


        public List<WBCollection> AllWorkBoxCollections
        {
            get { return WBCollection.makeListFromProperty(_farm.WBxGetProperty(FARM_PROPERTY__ALL_WORK_BOX_COLLECTIONS)); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__ALL_WORK_BOX_COLLECTIONS, WBCollection.makePropertyFromList(value)); }
        }

        // only used for the farm wide settigs - should probably remove this one day!
        public String AllWorkBoxCollectionsPropertyValue
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__ALL_WORK_BOX_COLLECTIONS); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__ALL_WORK_BOX_COLLECTIONS, value); }
        }

        public List<WBCollection> PublicWorkBoxCollections
        {
            get { return null; } 
        }

        public String MigrationType
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__MIGRATION_TYPE, MIGRATION_TYPE__NONE); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_TYPE, value); }
        }

        public String MigrationSourceSystem
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_SOURCE_SYSTEM); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_SOURCE_SYSTEM, value); }
        }

        public String MigrationControlListUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_CONTROL_LIST_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_CONTROL_LIST_URL, value); }
        }

        public String MigrationControlListView
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_CONTROL_LIST_VIEW); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_CONTROL_LIST_VIEW, value); }
        }

        public String MigrationMappingListUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_MAPPING_LIST_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_MAPPING_LIST_URL, value); }
        }

        public String MigrationMappingListView
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_MAPPING_LIST_VIEW); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_MAPPING_LIST_VIEW, value); }
        }

        public String MigrationSubjectsListUrl
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_SUBJECTS_LIST_URL); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_SUBJECTS_LIST_URL, value); }
        }

        public String MigrationSubjectsListView
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_SUBJECTS_LIST_VIEW); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_SUBJECTS_LIST_VIEW, value); }
        }

        public String MigrationItemsPerCycle
        {
            get { return _farm.WBxGetPropertyOrDefault(FARM_PROPERTY__MIGRATION_ITEMS_PER_CYCLE, "10"); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_ITEMS_PER_CYCLE, value); }
        }


        public String MigrationUserName
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_USER_NAME); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_USER_NAME, value); }
        }

        public String MigrationPassword
        {
            get { return _farm.WBxGetProperty(FARM_PROPERTY__MIGRATION_PASSWORD); }
            set { _farm.WBxSetProperty(FARM_PROPERTY__MIGRATION_PASSWORD, value); }
        }

        

        #endregion


        #region Methods

        public void Update()
        {
            _farm.Update();
        }


        public WBTeam SystemAdminTeam(SPSite site)
        {
            if (String.IsNullOrEmpty(SystemAdminTeamGUIDString)) return null;

            WBTaxonomy teamsTaxonomy = WBTaxonomy.GetTeams(site);
            return SystemAdminTeam(teamsTaxonomy);
        }

        public WBTeam SystemAdminTeam(WBTaxonomy teamsTaxonomy)
        {
            if (String.IsNullOrEmpty(SystemAdminTeamGUIDString)) return null;
            return new WBTeam(teamsTaxonomy, new Guid(SystemAdminTeamGUIDString));
        }

        public bool IsCurrentUserSystemAdmin()
        {
            if (SPContext.Current == null) return false;

            WBTeam sysadminTeam = SystemAdminTeam(SPContext.Current.Site);

            if (sysadminTeam == null) return false; 
            return sysadminTeam.IsCurrentUserTeamMember();
        }


        public void InitialFarmSetup(SPSite site, String adminTeamSiteURL, String serverForTimerJobs)
        {
            WBLogging.Generic.Monitorable("Running WBFarm.InitialFarmSetup()");

            InitialSetupOfTermSets(site);

            InitialSetupOfUserProfileProperties(site);

            using (SPSite adminSite = new SPSite(adminTeamSiteURL))
            using (SPWeb adminWeb = adminSite.OpenWeb())
            {
                SPWeb rootTeamSiteWeb = adminSite.RootWeb;

                CreateOrCheckWBFSiteColumns(adminSite, rootTeamSiteWeb);

                CreateOrCheckTeamSitesContentTypes(adminSite, rootTeamSiteWeb);

                CreateOrCheckCachedDetailsList(rootTeamSiteWeb);

                CreateOrCheckTimerTasksLists(rootTeamSiteWeb, adminWeb);

                this.TeamSitesSiteCollectionUrl = adminSite.Url;
                this.TimerJobsManagementSiteUrl = adminTeamSiteURL;
                this.OpenWorkBoxesCachedDetailsListUrl = adminSite.Url + "/Lists/CachedWorkBoxDetails";

                this.TimerJobsServerName = serverForTimerJobs;

                this.Update();

                RegisterTimerJobs(adminSite);

                if (rootTeamSiteWeb != adminWeb && (SPContext.Current == null || rootTeamSiteWeb != SPContext.Current.Web))
                {
                    rootTeamSiteWeb.Dispose();
                }
            }

            WBLogging.Generic.Monitorable("Completed WBFarm.InitialFarmSetup()");
        }

        internal void InitialWBCollectionSetup(SPSite site)
        {
            WBLogging.Generic.Monitorable("Running WBFarm.InitialWBCollectionSetup()");

            SPWeb rootWeb = site.RootWeb;

            CreateOrCheckWBFSiteColumns(site, rootWeb);

            CreateOrCheckWBCSiteContentTypes(site, rootWeb);

            if (SPContext.Current == null || rootWeb != SPContext.Current.Web)
            {
                rootWeb.Dispose();
            }

            WBLogging.Generic.Monitorable("Completed WBFarm.InitialWBCollectionSetup()");
        }


        private void InitialSetupOfTermSets(SPSite site)
        {
            WBLogging.Generic.Monitorable("Started term set initial setup.");

            TaxonomySession session = new TaxonomySession(site);
            WBFarm farm = WBFarm.Local;
            TermStore termStore = null;

            try
            {
                termStore = session.TermStores[farm.TermStoreName];
            }
            catch (Exception exception)
            {
                WBLogging.Generic.Unexpected("Couldn't find the term store with name: " + farm.TermStoreName, exception);
                return;
            }
            WBLogging.Generic.Unexpected("Found term store: " + termStore.Name);

            Group group = null;

            try
            {
                group = termStore.Groups[farm.TermStoreGroupName];
            }
            catch (Exception exception)
            {
                WBLogging.Generic.Unexpected("Couldn't find the term store group with name: " + farm.TermStoreGroupName, exception);
            }

            bool needsCommitting = false;

            if (group == null)
            {
                WBLogging.Generic.Unexpected("Trying to create term store group with name: " + farm.TermStoreGroupName);

                try
                {
                    group = termStore.CreateGroup(farm.TermStoreGroupName);
                    needsCommitting = true;
                }
                catch (Exception exception)
                {
                    WBLogging.Generic.Unexpected("Couldn't create a term store group with name: " + farm.TermStoreGroupName, exception);
                    return;
                }
            }

            WBLogging.Generic.Unexpected("Created the term store group with name: " + farm.TermStoreGroupName);


            TermSet recordsTypes = null;
            try
            {
                recordsTypes = group.TermSets[WorkBox.TERM_SET_NAME__RECORDS_TYPES];
            }
            catch (Exception e)
            {
                group.CreateTermSet(WorkBox.TERM_SET_NAME__RECORDS_TYPES);
                needsCommitting = true;
            }

            TermSet functionalAreas = null;
            try
            {
                functionalAreas = group.TermSets[WorkBox.TERM_SET_NAME__FUNCTIONAL_AREAS];
            }
            catch (Exception e)
            {
                group.CreateTermSet(WorkBox.TERM_SET_NAME__FUNCTIONAL_AREAS);
                needsCommitting = true;
            }

            TermSet teams = null;
            try
            {
                teams = group.TermSets[WorkBox.TERM_SET_NAME__TEAMS];
            }
            catch (Exception e)
            {
                group.CreateTermSet(WorkBox.TERM_SET_NAME__TEAMS);
                needsCommitting = true;
            }


            TermSet subjectTags = null;
            try
            {
                subjectTags = group.TermSets[WorkBox.TERM_SET_NAME__SUBJECT_TAGS];
            }
            catch (Exception e)
            {
                group.CreateTermSet(WorkBox.TERM_SET_NAME__SUBJECT_TAGS);
                needsCommitting = true;
            }

            TermSet seriesTags = null;
            try
            {
                seriesTags = group.TermSets[WorkBox.TERM_SET_NAME__SERIES_TAGS];
            }
            catch (Exception e)
            {
                group.CreateTermSet(WorkBox.TERM_SET_NAME__SERIES_TAGS);
                needsCommitting = true;
            }


            if (needsCommitting)
                termStore.CommitAll();

            WBLogging.Generic.Monitorable("Finished term set initial setup.");
        }



        internal void CreateOrCheckCachedDetailsList(SPWeb rootWeb)
        {
            WBColumn[] columns = 
            {             
                WBColumn.WorkBoxStatus,
                WBColumn.WorkBoxURL,
                WBColumn.WorkBoxGUID,

                WBColumn.WorkBoxDateLastModified,
                WBColumn.WorkBoxDateLastVisited,
                WBColumn.WorkBoxDateCreated,

                WBColumn.FunctionalArea,
                WBColumn.RecordsType,
                WBColumn.SubjectTags,
                WBColumn.ReferenceID,
                WBColumn.ReferenceDate,
                WBColumn.SeriesTag,
                WBColumn.OwningTeam,
                WBColumn.InvolvedTeams,
                WBColumn.VisitingTeams
            };

            WBUtils.CreateOrCheckCustomList(rootWeb, rootWeb, "CachedWorkBoxDetails", columns);

        }

        internal void CreateOrCheckTimerTasksLists(SPWeb rootweb, SPWeb web)
        {
            WBColumn[] columns = 
            {             
                WBColumn.ExecutionOrder,
                WBColumn.Command,
                WBColumn.TargetURL,
                WBColumn.Argument1
            };

            SPList dailyList = WBUtils.CreateOrCheckCustomList(rootweb, web, WBTimerTasksJob.DAILY_TIMER_TASKS__LIST_NAME, columns);

            SPViewCollection dailyViews = dailyList.Views;

            StringCollection viewFields = new StringCollection();

            viewFields.Add(WBColumn.ExecutionOrder.DisplayName); 
            viewFields.Add(WBColumn.Title.DisplayName); 
            viewFields.Add(WBColumn.Command.DisplayName);
            viewFields.Add(WBColumn.TargetURL.DisplayName);
            viewFields.Add(WBColumn.Argument1.DisplayName);
          
            string orderByCAML = "<OrderBy><FieldRef Name=\"ExecutionOrder\" Ascending=\"TRUE\" /></OrderBy>";

            dailyViews.Add(WBTimerTasksJob.DAILY_TIMER_TASKS__ORDERED_VIEW_NAME, viewFields, orderByCAML, 500, true, true);
            dailyList.Update();
            web.Update();

            SPList frequentList = WBUtils.CreateOrCheckCustomList(rootweb, web, WBTimerTasksJob.FREQUENT_TIMER_TASKS__LIST_NAME, columns);

            SPViewCollection frequentViews = frequentList.Views;

            frequentViews.Add(WBTimerTasksJob.FREQUENT_TIMER_TASKS__ORDERED_VIEW_NAME, viewFields, orderByCAML, 500, true, true);
            frequentList.Update();
            web.Update();

        }


        internal void CreateOrCheckWBFSiteColumns(SPSite site, SPWeb rootWeb)
        {
            WBLogging.Generic.Monitorable("Starting CreateOrCheckWBFSiteColumns");

            WBColumn[] columnsToCreate = 
            {
                WBColumn.WorkBoxStatus,
                WBColumn.WorkBoxStatusChangeRequest,
                WBColumn.WorkBoxLink,
                WBColumn.WorkBoxURL,
                WBColumn.WorkBoxGUID,
                WBColumn.WorkBoxLocalID,
                WBColumn.WorkBoxUniqueID,
                WBColumn.WorkBoxShortTitle,
                WBColumn.WorkBoxAuditLog,
                WBColumn.WorkBoxErrorMessage,

                WBColumn.WorkBoxCachedListItemID,

                WBColumn.WorkBoxDateLastModified,
                WBColumn.WorkBoxDateLastVisited,
                WBColumn.WorkBoxDateCreated,
                WBColumn.WorkBoxDateDeleted,
                WBColumn.WorkBoxDateLastClosed,
                WBColumn.WorkBoxDateLastOpened,
                WBColumn.WorkBoxRetentionEndDate,

                WBColumn.FunctionalArea,
                WBColumn.RecordsType,
                WBColumn.SubjectTags,
                WBColumn.ReferenceID,
                WBColumn.ReferenceDate,
                WBColumn.ScanDate,
                WBColumn.SeriesTag,
                WBColumn.OwningTeam,
                WBColumn.InvolvedTeams,
                WBColumn.VisitingTeams,
                WBColumn.InvolvedIndividuals,
                WBColumn.VisitingIndividuals,
                WBColumn.ProtectiveZone,
                WBColumn.OriginalFilename,
                WBColumn.SourceSystem,
                WBColumn.SourceID,
                WBColumn.RecordID,
                WBColumn.LiveOrArchived,

                WBColumn.WorkBoxTemplateTitle,
                WBColumn.WorkBoxTemplateStatus,
                WBColumn.WorkBoxDocumentTemplates,
                WBColumn.WorkBoxInviteInvovledEmailSubject,
                WBColumn.WorkBoxInviteInvovledEmailBody,
                WBColumn.WorkBoxInviteVisitingEmailSubject,
                WBColumn.WorkBoxInviteVisitingEmailBody,
                WBColumn.WorkBoxTemplateUseFolderPattern,
                WBColumn.WorkBoxTemplateName,
           
                WBColumn.ExecutionOrder,
                WBColumn.Command,
                WBColumn.TargetURL,
                WBColumn.Argument1
                                          
            };

            foreach (WBColumn column in columnsToCreate)
            {
                column.CreateOrCheck(site, rootWeb);
            }

            WBLogging.Generic.Monitorable("Finished CreateOrCheckWBFSiteColumns");
        }

        internal void CreateOrCheckWBCSiteContentTypes(SPSite site, SPWeb rootWeb)
        {
            WBLogging.Generic.Monitorable("Starting CreateOrCheckWBCSiteContentTypes");

            CreateOrCheckWorkBoxMetadataItemContentType(rootWeb);

            CreateOrCheckWorkBoxTemplatesItemContentType(rootWeb);

            CreateOrCheckWorkBoxDocumentContentType(rootWeb);

            // Not really the right place to be creating this content type:
            CreateOrCheckWorkBoxRecordContentType(rootWeb);

            WBLogging.Generic.Monitorable("Completed CreateOrCheckWBCSiteContentTypes");
        }

        internal void CreateOrCheckTeamSitesContentTypes(SPSite site, SPWeb rootWeb)
        {
            WBLogging.Generic.Monitorable("Starting CreateOrCheckTeamSitesContentTypes");

            CreateOrCheckWorkBoxDocumentContentType(rootWeb);

            // Not really the right place to be creating this content type:
            CreateOrCheckWorkBoxRecordContentType(rootWeb);

            WBLogging.Generic.Monitorable("Completed CreateOrCheckTeamSitesContentTypes");
        }


        private void CreateOrCheckWorkBoxMetadataItemContentType(SPWeb web)
        {
            WBColumn[] requiredFields = {};

            WBColumn[] optionalFields = 
            {
                WBColumn.WorkBoxStatus,
                WBColumn.WorkBoxStatusChangeRequest,
                WBColumn.WorkBoxLink,
                WBColumn.WorkBoxURL,
                WBColumn.WorkBoxGUID,
                WBColumn.WorkBoxLocalID,
                WBColumn.WorkBoxUniqueID,
                WBColumn.WorkBoxShortTitle,
                WBColumn.WorkBoxAuditLog,
                WBColumn.WorkBoxErrorMessage,

                WBColumn.WorkBoxCachedListItemID,

                WBColumn.WorkBoxDateLastModified,
                WBColumn.WorkBoxDateLastVisited,
                WBColumn.WorkBoxDateCreated,
                WBColumn.WorkBoxDateDeleted,
                WBColumn.WorkBoxDateLastClosed,
                WBColumn.WorkBoxDateLastOpened,
                WBColumn.WorkBoxRetentionEndDate,

                WBColumn.FunctionalArea,
                WBColumn.RecordsType,
                WBColumn.ReferenceID,
                WBColumn.ReferenceDate,
                WBColumn.SeriesTag,
                WBColumn.OwningTeam,
                WBColumn.InvolvedTeams,
                WBColumn.VisitingTeams,
                WBColumn.InvolvedIndividuals,
                WBColumn.VisitingIndividuals
            };

            WBUtils.CreateOrCheckContentType(web, WorkBox.CONTENT_TYPE__WORK_BOX_METADATA_ITEM, "Item", WorkBox.SITE_CONTENT_TYPES_GROUP_NAME, requiredFields, optionalFields);
        }

        private void CreateOrCheckWorkBoxTemplatesItemContentType(SPWeb web)
        {
            WBColumn[] requiredFields = 
            { 
                WBColumn.RecordsType,
                WBColumn.WorkBoxTemplateTitle,
                WBColumn.WorkBoxTemplateStatus                           
            };


            WBColumn descriptionField = new WBColumn("Description", "RoutingRuleDescription", WBColumn.DataTypes.Text);

            WBColumn[] optionalFields = 
            {
                descriptionField,
                WBColumn.WorkBoxDocumentTemplates,
                WBColumn.WorkBoxInviteInvovledEmailSubject,
                WBColumn.WorkBoxInviteInvovledEmailBody,
                WBColumn.WorkBoxInviteVisitingEmailSubject,
                WBColumn.WorkBoxInviteVisitingEmailBody,
                WBColumn.WorkBoxTemplateUseFolderPattern,
                WBColumn.WorkBoxTemplateName
            };

            WBUtils.CreateOrCheckContentType(web, WorkBox.CONTENT_TYPE__WORK_BOX_TEMPLATES_ITEM, "Item", WorkBox.SITE_CONTENT_TYPES_GROUP_NAME, requiredFields, optionalFields);
        }

        private void CreateOrCheckWorkBoxDocumentContentType(SPWeb web)
        {
            WBColumn[] requiredFields = 
            { 
            };

            WBColumn[] optionalFields = 
            {
                WBColumn.FunctionalArea,
                WBColumn.RecordsType,
                WBColumn.SubjectTags,
                WBColumn.ReferenceID,
                WBColumn.ReferenceDate,
                WBColumn.ScanDate,
                WBColumn.SeriesTag,
                WBColumn.OwningTeam,
                WBColumn.InvolvedTeams,
                WBColumn.VisitingTeams,
                WBColumn.InvolvedIndividuals,
                WBColumn.VisitingIndividuals,
                WBColumn.ProtectiveZone,
                WBColumn.OriginalFilename,
                WBColumn.SourceSystem,
                WBColumn.SourceID,
                WBColumn.RecordID,
                WBColumn.LiveOrArchived
            };

            WBUtils.CreateOrCheckContentType(web, WBFarm.Local.WorkBoxDocumentContentTypeName, "Document", WorkBox.SITE_CONTENT_TYPES_GROUP_NAME, requiredFields, optionalFields);
        }

        private void CreateOrCheckWorkBoxRecordContentType(SPWeb web)
        {
            WBColumn[] requiredFields = 
            { 
                WBColumn.RecordID,
                WBColumn.LiveOrArchived
            };
            
            WBColumn[] optionalFields = 
            {
            };


            SPContentType recordContentType = WBUtils.CreateOrCheckContentType(web, WBFarm.Local.WorkBoxRecordContentTypeName, WBFarm.Local.WorkBoxDocumentContentTypeName, WorkBox.SITE_CONTENT_TYPES_GROUP_NAME, requiredFields, optionalFields);

        }




        // This is based on code ideas from:
        // http://www.sharemuch.com/2010/03/30/how-to-create-custom-sharepoint-2010-user-profile-properties-programatically/
        // http://www.woaychee.com/sharepoint-2010-create-custom-user-profile-properties-programmatically-part-1/
        // http://www.woaychee.com/sharepoint-2010-create-custom-user-profile-properties-programmatically-part-2/
        // 
        private void InitialSetupOfUserProfileProperties(SPSite site)
        {
            WBLogging.Generic.Monitorable("Starting process of setting up the user profile properties");

            SPServiceContext serviceContext = SPServiceContext.GetContext(site);
            UserProfileManager profileManager = new UserProfileManager(serviceContext);

            UserProfileConfigManager profileConfigManager = new UserProfileConfigManager(serviceContext);
            CorePropertyManager corePropertyManager = profileConfigManager.ProfilePropertyManager.GetCoreProperties();

            if (corePropertyManager.GetSectionByName("WorkBoxFrameworkPropertySection") == null)
            {
                CoreProperty wbfSection = corePropertyManager.Create(true);
                wbfSection.Name = "WorkBoxFrameworkPropertySection";
                wbfSection.DisplayName = "Work Box Framework Property Section";
                corePropertyManager.Add(wbfSection);
            }

            ProfilePropertyManager propertyManager = profileConfigManager.ProfilePropertyManager;
            ProfileTypePropertyManager profileTypePropertyManager = propertyManager.GetProfileTypeProperties(ProfileType.User);


            ProfileSubtypeManager profileSubtypeManager = ProfileSubtypeManager.Get(serviceContext);
            ProfileSubtype profileSubtype = profileSubtypeManager.GetProfileSubtype(ProfileSubtypeManager.GetDefaultProfileName(ProfileType.User));

            ProfileSubtypePropertyManager profileSubtypePropertyManager = profileSubtype.Properties;

            // Now to try to create the various user profile properties:
            MaybeCreateNewUserProfileStringProperty(corePropertyManager, profileTypePropertyManager, profileSubtypePropertyManager,
                WorkBox.USER_PROFILE_PROPERTY__WORK_BOX_LAST_VISITED_GUID, "Work Box Last Visited GUID", "A Work Box Framework system property that holds the GUID of the work box that the user last visited.", 100);

            MaybeCreateNewUserProfileStringProperty(corePropertyManager, profileTypePropertyManager, profileSubtypePropertyManager,
                WorkBox.USER_PROFILE_PROPERTY__MY_RECENTLY_VISITED_WORK_BOXES, "My Recently Visited Work Boxes", "A Work Box Framework system property that holds the information about the work boxes that a user has recently visited.", 3600);

            MaybeCreateNewUserProfileStringProperty(corePropertyManager, profileTypePropertyManager, profileSubtypePropertyManager,
                WorkBox.USER_PROFILE_PROPERTY__MY_FAVOURITE_WORK_BOXES, "My Favourite Work Boxes", "A Work Box Framework system property that holds the information about a user's favourite work boxes.", 3600);

            MaybeCreateNewUserProfileStringProperty(corePropertyManager, profileTypePropertyManager, profileSubtypePropertyManager,
                WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD, "My Work Box Clipboard", "A Work Box Framework system property that holds the information about what a user has on their work box clipboard.", 3600);

            MaybeCreateNewUserProfileStringProperty(corePropertyManager, profileTypePropertyManager, profileSubtypePropertyManager,
                WorkBox.USER_PROFILE_PROPERTY__MY_UNPROTECTED_WORK_BOX_URL, "My Unprotected Work Box URL", "A Work Box Framework system property that holds the URL for the user's unprotected work box.", 100);

            WBLogging.Generic.Monitorable("Finished process of setting up the user profile properties");
        }

        private void MaybeCreateNewUserProfileStringProperty(
            CorePropertyManager corePropertyManager,
            ProfileTypePropertyManager profileTypePropertyManager, 
            ProfileSubtypePropertyManager profileSubtypePropertyManager,
            String propertyName, String displayName, String description,
            int propertyLength)
        {
            try
            {
                /*  This was just necessary while debugging the unprotected zone.
                if (corePropertyManager.GetPropertyByName(propertyName) != null)
                {
                    corePropertyManager.RemovePropertyByName(propertyName);
                    WBLogging.Generic.Monitorable("Removed coreProperty: " + propertyName);
                }
            
                if (profileTypePropertyManager.GetPropertyByName(propertyName) != null)
                {
                    profileTypePropertyManager.RemovePropertyByName(propertyName);
                    WBLogging.Generic.Monitorable("Removed profileTypeProperty: " + propertyName);
                }

                if (profileSubtypePropertyManager.GetPropertyByName(propertyName) != null)
                {
                    profileSubtypePropertyManager.RemovePropertyByName(propertyName);
                    WBLogging.Generic.Monitorable("Removed profileSubtypeProperty: " + propertyName);
                }
                */

                if (corePropertyManager.GetPropertyByName(propertyName) == null)
                {
                    WBLogging.Generic.Monitorable("Trying to create user profile property: " + propertyName);

                    // First add the property as a 'core property'
                    CoreProperty propertyInstance = corePropertyManager.Create(false);
                    propertyInstance.Name = propertyName;
                    propertyInstance.Type = "string (Single Value)";
                    propertyInstance.Length = propertyLength;
                    propertyInstance.DisplayName = displayName;
                    propertyInstance.Description = description;
                    propertyInstance.IsAlias = false;
                    propertyInstance.IsSearchable = false;
                    propertyInstance.Commit();

                    //corePropertyManager.Add(propertyInstance);

                    WBLogging.Generic.Monitorable("Added as a core property: " + propertyName);


                    // Next add the property as a profileTypeProperty:
                    ProfileTypeProperty profileTypeProperty = profileTypePropertyManager.Create(propertyInstance);
                    profileTypeProperty.IsVisibleOnViewer = true;
                    profileTypeProperty.IsVisibleOnEditor = true;
                    profileTypeProperty.Commit();

                    //profileTypePropertyManager.Add(profileTypeProperty);


                    WBLogging.Generic.Monitorable("Added as a profileTypePropertyManager: " + propertyName);


                    // Finally add the property as a profileSubtypePropery: 
                    ProfileSubtypeProperty profileSubtypeProperty = profileSubtypePropertyManager.Create(profileTypeProperty);
                    profileSubtypeProperty.PrivacyPolicy = PrivacyPolicy.OptIn;
                    profileSubtypeProperty.DefaultPrivacy = Privacy.Private;
                    profileSubtypeProperty.UserOverridePrivacy = false;
                    profileSubtypeProperty.IsUserEditable = true;
                    profileSubtypeProperty.Commit();

                    //profileSubtypePropertyManager.Add(profileSubtypeProperty);

                    WBLogging.Generic.Monitorable("Created user profile property: " + propertyName);
                }
                else
                {
                    WBLogging.Generic.Monitorable("User profile property already exists: " + propertyName);
                }

            }
            catch (DuplicateEntryException exception)
            {
                WBLogging.Generic.Unexpected("An error occurred while trying to create the user profile property with name: " + propertyName);
                WBLogging.Generic.Unexpected(exception);
            }
        }


        internal void RegisterTimerJobs(SPSite site)
        {
            SPWebApplication webApplication = site.WebApplication;

                        // make sure the job isn't already registered
            foreach (SPJobDefinition job in webApplication.JobDefinitions) {
                if (job.Name == WBTimerTasksJob.DAILY_TIMER_TASKS__TIMER_JOB_NAME)
                    job.Delete();

                if (job.Name == WBTimerTasksJob.FREQUENT_TIMER_TASKS__TIMER_JOB_NAME)
                    job.Delete();

                if (job.Name == WBMigrationTimerJob.MIGRATION_TIMER_JOB__TIMER_JOB_NAME)
                    job.Delete();
            }

            SPServer server = null;
            WBFarm farm = WBFarm.Local;

            if (farm.TimerJobsServerName != "")
            {
                server = farm.SPFarm.Servers[farm.TimerJobsServerName];

                if (server != null)
                {

                    /* */
                    /* First adding the Daily Timer Job  */
                    /* */

                    WBLogging.Generic.Monitorable("WBFarm.RegisterTimerJobs(): Adding a timer job to server : " + server.Name + " with name: " + WBTimerTasksJob.DAILY_TIMER_TASKS__TIMER_JOB_NAME);

                    WBTimerTasksJob timerJob = new WBTimerTasksJob(
                        WBTimerTasksJob.DAILY_TIMER_TASKS__TIMER_JOB_NAME,
                        WBTimerTasksJob.DAILY_TIMER_TASKS__LIST_NAME,
                        WBTimerTasksJob.DAILY_TIMER_TASKS__ORDERED_VIEW_NAME,
                        webApplication,
                        server,
                        SPJobLockType.Job);

                    SPDailySchedule schedule = new SPDailySchedule();

                    schedule.BeginHour = 5;
                    schedule.BeginMinute = 0;
                    schedule.BeginSecond = 0;

                    schedule.EndHour = 5;
                    schedule.EndMinute = 10;
                    schedule.EndSecond = 0;

                    timerJob.Schedule = schedule;

                    timerJob.Update();

                    /* */
                    /* Now adding the Frequent Timer Job  */
                    /* */

                    WBLogging.Generic.Monitorable("WBFarm.RegisterTimerJobs(): Adding a timer job to server : " + server.Name + " with name: " + WBTimerTasksJob.FREQUENT_TIMER_TASKS__TIMER_JOB_NAME);

                    WBTimerTasksJob frequentTimerJob = new WBTimerTasksJob(
                        WBTimerTasksJob.FREQUENT_TIMER_TASKS__TIMER_JOB_NAME,
                        WBTimerTasksJob.FREQUENT_TIMER_TASKS__LIST_NAME,
                        WBTimerTasksJob.FREQUENT_TIMER_TASKS__ORDERED_VIEW_NAME,
                        webApplication,
                        server,
                        SPJobLockType.Job);

                    SPMinuteSchedule frequentSchedule = new SPMinuteSchedule();

                    frequentSchedule.BeginSecond = 0;
                    frequentSchedule.EndSecond = 59;
                    frequentSchedule.Interval = 10;

                    frequentTimerJob.Schedule = frequentSchedule;

                    frequentTimerJob.Update();

                }
                else
                {
                    WBLogging.Generic.Unexpected("WBFarm.RegisterTimerJobs(): Couldn't find the server with the name: " + farm.TimerJobsServerName);
                }
            }
            else
            {
                WBLogging.Generic.Unexpected("WBFarm.RegisterTimerJobs(): The WBF farm wide setting of which server to use for the timer job has not been set.");
            }

        }



        #endregion


        public static List<String> GetFarmInstances()
        {
            List<String> list = new List<String>();
            list.Add(FARM_INSTANCE__PROTECTED_INTERNAL_FARM);
            list.Add(FARM_INSTANCE__PUBLIC_EXTERNAL_FARM);
            list.Add(FARM_INSTANCE__DEVELOPMENT_FARM);

            return list;
        }

        public static List<String> GetMigrationTypes()
        {
            List<String> list = new List<String>();
            list.Add(MIGRATION_TYPE__NONE);
            list.Add(MIGRATION_TYPE__MIGRATE_IZZI_PAGES);
            list.Add(MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_LIBRARY);
            list.Add(MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_WORK_BOXES);

            return list;
        }




    }
}
