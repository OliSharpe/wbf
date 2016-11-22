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

                using (WBRecordsManager manager = new WBRecordsManager())
                {
                    WBRecord record = manager.Libraries.GetRecordByID(RecordID.Text);

                    Filename.Text = record.Name;
                    Title.Text = record.Title;

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

                    RecordSeriesStatus.Text = record.RecordSeriesStatus;

                    RecordSeriesStatusChange.DataSource = new String[] { "", "Retire", "Archive" };
                    RecordSeriesStatusChange.DataBind();
                    RecordSeriesStatusChange.SelectedValue = "";

                    ProtectiveZone.DataSource = WBRecordsType.getProtectiveZones();
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

            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager elevatedManager = new WBRecordsManager())
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

                        record.ProtectiveZone = ProtectiveZone.SelectedValue;
                        record.SubjectTagsUIControlValue = SubjectTags.Text;
                        record.OwningTeamUIControlValue = OwningTeam.Text;
                        record.InvolvedTeamsWithoutOwningTeamAsUIControlValue = InvolvedTeams.Text;

                        record.Update();
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
