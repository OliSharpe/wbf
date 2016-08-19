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
using Microsoft.SharePoint.Taxonomy;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework
{
    // Currently the implementation uses the Term's hashtable to store values - maybe it should be using more local variables.

    // Still need to implement the constructor (and related behaviour) when created from a Team Site rather than a Term.
    public class WBTeam : WBTerm
    {
        #region Constants
        public const string TEAM_SITE_PROPERTY__TERM_GUID = "wbf__team_site__term_guid";

        public const string TEAM_TERM_PROPERTY__TEAM_SITE_GUID = "wbf__team_term__team_site_guid";
        public const string TEAM_TERM_PROPERTY__TEAM_SITE_URL = "wbf__team_term__team_site_url";
        public const string TEAM_TERM_PROPERTY__MEMBERS_GROUP_NAME = "wbf__team_term__members_group_name";
        public const string TEAM_TERM_PROPERTY__OWNERS_GROUP_NAME = "wbf__team_term__owners_group_name";
        public const string TEAM_TERM_PROPERTY__PUBLISHERS_GROUP_NAME = "wbf__team_term__publishers_group_name";
        public const string TEAM_TERM_PROPERTY__STATUS = "wbf__team_term__status";
        public const string TEAM_TERM_PROPERTY__ERROR_MESSAGE = "wbf__team_term__error_message";
        public const string TEAM_TERM_PROPERTY__RECORDS_TYPES_LIST_URL = "wbf__team_term__records_types_list_url";
        public const string TEAM_TERM_PROPERTY__COMMON_ACTIVITIES_LIST_URL = "wbf__team_term__common_activities_list_url";
        public const string TEAM_TERM_PROPERTY__FUNCTIONAL_ACTIVITIES_LIST_URL = "wbf__team_term__functional_activities_list_url";
        public const string TEAM_TERM_PROPERTY__FUNCTIONAL_AREA = "wbf__team_term__functional_area";
        public const string TEAM_TERM_PROPERTY__ACRONYM = "wbf__team_term__acronym";
        public const string TEAM_TERM_PROPERTY__MANAGER_LOGIN = "wbf__team_term__manager_login";

        public const string TEAM_TERM_STATUS__NEW = "New";
        public const string TEAM_TERM_STATUS__OK = "OK";
        public const string TEAM_TERM_STATUS__ERROR = "Error";
        #endregion

        #region Constructors and Factories

        [Obsolete("getFromTeamSite is deprecated, please use capitalised GetFromTeamSite instead.", true)]
        public static WBTeam getFromTeamSite(SPContext context)
        {
            return GetFromTeamSite(WBTaxonomy.GetTeams(context.Site), context.Web);
        }

        [Obsolete("getFromTeamSite is deprecated, please use capitalised GetFromTeamSite instead.", true)]
        public static WBTeam getFromTeamSite(WBTaxonomy teams, SPContext context)
        {
            return GetFromTeamSite(teams, context.Web);
        }

        [Obsolete("getFromTeamSite is deprecated, please use capitalised GetFromTeamSite instead.", true)]
        public static WBTeam getFromTeamSite(WBTaxonomy teams, SPWeb web) 
        {
            return GetFromTeamSite(teams, web);
        }

        public static WBTeam GetFromTeamSite(SPContext context)
        {
            return GetFromTeamSite(WBTaxonomy.GetTeams(context.Site), context.Web);
        }

        public static WBTeam GetFromTeamSite(WBTaxonomy teams, SPContext context)
        {
            return GetFromTeamSite(teams, context.Web);
        }

        public static WBTeam GetFromTeamSite(WBTaxonomy teams, SPWeb web)
        {
            if (teams == null || web == null) return null;

            String guidString = web.WBxGetProperty(TEAM_SITE_PROPERTY__TERM_GUID);

            if (guidString == "") return null;
            Guid termGuid = new Guid(guidString);

            return teams.GetTeam(termGuid);
        }

        public WBTeam(WBTaxonomy taxonomy, Term teamsTerm) : base(taxonomy, teamsTerm)
        {
            _individualCommit = true;
        }

        public WBTeam(WBTaxonomy taxonomy, Guid teamsGuid)
            : base(taxonomy, teamsGuid)
        {
            _individualCommit = true;
        }

        
        public WBTeam(WBTaxonomy taxonomy, String UIControlValue)
            : base(taxonomy, UIControlValue)
        {
            _individualCommit = true;
        }

        public WBTeam() : base() { } 

        #endregion

        #region Properties

        private WBTeam _parent = null;
        public WBTeam Parent
        {
            get
            {
                if (_parent == null)
                {
                    Term parentTerm = Term.Parent;
                    if (parentTerm != null)
                    {
                        _parent = new WBTeam(Taxonomy, parentTerm);
                    }
                }
                return _parent;
            }
        }


        private String _currentTeamSiteUrl = null;

        private String CurrentTeamSiteUrl
        {
            get
            {
                if (_currentTeamSiteUrl == null) _currentTeamSiteUrl = Term.WBxGetProperty(TEAM_TERM_PROPERTY__TEAM_SITE_URL);
                return _currentTeamSiteUrl;
            }
        }

        public String TeamSiteUrl
        {
            get
            {
                if (_currentTeamSiteUrl == null) _currentTeamSiteUrl = Term.WBxGetProperty(TEAM_TERM_PROPERTY__TEAM_SITE_URL);
                return Term.WBxGetProperty(TEAM_TERM_PROPERTY__TEAM_SITE_URL);
            }
            set
            {
                Term.WBxSetProperty(TEAM_TERM_PROPERTY__TEAM_SITE_URL, value);
            }
        }

        public String TeamSiteGuidString
        {
            get
            {
                return Term.WBxGetProperty(TEAM_TERM_PROPERTY__TEAM_SITE_GUID);
            }
        }

        public String MembersGroupName
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__MEMBERS_GROUP_NAME); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__MEMBERS_GROUP_NAME, value); }
        }

        public String OwnersGroupName
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__OWNERS_GROUP_NAME); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__OWNERS_GROUP_NAME, value); }
        }

        public String PublishersGroupName
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__PUBLISHERS_GROUP_NAME); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__PUBLISHERS_GROUP_NAME, value); }
        }

        public String Status
        {
            get
            {
                return Term.WBxGetProperty(TEAM_TERM_PROPERTY__STATUS);
            }
            set
            {
                Term.WBxSetProperty(TEAM_TERM_PROPERTY__STATUS, value);
            }
        }


        public bool StatusOK
        {
            get
            {
                return TEAM_TERM_STATUS__OK.Equals(_term.WBxGetProperty(TEAM_TERM_PROPERTY__STATUS));
            }
        }

        public bool StatusError { 
            get 
            {
                return TEAM_TERM_STATUS__ERROR.Equals(_term.WBxGetProperty(TEAM_TERM_PROPERTY__STATUS)); 
            } 
        }

        public String ErrorMessage
        {
            get
            {
                return Term.WBxGetProperty(TEAM_TERM_PROPERTY__ERROR_MESSAGE);
            }

            set 
            {
                Term.WBxSetProperty(TEAM_TERM_PROPERTY__STATUS, TEAM_TERM_STATUS__ERROR);
                Term.WBxSetProperty(TEAM_TERM_PROPERTY__ERROR_MESSAGE, value);
            }
        }

        public String RecordsTypesListUrl
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__RECORDS_TYPES_LIST_URL); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__RECORDS_TYPES_LIST_URL, value); }
        }

        public String CommonActivitiesListUrl
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__COMMON_ACTIVITIES_LIST_URL); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__COMMON_ACTIVITIES_LIST_URL, value); }
        }

        public String FunctionalActivitiesListUrl
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__FUNCTIONAL_ACTIVITIES_LIST_URL); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__FUNCTIONAL_ACTIVITIES_LIST_URL, value); }
        }

        public String FunctionalAreaUIControlValue
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__FUNCTIONAL_AREA); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__FUNCTIONAL_AREA, value); }
        }

        public String InheritedFunctionalAreaUIControlValue
        {
            get {
                string UIControlValue = FunctionalAreaUIControlValue;
                if ((UIControlValue == null || UIControlValue == "") && Parent != null)
                    return Parent.InheritedFunctionalAreaUIControlValue;

                return UIControlValue; 
            }            
        }

        public String Acronym
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__ACRONYM); }
            set { Term.WBxSetProperty(TEAM_TERM_PROPERTY__ACRONYM, value); }
        }

        public String ManagerLogin
        {
            get { return Term.WBxGetProperty(TEAM_TERM_PROPERTY__MANAGER_LOGIN); }
            private set {
                WBLogging.Teams.Verbose("Setting the manager (" + value + ") of the team (" + Name + ")");
                Term.WBxSetProperty(TEAM_TERM_PROPERTY__MANAGER_LOGIN, value); 
            }
        }


        public bool IsPickable { get { return Term.IsAvailableForTagging; } }


        private bool _individualCommit = true;
        public bool IndividualCommit
        {
            get { return _individualCommit; }
            set { _individualCommit = value; }
        }

        #endregion

        #region Methods
        public void SetStatusOK()
        {
            _term.WBxSetProperty(TEAM_TERM_PROPERTY__STATUS, TEAM_TERM_STATUS__OK);
            _term.WBxSetProperty(TEAM_TERM_PROPERTY__ERROR_MESSAGE, "");
        }

        public WBTermCollection<WBTerm> FunctionalArea(WBTaxonomy functionalAreas)
        {
            string UIControlValue = FunctionalAreaUIControlValue;
            if ((UIControlValue == null || UIControlValue == "") && Parent != null) 
                return Parent.FunctionalArea(functionalAreas);

            return new WBTermCollection<WBTerm>(functionalAreas, FunctionalAreaUIControlValue);
        }

        public SPUser Manager(SPWeb web)
        {
            WBLogging.Teams.Verbose("Getting the manager (" + ManagerLogin + ") of the team (" + Name + ")");
            if (String.IsNullOrEmpty(ManagerLogin)) return null;

            return web.WBxEnsureUserOrNull(ManagerLogin);
        }

        public void SetManager(SPSite site, SPUser user)
        {
            if (user != null)
            {
                AddOwner(site, user);
                ManagerLogin = user.LoginName;
            }
            else
            {
                ManagerLogin = "";
            }
        }

        public override void Update()
        {
            UpdateWithTeamSiteWeb(null);
        }

        public void UpdateWithTeamSiteWeb(SPWeb teamSiteWeb)
        {
            if (_term == null) throw new Exception("You cannot call update until the term in the term store has been established");

            SetStatusOK();

            bool teamSiteHasChanged = (!CurrentTeamSiteUrl.Equals(TeamSiteUrl));

            if (teamSiteHasChanged)
            {
                // OK so first we're going to remove the association between the current team web site and this term:
                try
                {
                    using (SPSite currentSite = new SPSite(CurrentTeamSiteUrl))
                    {
                        using (SPWeb currentWeb = currentSite.OpenWeb())
                        {
                            currentWeb.WBxSetProperty(TEAM_SITE_PROPERTY__TERM_GUID, "");
                            currentWeb.Update();
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorMessage = "Failed to remove link to current team site.";
                    WBUtils.logMessage("Failed to remove link to current team site." + CurrentTeamSiteUrl + "  Exception details: " + e.StackTrace);

                }
            }

            if (StatusOK)
            {
                if (TeamSiteUrl != "")
                {
                    try
                    {
                        SPSite teamSiteObject = null;
                        bool needToDispose = false;

                        if (teamSiteWeb == null)
                        {
                            teamSiteObject = new SPSite(TeamSiteUrl);
                            teamSiteWeb = teamSiteObject.OpenWeb();
                            needToDispose = true;
                        }

                        teamSiteWeb.WBxSetProperty(TEAM_SITE_PROPERTY__TERM_GUID, Term.Id);
                        teamSiteWeb.Update();
                        WBUtils.logMessage("Called Update on team site with title: " + teamSiteWeb.Title);

                        Term.WBxSetProperty(TEAM_TERM_PROPERTY__TEAM_SITE_GUID, teamSiteWeb.ID);

                        if (needToDispose)
                        {
                            teamSiteWeb.Dispose();
                            teamSiteWeb = null;

                            teamSiteObject.Dispose();
                            teamSiteObject = null;
                        }
                                
                    }
                    catch (Exception e)
                    {
                        ErrorMessage = "Failed to add link to team site.";
                        WBUtils.logMessage("Failed to add link to team site: " + _currentTeamSiteUrl + "  Exception details: " + e.StackTrace);
                    }
                }

                //Term.IsAvailableForTagging = true;
                /*
                if (MembersGroupName == "")
                {
                    _term.IsAvailableForTagging = false;
                }
                else
                {
                    _term.IsAvailableForTagging = true;
                }
                 */ 
            }
            else
            {
                //_term.IsAvailableForTagging = false;
            }

            // For the moment let's leave all teams as being pickable.
            // Term.IsAvailableForTagging = true;

            if (_individualCommit) Term.TermStore.CommitAll();
            if (teamSiteWeb != null) teamSiteWeb.Update();
        }

        /// <summary>
        /// Synchronises the SharePoint SPGroup for this team from the Team Site site collection to the specified
        /// site colleciton.
        /// </summary>
        /// <param name="toSite"></param>
        public SPGroup SyncMembersGroup(SPSite toSite)
        {
            WBFarm farm = WBFarm.Local;

            using (SPSite teamsSite = new SPSite(farm.TeamSitesSiteCollectionUrl))
            {
                return SyncMembersGroup(teamsSite, toSite);
            }
        }

        public SPGroup SyncMembersGroup(SPSite teamsSite, SPSite toSite)
        {
            // If no members group has been defined then there is nothing to do:
            if (MembersGroupName == "")
            {
                WBLogging.Teams.Verbose("The team has no members group defined: " + Name);
                return null;
            }

            return WBUtils.SyncSPGroup(teamsSite, toSite, MembersGroupName);
        }

        public SPGroup MembersGroup(SPSite site)
        {
            SPGroup group = site.RootWeb.WBxGetGroupOrNull(MembersGroupName);

            if (group == null)
            {
                group = SyncMembersGroup(site);
            }
            return group;
        }

        public SPGroup OwnersGroup(SPSite site)
        {
            return site.RootWeb.WBxGetGroupOrNull(OwnersGroupName);
        }

        public SPGroup PublishersGroup(SPSite site)
        {
            return site.RootWeb.WBxGetGroupOrNull(PublishersGroupName);
        }


        public bool IsCurrentUserTeamMember()
        {
            if (SPContext.Current == null) return false;
            if (String.IsNullOrEmpty(MembersGroupName)) return false;
            SPGroup members = MembersGroup(SPContext.Current.Site);
            if (members == null) return false;
            return members.ContainsCurrentUser;
        }

        public bool IsUserTeamMember(SPUser user)
        {
            SPGroup members = MembersGroup(SPContext.Current.Site);
            if (members == null) return false;
            return members.WBxContainsUser(user);
        }

        public bool IsCurrentUserTeamOwner()
        {
            if (SPContext.Current == null) return false;
            SPGroup owners = OwnersGroup(SPContext.Current.Site);
            if (owners == null) return false;
            return owners.ContainsCurrentUser;
        }


        public bool IsCurrentUserTeamOwnerOrSystemAdmin()
        {
            return (IsCurrentUserTeamOwner() || WBFarm.Local.IsCurrentUserSystemAdmin());
        }

        public bool IsUserTeamOwner(SPUser user)
        {
            SPGroup owners = OwnersGroup(SPContext.Current.Site);
            if (owners == null) return false;
            return owners.WBxContainsUser(user);
        }

        public bool IsCurrentUserTeamManager()
        {
            if (SPContext.Current == null) return false;
            if (String.IsNullOrEmpty(ManagerLogin)) return false;

            return (ManagerLogin == SPContext.Current.Web.CurrentUser.LoginName);
        }

        public bool IsCurrentUserTeamManagerOrSystemAdmin()
        {
            return (IsCurrentUserTeamManager() || WBFarm.Local.IsCurrentUserSystemAdmin());
        }


        public bool IsUserTeamManager(SPUser user)
        {
            if (user == null) return false;
            if (String.IsNullOrEmpty(ManagerLogin)) return false;

            return (ManagerLogin == user.LoginName);
        }


        public void EmailTeam(SPSite site, SPWeb web, String subject, String body, bool isBodyHTML)
        {
            subject = subject.Replace("[TeamName]", Name);
            body = body.Replace("[TeamName]", Name);

            foreach (SPUser user in this.MembersGroup(site).Users) 
            {
                WBUtils.SendEmail(web, user.Email, subject, body, isBodyHTML);
            }            
        }

        #endregion

        #region Static Methods

        private static void SyncAllSubTeams(WBTaxonomy teams, TermCollection terms, SPSite teamsSite, SPSite site)
        {
            foreach (Term term in terms)
            {
                WBLogging.Teams.Verbose("Trying to sync the team with term name: " + term.Name);

                WBTeam team = new WBTeam(teams, term);
                try
                {
                    team.SyncMembersGroup(teamsSite, site);
                }
                catch (Exception e)
                {
                    WBLogging.Teams.Unexpected("Syncing failed for team: " + team.Name + " to site: " + site.Url);
                }

                WBLogging.Teams.Verbose("Next syncing all sub-teams of team: " + team.Name);
                SyncAllSubTeams(teams, term.Terms, teamsSite, site);
            }
        }

        public static void SyncAllTeams(SPSite site)
        {
            WBTaxonomy teams = WBTaxonomy.GetTeams(site);

            WBLogging.Teams.Verbose("Syncing all teams within the TermSet: " + teams.TermSet.Name);
                        
            WBFarm farm = WBFarm.Local;

            using (SPSite teamsSite = new SPSite(farm.TeamSitesSiteCollectionUrl))                        
            {
                SyncAllSubTeams(teams, teams.TermSet.Terms, teamsSite, site);
            }
        }

        #endregion

        internal void AddOwner(SPSite site, SPUser user)
        {
            SPGroup owners = this.OwnersGroup(site);

            if (owners == null)
            {
                WBLogging.Teams.Unexpected("Trying to add a user to be an owner in a team ("+Name+") where there is no set owners group!");
                return;
            }

            if (!owners.WBxContainsUser(user))
            {
                owners.AddUser(user);
                owners.Update();
            }
        }


        internal void AddOwners(SPSite site, List<SPUser> newUsers)
        {
            SPGroup owners = this.OwnersGroup(site);

            if (owners != null)
            {
                foreach (SPUser user in newUsers)
                {
                    owners.AddUser(user);
                }

                owners.Update();
            }
        }

        internal void AddMembers(SPSite site, List<SPUser> newUsers)
        {
            SPGroup members = this.MembersGroup(site);

            if (members != null)
            {
                foreach (SPUser user in newUsers)
                {
                    members.AddUser(user);
                }

                members.Update();

                SyncAddMembers(members, newUsers);
            }
        }


        internal void RemoveOwner(SPSite site, SPUser userToRemove)
        {
            if (userToRemove == null) return;
            if (IsUserTeamManager(userToRemove))
            {
                WBLogging.Teams.HighLevel("You can't remove the team manager ("+ManagerLogin+") from being an owner of the team ("+Name+")");
                return;
            }

            SPGroup owners = this.OwnersGroup(site);

            if (owners != null)
            {
                owners.RemoveUser(userToRemove);
                owners.Update();
            }
        }

        internal void RemoveMember(SPSite site, SPUser userToRemove)
        {
            SPGroup members = this.MembersGroup(site);

            if (members != null)
            {
                members.RemoveUser(userToRemove);
                members.Update();

                SyncRemoveMember(members, userToRemove);
            }
        }

        public void AddManagersDirectReports()
        {
            AddManagersDirectReports(SPContext.Current.Site, SPContext.Current.Web);
        }

        public void AddManagersDirectReports(SPSite site, SPWeb web)
        {
            // Let's just check that the current user has the rights of an owner in order to perform this method:
            if (!IsCurrentUserTeamOwnerOrSystemAdmin())
            {
                WBLogging.Generic.Verbose("Cannot do 'AddManagersDirectReports' as the current user (" + web.CurrentUser + ") doesn't have owner role on this team (" + Name + ")");
                return;
            }

            WBUser manager = Manager(web).WBxUser(site, web);

            if (manager != null && manager.HasProfile)
            {
                AddMembers(site, WBUtils.GetSPUsers(web, manager.Profile.WBxGetDirectReportsLogins()));
            }
        }

        public void AddAllManagersReports()
        {
            AddAllManagersReports(SPContext.Current.Site, SPContext.Current.Web);
        }

        public void AddAllManagersReports(SPSite site, SPWeb web)
        {
            // Let's just check that the current user has the rights of an owner in order to perform this method:
            if (!IsCurrentUserTeamOwnerOrSystemAdmin())
            {
                WBLogging.Generic.Verbose("Cannot do 'AddAllManagersReports' as the current user (" + web.CurrentUser + ") doesn't have owner role on this team (" + Name + ")");
                return;
            }

            WBUser manager = Manager(web).WBxUser(site, web);

            if (manager != null && manager.HasProfile)
            {
                AddMembers(site, WBUtils.GetSPUsers(web, manager.Profile.WBxGetAllReportsLogins()));
            }
        }


        private void SyncAddMembers(SPGroup fromGroup, List<SPUser> newUsers)
        {
            WBFarm farm = WBFarm.Local;
            String groupName = fromGroup.Name;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(farm.TimerJobsManagementSiteUrl))
                using (SPWeb web = site.OpenWeb())
                {
                    SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                    UserProfileManager profileManager = new UserProfileManager(serviceContext);

                    SPList dailyJobs = web.Lists[WBTimerTasksJob.DAILY_TIMER_TASKS__LIST_NAME];
                    SPView inOrderToExecute = dailyJobs.Views[WBTimerTasksJob.DAILY_TIMER_TASKS__ORDERED_VIEW_NAME];

                    foreach (SPListItem task in dailyJobs.GetItems(inOrderToExecute))
                    {
                        string command = task.WBxGetColumnAsString(WBTimerTask.COLUMN_NAME__COMMAND);
                        string targetUrl = task.WBxGetColumnAsString(WBTimerTask.COLUMN_NAME__TARGET_URL);
                        string argument1 = task.WBxGetColumnAsString(WBTimerTask.COLUMN_NAME__ARGUMENT_1);

                        if (command == WBTimerTask.COMMAND__SYNCHRONISE_ALL_TEAMS)
                        {
                            try
                            {
                                using (SPSite toSite = new SPSite(targetUrl))
                                {
                                    SPGroup toGroup = toSite.RootWeb.WBxGetGroupOrNull(groupName);

                                    toSite.AllowUnsafeUpdates = true;
                                    toSite.RootWeb.AllowUnsafeUpdates = true;

                                    if (toGroup == null)
                                    {
                                        SPUser defaultUser = WBUtils.GetLocalUserFromGroupOrSystemAccount(toSite, fromGroup);
                                        SPUser systemUser = toSite.SystemAccount;

                                        WBLogging.Teams.Verbose("Found the user - about to create new group");
                                        toSite.RootWeb.SiteGroups.Add(groupName, systemUser, defaultUser, fromGroup.Description);

                                        WBLogging.Teams.Verbose("Created new group.");

                                        toGroup = toSite.RootWeb.SiteGroups[groupName];
                                    }

                                    foreach (SPUser fromUser in newUsers)
                                    {
                                        // If the user doesn't exist in the user profile - then we assume that they've been disabled:
                                        if (!profileManager.UserExists(fromUser.LoginName))
                                        {
                                            WBLogging.Teams.Monitorable("Ignoring user as they appear to be disabled: " + fromUser.LoginName);
                                            continue;
                                        }

                                        WBLogging.Teams.Verbose("Copying across a user: " + fromUser.LoginName);

                                        SPUser toUser = toSite.RootWeb.WBxEnsureUserOrNull(fromUser.LoginName);

                                        if (toUser != null)
                                        {
                                            try
                                            {
                                                toGroup.AddUser(toUser);
                                            }
                                            catch (Exception exception)
                                            {
                                                WBLogging.Teams.Unexpected("Something went wrong when trying to add " + fromUser.LoginName + " to " + groupName + " on site collection " + targetUrl, exception);
                                            }
                                        }
                                    }

                                    toGroup.OnlyAllowMembersViewMembership = false;

                                    toGroup.Update();
                                }
                            }
                            catch (Exception exception)
                            {
                                WBLogging.Teams.Unexpected("Something went wrong when trying to add a set of users to " + groupName + " on site collection " + targetUrl, exception);
                            }
                        }

                    }
                }

            });



        }

        private void SyncRemoveMember(SPGroup fromGroup, SPUser userToRemove)
        {
            WBFarm farm = WBFarm.Local;
            String groupName = fromGroup.Name;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(farm.TimerJobsManagementSiteUrl))
                using (SPWeb web = site.OpenWeb())
                {
                    SPList dailyJobs = web.Lists[WBTimerTasksJob.DAILY_TIMER_TASKS__LIST_NAME];
                    SPView inOrderToExecute = dailyJobs.Views[WBTimerTasksJob.DAILY_TIMER_TASKS__ORDERED_VIEW_NAME];

                    foreach (SPListItem task in dailyJobs.GetItems(inOrderToExecute))
                    {
                        string command = task.WBxGetColumnAsString(WBTimerTask.COLUMN_NAME__COMMAND);
                        string targetUrl = task.WBxGetColumnAsString(WBTimerTask.COLUMN_NAME__TARGET_URL);
                        string argument1 = task.WBxGetColumnAsString(WBTimerTask.COLUMN_NAME__ARGUMENT_1);

                        if (command == WBTimerTask.COMMAND__SYNCHRONISE_ALL_TEAMS)
                        {
                            using (SPSite toSite = new SPSite(targetUrl))
                            {
                                SPGroup toGroup = toSite.RootWeb.WBxGetGroupOrNull(groupName);

                                toSite.AllowUnsafeUpdates = true;
                                toSite.RootWeb.AllowUnsafeUpdates = true;

                                if (toGroup != null)
                                {
                                    WBLogging.Teams.Verbose("Removing a user: " + userToRemove.LoginName);

                                    try
                                    {
                                        toGroup.RemoveUser(userToRemove);
                                        toGroup.Update();
                                    }
                                    catch (Exception exception)
                                    {
                                        WBLogging.Teams.Unexpected("Something went wrong when trying to remove " + userToRemove.LoginName + " from " + groupName + " on site collection " + targetUrl, exception);
                                    }

                                }

                            }
                        }

                    }
                }

            });



        }

    }


}
