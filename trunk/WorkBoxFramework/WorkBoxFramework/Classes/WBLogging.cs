#region Copyright and License

// Copyright (c) Islington Council 2010-2013
// Author: Oli Sharpe  (oli@gometa.co.uk)
//
// This file is part of the Work Box Framework.
//
// The Work Box Framework is free software: you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public License as  
// published by the Free Software Foundation, either version 2.1 of the 
// License, or (at your option) any later version.
//
// The Work Box Framework (WBF) is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with the WBF.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace WorkBoxFramework
{
    /// <summary>
    /// This class is used to write log messages into the ULS trace log under the 'Work Box Framework' area.
    /// </summary>
    /// <remarks>
    /// This class learns from ideas in the following blog:
    /// http://jbaurle.wordpress.com/2011/01/16/how-to-implement-a-custom-sharepoint-2010-logging-service-for-uls-and-windows-event-log/
    /// </remarks>
    public class WBLogging : SPDiagnosticsServiceBase
    {
        public const string LOGGING_AREA_NAME = "Work Box Framework";
        public const string DEFAULT_LOGGING_SERVICE_NAME = "Work Box Framework Logging Service";

        public const string CATEGORY__TEAMS = "Teams";
        public const string CATEGORY__RECORDS_TYPES = "Records Types";
        public const string CATEGORY__TIMER_TASKS = "Timer Tasks";
        public const string CATEGORY__WORK_BOXES = "Work Boxes";
        public const string CATEGORY__WORK_BOX_COLLECTIONS = "Work Box Collections";
        public const string CATEGORY__MIGRATION = "Migration";
        public const string CATEGORY__QUERIES = "Queries";
        public const string CATEGORY__CONFIG = "Config";
        public const string CATEGORY__GENERIC = "Generic";

        public WBLogging()
            : base ("Work Box Framework Logging Service", SPFarm.Local)
        {
        }


        public static WBLogging Local
        {
            get { return SPFarm.Local.Services.GetValue<WBLogging>(DEFAULT_LOGGING_SERVICE_NAME); }
        }


        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea> { 
                new SPDiagnosticsArea(LOGGING_AREA_NAME, new List<SPDiagnosticsCategory> {
                    new SPDiagnosticsCategory(CATEGORY__TEAMS, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__RECORDS_TYPES, TraceSeverity.Monitorable, EventSeverity.Information),                    
                    new SPDiagnosticsCategory(CATEGORY__TIMER_TASKS, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__WORK_BOXES, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__MIGRATION, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__QUERIES, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__CONFIG, TraceSeverity.Monitorable, EventSeverity.Information),
                    new SPDiagnosticsCategory(CATEGORY__GENERIC, TraceSeverity.Monitorable, EventSeverity.Information)
            })
            };

            return areas;
        }


        public static void WriteTrace(String categoryName, TraceSeverity traceSeverity, String message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                WBLogging service = Local;

                if (service != null)
                {
                    SPDiagnosticsCategory category = service.Areas[LOGGING_AREA_NAME].Categories[categoryName];
                    service.WriteTrace(1, category, traceSeverity, message);
                }
            }
            catch { }
        }

        public static void Debug(String message)
        {
            if (WBFarm.Local.FarmInstance == WBFarm.FARM_INSTANCE__DEVELOPMENT_FARM)
            {
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.High, "DEBUG: " + message);
            }
            else
            {
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.Verbose, "DEBUG: " + message);
            }

        }



        #region Convenience classes

        public static class Generic
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__GENERIC, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__GENERIC, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__GENERIC, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }


            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__GENERIC, TraceSeverity.Verbose, message);
            }
        }

        public static class Config
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__CONFIG, TraceSeverity.Unexpected, message);
                }

                if (exception != null)
                {
                    WriteTrace(CATEGORY__CONFIG, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                    WriteTrace(CATEGORY__CONFIG, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                    if (exception.InnerException != null)
                    {
                        WriteTrace(CATEGORY__CONFIG, TraceSeverity.Unexpected, "Has nested inner exception: ");
                        Unexpected(exception.InnerException);
                    }
                }
            }


            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__CONFIG, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__CONFIG, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__CONFIG, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__CONFIG, TraceSeverity.Verbose, message);
            }
        }

        public static class Teams 
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__TEAMS, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__TEAMS, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__TEAMS, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__TEAMS, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__TEAMS, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__TEAMS, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__TEAMS, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__TEAMS, TraceSeverity.Verbose, message);
            }
        }

        public static class RecordsTypes
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__RECORDS_TYPES, TraceSeverity.Verbose, message);
            }
        }

        public static class TimerTasks
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__TIMER_TASKS, TraceSeverity.Verbose, message);
            }
        }

        public static class WorkBoxes
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__WORK_BOXES, TraceSeverity.Verbose, message);
            }
        }

        public static class WorkBoxCollections
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__WORK_BOX_COLLECTIONS, TraceSeverity.Verbose, message);
            }
        }

        public static class Migration
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__MIGRATION, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__MIGRATION, TraceSeverity.Verbose, message);
            }
        }

        public static class Queries
        {
            public static void Unexpected(Exception exception)
            {
                Unexpected(null, exception);
            }

            public static void Unexpected(String message, Exception exception)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    WriteTrace(CATEGORY__QUERIES, TraceSeverity.Unexpected, message);
                }

                WriteTrace(CATEGORY__QUERIES, TraceSeverity.Unexpected, "An exception occurred: " + exception.Message);
                WriteTrace(CATEGORY__QUERIES, TraceSeverity.Unexpected, "Stack trace: " + exception.StackTrace);

                if (exception.InnerException != null)
                {
                    WriteTrace(CATEGORY__QUERIES, TraceSeverity.Unexpected, "Has nested inner exception: ");
                    Unexpected(exception.InnerException);
                }
            }

            public static void Unexpected(String message)
            {
                WriteTrace(CATEGORY__QUERIES, TraceSeverity.Unexpected, message);
            }

            public static void Monitorable(String message)
            {
                WriteTrace(CATEGORY__QUERIES, TraceSeverity.Monitorable, message);
            }

            public static void HighLevel(String message)
            {
                WriteTrace(CATEGORY__QUERIES, TraceSeverity.High, message);
            }

            public static void Verbose(String message)
            {
                WriteTrace(CATEGORY__QUERIES, TraceSeverity.Verbose, message);
            }
        }



        #endregion

        #region Unused event trace code
        /*

        public static void WriteEvent(String categoryName, EventSeverity eventSeverity, String message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                WBLogging service = Local;

                if (service != null)
                {
                    SPDiagnosticsCategory category = service.Areas[LOGGING_AREA_NAME].Categories[categoryName];
                    service.WriteEvent(1, category, eventSeverity, message);
                }
            }
            catch { }
        }
         * */
        #endregion

    }

}
