#region Copyright and License

// Copyright (c) Islington Council 2010-2016
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
using System.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class TreeViewLocation : IHierarchyData
    {
        public const string LOCATION_TYPE__FUNCTIONAL_AREA = "Functional Area";
        public const string LOCATION_TYPE__RECORDS_TYPE = "Records Type";
        public const string LOCATION_TYPE__FOLDER = "Folder";
        public const string LOCATION_TYPE__DOCUMENT = "Document";

        public const string VIEW_MODE__NEW = "New";
        public const string VIEW_MODE__REPLACE = "Replace";
        public const string VIEW_MODE__BROWSE_FOLDERS = "Browse Folders";
        public const string VIEW_MODE__BROWSE_DOCUMENTS = "Browse Documents";
        

        private string _type = null;
        private WBRecordsManager _manager = null;
        private string _minimumProtectiveZone = null;
        private string _mode = null;
        private WBTerm _functionalArea;
        private WBRecordsType _recordsType;
        private SPFolder _folder;
        private WBDocument _masterRecord;
        private string _guidString;
        private TreeViewLocation _parent;
        private TreeViewLocationCollection _children;
        private string _name;
        private bool _thenDocuments = false;

        /// <summary>
        /// Hide the default public constructor
        /// </summary>
        private TreeViewLocation()
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="term">The underlying Term object that is being wrapped</param>
        public TreeViewLocation(TreeViewLocation parent, WBRecordsManager manager, string mode, string minimumProtectiveZone, WBTerm functionalArea)
        {
            _type = LOCATION_TYPE__FUNCTIONAL_AREA;

            _parent = parent;
            _manager = manager;
            _mode = mode;
            _minimumProtectiveZone = minimumProtectiveZone;

            _functionalArea = functionalArea;
            _name = functionalArea.Name;
            _guidString = functionalArea.Id.ToString();

            if (_mode != VIEW_MODE__NEW)
            {
                _folder = manager.Libraries.GetMasterFolderByPath(functionalArea.Name);
            }
        }

        public TreeViewLocation(TreeViewLocation parent, WBRecordsManager manager, string mode, string minimumProtectiveZone, WBTerm functionalArea, WBRecordsType recordsType)
        {
            _type = LOCATION_TYPE__RECORDS_TYPE;

            _parent = parent;
            _manager = manager;
            _mode = mode;
            _minimumProtectiveZone = minimumProtectiveZone;

            _functionalArea = functionalArea;
            _recordsType = recordsType;
            _name = _recordsType.Name;
            _guidString = _recordsType.Id.ToString();

            if (_mode != VIEW_MODE__NEW)
            {
                _folder = _parent._folder.WBxGetSubFolder(recordsType.Name);
                if (_folder == null) WBLogging.Debug("Did not find folder for: " + recordsType.Name);
            }
        }

        public TreeViewLocation(TreeViewLocation parent, WBRecordsManager manager, string mode, string minimumProtectiveZone, SPFolder folder)
        {
            _type = LOCATION_TYPE__FOLDER;

            _parent = parent;
            _manager = manager;
            _mode = mode;
            _minimumProtectiveZone = minimumProtectiveZone;

            _folder = folder;
            _name = _folder.Name;
            _guidString = _folder.Item.ID.ToString();           
        }

        public TreeViewLocation(TreeViewLocation parent, WBRecordsManager manager, string mode, string minimumProtectiveZone, WBDocument masterRecord)
        {
            _type = LOCATION_TYPE__DOCUMENT;

            _parent = parent;
            _manager = manager;
            _mode = mode;
            _minimumProtectiveZone = minimumProtectiveZone;

            _masterRecord = masterRecord;
            _name = _masterRecord.Name;
            _guidString = _masterRecord.Item.ID.ToString();           
        }


        #region IHierarchyData Members

        public IHierarchicalEnumerable GetChildren()
        {
            if (_children == null)
            {
                _children = new TreeViewLocationCollection();

                switch (_type) {
                    case LOCATION_TYPE__FUNCTIONAL_AREA: {

                        WBLogging.Debug("In GetChildren() for type Functional Area");
                        WBTaxonomy recordsTypes = _manager.RecordsTypesTaxonomy;
                        TermCollection terms = recordsTypes.TermSet.Terms;

                        foreach (Term childTerm in terms)
                        {
                            WBRecordsType recordsType = new WBRecordsType(recordsTypes, childTerm);
                            bool protectiveZoneOK = true;
                            //if (!String.IsNullOrEmpty(_minimumProtectiveZone))
                            //{
                             //   protectiveZoneOK = (recordsType.IsZoneAtLeastMinimum(_minimumProtectiveZone));
                           // }

                            if (recordsType.BranchCanHaveDocuments() && recordsType.IsRelevantToFunctionalArea(_functionalArea) && protectiveZoneOK)
                            {
                                TreeViewLocation newLocation = new TreeViewLocation(this, _manager, _mode, _minimumProtectiveZone, _functionalArea, recordsType);

                                // If we're looking for existing records then we'll only add this location if it has a real folder existing underneath it:
                                if (_mode == VIEW_MODE__NEW || newLocation._folder != null)
                                {
                                    _children.Add(newLocation);
                                }
                            }
                            else
                            {
                                WBLogging.Debug("In GetChildren() excluded " + recordsType.Name + " because " + recordsType.BranchCanHaveDocuments() + " && " + protectiveZoneOK);

                            }
                        }

                        break;
                    }

                    case LOCATION_TYPE__RECORDS_TYPE: {
                        WBLogging.Debug("In GetChildren() for type Records Type");
                        WBTaxonomy recordsTypes = _manager.RecordsTypesTaxonomy;

                        TermCollection terms = _recordsType.Term.Terms;
                        if (terms.Count > 0) {
                            foreach (Term childTerm in terms)
                            {
                                WBRecordsType recordsType = new WBRecordsType(recordsTypes, childTerm);
                                bool protectiveZoneOK = true;
                                if (!String.IsNullOrEmpty(_minimumProtectiveZone))
                                {
                                    protectiveZoneOK = (recordsType.IsZoneAtLeastMinimum(_minimumProtectiveZone));
                                }

                                if (recordsType.BranchCanHaveDocuments() && recordsType.IsRelevantToFunctionalArea(_functionalArea) && protectiveZoneOK)
                                {
                                    TreeViewLocation newLocation = new TreeViewLocation(this, _manager, _mode, _minimumProtectiveZone, _functionalArea, recordsType);

                                    // If we're looking for existing records then we'll only add this location if it has a real folder existing underneath it:
                                    if (_mode == VIEW_MODE__NEW || newLocation._folder != null)
                                    {
                                        _children.Add(newLocation);
                                    }
                                }
                            }
                        } else {

                            if (_mode != VIEW_MODE__NEW)
                            {
                                // WBLogging.Debug("In view mode replace switching to folders part of tree");

                                string fullClassPath = WBUtils.NormalisePath(Path);

                                // WBLogging.Debug("Looking for starting folder = " + fullClassPath);

                                SPFolder protectedLibraryRootFolder = _manager.Libraries.ProtectedMasterLibrary.List.RootFolder;

                                // WBLogging.Debug("Got library root folder");

                                SPFolder recordsTypeFolder = protectedLibraryRootFolder.WBxGetFolderPath(fullClassPath);

                                // WBLogging.Debug("Got records type folder - definitely changed .. " + recordsTypeFolder);
                                if (recordsTypeFolder != null)
                                {
                                    foreach (SPFolder child in recordsTypeFolder.SubFolders)
                                    {
                                        _children.Add(new TreeViewLocation(this, _manager, _mode, _minimumProtectiveZone, child));
                                    }
                                }
                                else
                                {
                                    WBLogging.Debug("The master library doesn't have a folder with path: " + fullClassPath);
                                }

                                // WBLogging.Debug("Added children folders");

                            }
                        }

                        break;
                    }

                    case LOCATION_TYPE__FOLDER: {
                        WBLogging.Debug("In GetChildren() for type Folder");

                        if (_folder.SubFolders.Count > 0)
                        {
                            foreach (SPFolder child in _folder.SubFolders)
                            {
                                _children.Add(new TreeViewLocation(this, _manager, _mode, _minimumProtectiveZone, child));
                            }
                        }
                        else
                        {
                            if (_mode == VIEW_MODE__REPLACE)
                            {
                                SPListItemCollection items = GetItemsRecursive(_folder);
                                foreach (SPListItem item in items)
                                {
                                    if (ItemCanBePicked(item))
                                    {
                                        _children.Add(new TreeViewLocation(this, _manager, _mode, _minimumProtectiveZone, new WBDocument(_manager.Libraries.ProtectedMasterLibrary, item)));
                                    }
                                }
                            }
                        }
                        break;
                    }
                    
                    case LOCATION_TYPE__DOCUMENT: {
                        WBLogging.Debug("In GetChildren() for type Document");

                        break;
                    }
                }
            }

            return _children;
        }

        private bool ItemCanBePicked(SPListItem item)
        {
            if (item == null) return false;

            if (String.IsNullOrEmpty(item.WBxGetAsString(WBColumn.RecordID))) return false;
            if (item.WBxGetAsString(WBColumn.LiveOrArchived) == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED) return false;

            String recordSeriesStatus = item.WBxGetAsString(WBColumn.RecordSeriesStatus);
            if (recordSeriesStatus != "Latest" && !String.IsNullOrEmpty(recordSeriesStatus)) return false;

            String itemProtectiveZone = item.WBxGetAsString(WBColumn.ProtectiveZone);
            if (itemProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC) return true;

            if (_minimumProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET && itemProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET) return true;

            if (_minimumProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PROTECTED) return true;

            return false;
        }

        public IHierarchyData GetParent()
        {
            return _parent;
        }

        public bool HasChildren
        {
            get
            {
                TreeViewLocationCollection children = GetChildren() as TreeViewLocationCollection;
                return children.Count > 0;
            }
        }

        public object Item
        {
            get { return this; }
        }

        public string Path
        {
            get {

                string parentsPath = ""; 
                if (_parent != null) parentsPath = _parent.Path;
                

                return parentsPath + "/" + _name; 
            
            }
        }

        public string Type
        {
            get { return this.GetType().ToString(); }
        }

        #endregion

        public override string ToString()
        {
            return _name;
        }

        public static SPListItemCollection GetItemsRecursive(SPFolder folder)
        {
            SPList list = folder.ParentWeb.Lists[folder.ParentListId];
            SPQuery query = new SPQuery();
            query.Folder = folder;                        //set folder for seaching;
            query.ViewAttributes = "Scope=\"Recursive\""; //set recursive mode for items seaching;
            return list.GetItems(query);
        }

    }
}

