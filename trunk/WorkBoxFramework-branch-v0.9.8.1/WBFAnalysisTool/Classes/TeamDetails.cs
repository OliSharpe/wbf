using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class TeamDetails : DataSheetType
    {
        public String Error;
        public String ExceptionDetails;
        public String TeamName;
        public String PathToTeam;
        public String FunctionalArea;
        public String TeamSiteURL;
        public bool HasSitePagesInURL = false;
        public bool hasBeenCreated = false;
        public DateTime DateCreated;
        public String RecordsTypesListURL;
        public int NumOwners;
        public int NumMembers;
        public String TeamManager;
        public String TeamOwners;
        public String TeamMembers;
        public String TeamOwnersEmails;
        public String TeamMembersEmails;
        public String TeamOwnersSPGroupName;
        public String TeamMembersSPGroupName;
        public String PublicPublishersSPGroupName;
        public String PublicPublishers;
        public int NumUsersThisAndBelow;
        public String AllEmailsThisAndBelow;

        public int totalOpenWBs = 0;
        public int totalClosedWBs = 0;
        public int totalWorkBoxes = 0;

        public int totalDocsInOpenWBs = 0;
        public int totalDocsInClosedWBs = 0;
        public int totalDocsInAllWBs = 0;

        public long SizeOfDocsInOpenWBs = 0;
        public long SizeOfDocsInClosedWBs = 0;
        public long SizeOfDocsInAllWBs = 0;

        public bool newTeamFound = false;

        public TeamDetails()
        {
        }

        public TeamDetails(String pathToTeam)
        {
            PathToTeam = pathToTeam;
            newTeamFound = true;
        }



        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            datasheet.AddTextColumn("ERROR?", 23);
            datasheet.AddTextColumn("Exception Details", 14);
            datasheet.AddTextColumn("Team Name", 20);
            datasheet.AddTextColumn("Path To Team", 20);
            datasheet.AddTextColumn("Functional Area", 20);
            datasheet.AddTextColumn("Team site URL", 20);
            datasheet.AddTextColumn("Has SitePages in URL", 10);
            datasheet.AddDateColumn("Date Created");
            datasheet.AddTextColumn("Records Types List URL", 20);
            datasheet.AddIntegerColumn("Num Owners", 20);
            datasheet.AddIntegerColumn("Num Members", 20);
            datasheet.AddTextColumn("Team Manager", 20);
            datasheet.AddTextColumn("Team Owners", 20);
            datasheet.AddTextColumn("Team Members", 20);
            datasheet.AddTextColumn("Team Owners Emails", 20);
            datasheet.AddTextColumn("Team Members Emails", 20);
            datasheet.AddTextColumn("Team Owners SPGroup Name", 20);
            datasheet.AddTextColumn("Team Members SPGroup Name", 20);
            datasheet.AddTextColumn("Public Publishers SPGroup Name", 20);
            datasheet.AddTextColumn("Public Publishers", 20);
            datasheet.AddIntegerColumn("Num users this team and below", 20);
            datasheet.AddTextColumn("All emails for this team and below", 20);

            datasheet.AddIntegerColumn("Total Open WBs", 20);
            datasheet.AddIntegerColumn("Total Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Work Boxes", 20);

            datasheet.AddIntegerColumn("Total Docs in Open WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in All WBs", 20);

            datasheet.AddFileSizeMBColumn("Size of Docs in Open WBs", 20);
            datasheet.AddFileSizeMBColumn("Size of Docs in Closed WBs", 20);
            datasheet.AddFileSizeMBColumn("Size of Docs in All WBs", 20);


        }

        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {

            Error = (String)datasheet.Columns["ERROR?"][rowIndex];
            ExceptionDetails = (String)datasheet.Columns["Exception Details"][rowIndex];
            TeamName = (String)datasheet.Columns["Team Name"][rowIndex];
            PathToTeam = (String)datasheet.Columns["Path To Team"][rowIndex];
            FunctionalArea = (String)datasheet.Columns["Functional Area"][rowIndex];
            TeamSiteURL = (String)datasheet.Columns["Team site URL"][rowIndex];
            if (datasheet.Columns["Date Created"].HasValue(rowIndex))
            {
                hasBeenCreated = true;
                DateCreated = (DateTime)datasheet.Columns["Date Created"][rowIndex];
            }

            RecordsTypesListURL = (String)datasheet.Columns["Records Types List URL"][rowIndex];
            NumOwners = (int)datasheet.Columns["Num Owners"][rowIndex];
            NumMembers = (int)datasheet.Columns["Num Members"][rowIndex];
            TeamManager = (String)datasheet.Columns["Team Manager"][rowIndex];
            TeamOwners = (String)datasheet.Columns["Team Owners"][rowIndex];
            TeamMembers = (String)datasheet.Columns["Team Members"][rowIndex];
            TeamOwnersEmails = (String)datasheet.Columns["Team Owners Emails"][rowIndex];
            TeamMembersEmails = (String)datasheet.Columns["Team Members Emails"][rowIndex];
            TeamOwnersSPGroupName = (String)datasheet.Columns["Team Owners SPGroup Name"][rowIndex];
            TeamMembersSPGroupName = (String)datasheet.Columns["Team Members SPGroup Name"][rowIndex];
            PublicPublishersSPGroupName = (String)datasheet.Columns["Public Publishers SPGroup Name"][rowIndex];
            PublicPublishers = (String)datasheet.Columns["Public Publishers"][rowIndex];
            NumUsersThisAndBelow = (int)datasheet.Columns["Num users this team and below"][rowIndex];
            AllEmailsThisAndBelow = (String)datasheet.Columns["All emails for this team and below"][rowIndex];

        }


        public void Add(WorkBoxDetails workBox)
        {
            if (this.PathToTeam.Equals(workBox.OwningTeam))
            {
                if (!String.IsNullOrEmpty(workBox.WorkBoxURL))
                {
                    totalWorkBoxes++;
                    if (workBox.IsOpen) totalOpenWBs++;
                    if (workBox.IsClosed) totalClosedWBs++;
                }
                if (workBox.FoundDocuments)
                {
                    if (workBox.IsOpen)
                    {
                        totalDocsInOpenWBs += workBox.NumDocuments;
                        SizeOfDocsInOpenWBs += workBox.TotalSizeOfFiles;
                    }

                    if (workBox.IsClosed)
                    {
                        totalDocsInClosedWBs += workBox.NumDocuments;
                        SizeOfDocsInClosedWBs += workBox.TotalSizeOfFiles;
                    }

                    totalDocsInAllWBs += workBox.NumDocuments;
                    SizeOfDocsInAllWBs += workBox.TotalSizeOfFiles;
                }
            }

        }


        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Columns = datasheet.Columns;

            if (newTeamFound)
            {
                Columns["Path To Team"][rowIndex] = PathToTeam;
            }

            Columns["Total Open WBs"][rowIndex] = totalOpenWBs;
            Columns["Total Closed WBs"][rowIndex] = totalClosedWBs;
            Columns["Total Work Boxes"][rowIndex] = totalWorkBoxes;

            Columns["Total Docs in Open WBs"][rowIndex] = totalDocsInOpenWBs;
            Columns["Total Docs in Closed WBs"][rowIndex] = totalDocsInClosedWBs;
            Columns["Total Docs in All WBs"][rowIndex] = totalDocsInAllWBs;

            Columns["Size of Docs in Open WBs"][rowIndex] = SizeOfDocsInOpenWBs;
            Columns["Size of Docs in Closed WBs"][rowIndex] = SizeOfDocsInClosedWBs;
            Columns["Size of Docs in All WBs"][rowIndex] = SizeOfDocsInAllWBs;

        }

        public override String Key()
        {
            return PathToTeam;
        }

    }
}
