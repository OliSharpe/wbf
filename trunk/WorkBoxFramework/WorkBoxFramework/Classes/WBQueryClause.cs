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
    public class WBQueryClause
    {
        public enum Comparators
        {
            Equals,
            NotEquals,
            LessThan,
            LessThanEquals,
            GreaterThan,
            GreaterThanEquals,
            IsNull,
            IsNotNull
        }

        public enum Modifiers
        {
            DoNotUseDescendants,
            UseDescendants
        }

        public WBQueryClause(WBColumn column, Comparators comparator, Object value)
        {
            Column = column;
            Comparator = comparator;
            Value = value;

            switch (Comparator)
            {
                case Comparators.Equals: { break; }
                case Comparators.NotEquals: { break; }
                case Comparators.LessThan: { break; }
                case Comparators.LessThanEquals: { break; }
                case Comparators.GreaterThan: { break; }
                case Comparators.GreaterThanEquals: { break; }
                case Comparators.IsNull: { break; }
                case Comparators.IsNotNull: { break; }
                default: throw new NotImplementedException("The selected comparator has not been implemented yet.");
            }

            UseDescendants = false;
        }

        public WBColumn Column { get; set; }

        public Object Value { get; set; }

        public Comparators Comparator { get; set; }

        public bool UseDescendants { get; set; }


        private bool _renderForView = false;
        public bool RenderForView
        {
            get;
            set; 
        }

        public StringBuilder AppendCAMLClauseTo(StringBuilder builder, SPSite site)
        {
            if (RenderForView)
            {
                if (Column.DataType == WBColumn.DataTypes.ManagedMetadata)
                {
                    return AppendCAMLClause(builder, Column.InternalName, "Text", ((WBTerm)Value).Name);
                }
                else
                {
                    return AppendCAMLClause(builder, Column.InternalName, "Text", Value.WBxToString());
                }
            }

            switch (Column.DataType)
            {
                case WBColumn.DataTypes.ManagedMetadata:
                    {
                        return AppendMMCAMLClause(builder, site, Column.InternalName, (WBTerm)Value, UseDescendants);
                    }
                case WBColumn.DataTypes.Text:
                    {
                        return AppendCAMLClause(builder, Column.InternalName, "Text", (String)Value);
                    }
                case WBColumn.DataTypes.Integer:
                    {
                        return AppendCAMLClause(builder, Column.InternalName, "Integer", Value.ToString());
                    }
                case WBColumn.DataTypes.Counter:
                    {
                        return AppendCAMLClause(builder, Column.InternalName, "Counter", Value.ToString());
                    }
                case WBColumn.DataTypes.Choice:
                    {
                        return AppendCAMLClause(builder, Column.InternalName, "Choice", (String)Value);
                    }
                case WBColumn.DataTypes.DateTime:
                    {
                        return AppendCAMLClause(builder, Column.InternalName, "DateTime", (String)Value);
                    }
                case WBColumn.DataTypes.URL:
                    {
                        return AppendCAMLClause(builder, Column.InternalName, "URL", (String)Value);
                    }
            }

            throw new NotImplementedException("Creation of CAML clauses for WBColumns of data type " + WBColumn.DataTypeToString(Column.DataType) + " has not yet been implmenented");
        }

        private void AppendComparatorStartTag(StringBuilder builder)
        {
            switch (Comparator)
            {
                case Comparators.Equals: { builder.Append("<Eq>"); break; } 
                case Comparators.NotEquals: { builder.Append("<Neq>"); break; } 
                case Comparators.LessThan: { builder.Append("<Lt>"); break; } 
                case Comparators.LessThanEquals: { builder.Append("<Leq>"); break; } 
                case Comparators.GreaterThan: { builder.Append("<Gt>"); break; }
                case Comparators.GreaterThanEquals: { builder.Append("<Geq>"); break; }
                case Comparators.IsNull: { builder.Append("<IsNull>"); break; }
                case Comparators.IsNotNull: { builder.Append("<IsNotNull>"); break; }
                default: throw new NotImplementedException("The selected comparator has not been implemented yet.");
            }
        }

        private void AppendComparatorEndTag(StringBuilder builder)
        {
            switch (Comparator)
            {
                case Comparators.Equals: { builder.Append("</Eq>"); break; } 
                case Comparators.NotEquals: { builder.Append("</Neq>"); break; } 
                case Comparators.LessThan: { builder.Append("</Lt>"); break; } 
                case Comparators.LessThanEquals: { builder.Append("</Leq>"); break; } 
                case Comparators.GreaterThan: { builder.Append("</Gt>"); break; }
                case Comparators.GreaterThanEquals: { builder.Append("</Geq>"); break; }
                case Comparators.IsNull: { builder.Append("</IsNull>"); break; }
                case Comparators.IsNotNull: { builder.Append("</IsNotNull>"); break; }
                default: throw new NotImplementedException("The selected comparator has not been implemented yet.");
            }
        }
        
        public static int[] GetWssIdsOfTerm(SPSite site, WBTerm term, bool includeDescendants)
        {
            return TaxonomyField.GetWssIdsOfTerm(site, term.Taxonomy.TermStore.Id, term.Taxonomy.TermSet.Id, term.Id, includeDescendants, 500);
        }


        public StringBuilder AppendMMCAMLClause(StringBuilder builder, SPSite site, string fieldName, WBTerm term, bool includeDescendants)
        {

            if (term == null) return builder;
            if (Comparator != Comparators.Equals) return builder;

            WBLogging.Queries.Verbose("Looking for term: " + term.Name + " in site: " + site.HostName + site.ServerRelativeUrl + " for field: " + fieldName);

            int[] wssIds = site.WBxGetWssIdsOfTerm(term, includeDescendants);

            // If the site had no matching WssIds that means the term isn't being used in the site
            // so it can't possibly match - hence returning a filter clause that should always fail:
            if (wssIds.Length == 0)
                return AppendNoResultsClause(builder);

            if (wssIds.Length == 1)
            {

                builder.Append("<Eq><FieldRef Name='").Append(fieldName).Append("' LookupId='TRUE'/>");

                foreach (int wssId in wssIds)
                {
                    builder.Append("<Value Type='Lookup'>").Append(wssId).Append("</Value>");
                }

                builder.Append("</Eq>");

            }
            else
            {

                builder.Append("<In><FieldRef Name='").Append(fieldName).Append("'  LookupId='TRUE'/><Values>");

                foreach (int wssId in wssIds)
                {
                    builder.Append("<Value Type='Lookup'>").Append(wssId).Append("</Value>");
                }

                builder.Append("</Values></In>");
            }

            return builder;
        }


        public StringBuilder AppendCAMLClause(StringBuilder builder, String fieldName, String valueType, String value)
        {
            AppendComparatorStartTag(builder);
            builder.Append("<FieldRef Name='").Append(fieldName).Append("'/>\n");
            if (Comparator != Comparators.IsNull && Comparator != Comparators.IsNotNull)
            {
                builder.Append("<Value Type='").Append(valueType).Append("'>").Append(value).Append("</Value>");
            }
            AppendComparatorEndTag(builder);

            return builder;
        }

        public static StringBuilder AppendNoResultsClause(StringBuilder builder)
        {
            return builder.Append("<Eq><FieldRef Name='ContentType'/><Value Type='Text'>NoSuchContentTypeExists</Value></Eq>");
        }


    }
}
