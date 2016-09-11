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
using System.Web;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ControlTemplates.WorkBoxFramework
{
    public partial class TeamSiteNavigation : UserControl
    {
        public String ConfigurationListName;
        public String RecordsGroup;
        public String AdditionalCSSStyle;
        public String NotSetupText = "";

        public const String LINK_TEXT = "LinkText";
        public const String LINK_URL = "LinkURL";
        public const String ON_CLICK_COMMAND = "OnClickCommand";
        public const String UNIQUE_TOGGLE_ID = "UniqueToggleID";
        public const String RECORDS_TYPES = "RecordsTypes";
        public const String SELECTED_CLASS_CSS_STYLE = "SelectedClassCssStyle";
        public const String SELECTED_TYPE_CSS_STYLE = "SelectedTypeCssStyle";
        public const String ADDITIONAL_TYPE_CSS_STYLE = "AdditionalTypeCssStyle";

        public SPList ConfigurationList = null;

        public List<Hashtable> RecordsClasses = null;

        public WBTeam Team;

        protected void Page_Load(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPSite site = SPContext.Current.Site;
                
            string selectedToggleID = Request.Params["selectedToggleID"];    
            string selectedRecordsTypeGUID = Request.Params["recordsTypeGUID"];

            WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            string teamGUIDString = "";
            Team = WBTeam.GetFromTeamSite(SPContext.Current);
            if (Team == null) return;
                
            teamGUIDString = WBExtensions.WBxToString(Team.Id);
            string recordsTypesListUrl = Team.RecordsTypesListUrl;

            if (recordsTypesListUrl == null || recordsTypesListUrl == "")
            {
                // recordsTypesListUrl = web.Url + "/Lists/Configure%20Teams%20Records%20Classes";
                NotSetupText = "(<i>The team has no records types list setup yet.</i>)";
                return;
            }

            using (SPWeb configWeb = site.OpenWeb(recordsTypesListUrl))
            {
                ConfigurationList = configWeb.GetList(recordsTypesListUrl);
                if (ConfigurationList != null)
                {
                    if (!ConfigurationList.Fields.ContainsField("Records Class"))
                    {
                        ConfigurationList = null;
                        NotSetupText = "(<i>The configuration list " + recordsTypesListUrl + " has no 'Records Class' column.</i>)";

                    }
                }
                else
                {
                    NotSetupText = "(<i>The configuration list " + recordsTypesListUrl + " was not set up correctly or does not exist.</i>)";
                }

                if (ConfigurationList != null)
                {
                    RecordsClasses = new List<Hashtable>();

                    int classCount = 0;
                    int classTotal = 0;

                    foreach (SPListItem item in ConfigurationList.Items)
                    {
                        try
                        {

                            string groupName = item.WBxGetColumnAsString("Records Group");
                            if (groupName.Equals(RecordsGroup))
                            {
                                classTotal++;
                            }
                        }
                        catch (Exception exception)
                        {
                            WBUtils.logMessage("The error message when counting the classes was: " + exception.StackTrace);
                        }
                    }

                    WBUtils.logMessage("The class count was found to be: " + classCount);


                    foreach (SPListItem item in ConfigurationList.Items)
                    {
                        try
                        {

                            string groupName = item.WBxGetColumnAsString("Records Group");
                            if (groupName.Equals(RecordsGroup))
                            {
                                classCount++;

                                Hashtable recordsClassDetails = new Hashtable();

                                List<Hashtable> recordsTypes = new List<Hashtable>();

                                WBRecordsType recordsClass = new WBRecordsType(recordsTypesTaxonomy, WBExtensions.WBxGetColumnAsString(item, "Records Class"));


                                string workBoxCollectionURL = recordsClass.WorkBoxCollectionUrl;
                                string viewPagerelativeURL = WBExtensions.WBxGetColumnAsString(item, "ViewPageRelativeURL");
                                string recordsTypeGUID = recordsClass.Id.WBxToString();
                                string uniqueToggleID = string.Format("WBF_Grouping_{0}_Child_{1}", AdditionalCSSStyle, classCount);

                                string viewURL = "#";

                                if (viewPagerelativeURL != "")
                                {
                                    viewURL = string.Format("{0}{1}?selectedToggleID={2}&recordsTypeGUID={3}&workBoxCollectionURL={4}",
                                        SPContext.Current.Web.ServerRelativeUrl,
                                        viewPagerelativeURL,
                                        uniqueToggleID,
                                        recordsTypeGUID,
                                        Uri.EscapeDataString(workBoxCollectionURL));
                                }
                                /*
                                string viewURL = string.Format("{0}{1}?selectedToggleID={2}",
                                    SPContext.Current.Web.ServerRelativeUrl,
                                    viewPagerelativeURL,
                                    uniqueToggleID);
                                */


                                string selectedRecordsClass = "";
                                if (uniqueToggleID.Equals(selectedToggleID)) selectedRecordsClass = " wbf-selected-records-class";

                                string selectedRecordsType = "";
                                if (recordsTypeGUID.Equals(selectedRecordsTypeGUID)) selectedRecordsType = " wbf-selected-records-type";

                                WBUtils.logMessage("Class count and class total: " + classCount + "  " + classTotal);
                                if (classCount == classTotal) selectedRecordsClass += " wbf-last-class";


                                recordsClassDetails[LINK_TEXT] = recordsClass.Name;
                                recordsClassDetails[LINK_URL] = viewURL;
                                recordsClassDetails[ON_CLICK_COMMAND] = "javascript: $('#" + uniqueToggleID +"').toggle(200);";
                                recordsClassDetails[UNIQUE_TOGGLE_ID] = uniqueToggleID;
                                recordsClassDetails[SELECTED_CLASS_CSS_STYLE] = selectedRecordsClass;
                                recordsClassDetails[SELECTED_TYPE_CSS_STYLE] = selectedRecordsType;

                                int typeCount = 0;
                                int typeTotal = recordsClass.Term.Terms.Count;

                                Dictionary<String, Hashtable> allRecordsTypeDetails = new Dictionary<String, Hashtable>();

                                foreach (Term term in recordsClass.Term.Terms)
                                {
                                    typeCount++;
                                    Hashtable recordsTypeDetails = new Hashtable();
                                    WBRecordsType recordsType = new WBRecordsType(recordsTypesTaxonomy, term);
                                    
                                    // If the term has been marked as unavailable then it shouldn't be liseted here.
                                    if (!recordsType.IsAvailableForTagging || !recordsType.AllowWorkBoxRecords) continue;

                                    recordsTypeGUID = recordsType.Id.ToString();

                                    selectedRecordsType = "";
                                    if (recordsTypeGUID.Equals(selectedRecordsTypeGUID)) selectedRecordsType = " wbf-selected-records-type";

                                    viewURL = string.Format("{0}{1}?selectedToggleID={2}&recordsTypeGUID={3}&workBoxCollectionURL={4}",
                                        SPContext.Current.Web.ServerRelativeUrl,
                                        viewPagerelativeURL,
                                        uniqueToggleID,
                                        recordsTypeGUID,
                                        Uri.EscapeDataString(workBoxCollectionURL));

                                    if (typeCount == typeTotal) selectedRecordsType += " wbf-last-type";

                                    recordsTypeDetails[LINK_TEXT] = recordsType.Name;
                                    recordsTypeDetails[LINK_URL] = viewURL;
                                    recordsTypeDetails[ON_CLICK_COMMAND] = "";
                                    recordsTypeDetails[SELECTED_TYPE_CSS_STYLE] = selectedRecordsType;


                                    allRecordsTypeDetails.Add(recordsType.Name, recordsTypeDetails);
                                }

                                List<String> allNames = new List<String>(allRecordsTypeDetails.Keys);
                                allNames.Sort();

                                foreach (String name in allNames)
                                {
                                    recordsTypes.Add(allRecordsTypeDetails[name]);
                                }

                                /* Not showing the create link here any more.
                                string createNewURL = "";
                                string createNewText = "";
                                WBCollection collection = null;

                                if (workBoxCollectionURL != null && workBoxCollectionURL != "")
                                {
                                    collection = new WBCollection(workBoxCollectionURL);
                                    if (collection.CanAnyoneCreate == true)
                                    {
                                        createNewURL = collection.GetUrlForNewDialog(Team);
                                        createNewText = collection.CreateNewWorkBoxText;
                                    }
                                }
                                else
                                {
                                    createNewText = "";
                                    createNewURL = "";
                                    workBoxCollectionURL = "";
                                }

                                if (createNewText != "")
                                {
                                    Hashtable createLink = new Hashtable();

                                    createLink[LINK_TEXT] = createNewText;
                                    createLink[LINK_URL] = "#";
                                    createLink[ON_CLICK_COMMAND] = "javascript: WorkBoxFramework_commandAction('" + createNewURL + "', 600, 500);";
                                    createLink[ADDITIONAL_TYPE_CSS_STYLE] = "wbf-create-new-link";

                                    recordsTypes.Add(createLink);
                                }
                                */
 
                                recordsClassDetails[RECORDS_TYPES] = recordsTypes;

                                RecordsClasses.Add(recordsClassDetails);
                            }
                        }
                        catch (Exception exception)
                        {
                            RecordsClasses.Add(makeErrorRecordsClassEntry());
                            WBUtils.logMessage("The error message was: " + exception.StackTrace);
                        }
                    }
                }
            }

        }

        private Hashtable makeErrorRecordsClassEntry()
        {
            Hashtable recordsClassDetails = new Hashtable();

            recordsClassDetails[LINK_TEXT] = "<i>(Access denied)</i>";
            recordsClassDetails[LINK_URL] = "#";
            recordsClassDetails[ON_CLICK_COMMAND] = "";
            recordsClassDetails[UNIQUE_TOGGLE_ID] = "";
            recordsClassDetails[SELECTED_CLASS_CSS_STYLE] = "";
            recordsClassDetails[SELECTED_TYPE_CSS_STYLE] = "";

            List<Hashtable> recordsTypes = new List<Hashtable>();
            recordsClassDetails[RECORDS_TYPES] = recordsTypes;

            return recordsClassDetails;
        }

        protected void TestAsync_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("So we're in TestAsync_OnClick as part of Nav control code behind");
        }
    }
}
