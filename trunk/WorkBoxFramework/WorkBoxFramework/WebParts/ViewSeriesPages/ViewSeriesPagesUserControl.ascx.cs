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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.ViewSeriesPages
{
    public partial class ViewSeriesPagesUserControl : UserControl
    {
        protected ViewSeriesPages webPart = default(ViewSeriesPages);

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as ViewSeriesPages;

            string additionalPath = Request.QueryString["AdditionalPath"];

            if (additionalPath == null) additionalPath = "";

            if (webPart.ParentSeriesTag == null || webPart.ParentSeriesTag == "")
            {
                PageSeriesTagName.Text = "<i>(Web part not yet configured)</i>";
                return;
            }

            string fullPath = webPart.ParentSeriesTag + additionalPath;

            WBTaxonomy seriesTags = WBTaxonomy.GetSeriesTags(SPContext.Current.Site);

            WBTerm pageSeriesTag = null;

            Term pageSeriesTagTerm = seriesTags.GetSelectedTermByPath(fullPath);
            if (pageSeriesTagTerm != null)
                pageSeriesTag = new WBTerm(seriesTags, pageSeriesTagTerm);

            if (pageSeriesTag == null)
            {
                PageSeriesTagName.Text = "<i>(Could not find the series tag with path: " + fullPath + ")</i>";
                return;
            }


            PageSeriesTagName.Text = pageSeriesTag.Name;
            PageSeriesTagDescription.Text = pageSeriesTag.Description;

            string html = "<table cellspacing=\"10\" cellpadding=\"10\" class=\"seriesTags\">";

            foreach (Term child in pageSeriesTag.Term.Terms)
            {
                if (child.Terms.Count > 0)
                {
                    html = html + createTableRowForChildSeriesTag(seriesTags, additionalPath, child);
                }
                else
                {
                    html = html + createTableRowForDocument(seriesTags, child);
                }
            }

            html += "</table>";

            TableOfChildTerms.Text = html;
        }

        private string createTableRowForChildSeriesTag(WBTaxonomy seriesTags, String additionalPath, Term child)
        {
            string currentURL = Request.Url.ToString();
            int startIndex = currentURL.IndexOf("?");
            if (startIndex > 0)
            {
                currentURL = currentURL.Substring(0, startIndex);
            }

            string childURL = currentURL + "?AdditionalPath=" + additionalPath + "/" + child.Name;
            string html = "<tr class=\"seriesTags\"><td class=\"seriesTags\"><a href=\"" + childURL + "\">" + child.Name + "</a></td></tr>";

            return html;
        }

        private string createTableRowForDocument(WBTaxonomy seriesTags, Term docSeriesTerm)
        {
            WBQuery query = new WBQuery();
            WBTerm docSeriesTag = new WBTerm(seriesTags, docSeriesTerm);

            query.AddEqualsFilter(WBColumn.SeriesTag, docSeriesTag);
            query.OrderBy(WBColumn.DeclaredRecord, false);

            query.AddViewColumn(WBColumn.Name);
            query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
            query.AddViewColumn(WBColumn.DeclaredRecord);

            WBFarm farm = WBFarm.Local;

            SPListItem document = null;

            using (SPSite site = new SPSite(farm.ProtectedRecordsLibraryUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList recordsLibrary = web.GetList(farm.ProtectedRecordsLibraryUrl);

                    SPListItemCollection items = recordsLibrary.WBxGetItems(site, query);

                    if (items.Count > 0)
                    {
                        document = items[0];
                    }

                }
            }

            string docURL = "#";
            string docName = "Did not find a docuemnt for this series tag";

            if (document != null)
            {
                docURL = document.WBxGetAsString(WBColumn.EncodedAbsoluteURL);
                docName = document.WBxGetAsString(WBColumn.Name);
            }

            string html = "<tr class=\"seriesTags\"><td><img src=\"/_layouts/images/icdocx.png\"/></td><td class=\"seriesTags\"><a href=\"" + docURL + "\">" + docSeriesTag.Name + "</a></td><td>(" + docName + ")</td></tr>";

            return html;
        }
    }
}

