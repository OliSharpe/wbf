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

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class SubjectTagsManagement : LayoutsPageBase
    {
        #region Private variables

        WBTaxonomy _subjectTags; 

        #endregion

        #region Page OnInit
        protected override void OnInit(EventArgs e)
        {
            tvAllSubjectTags.SelectedNodeChanged += new EventHandler(tvAllSubjectTags_SelectedNodeChanged);
            base.OnInit(e);
        } 
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            //SPSite site = SPContext.Current.Site;
            _subjectTags = WBTaxonomy.GetSubjectTags(SPContext.Current.Site);

            if (!IsPostBack)
            {
                TreeViewTermCollection collection = new TreeViewTermCollection(_subjectTags.TermSet);

                // Bind the data source to your collection
                tvAllSubjectTags.DataSource = collection;
                tvAllSubjectTags.DataBind();

                var teamsTax = WBTaxonomy.GetTeams(SPContext.Current.Site);

                teamsTax.InitialiseTaxonomyControl(taxTeams, "Select team with permissions to edit this term and create children", true, false, this);
            }
        } 
        #endregion


        #region Control Events

        void tvAllSubjectTags_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (tvAllSubjectTags.SelectedNode != null)
            {
                BindSubjectTagForm();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Save permission group changes - its the only thing editable at the moment.
            var selectedSubjectTag = _subjectTags.GetSelectedTermByPath(tvAllSubjectTags.SelectedNode.ValuePath);
            if (selectedSubjectTag != null)
            {
                WBSubjectTag subjectTag = null;

                subjectTag = new WBSubjectTag(_subjectTags, selectedSubjectTag);
                if (subjectTag == null)
                    return; // something better!

                subjectTag.TeamsWithPermissionToEditUIControlValue = taxTeams.Text;

                subjectTag.Update();

                lblPageMessage.Text = "Saved";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            
        }

        #endregion


        #region Methods

        void BindSubjectTagForm()
        {
            var selectedSubjectTag = _subjectTags.GetSelectedTermByPath(tvAllSubjectTags.SelectedNode.ValuePath);
            if (selectedSubjectTag != null)
            {
                WBSubjectTag subjectTag = null;

                subjectTag = new WBSubjectTag(_subjectTags, selectedSubjectTag);
                if (subjectTag == null)
                    return; // something better!

                if (String.IsNullOrEmpty(subjectTag.PageContent))
                {
                    litPageContent.Text = subjectTag.Description;
                }
                else
                {
                    litPageContent.Text = subjectTag.PageContent;
                }
                litInternalContact.Text = subjectTag.InternalContactLoginName;
                litExternalContact.Text = subjectTag.ExternalContact;
                taxTeams.Text = subjectTag.TeamsWithPermissionToEditUIControlValue;

            }
        }

        #endregion
    }
}
