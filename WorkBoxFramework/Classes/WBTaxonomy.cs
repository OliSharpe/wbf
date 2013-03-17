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
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;


namespace WorkBoxFramework
{
    public class WBTaxonomy
    {
        #region Constructors and Factories

        private String _termStoreName = null;
        private String _groupName = null;
        private String _termSetName = null;

        public WBTaxonomy(SPSite site, String termStoreName, String groupName, String termSetName)
        {
            _session = new TaxonomySession(site);
            
            _termStore = null;
            _termStoreName = termStoreName;

            _group = null;
            _groupName = groupName;

            _termSet = null;
            _termSetName = termSetName;
        }

        public WBTaxonomy(WBTaxonomy taxonomy, String termSetName)
        {
            _session = taxonomy._session;

            _termStore = taxonomy._termStore;
            _termStoreName = taxonomy._termStoreName;

            _group = taxonomy._group;
            _groupName = taxonomy._groupName;

            _termSet = null;
            _termSetName = termSetName;
        }

        #endregion

        public static WBTaxonomy GetTeams(SPSite site)
        {
            WBFarm farm = WBFarm.Local;
            return new WBTaxonomy(site,
                farm.TermStoreName,
                farm.TermStoreGroupName,
                WorkBox.TERM_SET_NAME__TEAMS);
        }

        public static WBTaxonomy GetTeams(WBTaxonomy taxonomy)
        {
            return new WBTaxonomy(taxonomy,
                WorkBox.TERM_SET_NAME__TEAMS);
        }


        public static WBTaxonomy GetRecordsTypes(SPSite site)
        {
            WBFarm farm = WBFarm.Local;
            return new WBTaxonomy(site,
                farm.TermStoreName,
                farm.TermStoreGroupName,
                WorkBox.TERM_SET_NAME__RECORDS_TYPES);
        }

        public static WBTaxonomy GetRecordsTypes(WBTaxonomy taxonomy)
        {
            return new WBTaxonomy(taxonomy,
                WorkBox.TERM_SET_NAME__RECORDS_TYPES);
        }

        public static WBTaxonomy GetSeriesTags(SPSite site)
        {
            WBFarm farm = WBFarm.Local;
            return new WBTaxonomy(site,
                farm.TermStoreName,
                farm.TermStoreGroupName,
                WorkBox.TERM_SET_NAME__SERIES_TAGS);
        }

        public static WBTaxonomy GetSeriesTags(WBTaxonomy taxonomy)
        {
            return new WBTaxonomy(taxonomy,
                WorkBox.TERM_SET_NAME__SERIES_TAGS);
        }

        public static WBTaxonomy GetSubjectTags(SPSite site)
        {
            WBFarm farm = WBFarm.Local;
            return new WBTaxonomy(site,
                farm.TermStoreName,
                farm.TermStoreGroupName,
                WorkBox.TERM_SET_NAME__SUBJECT_TAGS);
        }

        public static WBTaxonomy GetSubjectTags(WBTaxonomy taxonomy)
        {
            return new WBTaxonomy(taxonomy,
                WorkBox.TERM_SET_NAME__SUBJECT_TAGS);
        }

        public static WBTaxonomy GetFunctionalAreas(WBTaxonomy taxonomy)
        {
            return new WBTaxonomy(taxonomy,
                WorkBox.TERM_SET_NAME__FUNCTIONAL_AREAS);
        }


        #region Properties

        private TaxonomySession _session = null;
        public TaxonomySession Session { get { return _session; } }  // The _session object should never be null.
                       
        private TermStore _termStore = null;
        public TermStore TermStore
        {
            get 
            {
                if (_termStore == null)
                {
                    WBLogging.Debug("In WBTaxonomy.TermStore: trying to get term store: " + _termStoreName);

                    _termStore = Session.TermStores[_termStoreName];
                }
                return _termStore;
            }
        }

