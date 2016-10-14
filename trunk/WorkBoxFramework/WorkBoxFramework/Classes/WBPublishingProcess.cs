using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.SharePoint;
using Newtonsoft.Json;


namespace WorkBoxFramework
{
    public class WBPublishingProcess
    {

        public const String DOCUMENT_STATUS__UNPUBLISHED = "Unpublished";
        public const String DOCUMENT_STATUS__PUBLISHED = "Published";
        public const String DOCUMENT_STATUS__ERROR = "Error";

        public const String PUBLISH_MODE__JUST_ONE_DOCUMENT = "Just One Document";
        public const String PUBLISH_MODE__ONE_AT_A_TIME = "One At A Time";
        public const String PUBLISH_MODE__ALL_TOGETHER = "All Together";


        public const String REPLACE_ACTION__CREATE_NEW_SERIES = "Create New Series";
//        public const String REPLACE_ACTION__LEAVE_ON_PUBLIC = "Leave On Public";
        public const String REPLACE_ACTION__LEAVE_ON_IZZI = "Leave On izzi";
        public const String REPLACE_ACTION__ARCHIVE_FROM_IZZI = "Archive From izzi";

        #region Constructors

        public WBPublishingProcess()
        {
        }

        public WBPublishingProcess(WorkBox workBox, String listGUID, IEnumerable<String> listOfItemIDs)
        {
            this._workBox = workBox;
            this.WorkBoxURL = workBox.Url;
            this.ListGUID = listGUID;

            // If this GUID is in the format with brackets around the numbers - then we're going to strip those brackets:
            if (listGUID[0] == '{') this.ListGUID = listGUID.Substring(1, listGUID.Length - 2).ToLower();

            this.ItemStatus = new Dictionary<String, String>();
            foreach (String itemID in listOfItemIDs)
            {
                if (!String.IsNullOrEmpty(itemID))
                {
                    this.ItemStatus.Add(itemID, DOCUMENT_STATUS__UNPUBLISHED);
                }
            }

            this.ProtectiveZone = WBRecordsType.PROTECTIVE_ZONE__PROTECTED;

            this.TeamFunctionalAreasUIControlValue = workBox.OwningTeam.InheritedFunctionalAreaUIControlValue;

            this.OwningTeamUIControlValue = workBox.OwningTeam.UIControlValue;
            this.InvolvedTeamsUIControlValue = workBox.InvolvedTeams.UIControlValue;

            this.ReplaceAction = REPLACE_ACTION__CREATE_NEW_SERIES;

            if (ItemStatus.Count > 1)
            {
                this.PublishMode = PUBLISH_MODE__ONE_AT_A_TIME;
            }
            else
            {
                this.PublishMode = PUBLISH_MODE__JUST_ONE_DOCUMENT;
            }


            // This is here just to speed up some testing by jumping some pages!
            //this.RecordsTypeUIControlValue = "Tenders|82148fdb-301c-4fdd-8072-0fdcf19fd84d"; //workBox.RecordsType.UIControlValue;  //
            //this.FunctionalAreaUIControlValue = this.TeamFunctionalAreasUIControlValue;
        }
        #endregion

        #region JSON Properties

        [JsonProperty]
        public String WorkBoxURL { get; set; }

        [JsonProperty]
        public String ListGUID { get; set; }

        [JsonProperty]
        public Dictionary<String,String> ItemStatus { get; set; }

        private Dictionary<String, String> _mappedFilenames = null;
        [JsonProperty]
        public Dictionary<String, String> MappedFilenames
        {
            get
            {
                if (_mappedFilenames == null)
                {
                    _mappedFilenames = new Dictionary<String, String>();
                    foreach (SPListItem item in Items)
                    {
                        _mappedFilenames.Add(item.ID.ToString(), item.Name);
                    }
                }
                return _mappedFilenames;
            }
            set
            {
                _mappedFilenames = value;
            }
        }

        [JsonProperty]
        public String PublishMode { get; set; }

        [JsonProperty]
        public String ProtectiveZone { get; set; }

        [JsonProperty]
        public String FunctionalAreaUIControlValue { get; set; }

        [JsonProperty]
        public String TeamFunctionalAreasUIControlValue { get; set; }

        [JsonProperty]
        public String RecordsTypeUIControlValue { get; set; }

        [JsonProperty]
        public String SubjectTagsUIControlValue { get; set; }

        [JsonProperty]
        public String OwningTeamUIControlValue { get; set; }

        [JsonProperty]
        public String InvolvedTeamsUIControlValue { get; set; }

        [JsonProperty]
        public String WebPageURL { get; set; }

