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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using Newtonsoft.Json;
using System.Text;


namespace WorkBoxFramework.ControlTemplates.WorkBoxFramework
{
    public partial class MaybeShowWorkBoxRibbonTools : UserControl, ICallbackEventHandler
    {
        public bool isWorkBox = false;

        public String scriptForSettingGlobalVariables = "";

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                if (this.Page == null) return;
                if (SPContext.Current == null) return;

                SPRibbon currentRibbon = SPRibbon.GetCurrent(this.Page);

                WorkBox workBox = null;

                // If we're looking at a modal dialog box then we want to 
                // leave workBox == null so that no further action is taken:
                if (Request.QueryString["IsDlg"] == null || Request.QueryString["IsDlg"] != "1")
                {
                    workBox = WorkBox.GetIfWorkBox(SPContext.Current);
                }

                if (workBox != null)
                {
                    //OK so we are looking at a work box.
                    isWorkBox = true;
                    SPWeb workBoxWeb = workBox.Web;

                    scriptForSettingGlobalVariables = makeScriptForSettingWorkBoxVariables(workBox);

                    if (!currentRibbon.IsTabAvailable("WorkBoxFramework.Ribbon.WorkBox"))
                    {
                        currentRibbon.MakeTabAvailable("WorkBoxFramework.Ribbon.WorkBox");
                    }

                    // Now let's register the commands for the tasks flyout button:
                    // Inspired by blogs:
                    // http://www.sharepointnutsandbolts.com/2010/02/ribbon-customizations-dropdown-controls.html
                    // http://patrickboom.wordpress.com/2010/05/25/adding-a-custom-company-menu-tab-with-dynamic-menu-on-the-ribbon/
                    // http://www.wictorwilen.se/Post/Creating-a-SharePoint-2010-Ribbon-extension-part-2.aspx

                    WBLogging.Generic.Monitorable("About to do various for Tasks flyout menu:");

                    ScriptLink.RegisterScriptAfterUI(this.Page, "SP.Core.js", false, false);
                    ScriptLink.RegisterScriptAfterUI(this.Page, "CUI.js", false, false);
                    ScriptLink.RegisterScriptAfterUI(this.Page, "core.js", true, false);
                    ScriptLink.RegisterScriptAfterUI(this.Page, "SP.Ribbon.js", false, false);
                    ScriptLink.RegisterScriptAfterUI(this.Page, "SP.Runtime.js", false, false);
                    ScriptLink.RegisterScriptAfterUI(this.Page, "SP.js", false, false);
                    //ScriptLink.RegisterScriptAfterUI(this.Page, "WorkBoxFramework/PageComponent.js", false, true);

                    var commands = new List<IRibbonCommand>();

                    // register the command at the ribbon. Include the callback to the server to generate the xml
                    commands.Add(new SPRibbonCommand("WorkBoxFramework.Command.PopulateDynamicTasks", "if (wbf_callCount==0) WorkBoxFramework_getDynamicTasksMenu('',''); wbf_callCount++; if (wbf_callCount > 100) wbf_menuXml = WorkBoxFramework_errorMenuXml('Timeout'); if (wbf_menuXml != '') properties.PopulationXML = wbf_menuXml;"));
                    commands.Add(new SPRibbonCommand("WorkBoxFramework.Command.PopulateDynamicTemplates", "if (wbf_callCount==0) WorkBoxFramework_getDynamicTasksMenu('',''); wbf_callCount++; if (wbf_callCount > 100) wbf_menu2Xml = WorkBoxFramework_errorMenuXml('Timeout'); if (wbf_menu2Xml != '') properties.PopulationXML = wbf_menu2Xml;"));

                    //                commands.Add(new SPRibbonCommand("PopulateDynamicTasksCommand", "properties.PopulationXML = errorMenuXml();"));
                    //commands.Add(new SPRibbonCommand("PopulateDynamicTasksCommand", "alert('Callaa to Popdyn'); if (menuXml == '') { CreateServerMenu('',''); } else { properties.PopulationXML = menuXml; }"));
                    //                commands.Add(new SPRibbonCommand("PopulateDynamicTasksCommand", "alert('Call to Popdyn: ' + menuXml); properties.PopulationXML = menuXml;"));

                    //Register various:
                    var manager = new SPRibbonScriptManager();

                    // Register ribbon scripts
                    manager.RegisterGetCommandsFunction(Page, "getGlobalCommands", commands);
                    manager.RegisterCommandEnabledFunction(Page, "commandEnabled", commands);
                    manager.RegisterHandleCommandFunction(Page, "handleCommand", commands);

                    WBLogging.Generic.Monitorable("Registered ribbon scripts");


                    //Register initialize function
                    var methodInfo = typeof(SPRibbonScriptManager).GetMethod("RegisterInitializeFunction", BindingFlags.Instance | BindingFlags.NonPublic);
                    methodInfo.Invoke(manager, new object[] { Page, "InitPageComponent", "/_layouts/WorkBoxFramework/PageComponent.js", false, "WorkBoxFramework.PageComponent.initialize()" });


                    // register the client callbacks so that the JavaScript can call the server. 
                    ClientScriptManager cm = this.Page.ClientScript;

                    String cbReference = cm.GetCallbackEventReference(this, "arg", "WorkBoxFramework_receiveTasksMenu", "", "WorkBoxFramework_processCallBackError", false);
                    String callbackScript = "function WorkBoxFramework_getDynamicTasksMenu(arg, context) {" + cbReference + "; }";
                    WBLogging.Generic.Verbose("Creating the call back function WorkBoxFramework_getDynamicTasksMenu to call: \n" + callbackScript);
                    cm.RegisterClientScriptBlock(this.GetType(), "WorkBoxFramework_getDynamicTasksMenu", callbackScript, true);



                    // Now let's check or set the last visited Guid:
                    SPSite _site = SPContext.Current.Site;
                    SPServiceContext _serviceContext = SPServiceContext.GetContext(_site);
                    UserProfileManager _profileManager = new UserProfileManager(_serviceContext);
                    UserProfile profile = _profileManager.GetUserProfile(true);

                    UserProfileValueCollection lastVisitedGuidUserProfileValueCollection = profile[WorkBox.USER_PROFILE_PROPERTY__WORK_BOX_LAST_VISITED_GUID];
                    bool needsUpdating = false;
                    if (lastVisitedGuidUserProfileValueCollection == null || lastVisitedGuidUserProfileValueCollection.Count == 0)
                    {
                        needsUpdating = true;
                    }
                    else
                    {
                        Guid lastGuid = new Guid(lastVisitedGuidUserProfileValueCollection.Value.ToString());

                        if (!lastGuid.Equals(workBoxWeb.ID)) needsUpdating = true;
                    }

                    if (needsUpdating)
                    {
                        workBoxWeb.AllowUnsafeUpdates = true;

                        string currentGuidString = workBoxWeb.ID.ToString();
                        lastVisitedGuidUserProfileValueCollection.Clear();
                        lastVisitedGuidUserProfileValueCollection.Add(currentGuidString);

                        // OK now we're going to make sure that this work box is the latest on the list of recently visited work boxes:
                        WBUtils.logMessage("Updating the list of recently visited work boxes - as we've just come to this work box");
                        UserProfileValueCollection workBoxesRecentlyVisited = profile[WorkBox.USER_PROFILE_PROPERTY__MY_RECENTLY_VISITED_WORK_BOXES];


                        string mostRecentWorkBoxDetails = workBoxWeb.Title + "|" + workBoxWeb.Url + "|" + workBox.UniqueID + "|" + workBoxWeb.ID.ToString() + "|" + DateTime.Now.Ticks;
                        WBUtils.logMessage("The most recent work box details are: " + mostRecentWorkBoxDetails);

                        List<String> newList = new List<String>();
                        newList.Add(mostRecentWorkBoxDetails);

                        if (workBoxesRecentlyVisited.Value != null)
                        {
                            string[] recentWorkBoxes = workBoxesRecentlyVisited.Value.ToString().Split(';');
                            int count = 0;
                            int totalLength = 0;
                            foreach (string recentWorkBox in recentWorkBoxes)
                            {
                                count++;
                                if (count > 15 || totalLength >= 3000) break;
                                if (!recentWorkBox.Contains(currentGuidString))
                                {
                                    newList.Add(recentWorkBox);
                                    totalLength += recentWorkBox.Length + 1;
                                }
                            }
                        }

                        profile[WorkBox.USER_PROFILE_PROPERTY__MY_RECENTLY_VISITED_WORK_BOXES].Value = String.Join(";", newList.ToArray());

                        profile.Commit();
                        workBoxWeb.AllowUnsafeUpdates = false;
                    }


                }
                else
                {
                    scriptForSettingGlobalVariables = makeScriptForSettingNonWorkBoxVariables(SPContext.Current.Web);
                    if (currentRibbon.IsTabAvailable("WorkBoxFramework.Ribbon.WorkBox"))
                    {
                        currentRibbon.TrimById("WorkBoxFramework.Ribbon.WorkBox");
                    }
                }

