using System;
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
                    WBRecordsLibrary masterLibrary = _libraries.ProtectedMasterLibrary;

                    _protectedMasterRecord = masterLibrary[_recordID];
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

                    _metadata.CopyColumns(ProtectedMasterRecord, WBRecord.DefaultColumnsToCopy);

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

        /// <summary>
        /// This constructor should only be used to create a newly declared record
        /// </summary>
        /// <param name="libraries"></param>
        /// <param name="newMasterRecordDocument"></param>
        public WBRecord(WBRecordsLibraries libraries, SPListItem newRecordItem, String newRecordID, WBDocument originalDocument, WBItem extraMetadata)
        {

            _libraries = libraries;
            _protectedMasterRecord = new WBDocument(libraries.ProtectedMasterLibrary, newRecordItem); ;
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

            if (Metadata.LiveOrArchived == "Live")
            {
                if (Metadata.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                {
                    _librariesNeedingACopy.WBxAddIfNotNullOrEmpty(_farm.PublicRecordsLibraryUrl);

                    _librariesNeedingACopy.AddRange(Routings.PublicLibrariesToRouteTo(Metadata.SubjectTags));
                    _librariesMustNotHaveCopy.AddRange(Routings.AllPublicLibraries().Except(_librariesNeedingACopy));

                    _librariesMustNotHaveCopy.AddRange(Routings.AllExtranetLibraries());
                    _librariesMustNotHaveCopy.WBxAddIfNotNullOrEmpty(_farm.PublicExtranetRecordsLibraryUrl);

                }

                if (Metadata.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
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
        private void CheckAllCopiesAreCreatedAndLoaded
            ()
        {
            if (_allCopiesLoaded) return;
                
            foreach (String libraryURL in _librariesNeedingACopy)
            {
                WBDocument document = _libraries[libraryURL].GetOrCreateRecordCopy(this);

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

        public void UpdateMasterAndCreateCopies()
        {
            ProtectedMasterRecord.MaybeCopyColumns(Metadata, WBRecord.DefaultMasterColumnsToSave);
            ProtectedMasterRecord.Update();

            UpdateWhichLibrariesNeedACopy();
            CheckAllCopiesAreCreatedAndLoaded();
        }

        public void Update()
        {
            if (Metadata.ValuesHaveChanged)
            {
                ProtectedMasterRecord.MaybeUpdateRecordColumns(Metadata, WBRecord.DefaultMasterColumnsToSave);
                
                UpdateWhichLibrariesNeedACopy();
                CheckNoCopiesInWrongLibraries();
                CheckAllCopiesAreCreatedAndLoaded();

                foreach (WBDocument recordCopy in _recordCopies.Values)
                {
                    recordCopy.MaybeUpdateRecordColumns(Metadata, WBRecord.DefaultColumnsToCopy);
                }
            }
        }

        public static WBColumn[] DefaultColumnsToCopy = { 
                                        WBColumn.RecordID,
                                        WBColumn.RecordSeriesID,
                                        WBColumn.RecordSeriesIssue,
                                        WBColumn.DeclaredRecord,
                                        WBColumn.Title,
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
                                        WBColumn.LiveOrArchived
                                      };

        // Have a separate list of the metadata to save to the master record - because it might include some extra details:
        public static WBColumn[] DefaultMasterColumnsToSave = { 
                                        WBColumn.RecordID,
                                        WBColumn.RecordSeriesID,
                                        WBColumn.RecordSeriesIssue,
                                        WBColumn.DeclaredRecord,
                                        WBColumn.Title,
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
                                        WBColumn.LiveOrArchived
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
            get { return Metadata.ReferenceDate; }
            set { Metadata.ReferenceDate = value; }
        }

        public bool HasScanDate { get { return this.IsNotEmpty(WBColumn.ScanDate); } }
        public DateTime ScanDate
        {
            get { return Metadata.ReferenceDate; }
            set { Metadata.ReferenceDate = value; }
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
            get { return Metadata.Name; }
            set { Metadata.Name = value; }
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


        public WBTeam OwningTeam
        {
            get { return Metadata.OwningTeam; }
            set { Metadata.OwningTeam = value; }
        }


        public WBTermCollection<WBTeam> InvolvedTeams
        {
            get { return Metadata.InvolvedTeams; }
            set { Metadata.InvolvedTeams = value; }
        }

    }
}