        private Group _group = null;
        public Group Group
        {
            get 
            {
                if (_group == null)
                {
                    WBLogging.Generic.Verbose("In WBTaxonomy.Group: trying to get group: " + _groupName);

                    _group = TermStore.Groups[_groupName];
                }
                return _group;
            }
        }

        private TermSet _termSet = null;
        public TermSet TermSet
        {
            get
            {
                if (_termSet == null)
                {
                    _termSet = Group.TermSets[_termSetName];
                }
                return _termSet;
            }
        }

        #endregion


        #region Methods

        public void InitialiseTaxonomyControl(TaxonomyWebTaggingControl taggingControl, String title, bool isMultiple)
        {
            InitialiseTaxonomyControl(taggingControl, title, isMultiple, false, null);
        }

        public void InitialiseTaxonomyControl(TaxonomyWebTaggingControl taggingControl, String title, bool isMultiple, bool allowFillIn, Control displayControlUsingAJAX)
        {
            taggingControl.SspId.Add(TermStore.Id);
            taggingControl.GroupId = Group.Id;
            taggingControl.TermSetList = TermSet.Id.ToString();
            taggingControl.AllowFillIn = allowFillIn;
            taggingControl.IsMulti = isMultiple;
            taggingControl.AnchorId = Guid.Empty;
            taggingControl.FieldName = title;

            if (displayControlUsingAJAX != null) 
            {
                String key = "TaxonomyWebTaggingAjaxIncludeOnce_" + taggingControl.ID;
                if (!displayControlUsingAJAX.Page.ClientScript.IsClientScriptBlockRegistered(displayControlUsingAJAX.GetType(), key))
                {
                    displayControlUsingAJAX.Page.ClientScript.RegisterClientScriptBlock(displayControlUsingAJAX.GetType(), key, GetReloadJavaScript(taggingControl), true);
                }
            }
        }

        // Thank you to the following blog for the code to enable AJAX use of the TaxonomyWebTaggingControl
        // http://pholpar.wordpress.com/2010/03/03/ajax-enabling-the-taxonomywebtaggingcontrol/

        private string GetReloadJavaScript(TaxonomyWebTaggingControl taxonomyControl)
        {
            String script = String.Empty;

            String containerId = SPEncode.ScriptEncode(taxonomyControl.Controls[1].ClientID);

            Type type_TaxonomyWebTaggingControl = typeof(TaxonomyWebTaggingControl);

            MethodInfo mi_getOnloadJavascript = type_TaxonomyWebTaggingControl.GetMethod("getOnloadJavascript", BindingFlags.NonPublic | BindingFlags.Instance);
            String fullScript = (String)mi_getOnloadJavascript.Invoke(taxonomyControl, null);
            int pos = fullScript.IndexOf(String.Format("function {0}_load()", containerId));

            if (pos > -1)
            {
                string endRequestFunctionName = string.Format("{0}_EndRequest", containerId);

                StringBuilder builder = new StringBuilder();
                builder.Append("var myPrm = Sys.WebForms.PageRequestManager.getInstance();");
                builder.Append("myPrm.add_endRequest(").Append(endRequestFunctionName).Append(");");
                builder.Append("function ").Append(endRequestFunctionName).Append("(sender, args)");
                builder.Append("{");
                // we get te first part of the script needed to initialization
                // we start from pos 1, because we don't need the leading '{'
                builder.Append(fullScript.Substring(1, pos - 1));
                builder.Append("Microsoft.SharePoint.Taxonomy.ScriptForWebTaggingUI.onLoad('");
                builder.Append(containerId);
                builder.Append("');");
                builder.Append("}}");

                script = builder.ToString();
            }

            return script;
        }




        public void CommitAll()
        {
//            WBUtils.logMessage("WBTaxonomy committing changes to term store | group | term set: " + TermStore.Name + " | " + Group.Name + " | " + TermSet.Name);
            TermStore.CommitAll();
        }

