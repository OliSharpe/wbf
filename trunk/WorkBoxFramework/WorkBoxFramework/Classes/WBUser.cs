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
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework
{
    public class WBUser
    {
        public const String CLIPBOARD_ACTION__COPY = "COPY";
        public const String CLIPBOARD_ACTION__CUT = "CUT";

        public SPUser User { get; private set; }   

        public WBUser(SPUser user)
        {
            User = user;
        }

        public String GetUrlToMyUnprotectedWorkBox(SPSite site)        
        {
            UserProfile profile = GetUserProfile(site);

            UserProfileValueCollection myUnprotectedWorkBoxURL = profile[WorkBox.USER_PROFILE_PROPERTY__MY_UNPROTECTED_WORK_BOX_URL];

            String url = "";
            if (myUnprotectedWorkBoxURL != null)
            {
                url = myUnprotectedWorkBoxURL.Value.WBxToString().Trim();
            }

            return url;
        }


        public UserProfile GetUserProfile(SPSite site)
        {
            SPServiceContext _serviceContext = SPServiceContext.GetContext(site);
            UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
            UserProfile profile = _profileManager.GetUserProfile(true);
                
            return profile;
        }

        public String AddToClipboard(String action, WorkBox workBox, String[] itemIDs)
        {
            return AddToClipboard(action, workBox, itemIDs, false);
        }


        public String AddToClipboard(String action, WorkBox workBox, String[] itemIDs, bool clearExistingItems)
        {
            String errorString = "";

            UserProfile userProfile = GetUserProfile(workBox.Site);

            Dictionary<String, List<int>> clipboardItems = new Dictionary<String, List<int>>();
            String clipboardAction = "";
            if (clearExistingItems)
            {
                // If we are clearing the clipboard then we don't have to match the new action with the existing action:
                clipboardAction = action;
            }
            else
            {
                clipboardAction = GetClipboard(userProfile, clipboardItems);
                if (String.IsNullOrEmpty(clipboardAction))
                {
                    clipboardAction = action;
                }
            }

            if (clipboardAction != action) return "You can't mix CUT and COPY actions!"; //"The action for the current items is: " + clipboardAction + " so you can't add items with the action: " + action + " without first clearing the clipboard";

            List<int> currentIDsForWorkBox = null;
            if (clipboardItems.ContainsKey(workBox.Url))
            {
                currentIDsForWorkBox = clipboardItems[workBox.Url];
            }
            else
            {
                currentIDsForWorkBox = new List<int>();
            }

            foreach (string idString in itemIDs)
            {
                if (!String.IsNullOrEmpty(idString))
                {
                    int id = Int32.Parse(idString);

                    if (!currentIDsForWorkBox.Contains(id))
                    {
                        currentIDsForWorkBox.Add(id);
                    }
                }
            }

            if (String.IsNullOrEmpty(errorString))
            {
                clipboardItems[workBox.Url] = currentIDsForWorkBox;

                return SetClipboard(userProfile, clipboardAction, clipboardItems);
            }

            return errorString;
        }

        public String GetClipboardAction(SPSite site)
        {
            UserProfile profile = GetUserProfile(site);
            return GetClipboardAction(profile);
        }

        public String GetClipboardAction(UserProfile userProfile)
        {
            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            String clipboardString = "";
            if (clipboardPropertyValue != null)
            {
                clipboardString = clipboardPropertyValue.Value.WBxToString().Trim();
            }

            if (String.IsNullOrEmpty(clipboardString)) return "";

            String actionString = ""; 

            string[] actionItemsSplit = clipboardString.Split('#');
            if (actionItemsSplit.Length == 1)
            {
                actionString = CLIPBOARD_ACTION__COPY;
            }
            else if (actionItemsSplit.Length == 2)
            {
                actionString = actionItemsSplit[0];
            }
            else
            {
                throw new NotImplementedException("The clipboard string is badly formed: " + clipboardString);
            }

            return actionString;
        }
               
        /// <summary>
        /// This method returns the action of the clipboard while filling the 'clipboardItems' dictionary and
        /// therefore returns the two key values from the clipboard in one go. The passed in clipboardItems dictionary
        /// must be empty.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="clipboardItems">An empty dictionary object that will be filled with any clipboard items.</param>
        /// <returns>The clipboard action of either 'COPY' or 'CUT'</returns>
        public String GetClipboard(SPSite site, Dictionary<String, List<int>> clipboardItems)
        {
            UserProfile profile = GetUserProfile(site);
            return GetClipboard(profile, clipboardItems);
        }

        /// <summary>
        /// This method returns the action of the clipboard while filling the 'clipboardItems' dictionary and
        /// therefore returns the two key values from the clipboard in one go. The passed in clipboardItems dictionary
        /// must be empty.
        /// </summary>
        /// <param name="userProfile"></param>
        /// <param name="clipboardItems">An empty dictionary object that will be filled with any clipboard items.</param>
        /// <returns>The clipboard action of either 'COPY' or 'CUT'</returns>
        public String GetClipboard(UserProfile userProfile, Dictionary<String, List<int>> clipboardItems)
        {
            if (clipboardItems.Count != 0) throw new NotImplementedException("You should only use this method with an empty clipboardItems dictionary object");

            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            String clipboardString = "";
            if (clipboardPropertyValue != null)
            {
                clipboardString = clipboardPropertyValue.Value.WBxToString().Trim();
            }

            if (String.IsNullOrEmpty(clipboardString)) return "";

            String clipboardItemsString = "";
            String actionString = CLIPBOARD_ACTION__COPY;

            string[] actionItemsSplit = clipboardString.Split('#');
            if (actionItemsSplit.Length == 1)
            {
                clipboardItemsString = actionItemsSplit[0];
            }
            else if (actionItemsSplit.Length == 2)
            {
                actionString = actionItemsSplit[0];
                clipboardItemsString = actionItemsSplit[1];
            }
            else
            {
                throw new NotImplementedException("The clipboard string is badly formed: " + clipboardString);
            }

            string[] valuesForEachWorkBox = clipboardItemsString.Split(';');

            foreach (string valueForAWorkBox in valuesForEachWorkBox)
            {
                if (String.IsNullOrEmpty(valueForAWorkBox) || !valueForAWorkBox.Contains('|'))
                {
                    throw new NotImplementedException("The clipboard string is badly formed: " + clipboardString);
                }

                List<String> listOfIDStrings = new List<String>(valueForAWorkBox.Split('|'));

                String workBoxURL = listOfIDStrings[0];
                listOfIDStrings.RemoveAt(0);

                List<int> listOfIDs = new List<int>();
                foreach (String idString in listOfIDStrings)
                {
                    listOfIDs.Add(Int32.Parse(idString));
                }

                clipboardItems.Add(workBoxURL, listOfIDs);
            }

            return actionString;
        }


        private String SetClipboard(UserProfile userProfile, String clipboardAction, Dictionary<String, List<int>> clipboardItems)
        {
            if (String.IsNullOrEmpty(clipboardAction))
            {
                clipboardAction = CLIPBOARD_ACTION__COPY;
            }

            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            String clipboardString = "";

            List<String> stringsForEachWorkBox = new List<String>();

            foreach (String workBoxURL in clipboardItems.Keys)
            {
                List<int> ids = clipboardItems[workBoxURL];

                List<String> idStrings = new List<String>();
                foreach (int id in ids)
                {
                    idStrings.Add(id.ToString());
                }

                String stringForAWorkBox = workBoxURL + "|" + String.Join("|", idStrings.ToArray());
                stringsForEachWorkBox.Add(stringForAWorkBox);
            }

            clipboardString = clipboardAction + "#" + String.Join(";", stringsForEachWorkBox.ToArray());

            clipboardPropertyValue.Value = clipboardString;
            userProfile.Commit();

            // returning an error string:
            return "";
        }

        public String ClearClipboard(SPSite site)
        {
            UserProfile userProfile = GetUserProfile(site);
            return ClearClipboard(userProfile);
        }

        public String ClearClipboard(UserProfile userProfile)
        {
            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            clipboardPropertyValue.Value = "";  
            userProfile.Commit();

            // returning an error string:
            return "";
        }

        public String PasteClipboard(WorkBox workBox, String folderPath)
        {
            Dictionary<String, List<int>> clipboardItems = new Dictionary<String, List<int>>();
            UserProfile userProfile = GetUserProfile(workBox.Site);

            String clipboardAction = "";

            try
            {
                clipboardAction = GetClipboard(userProfile, clipboardItems);

                SPFolder folder = workBox.DocumentLibrary.RootFolder;
                WBLogging.Generic.Unexpected("Folder path: ##" + folderPath + "##");
                if (folder == null)
                {
                    WBLogging.Generic.Unexpected("folder is null !!!");
                }

                folderPath = folderPath.WBxTrim();
                if (!String.IsNullOrEmpty(folderPath))
                {
                    folder = folder.WBxGetFolderPath(folderPath);
                }

                bool allowUnsafeUpdatesOriginalValue = workBox.Web.AllowUnsafeUpdates;
                workBox.Web.AllowUnsafeUpdates = true;

                foreach (String workBoxURL in clipboardItems.Keys)
                {
                    List<int> ids = clipboardItems[workBoxURL];

                    using (WorkBox clipboardWorkBox = new WorkBox(workBoxURL))
                    {
                        clipboardWorkBox.Web.AllowUnsafeUpdates = true;

                        SPDocumentLibrary documents = clipboardWorkBox.DocumentLibrary;

                        foreach (int id in ids)
                        {
                            SPListItem item = documents.GetItemById(id);

                            bool cutOriginal = (clipboardAction == WBUser.CLIPBOARD_ACTION__CUT);

                            try
                            {
                                WBUtils.CutOrCopyIntoFolder(workBox.Web, folder, item, cutOriginal);
                            } 
                            catch (Exception docLevelException) 
                            {
                                WBUtils.SendErrorReport(workBox.Web, "Error pasting a particular document in PasteClipboard", "Exception : " + docLevelException + " \n\n " + docLevelException.StackTrace);
                            }
                        }

                        clipboardWorkBox.Web.AllowUnsafeUpdates = false;
                    }
                }

                if (clipboardAction == CLIPBOARD_ACTION__CUT)
                {
                    // You cannot paste more than once items that have been cut:
                    ClearClipboard(userProfile);
                }

                workBox.Web.AllowUnsafeUpdates = allowUnsafeUpdatesOriginalValue;

            }
            catch (Exception exception)
            {
                WBUtils.SendErrorReport(workBox.Web, "Error in PasteClipboard", "Exception : " + exception + " \n\n " + exception.StackTrace);
                WBLogging.Generic.Unexpected("Clearing the user's clipboard in the hope that that will fix the error they are having.");
                ClearClipboard(userProfile);
            }


            return clipboardAction;
        }

        public String RenderClipboardAction(SPSite site)
        {
            UserProfile profile = GetUserProfile(site);
            String clipboardAction = GetClipboardAction(profile);

            String html = "";

            if (clipboardAction == CLIPBOARD_ACTION__CUT)
            {
                html += "<div class='wbf-clipboard-action'><p><img src='/_layouts/images/cuths.png' alt='Cut items' /> &nbsp; If you paste these items they will be <b>CUT</b> from their original location:</p></div>\n\n";
            }

            if (clipboardAction == CLIPBOARD_ACTION__COPY)
            {
                html += "<div class='wbf-clipboard-action'><p><img src='/_layouts/images/copy16.gif' alt='Copy items' /> &nbsp; If you paste these items they will be <b>COPIED</b> from their original location:</p></div>\n\n";
            }

            return html;
        }

        public String RenderClipboardItems(SPSite site)
        {
            UserProfile profile = GetUserProfile(site);
            Dictionary<String, List<int>> clipboardItems = new Dictionary<String, List<int>>();
            String clipboardAction = GetClipboard(profile, clipboardItems);

            if (clipboardItems.Count == 0)
            {
                return "<div class='wbf-clipboard-items'><p>Clipboard is empty</p></div>";
            }

            String html = "<div class='wbf-clipboard-items'>";

            String actionImageSrc = "";

            if (clipboardAction == CLIPBOARD_ACTION__CUT)
            {
                actionImageSrc = "/_layouts/images/cuths.png";
            }
            else
            {
                actionImageSrc = "/_layouts/images/copy16.gif";
            }

            foreach (String workBoxURL in clipboardItems.Keys)
            {
                List<int> ids = clipboardItems[workBoxURL];

                using (WorkBox clipboardWorkBox = new WorkBox(workBoxURL))
                {
                    html += "<div class='wbf-clipboard-from-work-box'><b>From:</b> <img src=\"/_layouts/images/WorkBoxFramework/work-box-16.png\"/> <b>" + clipboardWorkBox.Title + "</b> ";

                    SPDocumentLibrary documents = clipboardWorkBox.DocumentLibrary;

                    if (ids.Count > 0)
                    {
                        Dictionary<String, String> htmlFragmentsToOrder = new Dictionary<string, string>();

                        foreach (int id in ids)
                        {
                            String htmlFragment = "<div class='wbf-clipboard-item'><i>(could not find an item)</i></div>";

                            try
                            {
                                SPListItem item = documents.GetItemById(id);

                                SPFolder fromFolder = null;
                                String itemImageSrc = "";
                                if (item.Folder == null)
                                {
                                    fromFolder = item.File.ParentFolder;
                                    itemImageSrc = WBUtils.DocumentIcon16(item.Name);
                                }
                                else
                                {
                                    fromFolder = item.Folder.ParentFolder;
                                    itemImageSrc = "/_layouts/images/folder.gif";
                                }

                                htmlFragment = "<div class='wbf-clipboard-item'>";
                                htmlFragment += "<img src=\"" + actionImageSrc + "\"/>  &nbsp; /" + fromFolder.Url + " &nbsp; <img src=\"" + itemImageSrc + "\"/> <b>" + item.Name + "</b>";
                                htmlFragment += "</div>\n";

                                htmlFragmentsToOrder.Add(fromFolder.Url + "/" + item.Name, htmlFragment);
                            }
                            catch (Exception itemException)
                            {
                                // Trying to add this to the end of the list of items found:
                                htmlFragmentsToOrder.Add("zzzzzzz", htmlFragment);

                                WBUtils.SendErrorReport(clipboardWorkBox.Web, "Error in RenderClipboardItems", "Exception : " + itemException + " \n\n " + itemException.StackTrace);
                            }

                        }

                        List<String> ordering = new List<String>(htmlFragmentsToOrder.Keys);
                        ordering.Sort();

                        foreach (String key in ordering)
                        {
                            html += htmlFragmentsToOrder[key];
                        }
                    }
                    else
                    {
                        html += "<p>Nothing from this work box</p>";
                    }

                    html += "</div>\n";
                }
            }

            html += "</div>\n";

            return html;
        }

    }
}
