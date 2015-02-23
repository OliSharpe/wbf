using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class UserDetails: DataSheetType
    {
        public String Name = "";
        public String AccountName = "";
        public String Email = "";
        public bool VisitedAnyWB = false;
        public int PropertyLength = 0;
        public int NumOfRecordedWB = 0;
        public DateTime OldestVisitDate;
        public DateTime MostRecentVisitDate;
        public String WBDetails;

        public UserDetails()
        {
        }

        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            datasheet.AddTextColumn("Name", 20);
            datasheet.AddTextColumn("Account Name", 20);
            datasheet.AddTextColumn("Email", 20);
            datasheet.AddTextColumn("Visited Any WB?", 20);
            datasheet.AddIntegerColumn("Property Length", 20);
            datasheet.AddIntegerColumn("Num of Recorded WB", 20);
            datasheet.AddDateColumn("Oldest Visit Date");
            datasheet.AddDateColumn("Most Recent Visit Date");
            datasheet.AddTextColumn("WB Details", 20);

        }

        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Name = (String)datasheet.Columns["Name"][rowIndex];
            AccountName = (String)datasheet.Columns["Account Name"][rowIndex];
            Email = (String)datasheet.Columns["Email"][rowIndex];

            if (datasheet.Columns["Oldest Visit Date"].HasValue(rowIndex))
            {
                VisitedAnyWB = true;
                OldestVisitDate = (DateTime)datasheet.Columns["Oldest Visit Date"][rowIndex];
                MostRecentVisitDate = (DateTime)datasheet.Columns["Most Recent Visit Date"][rowIndex];
            }

            PropertyLength = (int)datasheet.Columns["Property Length"][rowIndex];
            NumOfRecordedWB = (int)datasheet.Columns["Num of Recorded WB"][rowIndex];
            WBDetails = (String)datasheet.Columns["WB Details"][rowIndex];
        }



        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {

        }

        public override String Key()
        {
            if (String.IsNullOrEmpty(Email)) return AccountName;
            return Email;
        }

    }
}

