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
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Publishing;
using System.Web.UI.WebControls;

namespace WorkBoxFramework
{
    public static class WBUtils
    {
        public static void logMessage(String message)
        {
            WBLogging.Generic.HighLevel(message);
        }


        public static void shouldThrowError(String message)
        {
            WBLogging.Generic.Unexpected("SHOULD THROW AN ERROR!!: " + message);
        }


        public static ArrayList CreateEntitiesArrayList(String value)
        {
            ArrayList entitiesArrayList = new ArrayList();
            if (value != null && value != "")
            {
                string[] stringEntities = value.Split(';');

                foreach (string stringEntity in stringEntities)
                {
                    PickerEntity entity = new PickerEntity();
                    entity.Key = stringEntity;
                    entitiesArrayList.Add(entity);
                }
            }
            return entitiesArrayList;
        }

        public static String EntitiesToPropertyString(ArrayList resolvedEntities)
        {
            return EntitiesToPropertyString(resolvedEntities, 1000000);
        }

        public static String EntitiesToPropertyString(ArrayList resolvedEntities, int maxNumber)
        {
            if (resolvedEntities == null) return "";

            if (resolvedEntities.Count > 0)
            {
                List<String> eachAsString = new List<String>();

                int count = 0;
                foreach (PickerEntity resolvedEntity in resolvedEntities)
                {
                    if (count < maxNumber) count++;
                    else break;

                    eachAsString.Add(resolvedEntity.Key);
                }

                return String.Join(";", eachAsString.ToArray());
            }
            else
            {
                return "";
            }
        }
        /*
        public static SPUser EnsureUserOrNull(SPSite site, String loginName)
        {
            SPUser user = null;
            try
            {
                user = site.RootWeb.EnsureUser(loginName);
            }
            catch (Exception e)
            {
                // Do nothing - we'll just return null;
            }

            return user;
        }
         * */

        public static SPUser GetLocalUserFromGroupOrSystemAccount(SPSite site, SPGroup fromGroup)
        {
            SPUser user = null;

            int index = 0;
            while (index < fromGroup.Users.Count && user == null)
            {
                user = site.RootWeb.WBxEnsureUserOrNull(fromGroup.Users[index].LoginName);
                index++;
            }

            if (user == null)
            {
                // OK as a last resort we'll return the system user account:
                user = site.SystemAccount;
            }

            return user;
        }

        public static void SyncSPGroup(SPSite fromSite, SPSite toSite, String groupName)
        {
            // If these happen to be the same site collection then there is nothing to do:
            if (fromSite.ID.Equals(toSite.ID)) return;

            WBLogging.Teams.Verbose("Syncing SPGroup | from | to : " + groupName + " | " + fromSite.Url + " | " + toSite.Url);

            SPGroup fromGroup = fromSite.RootWeb.WBxGetGroupOrNull(groupName);

            if (fromGroup == null)
            {
                WBUtils.shouldThrowError("Couldn't find the group that was being synced. Group Name: " + groupName);
                return;
            }

            WBLogging.Teams.Verbose("Found group in the 'from' site collection. ");


            SPGroup toGroup = toSite.RootWeb.WBxGetGroupOrNull(groupName);

            toSite.AllowUnsafeUpdates = true;
            toSite.RootWeb.AllowUnsafeUpdates = true;

            if (toGroup == null)
            {
                WBLogging.Teams.Verbose("Did not find group in the 'to' site collection. ");

                SPUser defaultUser = GetLocalUserFromGroupOrSystemAccount(toSite, fromGroup);
                SPUser systemUser = toSite.SystemAccount;

                WBLogging.Teams.Verbose("Found the user - about to create new group");
                toSite.RootWeb.SiteGroups.Add(groupName, systemUser, defaultUser, fromGroup.Description);

                WBLogging.Teams.Verbose("Created new group.");

                toGroup = toSite.RootWeb.SiteGroups[groupName];

            }
            else
            {
                WBLogging.Teams.Verbose("FOUND!! group in the 'to' site collection. ");                
            }

            // First we're going to empty the destination group of all users:
            foreach (SPUser toUser in toGroup.Users)
            {
                toGroup.RemoveUser(toUser);
            }

            // And now we'll add into the group all of the users from the source:
            foreach (SPUser fromUser in fromGroup.Users)
            {
                WBLogging.Teams.Verbose("Copying across a user: " + fromUser.LoginName);

                SPUser toUser = toSite.RootWeb.WBxEnsureUserOrNull(fromUser.LoginName);

                if (toUser != null)
                {
                    toGroup.Users.Add(toUser.LoginName, toUser.Email, toUser.Name, toUser.Notes);
                }
            }

            // Finally we'll make sure that everyone can see the membership of this group:
            toGroup.OnlyAllowMembersViewMembership = false;

            toGroup.Update();
        }


