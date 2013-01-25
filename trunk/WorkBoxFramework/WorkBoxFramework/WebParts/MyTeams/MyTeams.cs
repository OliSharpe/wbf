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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.MyTeams
{
    [ToolboxItemAttribute(false)]
    public class MyTeams : WebPart
    {

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Parent Teams Term")]
        [WebDescription("The Teams term that should be used as the root parent when iterating through the teams.")]
        [System.ComponentModel.Category("Configuration")]
        public String ParentTeamsTerm { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show All Teams")]
        [WebDescription("Include the teams that this user is not a member of.")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowAllTeams { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Use Tree View")]
        [WebDescription("Use a tree view UI control")]
        [System.ComponentModel.Category("Configuration")]
        public bool UseTreeView { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Always Provide Link")]
        [WebDescription("Always have an active link to any of the teams that are show.")]
        [System.ComponentModel.Category("Configuration")]
        public bool AlwaysProvideLink { get; set; }

        private bool displayingAnyTeams = false;
        private SPUser selectedUser = null;

        protected override void CreateChildControls()
        {
            WBTaxonomy teams = WBTaxonomy.GetTeams(SPContext.Current.Site);

            String selectedUserLoginName = Page.Request.QueryString["LoginName"];
            if (!String.IsNullOrEmpty(selectedUserLoginName))
            {
                selectedUser = SPContext.Current.Web.WBxEnsureUserOrNull(selectedUserLoginName);
            }

            TermCollection terms = teams.TermSet.Terms;

            if (ParentTeamsTerm != null && ParentTeamsTerm != "")
            {
                Term parentTerm = teams.GetSelectedTermByPath(ParentTeamsTerm);

                terms = parentTerm.Terms;
            }

            SPTreeView treeView = new SPTreeView();

            TreeNodeStyle nodeStyle = new TreeNodeStyle();
            treeView.NodeStyle.HorizontalPadding = new Unit(3);
            treeView.NodeStyle.CssClass = "wbf-team-tree-node";

            this.Controls.Clear();
            this.Controls.Add(treeView);

            if (UseTreeView)
            {
                treeView.ShowLines = true;

                treeView.Nodes.Clear();

                PopulateTreeView(treeView.Nodes, teams, terms);
            }
            else
            {
                PopulateListView(teams, terms);
            }

            if (!displayingAnyTeams)
            {
                Literal noTeamsLiteral = new Literal();
                noTeamsLiteral.Text = "<i>(You are not a member of such a team site yet. Contact the SharePoint champion in your area for more information)</i>";
                this.Controls.Add(noTeamsLiteral);
            }
        }

        private bool PopulateTreeView(TreeNodeCollection nodes, WBTaxonomy teams, TermCollection terms)
        {
            Dictionary<String, TreeNode> allNodes = new Dictionary<String, TreeNode>();

            bool containsTeamForMe = false;
            foreach (Term term in terms)
            {
                WBTeam team = new WBTeam(teams, term);

                if (term.IsAvailableForTagging)
                {
                    bool isTeamMember = IsSelectedUserTeamMember(team);

                    TreeNode teamTreeNode = new TreeNode();
                    teamTreeNode.Text = team.Name;
                    teamTreeNode.Value = team.Id.ToString();

                    if (isTeamMember)
                    {
                        displayingAnyTeams = true;

                        teamTreeNode.NavigateUrl = team.TeamSiteUrl;
                        teamTreeNode.ImageUrl = "http://sp.izzi/Style%20Library/team-16.png";                    
                        //teamTreeNode.ImageUrl = "/_layouts/Images/WorkBoxFramework/team-16.png";
                    }

                    if (AlwaysProvideLink)
                    {
                        teamTreeNode.NavigateUrl = team.TeamSiteUrl;
                    }



                    if (PopulateTreeView(teamTreeNode.ChildNodes, teams, term.Terms) || ShowAllTeams || isTeamMember)
                    {
                        containsTeamForMe = true;
                        allNodes.Add(team.Name, teamTreeNode);
                    }
                }
            }

            List<String> names = new List<String>(allNodes.Keys);
            names.Sort();

            foreach (String name in names)
            {
                nodes.Add(allNodes[name]);
            }

            return containsTeamForMe;
        }

        private bool IsSelectedUserTeamMember(WBTeam team)
        {
            if (selectedUser == null) 
            {
                return team.IsCurrentUserTeamMember();
            }
            else
            {
                return team.IsUserTeamMember(selectedUser);
            }
        }

        private void PopulateListView(WBTaxonomy teams, TermCollection terms)
        {

            Literal literal = new Literal();

            string html = ""; // tempStyling();

            html += addChildTeamsFromTerms(teams, terms);

            literal.Text = html;

            this.Controls.Add(literal);

        }

        private string addChildTeamsFromTerms(WBTaxonomy teams, TermCollection terms)
        {
            if (terms.Count == 0) return "";

            string finalHtml = ""; 

            string liClassList = "";
            string liText = "";
            bool containsTeamForMe = false;
            

            foreach (Term term in terms)
            {
                WBTeam team = new WBTeam(teams, term);
                string html = "";

                containsTeamForMe = false;

                liClassList = "wbf-team";
                liText = team.Name;

                if (team.MembersGroupName != "")
                {
                    if (IsSelectedUserTeamMember(team))
                    {
                        displayingAnyTeams = true;

                        liClassList += " wbf-am-team-member";
                        liText = string.Format("<a href=\"{0}\">{1}</a>",
                            team.TeamSiteUrl,
                            team.Name);
                        containsTeamForMe = true;
                    }
                    else
                    {
                        liClassList += " wbf-not-team-member";
                    }
                } 
                else
                {
                        liClassList += " wbf-no-team-group-defined";
                }
                  
                if (term.Terms.Count > 0)
                {
                    liClassList += " wbf-team-has-children";
                }

                html += "<li class=\"" + liClassList + "\">" + liText;
                html += "\n";

                // Note that this method returns "" if term.Terms.Count == 0 or there are no child teams for this user
                string childHtml = addChildTeamsFromTerms(teams, term.Terms);

                if (childHtml != "")
                {
                    containsTeamForMe = true;
                    html += childHtml;
                }
                html += "</li>\n";

                if (containsTeamForMe || ShowAllTeams)
                {
                    finalHtml += html;
                }
            }

            if (finalHtml != "") 
            {
                finalHtml = "<ul class=\"wbf-my-teams-list\">\n" + finalHtml + "</ul>\n";
            }
            return finalHtml;
        }
    }
}