        private String _currentItemID = null;
        [JsonProperty]
        public String CurrentItemID
        {
            get
            {
                if (String.IsNullOrEmpty(_currentItemID) || ItemStatus[_currentItemID] == DOCUMENT_STATUS__PUBLISHED)
                {
                    _currentItemID = null;

                    foreach (String itemID in ItemStatus.Keys)
                    {
                        if (ItemStatus[itemID] == DOCUMENT_STATUS__UNPUBLISHED)
                        {
                            _currentItemID = itemID;
                            break;
                        }
                    }
                }
                return _currentItemID;
            }
            set
            {
                _currentItemID = value;
                _currentItem = null;
                _currentShortTitle = null;
            }
        }

        private String _currentShortTitle = null;
        [JsonProperty]
        public String CurrentShortTitle
        {
            get
            {
                if (_currentShortTitle == null)
                {
                    if (CurrentItem != null)
                    {
                        _currentShortTitle = CurrentItem.Title;
                        if (String.IsNullOrEmpty(_currentShortTitle))
                        {
                            _currentShortTitle = Path.GetFileNameWithoutExtension(CurrentItem.Name);
                            if (_currentShortTitle.Length > 0 && _currentShortTitle[0] == '(')
                            {
                                int indexOfClosingBracket = _currentShortTitle.IndexOf(')');
                                if (indexOfClosingBracket > -1 && indexOfClosingBracket < _currentShortTitle.Length - 2)
                                {
                                    _currentShortTitle = _currentShortTitle.Substring(indexOfClosingBracket + 1);
                                }
                            }
                        }
                    }
                }
                return _currentShortTitle;
            }
            set
            {
                _currentShortTitle = value;
            }
        }



        [JsonProperty]
        public String ToReplaceRecordID { get; set; }

        [JsonProperty]
        public String ToReplaceRecordPath { get; set; }

        [JsonProperty]
        public String ReplaceAction { get; set; }

        [JsonProperty]
        public Dictionary<String,String> SelfApprovalDictionary { get; set; }

        #endregion

        #region Non-JSON Properties

        private SPList _list = null;
        [JsonIgnore]
        public SPList List
        {
            get
            {
                if (_list == null)
                {
                    WBLogging.Debug("In WBPublishingProcess.List get : _list was null");

                    Guid guid = new Guid(ListGUID);
                    _list = WorkBox.Web.Lists[guid];
                }
                return _list;
            }
        }

        private List<SPListItem> _items = null;
        [JsonIgnore]
        public List<SPListItem> Items
        {
            get
            {
                if (_items == null)
                {
                    WBLogging.Debug("In WBPublishingProcess.Items get : _items was null");

                    _items = new List<SPListItem>();

                    foreach (String itemID in ItemStatus.Keys)
                    {
                        _items.Add(List.GetItemById(int.Parse(itemID)));
                    }
                }
                return _items;
            }
        }

        [JsonIgnore]
        public ICollection<String> ItemIDs
        {
            get
            {
                return ItemStatus.Keys;
            }
        }
        
        private List<String> _listOfFilenames = null;
        [JsonIgnore]
        public List<String> ListOfFilenames
        {
            get {
                if (_listOfFilenames == null)
                {
                    _listOfFilenames = new List<String>();
                    foreach (String itemID in ItemIDs)
                    {
                        _listOfFilenames.Add(MappedFilenames[itemID]);
                    }
                }
                return _listOfFilenames;
            }
        }
             
        private SPListItem _currentItem = null;
        [JsonIgnore]
        public SPListItem CurrentItem
        {
            get
            {
                if (_currentItem == null)
                {
                    if (!String.IsNullOrEmpty(CurrentItemID))
                    {
                        _currentItem = List.GetItemById(int.Parse(CurrentItemID));
                    }
                }
                return _currentItem;
            }
        }

        private WorkBox _workBox = null;
        [JsonIgnore]
        public WorkBox WorkBox {
            get
            {
                if (_workBox == null)
                {
                    if (SPContext.Current != null && WorkBox.IsWebAWorkBox(SPContext.Current.Web))
                    {
                        _workBox = new WorkBox(SPContext.Current);
                    }
                }

                // Currently NOT fetching the work box if it's not been set! - could do from context and then from URL
                return _workBox;
            }
            set
            {
                _workBox = value;
            }
        }

        [JsonIgnore]
        public WBItem SelfApprovalItem { get; set; }


        private WBTaxonomy _recordsTypeTaxonomy = null;
        [JsonIgnore]
        public WBTaxonomy RecordsTypeTaxonomy
        {
            get
            {
                if (_recordsTypeTaxonomy == null)
                {
                    _recordsTypeTaxonomy = WBTaxonomy.GetRecordsTypes(WorkBox.Site);
                }
                return _recordsTypeTaxonomy;
            }
        }


