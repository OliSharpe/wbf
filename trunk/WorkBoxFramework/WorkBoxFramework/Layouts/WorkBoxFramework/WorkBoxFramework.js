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


// Copied the following from:
// http://stackoverflow.com/questions/901115/get-query-string-values-in-javascript
//
function WorkBoxFramework_getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null) 
        return "";
    else 
        return decodeURIComponent(results[1].replace(/\+/g, " ")); 
} 

// 1 = OK
// 2 = Refresh
// 3 = Redirect

function dialogReturnOKAndRefresh() {
    frameElement.commonModalDialogClose(2, "");
}

function dialogReturnOKAndRedirect(redirectUrl) {
    frameElement.commonModalDialogClose(3, redirectUrl);
}


function WorkBoxFramework_callback(dialogResult, returnValue) {

    if (dialogResult == SP.UI.DialogResult.OK) {

        if (returnValue == null || returnValue == "") {
            return;
        }

        this.statusId = SP.UI
            .Status
            .addStatus("Action Completed OK", returnValue, true);

        SP.UI.Status.setStatusPriColor(this.statusId, "green");
    }

    if (dialogResult == SP.UI.DialogResult.cancel) {

        this.statusId = SP.UI
            .Status
            .addStatus("Action Cancelled", returnValue, true);

        SP.UI.Status.setStatusPriColor(this.statusId, "blue");
    }

    if (dialogResult == SP.UI.DialogResult.invalid) {

        this.statusId = SP.UI
            .Status
            .addStatus("An Error Occurred", returnValue, true);

        SP.UI.Status.setStatusPriColor(this.statusId, "red");
    }

    // result value 2 is used to refresh the calling page:
    if (dialogResult == 2) {

        if (returnValue != null && returnValue != "") {
            var locationUrl = window.location.href;
            locationUrl = locationUrl.replace("#", "");
            var index = locationUrl.indexOf("?");
            if (index != -1) {
                locationUrl = locationUrl.substr(0, index);
            }
            locationUrl = locationUrl + returnValue;
            window.location.href = locationUrl;
        } else {
            var locationUrl = window.location.href;
            locationUrl = locationUrl.replace("#", "");
            var index = locationUrl.indexOf("?");
            if (index != -1) {
                locationUrl = locationUrl + "&ignoreThisParameter=123456789";
            }
            window.location.href = locationUrl;
        }
        
        return;
    }

    // result value 3 is used to request a redirect on return:
    if (dialogResult == 3) {

        window.location.href = returnValue;
        return;
    }





    setTimeout(WorkBoxFramework_removeStatus, 5000);
}

function WorkBoxFramework_removeStatus() {
    SP.UI.Status.removeAllStatus(true);

    statusId = '';
}

function WorkBoxFramework_relativeCommandAction(relativePageName, width, height) {
    // I don't really like this solution - but it works!
    var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/' + relativePageName;

    WorkBoxFramework_commandAction(urlValue, width, height);
}

function WorkBoxFramework_commandAction(urlValue, width, height) {

    var options = {
        url: urlValue,
        tite: 'Work Box Dialog',
        allowMaximize: false,
        showClose: true,
        width: width,
        height: height,
        dialogReturnValueCallback: WorkBoxFramework_callback
    };

    SP.UI.ModalDialog.showModalDialog(options);    
}

function WorkBoxFramework_actionIsEnabled(actionKey) {
    if (typeof wbf_json__all_actions_enable_flags !== 'undefined') {
        var allActionsEnableFlags = JSON.parse(wbf_json__all_actions_enable_flags);
        return allActionsEnableFlags[actionKey];
    } else {
        return false;
    }
}

