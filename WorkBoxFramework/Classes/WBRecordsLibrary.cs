using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Office.RecordsManagement.RecordsRepository;

namespace WorkBoxFramework
{
    public class WBRecordsLibrary : IDisposable
    {
        #region Constants

        public const string PROTECTIVE_ZONE__PROTECTED = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;
        public const string PROTECTIVE_ZONE__PUBLIC_EXTRANET = WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET;
        public const string PROTECTIVE_ZONE__PUBLIC = WBRecordsType.PROTECTIVE_ZONE__PUBLIC;

        #endregion


        #region Properties

        public WBRecordsLibraries Libraries { get; private set; }

        private String _url = null;
        public String URL
        {
            get { return _url; }
        }

        private String _protectiveZone = null;
        public String ProtectiveZone 
        {
            get { return _protectiveZone; }
        }

        private bool _isOpen = false;
        private bool _openedByThisObject = false;
        public bool IsOpen
        {
            get { return _isOpen; }
        }


        private SPSite _site = null;        
        public SPSite Site
        {
            get {
                Open();
                return _site; 
            }
        }

        private SPWeb _web = null;
        public SPWeb Web
        {
            get {
                Open();
                return _web; 
            }
        }

        private SPList _list = null;
        public SPList List
        {
            get {
                Open();
                return _list; 
            }
        }

        private WBTaxonomy _recordsTypesTaxonomy = null;
        public WBTaxonomy RecordsTypesTaxonomy {
            get
            {
                if (_recordsTypesTaxonomy == null)
                {
                    _recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(Site);
                }
                return _recordsTypesTaxonomy;
            }
        }

        private WBTaxonomy _teamsTaxonomy = null;
        public WBTaxonomy TeamsTaxonomy {
            get
            {
                if (_teamsTaxonomy == null)
                {
                    _teamsTaxonomy = WBTaxonomy.GetTeams(RecordsTypesTaxonomy);
                }
                return _teamsTaxonomy;
            }
        }

        private WBTaxonomy _seriesTagsTaxonomy = null;
        public WBTaxonomy SeriesTagsTaxonomy
        {
            get
            {
                if (_seriesTagsTaxonomy == null)
                {
                    _seriesTagsTaxonomy = WBTaxonomy.GetSeriesTags(RecordsTypesTaxonomy);
                }
                return _seriesTagsTaxonomy;
            }
        }

        private WBTaxonomy _subjectTagsTaxonomy = null;
        public WBTaxonomy SubjectTagsTaxonomy
        {
            get
            {
                if (_subjectTagsTaxonomy == null)
                {
                    _subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(RecordsTypesTaxonomy);
                }
                return _subjectTagsTaxonomy;
            }
        }

        private WBTaxonomy _functionalAreasTaxonomy = null;
        public WBTaxonomy FunctionalAreasTaxonomy
        {
            get
            {
                if (_functionalAreasTaxonomy == null)
                {
                    _functionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(RecordsTypesTaxonomy);
                }
                return _functionalAreasTaxonomy;
            }
        }

        #endregion

        #region Constructors

        public WBRecordsLibrary(WBRecordsLibraries libraries, String url, String protectiveZone)
        {
            WBLogging.Debug("In WBRecordsLibrary() constructor for: " + url);

            Libraries = libraries;
            _url = url;
            _protectiveZone = protectiveZone;

            WBLogging.Debug("Finished WBRecordsLibrary() constructor for: " + url);
        }

        #endregion


        #region Methods
        public bool Open()
        {
            if (IsOpen) return false;
            if (String.IsNullOrEmpty(_url))
            {
                WBLogging.RecordsTypes.Unexpected("You can't open a WBRecordsLibrary if the URL is null or empty");
                throw new Exception("You can't open a WBRecordsLibrary if the URL is null or empty");
            }

            WBLogging.Debug("In WBRecordsLibrary().Open() for: " + _url);

            _site = new SPSite(_url);
            _web = _site.OpenWeb();
            _list = _web.GetList(_url);

            _openedByThisObject = true;

            _site.AllowUnsafeUpdates = true;
            _web.AllowUnsafeUpdates = true;

            _isOpen = true;

            WBLogging.Debug("Finished WBRecordsLibrary().Open() for: " + URL);
            return true;
        }


        public bool CloseAndMaybeDisposeParts()
        {
            if (!IsOpen) return false;

            if (_openedByThisObject)
            {
                _list = null;

                _web.Dispose();
                _web = null;

                _site.Dispose();
                _site = null;

                _openedByThisObject = false;
            }

            _isOpen = false;

            return true;
        }

