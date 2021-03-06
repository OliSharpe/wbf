﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class WBRecordsLibraries : IDisposable
    {
        private Dictionary<String,WBRecordsLibrary> _libraries = new Dictionary<String,WBRecordsLibrary>();

        public WBRecordsManager Manager { get; private set; }

        public WBRecordsLibrary ProtectedMasterLibrary
        {
            get; 
            private set;
        }

        public WBRecordsLibraries(WBRecordsManager manager)
        {
            WBLogging.Debug("In WBRecordsLibraries() constructor");


            WBFarm farm = WBFarm.Local;
            Manager = manager;

            if (String.IsNullOrEmpty(farm.ProtectedRecordsLibraryUrl))
            {
                WBLogging.RecordsTypes.Unexpected("The central, protected, master library has not been configured - so no records management is possible!");
                return;
            }

            ProtectedMasterLibrary = new WBRecordsLibrary(this, farm.ProtectedRecordsLibraryUrl, WBRecordsLibrary.PROTECTIVE_ZONE__PROTECTED);
            Add(ProtectedMasterLibrary);

            AddIfMissing(farm.PublicRecordsLibraryUrl, WBRecordsLibrary.PROTECTIVE_ZONE__PUBLIC);
            AddIfMissing(farm.PublicExtranetRecordsLibraryUrl, WBRecordsLibrary.PROTECTIVE_ZONE__PUBLIC_EXTRANET);

            WBSubjectTagsRecordsRoutings routings = farm.SubjectTagsRecordsRoutings(null);

            List<String> extraPublicLibraries = routings.AllPublicLibraries();
            foreach (String libraryURL in extraPublicLibraries)
            {
                AddIfMissing(libraryURL, WBRecordsLibrary.PROTECTIVE_ZONE__PUBLIC);
            }

            List<String> extraExtranetLibraries = routings.AllExtranetLibraries();
            foreach (String libraryURL in extraExtranetLibraries)
            {
                AddIfMissing(libraryURL, WBRecordsLibrary.PROTECTIVE_ZONE__PUBLIC_EXTRANET);
            }

            WBLogging.Debug("Finished WBRecordsLibraries() constructor");
        }


        public void Add(WBRecordsLibrary library)
        {
            _libraries.Add(library.URL, library);
        }

        public void Add(String libraryURL, String protectiveZone)
        {
            _libraries.Add(libraryURL, new WBRecordsLibrary(this, libraryURL, protectiveZone));
        }

        public void AddIfMissing(String libraryURL, String protectiveZone)
        {
            if (String.IsNullOrEmpty(libraryURL)) return;

            if (!_libraries.ContainsKey(libraryURL))
            {
                Add(libraryURL, protectiveZone);
            }
        }

        public WBRecordsLibrary this[String libraryURL]
        {
            get
            {
                WBRecordsLibrary library = _libraries[libraryURL];

                // if (!library.IsOpen) library.Open();

                return library;
            }
        }

        public WBRecord GetRecordByID(String recordID)
        {
            return new WBRecord(this, recordID);
        }

        public WBRecord GetRecordByPath(String path)
        {
            String serverRelativePath = ProtectedMasterLibrary.Web.ServerRelativeUrl + "/" + ProtectedMasterLibrary.List.RootFolder.Name + "/" + path;

            WBLogging.Debug("Trying to find the RecordID by way of the server relative path: " + serverRelativePath);

            SPListItem masterRecordItem = ProtectedMasterLibrary.Web.GetListItem(serverRelativePath);

            return new WBRecord(this, masterRecordItem);
        }

        public SPFolder GetMasterFolderByPath(String path)
        {
            String serverRelativePath = ProtectedMasterLibrary.Web.ServerRelativeUrl + "/" + ProtectedMasterLibrary.List.RootFolder.Name + "/" + path;

            WBLogging.Debug("Trying to find the SPFolder in the master library by way of the server relative path: " + serverRelativePath);

            SPFolder folder = ProtectedMasterLibrary.Web.GetFolder(serverRelativePath);

            return folder;
        }


        public void Dispose()
        {
            foreach (WBRecordsLibrary library in _libraries.Values)
            {
                library.Dispose();
            }
        }


        public WBRecord DeclareNewRecord(WBTaskFeedback feedback, String callingUserLogin, WBDocument document, WBRecord recordToReplace, String replacingAction, WBItem extraMetadata)
        {
            WBTerm functionalArea = document.FunctionalArea[0];
            WBRecordsType recordsType = document.RecordsType;

            string fullClassPath = WBUtils.NormalisePath(functionalArea.Name + "/" + recordsType.FullPath);

            WBLogging.RecordsTypes.HighLevel("Declaring a document to the library with path: " + fullClassPath);

            string datePath = "NO DATE SET";
            string dateForName = "YYYY-MM-DD";
            string oldDateFormat = "YYYYMMDD-";

            // If nothing else we'll use the time now (which will be roughly the date / time declared as the date for the naming convention:
            DateTime referenceDate = DateTime.Now;
            if (document.HasReferenceDate && recordsType.DocumentReferenceDateRequirement != WBRecordsType.METADATA_REQUIREMENT__HIDDEN)
            {
                referenceDate = document.ReferenceDate;
            }
            else
            {
                document.ReferenceDate = referenceDate;
            }

            int year = referenceDate.Year;
            int month = referenceDate.Month;

            if (month >= 4) datePath = String.Format("{0}-{1}", year.ToString("D4"), (year + 1).ToString("D4"));
            else datePath = String.Format("{0}-{1}", (year - 1).ToString("D4"), year.ToString("D4"));

            dateForName = String.Format("{0}-{1}-{2}",

                        referenceDate.Year.ToString("D4"),
                        referenceDate.Month.ToString("D2"),
                        referenceDate.Day.ToString("D2"));

            oldDateFormat = String.Format("{0}{1}{2}-",
                        referenceDate.Year.ToString("D4"),
                        referenceDate.Month.ToString("D2"),
                        referenceDate.Day.ToString("D2"));


            string fullFilingPath = String.Join("/", recordsType.FilingPathForDocument(document).ToArray());

            WBLogging.Debug("The original filename is set as: " + document.OriginalFilename);

            String extension = Path.GetExtension(document.OriginalFilename);
            String filename = WBUtils.RemoveDisallowedCharactersFromFilename(document.OriginalFilename);

            String titleForFilename = document[WBColumn.Title].WBxToString();
            String referenceID = document.ReferenceID;

            // We don't want to use a title that is too long:
            if (String.IsNullOrEmpty(titleForFilename) || titleForFilename.Length > 50) titleForFilename = "";

            if (String.IsNullOrEmpty(titleForFilename) && String.IsNullOrEmpty(referenceID))
            {
                titleForFilename = Path.GetFileNameWithoutExtension(filename);

                // Let's now remove the old date format if the date is the same as the one
                // that is going to be used for the new date format:
                titleForFilename = titleForFilename.Replace(oldDateFormat, "");
            }

            if (String.IsNullOrEmpty(referenceID))
            {
                filename = "(" + dateForName + ") " + titleForFilename + extension;
            }
            else
            {
                if (String.IsNullOrEmpty(titleForFilename))
                {
                    filename = "(" + dateForName + ") " + referenceID + extension;
                }
                else
                {
                    filename = "(" + dateForName + ") " + referenceID + " - " + titleForFilename + extension;
                }
            }

            filename = WBUtils.RemoveDisallowedCharactersFromFilename(filename);

            SPContentType classFolderType = null;
            SPContentType filePartFolderType = null;

            try
            {
                classFolderType = ProtectedMasterLibrary.Site.RootWeb.ContentTypes[WBRecordsType.RECORDS_LIBRARY__CLASS_FOLDER_CONTENT_TYPE];
                filePartFolderType = ProtectedMasterLibrary.Site.RootWeb.ContentTypes[WBRecordsType.RECORDS_LIBRARY__FILE_PART_FOLDER_CONTENT_TYPE];
            }
            catch (Exception exception)
            {
                WBLogging.RecordsTypes.Unexpected("Couldn't find the class and/or file part folder content types.");
                throw new Exception("Couldn't find the class and/or file part folder content types.", exception);
            }

            if (classFolderType == null)
            {
                classFolderType = ProtectedMasterLibrary.Site.RootWeb.ContentTypes[WBRecordsType.RECORDS_LIBRARY__FALL_BACK_FOLDER_CONTENT_TYPE];
            }

            if (filePartFolderType == null)
            {
                filePartFolderType = ProtectedMasterLibrary.Site.RootWeb.ContentTypes[WBRecordsType.RECORDS_LIBRARY__FALL_BACK_FOLDER_CONTENT_TYPE];
            }

            SPFolder protectedLibraryRootFolder = ProtectedMasterLibrary.List.RootFolder;

            protectedLibraryRootFolder.WBxGetOrCreateFolderPath(fullClassPath, classFolderType.Id);
            SPFolder actualDestinationFolder = protectedLibraryRootFolder.WBxGetOrCreateFolderPath(fullFilingPath, filePartFolderType.Id);

            /*
            // This next bit is all because we've been having problems when new folders had to be created:
            if (actualDestinationFolder == null)
            {
                WBLogging.RecordsTypes.HighLevel("We have to create part of the folder path: " + fullFilingPath); 
                actualDestinationFolder = protectedLibraryRootFolder.WBxGetOrCreateFolderPath(fullFilingPath, filePartFolderType.Id);

                WBLogging.RecordsTypes.HighLevel("Now we're going to add a dummy first file:");

                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Please ignore this file."));
                SPFile dummyFile = actualDestinationFolder.Files.Add("FirstFile.txt", memoryStream);

                WBLogging.RecordsTypes.HighLevel("Now we're going to try to update the file");
                try
                {
                    dummyFile.Item.Update();
                }
                catch (Exception e)
                {
                    WBLogging.RecordsTypes.Unexpected("And exception did occur while updating the dummy item", e);
                }

                memoryStream.Dispose();

                WBLogging.RecordsTypes.HighLevel("Now re-opening the whole ProtectedMasterLibrary object");

                ProtectedMasterLibrary.ReOpen();

                WBLogging.RecordsTypes.HighLevel("Have re-opened the whole ProtectedMasterLibrary object - now re-getting the SPFolder:");

                protectedLibraryRootFolder = ProtectedMasterLibrary.List.RootFolder;
                actualDestinationFolder = protectedLibraryRootFolder.WBxGetFolderPath(fullFilingPath);
            }
              */
            
     
            if (ProtectedMasterLibrary.Web.WBxFileExists(actualDestinationFolder, filename))
            {
                filename = ProtectedMasterLibrary.Web.WBxMakeFilenameUnique(actualDestinationFolder, filename);
            }

            SPFile uploadedFile = actualDestinationFolder.Files.Add(filename, document.OpenBinaryStream());

            SPListItem uploadedItem = uploadedFile.Item;

            if (extraMetadata == null) extraMetadata = new WBItem();
            if (!extraMetadata.IsUsingColumn(WBColumn.DatePublished)) extraMetadata[WBColumn.DatePublished] = DateTime.Now;
            if (!extraMetadata.IsUsingColumn(WBColumn.PublishedBy) && SPContext.Current != null)
            {
                extraMetadata[WBColumn.PublishedBy] = SPContext.Current.Web.CurrentUser;
            }

            WBRecord newRecord = new WBRecord(this, uploadedItem, uploadedItem.ID.ToString(), document, extraMetadata);
            if (feedback != null)
            {
                String urlToFolder = newRecord.ProtectedMasterRecord.AbsoluteURL.Replace(newRecord.Name, "");

                feedback.AddFeedback("Uploaded file to protected, master records library");
                feedback.AddFeedback("Master record: <a href='" + newRecord.ProtectedMasterRecord.AbsoluteURL + "' target='_blank'>" + newRecord.ProtectedMasterRecord.AbsoluteURL + "</a>");
                feedback.AddFeedback("In folder: <a href='" + urlToFolder + "' target='_blank'>" + urlToFolder + "</a>");
            }

            if (recordToReplace != null)
            {
                // OK so actually we need to do the replacement actions with elevated priviledges:

            //bool digestOK = SPContext.Current.Web.ValidateFormDigest();
            //if (digestOK)
            //{
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (WBRecordsManager manager = new WBRecordsManager(callingUserLogin))
                    {
                        WBRecord elevatedRecordToReplace = manager.Libraries.GetRecordByID(recordToReplace.RecordID);

                        if (replacingAction == WBPublishingProcess.REPLACE_ACTION__ARCHIVE_FROM_IZZI)
                        {
                            elevatedRecordToReplace.LiveOrArchived = WBColumn.LIVE_OR_ARCHIVED__ARCHIVED;
                            elevatedRecordToReplace.RecordSeriesStatus = WBColumn.RECORD_SERIES_STATUS__ARCHIVED;
                        }
                        else
                        {
                            elevatedRecordToReplace.RecordSeriesStatus = WBColumn.RECORD_SERIES_STATUS__RETIRED;
                        }

                        elevatedRecordToReplace.Update(callingUserLogin, "Record was " + elevatedRecordToReplace.RecordSeriesStatus + " because it was replaced through publishing process");

                        if (feedback != null) feedback.AddFeedback("Archived record being replaced");
                        WBLogging.Debug("WBRecordsLibraries.DeclareNewRecord(): Archived the record being replaced Record ID = " + recordToReplace.RecordID);
                    }
                });

                newRecord.ReplacesRecordID = recordToReplace.RecordID;
                newRecord.RecordSeriesID = recordToReplace.RecordSeriesID;
                newRecord.RecordSeriesIssue = "" + (recordToReplace.RecordSeriesIssue.WBxToInt() + 1);
                newRecord.RecordSeriesStatus = WBColumn.RECORD_SERIES_STATUS__LATEST;

            }
            else
            {
                newRecord.ReplacesRecordID = null;
                newRecord.RecordSeriesID = newRecord.RecordID;
                newRecord.RecordSeriesIssue = "1";
                newRecord.RecordSeriesStatus = WBColumn.RECORD_SERIES_STATUS__LATEST;
            }

            newRecord.LiveOrArchived = WBColumn.LIVE_OR_ARCHIVED__LIVE;

            newRecord.UpdateMasterAndCreateCopies(feedback, callingUserLogin);

            bool beforeForDocument = document.Web.AllowUnsafeUpdates;
            document.Web.AllowUnsafeUpdates = true;

            // And now just copy back to the original document any metadata changes:
            document.MaybeCopyColumns(newRecord.Metadata, WBRecord.DefaultColumnsToCopy);

            // And let's make sure that the original document is using the work box document content type:
            if (document.IsSPListItem)
            {
                SPContentType workBoxDocumentType = document.Item.ParentList.ContentTypes[WBFarm.Local.WorkBoxDocumentContentTypeName];
                if (workBoxDocumentType != null)
                {
                    document.Item["ContentTypeId"] = workBoxDocumentType.Id;
                }
            }

            document.Update();

//            uploadedItem.Update();
  //          uploadedFile.Update();

            bool beforeForUploadedFile = uploadedFile.Web.AllowUnsafeUpdates;
            uploadedFile.Web.AllowUnsafeUpdates = true;

            if (uploadedFile.CheckOutType != SPFile.SPCheckOutType.None)
            {
                uploadedFile.WBxCheckInAs("Declared new major version of record.", SPCheckinType.MajorCheckIn, callingUserLogin);
            }
            else
            {
                WBLogging.Migration.Verbose("There was no need to check in file: " + uploadedFile.Name);
            }

            uploadedFile.Web.AllowUnsafeUpdates = beforeForUploadedFile;
            document.Web.AllowUnsafeUpdates = beforeForDocument;

            return newRecord;
        }

    }
}
