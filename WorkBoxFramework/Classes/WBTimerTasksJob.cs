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
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace WorkBoxFramework
{
    class WBTimerTasksJob : SPJobDefinition
    {
        internal const string DAILY_TIMER_TASKS__TIMER_JOB_NAME = "Work Box Framework - Daily Timer Tasks";
        internal const string DAILY_TIMER_TASKS__LIST_NAME = "Daily Timer Tasks";
        internal const string DAILY_TIMER_TASKS__ORDERED_VIEW_NAME = "In Order To Execute";

        internal const string FREQUENT_TIMER_TASKS__TIMER_JOB_NAME = "Work Box Framework - Frequent Timer Tasks";
        internal const string FREQUENT_TIMER_TASKS__LIST_NAME = "Frequent Timer Tasks";
        internal const string FREQUENT_TIMER_TASKS__ORDERED_VIEW_NAME = "In Order To Execute";


        [Persisted]
        private String listName;

        [Persisted]
        private String viewName;

        public WBTimerTasksJob() : base() { }

        public WBTimerTasksJob(String jobName, String listName, String viewName, SPWebApplication webApplication, SPServer server, SPJobLockType jobLockType)
            : base (jobName, webApplication, server, jobLockType)
        {
            this.Title = jobName; 
            this.listName = listName;
            this.viewName = viewName;
        }

        public override void Execute(Guid targetInstanceId)
        {
            WBLogging.TimerTasks.HighLevel("WBTimerTasksJob.Execute(): Starting: " + Title);

            WBFarm farm = WBFarm.Local;

            using (SPSite site = new SPSite(farm.TimerJobsManagementSiteUrl))
            using (SPWeb web = site.OpenWeb())
            {
                SPList dailyJobs = web.Lists[listName];
                SPView inOrderToExecute = dailyJobs.Views[viewName];

                foreach (SPListItem task in dailyJobs.GetItems(inOrderToExecute))
                {
                    WBLogging.TimerTasks.HighLevel("WBTimerTasksJob.Execute(): About to execute task: " + task.Title);

                    bool originalAccessDeniedCatchValue = SPSecurity.CatchAccessDeniedException;
                    SPSecurity.CatchAccessDeniedException = false;

                    try
                    {
                        WBTimerTask.Execute(task);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        WBLogging.TimerTasks.Unexpected("WBTimerTasksJob.Execute(): UnauthorizedAccessException thrown when trying to run task: " + task.Title + " Exception was: " + e.Message);
                    }
                    catch (Exception e)
                    {
                        WBLogging.TimerTasks.Unexpected("WBTimerTasksJob.Execute(): Exception thrown when trying to run task: " + task.Title + " Exception was: " + e.Message);
                    }
                    finally
                    {
                        SPSecurity.CatchAccessDeniedException = originalAccessDeniedCatchValue;
                    }

                    WBLogging.TimerTasks.HighLevel("WBTimerTasksJob.Execute(): Finished executing task: " + task.Title);
                }
            }

            WBLogging.TimerTasks.HighLevel("WBTimerTasksJob.Execute(): Finished: " + Title);
        }
    }
}
