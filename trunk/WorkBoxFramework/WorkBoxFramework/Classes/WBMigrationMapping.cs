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
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBMigrationMapping
    {
        private Dictionary<String, WBMappedPath> _mapping;
        private String _errorStatusMessage = String.Empty;

        public WBMigrationMapping(String mappingListUrl, String mappingListView)
        {
            _mapping = new Dictionary<String, WBMappedPath>();
            SetUpMapping(mappingListUrl, mappingListView);
        }


        private void SetUpMapping(String mappingListUrl, String mappingListView)
        {
            if (String.IsNullOrEmpty(mappingListUrl))
            {
                ErrorMessage("There was no mapping list URL specified.");
                return;
            }

            using (SPSite site = new SPSite(mappingListUrl))
            using (SPWeb web = site.OpenWeb())
            {
                SPList mappingList = web.GetList(mappingListUrl);
                SPListItemCollection mappingItems = null;

                if (String.IsNullOrEmpty(mappingListView))
                {
                    mappingItems = mappingList.Items;
                }
                else
                {
                    SPView mappingView = mappingList.Views[mappingListView];
                    mappingItems = mappingList.GetItems(mappingView);
                }

                foreach (SPListItem mappingItem in mappingItems)
                {
                    WBMappedPath mappedPath = new WBMappedPath(this, mappingItem);

                    if (_mapping.ContainsKey(mappedPath.MappingPath))
                    {
                        mappingItem.WBxSet(WBColumn.MigrationStatus, "Duplicate");
                        mappingItem.Update();
                    }
                    else
                    {
                        _mapping.Add(mappedPath.MappingPath, mappedPath);
                    }

                }
            }
        }

        private SPSite _site = null;
        private WBTaxonomy _functionalAreasTaxonomy;
        private WBTaxonomy _recordsTypesTaxonomy;
        private WBTaxonomy _subjectTagsTaxonomy;
        private WBTaxonomy _teamsTaxonomy;

        public void ConnectToSite(SPSite site)
        {
            _site = site;
            _recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(site);
            _functionalAreasTaxonomy = WBTaxonomy.GetFunctionalAreas(_recordsTypesTaxonomy);
            _subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(_recordsTypesTaxonomy);
            _teamsTaxonomy = WBTaxonomy.GetTeams(_recordsTypesTaxonomy);
        }

        public WBTaxonomy FunctionalAreasTaxonomy { get { return _functionalAreasTaxonomy; } }
        public WBTaxonomy RecordsTypesTaxonomy { get { return _recordsTypesTaxonomy; } }
        public WBTaxonomy SubjectTagsTaxonomy { get { return _subjectTagsTaxonomy; } }
        public WBTaxonomy TeamsTaxonomy { get { return _teamsTaxonomy; } }

        public WBMappedPath this[String path]
        {
            get
            {
                if (String.IsNullOrEmpty(path)) return null;
                return _mapping[path];
            }
        }

        public bool InErrorStatus { get { return !String.IsNullOrEmpty(_errorStatusMessage); } }
        public String ErrorStatusMessage { get { return _errorStatusMessage; } }


        private void ErrorMessage(String message)
        {
            _errorStatusMessage += message + " ";
        }
    }
}
