using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class FiguresForWBC : DataSheetType
    {
        public String WorkBoxCollectionURL;
        public List<WorkBoxDetails> WorkBoxes = new List<WorkBoxDetails>();

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

        public FiguresForWBC()
        {
        }

        public FiguresForWBC(String workBoxCollectionURL)
        {
            WorkBoxCollectionURL = workBoxCollectionURL;
        }


        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {

            datasheet.AddTextColumn("Work Box Collection URL", 40);

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

        }

        public void Add(WorkBoxDetails workBox)
        {


            if (workBox.WorkBoxCollectionURL.Equals(this.WorkBoxCollectionURL))
            {
                numberOfEntries++;
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


        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            // Doing nothing as this is a write only sheet
            // Date = worksheet.xGetCellAsDateTime(rowIndex, "A");
        }

        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Columns = datasheet.Columns;

            Columns["Work Box Collection URL"][rowIndex] = WorkBoxCollectionURL;
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
        }

        public override String Key()
        {
            return WorkBoxCollectionURL;
        }
    }
}
