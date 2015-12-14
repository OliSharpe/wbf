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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class WBRecordsType : WBTerm
    {

        #region Constants

        private const string RECORDS_TYPE_TERM_PROPERTY__DEFAULT_FUNCTIONAL_AREA = "wbf__records_type__default_functional_area";
        private const string RECORDS_TYPE_TERM_PROPERTY__ALLOW_OTHER_FUNCTIONAL_AREAS = "wbf__records_type__allow_other_functional_areas";


        // Work Box Record Properties:
        private const string RECORDS_TYPE_TERM_PROPERTY__ALLOW_WORK_BOX_RECORDS = "wbf__records_type__allow_work_box_records";
        private const string RECORDS_TYPE_TERM_PROPERTY__WHO_CAN_CREATE_NEW_WORK_BOXES = "wbf__records_type__who_can_create_new_work_boxes";
        private const string RECORDS_TYPE_TERM_PROPERTY__CREATE_NEW_WORK_BOX_TEXT = "wbf__records_type__create_new_work_box_text";
        
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_COLLECTION_URL = "wbf__records_type__wbc_url";

        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_UNIQUE_ID_PREFIX = "wbf__records_type__work_box_unique_id_prefix";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_LOCAL_ID_SOURCE = "wbf__records_type__work_box_local_id_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_GENERATED_LOCAL_ID_OFFSET = "wbf__records_type__work_box_generated_local_id_offset";

        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_REQUIREMENT = "wbf__records_type__work_box_short_title_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_SOURCE = "wbf__records_type__work_box_short_title_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_DESCRIPTION = "wbf__records_type__work_box_short_title_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_REQUIREMENT = "wbf__records_type__work_box_subject_tags_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_SOURCE = "wbf__records_type__work_box_subject_tags_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_DESCRIPTION = "wbf__records_type__work_box_subject_tags_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_REQUIREMENT = "wbf__records_type__work_box_reference_id_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_SOURCE = "wbf__records_type__work_box_reference_id_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_DESCRIPTION = "wbf__records_type__work_box_reference_id_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_REQUIREMENT = "wbf__records_type__work_box_reference_date_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_SOURCE = "wbf__records_type__work_box_reference_date_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_DESCRIPTION = "wbf__records_type__work_box_reference_date_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_REQUIREMENT = "wbf__records_type__work_box_series_tag_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_SOURCE = "wbf__records_type__work_box_series_tag_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_PARENT_TERM = "wbf__records_type__work_box_series_tag_parent_term";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_ALLOW_NEW_TERMS = "wbf__records_type__work_box_series_tag_allow_new_terms";
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_DESCRIPTION = "wbf__records_type__work_box_series_tag_description";
        
        private const string RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_NAMING_CONVENTION = "wbf__records_type__work_box_naming_convention";

        private const string RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TRIGGER_DATE = "wbf__records_type__auto_close_trigger_date";
        private const string RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_SCALAR = "wbf__records_type__auto_close_time_scalar";
        private const string RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_UNIT = "wbf__records_type__auto_close_time_unit";

        private const string RECORDS_TYPE_TERM_PROPERTY__RETENTION_TRIGGER_DATE = "wbf__records_type__retention_trigger_date";
        private const string RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_SCALAR = "wbf__records_type__retention_time_scalar";
        private const string RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_UNIT = "wbf__records_type__retention_time_unit";

        private const string RECORDS_TYPE_TERM_PROPERTY__ALLOW_PUBLISHING_OUT = "wbf__records_type__allow_publishing_out";
        private const string RECORDS_TYPE_TERM_PROPERTY__MINIMUM_PUBLISHING_OUT_PROTECTIVE_ZONE = "wbf__records_type__minimum_publishing_out_protective_zone";

        private const string RECORDS_TYPE_TERM_PROPERTY__GENERATE_PUBLISH_OUT_FILENAMES = "wbf__records_type__generate_publish_out_filenames";

        private const string RECORDS_TYPE_TERM_PROPERTY__USE_DEFAULTS_WHEN_PUBLISHING_OUT = "wbf__records_type__use_defaults_when_publishing_out";
        private const string RECORDS_TYPE_TERM_PROPERTY__DEFAULT_PUBLISHING_OUT_RECORDS_TYPE = "wbf__records_type__default_publishing_out_records_type";

        private const string RECORDS_TYPE_TERM_PROPERTY__CACHE_DETAILS_FOR_OPEN_WORK_BOXES = "wbf__records_type__cache_details_for_open_work_boxes";

        // Document Record Properties:
        private const string RECORDS_TYPE_TERM_PROPERTY__ALLOW_DOCUMENT_RECORDS = "wbf__records_type__allow_document_records";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_MINIMUM_PROTECTIVE_ZONE = "wbf__records_type__document_minimum_protective_zone";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_REQUIREMENT = "wbf__records_type__document_short_title_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_SOURCE = "wbf__records_type__document_short_title_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_DESCRIPTION = "wbf__records_type__document_short_title_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_REQUIREMENT = "wbf__records_type__document_subject_tags_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_SOURCE = "wbf__records_type__document_subject_tags_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_DESCRIPTION = "wbf__records_type__document_subject_tags_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_REQUIREMENT = "wbf__records_type__document_reference_id_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_SOURCE = "wbf__records_type__document_reference_id_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_DESCRIPTION = "wbf__records_type__document_reference_id_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_REQUIREMENT = "wbf__records_type__document_reference_date_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_SOURCE = "wbf__records_type__document_reference_date_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_DESCRIPTION = "wbf__records_type__document_reference_date_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_REQUIREMENT = "wbf__records_type__document_series_tag_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_SOURCE = "wbf__records_type__document_series_tag_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_PARENT_TERM = "wbf__records_type__document_series_tag_parent_term";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_ALLOW_NEW_TERMS = "wbf__records_type__document_series_tag_allow_new_terms";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_DESCRIPTION = "wbf__records_type__document_series_tag_description";

        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_REQUIREMENT = "wbf__records_type__document_scan_date_requirement";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_SOURCE = "wbf__records_type__document_scan_date_source";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_DESCRIPTION = "wbf__records_type__document_scan_date_description";



        private const string RECORDS_TYPE_TERM_PROPERTY__ENFORCE_DOCUMENT_NAMING_CONVENTION = "wbf__records_type__enforce_document_naming_convention";
        private const string RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_NAMING_CONVENTION = "wbf__records_type__document_naming_convention";

        private const string RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_1 = "wbf__records_type__filing_rule_level_1";
        private const string RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_2 = "wbf__records_type__filing_rule_level_2";
        private const string RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_3 = "wbf__records_type__filing_rule_level_3";
        private const string RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_4 = "wbf__records_type__filing_rule_level_4";

        public const string AUTO_CLOSE_TRIGGER_DATE__NONE = "";
        public const string AUTO_CLOSE_TRIGGER_DATE__LAST_MODIFIED_DATE = "Last Modified Date";
        public const string AUTO_CLOSE_TRIGGER_DATE__REFERENCE_DATE = "Reference Date";
        public const string AUTO_CLOSE_TRIGGER_DATE__DATE_CREATED = "Date Created";
        public const string AUTO_CLOSE_TRIGGER_DATE__END_FINANCIAL_LAST_MODIFIED_DATE = "End of financial year containing Last Modified Date";
        public const string AUTO_CLOSE_TRIGGER_DATE__END_FINANCIAL_REFERENCE_DATE = "End of financial year containing Reference Date";

        public const string AUTO_CLOSE_TIME_UNIT__NONE = "";
        public const string AUTO_CLOSE_TIME_UNIT__YEARS = "Years";
        public const string AUTO_CLOSE_TIME_UNIT__MONTHS = "Months";
        public const string AUTO_CLOSE_TIME_UNIT__DAYS = "Days";
        public const string AUTO_CLOSE_TIME_UNIT__HOURS = "Hours";
        public const string AUTO_CLOSE_TIME_UNIT__MINUTES = "Minutes";

        public const string RETENTION_TRIGGER_DATE__NONE = "";
        public const string RETENTION_TRIGGER_DATE__LAST_CLOSED_DATE = "Last Closed Date";
        public const string RETENTION_TRIGGER_DATE__REFERENCE_DATE = "Reference Date";
        public const string RETENTION_TRIGGER_DATE__END_FINANCIAL_LAST_CLOSED_DATE = "End of financial year containing Last Closed Date";
        public const string RETENTION_TRIGGER_DATE__END_FINANCIAL_REFERENCE_DATE = "End of financial year containing Reference Date";

        public const string RETENTION_TIME_UNIT__PERMANENT = "Permanent";
        public const string RETENTION_TIME_UNIT__YEARS = "Years";
        public const string RETENTION_TIME_UNIT__MONTHS = "Months";
        public const string RETENTION_TIME_UNIT__DAYS = "Days";
        public const string RETENTION_TIME_UNIT__HOURS = "Hours";
        public const string RETENTION_TIME_UNIT__MINUTES = "Minutes";

        public const string LOCAL_ID_SOURCE__GENERATE_LOCAL_ID = "Generate Local ID";
        public const string LOCAL_ID_SOURCE__USE_REFERENCE_ID = "Use Reference ID";
        public const string LOCAL_ID_SOURCE__USE_CURRENT_USER_LOGIN_NAME = "Use Current User Login Name";

        public const int NUMBER_OF_DIGITS_IN_GENERATED_LOCAL_IDS = 6;

        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_LOCALID_TITLE_OLD = "(Unique ID Prefix) (Local ID) - (Short Title)";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_REFERENCEID_TITLE_OLD = "(Unique ID Prefix) (Reference ID) - (Short Title)";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_DATE_OLD = "(Unique ID Prefix) (YYYY-MM-DD)";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_DATE_TITLE_OLD = "(Unique ID Prefix) (YYYY-MM-DD) - (Short Title)";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_DATE_REFERENCEID_TITLE_OLD = "(Unique ID Prefix) (YYYY-MM-DD) (Reference ID) - (Short Title)";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_SERIES_REFERENCEID_TITLE_OLD = "(UID Prefix) (Series Tag) (Reference ID) - (Short Title)";

        public const string WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_TITLE = "<Team Achronym> <Unique ID Prefix> - <Short Title>";
        public const string WORK_BOX_NAMING_CONVENTION__TEAM_TITLE = "<Team Achronym> <Short Title>";
        public const string WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_OPTIONAL_SERIES_DATE = "<Team Achronym> <Unique ID Prefix> [<Series>] (YYYY-MM-DD)";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_TITLE = "<Unique ID Prefix> - <Short Title>";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_LOCALID_TITLE = "<Unique ID Prefix> <Local ID> - <Short Title>";
        public const string WORK_BOX_NAMING_CONVENTION__PREFIX_REFERENCEID_TITLE = "<Unique ID Prefix> <Reference ID> - <Short Title>";


        public const string PROTECTIVE_ZONE__PROTECTED = "Protected";
        public const string PROTECTIVE_ZONE__UNPROTECTED = "Unprotected";
        public const string PROTECTIVE_ZONE__PUBLIC_EXTRANET = "Public Extranet";
        public const string PROTECTIVE_ZONE__PUBLIC = "Public";


        public const string METADATA_REQUIREMENT__REQUIRED = "Required";
        public const string METADATA_REQUIREMENT__OPTIONAL = "Optional";
        public const string METADATA_REQUIREMENT__HIDDEN = "Hidden";
        public const string METADATA_REQUIREMENT__DEFAULT = "Hidden";



        public const string DOCUMENT_REFERENCE_ID_SOURCE__NONE = "";
        public const string DOCUMENT_REFERENCE_ID_SOURCE__MANUAL_ENTRY = "Manual Entry";
        public const string DOCUMENT_REFERENCE_ID_SOURCE__WORK_BOX_REFERENCE_ID = "Work Box Reference ID";
        
        public const string DOCUMENT_REFERENCE_DATE_SOURCE__MANUAL_ENTRY = "Manual Entry";
        public const string DOCUMENT_REFERENCE_DATE_SOURCE__PUBLISH_OUT_DATE = "Publish Out Date";
        public const string DOCUMENT_REFERENCE_DATE_SOURCE__WORK_BOX_REFERENCE_DATE = "Work Box Reference Date";

        public const string DOCUMENT_SERIES_TAG_SOURCE__NONE = "";
        public const string DOCUMENT_SERIES_TAG_SOURCE__MANUAL_ENTRY = "Manual Entry";
        public const string DOCUMENT_SERIES_TAG_SOURCE__WORK_BOX_SERIES_TAG = "Work Box Series Tag";

        public const string DOCUMENT_NAMING_CONVENTION__NONE = "";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_TITLE = "(YYYY-MM-DD) <Title>";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_SERIES = "(YYYY-MM-DD) <Series Tag>";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_SERIES_TITLE = "(YYYY-MM-DD) <Series Tag> - <Title>";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_SERIES_REFERENCE_ID = "(YYYY-MM-DD) <Series Tag> - <Reference ID>";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_SERIES_REFID_TITLE = "(YYYY-MM-DD) <Series Tag> - <Ref ID> - <Title>";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_WORK_BOX_ID_TITLE = "(YYYY-MM-DD) (Work Box ID) <Title>";
        public const string DOCUMENT_NAMING_CONVENTION__DATE_REFERENCE_ID_TITLE = "(YYYY-MM-DD) <Reference ID> - <Title>";
        public const string DOCUMENT_NAMING_CONVENTION__WORK_BOX_ID_TITLE = "(<Work Box ID>) <Title>";

        public const string FILING_RULE__NONE = "";
        public const string FILING_RULE__BY_FINANCIAL_YEAR = "By Financial Year (YYYY-YYYY)";
        public const string FILING_RULE__BY_CALENDAR_YEAR = "By Calendar Year (YYYY)";
        public const string FILING_RULE__BY_MONTH = "By Month (MM)";
        public const string FILING_RULE__BY_DAY_OF_MONTH = "By Day of Month (DD)";
        public const string FILING_RULE__BY_FULL_DATE = "By Full Date (YYYY-MM-DD)";
        public const string FILING_RULE__BY_FUNCTIONAL_AREA = "By Functional Area";
        public const string FILING_RULE__BY_REFERENCE_ID = "By Reference ID";
        public const string FILING_RULE__BY_SERIES_TAG = "By Series Tag";
        public const string FILING_RULE__BY_OWNING_TEAM = "By Owning Team";
        public const string FILING_RULE__BY_WORK_BOX_ID = "By Work Box ID";

        public const string WHO_CAN_CREATE_NEW_WORK_BOXES__ANYONE = "Anyone";
        public const string WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_MEMBERS = "Team Members";
        public const string WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_OWNERS = "Team Owners";
        public const string WHO_CAN_CREATE_NEW_WORK_BOXES__WORK_BOX_COLLECTION_ADMINISTRATORS = "Work Box Collection Administrators";
        public const string WHO_CAN_CREATE_NEW_WORK_BOXES__SYSTEM_ONLY = "System Only";

        private const string DEFAULT__WORK_BOX_SHORT_TITLE_DESCRIPTION = "Give a short, meaningful title.";
        private const string DEFAULT__WORK_BOX_SUBJECT_TAGS_DESCRIPTION = "Select subject tags for this work box.";
        private const string DEFAULT__WORK_BOX_REFERENCE_ID_DESCRIPTION = "The ID of the thing that this work box relates to.";
        private const string DEFAULT__WORK_BOX_REFERENCE_DATE_DESCRIPTION = "The key date of the thing that this work box relates to.";
        private const string DEFAULT__WORK_BOX_SERIES_TAG_DESCRIPTION = "The name of the series of records that this work box belongs to.";

        private const string DEFAULT__DOCUMENT_SHORT_TITLE_DESCRIPTION = "A short title to be displayed as the name of the document.";
        private const string DEFAULT__DOCUMENT_SUBJECT_TAGS_DESCRIPTION = "Select subject tags for this document.";
        private const string DEFAULT__DOCUMENT_REFERENCE_ID_DESCRIPTION = "The ID of the thing that this document relates to.";
        private const string DEFAULT__DOCUMENT_REFERENCE_DATE_DESCRIPTION = "The key date of the thing that this document relates to.";
        private const string DEFAULT__DOCUMENT_SERIES_TAG_DESCRIPTION = "The name of the series of records that this document belongs to.";
        private const string DEFAULT__DOCUMENT_SCAN_DATE_DESCRIPTION = "The date on which this document was scanned.";

        private const string DEFAULT__CREATE_NEW_WORK_BOX_TEXT = "Create New Work Box";


        public const int YEAR_REPRESENTING_A_PERMANENT_DATE = 3000;

        public const string RECORDS_LIBRARY__CLASS_FOLDER_CONTENT_TYPE = "Compliance Extender Class";
        public const string RECORDS_LIBRARY__FILE_PART_FOLDER_CONTENT_TYPE = "Compliance Extender Folder";

        public const string RECORDS_LIBRARY__FALL_BACK_FOLDER_CONTENT_TYPE = "Folder";

        #endregion


        #region Constructors

        public WBRecordsType() : base() {} 

        public WBRecordsType(WBTaxonomy taxonomy, String UIControlValue)
            : base(taxonomy, UIControlValue)
        {
        }

        public WBRecordsType(WBTaxonomy taxonomy, Term term)
            : base(taxonomy, term)
        {
        }
        #endregion


        #region Properties

        private WBRecordsType _parent = null;
        public WBRecordsType Parent
        {
            get
            {
                if (_parent == null)
                {
                    Term parentTerm = Term.Parent;
                    if (parentTerm != null)
                    {
                        _parent = new WBRecordsType(Taxonomy, parentTerm);
                    }
                }
                return _parent;
            }
        }

        public String DefaultFunctionalAreaUIControlValue
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DEFAULT_FUNCTIONAL_AREA); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DEFAULT_FUNCTIONAL_AREA, value); }
        }

        public bool AllowOtherFunctionalAreas
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__ALLOW_OTHER_FUNCTIONAL_AREAS, false); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_OTHER_FUNCTIONAL_AREAS, value); }
        }

        public bool IsFunctionalAreaEditable
        {
            get { return ((DefaultFunctionalAreaUIControlValue == "") || AllowOtherFunctionalAreas); } 
        }

        public bool AllowWorkBoxRecords
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__ALLOW_WORK_BOX_RECORDS, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_WORK_BOX_RECORDS, value); }
        }

        public String WhoCanCreateNewWorkBoxes
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WHO_CAN_CREATE_NEW_WORK_BOXES, WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_OWNERS); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WHO_CAN_CREATE_NEW_WORK_BOXES, value); }
        }

        public String CreateNewWorkBoxText
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__CREATE_NEW_WORK_BOX_TEXT, DEFAULT__CREATE_NEW_WORK_BOX_TEXT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__CREATE_NEW_WORK_BOX_TEXT, value); }
        }

        
        // This is here as an accessor for the actual property value for editing purposes:
        public String WorkBoxCollectionUrlProperty
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_COLLECTION_URL); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_COLLECTION_URL, value); }
        }
        /// <summary>
        /// Then the following property on WBRecordsType is the one that is actually used to find the Url for the 
        /// work box collection that is associated with this reocrds type. It traverses
        /// back up the term hierarchy to find the first term that has a value set and uses that value:
        /// </summary>
        public String WorkBoxCollectionUrl
        {
            get {
                string workBoxCollectionUrl = WorkBoxCollectionUrlProperty;
                if (workBoxCollectionUrl == "" && Parent != null) 
                {
                    workBoxCollectionUrl = Parent.WorkBoxCollectionUrl;
                }
                return workBoxCollectionUrl; 
            }
            set { WorkBoxCollectionUrlProperty = value; }
        }

        public String WorkBoxUniqueIDPrefix
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_UNIQUE_ID_PREFIX); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_UNIQUE_ID_PREFIX, value); }
        }

        public String WorkBoxLocalIDSource
        {
            get 
            {
                string value = Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_LOCAL_ID_SOURCE);
                if (value == "") return LOCAL_ID_SOURCE__GENERATE_LOCAL_ID;
                return value;
            }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_LOCAL_ID_SOURCE, value); }
        }

        public int WorkBoxGeneratedLocalIDOffset
        {
            get { return Term.WBxGetIntProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_GENERATED_LOCAL_ID_OFFSET); }
            set { Term.WBxSetIntProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_GENERATED_LOCAL_ID_OFFSET, value); }
        }

       
        public bool IsWorkBoxShortTitleRequired { get { return WorkBoxShortTitleRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String WorkBoxShortTitleRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_REQUIREMENT, METADATA_REQUIREMENT__REQUIRED); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_REQUIREMENT, value); }
        }

        public String WorkBoxShortTitleDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_DESCRIPTION, DEFAULT__WORK_BOX_SHORT_TITLE_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SHORT_TITLE_DESCRIPTION, value); }
        }


        public bool IsWorkBoxSubjectTagsRequired { get { return WorkBoxSubjectTagsRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String WorkBoxSubjectTagsRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_REQUIREMENT, value); }
        }

        public String WorkBoxSubjectTagsDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_DESCRIPTION, DEFAULT__WORK_BOX_SUBJECT_TAGS_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SUBJECT_TAGS_DESCRIPTION, value); }
        }


        public bool IsWorkBoxReferenceIDRequired { get { return WorkBoxReferenceIDRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String WorkBoxReferenceIDRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_REQUIREMENT, value); }
        }
