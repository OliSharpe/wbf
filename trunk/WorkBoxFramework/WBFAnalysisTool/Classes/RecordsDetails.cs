using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class RecordsDetails : DataSheetType
    {
        public String Name = "";
        public String AbsoluteURL = "";

        public String FunctionalArea = "";
        public String RecordsType = "";

        public bool hasBeenCreated = false;
        public DateTime DateCreated;

        public bool hasBeenModified = false;
        public DateTime DateLastModified;

        public bool hasBeenDeclared = false;
        public DateTime DateDeclared;

        public bool IsArchived = false;

        public long FileSize = 0;

        public RecordsDetails()
        {
        }


        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            datasheet.AddTextColumn("ERROR?", 20);
            datasheet.AddTextColumn("Exception Details", 20);
            datasheet.AddTextColumn("ID", 20);
            datasheet.AddTextColumn("Name", 20);
            datasheet.AddTextColumn("Title", 20);
            datasheet.AddTextColumn("Absolute URL", 20);
            datasheet.AddTextColumn("Functional Area", 20);
            datasheet.AddTextColumn("Records Type", 20);
            datasheet.AddTextColumn("Subject Tags", 20);
            datasheet.AddTextColumn("Reference ID", 20);
            datasheet.AddDateColumn("Reference Date");
            datasheet.AddDateColumn("Scan Date");
            datasheet.AddTextColumn("Series Tag", 20);
            datasheet.AddTextColumn("Owning Team", 20);
            datasheet.AddTextColumn("Involved Teams", 20);
            datasheet.AddTextColumn("Protective Zone", 20);
            datasheet.AddTextColumn("Original Filename", 20);
            datasheet.AddTextColumn("Source System", 20);
            datasheet.AddTextColumn("Source ID", 20);
            datasheet.AddTextColumn("Record ID", 20);
            datasheet.AddTextColumn("Live?", 20);
            datasheet.AddDateColumn("Date Created");
            datasheet.AddDateColumn("Date Last Modified");
            datasheet.AddDateColumn("Date Declared");
            datasheet.AddLongColumn("File Size in Bytes", 10);
        }


        public Object this[String title]
        {
            get
            {
                if (RowIndex == -1 || DataSheet == null) return null;

                DataSheet<RecordsDetails> sheet = (DataSheet<RecordsDetails>)DataSheet;    

                return sheet.Columns[title][RowIndex];
            }
        }

        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Name = (String)datasheet.Columns["Name"][rowIndex];
            AbsoluteURL = (String)datasheet.Columns["Absolute URL"][rowIndex];

            FunctionalArea = (String)datasheet.Columns["Functional Area"][rowIndex];
            RecordsType = (String)datasheet.Columns["Records Type"][rowIndex];

            if (datasheet.Columns["Date Created"].HasValue(rowIndex))
            {
                hasBeenCreated = true;
                DateCreated = (DateTime)datasheet.Columns["Date Created"][rowIndex];
            }

            if (datasheet.Columns["Date Last Modified"].HasValue(rowIndex))
            {
                hasBeenModified = true;
                DateLastModified = (DateTime)datasheet.Columns["Date Last Modified"][rowIndex];
            }

            if (datasheet.Columns["Date Declared"].HasValue(rowIndex))
            {
                hasBeenDeclared = true;
                DateLastModified = (DateTime)datasheet.Columns["Date Declared"][rowIndex];
            }

            if (datasheet.Columns["File Size in Bytes"].HasValue(rowIndex))
            {
                FileSize = (long)datasheet.Columns["File Size in Bytes"][rowIndex];
            }

            if (datasheet.Columns["Live?"].HasValue(rowIndex))
            {
                IsArchived = "Archived".Equals((String)datasheet.Columns["Live?"][rowIndex]);
            }

            
        }



        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Columns = datasheet.Columns;

        }

        public override String Key()
        {
            return AbsoluteURL;
        }

    }
}
