﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Office.RecordsManagement.RecordsRepository;

namespace WorkBoxFramework
{
    public class WBRecord
    {
        WBFarm _farm = WBFarm.Local;

        private WBRecordsLibraries _libraries = null;

        private Dictionary<String, WBDocument> _recordCopies = new Dictionary<string, WBDocument>();
        private List<String> _librariesNeedingACopy = new List<string>();
        private List<String> _librariesMustNotHaveCopy = new List<string>();

        private String _recordID = null;

        private WBDocument _protectedMasterRecord = null;
        public WBDocument ProtectedMasterRecord
        {
            get
            {
                if (_protectedMasterRecord == null)
                {
                    if (_libraries == null) WBLogging.Debug("_libraries == null");
                    WBRecordsLibrary masterLibrary = _libraries.ProtectedMasterLibrary;

                    if (masterLibrary == null) WBLogging.Debug("masterLibrary == null");
                    _protectedMasterRecord = masterLibrary[_recordID];

                    if (_protectedMasterRecord == null) WBLogging.Debug("_protectedMasterRecord == null");
                    _protectedMasterRecord.DebugName = "ProtectedMasterRecord";
                }

                return _protectedMasterRecord;
            }
        }

        private WBDocument _metadata = null;
        public WBDocument Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new WBDocument(_libraries.ProtectedMasterLibrary);
                    _metadata.DebugName = "Metadata";

                    _metadata.CopyColumns(ProtectedMasterRecord, WBRecord.DefaultMasterColumnsToSave);

                    WBLogging.Debug("On creation we have: _metadata.RecordID = " + _metadata.RecordID  + " when _recordID = " + _recordID);

                    _metadata.CheckForChangesFromNow();
                }