/*
        public String WorkBoxReferenceIDSource
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_SOURCE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_SOURCE, value); }
        }
*/
        public String WorkBoxReferenceIDDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_DESCRIPTION, DEFAULT__WORK_BOX_REFERENCE_ID_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_DESCRIPTION, value); }
        }

        public bool IsWorkBoxReferenceDateRequired { get { return WorkBoxReferenceDateRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String WorkBoxReferenceDateRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_REQUIREMENT, value); }
        }
/*
        public String WorkBoxReferenceDateSource
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_SOURCE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_SOURCE, value); }
        }
*/
        public String WorkBoxReferenceDateDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_DESCRIPTION, DEFAULT__WORK_BOX_REFERENCE_DATE_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_DESCRIPTION, value); }
        }

        public bool IsWorkBoxSeriesTagRequired { get { return WorkBoxSeriesTagRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String WorkBoxSeriesTagRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_REQUIREMENT, value); }
        }
/*
        public String WorkBoxSeriesTagSource
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_SOURCE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_SOURCE, value); }
        }
*/
        public String WorkBoxSeriesTagParentTermUIControlValue
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_PARENT_TERM); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_PARENT_TERM, value); }
        }

        public bool WorkBoxSeriesTagAllowNewTerms
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_ALLOW_NEW_TERMS, false); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_ALLOW_NEW_TERMS, value); }
        }

        public String WorkBoxSeriesTagDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_DESCRIPTION, DEFAULT__WORK_BOX_SERIES_TAG_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_DESCRIPTION, value); }
        }


        public String WorkBoxNamingConvention
        {
            get {

                String savedNamingConvention = Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_NAMING_CONVENTION, WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_TITLE);

                if (savedNamingConvention.Equals(WORK_BOX_NAMING_CONVENTION__PREFIX_LOCALID_TITLE_OLD))
                {
                    savedNamingConvention = WORK_BOX_NAMING_CONVENTION__PREFIX_LOCALID_TITLE;
                }

                if (savedNamingConvention.Equals(WORK_BOX_NAMING_CONVENTION__PREFIX_REFERENCEID_TITLE_OLD))
                {
                    savedNamingConvention = WORK_BOX_NAMING_CONVENTION__PREFIX_REFERENCEID_TITLE;
                }


                if (!WBRecordsType.getWorkBoxNamingConventions().Contains(savedNamingConvention))
                {
                    savedNamingConvention = WORK_BOX_NAMING_CONVENTION__PREFIX_TITLE;
                }

                return savedNamingConvention; 
            
            } 
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_NAMING_CONVENTION, value); }
        }

        public String AutoCloseTriggerDate
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TRIGGER_DATE, AUTO_CLOSE_TRIGGER_DATE__NONE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TRIGGER_DATE, value); }
        }

        public String AutoCloseTimeUnit
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_UNIT, AUTO_CLOSE_TIME_UNIT__NONE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_UNIT, value); }
        }

        public int AutoCloseTimeScalar
        {
            get { return Term.WBxGetIntProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_SCALAR); }
            set { Term.WBxSetIntProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_SCALAR, value); }
        }

        public String AutoCloseTimeScalarAsString
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_SCALAR); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_SCALAR, value); }
        }


        public String RetentionTriggerDate
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TRIGGER_DATE, RETENTION_TRIGGER_DATE__NONE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TRIGGER_DATE, value); }
        }

        public String RetentionTimeUnit
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_UNIT, RETENTION_TIME_UNIT__PERMANENT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_UNIT, value); }
        }

        public int RetentionTimeScalar
        {
            get { return Term.WBxGetIntProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_SCALAR); }
            set { Term.WBxSetIntProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_SCALAR, value); }
        }

        public String RetentionTimeScalarAsString
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_SCALAR); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_SCALAR, value); }
        }

        public bool AllowPublishingOut
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__ALLOW_PUBLISHING_OUT, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_PUBLISHING_OUT, value); }
        }

        public String MinimumPublishingOutProtectiveZone
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__MINIMUM_PUBLISHING_OUT_PROTECTIVE_ZONE, PROTECTIVE_ZONE__PROTECTED); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__MINIMUM_PUBLISHING_OUT_PROTECTIVE_ZONE, value); }
        }

        public bool GeneratePublishOutFilenames
        {
            get { return true; } // return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__GENERATE_PUBLISH_OUT_FILENAMES, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__GENERATE_PUBLISH_OUT_FILENAMES, value); }
        }

            
        public bool UseDefaultsWhenPublishingOut
        {
            get { return true; } // return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__USE_DEFAULTS_WHEN_PUBLISHING_OUT, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__USE_DEFAULTS_WHEN_PUBLISHING_OUT, value); }
        }

        public String DefaultPublishingOutRecordsTypeUIControlValue
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DEFAULT_PUBLISHING_OUT_RECORDS_TYPE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DEFAULT_PUBLISHING_OUT_RECORDS_TYPE, value); }
        }

        public WBRecordsType DefaultPublishingOutRecordsType
        {
            get 
            {
                // If the default value is not explictly set then assume we use this same type for publishing out.
                if (DefaultPublishingOutRecordsTypeUIControlValue == "") return this;
                return new WBRecordsType(Taxonomy, DefaultPublishingOutRecordsTypeUIControlValue);
            }

            set { DefaultPublishingOutRecordsTypeUIControlValue = value.UIControlValue; }
        }


        public bool CacheDetailsForOpenWorkBoxes
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__CACHE_DETAILS_FOR_OPEN_WORK_BOXES, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__CACHE_DETAILS_FOR_OPEN_WORK_BOXES, value); }
        }
       

        public bool AllowDocumentRecords
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__ALLOW_DOCUMENT_RECORDS, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_DOCUMENT_RECORDS, value); }
        }

        public String DocumentMinimumProtectiveZone
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_MINIMUM_PROTECTIVE_ZONE, PROTECTIVE_ZONE__PROTECTED); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_MINIMUM_PROTECTIVE_ZONE, value); }
        }

        public bool IsDocumentShortTitleRequired { get { return DocumentShortTitleRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String DocumentShortTitleRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_REQUIREMENT, METADATA_REQUIREMENT__REQUIRED); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_REQUIREMENT, value); }
        }

        public String DocumentShortTitleDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_DESCRIPTION, DEFAULT__DOCUMENT_SHORT_TITLE_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SHORT_TITLE_DESCRIPTION, value); }
        }


        public bool IsDocumentSubjectTagsRequired { get { return DocumentSubjectTagsRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String DocumentSubjectTagsRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_REQUIREMENT, METADATA_REQUIREMENT__OPTIONAL); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_REQUIREMENT, value); }
        }

        public String DocumentSubjectTagsDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_DESCRIPTION, DEFAULT__DOCUMENT_SUBJECT_TAGS_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SUBJECT_TAGS_DESCRIPTION, value); }
        }



        public bool IsDocumentReferenceIDRequired { get { return DocumentReferenceIDRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String DocumentReferenceIDRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_REQUIREMENT, value); }
        }
