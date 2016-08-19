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
        internal const String WBF_WEB_PARTS__RECORDS_LIBRARY__PUBLIC = "Public Library";
        internal const String WBF_WEB_PARTS__RECORDS_LIBRARY__PROTECTED = "Protected Library";
        internal const String WBF_WEB_PARTS__RECORDS_LIBRARY__LOCAL = "Local Public Library";

        internal const String SP_SITE_PROPERTY__RECORDS_LIBRARY_TO_USE = "wbf__sp_site__wbf_web_parts__records_library_to_use";
        internal const String SP_SITE_PROPERTY__LOCAL_PUBLIC_LIBRARY_URL = "wbf__sp_site__wbf_web_parts__local_public_library_url";
        internal const String SP_SITE_PROPERTY__USE_EXTRANET_LIBRARY = "wbf__sp_site__wbf_web_parts__use_extranet_library";
        internal const String SP_SITE_PROPERTY__LOCAL_EXTRANET_LIBRARY_URL = "wbf__sp_site__wbf_web_parts__local_extranet_library_url";
        internal const String SP_SITE_PROPERTY__SHOW_FILE_ICONS = "wbf__sp_site__wbf_web_parts__show_file_icons";
        internal const String SP_SITE_PROPERTY__SHOW_KB_FILE_SIZE = "wbf__sp_site__wbf_web_parts__show_kb_file_size";
        internal const String SP_SITE_PROPERTY__SHOW_DESCRIPTION = "wbf__sp_site__wbf_web_parts__show_description";

        internal static List<String> GetRecordsLibraryOptions()
        {
            List<String> options = new List<String>();

            options.Add(WBF_WEB_PARTS__RECORDS_LIBRARY__PUBLIC);

            WBFarm farm = WBFarm.Local;
            if (!String.IsNullOrEmpty(farm.FarmInstance) && !farm.FarmInstance.Equals(WBFarm.FARM_INSTANCE__PUBLIC_EXTERNAL_FARM))
            {
                // So we're only adding the option to link to the protected library if we are not on the external public farm
                options.Add(WBF_WEB_PARTS__RECORDS_LIBRARY__PROTECTED);
            }

            options.Add(WBF_WEB_PARTS__RECORDS_LIBRARY__LOCAL);

            return options;
        }

        internal static String GetRecordsLibraryToUse(SPSite site) {
            WBFarm farm = WBFarm.Local;

            if (String.IsNullOrEmpty(farm.FarmInstance) || farm.FarmInstance.Equals(WBFarm.FARM_INSTANCE__PUBLIC_EXTERNAL_FARM))
            {
                return WBF_WEB_PARTS__RECORDS_LIBRARY__PUBLIC;
            }
            else
            {
                String libraryToUse = site.RootWeb.WBxGetProperty(SP_SITE_PROPERTY__RECORDS_LIBRARY_TO_USE);

                if (String.IsNullOrEmpty(libraryToUse))
                {
                    return WBF_WEB_PARTS__RECORDS_LIBRARY__PUBLIC;
                }
                else 
                {
                    return libraryToUse;
                }
            }
        }

        internal static void SetRecordsLibraryToUse(SPSite site, String recordsLibraryToUse)
        {
            WBFarm farm = WBFarm.Local;

            if (String.IsNullOrEmpty(farm.FarmInstance) || farm.FarmInstance.Equals(WBFarm.FARM_INSTANCE__PUBLIC_EXTERNAL_FARM))
            {
                recordsLibraryToUse = WBF_WEB_PARTS__RECORDS_LIBRARY__PUBLIC;
            }

            site.RootWeb.WBxSetProperty(SP_SITE_PROPERTY__RECORDS_LIBRARY_TO_USE, recordsLibraryToUse);
        }

        internal static void SetLocalPublicLibraryURL(SPSite site, String localPublicURL)
        {
            site.RootWeb.WBxSetProperty(SP_SITE_PROPERTY__LOCAL_PUBLIC_LIBRARY_URL, localPublicURL);
        }

        internal static String GetLocalPublicLibraryURL(SPSite site)
        {
            return site.RootWeb.WBxGetProperty(SP_SITE_PROPERTY__LOCAL_PUBLIC_LIBRARY_URL);
        }       


        internal static String GetRecordsLibraryURL(SPSite site)
        {
            WBFarm farm = WBFarm.Local;

            String libraryToUse = GetRecordsLibraryToUse(site);
            String localLibraryURL = site.RootWeb.WBxGetProperty(SP_SITE_PROPERTY__LOCAL_PUBLIC_LIBRARY_URL);

            if (libraryToUse == WBF_WEB_PARTS__RECORDS_LIBRARY__PROTECTED)
            {
                return farm.ProtectedRecordsLibraryUrl;
            }
            else if (libraryToUse == WBF_WEB_PARTS__RECORDS_LIBRARY__LOCAL && !String.IsNullOrEmpty(localLibraryURL))
            {
                return localLibraryURL;
            } 
            else 
            {
                return farm.PublicRecordsLibraryUrl; 
            }
        }       

        internal static bool UseExtranetLibrary(SPSite site)
        {
            return site.RootWeb.WBxGetBoolPropertyOrDefault(SP_SITE_PROPERTY__USE_EXTRANET_LIBRARY, false);            
        }

        internal static void SetUseExtranetLibrary(SPSite site, bool value)
        {
            site.RootWeb.WBxSetBoolProperty(SP_SITE_PROPERTY__USE_EXTRANET_LIBRARY, value);
        }

        internal static String GetExtranetLibraryURL(SPSite site)
        {
            WBFarm farm = WBFarm.Local;

            String localExtranetURL = site.RootWeb.WBxGetProperty(SP_SITE_PROPERTY__LOCAL_EXTRANET_LIBRARY_URL);

            if (String.IsNullOrEmpty(localExtranetURL))
            {
                return farm.PublicExtranetRecordsLibraryUrl;
            }
            else
            {
                return localExtranetURL;
            }
        }

        internal static void SetLocalExtranetLibraryURL(SPSite site, String localExtranetURL)
        {
            site.RootWeb.WBxSetProperty(SP_SITE_PROPERTY__LOCAL_EXTRANET_LIBRARY_URL, localExtranetURL);
        }

        internal static String GetLocalExtranetLibraryURL(SPSite site)
        {
            return site.RootWeb.WBxGetProperty(SP_SITE_PROPERTY__LOCAL_EXTRANET_LIBRARY_URL);
        }       


        internal static bool ShowKBFileSize(SPSite site)
        {
            return site.RootWeb.WBxGetBoolPropertyOrDefault(SP_SITE_PROPERTY__SHOW_KB_FILE_SIZE, false);            
        }

        internal static void SetShowKBFileSize(SPSite site, bool value)
        {
            site.RootWeb.WBxSetBoolProperty(SP_SITE_PROPERTY__SHOW_KB_FILE_SIZE, value);         
        }

        internal static bool ShowFileIcons(SPSite site)
        {
            return site.RootWeb.WBxGetBoolPropertyOrDefault(SP_SITE_PROPERTY__SHOW_FILE_ICONS, false);
        }

        internal static void SetShowFileIcons(SPSite site, bool value)
        {
            site.RootWeb.WBxSetBoolProperty(SP_SITE_PROPERTY__SHOW_FILE_ICONS, value);
        }

        internal static bool ShowDescription(SPSite site)
        {
            return site.RootWeb.WBxGetBoolPropertyOrDefault(SP_SITE_PROPERTY__SHOW_DESCRIPTION, false);
        }

        internal static void SetShowDescription(SPSite site, bool value)
        {
            site.RootWeb.WBxSetBoolProperty(SP_SITE_PROPERTY__SHOW_DESCRIPTION, value);
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
