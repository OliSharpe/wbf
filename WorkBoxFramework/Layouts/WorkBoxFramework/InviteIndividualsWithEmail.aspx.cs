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
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Collections.Specialized;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class InviteIndividualsWithEmail : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WorkBox.CurrentUserIsOwner() && !WorkBox.CurrentUserIsBusinessAdmin() && !WorkBox.CurrentUserIsSystemAdmin())
            {
                ErrorText.Text = "Only owners or admin can invite individuals to have access to a work box.";
                return;
            }


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

                IndividualsToInviteControl.Focus();

                DisposeWorkBox();
            }
        }

        protected void inviteButton_OnClick(object sender, EventArgs e)
        {
            List<SPUser> newUsers = IndividualsToInviteControl.WBxGetMultiResolvedUsers(SPContext.Current.Web);

            if (newUsers.Count == 0)
            {
                IndividualsToInviteFieldMessage.Text = "You must enter at least one individual to invite.";
                return;
            }

            List<String> newUsersNames = new List<String>();
            foreach (SPUser user in newUsers)
            {
                newUsersNames.Add(user.Name);
            }
            String newUsersString = String.Join(", ", newUsersNames.ToArray());

            if (InviteType.SelectedValue == "Involved") 
            {
                List<SPUser> involvedUsers = WorkBox.InvolvedIndividuals;
                involvedUsers.AddRange(newUsers);
                WorkBox.InvolvedIndividuals = involvedUsers;
                WorkBox.AuditLogEntry("Invited individuals", "Involved: " + newUsersString);
                WorkBox.Update();
            } 
            else
            {
                List<SPUser> visitingUsers = WorkBox.VisitingIndividuals;
                visitingUsers.AddRange(newUsers);
                WorkBox.VisitingIndividuals = visitingUsers;
                WorkBox.AuditLogEntry("Invited individuals", "Visiting: " + newUsersString);
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

                    String subject = WBUtils.ProcessEmailTemplate(WorkBox, subjectTemplate, false);
                    String body = WBUtils.ProcessEmailTemplate(WorkBox, bodyTemplate, true);

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

                        String subject = WBUtils.ProcessEmailTemplate(WorkBox, null, user, subjectTemplate, false);
                        String body = WBUtils.ProcessEmailTemplate(WorkBox, null, user, bodyTemplate, true);

                        headers.Add("subject", subject);

                        WBUtils.SendEmail(SPContext.Current.Web, headers, body);
                    }
                }

            }


            DisposeWorkBox();

            CloseDialogAndRefresh();
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            DisposeWorkBox();

            CloseDialogWithCancel("Inviting of individuals was cancelled");
        }
/*
        protected void inviteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            WBLogging.Debug("In inviteType_SelectedIndexChanged() with SelectedValue = " + InviteType.SelectedValue + " OtherInviteValue = " + OtherInviteType.Value);

            if (InviteType.SelectedValue == OtherInviteType.Value)
            {
                String subject = OtherEmailSubject.Value;
                String body = OtherEmailBody.Value;

                OtherEmailSubject.Value = EmailSubject.Text;
                OtherEmailBody.Value = EmailBody.Text;

                EmailSubject.Text = subject;
                EmailBody.Text = body;

                if (InviteType.SelectedValue == "Involved")
                {
                    OtherInviteType.Value = "Visiting";
                }
                else
                {
                    OtherInviteType.Value = "Involved";
                }
            }
        }
         */ 
    }
}
