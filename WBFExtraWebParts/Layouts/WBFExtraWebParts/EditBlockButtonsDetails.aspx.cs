using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using WorkBoxFramework;

namespace WBFExtraWebParts.Layouts.WBFExtraWebParts
{
    public partial class EditBlockButtonsDetails : WBDialogPageBase
    {
        public int NumberOfButtons = 0;
        private List<TableRow> allRows = new List<TableRow>();

        private static string DEFAULT_HEX_COLOR = "4e5ec7";

        public String CSSExtraClass = "";
        public String CSSExtraStyles = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String allDetails = Request.QueryString["CurrentDetails"];

                string[] details = allDetails.Split(',');

                if (details.Length != 5)
                {
                    WBLogging.Debug("The details sent to this page have the wrong structure: " + allDetails);
                    ErrorMessage.Text = "There was a problem with the data sent to this page.";
                    return;
                }

                EditWidth.Text = WBUtils.PutBackDelimiterCharacters(details[0]);
                EditHeight.Text = WBUtils.PutBackDelimiterCharacters(details[1]);
                BlockButtonsDetails.Value = WBUtils.PutBackDelimiterCharacters(details[2]);
                HiddenCSSExtraClass.Value = WBUtils.PutBackDelimiterCharacters(details[3]);
                HiddenCSSExtraStyles.Value = WBUtils.PutBackDelimiterCharacters(details[4]);
            }

            CSSExtraClass = HiddenCSSExtraClass.Value;
            CSSExtraStyles = HiddenCSSExtraStyles.Value;

