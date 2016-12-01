using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class EditRecordsMetadata : WBDialogPageBase
    {
        private String currentUserLoginName = "";
        private WBTaxonomy teams = null;
        private WBTeam team = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            teams = WBTaxonomy.GetTeams(SPContext.Current.Site);
            team = WBTeam.GetFromTeamSite(teams, SPContext.Current.Web);
            if (team == null)
            {
                WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);
                if (workBox != null)
                {
                    team = workBox.OwningTeam;
                }
            }

            // Check if this user has permission - checking basic team membership:
            if (team == null || !team.IsCurrentUserTeamMember())
            {
                AccessDeniedPanel.Visible = true;
                UpdateRecordsMetadataPanel.Visible = false;
                AccessDeniedReason.Text = "You are not a member of this team";
                return;
            }

            currentUserLoginName = SPContext.Current.Web.CurrentUser.LoginName;

            if (!IsPostBack)
            {
                RecordID.Text = Request.QueryString["RecordID"];
                WBLogging.Debug("Record ID is found to be: " + RecordID.Text);

                using (WBRecordsManager manager = new WBRecordsManager(currentUserLoginName))
                {
                    WBRecord record = manager.Libraries.GetRecordByID(RecordID.Text);

                    Filename.Text = record.Name;
                    RecordTitle.Text = record.Title;

                    String location = "<unknown>";
                    if (record.FunctionalArea != null && record.FunctionalArea.Count > 0)
                    {
                        WBLogging.Debug("Found functional area = " + record.FunctionalArea);
                        WBTerm functionalArea = record.FunctionalArea[0];
                        location = functionalArea.FullPath;
                        WBLogging.Debug("location = " + location);

                        WBTermCollection<WBTerm> teamsFunctionalAreas = team.FunctionalArea(teams);

                        if (!teamsFunctionalAreas.Contains(functionalArea))
                        {
                            AccessDeniedPanel.Visible = true;
                            UpdateRecordsMetadataPanel.Visible = false;
                            AccessDeniedReason.Text = "The team " + team.Name + " does not have permission to edit this functional area: " + functionalArea.Name;
                            return;
                        }

                    }
                    location += "/" + record.RecordsType.FullPath;

                    String folders = record.ProtectedMasterRecord.LibraryRelativePath.Replace(record.Name, "").Replace(location, "");

                    RecordsLocation.Text = "<b>" + location + "</b> " + folders;

                    String status = record.RecordSeriesStatus;
                    RecordSeriesStatus.Text = status;

                    String explainStatus = "";
                    if (status == "Latest")
                    {
                        if (record.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                        {
                            explainStatus = "(live on the public website)";
                        }
                        else if (record.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                        {
                            explainStatus = "(live on a public extranet website)";
                        }
                        else
                        {
                            explainStatus = "(live on izzi intranet)";
                        }
                    }
                    else if (status == "Retired")
                    {
                        explainStatus = "(visible on izzi intranet searches)";
                    }
                    else if (status == "Archived")
                    {
                        explainStatus = "(archived in the protected, master records library)";
                    }
                    ExplainStatus.Text = explainStatus;

                    RecordSeriesStatusChange.DataSource = new String[] { "", "Retire", "Archive" };
                    RecordSeriesStatusChange.DataBind();
                    RecordSeriesStatusChange.SelectedValue = "";

                    ProtectiveZone.DataSource = new String[] { WBRecordsType.PROTECTIVE_ZONE__PROTECTED, WBRecordsType.PROTECTIVE_ZONE__PUBLIC };
                    ProtectiveZone.DataBind();
                    ProtectiveZone.SelectedValue = record.ProtectiveZone;

                    manager.SubjectTagsTaxonomy.InitialiseTaxonomyControl(SubjectTags, WBColumn.SubjectTags.DisplayName, true);
                    SubjectTags.Text = record.SubjectTagsUIControlValue;

                    manager.TeamsTaxonomy.InitialiseTaxonomyControl(OwningTeam, WBColumn.OwningTeam.DisplayName, false);
                    OwningTeam.Text = record.OwningTeam.UIControlValue;

                    manager.TeamsTaxonomy.InitialiseTaxonomyControl(InvolvedTeams, WBColumn.InvolvedTeams.DisplayName, true);
                    InvolvedTeams.Text = record.InvolvedTeamsWithoutOwningTeamAsUIControlValue;
                }
            }
        }


        protected void updateButton_OnClick(object sender, EventArgs e)
        {
            if (team == null || !team.IsCurrentUserTeamMember())
            {
                return;
            }
            WBTermCollection<WBTerm> teamsFunctionalAreas = team.FunctionalArea(teams);

            String callingUserLogin = SPContext.Current.Web.CurrentUser.LoginName;
            if (true)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager(callingUserLogin))
                    {
                        WBRecord record = elevatedManager.Libraries.GetRecordByID(RecordID.Text);

                        // Let's just double check the permissions to edit:
                        WBTermCollection<WBTerm> recordsFunctionalAreas = record.FunctionalArea;
                        if (recordsFunctionalAreas != null && recordsFunctionalAreas.Count > 0)
                        {
                            WBTerm functionalArea = record.FunctionalArea[0];
                            if (!teamsFunctionalAreas.Contains(functionalArea))
                            {
                                throw new Exception("You are trying to edit a record (" + record.RecordID + ") which has a functional area (" + functionalArea.Name + ") that your team (" + team.Name + ") doesn't have permission to edit!");
                            }
                        }

                        if (RecordSeriesStatusChange.SelectedValue == "Retire")
                        {
                            if (record.RecordSeriesStatus == "Latest") record.RecordSeriesStatus = "Retired";
                        }
                        if (RecordSeriesStatusChange.SelectedValue == "Archive")
                        {
                            record.RecordSeriesStatus = "Archived";
                            record.LiveOrArchived = "Archived";
                        }

                        record.Title = RecordTitle.Text;
                        record.ProtectiveZone = ProtectiveZone.SelectedValue;
                        record.SubjectTagsUIControlValue = SubjectTags.Text;
                        record.OwningTeamUIControlValue = OwningTeam.Text;
                        record.InvolvedTeamsWithoutOwningTeamAsUIControlValue = InvolvedTeams.Text;

                        if (record.ProtectiveZone != WBRecordsType.PROTECTIVE_ZONE__PROTECTED && record.Metadata.IsNullOrEmpty(WBColumn.ReviewDate))
                        {
                            record[WBColumn.ReviewDate] = DateTime.Now.AddYears(2);
                        }

                        WBLogging.Debug("About to udpate with callingUser = " + callingUserLogin);

                        record.Update(callingUserLogin, ReasonForChange.Text);
                    }
                });

                CloseDialogAndRefresh();
            }
            else
            {
                returnFromDialogError("The security digest for the request was not OK");
            }
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Update to the record was cancelled.");
        }

    }
}
