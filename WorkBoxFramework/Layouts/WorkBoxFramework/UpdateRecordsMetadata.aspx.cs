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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Publishing;
using Microsoft.Office.RecordsManagement.RecordsRepository;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class UpdateRecordsMetadata : WBDialogPageBase
    {
        private SPSite librarySite = null;
        private SPWeb libraryWeb = null;
        private bool needToDispose = false;

        private SPList libraryList = null;
        private SPListItem recordItem = null;
        private WBDocument record = null;

        private WBTaxonomy subjectTagsTaxonomy = null;
        private WBTaxonomy recordsTypesTaxonomy = null;

        private String currentUserLoginName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // First we're going to check membership of the records management group:
            String rmGroupName = WBFarm.Local.RecordsManagersGroupName;
            SPGroup rmGroup = null;

            if (!String.IsNullOrEmpty(rmGroupName))
            {
                try
                {
                    rmGroup = SPContext.Current.Web.SiteGroups[rmGroupName];
                }
                catch (Exception exception)
                {
                    // Probably the group hasn't been created or setup here yet
                }
            }

            if (rmGroup == null || !rmGroup.ContainsCurrentUser)
            {
                AccessDeniedPanel.Visible = true;
                UpdateRecordsMetadataPanel.Visible = false;
                return;
            }

            currentUserLoginName = SPContext.Current.Web.CurrentUser.LoginName;


            String listIDString = "";
            String itemIDString = "";
            String recordIDString = "";

            recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);
            subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(recordsTypesTaxonomy);

            librarySite = new SPSite(WBFarm.Local.ProtectedRecordsLibraryUrl);
            libraryWeb = librarySite.OpenWeb();
            
            libraryList = libraryWeb.GetList(WBFarm.Local.ProtectedRecordsLibraryUrl);


            if (!IsPostBack)
            {
                recordIDString = Request.QueryString["RecordID"];
                
                if (String.IsNullOrEmpty(recordIDString))
                {
                    listIDString = Request.QueryString["ListID"];
                    itemIDString = Request.QueryString["ItemID"];
                }
                else
                {
                    listIDString = libraryList.ID.ToString();
                }

                ListID.Value = listIDString;
                ItemID.Value = itemIDString;
            }
            else
            {
                recordIDString = RecordID.Text;
                listIDString = ListID.Value;
                itemIDString = ItemID.Value;
            }

            if (!String.IsNullOrEmpty(itemIDString))
            {
                int itemID = Convert.ToInt32(itemIDString);

                recordItem = libraryList.GetItemById(itemID);
            }
            else
            {
                recordItem = WBUtils.FindItemByColumn(SPContext.Current.Site, libraryList, WBColumn.RecordID, recordIDString);

                if (recordItem != null)
                {
                    itemIDString = recordItem.ID.ToString();
                    ItemID.Value = itemIDString;
                }
            }

            if (recordItem != null)
            {
                record = new WBDocument(recordItem);

                FunctionalArea.Text = record.FunctionalArea.Names();

                WBRecordsType recordsType = record.RecordsType;

                recordsType.Taxonomy = recordsTypesTaxonomy;

                RecordsType.Text = recordsType.FullPath.Replace("/", " / ");


                if (!IsPostBack)
                {
                    Filename.Text = recordItem.Name;
                    Title.Text = recordItem.Title;
                    RecordID.Text = record[WBColumn.RecordID].WBxToString(); 

                    LiveOrArchived.DataSource = new String[] { "Live", "Archived" };
                    LiveOrArchived.DataBind();
                    LiveOrArchived.SelectedValue = record[WBColumn.LiveOrArchived] as String;

                    ProtectiveZone.DataSource = WBRecordsType.getProtectiveZones();
                    ProtectiveZone.DataBind();
                    ProtectiveZone.SelectedValue = record.ProtectiveZone;

                    subjectTagsTaxonomy.InitialiseTaxonomyControl(SubjectTags, WBColumn.SubjectTags.DisplayName, true);
                    SubjectTags.Text = record.SubjectTags.UIControlValue;
                }
            }

            if (!IsPostBack)
            {
                libraryWeb.Dispose();
                librarySite.Dispose();
            }
        }

        protected void updateButton_OnClick(object sender, EventArgs e)
        {
            bool digestOK = SPContext.Current.Web.ValidateFormDigest();

            if (digestOK)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite elevatedSite = new SPSite(librarySite.ID))
                    using (SPWeb elevatedWeb = elevatedSite.OpenWeb(libraryWeb.ID))
                    {
                        SPList elevatedList = elevatedWeb.Lists[new Guid(ListID.Value)];
                        SPListItem elevatedItem = elevatedList.GetItemById(Convert.ToInt32(ItemID.Value));

                        Records.BypassLocks(elevatedItem, delegate(SPListItem item)
                        {
                            item.File.CheckOut();    

                            item[WBColumn.LiveOrArchived.DisplayName] = LiveOrArchived.SelectedValue;
                            item[WBColumn.ProtectiveZone.DisplayName] = ProtectiveZone.SelectedValue;
                            item.WBxSetMultiTermColumn(WBColumn.SubjectTags.DisplayName, SubjectTags.Text);

                            item.Update();

                            item.File.CheckIn("Metadata update by user: " + currentUserLoginName + " Reason: " + ReasonForChange.Text);
                        });
                    }
                });

                if (LiveOrArchived.SelectedValue == "Live")
                {
                    if (ProtectiveZone.SelectedValue == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                    {
                        UpdateCopyInLibrary(recordItem, WBFarm.Local.PublicRecordsLibraryUrl);
                    }
                    else
                    {
                        RemoveCopyFromLibrary(recordItem, WBFarm.Local.PublicRecordsLibraryUrl);
                    }

                    if (ProtectiveZone.SelectedValue == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                    {
                        UpdateCopyInLibrary(recordItem, WBFarm.Local.PublicExtranetRecordsLibraryUrl);
                    }
                    else
                    {
                        RemoveCopyFromLibrary(recordItem, WBFarm.Local.PublicExtranetRecordsLibraryUrl);
                    }
                }
                else
                {
                    RemoveCopyFromLibrary(recordItem, WBFarm.Local.PublicRecordsLibraryUrl);
                    RemoveCopyFromLibrary(recordItem, WBFarm.Local.PublicExtranetRecordsLibraryUrl);
                }


                returnFromDialogOKAndRefresh();                
            }
            else
            {
                returnFromDialogError("The security digest for the request was not OK");
            }

            libraryWeb.Dispose();
            librarySite.Dispose();
        }

        private void RemoveCopyFromLibrary(SPListItem recordItem, string copyLibraryUrl)
        {
            using (SPSite copyLibrarySite = new SPSite(copyLibraryUrl))
            using (SPWeb copyLibraryWeb = copyLibrarySite.OpenWeb(copyLibraryUrl))
            {
                SPList copyLibraryList = copyLibraryWeb.GetList(copyLibraryUrl);

                if (copyLibraryList != null)
                {
                    SPListItem existingCopyItem = WBUtils.FindItemByColumn(copyLibrarySite, copyLibraryList, WBColumn.RecordID, recordItem[WBColumn.RecordID.DisplayName].WBxToString());

                    if (existingCopyItem == null)
                    {
                        // There is currently no such item - so there is nothing to remove.
                    }
                    else
                    {
                        Records.UndeclareItemAsRecord(existingCopyItem);
                        existingCopyItem.Delete();
                        //libraryWeb.Update();
                    }
                }
            }
        }

        private void UpdateCopyInLibrary(SPListItem recordItem, string copyLibraryUrl)
        {
            //WBLogging.RecordsTypes.Unexpected("UpdateCopyInLibrary(): looking at libraryURL: " + copyLibraryUrl);

            using (SPSite copyLibrarySite = new SPSite(copyLibraryUrl))
            using (SPWeb copyLibraryWeb = copyLibrarySite.OpenWeb(copyLibraryUrl))
            {
                SPList copyLibraryList = copyLibraryWeb.GetList(copyLibraryUrl);

                if (copyLibraryList != null)
                {
                    SPListItem existingCopyItem = WBUtils.FindItemByColumn(copyLibrarySite, copyLibraryList, WBColumn.RecordID, recordItem[WBColumn.RecordID.DisplayName].WBxToString());

                    if (existingCopyItem == null)
                    {
                        // There is currently no such item - so we need to copy across the document:
                        List<String> folderPath = WBUtils.GetFolderPathWithoutFilename(recordItem.Url);
                        // We're going to remove the first item from this list as that is the name of the 'root folder' which is
                        // essentially the name of the list:
                        folderPath.RemoveAt(0);

                        recordItem.File.WBxCopyTo(copyLibraryUrl, folderPath, false, true);
                    }
                    else
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (SPSite elevatedSite = new SPSite(copyLibrarySite.ID))
                            using (SPWeb elevatedWeb = elevatedSite.OpenWeb(copyLibraryWeb.ID))
                            {
                                SPList elevatedList = elevatedWeb.Lists[copyLibraryList.ID];
                                SPListItem elevatedItem = elevatedList.GetItemById(Convert.ToInt32(existingCopyItem.ID));

                                Records.BypassLocks(elevatedItem, delegate(SPListItem item)
                                {
                                    item.File.CheckOut();

                                    item[WBColumn.LiveOrArchived.DisplayName] = LiveOrArchived.SelectedValue;
                                    item[WBColumn.ProtectiveZone.DisplayName] = ProtectiveZone.SelectedValue;
                                    item.WBxSetMultiTermColumn(WBColumn.SubjectTags.DisplayName, SubjectTags.Text);

                                    item.Update();

                                    item.File.CheckIn("Updated record");
                                });
                            }
                        });
                    }

                }
            }
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Update to the record was cancelled.");

            libraryWeb.Dispose();
            librarySite.Dispose();
        }

    }
}
