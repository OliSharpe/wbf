using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class CheckTeamSync : WBDialogPageBase
    {
        private WBTaxonomy teams = null;
        private WBTeam team = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                StringBuilder html = new StringBuilder(); 

                WBFarm farm = WBFarm.Local;

                teams = WBTaxonomy.GetTeams(SPContext.Current.Site);
                team = WBTeam.GetFromTeamSite(teams, SPContext.Current);

                if (team == null)
                {
                    ErrorText.Text = "You do not appear to be viewing this form while on a team site.";
                    return;
                }

                if (!WBFarm.Local.IsCurrentUserSystemAdmin())
                {
                    ErrorText.Text = "Only team owners or system admin can invite individuals to a team.";
                    return;
                }

                List<List<String>> tableColumnThenRow = new List<List<String>>();


                // First we're going to add the list of group members on the team sites site collection:
                // Note that we're actually assuming here that the web part is being used on the teams sites site collection!
                SPGroup fromGroup = team.MembersGroup(SPContext.Current.Site);

                List<String> column = new List<String>();
                foreach (SPUser user in fromGroup.Users)
                {
                    column.Add(user.Name);
                }

                column.Sort();

                column.Insert(0, farm.TeamSitesSiteCollectionUrl);
                tableColumnThenRow.Add(column);


                
                String groupName = team.MembersGroupName;

                html.Append("<p>Checking synchronisation of team   : <b>").Append(team.Name).Append("</b></p>");
                html.Append("<p>The underlying SharePoint group is : <b>").Append(groupName).Append("</b></p>");

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
                                // Create a new column object:
                                column = new List<String>();

                                try
                                {

                                    using (SPSite toSite = new SPSite(targetUrl))
                                    {
                                        toSite.AllowUnsafeUpdates = true;
                                        toSite.RootWeb.AllowUnsafeUpdates = true;

                                        SPGroup toGroup = WBUtils.SyncSPGroup(SPContext.Current.Site, toSite, groupName);

                                        foreach (SPUser user in toGroup.Users)
                                        {
                                            column.Add(user.Name);
                                        }

                                        column.Sort();

                                    }
                                }
                                catch (Exception exception)
                                {
                                    WBLogging.Teams.Unexpected("Something went wrong when trying to add a set of users to " + groupName + " on site collection " + targetUrl, exception);

                                    column.Add("Exception: " + exception.Message);
                                }

                                // First row in all columns is the URL of the site collection:
                                column.Insert(0, targetUrl);

                                tableColumnThenRow.Add(column);
                            }

                        }

                    }

                });

                html.Append("\n<table width='100%' cellpadding='2px' cellspacing='5px'>\n");

                int columns = tableColumnThenRow.Count;
                bool foundValues = true;

                html.Append("<tr>\n");
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    html.Append("<th>").Append(tableColumnThenRow[columnIndex][0]).Append("</th>");
                }
                html.Append("</tr>\n");

                int rowIndex = 1;
                while (foundValues)
                {
                    foundValues = false;

                    html.Append("<tr>\n");
                    for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                    {
                        if (tableColumnThenRow[columnIndex].Count > rowIndex)
                        {
                            html.Append("<td>").Append(tableColumnThenRow[columnIndex][rowIndex]).Append("</td>");
                            foundValues = true;
                        }
                        else
                        {
                            html.Append("<td></td>");
                        }
                    }
                    html.Append("</tr>\n");

                    rowIndex++;
                }

                html.Append("</table>\n");

                TeamSyncInformation.Text = html.ToString();
            }

        }

        protected void OKButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithOK();
        }

    }
}
