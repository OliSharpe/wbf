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
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class WBMappedPath
    {
        private WBMigrationMapping _mapping = null;
        public String MappingPath { get; private set; }
        public String FunctionalAreaPath { get; private set; }
        public String RecordsTypePath { get; private set; }
        public String SubjectTagsPaths { get; private set; }
        public String ProtectiveZone { get; private set; }
        public String LiveOrArchived { get; private set; }
        public String OwningTeamPath { get; private set; }

        private String _errorStatusMessage = String.Empty;

        public WBMappedPath(WBMigrationMapping mapping, SPListItem mappingItem) 
        {
            _mapping = mapping;
            MappingPath = WBUtils.NormalisePath(mappingItem.WBxGetAsString(WBColumn.MappingPath));
            FunctionalAreaPath = WBUtils.NormalisePaths(mappingItem.WBxGetAsString(WBColumn.FunctionalAreaPath));
            RecordsTypePath = WBUtils.NormalisePath(mappingItem.WBxGetAsString(WBColumn.RecordsTypePath));
            SubjectTagsPaths = WBUtils.NormalisePaths(mappingItem.WBxGetAsString(WBColumn.SubjectTagsPaths));
            OwningTeamPath = WBUtils.NormalisePath(mappingItem.WBxGetAsString(WBColumn.OwningTeamPath));
            ProtectiveZone = mappingItem.WBxGetAsString(WBColumn.ProtectiveZone);
            LiveOrArchived = mappingItem.WBxGetAsString(WBColumn.LiveOrArchived);
        }

        private WBTermCollection<WBTerm> _functionalArea;
        public WBTermCollection<WBTerm> FunctionalArea
        {
            get
            {
                if (_functionalArea == null)
                {
                    if (_mapping == null) return null;
                    if (_mapping.FunctionalAreasTaxonomy == null) return null;

                    if (String.IsNullOrEmpty(FunctionalAreaPath))
                    {
                        ErrorMessage("The functional area path was null or empty.");
                        return null;
                    }

                    string[] paths = FunctionalAreaPath.Split(';');

                    List<WBTerm> terms = new List<WBTerm>();

                    foreach (string path in paths)
                    {
                        WBLogging.Migration.Verbose("Trying to get a Functional Area by path with: " + path);

                        Term term = _mapping.FunctionalAreasTaxonomy.GetOrCreateSelectedTermByPath(path);
                        if (term != null)
                        {
                            terms.Add(new WBTerm(_mapping.FunctionalAreasTaxonomy, term));
                        }
                        else
                        {
                            ErrorMessage("Coundn't find the functional area with path: " + path);
                        }
                    }

                    if (terms.Count > 0)
                    {
                        _functionalArea = new WBTermCollection<WBTerm>(_mapping.FunctionalAreasTaxonomy, terms);
                    }
                    else
                    {
                        ErrorMessage("Was not able to resolve any of the functional areas from the paths list: " + FunctionalAreaPath);
                    }

                }
                return _functionalArea;
            }
        }

        private WBRecordsType _recordsType;
        public WBRecordsType RecordsType
        {
            get
            {
                if (_recordsType == null)
                {
                    if (_mapping == null) return null;
                    if (_mapping.RecordsTypesTaxonomy == null) return null;

                    WBLogging.Migration.Verbose("Trying to get Records Type by path with: " + RecordsTypePath);

                    Term term = _mapping.RecordsTypesTaxonomy.GetSelectedTermByPath(RecordsTypePath);
                    if (term != null)
                    {
                        _recordsType = new WBRecordsType(_mapping.RecordsTypesTaxonomy, term);
                    }
                    else
                    {
                        ErrorMessage("Coundn't find the records type with path: " + RecordsTypePath);
                    }
                }
                return _recordsType;
            }
        }


        private WBTermCollection<WBSubjectTag> _subjectTags;
        public WBTermCollection<WBSubjectTag> SubjectTags
        {
            get
            {
                if (_subjectTags == null)
                {
                    if (_mapping == null) return null;
                    if (_mapping.SubjectTagsTaxonomy == null) return null;

                    List<WBSubjectTag> terms = new List<WBSubjectTag>();


                    // Note that it is not necessarily an error for the subject tags to be empty.
                    if (!String.IsNullOrEmpty(SubjectTagsPaths) && SubjectTagsPaths != "/")
                    {
                        string[] paths = SubjectTagsPaths.Split(';');

                        foreach (string path in paths)
                        {
                            WBLogging.Migration.Verbose("Trying to get a Subject Tag by path with: " + path);

                            if (path != "/")
                            {
                                Term term = _mapping.SubjectTagsTaxonomy.GetOrCreateSelectedTermByPath(path);
                                if (term != null)
                                {
                                    terms.Add(new WBSubjectTag(_mapping.SubjectTagsTaxonomy, term));
                                }
                                else
                                {
                                    ErrorMessage("Coundn't find the subject tag with path: " + path);
                                }
                            }
                        }
                    }

                    _subjectTags = new WBTermCollection<WBSubjectTag>(_mapping.SubjectTagsTaxonomy, terms);

                }
                return _subjectTags;
            }
        }

        public bool InErrorStatus { get { return !String.IsNullOrEmpty(_errorStatusMessage); } }
        public String ErrorStatusMessage { get { return _errorStatusMessage; } }


        private void ErrorMessage(String message)
        {
            WBLogging.Migration.Unexpected("Error with MappedPath: " + message);
            _errorStatusMessage += message + " ";
        }


    }
}