function WorkBoxFramework_doAction(actionKey) {

    if (typeof wbf_json__all_actions_details === 'undefined') return;

    var allActionsDetails = JSON.parse(wbf_json__all_actions_details);
    var action = allActionsDetails[actionKey];

    if (action == null) {
        alert("There was no action for: " + actionKey + " This is an error. Please take a screenshot and email it to the SharePoint system administrators.");
        return;
    }

    if (action.IsModal) {
        var options = {
            url: action.ActionUrl,
            tite: 'Work Box Action',
            allowMaximize: action.AllowMaximise,
            showClose: action.ShowClose,
            width: action.Width,
            height: action.Height,
            dialogReturnValueCallback: WorkBoxFramework_callback
        };

        SP.UI.ModalDialog.showModalDialog(options);

    } else {
        window.location = action.ActionUrl;
    }
}



function WorkBoxFramework_notEnabled() {
    return false;
}



function WorkBoxFramework_PublishDoc_commandAction() {

    var ctx = SP.ClientContext.get_current();
    var selectedItemIDs = SP.ListOperation.Selection.getSelectedItems(ctx);
    var selectedListGUID = SP.ListOperation.Selection.getSelectedList();
    var selectedItemsIDsString = '';
    var itemID;
    for (itemID in selectedItemIDs) {
        selectedItemsIDsString += '|' + selectedItemIDs[itemID].id;
    }

    var allActionsDetails = JSON.parse(wbf_json__all_actions_details);
    var action = allActionsDetails['publish_document'];

    if (action == null) {
        alert("There was no action for: " + actionKey + " This is an error. Please take a screenshot and email it to the SharePoint system administrators.");
        return;
    }

    var urlValue = action.ActionUrl + '?selectedItemsIDsString=' + selectedItemsIDsString + '&selectedListGUID=' + selectedListGUID;

    if (action.IsModal) {
        var options = {
            url: urlValue,
            tite: 'Publish Document Dialog',
            allowMaximize: action.AllowMaximise,
            showClose: action.ShowClose,
            width: action.Width,
            height: action.Height,
            dialogReturnValueCallback: WorkBoxFramework_callback
        };

        SP.UI.ModalDialog.showModalDialog(options);

    } else {
        window.location = action.ActionUrl;
    }
}

function WorkBoxFramework_PublishDoc_enable() {
    var items = SP.ListOperation.Selection.getSelectedItems();
    var itemCount = CountDictionary(items);
    return (itemCount == 1) && WorkBoxFramework_actionIsEnabled('publish_document');
}


function WorkBoxFramework_AddToClipboard_commandAction() {

    var ctx = SP.ClientContext.get_current();
    var selectedItemIDs = SP.ListOperation.Selection.getSelectedItems(ctx);
    var selectedListGUID = SP.ListOperation.Selection.getSelectedList();
    var selectedItemsIDsString = '';
    var itemID;
    for (itemID in selectedItemIDs) {
        selectedItemsIDsString += '|' + selectedItemIDs[itemID].id;
    }

    var allActionsDetails = JSON.parse(wbf_json__all_actions_details);
    var action = allActionsDetails['add_to_clipboard'];

    if (action == null) {
        alert("There was no action for: " + actionKey + " This is an error. Please take a screenshot and email it to the SharePoint system administrators.");
        return;
    }

    var urlValue = action.ActionUrl + '?selectedItemsIDsString=' + selectedItemsIDsString + '&selectedListGUID=' + selectedListGUID;

    if (action.IsModal) {
        var options = {
            url: urlValue,
            tite: 'Add To Clipboard Dialog',
            allowMaximize: action.AllowMaximise,
            showClose: action.ShowClose,
            width: action.Width,
            height: action.Height,
            dialogReturnValueCallback: WorkBoxFramework_callback
        };

        SP.UI.ModalDialog.showModalDialog(options);

    } else {
        window.location = action.ActionUrl;
    }
}

function WorkBoxFramework_AddToClipboard_enable() {
    var items = SP.ListOperation.Selection.getSelectedItems();
    var itemCount = CountDictionary(items);
    return (itemCount > 0) && WorkBoxFramework_actionIsEnabled('add_to_clipboard');
}

