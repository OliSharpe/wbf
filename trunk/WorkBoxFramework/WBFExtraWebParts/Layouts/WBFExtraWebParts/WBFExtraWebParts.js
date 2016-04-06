/* Javascript functions used by the WBF Extra Web Parts solution */



function WBF_editBlockButtons(callbackFunction, currentBlockButtonsDetailsHiddenFieldID) {

    var blockButtonsDetails = $("#" + currentBlockButtonsDetailsHiddenFieldID);

    var urlValue = L_Menu_BaseUrl + '/_layouts/WBFExtraWebParts/EditBlockButtonsDetails.aspx?CurrentDetails=' + blockButtonsDetails.val();

    var options = {
        url: urlValue,
        tite: 'Edit Block Buttons',
        allowMaximize: false,
        showClose: true,
        dialogReturnValueCallback: callbackFunction
    };

    SP.UI.ModalDialog.showModalDialog(options);
}


function WBF_UpdateOneBlockButton(idPrefix, webPartUniqueID, index, width, height, title, link, extraText, buttonColor, borderColor, textColor) {

    var titleDiv = $("#" + idPrefix + "-title-" + webPartUniqueID + "-" + index).text(title);
    if (title == "") titleDiv.hide(); else titleDiv.show();

    $("#" + idPrefix + "-link-" + webPartUniqueID + "-" + index).attr("href", link);
    
    var extraTextDiv = $("#" + idPrefix + "-extra-text-" + webPartUniqueID + "-" + index).text(extraText);
    if (extraText == "") extraTextDiv.hide(); else extraTextDiv.show();

    $("#" + idPrefix + "-" + webPartUniqueID + "-" + index).css({ 'background-color': buttonColor, 'border-color': borderColor, 'color': textColor , 'width': width, 'height': height });
}

function WBF_HtmlForOneBlockButton(idPrefix, webPartUniqueID, index, extraClass, width, height, title, link, extraText, buttonColor, borderColor, textColor) {
    var html = "";

    html += "<td>\n<a class=\"block-button-link " + extraClass + "\"  id=\"wbf-block-button-link-" + webPartUniqueID + "-" + index + "\" href=\"" + link + "\">\n";
    html += "<div class=\"block-button block-button-group-" + webPartUniqueID + " " + extraClass + "\" id=\"wbf-block-button-" + webPartUniqueID + "-" + index + "\" style=\"background-color: " + buttonColor + "; border-color: " + borderColor + "; color: " + textColor + "; width: " + width + "; height: " + height + ";\">\n";
    html += "<div class=\"block-button-content " + extraClass + "\">\n";
    html += "<div class=\"block-button-title " + extraClass + "\" id=\"wbf-block-button-title-" + webPartUniqueID + "-" + index + "\" " + ((title == "") ? " style=\" display: none;\"" : "") + ">" + title + "</div> \n";
    html += "<div class=\"block-button-extra-text " + extraClass + "\" id=\"wbf-block-button-extra-text-" + webPartUniqueID + "-" + index + "\" " + ((extraText == "") ? " style=\" display: none;\"" : "") + ">" + extraText + "</div> \n"; 
    html += "</div></div></a></td>";

    return html;
}

function WBF_UpdateBlockButtons(webPartUniqueID, callbackBlockButtonDetails) {

    var details = callbackBlockButtonDetails.split(",");

    var width = WBF_putBackDelimiterCharacters(details[0]);
    var height = WBF_putBackDelimiterCharacters(details[1]);
    var buttonsDetails = WBF_putBackDelimiterCharacters(details[2]);
    var extraClass = WBF_putBackDelimiterCharacters(details[3]);
    /* Not currently doing anything dynamic with the extra CSS styles */

    var buttons = buttonsDetails.split("^");

    var tableRows = $("#wbf-block-buttons-table-" + webPartUniqueID + " tbody tr");

    tableRows.remove();

    var newRow = "<tr>";

    var index;
    for (index = 0; index < buttons.length; ++index) {
        var oneButtonDetails = buttons[index].split("|");

        var title = oneButtonDetails[0];
        var link = oneButtonDetails[1];
        var extraText = oneButtonDetails[2];
        var buttonColor = oneButtonDetails[3];
        var borderColor = oneButtonDetails[5];
        var textColor = oneButtonDetails[7];

        newRow += WBF_HtmlForOneBlockButton("wbf-block-button", webPartUniqueID, index, extraClass, width, height, title, link, extraText, buttonColor, borderColor, textColor);
    }

    newRow += "</tr>";
    $("#wbf-block-buttons-table-" + webPartUniqueID + " tbody").append(newRow);

    WBF_checkBlockButtonsHeights(webPartUniqueID, height);
}


