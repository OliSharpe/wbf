using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    class FiguresForFunctionalArea : DataSheetType
    {
        public String FunctionalArea;
        public List<WorkBoxDetails> WorkBoxes = new List<WorkBoxDetails>();

        public int totalTeams = 0;

        public int numberOfEntries = 0;

        public int totalOpenWBs = 0;
        public int totalClosedWBs = 0;
        public int totalWorkBoxes = 0;

        public int totalDocsInOpenWBs = 0;
        public int totalDocsInClosedWBs = 0;
        public int totalDocsInAllWBs = 0;

        public long SizeOfDocsInOpenWBs = 0;
        public long SizeOfDocsInClosedWBs = 0;
        public long SizeOfDocsInAllWBs = 0;


        public int totalDocRecords = 0;
        public long totalDocRecordsSize = 0;

        public FiguresForFunctionalArea()
        {
        }

        public FiguresForFunctionalArea(String functionalArea)
        {
            FunctionalArea = functionalArea;
        }


        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {

            datasheet.AddTextColumn("Functional Area", 30);

            datasheet.AddIntegerColumn("Total Teams", 20);


            datasheet.AddIntegerColumn("Total Entries", 20);

            datasheet.AddIntegerColumn("Total Open WBs", 20);
            datasheet.AddIntegerColumn("Total Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Work Boxes", 20);

            datasheet.AddIntegerColumn("Total Docs in Open WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in All WBs", 20);

            datasheet.AddFileSizeMBColumn("Size of Docs in Open WBs", 20);
            datasheet.AddFileSizeMBColumn("Size of Docs in Closed WBs", 20);
            datasheet.AddFileSizeMBColumn("Size of Docs in All WBs", 20);

            datasheet.AddIntegerColumn("Total Doc Records", 20);
            datasheet.AddFileSizeMBColumn("Total Doc Records Size", 20);

        }

        public void Add(TeamDetails team)
        {
            if (team.FunctionalArea.Equals(this.FunctionalArea))
            {
                totalTeams++;
            }
        }

        public void Add(WorkBoxDetails workBox)
        {
            if (workBox.FunctionalArea.Equals(this.FunctionalArea))
            {
                numberOfEntries++;
                if (!String.IsNullOrEmpty(workBox.FunctionalArea))
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

        public void Add(RecordsDetails record)
        {
            if (record.FunctionalArea.Equals(this.FunctionalArea))
            {
                totalDocRecords++;
                totalDocRecordsSize += record.FileSize;                    
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

            Columns["Functional Area"][rowIndex] = FunctionalArea;

            Columns["Total Teams"][rowIndex] = totalTeams;
            
            Columns["Total Entries"][rowIndex] = numberOfEntries;

            Columns["Total Open WBs"][rowIndex] = totalOpenWBs;
            Columns["Total Closed WBs"][rowIndex] = totalClosedWBs;
            Columns["Total Work Boxes"][rowIndex] = totalWorkBoxes;
            
            Columns["Total Docs in Open WBs"][rowIndex] = totalDocsInOpenWBs;
            Columns["Total Docs in Closed WBs"][rowIndex] = totalDocsInClosedWBs;
            Columns["Total Docs in All WBs"][rowIndex] = totalDocsInAllWBs;

            Columns["Size of Docs in Open WBs"][rowIndex] = SizeOfDocsInOpenWBs;
            Columns["Size of Docs in Closed WBs"][rowIndex] = SizeOfDocsInClosedWBs;
            Columns["Size of Docs in All WBs"][rowIndex] = SizeOfDocsInAllWBs;

            Columns["Total Doc Records"][rowIndex] = totalDocRecords;
            Columns["Total Doc Records Size"][rowIndex] = totalDocRecordsSize;

        }

        public override String Key()
        {
            return FunctionalArea;
        }
    }
}
