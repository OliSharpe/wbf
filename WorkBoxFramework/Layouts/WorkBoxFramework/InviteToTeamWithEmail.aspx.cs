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
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Specialized;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class InviteToTeamWithEmail : WBDialogPageBase
    {
        private WBTaxonomy teams = null;
        private WBTeam team = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            teams = WBTaxonomy.GetTeams(SPContext.Current.Site);
            team = WBTeam.getFromTeamSite(teams, SPContext.Current);

            if (team == null)
            {
                ErrorMessageLabel.Text = "You do not appear to be viewing this form while on a team site.";
                return;
            }


            if (!team.IsCurrentUserTeamOwner())
            {
                ErrorText.Text = "Only team owners can invite individuals to a team.";
                return;
            }


            if (!IsPostBack)
            {
                TeamName.Text = team.Name;

                InviteAsMember.Checked = true;
                InviteAsOwner.Checked = false;

                SendInviteEmail.Checked = true;
                SendAsOne.Checked = true;
                CCToYou.Checked = true;

                EmailSubject.Text = WBFarm.Local.InviteToTeamDefaultEmailSubject;
                EmailBody.Text = WBFarm.Local.InviteToTeamDefaultEmailBody;

            }
        }

        protected void inviteButton_OnClick(object sender, EventArgs e)
        {
            List<SPUser> newUsers = IndividualsToInviteControl.WBxGetMultiResolvedUsers(SPContext.Current.Web);

            List<String> newUsersNames = new List<String>();
            foreach (SPUser user in newUsers)
            {
                newUsersNames.Add(user.Name);
            }
            String newUsersString = String.Join(", ", newUsersNames.ToArray());

            if (!InviteAsMember.Checked && !InviteAsOwner.Checked)
            {
                ErrorText.Text = "You didn't select to invite as either a member or as an owner!";
                return;
            }

            String roleInTeam = "";

            if (InviteAsMember.Checked)
            {
                team.AddMembers(SPContext.Current.Site, newUsers);
                roleInTeam = "team member";
            }

            if (InviteAsOwner.Checked)
            {
                team.AddOwners(SPContext.Current.Site, newUsers);
                if (String.IsNullOrEmpty(roleInTeam))
                {
                    roleInTeam = "team owner";
                }
                else
                {
                    roleInTeam += " and team owner";
                }
                
            }
            Dictionary<String, String> textForTokens = new Dictionary<String, String>();
            textForTokens.Add("[ROLE_WITHIN_TEAM]", roleInTeam);

            if (SendInviteEmail.Checked)
            {
                String subjectTemplate = EmailSubject.Text;
                String bodyTemplate = EmailBody.Text;

                if (SendAsOne.Checked)
                {
                    StringDictionary headers = new StringDictionary();

                    List<String> emailAddresses = new List<String>();
                    foreach (SPUser user in newUsers)
                    {
                        emailAddresses.Add(user.Email);
                    }

                    headers.Add("to", String.Join(";", emailAddresses.ToArray()));
                    headers.Add("content-type", "text/html"); //This is the default type, so isn't neccessary.

                    if (CCToYou.Checked)
                    {
                        headers.Add("cc", SPContext.Current.Web.CurrentUser.Email);
                    }

                    String subject = WBUtils.ProcessEmailTemplate(textForTokens, null, team, null, subjectTemplate, false);
                    String body = WBUtils.ProcessEmailTemplate(textForTokens, null, team, null, bodyTemplate, true);

                    headers.Add("subject", subject);

                    WBUtils.SendEmail(SPContext.Current.Web, headers, body);
                }
                else
                {
                    foreach (SPUser user in newUsers)
                    {
                        StringDictionary headers = new StringDictionary();

                        headers.Add("to", user.Email);
                        headers.Add("content-type", "text/html"); //This is the default type, so isn't neccessary.

                        if (CCToYou.Checked)
                        {
                            headers.Add("cc", SPContext.Current.Web.CurrentUser.Email);
                        }

                        String subject = WBUtils.ProcessEmailTemplate(textForTokens, null, team, user, subjectTemplate, false);
                        String body = WBUtils.ProcessEmailTemplate(textForTokens, null, team, user, bodyTemplate, true);

                        headers.Add("subject", subject);

                        WBUtils.SendEmail(SPContext.Current.Web, headers, body);
                    }
                }

            }

            this.returnFromDialogOKAndRefresh();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Inviting of individuals to the team.");
        }
    }
}
