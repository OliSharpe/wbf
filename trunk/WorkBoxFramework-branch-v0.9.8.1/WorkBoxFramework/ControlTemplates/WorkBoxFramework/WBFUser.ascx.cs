using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;

namespace WorkBoxFramework.ControlTemplates.WorkBoxFramework
{
    public partial class WBFUser : UserControl
    {
        #region Properties

        /// <summary>
        /// Login name of user to display
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// SharePoint user to display
        /// </summary>
        public SPUser User { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(LoginName) && User == null) return;

            EnsureChildControls();

            if (User == null)
                GetUser();
            BindUser();
        }

        /// <summary>
        /// Get the SPUser object by LoginName
        /// </summary>
        void GetUser()
        {
            User = SPContext.Current.Web.WBxEnsureUserOrNull(LoginName);
        }

        /// <summary>
        /// Bind user data to the UI
        /// </summary>
        public void BindUser()
        {
            if (User == null) return;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb site = siteCollection.OpenWeb())
                    {
                        SPServiceContext serviceContext = SPServiceContext.GetContext(SPContext.Current.Site);
                        UserProfileManager profileManager = new UserProfileManager(serviceContext);

                        lblName.Text = User.WBxToHTML(profileManager, SPContext.Current.Site.RootWeb);
                        hlEmail.Text = User.Email;
                        hlEmail.NavigateUrl = "mailto:" + User.Email;

                        if (!profileManager.UserExists(User.LoginName))
                        {
                            return;
                        }

                        UserProfile profile = profileManager.GetUserProfile(User.LoginName);

                        try
                        {
                            lblDept.Text = profile["Department"].Value != null ? profile["Department"].Value.ToString() : "";
                            lblPhone.Text = profile["WorkPhone"].Value != null ? profile["WorkPhone"].Value.ToString() : "";
                            
                            imgUserPhoto.ImageUrl = profile["PictureURL"].Value != null ? profile["PictureURL"].Value.ToString() : "/_layouts/images/O14_person_placeHolder_96.png";

                            if (String.IsNullOrEmpty(User.Email))
                            {
                                var workEmail = profile["WorkEmail"].Value != null ? profile["WorkEmail"].Value.ToString() : "";
                                hlEmail.Text = workEmail;
                                hlEmail.NavigateUrl = "mailto:" + workEmail;
                            }
                        }
                        catch
                        {
                            WBUtils.logMessage("WBFUser.ascx - BindUser - Error presenting userprofile");
                            throw;
                        }
                    }
                }
            });
        }
    }
}
