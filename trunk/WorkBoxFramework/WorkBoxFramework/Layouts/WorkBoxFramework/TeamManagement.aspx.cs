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
using System.Web.UI;
using System.Data;
using System.Collections;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Administration;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class TeamManagement : LayoutsPageBase
    {
        WBTaxonomy teams;
        WBTaxonomy functionalAreas;

        protected void Page_Load(object sender, EventArgs e)
        {
            SPSite site = SPContext.Current.Site;

            teams = WBTaxonomy.GetTeams(site);
            functionalAreas = WBTaxonomy.GetFunctionalAreas(teams);

            functionalAreas.InitialiseTaxonomyControl(TeamFunctionalAreas, "Select Functional Area(s)", false, false, this);


            if (!IsPostBack)
            {

                TreeViewTermCollection collection = new TreeViewTermCollection();
                collection.Add(new TreeViewTerm(teams.TermSet));

                // Bind the data source to your collection
                AllTeamsTreeView.DataSource = collection;
                AllTeamsTreeView.DataBind();
            }
        }

        protected void updatePanelWithTeamDetails(WBTeam team)
        {

            TeamName.Text = team.Name;
            TeamGUID.Text = team.Id.ToString();
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

            TeamsSiteURL.Text = team.TeamSiteUrl;

            TeamsSiteGUID.Text = team.TeamSiteGuidString;

            WBLogging.Debug("In TeamManagement.updatePanelWithTeamDetails(): OwnersGroupName = " + team.OwnersGroupName);
            WBLogging.Debug("In TeamManagement.updatePanelWithTeamDetails(): MembersGroupName = " + team.MembersGroupName);

            InformationAssetOwner.WBxInitialise(team.InformationAssetOwner(SPContext.Current.Web));
            TeamManager.WBxInitialise(team.Manager(SPContext.Current.Web));

            //TeamOwnersSharePointUserGroup.CommaSeparatedAccounts = "";
            //TeamOwnersSharePointUserGroup.ResolvedEntities.Clear();
            //TeamOwnersSharePointUserGroup.Entities.Clear();
            TeamOwnersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(team.OwnersGroupName));


            //TeamMembersSharePointUserGroup.CommaSeparatedAccounts = "";
            //TeamMembersSharePointUserGroup.ResolvedEntities.Clear();
            //TeamMembersSharePointUserGroup.Entities.Clear();
            TeamMembersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(team.MembersGroupName));

            //TeamPublishersSharePointUserGroup.CommaSeparatedAccounts = "";
            TeamPublishersSharePointUserGroup.UpdateEntities(WBUtils.CreateEntitiesArrayList(team.PublishersGroupName));

            RecordsTypesListUrl.Text = team.RecordsTypesListUrl;
            CommonActivitiesListUrl.Text = team.CommonActivitiesListUrl;
            FunctionalActivitiesListUrl.Text = team.FunctionalActivitiesListUrl;
        }

        private void resetPanelToSelectedTermValues()
        {
            WBTeam team = teams.GetSelectedTeam(AllTeamsTreeView.SelectedNode.ValuePath);
            updatePanelWithTeamDetails(team);
        }

        protected void AllTeamsTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (AllTeamsTreeView.SelectedNode != null)
            {
                resetPanelToSelectedTermValues();
            }
        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            WBLogging.Debug("In TeamManagement.saveButton_OnClick(): just started");

            WBTeam team = teams.GetSelectedTeam(AllTeamsTreeView.SelectedNode.ValuePath);

            team.Name = TeamName.Text;
            team.Acronym = TeamAcronym.Text;

            WBLogging.Debug("Set name and acronym");

            team.FunctionalAreaUIControlValue = TeamFunctionalAreas.Text;

            team.TeamSiteUrl = TeamsSiteURL.Text;

            WBLogging.Debug("About to set manager");

            team.SetInformationAssetOwner(InformationAssetOwner.WBxGetSingleResolvedUser(SPContext.Current.Web));
            team.SetManager(SPContext.Current.Site, TeamManager.WBxGetSingleResolvedUser(SPContext.Current.Web));

            WBLogging.Debug("Set manager");

            team.OwnersGroupName = WBUtils.EntitiesToPropertyString(TeamOwnersSharePointUserGroup.ResolvedEntities, 1);
            team.MembersGroupName = WBUtils.EntitiesToPropertyString(TeamMembersSharePointUserGroup.ResolvedEntities, 1);
            team.PublishersGroupName = WBUtils.EntitiesToPropertyString(TeamPublishersSharePointUserGroup.ResolvedEntities, 1);

            WBLogging.Debug("In TeamManagement.saveButton_OnClick(): OwnersGroupName is now = " + team.OwnersGroupName);
            WBLogging.Debug("In TeamManagement.saveButton_OnClick(): MembersGroupName is now = " + team.MembersGroupName);
            WBLogging.Debug("In TeamManagement.saveButton_OnClick(): PublishersGroupName is now = " + team.PublishersGroupName);

            team.RecordsTypesListUrl = RecordsTypesListUrl.Text;
            team.CommonActivitiesListUrl = CommonActivitiesListUrl.Text;
            team.FunctionalActivitiesListUrl = FunctionalActivitiesListUrl.Text;            

            team.Update();
            updatePanelWithTeamDetails(team);

            popupMessageOnUpdate("Changes saved OK.");

            WBUtils.logMessage("Clicked Save Changes with TeamsSiteURL.Text = " + TeamsSiteURL.Text);
            WBUtils.logMessage("... and selected path: " + AllTeamsTreeView.SelectedNode.ValuePath);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            resetPanelToSelectedTermValues();

            popupMessageOnUpdate("Changes cancelled.");
        }

        private void recursivelyUpdateTeams(TermCollection terms)
        {
            foreach (Term term in terms)
            {
                WBTeam team = new WBTeam(teams, term);

                team.IndividualCommit = false;
                team.Update();

                recursivelyUpdateTeams(term.Terms);
            }
        }

        protected void checkAllButton_OnClick(object sender, EventArgs e)
        {
            TermCollection terms = teams.TermSet.Terms;

            recursivelyUpdateTeams(terms);

            teams.CommitAll();
        }

        private void popupMessageOnUpdate(String message)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "PopupMessage", String.Format("alert('{0}');", message), true);
        }

    }

}