function WBF_buttonColorToBorderColor(buttonColor) {
    buttonRGB = $.colpick.hexToRgb(buttonColor);

    var red = Math.floor(32 + (buttonRGB.r * 1.5));
    var green = Math.floor(32 + (buttonRGB.g * 1.5));
    var blue = Math.floor(32 + (buttonRGB.b * 1.5));

    if (red > 255) red = 255;
    if (green > 255) green = 255;
    if (blue > 255) blue = 255;

    return $.colpick.rgbToHex({ r: red, g: green, b: blue });
}

function WBF_buttonColorToTextColor(buttonColor) {
    buttonRGB = $.colpick.hexToRgb(buttonColor);

    var multiplier = 1.25;
    var constant = 48 + (((3 * 255) - buttonRGB.r - buttonRGB.g - buttonRGB.b) / (5));
    /* If the button colour is lightish then we'll go darker with the text: */
    // if (buttonRGB.r > 160 || buttonRGB.g > 160 || buttonRGB.b > 160) {
    if ((buttonRGB.r + buttonRGB.g + buttonRGB.b > 400) || (buttonRGB.g > 216)) {
        multiplier = 0.4;
        constant = 0;
    }

    var red = Math.floor(constant + (buttonRGB.r * multiplier));
    var green = Math.floor(constant + (buttonRGB.g * multiplier));
    var blue = Math.floor(constant + (buttonRGB.b * multiplier));

    if (red > 255) red = 255;
    if (green > 255) green = 255;
    if (blue > 255) blue = 255;

    if (red < 0) red = 0;
    if (green < 0) green = 0;
    if (blue < 0) blue = 0;

    return $.colpick.rgbToHex({ r: red, g: green, b: blue });
}

function WBF_processInputtedText(text) {
    if (text == "") return "";

    text = $("<div/>").html(text).text();

    text = text.replace(/\/\//g, "<br/>");
    return text;
}

function WBF_putBackDelimiterCharacters(text) {
    if (text == "") return "";

    text = text.replace(/__HASH__/g, "#");
    text = text.replace(/__SEMICOLON__/g, ";");
    text = text.replace(/__COMMA__/g, ",");
    text = text.replace(/__PIPE__/g, "|");
    text = text.replace(/__NEW_LINE__/g, "\n");

    return text;
}

function WBF_checkBlockButtonsHeights(blockButtonsGroup, heightString) {

    heightString = heightString.replace(/px/g, "");
    var height = parseInt(heightString);

    var buttons = $(".block-button-group-" + blockButtonsGroup);
    var index = 0;

    // alert("Num buttons found: " + buttons.length);

    // First reset all of the heights to be the set value:
    for (index = 0; index < buttons.length; index++) {
        var button = $("#wbf-block-button-" + blockButtonsGroup + "-" + index);
        if (button.length > 0) {
            button.height(height);
        }
    }

    // Then check to see if any of the buttons are higher than the set value:
    var maxHeight = height;
    for (index = 0; index < buttons.length; index++) {
        var button = $("#wbf-block-button-" + blockButtonsGroup + "-" + index);
        if (button.length > 0) {
            if (button.height() > maxHeight) maxHeight = button.height();
            // alert("Found: " + "#wbf-block-button-" + blockButtonsGroup + "-" + index + "    with outerHeight: " + button.outerHeight() + " and height(): " + button.height());
        } else {
            // alert("Didn't find: " + "#wbf-block-button-" + blockButtonsGroup + "-" + index);
        }
    }

    // Finally, if needs be, set the height of all of the buttons to be equal to the largest height:
    if (maxHeight > height) {
        // alert("dyn height IS greater: " + maxHeight + " <= " + height);

        for (index = 0; index < buttons.length; index++) {
            var button = $("#wbf-block-button-" + blockButtonsGroup + "-" + index);
            if (button.length > 0) {
                button.height(maxHeight);
            }
        }

        $("#wbf-dynamic-buttons-height").text("Dynamic height is: " + maxHeight + "px");
    } else {
        // alert("dyn height not greater: " + maxHeight + " <= " + height);
        $("#wbf-dynamic-buttons-height").text("");
    }

}
