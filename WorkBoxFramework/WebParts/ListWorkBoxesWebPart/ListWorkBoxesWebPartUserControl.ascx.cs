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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace WorkBoxFramework.ListWorkBoxesWebPart
{
    public partial class TeamsWorkBoxesWebPartUserControl : UserControl
    {
        ListWorkBoxesWebPart webPart = default(ListWorkBoxesWebPart);

        public string teamsTermGuid = "";
        public string encodedWorkBoxCollectionUrl = "";
        public string workBoxCollectionUrl = "";
        public bool groupByWorkBoxTemplate = true;
        public bool showEmptyWorkBoxesTypes = true;
        public bool showClosedWorkBoxes = false;
        public bool showDeletedWorkBoxes = false;
        public bool showCreateNewLink = true;
        public string createNewLinkText;

        protected override void OnInit(EventArgs e)
        {
            webPart = this.Parent as ListWorkBoxesWebPart;

            if (webPart.WorkBoxCollectionURL != null && !webPart.WorkBoxCollectionURL.Equals(""))
            {
                workBoxCollectionUrl = webPart.WorkBoxCollectionURL;
                encodedWorkBoxCollectionUrl = Uri.EscapeDataString(workBoxCollectionUrl);
                groupByWorkBoxTemplate = webPart.GroupByWorkBoxTemplate;
                showEmptyWorkBoxesTypes = webPart.ShowEmptyWorkBoxTemplates;
                showClosedWorkBoxes = webPart.ShowClosedWorkBoxes;
                showDeletedWorkBoxes = webPart.ShowDeletedWorkBoxes;
                showCreateNewLink = webPart.ShowCreateNewLink;
                createNewLinkText = webPart.CreateNewLinkText;
            }

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (webPart.WorkBoxCollectionURL == null || webPart.WorkBoxCollectionURL.Equals(""))
            {
                WebPartContent.Text = "<i>(You still have to configure the web part)</i>";
            }
            else
            {

                using (WBCollection collection = new WBCollection(workBoxCollectionUrl))
                {
//                    using (SPWeb workBoxCollectionWeb = workBoxCollectionSite.OpenWeb())
  //                  {

                        // Let's first populate the web parts title to initially reflect the pointed to work box collection title.
                        webPart.TitleUrl = workBoxCollectionUrl;
                        if (webPart.Title.Equals("") || webPart.Title.Equals("List Work Boxes Web Part"))
                        {
                            webPart.Title = collection.Web.Title;
                        }

                                                
                    SPList allWorkBoxesList = collection.List;

                            if (allWorkBoxesList != null)
                            {

                                WBUtils.logMessage("Did pick up the list which has title: " + allWorkBoxesList.Title);

                                SPWeb teamSiteWeb = SPContext.Current.Web;

                                populatePlaceHolder(teamSiteWeb, collection, allWorkBoxesList);

                            }
                            else
                            {
                                WBUtils.logMessage("Could not find the all work boxes list.");
                            }
                 //   }
                }
            }
        }

        private void populatePlaceHolder(SPWeb teamSiteWeb, WBCollection collection, SPList workBoxesList)
        {
            string html = ""; 

            if (teamSiteWeb != null)
            {
                Object valueObj = teamSiteWeb.AllProperties[WorkBox.TEAM_SITE_PROPERTY__TERM_GUID];

                if (valueObj != null)
                {
                    teamsTermGuid = valueObj.ToString();
                   // html += "<b>The team Guid = " + teamsGuid + "</b>";
                }
                else
                {
                    WBUtils.logMessage("The site to term mapping appears to be missing so web part wont filter by team.");
//                    html += "<b>The site to term mapping appears to be missing.</b>";
                }

            }
            else
            {
                WBUtils.logMessage("Wasn't able to find the teamSiteWeb.");
                html += "<b>Wasn't able to find team</b>";
            }

            // THIS SHOULD BE DONE IN CSS!!!!
            html += "<table cellpadding='5'>\n";

            if (groupByWorkBoxTemplate)
            {
                SPList workBoxTemplates = null;
                workBoxTemplates = collection.TemplatesList;
                if (workBoxTemplates == null) WBUtils.logMessage("Couldn't find the Work Box Templates list");

                int maxID = 0;
                foreach (SPListItem template in workBoxTemplates.Items) 
                {
                    if (template.ID > maxID) maxID = template.ID;
                }

                WBUtils.logMessage("Found work box template max ID to be = " + maxID);

                List<List<SPListItem>> groupedByTemplates = new List<List<SPListItem>>(maxID+1);
                for (int i = 0; i <= maxID; i++)
                {
                    groupedByTemplates.Add(new List<SPListItem>());
                }

                int id = -1;
                List<SPListItem> subList = null;
                string lookupValueString = "";

                foreach (SPListItem workBoxMetadataItem in workBoxesList.Items)
                {
                    if (!includeThisWorkBox(workBoxMetadataItem)) continue;

                    lookupValueString = workBoxMetadataItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_TEMPLATE);

                    if (lookupValueString != "") 
                    {
                        SPFieldLookupValue linkedType = new SPFieldLookupValue(lookupValueString);
                        id = linkedType.LookupId;
                        WBUtils.logMessage("Working with ID = " + id);

                        subList = groupedByTemplates[id];

                        if (subList == null)
                        {
                            WBUtils.logMessage("Should never get here = " + id);
                            subList = new List<SPListItem>();
                            groupedByTemplates.Insert(id, subList);
                        }

                        subList.Add(workBoxMetadataItem);
                    }
                    else
                    {
                        WBUtils.logMessage("Couldn't find the type for work box: " + workBoxMetadataItem.Title);
                    }

                }

                foreach (SPListItem typeToOutput in workBoxTemplates.Items)
                {
                    WBUtils.logMessage("Iterating through ID = " + typeToOutput.ID);
                    
                    List<SPListItem> subListToOutput = groupedByTemplates[typeToOutput.ID];


                    if (subListToOutput != null && (subListToOutput.Count > 0 || showEmptyWorkBoxesTypes))
                    {
                        WBUtils.logMessage("SubList contains " + subListToOutput.Count);
                        html += "<tr><td colspan=4><b>" + typeToOutput.Title + "</b></td></tr>";  
                        html += makeTableRowsOfWorkBoxes(subListToOutput);
                    }

                }
            }
            else            
            {
                List<SPListItem> workBoxes = new List<SPListItem>();
                foreach (SPListItem workBoxMetadataItem in workBoxesList.Items)
                {

                    if (!includeThisWorkBox(workBoxMetadataItem)) continue;

                    workBoxes.Add(workBoxMetadataItem);
                }

                html += makeTableRowsOfWorkBoxes(workBoxes);

            }

            html += "</table>\n";


//            html += "<script type=\"text/javascript\">var parameters='?WorkBoxCollectionURL=";
            //           html += Uri.EscapeDataString(webPart.WorkBoxCollectionURL) + "&TeamsGuid=" + teamsGuid;
   //         html += "'</script>";

//            html += "<script type=\"text/javascript\">var workBoxCollectionRoot='" + webPart.WorkBoxCollectionURL + "';";

            //            html += "var parameters = '?teamsTermGuid=" + teamsGuid + "&workBoxCollectionUrl=" + Uri.EscapeDataString(webPart.WorkBoxCollectionURL) + "';";
               
  //          html += "</script>";

            WebPartContent.Text = html;
        }

        private bool includeThisWorkBox(SPListItem workBox)
        {
                string involvedTeams = workBox.WBxGetColumnAsString(WorkBox.COLUMN_NAME__INVOLVED_TEAMS);
                if (!teamsTermGuid.Equals("") && !involvedTeams.Contains(teamsTermGuid)) return false;

                string workBoxStatus = workBox.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_STATUS);
                if (workBoxStatus.Equals(WorkBox.WORK_BOX_STATUS__OPEN)) return true;

                if (showClosedWorkBoxes && workBoxStatus.Equals(WorkBox.WORK_BOX_STATUS__CLOSED)) return true;

                if (showDeletedWorkBoxes && workBoxStatus.Equals(WorkBox.WORK_BOX_STATUS__DELETED)) return true;

                return false;
        }

        private string makeTableRowsOfWorkBoxes(List<SPListItem> listOfWorkBoxes)
        {
            int count = 0;

            string html = "";

            foreach (SPListItem workBox in listOfWorkBoxes)
            {
                    count++;

                    string status = workBox.WBxGetColumnAsString(WorkBox.COLUMN_NAME__WORK_BOX_STATUS);

                    // THIS SHOULD BE DONE WITH CSS STYLING !!!!!
                    if (status.Equals(WorkBox.WORK_BOX_STATUS__OPEN))
                    {
                        html += "<tr><td></td>";
                        html += "<td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td>";
                        html += "<td><a href='" + workBox[WorkBox.COLUMN_NAME__WORK_BOX_URL] + "'>" + workBox.Name + "</a></td>";
                        html += "<td>" + workBox[WorkBox.COLUMN_NAME__WORK_BOX_STATUS] + "</td>";
                        html += "</tr>";
                    }
                    if (status.Equals(WorkBox.WORK_BOX_STATUS__CLOSED))
                    {
                        html += "<tr><td></td>";
                        html += "<td><img src='/_layouts/images/WorkBoxFramework/work-box-16.png'/></td>";
                        html += "<td><i><a href='" + workBox[WorkBox.COLUMN_NAME__WORK_BOX_URL] + "'>" + workBox.Name + "</a></i></td>";
                        html += "<td><i>" + workBox[WorkBox.COLUMN_NAME__WORK_BOX_STATUS] + "</i></td>";
                        html += "</tr>";
                    }
                    if (status.Equals(WorkBox.WORK_BOX_STATUS__DELETED))
                    {
                        html += "<tr><td></td>";
                        html += "<td></td>";
                        html += "<td><i>" + workBox.Name + "</i></td>";
                        html += "<td><i>" + workBox[WorkBox.COLUMN_NAME__WORK_BOX_STATUS] + "</i></td>";
                        html += "</tr>";
                    }
            }

            if (count == 0)
            {
                html += "<tr><td></td><td colspan=3><i>(None)</i></td></tr>";
            }

            return html;
        }
  
    }


}