        private WBTaxonomy _teamsTaxonomy = null;
        [JsonIgnore]
        public WBTaxonomy TeamsTaxonomy
        {
            get
            {
                if (_teamsTaxonomy == null)
                {
                    _teamsTaxonomy = WBTaxonomy.GetTeams(RecordsTypeTaxonomy);
                }
                return _teamsTaxonomy;
            }
        }


        private WBTaxonomy _seriesTagsTaxonomy = null;
        [JsonIgnore]
        public WBTaxonomy SeriesTagsTaxonomy
        {
            get
            {
                if (_seriesTagsTaxonomy == null)
                {
                    _seriesTagsTaxonomy = WBTaxonomy.GetSeriesTags(RecordsTypeTaxonomy);
                }
                return _seriesTagsTaxonomy;
            }
        }

        private WBTaxonomy _subjectTagsTaxonomy = null;
        [JsonIgnore]
        public WBTaxonomy SubjectTagsTaxonomy
        {
            get
            {
                if (_subjectTagsTaxonomy == null)
                {
                    _subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(RecordsTypeTaxonomy);
                }
                return _subjectTagsTaxonomy;
            }
        }

        private WBTaxonomy _functionalAreasTaxonomy = null;
        [JsonIgnore]
        public WBTaxonomy FunctionalAreasTaxonomy
        {
            get
            {
                if (_functionalAreasTaxonomy == null)
                {
                    _functionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(RecordsTypeTaxonomy);
                }
                return _functionalAreasTaxonomy;
            }
        }

        [JsonIgnore]
        public bool IsReplaceActionToCreateNewSeries
        {
            get
            {
                return (ReplaceAction == REPLACE_ACTION__CREATE_NEW_SERIES);
            }
        }

        [JsonIgnore]
        public bool AllowBulkPublishAllTogether
        {
            get
            {
                int countStillToPublish = 0;
                bool allPDFs = true;
                foreach (String itemID in ItemIDs)
                {
                    if (ItemStatus[itemID] == DOCUMENT_STATUS__UNPUBLISHED)
                    {
                        countStillToPublish++;
                        if (Path.GetExtension(MappedFilenames[itemID]).WBxTrim().ToLower().Replace(".", "") != "pdf")
                        {
                            allPDFs = false;
                        }
                    }
                }

                return (countStillToPublish > 1 && (ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PROTECTED || allPDFs));
            }
        }



        [JsonIgnore]
        public bool HasMoreDocumentsToPublish
        {
            get
            {
                return !String.IsNullOrEmpty(CurrentItemID);
            }
        }

        [JsonIgnore]
        public WBTaskFeedback LastTaskFeedback { get; set; }

        #endregion


        #region Methods

        public String GetInvolvedTeamsWithoutOwningTeamAsUIControlValue()
        {
            WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(TeamsTaxonomy, InvolvedTeamsUIControlValue);

            if (!String.IsNullOrEmpty(OwningTeamUIControlValue))
            {
                WBTeam owningTeam = new WBTeam(TeamsTaxonomy, OwningTeamUIControlValue);

                involvedTeams.Remove(owningTeam);
            }

            return involvedTeams.UIControlValue;
        }

        public void SetInvolvedTeamsWithoutOwningTeamAsUIControlValue(String newInvolvedTeamsUIControlValue)
        {
            WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(TeamsTaxonomy, newInvolvedTeamsUIControlValue);
            if (!String.IsNullOrEmpty(OwningTeamUIControlValue))
            {
                WBTeam owningTeam = new WBTeam(TeamsTaxonomy, OwningTeamUIControlValue);
                if (!involvedTeams.Contains(owningTeam))
                {
                    involvedTeams.Add(owningTeam);
                }
            }

            InvolvedTeamsUIControlValue = involvedTeams.UIControlValue;
        }

        public void ReloadCurrentItem()
        {
            _currentItem = List.GetItemById(int.Parse(CurrentItemID));
        }

        public void CurrentItemFailed()
        {
            this.ItemStatus[this.CurrentItemID] = WBPublishingProcess.DOCUMENT_STATUS__ERROR;
            this.CurrentItemID = "";

            // You certainly wouldn't want to replace the same document again - so we revert replace action back to 'new' for the moment:
            this.ReplaceAction = REPLACE_ACTION__CREATE_NEW_SERIES;
            this.ToReplaceRecordID = null;
            this.ToReplaceRecordPath = null;
        }