        #region CAML query helpers:
        public static string MakeCAMLClauseFilterBy(string fieldName, String valueType, String value)
        {
            string queryString = "";

            queryString = "<Eq><FieldRef Name='" + fieldName + "'/>\n";
            queryString += string.Format(@"    <Value Type='{0}'>{1}</Value>\n", valueType, value);
            queryString += "</Eq>\n";

            return queryString;
        }

        public static String CombineCAMLClausesWithAND(string[] clauses)
        {
            if (clauses == null || clauses.Length == 0) return "";
            if (clauses.Length == 1) return clauses[0];

            StringBuilder combined = new StringBuilder("<And>");
            foreach (string clause in clauses)
            {
                combined.Append(clause);
            }
            combined.Append("</And>");

            return combined.ToString();
        }

        #endregion


        #region BoundField for Control Views

        public static BoundField BoundField(WBColumn column, WBColumn sortColumn, bool ascending)
        {
            BoundField boundField = new BoundField();
            boundField.HeaderText = column.PrettyName;
            boundField.DataField = column.InternalName;
            boundField.SortExpression = column.InternalName;

            if (column.DataType == WBColumn.DataTypes.DateTime)
            {
                boundField.DataFormatString = "{0:dd/MM/yyyy}";
                boundField.HtmlEncode = false;
                boundField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            }

            if (sortColumn == column)
            {
                if (ascending) boundField.HeaderText += " ^";
                else boundField.HeaderText += " v";
            }

            return boundField;
        }

        public static BoundField BoundField(WBColumn column, HorizontalAlign horizontalAlign, WBColumn sortColumn, bool ascending)
        {
            BoundField boundField = BoundField(column, sortColumn, ascending);
            boundField.ItemStyle.HorizontalAlign = horizontalAlign;

            return boundField;
        }


        public static ButtonField RandomName(String buttonText, String commandName, WBColumn sortColumn, bool ascending)
        {


            ButtonField buttonField = new ButtonField();
            buttonField.Text = buttonText; 
            buttonField.CommandName = commandName; 

            return buttonField;
        }


        public static String DocumentIcon(String filename, IconSize size)
        {
            String url = "/_layouts/images/icdocx.png";

            if (SPContext.Current != null && !String.IsNullOrEmpty(filename))
            {
                SPWeb currentWeb = SPContext.Current.Web;


                url = SPUtility.ConcatUrls("/_layouts/images/",
                                                    SPUtility.MapToIcon(currentWeb,
                                                    SPUtility.ConcatUrls(currentWeb.Url, filename), "", size));
            }

            return url;
        }

        public static String DocumentIcon16(String filename)
        {
            return DocumentIcon(filename, IconSize.Size16);
        }

        public static String DocumentIcon32(String filename)
        {
            return DocumentIcon(filename, IconSize.Size32);
        }

        public static HyperLinkField HyperLinkField(WBColumn textColumn, String headerText, String fixedText, List<WBColumn> valuesColumns, String formatString)
        {
            HyperLinkField linkField = new HyperLinkField();
            linkField.HeaderText = headerText;
            linkField.DataTextField = textColumn.InternalName;
            //linkField.HeaderText = "";
            linkField.DataTextFormatString = fixedText;

            List<String> urlFieldsNames = new List<String>();
            foreach (WBColumn column in valuesColumns)
            {
                urlFieldsNames.Add(column.InternalName);
            }

            linkField.DataNavigateUrlFields = urlFieldsNames.ToArray();
            linkField.DataNavigateUrlFormatString = formatString;

            /*
            linkField.SortExpression = textColumn.InternalName;

            if (sortColumn == textColumn)
            {
                if (ascending) linkField.HeaderText += " ^";
                else linkField.HeaderText += " v";
            }
             */

            return linkField;
        }


