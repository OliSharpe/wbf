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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using System.IO;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class TestDocumentPicker : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void UploadPicture_OnClick(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(FileToLoad.Text)) return;


            try 
            {
                SPSite _site = SPContext.Current.Site;
                SPServiceContext _serviceContext = SPServiceContext.GetContext(_site);
                UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
                UserProfile profile = _profileManager.GetUserProfile(true);

                string filename = FileToLoad.Text;
                if (File.Exists(filename))
                {
                    FileStream fileStream = File.OpenRead(filename);
                    BinaryReader reader = new BinaryReader(fileStream);

                    int length = (int)new FileInfo(filename).Length;
                    byte[] byteArray = reader.ReadBytes(length);

//                    profile.
                }
            }
            catch (Exception error)
            {
                WBLogging.Debug("An error occurred: " + error.Message);
            }



        }

    }
}
