﻿#region Copyright and License

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
            Dictionary
        }

        private Dictionary<WBColumn, Object> _dictionary = null;
        private SPListItem _listItem = null;
        private List<WBColumn> _usedColumns = new List<WBColumn>();


        #region Constructors

        public WBItem(SPListItem item)
        {
            _listItem = item;
            _dictionary = null;
            BackingType = BackingTypes.SPListItem;
        }

        public WBItem()
        {
            _listItem = null;
            _dictionary = new Dictionary<WBColumn, Object>();           
            BackingType = BackingTypes.Dictionary;
        }
        #endregion


        #region Properties

        public BackingTypes BackingType { get; private set; }


        public bool IsSPListItem { get { return (_listItem != null && BackingType == BackingTypes.SPListItem); } }
        public bool IsDictionaryItem { get { return (_dictionary != null && BackingType == BackingTypes.Dictionary); } } 


        #endregion

        #region Methods

        public void UseColumn(WBColumn column)
        {
            if (!_usedColumns.Contains(column)) _usedColumns.Add(column);
        }

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
                            WBLogging.Generic.Verbose("Setting WBItem metadata value backed by SPListItem using Column: " + column.DisplayName + "  Value: " + processedValue);
                            _listItem.WBxSet(column, processedValue);
                            break;
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

        public bool IsNotEmpty(WBColumn column)
        {
            UseColumn(column);

            return !IsNullOrEmpty(column);
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
            switch (BackingType)
            {
                case BackingTypes.SPListItem:
                    {
                        WBLogging.Generic.Verbose("Calling WBItem.Update() on item backed by SPListItem");
                        _listItem.Update();
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


        public IEnumerable<WBColumn> Columns
        {
            get { return _usedColumns.AsEnumerable(); }
        }

        #endregion
    }
}