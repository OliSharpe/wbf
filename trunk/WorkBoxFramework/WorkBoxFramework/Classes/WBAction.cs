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
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Newtonsoft.Json;

namespace WorkBoxFramework
{
    /// <summary>
    /// This class is used to model the custom ribbon actions being used by the Work Box Framework. 
    /// </summary>
    /// <remarks>
    /// In particular this class is used to allow the configuration of the actions for each of the ribbon buttons on 
    /// the Work Box tab to be different within each of the work box collections. It is also might get used for some of the configuration of the Publish Out action.
    /// <para>
    /// The WBAction class works closely with <c>WorkBoxCustomActions</c> Elements.xml definition, the <c>MaybeShowWorkBoxRibbonTools.ascx</c> delegate control and the Javascript functions:
    /// <c>WorkBoxFramework_actionIsEnabled(actionKey)</c> and <c>WorkBoxFramework_doAction(actionKey)</c>. Together all of these make the Work Box tab's buttons function
    /// as set within the settings page: <c>EditRibbonButtonsSettings.aspx</c>
    /// </para>
    /// </remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public class WBAction
    {
        #region Constants

        public const string ACTION_KEY__VIEW_PROPERTIES = "view_properties";
        public const string ACTION_KEY__EDIT_PROPERTIES = "edit_properties";
        public const string ACTION_KEY__VIEW_AUDIT_LOG = "view_audit_log";
        public const string ACTION_KEY__VIEW_ALL_INVOLVED = "view_all_involved";
        public const string ACTION_KEY__INVITE_TEAMS = "invite_teams";
        public const string ACTION_KEY__INVITE_INDIVIDUALS = "invite_individuals";
        public const string ACTION_KEY__CHANGE_OWNER = "change_owner";
        public const string ACTION_KEY__CLOSE = "close";
        public const string ACTION_KEY__REOPEN = "reopen";
        public const string ACTION_KEY__ADD_TO_FAVOURITES = "add_to_favourites";
        public const string ACTION_KEY__ADD_TO_CLIPBOARD = "add_to_clipboard";
        public const string ACTION_KEY__VIEW_CLIPBOARD = "view_clipboard";
        public const string ACTION_KEY__PASTE_FROM_CLIPBOARD = "paste_from_clipboard";

        private const string ACTION_KEY__PUBLISH_DOCUMENT = "publish_document";

        private const int NUM_OF_PROPERTIES = 13;

        #endregion


        #region Constructors

        public WBAction(String actionKey, String propertyValue)
        {
            if (actionKey == null || actionKey == "") throw new Exception("The constructor was called without an actionKey value: " + actionKey);
            ActionKey = actionKey.WBxTrim();

            SetFromPropertyValue(propertyValue);
        }

        public WBAction(String actionKey) {
            if (actionKey == null || actionKey == "") throw new Exception("The constructor was called without an actionKey value: " + actionKey);

            ActionKey = actionKey.WBxTrim();

            SetToDefaultValues();
        }


        #endregion


        #region Properties

        /// <summary>
        /// The ActionKey is used in hashtables as the key for this action and is also used as the key for this action's details
        /// in the JSON outputted associative array objects that will be used client side by the WorkBoxFramework.js library functions
        /// used to actually drive these custom actions.
        /// <para>The ActionKey should always be in lower case with no spaces and is set in the constructor.</para>
        /// </summary>
        [JsonProperty]
        public String ActionKey { get; private set; } 

        /// <summary>
        /// Holds the label for the action. The value can only be set by the initial constructor. The 
        /// value here has to be duplicated manually into the LabelText attribute for the action within 
        /// the Elements.xml file that actually defines the custom actions for the ribbon buttons.
        /// </summary>
        public String Label { get; private set; }

        /// <summary>
        /// Optionally holds the url for the image for the icon of the action. The value is only ever
        /// set by the <c>SetToDefaultValues()</c> method, although it is also saved and retrieved from the persisted properties value. 
        /// The value here has to be duplicated manually into the Image32by32
        /// attribute for the action within the Elements.xml file that actually defines the custom actions
        /// for the ribbon buttons.
        /// </summary>
        public String Image32x32Url { get; private set; }

        /// <summary>
        /// Set to false if you want this action to be disabled for all users within the given work box collection.
        /// </summary>
        [JsonProperty]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Set to true if you want to allow owners to use this command in the given work box collection. 
        /// </summary>
        public bool AllowOwnersToUse { get; set; }

        /// <summary>
        /// Set to true if you want to allow involved users to use this command in the given work box collection. 
        /// </summary>
        public bool AllowInvolvedToUse { get; set; }

        /// <summary>
        /// Set to true if you want to allow visitors to use this command in the given work box collection. 
        /// </summary>
        public bool AllowVisitorsToUse { get; set; }

        /// <summary>
        /// Usually set to true, this property when set to false will make the action simply link to the given action URL.
        /// </summary>
        [JsonProperty]
        public bool IsModal { get; set; }

        /// <summary>
        /// Set to true to show the close button on the modal dialog.
        /// </summary>
        [JsonProperty]
        public bool ShowClose { get; set; }

        /// <summary>
        /// Set to true to allow the modal dialog to be maximised.
        /// </summary>
        [JsonProperty]
        public bool AllowMaximise { get; set; }

        /// <summary>
        /// Typically this Url will point to the application page used by the modal dialog.
        /// </summary>
        [JsonProperty]
        public String ActionUrl { get; set; }

        /// <summary>
        /// The width of the modal dialog box for this action.
        /// </summary>
        [JsonProperty]
        public int Width { get; set; }

        /// <summary>
        /// The height of the modal dialog box for this action.
        /// </summary>
        [JsonProperty]
        public int Height { get; set; }

        /// <summary>
        /// Gets the string that will be saved for this action in the WBCollection properties.
        /// </summary>
        public String PropertyValue
        {
            get
            {
                List<String> properties = new List<String>();

                properties.Add(this.ActionKey);  // Just as a check value - however you probably have the key already.
                properties.Add(this.Label.WBxTrim());
                properties.Add(this.Image32x32Url);
                properties.Add(this.IsEnabled.ToString());
                properties.Add(this.AllowOwnersToUse.ToString());
                properties.Add(this.AllowInvolvedToUse.ToString());
                properties.Add(this.AllowVisitorsToUse.ToString());
                properties.Add(this.IsModal.ToString());
                properties.Add(this.ShowClose.ToString());
                properties.Add(this.AllowMaximise.ToString());
                properties.Add(this.ActionUrl.WBxTrim());
                properties.Add(this.Width.ToString());
                properties.Add(this.Height.ToString());

                return string.Join("|", properties.ToArray());
            }
        }

        /// <summary>
        /// Gets the key that will be used to store the information about this action as a property on the WBCollection
        /// </summary>
        public String PropertyKey
        {
            get
            {
                return "wbf_action_details__" + ActionKey;
            }
        }

        /// <summary>
        /// Gets the CSS class name that should be used when generating HTML views of this WBAction.
        /// </summary>
        public String CSSClassName
        {
            get
            {
                string cssActionName = ActionKey.Replace('_', '-');
                return "wbf-action-" + cssActionName;
            }
        }

        #endregion


        #region Methods

        private TableRow _editableTableRow = null;
        /// <summary>
        /// This method is used to create the set of dynamic ASP controls that are used to form the editing
        /// interface for this WBAction. Note that this same method is called whether the editing form is first
        /// being created or is being re-created in a post back request and therefore none of the values are
        /// set during this method, only the Control structure is created.
        /// </summary>
        /// <returns></returns>
        public TableRow CreateEditableTableRow()
        {
            if (_editableTableRow != null) return _editableTableRow;

            TableRow row = new TableRow();
            row.ID = MakeControlID("row");
            row.CssClass = "wbf-edit-action-row";

            Label label = new Label();
            label.Text = this.Label;
            row.WBxAddInTableCell(label);

            if (Image32x32Url != null && Image32x32Url != "")
            {
                Image image = new Image();
                image.ImageUrl = this.Image32x32Url;
                image.Width = Unit.Pixel(32);
                image.Height = Unit.Pixel(32);
                row.WBxAddInTableCell(image);
            }
            else
            {
                Label blank = new Label();
                blank.Text = "";
                row.WBxAddInTableCell(blank);
            }

            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("isEnabled"));
            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("allowOwnersToUse"));
            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("allowInvolvedToUse"));
            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("allowVisitorsToUse"));
            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("isModal"));
            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("showClose"));
            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("allowMaximise"));

            row.WBxAddWithIDInTableCell(new TextBox(), MakeControlID("actionUrl"));
            TextBox width = (TextBox)row.WBxAddWithIDInTableCell(new TextBox(), MakeControlID("Width"));
            width.Columns = 4;
            TextBox height = (TextBox)row.WBxAddWithIDInTableCell(new TextBox(), MakeControlID("Height"));
            height.Columns = 4;

            row.WBxAddWithIDInTableCell(new CheckBox(), MakeControlID("revertToDefaults"));

            _editableTableRow = row;
            return row;
        }

        /// <summary>
        /// Sets all of the values of this WBAction to their default values depending on which ActionKey
        /// has been used to create this WBAction. It is in this method that the default values of any 
        /// new actions needs to be set.
        /// </summary>
        public void SetToDefaultValues()
        {
            switch (ActionKey)
            {
                case ACTION_KEY__VIEW_PROPERTIES: 
                    {
                        Label = "View Properites";
                        Image32x32Url = "";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = true;

                        ActionUrl = "[CollectionURL]/Lists/[AllWorkBoxesListName]/DispForm.aspx?ID=[ID]";
                        Width = 700;
                        Height = 500;

                        break;
                    }

                case ACTION_KEY__EDIT_PROPERTIES:  
                    {
                        Label = "Edit Properites";
                        Image32x32Url = "";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = true;

                        ActionUrl = "[CollectionURL]/Lists/[AllWorkBoxesListName]/EditForm.aspx?ID=[ID]";
                        Width = 700;
                        Height = 500;

                        break;
                    }

                case ACTION_KEY__VIEW_AUDIT_LOG: 
                    {
                        Label = "View Audit Log";
                        Image32x32Url = "";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = true;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/ViewAuditLog.aspx";
                        Width = 700;
                        Height = 500;

                        break;
                    }

                case ACTION_KEY__VIEW_ALL_INVOLVED: 
                    {
                        Label = "View All Involved";
                        Image32x32Url = "/_layouts/images/centraladmin_security_users_32x32.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = true;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/ViewAllInvolved.aspx";
                        Width = 700;
                        Height = 500;

                        break;
                    }

                case ACTION_KEY__INVITE_TEAMS: 
                    {
                        Label = "Invite Teams";
                        Image32x32Url = "/_layouts/images/centraladmin_security_users_32x32.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/InviteTeams.aspx";
                        Width = 600;
                        Height = 200;

                        break;
                    }

                case ACTION_KEY__INVITE_INDIVIDUALS: 
                    {
                        Label = "Invite Individuals";
                        Image32x32Url = "/_layouts/images/gbsmpset.gif";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/InviteIndividuals.aspx";
                        Width = 600;
                        Height = 200;

                        break;
                    }

                case ACTION_KEY__CHANGE_OWNER: 
                    {
                        Label = "Change Owner";
                        Image32x32Url = "/_layouts/images/gbsmpset.gif";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/ChangeWorkBoxOwner.aspx";
                        Width = 600;
                        Height = 200;

                        break;
                    }

                case ACTION_KEY__CLOSE: 
                    {
                        Label = "Close";
                        Image32x32Url = "/_layouts/images/WorkBoxFramework/work-box-32.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/CloseWorkBox.aspx";
                        Width = 400;
                        Height = 200;

                        break;
                    }

                case ACTION_KEY__REOPEN: 
                    {
                        Label = "Re-Open";
                        Image32x32Url = "/_layouts/images/WorkBoxFramework/work-box-32.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/ReOpenWorkBox.aspx";
                        Width = 400;
                        Height = 200;

                        break;
                    }

                case ACTION_KEY__ADD_TO_FAVOURITES: 
                    {
                        Label = "Add To Favourites";
                        Image32x32Url = "/_layouts/images/WorkBoxFramework/work-box-32.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = true;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/AddWorkBoxToFavourites.aspx";
                        Width = 400;
                        Height = 200;

                        break;
                    }



                case ACTION_KEY__ADD_TO_CLIPBOARD:
                    {
                        Label = "Copy To Clipboard";
                        Image32x32Url = "/_layouts/images/pastehh.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = true;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/AddToClipboard.aspx";
                        Width = 600;
                        Height = 300;

                        break;
                    }

                case ACTION_KEY__VIEW_CLIPBOARD:
                    {
                        Label = "View Clipboard";
                        Image32x32Url = "/_layouts/images/pastehh.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = true;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/ViewClipboard.aspx";
                        Width = 600;
                        Height = 300;

                        break;
                    }

                case ACTION_KEY__PASTE_FROM_CLIPBOARD:
                    {
                        Label = "Paste From Clipboard";
                        Image32x32Url = "/_layouts/images/pastehh.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = true;
                        IsModal = true;
                        ShowClose = true;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/PasteFromClipboard.aspx";
                        Width = 600;
                        Height = 300;

                        break;
                    }


                // Not really use this yet - maybe never will.
                case ACTION_KEY__PUBLISH_DOCUMENT: 
                    {
                        Label = "Publish Document";
                        Image32x32Url = "/_layouts/images/WorkBoxFramework/work-box-out-32.png";

                        IsEnabled = true;
                        AllowOwnersToUse = true;
                        AllowInvolvedToUse = true;
                        AllowVisitorsToUse = false;
                        IsModal = true;
                        ShowClose = false;
                        AllowMaximise = false;

                        ActionUrl = "[WorkBoxURL]/_layouts/WorkBoxFramework/PublishDocDialogSelectDestinationPage.aspx";
                        Width = 730;
                        Height = 800;

                        break;
                    }

                default: 
                    {
                        Label = "<<NONE>>";
                        Image32x32Url = "";

                        IsEnabled = false;
                        AllowOwnersToUse = false;
                        AllowInvolvedToUse = false;
                        AllowVisitorsToUse = false;
                        IsModal = false;
                        ShowClose = false;
                        AllowMaximise = false;

                        ActionUrl = "";
                        Width = 0;
                        Height = 0;

                        break;
                    }

            }
        }

        /// <summary>
        /// Generates the uniqely identifiable ID to be used by the various control elements
        /// that make up the editing interface for this WBAction.
        /// </summary>
        /// <param name="innerName"></param>
        /// <returns></returns>
        public String MakeControlID(String innerName)
        {
            return this.WBxMakeControlID(ActionKey, innerName);
        }

        /// <summary>
        /// Once the editable table row has been created using CreateEditableTableRow() then this method
        /// can be called to set the values of the various controls to the current values of the object.
        /// </summary>
        public void SetControlValues()
        {
            if (_editableTableRow == null) throw new Exception("You can only call method SetControlValues() after an initial call to property EditableTableRow");
            TableRow row = _editableTableRow;

            CheckBox isEnabled = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("isEnabled"));
            isEnabled.Checked = IsEnabled;

            CheckBox allowOwnersToUse = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowOwnersToUse"));
            allowOwnersToUse.Checked = AllowOwnersToUse;

            CheckBox allowInvolvedToUse = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowInvolvedToUse"));
            allowInvolvedToUse.Checked = AllowInvolvedToUse;

            CheckBox allowVisitorsToUse = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowVisitorsToUse"));
            allowVisitorsToUse.Checked = AllowVisitorsToUse;

            CheckBox isModal = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("isModal"));
            isModal.Checked = IsModal;

            CheckBox showClose = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("showClose"));
            showClose.Checked = ShowClose;

            CheckBox allowMaximise = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowMaximise"));
            allowMaximise.Checked = AllowMaximise;

            TextBox actionUrl = (TextBox)row.WBxFindNestedControlByID(MakeControlID("actionUrl"));
            actionUrl.Text = ActionUrl;

            TextBox width = (TextBox)row.WBxFindNestedControlByID(MakeControlID("width"));
            width.Text = Width.ToString();

            TextBox height = (TextBox)row.WBxFindNestedControlByID(MakeControlID("height"));
            height.Text = Height.ToString();

            CheckBox revertToDefaults = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("revertToDefaults"));
            revertToDefaults.Checked = false;
        }

        /// <summary>
        /// This method is called to retrieve from the ASP editing controls the values set by the user 
        /// for this WBAction and set the properites on this object appropriately.
        /// </summary>
        public void CaptureControlValues()
        {
            if (_editableTableRow == null) throw new Exception("You can only call method CaptureControlValues() after an initial call to property EditableTableRow");
            TableRow row = _editableTableRow;

            CheckBox isEnabled = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("isEnabled"));
            IsEnabled = isEnabled.Checked;

            CheckBox allowOwnersToUse = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowOwnersToUse"));
            AllowOwnersToUse = allowOwnersToUse.Checked;

            CheckBox allowInvolvedToUse = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowInvolvedToUse"));
            AllowInvolvedToUse = allowInvolvedToUse.Checked;

            CheckBox allowVisitorsToUse = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowVisitorsToUse"));
            AllowVisitorsToUse = allowVisitorsToUse.Checked;

            CheckBox isModal = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("isModal"));
            IsModal = isModal.Checked;

            CheckBox showClose = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("showClose"));
            ShowClose = showClose.Checked;

            CheckBox allowMaximise = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("allowMaximise"));
            AllowMaximise = allowMaximise.Checked;

            TextBox actionUrl = (TextBox)row.WBxFindNestedControlByID(MakeControlID("actionUrl"));
            ActionUrl = actionUrl.Text;

            TextBox width = (TextBox)row.WBxFindNestedControlByID(MakeControlID("width"));
            Width = Convert.ToInt32(width.Text);

            TextBox height = (TextBox)row.WBxFindNestedControlByID(MakeControlID("height"));
            Height = Convert.ToInt32(height.Text);

            CheckBox revertToDefaults = (CheckBox)row.WBxFindNestedControlByID(MakeControlID("revertToDefaults"));
            if (revertToDefaults.Checked) this.SetToDefaultValues();
        }

        /// <summary>
        /// Set the object's properties using the values that have been stored against the PropertyKey
        /// property of the WBCollection for this action.
        /// </summary>
        /// <param name="propertyValue"></param>
        public void SetFromPropertyValue(String propertyValue)
        {
            if (propertyValue == null || propertyValue == "")
            {
                SetToDefaultValues();
            }
            else
            {
                string[] properties = propertyValue.Split('|');
                if (properties.Length != NUM_OF_PROPERTIES)
                {
                    WBUtils.logMessage("The number of parts in this property value for the WBAction are incorrect. Dont yet know how to handle this many parts: " + properties.Length.ToString());
                    SetToDefaultValues();
                    return;
                }

                if (!ActionKey.Equals(properties[0].WBxTrim())) throw new Exception("The retrieved action key value (" + properties[0] + ") does not match the existing ActionKey value (" + ActionKey + ")");


                Label = properties[1].WBxTrim();
                Image32x32Url = properties[2].WBxTrim();

                IsEnabled = true.ToString().Equals(properties[3]);
                AllowOwnersToUse = true.ToString().Equals(properties[4]);
                AllowInvolvedToUse = true.ToString().Equals(properties[5]);
                AllowVisitorsToUse = true.ToString().Equals(properties[6]);
                IsModal = true.ToString().Equals(properties[7]);
                ShowClose = true.ToString().Equals(properties[8]);
                AllowMaximise = true.ToString().Equals(properties[9]);

                ActionUrl = properties[10].WBxTrim();
                Width = Convert.ToInt32(properties[11]);
                Height = Convert.ToInt32(properties[12]);
            }
        }

        /// <summary>
        /// Performs any necessary work to change the default values of the WBAction as set in 
        /// the WBCollection to be appropriate for the current user within the context of a specific WorkBox.
        /// </summary>
        /// <param name="workBox"></param>
        public void SpecialiseForCurrentContext(WorkBox workBox)
        {
            ActionUrl = ActionUrl.WBxReplaceTokens(workBox);

            if (IsEnabled)
            {
                // The button might be enabled in general, but maybe we should disable 
                // the button in the current context.
                //
                // Unfortunately the logic for some buttons is slightly different:
                IsEnabled = enableLogic(workBox);
            }

        }

        private bool enableLogic(WorkBox workBox)
        {
            switch (ActionKey)
            {
                case ACTION_KEY__EDIT_PROPERTIES:
                case ACTION_KEY__INVITE_TEAMS:
                case ACTION_KEY__INVITE_INDIVIDUALS:
                case ACTION_KEY__CHANGE_OWNER:
                case ACTION_KEY__CLOSE:
                    {
                        if (!workBox.IsOpen) return false;
                        break;
                    }

                case ACTION_KEY__REOPEN:
                    {
                        if (!workBox.IsClosed) return false;
                        break;
                    }
            }

            if (workBox.Web.CurrentUser == null) return false;

            if (workBox.CurrentUserIsBusinessAdmin() || workBox.CurrentUserIsSystemAdmin()) return true;

            if (workBox.CurrentUserIsOwner() && AllowOwnersToUse) return true;
            if (workBox.CurrentUserIsInvolved() && AllowInvolvedToUse) return true;
            if (workBox.CurrentUserCanVisit() && AllowVisitorsToUse) return true;

            return false; 
        }


        #endregion


        #region Static Methods
        /// <summary>
        /// Gets a list of the action keys that are used by the buttons on the 'Work Box' ribbon tab that can 
        /// have their values edited via the 'Work Box Ribbon Buttons' settings page.
        /// </summary>
        /// <returns></returns>
        public static List<String> GetKeysForEditableRibbonTabButtons()
        {
            List<String> editable = new List<String>();

            editable.Add(ACTION_KEY__VIEW_PROPERTIES);
            editable.Add(ACTION_KEY__EDIT_PROPERTIES);
            editable.Add(ACTION_KEY__VIEW_AUDIT_LOG);
            editable.Add(ACTION_KEY__VIEW_ALL_INVOLVED);
            editable.Add(ACTION_KEY__INVITE_TEAMS);
            editable.Add(ACTION_KEY__INVITE_INDIVIDUALS);
            editable.Add(ACTION_KEY__CHANGE_OWNER);
            editable.Add(ACTION_KEY__CLOSE);
            editable.Add(ACTION_KEY__REOPEN);
            editable.Add(ACTION_KEY__ADD_TO_FAVOURITES);
            editable.Add(ACTION_KEY__ADD_TO_CLIPBOARD);
            editable.Add(ACTION_KEY__VIEW_CLIPBOARD);
            editable.Add(ACTION_KEY__PASTE_FROM_CLIPBOARD);
            editable.Add(ACTION_KEY__PUBLISH_DOCUMENT);

            return editable;
        }

        #endregion

    }
}
