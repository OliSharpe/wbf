using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class FiguresForOneMonth : DataSheetType
    {
        public DateTime Date;
        public DateTime NextDate;
        public List<WorkBoxDetails> WorkBoxes = new List<WorkBoxDetails>();

        public int numWBCreated = 0;
        public int numWBOpened = 0;
        public int numWBClosed = 0;
        public int numWBDeleted = 0;

        public int numWBLastModified = 0;
        public int numWBLastVisited = 0;

        public int totalDeleted = 0;
        public int totalToDelete = 0;
        public int totalDocsInWBToDelete = 0;
        public long totalSizeOfDocsInWBToDelete = 0;

        public int numWBInCreatedState = 0;
        public int numWBInOpenState = 0;
        public int numWBInClosedState = 0;
        public int numWBInDeletedState = 0;
        public int openMultiTeamsWBs = 0;
        public int totalMultiTeamsWBs = 0;
        public int numberOfWorkBoxes = 0;

        public int numDocsInOpenWBs = 0;
        public int numDocsInClosedWBs = 0;
        public int totalNumDocsInWBs = 0;

        public long sizeOfDocsInOpenWBs = 0;
        public long sizeOfDocsInClosedWBs = 0;
        public long totalSizeOfDocsInWBs = 0;

        public int numTeamsCreated = 0;
        public int numTotalTeams = 0;

        public List<String> owningTeams = new List<String>();
        public List<String> involvedTeams = new List<String>();
        public List<String> visitingTeams = new List<String>();
        public List<String> involvedIndividuals = new List<String>();
        public List<String> visitingIndividuals = new List<String>();

        public int lastVistedWBThisMonth = 0;
        public int visitedWBEver = 0;

        public int totalDocRecordsCreated = 0;
        public int publicDocRecordsCreated = 0;
        public int publicDocRecordsArchived = 0;

        public int totalEverDocRecords = 0;
        public int totalEverPublicRecords = 0;
        public int totalEverArchivedPublicRecords = 0;

        public long sizeOfAllDocRecords = 0;
        public long sizeOfAllLivePublicDocRecords = 0;
        public long sizeOfAllArchivedPublicDocRecords = 0;

        public FiguresForOneMonth()
        {
        }

        public FiguresForOneMonth(DateTime date)
        {
            Date = date;
            NextDate = Date.AddMonths(1);
        }


        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            // Nothing to be done here as configuration should be done by the ConfigureWithCategories for the DataSheet using this DataSheetType
            //throw new NotSupportedException("This method should never get called on the FiguresForOneDay class.");

            datasheet.AddDateColumn();

            datasheet.AddIntegerColumn("Num Created", 20);
            datasheet.AddIntegerColumn("Num Opened", 20);
            datasheet.AddIntegerColumn("Num Closed", 20);
            datasheet.AddIntegerColumn("Num Deleted", 20);

            datasheet.AddIntegerColumn("Last Modified", 20);
            datasheet.AddIntegerColumn("Last Visited", 20);

            datasheet.AddTextColumn(".", 5);
            datasheet.AddDateColumn("Date.");
            datasheet.AddIntegerColumn("Total WB Deleted", 20);
            datasheet.AddIntegerColumn("Num WB To Delete", 20);
            datasheet.AddIntegerColumn("Actually Deleted", 20);
            datasheet.AddIntegerColumn("Total Docs in WB To Delete", 20);
            datasheet.AddFileSizeGBColumn("Total Size of Docs in WB To Delete", 20);


            datasheet.AddTextColumn(" .", 5);
            datasheet.AddDateColumn("Date .");

            datasheet.AddIntegerColumn("In Created State", 20);
            datasheet.AddIntegerColumn("In Open State", 20);
            datasheet.AddIntegerColumn("In Closed State", 20);
            datasheet.AddIntegerColumn("In Deleted State", 20);
            datasheet.AddIntegerColumn("Open Multi Teams WBs", 20);
            datasheet.AddIntegerColumn("Total Multi Teams WBs", 20);
            datasheet.AddIntegerColumn("Total Work Boxes", 20);

            datasheet.AddTextColumn("  .", 5);
            datasheet.AddDateColumn("Date  .");

            datasheet.AddIntegerColumn("Total Docs in Open WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in All WBs", 20);

            datasheet.AddTextColumn("   .", 5);
            datasheet.AddDateColumn("Date   .");

            datasheet.AddFileSizeGBColumn("Size of Docs in Open WBs", 20);
            datasheet.AddFileSizeGBColumn("Size of Docs in Closed WBs", 20);
            datasheet.AddFileSizeGBColumn("Size of Docs in All WBs", 20);


            datasheet.AddTextColumn("    .", 5);
            datasheet.AddDateColumn("Date    .");

            datasheet.AddIntegerColumn("Teams Created", 20);
            datasheet.AddIntegerColumn("Total Teams", 20);
            datasheet.AddIntegerColumn("Teams Owning", 20);
            datasheet.AddIntegerColumn("Teams Involved", 20);
            datasheet.AddIntegerColumn("Teams Visiting", 20);
            datasheet.AddIntegerColumn("Individuals Involved", 20);
            datasheet.AddIntegerColumn("Individuals Visiting", 20);
            datasheet.AddIntegerColumn("Users Last Visited WB", 20);
            datasheet.AddIntegerColumn("Users Ever Visited WB", 20);


            datasheet.AddTextColumn("     .", 5);
            datasheet.AddDateColumn("Date     .");
            
            datasheet.AddIntegerColumn("Total Doc Records Created", 20);
            datasheet.AddIntegerColumn("Public Doc Records Created", 20);
            datasheet.AddIntegerColumn("Public Doc Records Archived", 20);

            datasheet.AddIntegerColumn("Total Ever Doc Records", 20);
            datasheet.AddIntegerColumn("Total Ever Public Records", 20);
            datasheet.AddIntegerColumn("Total Ever Archived Public Records", 20);

            datasheet.AddFileSizeGBColumn("Size Of All Doc Records", 20);
            datasheet.AddFileSizeGBColumn("Size Of All Live Public Doc Records", 20);
            datasheet.AddFileSizeGBColumn("Size Of All Archived Public Doc Records", 20);
        }

        internal void Add(TeamDetails team)
        {
            if (team.hasBeenCreated)
            {
                if (team.DateCreated >= this.Date && team.DateCreated < this.NextDate) numTeamsCreated++;
                if (team.DateCreated < this.NextDate) numTotalTeams++;
            }
        }

        internal void Add(UserDetails user)
        {
            if (user.VisitedAnyWB)
            {
                if (user.MostRecentVisitDate >= this.Date && user.MostRecentVisitDate < this.NextDate) lastVistedWBThisMonth++;
                if (user.OldestVisitDate < this.NextDate) visitedWBEver++;
            }
        }



        public void Add(WorkBoxDetails workBox)
        {
            bool isMultiTeamsWBs = false;

            // Working out just the planned deletions information - based on deleted date or retention end date for planned deletions
            if (workBox.hasBeenDeleted && workBox.DateDeleted < this.NextDate)
            {
                totalDeleted++;
            }
            else
            {
                if (workBox.willBeDeleted && workBox.DateToBeDeleted < this.NextDate)
                {
                    totalDeleted++;
                    if (workBox.FoundDocuments)
                    {
                        totalDocsInWBToDelete += workBox.NumDocuments;
                        totalSizeOfDocsInWBToDelete += workBox.TotalSizeOfFiles;
                    }
                    if (workBox.DateToBeDeleted >= this.Date) totalToDelete++;
                }
            }

            // We're only intersted to do the other calculations if the date for this month is this month or ealier.
            if (Date > DateTime.Now) return;

            // We're only interested to count work boxes that had been created within this month (between 'Date' and 'NextDate')
            if (workBox.hasBeenCreated && workBox.DateCreated < this.NextDate)
            {
                // WorkBoxes.Add(workBox);
                
                numberOfWorkBoxes++;

                if (workBox.hasBeenOpened)
                {
                    if (workBox.FoundDocuments) 
                    {
                        totalNumDocsInWBs += workBox.NumDocuments;
                        totalSizeOfDocsInWBs += workBox.TotalSizeOfFiles;
                    }


                    if (!String.IsNullOrEmpty(workBox.OwningTeam))
                    {
                        if (!owningTeams.Contains(workBox.OwningTeam))
                        {
                            owningTeams.Add(workBox.OwningTeam);
                        }
                    }

                    if (!String.IsNullOrEmpty(workBox.InvolvedTeams))
                    {
                        String[] teams = workBox.InvolvedTeams.Split(';');
                        
                        foreach (string team in teams)
                        {
                            if (!team.Equals(workBox.OwningTeam))
                            {
                                isMultiTeamsWBs = true;

                                if (!involvedTeams.Contains(team))
                                {
                                    involvedTeams.Add(team);
                                }
                            }
                        }
                        if (isMultiTeamsWBs) totalMultiTeamsWBs++;
                    }

                    if (!String.IsNullOrEmpty(workBox.VisitingTeams))
                    {
                        String[] teams = workBox.VisitingTeams.Split(';');
                        foreach (string team in teams)
                        {
                            if (!team.Equals(workBox.OwningTeam))
                            {
                                if (!visitingTeams.Contains(team))
                                {
                                    visitingTeams.Add(team);
                                }
                            }
                        }

                    }

                    if (!String.IsNullOrEmpty(workBox.InvolvedIndividuals))
                    {
                        String[] users = workBox.InvolvedIndividuals.Split(';');
                        foreach (string user in users)
                        {
                            if (!involvedIndividuals.Contains(user))
                            {
                                involvedIndividuals.Add(user);
                            }
                        }

                    }

                    if (!String.IsNullOrEmpty(workBox.VisitingIndividuals))
                    {
                        String[] users = workBox.VisitingIndividuals.Split(';');
                        foreach (string user in users)
                        {
                            if (!visitingIndividuals.Contains(user))
                            {
                                visitingIndividuals.Add(user);
                            }
                        }

                    }
                }


                if (workBox.hasBeenCreated && workBox.DateCreated >= this.Date && workBox.DateCreated < this.NextDate)
                {
                    numWBCreated++;
                }

                if (workBox.hasBeenOpened && workBox.DateOpened >= this.Date && workBox.DateOpened < this.NextDate)
                {
                    numWBOpened++;
                }

                if (workBox.hasBeenClosed && workBox.DateClosed >= this.Date && workBox.DateClosed < this.NextDate)
                {
                    numWBClosed++;
                }

                if (workBox.hasBeenDeleted && workBox.DateDeleted >= this.Date && workBox.DateDeleted < this.NextDate)
                {
                    numWBDeleted++;
                }

                if (workBox.hasBeenModified && workBox.DateLastModified >= this.Date && workBox.DateLastModified < this.NextDate)
                {
                    numWBLastModified++;
                }

                if (workBox.hasBeenVisited && workBox.DateLastVisited >= this.Date && workBox.DateLastVisited < this.NextDate)
                {
                    numWBLastVisited++;
                }

                // Note we're looking for the last state of the work box in the month between 'Date' and 'NextDate':
                if (workBox.hasBeenDeleted && workBox.DateDeleted < this.NextDate)
                {
                    numWBInDeletedState++;

                }
                else
                {
                    if (workBox.hasBeenClosed && workBox.DateClosed < this.NextDate)
                    {
                        numWBInClosedState++;
                        if (workBox.FoundDocuments)
                        {
                            numDocsInClosedWBs += workBox.NumDocuments;
                            sizeOfDocsInClosedWBs += workBox.TotalSizeOfFiles;
                        }

                    }
                    else
                    {
                        if (workBox.hasBeenOpened && workBox.DateOpened < this.NextDate)
                        {
                            numWBInOpenState++;
                            if (workBox.FoundDocuments)
                            {
                                numDocsInOpenWBs += workBox.NumDocuments;
                                sizeOfDocsInOpenWBs += workBox.TotalSizeOfFiles;
                            }
                            if (isMultiTeamsWBs)
                            {
                                openMultiTeamsWBs++;
                            }
                        }
                        else
                        {
                            if (workBox.hasBeenCreated && workBox.DateCreated < this.NextDate)
                            {
                                numWBInCreatedState++;
                            }
                        }
                    }
                }

            }
        }



        internal void Add(RecordsDetails record)
        {
            if (record.hasBeenCreated)
            {
                bool isPublicZone = "Public".Equals(record["Protective Zone"]);

                if (record.DateCreated >= this.Date && record.DateCreated < this.NextDate)
                {
                    this.totalDocRecordsCreated++;
                    if (isPublicZone)
                    {
                        publicDocRecordsCreated++;
                    }
                }

                if (record.IsArchived && record.hasBeenModified && record.DateLastModified >= this.Date && record.DateLastModified < this.NextDate)
                {
                    if ((record.DateLastModified > record.DateCreated.AddDays(1)) && isPublicZone)
                    {
                        publicDocRecordsArchived++;
                    }
                }

                if (record.DateCreated < this.NextDate)
                {
                    totalEverDocRecords++;
                    sizeOfAllDocRecords += record.FileSize; 
                    if (isPublicZone)
                    {
                        totalEverPublicRecords++;

                        if (record.DateLastModified <= this.NextDate && record.IsArchived)
                        {
                            sizeOfAllArchivedPublicDocRecords += record.FileSize; 
                        }
                        else
                        {
                            sizeOfAllLivePublicDocRecords += record.FileSize; 
                        }
                    }
                }

                if (record.DateLastModified < this.NextDate && record.IsArchived && isPublicZone)
                {
                    totalEverArchivedPublicRecords++;
                }
            }
        }


        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            // Doing nothing as this is a write only sheet
            // Date = worksheet.xGetCellAsDateTime(rowIndex, "A");
        }

        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Columns = datasheet.Columns;

            Columns["Date."][rowIndex] = Date;
            Columns["Total WB Deleted"][rowIndex] = totalDeleted;
            Columns["Num WB To Delete"][rowIndex] = totalToDelete;

            Columns["Total Docs in WB To Delete"][rowIndex] = totalDocsInWBToDelete;
            Columns["Total Size of Docs in WB To Delete"][rowIndex] = totalSizeOfDocsInWBToDelete;


            // We'll only output the rest for the historical dates up until now.
            if (Date > DateTime.Now) return;

            Columns["Actually Deleted"][rowIndex] = numWBInDeletedState;


            Columns["Date"][rowIndex] = Date;
            Columns["Num Created"][rowIndex] = numWBCreated;
            Columns["Num Opened"][rowIndex] = numWBOpened;
            Columns["Num Closed"][rowIndex] = numWBClosed;
            Columns["Num Deleted"][rowIndex] = numWBDeleted;

            Columns["Last Modified"][rowIndex] = numWBLastModified;
            Columns["Last Visited"][rowIndex] = numWBLastVisited;

            Columns["Date ."][rowIndex] = Date;
            Columns["In Created State"][rowIndex] = numWBInCreatedState;
            Columns["In Open State"][rowIndex] = numWBInOpenState;
            Columns["In Closed State"][rowIndex] = numWBInClosedState;
            Columns["In Deleted State"][rowIndex] = numWBInDeletedState;
            Columns["Open Multi Teams WBs"][rowIndex] = openMultiTeamsWBs;
            Columns["Total Multi Teams WBs"][rowIndex] = totalMultiTeamsWBs;
            Columns["Total Work Boxes"][rowIndex] = numberOfWorkBoxes;

            Columns["Date  ."][rowIndex] = Date;
            Columns["Total Docs in Open WBs"][rowIndex] = numDocsInOpenWBs;
            Columns["Total Docs in Closed WBs"][rowIndex] = numDocsInClosedWBs;
            Columns["Total Docs in All WBs"][rowIndex] = totalNumDocsInWBs;

            Columns["Date   ."][rowIndex] = Date;
            Columns["Size of Docs in Open WBs"][rowIndex] = sizeOfDocsInOpenWBs;
            Columns["Size of Docs in Closed WBs"][rowIndex] = sizeOfDocsInClosedWBs;
            Columns["Size of Docs in All WBs"][rowIndex] = totalSizeOfDocsInWBs;


            Columns["Date    ."][rowIndex] = Date;
            Columns["Teams Created"][rowIndex] = numTeamsCreated;
            Columns["Total Teams"][rowIndex] = numTotalTeams;
            Columns["Teams Owning"][rowIndex] = owningTeams.Count;
            Columns["Teams Involved"][rowIndex] = involvedTeams.Count;
            Columns["Teams Visiting"][rowIndex] = visitingTeams.Count;
            Columns["Individuals Involved"][rowIndex] = involvedIndividuals.Count;
            Columns["Individuals Visiting"][rowIndex] = visitingIndividuals.Count;

            Columns["Users Last Visited WB"][rowIndex] = lastVistedWBThisMonth;
            Columns["Users Ever Visited WB"][rowIndex] = visitedWBEver;


            Columns["Total Doc Records Created"][rowIndex] = totalDocRecordsCreated;
            Columns["Public Doc Records Created"][rowIndex] = publicDocRecordsCreated;
            Columns["Public Doc Records Archived"][rowIndex] = publicDocRecordsArchived;

            Columns["Total Ever Doc Records"][rowIndex] = totalEverDocRecords; 
            Columns["Total Ever Public Records"][rowIndex] = totalEverPublicRecords;
            Columns["Total Ever Archived Public Records"][rowIndex] = totalEverArchivedPublicRecords; 
            
            Columns["Size Of All Doc Records"][rowIndex] = sizeOfAllDocRecords; 
            Columns["Size Of All Live Public Doc Records"][rowIndex] = sizeOfAllLivePublicDocRecords;
            Columns["Size Of All Archived Public Doc Records"][rowIndex] = sizeOfAllArchivedPublicDocRecords; 

        }

        public override String Key()
        {
            return Date.Month + " " + Date.Year;
        }



    }
}