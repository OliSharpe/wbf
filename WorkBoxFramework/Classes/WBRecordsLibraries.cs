using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkBoxFramework
{
    public class WBRecordsLibraries : IDisposable
    {
        private Dictionary<String,WBRecordsLibrary> _libraries = new Dictionary<String,WBRecordsLibrary>();

        public WBRecordsLibrary ProtectedMasterLibrary
        {
            public get; 
            private set;
        }

        public WBRecordsLibraries()
        {
            WBFarm farm = WBFarm.Local;

            if (String.IsNullOrEmpty(farm.ProtectedRecordsLibraryUrl))
            {
                WBLogging.RecordsTypes.Unexpected("The central, protected, master library has not been configured - so no records management is possible!");
                return;
            }

            ProtectedMasterLibrary = new WBRecordsLibrary(farm.ProtectedRecordsLibraryUrl, WBRecordsLibrary.PROTECTIVE_ZONE__PROTECTED);
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

        }


        public void Add(WBRecordsLibrary library)
        {
            _libraries.Add(library.URL, library);
        }

        public void Add(String libraryURL, String protectiveZone)
        {
            _libraries.Add(libraryURL, new WBRecordsLibrary(libraryURL, protectiveZone));
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


        public void Dispose()
        {
            foreach (WBRecordsLibrary library in _libraries.Values)
            {
                library.Dispose();
            }
        }

    }
}
