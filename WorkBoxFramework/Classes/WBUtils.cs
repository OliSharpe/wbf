﻿#region Copyright and License

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
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Navigation;
using Microsoft.Office.Server.UserProfiles;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

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

        public static SPGroup SyncSPGroup(SPSite fromSite, SPSite toSite, String groupName)
        {
            WBLogging.Teams.Verbose("Syncing SPGroup | from | to : " + groupName + " | " + fromSite.Url + " | " + toSite.Url);

            SPGroup fromGroup = fromSite.RootWeb.WBxGetGroupOrNull(groupName);

            // If these happen to be the same site collection then there is nothing to do:
            if (fromSite.ID.Equals(toSite.ID)) return fromGroup;

            if (fromGroup == null)
            {
                WBUtils.shouldThrowError("Couldn't find the group that was being synced. Group Name: " + groupName);
                return null;
            }

            WBLogging.Teams.Verbose("Found group in the 'from' site collection. ");

            SPServiceContext serviceContext = SPServiceContext.GetContext(fromSite);
            UserProfileManager profileManager = new UserProfileManager(serviceContext);

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

            // First we're going to remove the extra users in the toGroup that need to be removed:
            foreach (SPUser toUser in toGroup.Users)
            {
                try
                {
                    if (!fromGroup.WBxContainsUser(toUser))
                    {
                        WBLogging.Teams.Verbose("On site removing from group an un-needed user: " + toSite.Url + " | " + toGroup.Name + " | " + toUser.LoginName);

                        toGroup.RemoveUser(toUser);
                    }
                }
                catch (Exception e)
                {
                    WBLogging.Teams.Monitorable("There was a exception when trying to remove user: " + toUser.LoginName + " from group: " + toGroup.Name + " on site: " + toSite.Url);
                }
            }

            // And now we'll add into the group all of missing users from the fromGroup that need to be added:
            foreach (SPUser fromUser in fromGroup.Users)
            {
                // If the user doesn't exist in the user profile - then we assume that they've been disabled:
                if (!profileManager.UserExists(fromUser.LoginName))
                {
                    WBLogging.Teams.Monitorable("Ignoring user as they appear to be disabled: " + fromUser.LoginName);
                    continue;
                }

                SPUser toUser = toSite.RootWeb.WBxEnsureUserOrNull(fromUser.LoginName);

                try
                {
                    if (toUser != null && !toGroup.WBxContainsUser(toUser))
                    {
                        WBLogging.Teams.Verbose("On site adding to group a missing user: " + toSite.Url + " | " + toGroup.Name + " | " + toUser.LoginName);

                        toGroup.AddUser(toUser);
                    }
                }
                catch (Exception e)
                {
                    WBLogging.Teams.Monitorable("There was a exception when trying to add user: " + fromUser.LoginName + " to group: " + fromGroup.Name + " on site: " + toSite.Url);
                }

            }

            if (toGroup.Users.Count != fromGroup.Users.Count)
            {
                WBLogging.Teams.Unexpected("Synced groups have different number of users: toSite | fromGroup | toGroup : " + toSite.Url + " | " + fromGroup.Users.Count + " | " + toGroup.Users.Count);
            }
            else
            {
                WBLogging.Teams.Verbose("Synced groups now have same number of users: toSite | fromGroup | toGroup : " + toSite.Url + " | " + fromGroup.Users.Count + " | " + toGroup.Users.Count);
            }

            // Finally we'll make sure that everyone can see the membership of this group:
            toGroup.OnlyAllowMembersViewMembership = false;

            toGroup.Update();

            return toGroup;
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

        public static HyperLinkField HyperLinkField(WBColumn textColumn, WBColumn urlLinkColumn, WBColumn sortColumn, bool ascending, String target)
        {
            HyperLinkField linkField = new HyperLinkField();
            linkField.HeaderText = textColumn.PrettyName;
            linkField.DataTextField = textColumn.InternalName;
            linkField.Target = target;

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

        public static String StatusIconImageURL(String status, String size)
        {
            status = status.ToLower();

            if (status != "open" && status != "closed" && status != "deleted") status = "error";

            return "/_layouts/images/WorkBoxFramework/work-box-" + status + "-" + size + ".png";
        }

        public static TemplateField StatusIconTemplateField(String size)
        {
            TemplateField iconLink = new TemplateField();
            iconLink.HeaderText = "";
            iconLink.ItemTemplate = new WBStatusIconTemplateField(size);

            return iconLink;
        }

        public static TemplateField DynamicFormattedIconTemplateField(String formatString, List<WBColumn> columns, WBColumn urlLinkColumn)
        {
            TemplateField iconLink = new TemplateField();
            iconLink.HeaderText = "";
            iconLink.ItemTemplate = new WBDynamicFormattedIconTemplateField(formatString, columns, urlLinkColumn);

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

        public static TemplateField FormatStringTemplateField(String headerText, String formatString, List<WBColumn> columns, bool useLowerCase)
        {
            TemplateField templateField = new TemplateField();
            templateField.HeaderText = headerText;
            templateField.ItemTemplate = new WBFormatStringTemplateField(formatString, columns, useLowerCase);

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

        public static String GetExtension(String filename)
        {
            return Path.GetExtension(filename).Replace(".", "").ToLower();
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


        public static bool CutOrCopyIntoFolder(SPWeb web, SPFolder folder, SPListItem item, bool cutOriginal)
        {
            bool success = true;

            try
            {
                if (item.Folder == null)
                {
                    String filename = item.Name;

                    // I'm fairly certain that this wont introduce a new 'SPWeb' object that isn't (in theory)
                    // being handled somewhere in the calling code for this method.
                    filename = web.WBxMakeFilenameUnique(folder, filename);

                    SPFile copiedFile = null;

                    using (Stream stream = item.File.OpenBinaryStream())
                    {
                        copiedFile = folder.Files.Add(filename, stream);
                        stream.Close();
                    }
                }
                else
                {
                    String folderName = item.Name;

                    folderName = folder.WBxMakeSubFolderNameUnique(folderName);

                    SPFolder subFolder = folder.SubFolders.Add(folderName);

                    foreach (SPFile file in item.Folder.Files)
                    {
                        CutOrCopyIntoFolder(web, subFolder, file.Item, cutOriginal);
                    }

                    foreach (SPFolder child in item.Folder.SubFolders)
                    {
                        CutOrCopyIntoFolder(web, subFolder, child.Item, cutOriginal);
                    }
                }

                if (cutOriginal)
                {
                    try
                    {
                        item.Recycle();
                    }
                    catch (Exception exception)
                    {
                        WBLogging.Generic.Unexpected("Was not able to recycle an item that should be cut from " + folder.ServerRelativeUrl + " with ID " + item.ID + " and name: " + item.Name, exception);
                    }
                }
            }
            catch (Exception e)
            {
                WBLogging.Generic.Unexpected(e);
                success = false;
            }

            return success;
        }



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

        internal static String ProcessColumnTokensTemplate(String templateText, SPListItem item)
        {
            StringDictionary textForToken = new StringDictionary();

            Regex expression = new Regex(@"\[(?<Token>.*)\]");

            MatchCollection matches = expression.Matches(templateText);
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Groups["Token"].Value);
            }

            return ProcessTemplate(templateText, textForToken);
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


        public static void AddEmailAddresses(IEnumerable users, List<String> emailAddresses)
        {
            foreach (SPUser user in users)
            {
                if (!String.IsNullOrEmpty(user.Email))
                {
                    if (!emailAddresses.Contains(user.Email))
                    {
                        emailAddresses.Add(user.Email);
                    }
                }
            }
        }

        public static void SendEmails(SPWeb spWeb, List<String> emails, String subject, String body, bool isBodyHtml)
        {
            foreach (String email in emails)
            {
                SendEmail(spWeb, email.WBxTrim(), subject, body, isBodyHtml);
            }
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

        public static void SendErrorReport(SPWeb spWeb, String subject, String body)
        {
            WBLogging.Generic.Unexpected("Sending error report: " + subject);

            try
            {
                String emailAddressToUse = WBFarm.Local.SendErrorReportEmailsTo;

                if (String.IsNullOrEmpty(emailAddressToUse))
                {
                    WBLogging.Generic.Unexpected("No configured email address to send error reports from this farm: " + subject + "\n\n" + body);
                    return;
                }

                if (emailAddressToUse.Contains(";"))
                {
                    List<String> emailAddresses = new List<String>(emailAddressToUse.Split(';'));

                    WBUtils.SendEmails(spWeb, emailAddresses, subject, body, false);
                }
                else
                {
                    WBUtils.SendEmail(spWeb, emailAddressToUse, subject, body, false);
                }

            }
            catch (Exception e)
            {
                WBLogging.Generic.Unexpected("Something went wrong when trying to send an error report", e);
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


        public static String GenerateLinkToEmailGroup(String text, List<String> emails)
        {
            return GenerateLinkToEmailGroup(text, emails, null, null);
        }

        public static String GenerateLinkToEmailGroup(String text, List<String> emails, Dictionary<String,String> headers)
        {
            return GenerateLinkToEmailGroup(text, emails, headers, null);
        }

        public static String GenerateLinkToEmailGroup(String text, List<String> emails, Dictionary<String, String> headers, String cssClass)
        {
            String cssString = "";
            if (!String.IsNullOrEmpty(cssClass))
            {
                cssString = " class=\"" + cssClass + "\"";
            }

            String headersString = "";
            List<String> forMailTo = new List<String>();
            List<String> forDialogLink = new List<String>();

            if (headers != null)
            {
                foreach (String key in headers.Keys)
                {
                    forMailTo.Add(key + "=" + HttpUtility.UrlPathEncode(headers[key]));
                    forDialogLink.Add(key + "=" + headers[key]);
                }

                headersString = "?" + String.Join("&", forMailTo.ToArray());
            }

            String mailToLink = "mailto:" + String.Join(";", emails.ToArray()) + headersString;

            WBFarm farm = WBFarm.Local;
            if (farm.UseMailToLinks && mailToLink.Length < farm.ChatacterLimitForMailToLinks)
            {
                return "<a href=\"" + mailToLink + "\"" + cssString + ">" + text + "</a>";
            }
            else
            {
                if (headers == null)
                {
                    headers = new Dictionary<String, String>();
                }

                if (!headers.ContainsKey("subject")) { headers["subject"] = ""; }
                if (!headers.ContainsKey("body")) { headers["body"] = ""; }

                String emailsList = "";
                if (emails != null && emails.Count > 0)
                {
                    emailsList = String.Join("; ", emails.ToArray());
                }

                int mailtoReplacementCounter = WBUtils.Counter("WBF_MailtoReplacementCounter");

                String html = "<script type=\"text/javascript\">\n";
                html += "    var emailTo_" + mailtoReplacementCounter + " = \"" + HttpUtility.UrlEncode(emailsList) + "\";\n";
                html += "    var emailSubject_" + mailtoReplacementCounter + " = \"" + HttpUtility.UrlEncode(headers["subject"]) + "\";\n";
                html += "    var emailBody_" + mailtoReplacementCounter + " = \"" + HttpUtility.UrlEncode(headers["body"]) + "\";\n";
                html += "</script>\n";

                html += "<a href=\"javascript: WorkBoxFramework_relativeCommandAction('MailToLinkReplacement.aspx?to=' + emailTo_" + mailtoReplacementCounter + " + '&subject=' + emailSubject_" + mailtoReplacementCounter + " + '&body=' + emailBody_" + mailtoReplacementCounter + " , 0, 0); \"" + cssString + ">" + text + "</a>\n";

                return html;
            }
           
        }


        /// <summary>
        /// Returns true if the method created a new list otherwise false to indicate that the named list exists already.
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="rootWeb"></param>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <param name="itemContentTypeName"></param>
        /// <returns></returns>
        public static bool CreateOrCheckListUsingContentType(WBTaskFeedback feedback, SPWeb rootWeb, SPWeb web, String listName, String itemContentTypeName)
        {
            feedback.JustLog("Starting CreateOrCheckCustomList with custom content type for: " + listName);

            SPList list = web.Lists.TryGetList(listName);
            if (list != null)
            {
                feedback.Checked("Found existig list - not updating yet so: Finished CreateOrCheckCustomList for: " + listName);

                return false;
            }

            feedback.JustLog("Here: " + listName);

            SPContentType itemContentType = rootWeb.ContentTypes.Cast<SPContentType>()
                .FirstOrDefault(c => c.Name == itemContentTypeName);

            feedback.JustLog("Here now: " + listName);

            if (itemContentType == null)
            {
                throw new NotImplementedException("Not yet handling the situation where the list item content type for a new list has not yet been created as a site content type: " + itemContentTypeName);
            }

            feedback.JustLog("Next: " + listName);

            Guid newListGuid = web.Lists.Add(listName, "", SPListTemplateType.GenericList);

            feedback.Created("Created list: " + listName);

            feedback.JustLog("One more: " + listName);

            list = web.Lists[newListGuid];

            list.ContentTypesEnabled = true;

            list.ContentTypes.Add(itemContentType);
            list.Update();


            List<SPContentType> contentTypesToRemove = new List<SPContentType>();
            foreach (SPContentType contentType in list.ContentTypes)
            {
                feedback.JustLog("List has content type: " + contentType.Name);
                if (contentType.Name != itemContentType.Name)
                {
                    feedback.JustLog("Added to list to remove content type: " + contentType.Name);
                    contentTypesToRemove.Add(contentType);
                }

            }

            foreach (SPContentType contentType in contentTypesToRemove)
            {
                feedback.JustLog("Trying to remove content type: " + contentType.Name);
                list.ContentTypes.Delete(contentType.Id);
            }

            list.Update();

            feedback.Success("Finished config for: " + listName);

            return true;
        }

        public static SPList CreateOrCheckCustomList(WBTaskFeedback feedback, SPWeb rootWeb, SPWeb web, String listName, IEnumerable<WBColumn> columns)
        {
            return CreateOrCheckCustomList(feedback, rootWeb, web, listName, columns, false);
        }

        public static SPList CreateOrCheckCustomList(WBTaskFeedback feedback, SPWeb rootWeb, SPWeb web, String listTitle, IEnumerable<WBColumn> columns, bool removeSpacesFromListName)
        {
            feedback.JustLog("Starting CreateOrCheckCustomList with custom columns for: " + listTitle);
            String listName = listTitle;
            if (removeSpacesFromListName)
            {
                listName = listName.Replace(" ", "");
            }

            SPList list = web.Lists.TryGetList(listTitle);

            bool listNeedsUpdating = false;
            if (list == null)
            {
                Guid listGuid = web.Lists.Add(listName, "A WBF configuration list", SPListTemplateType.GenericList);

                feedback.Created("On " + web.Url + " Created list: " + listName);

                list = web.Lists[listGuid];
                list.Title = listTitle;
                listNeedsUpdating = true;
            }
            else
            {
                feedback.Checked("On " + web.Url + " Found list: " + listTitle);
            }

            foreach (WBColumn column in columns)
            {
                if (!list.Fields.ContainsField(column.DisplayName))
                {
                    SPField field = rootWeb.Fields[column.DisplayName];

                    list.Fields.Add(field);

                    feedback.Checked("List " + listTitle + " Added column: " + column.DisplayName);

                    listNeedsUpdating = true;
                }
                else
                {
                    feedback.Checked("List " + listTitle + " has column: " + column.DisplayName);
                }
            }

            if (listNeedsUpdating)
            {
                list.Update();
                web.Update();
            }

            feedback.JustLog("Finished CreateOrCheckCustomList for: " + listTitle);

            return list;
        }



        public static SPContentType CreateOrCheckContentType(
            WBTaskFeedback feedback, 
            SPWeb web,
            String contentTypeName,
            String parentContentTypeName,
            String groupName,
            IEnumerable<WBColumn> requiredFields,
            IEnumerable<WBColumn> optionalFields)
        {

            // We're only going to create this content type if it doesn't already exist:
            SPContentType existingContentType = web.ContentTypes.Cast<SPContentType>()
                .FirstOrDefault(c => c.Name == contentTypeName);

            if (existingContentType != null)
            {
                bool columnsAdded = false;

                feedback.JustLog("On " + web.Url + " Found content type " + contentTypeName);

                foreach (WBColumn column in requiredFields)
                {
                    if (!existingContentType.Fields.ContainsField(column.DisplayName))
                    {
                        SPFieldLink fieldLink = new SPFieldLink(web.Fields[column.DisplayName]);
                        existingContentType.FieldLinks.Add(fieldLink);
                        fieldLink.Required = true;
                        columnsAdded = true;
                        feedback.Updating("Added column " + column.DisplayName + " to content type " + contentTypeName);
                    }
                }

                foreach (WBColumn column in optionalFields)
                {
                    if (!existingContentType.Fields.ContainsField(column.DisplayName))
                    {
                        SPFieldLink fieldLink = new SPFieldLink(web.Fields[column.DisplayName]);
                        existingContentType.FieldLinks.Add(fieldLink);
                        fieldLink.Required = false;
                        columnsAdded = true;
                        feedback.Updating("Added column " + column.DisplayName + " to content type " + contentTypeName);
                    }
                }

                if (columnsAdded)
                {
                    existingContentType.Update(true);
                    feedback.Updated("Finished adding any missing columns to " + contentTypeName);
                }
                else
                {
                    feedback.Checked("Checked no missing columns for " + contentTypeName);
                }

                return existingContentType;
            }

            // OK so now we can create the content type:
            feedback.JustLog("On " + web.Url + " Creating content type: " + contentTypeName);

            SPContentType newContentType = new SPContentType(
                web.ContentTypes[parentContentTypeName],
                web.ContentTypes,
                contentTypeName);

            newContentType.Group = groupName;

            foreach (WBColumn column in requiredFields)
            {
                SPFieldLink fieldLink = new SPFieldLink(web.Fields[column.DisplayName]);
                newContentType.FieldLinks.Add(fieldLink);
                fieldLink.Required = true;
            }

            foreach (WBColumn column in optionalFields)
            {
                SPFieldLink fieldLink = new SPFieldLink(web.Fields[column.DisplayName]);
                newContentType.FieldLinks.Add(fieldLink);
                fieldLink.Required = false;
            }

            // And finally add this content type to the web (should be a root web):
            web.ContentTypes.Add(newContentType);
            newContentType.Update();

            feedback.Created("On " + web.Url + " Created content type: " + contentTypeName);

            return newContentType;
        }


        public static void CreateOrCheckPermissionLevel(
            WBTaskFeedback feedback, 
            SPSite site,
            String permissionLevelName,
            String permissionLevelDescription,
            SPBasePermissions permissionLevelBasePermissions
            )
        {
            SPRoleDefinitionCollection roles = site.RootWeb.RoleDefinitions;
            foreach (SPRoleDefinition role in roles)
            {
                if (role.Name == permissionLevelName)
                {
                    feedback.Checked("Found permission level: " + permissionLevelName);
                    return;
                }
            }

            // if we're here the we haven't found the permission level:
            SPRoleDefinition newPermissionLevel = new SPRoleDefinition();
            newPermissionLevel.Name = permissionLevelName;
            newPermissionLevel.Description = permissionLevelDescription;
            newPermissionLevel.BasePermissions = permissionLevelBasePermissions;

            site.RootWeb.RoleDefinitions.Add(newPermissionLevel);

            feedback.Created("Created permission level: " + permissionLevelName);
        }

        /// <summary>
        /// Create a new top level Quick Navigation link (internal to site collection)
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="web"></param>
        /// <param name="navLinkTitle"></param>
        /// <param name="navLinkURL"></param>
        public static void CheckOrCreateQuickLaunchNav(WBTaskFeedback feedback, SPWeb web, String navLinkTitle, String navLinkURL)
        {
            CheckOrCreateQuickLaunchNav(feedback, web, "", navLinkTitle, navLinkURL, false);
        }

        /// <summary>
        /// Create a new second level Quick Navigation link (internal to site collection)
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="web"></param>
        /// <param name="heading"></param>
        /// <param name="navLinkTitle"></param>
        /// <param name="navLinkURL"></param>
        public static void CheckOrCreateQuickLaunchNav(WBTaskFeedback feedback, SPWeb web, String heading, String navLinkTitle, String navLinkURL)
        {
            CheckOrCreateQuickLaunchNav(feedback, web, heading, navLinkTitle, navLinkURL, false);
        }

        /// <summary>
        /// Create a new Quick Navigation link with all creation options available.
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="web"></param>
        /// <param name="heading"></param>
        /// <param name="navLinkTitle"></param>
        /// <param name="navLinkURL"></param>
        /// <param name="isExternal"></param>
        public static void CheckOrCreateQuickLaunchNav(WBTaskFeedback feedback, SPWeb web, String heading, String navLinkTitle, String navLinkURL, bool isExternal)
        {
            SPNavigationNodeCollection nodes = web.Navigation.QuickLaunch;

            bool webNeedsUpdating = false;

            SPNavigationNode headingNode = null;
            if (!String.IsNullOrEmpty(heading))
            {
                foreach (SPNavigationNode node in nodes)
                {
                    if (node.Title == heading)
                    {
                        headingNode = node;
                        feedback.Checked("Found quick launch nav heading: " + heading);
                        break;
                    }
                }

                if (headingNode == null)
                {
                    // OK we're going to add the heading:
                    String headingURL = "";

                    // Just to restore some of the classic headings if that is what is being asked for here:
                    if (heading == "Libraries") headingURL = "/_layouts/viewlsts.aspx?BaseType=1";

                    if (heading == "Lists") headingURL = "/_layouts/viewlsts.aspx?BaseType=0";

                    if (heading == "Discussions") headingURL = "/_layouts/viewlsts.aspx?BaseType=0&ListTemplate=108";

                    headingNode = new SPNavigationNode(heading, headingURL);

                    nodes.AddAsLast(headingNode);
                    webNeedsUpdating = true;
                    feedback.Created("Created new header in quick nav: " + heading);
                }

                nodes = headingNode.Children;
            }

            foreach (SPNavigationNode node in nodes)
            {
                if (node.Title == navLinkTitle)
                {
                    feedback.Checked("Found quick launch nav link: " + navLinkTitle);
                    if (webNeedsUpdating) web.Update();
                    return;
                }
            }

            // So if we're here then we need to create the nav link:
            SPNavigationNode newNode = new SPNavigationNode(navLinkTitle, navLinkURL);
            nodes.AddAsLast(newNode);
            web.Update();
        }

        public static void AddColumnsToView(WBTaskFeedback feedback, SPView view, IEnumerable<WBColumn> columns)
        {
            foreach (WBColumn column in columns) {
                if (!view.ViewFields.Exists(column.DisplayName))
                {
                    view.ViewFields.Add(column.DisplayName);
                    feedback.Created("Added column " + column.DisplayName + " to view " + view.Title);
                }
                else
                {
                    feedback.Checked("Found column " + column.DisplayName + " already in view " + view.Title);
                }
            }
            view.Update();
        }

        public static String JoinUpToLimit(String joinString, IEnumerable<String> strings, int characterLimit)
        {
            StringBuilder actual = new StringBuilder();

            bool first = true;
            int charactersToAdd = 0;
            foreach (String nextString in strings)
            {
                if (!first) charactersToAdd = joinString.Length;
                else charactersToAdd = 0;

                charactersToAdd += nextString.Length;

                if (actual.Length + charactersToAdd > characterLimit) break;

                // We have to set first to false here to make sure first stays true for the first string until now:
                if (first) first = false;
                else actual.Append(joinString);

                actual.Append(nextString);
            }

            return actual.ToString();
        }

        public static int Counter(String counterID)
        {
            HttpContext context = HttpContext.Current;

            int currentCount = 0;
            if (context.Items.Contains(counterID))
            {
                currentCount = (int)context.Items[counterID];
            }
            currentCount++;
            context.Items[counterID] = currentCount;

            return currentCount;
        }

        public static String UrlDataEncode(String value)
        {
            if (String.IsNullOrEmpty(value)) return "";
            return Uri.EscapeDataString(value);
        }

        public static String SerializeToCompressedJSONForURI(Object obj)
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
            //return JsonConvert.SerializeObject(obj, settings);

            return Uri.EscapeDataString(Compress(JsonConvert.SerializeObject(obj)));
        }

        public static T DeserializeFromCompressedJSONInURI<T>(String value)
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
            //return JsonConvert.DeserializeObject<T>(value, settings);

            return JsonConvert.DeserializeObject<T>(Decompress(Uri.UnescapeDataString(value)));
        }


        // With thanks to http://madskristensen.net/post/compress-and-decompress-strings-in-c
        public static string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        // With thanks to http://madskristensen.net/post/compress-and-decompress-strings-in-c
        public static string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static DateTime DateTimeFromISO8601Format(String value)
        {
            return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        // With thanks to: https://hristopavlov.wordpress.com/2008/06/17/preserving-the-last-modified-by-when-checking-in-a-file/
        public static void CheckInFileByUser(
            SPFile file,
            string checkinComment,
            SPCheckinType checkinType,
            SPUser modifiedByUser)
        {
            MethodInfo mi = typeof(SPFile).GetMethod(
                "CheckIn",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(string), typeof(SPCheckinType), typeof(bool), typeof(SPUser) },
                null);

            try
            {
                mi.Invoke(
                    file,
                    new object[] { checkinComment, checkinType, false, modifiedByUser }
                );
            }
            catch (TargetInvocationException invokeEx)
            {
                throw invokeEx.InnerException;
            }
        }

    }
}