        public static HyperLinkField HyperLinkField(WBColumn textColumn, WBColumn urlLinkColumn, WBColumn sortColumn, bool ascending)
        {
            HyperLinkField linkField = new HyperLinkField();
            linkField.HeaderText = "";
            linkField.DataTextField = textColumn.InternalName;
            
            string[] urlFields = { urlLinkColumn.InternalName };
            linkField.DataNavigateUrlFields = urlFields;
            linkField.DataNavigateUrlFormatString = "{0}";

            linkField.SortExpression = textColumn.InternalName;
            
            if (sortColumn == textColumn)
            {
                if (ascending) linkField.HeaderText += " ^";
                else linkField.HeaderText += " v";
            }
            
            return linkField;
        }


        public static TemplateField FixedIconTemplateField(String iconImageURL, WBColumn urlLinkColumn)
        {
            TemplateField iconLink = new TemplateField();
            iconLink.HeaderText = "";
            iconLink.ItemTemplate = new WBIconItemTemplateField(iconImageURL, urlLinkColumn);

            return iconLink;
        }

        public static TemplateField DynamicIconTemplateField(WBColumn iconImageURLColumn, WBColumn urlLinkColumn)
        {
            TemplateField iconLink = new TemplateField();
            iconLink.HeaderText = "";
            iconLink.ItemTemplate = new WBIconItemTemplateField(iconImageURLColumn, urlLinkColumn);

            return iconLink;
        }

        public static TemplateField FormatStringTemplateField(String formatString, List<WBColumn> columns)
        {
            return FormatStringTemplateField("", formatString, columns);
        }

        public static TemplateField FormatStringTemplateField(String headerText, String formatString, List<WBColumn> columns)
        {
            TemplateField templateField = new TemplateField();
            templateField.HeaderText = headerText;
            templateField.ItemTemplate = new WBFormatStringTemplateField(formatString, columns);

            return templateField;
        }


