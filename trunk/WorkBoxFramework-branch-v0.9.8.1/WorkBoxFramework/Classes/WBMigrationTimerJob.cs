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
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    class WBMigrationTimerJob : SPJobDefinition
    {
        internal  const string MIGRATION_TIMER_JOB__TIMER_JOB_NAME = "Work Box Framework - Migration Timer Job";

        internal  const string MIGRATION_SOURCE__ALFRESCO_RECORDS = "Alfresco Records";
        internal  const string MIGRATION_SOURCE__HFI_INTRANET_DOCUMENTS = "HFI Intranet Documents";
        internal  const string MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS = "Documentum Web Documents";

        internal static string[] KNOWN_MIGRATION_SOURCES = { 
                                        MIGRATION_SOURCE__ALFRESCO_RECORDS,
                                        MIGRATION_SOURCE__HFI_INTRANET_DOCUMENTS,
                                        MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS
                                                          };

        private String MigrationSourceSystem { get; set; }

        private SPList MigrationSubjectsList { get; set; }

        private SPList MigrationMappingList { get; set; }

        public WBMigrationTimerJob() : base() { }

        public WBMigrationTimerJob(String jobName, SPWebApplication webApplication, SPServer server, SPJobLockType jobLockType)
            : base (jobName, webApplication, server, jobLockType)
        {
            this.Title = jobName; 
        }

        public override void Execute(Guid targetInstanceId)
        {
            WBLogging.Migration.HighLevel("WBMigrationTimerJob.Execute(): Starting: " + Title);

            WBFarm farm = WBFarm.Local;


            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): farm.MigrationType : " + farm.MigrationType);
            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): farm.MigrationControlListUrl : " + farm.MigrationControlListUrl);
            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): farm.MigrationControlListView : " + farm.MigrationControlListView);
            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): farm.MigrationMappingListUrl : " + farm.MigrationMappingListUrl);
            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): farm.MigrationMappingListView : " + farm.MigrationMappingListView);
            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): farm.MigrationItemsPerCycle : " + farm.MigrationItemsPerCycle);

            WBLogging.Migration.Verbose("WBMigrationTimerJob.Execute(): running as user : " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            if (farm.MigrationType == WBFarm.MIGRATION_TYPE__NONE)
            {
                WBLogging.Migration.HighLevel("WBMigrationTimerJob.Execute(): No migration type has been set so doing nothing.");
            }
            else if (!KNOWN_MIGRATION_SOURCES.Contains(farm.MigrationSourceSystem))
            {
                WBLogging.Migration.HighLevel("WBMigrationTimerJob.Execute(): The migration source system wasn't recognised: " + farm.MigrationSourceSystem);
            }
            else
            {
                MigrationSourceSystem = farm.MigrationSourceSystem;

                RunOneMigrationCycle();
            }

            WBLogging.Migration.HighLevel("WBMigrationTimerJob.Execute(): Finished: " + Title);
        }

        private void RunOneMigrationCycle()
        {
            WBFarm farm = WBFarm.Local;

            WBMigrationMapping mapping = new WBMigrationMapping(farm.MigrationMappingListUrl, farm.MigrationMappingListView);

            using (SPSite controlSite = new SPSite(farm.MigrationControlListUrl))
            using (SPWeb controlWeb = controlSite.OpenWeb())
            {
                SPList controlList = controlWeb.GetList(farm.MigrationControlListUrl);
                SPView controlView = null;
                SPListItemCollection migrationItems = null;

                MigrationMappingList = controlWeb.GetList(farm.MigrationMappingListUrl);

                if (!String.IsNullOrEmpty(farm.MigrationSubjectsListUrl))
                {
                    MigrationSubjectsList = controlWeb.GetList(farm.MigrationSubjectsListUrl);
                }

                if (String.IsNullOrEmpty(farm.MigrationControlListView))
                {
                    migrationItems = controlList.Items;
                }
                else
                {
                    controlView = controlList.Views[farm.MigrationControlListView];
                    migrationItems = controlList.GetItems(controlView);
                }

                int total = migrationItems.Count;

                int itemsPerCycle = Convert.ToInt32(farm.MigrationItemsPerCycle);
                if (itemsPerCycle <= 0 ) itemsPerCycle = total;

                int count = 0;

                // Now let's get the additional subjects list if it is defined:




                /*
                 * 
                 *   Now opening the SPSite and SPWeb objects that can be re-used for each migrated item (where appropriate).
                 * 
                 */ 
                
                SPSite destinationSite = null;
                SPWeb destinationWeb = null;
                SPFolder destinationRootFolder = null;

                SPSite sourceSite = null;
                SPWeb sourceWeb = null;
                SPDocumentLibrary sourceLibrary = null;

                switch (farm.MigrationType)
                {
                    case WBFarm.MIGRATION_TYPE__MIGRATE_IZZI_PAGES:
                        {
                            // nothing at the moment
                            break;
                        }

                    case WBFarm.MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_LIBRARY:
                        {
                            destinationSite = new SPSite(farm.ProtectedRecordsLibraryUrl);
                            destinationWeb = destinationSite.OpenWeb();
                            destinationRootFolder = destinationWeb.GetFolder(farm.ProtectedRecordsLibraryUrl);

                            mapping.ConnectToSite(destinationSite);


                            if (MigrationSourceSystem == MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS)
                            {
                                if (farm.FarmInstance == WBFarm.FARM_INSTANCE__DEVELOPMENT_FARM)
                                {
                                    sourceSite = new SPSite("http://sharepointhub/records");
                                    sourceWeb = sourceSite.OpenWeb();
                                    sourceLibrary = sourceWeb.Lists["Source Library"] as SPDocumentLibrary;
                                } 
                                else
                                {
                                    sourceSite = new SPSite("http://stagingweb/publicrecords");
                                    sourceWeb = sourceSite.OpenWeb();
                                    sourceLibrary = sourceWeb.Lists["Documents"] as SPDocumentLibrary;

                                    foreach (SPField field in sourceLibrary.Fields)
                                    {
                                        WBLogging.Generic.Verbose("Field found: " + field.Title + " internal: " + field.InternalName + " type: " + field.Type.ToString());
                                    }
                                }

                            }



                            break;
                        }

                    case WBFarm.MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_WORK_BOXES:
                        {
                            // nothing at the moment
                            break;
                        }
                }

                try
                {
                    foreach (SPListItem migrationItem in migrationItems)
                    {
                        count++;
                        if (count > itemsPerCycle) break;

                        string progressString = String.Format("Migrating item {0} of {1} in a cycle of {2} items using control list: {3}", count, total, itemsPerCycle, farm.MigrationControlListUrl);

                        try
                        {
                            switch (farm.MigrationType)
                            {
                                case WBFarm.MIGRATION_TYPE__MIGRATE_IZZI_PAGES:
                                    {
                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): Start izzi Page Migration Cycle. " + progressString);

                                        MigrateOneWebPage(mapping, controlSite, controlWeb, controlList, controlView, migrationItem);

                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): End izzi Page Migration Cycle.");
                                        break;
                                    }

                                case WBFarm.MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_LIBRARY:
                                    {
                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): Start Documents To Library Migration Cycle. " + progressString);

                                        if (migrationItem.WBxGetAsString(WBColumn.FileOrFolder) == WBColumn.FILE_OR_FOLDER__FOLDER)
                                        {
                                            AddSubFilesAndFolders(mapping,
                                                controlSite, controlWeb, controlList, controlView,
                                                migrationItem);
                                        }
                                        else
                                        {
                                            MigrateOneDocumentToLibrary(mapping,
                                                sourceSite, sourceWeb, sourceLibrary,
                                                destinationSite, destinationWeb, destinationRootFolder,
                                                controlSite, controlWeb, controlList, controlView,
                                                migrationItem);
                                        }


                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): End Documents To Library Migration Cycle.");
                                        break;
                                    }

                                case WBFarm.MIGRATION_TYPE__MIGRATE_DOCUMENTS_TO_WORK_BOXES:
                                    {
                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): Start Documents To Work Boxes Migration Cycle. " + progressString);

                                        MigrateOneDocumentToWorkBox(mapping, controlSite, controlWeb, controlList, controlView, migrationItem);

                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): End Documents To Work Boxes Migration Cycle.");
                                        break;
                                    }

                                default:
                                    {
                                        WBLogging.Migration.HighLevel("WBMigrationTimerJob.RunOneMigrationCycle(): No migration setup to run - doing nothing.");
                                        break;
                                    }
                            }

                        }
                        catch (Exception itemLevelException)
                        {
                            string messageSoFar = migrationItem.WBxGetAsString(WBColumn.MigrationMessage);

                            migrationItem.WBxSet(WBColumn.MigrationStatus, WBColumn.MIGRATION_STATUS__ERROR);
                            migrationItem.WBxSet(WBColumn.MigrationMessage, messageSoFar + "Exception Thrown: " + itemLevelException.Message);
                            migrationItem.Update();

                            WBLogging.Migration.Unexpected("An item level exception has occurred:");
                            WBLogging.Migration.Unexpected(itemLevelException);
                        }

                    }
                }
                catch (Exception exception)
                {
                    WBLogging.Migration.Unexpected(exception);

                }
                finally
                {
                    if (destinationWeb != null) destinationWeb.Dispose();
                    if (destinationSite != null) destinationSite.Dispose();

                    if (sourceWeb != null) sourceWeb.Dispose();
                    if (sourceSite != null) sourceSite.Dispose();
                }
            }
        }

        private void AddSubFilesAndFolders(WBMigrationMapping mapping, SPSite controlSite, SPWeb controlWeb, SPList controlList, SPView controlView, SPListItem migrationItem)
        {
            string folderPath = migrationItem.WBxGetAsString(WBColumn.SourceFilePath);

            if (String.IsNullOrEmpty(folderPath))
            {
                MigrationError(migrationItem, "Could not add new files and folders as the folder path was empty");
                return;
            }

            switch (MigrationSourceSystem)
            {
                case MIGRATION_SOURCE__ALFRESCO_RECORDS:
                    {
                        AddFilesDetailsFromAlfresco(mapping, controlSite, controlWeb, controlList, controlView, migrationItem, folderPath);
                        return;
                    }
                case MIGRATION_SOURCE__HFI_INTRANET_DOCUMENTS:
                    {
                        AddSubFilesAndFolders(mapping, controlSite, controlWeb, controlList, controlView, migrationItem, folderPath);
                        return;
                    }
                default:
                    {
                        WBLogging.Migration.Unexpected("You shouldn't get here for the migrations from source: " + MigrationSourceSystem);
                        return;
                    }
            }

        }

        private void AddFilesDetailsFromAlfresco(WBMigrationMapping mapping, SPSite controlSite, SPWeb controlWeb, SPList controlList, SPView controlView, SPListItem migrationItem, string folderPath)
        {
            WBFarm farm = WBFarm.Local;
            string csvFileDetails = WBUtils.GetURLContents(folderPath, farm.MigrationUserName, farm.MigrationPassword);

            string[] filesDetails = csvFileDetails.Split('\n');

            foreach (string fileDetails in filesDetails)
            {
                string[] parts = fileDetails.Split(',');

                if (parts.Length > 3)
                {
                    // So we're only going to add new file paths to the migration control list:
                    if (WBUtils.FindItemByColumn(controlSite, controlList, WBColumn.SourceFilePath, parts[0]) == null)
                    {
                        SPListItem newMigrationItem = controlList.AddItem();

                        newMigrationItem.WBxCopyFrom(migrationItem, WBColumn.MappingPath);
                        newMigrationItem.WBxSet(WBColumn.SourceFilePath, parts[0]);
                        newMigrationItem.WBxSet(WBColumn.FileOrFolder, WBColumn.FILE_OR_FOLDER__FILE);
                        newMigrationItem.WBxSet(WBColumn.Title, parts[1]);
                        newMigrationItem.WBxSet(WBColumn.SourceID, parts[2]);
                        newMigrationItem.WBxSet(WBColumn.ReferenceID, parts[4]);
                        newMigrationItem.WBxSet(WBColumn.ReferenceDateString, parts[5]);
                        newMigrationItem.WBxSet(WBColumn.DeclaredDateString, parts[7]);

                        newMigrationItem.Update();
                    }
                }
            }

            migrationItem.WBxSet(WBColumn.MigrationStatus, WBColumn.MIGRATION_STATUS__DONE);
            migrationItem.Update();
        }

        private void AddSubFilesAndFolders(WBMigrationMapping mapping, SPSite controlSite, SPWeb controlWeb, SPList controlList, SPView controlView, SPListItem migrationItem, String folderPath)
        {
            foreach (String subFolderPath in Directory.GetDirectories(folderPath))
            {
                // So we're only going to add new file paths to the migration control list:
                if (WBUtils.FindItemByColumn(controlSite, controlList, WBColumn.SourceFilePath, subFolderPath) == null)
                {
                    SPListItem newMigrationItem = controlList.AddItem();

                    newMigrationItem.WBxCopyFrom(migrationItem, WBColumn.MappingPath);
                    newMigrationItem.WBxSet(WBColumn.SourceFilePath, subFolderPath);
                    newMigrationItem.WBxSet(WBColumn.FileOrFolder, WBColumn.FILE_OR_FOLDER__FOLDER);

                    newMigrationItem.Update();

                    AddSubFilesAndFolders(mapping, controlSite, controlWeb, controlList, controlView, newMigrationItem, subFolderPath);
                }
            }

            foreach (String fileInFolder in Directory.GetFiles(folderPath))
            {
                // We're only going to add file paths that don't already exist in the control list:
                if (WBUtils.FindItemByColumn(controlSite, controlList, WBColumn.SourceFilePath, fileInFolder) == null)
                {
                    SPListItem newMigrationItem = controlList.AddItem();

                    newMigrationItem.WBxCopyFrom(migrationItem, WBColumn.MappingPath);
                    newMigrationItem.WBxSet(WBColumn.SourceFilePath, fileInFolder);
                    newMigrationItem.WBxSet(WBColumn.FileOrFolder, WBColumn.FILE_OR_FOLDER__FILE);

                    newMigrationItem.Update();
                }
            }

            // OK so if we've added all sub-files and folders for this folder then this folder is done:
            migrationItem.WBxSet(WBColumn.MigrationStatus, WBColumn.MIGRATION_STATUS__DONE);
            migrationItem.WBxSet(WBColumn.MigratedToUrl, "n/a");
            migrationItem.WBxSet(WBColumn.DateMigrated, DateTime.Now);
            migrationItem.Update();
        }

        private void MigrationError(SPListItem migrationItem, String message)
        {
            WBLogging.Migration.Unexpected("Migration Error: " + message);
            migrationItem.WBxSet(WBColumn.MigrationStatus, WBColumn.MIGRATION_STATUS__ERROR);
            migrationItem.WBxSet(WBColumn.MigrationMessage, message);
            migrationItem.Update();
        }

        private void MigrateOneWebPage(WBMigrationMapping mapping, SPSite controlSite, SPWeb controlWeb, SPList controlList, SPView controlView, SPListItem migrationItem)
        {
            throw new NotImplementedException();
        }

        private void MigrateOneDocumentToWorkBox(WBMigrationMapping mapping, SPSite controlSite, SPWeb controlWeb, SPList controlList, SPView controlView, SPListItem migrationItem)
        {
            throw new NotImplementedException();
        }

        private void MigrateOneDocumentToLibrary(
            WBMigrationMapping mapping,
            SPSite sourceSite,
            SPWeb sourceWeb,
            SPDocumentLibrary sourceLibrary,
            SPSite destinationSite,
            SPWeb destinationWeb,
            SPFolder destinationRootFolder,
            SPSite controlSite, 
            SPWeb controlWeb, 
            SPList controlList, 
            SPView controlView, 
            SPListItem migrationItem)
        {
            WBFarm farm = WBFarm.Local;            

            //foreach (SPField field in migrationItem.Fields)
            //{
            //    WBLogging.Migration.Verbose("Field InternalName: " + field.InternalName + "  Field Title: " + field.Title +  " item[field.Title] : " + migrationItem[field.Title]);
            //}

            String sourceFilePath = migrationItem.WBxGetAsString(WBColumn.SourceFilePath);
            String mappingPath = WBUtils.NormalisePath(migrationItem.WBxGetAsString(WBColumn.MappingPath));

            WBLogging.Migration.Verbose("Trying to migrate file      : " + sourceFilePath);
            WBLogging.Migration.Verbose("Migrating with mapping path : " + mappingPath);

            WBMappedPath mappedPath = mapping[mappingPath];

            SPListItem controlItem = migrationItem;
            SPListItem mappingItem = null;
            SPListItem subjectItem = null;

            String documentumSourceID = "";

            if (MigrationSourceSystem == MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS)
            {
                documentumSourceID = controlItem.WBxGetAsString(WBColumn.SourceID);

                if (!String.IsNullOrEmpty(documentumSourceID))
                {
                    mappingItem = WBUtils.FindItemByColumn(controlSite, MigrationMappingList, WBColumn.SourceID, documentumSourceID);

                    subjectItem = WBUtils.FindItemByColumn(controlSite, MigrationSubjectsList, WBColumn.SourceID, documentumSourceID);
                }
            }




            if (mappedPath.InErrorStatus)
            {
                WBLogging.Migration.HighLevel("WBMigrationTimerJob.MigrateOneDocumentToLibrary(): There was an error with the mapped path: " + mappedPath.ErrorStatusMessage);
                return;
            }

            // OK so let's first get the various WBTerms from the mapped path so that if these
            // fail they fail before we copy the document!

            WBRecordsType recordsType = null;
            WBTermCollection<WBTerm> functionalArea = null;
            WBTermCollection<WBTerm> subjectTags = null;

            if (MigrationSourceSystem == MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS)
            {
                string recordsTypePath = controlItem.WBxGetAsString(WBColumn.RecordsTypePath);
                if (String.IsNullOrEmpty(recordsTypePath) && mappingItem != null) recordsTypePath = mappingItem.WBxGetAsString(WBColumn.RecordsTypePath);

                Term rterm = mapping.RecordsTypesTaxonomy.GetSelectedTermByPath(recordsTypePath);
                if (rterm != null)
                {
                    recordsType = new WBRecordsType(mapping.RecordsTypesTaxonomy, rterm);
                }


                string functionalAreaPath = controlItem.WBxGetAsString(WBColumn.FunctionalAreaPath);
                if (String.IsNullOrEmpty(functionalAreaPath) && mappingItem != null) functionalAreaPath = mappingItem.WBxGetAsString(WBColumn.FunctionalAreaPath);

                if (!String.IsNullOrEmpty(functionalAreaPath))
                {
                    string[] paths = functionalAreaPath.Split(';');

                    List<WBTerm> fterms = new List<WBTerm>();

                    foreach (string path in paths)
                    {
                        WBLogging.Migration.Verbose("Trying to get a Functional Area by path with: " + path);

                        Term fterm = mapping.FunctionalAreasTaxonomy.GetOrCreateSelectedTermByPath(path);
                        if (fterm != null)
                        {
                            fterms.Add(new WBTerm(mapping.FunctionalAreasTaxonomy, fterm));
                        }
                        else
                        {
                            WBLogging.Debug("Coundn't find the functional area with path: " + path);
                        }
                    }

                    if (fterms.Count > 0)
                    {
                        functionalArea = new WBTermCollection<WBTerm>(mapping.FunctionalAreasTaxonomy, fterms);
                    }

                }
            

                string subjectTagsPaths = controlItem.WBxGetAsString(WBColumn.SubjectTagsPaths);
                if (String.IsNullOrEmpty(subjectTagsPaths) && mappingItem != null) subjectTagsPaths = mappingItem.WBxGetAsString(WBColumn.SubjectTagsPaths);

                if (!String.IsNullOrEmpty(subjectTagsPaths))
                {
                    List<WBTerm> sterms = new List<WBTerm>();


                    // Note that it is not necessarily an error for the subject tags to be empty.
                    if (!String.IsNullOrEmpty(subjectTagsPaths) && subjectTagsPaths != "/")
                    {
                        string[] paths = subjectTagsPaths.Split(';');

                        foreach (string path in paths)
                        {
                            WBLogging.Migration.Verbose("Trying to get a Subject Tag by path with: " + path);

                            if (path != "/")
                            {
                                Term sterm = mapping.SubjectTagsTaxonomy.GetOrCreateSelectedTermByPath(path);
                                if (sterm != null)
                                {
                                    sterms.Add(new WBTerm(mapping.SubjectTagsTaxonomy, sterm));
                                }
                                else
                                {
                                    WBLogging.Debug("Coundn't find the subject tag with path: " + path);
                                }
                            }
                        }
                    }

                    subjectTags = new WBTermCollection<WBTerm>(mapping.SubjectTagsTaxonomy, sterms);
                }
            }
            else
            {
                recordsType = mappedPath.RecordsType;
                functionalArea = mappedPath.FunctionalArea;
                subjectTags = mappedPath.SubjectTags;
            }



            if (MigrationSubjectsList != null && MigrationSourceSystem == MIGRATION_SOURCE__HFI_INTRANET_DOCUMENTS)
            {
                //foreach (SPField field in migrationItem.Fields)
                //{
                 //   WBLogging.Debug("Found field: " + field.Title + " field inner name: " + field.InternalName);
                //}

                subjectTags = AddAdditionalSubjectTags(controlSite, subjectTags, migrationItem.WBxGetAsString(WBColumn.SourceID));
            }


            if (recordsType == null)
            {
                MigrationError(migrationItem, "The records type for this item could not be found. Looked for: " + mappedPath.RecordsTypePath);
                return;
            }

            if (functionalArea == null || functionalArea.Count == 0)
            {
                MigrationError(migrationItem, "The functional area for this item could not be found. Looked for: " + mappedPath.FunctionalAreaPath);
                return;
            }

            // OK so we can start building up our information about the document we are going to declare:
            WBDocument document = new WBDocument();

            document.RecordsType = recordsType;
            document.FunctionalArea = functionalArea;
            document.SubjectTags = subjectTags;

            document[WBColumn.SourceFilePath] = sourceFilePath;

            string sourceSystem = migrationItem.WBxGetAsString(WBColumn.SourceSystem);
            if (String.IsNullOrEmpty(sourceSystem)) sourceSystem = farm.MigrationSourceSystem;
            if (String.IsNullOrEmpty(sourceSystem)) sourceSystem = farm.MigrationControlListUrl;
            document[WBColumn.SourceSystem] = sourceSystem;

            String sourceID = migrationItem.WBxGetAsString(WBColumn.SourceID);
            if (String.IsNullOrEmpty(sourceID) && MigrationSourceSystem != MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS) sourceID = sourceFilePath;
            document[WBColumn.SourceID] = sourceID;

            SPFile sourceFile = null;
            SPListItem sourceItem = null;
            if (MigrationSourceSystem == MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS)
            {
                if (String.IsNullOrEmpty(sourceID))
                {
                    sourceItem = sourceWeb.GetListItem(sourceFilePath);
                    document[WBColumn.SourceID] = sourceFilePath;
                    document[WBColumn.SourceSystem] = "Initial SharePoint Web Docs";
                }
                else
                {
                    sourceItem = WBUtils.FindItemByColumn(sourceSite, (SPList)sourceLibrary, WBColumn.Source_ID, sourceID);
                }

                if (sourceItem == null)
                {
                    MigrationError(migrationItem, "Could not find the doc with source id = " + sourceFilePath);
                    return;
                }
                sourceFile = sourceItem.File;
            }






            if (migrationItem.WBxIsNotBlank(WBColumn.ReferenceDateString))
            {
                document.ReferenceDate = WBUtils.ParseDate(migrationItem.WBxGetAsString(WBColumn.ReferenceDateString));
            }

            if (migrationItem.WBxIsNotBlank(WBColumn.ModifiedDateString))
            {
                document.Modified = WBUtils.ParseDate(migrationItem.WBxGetAsString(WBColumn.ModifiedDateString));
            }
            else
            {
                if (mappingItem != null)
                {
                    if (mappingItem.WBxIsNotBlank(WBColumn.ModifiedDateString))
                    {
                        document.Modified = WBUtils.ParseDate(mappingItem.WBxGetAsString(WBColumn.ModifiedDateString));
                    }
                }
                else if (subjectItem != null)
                {
                    if (subjectItem.WBxIsNotBlank(WBColumn.ModifiedDateString))
                    {
                        document.Modified = WBUtils.ParseDate(subjectItem.WBxGetAsString(WBColumn.ModifiedDateString));
                    }
                }
                else if (sourceItem != null)
                {
                    if (sourceItem.WBxHasValue(WBColumn.Modified))
                      document.Modified = (DateTime)sourceItem["Modified"];
                }
            }

            if (migrationItem.WBxIsNotBlank(WBColumn.DeclaredDateString))
            {
                document.DeclaredRecord = WBUtils.ParseDate(migrationItem.WBxGetAsString(WBColumn.DeclaredDateString));
            }

            if (migrationItem.WBxIsNotBlank(WBColumn.ScanDateString))
            {
                document.ScanDate = WBUtils.ParseDate(migrationItem.WBxGetAsString(WBColumn.ScanDateString));
            }


            if (migrationItem.WBxIsNotBlank(WBColumn.OwningTeamPath) || !String.IsNullOrEmpty(mappedPath.OwningTeamPath))
            {
                WBTaxonomy teamsTaxonomy = mapping.TeamsTaxonomy;

                string owningTeamPath = migrationItem.WBxGetAsString(WBColumn.OwningTeamPath);
                if (owningTeamPath == "")
                {
                    owningTeamPath = mappedPath.OwningTeamPath;
                }
                WBTeam foundTeam = teamsTaxonomy.GetSelectedTeam(WBUtils.NormalisePath(owningTeamPath));

                if (foundTeam != null)
                {
                    WBLogging.Migration.Verbose("Found the owning team: " + foundTeam.Name);
                    document.OwningTeam = foundTeam;
                }
                else
                {
                    MigrationError(migrationItem, "Could not find the owning team at: " + owningTeamPath);
                    return;
                }
            }

            if (migrationItem.WBxIsNotBlank(WBColumn.Title))
            {
                document[WBColumn.Title] = migrationItem.WBxGetAsString(WBColumn.Title);
            }


            if (MigrationSourceSystem == MIGRATION_SOURCE__HFI_INTRANET_DOCUMENTS)
            {
                document.Modified = File.GetLastWriteTime(sourceFilePath);
                WBLogging.Debug("Found the last modified date to be: " + document.Modified);
            }

            // We'll set the reference date for these imported files based on their existing declared date or modified date if it exists.
            if (!document.HasReferenceDate)
            {
                if (document.HasDeclaredRecord) document.ReferenceDate = document.DeclaredRecord;
                else if (document.HasScanDate) document.ReferenceDate = document.ScanDate;
                else if (document.HasModified) document.ReferenceDate = document.Modified;                
            }

            if (migrationItem.WBxHasValue(WBColumn.ReferenceID))
            {
                document.ReferenceID = migrationItem.WBxGetAsString(WBColumn.ReferenceID);
            }


            string protectiveZone = migrationItem.WBxGetAsString(WBColumn.ProtectiveZone);
            if (String.IsNullOrEmpty(protectiveZone))
            {
                protectiveZone = mappedPath.ProtectiveZone;
                if (String.IsNullOrEmpty(protectiveZone)) protectiveZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;
            }
            document[WBColumn.ProtectiveZone] = protectiveZone;

            string liveOrArchived = migrationItem.WBxGetAsString(WBColumn.LiveOrArchived);
            if (String.IsNullOrEmpty(liveOrArchived))
            {
                liveOrArchived = mappedPath.LiveOrArchived;
                if (String.IsNullOrEmpty(liveOrArchived)) liveOrArchived = WBColumn.LIVE_OR_ARCHIVED__LIVE;
            }
            document[WBColumn.LiveOrArchived] = liveOrArchived;

            bool downloadFromWebSite = false;
            if (MigrationSourceSystem == MIGRATION_SOURCE__ALFRESCO_RECORDS)
            {
                downloadFromWebSite = true;
            }


            String originalFileName = migrationItem.WBxGetAsString(WBColumn.OriginalFilename).Trim();

            if (String.IsNullOrEmpty(originalFileName)) originalFileName = Path.GetFileName(sourceFilePath);
            if (MigrationSourceSystem == MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS) originalFileName = sourceFile.Name;
            if (downloadFromWebSite) originalFileName = HttpUtility.UrlDecode(originalFileName);

            document.OriginalFilename = originalFileName;

            //String extension = Path.GetExtension(filename);


            WBItemMessages metadataErrors = recordsType.CheckMetadataIsOK(document);

            if (metadataErrors.Count > 0)
            {
                string message = "There were problems with the prepared metadata. "; 
                foreach (WBColumn column in metadataErrors.Keys) 
                {
                    message += "Error for column: " + column.DisplayName + " message: " + metadataErrors[column]  + "  ";
                }
                MigrationError(migrationItem, message);
                return;
            }

            Stream fileStream = null;

            if (downloadFromWebSite)
            {
                WebClient webClient = new WebClient();

                if (!String.IsNullOrEmpty(farm.MigrationUserName) && !String.IsNullOrEmpty(farm.MigrationPassword))
                {
                    webClient.Credentials = new NetworkCredential(farm.MigrationUserName, farm.MigrationPassword);
                }

                string tempFile = @"C:\Temp\tmp.bin";
                if (farm.FarmInstance == WBFarm.FARM_INSTANCE__PROTECTED_INTERNAL_FARM)
                {
                    tempFile = @"E:\Temp\tmp.bin";
                }

                webClient.DownloadFile(sourceFilePath, tempFile);
                WBLogging.Migration.Verbose("Downloaded to local tmp file using webClient.DownloadFile() successfully");

                fileStream = File.OpenRead(tempFile);
                WBLogging.Migration.Verbose("Opened local tmp file using File.OpenRead() successfully");
            }
            else if (MigrationSourceSystem == MIGRATION_SOURCE__DOCUMENTUM_WEB_DOCUMENTS)
            {
                fileStream = sourceFile.OpenBinaryStream();
                WBLogging.Migration.Verbose("Opened using sourceFile.OpenBinaryStream() successfully");
            }
            else
            {
                fileStream = File.OpenRead(sourceFilePath);
                WBLogging.Migration.Verbose("Opened using File.OpenRead() successfully");
            }

            SPListItem uploadedItem = null;

            try
            {
                uploadedItem = recordsType.PublishDocument(destinationWeb, destinationRootFolder, document, fileStream);
            }
            finally
            {
                fileStream.Close();
                fileStream.Dispose();
            }

            if (uploadedItem == null)
            {
                MigrationError(migrationItem, "There was a problem in the call to recordsType.PublishDocument() as the uploaded item is null.");
                return;
            }

            migrationItem.WBxSet(WBColumn.DateMigrated, DateTime.Now);
            migrationItem.WBxSet(WBColumn.MigratedToUrl, uploadedItem.WBxGet(WBColumn.EncodedAbsoluteURL));
            migrationItem.WBxSet(WBColumn.RecordID, uploadedItem.WBxGet(WBColumn.RecordID));
            migrationItem.WBxSet(WBColumn.MigrationStatus, WBColumn.MIGRATION_STATUS__DONE);

            migrationItem.Update();
        }

        private WBTermCollection<WBTerm> AddAdditionalSubjectTags(SPSite controlSite, WBTermCollection<WBTerm> subjectTags, String sourceID)
        {
            WBLogging.Migration.Verbose("Adding additional subject tags for item with Source ID = " + sourceID);
            WBQuery query = new WBQuery();

            query.AddFilter(WBColumn.SourceID, WBQueryClause.Comparators.Equals, sourceID);

            SPListItemCollection items = MigrationSubjectsList.WBxGetItems(controlSite, query);

            if (items.Count > 0)
            {
                WBTaxonomy subjectTagsTaxonomy = subjectTags.Taxonomy;

                subjectTags = new WBTermCollection<WBTerm>(subjectTags);

                foreach (SPListItem item in items)
                {
                    String paths = WBUtils.NormalisePaths(item.WBxGetAsString(WBColumn.SubjectTagsPaths));

                    string[] pathsArray = paths.Split(';');
                    foreach (String path in pathsArray)
                    {
                        Term subjectTerm = subjectTagsTaxonomy.GetOrCreateSelectedTermByPath(path);
                        if (subjectTerm != null)
                        {
                            WBLogging.Migration.Verbose("Adding additional subject: " + path);

                            subjectTags.Add(new WBTerm(subjectTagsTaxonomy, subjectTerm));
                        }
                        else
                        {
                            WBLogging.Migration.Unexpected("Could not find or create subject: " + path);
                        }
                    }
                }
            }

//            WBLogging.Migration.Verbose("At this point the subjectTags = " + subjectTags);

            return subjectTags;
        }

    }
}
