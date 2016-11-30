using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class EditRecordsMetadata : WBDialogPageBase
    {
        private String currentUserLoginName = "";

        protected void Page_Load(object sender, EventArgs e)
        {

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
                        location = record.FunctionalArea[0].FullPath;

                        WBLogging.Debug("location = " + location);
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

                    ProtectiveZone.DataSource = new String[] { WBRecordsType.PROTECTIVE_ZONE__PROTECTED, WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET, WBRecordsType.PROTECTIVE_ZONE__PUBLIC };
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
            bool digestOK = SPContext.Current.Web.ValidateFormDigest();
            String callingUserLogin = SPContext.Current.Web.CurrentUser.LoginName;
            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager(callingUserLogin))
                    {
                        WBRecord record = elevatedManager.Libraries.GetRecordByID(RecordID.Text);

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
