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
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Publishing;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class UploadAndCreate : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void uploadAndCreateButton_OnClick(object sender, EventArgs e)
        {

            bool digestOK = Web.ValidateFormDigest();
            WBLogging.Migration.Verbose("The FormDigest validation value when uploading and creating was: " + digestOK);

            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    string progress = uploadAndCreate(WorkBoxCollectionURL.Text, ControlFile.Text);

                    ProgressReport.Text = progress;
                });
            }

//            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        private List<string[]> parseCSV(string path)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                WBLogging.Migration.Unexpected(e.Message);
            }

            return parsedData;
        }

        private String uploadAndCreate(String workBoxCollectionURL, String filePath)
        {
            string progress = "<table>\n";

            List<string[]> uploadData = parseCSV(filePath);

            int total = uploadData.Count;
            int count = 0;

            using (WBCollection collection = new WBCollection(workBoxCollectionURL))
            {
                string localID, title, description, rights, dateString, fileToUpload, folderPath;

                foreach (string[] dataRow in uploadData)
                {
                    count++;

                    // Skip the first row as this contains the titles for the columsn, not data.
                    if (count == 1) continue;

                    fileToUpload = dataRow[0];
                    localID = dataRow[1];
                    folderPath = dataRow[2];
                    title = dataRow[3];
                    description = dataRow[4];
                    rights = dataRow[5];
                    dateString = dataRow[6].Replace(" 00:00", "");

                    WBLogging.Migration.Verbose(string.Format("Uploading {0} of {1} : with filename: {2}", count, total, fileToUpload));
                    string success = uploadDocument(collection, fileToUpload, localID, folderPath, title, description, dateString);
                    WBLogging.Migration.Verbose("Success status: " + success);

                    progress += "<tr><td>" + success + "</td><td>" + dataRow[0] + "</td></tr>\n";
                }
            }

            progress += "</table>\n";

            return progress;
        }
          
        private String uploadDocument(WBCollection collection, String fileToUpload, String localID, String folderPath, String title,  String description, String scanDateString)
        {
            // First check if the work box has been created:
            using (WorkBox workBox = collection.FindOrCreateNewByLocalID(localID))
            {
                if (!workBox.IsOpen) workBox.Open();
            }

            // Then re-load the work box to ensure we dont get security problems:
            using (WorkBox workBox = collection.FindOrCreateNewByLocalID(localID))
            {
                if (!System.IO.File.Exists(fileToUpload))
                    return "File not found.";

                try
                {

                    SPFolder folder = workBox.DocumentLibrary.RootFolder.WBxGetOrCreateFolderPath(folderPath);

                    // Prepare to upload 
                    Boolean replaceExistingFiles = true;
                    String fileName = Path.GetFileName(fileToUpload);
                    String extension = Path.GetExtension(fileName);

                    String newFileName = title + extension;

                    FileStream fileStream = File.OpenRead(fileToUpload);

                    fileName = fileName.Replace('~', '-');
                    WBLogging.Migration.Verbose("fileName = " + fileName);
                    WBLogging.Migration.Verbose("newFileName = " + newFileName);
                    WBLogging.Migration.Verbose("folder.Name = " + folder.Name);
                    WBLogging.Migration.Verbose("folder.Url = " + folder.Url);
                    // Upload document 

                    string uniqueName = workBox.Web.WBxMakeFilenameUnique(folder, newFileName);

                    SPFile spfile = folder.Files.Add(uniqueName, fileStream, replaceExistingFiles);
//                    folder.Update();


                    try
                    {
                        DateTime scanDate = Convert.ToDateTime(scanDateString);
                        spfile.Item[WorkBox.COLUMN_NAME__SCAN_DATE] = scanDate;

                        spfile.Update();

                    }
                    catch (Exception e)
                    {
                        WBLogging.Migration.Unexpected("Couldn't convert the date: " + scanDateString + " error was: " + e.Message);
                    }


                }
                catch (Exception e)
                {
                    WBLogging.Migration.Unexpected("An exception: " + e.Message);
                    return "An exception: " + e.Message;
                }

                return "Success.";
            }
        }

        public const string CURRENT_LOCATION = "Current Location";
        public const string NEW_LOGICAL_LOCATION = "New Logical Location";
        public const string REMOTE_PAGE_URL = "Remote Page URL";
        public const string LOCAL_PAGE_URL = "Local Page URL";
        public const string SITE_OR_PAGE = "Site Or Page";
        public const string PAGE_TEMPLATE = "Page Template";
        public const string RESULT_MESSAGE = "Result Message";
        public const string MIGRATION_ACTION = "Migration Action";
        public const string LAST_MIGRATION = "Last Migration";
        public const string ORIGINAL_MAPPING = "Original Mapping";

        public const string COMMENTS = "Comments";

        public const string MIGRATION_ACTION__NOTHING = "";
        public const string MIGRATION_ACTION__ADD_CHILDREN = "Add Children";
        public const string MIGRATION_ACTION__MIGRATE = "Migrate";
        public const string MIGRATION_ACTION__MIGRATE_ONCE = "Migrate Once";
        public const string MIGRATION_ACTION__DELETE = "Delete";
        public const string MIGRATION_ACTION__FIX_PERMISSIONS = "Fix Permissions";

        public const string SITE = "Site";
        public const string PAGE = "Page";
        public const string NO_APPLICABLE = "n/a";

        public WBColumn CurrentLocationColumn = WBColumn.TextColumn(CURRENT_LOCATION);
        public WBColumn NewLogicalLocationColumn = WBColumn.TextColumn(NEW_LOGICAL_LOCATION);
        public WBColumn RemotePageURLColumn = WBColumn.TextColumn(REMOTE_PAGE_URL);
        public WBColumn LocalPageURLColumn = WBColumn.TextColumn(LOCAL_PAGE_URL);
        public WBColumn ResultMessageColumn = WBColumn.TextColumn(RESULT_MESSAGE);
        public WBColumn MigrationActionColumn = WBColumn.TextColumn(MIGRATION_ACTION);
        public WBColumn LastMigrationColumn = WBColumn.DateTimeColumn(LAST_MIGRATION);

        public WBColumn TitleColumn = WBColumn.TextColumn("Title");

        protected void MigratePages_OnClick(object sender, EventArgs e)
        {
            string listURL = WebPageMigrationList.Text.WBxTrim();

            string username = UserName.Text.WBxTrim();
            string password = UserPassword.Text.WBxTrim();

            if (listURL == "") return;

            using (SPSite site = new SPSite(listURL))
            using (SPWeb web = site.OpenWeb())
            {
                SPList list = web.GetList(listURL);

                foreach (SPListItem item in list.Items)
                {
                    string migrationAction = item.WBxGetColumnAsString(MIGRATION_ACTION);

                    switch (migrationAction) 
                    {
                        case MIGRATION_ACTION__ADD_CHILDREN:
                        {
                            AddChildrenForPage(site, web, list, item, username, password);
                            item.WBxSetColumnAsString(MIGRATION_ACTION, MIGRATION_ACTION__NOTHING);
                            item.Update();
                            break;
                        }

                        case MIGRATION_ACTION__MIGRATE:
                        {
                            MigrateOnePage(site, web, list, item, username, password);
                            break;
                        }

                        case MIGRATION_ACTION__MIGRATE_ONCE:
                        {
                            MigrateOnePage(site, web, list, item, username, password);
                            item.WBxSetColumnAsString(MIGRATION_ACTION, MIGRATION_ACTION__NOTHING);
                            item.Update();
                            break;
                        }

                        case MIGRATION_ACTION__DELETE:
                        {
                            RecursivelyDeleteIfExists(site, web, list, item, username, password);
                            break;
                        }

                        case MIGRATION_ACTION__FIX_PERMISSIONS:
                        {
                            RecursivelyFixPermissions(site, web, list, item, username, password);
                            break;
                        }

                        default:
                        {
                            // Do nothing!
                            break;
                        }
                    }
                }

            }

        }

        private String MakeLocalPageURL(String newLogicalLocation, String siteOrPage)
        {
            if (siteOrPage == SITE)
            {
                return  "http://sp.izzi" + newLogicalLocation;
            }
            else
            {
                string[] parts = newLogicalLocation.Split('/');

                List<String> trimmedParts = new List<String>();
                foreach (String part in parts)
                {
                    if (part.WBxTrim() != "")
                    {
                        trimmedParts.Add(part);
                    }
                }

                int count = trimmedParts.Count;
                if (count < 2) 
                {
                    return "No sensible page URL can be made for: " + newLogicalLocation;
                }

                string pageName = trimmedParts[count - 1];

                trimmedParts.RemoveAt(count - 1);

                return "http://sp.izzi/" + String.Join("/", trimmedParts.ToArray()) + "/Pages/" + pageName + ".aspx";
            }
        }

        private void RecursivelyFixPermissionsSPWeb(SPWeb web)
        {
            web.ResetRoleInheritance();
            web.Update();

            foreach (SPWeb childWeb in web.Webs)
            {
                RecursivelyFixPermissionsSPWeb(childWeb);
            }
        }


        private void RecursivelyFixPermissions(SPSite site, SPWeb web, SPList list, SPListItem item, string username, string password)
        {
            string localPageURL = item.WBxGetColumnAsString(LOCAL_PAGE_URL);
            string localWebRelativeURL = WBUtils.GetURLWithoutHostHeader(localPageURL);
//            bool isOriginalMapping = item.WBxGetColumnAsBool(ORIGINAL_MAPPING);

            string isSiteOrPage = item.WBxGetColumnAsString(SITE_OR_PAGE);

            if (isSiteOrPage == SITE)
            {
                using (SPWeb pageWeb = site.OpenWeb(localWebRelativeURL))
                {
                    RecursivelyFixPermissionsSPWeb(pageWeb);
                }
            }
            else
            {
                WBLogging.Debug("Not doing anything to fix permissions as this is just a page: " + localPageURL);
            }

        }


        private void RecursivelyDeleteIfExists(SPSite site, SPWeb web, SPList list, SPListItem item, string username, string password)
        {
            string localWebURL = item.WBxGetColumnAsString(LOCAL_PAGE_URL);
            string localWebRelativeURL = WBUtils.GetURLWithoutHostHeader(localWebURL);

            bool isOriginalMapping = item.WBxGetColumnAsBool(ORIGINAL_MAPPING);

            try
            {
                SPWeb localWeb = site.OpenWeb(localWebRelativeURL);

                if (localWeb.Exists)
                {
                    // NB that this method also disposes of the SPWeb object passed in:
                    WBUtils.RecursivelyDeleteSPWeb(localWeb);
                }

                if (!isOriginalMapping)
                {
                    item.Delete();
                }
            }
            catch (Exception error)
            {
                WBLogging.Migration.Unexpected("Error when trying to perform 'Delete' action: " + error.Message);
            }
        }



        private void AddChildrenForPage(SPSite site, SPWeb web, SPList list, SPListItem item, String username, String password)
        {
            string currentLocation = item.WBxGetColumnAsString(CURRENT_LOCATION);
            string newLogicalLocation = item.WBxGetColumnAsString(NEW_LOGICAL_LOCATION);
            string remotePageURL = item.WBxGetColumnAsString(REMOTE_PAGE_URL);

            if (currentLocation == "")
            {
                item.WBxSetColumnAsString(SITE_OR_PAGE, SITE);
                item.Update();

                return;
            }

            if (newLogicalLocation == "")
            {
                WBLogging.Migration.Verbose("The new location for this item was blank! :" + currentLocation);
                return;
            }

            if (remotePageURL == "")
            {
                remotePageURL = "http://izzi/alfresco/web/izzi" + currentLocation;
                item.WBxSetColumnAsString(REMOTE_PAGE_URL, remotePageURL);
            }

            string remotePageURLToUse = remotePageURL.Replace("/alfresco/web/izzi/", "/alfresco/service/mizzi/");

            // Let's make sure that our parent URLs end with a forward slash:
            currentLocation = WBUtils.EnsureTrailingForwardSlash(currentLocation);
            newLogicalLocation = WBUtils.EnsureTrailingForwardSlash(newLogicalLocation);
            remotePageURL = WBUtils.EnsureTrailingForwardSlash(remotePageURL);

            string childrenListString = WBUtils.GetURLContents(remotePageURLToUse + "?JUST_CHILD_PAGES=true", username, password);

            string parentIsSiteOrPage = PAGE;

            if (childrenListString.WBxTrim() != "")
            {
                parentIsSiteOrPage = SITE;

                string comments = item.WBxGetColumnAsString(COMMENTS);

                String[] children = childrenListString.Split(';');

                foreach (String child in children)
                {
                    if (child.WBxTrim() == "") continue;

                    string cleanChild = child.Trim().Replace("&", "and").Replace(" ", "-").Replace(",", "-");

                    string childCurrentLocation = currentLocation + child + "/";
                    string childNewLogicalLocation = newLogicalLocation + cleanChild + "/";
                    string childRemotePageURL = remotePageURL + child + "/";

                    // OK so we need to check if another line item is already mapping to this new logical location:
                    SPListItem currentLocationExists = WBUtils.FindItemByColumn(site, list, CurrentLocationColumn, childCurrentLocation);
                    SPListItem newLocationExists = WBUtils.FindItemByColumn(site, list, NewLogicalLocationColumn, childNewLogicalLocation);

                    if (newLocationExists == null && currentLocationExists == null)
                    {
                        // OK so this logical location isn't yet mapped to so we can create the child item:
                        WBLogging.Migration.Verbose("No duplicates found for current | new : " + childCurrentLocation + " | " + childNewLogicalLocation);

                        SPListItem childItem = list.AddItem();

                        childItem.WBxSetColumnAsString(CURRENT_LOCATION, childCurrentLocation);
                        childItem.WBxSetColumnAsString(NEW_LOGICAL_LOCATION, childNewLogicalLocation);
                        childItem.WBxSetColumnAsString(REMOTE_PAGE_URL, childRemotePageURL);
                        childItem.WBxSetColumnAsString(MIGRATION_ACTION, MIGRATION_ACTION__NOTHING);

                        childItem.Update();

                        AddChildrenForPage(site, web, list, childItem, username, password);
                    }
                    else
                    {
                        string duplicateComment = " Found duplication of";
                        if (currentLocationExists != null)
                        {
                            duplicateComment += " current location (" + currentLocationExists.ID + " : " + childCurrentLocation + ")";

                            if (newLocationExists != null)
                            {
                                duplicateComment += " and";
                            }
                        }

                        if (newLocationExists != null)
                        {
                            duplicateComment += " new logical location (" + newLocationExists.ID + " : " + childNewLogicalLocation + ")";
                        }

                        WBLogging.Migration.Verbose("Duplicates found comment: " + duplicateComment);
                        comments += duplicateComment;
                    }
                }

                item.WBxSetColumnAsString(COMMENTS, comments);
            }

            item.WBxSetColumnAsString(SITE_OR_PAGE, parentIsSiteOrPage);
            item.Update();
        }



        private void MigrationError(SPListItem item, String errorMessage)
        {
            WBLogging.Migration.Unexpected("ERROR: " + errorMessage);
            item.WBxSetColumnAsString(RESULT_MESSAGE, "ERROR: " + errorMessage);
            item.Update();
        }

        private void MigrateOnePage(SPSite site, SPWeb web, SPList list, SPListItem item, String username, String password)
        {
            string remotePageURL = item.WBxGetColumnAsString(REMOTE_PAGE_URL);
            string newLogicalLocation = item.WBxGetColumnAsString(NEW_LOGICAL_LOCATION);
            string siteOrPage = item.WBxGetColumnAsString(SITE_OR_PAGE);
            string localPageURL = item.WBxGetColumnAsString(LOCAL_PAGE_URL);
            string resultMessage = "";

            if (newLogicalLocation == "") 
            {
                MigrationError(item, "There was no new logical location set!");
                return;
            }

            WBLogging.Migration.HighLevel("Starting MigrateOnePage() for newLogicalLocation : " + newLogicalLocation);

            if (siteOrPage == "") 
            {
                MigrationError(item, "The 'Site or Page' value wasn't set!");
                return;
            }

            if (localPageURL == "")
            {
                localPageURL = MakeLocalPageURL(newLogicalLocation, siteOrPage);
                item.WBxSetColumnAsString(LOCAL_PAGE_URL, localPageURL);
            }
            
            string remotePageURLToUse = remotePageURL.Replace("/alfresco/web/izzi/", "/alfresco/service/mizzi/");

            string localSPWebRelativeURL = newLogicalLocation;
            if (siteOrPage == PAGE)
            {
                localSPWebRelativeURL = WBUtils.GetParentPath(newLogicalLocation, false);
            }

            if (remotePageURL != "")
            {
                WBLogging.Migration.Verbose("Migrating remote -> local: " + remotePageURL + " -> " + localPageURL);
            }
            else
            {
                WBLogging.Migration.Verbose("Creating new local unmapped site: " + localPageURL);
            }

            SPWeb localWeb = null;

            try
            {
                string pageTitle = item.WBxGetColumnAsString("Title");
                string pageTemplate = "";

                if (remotePageURL != "")
                {
                    pageTitle = WBUtils.GetURLContents(remotePageURLToUse + "?JUST_PAGE_TITLE=true", username, password);
                    item["Title"] = pageTitle;

                    pageTemplate = WBUtils.GetURLContents(remotePageURLToUse + "?JUST_PAGE_TEMPLATE=true", username, password);
                    item[PAGE_TEMPLATE] = pageTemplate;
                }


                if (remotePageURL != "" && pageTemplate != "izziThreeColumn.ftl")
                {
                    resultMessage = "Not migrated yet (due to unhandled template)";
                }
                else
                {
                    bool newSiteCreated = false;

                    localWeb = site.OpenWeb(localSPWebRelativeURL);

                    if (!localWeb.Exists)
                    {
                        // OK let's try to get the parent web:
                        string parentURL = WBUtils.GetParentPath(localSPWebRelativeURL, false);
                        string childName = WBUtils.GetLastNameInPath(localSPWebRelativeURL);

                        WBLogging.Migration.Verbose("Trying to find parent URL: " + parentURL);

                        using (SPWeb parentWeb = site.OpenWeb(parentURL))
                        {
                            if (parentWeb.Exists)
                            {
                                if (pageTitle == "")
                                {
                                    pageTitle = childName.WBxToUpperFirstLetter();
                                    item["Title"] = pageTitle;
                                }
                                localWeb = parentWeb.Webs.Add(childName, pageTitle, pageTitle, 1033, "CMSPUBLISHING#0", true, false);
                                newSiteCreated = true;
                            }
                            else
                            {
                                WBLogging.Migration.Verbose("Couldn't find parente web site - Don't know how to handle this situation.");
                                resultMessage = "Couldn't find the parent web site";
                            }
                        }
                    }

                    if (localWeb.Exists)
                    {
                        if (localWeb.HasUniqueRoleAssignments)
                        {
                            localWeb.ResetRoleInheritance();
                            localWeb.Update();
                        }
                       
                        PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(localWeb);

                        PageLayout layout = WBUtils.GetPageLayout(publishingWeb, "LBI Standard page layout");

                        if (layout == null)
                        {
                            MigrationError(item, "Didn't find the page layout!!");
                            return;
                        }

                        SPFile pageFile = null;
                        PublishingPage page = null;

                        // If this location is for a site then we get the default page:
                        if (siteOrPage == SITE)
                        {
                            pageFile = publishingWeb.DefaultPage;
                            page = PublishingPage.GetPublishingPage(pageFile.Item);
                        }
                        else
                        {
                            // Otherwise we have to get or create a named page:
                            string pageName = WBUtils.GetLastNameInPath(newLogicalLocation) + ".aspx";

                            SPListItem pageItem = WBUtils.FindItemByColumn(site, publishingWeb.PagesList, WBColumn.Name, pageName);
                            if (pageItem != null)
                            {
                                page = PublishingPage.GetPublishingPage(pageItem);
                            }
                            else
                            {
                                // We couldn't find the page so we'll add it as a new page:
                                page = publishingWeb.AddPublishingPage(pageName, layout);
                            }

                            pageFile = page.ListItem.File;

                            string urlFromItem = "http://sp.izzi" + page.ListItem.File.ServerRelativeUrl;
                            if (localPageURL != urlFromItem)
                            {
                                MigrationError(item, "The generated names don't match: localPageURL | urlFromItem : " + localPageURL + " | " + urlFromItem);
                                return;
                            }
                        }

                        // So we'll update the content if we're migrating an izzi page or it's the first time
                        // creation of a new local page:
                        if (remotePageURL != "" || newSiteCreated)
                        {
                            string pageText = "This page is not being migrated so needs to be edited locally.";
                            if (remotePageURL != "")
                            {
                                pageText = WBUtils.GetURLContents(remotePageURLToUse + "?JUST_PAGE_TEXT=true", username, password);
                                pageText = ProcessPageText(site, list, pageText);
                            }

                            if (pageFile.CheckOutType == SPFile.SPCheckOutType.None)
                            {
                                WBLogging.Migration.Verbose("Checking out the pageFile");
                                pageFile.CheckOut();
                            }
                            else
                            {
                                WBLogging.Migration.Verbose("No need to check out the pageFile");
                            }

                            if (newSiteCreated)
                            {
                                page.Layout = layout;
                                page.Update();
                            }

                            pageFile.Item["Page Content"] = pageText;
                            pageFile.Item["Title"] = pageTitle;


                            pageFile.Item.Update();
                            pageFile.Update();


                            pageFile.CheckIn("Checked in programmatically");
                            pageFile.Publish("Published programmatically");

                            WBLogging.Migration.Verbose("Publisehd migrated page: " + localPageURL);
                        }

                    }
                    else
                    {
                        WBLogging.Migration.Unexpected("Wasn't able to find or create the local web: " + localSPWebRelativeURL);
                        resultMessage += " Wasn't able to find or create the local web: " + localSPWebRelativeURL;
                    }
                }
            }
            catch (Exception error)
            {
                WBLogging.Migration.Unexpected("There was an error: " + error.Message + " Tried with remote | local : " + remotePageURL + " | " + localPageURL);
                resultMessage = "There was an error: " + error.Message + " Tried with remote | local : " + remotePageURL + " | " + localPageURL;
            }
            finally
            {
                if (localWeb != null) localWeb.Dispose();
            }

            if (resultMessage == "")
            {
                resultMessage = "Processed OK";
                item[LAST_MIGRATION] = DateTime.Now;
            }


            WBLogging.Migration.Verbose("Result message : " + resultMessage);

            item.WBxSetColumnAsString(RESULT_MESSAGE, resultMessage);
            item.Update();

            WBLogging.Migration.HighLevel("Finished MigrateOnePage() for newLogicalLocation : " + newLogicalLocation);
        }

        private string ProcessPageText(SPSite site, SPList list, String pageText)
        {
            pageText = pageText.Replace("<h5>", "<h2>");
            pageText = pageText.Replace("</h5>", "</h2>");


            List<String> referencedURLs = WBUtils.GetReferencedURLs(pageText);

            foreach (String referencedURL in referencedURLs)
            {
                string withTrailing = WBUtils.EnsureHasHostHeader("http://izzi/", referencedURL);
                withTrailing = WBUtils.EnsureTrailingForwardSlash(withTrailing);

                SPListItem mappedPage = WBUtils.FindItemByColumn(site, list, RemotePageURLColumn, withTrailing);

                if (mappedPage == null)
                {
                    string withoutTrailing = WBUtils.EnsureNoTrailingForwardSlash(withTrailing);
                    mappedPage = WBUtils.FindItemByColumn(site, list, RemotePageURLColumn, withoutTrailing);
                }

                if (mappedPage != null)
                {
                    string newURLToUse = mappedPage.WBxGetColumnAsString(LOCAL_PAGE_URL);

                    if (newURLToUse == "")
                    {
                        string mappedPageNewLogicalLocation = mappedPage.WBxGetColumnAsString(NEW_LOGICAL_LOCATION);
                        string mappedPageSiteOrPage = mappedPage.WBxGetColumnAsString(SITE_OR_PAGE);

                        newURLToUse = MakeLocalPageURL(mappedPageNewLogicalLocation, mappedPageSiteOrPage);
                    }

                    newURLToUse = WBUtils.GetURLWithoutHostHeader(newURLToUse);

                    WBLogging.Migration.Verbose("Replacing URL -> URL: " + referencedURL + " -> " + newURLToUse);

                    pageText = pageText.Replace(referencedURL, newURLToUse);
                }
                else
                {
                    WBLogging.Migration.Verbose("NOT Replacing URL: " + referencedURL);
                }
            }

            return pageText;
        }
    }
}
