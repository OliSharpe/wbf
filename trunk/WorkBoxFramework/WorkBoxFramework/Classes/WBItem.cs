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
// The Work Box Framework (WBF) is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with the WBF.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    /// <summary>
    /// The WBItem provides a single wrapper for either an actual SPListItem or a 'virtual' item
    /// that is only backed by a Dictionary object. This allows the creation and manipulation of 
    /// such virtual items before creating an actual SPListItem.
    /// </summary>
    public class WBItem
    {
        public enum BackingTypes
        {
            SPListItem,
            SPListItemVersion,
            Dictionary
        }

        private Dictionary<WBColumn, Object> _dictionary = null;
        private SPListItem _listItem = null;
        private SPListItemVersion _listItemVersion = null;
        private List<WBColumn> _usedColumns = new List<WBColumn>();

        private bool _valuesHaveChanged = false;

        #region Constructors

        public WBItem(SPListItem item)
        {
            _listItem = item;
            _listItemVersion = null;
            _dictionary = null;
            BackingType = BackingTypes.SPListItem;
        }

        public WBItem(SPListItemVersion version)
        {
            _listItem = null;
            _listItemVersion = version;
            _dictionary = null;
            BackingType = BackingTypes.SPListItemVersion;
        }

        public WBItem()
        {
            _listItem = null;
            _listItemVersion = null;
            _dictionary = new Dictionary<WBColumn, Object>();           
            BackingType = BackingTypes.Dictionary;
        }

        public WBItem(Dictionary<String, String> values)
        {
            _listItem = null;
            _listItemVersion = null;
            _dictionary = new Dictionary<WBColumn, Object>();
            BackingType = BackingTypes.Dictionary;

            if (values != null)
            {
                foreach (String internalColumnName in values.Keys)
                {
                    WBColumn column = WBColumn.GetKnownColumnByInternalName(internalColumnName);
                    if (column == null) throw new Exception("In WBItem(Dictionary<,>): Not yet handling situation when an unknown internal name is used: " + internalColumnName);
                    this[column] = values[internalColumnName];
                }
            }

            _valuesHaveChanged = false;
        }
        #endregion


        #region Properties

        public BackingTypes BackingType { get; private set; }


        public bool IsSPListItem { get { return (_listItem != null && BackingType == BackingTypes.SPListItem); } }
        public bool IsSPListItemVersion { get { return (_listItemVersion != null && BackingType == BackingTypes.SPListItemVersion); } }
        public bool IsDictionaryItem { get { return (_dictionary != null && BackingType == BackingTypes.Dictionary); } }

        public bool ValuesHaveChanged { get { return _valuesHaveChanged; } }

        public SPListItem Item
        {
            get 
            {
                if (IsSPListItem) return _listItem;
                if (IsSPListItemVersion) return _listItemVersion.ListItem;
                return null;
            }
        }


        public SPListItemVersion ItemVersion
        {
            get
            {
                if (IsSPListItemVersion) return _listItemVersion;
                return null;
            }
        }


        #endregion

        #region Methods

        public void CheckForChangesFromNow()
        {
            _valuesHaveChanged = false;
        }

        public void UseColumn(WBColumn column)
        {
            if (!_usedColumns.Contains(column)) _usedColumns.Add(column);
        }

        public bool IsUsingColumn(WBColumn column)
        {
            if (IsSPListItem) return _listItem.WBxExists(column);
            if (IsSPListItemVersion) return _listItemVersion.WBxExists(column);
            return _usedColumns.Contains(column);
        }

        [System.Runtime.CompilerServices.IndexerName("Get")]
        public Object this[WBColumn column]
        {
            get 
            {
                UseColumn(column);

                Object value = null;

                switch (BackingType)
                {
                    case BackingTypes.SPListItem:
                        {
                            value = _listItem.WBxGet(column);
                            // WBLogging.Debug("Got value: " + value + " for column: " + column.DisplayName);
                            break;
                        }
                    case BackingTypes.SPListItemVersion:
                        {
                            value = _listItemVersion.WBxGet(column);
                            // WBLogging.Debug("Got value: " + value + " for column: " + column.DisplayName);
                            break;
                        }
                    case BackingTypes.Dictionary:
                        {
                            if (_dictionary.ContainsKey(column))
                            {
                                value = _dictionary[column];
                            }
                            break;
                        }
                    default: throw new NotImplementedException("The backing type selected has no implementation for public Object this[WBColumn column]");
                }

                // This approach allows for any generic column based processing of the returned value
                return value;
            }

            set 
            {
                UseColumn(column);
                _valuesHaveChanged = true;

                // First we might do some general processing of the value being set dependent on the type of column:
                Object processedValue = value;

                if (column.DataType == WBColumn.DataTypes.ManagedMetadata)
                {
                    processedValue = processedValue.WBxToString();
                }


                // Then we set the value in a way that depends on what is backing the item:
                switch (BackingType)
                {
                    case BackingTypes.SPListItem:
                        {
                            // WBLogging.Debug("Setting WBItem metadata value backed by SPListItem using Column: " + column.DisplayName + "  Value: " + processedValue);
                            _listItem.WBxSet(column, processedValue);
                            break;
                        }
                    case BackingTypes.SPListItemVersion:
                        {
                            throw new Exception("In call to set column value for a WBItem backed by SPListItemVersion which are read only so cannot have any value set");
                        }
                    case BackingTypes.Dictionary:
                        {
                            _dictionary[column] = processedValue;
                            break;
                        }
                    default: throw new NotImplementedException("The backing type selected has no implementation for public Object this[WBColumn column]");
                }
            }
        }

        public bool HasValue(WBColumn column)
        {
            if (IsSPListItem) return _listItem.WBxHasValue(column);
            if (IsSPListItemVersion) return _listItemVersion.WBxHasValue(column);
            return IsUsingColumn(column) && !IsNullOrEmpty(column); 
        }

        public bool IsNotEmpty(WBColumn column)
        {
            return HasValue(column);
        }

        public bool IsNullOrEmpty(WBColumn column)
        {
            UseColumn(column);

            switch (BackingType)
            {
                case BackingTypes.SPListItem:
                    {
                        return _listItem.WBxGetAsString(column).Trim() == "";
                    }
                case BackingTypes.SPListItemVersion:
                    {
                        return _listItemVersion.WBxGetAsString(column).Trim() == "";
                    }
                case BackingTypes.Dictionary:
                    {
                        if (_dictionary.ContainsKey(column))
                            return _dictionary[column].WBxToString().Trim() == "";
                        return true;
                    }
                default: throw new NotImplementedException("The backing type selected has no implementation for public Object this[WBColumn column]");
            }
        }

        public void Update()
        {
            UpdateAs(null);
        }


        public void UpdateAs(String callingUserLogin)
        {
            switch (BackingType)
            {
                case BackingTypes.SPListItem:
                    {

                        bool before = _listItem.Web.AllowUnsafeUpdates;
                        _listItem.Web.AllowUnsafeUpdates = true;

                        SPUser callingUser = _listItem.Web.WBxEnsureUserOrNull(callingUserLogin);

                        if (callingUser != null)
                        {
                            WBLogging.Debug("Calling WBItem.Update() on item backed by callingUserLogin = " + callingUserLogin + " and SPUser = " + callingUser.Name);
                            WBLogging.Generic.Verbose("Calling WBItem.Update() on item backed by callingUserLogin = " + callingUserLogin + " and SPUser = " + callingUser.Name);

                            _listItem.WBxSet(WBColumn.ModifiedBy, callingUser);
                            _listItem.WBxSet(WBColumn.Modified, DateTime.Now);
                        }
                        else
                        {
                            WBLogging.Generic.Verbose("Calling WBItem.Update() on item backed by SPListItem with no passed in user");
                            WBLogging.Debug("Calling WBItem.Update() on item backed by SPListItem with no passed in user");
                        }

                        _listItem.Update();

                        _listItem.Web.AllowUnsafeUpdates = before;
                        return;
                    }
                case BackingTypes.Dictionary:
                    {
                        // At the moment there is nothing to do
                        WBLogging.Debug("Called update on a WBItem derived class that is backed by a dictionary!");
                        return;
                    }
                default: throw new NotImplementedException("The backing type selected has no implementation for public Object this[WBColumn column]");
            }
        }

        public void Reload()
        {
            if (BackingType == BackingTypes.SPListItem)
            {
                _listItem = _listItem.ParentList.GetItemById(_listItem.ID);
            }
        }

        public IEnumerable<WBColumn> Columns
        {
            get { return _usedColumns.AsEnumerable(); }
        }

        public void CopyColumns(WBItem itemToCopy, IEnumerable<WBColumn> columnsToCopy)
        {
            foreach (WBColumn column in columnsToCopy)
            {
                WBLogging.Debug("Copying column: " + column.DisplayName);
                this[column] = itemToCopy[column];
            }
        }

        public void CopyColumns(WBItem itemToCopy)
        {
            IEnumerable<WBColumn> columnsToCopy = itemToCopy.Columns;

            foreach (WBColumn column in columnsToCopy)
            {
                WBLogging.Debug("Copying column: " + column.DisplayName);
                this[column] = itemToCopy[column];
            }
        }

        public bool MaybeCopyColumns(WBItem itemToCopy, IEnumerable<WBColumn> columnsToCopy)
        {
            // Let's pick up the item again before copying across the metadata:
            this.Reload();

            bool allCorrect = false;
            foreach (WBColumn column in columnsToCopy)
            {
                WBLogging.Debug("Copying column in MaybeCopyColumns(): " + column.DisplayName);
                if (this[column] != itemToCopy[column])
                {
                    allCorrect = false;
                    this[column] = itemToCopy[column];
                }
            }

            return allCorrect;
        }

        public SPUser GetSingleUserColumn(WBColumn column)
        {
            switch (BackingType)
            {
                case BackingTypes.SPListItem:
                    {
                        return _listItem.WBxGetSingleUserColumn(column);
                    }
                case BackingTypes.SPListItemVersion:
                    {
                        return _listItemVersion.WBxGetSingleUserColumn(column);
                    }
                default: throw new NotImplementedException("The backing type selected has no implementation for GetSingleUserColumn");
            }            
        }

        public List<SPUser> GetMultiUserColumn(WBColumn column)
        {
            switch (BackingType)
            {
                case BackingTypes.SPListItem:
                    {
                        return _listItem.WBxGetMultiUserColumn(column);
                    }
                case BackingTypes.SPListItemVersion:
                    {
                        return _listItemVersion.WBxGetMultiUserColumn(column);
                    }
                default: throw new NotImplementedException("The backing type selected has no implementation for GetMultiUserColumn");
            }
        }


        #endregion
    }
}
