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
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ViewSubjectPages
{
    public partial class ViewSubjectPagesUserControl : UserControl
    {
        protected ViewSubjectPages webPart = default(ViewSubjectPages);

        private WBColumn sortColumn = null;
        private bool ascending = false;
        private String pickedLetter = "";

        private String recordsTypeFullPath = "";
        private const String NO_RECORDS_TYPE_SELECTED = "NONE_SELECTED";

        private bool viewingALetter = false;

        public bool showAtoZ = true;
        public bool showFilters = false;
        public bool onRootOfAtoZ = false;

        bool foundChildSubjectTags = false;

        private List<String> allUsedTerms = new List<String>();

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as ViewSubjectPages;

            DocumentsForSubject.AllowSorting = true;
            DocumentsForSubject.Sorting += new GridViewSortEventHandler(DocumentsForSubject_Sorting);

            DocumentsForSubject.AllowPaging = true;
            DocumentsForSubject.PageIndexChanging += new GridViewPageEventHandler(DocumentsForSubject_PageIndexChanging);
            DocumentsForSubject.PagerSettings.Mode = PagerButtons.Numeric;
            DocumentsForSubject.PagerSettings.Position = PagerPosition.Bottom;
            DocumentsForSubject.PagerSettings.PageButtonCount = 20;
            DocumentsForSubject.PagerSettings.Visible = true;
            DocumentsForSubject.PageSize = 20;

            // this odd statement is required in order to get the pagination to work with an SPGridView!
            DocumentsForSubject.PagerTemplate = null;


            showAtoZ = webPart.ShowAToZ;
            showFilters = false;

            string additionalPath = "";

            if (String.IsNullOrEmpty(webPart.RootSubjectTag))
            {
                PageName.Text = "<i>(Web part not yet configured)</i>";
                return;
            }

            // Let's capture the information about what we should be looking at:
            pickedLetter = Request.QueryString["Letter"];
            if (!String.IsNullOrEmpty(pickedLetter)) viewingALetter = true;

            additionalPath = Request.QueryString["AdditionalPath"];
            if (additionalPath == null) additionalPath = "";

            recordsTypeFullPath = Request.QueryString["RecordsType"];
            if (String.IsNullOrEmpty(recordsTypeFullPath)) recordsTypeFullPath = NO_RECORDS_TYPE_SELECTED;

            if (webPart.ShowRecordTypes)
            {
                WBLogging.Debug("ViewSubjectPages: Using records types. Currently set as: " + recordsTypeFullPath);
            }
            else
            {
                WBLogging.Debug("ViewSubjectPages: Not using records types");
            }
            
            FullSubjectTagPath = webPart.RootSubjectTag + additionalPath;
            WBLogging.Debug("FullSubjectTagPath = " + FullSubjectTagPath);

            WBTaxonomy subjectTags = WBTaxonomy.GetSubjectTags(SPContext.Current.Site);

            Term rootSubjectTagTerm = subjectTags.GetSelectedTermByPath(webPart.RootSubjectTag);
            WBTerm rootSubjectTag = null;

            if (rootSubjectTagTerm != null)
                rootSubjectTag = new WBTerm(subjectTags, rootSubjectTagTerm);

            if (rootSubjectTag == null)
            {
                PageName.Text = "<i>(Could not find the root subject tag with path: " + webPart.RootSubjectTag + ")</i>";
                return;
            }


            Term pageSubjectTagTerm = subjectTags.GetSelectedTermByPath(FullSubjectTagPath);
            WBTerm pageSubjectTag = null;

            if (pageSubjectTagTerm != null)
                pageSubjectTag = new WBTerm(subjectTags, pageSubjectTagTerm);

            if (pageSubjectTag == null)
            {
                PageName.Text = "<i>(Could not find the page subject tag with path: " + FullSubjectTagPath + ")</i>";
                return;
            }


            string html = "";

            string recordsTypeParameter = "";
            if (webPart.ShowRecordTypes)
            {
                recordsTypeParameter = "&RecordsType=" + recordsTypeFullPath;
            }

            //webPart.OnlyTermsWithDocuments = false;

            if (webPart.OnlyTermsWithDocuments)
            {
                BuildListOfAllowedTerms();
            }

            if (viewingALetter)
            {
                PageName.Text = "<a href='?'>" + webPart.RootSubjectTag + "</a> - " + pickedLetter;
                PageSubjectTagDescription.Text = "You are viewing a list of all of the subjects with the letter '" + pickedLetter + "'.";

                Dictionary<String, String> allTermPaths = new Dictionary<string, string>();
                AddAllTermsThatMatch(pickedLetter, "", rootSubjectTagTerm, allTermPaths);

                if (allTermPaths.Count > 0)
                {
                    List<String> terms = new List<String>(allTermPaths.Keys);
                    terms.Sort();

                    List<String> termsWithLetterFirst = new List<String>();
                    List<String> remainingTerms = new List<String>();

                    foreach (String term in terms)
                    {
                        if (term.IndexOf(pickedLetter) == 0)
                        {
                            termsWithLetterFirst.Add(term);
                        }
                        else
                        {
                            remainingTerms.Add(term);
                        }
                    }

                    foreach (String term in termsWithLetterFirst)
                    {
                        html += "<div class='lbi-a-to-z-child-subject'><a href='?AdditionalPath=" + allTermPaths[term] + recordsTypeParameter + "'>" + term + "</a></div>\n";
                    }

                    if (remainingTerms.Count > 0)
                    {
                        html += "<div>&nbsp;</div>\n";
                        foreach (String term in remainingTerms)
                        {
                            html += "<div class='lbi-a-to-z-child-subject'><a href='?AdditionalPath=" + allTermPaths[term] + recordsTypeParameter + "'>" + term + "</a></div>\n";
                        }
                    }

                }
                else
                {
                    html += "<p>There were no terms found for the letter '" + pickedLetter + "'.</p>";
                }
            }
            else
            {
                List<String> names = new List<String>(FullSubjectTagPath.Split('/'));
                List<String> path = new List<String>(names);
                path.RemoveAt(0);


                String justRecordsType = "";
                if (!String.IsNullOrEmpty(recordsTypeFullPath) && recordsTypeFullPath != NO_RECORDS_TYPE_SELECTED)
                {
                    justRecordsType = GetJustRecordsTypeName(recordsTypeFullPath);
                }

                PageName.Text = BuildPageNamePath(names, path, justRecordsType);

                PageSubjectTagDescription.Text = pageSubjectTag.Description;

                foundChildSubjectTags = false;

                onRootOfAtoZ = (webPart.ShowAToZ && pageSubjectTag.Name == webPart.RootSubjectTag);

                bool showDocuments = true;

                if (!onRootOfAtoZ)
                {
                    // OK so now we need to find the sub terms and put them in order.
                    List<String> termLabels = new List<String>();
                    foreach (Term child in pageSubjectTag.Term.Terms)
                    {
                        if (child.IsAvailableForTagging && CheckTermIsAllowed(child))
                        {
                            termLabels.Add(child.Name);
                            foundChildSubjectTags = true;
                        }
                    }

                    if (termLabels.Count > 0)
                    {
                        showDocuments = false;

                        termLabels.Sort();

                        foreach (String label in termLabels)
                        {
                            html += "<div class='lbi-a-to-z-child-subject'><a href='?AdditionalPath=" + additionalPath + "/" + label + recordsTypeParameter + "'>" + label + "</a></div>\n";
                        }
                    }
                    else
                    {
                        // OK so there are no further subject tags, but should we be presenting records types:
                        if (webPart.ShowRecordTypes)
                        {
                            if (recordsTypeFullPath == NO_RECORDS_TYPE_SELECTED)
                            {
                                Dictionary<String,String> recordsTypesToList = FindRecordsTypesToList();
                                if (recordsTypesToList.Count > 0)
                                {
                                    showDocuments = false;

                                    List<String> justRecordsTypes = new List<String>(recordsTypesToList.Keys);
                                    justRecordsTypes.Sort();

                                    foreach (String recordsType in justRecordsTypes)
                                    {
                                        String recordsTypePath = recordsTypesToList[recordsType];

                                        html += "<div class='lbi-a-to-z-child-subject'><a href='?AdditionalPath=" + additionalPath + "&RecordsType=" + recordsTypePath + "'>" + recordsType + "</a></div>\n";
                                    }
                                }

                            }
                        }
                    }
                }

                if (webPart.HideDocumentsOnRootPage && pageSubjectTag.Name == webPart.RootSubjectTag) showDocuments = false;

                if (showDocuments)
                {
                    RefreshBoundDocumentsList();
                }
            }

            TableOfChildSubjects.Text = html;
        }

        private bool CheckTermIsAllowed(Term term)
        {
            if (webPart.OnlyTermsWithDocuments)
            {
                if (this.allUsedTerms.Contains(term.WBxFullPath()))
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }


        public void AddAllTermsThatMatch(String pickedLetter, String pathToHere, Term term, Dictionary<String, String> allTermPaths)
        {
            if (term.TermsCount > 0)
            {
                foreach (Term child in term.Terms)
                {
                    if (child.IsAvailableForTagging && CheckTermIsAllowed(child))
                    {
                        bool include = false;

                        string[] nameParts = child.Name.Split(' ');

                        foreach (string namePart in nameParts)
                        {
                            if (namePart.Length > 0 && (namePart[0] == pickedLetter[0]))
                            {
                                include = true;
                                break;
                            }
                        }

                        if (include)
                        {
                            string termName = child.Name;

                            int duplicate = 2;

                            while (allTermPaths.ContainsKey(termName))
                            {
                                if (duplicate > 100) throw new Exception("You have more than 100 terms with the same name - this does not make sense!!");

                                termName = String.Format("{0} ({1})", child.Name, duplicate);
                                duplicate++;
                            }

                            allTermPaths.Add(termName, pathToHere + "/" + child.Name);
                        }

                        AddAllTermsThatMatch(pickedLetter, pathToHere + "/" + child.Name, child, allTermPaths);
                    }
                }
            }
        }


        private string BuildPageNamePath(List<String> names, List<String> path, String justRecordsType)
        {
            if (names == null || names.Count == 0) return "";
            string name = names[names.Count - 1];
            string additionalPath = "/" + String.Join("/", path.ToArray());

            names.RemoveAt(names.Count - 1);

            if (names.Count == 0)
            {
                if (String.IsNullOrEmpty(additionalPath) || additionalPath == "/")
                {
                    return String.Format("<a href=\"?\">{0}</a>", name);
                }
                else
                {
                    string urlToUse = "?AdditionalPath=" + additionalPath;

                    if (!webPart.OnlyLiveRecords)
                    {
                        urlToUse += "&LiveOrArchived=" + SelectedLiveOrArchivedStatusFilter;
                    }

                    return String.Format("<a href=\"{0}\">{1}</a>",
                    urlToUse, name);
                }
            }


            if (path.Count > 0)
            {
                path.RemoveAt(path.Count - 1);
            }

            if (!String.IsNullOrEmpty(justRecordsType))
            {
                justRecordsType = " &gt;&gt; " + justRecordsType;
            }

            return String.Format("{0} &gt; <a href=\"?AdditionalPath={1}\">{2}</a>{3}",
                BuildPageNamePath(names, path, ""), additionalPath, name, justRecordsType);
        }

        /*
        private string createTableRowForChildSubjectTag(WBTaxonomy seriesTags, String additionalPath, Term child)
        {
            string currentURL = Request.Url.ToString();
            int startIndex = currentURL.IndexOf("?");
            if (startIndex > 0)
            {
                currentURL = currentURL.Substring(0, startIndex);
            }

            string childURL = currentURL + "?AdditionalPath=" + additionalPath + "/" + child.Name;

            if (!webPart.OnlyLiveRecords)
            {
                childURL += "&LiveOrArchived=" + SelectedLiveOrArchivedStatusFilter;
            }


            string html = "<tr class=\"subjectTags\"><td class=\"subjectTags\"><a href=\"" + childURL + "\">" + child.Name + "</a></td></tr>";

            return html;
        }
        */


        private int CountArchivedDocsOfThisSelection()
        {
            WBFarm farm = WBFarm.Local;

            int foundDocuments = 0;

            using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
            {
                WBTaxonomy subjectTags = WBTaxonomy.GetSubjectTags(site);
                WBTaxonomy teamsTaxonomy = WBTaxonomy.GetTeams(subjectTags);
                WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(teamsTaxonomy);

                Term pageSeriesTagTerm = subjectTags.GetSelectedTermByPath(FullSubjectTagPath);
                WBTerm localPageSubjectTag = null;
                if (pageSeriesTagTerm != null)
                    localPageSubjectTag = new WBTerm(subjectTags, pageSeriesTagTerm);

                if (localPageSubjectTag != null)
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        WBQuery query = new WBQuery();

                        WBQueryClause subjectTagClause = new WBQueryClause(WBColumn.SubjectTags, WBQueryClause.Comparators.Equals, localPageSubjectTag);
                        subjectTagClause.UseDescendants = false;
                        query.AddClause(subjectTagClause);

                        WBQueryClause isArchivedClause = new WBQueryClause(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__ARCHIVED);
                        query.AddClause(isArchivedClause);


                        WBTeam team = null;
                        if (!String.IsNullOrEmpty(webPart.FilterByOwningTeam))
                        {
                            team = teamsTaxonomy.GetSelectedTeam(webPart.FilterByOwningTeam);

                            if (team != null)
                            {
                                query.AddEqualsFilter(WBColumn.OwningTeam, team);
                            }
                        }


                        if (webPart.ShowRecordTypes && !String.IsNullOrEmpty(recordsTypeFullPath) && recordsTypeFullPath != NO_RECORDS_TYPE_SELECTED)
                        {
                            WBRecordsType recordsTypeToFilterBy = recordsTypesTaxonomy.GetSelectedRecordsType(recordsTypeFullPath);

                            if (recordsTypeToFilterBy != null)
                            {
                                query.AddEqualsFilter(WBColumn.RecordsType, recordsTypeToFilterBy);
                            }
                        }

                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.RecordID);
                        

                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        SPListItemCollection foundArchivedItems = recordsLibrary.WBxGetItems(site, query);

                        foundDocuments = foundArchivedItems.Count;
                    }
                }
                else
                {
                    WBUtils.logMessage("pageSubjectTag was null");
                }

            }

            return foundDocuments;
        }


        private void RefreshBoundDocumentsList()
        {
            WBFarm farm = WBFarm.Local;

            bool foundDocuments = false;

            using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
            {
                WBTaxonomy subjectTags = WBTaxonomy.GetSubjectTags(site);
                WBTaxonomy teamsTaxonomy = WBTaxonomy.GetTeams(subjectTags);
                WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(teamsTaxonomy);

                Term pageSeriesTagTerm = subjectTags.GetSelectedTermByPath(FullSubjectTagPath);
                WBTerm localPageSubjectTag = null;
                if (pageSeriesTagTerm != null)
                    localPageSubjectTag = new WBTerm(subjectTags, pageSeriesTagTerm);

                if (localPageSubjectTag != null)
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        WBQuery query = new WBQuery();

                        WBQueryClause subjectTagClause = new WBQueryClause(WBColumn.SubjectTags, WBQueryClause.Comparators.Equals, localPageSubjectTag);
                        subjectTagClause.UseDescendants = false;
                        query.AddClause(subjectTagClause);

                        if (webPart.OnlyLiveRecords)
                        {
                            WBQueryClause isLiveClause = new WBQueryClause(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__LIVE);
                            query.AddClause(isLiveClause);
                        }
                        else
                        {
                            string statusFilter = SelectedLiveOrArchivedStatusFilter;
                            if (statusFilter == null || statusFilter == "") statusFilter = WBColumn.LIVE_OR_ARCHIVED__LIVE;
                            if (statusFilter != "All")
                            {
                                query.AddEqualsFilter(WBColumn.LiveOrArchived, statusFilter);
                            }

                            FilterLiveStatus.CssClass = "wbf-unselected-filter";
                            FilterArchivedStatus.CssClass = "wbf-unselected-filter";
                            FilterAllStatus.CssClass = "wbf-unselected-filter";

                            if (statusFilter == WBColumn.LIVE_OR_ARCHIVED__LIVE) FilterLiveStatus.CssClass = "wbf-selected-filter";
                            if (statusFilter == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED) FilterArchivedStatus.CssClass = "wbf-selected-filter";
                            if (statusFilter == "All") FilterAllStatus.CssClass = "wbf-selected-filter";
                        }

                        WBTeam team = null;
                        if (!String.IsNullOrEmpty(webPart.FilterByOwningTeam))
                        {
                            team = teamsTaxonomy.GetSelectedTeam(webPart.FilterByOwningTeam);

                            if (team != null)
                            {
                                query.AddEqualsFilter(WBColumn.OwningTeam, team);
                            }
                        }


                        if (webPart.ShowRecordTypes && !String.IsNullOrEmpty(recordsTypeFullPath) && recordsTypeFullPath != NO_RECORDS_TYPE_SELECTED)
                        {
                            WBRecordsType recordsTypeToFilterBy = recordsTypesTaxonomy.GetSelectedRecordsType(recordsTypeFullPath);

                            if (recordsTypeToFilterBy != null)
                            {
                                query.AddEqualsFilter(WBColumn.RecordsType, recordsTypeToFilterBy);
                            }
                        }

                        /*
                        string protectiveZoneFilter = "Public"; // Request.QueryString["ProtectiveZone"];
                        if (protectiveZoneFilter != null && protectiveZoneFilter != "")
                        {
                            query.AddEqualsFilter(WBColumn.ProtectiveZone, protectiveZoneFilter);
                        }
                         * */

                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.TitleOrName);
                        query.AddViewColumn(WBColumn.FileSize);
                        query.AddViewColumn(WBColumn.FileTypeIcon);
                        query.AddViewColumn(WBColumn.FileType);
                        query.AddViewColumn(WBColumn.DisplayFileSize);
                        query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
                        query.AddViewColumn(WBColumn.LiveOrArchived);
                        query.AddViewColumn(WBColumn.RecordID);
                        
//                        query.AddViewColumn(WBColumn.OwningTeam);
                        query.AddViewColumn(WBColumn.ReferenceDate);
   //                     query.AddViewColumn(WBColumn.ReferenceID);
  //                      query.AddViewColumn(WBColumn.SeriesTag);
//                        query.AddViewColumn(WBColumn.ProtectiveZone);
                        //query.AddViewColumn(WBColumn.DeclaredRecord);
    //                    query.AddViewColumn(WBColumn.SubjectTags);

                        if (sortColumn == null)
                        {
                            sortColumn = WBColumn.ReferenceDate;
                            ascending = false;
                        }

                        if (sortColumn != null)
                            query.OrderBy(sortColumn, ascending);

                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        DataTable dataTable = recordsLibrary.WBxGetDataTable(site, query);

                        if (dataTable.Rows.Count > 0) foundDocuments = true;

                        DocumentsForSubject.DataSource = dataTable;

                        DocumentsForSubject.Columns.Clear();
                        DocumentsForSubject.Columns.Add(WBUtils.DynamicIconTemplateField(WBColumn.FileTypeIcon, WBColumn.EncodedAbsoluteURL));
                        DocumentsForSubject.Columns.Add(WBUtils.HyperLinkField(WBColumn.TitleOrName, WBColumn.EncodedAbsoluteURL, sortColumn, ascending));
                        DocumentsForSubject.Columns.Add(WBUtils.BoundField(WBColumn.FileType, sortColumn, ascending));
                        DocumentsForSubject.Columns.Add(WBUtils.BoundField(WBColumn.DisplayFileSize, sortColumn, ascending));
                        if (!webPart.OnlyLiveRecords)
                        {
                            DocumentsForSubject.Columns.Add(WBUtils.BoundField(WBColumn.LiveOrArchived, sortColumn, ascending));
                        }
                        DocumentsForSubject.Columns.Add(WBUtils.BoundField(WBColumn.ReferenceDate, sortColumn, ascending));


                        SPGroup rmManagersGroup = SPContext.Current.Web.WBxGetGroupOrNull(WBFarm.Local.RecordsManagersGroupName);

                        if (rmManagersGroup != null)
                        {
                            if (rmManagersGroup.ContainsCurrentUser)
                            {
                                List<WBColumn> valueColumns = new List<WBColumn>();
                                valueColumns.Add(WBColumn.RecordID);

                                String formatString = SPContext.Current.Web.Url + "/_layouts/WorkBoxFramework/UpdateRecordsMetadata.aspx?RecordID={0}";

                                formatString = "<a href=\"javascript: WorkBoxFramework_commandAction('" + formatString + "', 800, 600); \">(edit metadata)</a>";

                                DocumentsForSubject.Columns.Add(WBUtils.FormatStringTemplateField(formatString, valueColumns));
                            }
                        }


                        DocumentsForSubject.DataBind();

                    }
                }
                else
                {
                    WBUtils.logMessage("pageSubjectTag was null");
                }

            }


            if (foundDocuments && !webPart.OnlyLiveRecords)
            {
                showFilters = true;
            }

            if (!foundDocuments && !foundChildSubjectTags && !(onRootOfAtoZ))
            {
                int archivedDocs = 0;

                if (!webPart.OnlyLiveRecords)
                {
                    if (SelectedLiveOrArchivedStatusFilter == WBColumn.LIVE_OR_ARCHIVED__LIVE)
                    {
                        archivedDocs = this.CountArchivedDocsOfThisSelection();
                    }

                    showFilters = true;
                }


                if (archivedDocs > 0)
                {
                    DynamicNoDocumentsMessage.Text = "(No live documents have been found. There are " + archivedDocs + " archived documents of this type.)";
                }
                else
                {
                    DynamicNoDocumentsMessage.Text = "(No documents have been found)";
                }

            }
            else
            {
                DynamicNoDocumentsMessage.Text = "";
            }
        }


        private Dictionary<String, String> FindRecordsTypesToList()
        {
            WBFarm farm = WBFarm.Local;

            Dictionary<String, String> typesToList = new Dictionary<String, String>();

            using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
            {
                WBTaxonomy subjectTags = WBTaxonomy.GetSubjectTags(site);
                WBTaxonomy teamsTaxonomy = WBTaxonomy.GetTeams(subjectTags);
                WBTaxonomy recordsTypesTaxonomy = WBTaxonomy.GetRecordsTypes(teamsTaxonomy);

                Term pageSeriesTagTerm = subjectTags.GetSelectedTermByPath(FullSubjectTagPath);
                WBTerm localPageSubjectTag = null;
                if (pageSeriesTagTerm != null)
                    localPageSubjectTag = new WBTerm(subjectTags, pageSeriesTagTerm);

                if (localPageSubjectTag != null)
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        WBQuery query = new WBQuery();

                        WBQueryClause subjectTagClause = new WBQueryClause(WBColumn.SubjectTags, WBQueryClause.Comparators.Equals, localPageSubjectTag);
                        subjectTagClause.UseDescendants = false;
                        query.AddClause(subjectTagClause);

                        if (webPart.OnlyLiveRecords)
                        {
                            WBQueryClause isLiveClause = new WBQueryClause(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__LIVE);
                            query.AddClause(isLiveClause);
                        }

                            /*
                        else
                        {
                            string statusFilter = SelectedLiveOrArchivedStatusFilter;
                            if (statusFilter == null || statusFilter == "") statusFilter = WBColumn.LIVE_OR_ARCHIVED__LIVE;
                            if (statusFilter != "All")
                            {
                                query.AddEqualsFilter(WBColumn.LiveOrArchived, statusFilter);
                            }
                        }
                             */ 

                        WBTeam team = null;
                        if (!String.IsNullOrEmpty(webPart.FilterByOwningTeam))
                        {
                            team = teamsTaxonomy.GetSelectedTeam(webPart.FilterByOwningTeam);

                            if (team != null)
                            {
                                query.AddEqualsFilter(WBColumn.OwningTeam, team);
                            }
                        }

                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.Title);
                        query.AddViewColumn(WBColumn.RecordsType);

                        if (sortColumn == null)
                        {
                            sortColumn = WBColumn.ReferenceDate;
                            ascending = false;
                        }

                        if (sortColumn != null)
                            query.OrderBy(sortColumn, ascending);

                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        SPListItemCollection listOfFoundDocuments = recordsLibrary.WBxGetItems(site, query);

                        foreach (SPListItem item in listOfFoundDocuments)
                        {
                            WBRecordsType recordsType = item.WBxGetSingleTermColumn<WBRecordsType>(recordsTypesTaxonomy, WBColumn.RecordsType);
                            String justRecordsType = GetJustRecordsTypeName(recordsType.Name);

                            if (!typesToList.ContainsKey(justRecordsType))
                            {
                                typesToList.Add(justRecordsType, recordsType.FullPath);
                            }
                        }
                    }
                }
                else
                {
                    WBLogging.Debug("pageSubjectTag was null");
                }

            }

            return typesToList;
        }

        private String GetJustRecordsTypeName(String fullRecordsTypePath)
        {
            String justRecordsType = fullRecordsTypePath;
            if (justRecordsType.Contains(":"))
            {
                int split = justRecordsType.IndexOf(':');
                justRecordsType = justRecordsType.Substring(split+1);
            }

            if (justRecordsType.Contains("/"))
            {
                int split = justRecordsType.IndexOf('/');
                justRecordsType = justRecordsType.Substring(split+1);
            }

            return justRecordsType;
        }

        void DocumentsForSubject_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            WBLogging.Debug("In DocumentsForSubject_PageIndexChanging - not sure if there's anything that needs to be done!");

            DocumentsForSubject.PageIndex = e.NewPageIndex;

            checkSortState();
            RefreshBoundDocumentsList();
        }


        private void checkSortState()
        {
            sortColumn = WBColumn.GetKnownColumnByInternalName(SortExpression);

            if (GridViewSortDirection == SortDirection.Ascending)
                ascending = true;
            else
                ascending = false;

        }

        protected void DocumentsForSubject_Sorting(object sender, GridViewSortEventArgs e)
        {
            WBLogging.Debug("In DocumentsForSubject_Sorting with e.SortExpression = " + e.SortExpression);

            SortExpression = e.SortExpression;

            sortColumn = WBColumn.GetKnownColumnByInternalName(SortExpression);

            if (GridViewSortDirection == SortDirection.Ascending)
            {
                GridViewSortDirection = SortDirection.Descending;
                ascending = false;
                WBLogging.Debug("In gridView_Sorting setting to descending");
            }
            else
            {
                GridViewSortDirection = SortDirection.Ascending;
                ascending = true;
                WBLogging.Debug("In gridView_Sorting setting to ascending");
            }

            // If we're re-sorting the data let's start back on page 0:
            DocumentsForSubject.PageIndex = 0;

            RefreshBoundDocumentsList();
        }

        private SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["WBF_SortDirection"] == null)
                    ViewState["WBF_SortDirection"] = SortDirection.Descending;
                return (SortDirection)ViewState["WBF_SortDirection"];
            }
            set { ViewState["WBF_SortDirection"] = value; }
        }

        private String FullSubjectTagPath
        {
            get { return (String)ViewState["WBF_FullSubjectTagPath"];  }
            set { ViewState["WBF_FullSubjectTagPath"] = value; }
        }

        private String SortExpression
        {
            get { return (String)ViewState["WBF_SortExpression"]; }
            set { ViewState["WBF_SortExpression"] = value; }
        }



        private void BuildListOfAllowedTerms()
        {
            WBFarm farm = WBFarm.Local;

            using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
            {
                WBTaxonomy subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(site);
                WBTaxonomy teamsTaxonomy = WBTaxonomy.GetTeams(subjectTagsTaxonomy);

                Term pageSeriesTagTerm = subjectTagsTaxonomy.GetSelectedTermByPath(FullSubjectTagPath);
                WBTerm localPageSubjectTag = null;
                if (pageSeriesTagTerm != null)
                    localPageSubjectTag = new WBTerm(subjectTagsTaxonomy, pageSeriesTagTerm);

                if (localPageSubjectTag != null)
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        WBQuery query = new WBQuery();

                        //WBQueryClause subjectTagClause = new WBQueryClause(WBColumn.SubjectTags, WBQueryClause.Comparators.Equals, localPageSubjectTag);
                        //subjectTagClause.UseDescendants = true;
                        //query.AddClause(subjectTagClause);

                        if (webPart.OnlyLiveRecords)
                        {
                            WBQueryClause isLiveClause = new WBQueryClause(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__LIVE);
                            query.AddClause(isLiveClause);
                        }

                        WBTeam team = null;
                        if (!String.IsNullOrEmpty(webPart.FilterByOwningTeam))
                        {
                            team = teamsTaxonomy.GetSelectedTeam(webPart.FilterByOwningTeam);

                            if (team != null)
                            {
                                query.AddEqualsFilter(WBColumn.OwningTeam, team);
                            }
                        }


                        /*
                        string protectiveZoneFilter = "Public"; // Request.QueryString["ProtectiveZone"];
                        if (protectiveZoneFilter != null && protectiveZoneFilter != "")
                        {
                            query.AddEqualsFilter(WBColumn.ProtectiveZone, protectiveZoneFilter);
                        }
                         * */

                        query.AddViewColumn(WBColumn.Name);
                        query.AddViewColumn(WBColumn.SubjectTags);
                        
                        SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl); //"Documents"]; //farm.RecordsCenterRecordsLibraryName];

                        SPListItemCollection documents = recordsLibrary.WBxGetItems(site, query);

                        WBLogging.Debug("Got documents back");

                        WBLogging.Debug("Documents contains " + documents.Count + " items");

                        foreach (SPListItem document in documents)
                        {
                            WBTermCollection<WBTerm> subjectTags = document.WBxGetMultiTermColumn<WBTerm>(subjectTagsTaxonomy, WBColumn.SubjectTags.DisplayName);
                            foreach (WBTerm subjectTag in subjectTags)
                            {
                                string fullPath = subjectTag.FullPath;
                                if (allUsedTerms.Contains(fullPath))
                                {
                                    WBLogging.Debug("Already has term: " + fullPath + "  so not adding");
                                }
                                else
                                {
                                    WBLogging.Debug("Adding to list of allowed terms: " + fullPath);
                                    this.allUsedTerms.Add(fullPath);
                                    while (fullPath.Contains("/"))
                                    {
                                        int lastIndex = fullPath.LastIndexOf('/');
                                        fullPath = fullPath.Substring(0, lastIndex);
                                        if (allUsedTerms.Contains(fullPath))
                                        {
                                            WBLogging.Debug("Already has term: " + fullPath + "  so not adding");
                                            fullPath = "";
                                        }
                                        else
                                        {
                                            allUsedTerms.Add(fullPath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    WBUtils.logMessage("pageSubjectTag was null");
                }

            }

        }


        protected void FilterLiveStatus_OnClick(object sender, EventArgs e)
        {
            SelectedLiveOrArchivedStatusFilter = WBColumn.LIVE_OR_ARCHIVED__LIVE;
            RefreshBoundDocumentsList();
        }

        protected void FilterArchivedStatus_OnClick(object sender, EventArgs e)
        {
            SelectedLiveOrArchivedStatusFilter = WBColumn.LIVE_OR_ARCHIVED__ARCHIVED;
            RefreshBoundDocumentsList();
        }

        protected void FilterAllStatus_OnClick(object sender, EventArgs e)
        {
            SelectedLiveOrArchivedStatusFilter = "All";
            RefreshBoundDocumentsList();
        }

        private String SelectedLiveOrArchivedStatusFilter
        {
            get
            {
                string currentStatusFilter = ViewState["WBF_SelectedLiveOrArchivedStatusFilter"].WBxToString();
                if (String.IsNullOrEmpty(currentStatusFilter))
                {
                    currentStatusFilter = Request.QueryString["LiveOrArchived"];

                    if (String.IsNullOrEmpty(currentStatusFilter))
                    {
                        currentStatusFilter = WBColumn.LIVE_OR_ARCHIVED__LIVE;
                    }

                    ViewState["WBF_SelectedLiveOrArchivedStatusFilter"] = currentStatusFilter;
                }
                return currentStatusFilter;
            }
            set { ViewState["WBF_SelectedLiveOrArchivedStatusFilter"] = value; }
        }

    }
}
