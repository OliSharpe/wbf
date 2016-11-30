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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;

namespace WorkBoxFramework.Features.TimerJobs
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    // Developed with key ideas taken from the following blog post:
    // http://www.andrewconnell.com/blog/articles/CreatingCustomSharePointTimerJobs.aspx

    [Guid("c88756e1-fa13-44ee-aa95-09a323626abe")]
    public class TimerJobsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            WBLogging.Generic.HighLevel("TimerJobsEventReceiver.FeatureActivated(): Activating the WBF Timer Jobs feature");

            SPWebApplication webApplication = properties.Feature.Parent as SPWebApplication;
  
            bool onSharePointHub = false;
            if (webApplication.Name.ToLower().Contains("sharepointhub")) onSharePointHub = true;

            foreach(SPAlternateUrl alternateUrl in webApplication.AlternateUrls)
            {
                if (alternateUrl.IncomingUrl.ToString().ToLower() == "http://sharepointhub/")
                {
                    onSharePointHub = true;
                    break;
                }
            }

            // So if we're not on the SharePointHub web application then we'll just end the activation process here:
            if (!onSharePointHub)
            {
                WBLogging.Generic.HighLevel("TimerJobsEventReceiver.FeatureActivated(): No activation is being done as we are on: " + webApplication.Name);
                return;
            }

            WBLogging.Generic.Verbose("TimerJobsEventReceiver.FeatureActivated(): Activation is happening on SharePointHub.");

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

                    WBLogging.Generic.HighLevel("TimerJobsEventReceiver.FeatureActivated(): Adding a timer job to server : " + server.Name + " with name: " + WBTimerTasksJob.DAILY_TIMER_TASKS__TIMER_JOB_NAME);

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

                    WBLogging.Generic.HighLevel("TimerJobsEventReceiver.FeatureActivated(): Adding a timer job to server : " + server.Name + " with name: " + WBTimerTasksJob.FREQUENT_TIMER_TASKS__TIMER_JOB_NAME);

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


                    /* */
                    /* Now adding the Mirgation Timer Job  */
                    /* */

                    WBLogging.Generic.HighLevel("TimerJobsEventReceiver.FeatureActivated(): Adding a timer job to server : " + server.Name + " with name: " + WBMigrationTimerJob.MIGRATION_TIMER_JOB__TIMER_JOB_NAME);

                    WBMigrationTimerJob migrationTimerJob = new WBMigrationTimerJob(
                        WBMigrationTimerJob.MIGRATION_TIMER_JOB__TIMER_JOB_NAME,
                        webApplication,
                        server,
                        SPJobLockType.Job);

                    SPWeeklySchedule migrationTimerJobSchedule = new SPWeeklySchedule();
                    migrationTimerJobSchedule.BeginDayOfWeek = System.DayOfWeek.Monday;
                    migrationTimerJobSchedule.BeginHour = 5;
                    migrationTimerJobSchedule.BeginMinute = 0;
                    migrationTimerJobSchedule.BeginSecond = 0;

                    migrationTimerJobSchedule.EndDayOfWeek = System.DayOfWeek.Monday;
                    migrationTimerJobSchedule.EndHour = 5;
                    migrationTimerJobSchedule.EndMinute = 10;
                    migrationTimerJobSchedule.EndSecond = 0;

                    migrationTimerJob.Schedule = migrationTimerJobSchedule;

                    migrationTimerJob.Update();
                }
                else
                {
                    WBLogging.Generic.Unexpected("TimerJobsEventReceiver.FeatureActivated(): Couldn't find the server with the name: " + farm.TimerJobsServerName);
                }
            }
            else
            {
                WBLogging.Generic.Unexpected("TimerJobsEventReceiver.FeatureActivated(): The WBF farm wide setting of which server to use for the timer job has not been set.");
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            /*
            SPWebApplication webApplication = properties.Feature.Parent as SPWebApplication;

            // Delete the job
            foreach (SPJobDefinition job in webApplication.JobDefinitions)
            {
                if (job.Name == WBTimerTasksJob.DAILY_TIMER_TASKS__TIMER_JOB_NAME)
                    job.Delete();

                if (job.Name == WBTimerTasksJob.FREQUENT_TIMER_TASKS__TIMER_JOB_NAME)
                    job.Delete();

                if (job.Name == WBMigrationTimerJob.MIGRATION_TIMER_JOB__TIMER_JOB_NAME)
                    job.Delete();
            }
             */ 
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