                //          currentRibbon.MakeContextualGroupInitiallyVisible("WorkBoxFramework.Ribbon.ContextualGroup", string.Empty);
            }
            catch (Exception exception)
            {
                // If this isn't working - let's just do nothing so that at least the SharePoint site is visible.
            }
        }

        private String makeScriptForSettingWorkBoxVariables(WorkBox workBox)
        {
            string htmlForScript = "<script type=\"text/javascript\">\n";

            Dictionary<String, WBAction> allActions = workBox.GetAllActions();

            Dictionary<String, bool> allEnableFlags = new Dictionary<String, Boolean>();
            foreach (WBAction action in allActions.Values)
            {
                allEnableFlags.Add(action.ActionKey, action.IsEnabled);
            }

            htmlForScript += makeVarDeclaration("wbf_json__all_actions_details", JsonConvert.SerializeObject(allActions));
            htmlForScript += makeVarDeclaration("wbf_json__all_actions_enable_flags", JsonConvert.SerializeObject(allEnableFlags));

            htmlForScript += makeVarDeclaration("wbf__enable_tasks_button", false);
            htmlForScript += makeVarDeclaration("wbf__enable_document_templates_button", (workBox.DocumentTemplates != null));
            htmlForScript += makeVarDeclaration("wbf__document_library_root_folder_url", workBox.Web.Url + "/" + workBox.DocumentLibrary.RootFolder.Url);

            htmlForScript += makeVarDeclaration("wbf__spweb_url", workBox.Web.Url);

            htmlForScript += "</script>\n";
            return htmlForScript;
        }


        private String makeScriptForSettingNonWorkBoxVariables(SPWeb web)
        {
            string htmlForScript = "<script type=\"text/javascript\">\n";

            htmlForScript += makeVarDeclaration("wbf__spweb_url", web.Url);

            htmlForScript += "</script>\n";
            return htmlForScript;
        }


        private String makeVarDeclaration(String varName, String varValue)
        {
            // Double escaping the string value:
            varValue = varValue.Replace("\\", "\\\\");
            varValue = varValue.Replace("\"", "\\\"");

            return string.Format("var {0} = \"{1}\";\n", varName, varValue);
        }

        private String makeVarDeclaration(String varName, bool truthValue)
        {
            if (truthValue) return string.Format("var {0} = true;\n", varName);
            return string.Format("var {0} = false;\n", varName);
        }



        string ICallbackEventHandler.GetCallbackResult()
        {
            //return "";

            Random random = new Random();
            int items = random.Next(3) + 1;

            WBLogging.Generic.Monitorable("In call to GetCallbackResult() for the flyout menu");


  string dynamicMenuXml = "<Menu Id='WorkBoxFramework.Menu.Menu'>"
  + "<MenuSection Id='WorkBoxFramework.Menu.Section1' DisplayMode='Menu32'>"
  + "<Controls Id='WorkBoxFramework.Menu.Section1.Controls'>";
            
  for (int i = 0; i < items; i++)
  {
      dynamicMenuXml += String.Format(
        "<Button Id='DynamicButton{0}' "
        + "Command='DynamicButtonCommand' "
        + "MenuItemId='{0}' "
        + "Image16by16=\"/_layouts/images/WorkBoxFramework/Task_16x16.gif\" "
        + "Image32by32=\"/_layouts/images/WorkBoxFramework/Task_32x32.gif\" "
        + "LabelText='My Custom menu {0}' "
        + "Description='This is the description' "
        + "ToolTipTitle='My Custom menu {0}' "
        + "ToolTipDescription='Dynamic Button' />", i);
  }
              
  dynamicMenuXml += "</Controls>" + "</MenuSection>" + "</Menu>";


  WBLogging.Debug("The dynamic XML = \n" + dynamicMenuXml);


  return dynamicMenuXml + "|" + makeTemplatesMenu();

        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            // Do nothing!
        }



        private string makeTemplatesMenu()
        {
            WBLogging.Debug("In makeTemplatesMenu(): Start");

            // OK so a first simple attempt to build up the correct info into this dynamic menu:

            WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);

            if (workBox == null)
            {
                WBLogging.Debug("In makeTemplatesMenu(): workBox was null!");
                return makeNoTemplatesMenu("workBox was null");
            }

            SPDocumentLibrary templatesLibrary = workBox.DocumentTemplates;

            if (templatesLibrary == null)
            {
                WBLogging.Debug("In makeTemplatesMenu(): templatesLibrary was null!");
                return makeNoTemplatesMenu("templatesLibrary was null");
            }

            StringBuilder dynamicMenuXml = new StringBuilder("<Menu Id='WorkBoxFramework.DocumentTemplates.Menu'>");
                dynamicMenuXml.Append("<MenuSection Id='WorkBoxFramework.DocumentTemplates.Menu.Section' DisplayMode='Menu16'>");
                dynamicMenuXml.Append("<Controls Id='WorkBoxFramework.DocumentTemplates.Menu.Section.Controls'>");

            SPFolder rootFolder = templatesLibrary.RootFolder;

            addFolderContents(dynamicMenuXml, rootFolder, true);

            dynamicMenuXml.Append("</Controls> </MenuSection> </Menu>");

            string finalXML = dynamicMenuXml.ToString();
            WBLogging.Debug("In makeTemplatesMenu(): Finished creating XML: " + finalXML);

            return finalXML;
        }

        private void addFolderContents(StringBuilder dynamicMenuXml, SPFolder folder, bool includeNextLevel)
        {
            WBLogging.Debug("In addFolderContents(): " + folder.Name + "   URL: " + folder.Url);

            if (includeNextLevel)
            {
                WBLogging.Debug("In addFolderContents(): including sub folders");
                foreach (SPFolder subFolder in folder.SubFolders)
                {
                    if (subFolder.Name.ToString() != "Forms")
                    {
                        addSubFolder(dynamicMenuXml, subFolder);
                    }
                }
            }
            else
            {
                WBLogging.Debug("In addFolderContents(): NOT including sub folders");
            }
           
            foreach (SPFile file in folder.Files)
            {
                string titleOrName = file.Title;
                if (String.IsNullOrEmpty(titleOrName))
                {
                    titleOrName = file.Name;
                }

                // This will either get a description or return a blank string:
                string description = file.Item.WBxGetColumnAsString("Description");

                // Make sure that the description doesn't prematurely terminate the javascript:
                description = description.Replace("'", " ").Replace("\"", " ");


                WBLogging.Debug("In addFolderContents(): adding a file: " + file.Name);
                WBLogging.Debug("In addFolderContents(): description = " + description);

                dynamicMenuXml.Append(String.Format(
                  "<Button Id='DynamicButton{0}' "
                  + "Command='DynamicButtonCommand' "
                  + "MenuItemId='{1}' "
                  + "Image16by16=\"/_layouts/images/{5}\" "
                  + "LabelText='{2}' "
                  + "Description='{3}' "
                  + "ToolTipTitle='{3}' "
                  + "ToolTipDescription='{4}' />", file.Item.ID, (String)file.Item[SPBuiltInFieldId.EncodedAbsUrl], titleOrName, file.Name, description, file.IconUrl));
            }
        }

        private void addSubFolder(StringBuilder dynamicMenuXml, SPFolder subFolder)
        {
            dynamicMenuXml.Append("<FlyoutAnchor ");
            dynamicMenuXml.Append(" Id=\"Ribbon.Flyoutanchor\" ");
            dynamicMenuXml.Append(" LabelText=\"").Append(subFolder.Name).Append("\" ");
            dynamicMenuXml.Append(" Command=\"DoNothingCommand\" ");
            dynamicMenuXml.Append(" CommandType=\"IgnoredByMenu\" ");
            dynamicMenuXml.Append(" Image16by16=\"/_layouts/images/folder.gif\" ");
            dynamicMenuXml.Append(" > ");
            dynamicMenuXml.Append(" <Menu Id=\"Ribbon.Flyoutanchor.Menu\"> ");
            dynamicMenuXml.Append(" <MenuSection Id=\"Ribbon.Flyoutanchor.Menu.MenuSection  DisplayMode='Menu16'\"> ");
            dynamicMenuXml.Append("  <Controls Id=\"Ribbon.Flyoutanchor.Menu.MenuSection.Controls\"> ");

            addFolderContents(dynamicMenuXml, subFolder, false);

            dynamicMenuXml.Append("   </Controls> ");
            dynamicMenuXml.Append("   </MenuSection> ");
            dynamicMenuXml.Append("  </Menu> ");
            dynamicMenuXml.Append("   </FlyoutAnchor> ");
        }

        private String makeNoTemplatesMenu(String message)
        {
            string dynamicMenuXml = "<Menu Id='WorkBoxFramework.DocumentTemplates.Menu'>"
    + "<MenuSection Id='WorkBoxFramework.DocumentTemplates.Menu.Section' DisplayMode='Menu16'>"
    + "<Controls Id='WorkBoxFramework.DocumentTemplates.Menu.Section.Controls'>";

                dynamicMenuXml += String.Format(
                  "<Button Id='DynamicButton{0}' "
                  + "Command='DynamicButtonCommand' "
                  + "MenuItemId='{0}' "
                  + "LabelText='{1}' "
                  + "Description='{2}' "
                  + "ToolTipTitle='{1}' "
                  + "ToolTipDescription='{2} (ID={0})' />", 0, "No Templates Found", message);

            dynamicMenuXml += "</Controls>" + "</MenuSection>" + "</Menu>";

            return dynamicMenuXml;
        }

    }
}
