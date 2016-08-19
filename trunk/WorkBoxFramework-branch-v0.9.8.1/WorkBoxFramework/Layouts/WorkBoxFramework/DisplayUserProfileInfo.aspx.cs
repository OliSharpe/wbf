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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class DisplayUserProfileInfo : LayoutsPageBase
    {
        // A lot of the ideas for this page come directly from the following blog post:
        // http://pholpar.wordpress.com/2010/03/17/creating-profile-properties-and-sections-the-sharepoint-2010-way-part-2-the-code/ 

        private SPSite _site; 
        private SPServiceContext _serviceContext; 
        private UserProfileManager _profileManager; 
        private ProfileSubtypePropertyManager _profileSubtypePropertyManager; 
        private UserProfileConfigManager _userProfileConfigManager; 
        private ProfilePropertyManager _profilePropertyManager; 
        private CorePropertyManager _corePropertyManager; 
        private ProfileTypePropertyManager _profileTypePropertyManager; 
  

        protected void Page_Load(object sender, EventArgs e)
        {

            string html = "";
            string userValue = "";
            UserProfileValueCollection valueCollection = null;
            object valueObject = null;

            _site = SPContext.Current.Site;
            _serviceContext = SPServiceContext.GetContext(_site); 
            _userProfileConfigManager = new UserProfileConfigManager(_serviceContext); 
            _profilePropertyManager = _userProfileConfigManager.ProfilePropertyManager; 

            _profileManager = new UserProfileManager(_serviceContext); 
            _profileSubtypePropertyManager = _profileManager.DefaultProfileSubtypeProperties; 
            
            // if you need another profile subtype 
            //_profileSubtypePropertyManager = _profilePropertyManager.GetProfileSubtypeProperties("ProfileSubtypeName"); 
  
            _corePropertyManager = _profilePropertyManager.GetCoreProperties(); 
            _profileTypePropertyManager = _profilePropertyManager.GetProfileTypeProperties(ProfileType.User);

            UserProfile profile = _profileManager.GetUserProfile(true);

            html += "<h1>First listing out all of the property types themselves</h1>";

            html += "<h2>ProfileSubtypeProperty list</h2>";

            html += "<table cellspacing='2' border='1'>";

            html += "<tr><th>Section/Property</th><th>Name</th><th>Display Name</th><th>Type Property Name</th><th>Core Property Name</th><th>Display Order</th><th>Current User's Value</th></tr>";

            foreach (ProfileSubtypeProperty profileSubtypeProperty in _profileSubtypePropertyManager.PropertiesWithSection) 
            {
                userValue = "";
                if (!profileSubtypeProperty.IsSection)
                {
                    userValue = "<i>(none)</i>";
                    valueCollection = profile[profileSubtypeProperty.Name];
                    if (valueCollection != null) 
                    {
                        valueObject = valueCollection.Value;
                        if (valueObject != null) userValue = valueObject.ToString();
                    }
                }


                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>", 
                    profileSubtypeProperty.IsSection ? "Section" : "Property", 
                    profileSubtypeProperty.Name, 
                    profileSubtypeProperty.DisplayName, 
                    profileSubtypeProperty.TypeProperty.Name, 
                    profileSubtypeProperty.CoreProperty.Name, 
                    profileSubtypeProperty.DisplayOrder,
                    userValue); 
            }

            html += "</table>";

            html += "<h2>ProfileTypeProperty list</h2>";

            html += "<table cellspacing='2' border='1'>";

            html += "<tr><th>Section/Property</th><th>Name</th><th>Core Property Name</th></tr>";

            foreach (ProfileTypeProperty profileTypeProperty in _profileTypePropertyManager.PropertiesWithSection) 
            { 
                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", 
                    profileTypeProperty.IsSection ? "Section" : "Property", 
                    profileTypeProperty.Name, 
                    profileTypeProperty.CoreProperty.Name); 
            }

            html += "</table>";

            html += "<h2>CoreProperty list</h2>";
            html += "<table cellspacing='2' border='1'>";

            html += "<tr><th>Section/Property</th><th>(Core Property) Name</th><th>Display Name</th><th>Type</th></tr>";

            foreach (CoreProperty coreProperty in _corePropertyManager.PropertiesWithSection) 
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>",
                    coreProperty.IsSection ? "Section" : "Property",
                    coreProperty.Name,
                    coreProperty.DisplayName,
                    coreProperty.Type); //, 
                    //coreProperty.UseCount 
                    //"BUG!" ); 
            }

            html += "</table>";

            UserProfileValueCollection values = profile[PropertyConstants.PictureUrl];          
            if (values.Count > 0)         
            {            
                // Author Image: {37A5CA4C-7621-44d7-BF3B-583F742CE52F}             
                
                SPFieldUrlValue urlValue = new SPFieldUrlValue(values.Value.ToString());

                html += "<p><b>PictureUrl = " + urlValue.Url + "</b></p>";

            }

            html += "<p><b>User account = " + profile[PropertyConstants.AccountName].Value.ToString() + "</b></p>";


            Content.Text = html;

        }
    }
}
