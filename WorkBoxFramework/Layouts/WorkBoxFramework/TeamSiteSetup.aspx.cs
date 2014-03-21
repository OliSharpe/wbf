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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class TeamSiteSetup : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SPSite site = SPContext.Current.Site;
                SPWeb web = SPContext.Current.Web;

                WBTaxonomy teams = WBTaxonomy.GetTeams(site);
                WBTaxonomy functionalAreas = WBTaxonomy.GetFunctionalAreas(teams);

                teams.InitialiseTaxonomyControl(TeamTerm, "Select Team Term", false);
                teams.InitialiseTaxonomyControl(ParentTeamTerm, "Select the Parent Team Term", false);
                functionalAreas.InitialiseTaxonomyControl(TeamFunctionalAreas, "Select the functional area", false);

                TeamName.Text = web.Title;

                WBTeam team = WBTeam.GetFromTeamSite(teams, web);
                if (team != null)
                {
                    TeamTerm.Text = team.UIControlValue;
                    TeamAcronym.Text = team.Acronym;

                    TeamFunctionalAreas.Text = team.FunctionalAreaUIControlValue;

                    if (TeamFunctionalAreas.Text == "")
                    {
                        InheritedFunctionalAreas.Text = team.FunctionalArea(functionalAreas).Names();
                    }
                    else
                    {
                        InheritedFunctionalAreas.Text = "";
                    }

                    TeamManager.WBxInitialise(team.Manager(web));

                    TeamOwnersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(team.OwnersGroupName));
                    TeamMembersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(team.MembersGroupName));
                    TeamPublishersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(team.PublishersGroupName));

                    TeamTerm.Enabled = false;
                    ParentTeamTerm.Enabled = false;
                    TeamOwnersSharePointUserGroup.Enabled = false;
                    TeamMembersSharePointUserGroup.Enabled = false;
                    RecordsTypesListUrl.Text = team.RecordsTypesListUrl;

                }
                else
                {
                    TeamTerm.Text = "";

                    SPWeb parentWeb = web.ParentWeb;
                    WBTeam parentTeam = WBTeam.GetFromTeamSite(teams, parentWeb);

                    if (parentTeam != null)
                    {
                        ParentTeamTerm.Text = parentTeam.UIControlValue;

                        InheritedFunctionalAreas.Text = parentTeam.FunctionalArea(functionalAreas).Names();

                        RecordsTypesListUrl.Text = parentTeam.RecordsTypesListUrl;
                    }

                    TeamOwnersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(web.Title + " - Owners"));
                    TeamMembersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(web.Title + " - Members"));

                }
            }

        }

        protected void okButton_OnClick(object sender, EventArgs e)
        {
            SPSite site = SPContext.Current.Site;
            SPWeb web = SPContext.Current.Web;

            WBTaxonomy teams = WBTaxonomy.GetTeams(site);

            WBTeam team = null;

            if (TeamTerm.Enabled)
            {
                if (TeamTerm.Text != "" && ParentTeamTerm.Text != "")
                {
                    TeamTermStatus.Text = "You can only select either a direct term or the parent term, not both!";
                    return;
                }

                if (TeamTerm.Text == "" && ParentTeamTerm.Text == "")
                {
                    TeamTermStatus.Text = "You must select either a direct term or the parent term.";
                    return;
                }

                if (TeamTerm.Text != "")
                {
                    team = new WBTeam(teams, TeamTerm.Text);
                }
                else
                {
                    if (ParentTeamTerm.Text != "")
                    {
                        WBTerm parent = new WBTerm(teams, ParentTeamTerm.Text);

                        Term newTerm = parent.Term.CreateTerm(web.Title, WorkBox.LOCALE_ID_ENGLISH);
                        team = new WBTeam(teams, newTerm);
                    }
                }

                team.OwnersGroupName = WBUtils.EntitiesToPropertyString(TeamOwnersSharePointUserGroup.ResolvedEntities, 1);
                team.MembersGroupName = WBUtils.EntitiesToPropertyString(TeamMembersSharePointUserGroup.ResolvedEntities, 1);
                team.TeamSiteUrl = web.Url;                

            }
            else
            {
                team = WBTeam.GetFromTeamSite(teams, web);
            }

            if (team == null)
            {
                TeamTermStatus.Text = "Had a problem finding or creating the term!";
                return;
            }

            team.Name = TeamName.Text;
            web.Title = TeamName.Text;
            team.Acronym = TeamAcronym.Text;

            team.SetManager(site, TeamManager.WBxGetSingleResolvedUser(web));

            team.PublishersGroupName = WBUtils.EntitiesToPropertyString(TeamPublishersSharePointUserGroup.ResolvedEntities, 1);
            team.RecordsTypesListUrl = RecordsTypesListUrl.Text;

            if (!String.IsNullOrEmpty(TeamFunctionalAreas.Text))
            {
                team.FunctionalAreaUIControlValue = TeamFunctionalAreas.Text;
            }

            // This will actually update the web as well as the term.
            team.UpdateWithTeamSiteWeb(web);           

            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

    }
}
