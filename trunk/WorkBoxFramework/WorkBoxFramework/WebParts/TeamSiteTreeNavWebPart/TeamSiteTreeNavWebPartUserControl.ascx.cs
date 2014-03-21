using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;


namespace WorkBoxFramework.WebParts.TeamSiteTreeNavWebPart
{
    public partial class TeamSiteTreeNavWebPartUserControl : UserControl
    {
        public WBTeam Team;
        public String NotSetupText = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPSite site = SPContext.Current.Site;

            WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            string teamGUIDString = "";
            Team = WBTeam.GetFromTeamSite(SPContext.Current);
            if (Team == null)
            {
                NotSetupText = "(<i>This site doesn't appear to be a team site so this web part wont work here.</i>)";
                return;
            }

            teamGUIDString = WBExtensions.WBxToString(Team.Id);
            string recordsTypesListUrl = Team.RecordsTypesListUrl;

            /*
             * For the moment this web part is just going to list all of the available records types so the following code is not needed
             * 
            if (recordsTypesListUrl == null || recordsTypesListUrl == "")
            {
                //recordsTypesListUrl = web.Url + "/Lists/Configure%20Teams%20Records%20Classes";
                NotSetupText = "(<i>The team has no records types list setup yet.</i>)";
                return;
            }
             */ 

            string selectedRecordsTypeGUID = Request.QueryString["recordsTypeGUID"];

            try
            {

                foreach (Term term in recordsTypesTaxonomy.TermSet.Terms)
                {
                    WBRecordsType recordsClass = new WBRecordsType(recordsTypesTaxonomy, term);

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
