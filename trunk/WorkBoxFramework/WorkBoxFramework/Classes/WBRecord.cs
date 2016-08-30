using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public String RecordID
        {
            get { return _recordID; }
        }

        private WBDocument _protectedMasterRecord = null;
        public WBDocument ProtectedMasterRecord
        {
            get
            {
                if (_protectedMasterRecord == null)
                {
                    WBRecordsLibrary masterLibrary = _libraries.ProtectedMasterLibrary;

                    _protectedMasterRecord = masterLibrary[_recordID];
                }


                return _protectedMasterRecord;
            }

        }

        WBTaxonomy _subjectTagsTaxonomy = null;
        WBTaxonomy SubjectTagsTaxonomy
        {
            get
            {
                if (_subjectTagsTaxonomy == null)
                {
                    _subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(_libraries.ProtectedMasterLibrary.Site);
                }
                return _subjectTagsTaxonomy;
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

        private void UpdateWhichLibrariesNeedACopy()
        {
            WBTermCollection<WBSubjectTag> subjectTagsApplied = new WBTermCollection<WBSubjectTag>(SubjectTagsTaxonomy, ProtectedMasterRecord.SubjectTags.UIControlValue);

            _librariesNeedingACopy.Clear();
            _librariesMustNotHaveCopy.Clear();

            if (ProtectedMasterRecord.LiveOrArchived == "Live")
            {
                if (ProtectedMasterRecord.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                {
                    _librariesNeedingACopy.WBxAddIfNotNullOrEmpty(_farm.PublicRecordsLibraryUrl);

                    _librariesNeedingACopy.AddRange(Routings.PublicLibrariesToRouteTo(subjectTagsApplied));
                    _librariesMustNotHaveCopy.AddRange(Routings.AllPublicLibraries().Except(_librariesNeedingACopy));

                    _librariesMustNotHaveCopy.AddRange(Routings.AllExtranetLibraries());
                    _librariesMustNotHaveCopy.WBxAddIfNotNullOrEmpty(_farm.PublicExtranetRecordsLibraryUrl);

                }

                if (ProtectedMasterRecord.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                {
                    _librariesNeedingACopy.WBxAddIfNotNullOrEmpty(_farm.PublicExtranetRecordsLibraryUrl);

                    _librariesNeedingACopy.AddRange(Routings.ExtranetLibrariesToRouteTo(subjectTagsApplied));
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
        }

        private void CheckNoCopiesInWrongLibraries()
        {
            foreach (String libraryURL in _librariesMustNotHaveCopy)
            {
                _libraries[libraryURL].RemoveDocumentByID(this.RecordID);
            }
        }

        private void GetAllCopies()
        {
            foreach (String libraryURL in _librariesNeedingACopy)
            {
                WBDocument document = _libraries[libraryURL].GetOrCreateCopyFromMaster(ProtectedMasterRecord);

                _recordCopies[libraryURL] = document;
            }
        }


    }
}
