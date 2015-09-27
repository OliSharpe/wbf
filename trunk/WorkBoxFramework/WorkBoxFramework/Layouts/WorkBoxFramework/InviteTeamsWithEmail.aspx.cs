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
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Collections.Specialized;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class InviteTeamsWithEmail : WorkBoxDialogPageBase
    {
        private WBTaxonomy teamsTaxonomy = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WorkBox.CurrentUserIsOwner() && !WorkBox.CurrentUserIsBusinessAdmin() && !WorkBox.CurrentUserIsSystemAdmin())
            {
                ErrorText.Text = "Only owners or admin can invite teams to have access to a work box.";
                return;
            }


            teamsTaxonomy = WBTaxonomy.GetTeams(SPContext.Current.Site);

            if (!IsPostBack)
            {
                WorkBoxTitle.Text = WorkBox.Title;

                InviteType.SelectedValue = "Involved";
                CurrentlySelectedValue.Value = "Involved";

                SendInviteEmail.Checked = true;
                SendAsOne.Checked = true;
                CCToYou.Checked = true;

                EmailSubject.Text = WorkBox.Template.InviteInvolvedUserEmailSubject;
                EmailBody.Text = WorkBox.Template.InviteInvolvedUserEmailBody;

                OtherEmailSubject.Value = WorkBox.Template.InviteVisitingUserEmailSubject;
                OtherEmailBody.Value = WorkBox.Template.InviteVisitingUserEmailBody;

                teamsTaxonomy.InitialiseTaxonomyControl(TeamsToInviteControl, "Teams to invite", true);

                TeamsToInviteControl.Focus();

                DisposeWorkBox();
            }
        }

        protected void inviteButton_OnClick(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(TeamsToInviteControl.Text))
            {
                InvolvedTeamsFieldMessage.Text = "You must enter at least one team to invite";
                return;
            }


            WBTermCollection<WBTeam> newTeams = new WBTermCollection<WBTeam>(teamsTaxonomy, TeamsToInviteControl.Text);

            String newTeamsString = newTeams.Names().Replace(";", ",");

            if (InviteType.SelectedValue == "Involved")
            {
                WBTermCollection<WBTeam> involvedTeams = WorkBox.InvolvedTeams;
                involvedTeams.Add(newTeams);
                WorkBox.InvolvedTeams = involvedTeams;
                WorkBox.AuditLogEntry("Invited teams", "Involved: " + newTeamsString);
                WorkBox.Update();
            }
            else
            {
                WBTermCollection<WBTeam> visitingTeams = WorkBox.VisitingTeams;
                visitingTeams.Add(newTeams);
                WorkBox.VisitingTeams = visitingTeams;
                WorkBox.AuditLogEntry("Invited teams", "Visiting: " + newTeamsString);
                WorkBox.Update();
            }

            if (SendInviteEmail.Checked)
            {
                String subjectTemplate = EmailSubject.Text;
                String bodyTemplate = EmailBody.Text;

                if (SendAsOne.Checked)
                {
                    StringDictionary headers = new StringDictionary();

                    List<String> emailAddresses = new List<String>();

                    foreach (WBTeam team in newTeams)
                    {
                        foreach (SPUser user in team.MembersGroup(SPContext.Current.Site).Users)
                        {
                            if (!emailAddresses.Contains(user.Email))
                            {
                                emailAddresses.Add(user.Email);
                            }
                        }
                    }

                    headers.Add("to", String.Join(";", emailAddresses.ToArray()));
                    headers.Add("content-type", "text/html"); //This is the default type, so isn't neccessary.

                    if (CCToYou.Checked)
                    {
                        headers.Add("cc", SPContext.Current.Web.CurrentUser.Email);
                    }

                    String subject = WBUtils.ProcessEmailTemplate(WorkBox, subjectTemplate, false);
                    String body = WBUtils.ProcessEmailTemplate(WorkBox, bodyTemplate, true);

                    headers.Add("subject", subject);

                    WBUtils.SendEmail(SPContext.Current.Web, headers, body);
                }
                else
                {
                    List<String> emailAddresses = new List<String>();

                    foreach (WBTeam team in newTeams)
                    {
                        foreach (SPUser user in team.MembersGroup(SPContext.Current.Site).Users)
                        {
                            // Check if we're emailed this person already:
                            if (!emailAddresses.Contains(user.Email))
                            {
                                emailAddresses.Add(user.Email);

                                StringDictionary headers = new StringDictionary();

                                headers.Add("to", user.Email);
                                headers.Add("content-type", "text/html"); //This is the default type, so isn't neccessary.

                                if (CCToYou.Checked)
                                {
                                    headers.Add("cc", SPContext.Current.Web.CurrentUser.Email);
                                }

                                String subject = WBUtils.ProcessEmailTemplate(WorkBox, null, user, subjectTemplate, false);
                                String body = WBUtils.ProcessEmailTemplate(WorkBox, null, user, bodyTemplate, true);

                                headers.Add("subject", subject);

                                WBUtils.SendEmail(SPContext.Current.Web, headers, body);
                            }
                        }
                    }
                }

            }


            DisposeWorkBox();

            CloseDialogAndRefresh("?panel=TeamDetails");
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            CloseDialogWithCancel("Inviting of teams was cancelled");
        }
    }
}
