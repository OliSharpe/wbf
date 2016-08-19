using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBFAnalysisTool
{
    public class WorkBoxDetails : DataSheetType
    {
        public String Error;
        public String ExceptionDetails;
        public String WorkBoxCollectionURL;

        public String WorkBoxURL;
        public String ListItemID;

        public String Status = "";

        public String FunctionalArea = "";
        public String RecordsType = "";
        
        public DateTime DateCreated;
        public DateTime DateOpened;
        public DateTime DateClosed;
        public DateTime DateDeleted;
        public DateTime DateToBeDeleted;

        public DateTime DateLastModified;
        public DateTime DateLastVisited;

        public bool hasBeenCreated = false;
        public bool hasBeenOpened = false;
        public bool hasBeenClosed = false;
        public bool hasBeenDeleted = false;
        public bool willBeDeleted = false;
        public bool projectedRetention = false;

        public bool hasBeenModified = false;
        public bool hasBeenVisited = false;

        public String OwningTeam = "";
        public String InvolvedTeams = "";
        public String VisitingTeams = "";
        public String InvolvedIndividuals = "";
        public String VisitingIndividuals = "";

        public bool FoundDocuments = false;
        public int NumDocuments = 0;
        public long TotalSizeOfFiles = 0;

        public WorkBoxDetails()
        {
        }

        public static Dictionary<String, int> KeyRetentionPeriods = new Dictionary<String, int>();


        public override void ConfigureDataSheet<T>(DataSheet<T> datasheet)
        {
            datasheet.AddTextColumn("ERROR?", 23);
            datasheet.AddTextColumn("Exception Details", 14);
            datasheet.AddTextColumn("WBC URL", 20);
            datasheet.AddTextColumn("ID", 20);
            datasheet.AddTextColumn("Title", 20);
            datasheet.AddTextColumn("Status", 20);
            datasheet.AddTextColumn("WorkBoxStatusChangeRequest", 20);
            datasheet.AddTextColumn("WorkBoxURL", 20);
            datasheet.AddTextColumn("WorkBoxGUID", 20);
            datasheet.AddTextColumn("WorkBoxLocalID", 20);
            datasheet.AddTextColumn("WorkBoxUniqueID", 20);
            datasheet.AddTextColumn("Short Title", 20);
            datasheet.AddTextColumn("WorkBoxAuditLog", 20);
            datasheet.AddTextColumn("WorkBoxErrorMessage", 20);
            datasheet.AddTextColumn("WorkBoxCachedListItemID", 20);
            datasheet.AddDateColumn("Modified (approx)");
            datasheet.AddDateColumn("Visited (approx)");
            datasheet.AddDateColumn("Created");
            datasheet.AddDateColumn("Deleted");
            datasheet.AddDateColumn("Closed");
            datasheet.AddDateColumn("Opened");
            datasheet.AddDateColumn("Retention End Date");
            datasheet.AddTextColumn("Functional Area", 20);
            datasheet.AddTextColumn("Records Type", 20);
            datasheet.AddTextColumn("Reference ID", 20);
            datasheet.AddTextColumn("Reference Date", 20);
            datasheet.AddTextColumn("Series Tag", 20);
            datasheet.AddTextColumn("Owning Team", 20);
            datasheet.AddTextColumn("Involved Teams", 20);
            datasheet.AddTextColumn("Visiting Teams", 20);
            datasheet.AddTextColumn("Involved Individuals", 20);
            datasheet.AddTextColumn("Visiting Individuals", 20);
            datasheet.AddTextColumn("Involved Individuals Emails", 20);
            datasheet.AddTextColumn("Visiting Individuals Emails", 20);
            datasheet.AddTextColumn("Found Documents", 10);
            datasheet.AddIntegerColumn("Num Documents", 10);
            datasheet.AddLongColumn("Total Size of Docs", 10);

            datasheet.AddTextColumn("Owning Team + !Open", 20);
            datasheet.AddTextColumn("URL + !Open", 20);

            KeyRetentionPeriods.Add("Freedom of Information/FOI Case file", 3);
            KeyRetentionPeriods.Add("ICT Projects/Structured", 7);
            KeyRetentionPeriods.Add("Information/Advice and information", 3);
            KeyRetentionPeriods.Add("Team management/Team meetings", 3);
            KeyRetentionPeriods.Add("Team management/Team projects", 3);

        }

        public bool IsOpen
        {
            get { return ("Open".Equals(Status)); }
        }

        public bool IsClosed
        {
            get { return ("Closed".Equals(Status)); }
        }

        public bool IsDeleted
        {
            get { return ("Deleted".Equals(Status)); }
        }

        public override void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex)
        {

            Error = (String)datasheet.Columns["ERROR?"][rowIndex];
            ExceptionDetails = (String)datasheet.Columns["Exception Details"][rowIndex];
            WorkBoxCollectionURL = (String)datasheet.Columns["WBC URL"][rowIndex];

            ListItemID = (String)datasheet.Columns["ID"][rowIndex];
            WorkBoxURL = (String)datasheet.Columns["WorkBoxURL"][rowIndex];

            Status = (String)datasheet.Columns["Status"][rowIndex];

            FunctionalArea = (String)datasheet.Columns["Functional Area"][rowIndex];
            RecordsType = (String)datasheet.Columns["Records Type"][rowIndex];

            if (datasheet.Columns["Created"].HasValue(rowIndex)) 
            {
                hasBeenCreated = true;
                DateCreated = (DateTime)datasheet.Columns["Created"][rowIndex];
            }

            if (datasheet.Columns["Opened"].HasValue(rowIndex)) 
            {
                hasBeenOpened = true;
                DateOpened = (DateTime)datasheet.Columns["Opened"][rowIndex];
            }

            if (datasheet.Columns["Closed"].HasValue(rowIndex)) 
            {
                hasBeenClosed = true;
                DateClosed = (DateTime)datasheet.Columns["Closed"][rowIndex];
            }

            if (datasheet.Columns["Deleted"].HasValue(rowIndex)) 
            {
                hasBeenDeleted = true;
                DateDeleted = (DateTime)datasheet.Columns["Deleted"][rowIndex];
            }

            if (datasheet.Columns["Retention End Date"].HasValue(rowIndex))
            {
                willBeDeleted = true;
                DateToBeDeleted = (DateTime)datasheet.Columns["Retention End Date"][rowIndex];

                if (KeyRetentionPeriods.ContainsKey(RecordsType))
                {
                    if (hasBeenClosed && DateToBeDeleted > DateTime.Now.AddYears(100))
                    {
                        DateToBeDeleted = DateClosed.AddYears(KeyRetentionPeriods[RecordsType]);
                        willBeDeleted = true;
                        projectedRetention = true;
                    }
                }

            }
            else
            {
                if (KeyRetentionPeriods.ContainsKey(RecordsType))
                {
                    if (hasBeenClosed)
                    {
                        DateToBeDeleted = DateClosed.AddYears(KeyRetentionPeriods[RecordsType]);
                        willBeDeleted = true;
                        projectedRetention = true;
                    }
                }
            }

            if (datasheet.Columns["Modified (approx)"].HasValue(rowIndex))
            {
                hasBeenModified = true;
                DateLastModified = (DateTime)datasheet.Columns["Modified (approx)"][rowIndex];
            }

            if (datasheet.Columns["Visited (approx)"].HasValue(rowIndex))
            {
                hasBeenVisited = true;
                DateLastVisited = (DateTime)datasheet.Columns["Visited (approx)"][rowIndex];
            }


            OwningTeam = (String)datasheet.Columns["Owning Team"][rowIndex];
            InvolvedTeams = (String)datasheet.Columns["Involved Teams"][rowIndex];
            VisitingTeams = (String)datasheet.Columns["Visiting Teams"][rowIndex];
            InvolvedIndividuals = (String)datasheet.Columns["Involved Individuals"][rowIndex];
            VisitingIndividuals = (String)datasheet.Columns["Visiting Individuals"][rowIndex];

            if (datasheet.Columns["Found Documents"].HasValue(rowIndex))
            {
                FoundDocuments = true.ToString().Equals((String)datasheet.Columns["Found Documents"][rowIndex]);
                if (FoundDocuments)
                {
                    NumDocuments = (int)datasheet.Columns["Num Documents"][rowIndex];
                    TotalSizeOfFiles = (long)datasheet.Columns["Total Size of Docs"][rowIndex];
                }
            }

        }

        public override void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex)
        {
            Columns = datasheet.Columns;

            Columns["Owning Team + !Open"][rowIndex] = (!String.IsNullOrEmpty(this.OwningTeam) && !this.hasBeenOpened).ToString();
            Columns["URL + !Open"][rowIndex] = (!String.IsNullOrEmpty(this.WorkBoxURL) && !this.hasBeenOpened).ToString();

        }

        public override String Key()
        {
            return WorkBoxCollectionURL + " " + ListItemID;
        }

    }
}
