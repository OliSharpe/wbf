﻿#region Copyright and License

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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.OurTeamsWorkBoxes
{
    [ToolboxItemAttribute(false)]
    public class OurTeamsWorkBoxes : WebPart
    {

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Just One Records Type")]
        [WebDescription("Show just this records type.")]
        [System.ComponentModel.Category("Configuration")]
        public String JustRecordsType { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show All Records Types")]
        [WebDescription("Also include the records types that this team does not have any work boxes in.")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowAllRecordsTypes { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show All Team Is Involved")]
        [WebDescription("Also include the work boxes that this team is invovled with.")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowAllTeamInvolved { get; set; }

        private WBTeam team = null;

        protected override void CreateChildControls()
        {
            WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);
            team = WBTeam.GetFromTeamSite(teams, SPContext.Current);

            Literal literal = new Literal();
            string html = "";

            //html += tempStyling();

            WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            TermCollection recordsClassesTerms = recordsTypes.TermSet.Terms;

            html += addWorkBoxesForRecordsClasses(recordsTypes, recordsClassesTerms);


            literal.Text = html;
            this.Controls.Add(literal);
        }


        private string tempStyling()
        {
            string html = "<style type=\"text/css\">\n";

            html += "ul.wbf-our-work-boxes-list { list-style-type: circle; border: auto; padding: 2px; margin: auto; } \n";
            html += "ul.wbf-our-work-boxes-list li { font-weight: normal; list-style-type: circle; border: auto; padding: 2px; margin: auto; } \n";

            html += "ul.wbf-our-work-boxes-list li.wbf-records-class { font-weight: bold; } \n";
            html += "ul.wbf-our-work-boxes-list li.wbf-records-type { font-weight: bold; } \n";

            html += "ul.wbf-our-work-boxes-list li.wbf-records-type td { font-weight: normal; } \n";

            html += "ul.wbf-our-work-boxes-list { margin-left: 10px; } \n";
            html += "ul.wbf-our-work-boxes-list ul { margin-left: 20px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul { margin-left: 30px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul ul { margin-left: 40px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul ul ul { margin-left: 50px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul ul ul ul { margin-left: 60px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul ul ul ul ul { margin-left: 70px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul ul ul ul ul ul { margin-left: 80px; } \n";
            html += "ul.wbf-our-work-boxes-list ul ul ul ul ul ul ul ul { margin-left: 90px; } \n";

            html += "</style>\n\n";

            return html;

        }

        private string addWorkBoxesForRecordsClasses(WBTaxonomy recordsTypes, TermCollection recordsClassesTerms)
        {
            if (recordsClassesTerms.Count == 0) return "";

            string finalHtml = "<ul class=\"wbf-our-work-boxes-list wbf-records-classes\">\n";

            foreach (Term recordsClassTerm in recordsClassesTerms)
            {
                string html = addWorkBoxesForRecordsClass(recordsTypes, recordsClassTerm.Terms);

                if (html != "" || ShowAllRecordsTypes)
                {
                    html = "<li class=\"wbf-records-class\">\n" + recordsClassTerm.Name + "\n" + html + "</li>\n";
                    finalHtml += html;
                }
            }

            finalHtml += "</ul>\n";

            return finalHtml;
        }

        private string addWorkBoxesForRecordsClass(WBTaxonomy recordsTypes, TermCollection recordsTypesTerms)
        {
            if (recordsTypesTerms.Count == 0) return "";

            string finalHtml = "";

            bool containsWorkBoxesForTeam = false;

            foreach (Term recordsTypeTerm in recordsTypesTerms)
            {
                WBRecordsType recordsType = new WBRecordsType(recordsTypes, recordsTypeTerm);

                // If the 'JustRecordsType' value has been set then we just want to look for that one type:
                if (JustRecordsType != null && JustRecordsType != "" && recordsType.Name != JustRecordsType) continue;

                string html = "";
                string workBoxesHtml = "";
                containsWorkBoxesForTeam = false;

                html += "<li class=\"wbf-records-type\">" + recordsType.Name;
                html += "\n";

                string workBoxCollectionURL = recordsType.WorkBoxCollectionUrl;
                WBUtils.logMessage("The work box collection url = " + workBoxCollectionURL);

                if (workBoxCollectionURL != "")
                {
                    bool originalAccessDeniedCatchValue = SPSecurity.CatchAccessDeniedException;
                    SPSecurity.CatchAccessDeniedException = false;

                    try
                    {
                        using (WBCollection collection = new WBCollection(workBoxCollectionURL))
                        {
                            SPListItemCollection workBoxResults = collection.QueryFilteredBy(team, recordsType, false);

                            if (workBoxResults != null && workBoxResults.Count > 0)
                            {
                                containsWorkBoxesForTeam = true;

                                workBoxesHtml = addWorkBoxResults(collection, workBoxResults);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        WBUtils.logMessage("UnauthorizedAccessException thrown for user trying to access: " + workBoxCollectionURL + " Exception was: " + e.Message);

                        // Let's just hide this for the moment as the user doesn't have access to here anyway.
                        workBoxesHtml = "";
                    }
                    catch (Exception e)
                    {
                        workBoxesHtml = "<i>Exception occured when trying to get results from the work box collection at: " + workBoxCollectionURL + " Exception was: " + e.Message + "</i>";
                    }
                    finally
                    {
                        SPSecurity.CatchAccessDeniedException = originalAccessDeniedCatchValue;
                    }


                }

                if (containsWorkBoxesForTeam || ShowAllRecordsTypes)
                {
                    html += workBoxesHtml;
                    html += "</li>\n";

                    finalHtml += html;
                }
            }

            if (finalHtml != "" || ShowAllRecordsTypes)
            {
                finalHtml = "<ul class=\"wbf-our-work-boxes-list wbf-records-types\">\n" + finalHtml + "</ul>\n";
            }
            return finalHtml;
        }

        private String addWorkBoxResults(WBCollection collection, SPListItemCollection workBoxResults)
        {
            if (workBoxResults == null || workBoxResults.Count == 0) return "";

            String html = "<table cellpadding=\"2\">\n";

            foreach (SPListItem item in workBoxResults)
            {
                using (WorkBox workBox = new WorkBox(collection, item))
                {
                    bool include = false;

                    if (ShowAllTeamInvolved) include = true;
                    else if (workBox.OwningTeam.Name == team.Name) include = true;

                    if (include)
                    {
                        html += string.Format("<tr><td><img src=\"{0}\"/></td><td><a href=\"{1}\">{2}</a></td><td><a href=\"{3}\">{4}</a></td><td>{5}</td></tr>\n",
                            "/_layouts/images/WorkBoxFramework/work-box-16.png",
                            workBox.Url,
                            workBox.Title,
                            workBox.OwningTeam.TeamSiteUrl,
                            workBox.OwningTeam.Name,
                            workBox.DateCreated.ToString("d"));
                    }
                }
            }

            html += "</table>\n";

            return html;
        }
    }
}
