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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBQuery
    {
        private List<WBColumn> _viewColumns;         
        private List<WBQueryClause> _clauses;
        private WBColumn _orderByColumn = null;
        private bool _ascending = true;
        private bool _recursiveAll = true;
        private String _filterByFolderPath = null;
        private bool _logicallyCannotHaveResults = false;

        public WBQuery() 
        {
            _viewColumns = new List<WBColumn>();
            _clauses = new List<WBQueryClause>();
        }

        public List<WBColumn> ViewColumns { get { return _viewColumns; } }


        public bool LogicallyCannotHaveResults
        {
            get { return _logicallyCannotHaveResults; }
            set { _logicallyCannotHaveResults = value; }
        }

        public bool RecursiveAll
        {
            get { return _recursiveAll; }
            set { _recursiveAll = value; }
        }

        public String FilterByFolderPath
        {
            get { return _filterByFolderPath; }
            set { _filterByFolderPath = value; }
        }

        public void AddViewColumn(WBColumn column)
        {
            _viewColumns.Add(column);
        }


        public void AddClause(WBQueryClause clause)
        {
            _clauses.Add(clause);
        }

        public void AddFilter(WBColumn column, WBQueryClause.Comparators comparator, Object value)
        {
            _clauses.Add(new WBQueryClause(column, comparator, value));
        }

        public void AddEqualsFilter(WBColumn column, Object value)
        {
            _clauses.Add(new WBQueryClause(column, WBQueryClause.Comparators.Equals, value));
        }

        public void OrderBy(WBColumn column, bool ascending)
        {
            this._orderByColumn = column;
            this._ascending = ascending;
        }

        public void OrderByAscending(WBColumn column)
        {
            OrderBy(column, true);
        }

        public void OrderByDescending(WBColumn column)
        {
            OrderBy(column, false);
        }


        public SPQuery AsSPQuery(SPSite site, SPList list)
        {
            SPQuery query = new SPQuery();

            if (!String.IsNullOrEmpty(_filterByFolderPath))
            {
                WBLogging.Queries.Verbose("WBQuery.AsSPQuery(): Looking for SPFolder at path: " + _filterByFolderPath);
                SPFolder folder = list.RootFolder.WBxGetFolderPath(_filterByFolderPath);
                if (folder != null)
                {
                    query.Folder = folder;
                    WBLogging.Queries.Verbose("WBQuery.AsSPQuery(): Filtering with the SPFolder at path: " + _filterByFolderPath);
                }
                else
                {
                    WBLogging.Queries.Unexpected("WBQuery.AsSPQuery(): The specified folder path wasn't found: " + _filterByFolderPath);
                    _logicallyCannotHaveResults = true;
                }
            }

            if (_viewColumns.Count > 0)
            {
                StringBuilder viewBuilder = new StringBuilder("");
                foreach (WBColumn column in _viewColumns)
                {
                    if (!column.IsVirtual)
                    {
                        viewBuilder.Append("<FieldRef Name=\"").Append(column.InternalName).Append("\" />");
                    }
                }

                WBLogging.Queries.Monitorable("The view XML is: " + viewBuilder.ToString());

                query.ViewFields = viewBuilder.ToString();
                query.ViewFieldsOnly = true;
            }

            StringBuilder queryBuilder = new StringBuilder("");
            if (_logicallyCannotHaveResults)
            {
                queryBuilder.Append("<Where>");
                WBQueryClause.AppendNoResultsClause(queryBuilder);
                queryBuilder.Append("</Where>");
            }
            else if (_clauses.Count > 0)
            {
                queryBuilder.Append("<Where>");
                buildNestedAndClauses(queryBuilder, site, _clauses, 0);
                queryBuilder.Append("</Where>");
            }

            
            if (_orderByColumn != null)
            {
                queryBuilder.Append("<OrderBy>");


                if (_orderByColumn.InternalName == WBColumn.TitleOrName.InternalName)
                {
                    queryBuilder.Append("<FieldRef Name=\"").Append(WBColumn.Title.InternalName).Append("\" Ascending=\"").Append(_ascending.ToString().ToUpper()).Append("\" />");
                    queryBuilder.Append("<FieldRef Name=\"").Append(WBColumn.Name.InternalName).Append("\" Ascending=\"").Append(_ascending.ToString().ToUpper()).Append("\" />");
                }
                else if (_orderByColumn.InternalName == WBColumn.DisplayFileSize.InternalName)
                {
                    queryBuilder.Append("<FieldRef Name=\"").Append(WBColumn.FileSize.InternalName).Append("\" Ascending=\"").Append(_ascending.ToString().ToUpper()).Append("\" />");
                }
                else
                {
                    queryBuilder.Append("<FieldRef Name=\"").Append(_orderByColumn.InternalName).Append("\" Ascending=\"").Append(_ascending.ToString().ToUpper()).Append("\" />");
                }

                queryBuilder.Append("</OrderBy>");
            }

            WBLogging.Queries.Monitorable("The query XML is: " + queryBuilder.ToString());

            query.Query = queryBuilder.ToString();

            if (_recursiveAll)
            {
                query.ViewAttributes = "Scope='RecursiveAll'";
            }

            return query;
        }


        public void buildNestedAndClauses(StringBuilder queryBuilder, SPSite site, List<WBQueryClause> clauses, int index)
        {
            WBQueryClause clause = clauses[index];
            index++;

            // If we're at the end of the recursion then just output the last clause:
            if (index == clauses.Count)
            {
                clause.AppendCAMLClauseTo(queryBuilder, site);
            }
            else
            {
                queryBuilder.Append("<And>");
                clause.AppendCAMLClauseTo(queryBuilder, site);
                buildNestedAndClauses(queryBuilder, site, clauses, index);
                queryBuilder.Append("</And>");
            }
        }

        public DataTable MakeResultsDataTable()
        {
            DataTable table = new DataTable();
            foreach (WBColumn column in _viewColumns)
            {
                switch (column.DataType)
                {
                    case WBColumn.DataTypes.Text:
                        table.Columns.Add(column.InternalName, typeof(String));
                        break;
                    case WBColumn.DataTypes.Boolean:
                        table.Columns.Add(column.InternalName, typeof(bool));
                        break;
                    case WBColumn.DataTypes.DateTime:
                        table.Columns.Add(column.InternalName, typeof(DateTime));
                        break;
                    case WBColumn.DataTypes.ManagedMetadata:
                        table.Columns.Add(column.InternalName, typeof(String));
                        break;
                    case WBColumn.DataTypes.Choice:
                        table.Columns.Add(column.InternalName, typeof(String));
                        break;
                    case WBColumn.DataTypes.Lookup:
                        table.Columns.Add(column.InternalName, typeof(String));
                        break;
                    default:
                        table.Columns.Add(column.InternalName, typeof(String));
                        break;
                }
            }

            return table;
        }
    }
}