        public WBDocument GetDocumentByID(String recordID)
        {
            if (!IsOpen) Open();

            SPListItem recordItem = WBUtils.FindItemByColumn(Site, List, WBColumn.RecordID, recordID);

            if (recordItem == null) return null;
            return new WBDocument(this, recordItem);
        }

        public SPListItemCollection GetLiveVersionsUpTo(String recordSeriesID, String recordID)
        {
            WBQuery query = new WBQuery();

            if (String.IsNullOrEmpty(recordSeriesID) || recordSeriesID == recordID)
            {
                query.AddFilter(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__LIVE);
                query.AddFilter(WBColumn.RecordID, WBQueryClause.Comparators.Equals, recordID);
            }
            else
            {
                query.AddFilter(WBColumn.RecordSeriesID, WBQueryClause.Comparators.Equals, recordSeriesID);
                query.AddFilter(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__LIVE);
                query.AddFilter(WBColumn.RecordID, WBQueryClause.Comparators.LessThanEquals, recordID);
            }

            query.OrderByDescending(WBColumn.RecordID);

            return List.WBxGetItems(Site, query);
        }


        public WBDocument this[String recordID]
        {
            get {
                return GetDocumentByID(recordID);
            }
        }

        public bool RemoveDocumentByID(String recordID)
        {
            if (!IsOpen) Open();

            WBLogging.Debug("Call to RemoveDocumentByID with recordID = " + recordID + " for library: " + this.URL);

            SPListItem recordItem = WBUtils.FindItemByColumn(Site, List, WBColumn.RecordID, recordID);

            if (recordItem == null)
            {
                // There is currently no such item - so there is nothing to remove.
                return false;
            }
            else
            {
                Records.UndeclareItemAsRecord(recordItem);
                recordItem.Delete();
                //libraryWeb.Update();

                return true;
            }
        }

        public WBDocument GetOrCreateRecordCopy(WBTaskFeedback feedback, WBRecord record)
        {
            WBDocument masterRecordDocument = record.ProtectedMasterRecord;
            WBDocument recordCopyDocument = GetDocumentByID(record.RecordID);

            if (recordCopyDocument == null)
            {
                Web.AllowUnsafeUpdates = true;

                bool forPublicWeb = true;
                if (ProtectiveZone == WBRecordsLibrary.PROTECTIVE_ZONE__PROTECTED) forPublicWeb = false;

                List<String> path = masterRecordDocument.Item.WBxGetFolderPath();

                SPFolder rootFolder = List.RootFolder;
                SPFolder actualDestinationFolder = rootFolder.WBxGetOrCreateFolderPath(path, forPublicWeb);

                string filename = masterRecordDocument.Item.Name;

                if (forPublicWeb)
                {
                    filename = WBUtils.PrepareFilenameForPublicWeb(filename);
                }

                if (Web.WBxFileExists(actualDestinationFolder, filename))
                {
                    throw new Exception("The file being copied already exists in the library - this should never happen!");
                }

                SPFile copiedFile = null;

                using (Stream stream = masterRecordDocument.OpenBinaryStream())
                {
                    copiedFile = actualDestinationFolder.Files.Add(filename, stream);
                    stream.Close();
                }

                recordCopyDocument = new WBDocument(this, copiedFile.Item);

                recordCopyDocument.MaybeCopyColumns(record.Metadata, WBRecord.DefaultColumnsToCopy);

                recordCopyDocument.Item.UpdateOverwriteVersion();

                // If the new file is checked out by this creation process - then check it in:
                if (copiedFile.CheckOutType != SPFile.SPCheckOutType.None)
                {
                    copiedFile.CheckIn("Document published here from a workbox. The original source URL was: " + masterRecordDocument.AbsoluteURL, SPCheckinType.MajorCheckIn);
                }

                Web.AllowUnsafeUpdates = false;

                recordCopyDocument.DebugName = "Copy of " + record.RecordID + " for: " + this.URL;

                if (feedback != null)
                {
                    String folderURL = recordCopyDocument.AbsoluteURL.Replace(recordCopyDocument.Name, "");

                    feedback.Created("Created copy: <a href='" + recordCopyDocument.AbsoluteURL + "' target='_blank'>" + recordCopyDocument.AbsoluteURL + "</a>");
                    feedback.Created("In folder: <a href='" + folderURL + "' target='_blank'>" + folderURL + "</a>");
                }
            }

            return recordCopyDocument;
        }


        public void Dispose()
        {
            CloseAndMaybeDisposeParts();
        }

        #endregion
    }
}