/*
        public String DocumentReferenceIDSource
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_SOURCE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_SOURCE, value); }
        }
*/
        public String DocumentReferenceIDDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_DESCRIPTION, DEFAULT__DOCUMENT_REFERENCE_ID_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_DESCRIPTION, value); }
        }

        public bool IsDocumentReferenceDateRequired { get { return DocumentReferenceDateRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String DocumentReferenceDateRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_REQUIREMENT, value); }
        }

        public String DocumentReferenceDateSource
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_SOURCE, DOCUMENT_REFERENCE_DATE_SOURCE__PUBLISH_OUT_DATE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_SOURCE, value); }
        }

        public String DocumentReferenceDateDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_DESCRIPTION, DEFAULT__DOCUMENT_REFERENCE_DATE_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_DESCRIPTION, value); }
        }

        public bool IsDocumentSeriesTagRequired { get { return DocumentSeriesTagRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String DocumentSeriesTagRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_REQUIREMENT, value); }
        }
/*
        public String DocumentSeriesTagSource
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_SOURCE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_SOURCE, value); }
        }
*/
        public String DocumentSeriesTagParentTermUIControlValue
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_PARENT_TERM); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_PARENT_TERM, value); }
        }

        public bool DocumentSeriesTagAllowNewTerms
        {
            get { return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_ALLOW_NEW_TERMS, false); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_ALLOW_NEW_TERMS, value); }
        }

        public String DocumentSeriesTagDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_DESCRIPTION, DEFAULT__DOCUMENT_SERIES_TAG_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_DESCRIPTION, value); }
        }

        public bool IsDocumentScanDateRequired { get { return DocumentScanDateRequirement == METADATA_REQUIREMENT__REQUIRED; } }
        public String DocumentScanDateRequirement
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_REQUIREMENT, METADATA_REQUIREMENT__DEFAULT); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_REQUIREMENT, value); }
        }
