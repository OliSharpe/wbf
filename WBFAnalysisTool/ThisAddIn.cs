using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Tools.Excel.Extensions;
using System.Windows.Forms;

namespace WBFAnalysisTool
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion

        DataSheet<RecordsTypesDetails> AllRecordsTypes = null;
        DataSheet<TeamDetails> AllTeams = null;
        DataSheet<UserDetails> AllUsers = null;
        DataSheet<WorkBoxDetails> AllWorkBoxes = null;
        DataSheet<RecordsDetails> AllRecords = null;

        internal int ProcessWBFData(Label ProgressInformation)
        {
            Excel.Workbook processed = this.Application.Workbooks.Open("Z:\\VS2010\\WBFAnalysisTool\\Data\\WBFDataAnalysis.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


            ProgressInformation.Text = "Opened the work book";

            AllRecordsTypes = new DataSheet<RecordsTypesDetails>(processed, "Records Types Details", false);

            ProgressInformation.Text = "Loaded all records types";

            AllTeams = new DataSheet<TeamDetails>(processed, "Team Details", false);

            ProgressInformation.Text = "Loaded all teams";

            AllUsers = new DataSheet<UserDetails>(processed, "User Details", false);

            ProgressInformation.Text = "Loaded all users";

            AllWorkBoxes = new DataSheet<WorkBoxDetails>(processed, "Work Box Details", false);
            // This will just save the load error flags:
            AllWorkBoxes.SaveToSheet();

            ProgressInformation.Text = "Loaded all work boxes";

            AllRecords = new DataSheet<RecordsDetails>(processed, "Records Details", false);

            ProgressInformation.Text = "Loaded all records";

            // Now adding the WBs the belong to each team:
            calculateTeamFigures(processed);

            ProgressInformation.Text = "Calculated team figures";

            calculateWBCFigures(processed);

            ProgressInformation.Text = "Calculated WBC figures";


            int failures = calculateMonthlyFigures("Monthly Figures", processed, true, true);

            ProgressInformation.Text = "Calculated monthly figures";

            calculateMonthlyFigures("Monthly Figures w FOI", processed, false, true);

            ProgressInformation.Text = "Calculated monthly figures w FOI";

            calculateMonthlyFigures("Monthly Figures w FOI w IAS", processed, false, false);

            ProgressInformation.Text = "Calculated monthly figures w FOI w IAS";

            calculateUsedRecordsTypesFigures(processed);

            ProgressInformation.Text = "Calculated records types";

            calculateFunctionalAreasFigures(processed);

            ProgressInformation.Text = "Calculated functional areas";

            return failures;
        }

        private void calculateTeamFigures(Excel.Workbook processed)
        {
            foreach (WorkBoxDetails workBox in AllWorkBoxes)
            {
                if (!workBox.LoadedOK || String.IsNullOrEmpty(workBox.OwningTeam)) continue;

                TeamDetails figuresForTeam = AllTeams[workBox.OwningTeam];
                if (figuresForTeam == null)
                {
                    figuresForTeam = new TeamDetails(workBox.OwningTeam);
                    AllTeams.Add(figuresForTeam);
                }

                figuresForTeam.Add(workBox);
            }
            AllTeams.SaveToSheet();
        }

        private void calculateWBCFigures(Excel.Workbook processed)
        {
            // Now we're going to work out the figures for each WBC
            DataSheet<FiguresForWBC> wbcFigures = new DataSheet<FiguresForWBC>(processed, "WBC Figures", true);

            foreach (WorkBoxDetails workBox in AllWorkBoxes)
            {
                if (!workBox.LoadedOK) continue;

                FiguresForWBC figuresForWBC = wbcFigures[workBox.WorkBoxCollectionURL];
                if (figuresForWBC == null)
                {
                    figuresForWBC = new FiguresForWBC(workBox.WorkBoxCollectionURL);
                    wbcFigures.Add(figuresForWBC);
                }

                figuresForWBC.Add(workBox);
            }
            wbcFigures.SaveToSheet();
        }

        private int calculateMonthlyFigures(String workSheetTitle, Excel.Workbook processed, bool excludeFOI, bool excludeIAS)
        {
            // Now we're going to work out the monthly figures:
            DataSheet<FiguresForOneMonth> monthlyFigures = new DataSheet<FiguresForOneMonth>(processed, workSheetTitle, true);
            DateTime monthWalker = new DateTime(2012, 01, 01);
            DateTime continueUntil = new DateTime(2020, 01, 01);

            // First add all of the months up until today + two years.
            while (monthWalker <= continueUntil)
            {
                monthlyFigures.Add(new FiguresForOneMonth(monthWalker));
                monthWalker = monthWalker.AddMonths(1);
            }


            int failures = 0;

            // Now add all of the work boxes to each of the relevant months:
            foreach (WorkBoxDetails workBox in AllWorkBoxes)
            {
                if (!workBox.LoadedOK)
                {
                    failures++;
                    continue;
                }

                try
                {
                    if (workBox.hasBeenCreated)
                    {
                        if (excludeFOI) if (workBox.WorkBoxCollectionURL.Equals("http://collection.izzi/foi")) continue;
                        if (excludeIAS) if (workBox.WorkBoxCollectionURL.Equals("http://hass.izzi/ias")) continue;

                        monthWalker = (new DateTime(workBox.DateCreated.Year, workBox.DateCreated.Month, 1)).AddMonths(-1);

                        while (monthWalker <= continueUntil)
                        {
                            FiguresForOneMonth month = monthlyFigures[monthWalker.Date.Month + " " + monthWalker.Date.Year];

                            if (month != null) month.Add(workBox);

                            monthWalker = monthWalker.AddMonths(1);
                        }

                    }

                }
                catch (Exception exception)
                {
                    failures++;
                }
            }


            foreach (TeamDetails team in AllTeams)
            {
                if (team.hasBeenCreated)
                {
                    monthWalker = (new DateTime(team.DateCreated.Year, team.DateCreated.Month, 1)).AddMonths(-1);

                    while (monthWalker < DateTime.Now)
                    {
                        FiguresForOneMonth month = monthlyFigures[monthWalker.Date.Month + " " + monthWalker.Date.Year];

                        if (month != null) month.Add(team);

                        monthWalker = monthWalker.AddMonths(1);
                    }
                }
            }


            foreach (UserDetails user in AllUsers)
            {
                if (user.VisitedAnyWB)
                {
                    monthWalker = (new DateTime(user.OldestVisitDate.Year, user.OldestVisitDate.Month, 1)).AddMonths(-1);

                    while (monthWalker < DateTime.Now)
                    {
                        FiguresForOneMonth month = monthlyFigures[monthWalker.Date.Month + " " + monthWalker.Date.Year];

                        if (month != null) month.Add(user);

                        monthWalker = monthWalker.AddMonths(1);
                    }
                }
            }

            foreach (RecordsDetails record in AllRecords)
            {
                if (record.hasBeenCreated)
                {
                    monthWalker = (new DateTime(record.DateCreated.Year, record.DateCreated.Month, 1)).AddMonths(-1);

                    while (monthWalker < DateTime.Now)
                    {
                        FiguresForOneMonth month = monthlyFigures[monthWalker.Date.Month + " " + monthWalker.Date.Year];

                        if (month != null) month.Add(record);

                        monthWalker = monthWalker.AddMonths(1);
                    }
                }
            }

            monthlyFigures.SaveToSheet();

            return failures;
        }

        /*
        private void calculateMonthlyFiguresWithFOI(Excel.Workbook processed, DataSheet<WorkBoxDetails> allWorkBoxes)
        {
            // Now the monthly figures with FOI included:
            DataSheet<FiguresForOneMonth> monthlyFiguresWithFOI = new DataSheet<FiguresForOneMonth>(processed, "Monthly Figures w FOI");
            DateTime monthWalker = new DateTime(2012, 01, 01);

            // First add all of the months up until today:
            while (monthWalker < DateTime.Now)
            {
                monthlyFiguresWithFOI.Add(new FiguresForOneMonth(monthWalker));
                monthWalker = monthWalker.AddMonths(1);
            }


            // Now add all of the work boxes to each of the relevant months:
            foreach (WorkBoxDetails workBox in allWorkBoxes)
            {
                if (!workBox.LoadedOK)
                {
                    continue;
                }

                try
                {
                    if (workBox.hasBeenCreated)
                    {
                        monthWalker = (new DateTime(workBox.DateCreated.Year, workBox.DateCreated.Month, 1)).AddMonths(-1);

                        while (monthWalker < DateTime.Now)
                        {
                            FiguresForOneMonth month = monthlyFiguresWithFOI[monthWalker.Date.Month + " " + monthWalker.Date.Year];

                            if (month != null) month.Add(workBox);

                            monthWalker = monthWalker.AddMonths(1);
                        }

                    }

                }
                catch (Exception exception)
                {
                }
            }

            monthlyFiguresWithFOI.SaveToSheet();
        }
        */

        private void calculateUsedRecordsTypesFigures(Excel.Workbook processed)
        {
            // Now we're going to work out the figures for each records type
            DataSheet<FiguresForOneRecordsType> recordsTypesFigures = new DataSheet<FiguresForOneRecordsType>(processed, "Used Records Types", true);

            foreach (WorkBoxDetails workBox in AllWorkBoxes)
            {
                if (!workBox.LoadedOK || !workBox.hasBeenOpened || String.IsNullOrEmpty(workBox.RecordsType)) continue;

                FiguresForOneRecordsType figuresForRecordsType = recordsTypesFigures[workBox.RecordsType];
                if (figuresForRecordsType == null)
                {
                    figuresForRecordsType = new FiguresForOneRecordsType(workBox.RecordsType);
                    recordsTypesFigures.Add(figuresForRecordsType);

                    RecordsTypesDetails details = AllRecordsTypes[workBox.RecordsType];
                    figuresForRecordsType.SetRecordsTypeDetails(details);
                }

                figuresForRecordsType.Add(workBox);
            }

            foreach (RecordsDetails record in AllRecords)
            {
                if (!record.LoadedOK || String.IsNullOrEmpty(record.RecordsType)) continue;

                FiguresForOneRecordsType figuresForRecordsType = recordsTypesFigures[record.RecordsType];
                if (figuresForRecordsType == null)
                {
                    figuresForRecordsType = new FiguresForOneRecordsType(record.RecordsType);
                    recordsTypesFigures.Add(figuresForRecordsType);

                    RecordsTypesDetails details = AllRecordsTypes[record.RecordsType];
                    figuresForRecordsType.SetRecordsTypeDetails(details);
                }

                figuresForRecordsType.Add(record);
            }



            recordsTypesFigures.SaveToSheet();
        }


        private void calculateFunctionalAreasFigures(Excel.Workbook processed)
        {
            // Now we're going to work out the figures for each records type
            DataSheet<FiguresForFunctionalArea> funtionalAreas = new DataSheet<FiguresForFunctionalArea>(processed, "Functional Areas", true);

            foreach (TeamDetails team in AllTeams)
            {
                if (!team.LoadedOK || String.IsNullOrEmpty(team.FunctionalArea)) continue;

                FiguresForFunctionalArea figuresForFunctionalArea = funtionalAreas[team.FunctionalArea];
                if (figuresForFunctionalArea == null)
                {
                    figuresForFunctionalArea = new FiguresForFunctionalArea(team.FunctionalArea);
                    funtionalAreas.Add(figuresForFunctionalArea);
                }

                figuresForFunctionalArea.Add(team);
            }


            foreach (WorkBoxDetails workBox in AllWorkBoxes)
            {
                if (!workBox.LoadedOK || !workBox.hasBeenOpened || String.IsNullOrEmpty(workBox.FunctionalArea)) continue;

                FiguresForFunctionalArea figuresForFunctionalArea = funtionalAreas[workBox.FunctionalArea];
                if (figuresForFunctionalArea == null)
                {
                    figuresForFunctionalArea = new FiguresForFunctionalArea(workBox.FunctionalArea);
                    funtionalAreas.Add(figuresForFunctionalArea);
                }

                figuresForFunctionalArea.Add(workBox);
            }

            foreach (RecordsDetails record in AllRecords)
            {
                if (!record.LoadedOK || String.IsNullOrEmpty(record.FunctionalArea)) continue;

                FiguresForFunctionalArea figuresForFunctionalArea = funtionalAreas[record.FunctionalArea];
                if (figuresForFunctionalArea == null)
                {
                    figuresForFunctionalArea = new FiguresForFunctionalArea(record.FunctionalArea);
                    funtionalAreas.Add(figuresForFunctionalArea);
                }

                figuresForFunctionalArea.Add(record);
            }


            funtionalAreas.SaveToSheet();
        }

    }
}