        public static List<String> GetReferencedURLs(String html)
        {
            List<String> referencedURLs = new List<String>();

            MatchCollection aTagMatches = Regex.Matches(html, @"(<a.*?>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (Match match in aTagMatches)
	        {
        	    string aTag = match.Groups[1].Value;

                Match urlMatch = Regex.Match(aTag, @"href=\""(.*?)\""", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        	    if (urlMatch.Success)
	            {
		            string foundURL = urlMatch.Groups[1].Value;
                    WBLogging.Debug("Found URL reference: " + foundURL);
                    if (!referencedURLs.Contains(foundURL))
                        referencedURLs.Add(foundURL);
                }
                else
                {
                    urlMatch = Regex.Match(aTag, @"href='(.*?)'", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            	    if (urlMatch.Success)
	                {
		                string foundURL = urlMatch.Groups[1].Value;
                        WBLogging.Debug("Found URL reference: " + foundURL);
                        if (!referencedURLs.Contains(foundURL))
                            referencedURLs.Add(foundURL);
	                }
                    else
                    {
                        WBLogging.Debug("Counldn't find any href in the following <a> tag: " + aTag);
                    }
                }
            }

            return referencedURLs;
        }

        public static String EnsureTrailingForwardSlash(String url)
        {
            if (url.LastIndexOf('/') != url.Length - 1)
            {
                url = url + "/";
            }
            return url;
        }

        public static String EnsureNoTrailingForwardSlash(String url)
        {
            if (String.IsNullOrEmpty(url) || url.Length <= 1) return url;

            if (url.LastIndexOf('/') == url.Length - 1)
            {
                url = url.Substring(0, url.Length - 1);
            }
            return url;
        }

        public static String EnsureNoLeadingForwardSlash(String url)
        {
            if (url.IndexOf('/') == 0)
            {
                url = url.Substring(1, url.Length - 1);
            }
            return url;
        }


        public static String NormalisePaths(String paths)
        {
            if (paths == null) return null;
            // if (String.IsNullOrEmpty(paths)) return "/";

            string[] pathsArray = paths.Split(';');

            List<String> normalisedPaths = new List<String>();

            foreach (String path in pathsArray)
            {
                normalisedPaths.Add(NormalisePath(path));
            }

            return String.Join(";", normalisedPaths.ToArray());
        }

        public static String NormalisePath(String path)
        {
            if (path == null) return null;
            string normalised = path.Trim();
            normalised = normalised.Replace("\\", "/");

            normalised = normalised.Replace("/////", "/");
            normalised = normalised.Replace("////", "/");
            normalised = normalised.Replace("///", "/");
            normalised = normalised.Replace("//", "/");

            normalised = WBUtils.EnsureNoTrailingForwardSlash(normalised);
            normalised = WBUtils.EnsureNoLeadingForwardSlash(normalised);

            normalised = WBUtils.RemoveDisallowedCharactersFromTermPath(normalised);
           
            return normalised;
        }

        public static List<String> GetPathStepsFromNormalisedPath(String path)
        {
            List<String> steps = new List<String>();
            if (path.Length == 1 && path.Equals("/")) return steps;
            string[] stepsArray = path.Split('/');
            foreach (String step in stepsArray)
            {
                steps.Add(step);
            }
            return steps;
        }

        public static String GetURLWithoutHostHeader(String path)
        {
            if (!path.Contains("http://")) return path;

            String newPath = path.Replace("http://", "");

            int firstForwardSlash = newPath.IndexOf('/');

            newPath = newPath.Substring(firstForwardSlash);

            // Let's now make sure that this new path starts with a forward slash:
            if (newPath.IndexOf('/') != 0) 
            {
                newPath = "/" + newPath;
            }

            return newPath;
        }

        public static String GetLastNameInPath(String path)
        {
            string[] parts = path.Split('/');

            string lastPart = null;

            foreach (String part in parts)
            {
                if (part.WBxTrim() != "")
                {
                    lastPart = part;
                }
            }

            return lastPart;
        }

        public static List<String> GetFolderPathWithoutFilename(String pathString)
        {
            List<String> path = new List<String>(pathString.Split('/'));

            int lastLocation = path.Count - 1;
            if (lastLocation >= 0)
                path.RemoveAt(lastLocation);

            return path;
        }



        public static String GetParentPath(String path, bool keepHTTPAndDomain)
        {
            bool pathContainsHTTP = false;
            if (path.Contains("http://"))
            {
                path = path.Replace("http://", "");
                pathContainsHTTP = true;
            }

            string[] parts = path.Split('/');

            List<String> trimmedParts = new List<String>();
            foreach (String part in parts) 
            {
                if (part.WBxTrim() != "")
                {
                    trimmedParts.Add(part);
                    WBLogging.Debug("Adding part: " + part);
                }

            }

            int count = trimmedParts.Count;
            if (count > 1)
            {
                trimmedParts.RemoveAt(count - 1);
            }

            path = "/" + String.Join("/", trimmedParts.ToArray());

            if (pathContainsHTTP) 
            {
                if (keepHTTPAndDomain)
                {
                    path = "http:/" + path;
                }
                else
                {
                    trimmedParts.RemoveAt(0);
                    path = "/" + String.Join("/", trimmedParts.ToArray());
                }
            }

            return path;
        }



        public static String GetURLContents(String url, String username, String password)
        {
            WBLogging.Debug("Trying to get the URL contents from: " + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
            string contents = reader.ReadToEnd();
            reader.Close();
            response.Close();

            Match contentMatch = Regex.Match(contents, @"@@@@(.*)@@@@", RegexOptions.Singleline);
            if (contentMatch.Success)
            {
                contents = contentMatch.Groups[1].Value;
            }

            WBLogging.Debug("From the URL: " + url + " got the contents: " + contents);

            return contents;
        }

        public static PageLayout GetPageLayout(PublishingWeb publishingWeb, String layoutTitle)
        {
            PageLayout[] layouts = publishingWeb.GetAvailablePageLayouts();
            PageLayout pageLayout = null;

            foreach (PageLayout layout in layouts) 
            {
                if (layout.Title.ToLower().Equals(layoutTitle.ToLower()))
                {
                    pageLayout = layout;
                    break;
                }
            }

            return pageLayout;
        }

        public static String EnsureHasHostHeader(String hostHeader, String url)
        {
            if (url.Contains("http://")) return url;

            // First remove any initial forward slash so we don't end up with double:
            if (url.IndexOf('/') == 0)
            {
                url = url.Substring(1);
            }

            return hostHeader + url;
        }

        #endregion


        internal static void RecursivelyDeleteSPWeb(SPWeb web)
        {
            if (web.Exists)
            {
                if (web.Webs.Count > 0)
                {
                    foreach (SPWeb childWeb in web.Webs)
                    {
                        RecursivelyDeleteSPWeb(childWeb);
                    }
                }

                web.Delete();
                web.Dispose();
            }
            else
            {
                WBLogging.Debug("Trying to delete an SPWeb that doesn't exist");
            }
        }

        public static SPListItem FindItemByColumn(SPSite site, SPList list, WBColumn column, String value)
        {
            WBQuery query = new WBQuery();

            query.AddFilter(column, WBQueryClause.Comparators.Equals, value);

            SPListItemCollection items = list.WBxGetItems(site, query);

            if (items.Count > 0) return items[0];

            return null;
        }

        public static String PrepareFilenameForPublicWeb(String filename)
        {
            string cleaned = filename.Replace(" ", "-");

            cleaned = cleaned.Replace("----", "-");
            cleaned = cleaned.Replace("---", "-");
            cleaned = cleaned.Replace("--", "-");

            return cleaned;
        }

        public static String RemoveDisallowedCharactersFromFilename(String filename)
        {
             // Removing the following characters:  " # % & * : < > ? \ / { | } ~

            // Also removing ; and , even though they are technically allowed by SharePoint
            char[] toClean = filename.ToCharArray();

            for (int i = 0; i < toClean.Length; i++)
            {
                char c = toClean[i];

                // We'll remove completely all of these tildas afterwards:
                if (c < 32 || c > 126) toClean[i] = '~';
                else
                {
                    switch (c)
                    {
                        case '"': { toClean[i] = '_'; break; }
                        case '*': { toClean[i] = '_'; break; }
                        case ':': { toClean[i] = '-'; break; }
                        case '<': { toClean[i] = '_'; break; }
                        case '>': { toClean[i] = '_'; break; }
                        case '?': { toClean[i] = '_'; break; }
                        case '\\': { toClean[i] = '-'; break; }
                        case '/': { toClean[i] = '-'; break; }
                        case '{': { toClean[i] = '('; break; }
                        case '|': { toClean[i] = '-'; break; }
                        case '}': { toClean[i] = ')'; break; }
                        case '~': { toClean[i] = '-'; break; }
                        case ';': { toClean[i] = '-'; break; }
                        case ',': { toClean[i] = ' '; break; }
                        case '^': { toClean[i] = ' '; break; }
                        case '!': { toClean[i] = ' '; break; }
                        case '=': { toClean[i] = '-'; break; }
                        case '¬': { toClean[i] = '-'; break; }
                    }
                }
            }

            String cleaned = new String(toClean);

            cleaned = cleaned.Replace("~", "");
            cleaned = cleaned.Replace("#", "hash");
            cleaned = cleaned.Replace("%", "percent");
            cleaned = cleaned.Replace("&", "and");
            cleaned = cleaned.Replace("....", ".");
            cleaned = cleaned.Replace("...", ".");
            cleaned = cleaned.Replace("..", ".");
            cleaned = cleaned.Replace("    ", " ");
            cleaned = cleaned.Replace("   ", " ");
            cleaned = cleaned.Replace("  ", " ");
            cleaned = cleaned.Replace("----", "-");
            cleaned = cleaned.Replace("---", "-");
            cleaned = cleaned.Replace("--", "-");

            return cleaned;
        }

        public static String RemoveDisallowedCharactersFromTermPath(String termPath)
        {
            // Removing the following characters:  " # % & * : < > ? \ / { | } ~

            termPath = termPath.Replace('"', ' ');
            termPath = termPath.Replace("#", "hash");
            termPath = termPath.Replace("%", "percent");
            termPath = termPath.Replace("&", "and");
            termPath = termPath.Replace('*', ' ');
            termPath = termPath.Replace(':', '-');
            termPath = termPath.Replace('<', ' ');
            termPath = termPath.Replace('>', ' ');
            termPath = termPath.Replace('?', ' ');
            termPath = termPath.Replace('\\', '/');
            termPath = termPath.Replace('{', '(');
            termPath = termPath.Replace('|', '-');
            termPath = termPath.Replace('}', ')');
            termPath = termPath.Replace('~', '-');

            return termPath;
        }

        //private static 
        public static DateTime ParseDate(String dateString)
        {
            string[] dateFormatsToTry = {
                                                  "yyyy'-'MM'-'dd HH':'mm':'ss",
                                                  "yyyy'/'MM'/'dd HH':'mm",
                                                  "dd'/'MM'/'yyyy HH':'mm",
                                                  "dd'/'MM'/'yyyy"
                                              };

            try
            {
                DateTime dateParsed = DateTime.ParseExact(dateString, dateFormatsToTry, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal);

                WBLogging.Debug("Successfully parsed the String: " + dateString + " to get DateTime: " + dateParsed.ToLongDateString());

                if (dateParsed.Year < 1900 || dateParsed.Year > 2100)
                    throw new Exception("The date was either before 1900 or after 2100 which is a bit odd - hence failing for now.");

                return dateParsed;
            }
            catch (FormatException exception)
            {
                throw new Exception("WBUtils.ParseDate(): The following string could not be parsed as a DateTime: " + dateString, exception);
            }
        }


        public const String HASH_REPLACEMENT_TEXT = "__HASH__";
        public const String SEMICOLON_REPLACEMENT_TEXT = "__SEMICOLON__";
        public const String COMMA_REPLACEMENT_TEXT = "__COMMA__";
        public const String PIPE_REPLACEMENT_TEXT = "__PIPE__";
        public const String NEW_LINE_REPLACEMENT_TEXT = "__NEW_LINE__";

        public const String OLD_HASH_REPLACEMENT_TEXT = "__HASH_CHARACTER__";
        public const String OLD_SEMICOLON_REPLACEMENT_TEXT = "__SEMICOLON_CHARACTER__";
        public const String OLD_COMMA_REPLACEMENT_TEXT = "__COMMA_CHARACTER__";
        public const String OLD_PIPE_REPLACEMENT_TEXT = "__PIPE_CHARACTER__";
        public const String OLD_NEW_LINE_REPLACEMENT_TEXT = "__NEW_LINE_CHARACTER__";


        public static String PutBackDelimiterCharacters(String text)
        {
            if (String.IsNullOrEmpty(text)) return "";

            text = text.Replace(HASH_REPLACEMENT_TEXT, "#");
            text = text.Replace(SEMICOLON_REPLACEMENT_TEXT, ";");
            text = text.Replace(COMMA_REPLACEMENT_TEXT, ",");
            text = text.Replace(PIPE_REPLACEMENT_TEXT, "|");
            text = text.Replace(NEW_LINE_REPLACEMENT_TEXT, "\n");

            text = text.Replace(OLD_HASH_REPLACEMENT_TEXT, "#");
            text = text.Replace(OLD_SEMICOLON_REPLACEMENT_TEXT, ";");
            text = text.Replace(OLD_COMMA_REPLACEMENT_TEXT, ",");
            text = text.Replace(OLD_PIPE_REPLACEMENT_TEXT, "|");
            text = text.Replace(OLD_NEW_LINE_REPLACEMENT_TEXT, "\n");

            return text;
        }

        public static String ReplaceDelimiterCharacters(String text)
        {
            if (String.IsNullOrEmpty(text)) return "";

            text = text.Replace("#", HASH_REPLACEMENT_TEXT);
            text = text.Replace(";", SEMICOLON_REPLACEMENT_TEXT);
            text = text.Replace(",", COMMA_REPLACEMENT_TEXT);
            text = text.Replace("|", PIPE_REPLACEMENT_TEXT);
            text = text.Replace("\r\n", NEW_LINE_REPLACEMENT_TEXT);
            text = text.Replace("\n", NEW_LINE_REPLACEMENT_TEXT);
           
            return text;
        }

        public static String MaybeAddParagraphTags(String text)
        {
            if (String.IsNullOrEmpty(text)) return "";
            if (text.Contains("<p>") || text.Contains("</p>")) return text;

            text = "<p>" + text.Trim().Replace("\r\n", "</p><p>").Replace("\n","</p><p>") + "</p>";

            return text;
        }

        internal static String ProcessTemplate(String templateText, StringDictionary textForToken)
        {
            String processedText = templateText;

            foreach (String token in textForToken.Keys)
            {
                processedText = processedText.Replace(token, textForToken[token]);
            }

            return processedText;
        }

        internal static string ProcessEmailTemplate(WorkBox workBox, string text, bool forHTML)
        {
            return ProcessEmailTemplate(workBox, null, null, text, forHTML);
        }

        internal static string ProcessEmailTemplate(WorkBox workBox, WBTeam team, SPUser user, string text, bool forHTML)
        {
            return ProcessEmailTemplate(null, workBox, team, user, text, forHTML);
        }

        internal static string ProcessEmailTemplate(Dictionary<String,String> textForTokens, WorkBox workBox, WBTeam team, SPUser user, string text, bool forHTML)
        {
            if (textForTokens != null)
            {
                foreach (String token in textForTokens.Keys)
                {
                    WBLogging.Debug("Replacing " + token + " with: " + textForTokens[token]);
                    text = text.Replace(token, textForTokens[token]);
                }
            }

            if (workBox != null)
            {
                text = text.Replace("[WORK_BOX_TITLE]", workBox.Title);
                if (forHTML)
                {
                    text = text.Replace("[WORK_BOX_URL]", "<a href=\"" + workBox.Url + "\">" + workBox.Url + "</a>");
                }
                else
                {
                    text = text.Replace("[WORK_BOX_URL]", workBox.Url);
                }
            }

            if (team != null)
            {
                text = text.Replace("[TEAM_NAME]", team.Name);
                if (forHTML)
                {
                    text = text.Replace("[TEAM_SITE_URL]", "<a href=\"" + team.TeamSiteUrl + "\">" + team.TeamSiteUrl + "</a>");
                }
                else
                {
                    text = text.Replace("[TEAM_SITE_URL]", team.TeamSiteUrl);
                }
            }

            if (user != null)
            {
                text = text.Replace("[USER_NAME]", user.Name);
            }
            else
            {
                text = text.Replace("[USER_NAME]", "All");
            }

            if (forHTML)
            {
                text = WBUtils.MaybeAddParagraphTags(text);
            }

            return text;
        }


        public static bool SendEmail(SPWeb spWeb, String to, String subject, String body, bool isBodyHtml)
        {
            StringDictionary messageHeaders = new StringDictionary();

            messageHeaders.Add("to", to);
            messageHeaders.Add("subject", subject);

            string mimeType = "text/plain";
            if (isBodyHtml)
            {
                mimeType = "text/html";
            }

            messageHeaders.Add("content-type", mimeType);

            bool mailSent = SendEmail(
                   spWeb,
                   messageHeaders,
                   body);

            return mailSent;
        }


        public static bool SendEmail(SPWeb web, StringDictionary headers, string body)
        {
            if (WBFarm.Local.FarmInstance == WBFarm.FARM_INSTANCE__DEVELOPMENT_FARM)
            {
                WBLogging.Debug("Trying to send an email. Headers:");
                foreach (String key in headers.Keys)
                {
                    WBLogging.Debug("HEADER: " + key + " : " + headers[key]);
                }
                WBLogging.Debug("BODY: " + body);

                return true;
            }
            else
            {
                return SPUtility.SendEmail(web, headers, body);
            }
        }

        internal static List<SPUser> RemoveUser(List<SPUser> users, SPUser userToRemove)
        {
            List<SPUser> newUsersList = new List<SPUser>();

            foreach (SPUser user in users)
            {
                if (user.LoginName != userToRemove.LoginName)
                {
                    newUsersList.Add(user);
                }
            }

            return newUsersList;
        }

        public static List<SPUser> GetSPUsers(SPWeb web, List<String> loginNames)
        {
            List<SPUser> spUsers = new List<SPUser>();

            foreach (String loginName in loginNames)
            {
                SPUser spUser = null;
                try
                {
                    spUser = web.EnsureUser(loginName);
                }
                catch 
                {
                    WBLogging.Generic.Verbose("Couldn't find the SPUser details for login name: " + loginName);                        
                }

                if (spUser != null)
                {
                    spUsers.Add(spUser);
                }

            }

            return spUsers;
        }
    }
}
