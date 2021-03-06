﻿#region Copyright and License

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint.Taxonomy;
using System.Collections.Specialized;
using Microsoft.Office.RecordsManagement.RecordsRepository;

namespace WorkBoxFramework
{
    internal class WBTimerTask
    {
        internal const string COLUMN_NAME__COMMAND = "Command";
        internal const string COLUMN_NAME__TARGET_URL = "Target URL";
        internal const string COLUMN_NAME__ARGUMENT_1 = "Argument 1";
        internal const string COLUMN_NAME__EXECUTION_ORDER = "Execution Order";

        internal const string COMMAND__COMPOSITE_TEAMS = "Composite Teams (Management Site | [List Name])";
        internal const string COMMAND__SYNCHRONISE_ALL_TEAMS = "Synchronise All Teams (Site Collection)";
        internal const string COMMAND__FOLDER_GROUPS_MAPPING = "Folder Groups Mapping (Work Box Collection | [List Name])";
        internal const string COMMAND__WORK_BOX_STATUS_UPDATES = "Work Box Status Updates (Work Box Collection | [Current Status])";
        internal const string COMMAND__CACHE_WORK_BOX_DETAILS = "Cache Work Box Details (Work Box Collection | [Current Status])";
        internal const string COMMAND__UPDATE_RECENTLY_VISITED_WORK_BOXES = "Update Recently Visited Work Boxes ([] | [All])";
        internal const string COMMAND__UPDATE_WORK_BOX_DOCUMENTS_METADATA = "Update Work Box Documents Metadata ([] | [All])";
        internal const string COMMAND__PRECREATE_WORK_BOXES = "Precreate Work Boxes (Work Box Collection)";
        internal const string COMMAND__SEND_PUBLIC_RECORDS_REVIEW_EMAILS = "Send Public Records Review Emails ([] | [DayOfWeek])";
        internal const string COMMAND__AUTO_ARCHIVE_OLD_PUBLIC_RECORDS = "Auto Archive Old Public Records ([] | [DayOfWeek])";
        internal const string COMMAND__SEND_NEW_PUBLIC_RECORDS_ALERTS = "Send New Public Records Alerts";


        private const string DEFAULT_LIST_NAME__COMPOSITE_TEAMS = "Composite Teams";
        private const string DEFAULT_LIST_NAME__FOLDER_GROUPS_MAPPING = "Folder Groups Mapping";


        private const string COLUMN_NAME__TEAM = "Team";
        private const string COLUMN_NAME__FOLDER_GROUPS = "Folder Groups";
        private const string COLUMN_NAME__COMPOSE_FROM = "Compose From";

        private const string VIEW_NAME__IN_ORDER_TO_BUILD = "In Order To Build";

        #region Static Methods

        internal static void Execute(SPListItem task)
        {
            string command = task.WBxGetColumnAsString(COLUMN_NAME__COMMAND);
            string targetUrl = task.WBxGetColumnAsString(COLUMN_NAME__TARGET_URL);
            string argument1 = task.WBxGetColumnAsString(COLUMN_NAME__ARGUMENT_1);

            switch (command)
            {
                case COMMAND__COMPOSITE_TEAMS:
                    {
                        doCompositeTeams(targetUrl, argument1);
                        break;
                    }

                case COMMAND__SYNCHRONISE_ALL_TEAMS:
                    {
                        doSynchroniseAllTeams(targetUrl);
                        break;
                    }

                case COMMAND__FOLDER_GROUPS_MAPPING:
                    {
                        doFolderGroupsMapping(targetUrl, argument1); 
                        break;
                    }

                case COMMAND__WORK_BOX_STATUS_UPDATES:
                    {
                        doWorkBoxStatusUpdates(targetUrl, argument1);
                        break;
                    }

                case COMMAND__CACHE_WORK_BOX_DETAILS:
                    {
                        doCacheWorkBoxDetails(targetUrl, argument1);
                        break;
                    }

                case COMMAND__UPDATE_RECENTLY_VISITED_WORK_BOXES:
                    {
                        doUpdateRecentlyVisitedWorkBoxes(targetUrl, argument1);
                        break;
                    }

                case COMMAND__UPDATE_WORK_BOX_DOCUMENTS_METADATA:
                    {
                        doUpdateWorkBoxDocumentsMetadata(targetUrl, argument1);
                        break;
                    }                    

                case COMMAND__PRECREATE_WORK_BOXES:
                    {
                        doPrecreateWorkBoxes(targetUrl, argument1);
                        break;

                    }
                case COMMAND__SEND_PUBLIC_RECORDS_REVIEW_EMAILS:
                    {
                        doSendPublicRecordsReviewEmails(targetUrl, argument1);
                        break;
                    }

                case COMMAND__AUTO_ARCHIVE_OLD_PUBLIC_RECORDS:
                    {
                        doAutoArchiveOldPublicRecords(targetUrl, argument1);
                        break;
                    }
                case COMMAND__SEND_NEW_PUBLIC_RECORDS_ALERTS:
                    {
                        doSendNewPublicRecordsAlerts(targetUrl, argument1);
                        break;
                    }

                default:
                    {
                        WBUtils.logMessage("Didn't know how to process the command with name: " + command);
                        break;
                    }
            }
        }

        #endregion


        #region Command Implementations as Private Static Methods