        public WBTeam GetTeam(Guid id)
        {
            Term term = TermSet.GetTerm(id);
            if (term == null) return null;
            return new WBTeam(this, term);
        }

        public WBRecordsType GetRecordsType(Guid id)
        {
            Term term = TermSet.GetTerm(id);
            if (term == null) return null;
            return new WBRecordsType(this, term);
        }

        /*
        public int[] GetWssIdsFromSiteForTerm(SPSite site, WBTerm term)
        {
            WBUtils.logMessage("Getting WssIds from site | term " + site.Url + " | " + term.Name);
            return TaxonomyField.GetWssIdsOfTerm(site, TermStore.Id, TermSet.Id, term.Id, false, 500);
        }
         */

        public WBTerm GetOrCreateSelectedWBTermByPath(String path)
        {
            return GetSelectedWBTermByPath(path, true);
        }

        public WBTerm GetSelectedWBTermByPath(String path)
        {
            return GetSelectedWBTermByPath(path, false);
        }

        public WBTerm GetSelectedWBTermByPath(String path, bool createIfNew)
        {
            Term term = GetSelectedTermByPath(path, createIfNew);
            if (term == null) return null;
            return new WBTerm(this, term);
        }

        public Term GetOrCreateSelectedTermByPath(String path)
        {
            return GetSelectedTermByPath(path, true);
        }

        public Term GetSelectedTermByPath(String path)
        {
            return GetSelectedTermByPath(path, false);
        }

        public Term GetSelectedTermByPath(String path, bool createIfNew)
        {
            WBLogging.Generic.Verbose("In GetSelectedTermByPath(): started");
            string[] steps = path.Split('/');
            TermCollection nextLevelTerms = TermSet.Terms;
            WBLogging.Generic.Verbose("In GetSelectedTermByPath(): got top level terms");
            Term nextTerm = null;
            bool taxonomyNeedsSaving = false;
            foreach (string step in steps)
            {
                if (String.IsNullOrEmpty(step)) continue;
                if (step.Equals(TermSet.Name)) continue;

                try
                {
                    nextTerm = nextLevelTerms[step];
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    WBLogging.Generic.Verbose("WBTaxonomy.GetSelectedTermByPath(): The next step in path clearly doesn't exist: " + step + " Exception message: " + exception.Message);

                    if (createIfNew)
                    {
                        if (nextTerm == null)
                        {
                            // so we need to create a top level term in the term set:
                            nextTerm = TermSet.CreateTerm(step, WorkBox.LOCALE_ID_ENGLISH);
                        }
                        else
                        {
                            nextTerm = nextTerm.CreateTerm(step, WorkBox.LOCALE_ID_ENGLISH);
                        }

                        taxonomyNeedsSaving = true;
                        WBLogging.Generic.Verbose("WBTaxonomy.GetSelectedTermByPath(): Created new term: " + nextTerm.WBxFullPath());
                    }
                    else
                    {
                        nextTerm = null;
                        break;
                    }
                }

                nextLevelTerms = nextTerm.Terms;
            }

            if (taxonomyNeedsSaving)
                CommitAll();

            return nextTerm;
        }

        public WBRecordsType GetSelectedRecordsType(String path)
        {
            if (!TermSet.Name.Equals(WorkBox.TERM_SET_NAME__RECORDS_TYPES)) return null;
            Term selectedTerm = GetSelectedTermByPath(path);
            if (selectedTerm == null) return null;
            return new WBRecordsType(this, selectedTerm);
        }

        public WBTeam GetSelectedTeam(String path)
        {
            if (!TermSet.Name.Equals(WorkBox.TERM_SET_NAME__TEAMS)) return null;
            Term selectedTerm = GetSelectedTermByPath(path);
            if (selectedTerm == null) return null;
            return new WBTeam(this, selectedTerm);
        }



        #endregion
    }
}
