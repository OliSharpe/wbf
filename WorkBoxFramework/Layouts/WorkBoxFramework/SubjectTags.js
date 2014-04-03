/// <reference path="C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\14\TEMPLATE\LAYOUTS\MicrosoftAjax.js" />
/// <reference path="C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\14\TEMPLATE\LAYOUTS\SP.debug.js" />
/// <reference path="C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\14\TEMPLATE\LAYOUTS\SP.UI.Dialog.js" />
/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.2-vsdoc.js" />

var console = console || { log: function () { } };

var Tags = (function () {
    "use strict";

    var context, web, _btnId;

    return {

        init: function () {
        }
        ,
        setPB: function (btnId) {
            _btnId = btnId;
        }
        ,
        ShowDialog: function (sender, mode) {
            var $btn = $(sender);
            var webUrl = "";
            if (_spPageContextInfo && _spPageContextInfo.webServerRelativeUrl && _spPageContextInfo.webServerRelativeUrl != "/") {
                webUrl = _spPageContextInfo.webServerRelativeUrl;
            }

            var path = $btn.data("mmspath");
            if (!mode) mode = 1; // if no mode, assume new
            var modalUrl = webUrl + "/_layouts/WorkBoxFramework/AddSubjectTag.aspx?Mode=" + mode + "&Path=" + path;

            var options = {
                title: (mode == 1 ? "Add Subject Tag" : "Edit Subject Tag"),
                autoSize: true,
                allowMaximise: true,
                showClose: true,
                url: modalUrl,
                dialogReturnValueCallback: Function.createDelegate(null, Tags.DialogClosed)
            };

            SP.SOD.execute("sp.ui.dialog.js", "SP.UI.ModalDialog.showModalDialog", options);

            return false;
        }
        ,
        CloseDialog: function (sender) {
            SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.Cancel);
            return false;
        }
        ,
        DialogClosed: function (result, target) {
            if (result) {
                //location.reload(true);
                __doPostBack(_btnId, "");
            }
        }
        
    }

})();

ExecuteOrDelayUntilScriptLoaded(Tags.init, "sp.js");