                return _metadata;
            }
        }


        WBTaxonomy SubjectTagsTaxonomy
        {
            get
            {
                return ProtectedMasterRecord.SubjectTagsTaxonomy;
            }
        }

        WBSubjectTagsRecordsRoutings _routings = null; 
        WBSubjectTagsRecordsRoutings Routings {
            get {
                if (_routings == null) {
                    _routings = _farm.SubjectTagsRecordsRoutings(SubjectTagsTaxonomy);
                }
                return _routings;
            }
        }


        public WBRecord(WBRecordsLibraries libraries, String recordID)
        {
            _libraries = libraries;
            _recordID = recordID;
        }


        public WBRecord(WBRecordsLibraries libraries, SPListItem masterRecordItem)
        {
            _libraries = libraries;
            _protectedMasterRecord = new WBDocument(libraries.ProtectedMasterLibrary, masterRecordItem); ;
            _recordID = _protectedMasterRecord.RecordID;

            if (String.IsNullOrEmpty(_recordID)) throw new Exception("You cannot create a WBRecord with a SPListItem that doesn't have a RecordID value!");
        }


        /// <summary>
        /// This constructor should only be used to create a newly declared record
        /// </summary>
        /// <param name="libraries"></param>
        /// <param name="newMasterRecordDocument"></param>
        public WBRecord(WBRecordsLibraries libraries, SPListItem newRecordItem, String newRecordID, WBDocument originalDocument, WBItem extraMetadata)
        {
            _libraries = libraries;
            _protectedMasterRecord = new WBDocument(libraries.ProtectedMasterLibrary, newRecordItem);
            _recordID = newRecordID;

            Metadata.CopyColumns(originalDocument, WBRecord.DefaultColumnsToCopy);
            Metadata.CopyColumns(extraMetadata);

            // Now make sure that the record ID is set correctly:
            Metadata.RecordID = newRecordID;

//            UpdateWhichLibrariesNeedACopy();
  //          CheckAllCopiesAreLoaded();
        }

        private void UpdateWhichLibrariesNeedACopy()
        {
            WBLogging.Debug("In UpdateWhichLibrariesNeedACopy()");
            //WBTermCollection<WBSubjectTag> subjectTagsApplied = new WBTermCollection<WBSubjectTag>();

            _librariesNeedingACopy.Clear();
            _librariesMustNotHaveCopy.Clear();

            if (LiveOrArchived == "Live" && (RecordSeriesStatus == WBColumn.RECORD_SERIES_STATUS__LATEST || String.IsNullOrEmpty(RecordSeriesStatus)))
            {
                if (ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                {
                    _librariesNeedingACopy.WBxAddIfNotNullOrEmpty(_farm.PublicRecordsLibraryUrl);

                    _librariesNeedingACopy.AddRange(Routings.PublicLibrariesToRouteTo(Metadata.SubjectTags));
                    _librariesMustNotHaveCopy.AddRange(Routings.AllPublicLibraries().Except(_librariesNeedingACopy));

                    _librariesMustNotHaveCopy.AddRange(Routings.AllExtranetLibraries());
                    _librariesMustNotHaveCopy.WBxAddIfNotNullOrEmpty(_farm.PublicExtranetRecordsLibraryUrl);

                }

                if (ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                {
                    _librariesNeedingACopy.WBxAddIfNotNullOrEmpty(_farm.PublicExtranetRecordsLibraryUrl);

                    _librariesNeedingACopy.AddRange(Routings.ExtranetLibrariesToRouteTo(Metadata.SubjectTags));
                    _librariesMustNotHaveCopy.AddRange(Routings.AllExtranetLibraries().Except(_librariesNeedingACopy));

                    _librariesMustNotHaveCopy.AddRange(Routings.AllPublicLibraries());
                    _librariesMustNotHaveCopy.WBxAddIfNotNullOrEmpty(_farm.PublicRecordsLibraryUrl);
                }
            }
            else
            {
                _librariesMustNotHaveCopy.WBxAddIfNotNullOrEmpty(_farm.PublicRecordsLibraryUrl);
                _librariesMustNotHaveCopy.WBxAddIfNotNullOrEmpty(_farm.PublicExtranetRecordsLibraryUrl);

                _librariesMustNotHaveCopy.AddRange(Routings.AllPublicLibraries());
                _librariesMustNotHaveCopy.AddRange(Routings.AllExtranetLibraries());
            }


            // Section of debug output:
            if (_librariesNeedingACopy.Count == 0)
            {
                WBLogging.Debug("Record ID = " + RecordID + " no public libraries need a copy");
            }

            foreach (String libraryURL in _librariesNeedingACopy)
            {
                WBLogging.Debug("Record ID = " + RecordID + " needs to be in: " + libraryURL);
            }

            if (_librariesMustNotHaveCopy.Count == 0)
            {
                WBLogging.Debug("Record ID = " + RecordID + " there are no public libraries that must NOT have a copy");
            }
            foreach (String libraryURL in _librariesMustNotHaveCopy)
            {
                WBLogging.Debug("Record ID = " + RecordID + " must NOT be in: " + libraryURL);
            }

        }

        private void CheckNoCopiesInWrongLibraries()
        {
            foreach (String libraryURL in _librariesMustNotHaveCopy)
            {
                _libraries[libraryURL].RemoveDocumentByID(this.RecordID);
                if (_recordCopies.ContainsKey(libraryURL)) _recordCopies.Remove(libraryURL);
            }
        }

        private bool _allCopiesLoaded = false;
        private void CheckAllCopiesAreCreatedAndLoaded(WBTaskFeedback feedback)
        {
            if (_allCopiesLoaded) return;
                
            foreach (String libraryURL in _librariesNeedingACopy)
            {
                WBDocument document = _libraries[libraryURL].GetOrCreateRecordCopy(feedback, this);

                _recordCopies[libraryURL] = document;
            }

            _allCopiesLoaded = true;
        }

        public Object this[WBColumn column]
        {
            get
            {
                return Metadata[column];
            }

            set
            {
                Metadata[column] = value;
            }
        }

        public void UpdateMasterAndCreateCopies(WBTaskFeedback feedback, String callingUser)
        {
            ProtectedMasterRecord.MaybeCopyColumns(Metadata, WBRecord.DefaultMasterColumnsToSave);
            ProtectedMasterRecord.UpdateAs(callingUser);
            if (feedback != null)
            {
                feedback.AddFeedback("Updated metadata in protected, master records library");
            }

            UpdateWhichLibrariesNeedACopy();
            CheckAllCopiesAreCreatedAndLoaded(feedback);
        }

        public void Update(String callingUserLogin, String reasonForUpdate)
        {
            if (Metadata.ValuesHaveChanged)
            {
                WBLogging.RecordsTypes.Verbose("In WBRecords.Update() With changed metadata values - so saving the update");
                ProtectedMasterRecord.MaybeUpdateRecordColumns(callingUserLogin, Metadata, WBRecord.DefaultMasterColumnsToSave, reasonForUpdate);

                WBLogging.RecordsTypes.Verbose("In WBRecords.Update() Updated the master record.");

                UpdateWhichLibrariesNeedACopy();
                WBLogging.RecordsTypes.Verbose("In WBRecords.Update() Updated list of libraries that need a copy: " + String.Join(";", _librariesNeedingACopy.ToArray()));

                CheckNoCopiesInWrongLibraries();
                WBLogging.RecordsTypes.Verbose("In WBRecords.Update() Removed any copies from libraries that don't need a copy: " + String.Join(";", this._librariesMustNotHaveCopy.ToArray()));

                CheckAllCopiesAreCreatedAndLoaded(null);
                WBLogging.RecordsTypes.Verbose("In WBRecords.Update() Checked that there at least exists a copy in each library that needs a copy");

                foreach (WBDocument recordCopy in _recordCopies.Values)
                {
                    WBLogging.RecordsTypes.Verbose("In WBRecords.Update() About to update metadata in record copy in: " + recordCopy.RecordsLibrary.URL);
                    recordCopy.MaybeUpdateRecordColumns(callingUserLogin, Metadata, WBRecord.DefaultColumnsToCopy, reasonForUpdate);
                    WBLogging.RecordsTypes.Verbose("In WBRecords.Update() Finished updating metadata in record copy in: " + recordCopy.RecordsLibrary.URL);
                }
            }
        }

        public static WBColumn[] DefaultColumnsToCopy = { 
                                        WBColumn.Title,
                                        WBColumn.RecordID,
                                        WBColumn.RecordSeriesID,
                                        WBColumn.RecordSeriesIssue,
                                        WBColumn.RecordSeriesStatus,
                                        WBColumn.ReplacesRecordID,
                                        WBColumn.RecordsType, 
                                        WBColumn.FunctionalArea, 
                                        WBColumn.SubjectTags,
                                        WBColumn.SeriesTag,
                                        WBColumn.ReferenceID,
                                        WBColumn.ReferenceDate,
                                        WBColumn.ScanDate,
                                        WBColumn.OwningTeam,
                                        WBColumn.InvolvedTeams,
                                        WBColumn.OriginalFilename,
                                        WBColumn.ProtectiveZone,
                                        WBColumn.LiveOrArchived
                                      };

        // Have a separate list of the metadata to save to the master record - because it might include some extra details:
        public static WBColumn[] DefaultMasterColumnsToSave = { 
                                        WBColumn.Title,
                                        WBColumn.RecordID,
                                        WBColumn.RecordSeriesID,
                                        WBColumn.RecordSeriesIssue,
                                        WBColumn.RecordSeriesStatus,
                                        WBColumn.ReplacesRecordID,
                                        WBColumn.DeclaredRecord,
                                        WBColumn.RecordsType, 
                                        WBColumn.FunctionalArea, 
                                        WBColumn.SubjectTags,
                                        WBColumn.SeriesTag,
                                        WBColumn.ReferenceID,
                                        WBColumn.ReferenceDate,
                                        WBColumn.ScanDate,
                                        WBColumn.OwningTeam,
                                        WBColumn.InvolvedTeams,
                                        WBColumn.OriginalFilename,
                                        WBColumn.SourceID,
                                        WBColumn.SourceFilePath,
                                        WBColumn.SourceSystem,
                                        WBColumn.ProtectiveZone,
                                        WBColumn.LiveOrArchived,
                                        WBColumn.PublishingApprovedBy,
                                        WBColumn.PublishingApprovalChecklist,
                                        WBColumn.PublishedBy,
                                        WBColumn.DatePublished,
                                        WBColumn.ReviewDate,
                                        WBColumn.IntendedWebPageURL,
                                        WBColumn.IAOAtTimeOfPublishing
                                      };



        public bool IsNotEmpty(WBColumn column)
        {
            return ProtectedMasterRecord.IsNotEmpty(column);
        }

        public bool IsNullOrEmpty(WBColumn column)
        {
            return ProtectedMasterRecord.IsNullOrEmpty(column);
        }

        public WBRecordsType RecordsType
        {
            get { return Metadata.RecordsType; }
            set { Metadata.RecordsType = value; }
        }

        public WBTermCollection<WBTerm> FunctionalArea
        {
            get { return Metadata.FunctionalArea; }
            set { Metadata.FunctionalArea = value; }
        }

        public WBTermCollection<WBSubjectTag> SubjectTags
        {
            get { return Metadata.SubjectTags; }
            set { Metadata.SubjectTags = value; }
        }

        public String SubjectTagsUIControlValue
        {
            get { return SubjectTags.UIControlValue; }
            set { Metadata[WBColumn.SubjectTags] = value; }
        }

        public WBTerm SeriesTag
        {
            get { return Metadata.SeriesTag; }
            set { Metadata.SeriesTag = value; }
        }


        public bool HasReferenceDate { get { return Metadata.HasReferenceDate; } }
        public DateTime ReferenceDate
        {
            get { return Metadata.ReferenceDate; }
            set { Metadata.ReferenceDate = value; }
        }


        public bool HasDeclaredRecord { get { return Metadata.HasDeclaredRecord; } }
        public DateTime DeclaredRecord
        {
            get { return Metadata.DeclaredRecord; }
            set { Metadata.DeclaredRecord = value; }
        }

        public bool HasScanDate { get { return this.IsNotEmpty(WBColumn.ScanDate); } }
        public DateTime ScanDate
        {
            get { return Metadata.ScanDate; }
            set { Metadata.ScanDate = value; }
        }


        public String ReferenceID
        {
            get { return Metadata.ReferenceID; }
            set { Metadata.ReferenceID = value; }
        }

        public String OriginalFilename
        {
            get { return Metadata.OriginalFilename; }
            set { Metadata.OriginalFilename = value; }
        }

        public String Name
        {
            get { return ProtectedMasterRecord.Name; }
        }

        public String Title
        {
            get { return Metadata.Title; }
            set { Metadata.Title = value; }
        }

        public String Filename
        {
            get { return Metadata.Filename; }
            set { Metadata.Filename = value; }
        }

        public String ProtectiveZone
        {
            get { return Metadata.ProtectiveZone; }
            set { Metadata.ProtectiveZone = value; }
        }

        public String LiveOrArchived
        {
            get { return Metadata.LiveOrArchived; }
            set { Metadata.LiveOrArchived = value; }
        }

        public String RecordID
        {
            get {

                String metadataRecordID = Metadata.RecordID;

                if (metadataRecordID != _recordID) {
                    WBUtils.shouldThrowError("The set record ID " + _recordID + " is not the same as the WBRecord object's record ID value!! " + metadataRecordID);
                }

                return metadataRecordID; 
            }            
        }
        
        public String RecordSeriesID
        {
            get { return Metadata.RecordSeriesID; }
            set { Metadata.RecordSeriesID = value; }
        }

        public String ReplacesRecordID
        {
            get { return Metadata.ReplacesRecordID; }
            set { Metadata.ReplacesRecordID = value; }
        }

        public String RecordSeriesIssue
        {
            get { return Metadata.RecordSeriesIssue; }
            set { Metadata.RecordSeriesIssue = value; }
        }

        public String RecordSeriesStatus
        {
            get { return Metadata[WBColumn.RecordSeriesStatus].WBxToString(); }
            set { Metadata[WBColumn.RecordSeriesStatus] = value; }
        }


        public WBTeam OwningTeam
        {
            get { return Metadata.OwningTeam; }
            set { Metadata.OwningTeam = value; }
        }

        public String OwningTeamUIControlValue
        {
            get { return OwningTeam.UIControlValue; }
            set { Metadata[WBColumn.OwningTeam] = value; }
        }

        public WBTermCollection<WBTeam> InvolvedTeams
        {
            get { return Metadata.InvolvedTeams; }
            set { Metadata.InvolvedTeams = value; }
        }

        public String InvolvedTeamsWithoutOwningTeamAsUIControlValue
        {
            get
            {
                WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(InvolvedTeams);
                involvedTeams.Remove(OwningTeam);

                return involvedTeams.UIControlValue;
            }
            set
            {
                WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(ProtectedMasterRecord.TeamsTaxonomy, value);
                involvedTeams.Add(OwningTeam);

                InvolvedTeams = involvedTeams;
            }
        }

        /// <summary>
        /// This method just checks that the metadata of the record is in a sensible state. 
        /// </summary>
        public void CheckMetadata()
        {
            Metadata.CheckAndFixMetadataForRecord();
        }

    }
}
