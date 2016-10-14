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


        protected void Page_Load(object sender, EventArgs e)
        {

            manager = new WBRecordsManager();

            if (!IsPostBack)
            {
                process = JsonConvert.DeserializeObject<WBPublishingProcess>(Request.QueryString["PublishingProcessJSON"]);
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

                PublishingProcessJSON.Value = JsonConvert.SerializeObject(process);

                WBLogging.Debug("Serialized the WBProcessObject to hidden field");

                WBRecordsLibrary masterLibrary = manager.Libraries.ProtectedMasterLibrary;

                SPFolder rootFolder = masterLibrary.List.RootFolder;

                WBTermCollection<WBTerm> teamFunctionalAreas = new WBTermCollection<WBTerm>(manager.FunctionalAreasTaxonomy, process.TeamFunctionalAreasUIControlValue);

                String viewMode = process.IsReplaceActionToCreateNewSeries ? "New" : "Replace";
                TreeViewLocationCollection collection = new TreeViewLocationCollection(manager, viewMode, process.ProtectiveZone, teamFunctionalAreas);

                LibraryLocations.DataSource = collection;
                LibraryLocations.DataBind();

                SelectedFolderPath.Text = "/";
            }
            else
            {
                process = JsonConvert.DeserializeObject<WBPublishingProcess>(PublishingProcessJSON.Value);
                process.WorkBox = WorkBox;
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

        protected void LibraryLocations_Bound(object sender, TreeNodeEventArgs e)
        {
            if (e.Node.Text.Contains("."))
            {
                e.Node.ImageUrl = SPUtility.ConcatUrls("/_layouts/images/",
                            SPUtility.MapToIcon(WorkBox.Web,
                            SPUtility.ConcatUrls(WorkBox.Web.Url, e.Node.Text), "", IconSize.Size16));
            }

        }
        
        protected void LibraryLocations_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (LibraryLocations.SelectedNode != null)
            {
                string selectedPath = LibraryLocations.SelectedNode.ValuePath;
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
                }


                PublishingProcessJSON.Value = JsonConvert.SerializeObject(process);

            }
        }


        protected void selectButton_OnClick(object sender, EventArgs e)
        {
            String postBackValue = JsonConvert.SerializeObject(process); 

            WBLogging.Debug("About to post back with: " + postBackValue);

            ReturnJSONFromDialogOK(postBackValue);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }

    }
}
