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
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;


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
        internal const string COMMAND__PRECREATE_WORK_BOXES = "Precreate Work Boxes (Work Box Collection)";

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

                case COMMAND__PRECREATE_WORK_BOXES:
                    {
                        doPrecreateWorkBoxes(targetUrl, argument1);
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

        
        
        #endregion

    }
}