        private static void doCompositeTeams(String siteURL, String listName)
        {
            if (siteURL == null || siteURL == "")
                throw new Exception("You must enter a legitiate URL for the site when using doCompositeTeams");

            if (listName == "") listName = DEFAULT_LIST_NAME__COMPOSITE_TEAMS;

            using (SPSite site = new SPSite(siteURL))
            using (SPWeb web = site.OpenWeb())
            {
                WBUtils.logMessage("About to create teams taxonomy");
                WBTaxonomy teams = WBTaxonomy.GetTeams(site);
                WBUtils.logMessage("Created teams taxonomy");

                SPList list = web.Lists.TryGetList(listName);
                SPView view = list.Views[VIEW_NAME__IN_ORDER_TO_BUILD];
                SPListItemCollection compositeTeamsDefinitions = list.GetItems(view);

                foreach (SPListItem teamDefinition in compositeTeamsDefinitions)
                {
                    WBTeam compositeTeam = teamDefinition.WBxGetSingleTermColumn<WBTeam>(teams, COLUMN_NAME__TEAM);

                    WBLogging.TimerTasks.Unexpected("Creating composite team : " + compositeTeam.Name);

                    WBTermCollection<WBTeam> composeFromTeams = teamDefinition.WBxGetMultiTermColumn<WBTeam>(teams, COLUMN_NAME__COMPOSE_FROM);

                    SPGroup compositeTeamMembers = compositeTeam.MembersGroup(site);
                    compositeTeamMembers.WBxRemoveAllUsers();

                    foreach (WBTeam team in composeFromTeams)
                    {
                        WBLogging.TimerTasks.Unexpected("Copying into composite team : " + team.Name);
                        SPGroup teamMembers = team.MembersGroup(site);

                        teamMembers.WBxCopyUsersInto(compositeTeamMembers);
                    }
                }

            }

        }

        private static void doSynchroniseAllTeams(String siteCollectionUrl)
        {
            if (siteCollectionUrl == null || siteCollectionUrl == "")
                throw new Exception("You must enter a legitiate URL for the site collection when using doSynchroniseAllTeams");

            using (SPSite site = new SPSite(siteCollectionUrl))
            {
                WBTeam.SyncAllTeams(site);
            }
        }

        private static void doFolderGroupsMapping(String workBoxCollectionURL, String listName)
        {
            if (workBoxCollectionURL == null || workBoxCollectionURL == "")
                throw new Exception("You must enter a legitiate URL for the work box collection using folder mapping details");

            if (listName == "") listName = DEFAULT_LIST_NAME__FOLDER_GROUPS_MAPPING;

            WBLogging.TimerTasks.Verbose("doFolderGroupsMapping(): URL: " + workBoxCollectionURL + " List name: " + listName);

            using (WBCollection collection = new WBCollection(workBoxCollectionURL))
            {
                if (collection.UseFolderAccessGroupsPattern)
                {
                    WBLogging.TimerTasks.Verbose("This collection is using the folder access group pattern - so applying");

                    // So first we're going to remove all users from the folder access groups:
                    string[] folderNames = collection.FolderAccessGroupsFolderNames.Split(';');

                    SPGroup group;
                    string groupName;

                    foreach (string folderName in folderNames)
                    {
                        WBLogging.TimerTasks.Verbose("Clearing out the folder access group called: " + folderName);


                        groupName = collection.FolderAccessGroupsPrefix + " - " + folderName;
                        group = collection.Web.SiteGroups[groupName];

                        if (group == null)
                        {
                            WBLogging.TimerTasks.Unexpected("Could not find the folder access group: " + groupName);
                        }
                        else
                        {
                            group.WBxRemoveAllUsers();
                            WBLogging.TimerTasks.Verbose("Removed all users from group: " + groupName);
                        }
                    }

                    string allFoldersGroupName = collection.FolderAccessGroupsPrefix + " - All Folders";
                    group = collection.Web.SiteGroups[allFoldersGroupName];
                    if (group == null)
                    {
                        WBLogging.TimerTasks.Unexpected("Could not find the folder access group: " + allFoldersGroupName);                       
                    }
                    else
                    {
                        group.WBxRemoveAllUsers();
                        WBLogging.TimerTasks.Verbose("Removed all users from group: " + allFoldersGroupName);
                    }


                    // Now for each team listed we're going to add the team's members to the 
                    // mapped folder access groups:

                    WBTaxonomy teams = WBTaxonomy.GetTeams(collection.Site);

                    SPList list = collection.Web.Lists.TryGetList(listName);

                    if (list == null) throw new Exception("Failed to find list with the name: " + listName);

                    foreach (SPListItem item in list.Items)
                    {
                        WBTeam team = item.WBxGetSingleTermColumn<WBTeam>(teams, COLUMN_NAME__TEAM);
                        WBLogging.TimerTasks.Verbose("Doing mapping for team: " + team.Name);

//                        team.SyncSPGroup(collection.Site);

                        SPGroup teamGroup = team.MembersGroup(collection.Site);

                        WBLogging.TimerTasks.Verbose("Team's members group name: " + teamGroup.Name);

                        SPFieldUserValueCollection userValueCollection = new SPFieldUserValueCollection(collection.Web, item.WBxGetColumnAsString(COLUMN_NAME__FOLDER_GROUPS));
                        foreach (SPFieldUserValue userValue in userValueCollection)
                        {
                            if (userValue.User != null)
                            {
                                WBUtils.shouldThrowError("Should only ever be picking groups never individual users");
                            }
                            else
                            {
                                group = collection.Web.SiteGroups.GetByID(userValue.LookupId);

                                if (group == null)
                                {
                                    WBLogging.TimerTasks.Unexpected("Could not find a group into which the team was being mapped: " + userValue);
                                }
                                else
                                {
                                    WBLogging.TimerTasks.Verbose("Copying team members into folder access group: " + group.Name);
                                    teamGroup.WBxCopyUsersInto(group);
                                }

                            }
                        }
                    }
                }
            }
        }