            CreateTable(!IsPostBack);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdatePreivewBlockButtons", "WBF_checkPreviewButtonHeights();", true);
        }

        protected virtual void Page_LoadComplete(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (!String.IsNullOrEmpty(DeleteButtonIndex.Value))
                {
                    CaptureTable();

                    int buttonIndex = Convert.ToInt32(DeleteButtonIndex.Value);
                    DeleteButtonIndex.Value = "";

                    List<String> blockButtonsDetailsList = new List<String>();

                    string currentDetails = BlockButtonsDetails.Value.WBxTrim();

                    if (!String.IsNullOrEmpty(currentDetails))
                    {
                        blockButtonsDetailsList = new List<String>(currentDetails.Split('^'));
                    }

                    blockButtonsDetailsList.RemoveAt(buttonIndex);

                    BlockButtonsDetails.Value = String.Join("^", blockButtonsDetailsList.ToArray());

                    ClearTable();
                    CreateTable(true);
                }
            }
        }

        private void CreateTable(bool setValues)
        {
            List<String> blockButtonsDetailsList = new List<String>();

            string currentDetails = BlockButtonsDetails.Value.WBxTrim();

            WBLogging.Debug("Building the table with current details: " + currentDetails);

            if (!String.IsNullOrEmpty(currentDetails))
            {
                blockButtonsDetailsList = new List<String>(currentDetails.Split('^'));
            }

            NumberOfButtons = blockButtonsDetailsList.Count;

            Table table = new Table();
            table.ID = "table-of-button-details";
            //table.Width = Unit.Percentage(100);

            TableRow headers = new TableRow();
            headers.WBxAddTableHeaderCell("Title");
            headers.WBxAddTableHeaderCell("Link");
            headers.WBxAddTableHeaderCell("Extra Text");
            headers.WBxAddTableHeaderCell("Fill");
            headers.WBxAddTableHeaderCell("");
            headers.WBxAddTableHeaderCell("Outline");
            headers.WBxAddTableHeaderCell("");
            headers.WBxAddTableHeaderCell("Text");

            table.Rows.Add(headers);

            int index = 0;
            foreach (String details in blockButtonsDetailsList)
            {
                TableRow row = CreateEditableTableRow(index, blockButtonsDetailsList.Count, details, setValues);

                allRows.Add(row);

                table.Rows.Add(row);
                index++;
            }

            EditBlockButtonsTable.Controls.Add(table);

            CreatePreview();
        }

        private void CreatePreview()
        {
            String html = "";

            List<String> blockButtonsDetailsList = new List<String>();

            string currentDetails = BlockButtonsDetails.Value.WBxTrim();

            if (!String.IsNullOrEmpty(currentDetails))
            {
                blockButtonsDetailsList = new List<String>(currentDetails.Split('^'));
            }

            int index = 0;
            foreach (String buttonDetails in blockButtonsDetailsList)
            {
                string[] details = buttonDetails.Split('|');

                string title = details[0].Trim();
                string link = details[1].Trim();
                string extraText = details[2].Trim();
                string buttonColor = details[3].Trim();

                int buttonBorderColorIndex = 4;
                int textColorIndex = 5;

                if (details.Length > 6)
                {
                    buttonBorderColorIndex = 5;
                    textColorIndex = 7;
                }

                string buttonBorderColor = details[buttonBorderColorIndex].Trim();
                string textColor = details[textColorIndex].Trim(); 

                html += "<td>\n<a class=\"block-button-link " + CSSExtraClass + "\" href=\"" + link + "\">\n";
                html += "<div class=\"block-button block-button-group-preview " + CSSExtraClass + "\" id=\"wbf-block-button-preview-" + index + "\" style=\"background-color: " + buttonColor + "; border-color: " + buttonBorderColor + "; color: " + textColor + "; width: " + EditWidth.Text + "; height: " + EditHeight.Text + ";\">\n";
                html += "<div class=\"block-button-content " + CSSExtraClass + "\">\n";
                html += "<div class=\"block-button-title " + CSSExtraClass + "\" id=\"preview-button-title-" + index + "\" " + ((String.IsNullOrEmpty(title)) ? " style=\" display: none;\"" : "") + ">" + title + "</div>\n";
                html += "<div class=\"block-button-extra-text " + CSSExtraClass + "\" id=\"preview-button-extra-text-" + index + "\" " + ((String.IsNullOrEmpty(extraText)) ? " style=\" display: none;\"" : "") + ">" + extraText + "</div>"; 
                html += "</div></div></a></td>";

                index++;
            }

            BlockButtons.Text = html;

        }

        private String PrepareTextForEditBox(String text)
        {
            return text.Trim().Replace("<br/>", "//");
        }

        private String ProcessInputtedText(String text)
        {
            if (String.IsNullOrEmpty(text)) return "";
            text = text.Trim();
            text = text.Replace("//", "<br/>");
            return text;
        }

        public TableRow CreateEditableTableRow(int index, int totalRows, String buttonDetails, bool setValues)
        {
            TableRow row = new TableRow();
            row.ID = MakeControlID(index, "row");
            row.CssClass = "wbf-edit-action-row";

            WBLogging.Debug("Just starting with details: " + buttonDetails);

            string[] parts = buttonDetails.Split('|');

            string title = PrepareTextForEditBox(parts[0]);
            string link = parts[1].Trim();
            string extraText = PrepareTextForEditBox(parts[2]);
            string buttonColor = parts[3].Trim();
            string buttonColorHex = ColorToHex(buttonColor);

            int buttonBorderColorIndex = 4;
            int textColorIndex = 5;

            bool buttonBorderColorIsChained = true; //  buttonBorderColorHex.Contains("chained");
            bool textColorIsChained = true; // textColorHex.Contains("chained");

            if (parts.Length > 6)
            {
                buttonBorderColorIndex = 5;
                textColorIndex = 7;

                buttonBorderColorIsChained = parts[4].Trim().Equals("c");
                textColorIsChained = parts[6].Trim().Equals("c");
            }

            string buttonBorderColor = parts[buttonBorderColorIndex].Trim();
            string buttonBorderColorHex = ColorToHex(buttonBorderColor);
            if (buttonBorderColorIsChained) buttonBorderColorHex = ButtonToBorderColor(buttonColorHex);

            string textColor = parts[textColorIndex].Trim();
            if (String.IsNullOrEmpty(textColor)) textColor = "#dddddd";
            string textColorHex = ColorToHex(textColor);
            if (textColorIsChained) textColorHex = ButtonToTextColor(buttonColorHex);

            String script = "<script type=\"text/javascript\">\n  $(function () { \n";

            TextBox textBox = new TextBox();
            if (setValues) textBox.Text = title;
            row.WBxAddWithIDInTableCell(textBox, MakeControlID(index, "title"));

            script += CodeForOneTextBox(textBox.ClientID, "preview-button-title-" + index);


            textBox = new TextBox();
            if (setValues) textBox.Text = link;
            row.WBxAddWithIDInTableCell(textBox, MakeControlID(index, "link"));

            textBox = new TextBox();
            if (setValues) textBox.Text = extraText;
            //textBox.TextMode = TextBoxMode.MultiLine;
            //textBox.Rows = 1;
            row.WBxAddWithIDInTableCell(textBox, MakeControlID(index, "extraText"));

            script += CodeForOneTextBox(textBox.ClientID, "preview-button-extra-text-" + index);


            Panel panel = new Panel();
            panel.CssClass = "block-button-color-box";
            HiddenField hiddenField = new HiddenField();
            hiddenField.ID = MakeControlID(index, "hiddenButtonColor");
            if (setValues)
            {
                hiddenField.Value = buttonColorHex;
            }
            panel.Controls.Add(hiddenField);
            row.WBxAddWithIDInTableCell(panel, MakeControlID(index, "buttonColor"));

            //script += CodeForOneButton(buttonColorHex, "", panel.ClientID, hiddenField.ClientID, "preview-button-" + index, "background-color");
            string buttonPanelClientID = panel.ClientID;
            string buttonHiddenFieldClientID = hiddenField.ClientID;

            System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
            image.ID = MakeControlID(index, "buttonBorderColorChainImage");
            string buttonBorderColorChainImageClientID = image.ClientID;
            if (buttonBorderColorIsChained)
            {
                image.ImageUrl = "/_layouts/images/WBFExtraWebParts/chain.png";
            }
            else
            {
                image.ImageUrl = "/_layouts/images/WBFExtraWebParts/chain_unchain.png";
            }
            image.ImageAlign = ImageAlign.Middle;
            row.WBxAddInTableCell(image);

            panel = new Panel();
            panel.CssClass = "block-button-color-box";

            hiddenField = new HiddenField();
            hiddenField.ID = MakeControlID(index, "hiddenButtonBorderColorIsChained");
            string buttonBorderColorIsChainedHiddenFieldClientID = hiddenField.ClientID;
            if (setValues)
            {
                hiddenField.Value = buttonBorderColorIsChained ? "c" : "u";
            }
            panel.Controls.Add(hiddenField);
            
            hiddenField = new HiddenField();
            hiddenField.ID = MakeControlID(index, "hiddenButtonBorderColor");
            if (setValues)
            {
                hiddenField.Value = buttonBorderColorHex;
            }
            panel.Controls.Add(hiddenField);
            row.WBxAddWithIDInTableCell(panel, MakeControlID(index, "buttonBorderColor"));

            script += CodeForOneChainedColorPicker(buttonBorderColorHex, panel.ClientID, hiddenField.ClientID, buttonBorderColorIsChainedHiddenFieldClientID, "wbf-block-button-preview-" + index, "border-color");
            script += CodeFromOneChainImage(index, "borderColorChained", buttonBorderColorChainImageClientID, buttonBorderColorIsChainedHiddenFieldClientID, "WBF_buttonColorToBorderColor", panel.ClientID, hiddenField.ClientID, "wbf-block-button-preview-" + index, "border-color");
            // script += CodeForOneButton(buttonBorderColorHex, "WBF_buttonColorToBorderColor", , , "preview-button-" + index, "border-color");
            string borderPanelClientID = panel.ClientID;
            string borderHiddenFieldClientID = hiddenField.ClientID;

            image = new System.Web.UI.WebControls.Image();
            image.ID = MakeControlID(index, "textColorChainImage");
            string textColorChainImageClientID = image.ClientID;
            if (textColorIsChained)
            {
                image.ImageUrl = "/_layouts/images/WBFExtraWebParts/chain.png";
            }
            else
            {
                image.ImageUrl = "/_layouts/images/WBFExtraWebParts/chain_unchain.png";
            }
            image.ImageAlign = ImageAlign.Middle;
            row.WBxAddInTableCell(image);

            panel = new Panel();
            panel.CssClass = "block-button-color-box";

            hiddenField = new HiddenField();
            hiddenField.ID = MakeControlID(index, "hiddenTextColorIsChained");
            string textColorIsChainedHiddenFieldClientID = hiddenField.ClientID;
            if (setValues)
            {
                hiddenField.Value = textColorIsChained ? "c" : "u";
            }
            panel.Controls.Add(hiddenField);

            
            hiddenField = new HiddenField();
            hiddenField.ID = MakeControlID(index, "hiddenTextColor");
            if (setValues)
            {
                hiddenField.Value = textColorHex;
            }
            panel.Controls.Add(hiddenField);
            row.WBxAddWithIDInTableCell(panel, MakeControlID(index, "textColor"));

            script += CodeForOneChainedColorPicker(textColorHex, panel.ClientID, hiddenField.ClientID, textColorIsChainedHiddenFieldClientID, "wbf-block-button-preview-" + index, "color");
            script += CodeFromOneChainImage(index, "textColorChained", textColorChainImageClientID, textColorIsChainedHiddenFieldClientID, "WBF_buttonColorToTextColor", panel.ClientID, hiddenField.ClientID, "wbf-block-button-preview-" + index, "color");
            // script += CodeForOneButton(textColorHex, "WBF_buttonColorToTextColor", panel.ClientID, hiddenField.ClientID, "preview-button-" + index, "color");
            string textPanelClientID = panel.ClientID;
            string textHiddenFieldClientID = hiddenField.ClientID;

            /* Now we're going to add the scripts for all three colour choices */
            script += "var buttonColor_" + index + " = '" + buttonColorHex + "'; \n";
            script += "var borderColorChained_" + index + " = " + (buttonBorderColorIsChained ? "true" : "false") + "; \n";
            script += "var borderColor_" + index + " = " + (buttonBorderColorIsChained ? "WBF_buttonColorToBorderColor(buttonColor_" + index + ")" : "'" + buttonBorderColorHex + "'") + "; \n";
            script += "var textColorChained_" + index + " = " + (textColorIsChained ? "true" : "false") + "; \n";
            script += "var textColor_" + index + " = " + (textColorIsChained ? "WBF_buttonColorToTextColor(buttonColor_" + index + ")" : "'" + textColorHex + "'") + "; \n";

            script += "$('#ctl00_PlaceHolderMain_" + buttonPanelClientID + "').colpick({ \n";
            script += "    colorScheme: 'light', \n";
            script += "    color: buttonColor_" + index + ", \n";
            script += "    layout: 'rgbhex', \n";
            script += "    onSubmit: function (hsb, hex, rgb, el) { \n";
            script += "        buttonColor_" + index + " = hex; \n";
            script += "        $(el).css('background-color', '#' + buttonColor_" + index + "); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + buttonHiddenFieldClientID + "').val(buttonColor_" + index + "); \n";

            script += "        if (borderColorChained_" + index + ") {  \n";
            script += "            borderColor_" + index + " = WBF_buttonColorToBorderColor(buttonColor_" + index + "); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + borderPanelClientID + "').css('background-color', '#' + borderColor_" + index + ").colpickSetColor('#' + borderColor_" + index + ", true); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + borderHiddenFieldClientID + "').val(borderColor_" + index + "); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + buttonBorderColorIsChainedHiddenFieldClientID + "').data('unchained-color', borderColor_" + index + "); \n";
            script += "        }  \n";

            script += "        if (textColorChained_" + index + ") {  \n";
            script += "            textColor_" + index + " = WBF_buttonColorToTextColor(buttonColor_" + index + "); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + textPanelClientID + "').css('background-color', '#' + textColor_" + index + ").colpickSetColor('#' + textColor_" + index + ", true); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + textHiddenFieldClientID + "').val(textColor_" + index + "); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + textColorIsChainedHiddenFieldClientID + "').data('unchained-color', textColor_" + index + "); \n";
            script += "        }  \n";

            script += "        $('#wbf-block-button-preview-" + index + "').css( { 'background-color': '#' + buttonColor_" + index + ", 'border-color': '#' + borderColor_" + index + ", 'color': '#' + textColor_" + index + " } ); \n";

            script += "        $(el).colpickHide(); \n";
            script += "      }, \n";
            script += "    onChange: function (hsb, hex, rgb, el) { \n";
            script += "        buttonColor_" + index + " = hex; \n";
            script += "        $(el).css('background-color', '#' + buttonColor_" + index + "); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + buttonHiddenFieldClientID + "').val(buttonColor_" + index + "); \n";

            script += "        if (borderColorChained_" + index + ") {  \n";
            script += "            borderColor_" + index + " = WBF_buttonColorToBorderColor(buttonColor_" + index + "); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + borderPanelClientID + "').css('background-color', '#' + borderColor_" + index + ").colpickSetColor('#' + borderColor_" + index + ", true); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + borderHiddenFieldClientID + "').val(borderColor_" + index + "); \n";
            script += "        }  \n";

            script += "        if (textColorChained_" + index + ") {  \n";
            script += "            textColor_" + index + " = WBF_buttonColorToTextColor(buttonColor); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + textPanelClientID + "').css('background-color', '#' + textColor_" + index + ").colpickSetColor('#' + textColor_" + index + ", true); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + textHiddenFieldClientID + "').val(textColor); \n";
            script += "        }  \n";

            script += "        $('#wbf-block-button-preview-" + index + "').css( { 'background-color': '#' + buttonColor_" + index + ", 'border-color': '#' + borderColor_" + index + ", 'color': '#' + textColor_" + index + " } ); \n";
            script += "      } \n";

            script += "    }); \n";


            script += "$('#ctl00_PlaceHolderMain_" + buttonPanelClientID + "').css('background-color', '#' + $('#ctl00_PlaceHolderMain_" + buttonHiddenFieldClientID + "').val()); \n";
            script += "$('#ctl00_PlaceHolderMain_" + borderPanelClientID + "').css('background-color', '#' + $('#ctl00_PlaceHolderMain_" + borderHiddenFieldClientID + "').val()); \n";
            script += "$('#ctl00_PlaceHolderMain_" + textPanelClientID + "').css('background-color', '#' + $('#ctl00_PlaceHolderMain_" + textHiddenFieldClientID + "').val()); \n";


            script += "}); \n</script>\n";

            Literal literal = new Literal();
            literal.Text = script;
            panel.Controls.Add(literal);

            if (index != 0)
            {
                Button upButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "UpButton"));
                upButton.Text = "<<";
                upButton.CommandName = "Up";
                upButton.CommandArgument = index.ToString();
                upButton.Command += new CommandEventHandler(upButton_OnClick);
            }
            else
            {
                row.WBxAddWithIDInTableCell(new Panel(), MakeControlID(index, "UpButtonGap"));
            }

            if (index != totalRows - 1)
            {
                Button downButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "DownButton"));
                downButton.Text = ">>";
                downButton.CommandName = "Down";
                downButton.CommandArgument = index.ToString();
                downButton.Command += new CommandEventHandler(downButton_OnClick);
            }
            else
            {
                row.WBxAddWithIDInTableCell(new Panel(), MakeControlID(index, "downButtonGap"));
            }

            Button removeButton = (Button)row.WBxAddWithIDInTableCell(new Button(), MakeControlID(index, "RemoveButton"));
            removeButton.Text = "Remove";
            removeButton.OnClientClick = "WBF_DeleteButton(" + index + ",\"" + title + "\"); return false;";

            HiddenField buttonDetailsHiddenField = (HiddenField)row.WBxAddWithIDInTableCell(new HiddenField(), MakeControlID(index, "ButtonDetails"));

            //WBLogging.Debug("Really Here");


            if (!IsPostBack)
            {
                buttonDetailsHiddenField.Value = buttonDetails;
            }

            return row;
        }

        private string CodeFromOneChainImage(int index, string chainedFlag, string buttonBorderColorChainImageClientID, string buttonBorderColorIsChainedHiddenFieldClientID, string chainedColorFunction, string panelClientID, string hiddenInputClientID, string previewButtonID, string colourProperty)
        {
            String script = "";
            script += "$('#ctl00_PlaceHolderMain_" + buttonBorderColorChainImageClientID + "').click(function() { \n";
            script += "    var hiddenField = $('#ctl00_PlaceHolderMain_" + buttonBorderColorIsChainedHiddenFieldClientID + "'); \n";
            script += "    if (hiddenField.val() === \"c\") { \n";
            script += "        $(this).attr('src', '/_layouts/images/WBFExtraWebParts/chain_unchain.png'); \n";
            script += "        hiddenField.val(\"u\"); \n";
            script += "        " + chainedFlag + "_" + index + " = false; \n";
            script += "        var hex = hiddenField.data('unchained-color'); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + panelClientID + "').css('background-color', '#' + hex); \n";            
            script += "        $('#ctl00_PlaceHolderMain_" + hiddenInputClientID + "').val(hex); \n";
            script += "        $('#" + previewButtonID + "').css('" + colourProperty + "', '#' + hex); \n";
            script += "    } else { \n";
            script += "        $(this).attr('src', '/_layouts/images/WBFExtraWebParts/chain.png'); \n";
            script += "        hiddenField.val(\"c\"); \n";
            script += "        " + chainedFlag + "_" + index + " = true; \n";
            script += "         hiddenField.data('unchained-color', $('#ctl00_PlaceHolderMain_" + hiddenInputClientID + "').val()); \n";
            
            script += "        var hex = " + chainedColorFunction + "(buttonColor); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + panelClientID + "').css('background-color', '#' + hex); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + hiddenInputClientID + "').val(hex); \n";
            script += "        $('#" + previewButtonID + "').css('" + colourProperty + "', '#' + hex); \n";
            script += "    } \n";

            script += "}); \n";

            return script;
        }

        private string CodeForOneTextBox(string inputClientID, string previewDivID)
        {
            String script = "";
            script += "$('#ctl00_PlaceHolderMain_" + inputClientID + "').keyup(function() { \n";
            // Note that the html function is being used here so that <br/> tags can be inserted - but the inputted 
            // text is first cleaned up by the WBF_processInputtedText function to ensure that no cross site scripting 
            // hack is tried here - Oli Sharpe - 20/2/2015
            script += "    var textToUse = WBF_processInputtedText($(this).val()); \n";
            script += "    var div = $('#" + previewDivID + "'); \n";
            script += "    div.html(textToUse); \n";

            script += "    if (textToUse == \"\") div.hide(); \n";
            script += "    else div.show(); \n";

            script += "    WBF_checkPreviewButtonHeights(); \n";

            script += "}); \n";

            return script;
        }


        private string CodeForOneChainedColorPicker(string hexColor, string panelClientID, string hiddenInputClientID, string hiddenChainClientID, string previewButtonID, string colourProperty)
        {
            String script = "";
            script += "$('#ctl00_PlaceHolderMain_" + panelClientID + "').colpick({ \n";
            script += "    colorScheme: 'light', \n";
            script += "    color: '" + hexColor + "', \n";
            script += "    layout: 'rgbhex', \n";
            script += "    onSubmit: function (hsb, hex, rgb, el) { \n";
            script += "        $(el).css('background-color', '#' + hex); \n";
            script += "        $('#" + previewButtonID + "').css('" + colourProperty + "', '#' + hex); \n";
            script += "        $(el).colpickHide(); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + hiddenInputClientID + "').val(hex); \n";
            script += "        $('#ctl00_PlaceHolderMain_" + hiddenChainClientID + "').data('unchained-color', hex); \n";
            script += "      }, \n";
            script += "    onChange: function (hsb, hex, rgb, el, bySetColor) { \n";
            script += "        if (!bySetColor) { \n";
            script += "            $(el).css('background-color', '#' + hex); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + hiddenInputClientID + "').val(hex); \n";
            script += "            $('#" + previewButtonID + "').css('" + colourProperty + "', '#' + hex); \n";
            script += "            $('#ctl00_PlaceHolderMain_" + hiddenChainClientID + "').data('unchained-color', hex); \n";
            script += "        } \n";
            script += "      }, \n";
            script += "    onShow: function (el) { \n";
            script += "        if ($('#ctl00_PlaceHolderMain_" + hiddenChainClientID + "').val() === \"c\") return false; \n";
            script += "        return true; \n";
            script += "      } \n";
            
            script += "    }); \n";


            // script += "$('#ctl00_PlaceHolderMain_" + panelClientID + "').css('background-color', '#' + $('#ctl00_PlaceHolderMain_" + hiddenInputClientID + "').val()); \n";

            return script;
        }

        private string ColorToHex(string buttonColor)
        {
            if (String.IsNullOrEmpty(buttonColor)) return DEFAULT_HEX_COLOR;

            if (buttonColor.Contains("chained")) return buttonColor;
            if (buttonColor.Contains("#")) return buttonColor.Replace("#", "");
            if (buttonColor.Contains("rgb"))
            {
                String numbersString = buttonColor.Replace("rgb(", "").Replace(")", "");
                String[] numbersArray = numbersString.Split(',');
                if (numbersArray.Length != 3) return DEFAULT_HEX_COLOR;
                Color color = Color.FromArgb(Convert.ToInt32(numbersArray[0]), Convert.ToInt32(numbersArray[1]), Convert.ToInt32(numbersArray[2]));
                return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            }
            return DEFAULT_HEX_COLOR;
        }

        private String ButtonToBorderColor(string buttonColorString) {
            Color buttonColor = System.Drawing.ColorTranslator.FromHtml("#" + buttonColorString);

            int red = 32 + (int) (((int)buttonColor.R) * 1.5);
            int green = 32 + (int)(((int)buttonColor.G) * 1.5);
            int blue = 32 + (int)(((int)buttonColor.B) * 1.5);

            if (red > 255) red = 255;
            if (green > 255) green = 255;
            if (blue > 255) blue = 255;

            if (red < 0) red = 0;
            if (green < 0) green = 0;
            if (blue < 0) blue = 0;

            Color borderColor = Color.FromArgb(red, green, blue);

            return borderColor.R.ToString("X2") + borderColor.G.ToString("X2") + borderColor.B.ToString("X2"); ;
        }

        private String ButtonToTextColor(string buttonColorString)
        {
            Color buttonColor = System.Drawing.ColorTranslator.FromHtml("#" + buttonColorString);

            double multiplier = 1.25;
            int constant = 48 + (((3*255) - buttonColor.R - buttonColor.G - buttonColor.B)/(5));;
            
            // If the button colour is lightish then we'll go darker with the text:
            //if (buttonColor.R > 160 || buttonColor.G > 160 || buttonColor.B > 160)
            if ((buttonColor.R + buttonColor.G + buttonColor.B > 400) || (buttonColor.G > 216) ) 
            {
                multiplier = 0.4;
                constant = 0;
            }

            int red = constant + (int)(((int)buttonColor.R) * multiplier);
            int green = constant + (int)(((int)buttonColor.G) * multiplier);
            int blue = constant + (int)(((int)buttonColor.B) * multiplier);

            if (red > 255) red = 255;
            if (green > 255) green = 255;
            if (blue > 255) blue = 255;

            if (red < 0) red = 0;
            if (green < 0) green = 0;
            if (blue < 0) blue = 0;

            Color textColor = Color.FromArgb(red, green, blue);

            return textColor.R.ToString("X2") + textColor.G.ToString("X2") + textColor.B.ToString("X2"); ;
        }


        private void ClearTable()
        {
            EditBlockButtonsTable.Controls.Clear();
            allRows.Clear();
        }

        private void CaptureTable()
        {
            // We're also just going to check that the width and height values have 'px' at the end:
            if (!EditWidth.Text.Contains("px")) EditWidth.Text = EditWidth.Text + "px";
            if (!EditHeight.Text.Contains("px")) EditHeight.Text = EditHeight.Text + "px";

            // Table table = (Table)EditBlockButtonsTable.WBxFindNestedControlByID("table-of-button-details");

            List<String> blockButtonsDetailsList = new List<String>();

            int index = 0;
            foreach (TableRow row in allRows)
            {
                WBLogging.Debug("Looking at row " + index + " and row is currently: " + row);

                CaptureTableRow(index, row, blockButtonsDetailsList);
                index++;
            }

            BlockButtonsDetails.Value = String.Join("^", blockButtonsDetailsList.ToArray());
        }


        private void CaptureTableRow(int index, TableRow row, List<string> blockButtonsDetailsList)
        {

            WBLogging.Debug("Inside CaptureTableRow looking at " + index + " with row: " + row);

            List<String> buttonDetailsList = new List<String>();
            TextBox textBox = (TextBox)row.WBxFindNestedControlByID(MakeControlID(index, "title"));
            buttonDetailsList.Add(ProcessInputtedText(textBox.Text));

            WBLogging.Debug("Found title for row " + index);

            textBox = (TextBox)row.WBxFindNestedControlByID(MakeControlID(index, "link"));
            buttonDetailsList.Add(textBox.Text.WBxTrim());

            WBLogging.Debug("Found link for row " + index);


            textBox = (TextBox)row.WBxFindNestedControlByID(MakeControlID(index, "extraText"));
            buttonDetailsList.Add(ProcessInputtedText(textBox.Text));
            WBLogging.Debug("Found extraText for row " + index);

            HiddenField hiddenField = (HiddenField)row.WBxFindNestedControlByID(MakeControlID(index, "hiddenButtonColor"));
            buttonDetailsList.Add("#" + hiddenField.Value);
            WBLogging.Debug("Found buttonColor for row " + index + " to be: " + hiddenField.Value);

            hiddenField = (HiddenField)row.WBxFindNestedControlByID(MakeControlID(index, "hiddenButtonBorderColorIsChained"));
            buttonDetailsList.Add(hiddenField.Value);

            hiddenField = (HiddenField)row.WBxFindNestedControlByID(MakeControlID(index, "hiddenButtonBorderColor"));
            buttonDetailsList.Add("#" + hiddenField.Value);
            WBLogging.Debug("Found hiddenButtonBorderColor for row " + index + " to be: " + hiddenField.Value);

            hiddenField = (HiddenField)row.WBxFindNestedControlByID(MakeControlID(index, "hiddenTextColorIsChained"));
            buttonDetailsList.Add(hiddenField.Value);

            hiddenField = (HiddenField)row.WBxFindNestedControlByID(MakeControlID(index, "hiddenTextColor"));
            buttonDetailsList.Add("#" + hiddenField.Value);
            WBLogging.Debug("Found hiddenTextColor for row " + index + " to be: " + hiddenField.Value);

            blockButtonsDetailsList.Add(String.Join("|", buttonDetailsList.ToArray()));
        }


        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            CaptureTable();

            String[] detailsToSave = new String[5];

            detailsToSave[0] = WBUtils.ReplaceDelimiterCharacters(EditWidth.Text);
            detailsToSave[1] = WBUtils.ReplaceDelimiterCharacters(EditHeight.Text);
            detailsToSave[2] = WBUtils.ReplaceDelimiterCharacters(BlockButtonsDetails.Value);
            detailsToSave[3] = WBUtils.ReplaceDelimiterCharacters(HiddenCSSExtraClass.Value);  // We could in future edit these values here rather than on standard web part details edit panel
            detailsToSave[4] = WBUtils.ReplaceDelimiterCharacters(HiddenCSSExtraStyles.Value); // We could in future edit these values here rather than on standard web part details edit panel

            // Just make sure that the height and width are expressed in pixels:
            if (!detailsToSave[0].Contains("px")) detailsToSave[0] = detailsToSave[0] + "px";
            if (!detailsToSave[1].Contains("px")) detailsToSave[1] = detailsToSave[1] + "px";

            returnFromDialogOK(String.Join(",", detailsToSave));
        }


        protected void refreshButton_OnClick(object sender, EventArgs e)
        {
            CaptureTable();
            CreatePreview();
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("");
        }


        protected void upButton_OnClick(object sender, CommandEventArgs e)
        {
            CaptureTable();

            string currentDetails = BlockButtonsDetails.Value.WBxTrim();

            List<String> blockButtonsDetailsList = new List<String>();

            if (!String.IsNullOrEmpty(currentDetails))
            {
                blockButtonsDetailsList = new List<String>(currentDetails.Split('^'));
            }

            if (!String.IsNullOrEmpty(e.CommandArgument.WBxToString()))
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.WBxToString());


                if (rowIndex > 0)
                {
                    String valueToMove = blockButtonsDetailsList[rowIndex];

                    blockButtonsDetailsList.RemoveAt(rowIndex);
                    rowIndex--;
                    blockButtonsDetailsList.Insert(rowIndex, valueToMove);

                    BlockButtonsDetails.Value = String.Join("^", blockButtonsDetailsList.ToArray());
                }
            }
            ClearTable();
            CreateTable(true);
        }


        protected void downButton_OnClick(object sender, CommandEventArgs e)
        {
            CaptureTable();

            string currentDetails = BlockButtonsDetails.Value.WBxTrim();

            WBLogging.Debug("Current details : " + currentDetails);

            List<String> blockButtonsDetailsList = new List<String>();

            if (!String.IsNullOrEmpty(currentDetails))
            {
                blockButtonsDetailsList = new List<String>(currentDetails.Split('^'));
            }

            if (!String.IsNullOrEmpty(e.CommandArgument.WBxToString()))
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.WBxToString());

                WBLogging.Debug("rowIndex = " + rowIndex);

                if (rowIndex < blockButtonsDetailsList.Count - 1)
                {
                    String valueToMove = blockButtonsDetailsList[rowIndex];

                    blockButtonsDetailsList.RemoveAt(rowIndex);
                    rowIndex++;
                    blockButtonsDetailsList.Insert(rowIndex, valueToMove);

                    BlockButtonsDetails.Value = String.Join("^", blockButtonsDetailsList.ToArray());


                    WBLogging.Debug("Set current BlockButtonsDetails.Value =: " + BlockButtonsDetails.Value);

                }
            }
            ClearTable();

            CreateTable(true);
        }



        public String MakeControlID(int index, String innerName)
        {
            return this.WBxMakeControlID(index.ToString(), innerName);
        }

        public void AddNewBlockButtonButton_OnClick(object sender, EventArgs e)
        {
            CaptureTable();

            List<String> blockButtonsDetailsList = new List<String>();

            string currentDetails = BlockButtonsDetails.Value.WBxTrim();

            WBLogging.Debug("Building the table with current details: " + currentDetails);

            if (!String.IsNullOrEmpty(currentDetails))
            {
                blockButtonsDetailsList = new List<String>(currentDetails.Split('^'));
            }

            blockButtonsDetailsList.Add("New Button | # | Extra text | #044376 | c | #8ea9c1 | c | #dddddd");

            BlockButtonsDetails.Value = String.Join("^", blockButtonsDetailsList.ToArray());

            ClearTable();
            CreateTable(true);
        }
    }
}
