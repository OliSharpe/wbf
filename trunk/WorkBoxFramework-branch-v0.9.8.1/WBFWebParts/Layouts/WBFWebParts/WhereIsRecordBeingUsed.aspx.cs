using System;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Publishing;
using WBFWebParts.PickRelatedDocuments;
using WorkBoxFramework;

namespace WBFWebParts.Layouts.WBFWebParts
{
    public partial class WhereIsRecordBeingUsed : LayoutsPageBase
    {

        SPSite _recordsSite = null;
        SPWeb _recordsWeb = null;
        SPList _recordsLibrary = null;

        public String recordID = "<i>(no record specified)</i>";
        String _itemId;
        public SPListItem _recordItem = null;
        public String recordTitle = "<i>(no record specified)</i>";
        public String recordName = "<i>(no record specified)</i>";
        public String recordURL = "<i>(no record specified)</i>";
        public String recordURLToSearchFor = "<i>(no record specified)</i>";


        protected void Page_Load(object sender, EventArgs e)
        {
            recordID = Page.Request.QueryString["RecordID"];

            //            _listId = Page.Request.QueryString["ListId"];
            _itemId = Page.Request.QueryString["ItemId"];


            if (String.IsNullOrEmpty(recordID) && String.IsNullOrEmpty(_itemId))
            {
                _recordItem = null;
                return;
            }


            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                String recordsLibraryURL = WBFWebPartsUtils.GetRecordsLibraryURL(SPContext.Current.Site);

                using (_recordsSite = new SPSite(recordsLibraryURL))
                using (_recordsWeb = _recordsSite.OpenWeb())
                {
                    _recordsLibrary = _recordsWeb.GetList(recordsLibraryURL);

                    if (String.IsNullOrEmpty(_itemId))
                    {
                        _recordItem = WBFWebPartsUtils.GetRecord(_recordsSite, _recordsWeb, _recordsLibrary, "", recordID);
                    }
                    else
                    {
                        _recordItem = _recordsLibrary.GetItemById(Convert.ToInt32(_itemId));
                        recordID = _recordItem.WBxGetAsString(WBColumn.RecordID);
                    }

                    recordURL = _recordsWeb.ServerRelativeUrl + "/" + _recordItem.Url;
                    recordURLToSearchFor = _recordItem.Url.Substring(_recordItem.ParentList.RootFolder.Url.Length);


                    if (_recordItem != null)
                    {
                        recordTitle = _recordItem.Title;
                        recordName = _recordItem.Name;
                    }
                }
            });

        }

    }
}
