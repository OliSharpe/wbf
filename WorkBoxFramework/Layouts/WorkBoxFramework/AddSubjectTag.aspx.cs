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
using System.Text.RegularExpressions;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class AddSubjectTag : WBDialogPageBase
    {
        string _path = String.Empty;
        public bool CreateNew = false;  // Create or Edit mode

        protected override void OnInit(EventArgs e)
        {
            InitHtmlEditors();
            SPRibbon ribbon = SPRibbon.GetCurrent(this.Page);
            if (ribbon != null)
            {
                ribbon.CommandUIVisible = true;
                ribbon.TrimById("Ribbon.EditingTools.CPEditTab.Layout");
                ribbon.TrimById("Ribbon.EditingTools.CPEditTab.Styles");
                ribbon.TrimById("Ribbon.EditingTools.CPEditTab.EditAndCheckout");
            }

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _path = Request.QueryString["Path"] ?? String.Empty;
            CreateNew = ((Request.QueryString["Mode"] ?? "1") == "1") ? true : false;
            lblMMSPath.Text = _path;
            rfv_CurrentTagName.Enabled = !CreateNew;

            if (!CreateNew && !IsPostBack)
            {
                string currentTagName = GetCurrentTagName();
                lblMMSPath.Text = _path.Replace(currentTagName, string.Empty);
                txtEdit_CurrentTagName.Text = currentTagName;
            }

            if (!IsPostBack)
            {
                BindForm();
            }
        }

        /// <summary>
        /// Initialise the form
        /// </summary>
        void BindForm()
        {
            if (String.IsNullOrEmpty(_path)) return;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite elevatedSite = new SPSite(SPContext.Current.Site.ID))
                {
                    WBTaxonomy subjectTags = WBTaxonomy.GetSubjectTags(elevatedSite);

                    Term rootSubjectTagTerm = subjectTags.GetSelectedTermByPath(_path);
                    WBSubjectTag subjectTag = null;

                    if (rootSubjectTagTerm != null)
                    {
                        subjectTag = new WBSubjectTag(subjectTags, rootSubjectTagTerm);
                        if (subjectTag == null)
                            return;
                    }

                    // DOes the current user have permission?
                    WBTaxonomy teamsTax = WBTaxonomy.GetTeams(elevatedSite);
                    if (!subjectTag.TeamsWithPermissionToEdit(teamsTax).WBxContainsCurrentUserAsTeamMember())
                    {
                        Response.Redirect("AccessDenied.html");
                    }

                    if (!CreateNew) // Edit Mode
                    {
                        txtTagName.Text = subjectTag.Name;
                        txtTagName.ReadOnly = true;

                        // To support existing terms where content is stored in the description, first attempt to read from the the multi-property array Page Content,
                        // then fall back to the description. Changes will be saved to the multi-property array.
                        if (String.IsNullOrEmpty(subjectTag.PageContent))
                            htmlDescription.Field.Html = subjectTag.Description;
                        else
                            htmlDescription.Field.Html = subjectTag.PageContent;

                        ppInternalContact.WBxInitialise(subjectTag.InternalContact(SPContext.Current.Web)); // It's an option to use the RootWeb of the elevated site here, I have used SPContext for consistency
                        htmlExternalContact.Field.Html = subjectTag.ExternalContact;
                    }
                    else
                    {
                        // Nothing to do if creating a new child term
                    }
                }
            });

        }

        /// <summary>
        /// This will decide whether to perform a create or an update.
        /// </summary>
        void CommitForm()
        {
            if (String.IsNullOrEmpty(_path)) return;

            if (CreateNew)
            {
                CreateNewTag();
            }
            else
            {
                UpdateTag();
            }
        }

        /// <summary>
        /// Creates a new Subject Tag
        /// </summary>
        void CreateNewTag()
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite elevatedSite = new SPSite(SPContext.Current.Site.ID))
                {
                    WBTaxonomy wbTax = WBTaxonomy.GetSubjectTags(elevatedSite);

                    _path = String.Concat(_path, "/", txtTagName.Text);

                    Term rootSubjectTagTerm = wbTax.GetSelectedTermByPath(_path); // Try and get the tag, but don't auto create
                    WBSubjectTag subjectTag = null;

                    if (rootSubjectTagTerm != null)
                    {
                        lblValidationMessage.Text = "The term you are trying to create already exists";
                    }
                    else
                    {
                        rootSubjectTagTerm = wbTax.GetSelectedTermByPath(_path, true); // Now create
                    }

                    if (rootSubjectTagTerm != null)
                    {
                        subjectTag = new WBSubjectTag(wbTax, rootSubjectTagTerm);
                        
                        if (subjectTag == null)
                            return;

                        subjectTag.PageContent = htmlDescription.Html;
                        if (htmlExternalContact.Html.ToLower() != "<div>&#160;</div>")
                        {
                            subjectTag.ExternalContact = htmlExternalContact.Html;
                        }
                        SPUser pickedUser = ppInternalContact.WBxGetSingleResolvedUser(elevatedSite.RootWeb);
                        if (pickedUser != null)
                        {
                            subjectTag.InternalContactLoginName = pickedUser.LoginName;
                        }
                        /*if (ppInternalContact.Entities != null && ppInternalContact.Entities.Count > 0)
                        {
                            PickerEntity pe = (PickerEntity)ppInternalContact.Entities[0];
                            subjectTag.InternalContactLoginName = pe.DisplayText;
                        }*/
                        wbTax.CommitAll();
                    }
                    else
                    {
                        lblValidationMessage.Text = "Your new tag could not be created, please contact support";
                    }
                }
            });
        }

        /// <summary>
        /// Updates and existing subject tag
        /// </summary>
        void UpdateTag()
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite elevatedSite = new SPSite(SPContext.Current.Site.ID))
                {
                    WBTaxonomy wbTax = WBTaxonomy.GetSubjectTags(elevatedSite);

                    Term rootSubjectTagTerm = wbTax.GetSelectedTermByPath(_path);
                    WBSubjectTag subjectTag = null;

                    if (rootSubjectTagTerm != null)
                    {
                        subjectTag = new WBSubjectTag(wbTax, rootSubjectTagTerm);
                        if (subjectTag == null)
                            return;
                    }

                    // Page content
                    subjectTag.PageContent = htmlDescription.Html;

                    // Internal Contact
                    SPUser pickedUser = ppInternalContact.WBxGetSingleResolvedUser(elevatedSite.RootWeb);
                    if (pickedUser != null)
                    {
                        subjectTag.InternalContactLoginName = pickedUser.LoginName;
                    }
                    else
                    {
                        subjectTag.InternalContactLoginName = string.Empty;
                    }

                    // External Contact
                    subjectTag.ExternalContact = htmlExternalContact.Html;

                    // Tag Name
                    subjectTag.Name = txtEdit_CurrentTagName.Text;

                    subjectTag.Update();
                }
            });
        }

        /// <summary>
        /// Event handler for Add button
        /// </summary>
        protected void addButton_OnClick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                CommitForm();

                // Return the new tag path, so the parent page can redirect if the tag name is updated.
                CloseDialogWithOK(GetNewMMSPathForWebPart());
            }
            else
            {
                lblValidationMessage.Text = "Please check form values";
            }
        }

        void InitHtmlEditors()
        {
            htmlExternalContact.Field = DefaultRichHtmlField();
            htmlExternalContact.Width = new System.Web.UI.WebControls.Unit(380);

            htmlDescription.Field = DefaultRichHtmlField();
            htmlDescription.Width = new System.Web.UI.WebControls.Unit(380);
        }

        Microsoft.SharePoint.Publishing.WebControls.RichHtmlField DefaultRichHtmlField()
        {
            var field = new Microsoft.SharePoint.Publishing.WebControls.RichHtmlField();
            field.ControlMode = SPControlMode.Edit;
            field.Html = "<div></div>";
            field.AllowFonts = true;
            field.AllowFontColorsMenu = false;
            field.MinimumEditHeight = "200px";
            field.EnableViewState = true;
            field.AllowReusableContent = false;

            return field;
        }

        /// <summary>
        /// Extracts the current tag name from the path
        /// </summary>
        string GetCurrentTagName()
        {
            if (String.IsNullOrEmpty(_path)) return string.Empty;

            string[] sa = _path.Split('/');
            return sa[sa.Length - 1];
        }

        /// <summary>
        /// Get the path to the tag, excluding the first part (for use in the ViewSubjectPages Web part)
        /// </summary>
        string GetNewMMSPathForWebPart()
        {
            if (String.IsNullOrEmpty(_path)) return string.Empty;

            string newPath = string.Empty;
            string[] sa = _path.Split('/');

            if (CreateNew)
            {
                newPath = _path.Replace(sa[0], string.Empty);
            }
            else
            {
                for (int i = 1; i <= sa.Length - 1; i++)
                {
                    if (i == sa.Length - 1)
                        newPath += "/" + txtEdit_CurrentTagName.Text;
                    else
                        newPath += "/" + sa[i];
                }
            }

            return newPath;
        }
    }
}
