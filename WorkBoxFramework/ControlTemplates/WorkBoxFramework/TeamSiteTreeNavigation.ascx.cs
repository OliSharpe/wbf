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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ControlTemplates.WorkBoxFramework
{
    public partial class TeamSiteTreeNavigation : UserControl
    {
        public String ConfigurationListName;
        public String RecordsGroup;
        public String AdditionalCSSStyle;
        public String NotSetupText = "";

        public SPList ConfigurationList = null;

        public WBTeam Team;


        protected void Page_Load(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPSite site = SPContext.Current.Site;

            WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            string teamGUIDString = "";
            Team = WBTeam.getFromTeamSite(SPContext.Current);
            if (Team == null) return;
                
            teamGUIDString = WBExtensions.WBxToString(Team.Id);
            string recordsTypesListUrl = Team.RecordsTypesListUrl;

            if (recordsTypesListUrl == null || recordsTypesListUrl == "")
            {
                //recordsTypesListUrl = web.Url + "/Lists/Configure%20Teams%20Records%20Classes";
                NotSetupText = "(<i>The team has no records types list setup yet.</i>)";
                return;
            }

            string selectedRecordsTypeGUID = Request.QueryString["recordsTypeGUID"];            

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

                    RecordsTypeTreeView.Nodes.Clear();

                    foreach (SPListItem item in ConfigurationList.Items)
                    {
                        try
                        {

                            string groupName = item.WBxGetColumnAsString("Records Group");
                            if (groupName.Equals(RecordsGroup))
                            {

                                WBRecordsType recordsClass = new WBRecordsType(recordsTypesTaxonomy, WBExtensions.WBxGetColumnAsString(item, "Records Class"));

                                TreeNode node = createNodes(recordsClass);

                                RecordsTypeTreeView.Nodes.Add(node);

                                RecordsTypeTreeView.CollapseAll();

                                expandByRecordsTypeGUID(RecordsTypeTreeView.Nodes, selectedRecordsTypeGUID);
                            }
                        }
                        catch (Exception exception)
                        {
                            WBUtils.logMessage("The error message was: " + exception.Message);
                        }
                    }
                }
            }
        }

        private TreeNode createNodes(WBRecordsType recordsType)
        {
            TreeNode node = new TreeNode();
            node.Text = recordsType.Name;
            node.NavigateUrl = "javascript: WorkBoxFramework_triggerWebPartUpdate('" + recordsType.Id.ToString() + "'); ";
            node.Value = recordsType.Id.WBxToString();

            Dictionary<String, TreeNode> allNodes = new Dictionary<String, TreeNode>();

            foreach (Term term in recordsType.Term.Terms)
            {
                WBRecordsType childRecordsType = new WBRecordsType(recordsType.Taxonomy, term);

                if (term.IsAvailableForTagging && childRecordsType.AllowWorkBoxRecords)
                {
                    TreeNode childNode = createNodes(childRecordsType);
                    allNodes.Add(childNode.Text, childNode);
                }
            }

            List<String> names = new List<String>(allNodes.Keys);
            names.Sort();

            foreach (String name in names)
            {
                node.ChildNodes.Add(allNodes[name]);
            }

            return node;
        }

        private void expandByRecordsTypeGUID(TreeNodeCollection nodes, String recordsTypeGUID)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Value == recordsTypeGUID)
                {
                    WBLogging.Debug("Found the node to expand: " + node.Text);
                    expandNodeAndParents(node);
                    return;
                }

                expandByRecordsTypeGUID(node.ChildNodes, recordsTypeGUID);
            }
        }

        private void expandNodeAndParents(TreeNode node)
        {
            node.Expand();
            if (node.Parent != null) expandNodeAndParents(node.Parent);
        }
    }
}
