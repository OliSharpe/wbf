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
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Taxonomy.Generic;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework
{
    public static class WBExtensions
    {
        public const int BIG_CUSTOM_PROPERTIES__MAXIMUM_LENGTH = 10000;
        public const int BIG_CUSTOM_PROPERTIES__CHUNK_SIZE = 254;



        #region General Object Extensions
        public static String WBxToString(this Object value)
        {
            if (value == null) return "";
            return value.ToString();
        }

        #endregion

        #region String extensions

        public static String WBxTrim(this String value)
        {
            if (value == null) return "";
            return value.Trim();
        }

        public static String WBxToUpperFirstLetter(this String theString)
        {
            if (string.IsNullOrEmpty(theString))
            {
                return string.Empty;
            }

            return char.ToUpper(theString[0]) + theString.Substring(1);
        }

        public static String WBxReplaceTokens(this String text, WorkBox workBox)
        {
            text = text.Replace("[ID]", workBox.Item.ID.ToString());

            text = text.Replace("~WorkBoxCollection", workBox.Collection.Web.Url);

            text = text.Replace("~WorkBox", workBox.Web.Url);

            text = text.Replace("[CollectionURL]", workBox.Collection.Web.Url);

            text = text.Replace("[WorkBoxURL]", workBox.Web.Url);

            text = text.Replace("[AllWorkBoxesListName]", workBox.Collection.ListName);

            return text;
        }

        /// <summary>
        /// Returns 0 if string is empty otherwise uses Convert.ToInt32()
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int WBxToInt(this String text)
        {
            if (String.IsNullOrEmpty(text)) return 0;
            return Convert.ToInt32(text);
        }

        // Lazily based on the answer here:
        // http://stackoverflow.com/questions/1632078/split-string-in-512-char-chunks
        /// <summary>
        /// Splits the intput text string into chunks of chunkSize returning at least one blank chunk even if text is null.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IList<String> WBxSplitIntoChunksOfSize(this String text, int chunkSize)
        {
            // We're going to treat null strings as a blank string:
            if (String.IsNullOrEmpty(text)) text = "";

            List<String> chunks = new List<String>();
            int offset = 0;
            while (offset < text.Length)
            {
                int size = Math.Min(chunkSize, text.Length - offset);
                chunks.Add(text.Substring(offset, size));
                offset += size;
            }

            // If the string was empty we'll have no chunks at this point and we should return at 
            // least one blank chunk to represent the blank input string:
            if (chunks.Count == 0)
            {
                chunks.Add("");
            }

            return chunks;
        }


        #endregion

        #region WebControl extensions

        public static Control WBxFindNestedControlByID(this Control root, String id)
        {
            if (root == null) return null;
            if (root.ID != null && root.ID.Equals(id)) return root;
            if (root.Controls == null) return null;

            Control found = null;

            foreach (Control child in root.Controls)
            {
                found = child.WBxFindNestedControlByID(id);
                if (found != null) return found;
            }

            return null;
        }

        public static Control WBxAddWithIDInTableCell(this TableRow row, Control control, String id)
        {
            return WBxAddWithIDInTableCell(row, control, id, "");
        }

        public static Control WBxAddWithIDInTableCell(this TableRow row, Control control, String id, String cssClass)
        {
            control.ID = id;
            return row.WBxAddInTableCell(control, cssClass);
        }

        public static Control WBxAddInTableCell(this TableRow row, Control control)
        {
            return WBxAddInTableCell(row, control, "");
        }

        public static Control WBxAddInTableCell(this TableRow row, Control control, String cssClass)
        {
            TableCell cell = new TableCell();
            cell.CssClass = cssClass;
            cell.Controls.Add(control);
            row.Controls.Add(cell);

            if (control.GetType().Name.Equals("CheckBox"))
            {
                cell.HorizontalAlign = HorizontalAlign.Center;
            }

            return control;
        }

        public static System.Web.UI.WebControls.Label WBxAddTableHeaderCell(this TableRow row, String text)
        {
            TableHeaderCell cell = new TableHeaderCell();

            System.Web.UI.WebControls.Label label = new System.Web.UI.WebControls.Label();
            label.Text = text;
            cell.Controls.Add(label);
            row.Controls.Add(cell);

            return label;
        }

        public static String WBxMakeControlID(this Object obj, String outerName, String innerName)
        {
            return string.Format("WBF__{0}__{1}__{2}",
                obj.GetType().Name.ToUpper(),
                outerName.WBxTrim().ToUpper(),
                innerName.WBxTrim().ToUpper());
        }


        public static void WBxSetSelectedDate(this DateTimeControl dateTimeControl, SPListItem item, WBColumn column)
        {
            if (item.WBxHasValue(column))
            {
                dateTimeControl.SelectedDate = (DateTime)item.WBxGet(column);
            }
            else
            {
                dateTimeControl.ClearSelection();
            }
        }

        #endregion

        #region SPFarm SPWeb and SPList Property Set and Get Extensions

        public static int WBxGetIntProperty(this SPWeb web, String key)
        {
            return web.WBxGetProperty(key).WBxToInt();
//            if (value == "") return 0;
//            return Convert.ToInt32(value);
        }

        public static void WBxSetIntProperty(this SPWeb web, String key, int value)
        {
            web.WBxSetProperty(key, value);
        }

        public static int WBxGetIntProperty(this SPList list, String key)
        {
            return list.WBxGetProperty(key).WBxToInt();

//            string value = list.WBxGetProperty(key);
//            if (value == "") return 0;
//            return Convert.ToInt32(value);
        }

        public static void WBxSetIntProperty(this SPList list, String key, int value)
        {
            list.WBxSetProperty(key, value);
        }


        public static bool WBxGetBoolPropertyOrDefault(this SPWeb web, String key, bool defaultValue)
        {
            string stringValue = web.WBxGetProperty(key);

            if (stringValue == "") return defaultValue;
            return true.ToString().Equals(stringValue);
        }

        public static bool WBxGetBoolProperty(this SPWeb web, String key)
        {
            return true.ToString().Equals(web.WBxGetProperty(key));
        }

        public static void WBxSetBoolProperty(this SPWeb web, String key, bool value)
        {
            web.WBxSetProperty(key, value);
        }

        public static String WBxGetProperty(this SPList list, String key)
        {
            if (list == null) return "";
            string value = safeGetPropertyAsString(list.RootFolder.Properties, key);
            return value;
        }

        public static String WBxGetProperty(this SPWeb web, String key)
        {
            if (web == null) return "";
            string value = safeGetPropertyAsString(web.AllProperties, key);
            //WBUtils.logMessage("Get: " + key + " = " + value + " from: " + web.Url);
            return value;
        }

        public static String WBxGetPropertyOrDefault(this SPWeb web, String key, String defaultValue)
        {
            string value = web.WBxGetProperty(key);
            if (value == "") return defaultValue;
            return value;
        }

        public static String WBxGetProperty(this SPFarm farm, String key)
        {
            if (farm == null) return "";
            return safeGetPropertyAsString(farm.Properties, key);
        }

        public static String WBxGetPropertyOrDefault(this SPFarm farm, String key, String defaultValue)
        {
            string value = farm.WBxGetProperty(key);
            if (value == "") return defaultValue;
            return value;
        }

        public static bool WBxGetBoolPropertyOrDefault(this SPFarm farm, String key, bool defaultValue)
        {
            string value = farm.WBxGetProperty(key);
            if (value == "") return defaultValue;
            return true.ToString().Equals(value);
        }

        public static int WBxGetIntPropertyOrDefault(this SPFarm farm, String key, int defaultValue)
        {
            string value = farm.WBxGetProperty(key);
            if (value == "") return defaultValue;
            return value.WBxToInt();
        }

        public static void WBxSetProperty(this SPWeb web, String key, Object value)
        {
            web.WBxSetProperty(key, value.WBxToString());
        }

        public static void WBxSetProperty(this SPWeb web, String key, String value)
        {
            //WBUtils.logMessage("Setting: " + key + " = " + value + " on: " + web.Url);
            safeSetPropertyAsString(web.AllProperties, key, value);
        }

        public static void WBxSetProperty(this SPList list, String key, Object value)
        {
            list.WBxSetProperty(key, value.WBxToString());
        }

        public static void WBxSetProperty(this SPList list, String key, String value)
        {
            safeSetPropertyAsString(list.RootFolder.Properties, key, value);
        }


        public static void WBxSetProperty(this SPFarm farm, String key, Object value)
        {
            farm.WBxSetProperty(key, value.WBxToString());
        }

        public static void WBxSetProperty(this SPFarm farm, String key, String value)
        {
            safeSetPropertyAsString(farm.Properties, key, value);
        }


#endregion

#region Term Set and Get Extensions

        public static void WBxSetProperty(this Term term, String key, String value)
        {
            value = value.WBxTrim();
            if (value == "")
            {
                if (term.CustomProperties.ContainsKey(key))
                {
                    term.DeleteCustomProperty(key);
                }
            }
            else
            {
                //                WBUtils.logMessage("Setting term | key | value: " + term.Name + " | " + key + " | " + value);
                term.SetCustomProperty(key, value);
            }
        }

        public static void WBxSetProperty(this Term term, String key, Object value)
        {
            term.WBxSetProperty(key, value.WBxToString());
        }

        public static String WBxGetProperty(this Term term, String key)
        {
            if (term == null) return "";
            return safeGetPropertyAsString(term.CustomProperties, key);
        }

        public static String WBxGetPropertyOrDefault(this Term term, String key, String defaultValue)
        {
            string value = term.WBxGetProperty(key);
            if (value == "") return defaultValue;
            return value;
        }


        public static void WBxSetBoolProperty(this Term term, String key, bool value)
        {
            term.WBxSetProperty(key, value.WBxToString());
        }
        
        public static bool WBxGetBoolProperty(this Term term, String key)
        {
            if (term == null) return false;
            return true.ToString().Equals(term.WBxGetProperty(key));
        }

        public static bool WBxGetBoolPropertyOrDefault(this Term term, String key, bool defaultValue)
        {
            string value = term.WBxGetProperty(key);
            if (value == "") return defaultValue;
            return true.ToString().Equals(value);
        }


        public static void WBxSetIntProperty(this Term term, String key, int value)
        {
            term.WBxSetProperty(key, value);
        }

        public static int WBxGetIntProperty(this Term term, String key)
        {
            return term.WBxGetProperty(key).WBxToInt();

//            string value = ;
//            if (value == "") return 0;
//            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Sets a 'big' custom property on a term by splitting it into multiple shorter custom properties.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="bigPropertyName"></param>
        /// <param name="value"></param>
        public static void WBxSetBigProperty(this Term term, String bigPropertyName, String value)
        {
            if (String.IsNullOrEmpty(bigPropertyName))
            {
                throw new ArgumentException("You must use a non-empty value for the big property's name");
            }

            // We're going to treat setting to 'null' the same as setting to blank.
            if (value == null) value = "";
            if (value.Length > BIG_CUSTOM_PROPERTIES__MAXIMUM_LENGTH)
            {
                throw new ArgumentException("Length of value being set exceeds maximum limit of : " + BIG_CUSTOM_PROPERTIES__MAXIMUM_LENGTH);
            }

            // WBx Set and Get methods fail better than the direct Term methods
            int previousCount = term.WBxGetProperty("wbf__big_property__" + bigPropertyName + "__number_of_chunks").WBxToInt();

            IList<String> chunks = value.WBxSplitIntoChunksOfSize(BIG_CUSTOM_PROPERTIES__CHUNK_SIZE);

            term.WBxSetProperty("wbf__big_property__" + bigPropertyName + "__number_of_chunks", chunks.Count.ToString());
            for (int i = 0; i < chunks.Count; i++)
            {
                term.WBxSetProperty("wbf__big_property__" + bigPropertyName + "__chunk_" + i, chunks[i]);
            }

            // Now we need to remove any extra custom properies from previous, longer values being stored:
            for (int i = chunks.Count; i < previousCount; i++)
            {
                // THis WBx method will delete blank custom properties:
                term.WBxSetProperty("wbf__big_property__" + bigPropertyName + "__chunk_" + i, "");
            }

        }

        /// <summary>
        /// Retrieves a 'big' custom property from a term by collating the various smaller custom properties that were set by the WBxSetBigProperty method.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="bigPropertyName"></param>
        /// <returns></returns>
        public static String WBxGetBigProperty(this Term term, String bigPropertyName)
        {
            if (String.IsNullOrEmpty(bigPropertyName))
            {
                throw new ArgumentException("You must use a non-empty value for the big property's name");
            }

            // WBx Set and Get methods fail better than the direct Term methods
            int count = term.WBxGetProperty("wbf__big_property__" + bigPropertyName + "__number_of_chunks").WBxToInt();

            // If the property doesn't exist or has empty value then we'll get back a count of zero
            // but the following code should still return a blank string:

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                builder.Append(term.WBxGetProperty("wbf__big_property__" + bigPropertyName + "__chunk_" + i));
            }

            return builder.ToString();
        }


        #endregion


        #region Private Safe Get Set Helper Methods

        private static string safeGetPropertyAsString(ReadOnlyDictionary<string, string> readOnlyDictionary, string key)
        {
            if (readOnlyDictionary.ContainsKey(key))
            {
                return readOnlyDictionary[key].WBxToString();
            }
            else
            {
                return "";
            }
        }

        private static String safeGetPropertyAsString(Hashtable properties, String key)
        {
            if (properties.ContainsKey(key))
            {
                return properties[key].WBxToString();
            }
            else
            {
                return "";
            }
        }

        private static void safeSetPropertyAsString(Hashtable properties, String key, Object value)
        {
            safeSetPropertyAsString(properties, key, value.WBxToString());
        }

        private static void safeSetPropertyAsString(Hashtable properties, String key, String value)
        {
            properties[key] = value.WBxTrim();            
        }

        private static void migratePropertyKeyName(Hashtable properties, String oldKey, String newKey)
        {
            String value = safeGetPropertyAsString(properties, oldKey);
            properties[newKey] = value;
        }

        #endregion


        #region UIControlValue Extensions

        public static String WBxUIControlValue(this Term term)
        {
            if (term == null) return "";
            return string.Format("{0}|{1}", term.Name, term.Id.ToString());
        }

        public static String WBxUIControlValue(this TaxonomyFieldValue value)
        {
            if (value == null) return "";
            return string.Format("{0}|{1}", value.Label, value.TermGuid.ToString());
        }

        public static String WBxUIControlValue(this TaxonomyFieldValueCollection values)
        {
            if (values == null) return "";

            List<String> textParts = new List<String>();
            foreach (TaxonomyFieldValue value in values)
            {
                textParts.Add(string.Format("{0}|{1}", value.Label, value.TermGuid.ToString()));
            }

            return String.Join(TaxonomyField.TaxonomyMultipleTermDelimiter.ToString(), textParts.ToArray());
        }

        #endregion


        #region SPSite Extensions
        public static SPWebTemplate WBxGetWebTemplateByName(this SPSite site, String name)
        {
            if (site == null || name == null || name == "") return null;

            SPWebTemplateCollection Templates = site.GetWebTemplates(Convert.ToUInt32(WorkBox.LOCALE_ID_ENGLISH));

            return Templates[name];
        }

        public static SPWebTemplate WBxGetWebTemplateByTitle(this SPSite site, String title)
        {
            WBUtils.logMessage("Getting WBxGetWebTemplateByTitle with site | title:  " + site + " | " + title);

            if (site == null || title == null || title == "") return null;

            SPWebTemplateCollection Templates = site.GetWebTemplates(Convert.ToUInt32(WorkBox.LOCALE_ID_ENGLISH));

            title = title.Trim();
            foreach (SPWebTemplate template in Templates)
            {
                if (template.Title.Trim().Equals(title))
                {
                    WBUtils.logMessage("Found template: " + template.Name);
                    return template;
                }
            }

            return null;
        }

        #endregion

        #region SPListItem Extensions

        public static bool WBxColumnExists(this SPListItem item, String columnName)
        {
            return (item.Fields.ContainsField(columnName));
        }

        public static bool WBxColumnExists(this SPListItem item, WBColumn column)
        {
            return item.WBxExists(column);
        }

        public static bool WBxExists(this SPListItem item, WBColumn column)
        {
            return (item.Fields.ContainsField(column.DisplayName));
        }

        [Obsolete("WBxColumnHasValue with WBColumn arg is deprecated, please use WBxHasValue instead.", true)]
        public static bool WBxColumnHasValue(this SPListItem item, WBColumn column)
        {
            return item.WBxHasValue(column);
        }

        public static bool WBxColumnHasValue(this SPListItem item, String columnName)
        {
            return (item.WBxGetColumnAsString(columnName) != "");            
        }

        public static bool WBxHasValue(this SPListItem item, WBColumn column)
        {
            return (item.WBxToString(column) != "");
        }

        public static bool WBxIsNotBlank(this SPListItem item, WBColumn column)
        {
            return (item.WBxToString(column).Trim() != "");
        }

        public static Object WBxGet(this SPListItem item, WBColumn column)
        {
            WBLogging.Generic.Verbose("Trying to call WBxGet");
            WBLogging.Generic.Verbose("Trying to get with WBxGet column: " + column.DisplayName);


            if (item == null)
            {
                WBUtils.shouldThrowError("Calling WBxGet with a null item!");
                return null;
            }

            if (column == null)
            {
                WBUtils.shouldThrowError("Calling WBxGet with a null column!");
                return null;
            }

            if (String.IsNullOrEmpty(column.DisplayName))
            {
                WBUtils.shouldThrowError("Calling WBxGet with a column object that has no display name!");
                return null;
            }


            if (!item.WBxExists(column)) return null;

            switch (column.DataType)
            {
                case WBColumn.DataTypes.Text:
                    {
                        return item[column.DisplayName];
                    }
                case WBColumn.DataTypes.MultiLineText:
                    {
                        return item[column.DisplayName];
                    }
                case WBColumn.DataTypes.Integer:
                    {
                        return item[column.DisplayName];
                    }
                case WBColumn.DataTypes.Counter:
                    {
                        return item[column.DisplayName];
                    }
                case WBColumn.DataTypes.DateTime:
                    {
                        return item[column.DisplayName];
                    }
                case WBColumn.DataTypes.ManagedMetadata:
                    {
                        if (column.AllowMultipleValues)
                        {
                            WBTermCollection<WBTerm> terms = item.WBxGetMultiTermColumn<WBTerm>(null, column.DisplayName);
                            return terms.WBxToString();
                        }
                        else
                        {
                            WBTerm term = item.WBxGetSingleTermColumn<WBTerm>(null, column.DisplayName);
                            return term.WBxToString();
                        }
                    }

                case WBColumn.DataTypes.Choice:
                    {
                        return item[column.DisplayName];
                    }
                case WBColumn.DataTypes.User:
                    {
                        if (column.AllowMultipleValues)
                        {
                            return item.WBxGetMultiUserColumn(column);
                        }
                        else
                        {
                            return item.WBxGetSingleUserColumn(column);
                        }
                    }


                default: throw new Exception("There is no WBxGet implementation (yet) for WBColumn of type : " + column.DataType);
            }
        }


        public static String WBxGetAsString(this SPListItem item, WBColumn column)
        {
            if (item.WBxHasValue(column))
            {
                switch (column.DataType)
                {
                    case WBColumn.DataTypes.User:
                        {
                            if (column.AllowMultipleValues)
                            {
                                List<SPUser> users = item.WBxGetMultiUserColumn(column);
                                if (users != null && users.Count > 0)
                                {
                                    users.WBxToString();
                                }
                                return "<i>(couldn't find: " + item.WBxGetAsString(column) + ")</i>";
                            }
                            else
                            {
                                SPUser user = item.WBxGetSingleUserColumn(column);
                                if (user != null)
                                {
                                    return user.LoginName;
                                }
                                return "<i>(couldn't find: " + item.WBxGetAsString(column) + ")</i>";
                            }

                        }

                    default: return WBxGetColumnAsString(item, column.DisplayName);
                }
            }
            else
            {
                return "";
            }
        }

        public static String WBxToString(this SPListItem item, WBColumn column)
        {
            return WBxGetColumnAsString(item, column.DisplayName);
        }

        public static String WBxGetColumnAsString(this SPListItem item, String columnName)
        {
            if (!item.WBxColumnExists(columnName)) return "";

            string value = "";
            try {
                value = item[columnName].WBxToString();
            }
            catch  {}

            return value;
        }

        public static String WBxGetAsPrettyString(this SPListItem item, WBColumn column)
        {
            if (item.WBxHasValue(column))
            {
                switch (column.DataType)
                {
                    case WBColumn.DataTypes.DateTime:
                        {
                            DateTime date = (DateTime)item.WBxGet(column);

                            if (column.UseDateAndTime)
                            {
                                return date.ToShortDateString() + " " + date.ToShortTimeString();
                            }
                            else
                            {
                                return date.ToShortDateString();
                            }
                        }
                    case WBColumn.DataTypes.User:
                        {
                            if (column.AllowMultipleValues)
                            {
                                List<SPUser> users = item.WBxGetMultiUserColumn(column);
                                if (users != null && users.Count > 0)
                                {
                                    users.WBxToPrettyString();
                                }
                                return "<i>(couldn't find: " + item.WBxGetAsString(column) + ")</i>";
                            }
                            else
                            {
                                SPUser user = item.WBxGetSingleUserColumn(column);
                                if (user != null)
                                {
                                    return user.Name;
                                }
                                return "<i>(couldn't find: " + item.WBxGetAsString(column) + ")</i>";
                            }

                        }

                    case WBColumn.DataTypes.ManagedMetadata:
                        {
                            WBTerm term = item.WBxGetSingleTermColumn<WBTerm>(null, column);
                            if (term != null)
                            {
                                return term.Name;
                            }
                            else
                            {
                                return "";
                            }
                        }

                    case WBColumn.DataTypes.Lookup:
                        {
                            SPFieldLookup fieldLookup = (SPFieldLookup)item.Fields.GetField(column.DisplayName);
                            SPFieldLookupValue fieldLookupValue = (SPFieldLookupValue)fieldLookup.GetFieldValue(item[column.DisplayName].ToString());

                            return fieldLookupValue.LookupValue;
                        }

                    default: return item.WBxGetAsString(column);
                }
            }
            else
            {
                return "";
            }

        }


        public static bool WBxGetAsBool(this SPListItem item, WBColumn column)
        {
            return item.WBxGetColumnAsBool(column.DisplayName);
        }

        public static void WBxSetAsBool(this SPListItem item, WBColumn column, bool value)
        {
            item[column.DisplayName] = value;
        }

        public static bool WBxGetColumnAsBool(this SPListItem item, String columnName)
        {
            return (item.WBxGetColumnAsString(columnName) == "True");
        }


        public static int WBxGetAsInt(this SPListItem item, WBColumn column, int defaultValue)
        {
            return item.WBxGetColumnAsInt(column.DisplayName, defaultValue);
        }

        // This one is slightly out of sync with the naming convention used by others in this series of methods.
        public static int WBxGetColumnAsInt(this SPListItem item, WBColumn column, int defaultValue)
        {
            return item.WBxGetColumnAsInt(column.DisplayName, defaultValue);
        }

        public static int WBxGetColumnAsInt(this SPListItem item, String columnName, int defaultValue)
        {
            if (!item.WBxColumnHasValue(columnName)) return defaultValue;
            return (Convert.ToInt32(item.WBxGetColumnAsString(columnName)));
        }

        public static void WBxSetFrom(this SPListItem destination, SPListItem source, WBColumn column)
        {
            destination.WBxSet(column, source.WBxGet(column));
        }

        public static void WBxSetFrom(this SPListItem destination, WBItem source)
        {
            destination.WBxSetFrom(source, source.Columns);
        }

        public static void WBxSetFrom(this SPListItem destination, SPListItem source, IEnumerable<WBColumn> columnsToCopy)
        {
            foreach (WBColumn column in columnsToCopy)
            {
                destination.WBxSet(column, source.WBxGet(column));
            }
        }


        public static void WBxSetFrom(this SPListItem destination, WBItem source, IEnumerable<WBColumn> columnsToCopy)
        {
            foreach (WBColumn column in columnsToCopy)
            {
                destination.WBxSet(column, source[column]);
            }
        }

        public static void WBxCopyFrom(this SPListItem destination, SPListItem source, IEnumerable<WBColumn> columnsToCopy)
        {
            foreach (WBColumn column in columnsToCopy)
            {
                destination.WBxCopyFrom(source, column);
            }
        }

        public static void WBxCopyFrom(this SPListItem destination, SPListItem source, WBColumn column)
        {
            if (source.WBxExists(column) && destination.WBxExists(column))
                destination.WBxSet(column, source.WBxGet(column));
        }

        public static void WBxSet(this SPListItem item, WBColumn column, Object value)
        {
            if (item == null)
            {
                WBUtils.shouldThrowError("An attempt to save a value to an item that is null !!");
                if (column != null) WBUtils.shouldThrowError(" Column: " + column.DisplayName + " Value: " + value.WBxToString());
                return;
            }

            if (column == null)
            {
                WBUtils.shouldThrowError("An attempt to save a value to a column that is null !!");
                WBUtils.shouldThrowError("Value: " + value.WBxToString());
                return;
            }

            if (String.IsNullOrEmpty(column.DisplayName))
            {
                WBUtils.shouldThrowError("An attempt to save a value to a column that has no display name!!");
                WBUtils.shouldThrowError("Value: " + value.WBxToString());
                return;
            }

            WBLogging.Generic.Verbose("WBxSet(): Trying to set Column: " + column.DisplayName + " Value: " + value.WBxToString());

            if (!item.WBxExists(column)) 
            {
                WBUtils.shouldThrowError("An attempt to save a value to column that doesn't exist for the given item: Column: " + column.DisplayName + " Value: " + value.WBxToString());
                return;
            }

            switch (column.DataType)
            {
                case WBColumn.DataTypes.Text:
                    {
                        item[column.DisplayName] = value.WBxToString();
                        break;
                    }
                case WBColumn.DataTypes.MultiLineText:
                    {
                        item[column.DisplayName] = value.WBxToString();
                        break;
                    }
                case WBColumn.DataTypes.Integer:
                    {
                        item[column.DisplayName] = value;
                        break;
                    }
                case WBColumn.DataTypes.Counter:
                    {
                        item[column.DisplayName] = value;
                        break;
                    }
                case WBColumn.DataTypes.DateTime:
                    {
                        if (value == null || value is DateTime)
                        {
                            item[column.DisplayName] = value;
                        }
                        else if (value is DateTimeControl)
                        {
                            DateTimeControl dateTimeControl = (DateTimeControl)value;
                            if (dateTimeControl.IsDateEmpty)
                            {
                                item[column.DisplayName] = null;
                            }
                            else
                            {
                                item[column.DisplayName] = dateTimeControl.SelectedDate;
                            }
                        }
                        else
                        {
                            WBUtils.shouldThrowError("You can only set DateTime columns with null or with DateTime objects. Column: " + column.DisplayName + " Value: " + value);
                        }

                        break;
                    }
                case WBColumn.DataTypes.ManagedMetadata:
                    {
                        if (value == null)
                        {
                            WBLogging.Debug("Called to WBxSet for a ManagedMetadata column but the value was null");
                            value = "";
                        }

                        if (column.AllowMultipleValues)
                        {
                            if (value is String)
                            {
                                item.WBxSetMultiTermColumn(column.DisplayName, value.WBxToString());
                            }
                            else if (value is WBTermCollection<WBTerm>)
                            {
                                WBTermCollection<WBTerm> collection = value as WBTermCollection<WBTerm>;
                                //WBLogging.Debug("Setting " + column.DisplayName + " to value: " + collection.ToString());
                                item.WBxSetMultiTermColumn<WBTerm>(column.DisplayName, collection);
                            }
                            else if (value is WBTermCollection<WBRecordsType>)
                            {
                                WBTermCollection<WBRecordsType> collection = value as WBTermCollection<WBRecordsType>;
                                //WBLogging.Debug("Setting " + column.DisplayName + " to value: " + collection.ToString());
                                item.WBxSetMultiTermColumn<WBRecordsType>(column.DisplayName, collection);
                            }
                            else if (value is WBTermCollection<WBTeam>)
                            {
                                WBTermCollection<WBTeam> collection = value as WBTermCollection<WBTeam>;
                                //WBLogging.Debug("Setting " + column.DisplayName + " to value: " + collection.ToString());
                                item.WBxSetMultiTermColumn<WBTeam>(column.DisplayName, collection);
                            }
                            else if (value is WBTermCollection<WBSubjectTag>)
                            {
                                WBTermCollection<WBSubjectTag> collection = value as WBTermCollection<WBSubjectTag>;
                                //WBLogging.Debug("Setting " + column.DisplayName + " to value: " + collection.ToString());
                                item.WBxSetMultiTermColumn<WBSubjectTag>(column.DisplayName, collection);
                            }
                            else
                            {
                                WBUtils.shouldThrowError("You can only set multi ManagedMetadata columns with values of type String or WBTermCollection<WBTerm>. Column: " + column.DisplayName + " Value: " + value);
                            }

                            break;
                        }
                        else
                        {
                            if (value is String)
                            {
                                item.WBxSetSingleTermColumn(column.DisplayName, value.WBxToString());
                            }
                            else if (value is WBTerm)
                            {
                                item.WBxSetSingleTermColumn(column.DisplayName, value as WBTerm);
                            }
                            else if (value is WBRecordsType)
                            {
                                item.WBxSetSingleTermColumn(column.DisplayName, value as WBRecordsType);
                            }
                            else if (value is WBTeam)
                            {
                                item.WBxSetSingleTermColumn(column.DisplayName, value as WBTeam);
                            }
                            else if (value is WBSubjectTag)
                            {
                                item.WBxSetSingleTermColumn(column.DisplayName, value as WBSubjectTag);
                            }
                            else
                            {
                                WBUtils.shouldThrowError("You can only set singular ManagedMetadata columns with values of type String or WBTerm. Column: " + column.DisplayName + " Value: " + value);
                            }

                            break;
                        }
                    }

                case WBColumn.DataTypes.Choice:
                    {
                        string choice = value.WBxToString();

                        if (column.Choices.Contains(choice))
                        {
                            item[column.DisplayName] = choice;
                        }
                        else
                        {
                            WBUtils.shouldThrowError("You can only set valid choices for a choice field. Disallowed value was: " + choice);
                        }

                        break; 
                    }

                case WBColumn.DataTypes.User:
                    {
                        if (column.AllowMultipleValues == false)
                        {
                            if (value is SPUser)
                            {
                                item.WBxSetSingleUserColumn(item.Web, column.DisplayName, (SPUser)value);
                            }
                            else if (String.IsNullOrEmpty(value.WBxToString()))
                            {
                                item[column.DisplayName] = null;
                            }
                            else if (value is String)
                            {
                                item.WBxSetSingleUserColumn(item.Web, column.DisplayName, item.Web.EnsureUser((String)value));
                            }
                            else
                            {
                                throw new Exception("In WBxSet() for User column type: The value being saved was not an SPUser object or null: " + value + " for column: " + column.DisplayName);
                            }
                        }
                        else
                        {
                            if (value is List<SPUser>)
                            {
                                item.WBxSetMultiUserColumn(item.Web, column, value as List<SPUser>);
                            }
                            else if (value is SPFieldUserValueCollection)
                            {
                                item[column.DisplayName] = value;
                            }
                            else if (value is String)
                            {
                                item.WBxSetMultiUserColumn(item.Web, column, ((String)value).WBxToSPUsers(item.Web));
                            }
                            else if (value == null)
                            {
                                item[column.DisplayName] = null;
                            }
                            else
                            {                                
                                throw new Exception("In WBxSet() for User column type: The value is not a List<SPUser> object when setting a multi-User column:  " + column.DisplayName + " the value's type is: " + value.GetType());
                            }
                        }
                        break;
                    }

                case WBColumn.DataTypes.Boolean:
                    {
                        if (value is bool)
                        {
                            item[column.DisplayName] = value;
                        }
                        else if (value is String)
                        {
                            if (String.IsNullOrEmpty((String)value)) throw new Exception("In WBxSet() for Boolean column type: You cannot set a Boolean column with a null or empty string!");

                            String boolValue = ((String)value).Trim().ToLower();

                            if (boolValue == "true") item[column.DisplayName] = true;
                            else if (boolValue == "false") item[column.DisplayName] = false;
                            else
                            {
                                throw new Exception("In WBxSet() for Boolean column type: the value of the string being used to set was not recognised: " + value);
                            }
                        }
                        else
                        {
                            throw new Exception("In WBxSet() for Boolean column type: you can only set with a bool or String value. You attempted with: " + value.GetType());
                        }
                        break;
                    }

                default: throw new Exception("There is no WBxSet implementation (yet) for WBColumn of type : " + column.DataType);
            }

            WBLogging.Generic.Verbose("WBxSet(): Completed the setting of Column: " + column.DisplayName + " Value: " + value.WBxToString());

        }

        public static void WBxSetColumnAsString(this SPListItem item, WBColumn column, Object value)
        {
            item[column.DisplayName] = value.WBxToString();
        }

        public static void WBxSetColumnAsString(this SPListItem item, String columnName, Object value)
        {
            item[columnName] = value.WBxToString();
        }

        public static T WBxGetSingleTermColumn<T>(this SPListItem item, WBTaxonomy taxonomy, WBColumn column) where T : WBTerm, new()
        {
            return WBxGetSingleTermColumn<T>(item, taxonomy, column.DisplayName);
        }

        public static T WBxGetSingleTermColumn<T>(this SPListItem item, WBTaxonomy taxonomy, String columnName) where T : WBTerm, new()
        {
            TaxonomyFieldValue taxonomyFieldValue = item[columnName] as TaxonomyFieldValue;

            if (taxonomyFieldValue == null || taxonomyFieldValue.TermGuid == null || taxonomyFieldValue.TermGuid == "") return null;

            //WBUtils.logMessage("Getting a single term from a column with guid = " + taxonomyFieldValue.TermGuid);

            T term = new T();
            term.Initialise(taxonomy, taxonomyFieldValue.Label, new Guid(taxonomyFieldValue.TermGuid));

            return term;
        }

        public static WBTermCollection<T> WBxGetMultiTermColumn<T>(this SPListItem item, WBTaxonomy taxonomy, String columnName) where T : WBTerm, new()
        {
            TaxonomyFieldValueCollection taxonomyFieldValueCollection = item[columnName] as TaxonomyFieldValueCollection;

            if (taxonomyFieldValueCollection == null) return null;

            List<T> terms = new List<T>();

            foreach (TaxonomyFieldValue value in taxonomyFieldValueCollection)                                
            {
                T term = new T();
                term.Initialise(taxonomy, value.Label, new Guid(value.TermGuid));
                terms.Add(term);
            }

            return new WBTermCollection<T>(taxonomy, terms);
        }

        public static void WBxSetSingleTermColumn(this SPListItem item, String columnName, String termUIControlValue)
        {
            TaxonomyField taxonomyField = item.Fields[columnName] as TaxonomyField;

            TaxonomyFieldValue taxonomyFieldValue = new TaxonomyFieldValue(taxonomyField);

            taxonomyFieldValue.PopulateFromLabelGuidPair(termUIControlValue);

            taxonomyField.SetFieldValue(item, taxonomyFieldValue);
        }

        public static void WBxSetSingleTermColumn(this SPListItem item, WBColumn column, WBTerm term)
        {
            WBxSetSingleTermColumn(item, column.DisplayName, term);
        }

        public static void WBxSetSingleTermColumn(this SPListItem item, String columnName, WBTerm term)
        {
            if (term.TermNotResolvedYet)
            {
                WBxSetSingleTermColumn(item, columnName, term.UIControlValue);
            }
            else
            {
                TaxonomyField taxonomyField = item.Fields[columnName] as TaxonomyField;
                taxonomyField.SetFieldValue(item, term.Term);
            }
        }


        public static void WBxSetMultiTermColumn(this SPListItem item, String columnName, String termsUIControlValue) 
        {
            TaxonomyField taxonomyField = item.Fields[columnName] as TaxonomyField;

            TaxonomyFieldValueCollection taxonomyFieldValueCollection = new TaxonomyFieldValueCollection(taxonomyField);

            taxonomyFieldValueCollection.PopulateFromLabelGuidPairs(termsUIControlValue);

            taxonomyField.SetFieldValue(item, taxonomyFieldValueCollection);
        }

        public static void WBxSetMultiTermColumn<T>(this SPListItem item, WBColumn column, WBTermCollection<T> terms) where T : WBTerm, new()
        {
            WBxSetMultiTermColumn(item, column.DisplayName, terms.UIControlValue);
        }

        public static void WBxSetMultiTermColumn<T>(this SPListItem item, String columnName, WBTermCollection<T> terms) where T : WBTerm, new()
        {
            WBxSetMultiTermColumn(item, columnName, terms.UIControlValue);
        }

        public static SPUser WBxGetSingleUserColumn(this SPListItem item, WBColumn column)
        {
            if (!item.WBxHasValue(column)) return null;

            Object value = item[column.DisplayName];

            SPFieldUser fieldUser = (SPFieldUser)item.Fields.GetField(column.DisplayName);
            SPFieldUserValue fieldUserValue = (SPFieldUserValue)fieldUser.GetFieldValue(item[column.DisplayName].ToString());

            if (fieldUserValue.User == null)
            {
                WBLogging.Generic.Unexpected("Debug: found that fieldUserValue.User was null but LoginName: " + fieldUserValue.LookupValue);
            }

            return fieldUserValue.User;
        }

        public static List<SPUser> WBxGetMultiUserColumn(this SPListItem item, WBColumn column)
        {
            return item.WBxGetMultiUserColumn(column.DisplayName);
        }

        public static List<SPUser> WBxGetMultiUserColumn(this SPListItem item, String columnName)
        {
            SPFieldUserValueCollection userValueCollection = item[columnName] as SPFieldUserValueCollection;
            List<SPUser> users = new List<SPUser>();

            if (userValueCollection != null)
            {
                foreach (SPFieldUserValue userValue in userValueCollection)
                {
                    users.Add(userValue.User);
                }
            }

            return users;
        }

        public static void WBxSetMultiUserColumn(this SPListItem item, SPWeb web, WBColumn column, List<SPUser> users)
        {
            item.WBxSetMultiUserColumn(web, column.DisplayName, users);
        }

        public static void WBxSetMultiUserColumn(this SPListItem item, SPWeb web, String columnName, List<SPUser> users)
        {
            SPFieldUserValueCollection userValueCollection = new SPFieldUserValueCollection();

            if (users != null)
            {
                foreach (SPUser user in users)
                {
                    userValueCollection.Add(new SPFieldUserValue(web, user.ID, user.LoginName));
                }
            }

            item[columnName] = userValueCollection;
        }

        public static void WBxSetSingleUserColumn(this SPListItem item, SPWeb web, String columnName, SPUser user)
        {
            SPFieldUserValue userValue = new SPFieldUserValue(web, user.ID, user.LoginName);

            item[columnName] = userValue;
        }


        public static List<String> WBxGetFolderPath(this SPListItem item)
        {
            List<String> path = new List<String>(item.Url.Split('/'));

            int lastLocation = path.Count - 1;
            if (lastLocation >= 0)
                path.RemoveAt(lastLocation);

            //Then finally remove the first part of the path as this will just be the list name:
            if (path.Count > 0) path.RemoveAt(0);

            return path;
        }



        #endregion

        #region PeopleEditor

        public static void WBxInitialise(this PeopleEditor peopleEditor, List<SPUser> users)
        {
            ArrayList entityArrayList = new ArrayList();

            if (users != null)
            {
                foreach (SPUser user in users)
                {
                    PickerEntity entity = new PickerEntity();
                    entity.Key = user.LoginName;
                    entity.DisplayText = user.Name;
                    entityArrayList.Add(entity);
                }
            }

            peopleEditor.UpdateEntities(entityArrayList);
        }

        public static void WBxInitialise(this PeopleEditor peopleEditor, SPUser user)
        {
            ArrayList entityArrayList = new ArrayList();

            if (user != null)
            {
                PickerEntity entity = new PickerEntity();
                entity.Key = user.LoginName;
                entity.DisplayText = user.Name;
                entityArrayList.Add(entity);
            }

            peopleEditor.UpdateEntities(entityArrayList);
        }

        public static String WBxToString(this List<SPUser> users)
        {
            List<String> loginNames = new List<String>();
            foreach (SPUser user in users)
            {
                loginNames.Add(user.LoginName);
            }
            return String.Join(";", loginNames.ToArray());
        }

        public static String WBxToPrettyString(this List<SPUser> users)
        {
            if (users == null) return "";

            List<String> loginNames = new List<String>();
            foreach (SPUser user in users)
            {
                loginNames.Add(user.Name);
            }
            return String.Join(";", loginNames.ToArray());
        }

        public static List<SPUser> WBxToSPUsers(this String loginNamesString, SPWeb web) 
        {
            List<SPUser> users = new List<SPUser>();
            if (String.IsNullOrEmpty(loginNamesString)) return users;
            String[] loginNames = loginNamesString.Split(';');
            foreach (String loginName in loginNames) 
            {
                SPUser user = web.EnsureUser(loginName);
                users.Add(user);
            }
            return users;
        }


        public static List<SPUser> WBxGetMultiResolvedUsers(this PeopleEditor peopleEditor, SPWeb web)
        {

            List<SPUser> users = new List<SPUser>();

            if (peopleEditor.ResolvedEntities.Count > 0)
            {

                foreach (PickerEntity pickedEntity in peopleEditor.ResolvedEntities)
                {
                    WBUtils.logMessage("Found picked entity: " + pickedEntity.Key);

                    SPUser user = web.EnsureUser(pickedEntity.Key);

                    users.Add(user);
                }
            }
            else
            {
                WBUtils.logMessage("Couldn't find any resolved entities. comma sep value: " + peopleEditor.CommaSeparatedAccounts);
            }

            return users;
        }

        public static SPUser WBxGetSingleResolvedUser(this PeopleEditor peopleEditor, SPWeb web)
        {
            SPUser user = null; 

            if (peopleEditor.ResolvedEntities.Count > 0)
            {
                PickerEntity pickedEntity = peopleEditor.ResolvedEntities[0] as PickerEntity;
                user = web.WBxEnsureUserOrNull(pickedEntity.Key);
            }
            else
            {
                WBUtils.logMessage("Couldn't find any resolved entities. comma sep value: " + peopleEditor.CommaSeparatedAccounts);
            }

            return user;
        }

        /*
        public static List<SPUser> WBxGetMultiUserColumn(this SPListItem item, String columnName)
        {
            SPFieldUserValueCollection userValueCollection = item[columnName] as SPFieldUserValueCollection;

            List<SPUser> users = new List<SPUser>();

            if (userValueCollection != null)
            {
                foreach (SPFieldUserValue userValue in userValueCollection)
                {
                    users.Add(userValue.User);
                }
            }

            return users;
        }
         */ 

        /*
        public static void WBxSetMultiUserColumn(this SPListItem item, SPWeb web, String columnName, List<SPUser> users)
        {
            SPFieldUserValueCollection userValueCollection = new SPFieldUserValueCollection();

            if (users != null)
            {
                foreach (SPUser user in users)
                {
                    SPFieldUserValue userValue = new SPFieldUserValue(web, 
                }
            }

        }
        */

        #endregion


        #region SPList Extensions

        public static bool WBxIsDocumentLibrary(this SPList list)
        {
            return (list.BaseType == SPBaseType.DocumentLibrary);
        }

        public static SPView WBxCreateViewIfMissing(this SPList list, SPSite site, String viewName, WBQuery query)
        {
            return list.WBxCreateViewIfMissing(site, viewName, query, 50, true, false);
        }

        public static SPView WBxCreateViewIfMissing(this SPList list, SPSite site, String viewName, WBQuery query, uint itemsPerPage, bool paginate, bool setAsDefault)
        {
            SPView view = null;

            try
            {
                view = list.Views[viewName];
            }
            catch (Exception exception)
            {
            }

            if (view == null)
            {
                return list.Views.Add(viewName, query.JustViewFields(), query.JustCAMLQueryForView(site), itemsPerPage, paginate, setAsDefault);
            }
            else
            {
                return view;
            }
        }

        public static SPView WBxCreateOrUpdateView(this SPList list, SPSite site, String viewTitle, WBQuery query)
        {
            return list.WBxCreateOrUpdateView(site, viewTitle, query, 50, true, false);
        }

        public static SPView WBxCreateOrUpdateView(this SPList list, SPSite site, String viewTitle, WBQuery query, uint itemsPerPage, bool paginate, bool setAsDefault)
        {
            SPView view = null;

            String viewName = viewTitle.Replace(" ", "");

            try
            {
                view = list.Views[viewTitle];

                // First update the fields:
                // view.ViewFields.DeleteAll();
                foreach (String fieldName in query.JustViewFields())
                {
                    if (!view.ViewFields.Exists(fieldName))
                    {
                        view.ViewFields.Add(fieldName);
                    }
                }
                view.Update();

                // Then udpate the query:
                view.Query = query.JustCAMLQueryForView(site);
                view.Update();

                // Finally update the pagination:
                view.RowLimit = itemsPerPage;
                view.Paged = paginate;
                view.DefaultView = setAsDefault;
                view.Update();

            }
            catch (Exception exception)
            {
            }

            if (view == null)
            {
                view = list.Views.Add(viewName, query.JustViewFields(), query.JustCAMLQueryForView(site), itemsPerPage, paginate, setAsDefault);
                view.Title = viewTitle;
                view.Update();

                return view;
            }
            else
            {
                return view;
            }
        }

        public static bool WBxExists(this SPList list, WBColumn column)
        {
            return (list.Fields.ContainsField(column.DisplayName));
        }

        public static bool WBxAddContentType(this SPList list, SPWeb web, String contentTypeName)
        {
            SPContentType itemContentType = web.ContentTypes.Cast<SPContentType>()
                .FirstOrDefault(c => c.Name == contentTypeName);

            if (itemContentType == null) {
                WBLogging.Config.Unexpected("Could not find the content type " + contentTypeName + " in SPWeb " + web.Url);
                return false;
            }

            list.ContentTypesEnabled = true;
            list.ContentTypes.Add(itemContentType);
            list.Update();

            return true;
        }

        #endregion


        #region SPFile Extensions

        public static string WBxCopyTo(this SPFile sourceFile, String destinationRootFolderUrlString, String folderPathString)
        {
            return sourceFile.WBxCopyTo(destinationRootFolderUrlString, folderPathString, false);
        }

        public static string WBxCopyTo(this SPFile sourceFile, String destinationRootFolderUrlString, String folderPathString, bool forPublicWeb)
        {
            if (forPublicWeb)
            {
                folderPathString = WBUtils.PrepareFilenameForPublicWeb(folderPathString);
            }

            string[] steps = folderPathString.Split('/');
            List<string> folderPath = new List<String>(steps);

            return WBxCopyTo(sourceFile, destinationRootFolderUrlString, folderPath, true, forPublicWeb);
        }


        public static string WBxCopyTo(this SPFile sourceFile, String destinationRootFolderUrlString, List<String> folderPath)
        {
            return WBxCopyTo(sourceFile, destinationRootFolderUrlString, folderPath, true);
        }

        public static string WBxCopyTo(this SPFile sourceFile, String destinationRootFolderUrlString, List<String> folderPath, bool allowDuplicateNames)
        {
            return WBxCopyTo(sourceFile, destinationRootFolderUrlString, folderPath, allowDuplicateNames, false);
        }

        public static string WBxCopyTo(this SPFile sourceFile, String destinationRootFolderUrlString, List<String> folderPath, bool allowDuplicateNames, bool forPublicWeb)
        {
            // The following sources gave various ideas for this method:
            // http://social.technet.microsoft.com/Forums/en-ph/sharepoint2010programming/thread/6600b7ca-3211-4476-8ee1-7d60d8f50a1a
            // http://sharepoint.stackexchange.com/questions/17951/programmatically-move-a-document-in-a-library-to-another-site-collection
            // http://sharepointfieldnotes.blogspot.com/2009/11/how-to-copy-files-across-site.html
            // http://stackoverflow.com/questions/1059175/copy-files-to-document-library-in-sharepoint


            string errorMessage = "";

            Uri destinationRootFolderUrl = new Uri(destinationRootFolderUrlString);
            Uri destinationFileUrl = new Uri(destinationRootFolderUrl, sourceFile.Name);

            //IIdentity userIdentity = System.Web.HttpContext.Current.User.Identity;
            //WBLogging.Debug("In  IIdentity info: " + userIdentity.AuthenticationType + "  " + userIdentity.Name + "  " + userIdentity.IsAuthenticated);

            using (SPSite destinationSite = new SPSite(destinationRootFolderUrl.AbsoluteUri))
            using (SPWeb destinationWeb = destinationSite.OpenWeb())
            {

                WBLogging.Debug("In  WBxCopyTo(): Running as current user: " + destinationWeb.CurrentUser.Name); 

                SPFile copiedFile = null;

                destinationWeb.AllowUnsafeUpdates = true;

                SPDocumentLibrary library = sourceFile.DocumentLibrary;

                if (library.EnableVersioning)
                {
                    WBUtils.logMessage("Versioning is indeed enabled");

                    WBUtils.logMessage("sourceFile.Versions.Count = " + sourceFile.Versions.Count);

                    SPListItem docAsItem = sourceFile.Item;
                    SPListItemVersionCollection versionCollection = docAsItem.Versions;

                    WBUtils.logMessage("docAsItem.Versions.Count = " + docAsItem.Versions.Count);

                    SPListItemVersion version = versionCollection[0];

                    sourceFile = version.ListItem.File;
                }

                WBUtils.logMessage("1 About to create copy of file with sourceFile.Name = " + sourceFile.Name);

                SPFolder destinationRootFolder = destinationWeb.GetFolder(destinationRootFolderUrl.AbsolutePath);
                SPFolder actualDestinationFolder = destinationRootFolder.WBxGetOrCreateFolderPath(folderPath, forPublicWeb);

                string filename = sourceFile.Name;

                if (forPublicWeb)
                {
                    filename = WBUtils.PrepareFilenameForPublicWeb(filename);
                }

                if (destinationWeb.WBxFileExists(actualDestinationFolder, filename))
                {
                    if (allowDuplicateNames)
                    {
                        filename = destinationWeb.WBxMakeFilenameUnique(actualDestinationFolder, filename);
                    }
                    else
                    {
                        return "The filename " + filename + " already exists in the destination folder";
                    }
                }

                using (Stream stream = sourceFile.OpenBinaryStream())
                {
                    copiedFile = actualDestinationFolder.Files.Add(filename, stream);
                    stream.Close();
                }

                SPListItem destinationItem = copiedFile.Item;
                SPListItem sourceItem = sourceFile.Item;

                List<SPField> fieldsNotCopied = new List<SPField>();

                foreach (SPField field in sourceItem.Fields)
                {
                    // Note we're not copying the name field as we may have altered the name so that it is unique in the destination location.
                    if (!field.ReadOnlyField && field.Title != "Name")
                    {
                        if (destinationItem.Fields.ContainsField(field.Title))
                        {
                            WBUtils.logMessage("Attempting to update field: " + field.Title + " with value: " + sourceItem[field.Title]);

                            destinationItem[field.Title] = sourceItem[field.Title];
                        }
                        else
                        {
                            fieldsNotCopied.Add(field);
                        }
                    }
                    else
                    {
                        fieldsNotCopied.Add(field);
                    }

                }

                if (fieldsNotCopied.Count > 0)
                {
                    string notCopied = "The following fields were not copied: ";
                    foreach (SPField field in fieldsNotCopied)
                    {
                        notCopied += " '" + field.Title + "' ";
                    }

                    WBUtils.logMessage(notCopied);
                }

                destinationItem.UpdateOverwriteVersion();

                // If the new file is checked out by this creation process - then check it in:
                if (copiedFile.CheckOutType != SPFile.SPCheckOutType.None)
                {
                    copiedFile.CheckIn("Document published here from a workbox. The original source URL was: " + sourceFile.Web.Url + sourceFile.Url, SPCheckinType.MajorCheckIn);
                }

                destinationWeb.AllowUnsafeUpdates = false;

            }

            return errorMessage;
        }

        public static SPFolder WBxGetOrCreateFolderPath(this SPFolder rootFolder, String folderPathString, SPContentTypeId contentTypeId)
        {
            if (rootFolder == null) return null;
            if (String.IsNullOrEmpty(folderPathString)) return rootFolder;

            if (folderPathString.Length == 1 && folderPathString.Equals("/")) return rootFolder;

            string[] steps = folderPathString.Split('/');

            if (steps.Length == 1) return rootFolder.WBxGetOrCreateSubFolder(steps[0], contentTypeId);

            List<string> folderPath = new List<String>(steps);
            return rootFolder.WBxGetOrCreateFolderPath(folderPath, contentTypeId);
        }

        public static SPFolder WBxGetOrCreateFolderPath(this SPFolder rootFolder, List<string> folderPath, SPContentTypeId contentTypeId)
        {
            SPFolder actualFolder = rootFolder;

            foreach (String step in folderPath)
            {
                actualFolder = actualFolder.WBxGetOrCreateSubFolder(step, contentTypeId);
            }

            return actualFolder;
        }


        public static SPFolder WBxGetOrCreateSubFolder(this SPFolder parent, String childName, SPContentTypeId contentTypeId)
        {
            if (String.IsNullOrEmpty(childName)) return parent;

            SPFolderCollection subFolders = parent.SubFolders;
            SPFolder found = null;
            foreach (SPFolder subFolder in subFolders)
            {
                if (subFolder.Name.Equals(childName))
                {
                    found = subFolder;
                    break;
                }
            }

            if (found == null)
            {
                found = parent.SubFolders.Add(childName);

                found.Item.SystemUpdate();
                found.Update();

                found.Item[SPBuiltInFieldId.ContentTypeId] = contentTypeId;
                found.Item.SystemUpdate();
            }

            return found;
        }

        public static SPFolder WBxGetOrCreateFolderPath(this SPFolder rootFolder, String folderPathString)
        {
            return WBxGetOrCreateFolderPath(rootFolder, folderPathString, false);
        }

        public static SPFolder WBxGetOrCreateFolderPath(this SPFolder rootFolder, String folderPathString, bool forPublicWeb)
        {
            if (rootFolder == null) return null;
            if (String.IsNullOrEmpty(folderPathString)) return rootFolder;

            if (folderPathString.Length == 1 && folderPathString.Equals("/")) return rootFolder;

            string[] steps = folderPathString.Split('/');

            if (steps.Length == 1) return rootFolder.WBxGetOrCreateSubFolder(steps[0], forPublicWeb);

            List<string> folderPath = new List<String>(steps);
            return rootFolder.WBxGetOrCreateFolderPath(folderPath, forPublicWeb);
        }

        public static SPFolder WBxGetOrCreateFolderPath(this SPFolder rootFolder, List<string> folderPath)
        {
            return WBxGetOrCreateFolderPath(rootFolder, folderPath, false);
        }

        public static SPFolder WBxGetOrCreateFolderPath(this SPFolder rootFolder, List<string> folderPath, bool forPublicWeb)
        {
            SPFolder actualFolder = rootFolder;

            foreach (String step in folderPath)
            {
                actualFolder = actualFolder.WBxGetOrCreateSubFolder(step, forPublicWeb);
            }

            return actualFolder;
        }

        public static SPFolder WBxGetOrCreateSubFolder(this SPFolder parent, String childName)
        {
            return WBxGetOrCreateSubFolder(parent, childName, false);
        }

        public static SPFolder WBxGetOrCreateSubFolder(this SPFolder parent, String childName, bool forPublicWeb)
        {
            if (String.IsNullOrEmpty(childName)) return parent;

            if (forPublicWeb) childName = WBUtils.PrepareFilenameForPublicWeb(childName);

            SPFolderCollection subFolders = parent.SubFolders;
            SPFolder found = null;
            foreach (SPFolder subFolder in subFolders)
            {
                if (subFolder.Name.Equals(childName))
                {
                    found = subFolder;
                    break;
                }
            }

            if (found == null)
            {
                found = parent.SubFolders.Add(childName);
            }

            return found;
        }


        public static SPFolder WBxGetFolderPath(this SPFolder rootFolder, String folderPathString)
        {
            return WBxGetFolderPath(rootFolder, folderPathString, false);
        }

        public static SPFolder WBxGetFolderPath(this SPFolder rootFolder, String folderPathString, bool forPublicWeb)
        {
            if (rootFolder == null) return null;
            if (String.IsNullOrEmpty(folderPathString)) return rootFolder;

            if (folderPathString.Length == 1 && folderPathString.Equals("/")) return rootFolder;

            string[] steps = folderPathString.Split('/');

            if (steps.Length == 1) return rootFolder.WBxGetSubFolder(steps[0], forPublicWeb);

            List<string> folderPath = new List<String>(steps);
            return rootFolder.WBxGetFolderPath(folderPath, forPublicWeb);
        }

        public static SPFolder WBxGetFolderPath(this SPFolder rootFolder, List<string> folderPath)
        {
            return WBxGetFolderPath(rootFolder, folderPath, false);
        }

        public static SPFolder WBxGetFolderPath(this SPFolder rootFolder, List<string> folderPath, bool forPublicWeb)
        {
            SPFolder actualFolder = rootFolder;

            foreach (String step in folderPath)
            {
                actualFolder = actualFolder.WBxGetSubFolder(step, forPublicWeb);
                if (actualFolder == null) break;
            }

            return actualFolder;
        }

        public static SPFolder WBxGetSubFolder(this SPFolder parent, String childName)
        {
            WBLogging.Debug("Looking for subfolder called: " + childName + "       Called on folder = " + parent.Name);

            SPFolder folder = WBxGetSubFolder(parent, childName, false);

            if (folder == null) WBLogging.Debug("Didn't find folder with name " + childName);
            return folder;
        }

        public static SPFolder WBxGetSubFolder(this SPFolder parent, String childName, bool forPublicWeb)
        {
            if (parent == null) return null;
            if (String.IsNullOrEmpty(childName)) return parent;

            if (forPublicWeb) childName = WBUtils.PrepareFilenameForPublicWeb(childName);

            SPFolderCollection subFolders = parent.SubFolders;
            SPFolder found = null;
            foreach (SPFolder subFolder in subFolders)
            {
                if (subFolder.Name.Equals(childName))
                {
                    found = subFolder;
                    break;
                }
            }

            return found;
        }

        public static String WBxMakeSubFolderNameUnique(this SPFolder folder, String folderName)
        {
            String suggestedName = folderName;

            int count = 0;
            while (folder.WBxSubFolderExists(suggestedName))
            {
                count++;
                suggestedName = folderName + " (" + count + ")";

                //WBLogging.Debug(string.Format("New suggested name: {0}    ", suggestedName));

                if (count > 1000) throw new Exception("You are trying to create more than 1000 sub-folders with the same name in the same folder!");
            }

            return suggestedName;
        }

        public static bool WBxSubFolderExists(this SPFolder folder, String folderName)
        {
            SPFolderCollection subFolders = folder.SubFolders;

            foreach (SPFolder subFolder in subFolders)
            {
                if (subFolder.Name == folderName) return true;
            }

            return false;
        }

        #endregion

        #region Extensions For Creating and Executing CAML Queries

        public static SPListItemCollection WBxGetItems(this SPList list, SPSite site, WBQuery query)
        {
            return WBxGetItems(list, site, query, 0);
        }

        public static SPListItemCollection WBxGetItems(this SPList list, SPSite site, WBQuery query, int max)
        {
            return list.GetItems(query.AsSPQuery(site, list));
        }

        public static DataTable WBxGetDataTable(this SPList list, SPSite site, WBQuery query)
        {
            return WBxGetDataTable(list, site, query, 0);
        }

        public static DataTable WBxGetDataTable(this SPList list, SPSite site, WBQuery query, int max)
        {
//            WBLogging.Debug("About to do the query");

            SPListItemCollection items = list.WBxGetItems(site, query, max);

            DataTable dataTable = query.MakeResultsDataTable();

            int count = 0;

//            WBLogging.Debug("The number of items returned by query was: " + items.Count);

            foreach (SPListItem item in items)
            {
//                WBLogging.Debug("Looking at item number: " + count);
                count++;
                if (max > 0 && count > max) break;

                DataRow row = dataTable.NewRow();

                foreach (WBColumn column in query.ViewColumns)
                {
//                    WBLogging.Debug("Copying data from column: " + column.InternalName);

                    try
                    {
                        switch (column.DataType)
                        {
                            case WBColumn.DataTypes.ManagedMetadata:
                                {
                                    if (column.AllowMultipleValues)
                                    {
                                        WBTermCollection<WBTerm> terms = item.WBxGetMultiTermColumn<WBTerm>(null, column.DisplayName);
                                        if (terms != null)
                                            row[column.InternalName] = terms.Names();
                                    }
                                    else
                                    {
                                        WBTerm term = item.WBxGetSingleTermColumn<WBTerm>(null, column.DisplayName);
                                        if (term != null)
                                            row[column.InternalName] = term.Name;
                                    }
                                    break;
                                }

                            case WBColumn.DataTypes.VirtualFormattedString:
                                {
                                    if (column.InternalName == WBColumn.TitleOrName.InternalName)
                                    {
                                        string title = row[WBColumn.Title.InternalName].WBxToString();
                                        if (String.IsNullOrEmpty(title))
                                        {
                                            title = row[WBColumn.Name.InternalName].WBxToString();
                                        }
                                        row[column.InternalName] = title;
                                    }
                                    else if (column.InternalName == WBColumn.DisplayFileSize.InternalName)
                                    {
                                        string displaySize = "(unknown)";
                                        string sizeInBytesString = row[WBColumn.FileSize.InternalName].WBxToString();
                                        if (!String.IsNullOrEmpty(sizeInBytesString))
                                        {
                                            displaySize = SPUtility.FormatSize(Convert.ToInt32(sizeInBytesString));
                                        }

                                        row[column.InternalName] = displaySize;
                                    }
                                    else if (column.InternalName == WBColumn.FileType.InternalName)
                                    {
                                        string displayType = "(unknown)";
                                        string filename = row[WBColumn.Name.InternalName].WBxToString();
                                        if (!String.IsNullOrEmpty(filename))
                                        {
                                            displayType = Path.GetExtension(filename).Replace(".", "").ToUpper();
                                        }

                                        row[column.InternalName] = displayType;
                                    }
                                    else
                                    {

                                        List<String> values = new List<String>();

                                        foreach (WBColumn placeHolder in column.FormatStringPlaceHolders)
                                        {
                                            WBLogging.Debug("Looking for placeholder value for column: " + placeHolder.InternalName);
                                            WBLogging.Debug("Found value: " + row[placeHolder.InternalName].WBxToString());


                                            values.Add(row[placeHolder.InternalName].WBxToString());
                                        }

                                        row[column.InternalName] = String.Format(column.FormatString, values.ToArray());
                                    }

                                    break;
                                }

                            case WBColumn.DataTypes.VirtualConditional:
                                {
                                    if (row[column.TestColumnInternalName].ToString() == column.TestColumnValue)
                                        row[column.InternalName] = column.ValueIfEqual;
                                    else
                                        row[column.InternalName] = "";
                                    break;
                                }

                            case WBColumn.DataTypes.VirtualFileTypeIcon:
                                {
                                    row[column.InternalName] = WBUtils.DocumentIcon16(row[WBColumn.Name.InternalName].WBxToString());
                                    break;
                                }
                                

                            default:
                                {
                                    if (item[column.DisplayName] != null)
                                        row[column.InternalName] = item[column.DisplayName];
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        WBLogging.Debug("Something went wrong: " + e.Message);
                    }

                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }



        public static SPQuery WBxMakeCAMLQueryFilterBy(this SPSite site, WBTeam team, WBRecordsType recordsType, bool includeRecordsTypeDescendants)
        {
            WBTaxonomy teams = team.Taxonomy;
            WBTaxonomy recordsTypes = recordsType.Taxonomy;

            string teamFilter = site.WBxMakeCAMLClauseFilterBy("Involved_x0020_Teams", team, false);
            string recordsFilter = site.WBxMakeCAMLClauseFilterBy("Records_x0020_Type", recordsType, includeRecordsTypeDescendants);

            // Using the null value to represent 'nothing passes this filter' ... or query:
            if (teamFilter == null || recordsFilter == null) return null;

            string queryString = "";
            string queryFilters = "";
            if (teamFilter != "" && recordsFilter != "")
            {
                queryFilters += "<And>" + teamFilter + recordsFilter + "</And>";
            }
            else
            {
                // NB (at least!) one of these is blank so we wont have two clauses here:
                queryFilters += teamFilter + recordsFilter;
            }

            if (queryFilters != "")
            {
                queryString = "<Where>" + queryFilters + "</Where>";
            }

            WBUtils.logMessage("The query string was: \n" + queryString);

            SPQuery query = new SPQuery();
            query.Query = queryString;
            query.ViewAttributes = "Scope='RecursiveAll'";

            return query;
        }


        public static SPQuery WBxMakeCAMLQueryFilterBy(this SPSite site, WBRecordsType recordsType, String status, bool includeRecordsTypeDescendants)
        {
            WBTaxonomy recordsTypes = recordsType.Taxonomy;

            string statusFilter = WBUtils.MakeCAMLClauseFilterBy("WorkBoxStatus", "Text", status);
            string recordsFilter = site.WBxMakeCAMLClauseFilterBy("Records_x0020_Type", recordsType, includeRecordsTypeDescendants);

            // Using the null value to represent 'nothing passes this filter' ... or query:
            if (statusFilter == null || recordsFilter == null) return null;

            string queryString = "";
            string queryFilters = "";
            if (statusFilter != "" && recordsFilter != "")
            {
                queryFilters += "<And>" + statusFilter + recordsFilter + "</And>";
            }
            else
            {
                // NB (at least!) one of these is blank so we wont have two clauses here:
                queryFilters += statusFilter + recordsFilter;
            }

            if (queryFilters != "")
            {
                queryString = "<Where>" + queryFilters + "</Where>";
            }

            WBUtils.logMessage("The query string was: \n" + queryString);

            SPQuery query = new SPQuery();
            query.Query = queryString;
            query.ViewAttributes = "Scope='RecursiveAll'";

            return query;
        }

        
        public static int[] WBxGetWssIdsOfTerm(this SPSite site, WBTerm term) {                    
            return WBxGetWssIdsOfTerm(site, term, false);                
        }

        public static int[] WBxGetWssIdsOfTerm(this SPSite site, WBTerm term, bool includeDescendants)
        {
            return TaxonomyField.GetWssIdsOfTerm(site, term.Taxonomy.TermStore.Id, term.Taxonomy.TermSet.Id, term.Id, includeDescendants, 500);
        }


        // For this method returning "" means everything passes
        public static string WBxMakeCAMLClauseFilterBy(this SPSite site, string fieldName, WBTerm term, bool includeDescendants)
        {

            if (term == null) return "";
            int[] wssIds = site.WBxGetWssIdsOfTerm(term, includeDescendants);

            // If the site had no matching WssIds that means the term isn't being used in the site
            // so it can't possibly match - hence returning a filter clause that should always fail:
            if (wssIds.Length == 0) 
                return "<Eq><FieldRef Name='ContentType'/><Value Type='Text'>NoSuchContentTypeExists</Value></Eq>";

            string queryString = "";
            if (wssIds.Length == 1)
            {

                queryString = "<Eq><FieldRef Name='" + fieldName + "' LookupId='TRUE'/>";

                foreach (int wssId in wssIds)
                {
                    queryString += string.Format(@"<Value Type='Lookup'>{0}</Value>", wssId);
                }

                queryString += "</Eq>";

            }
            else
            {

                queryString = "<In><FieldRef Name='" + fieldName + "'  LookupId='TRUE'/><Values>";

                foreach (int wssId in wssIds)
                {
                    queryString += string.Format(@"<Value Type='Lookup'>{0}</Value>", wssId);
                }

                queryString += "</Values></In>";
            }

            return queryString;
        }


        #endregion

        #region SPWeb extensions

        public static String WBxMakeFilenameUnique(this SPWeb web, SPFolder folder, String suggestedName)
        {
            string fileNamePart = Path.GetFileNameWithoutExtension(suggestedName);
            string extension = Path.GetExtension(suggestedName);

            WBLogging.Generic.Verbose(string.Format("Trying to make the name unique: {0}    {1}", fileNamePart, extension));
            WBLogging.Generic.Verbose(string.Format("Suggested name: {0}    ", suggestedName));

            int count = 0;
            while (web.WBxFileExists(folder, suggestedName))
            {
                count++;
                suggestedName = fileNamePart + " (" + count + ")" + extension;

                WBLogging.Generic.Verbose(string.Format("New suggested name: {0}    ", suggestedName));

                if (count > 1000) throw new Exception("You are trying to create more than 1000 files with the same name in the same folder!");
            }

            return suggestedName;
        }

        public static bool WBxFileExists(this SPWeb web, SPFolder folder, String suggestedName)
        {
            string fullPath = folder.Url + "/" + suggestedName;

            WBLogging.Generic.Verbose("About to GetFile : " + fullPath + " in web: " + web.Url);
            SPFile file = web.GetFile(fullPath);

            if (file.Exists)
            {
                WBLogging.Generic.Verbose("File already exists: " + fullPath);
                return true;
            }
            else
            {
                WBLogging.Generic.Verbose("File does not exist: " + fullPath);
                return false;
            }
        }

        #endregion

        #region Permissions management extentions


        public static SPGroup WBxGetGroupOrNull(this SPWeb web, String groupName)
        {
            if (String.IsNullOrEmpty(groupName)) return null;

            foreach (SPGroup group in web.SiteGroups)
            {
                if (group.Name.ToLower() == groupName.ToLower())
                    return group;
            }

            return null;
        }

        public static SPUser WBxEnsureUserOrNull(this SPWeb web, String loginName)
        {
            SPUser user = null;

            bool previousSettingForAllowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;

            try
            {
                user = web.EnsureUser(loginName);
            }
            catch (Exception e)
            {                
            }

            web.AllowUnsafeUpdates = previousSettingForAllowUnsafeUpdates;

            return user;
        }

        public static WBUser WBxUser(this SPUser user, SPSite site, SPWeb web)
        {
            if (user == null) return null;
            return new WBUser(site, web, user);
        }

        public static String WBxToHTML(this SPUser user, UserProfileManager profileManager)
        {
            return WBxToHTML(user, profileManager, SPContext.Current.Site.RootWeb);
        }

        // Based on ideas picked up from: 
        // http://blogs.msdn.com/b/uksharepoint/archive/2010/05/07/office-communicator-integration-presence-in-a-custom-webpart-for-sharepoint-2010.aspx
        public static String WBxToHTML(this SPUser user, UserProfileManager profileManager, SPWeb rootWeb)
        {
            // If the user doesn't exist in the user profile - then we assume that they've been disabled:
            if (!profileManager.UserExists(user.LoginName))
            {
                return "<span class=\"wbf-disabled-user\">" + user.Name + "</span>";
            }

            int currentPawnCount = WBUtils.Counter("WBF_PresencePawnCounter");

            SPListItem userListItem = rootWeb.SiteUserInfoList.GetItemById(user.ID);
            string sipAddress = userListItem.WBxGetColumnAsString("SipAddress");

            string id = "WBF_PresencePawn_" + currentPawnCount;

            // return the html for this user
            return String.Concat(
            "<span id=\""
            , id
            , "_span\">"
            , "<img border=\"0\" height=\"12\" src=\"/_layouts/images/imnhdr.gif\" onload=\"WorkBoxFramework__add_user_presence('"
            , id
            , "','"
            , sipAddress
            , "', this)\" ShowOfflinePawn=\"1\" style=\"padding-right: 3px;\" id=\""
            , id 
            , "\" alt=\"Presence pawn for "
            , sipAddress
            , "\"/>"
            , "<a href=\""
            , rootWeb.Url
            , "/_layouts/userdisp.aspx?ID="
            , user.ID
            , "\" id=\""
            , id
            , "_link\">"
            , user.Name
            , "</a></span>"
            );
        }

        public static void WBxAssignADNameWithRole(this SPWeb web, String loginName, String roleName)
        {
            if (loginName == null || loginName == "") return;
            if (roleName == null || roleName == "") return;

            WBUtils.logMessage("Assigning ADName | Role: " + loginName + " | " + roleName);

            SPUser user = web.EnsureUser(loginName);
            SPRoleDefinition roleDefinition = web.RoleDefinitions[roleName];

            SPRoleAssignment roleAssignment = new SPRoleAssignment(user);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            web.RoleAssignments.Add(roleAssignment);
        }

        public static void WBxAssignGroupWithRole(this SPWeb web, String groupName, String roleName)
        {
            if (groupName == null || groupName == "") return;

            SPGroup group = web.SiteGroups[groupName];
            web.WBxAssignGroupWithRole(group, roleName);
        }

        public static void WBxAssignGroupWithRole(this SPWeb web, SPGroup group, String roleName) 
        {
            if (group == null) return;
            if (roleName == null || roleName == "") return;

            SPRoleDefinition roleDefinition = web.RoleDefinitions[roleName];

            SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            web.RoleAssignments.Add(roleAssignment);
            web.AssociatedGroups.Add(group);
        }

        public static void WBxAssignTeamMembersWithRole(this SPWeb web, SPSite site, WBTeam team, String roleName)
        {
            if (team == null) return;
            if (roleName == null || roleName == "") return;

            SPGroup group = team.MembersGroup(site);
            if (group == null)
            {
                WBLogging.Teams.Unexpected("There was no associated members group for this team: " + team.Name + " on: " + site.Url);
                return;
            }

            WBLogging.Teams.Verbose("Just about to add team members | to have role: " + team.Name + " | " + roleName);
            web.WBxAssignGroupWithRole(group, roleName);
        }

        public static void WBxAssignTeamOwnersWithRole(this SPWeb web, SPSite site, WBTeam team, String roleName)
        {
            if (team == null) return;
            if (roleName == null || roleName == "") return;

            SPGroup group = team.OwnersGroup(site);
            if (group == null)
            {
                WBLogging.Teams.Unexpected("There was no associated owners group for this team: " + team.Name + " on: " + site.Url);
                return;
            }

            WBLogging.Teams.Verbose("Just about to add team owners | to have role: " + team.Name + " | " + roleName);
            web.WBxAssignGroupWithRole(group, roleName);
        }


        public static void WBxAssignGroupWithRole(this SPListItem item, SPWeb web, SPGroup group, String roleName)
        {
            if (group == null) return;
            if (roleName == null || roleName == "") return;

            SPRoleDefinition roleDefinition = web.RoleDefinitions[roleName];

            SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            item.RoleAssignments.Add(roleAssignment);
        }

        public static void WBxAssignTeamMembersWithRole(this SPListItem item, SPSite site, SPWeb web, WBTeam team, String roleName)
        {
            if (team == null) return;
            if (roleName == null || roleName == "") return;

            SPGroup group = team.MembersGroup(site);
            if (group == null)
            {
                WBLogging.Teams.Unexpected("There was no associated members group for this team: " + team.Name + " on: " + site.Url);
                return;
            }

            WBLogging.Teams.Verbose("Just about to add team members | to have role: " + team.Name + " | " + roleName);
            item.WBxAssignGroupWithRole(web, group, roleName);
        }

        public static void WBxAssignTeamOwnersWithRole(this SPListItem item, SPSite site, SPWeb web, WBTeam team, String roleName)
        {
            if (team == null) return;
            if (roleName == null || roleName == "") return;

            SPGroup group = team.OwnersGroup(site);
            if (group == null)
            {
                WBLogging.Teams.Unexpected("There was no associated members group for this team: " + team.Name + " on: " + site.Url);
                return;
            }

            WBLogging.Teams.Verbose("Just about to add team members | to have role | to item: " + team.Name + " | " + roleName + " | " + item.Name);
            item.WBxAssignGroupWithRole(web, group, roleName);
        }


        public static void WBxRemoveGroupAssignment(this SPWeb web, String groupName)
        {
            if (groupName == null || groupName == "") return;

            SPGroup group = web.SiteGroups[groupName];
            web.RoleAssignments.Remove(group);
        }

        public static void WBxRemoveAllPermissionBindings(this SPWeb web)
        {
            List<SPPrincipal> membersToRemove = new List<SPPrincipal>();
            foreach (SPRoleAssignment assignment in web.RoleAssignments)
            {
                membersToRemove.Add(assignment.Member);
            }

            web.AllowUnsafeUpdates = true;
            foreach (SPPrincipal member in membersToRemove)
            {
                web.RoleAssignments.RemoveFromCurrentScopeOnly(member);
            }
        }


        public static void WBxRemoveAllPermissionBindings(this SPListItem item)
        {
            List<SPPrincipal> membersToRemove = new List<SPPrincipal>();
            foreach (SPRoleAssignment assignment in item.RoleAssignments)
            {
                membersToRemove.Add(assignment.Member);
            }

            foreach (SPPrincipal member in membersToRemove)
            {
                item.RoleAssignments.RemoveFromCurrentScopeOnly(member);
            }
        }


        public static void WBxAssignGroupWithRole(this SPListItem item, SPWeb web, String groupName, String roleName)
        {
            if (groupName == null || groupName == "") return;
            if (roleName == null || roleName == "") return;

            SPGroup group = web.SiteGroups[groupName];
            SPRoleDefinition roleDefinition = web.RoleDefinitions[roleName];

            SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            item.RoleAssignments.Add(roleAssignment);
            web.AssociatedGroups.Add(group);
        }



        #endregion


        #region SPGroup extensions

        public static void WBxRemoveAllUsers(this SPGroup group)
        {
            SPUserCollection allUsers = group.Users;
            foreach (SPUser user in allUsers)
            {
                group.RemoveUser(user);
            }

            group.Update();
        }

        public static void WBxCopyUsersInto(this SPGroup group, SPGroup intoGroup)
        {
            SPUserCollection allUsers = group.Users;
            foreach (SPUser user in allUsers)
            {
                try
                {
                    intoGroup.AddUser(user);
                }
                catch (Exception e)
                {
                    WBLogging.Teams.Monitorable("Failed to copy a user: " + user.LoginName + " into group: " + intoGroup.Name);
                }
            }

            intoGroup.Update();
        }

        public static bool WBxContainsUser(this SPGroup group, SPUser user)
        {
            if (user == null) return false;

            SPUserCollection allUsers = group.Users;
            foreach (SPUser groupMember in allUsers)
            {
                if (user.LoginName == groupMember.LoginName) return true;
            }
            return false;
        }

        #endregion


        #region Taxonomy extensions

        public static String WBxFullPath(this Term term)
        {
            if (term == null) return "";
            if (term.Parent == null) return term.Name;
            return term.Parent.WBxFullPath() + "/" + term.Name;
        }


        public static bool WBxContainsCurrentUserAsTeamMember(this WBTermCollection<WBTeam> teams)
        {
            foreach (WBTeam team in teams)
            {
                if (team.IsCurrentUserTeamMember()) return true;
            }

            return false;
        }


        #endregion


        #region UserProfile extensions

        public static List<String> WBxGetDirectReportsLogins(this UserProfile profile)
        {
            UserProfile[] directReportProfiles = profile.GetDirectReports();

            List<String> directReportsLogins = new List<String>();

            foreach (UserProfile directReportProfile in directReportProfiles)
            {
                String userLogin = directReportProfile["AccountName"].Value.WBxToString();

                if (!String.IsNullOrEmpty(userLogin))
                {
                    directReportsLogins.Add(userLogin);
                }
            }

            return directReportsLogins;
        }


        public static List<String> WBxGetAllReportsLogins(this UserProfile profile)
        {
            List<String> allReportsLogins = new List<String>();
            String userLogin = profile["AccountName"].Value.WBxToString();

            if (!String.IsNullOrEmpty(userLogin))
            {
                allReportsLogins.Add(userLogin);
            }


            UserProfile[] directReportProfiles = profile.GetDirectReports();

            foreach (UserProfile directReportProfile in directReportProfiles)
            {
                List<String> subLogins = directReportProfile.WBxGetAllReportsLogins();

                foreach (String subLogin in subLogins)
                {
                    if (!allReportsLogins.Contains(subLogin))
                    {
                        allReportsLogins.Add(subLogin);
                    }
                }
            }

            return allReportsLogins;
        }



        #endregion


        public static void WBxSafeSetSelectedValue(this DropDownList dropDownList, string value)
        {
            if (dropDownList.Items.FindByValue(value) != null)
            {
                dropDownList.SelectedValue = value;
            }
            else
            {
                WBLogging.Generic.Unexpected("Could not safely set the value for the drop down list. Value = " + value);
            }
        }


        public static List<String> WBxToEmails(this List<SPUser> users)
        {
            List<String> emails = new List<String>();

            foreach (SPUser user in users)
            {
                String email = user.Email;

                if (!String.IsNullOrEmpty(email))
                {
                    if (!emails.Contains(email))
                    {
                        emails.Add(email);
                    }
                }
            }

            return emails;
        }


        public static String WBxFlatten(this Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
                if (exception != null)
                {
                    stringBuilder.AppendLine("    ---- Inner Exception: ----");
                }
            }

            return stringBuilder.ToString();
        }

        public static void WBxSetLookupValue(this SPListItem item, WBColumn column, System.Web.UI.WebControls.DropDownList ddlControl)
        {
            item[column.DisplayName] = new SPFieldLookupValue(Convert.ToInt32(ddlControl.SelectedItem.Value), ddlControl.SelectedItem.Text);
        }

        public static void WBxPopulateLookupDropDownList(this System.Web.UI.WebControls.DropDownList ddlControl, SPWeb web, WBColumn column, SPListItem item)
        {
            SPListItemCollection listItems = web.Lists[column.TermSetName].Items;

            foreach (SPListItem membitem in listItems)
            {
                ddlControl.Items.Add(new System.Web.UI.WebControls.ListItem(membitem.WBxGetColumnAsString("Title"), membitem.ID.ToString()));
            }

            SPFieldLookupValue lookupValue = new SPFieldLookupValue(item.WBxGetAsString(column));
            if (lookupValue != null)
            {
                WBLogging.Debug("Setting the selected value of " + column.DisplayName + " to be: " + lookupValue.LookupId.ToString());
                ddlControl.SelectedValue = lookupValue.LookupId.ToString();
            }
            else
            {
                WBLogging.Debug("The lookup value for " + column.DisplayName + " was null!");
            }
        }

        public static void WBxCreateTasksTable(this PlaceHolder placeHolder, IEnumerable<String> taskNames)
        {
            WBxCreateTasksTable(placeHolder, taskNames, null);
        }

        public static void WBxCreateTasksTable(this PlaceHolder placeHolder, IEnumerable<String> taskNames, Dictionary<String, String> prettyNames)
        {
            Table table = new Table();
            table.CellPadding = 0;
            table.CellSpacing = 5;

            foreach (String taskName in taskNames)
            {
                TableRow row = new TableRow();
                row.ID = placeHolder.WBxMakeControlID(taskName, "row");
                row.CssClass = "wbf-task-table-row";

                Image image = new Image();
                image.ID = placeHolder.WBxMakeControlID(taskName, "image");
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/list-item-32.png";
                image.Width = Unit.Pixel(32);
                image.Height = Unit.Pixel(32);
                row.CssClass = "wbf-task-image";
                row.WBxAddInTableCell(image, "wbf-task-image-table-cell");

                System.Web.UI.WebControls.Label label = new System.Web.UI.WebControls.Label();
                label.ID = placeHolder.WBxMakeControlID(taskName, "name");

                if (prettyNames != null && prettyNames.ContainsKey(taskName))
                {
                    label.Text = prettyNames[taskName];
                }
                else
                {
                    label.Text = taskName;
                }
                label.CssClass = "wbf-task-name";
                row.WBxAddInTableCell(label, "wbf-task-name-table-cell");

                label = new System.Web.UI.WebControls.Label();
                label.ID = placeHolder.WBxMakeControlID(taskName, "status");
                label.Text = "";
                label.CssClass = "wbf-task-status";
                row.WBxAddInTableCell(label, "wbf-task-status-table-cell");

                Literal literal = new System.Web.UI.WebControls.Literal();
                literal.ID = placeHolder.WBxMakeControlID(taskName, "feedback");
                literal.Text = "";
                row.WBxAddInTableCell(literal, "wbf-task-feedback-table-cell");

                table.Rows.Add(row);
            }

            placeHolder.Controls.Add(table);
        }


        public static void WBxUpdateTask(this PlaceHolder placeHolder, WBTaskFeedback feedback)
        {
            Image image = (Image)placeHolder.WBxFindNestedControlByID(placeHolder.WBxMakeControlID(feedback.Name, "image"));
            if (feedback.Status == WBTaskFeedback.STATUS__SUCCESS)
            {
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/green-tick-32.png";
            }
            else
            {
                image.ImageUrl = "/_layouts/images/WorkBoxFramework/red-cross-32.png";
            }

            System.Web.UI.WebControls.Label label = (System.Web.UI.WebControls.Label)placeHolder.WBxFindNestedControlByID(placeHolder.WBxMakeControlID(feedback.Name, "status"));
            label.Text = feedback.TaskType + " " + feedback.Status;

            Literal literal = (System.Web.UI.WebControls.Literal)placeHolder.WBxFindNestedControlByID(placeHolder.WBxMakeControlID(feedback.Name, "feedback"));

            if (feedback.Feedback.Count <= 3) literal.Text = String.Join("<br/>", feedback.Feedback.ToArray());
            else
            {
                List<String> copy = new List<String>(feedback.Feedback);

                String first = copy[0]; copy.RemoveAt(0);
                String second = copy[0]; copy.RemoveAt(0);
                String showID = "wbf-task-feedback-show--" + feedback.Name.Replace(" ", "-");
                String hideID = "wbf-task-feedback-hide--" + feedback.Name.Replace(" ", "-");
                StringBuilder html = new StringBuilder();
                html.Append("<div>").Append(first).Append("<br/>").Append(second).Append("</div>");
                html.Append("<div id='").Append(showID).Append("'>").Append("<a href='#' onclick=' $(\"#").Append(showID).Append("\").hide(); $(\"#").Append(hideID).Append("\").show(); '/> ... show more feedback</a></div>");
                html.Append("<div id='").Append(hideID).Append("' style=' display: none;'>").Append(String.Join("<br/>", copy.ToArray())).Append("<br/><a href='#' onclick=' $(\"#").Append(hideID).Append("\").hide(); $(\"#").Append(showID).Append("\").show(); '/>Show less feedback</a></div>");

                literal.Text = html.ToString();
            }

        }

        #region Other extensions

        public static void WBxAddIfNotNullOrEmpty(this List<String> list, String value)
        {
            if (!String.IsNullOrEmpty(value)) list.Add(value);
        }



        #endregion



        /*
public static DateTime? safeStringToNullableDateTime(String value)
{
    if (value == null || value == "") return null;
    return Convert.ToDateTime(value);
}

public static String safeNullableDateTimeToString(DateTime? value)
{
    if (value == null) return "";
    return value.Value.ToString();
}
*/

    }




}