        public void CurrentItemSucceeded()
        {
            this.ItemStatus[this.CurrentItemID] = WBPublishingProcess.DOCUMENT_STATUS__PUBLISHED;
            this.CurrentItemID = "";

            // You certainly wouldn't want to replace the same document again - so we revert replace action back to 'new' for the moment:
            this.ReplaceAction = REPLACE_ACTION__CREATE_NEW_SERIES;
            this.ToReplaceRecordID = null;
            this.ToReplaceRecordPath = null;
        }

        public String GetStandardHTMLTableRows()
        {
            SPListItem currentItem = this.CurrentItem;
            String html = "";
            if (this.ItemIDs.Count == 0)
            {
                WBLogging.Debug("process.ItemIDs.Count == 0");
                html += "<i>No documents selected!</i>";
            }
            else
            {
                int numberOfDocuments = this.ItemIDs.Count;
                int itemIndex = -1;
                int currentItemIndex = -1;
                bool before = true;
                foreach (String itemID in this.ItemIDs)
                {
                    itemIndex++;

                    String filename = this.MappedFilenames[itemID];

                    WBLogging.Debug("list through item with name: " + filename);
                    if (itemID == this.CurrentItemID)
                    {
                        currentItemIndex = itemIndex;
                        if (itemIndex != 0)
                        {
                            // OK so we've got to close the table row of the already published documents:
                            html += @"
    </td>
</tr>";
                        }

                        before = false;

                        if (this.PublishMode != PUBLISH_MODE__ALL_TOGETHER)
                        {
                            String originalFilename = "";
                            if (currentItem != null)
                            {
                                originalFilename = currentItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__ORIGINAL_FILENAME);
                            }

                            html += @"
<tr>
    <td class=""wbf-field-name-panel"">
        <div class=""wbf-field-name"">Publishing Document</div>
    </td>
    <td class=""wbf-field-value-panel"">
        <div class=""wbf-field-read-only-title"">
            <table border=""0"" cellpadding=""0"" cellspacing=""2px"">
                <tr>
                    <td rowspan=""2"" style=""padding-right: 10px; "">
                        <img src='" + WBUtils.DocumentIcon32(filename) + "' alt='Icon for file " + filename + @"'/>
                    </td>
                    <td>" + filename + @"</td>
                </tr>
                <tr>
                    <td>" + originalFilename + @"</td>
                </tr>
            </table>
        </div>
    </td>
</tr>
";
                        }
                        else
                        {
                            // So if we're here then we are publishing all together - so let's slightly change the layout:
                            html += @"
<tr>
    <td class=""wbf-field-name-panel"">
        <div class=""wbf-field-name"">Publishing Documents</div>
    </td>
    <td class=""wbf-field-value-panel"">
        <div>
            <img src='/_layouts/images/WorkBoxFramework/list-item-16.png' alt='Unpublished document'/>
            <img src='" + WBUtils.DocumentIcon16(filename) + "' alt='Icon for file " + filename + "'/> " + filename + @"
        </div>
";
                        }                       
                    }
                    else
                    {
                        if (before)
                        {
                            String statusIcon = "/_layouts/images/WorkBoxFramework/green-tick-16.png";
                            String statusAltText = "Successfully published";
                            if (this.ItemStatus[itemID] == WBPublishingProcess.DOCUMENT_STATUS__ERROR)
                            {
                                statusIcon = "/_layouts/images/WorkBoxFramework/red-cross-16.png";
                                statusAltText = "Failed to publish";
                            }

                            if (itemIndex == 0)
                            {
                                // OK so let's start the table row of the documents that have already been published:
                                html += @"
<tr>
    <td class=""wbf-field-name-panel"">
        <div class=""wbf-field-name""></div>
    </td>
    <td class=""wbf-field-name-panel"">
";

                            }

                            html += @"
        <div>
            <img src='" + statusIcon + "' alt='" + statusAltText + @"'/>
            <img src='" + WBUtils.DocumentIcon16(filename) + "' alt='Icon for file " + filename + "'/> " + filename + @"
        </div>
";

                        }
                        else
                        {
                            if (itemIndex == currentItemIndex + 1 && this.PublishMode != PUBLISH_MODE__ALL_TOGETHER)
                            {
                                html += @"
<tr>
    <td class=""wbf-field-name-panel"">
        <div class=""wbf-field-name""></div>
    </td>
    <td class=""wbf-field-name-panel"">
";
                            }

                            html += @"
        <div>
            <img src='/_layouts/images/WorkBoxFramework/list-item-16.png' alt='Unpublished document'/>
            <img src='" + WBUtils.DocumentIcon16(filename) + "' alt='Icon for file " + filename + "'/> " + filename + @"
        </div>
";

                            if (itemIndex == numberOfDocuments - 1)
                            {
                                html += @"
    </td>
</tr>";
                            }
                        }
                    }
                }
            }
            return html;
        }



        #endregion

    }
}