/*
        public String DocumentScanDateSource
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_SOURCE); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_SOURCE, value); }
        }
*/
        public String DocumentScanDateDescription
        {
            get { return Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_DESCRIPTION, DEFAULT__DOCUMENT_SCAN_DATE_DESCRIPTION); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_DESCRIPTION, value); }
        }



        public String DocumentNamingConvention
        {
            get {
                String savedNamingConvention = Term.WBxGetPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_NAMING_CONVENTION, DOCUMENT_NAMING_CONVENTION__DATE_TITLE);             

                if (!WBRecordsType.getDocumentNamingConventions().Contains(savedNamingConvention))
                {
                    savedNamingConvention = DOCUMENT_NAMING_CONVENTION__DATE_TITLE;
                }

                WBLogging.RecordsTypes.Unexpected("All documents are being forced to have naming convention: " + DOCUMENT_NAMING_CONVENTION__DATE_TITLE);
                return savedNamingConvention;
            }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_NAMING_CONVENTION, value); }
        }

        public bool EnforceDocumentNamingConvention
        {
            get { return true; } // return Term.WBxGetBoolPropertyOrDefault(RECORDS_TYPE_TERM_PROPERTY__ENFORCE_DOCUMENT_NAMING_CONVENTION, true); }
            set { Term.WBxSetBoolProperty(RECORDS_TYPE_TERM_PROPERTY__ENFORCE_DOCUMENT_NAMING_CONVENTION, value); }
        }


        public String FilingRuleLevel1
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_1); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_1, value); }
        }

        public String FilingRuleLevel2
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_2); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_2, value); }
        }

        public String FilingRuleLevel3
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_3); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_3, value); }
        }

        public String FilingRuleLevel4
        {
            get { return Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_4); }
            set { Term.WBxSetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_4, value); }
        }


    
        #endregion

        #region Methods
        public WBTerm WorkBoxSeriesTagParentTerm(WBTaxonomy seriesTags)
        {
            return new WBTerm(seriesTags, WorkBoxSeriesTagParentTermUIControlValue);
        }

        public WBTerm DocumentSeriesTagParentTerm(WBTaxonomy seriesTags)
        {
            return new WBTerm(seriesTags, DocumentSeriesTagParentTermUIControlValue);
        }

        public WBTermCollection<WBTerm> DefaultFunctionalArea(WBTaxonomy functionalAreas)
        {
            return new WBTermCollection<WBTerm>(functionalAreas, DefaultFunctionalAreaUIControlValue);
        }

        public String GenerateCorrectDocumentName(WorkBox workBox, SPListItem sourceDocAsItem)            
        {
            WBTaxonomy seriesTagsTaxonomy = WBTaxonomy.GetSeriesTags(workBox.RecordsTypes);

            string seriesTagName = "";
            if (sourceDocAsItem.WBxColumnHasValue(WorkBox.COLUMN_NAME__SERIES_TAG))
            {
                WBTerm seriesTag = sourceDocAsItem.WBxGetSingleTermColumn<WBTerm>(seriesTagsTaxonomy, WorkBox.COLUMN_NAME__SERIES_TAG);
                if (seriesTagName != null) seriesTagName = seriesTag.Name;
            }

            string filename = sourceDocAsItem.Name;

            string extension = Path.GetExtension(filename);
            string justName = Path.GetFileNameWithoutExtension(filename);

            string title = sourceDocAsItem.Title.WBxTrim();
            if (title == "")
            {
                title = justName;
                sourceDocAsItem["Title"] = title;
            }

            string referenceID = sourceDocAsItem.WBxGetColumnAsString(WorkBox.COLUMN_NAME__REFERENCE_ID);

            DateTime referenceDate = DateTime.Now;

            if (sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE] != null)
            {
                referenceDate = (DateTime)sourceDocAsItem[WorkBox.COLUMN_NAME__REFERENCE_DATE];
            }

            WBLogging.WorkBoxes.Verbose(string.Format("OK prepared all the bits: {0}, {1}, {2}, {3}, {4}",
                referenceDate,
                title,
                seriesTagName,
                workBox.UniqueID,
                referenceID));

            WBUtils.logMessage("Making a document name for " + this.Name + " using: " + DocumentNamingConvention);

            title = title.WBxTrim();
            string workBoxID = workBox.UniqueID.WBxTrim();
            referenceID = referenceID.WBxTrim();

            string referenceDateString = string.Format("{0}-{1}-{2}",
                            referenceDate.Year.ToString("D4"),
                            referenceDate.Month.ToString("D2"),
                            referenceDate.Day.ToString("D2"));

            WBUtils.logMessage("The referenceDateString: " + referenceDateString);

            if (referenceID == "") referenceID = "(Reference ID)";
            if (referenceDateString == "") referenceDateString = "(YYYY-MM-DD)";
            if (seriesTagName == "") seriesTagName = "(Series Tag)";

            switch (this.DocumentNamingConvention)
            {
                case DOCUMENT_NAMING_CONVENTION__DATE_TITLE: 
                    {
                        if (title == "") return "";
                        return string.Format("({0}) {1}",
                            referenceDateString,
                            title);
                    }

                case DOCUMENT_NAMING_CONVENTION__DATE_SERIES:
                    {
                        if (seriesTagName == "") return "";

                        return string.Format("{0} - {1}",
                            referenceDateString,
                            seriesTagName);
                    }


                case DOCUMENT_NAMING_CONVENTION__DATE_SERIES_TITLE:
                    {
                        if (title == "") return "";
                        if (seriesTagName == "") return "";

                        return string.Format("{0} - {1} - {2}",
                            referenceDateString,
                            seriesTagName,
                            title);
                    }

                case DOCUMENT_NAMING_CONVENTION__DATE_SERIES_REFERENCE_ID:
                    {
                        if (seriesTagName == "") return "";
                        if (referenceID == "") return "";

                        return string.Format("{0} - {1} - {2}",
                            referenceDateString,
                            seriesTagName,
                            referenceID);
                    }                    

                case DOCUMENT_NAMING_CONVENTION__DATE_SERIES_REFID_TITLE:
                    {
                        if (title == "") return "";
                        if (seriesTagName == "") return "";
                        if (referenceID == "") return "";

                        return string.Format("{0} - {1} - {2} - {3}",
                            referenceDateString,
                            seriesTagName,
                            referenceID,
                            title);
                    }                    
                case DOCUMENT_NAMING_CONVENTION__DATE_WORK_BOX_ID_TITLE:
                    {
                        if (title == "") return "";
                        if (workBoxID == "") return "";

                        return string.Format("{0} - {1} - {2}",
                            referenceDateString,
                            workBoxID,
                            title);
                    }

                case DOCUMENT_NAMING_CONVENTION__DATE_REFERENCE_ID_TITLE:
                    {
                        if (title == "") return "";
                        if (referenceID == "") return "";

                        return string.Format("{0} - {1} - {2}",
                            referenceDateString,
                            referenceID,
                            title);
                    }

                case DOCUMENT_NAMING_CONVENTION__WORK_BOX_ID_TITLE:
                    {
                        if (title == "") return "";
                        if (workBoxID == "") return "";

                        return string.Format("{0} - {1}",
                            workBoxID,
                            title);
                    }
            }
            
            return "";
        }



        #endregion

        #region Static Methods

        public bool CanCurrentUserCreateWorkBoxForTeam(WBCollection collection, WBTeam team)
        {
            if (!AllowWorkBoxRecords) return false;

            switch (WhoCanCreateNewWorkBoxes)
            {
                case WHO_CAN_CREATE_NEW_WORK_BOXES__ANYONE:
                    return true;

                case WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_MEMBERS:
                    {
                        if (team.IsCurrentUserTeamMember())
                            return true;
                        return false;
                    }

                case WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_OWNERS:
                    {
                        if (team.IsCurrentUserTeamOwnerOrSystemAdmin())
                            return true;
                        return false;
                    }

                case WHO_CAN_CREATE_NEW_WORK_BOXES__WORK_BOX_COLLECTION_ADMINISTRATORS:
                    {
                        return false;
                    }

                case WHO_CAN_CREATE_NEW_WORK_BOXES__SYSTEM_ONLY:
                    return false;
            }

            return false;
        }

        public static List<String> getWhoCanCreateOptions() 
        {
            List<String> list = new List<String>();

            list.Add(WHO_CAN_CREATE_NEW_WORK_BOXES__ANYONE);
            list.Add(WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_MEMBERS);
            list.Add(WHO_CAN_CREATE_NEW_WORK_BOXES__TEAM_OWNERS);
            list.Add(WHO_CAN_CREATE_NEW_WORK_BOXES__WORK_BOX_COLLECTION_ADMINISTRATORS);
            list.Add(WHO_CAN_CREATE_NEW_WORK_BOXES__SYSTEM_ONLY);

            return list;
        }

        public static List<String> getWorkBoxLocalIDSources()
        {
            List<String> list = new List<String>();

            list.Add(LOCAL_ID_SOURCE__GENERATE_LOCAL_ID);
            list.Add(LOCAL_ID_SOURCE__USE_REFERENCE_ID);
            list.Add(LOCAL_ID_SOURCE__USE_CURRENT_USER_LOGIN_NAME);

            return list;
        }


        public static List<String> getWorkBoxNamingConventions()
        {
            List<String> list = new List<String>();

            list.Add(WORK_BOX_NAMING_CONVENTION__PREFIX_TITLE);
            list.Add(WORK_BOX_NAMING_CONVENTION__PREFIX_LOCALID_TITLE);
            list.Add(WORK_BOX_NAMING_CONVENTION__PREFIX_REFERENCEID_TITLE);
            list.Add(WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_TITLE);
            list.Add(WORK_BOX_NAMING_CONVENTION__TEAM_TITLE);
            list.Add(WORK_BOX_NAMING_CONVENTION__TEAM_PREFIX_OPTIONAL_SERIES_DATE);
            
            return list;
        }

        public static List<String> getAutoCloseTriggerDates()
        {
            List<String> list = new List<String>();

            list.Add(AUTO_CLOSE_TRIGGER_DATE__NONE);
            list.Add(AUTO_CLOSE_TRIGGER_DATE__LAST_MODIFIED_DATE);
            list.Add(AUTO_CLOSE_TRIGGER_DATE__REFERENCE_DATE);
            list.Add(AUTO_CLOSE_TRIGGER_DATE__DATE_CREATED);
            list.Add(AUTO_CLOSE_TRIGGER_DATE__END_FINANCIAL_LAST_MODIFIED_DATE);
            list.Add(AUTO_CLOSE_TRIGGER_DATE__END_FINANCIAL_REFERENCE_DATE);

            return list;
        }

        public static List<String> getAutoCloseUnits()
        {
            List<String> units = new List<String>();

            units.Add(AUTO_CLOSE_TIME_UNIT__NONE);
            units.Add(AUTO_CLOSE_TIME_UNIT__YEARS);
            units.Add(AUTO_CLOSE_TIME_UNIT__MONTHS);
            units.Add(AUTO_CLOSE_TIME_UNIT__DAYS);
            units.Add(AUTO_CLOSE_TIME_UNIT__HOURS);
            units.Add(AUTO_CLOSE_TIME_UNIT__MINUTES);

            return units;
        }

        public static List<String> getRetentionTriggerDates()
        {
            List<String> list = new List<String>();

            list.Add(RETENTION_TRIGGER_DATE__NONE);
            list.Add(RETENTION_TRIGGER_DATE__LAST_CLOSED_DATE);
            list.Add(RETENTION_TRIGGER_DATE__REFERENCE_DATE);
            list.Add(RETENTION_TRIGGER_DATE__END_FINANCIAL_LAST_CLOSED_DATE);
            list.Add(RETENTION_TRIGGER_DATE__END_FINANCIAL_REFERENCE_DATE);

            return list;
        }

        public static List<String> getRetentionUnits()
        {
            List<String> units = new List<String>();

            units.Add(RETENTION_TIME_UNIT__PERMANENT);
            units.Add(RETENTION_TIME_UNIT__YEARS);
            units.Add(RETENTION_TIME_UNIT__MONTHS);
            units.Add(RETENTION_TIME_UNIT__DAYS);
            units.Add(RETENTION_TIME_UNIT__HOURS);
            units.Add(RETENTION_TIME_UNIT__MINUTES);

            return units;
        }


        public static List<String> getProtectiveZones()
        {
            List<String> zones = new List<String>();

            zones.Add(PROTECTIVE_ZONE__PROTECTED);
            zones.Add(PROTECTIVE_ZONE__UNPROTECTED);
            zones.Add(PROTECTIVE_ZONE__PUBLIC_EXTRANET);
            zones.Add(PROTECTIVE_ZONE__PUBLIC);

            return zones;
        }

        public static List<String> getRequirementOptions()
        {
            List<String> list = new List<String>();

            list.Add(METADATA_REQUIREMENT__REQUIRED);
            list.Add(METADATA_REQUIREMENT__OPTIONAL);
            list.Add(METADATA_REQUIREMENT__HIDDEN);

            return list;
        }


        public static List<String> getReferenceDateSources()
        {
            List<String> list = new List<String>();

//            list.Add(DOCUMENT_REFERENCE_DATE_SOURCE__MANUAL_ENTRY);
            list.Add(DOCUMENT_REFERENCE_DATE_SOURCE__PUBLISH_OUT_DATE);
            list.Add(DOCUMENT_REFERENCE_DATE_SOURCE__WORK_BOX_REFERENCE_DATE);

            return list;
        }

        public static List<String> getDocumentNamingConventions()
        {
            List<String> namingConventions = new List<String>();

            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__NONE);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_TITLE);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_SERIES);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_SERIES_TITLE);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_SERIES_REFERENCE_ID);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_SERIES_REFID_TITLE);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_WORK_BOX_ID_TITLE);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__DATE_REFERENCE_ID_TITLE);
            namingConventions.Add(DOCUMENT_NAMING_CONVENTION__WORK_BOX_ID_TITLE);

            return namingConventions;
        }

        public static List<String> getFilingRules()
        {
            List<String> list = new List<String>();

            list.Add(FILING_RULE__NONE);
            list.Add(FILING_RULE__BY_FINANCIAL_YEAR);
            list.Add(FILING_RULE__BY_CALENDAR_YEAR);
            list.Add(FILING_RULE__BY_MONTH);
            list.Add(FILING_RULE__BY_DAY_OF_MONTH);
            list.Add(FILING_RULE__BY_FULL_DATE);
            list.Add(FILING_RULE__BY_FUNCTIONAL_AREA);
            list.Add(FILING_RULE__BY_REFERENCE_ID);
            list.Add(FILING_RULE__BY_SERIES_TAG);
            list.Add(FILING_RULE__BY_OWNING_TEAM);

            return list;
        }


        public static DateTime getPermanentDate()
        {
            return new DateTime(YEAR_REPRESENTING_A_PERMANENT_DATE, 1, 1);
        }

        #endregion

        internal List<String> FilingPathForDocument(WBDocument document)
        {
            List<String> path = new List<String>();

            // Build up the records type path from this records type up the parental chain:
            WBRecordsType recordTypeInPath = this;
            while (recordTypeInPath != null)
            {
                path.Insert(0, recordTypeInPath.Name);
                recordTypeInPath = recordTypeInPath.Parent;
            }

            // Then put the functional area at the start:
            WBTermCollection<WBTerm> functionalAreas = document.FunctionalArea;
            if (functionalAreas != null && functionalAreas.Count > 0)
            {
                path.Insert(0, functionalAreas[0].Name);
            }


            if (this.FilingRuleLevel1 != "")
            {
                path.Add(FilingRuleValue(FilingRuleLevel1, document));

                if (this.FilingRuleLevel2 != "")
                {
                    path.Add(FilingRuleValue(FilingRuleLevel2, document));

                    if (this.FilingRuleLevel3 != "")
                    {
                        path.Add(FilingRuleValue(FilingRuleLevel3, document));

                        if (this.FilingRuleLevel4 != "")
                        {
                            path.Add(FilingRuleValue(FilingRuleLevel4, document));
                        }
                    }
                }
            }
            else
            {
                // This is the default filing rule to be used if not rules are set on the records type:
                path.Add(FilingRuleValue(FILING_RULE__BY_FINANCIAL_YEAR, document));
            }

            WBUtils.logMessage("For item: " + document.Name + " created path: " + string.Join("/", path.ToArray()));

            return path;
        }

        private string FilingRuleValue(string filingRule, WBDocument document)
        {
            switch (filingRule)
            {
                case FILING_RULE__BY_FINANCIAL_YEAR: 
                    {
                        if (!document.HasReferenceDate) return "NO DATE SET";
                        DateTime referenceDate = document.ReferenceDate;

                        int year = referenceDate.Year;
                        int month = referenceDate.Month;
                        
                        if (month >= 4) return string.Format("{0}-{1}", year.ToString("D4"), (year+1).ToString("D4"));
                        else return string.Format("{0}-{1}", (year-1).ToString("D4"), year.ToString("D4"));
                    }

                case FILING_RULE__BY_CALENDAR_YEAR:
                    {
                        if (!document.HasReferenceDate) return "NO DATE SET";
                        DateTime referenceDate = document.ReferenceDate;

                        return referenceDate.Year.ToString("D4");
                    }
                case FILING_RULE__BY_MONTH:
                    {
                        if (!document.HasReferenceDate) return "NO DATE SET";
                        DateTime referenceDate = document.ReferenceDate;

                        return referenceDate.Month.ToString("D2");
                    }
                case FILING_RULE__BY_DAY_OF_MONTH:
                    {
                        if (!document.HasReferenceDate) return "NO DATE SET";
                        DateTime referenceDate = document.ReferenceDate;

                        return referenceDate.Day.ToString("D2");
                    }
                case FILING_RULE__BY_FULL_DATE:
                    {
                        if (!document.HasReferenceDate) return "NO DATE SET";
                        DateTime referenceDate = document.ReferenceDate;

                        return string.Format("{0}-{1}-{2}",
                            referenceDate.Year.ToString("D4"),
                            referenceDate.Month.ToString("D2"),
                            referenceDate.Day.ToString("D2"));
                    }
                case FILING_RULE__BY_FUNCTIONAL_AREA:
                    {
                        WBTermCollection<WBTerm> functionalArea = document.FunctionalArea;
                        if (functionalArea == null || functionalArea.Count == 0) return "NO FUNCTIONAL AREA SET";

                        return functionalArea[0].Name;
                    }
                case FILING_RULE__BY_REFERENCE_ID:
                    {
                        if (String.IsNullOrEmpty(document.ReferenceID)) return "NO REFERENCE ID SET";
                        return document.ReferenceID;
                    }
                case FILING_RULE__BY_SERIES_TAG:
                    {
                        if (document.SeriesTag == null) return "NO RECORD SERIES SET";
                        return document.SeriesTag.Name;
                    }
                case FILING_RULE__BY_OWNING_TEAM:
                    {
                        if (document.OwningTeam == null) return "NO OWNING TEAM SET";
                        return document.OwningTeam.Name;
                    }
                case FILING_RULE__BY_WORK_BOX_ID:
                    {
                        return "NOT IMPLEMENTED YET";
                    }

            }

            return "NO FILING RULE";
        }

        public bool IsZoneAtLeastMinimum(String zone)
        {
            if (zone == null || zone == "") return false;

            string minimum = this.DocumentMinimumProtectiveZone;
            if (minimum == null || minimum == "") return false;

            // Public zone is the most open, so if it's the minimum then any zone except Public Extranet is OK:
            if (minimum.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC))
            {
                // You can only publish to the public extranet zone if that is the minimum for the records type.
                if (zone.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)) return false;

                //otherwise:
                return true;
            }


            // The following is not the most efficient code - but it's easier to read and understand:
            if (minimum.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)) 
            {
                if (zone.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)) return true;
                if (zone.Equals(WBRecordsType.PROTECTIVE_ZONE__PROTECTED)) return true;
                return false;
            }
            else if (minimum.Equals(WBRecordsType.PROTECTIVE_ZONE__PROTECTED)) 
            {
                if (zone.Equals(WBRecordsType.PROTECTIVE_ZONE__PROTECTED)) return true;
                return false;
            }
            else
            {
                return false;
            }
        }


        public List<String> GetAllPropertyValues()
        {
            List<String> values = new List<String>();

            values.Add(this.Term.WBxFullPath());
            values.Add(Description);


           values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DEFAULT_FUNCTIONAL_AREA));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_OTHER_FUNCTIONAL_AREAS));


        // Work Box Record Properties:
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_WORK_BOX_RECORDS));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WHO_CAN_CREATE_NEW_WORK_BOXES));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__CREATE_NEW_WORK_BOX_TEXT));
        
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_COLLECTION_URL));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_UNIQUE_ID_PREFIX));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_LOCAL_ID_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_GENERATED_LOCAL_ID_OFFSET));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_ID_DESCRIPTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_REFERENCE_DATE_DESCRIPTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_PARENT_TERM));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_ALLOW_NEW_TERMS));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_SERIES_TAG_DESCRIPTION));
        
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__WORK_BOX_NAMING_CONVENTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TRIGGER_DATE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_SCALAR));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__AUTO_CLOSE_TIME_UNIT));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TRIGGER_DATE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_SCALAR));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__RETENTION_TIME_UNIT));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_PUBLISHING_OUT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__MINIMUM_PUBLISHING_OUT_PROTECTIVE_ZONE));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__GENERATE_PUBLISH_OUT_FILENAMES));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__USE_DEFAULTS_WHEN_PUBLISHING_OUT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DEFAULT_PUBLISHING_OUT_RECORDS_TYPE));


        // Document Record Properties:
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__ALLOW_DOCUMENT_RECORDS));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_MINIMUM_PROTECTIVE_ZONE));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_ID_DESCRIPTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_REFERENCE_DATE_DESCRIPTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_PARENT_TERM));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_ALLOW_NEW_TERMS));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SERIES_TAG_DESCRIPTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_REQUIREMENT));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_SOURCE));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_SCAN_DATE_DESCRIPTION));



        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__ENFORCE_DOCUMENT_NAMING_CONVENTION));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__DOCUMENT_NAMING_CONVENTION));

        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_1));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_2));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_3));
        values.Add(Term.WBxGetProperty(RECORDS_TYPE_TERM_PROPERTY__FILING_RULE_LEVEL_4));

            return values;

        }

        public void CheckBasicSetup()
        {
            // For the moment this is just going to set the values as needed for the import process:

            this.DocumentNamingConvention = DOCUMENT_NAMING_CONVENTION__DATE_TITLE;

            this.FilingRuleLevel1 = FILING_RULE__BY_FINANCIAL_YEAR;

            this.DocumentShortTitleRequirement = METADATA_REQUIREMENT__REQUIRED;
            this.DocumentSubjectTagsRequirement = METADATA_REQUIREMENT__OPTIONAL;

            this.Update();
        }

        public WBItemMessages CheckMetadataIsOK(WBItem item)
        {
            WBItemMessages metadataProblems = new WBItemMessages();

            /*
            WBLogging.Debug("Checking metadata for declaring an item");
            foreach (WBColumn column in item.Columns)
            {
                WBLogging.Debug("Column: " + column.DisplayName + " Value: " + item[column].WBxToString());
            }


            if (item[WBColumn.FunctionalArea] == null || ((WBTermCollection<WBTerm>)item[WBColumn.FunctionalArea]).Count != 1)
            {
                metadataProblems[WBColumn.FunctionalArea] = "You should have exactly one functional area defined for a document being published.";
            }
            */

//            if (OwningTeamField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__OWNING_TEAM, "You must enter the owning team.");

  //          if (InvolvedTeamsField.Text.Equals("")) metadataProblems.Add(WorkBox.COLUMN_NAME__INVOLVED_TEAMS, "You must enter at least one involved team.");


            /*

            if (RecordsType.Text.Equals(""))
            {
                metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "You must enter a records type for this document.");
            }
            else
            {
                // So here we'll load up the actual records type so that we can check what other metadata is required:
                documentRecordsType = new WBRecordsType(recordsTypeTaxonomy, RecordsTypeUIControlValue.Value);

                if (documentRecordsType != null)
                {
                    if (!documentRecordsType.AllowDocumentRecords)
                    {
                        metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "You cannot publish documents of this records type. Please choose another.");
                    }


                    if (documentRecordsType.IsFunctionalAreaEditable)
                    {
                        if (FunctionalAreaField.Text == "")
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__FUNCTIONAL_AREA, "The functional area must be set.");
                        }
                    }

                    if (!documentRecordsType.IsZoneAtLeastMinimum(ProtectiveZone.Text))
                    {
                        metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "The selected protective zone does not meet the minimum requirement for this records type of: " + documentRecordsType.DocumentMinimumProtectiveZone);
                    }

                    if (documentRecordsType.IsDocumentReferenceIDRequired)
                    {
                        if (ReferenceID.Text.Equals(""))
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__REFERENCE_ID, "You must enter a reference ID for this records type.");
                        }
                    }

                    if (documentRecordsType.IsDocumentReferenceDateRequired)
                    {
                        if (ReferenceDate.IsDateEmpty)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__REFERENCE_DATE, "You must enter a reference date for this records type.");
                        }
                    }

                    if (documentRecordsType.IsDocumentSeriesTagRequired)
                    {
                        if (SeriesTagDropDownList.SelectedValue.Equals(""))
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__SERIES_TAG, "You must select a series tag for this records type.");
                        }
                    }

                    if (documentRecordsType.IsDocumentScanDateRequired)
                    {
                        if (ScanDate.IsDateEmpty)
                        {
                            metadataProblems.Add(WorkBox.COLUMN_NAME__SCAN_DATE, "You must enter a scan date for this records type.");
                        }
                    }
                }
                else
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__RECORDS_TYPE, "Could not find this records type.");
                }
            }

            if (destinationType.Equals(WorkBox.PUBLISHING_OUT_DESTINATION_TYPE__PUBLIC_WEB_SITE)
                && !ProtectiveZone.SelectedValue.Equals(WBRecordsType.PROTECTIVE_ZONE__PUBLIC))
            {
                if (!metadataProblems.ContainsKey(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE))
                {
                    metadataProblems.Add(WorkBox.COLUMN_NAME__PROTECTIVE_ZONE, "Only documents marked as 'Public' can be published to the Public Web Site");
                }

            }
            */


            return metadataProblems;
        }

        public SPListItem PublishDocument(WBDocument document, Stream binaryStream)
        {
            WBFarm farm = WBFarm.Local;

            using (SPSite protectedLibrarySite = new SPSite(farm.ProtectedRecordsLibraryUrl))
            using (SPWeb protectedLibraryWeb = protectedLibrarySite.OpenWeb())
            {
                protectedLibrarySite.AllowUnsafeUpdates = true;
                protectedLibraryWeb.AllowUnsafeUpdates = true; 

                SPSite publicLibrarySite = null;
                SPWeb publicLibraryWeb = null;
                SPFolder publicLibraryRootFolder = null;

                if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                {
                    publicLibrarySite = new SPSite(farm.PublicRecordsLibraryUrl);
                    publicLibraryWeb = publicLibrarySite.OpenWeb();
                    publicLibraryRootFolder = publicLibraryWeb.GetFolder(farm.PublicRecordsLibraryUrl);

                    publicLibrarySite.AllowUnsafeUpdates = true;
                    publicLibraryWeb.AllowUnsafeUpdates = true; 

                }

                try
                {
                    return PublishDocument(
                        protectedLibraryWeb,
                        protectedLibraryWeb.GetFolder(farm.ProtectedRecordsLibraryUrl),
                        publicLibraryWeb,
                        publicLibraryRootFolder,
                        document,
                        binaryStream);
                }
                finally
                {
                    if (publicLibraryWeb != null) publicLibraryWeb.Dispose();
                    if (publicLibrarySite != null) publicLibrarySite.Dispose();
                }

            }


        }


        public SPListItem PublishDocument(
            SPWeb protectedLibraryWeb,
            SPFolder protectedLibraryRootFolder,
            SPWeb publicLibraryWeb,
            SPFolder publicLibraryRootFolder,
            WBDocument document, Stream binaryStream)
        {


            WBTerm functionalArea = document.FunctionalArea[0];
            string fullClassPath = WBUtils.NormalisePath(functionalArea.Name + "/" + this.FullPath);

            WBLogging.RecordsTypes.HighLevel("Declaring a document to the library with path: " + fullClassPath);

            string datePath = "NO DATE SET";
            string dateForName = "YYYY-MM-DD";
            string oldDateFormat = "YYYYMMDD-";

            // If nothing else we'll use the time now (which will be roughly the date / time declared as the date for the naming convention:
            DateTime referenceDate = DateTime.Now;
            if (document.HasReferenceDate)
            {
                // But ideally we'll be taking the reference date from the metadata of the document being declared:
                referenceDate = document.ReferenceDate;
            }
            else
            {
                document.ReferenceDate = referenceDate;
            }

            int year = referenceDate.Year;
            int month = referenceDate.Month;

            if (month >= 4) datePath = String.Format("{0}-{1}", year.ToString("D4"), (year + 1).ToString("D4"));
            else datePath = String.Format("{0}-{1}", (year - 1).ToString("D4"), year.ToString("D4"));

            dateForName = String.Format("{0}-{1}-{2}",
                        referenceDate.Year.ToString("D4"),
                        referenceDate.Month.ToString("D2"),
                        referenceDate.Day.ToString("D2"));

            oldDateFormat = String.Format("{0}{1}{2}-",
                        referenceDate.Year.ToString("D4"),
                        referenceDate.Month.ToString("D2"),
                        referenceDate.Day.ToString("D2"));


            string fullFilingPath = String.Join("/", FilingPathForDocument(document).ToArray());

            WBLogging.Debug("The original filename is set as: " + document.OriginalFilename);

            String extension = Path.GetExtension(document.OriginalFilename);
            String filename = WBUtils.RemoveDisallowedCharactersFromFilename(document.OriginalFilename);

            String titleForFilename = document[WBColumn.Title] as String;
            String referenceID = document.ReferenceID;

            // We don't want to use a title that is too long:
            if (String.IsNullOrEmpty(titleForFilename) || titleForFilename.Length > 50) titleForFilename = "";

            if (String.IsNullOrEmpty(titleForFilename) && String.IsNullOrEmpty(referenceID)) 
            {
                titleForFilename = Path.GetFileNameWithoutExtension(filename);

                // Let's now remove the old date format if the date is the same as the one
                // that is going to be used for the new date format:
                titleForFilename = titleForFilename.Replace(oldDateFormat, "");
            }

            if (String.IsNullOrEmpty(referenceID))
            {
                filename = "(" + dateForName + ") " + titleForFilename + extension;
            }
            else
            {
                if (String.IsNullOrEmpty(titleForFilename))
                {
                    filename = "(" + dateForName + ") " + referenceID + extension;
                }
                else
                {
                    filename = "(" + dateForName + ") " + referenceID + " - " + titleForFilename + extension;
                }
            }

            filename = WBUtils.RemoveDisallowedCharactersFromFilename(filename);

            SPContentType classFolderType = null;
            SPContentType filePartFolderType = null;

            try
            {
                classFolderType = protectedLibraryWeb.Site.RootWeb.ContentTypes[RECORDS_LIBRARY__CLASS_FOLDER_CONTENT_TYPE];
                filePartFolderType = protectedLibraryWeb.Site.RootWeb.ContentTypes[RECORDS_LIBRARY__FILE_PART_FOLDER_CONTENT_TYPE];
            }
            catch (Exception exception)
            {
                WBLogging.RecordsTypes.Unexpected("Couldn't find the class and/or file part folder content types.");
                throw new Exception("Couldn't find the class and/or file part folder content types.", exception);
            }

            if (classFolderType == null)
            {
                classFolderType = protectedLibraryWeb.Site.RootWeb.ContentTypes[RECORDS_LIBRARY__FALL_BACK_FOLDER_CONTENT_TYPE];
            }

            if (filePartFolderType == null)
            {
                filePartFolderType = protectedLibraryWeb.Site.RootWeb.ContentTypes[RECORDS_LIBRARY__FALL_BACK_FOLDER_CONTENT_TYPE];
            }

            protectedLibraryRootFolder.WBxGetOrCreateFolderPath(fullClassPath, classFolderType.Id);
            SPFolder actualDestinationFolder = protectedLibraryRootFolder.WBxGetOrCreateFolderPath(fullFilingPath, filePartFolderType.Id);

            if (protectedLibraryWeb.WBxFileExists(actualDestinationFolder, filename))
            {
                filename = protectedLibraryWeb.WBxMakeFilenameUnique(actualDestinationFolder, filename);
            }

            SPFile uploadedFile = actualDestinationFolder.Files.Add(filename, binaryStream);
            WBLogging.Migration.Verbose("Uploaded file: " + uploadedFile.Name);

            SPListItem uploadedItem = uploadedFile.Item;

            WBColumn[] columnsToSet = { 
                                        WBColumn.Title,
                                        WBColumn.RecordsType, 
                                        WBColumn.FunctionalArea, 
                                        WBColumn.SubjectTags,
                                        WBColumn.SeriesTag,
                                        WBColumn.ReferenceID,
                                        WBColumn.ReferenceDate,
                                        WBColumn.ScanDate,
                                        WBColumn.OwningTeam,
                                        WBColumn.InvolvedTeams,
                                        WBColumn.OriginalFilename,
                                        WBColumn.SourceID,
                                        WBColumn.SourceFilePath,
                                        WBColumn.SourceSystem,
                                        WBColumn.ProtectiveZone,
                                        WBColumn.LiveOrArchived
                                      };

            uploadedItem.WBxSetFrom(document, columnsToSet);

            if (document.IsNotEmpty(WBColumn.Modified))
            {
               // uploadedItem.Fields[WBColumn.Modified.DisplayName].ReadOnlyField = false;
               //  uploadedItem[WBColumn.Modified.DisplayName] = document[WBColumn.Modified];

                uploadedFile.Item["Modified"] = document[WBColumn.Modified];

            }

            uploadedItem.Update();
            uploadedFile.Update();

            uploadedItem.WBxSet(WBColumn.RecordID, uploadedItem.ID);
            uploadedItem.Update();

            /*
            if (document.ContainsKey(WBColumn.Modified))
            {
                uploadedItem.Fields[WBColumn.Modified.DisplayName].ReadOnlyField = true;
                uploadedItem.Fields[WBColumn.Modified.DisplayName].Update();
            }
            */

            WBLogging.Migration.Verbose("Updated file: " + uploadedFile.Name);

            if (uploadedFile.CheckOutType != SPFile.SPCheckOutType.None)
            {
                WBLogging.Migration.Verbose("Checking in file: " + uploadedFile.Name);
                uploadedFile.CheckIn("Automatically checked in", SPCheckinType.MajorCheckIn);
            }
            else
            {
                WBLogging.Migration.Verbose("There was no need to check in file: " + uploadedFile.Name);
            }


            if (uploadedItem.WBxGetAsString(WBColumn.ProtectiveZone) == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
            {
                // OK so we're goijng to copy this item to the public library:
                WBFarm farm = WBFarm.Local;


                string errorMessagePublic = uploadedFile.WBxCopyTo(farm.PublicRecordsLibraryUrl, fullFilingPath, true);

            }

            if (uploadedItem.WBxGetAsString(WBColumn.ProtectiveZone) == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
            {
                // OK so we're going to copy this item to the public library:
                WBFarm farm = WBFarm.Local;


                string errorMessagePublicExtranet = uploadedFile.WBxCopyTo(farm.PublicExtranetRecordsLibraryUrl, fullFilingPath, true);

            }


            return uploadedItem;
        }
    }
}