function WorkBoxFramework_PasteFromClipboard_commandAction() {

    var folderUrl = WorkBoxFramework_getParameterByName('RootFolder');

    var allActionsDetails = JSON.parse(wbf_json__all_actions_details);
    var action = allActionsDetails['paste_from_clipboard'];

    if (action == null) {
        alert("There was no action for: " + actionKey + " This is an error. Please take a screenshot and email it to the SharePoint system administrators.");
        return;
    }

    var urlValue = action.ActionUrl + '?RootFolder=' + folderUrl;

    if (action.IsModal) {
        var options = {
            url: urlValue,
            tite: 'Paste From Clipboard Dialog',
            allowMaximize: action.AllowMaximise,
            showClose: action.ShowClose,
            width: action.Width,
            height: action.Height,
            dialogReturnValueCallback: WorkBoxFramework_callback
        };

        SP.UI.ModalDialog.showModalDialog(options);

    } else {
        window.location = action.ActionUrl;
    }
}


function WorkBoxFramework_pickAWorkBox(callbackFunction) {

    var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/WorkBoxPicker.aspx';

    var options = {
        url: urlValue,
        tite: 'Work Box Picker',
        allowMaximize: false,
        showClose: true,
        width: 600,
        height: 800,
        dialogReturnValueCallback: callbackFunction
    };

    SP.UI.ModalDialog.showModalDialog(options);
}

function WorkBoxFramework_pickANewRecordsType(callbackFunction, currentRecordsTypeUIControlValue) {

    var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/RecordsTypePicker.aspx?recordsTypeUIControlValue=' + currentRecordsTypeUIControlValue;

    var options = {
        url: urlValue,
        tite: 'Records Type Picker',
        allowMaximize: false,
        showClose: true,
        width: 600,
        height: 150,
        dialogReturnValueCallback: callbackFunction
    };

    SP.UI.ModalDialog.showModalDialog(options);
}


function WorkBoxFramework_pickAPublishedDocument(callbackFunction, protectiveZone) {

    var urlValue = L_Menu_BaseUrl + '/_layouts/WorkBoxFramework/PublishedDocumentPicker.aspx';

    var options = {
        url: urlValue,
        tite: 'Published Document Picker',
        allowMaximize: false,
        showClose: true,
        width: 900,
        height: 700,
        dialogReturnValueCallback: callbackFunction
    };

    SP.UI.ModalDialog.showModalDialog(options);
}



//
// The next part is for the dynamic flyout control of the 'Tasks' ribbon button in a work box.
//
// Inspired by blogs:
// http://www.sharepointnutsandbolts.com/2010/02/ribbon-customizations-dropdown-controls.html
// http://patrickboom.wordpress.com/2010/05/25/adding-a-custom-company-menu-tab-with-dynamic-menu-on-the-ribbon/

// variable to hold the server menu
var wbf_menuXml = "";
var wbf_callCount = 0;

var wbf_menu2Xml = "";

// This function will receive the callback from the server with the menu items.
function WorkBoxFramework_receiveTasksMenu(arg, context) {

    var splitArgs = arg.split("|");

    wbf_menuXml = splitArgs[0];
    wbf_menu2Xml = splitArgs[1];
}

function WorkBoxFramework_processCallBackError(arg, context) { 
    wbf_menuXml = WorkBoxFramework_errorMenuXml('Server Error');
}

function WorkBoxFramework_errorMenuXml(errorMessage) {

    var dynamicMenuXml = "<Menu Id='WorkBoxFramework.Menu.Menu'>"
  + "<MenuSection Id='WorkBoxFramework.Menu.Section1' DisplayMode='Menu32'>"
  + "<Controls Id='WorkBoxFramework.Menu.Section1.Controls'>";

    dynamicMenuXml += "<Button Id='DynamicButton1' "
    + "Command='DynamicButtonCommand' "
    + "MenuItemId='1' "
    + "LabelText='An error occurred' "
    + "Description='" + errorMessage + "' "
    + "ToolTipTitle='Error Message' "
    + "ToolTipDescription='An error occurred while rendering the dynamic task list. Please try again. If the error persists please contact the system administrators.' />";

  dynamicMenuXml += "</Controls>" + "</MenuSection>" + "</Menu>";

  return dynamicMenuXml;

}

