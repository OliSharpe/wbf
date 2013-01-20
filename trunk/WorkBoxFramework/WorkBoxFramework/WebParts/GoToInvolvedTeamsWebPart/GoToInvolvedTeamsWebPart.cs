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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Taxonomy;
namespace WorkBoxFramework.GoToInvolvedTeamsWebPart
{
    [ToolboxItemAttribute(false)]
    public class GoToInvolvedTeamsWebPart : WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Teams Root Site URL")]
        [WebDescription("Enter the URL for the root site of all teams")]
        [System.ComponentModel.Category("Configuration")]
        public string TeamsRootSiteURL { get; set; }


        private string _errorMessage = "";

        public const string KEY_FOR_SITE_TO_TERM_MAPPING = "wb_key_for_site_to_term_mapping";
        public const string KEY_FOR_TERM_TO_SITE_MAPPING = "wb_key_for_term_to_site_mapping";

        protected override void CreateChildControls()
        {
            Literal literal = new Literal();
            string html = "";

            WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);

            if (workBox == null) return;

            WBTermCollection<WBTeam> involvedTeams = workBox.InvolvedTeams;

            if (involvedTeams == null)
            {
                addErrorMessage("Couldn't find a involvedTeams for the field");
            }
            else
            {
                if (involvedTeams.Count > 0)
                {
                    if (TeamsRootSiteURL == null || TeamsRootSiteURL == "")
                    {
                        TeamsRootSiteURL = WBFarm.Local.TeamSitesSiteCollectionUrl;
                    }


                    try
                    {
                        using (SPSite teamsSite = new SPSite(TeamsRootSiteURL))
                        {
                            html += "<table cellpadding='5'>";
                            foreach (WBTeam team in involvedTeams)
                            {
                                html += "<tr><td><a href='" + team.TeamSiteUrl + "'>" + team.Name + "</a></td></tr>";
                            }
                            html += "</table>";
                        }

                    }
                    catch (Exception e)
                    {
                        html = "Exception was thrown " + e.StackTrace;
                    }


                }
                else
                {
                    html += "<i>(none)</i>";
                }
            }

            literal.Text = html;

            this.Controls.Add(literal);
        }

        protected void addErrorMessage(string message)
        {
            _errorMessage += message;
            WBUtils.logMessage("Error on a GoToInvolvedTeams web part: " + message);
        }

    }
}
