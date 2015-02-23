using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class RecordsTypesDetails : DataSheetType
    {
        public String Name = "";
        public String FullPath = "";

        public bool hasBeenCreated = false;
        public DateTime DateCreated;

        public bool hasBeenModified = false;
        public DateTime DateLastModified;

        public RecordsTypesDetails()
        {
        }


        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            datasheet.AddTextColumn("ERROR?", 20);
            datasheet.AddTextColumn("Exception Details", 20);

            datasheet.AddTextColumn("Records Type Name", 20);
            datasheet.AddTextColumn("Records Type Full Path", 20);
            datasheet.AddTextColumn("Description", 20);
            datasheet.AddTextColumn("Default Functional Area", 20);
            datasheet.AddBooleanColumn("Allow Other Functional Areas");

            datasheet.AddDateColumn("Date Created");
            datasheet.AddDateColumn("Date Last Modified");

            datasheet.AddBooleanColumn("Allow Work Box Records");
            datasheet.AddTextColumn("Work Box Collection URL", 20);

            datasheet.AddTextColumn("Who Can Create New Work Boxes", 20);
            datasheet.AddTextColumn("Create New Work Box Text", 20);

            datasheet.AddTextColumn("Work Box Unique ID Prefix", 20);
            datasheet.AddTextColumn("Work Box Local ID Source", 20);
            datasheet.AddIntegerColumn("Work Box Generated Local ID Offset", 20);

            datasheet.AddTextColumn("Work Box Short Title Requirement", 20);
            datasheet.AddTextColumn("Work Box Short Title Description", 20);
            datasheet.AddTextColumn("Work Box Reference ID Requirement", 20);
            datasheet.AddTextColumn("Work Box Reference ID Description", 20);
            datasheet.AddTextColumn("Work Box Reference Date Requirement", 20);
            datasheet.AddTextColumn("Work Box Reference Date Description", 20);
            datasheet.AddTextColumn("Work Box Series Tag Requirement", 20);
            datasheet.AddTextColumn("Work Box Series Tag Description", 20);
            datasheet.AddTextColumn("Work Box Series Tag Parent Term", 20);
            datasheet.AddTextColumn("Work Box Series Tag Allow New Terms", 20);

            datasheet.AddTextColumn("Work Box Naming Convention", 20);

            datasheet.AddIntegerColumn("Auto-Close Time Scalar", 20);
            datasheet.AddTextColumn("Auto-Close Time Unit", 20);
            datasheet.AddTextColumn("Auto-Close Trigger Date", 20);

            datasheet.AddIntegerColumn("Retention Time Scalar", 20);
            datasheet.AddTextColumn("Retention Time Unit", 20);
            datasheet.AddTextColumn("Retention Trigger Date", 20);


            datasheet.AddBooleanColumn("Allow Publishing Out");
            datasheet.AddTextColumn("Minimum Publishing Out Protective Zone", 20);

            datasheet.AddTextColumn("Generate Publish Out Filenames", 20);
            datasheet.AddTextColumn("Use Defaults When Publishing Out", 20);
            datasheet.AddTextColumn("Default Publishing Out Records Type", 20);
            datasheet.AddTextColumn("Cache Details For Open Work Boxes", 20);

            datasheet.AddBooleanColumn("Allow Document Records");
            datasheet.AddTextColumn("Document Minimum Protective Zone", 20);

            datasheet.AddTextColumn("Document Reference ID Requirement", 20);
            datasheet.AddTextColumn("Document Reference ID Description", 20);
            datasheet.AddTextColumn("Document Reference Date Requirement", 20);
            datasheet.AddTextColumn("Document Reference Date Source", 20);
            datasheet.AddTextColumn("Document Reference Date Description", 20);
            datasheet.AddTextColumn("Document Series Tag Requirement", 20);
            datasheet.AddTextColumn("Document Series Tag Description", 20);
            datasheet.AddTextColumn("Document Scan Date Requirement", 20);
            datasheet.AddTextColumn("Document Scan Date Description", 20);

            datasheet.AddTextColumn("Document Naming Convention", 20);
            datasheet.AddBooleanColumn("Enforce Document Naming Convention");

            datasheet.AddTextColumn("Filing Rule Level 1", 20);
            datasheet.AddTextColumn("Filing Rule Level 2", 20);
            datasheet.AddTextColumn("Filing Rule Level 3", 20);
            datasheet.AddTextColumn("Filing Rule Level 4", 20);
        }


        public Object this[String title]
        {
            get
            {
                if (RowIndex == -1 || DataSheet == null) return null;

                DataSheet<RecordsTypesDetails> sheet = (DataSheet<RecordsTypesDetails>)DataSheet;    

                return sheet.Columns[title][RowIndex];
            }
        }

        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Name = (String)datasheet.Columns["Records Type Name"][rowIndex];
            FullPath = (String)datasheet.Columns["Records Type Full Path"][rowIndex];

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
        }



        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Columns = datasheet.Columns;

        }

        public override String Key()
        {
            return FullPath;
        }

    }
}