function WorkBoxFramework_createNewDocumentHere(templateUrl) {

    var folderUrl = WorkBoxFramework_getParameterByName('RootFolder');

    if (folderUrl == "") {
        folderUrl = wbf__document_library_root_folder_url;
    } else {
        folderUrl = "http://" + window.location.hostname + folderUrl;
    }

    // alert("Attempting to open with templateUrl = " + templateUrl);
    // alert("Attempting to open with folderUrl = " + folderUrl);

    CoreInvoke('createNewDocumentWithProgID', templateUrl, folderUrl, 'SharePoint.OpenDocuments', false);
}

function WorkBoxFramework_triggerWebPartUpdate(guid) {

    var madeConnection = false;

    if (typeof wbf__id_of_hidden_records_type_guid_field !== 'undefined') {
        if (wbf__id_of_hidden_records_type_guid_field != null && wbf__id_of_hidden_records_type_guid_field != "") {
            var guidField = document.getElementById(wbf__id_of_hidden_records_type_guid_field);

            // we'll assume that the var wbf__id_of_hidden_submit_link has value if the other one did:
            var link = document.getElementById(wbf__id_of_hidden_submit_link);

            if (guidField != null && link != null) {
                guidField.value = guid;
                link.click();
                madeConnection = true;
            }

        } 
    }
    
    if (!madeConnection) {
        // We're going to assume that we're simply not on the team site's home page so let's try to redirect there:
        if (typeof wbf__spweb_url !== 'undefined' && wbf__spweb_url != null && wbf__spweb_url != "") {
            window.location.href = wbf__spweb_url + '?recordsTypeGUID=' + guid;
        } else {
            alert("An error has occurred in WorkBoxFramework_triggerWebPartUpdate");
        }
    }

}

// The function below is to fix the annoying issue where peopleeditors controls don't clear after a post back:
// The function is derived from the function described here.
// http: //www.sharemuch.com/2011/12/04/how-to-address-sharepoint-2010-people-editor-issue-not-clearing/

// This function clears all of the hidden data on all of the PeopleEditors on the page:
function WorkBoxFramework_clearPeopleEditors() {

    var arr = document.getElementsByTagName("div");
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].id.indexOf("upLevelDiv") > 0) {
            arr[i].innerHTML = '';
        }
    }

    arr = document.getElementsByTagName("input");
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].name.indexOf("hiddenSpanData") > 0) {
            arr[i].value = '';
        }
    }
} 


// There's probably a nicer way to do this ....
var wbf__user_presence_sips = new Object();
var wbf__user_presence_elements = new Object();
var wbf__user_presence_ids = new Array();

function WorkBoxFramework__add_user_presence(id, sip, element) {
    wbf__user_presence_sips[id] = sip;
    wbf__user_presence_elements[id] = element;
    wbf__user_presence_ids.push(id);
    // alert("On pawn load for " + sip + " to element with ID = " + id);
}

function WorkBoxFramework__do_user_presence() {

    // alert("Rendering the user presence information for the user pawns");

    for (var index in wbf__user_presence_ids) {

        var id = wbf__user_presence_ids[index];
        var sip = wbf__user_presence_sips[id];
        var element = wbf__user_presence_elements[id];

        if (sip != "") {
        //    alert("Should be loading presence pawn for " + sip + " to element with ID = " + id);
            IMNRC(sip, element);
        }
        //else {
          //  alert("User pawn element with ID = " + id + " had a blank SIP value");
        //}
    }
}

function WorkBoxFramework__add_do_user_presence_function() {
    _spBodyOnLoadFunctionNames.push("WorkBoxFramework__do_user_presence");
}

// We want to run this function when the page has finished loading:
if (typeof (_spBodyOnLoadFunctionNames) != 'undefined') {
    _spBodyOnLoadFunctionNames.push("WorkBoxFramework__add_do_user_presence_function");
}