        private static void doWorkBoxStatusUpdates(String workBoxCollectionURL, String currentStatus)
        {
            WBLogging.TimerTasks.Monitorable("Running doWorkBoxStatusUpdates command with workBoxCollectionURL | currentStatus : " + workBoxCollectionURL + " | " + currentStatus);

            if (workBoxCollectionURL == null || workBoxCollectionURL == "")
                throw new Exception("You must enter a legitiate URL for the work box collection on which the work box status updates will be done.");

            if (currentStatus == null) 
                currentStatus = "";

            using (WBCollection collection = new WBCollection(workBoxCollectionURL)) 
            {
                SPListItemCollection workBoxes = null;

                if (currentStatus == "")
                {
                    WBLogging.TimerTasks.Monitorable("You have asked to update all of the work boxes in the work box collection: " + workBoxCollectionURL + ". This might take a long time to process");
                    workBoxes = collection.List.Items;
                }
                else
                {
                    workBoxes = collection.QueryFilteredByStatus(currentStatus);
                }

                if (workBoxes == null)
                {
                    WBLogging.TimerTasks.Monitorable("The SPListItemCollection was null");
                }
                else
                {
                    int total = workBoxes.Count;
                    int completed = 0;
                    int failed = 0;
                    WBLogging.TimerTasks.Monitorable("About to update the status of " + total + " work boxes");
                    foreach (SPListItem workBoxItem in workBoxes)
                    {
                        using (WorkBox workBox = new WorkBox(collection, workBoxItem))
                        {
                            try
                            {
                                workBox.UpdateStatus();
                            }
                            catch (Exception e)
                            {
                                WBLogging.TimerTasks.Unexpected("WBTimerTask.doWorkBoxStatusUpdates(): Error when trying to update a work box: " + e.Message);
                                failed++;
                            }
                        }

                        completed++;
                        if (completed % 10 == 0 || completed == total)
                        {
                            WBLogging.TimerTasks.Monitorable("Completed " + completed + " of " + total + " work boxes. " + failed + " updates failed.");
                        }
                    }
                }
            }

        }

        private static void doCacheWorkBoxDetails(String workBoxCollectionURL, String currentStatus)
        {
            WBLogging.TimerTasks.Monitorable("Running doCacheWorkBoxDetails command with workBoxCollectionURL | currentStatus : " + workBoxCollectionURL + " | " + currentStatus);

            if (workBoxCollectionURL == null || workBoxCollectionURL == "")
                throw new Exception("You must enter a legitiate URL for the work box collection from which the cached work box details will be updated.");

            if (currentStatus == null) 
                currentStatus = "";

            String cachedListUrl = WBFarm.Local.OpenWorkBoxesCachedDetailsListUrl;

            if (String.IsNullOrEmpty(cachedListUrl))
            {
                WBLogging.TimerTasks.Monitorable("The URL for the list of cached work box details does not seem to be set - hence can't do anything for this command.");
                return;
            }

            using (WBCollection collection = new WBCollection(workBoxCollectionURL)) 
            {
                SPListItemCollection workBoxes = null;

                if (currentStatus == "")
                {
                    WBLogging.TimerTasks.Monitorable("You have asked to update all of the cached details of work boxes in the work box collection: " + workBoxCollectionURL + ". This might take a long time to process");
                    workBoxes = collection.List.Items;
                }
                else
                {
                    workBoxes = collection.QueryFilteredByStatus(currentStatus);
                }

                if (workBoxes == null)
                {
                    WBLogging.TimerTasks.Monitorable("The SPListItemCollection was null");
                }
                else
                {
                    int total = workBoxes.Count;
                    int completed = 0;
                    int failed = 0;
                    WBLogging.TimerTasks.Monitorable("About to update the cached details of " + total + " work boxes");

                    using (SPSite cacheSite = new SPSite(cachedListUrl))
                    using (SPWeb cacheWeb = cacheSite.OpenWeb())
                    {
                        cacheWeb.AllowUnsafeUpdates = true;
                        SPList cachedDetailsList = cacheWeb.GetList(cachedListUrl);

                        foreach (SPListItem workBoxItem in workBoxes)
                        {
                            using (WorkBox workBox = new WorkBox(collection, workBoxItem))
                            {
                                try
                                {
                                    int cachedListItemID = workBox.UpdateCachedDetails(cachedDetailsList);
                                    workBox.CachedListItemID = cachedListItemID;
                                    workBox.JustUpdate();
                                }
                                catch (Exception e)
                                {
                                    WBLogging.TimerTasks.Unexpected("WBTimerTask.doCacheWorkBoxDetails(): Error when trying to update the cached details of a work box: " + e.Message);
                                    failed++;
                                }
                            }

                            completed++;
                            if (completed % 10 == 0 || completed == total)
                            {
                                WBLogging.TimerTasks.Monitorable("Completed " + completed + " of " + total + " work boxes. " + failed + " updates failed.");
                            }
                        }

                        cacheWeb.AllowUnsafeUpdates = false;
                    }

                }
            }

        }


