using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class FiguresForOneRecordsType : DataSheetType
    {
        public String RecordsType;

        RecordsTypesDetails Details = null;

        public String WBAutoCloseRule = "";
        public String WBRetentionRule = "";

        public int totalOpenWBs = 0;
        public int totalClosedWBs = 0;
        public int totalDeletedWBs = 0;
        public int totalWorkBoxes = 0;

        public DateTime DateFirstUsed = new DateTime(2100, 1, 1);
        public DateTime DateLastUsed = new DateTime(2000, 1, 1);

        public int totalDocsInOpenWBs = 0;
        public int totalDocsInClosedWBs = 0;
        public int totalDocsInAllWBs = 0;

        public long SizeOfDocsInOpenWBs = 0;
        public long SizeOfDocsInClosedWBs = 0;
        public long SizeOfDocsInAllWBs = 0;

        public int totalDocumentRecords = 0;
        public long totalSizeOfDocRecords = 0;

        public FiguresForOneRecordsType()
        {
        }

        public FiguresForOneRecordsType(String recordsType)
        {
            RecordsType = recordsType;
        }


        public void SetRecordsTypeDetails(RecordsTypesDetails recordsTypeDetails)
        {
            Details = recordsTypeDetails;
            if (Details == null) return;

            if (!String.IsNullOrEmpty(Details["Auto-Close Trigger Date"] as String))
            {
                WBAutoCloseRule = "" + Details["Auto-Close Time Scalar"] + " "+  Details["Auto-Close Time Unit"] + " after " + Details["Auto-Close Trigger Date"];
            }
            if (!String.IsNullOrEmpty(Details["Retention Trigger Date"] as String))
            {
                WBRetentionRule = "" + Details["Retention Time Scalar"] + " " + Details["Retention Time Unit"] + " after " + Details["Retention Trigger Date"];
            }
        }

        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            // Nothing to be done here as configuration should be done by the ConfigureWithCategories for the DataSheet using this DataSheetType
            //throw new NotSupportedException("This method should never get called on the FiguresForOneDay class.");

            datasheet.AddTextColumn("Records Type", 30);

            datasheet.AddTextColumn("WB Auto-Close Rule", 30);
            datasheet.AddTextColumn("WB Retention Rule", 30);

            datasheet.AddIntegerColumn("Total Open WBs", 20);
            datasheet.AddIntegerColumn("Total Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Deleted WBs", 20);
            datasheet.AddIntegerColumn("Total Work Boxes", 20);

            datasheet.AddDateColumn("Date First Used");
            datasheet.AddDateColumn("Date Last Used");

            datasheet.AddIntegerColumn("Total Docs in Open WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in Closed WBs", 20);
            datasheet.AddIntegerColumn("Total Docs in All WBs", 20);

            datasheet.AddFileSizeMBColumn("Size of Docs in Open WBs", 20);
            datasheet.AddFileSizeMBColumn("Size of Docs in Closed WBs", 20);
            datasheet.AddFileSizeMBColumn("Size of Docs in All WBs", 20);

            datasheet.AddIntegerColumn("Total Document Records", 20);
            datasheet.AddFileSizeMBColumn("Total Size of Doc Records", 20);
        }

        public void Add(WorkBoxDetails workBox)
        {
            // We're only interested to count work boxes that have been opened and that use the given records type:
            if (workBox.hasBeenOpened && RecordsType.Equals(workBox.RecordsType))
            {
                totalWorkBoxes++;
                if (workBox.IsOpen) totalOpenWBs++;
                if (workBox.IsClosed) totalClosedWBs++;
                if (workBox.IsDeleted) totalDeletedWBs++;

                if (DateFirstUsed > workBox.DateOpened) DateFirstUsed = workBox.DateOpened;
                if (DateLastUsed < workBox.DateOpened) DateLastUsed = workBox.DateOpened;

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
            // We're only interested to count work boxes that have been opened and that use the given records type:
            if (RecordsType.Equals(record.RecordsType))
            {
                totalDocumentRecords++;
                totalSizeOfDocRecords += record.FileSize;
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
            Columns["Records Type"][rowIndex] = RecordsType;

            Columns["WB Auto-Close Rule"][rowIndex] = WBAutoCloseRule;
            Columns["WB Retention Rule"][rowIndex] = WBRetentionRule;

            Columns["Total Open WBs"][rowIndex] = totalOpenWBs;
            Columns["Total Closed WBs"][rowIndex] = totalClosedWBs;
            Columns["Total Deleted WBs"][rowIndex] = totalDeletedWBs;
            Columns["Total Work Boxes"][rowIndex] = totalWorkBoxes;

            if (totalWorkBoxes > 0)
            {
                Columns["Date First Used"][rowIndex] = DateFirstUsed;
                Columns["Date Last Used"][rowIndex] = DateLastUsed;
            }
            else
            {
                Columns["Date First Used"][rowIndex] = null;
                Columns["Date Last Used"][rowIndex] = null;
            }

            Columns["Total Docs in Open WBs"][rowIndex] = totalDocsInOpenWBs;
            Columns["Total Docs in Closed WBs"][rowIndex] = totalDocsInClosedWBs;
            Columns["Total Docs in All WBs"][rowIndex] = totalDocsInAllWBs;

            Columns["Size of Docs in Open WBs"][rowIndex] = SizeOfDocsInOpenWBs;
            Columns["Size of Docs in Closed WBs"][rowIndex] = SizeOfDocsInClosedWBs;
            Columns["Size of Docs in All WBs"][rowIndex] = SizeOfDocsInAllWBs;


            Columns["Total Document Records"][rowIndex] = totalDocumentRecords;
            Columns["Total Size of Doc Records"][rowIndex] = totalSizeOfDocRecords;

        }

        public override String Key()
        {
            return RecordsType;
        }

    }
}

