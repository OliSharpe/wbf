#region Copyright and License

// Copyright (c) Islington Council 2010-2016
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
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Newtonsoft.Json;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class PublishDocDialogPickLocation : WorkBoxDialogPageBase
    {
        WBRecordsManager manager = null;
        WBPublishingProcess process = null;
        String newOrReplace = null;
        String archiveOrLeave = null;
        WBLocationTreeState treeState = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Debug("In Page_Load");

            WBLogging.Debug("EventArgs type: " + e.GetType().Name);

            manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName);

            if (!IsPostBack)
            {
                process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(Request.QueryString["PublishingProcessJSON"]);
                process.WorkBox = WorkBox;

                WBLogging.Debug("Created the WBProcessObject");

                newOrReplace = Request.QueryString["NewOrReplace"];
                archiveOrLeave = Request.QueryString["ArchiveOrLeave"];
                if (newOrReplace == "New")
                {
                    process.ReplaceAction = WBPublishingProcess.REPLACE_ACTION__CREATE_NEW_SERIES;
                }
                else
                {
                    if (archiveOrLeave == "Archive")
                    {
                        process.ReplaceAction = WBPublishingProcess.REPLACE_ACTION__ARCHIVE_FROM_IZZI;
                    }
                    else
                    {
                        process.ReplaceAction = WBPublishingProcess.REPLACE_ACTION__LEAVE_ON_IZZI;
                    }
                }

                WBLogging.Debug("Captured replace action as: " + process.ReplaceAction);

                PublishingProcessJSON.Value = WBUtils.SerializeToCompressedJSONForURI(process);

                WBLogging.Debug("Serialized the WBProcessObject to hidden field");

                WBRecordsLibrary masterLibrary = manager.Libraries.ProtectedMasterLibrary;

                SPFolder rootFolder = masterLibrary.List.RootFolder;

               
                /*
                TreeViewLocationCollection collection = new TreeViewLocationCollection(manager, viewMode, process.ProtectiveZone, teamFunctionalAreas);

                LibraryLocations.DataSource = collection;
                LibraryLocations.DataBind();
                */

                SelectedFolderPath.Text = "/";
            }
            else
            {
                process = WBUtils.DeserializeFromCompressedJSONInURI<WBPublishingProcess>(PublishingProcessJSON.Value);
                process.WorkBox = WorkBox;
            }

            LibraryLocations.TreeNodePopulate += new TreeNodeEventHandler(LibraryLocations_TreeNodePopulate);
            LibraryLocations.SelectedNodeChanged += new EventHandler(LibraryLocations_SelectedNodeChanged);
            LibraryLocations.PopulateNodesFromClient = true;
            //LibraryLocations.EnableClientScript = false;

            WBTermCollection<WBTerm> teamFunctionalAreas = new WBTermCollection<WBTerm>(manager.FunctionalAreasTaxonomy, process.TeamFunctionalAreasUIControlValue);
            String viewMode = process.IsReplaceActionToCreateNewSeries ? "New" : "Replace";

            treeState = new WBLocationTreeState(SPContext.Current.Web, viewMode, process.ProtectiveZone);

            if (!IsPostBack)
            {
                manager.PopulateWithFunctionalAreas(treeState, LibraryLocations.Nodes, viewMode, teamFunctionalAreas);
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (manager != null)
            {
                manager.Dispose();
                manager = null;
            }
        }

        protected void LibraryLocations_SelectedNodeChanged(object sender, EventArgs e)
        {
            WBLogging.Debug("In LibraryLocations_SelectedNodeChanged():");

            String selectedPath = "";
            if (e is TreeNodeEventArgs && ((TreeNodeEventArgs)e).Node != null)
            {
                TreeNode node = ((TreeNodeEventArgs)e).Node;
                WBLogging.Debug("event was a TreeNodeEventArgs with : " + node.ValuePath);
                selectedPath = node.ValuePath;
            }
            else
            {
                if (LibraryLocations.SelectedNode != null)
                {
                    WBLogging.Debug("In LibraryLocations_SelectedNodeChanged(): LibraryLocations.SelectedNode.ValuePath with : " + LibraryLocations.SelectedNode.ValuePath);
                    selectedPath = LibraryLocations.SelectedNode.ValuePath;
                }

            }

            if (!String.IsNullOrEmpty(selectedPath))
            {
                if (string.IsNullOrEmpty(selectedPath)) selectedPath = "/";

                selectedPath = selectedPath.Replace("(root)", "");
                if (string.IsNullOrEmpty(selectedPath)) selectedPath = "/";

                SelectedFolderPath.Text = selectedPath;

                // Now for the bit where the path is analysed to pick out the selected functional area and the records type:

                String[] pathSteps = selectedPath.Split('/');

                // We're only interested in selections of 3rd level 'folders' that are: functional area / records type / records type  ... or below.
                if (pathSteps.Length < 3) return;

                WBTerm functionalArea = manager.FunctionalAreasTaxonomy.GetSelectedWBTermByPath(pathSteps[0]);
                if (functionalArea == null)
                {
                    WBLogging.Debug("The functional area part of the selected path came back null: " + selectedPath);
                    return;
                }

                Term recordsTypeTerm = manager.RecordsTypesTaxonomy.GetOrCreateSelectedTermByPath(pathSteps[1] + "/" + pathSteps[2]);
                if (recordsTypeTerm == null)
                {
                    WBLogging.Debug("The records type part of the selected path came back null: " + selectedPath);
                    return;
                }
                WBRecordsType recordsType = new WBRecordsType(manager.RecordsTypesTaxonomy, recordsTypeTerm);


                process.FunctionalAreaUIControlValue = functionalArea.UIControlValue;
                process.RecordsTypeUIControlValue = recordsType.UIControlValue;

                WBLogging.Debug("Set the new records type to be: " + process.RecordsTypeUIControlValue);


                // Finally let's see if there is a specific record being selected as well:
                if (!process.IsReplaceActionToCreateNewSeries)
                {
                    WBRecord record = manager.Libraries.GetRecordByPath(selectedPath);

                    SelectedRecordID.Text = record.RecordID;
                    process.ToReplaceRecordID = record.RecordID;
                    process.ToReplaceRecordPath = selectedPath;
                    process.ToReplaceShortTitle = record.Title;
                    process.ToReplaceSubjectTagsUIControlValue = record.SubjectTagsUIControlValue;
                }


                PublishingProcessJSON.Value = WBUtils.SerializeToCompressedJSONForURI(process);

            }
            else
            {
                WBLogging.Debug("In LibraryLocations_SelectedNodeChanged(): Selected path was null");
            }
        }

        protected void LibraryLocations_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            WBLogging.Debug("Call to LibraryLocations_TreeNodePopulate");

            String viewMode = process.IsReplaceActionToCreateNewSeries ? "New" : "Replace";

            manager.PopulateTreeNode(treeState, e.Node, viewMode);

            /*
            TreeNode newNode = new TreeNode();
            newNode.Text = "Test1";
            newNode.Value = "Test1";
            newNode.PopulateOnDemand = false;

            // Add the new node to the ChildNodes collection of the parent node.
            e.Node.ChildNodes.Add(newNode);
            */

            /*
            WBLogging.Debug("Looking for tree node in state at: " + e.Node.ValuePath);
            TreeNode foundNode = LibraryLocations.FindNode(e.Node.ValuePath);

            WBLogging.Debug("foundNode.GetType() = " + foundNode.GetType());
            if (foundNode is WBRecordsTypeTreeNode) WBLogging.Debug("foundNode is WBRecordsTypeTreeNode");
            else WBLogging.Debug("foundNode is NOT WBRecordsTypeTreeNode");
            */
        }


        /*
        private void PopulateNode(TreeNode node)
        {
            WBLogging.Debug("Call to PopulateNode");

            WBLogging.Debug("Call came from node: " + node.Text);

            if (node.Value != "Test2")
            {
                TreeNode newNode = new TreeNode();
                newNode.Text = "Test1";
                newNode.Value = "Test1";
                newNode.PopulateOnDemand = false;

                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(newNode);

                newNode = new TreeNode();
                newNode.Text = "Test2";
                newNode.Value = "Test2";

                newNode.PopulateOnDemand = true;

                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(newNode);


                newNode = new TreeNode();
                newNode.Text = "Test3";
                newNode.Value = "Test3";

                newNode.PopulateOnDemand = false;

                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(newNode);
            }
            else
            {
                TreeNode newNode = new TreeNode();
                newNode.Text = "Test2-1";
                newNode.Value = "Test2-1";
                newNode.PopulateOnDemand = false;

                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(newNode);

                newNode = new TreeNode();
                newNode.Text = "Test2-2";
                newNode.Value = "Test2-2";

                newNode.PopulateOnDemand = false;

                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(newNode);


                newNode = new TreeNode();
                newNode.Text = "Test2-3";
                newNode.Value = "Test2-3";

                newNode.PopulateOnDemand = false;

                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(newNode);
            }


        }
        */

        protected void selectButton_OnClick(object sender, EventArgs e)
        {
            String postBackValue = WBUtils.SerializeToCompressedJSONForURI(process); 

            WBLogging.Debug("About to post back with: " + postBackValue);

            ReturnJSONFromDialogOK(postBackValue);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }

    }
}