        private static void doUpdateRecentlyVisitedWorkBoxes(String targetURL, String flag)
        {
            WBLogging.TimerTasks.Monitorable("Running doUpdateRecentlyVisitedWorkBoxes command");
            WBFarm farm = WBFarm.Local;
            String cachedDetailsListUrl = farm.OpenWorkBoxesCachedDetailsListUrl;

            if (String.IsNullOrEmpty(cachedDetailsListUrl)) return;

            long ticksAtLastUpdate = 0;
            if (flag != "All")
            {
                ticksAtLastUpdate = farm.TicksWhenLastUpdatedRecentlyVisited;
            }

            farm.TicksWhenLastUpdatedRecentlyVisited = DateTime.Now.Ticks;
            farm.Update();

            using (SPSite cacheSite = new SPSite(cachedDetailsListUrl))
            using (SPWeb cacheWeb = cacheSite.OpenWeb())
            {
                SPList cacheList = cacheWeb.GetList(cachedDetailsListUrl);

                SPServiceContext serviceContext = SPServiceContext.GetContext(cacheSite);
                UserProfileManager profileManager = new UserProfileManager(serviceContext);

                foreach (UserProfile profile in profileManager)
                {
                    try 
                    {
                        WBUser.CheckLastModifiedDatesAndTitlesOfRecentWorkBoxes(cacheSite, cacheList, profile, ticksAtLastUpdate);

                        WBUser.CheckTitlesOfFavouriteWorkBoxes(cacheSite, cacheList, profile);
                    }
                    catch (Exception exception)
                    {
                        WBLogging.TimerTasks.Unexpected("Exception happened when looking at user profile: " + profile.DisplayName);
                        WBLogging.TimerTasks.Unexpected(exception);
                    }

                }

                WBLogging.TimerTasks.Monitorable("Finished doUpdateRecentlyVisitedWorkBoxes command");                
            }


        }


