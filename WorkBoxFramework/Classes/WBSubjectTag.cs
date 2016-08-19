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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;

namespace WorkBoxFramework
{
    public class WBSubjectTag : WBTerm
    {
        #region Constants

        public const string SUBJECT_TAG_TERM_PROPERTY__INTERNAL_CONTACT = "wbf__subject_tag__internal_contact";
        public const string SUBJECT_TAG_TERM_PROPERTY__EXTERNAL_CONTACT = "wbf__subject_tag__external_contact";
        public const string SUBJECT_TAG_TERM_PROPERTY__PAGE_CONTENT = "wbf__subject_tag__page_content";
        public const string SUBJECT_TAG_TERM_PROPERTY__POST_CODE = "wbf__subject_tag__post_code";
        public const string SUBJECT_TAG_TERM_PROPERTY__POST_CODES = "wbf__subject_tag__post_codes";
        public const string SUBJECT_TAG_TERM_PROPERTY__TEAMS = "wbf__subject_tag__teams";

        #endregion

        #region Constructors

        public WBSubjectTag() : base() { } 

        public WBSubjectTag(WBTaxonomy taxonomy, String UIControlValue)
            : base(taxonomy, UIControlValue)
        {
        }

        public WBSubjectTag(WBTaxonomy taxonomy, Term subjectTagTerm)
            : base(taxonomy, subjectTagTerm)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Internal contact name as a string
        /// </summary>
        public string InternalContactLoginName
        {
            get {return Term.WBxGetProperty(SUBJECT_TAG_TERM_PROPERTY__INTERNAL_CONTACT);}
            set { Term.WBxSetProperty(SUBJECT_TAG_TERM_PROPERTY__INTERNAL_CONTACT, value); }
        }

        /// <summary>
        /// External Contact (HTML string)
        /// </summary>
        public string ExternalContact
        {
            get { return SPEncode.HtmlDecode(Term.WBxGetBigProperty(SUBJECT_TAG_TERM_PROPERTY__EXTERNAL_CONTACT)); }
            set { Term.WBxSetBigProperty(SUBJECT_TAG_TERM_PROPERTY__EXTERNAL_CONTACT, SPEncode.HtmlEncode(value.WBxCleanForTermCustomProperty())); }
        }

        private SPUser _internalContact;

        /// <summary>
        /// Get the Internal Contact SPUser object
        /// </summary>
        public SPUser InternalContact(SPWeb web)
        {
            if (_internalContact == null)
            {
                _internalContact = web.WBxEnsureUserOrNull(InternalContactLoginName);
            }
            return _internalContact;
        }

        /// <summary>
        /// Page Content (Large HTML String)
        /// </summary>
        public string PageContent
        {
            get { return SPEncode.HtmlDecode(Term.WBxGetBigProperty(SUBJECT_TAG_TERM_PROPERTY__PAGE_CONTENT)); }
            set { Term.WBxSetBigProperty(SUBJECT_TAG_TERM_PROPERTY__PAGE_CONTENT, SPEncode.HtmlEncode(value.WBxCleanForTermCustomProperty())); }
        }

        /// <summary>
        /// Specific post code associated with this subject tag
        /// </summary>
        public string PostCode
        {
            get { return Term.WBxGetProperty(SUBJECT_TAG_TERM_PROPERTY__POST_CODE); }
            set { Term.WBxSetProperty(SUBJECT_TAG_TERM_PROPERTY__POST_CODE, value); }
        }

        /// <summary>
        /// Post codes associated with this subject tag
        /// </summary>
        public IEnumerable<string> PostCodes
        {
            get
            {
                var csv = Term.WBxGetProperty(SUBJECT_TAG_TERM_PROPERTY__POST_CODES);
                return csv.Split(',');
            }
            set
            {
                var csv = string.Join(",", value.Select(i => i.ToString()).ToArray());
                Term.WBxSetProperty(SUBJECT_TAG_TERM_PROPERTY__POST_CODE, csv);
            }
        }

        WBSubjectTag _parent = null;
        /// <summary>
        /// Parent subject tag
        /// </summary>
        public WBSubjectTag Parent
        {
            get
            {
                if (_parent == null)
                {
                    if (this.Term.Parent != null && this.Term.Parent.Name != "Subject Tags")
                    {
                        _parent = new WBSubjectTag(this.Taxonomy, this.Term.Parent);
                    }
                }
                return _parent;
            }
        }

        /// <summary>
        /// Does this Subject tag inherit permissions from the parent
        /// </summary>
        public bool IsInheritingPermissions
        {
            get
            {
                return String.IsNullOrEmpty(this.TeamsWithPermissionToEditUIControlValue);
            }
        }

        /// <summary>
        /// Teams with specific permissions on this tag
        /// </summary>
        public string TeamsWithPermissionToEditUIControlValue
        {
            get { return Term.WBxGetProperty(SUBJECT_TAG_TERM_PROPERTY__TEAMS); }
            set { Term.WBxSetProperty(SUBJECT_TAG_TERM_PROPERTY__TEAMS, value); }
        }

        /// <summary>
        /// Teams from parent terms that have permissions to edit this tag
        /// </summary>
        public String InheritedTeamsWithPermissionToEditUIControlValue
        {
            get
            {
                // First get the collection of teams set directly on this term
                WBTermCollection<WBTeam> teams = new WBTermCollection<WBTeam>(null, TeamsWithPermissionToEditUIControlValue);

                // Then we’ll add all of the teams set with permission to edit higher up the subject tags hierarchy
                if (Parent != null)
                {
                    teams.Add(new WBTermCollection<WBTeam>(null, Parent.InheritedTeamsWithPermissionToEditUIControlValue));
                }

                return teams.UIControlValue;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collection of WBTeam that have permissions to edit this tag (and create children)
        /// </summary>
        /// <param name="teamsTaxonomy">Teams Taxonomy</param>
        public WBTermCollection<WBTeam> TeamsWithPermissionToEdit(WBTaxonomy teamsTaxonomy)
        {
            return new WBTermCollection<WBTeam>(teamsTaxonomy, InheritedTeamsWithPermissionToEditUIControlValue);
        }

        #endregion
    }

    /// <summary>
    /// Extension methods specific to SubjectTags
    /// </summary>
    public static class SubjectTagExtensions
    {
        // TODO: Move to WBExtensions.cs ?

        /// <summary>
        /// Removes \t and \n characters from a string, in preparation for storing in a Term custom property (SP RTE has a habit of putting them in!)
        /// </summary>
        public static string WBxCleanForTermCustomProperty(this string str)
        {
            return str.Replace("\t", "").Replace("\n", "");
        }

        /// <summary>
        /// Checks if the HTML string contains any real content (excluding html elements)
        /// </summary>
        /// <param name="html">Html string</param>
        public static bool WBxIsHtmlFieldEmpty(this string html)
        {
            // SC: This is not an ideal solution, but will work fine to simply tell if the RTE is empty
            var cleaned = Regex.Replace(html, "<.*?>", string.Empty).Replace("&#160;", "").WBxTrim();
            return (String.IsNullOrEmpty(cleaned));
        }
    }
}
