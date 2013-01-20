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
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    /// <summary>
    /// This class provides an abstract represetnation of a column independently of which 
    /// content type or list is using it as a field.
    /// </summary>
    public class WBColumn : IEquatable<WBColumn>
    {
        #region Constants

        public const bool INTERNAL_NAME_USES_SPACE_CHARACTERS = true;
        public const bool INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS = false;

        public const string ASCENDING_COLUMN_IMAGE = "/_layouts/images/sort.gif";
        public const string DESCENDING_COLUMN_IMAGE = "/_layouts/images/rsort.gif";

        public enum DataTypes
        {
            Text,
            Integer,
            Count,
            DateTime,
            ManagedMetadata,
            Lookup,
            Boolean,
            Choice,
            URL,
            VirtualFormattedString,
            VirtualConditional,
            VirtualFileTypeIcon
        }
        
        #endregion

        #region Constructors

        private static Dictionary<String, WBColumn> _knownColumnsByInternalName = new Dictionary<String, WBColumn>();

        public WBColumn(String displayName, String internalName, DataTypes dataType)
        {
            DisplayName = displayName;
            InternalName = internalName;
            DataType = dataType;

            AllowMultipleValues = false;

            _knownColumnsByInternalName[InternalName] = this;
        }


        public WBColumn(String displayName, String internalName, String prettyName, DataTypes dataType)
        {
            DisplayName = displayName;
            InternalName = internalName;
            PrettyName = prettyName;
            DataType = dataType;

            AllowMultipleValues = false;

            _knownColumnsByInternalName[InternalName] = this;
        }

        public WBColumn(String displayName, bool internalNameHasSpaceCharacters, DataTypes dataType)
        {
            DisplayName = displayName;
            SetInternalName(internalNameHasSpaceCharacters);
            DataType = dataType;

            AllowMultipleValues = false;

            _knownColumnsByInternalName[InternalName] = this;
        }

        public WBColumn(String displayName, DataTypes dataType)
        {
            DisplayName = displayName;
            SetInternalName(INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS);
            DataType = dataType;

            AllowMultipleValues = false;
            
            _knownColumnsByInternalName[InternalName] = this;
        }
        #endregion

        #region Implementation for equalities checks:

        public bool Equals(WBColumn other)
        {
            if (other == null)
                return false;

            if (this.InternalName == other.InternalName)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            WBColumn column = obj as WBColumn;
            if (column == null)
                return false;
            else
                return Equals(column);
        }

        public override int GetHashCode()
        {
            return this.InternalName.GetHashCode();
        }

        public static bool operator ==(WBColumn column1, WBColumn column2)
        {
            if ((object)column1 == null || ((object)column2) == null)
                return Object.Equals(column1, column2);

            return column1.Equals(column2);
        }

        public static bool operator !=(WBColumn column1, WBColumn column2)
        {
            if (column1 == null || column2 == null)
                return !Object.Equals(column1, column2);

            return !(column1.Equals(column2));
        }


        #endregion


        #region Factories
        public static WBColumn TextColumn(String displayName, bool internalNameHasSpaceCharacters)
        {
            return new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.Text);
        }

        public static WBColumn TextColumn(String displayName)
        {
            return new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.Text);
        }

        public static WBColumn TextColumn(String displayName, String prettyName)
        {
            WBColumn textColumn = new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.Text);
            textColumn.PrettyName = prettyName;
            return textColumn;
        }

        public static WBColumn IntegerColumn(String displayName, bool internalNameHasSpaceCharacters)
        {
            return new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.Integer);
        }

        public static WBColumn IntegerColumn(String displayName)
        {
            return new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.Integer);
        }

        public static WBColumn IntegerColumn(String displayName, String prettyName)
        {
            WBColumn integerColumn = new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.Integer);
            integerColumn.PrettyName = prettyName;
            return integerColumn;
        }

        public static WBColumn CountColumn(String displayName, bool internalNameHasSpaceCharacters)
        {
            return new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.Count);
        }

        public static WBColumn CountColumn(String displayName)
        {
            return new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.Count);
        }

        public static WBColumn CountColumn(String displayName, String prettyName)
        {
            WBColumn integerColumn = new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.Count);
            integerColumn.PrettyName = prettyName;
            return integerColumn;
        }



        public static WBColumn DateTimeColumn(String displayName, bool internalNameHasSpaceCharacters)
        {
            return new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.DateTime);
        }

        public static WBColumn DateTimeColumn(String displayName, bool internalNameHasSpaceCharacters, String prettyName)
        {
            WBColumn column = new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.DateTime);
            column.PrettyName = prettyName;
            return column;
        }


        public static WBColumn DateTimeColumn(String displayName)
        {
            return new WBColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, DataTypes.DateTime);
        }

        public static WBColumn DateTimeColumn(String displayName, String internalName)
        {
            return new WBColumn(displayName, internalName, DataTypes.DateTime);
        }

        public static WBColumn DateTimeColumn(String displayName, String internalName, String prettyName)
        {
            WBColumn column =  new WBColumn(displayName, internalName, DataTypes.DateTime);
            column.PrettyName = prettyName;
            return column;
        }


        public static WBColumn ChoiceColumn(String displayName, bool internalNameHasSpaceCharacters, IEnumerable<String> choices)
        {
            WBColumn choiceColumn = new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.Choice);

            foreach (String choice in choices)
            {
                choiceColumn.Choices.Add(choice);
            }

            return choiceColumn;
        }

        public static WBColumn ChoiceColumn(String displayName, IEnumerable<String> choices)
        {
            return ChoiceColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, choices);
        }

        public static WBColumn ChoiceColumn(String displayName, String prettyName, IEnumerable<String> choices)
        {
            WBColumn column = ChoiceColumn(displayName, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, choices);
            column.PrettyName = prettyName;

            return column;
        }


        public static WBColumn ManagedMedataColumn(String displayName, bool internalNameHasSpaceCharacters, String termSetName, bool allowMultipleValues)
        {
            WBColumn column = new WBColumn(displayName, internalNameHasSpaceCharacters, DataTypes.ManagedMetadata);
            column.TermSetName = termSetName;
            column.AllowMultipleValues = allowMultipleValues;
            return column;
        }


        #endregion

        #region Properties
        /// <summary>
        /// The display name of the column. When creating a field of this type this will become the 'Title' of the field.
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// The internal name of the column.
        /// </summary>
        public String InternalName { get; set; }

        public DataTypes DataType { get; set; }

        public String TermSetName { get; set; }

        public bool IsVirtual {
            get
            {
                return (DataType == DataTypes.VirtualFileTypeIcon 
                    || DataType == DataTypes.VirtualConditional
                    || DataType == DataTypes.VirtualFormattedString);
            }
        }

        private List<String> _choices = null;
        public List<String> Choices 
        {
            get
            {
                if (_choices == null) _choices = new List<String>();
                return _choices;
            }
            set
            {
                _choices = value;
            } 
        }

        public String FormatString { get; set; }

        public List<WBColumn> FormatStringPlaceHolders { get; set; }

        public bool AllowMultipleValues { get; set; }

        // This is clearly starting to get ugly!! Should be done with subclasses really!!
        public String TestColumnInternalName { get; set; }

        public String TestColumnValue { get; set; }

        public String ValueIfEqual { get; set; }

        public String DataTypeName
        {
            get
            {
                return DataTypeToString(DataType);
            }
        }

        private String _prettyName = null;
        public String PrettyName
        {
            get
            {
                if (_prettyName == null) _prettyName = DisplayName;
                return _prettyName;
            }

            set { _prettyName = value; } 
        }


        #endregion

        #region Methods

        public void AddPlaceHolder(WBColumn column)
        {
            if (FormatStringPlaceHolders == null) FormatStringPlaceHolders = new List<WBColumn>();

            FormatStringPlaceHolders.Add(column);
        }


        public bool CreateIfDoesNotExist(SPSite site)
        {
            using (SPWeb rootWeb = site.RootWeb)
            {
                return CreateIfDoesNotExist(site, rootWeb);
            }        
        }

        public bool CreateIfDoesNotExist(SPSite site, SPWeb web)
        {
            if (String.IsNullOrEmpty(InternalName)) throw new NotImplementedException("Cannot create a column that doesn't have an internal name set!");
            if (String.IsNullOrEmpty(DisplayName)) throw new NotImplementedException("Cannot create a column that doesn't have a display name set!");
            if (DataType == null) throw new NotImplementedException("Cannot create a column that doesn't have a data type set!");

            if (web.Fields.ContainsField(InternalName) || web.Fields.ContainsField(DisplayName))
            {
                WBLogging.Generic.Monitorable("The SPWeb already has a column with either the internal name: " + InternalName + " or the display name: " + DisplayName);
                return false;
            }

            switch (DataType)
            {
                case DataTypes.Text:
                    {
                        SPFieldText textField = web.Fields.CreateNewField(SPFieldType.Text.ToString(), InternalName) as SPFieldText;
                        textField.Title = DisplayName;
                        textField.Group = "Work Box Framework";

                        web.Fields.Add(textField);
                        web.Update();

                        break;
                    }

                case DataTypes.Count:
                    {
                        SPFieldNumber numberField = web.Fields.CreateNewField(SPFieldType.Number.ToString(), InternalName) as SPFieldNumber;
                        numberField.Title = DisplayName;
                        numberField.Group = "Work Box Framework";

                        numberField.EnforceUniqueValues = true;
                        numberField.Indexed = true;
                        numberField.DisplayFormat = SPNumberFormatTypes.NoDecimal;

                        web.Fields.Add(numberField);
                        web.Update();

                        break;
                    }

                case DataTypes.Integer:
                    {
                        SPFieldNumber numberField = web.Fields.CreateNewField(SPFieldType.Number.ToString(), InternalName) as SPFieldNumber;
                        numberField.Title = DisplayName;
                        numberField.Group = "Work Box Framework";

                        numberField.DisplayFormat = SPNumberFormatTypes.NoDecimal;

                        web.Fields.Add(numberField);
                        web.Update();

                        break;
                    }

                case DataTypes.DateTime:
                    {
                        SPFieldDateTime dateTimeField = web.Fields.CreateNewField(SPFieldType.DateTime.ToString(), InternalName) as SPFieldDateTime;
                        dateTimeField.Title = DisplayName;
                        dateTimeField.Group = "Work Box Framework";

                        dateTimeField.DisplayFormat = SPDateTimeFieldFormatType.DateTime;

                        web.Fields.Add(dateTimeField);
                        web.Update();

                        break;
                    }


                case DataTypes.ManagedMetadata:
                    {
                        TaxonomySession session = new TaxonomySession(site);
                        TermStore termStore = session.TermStores[WorkBox.TERM_STORE_NAME];
                        Group group = termStore.Groups[WorkBox.TERM_STORE_GROUP_NAME];
                        TermSet termSet = group.TermSets[TermSetName];

                        TaxonomyField taxonomyField = web.Fields.CreateNewField("TaxonomyFieldType", InternalName) as TaxonomyField;
                        taxonomyField.Title = DisplayName;
                        taxonomyField.Group = "Work Box Framework";

                        taxonomyField.SspId = termStore.Id;
                        taxonomyField.TermSetId = termSet.Id;

                        taxonomyField.AllowMultipleValues = AllowMultipleValues;

                        taxonomyField.TargetTemplate = string.Empty;
                        taxonomyField.CreateValuesInEditForm = true;
                        taxonomyField.Open = false;
                        taxonomyField.AnchorId = Guid.Empty;

                        web.Fields.Add(taxonomyField);
                        web.Update();

                        break;
                    }

        //        case DataTypes.Lookup: return "Lookup";

                case DataTypes.Boolean:
                    {
                        SPFieldBoolean booleanField = web.Fields.CreateNewField(SPFieldType.Boolean.ToString(), InternalName) as SPFieldBoolean;
                        booleanField.Title = DisplayName;
                        booleanField.Group = "Work Box Framework";

                        web.Fields.Add(booleanField);
                        web.Update();

                        break;
                    }

                case DataTypes.Choice:
                    {
                        SPFieldChoice choiceField = web.Fields.CreateNewField(SPFieldType.Choice.ToString(), InternalName) as SPFieldChoice;
                        choiceField.Title = DisplayName;
                        choiceField.Group = "Work Box Framework";

                        choiceField.Choices.AddRange(Choices.ToArray());

                        web.Fields.Add(choiceField);
                        web.Update();

                        break;
                    }

                case DataTypes.URL:
                    {
                        SPFieldUrl urlField = web.Fields.CreateNewField(SPFieldType.URL.ToString(), InternalName) as SPFieldUrl;
                        urlField.Title = DisplayName;
                        urlField.Group = "Work Box Framework";

                        web.Fields.Add(urlField);
                        web.Update();

                        break;
                    }


                default:
                    {
                        throw new NotImplementedException("There is currently no implementation to create WBColumns of type: " + DataTypeName);                        
                    }
            }

            return true;
        }

        #endregion



        #region Standard Columns as Static Properties

        public static readonly WBColumn Name = new WBColumn("Name", "BaseName", DataTypes.Text);
        public static readonly WBColumn Title = WBColumn.TextColumn("Title");
        public static readonly WBColumn Modified = WBColumn.DateTimeColumn("Modified");
        public static readonly WBColumn ContentType = WBColumn.TextColumn("Content Type");

        public static readonly WBColumn ServerURL = WBColumn.TextColumn("ServerUrl", "Server URL");
        public static readonly WBColumn EncodedAbsoluteURL = WBColumn.TextColumn("EncodedAbsUrl", "Absolute URL");


        public static readonly WBColumn WorkBoxStatus = WBColumn.TextColumn(WorkBox.COLUMN_NAME__WORK_BOX_STATUS, "Status");
        public static readonly WBColumn WorkBoxURL = WBColumn.TextColumn(WorkBox.COLUMN_NAME__WORK_BOX_URL);
        public static readonly WBColumn WorkBoxGUID = WBColumn.TextColumn(WorkBox.COLUMN_NAME__WORK_BOX_GUID);

        public static readonly WBColumn WorkBoxUniqueID = WBColumn.TextColumn(WorkBox.COLUMN_NAME__WORK_BOX_UNIQUE_ID);
        public static readonly WBColumn WorkBoxLocalID = WBColumn.TextColumn(WorkBox.COLUMN_NAME__WORK_BOX_LOCAL_ID);

                               

        // These two are multiple lines of text:
        // WorkBoxAuditLog
        // WorkBoxErrorMessage

        //HyperLink:
        //WorkBoxLink

        // Choice
        //WorkBoxStatusChangeRequest

        //WorkBoxShortTitle

        public static readonly WBColumn WorkBoxCachedListItemID = WBColumn.IntegerColumn(WorkBox.COLUMN_NAME__WORK_BOX_CACHED_LIST_ITEM_ID);


        public static readonly WBColumn WorkBoxDateLastModified = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_DATE_LAST_MODIFIED, false, "Modified (approx)");
        public static readonly WBColumn WorkBoxDateLastVisited = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_DATE_LAST_VISITED, false, "Visited (approx)");
        public static readonly WBColumn WorkBoxDateCreated = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_DATE_CREATED, false, "Created");
        public static readonly WBColumn WorkBoxDateDeleted = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_DATE_DELETED, false, "Deleted");
        public static readonly WBColumn WorkBoxDateLastClosed = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_DATE_LAST_CLOSED, false, "Closed");
        public static readonly WBColumn WorkBoxDateLastOpened = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_DATE_LAST_OPENED, false, "Opened");
        public static readonly WBColumn WorkBoxRetentionEndDate = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__WORK_BOX_RETENTION_END_DATE, false, "Retention End Date");


        public static readonly WBColumn FunctionalArea = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__FUNCTIONAL_AREAS, true);
        public static readonly WBColumn RecordsType = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__RECORDS_TYPE, INTERNAL_NAME_USES_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__RECORDS_TYPES, false);
        public static readonly WBColumn SubjectTags = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__SUBJECT_TAGS, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__SUBJECT_TAGS, true);

        public static readonly WBColumn ReferenceID = WBColumn.TextColumn(WorkBox.COLUMN_NAME__REFERENCE_ID, false);
        public static readonly WBColumn ReferenceDate = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__REFERENCE_DATE);
        public static readonly WBColumn ScanDate = WBColumn.DateTimeColumn(WorkBox.COLUMN_NAME__SCAN_DATE);
        public static readonly WBColumn SeriesTag = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__SERIES_TAG, INTERNAL_NAME_HAS_NO_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__SERIES_TAGS, false);

        public static readonly WBColumn OwningTeam = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__OWNING_TEAM, INTERNAL_NAME_USES_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__TEAMS, false);
        public static readonly WBColumn InvolvedTeams = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, INTERNAL_NAME_USES_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__TEAMS, true);
        public static readonly WBColumn VisitingTeams = WBColumn.ManagedMedataColumn(WorkBox.COLUMN_NAME__VISITING_TEAMS, INTERNAL_NAME_USES_SPACE_CHARACTERS, WorkBox.TERM_SET_NAME__TEAMS, true);

        public static readonly WBColumn ProtectiveZone = WBColumn.ChoiceColumn(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, WBRecordsType.getProtectiveZones());

        public static readonly WBColumn DeclaredRecord = WBColumn.DateTimeColumn("Declared Record", "_vti_ItemDeclaredRecord", "Published Date");

        public static readonly WBColumn OriginalFilename = WBColumn.TextColumn(WorkBox.COLUMN_NAME__ORIGINAL_FILENAME);
        public static readonly WBColumn SourceSystem = WBColumn.TextColumn("Source System");
        public static readonly WBColumn SourceID = WBColumn.TextColumn("Source ID");
        public static readonly WBColumn RecordID = WBColumn.CountColumn("Record ID");

        

        // The Perspecuity source id column:
        public static readonly WBColumn Source_ID = WBColumn.TextColumn("Source_ID");

        public static readonly WBColumn MappingPath = WBColumn.TextColumn("Mapping Path");
        public static readonly WBColumn FunctionalAreaPath = WBColumn.TextColumn("Functional Area Path");
        public static readonly WBColumn RecordsTypePath = WBColumn.TextColumn("Records Type Path");
        public static readonly WBColumn SubjectTagsPaths = WBColumn.TextColumn("Subject Tags Paths");

        public static readonly WBColumn OwningTeamPath = WBColumn.TextColumn("Owning Team Path");

        public static readonly WBColumn SourceFilePath = WBColumn.TextColumn("Source File Path");

        public static readonly WBColumn ReferenceDateString = WBColumn.TextColumn("Reference Date String");
        public static readonly WBColumn DeclaredDateString = WBColumn.TextColumn("Declared Date String");
        public static readonly WBColumn ModifiedDateString = WBColumn.TextColumn("Modified Date String");
        public static readonly WBColumn ScanDateString = WBColumn.TextColumn("Scan Date String");

        public static readonly WBColumn FileTypeIcon = new WBColumn("File Type Icon", "FileTypeIcon", "", DataTypes.VirtualFileTypeIcon);
        public static readonly WBColumn FileSize = new WBColumn("File Size", "File_x0020_Size", "File Size in Bytes", DataTypes.Integer);

        public static readonly WBColumn DisplayFileSize = new WBColumn("Display File Size", "DisplayFileSize", "Size", DataTypes.VirtualFormattedString);
        public static readonly WBColumn TitleOrName = new WBColumn("Title Or Name", "TitleOrName", "Title", DataTypes.VirtualFormattedString);
        public static readonly WBColumn FileType = new WBColumn("File Type", "FileType", "Type", DataTypes.VirtualFormattedString);

        public const string FILE_OR_FOLDER__FILE = "File";
        public const string FILE_OR_FOLDER__FOLDER = "Folder";
        private static readonly string[] _fileOrFolderChoices = { "", FILE_OR_FOLDER__FILE, FILE_OR_FOLDER__FOLDER };
        public static readonly WBColumn FileOrFolder = WBColumn.ChoiceColumn("File Or Folder", _fileOrFolderChoices);

        public static readonly WBColumn DateMigrated = WBColumn.DateTimeColumn("Date Migrated");
        public static readonly WBColumn MigratedToUrl = WBColumn.TextColumn("Migrated To URL");

        public const string MIGRATION_STATUS__STILL_TO_DO = "";
        public const string MIGRATION_STATUS__ERROR = "Error";
        public const string MIGRATION_STATUS__DONE = "Done";
        public const string MIGRATION_STATUS__DUPLICATE = "Duplicate";
        private static readonly string[] _migrationStatusChoices = { MIGRATION_STATUS__STILL_TO_DO, MIGRATION_STATUS__ERROR, MIGRATION_STATUS__DONE, MIGRATION_STATUS__DUPLICATE };
        public static readonly WBColumn MigrationStatus = WBColumn.ChoiceColumn("Migration Status", _migrationStatusChoices);

        public static readonly WBColumn MigrationMessage = WBColumn.TextColumn("Migration Message");
        public static readonly WBColumn OriginalEntry = new WBColumn("Original Entry", DataTypes.Boolean);

        public const string LIVE_OR_ARCHIVED__LIVE = "Live";
        public const string LIVE_OR_ARCHIVED__ARCHIVED = "Archived";
        private static readonly string[] _liveOrArchivedChoices = { LIVE_OR_ARCHIVED__LIVE, LIVE_OR_ARCHIVED__ARCHIVED };
        public static readonly WBColumn LiveOrArchived = WBColumn.ChoiceColumn("Live Or Archived", "Live?", _liveOrArchivedChoices);


        #endregion

        #region public static methods

        public static WBColumn GetKnownColumnByInternalName(String internalName)
        {
            if (internalName == null || internalName == "") return null;
            return _knownColumnsByInternalName[internalName];
        }

        #endregion


        #region private methods
        private void SetInternalName(bool internalNameHasSpaceCharacters)
        {
            if (internalNameHasSpaceCharacters)
            {
                InternalName = DisplayName.Replace(" ", "_x0020_");
            }
            else
            {
                InternalName = DisplayName.Replace(" ", "");
            }
        }



        public static String DataTypeToString(DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.Text: return "Text";
                case DataTypes.Count: return "Count";
                case DataTypes.Integer: return "Integer";
                case DataTypes.DateTime: return "DateTime";
                case DataTypes.ManagedMetadata: return "ManagedMetadata";
                case DataTypes.Lookup: return "Lookup";
                case DataTypes.Boolean: return "Boolean";
                case DataTypes.Choice: return "Choice";
                case DataTypes.URL: return "URL";
                case DataTypes.VirtualFormattedString: return "VirtualFormattedString";
                case DataTypes.VirtualConditional: return "VirtualConditional";
                case DataTypes.VirtualFileTypeIcon: return "VirtualFileTypeIcon";
            }

            return "<<Unrecognised WBColumn.DataType>>";
        }

        #endregion



    }
}
