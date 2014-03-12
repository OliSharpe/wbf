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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using WorkBoxFramework;

namespace WBFWebParts
{
    internal class WBFWebPartsUtils
    {

        internal static bool InDMZ(SPContext context)
        {
            return false;
        }


        internal static bool OnIzziOrPublicWeb(SPContext context)
        {
            string[] izziOrPublicWeb = { "sp.izzi", "collection.izzi", "izzi", "collection", "teststagingweb", "stagingweb", "liveweb", "www.islington.gov.uk" };

            if (izziOrPublicWeb.Contains(context.Site.HostName))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        internal static String GetPublicLibraryURL(SPContext context)
        {
            WBFarm farm = WBFarm.Local;

            string[] internalSites = { "sp.izzi", "collection.izzi", "izzi", "collection" };
            string[] publicSites = { "teststagingweb", "stagingweb", "liveweb", "www.islington.gov.uk" };

            if (OnPublicSite(context))
            {
                WBLogging.Debug("On a public site");

                if (publicSites.Contains(context.Site.HostName))
                {
                    return "http://" + context.Site.HostName + "/publicrecords/library/";
                }
                else
                {
                    if (InDMZ(context))
                    {
                        return "http://www.islington.gov.uk/publicrecords/library/";
                    }
                    else
                    {
                        return farm.PublicRecordsLibraryUrl;
                    }
                }
            }
            else
            {
                WBLogging.Debug("Not on a public site");
                return farm.ProtectedRecordsLibraryUrl;
            }
        }


        internal static String GetPublicExtranetLibraryURL(SPContext context)
        {
            WBFarm farm = WBFarm.Local;

            if (InDMZ(context))
            {
                WBLogging.Debug("In DMZ");

                return "http://extranets.islington.gov.uk/records/library";
            }
            else
            {
                return "http://stagingextranets/records/library";
            }
        }


        internal static bool OnPublicSite(SPContext context)
        {
//            string [] publicSites = { "teststagingweb", "stagingweb", "liveweb", "www.islington.gov.uk" };

            string[] internalSites = { "sp.izzi", "collection.izzi" , "izzi", "collection" };

            return (!internalSites.Contains(context.Site.HostName));
        }



        internal static SPListItem GetRecord(SPSite site, SPWeb web, SPList library, String zone, String recordID)
        {
            WBQuery query = new WBQuery();

            WBQueryClause recordIDClause = new WBQueryClause(WBColumn.RecordID, WBQueryClause.Comparators.Equals, recordID);
            query.AddClause(recordIDClause);

            WBQueryClause isLiveClause = new WBQueryClause(WBColumn.LiveOrArchived, WBQueryClause.Comparators.Equals, WBColumn.LIVE_OR_ARCHIVED__LIVE);
            query.AddClause(isLiveClause);

            query.AddViewColumn(WBColumn.Name);
            query.AddViewColumn(WBColumn.Title);
            query.AddViewColumn(WBColumn.FileSize);
            query.AddViewColumn(WBColumn.FileTypeIcon);
            query.AddViewColumn(WBColumn.FileType);
            query.AddViewColumn(WBColumn.TitleOrName);
            query.AddViewColumn(WBColumn.DisplayFileSize);
            query.AddViewColumn(WBColumn.EncodedAbsoluteURL);
            //query.AddViewColumn(WBColumn.FunctionalArea);
            //query.AddViewColumn(WBColumn.OwningTeam);
            query.AddViewColumn(WBColumn.ReferenceDate);
            query.AddViewColumn(WBColumn.SourceID);

            SPListItemCollection items = library.WBxGetItems(site, query);

            if (items.Count < 1)
            {
                WBLogging.Debug("Couldn't find the document with Record ID = " + recordID);
                return null;
            }
            else
            {

                if (items.Count > 1) WBUtils.shouldThrowError("Found " + items.Count + " items that matched the query for Record ID: " + recordID);

                return items[0];
            }
        }


    }
}
