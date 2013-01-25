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

        public String AddToClipboard(WorkBox workBox, String[] itemIDs)
        {
            return AddToClipboard(workBox, itemIDs, false);
        }


        public String AddToClipboard(WorkBox workBox, String[] itemIDs, bool clearExistingItems)
        {
            String errorString = "";

            UserProfile userProfile = GetUserProfile(workBox.Site);

            Dictionary<String, List<int>> clipboard = null;
            if (clearExistingItems)
            {
                clipboard = new Dictionary<String, List<int>>();
            }
            else
            {
                clipboard = GetClipboard(userProfile);
            }

            List<int> currentIDsForWorkBox = null;
            if (clipboard.ContainsKey(workBox.Url))
            {
                currentIDsForWorkBox = clipboard[workBox.Url];
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
                clipboard[workBox.Url] = currentIDsForWorkBox;

                return SetClipboard(userProfile, clipboard);
            }

            return errorString;
        }

        public Dictionary<String, List<int>> GetClipboard(WorkBox workBox)
        {
            return GetClipboard(workBox.Site);
        }

        public Dictionary<String, List<int>> GetClipboard(SPSite site)
        {
            UserProfile profile = GetUserProfile(site);
            return GetClipboard(profile);
        }

        private Dictionary<String, List<int>> GetClipboard(UserProfile userProfile)
        {
            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            String clipboardString = "";
            if (clipboardPropertyValue != null)
            {
                clipboardString = clipboardPropertyValue.Value.WBxToString().Trim();
            }

            Dictionary<String, List<int>> clipboard = new Dictionary<String, List<int>>();

            if (String.IsNullOrEmpty(clipboardString)) return clipboard;

            string[] valuesForEachWorkBox = clipboardString.Split(';');

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

                clipboard.Add(workBoxURL, listOfIDs);
            }

            return clipboard;
        }


        private String SetClipboard(UserProfile userProfile, Dictionary<String, List<int>> clipboard)
        {
            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            String clipboardString = "";

            List<String> stringsForEachWorkBox = new List<String>();

            foreach (String workBoxURL in clipboard.Keys)
            {
                List<int> ids = clipboard[workBoxURL];

                List<String> idStrings = new List<String>();
                foreach (int id in ids)
                {
                    idStrings.Add(id.ToString());
                }

                String stringForAWorkBox = workBoxURL + "|" + String.Join("|", idStrings.ToArray());
                stringsForEachWorkBox.Add(stringForAWorkBox);
            }

            clipboardString = String.Join(";", stringsForEachWorkBox.ToArray());

            clipboardPropertyValue.Value = clipboardString;
            userProfile.Commit();

            // returning an error string:
            return "";
        }


        public String ClearClipboard(SPSite site)
        {
            UserProfile userProfile = GetUserProfile(site);
            UserProfileValueCollection clipboardPropertyValue = userProfile[WorkBox.USER_PROFILE_PROPERTY__MY_WORK_BOX_CLIPBOARD];

            clipboardPropertyValue.Value = "";  
            userProfile.Commit();

            // returning an error string:
            return "";
        }

        public static String RenderClipboard(Dictionary<String, List<int>> clipboard)
        {
            if (clipboard.Count == 0)
            {
                return "<div class='wbf-clipboard'><p>Clipboard is empty</p></div>";
            }

            String html = "<div class='wbf-clipboard'>";

            foreach (String workBoxURL in clipboard.Keys)
            {
                List<int> ids = clipboard[workBoxURL];

                using (WorkBox clipboardWorkBox = new WorkBox(workBoxURL))
                {
                    html += "<div class='wbf-clipboard-from-work-box'><b>From:</b> <img src=\"/_layouts/images/WorkBoxFramework/work-box-16.png\"/> <b>" + clipboardWorkBox.Title + "</b> ";

                    SPDocumentLibrary documents = clipboardWorkBox.DocumentLibrary;

                    if (ids.Count > 0)
                    {
                        // OK we're first going to add the folder path to the parent folder 
                        // As all items on the clipboard (currently) only come from one folder 
                        // we can list this folder once at the start (and find it's details from
                        // any of the items on the clipboard)
                        SPListItem firstItem = documents.GetItemById(ids[0]);
                        SPFolder fromFolder = null;                        
                        if (firstItem.Folder == null)
                        {
                            fromFolder = firstItem.File.ParentFolder;
                        }
                        else
                        {
                            fromFolder = firstItem.Folder.ParentFolder;
                        }

                        // Now adding the folder's URL alongside the name of the work box.
                        html += " /" + fromFolder.Url;



                        foreach (int id in ids)
                        {
                            SPListItem item = documents.GetItemById(id);

                            html += "<div class='wbf-clipboard-item'>";

                            if (item.Folder == null)
                            {
                                html += "<img src=\"" + WBUtils.DocumentIcon16(item.Name) + "\"/> " + item.Name;
                            }
                            else
                            {
                                html += "<img src=\"/_layouts/images/folder.gif\"/> " + item.Name;
                            }

                            html += "</div>\n";


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
