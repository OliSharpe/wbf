using System;
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
            get { return _site; }
        }

        private SPWeb _web = null;
        public SPWeb Web
        {
            get { return _web; }
        }

        private SPList _list = null;
        public SPList List
        {
            get { return _list; }
        }
        #endregion

        #region Constructors

        public WBRecordsLibrary(String url, String protectiveZone)
        {
            _url = url;
            _protectiveZone = protectiveZone;
        }

        #endregion


        #region Methods
        public bool Open()
        {
            if (IsOpen) return false;
            if (String.IsNullOrEmpty(URL)) {
                WBLogging.RecordsTypes.Unexpected("You can't open a WBRecordsLibrary if the URL is null or empty");
                throw new Exception("You can't open a WBRecordsLibrary if the URL is null or empty");
            }

            _site = new SPSite(URL);
            _web = Site.OpenWeb();
            _list = _web.GetList(URL);

            _openedByThisObject = true;

            _site.AllowUnsafeUpdates = true;
            _web.AllowUnsafeUpdates = true;

            _isOpen = true;

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
            return new WBDocument(recordItem);
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

        public WBDocument GetOrCreateCopyFromMaster(WBDocument masterRecord)
        {
            WBDocument document = GetDocumentByID(masterRecord.RecordID);

            if (document == null)
            {
                // OK So i haven't implemented this yet!!
                throw new NotImplementedException("Not yet done!!");
            }

            return document;
        }


        public void Dispose()
        {
            CloseAndMaybeDisposeParts();
        }

        #endregion
    }
}