        private static void doUpdateWorkBoxDocumentsMetadata(String targetURL, String flag)
        {
            WBLogging.TimerTasks.Monitorable("Running doUpdateWorkBoxDocumentsMetadata command");
            WBFarm farm = WBFarm.Local;

            long ticksAtLastUpdate = 0;
            if (flag != "All")
            {
                ticksAtLastUpdate = farm.TicksWhenLastUpdatedWorkBoxDocumentsMetadata;
            }

            farm.TicksWhenLastUpdatedWorkBoxDocumentsMetadata = DateTime.Now.Ticks;
            farm.Update();

            List<String> workBoxURLsToCheck = new List<String>();

            using (SPSite teamsSite = new SPSite(farm.TeamSitesSiteCollectionUrl))
            {
                SPServiceContext serviceContext = SPServiceContext.GetContext(teamsSite);
                UserProfileManager profileManager = new UserProfileManager(serviceContext);


                foreach (UserProfile profile in profileManager)
                {
                    try
                    {
                        List<String> moreWorkBoxURLsToCheck = WBUser.GetWorkBoxURLsVisitedSinceTickTime(profile, ticksAtLastUpdate);

                        foreach (String workBoxURL in moreWorkBoxURLsToCheck)
                        {
                            if (!workBoxURLsToCheck.Contains(workBoxURL))
                            {
                                workBoxURLsToCheck.Add(workBoxURL);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        WBLogging.TimerTasks.Unexpected("Exception happened when looking at user profile: " + profile.DisplayName);
                        WBLogging.TimerTasks.Unexpected(exception);
                    }
                }
            }

            // Sort the work boxes so that work boxes from the same site collection should be nearby to each other:
            workBoxURLsToCheck.Sort();

            WBLogging.TimerTasks.Verbose("In doUpdateWorkBoxDocumentsMetadata(): Work Boxes to check: " + String.Join("; ", workBoxURLsToCheck.ToArray()));

            SPSite site = null;
            WBTaxonomy teams = null;
            SPWeb web = null;

            foreach (String workBoxURL in workBoxURLsToCheck)
            {
                WBLogging.TimerTasks.Verbose("In doUpdateWorkBoxDocumentsMetadata(): About to look at: " + workBoxURL);

                try
                {
                    if (site == null)
                    {
                        site = new SPSite(workBoxURL);
                        teams = WBTaxonomy.GetTeams(site);
                    }

                    WBLogging.TimerTasks.Verbose("In doUpdateWorkBoxDocumentsMetadata(): Opened SPSite for: " + workBoxURL);

                    web = null;
                    //String workBoxURLWithinSite = WBUtils.GetURLWithoutHostHeader(workBoxURL);

                    //WBLogging.TimerTasks.Verbose("In doUpdateWorkBoxDocumentsMetadata(): workBoxURLWithinSite: " + workBoxURLWithinSite);
                    web = site.OpenWeb(workBoxURL);
                    if (!web.Exists)
                    {
                        web = null;
                        WBLogging.TimerTasks.Verbose("Couldn't find " + workBoxURL + " in " + site.Url);
                    }

                    if (web == null)
                    {
                        WBLogging.TimerTasks.Verbose("In doUpdateWorkBoxDocumentsMetadata(): Couldn't find SPWeb in current SPSite: " + workBoxURL);

                        // OK so if we're here then this suggests that the current workBoxURL is for an SPWeb from a different SPSite so:
                        if (site != null) { site.Dispose(); site = null; }

                        site = new SPSite(workBoxURL);
                        teams = WBTaxonomy.GetTeams(site);
                        web = site.OpenWeb();  // this should work as the SPWebs URL was used to create the SPSite object
                    }

                    if (site == null || web == null)
                    {
                        WBLogging.TimerTasks.Unexpected("Should not have a null site (" + site + ") or web (" + web + ") object at this point!");
                        throw new Exception("Should not have a null site (" + site + ") or web (" + web + ") object at this point!");
                    }

                    if (web.LastItemModifiedDate.Ticks > ticksAtLastUpdate)
                    {
                        // OK so this SPWeb has been modified since we last looked to update the documents metadata
                        WorkBox workBox = new WorkBox(site, web);
                        workBox.UpdateDocumentsMetadata(teams);
                    }

                    web.Dispose();
                }
                catch (Exception e)
                {
                    WBLogging.TimerTasks.Unexpected("In doUpdateWorkBoxDocumentsMetadata(): Exception occurred for: " + workBoxURL, e);
                }
                finally
                {
                    if (web != null) web.Dispose();
                }

            }

            if (site != null) site.Dispose();

            WBLogging.TimerTasks.Monitorable("Finished doUpdateWorkBoxDocumentsMetadata command");
        }


        private static void doPrecreateWorkBoxes(String workBoxCollectionURL, String flag)
        {
            WBLogging.TimerTasks.Monitorable("Running doPrecreateWorkBoxes command");

            using (WBCollection collection = new WBCollection(workBoxCollectionURL))
            {
                try 
                {
                    SPList templates = collection.TemplatesList;

                    foreach (SPListItem item in templates.Items)
                    {
                        WBTemplate template = new WBTemplate(collection, item);

                        template.PrecreateWorkBoxes();
                    }

                }
                catch (Exception exception)
                {
                    WBLogging.TimerTasks.Unexpected("Exception happened when doing PrecreateWorkBoxes: " + workBoxCollectionURL, exception);
                }

            }

            WBLogging.TimerTasks.Monitorable("Finished doPrecreateWorkBoxes command");
        }

        private static void doSendPublicRecordsReviewEmails(String workBoxCollectionURL, String dayOfWeek)
        {
            WBLogging.TimerTasks.Monitorable("Running doSendPublicRecordsReviewEmails command");

            if (!String.IsNullOrEmpty(dayOfWeek))
            {
                String today = DateTime.Now.ToString("dddd");
                if (dayOfWeek == today)
                {
                    WBLogging.Debug("As today is " + today + " therefore running doSendPublicRecordsReviewEmails");
                }
                else
                {
                    WBLogging.Debug("As today (" + today + ") is not " + dayOfWeek + " therefore NOT running doSendPublicRecordsReviewEmails");
                    WBLogging.TimerTasks.Monitorable("Finished doSendPublicRecordsReviewEmails command");
                    return;
                }
            }

            try
            {
                using (WBRecordsManager manager = new WBRecordsManager(null))
                {
                    WBTaxonomy teams = manager.TeamsTaxonomy;

                    foreach (Term term in teams.TermSet.Terms)
                    {
                        SendPublicRecordsReviewEmailsToTeams(manager, new WBTeam(teams, term));
                    }
                }

            }
            catch (Exception e)
            {
                WBLogging.TimerTasks.Unexpected("An error occurred during execution of doSendPublicRecordsReviewEmails", e);
            }

            WBLogging.TimerTasks.Monitorable("Finished doSendPublicRecordsReviewEmails command");
        }

        private static void SendPublicRecordsReviewEmailsToTeams(WBRecordsManager manager, WBTeam team)
        {
            // We'll only look for records to review for teams that have their IAO set:
            if (!String.IsNullOrEmpty(team.InformationAssetOwnerLogin))
            {
                SendPublicRecordsReviewEmailsToTeam(manager, team);
            }

            // But we'll still look at all child teams as they might be relevant:
            foreach (Term term in team.Term.Terms)
            {
                SendPublicRecordsReviewEmailsToTeams(manager, new WBTeam(manager.TeamsTaxonomy, term));
            }
            
        }

        private static void SendPublicRecordsReviewEmailsToTeam(WBRecordsManager manager, WBTeam team)
        {
            WBLogging.Debug("In SendPublicRecordsReviewEmailsToTeam for team: " + team.Name);

            WBFarm farm = WBFarm.Local;

            WBQuery query = manager.GetQueryForTeamsPublicRecordsToReview(team);
            WBRecordsLibrary masterLibrary = manager.Libraries.ProtectedMasterLibrary;

            SPListItemCollection items = masterLibrary.List.WBxGetItems(masterLibrary.Site, query);

            if (items.Count > 0)
            {
                WBLogging.Debug("In SendPublicRecordsReviewEmailsToTeam. Found items.Count = " + items.Count);

                StringDictionary headers = new StringDictionary();

                List<String> emailAddresses = new List<String>();

                foreach (SPUser user in team.MembersGroup(masterLibrary.Site).Users)
                {
                    if (!emailAddresses.Contains(user.Email))
                    {
                        emailAddresses.Add(user.Email);
                    }
                }

                headers.Add("to", String.Join(";", emailAddresses.ToArray()));
                headers.Add("content-type", "text/html");

                headers.Add("cc", WBFarm.Local.PublicDocumentEmailAlertsTo);
                headers.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);

                String urlToGoTo = team.TeamSiteUrl + "/_layouts/WorkBoxFramework/OurRecordsToReview.aspx";
                urlToGoTo = urlToGoTo.Replace("//_layouts/", "/_layouts/");

                String subject = "You have " + items.Count + " public document/s to review (" + team.Name + ")";
                String body = @"<p>Dear " + team.Name + @",</p>

<p>The following record/s within the Public Records Library are due to be archived.</p>

<p>As the owning team you will need to mark these records as ‘keep’ for them to remain visible on the council website; if no action is taken the records will be archived and no longer visible to the public.</p>

<p>Your team can review these documents here: <a href=""" + urlToGoTo + @""">" + urlToGoTo + @"</a></p>

";

                for (int weeksTime = 1; weeksTime <= 4; weeksTime++)
                {
                    WBQuery queryToArchiveSoon = manager.GetQueryForTeamsPublicRecordsToArchiveInFutureWeek(team, weeksTime);
                    SPListItemCollection toArchiveSoon = masterLibrary.List.WBxGetItems(masterLibrary.Site, queryToArchiveSoon);

                    if (toArchiveSoon.Count > 0)
                    {
                        if (weeksTime == 1)
                        {
                            body += "<p><b>Documents Being Archived Next Week:</b><br/>\n";
                        }
                        else
                        {
                            body += "<p><b>Documents Being Archived In " + weeksTime + " Weeks:</b><br/>\n";
                        }

                        foreach (SPListItem item in toArchiveSoon)
                        {
                            WBDocument document = new WBDocument(masterLibrary, item);

                            String functionalAreaString = "";
                            if (document.FunctionalArea.Count > 0)
                            {
                                functionalAreaString = document.FunctionalArea[0].FullPath;
                            }

                            body += "<a href=\"" + document.AbsoluteURL + "\">" + document.Name + "</a><br/>\n";
                            body += "Location: (" + document.ProtectiveZone + "): " + functionalAreaString + "/" + document.RecordsType.FullPath + "<br/>\n";
                            body += "<br/>\n";
                        }
                        body += "</p>\n";
                    }
                }

                body += "\n\n<p>Many Thanks,<br/>\nWebteam.</p>";
                headers.Add("subject", subject);

                WBUtils.SendEmail(masterLibrary.Web, headers, body);
            }
            else
            {
                WBLogging.Debug("In SendPublicRecordsReviewEmailsToTeam. Found items.Count = 0 for team " + team.Name);
            }

        }


        private static void doAutoArchiveOldPublicRecords(String workBoxCollectionURL, String dayOfWeek)
        {
            WBLogging.TimerTasks.Monitorable("Running doAutoArchiveOldPublicRecords command");

            if (!String.IsNullOrEmpty(dayOfWeek))
            {
                String today = DateTime.Now.ToString("dddd");
                if (dayOfWeek == today)
                {
                    WBLogging.Debug("As today is " + today + " therefore running doAutoArchiveOldPublicRecords");
                }
                else
                {
                    WBLogging.Debug("As today (" + today + ") is not " + dayOfWeek + " therefore NOT running doAutoArchiveOldPublicRecords");
                    WBLogging.TimerTasks.Monitorable("Finished doSendPublicRecordsReviewEmails command");
                    return;
                }
            }

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager(null))
                    {
                        WBTaxonomy teams = elevatedManager.TeamsTaxonomy;

                        foreach (Term term in teams.TermSet.Terms)
                        {
                            ArchiveOldPublicDocumentsForTeams(elevatedManager, new WBTeam(teams, term));
                        }
                    }
                });

            }
            catch (Exception e)
            {
                WBLogging.TimerTasks.Unexpected("An error occurred during execution of doAutoArchiveOldPublicRecords", e);
            }

            WBLogging.TimerTasks.Monitorable("Finished doAutoArchiveOldPublicRecords command");
        }

        private static void ArchiveOldPublicDocumentsForTeams(WBRecordsManager elevatedManager, WBTeam team)
        {
            // We'll only look for records to archive for teams that have their IAO set:
            if (!String.IsNullOrEmpty(team.InformationAssetOwnerLogin))
            {
                ArchiveOldPublicDocumentsForTeam(elevatedManager, team);
            }

            // But we'll still look at all child teams as they might be relevant:
            foreach (Term term in team.Term.Terms)
            {
                ArchiveOldPublicDocumentsForTeams(elevatedManager, new WBTeam(elevatedManager.TeamsTaxonomy, term));
            }
        }

        private static void ArchiveOldPublicDocumentsForTeam(WBRecordsManager elevatedManager, WBTeam team)
        {
            WBLogging.Debug("In ArchiveOldPublicDocumentsForTeam for team " + team.Name);

            WBFarm farm = WBFarm.Local;

            WBQuery query = elevatedManager.GetQueryForTeamsPublicRecordsToArchive(team);

            WBRecordsLibrary masterLibrary = elevatedManager.Libraries.ProtectedMasterLibrary;
            SPListItemCollection items = masterLibrary.List.WBxGetItems(masterLibrary.Site, query);

            if (items.Count > 0)
            {
                WBLogging.Debug("In ArchiveOldPublicDocumentsForTeam. items.Count = " + items.Count);


                List<SPListItem> success = new List<SPListItem>();
                List<SPListItem> failure = new List<SPListItem>();

                Dictionary<SPListItem, String> failureMessages = new Dictionary<SPListItem, string>();

                foreach (SPListItem item in items)
                {
                    try
                    {
                        WBRecord record = new WBRecord(elevatedManager.Libraries, item);
                        record.LiveOrArchived = WBColumn.LIVE_OR_ARCHIVED__ARCHIVED;
                        record.Update(null, "Auto Archived");

                        success.Add(item);
                    }
                    catch (Exception e)
                    {
                        failure.Add(item);
                        failureMessages.Add(item, e.WBxFlatten());
                    }
                }

                if (success.Count > 0)
                {
                    StringDictionary headers = new StringDictionary();
                    List<String> emailAddresses = new List<String>();

                    foreach (SPUser user in team.MembersGroup(masterLibrary.Site).Users)
                    {
                        if (!emailAddresses.Contains(user.Email))
                        {
                            emailAddresses.Add(user.Email);
                        }
                    }

                    // Send an email to the team about the documents that were archived:
                    headers.Add("to", String.Join(";", emailAddresses.ToArray()));
                    headers.Add("content-type", "text/html");

                    headers.Add("cc", WBFarm.Local.PublicDocumentEmailAlertsTo);
                    headers.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);

                    String subject = success.Count + " public document/s have been auto archived (" + team.Name + ")";
                    String body = "<p>Dear " + team.Name + @",</p>

<p>The following record/s within the Public Records Library have now been archived.</p>

<p><b>Recently Archived Documents:</b></p>

";

                    foreach (SPListItem item in success)
                    {
                        WBDocument document = new WBDocument(masterLibrary, item);

                        String functionalAreaString = "";
                        if (document.FunctionalArea.Count > 0)
                        {
                            functionalAreaString = document.FunctionalArea[0].FullPath;
                        }

                        body += "<p><a href=\"" + document.AbsoluteURL + "\">" + document.Name + "</a><br/>\n";
                        body += "Old Location: (" + document.ProtectiveZone + "): " + functionalAreaString + "/" + document.RecordsType.FullPath + "<br/>\n";
                        body += "</p>\n";

                    }

                    body += "\n\n<p>Many Thanks,<br/>\nWebteam.</p>";
                    headers.Add("subject", subject);

                    WBUtils.SendEmail(masterLibrary.Web, headers, body);
                }

                if (failure.Count > 0)
                {
                    StringDictionary headers = new StringDictionary();
                    // Now send another email about the failures to webteam
                    headers.Add("to", WBFarm.Local.PublicDocumentEmailAlertsTo);
                    headers.Add("content-type", "text/html");
                    headers.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);

                    String subject = failure.Count + " public document/s failed to auto archive";
                    String body = @"<p>Dear Webteam,</p>

<p>The following public document(s) failed to auto archive:</p>

<p><b>Failed Documents:</b></p>

";


                    foreach (SPListItem item in failure)
                    {
                        WBDocument document = new WBDocument(masterLibrary, item);

                        String functionalAreaString = "";
                        if (document.FunctionalArea.Count > 0)
                        {
                            functionalAreaString = document.FunctionalArea[0].FullPath;
                        }

                        body += "<p><a href=\"" + document.AbsoluteURL + "\">" + document.Name + "</a><br/>\n";
                        body += "Location: (" + document.ProtectiveZone + "): " + functionalAreaString + "/" + document.RecordsType.FullPath + "<br/>\n";
                        body += "Error: " + failureMessages[item];
                        body += "</p>\n\n";
                    }

                    body += "\n\n<p>Many Thanks,<br/>\nWebteam.</p>";
                    headers.Add("subject", subject);

                    WBUtils.SendEmail(masterLibrary.Web, headers, body);
                }

            }
            else
            {
                WBLogging.Debug("In ArchiveOldPublicDocumentsForTeam. items.Count = 0 for team " + team.Name);
            }
        }


        private static void doSendNewPublicRecordsAlerts(String workBoxCollectionURL, String flag)
        {
            WBLogging.TimerTasks.Monitorable("Running doSendNewPublicRecordsAlerts command");

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager(null))
                    {
                        WBQuery query = elevatedManager.GetQueryForNewlyPublishedPublicDocsThatNeedEmailAlert();

                        WBRecordsLibrary masterLibrary = elevatedManager.Libraries.ProtectedMasterLibrary;

                        SPListItemCollection needEmailAlert = masterLibrary.List.WBxGetItems(masterLibrary.Site, query);

                        List<SPListItem> itemsForWebteamAsBackupIAO = new List<SPListItem>();
                        Dictionary<SPUser, List<SPListItem>> itemsForIAO = new Dictionary<SPUser, List<SPListItem>>();
                        Dictionary<String, SPUser> IAOs = new Dictionary<string, SPUser>();

                        if (needEmailAlert.Count == 0)
                        {
                            WBLogging.Debug("Found no newly published items with query: \n" + query.JustCAMLQuery(masterLibrary.Site));
                        }

                        foreach (SPListItem item in needEmailAlert)
                        {
                            WBLogging.Debug("Found newly published item that needs alert: " + item.Name);

                            String iaoLogin = item.WBxGetAsString(WBColumn.IAOAtTimeOfPublishing);
                            WBTeam owningTeam = item.WBxGetSingleTermColumn<WBTeam>(elevatedManager.TeamsTaxonomy, WBColumn.OwningTeam);
                            String owningTeamIAOLogin = owningTeam.InformationAssetOwnerLogin;

                            if (!String.IsNullOrEmpty(owningTeamIAOLogin) && owningTeamIAOLogin != iaoLogin)
                            {
                                iaoLogin = owningTeamIAOLogin;
                            }

                            SPUser iaoUser = null;

                            if (!String.IsNullOrEmpty(iaoLogin))
                            {
                                if (!IAOs.ContainsKey(iaoLogin))
                                {
                                    iaoUser = masterLibrary.Web.WBxEnsureUserOrNull(iaoLogin);
                                    if (iaoUser != null) IAOs.Add(iaoLogin, iaoUser);
                                }
                                else
                                {
                                    iaoUser = IAOs[iaoLogin];
                                }
                            }

                            if (iaoUser != null)
                            {
                                List<SPListItem> listOfItemsForIAO = null;
                                if (!itemsForIAO.ContainsKey(iaoUser))
                                {
                                    listOfItemsForIAO = new List<SPListItem>();
                                    itemsForIAO.Add(iaoUser, listOfItemsForIAO);
                                }
                                else
                                {
                                    listOfItemsForIAO = itemsForIAO[iaoUser];
                                }

                                WBLogging.Debug("Adding: " + item.Name +" to list for user: " + iaoUser.Name);

                                listOfItemsForIAO.Add(item);
                            }
                            else
                            {
                                WBLogging.Debug("Adding: " + item.Name + " to default list");

                                itemsForWebteamAsBackupIAO.Add(item);
                            }
                        }

                        // Now actually put together and send out the email to each IAO
                        foreach (SPUser iaoUser in itemsForIAO.Keys)
                        {
                            String body = MakeBodyOfEmailToIAO(masterLibrary, itemsForIAO[iaoUser]);

                            StringDictionary headers = new StringDictionary();

                            headers.Add("to", iaoUser.Email);
                            headers.Add("content-type", "text/html"); 

                            headers.Add("cc", WBFarm.Local.PublicDocumentEmailAlertsTo);
                            headers.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);

                            headers.Add("subject", "New documents published today for which you are IAO");

                            WBUtils.SendEmail(masterLibrary.Web, headers, body);
                        }

                        if (itemsForWebteamAsBackupIAO.Count > 0)
                        {
                            WBLogging.Debug("We've got to send an email for some docs that didn't have an IAO");
                            // Then we'll send an email to webteam with all of the documents that didn't have an assigned IAO
                            String bodyToWebteam = MakeBodyOfEmailToIAO(masterLibrary, itemsForWebteamAsBackupIAO);
                            StringDictionary headersToWebteam = new StringDictionary();

                            headersToWebteam.Add("to", WBFarm.Local.PublicDocumentEmailAlertsTo);
                            headersToWebteam.Add("content-type", "text/html"); 

                            headersToWebteam.Add("bcc", WBFarm.Local.SendErrorReportEmailsTo);

                            headersToWebteam.Add("subject", "New documents published today for which there is no assigned IAO");

                            WBUtils.SendEmail(masterLibrary.Web, headersToWebteam, bodyToWebteam);
                        }
                        else
                        {
                            WBLogging.Debug("Found no newly published docs that didn't have an assigned IAO");
                        }

                        // And finally we'll mark all of the documents as having had an email sent to them:
                        foreach (SPUser iaoUser in itemsForIAO.Keys)
                        {
                            List<SPListItem> items = itemsForIAO[iaoUser];
                            foreach (SPListItem item in items)
                            {
                                Records.BypassLocks(item, delegate(SPListItem bypassedItem)
                                {
                                    bypassedItem.WBxSet(WBColumn.SentNewlyPublishedAlert, DateTime.Now);
                                    bypassedItem.SystemUpdate();
                                });
                            }
                        }

                        foreach (SPListItem item in itemsForWebteamAsBackupIAO)
                        {
                            Records.BypassLocks(item, delegate(SPListItem bypassedItem)
                            {
                                bypassedItem.WBxSet(WBColumn.SentNewlyPublishedAlert, DateTime.Now);
                                bypassedItem.SystemUpdate();
                            });
                        }
                    }
                });
            }
            catch (Exception e)
            {
                WBLogging.TimerTasks.Unexpected("An error occurred during execution of doSendNewPublicRecordsAlerts", e);
            }

            WBLogging.TimerTasks.Monitorable("Finished doSendNewPublicRecordsAlerts command");
        }

        private static String MakeBodyOfEmailToIAO(WBRecordsLibrary masterLibrary, List<SPListItem> items)
        {
            String body = @"<p>Dear Information Asset Owner,</p>

<p>One or more documents have been published to the Public Records Library by a member of your team.</p>

<p>As the responsible Information Asset Owner for this document, please find details of their publication below along with a link.</p>
 
<p><b>Published Documents:</b></p>"; 

            foreach (SPListItem item in items)
            {
                WBDocument document = new WBDocument(masterLibrary, item);
                
                String functionalAreaString = "";
                if (document.FunctionalArea.Count > 0)
                {
                    functionalAreaString = document.FunctionalArea[0].FullPath;
                }

                SPUser publisehdByUser = document[WBColumn.PublishedBy] as SPUser;
                String publishedByString = "<unknown>";
                if (publisehdByUser != null)
                {
                    publishedByString = publisehdByUser.Name;
                }

                List<SPUser> approvedByUsers = document[WBColumn.PublishingApprovedBy] as List<SPUser>;
                String approvedByString = "<unknown>";
                if (approvedByUsers != null)
                {
                    approvedByString = approvedByUsers.WBxToPrettyString();
                }

                body += "<p><a href=\"" + document.AbsoluteURL + "\">" + document.Name + "</a><br/>\n";
                body += "Location: (" + document.ProtectiveZone + "): " + functionalAreaString + "/" + document.RecordsType.FullPath + "<br/>\n";
                body += "Published by: " + publishedByString  + "<br/>\n";
                body += "Approved by: " + approvedByString + "<br/>\n";
                body += "</p>\n";
            }

            return body;
        }

        #endregion

    }
}
