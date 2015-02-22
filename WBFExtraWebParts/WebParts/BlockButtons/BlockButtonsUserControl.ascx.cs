using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using WorkBoxFramework;

namespace WBFExtraWebParts.BlockButtons
{
    public partial class BlockButtonsUserControl : UserControl
    {
        protected BlockButtons webPart = default(BlockButtons);

        public String WebPartUniqueID = "";
        public bool InEditMode = false;

        public String CSSExtraClass = "";
        public String CSSExtraStyles = "";

        public String SetHeight = "";
        // public String CurrentBlockButtonDetails = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            webPart = this.Parent as BlockButtons;
            SPWebPartManager webPartManager = (SPWebPartManager)WebPartManager.GetCurrentWebPartManager(this.Page);
            Guid WebPartGuid = webPartManager.GetStorageKey(webPart);
            WebPartUniqueID = WebPartGuid.ToString().Replace("-", String.Empty); ;

            if (IsPostBack)
            {
                if (NeedToSave.Value == "true")
                {
                    WBLogging.Debug("Saving the new details: " + BlockButtonsDetails.Value);

                    string[] newDetails = BlockButtonsDetails.Value.WBxTrim().Split(',');

                    if (newDetails.Length != 5)
                    {
                        WBLogging.Debug("The details sent to this page have the wrong structure: " + BlockButtonsDetails.Value);
                        return;
                    }

                    webPart.BlockButtonsWidth = WBUtils.PutBackDelimiterCharacters(newDetails[0]);
                    webPart.BlockButtonsHeight = WBUtils.PutBackDelimiterCharacters(newDetails[1]);
                    webPart.BlockButtonsDetails = WBUtils.PutBackDelimiterCharacters(newDetails[2]);

                    // We're not editing this extra parts via the pop-up dialog so we're not saving them either - this code is here just for illustration:
                    // webPart.CssExtraClass = WBUtils.PutBackDelimiterCharacters(newDetails[3]);
                    // webPart.CssExtraStyles = WBUtils.PutBackDelimiterCharacters(newDetails[4]);

                    webPartManager.SaveChanges(WebPartGuid);

                    SPContext.Current.Web.Update();
                }
            }

            CSSExtraClass = webPart.CssExtraClass;
            CSSExtraStyles = webPart.CssExtraStyles;
            SetHeight = webPart.BlockButtonsHeight;

            String html = "";

            String[] buttonsDetails = webPart.BlockButtonsDetails.Split('^');

            int index = 0;
            foreach (String buttonDetails in buttonsDetails)
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

                html += "<td>\n<a class=\"block-button-link " + CSSExtraClass + "\"  id=\"wbf-block-button-link-" + WebPartUniqueID + "-" + index + "\" href=\"" + link + "\">\n";
                html += "<div class=\"block-button block-button-group-" + WebPartUniqueID + " " + CSSExtraClass + "\" id=\"wbf-block-button-" + WebPartUniqueID + "-" + index + "\" style=\"background-color: " + buttonColor + "; border-color: " + buttonBorderColor + "; color: " + textColor + "; width: " + webPart.BlockButtonsWidth + "; height: " + webPart.BlockButtonsHeight + ";\">\n";
                html += "<div class=\"block-button-content " + CSSExtraClass + "\">\n";
                html += "<div class=\"block-button-title " + CSSExtraClass + "\" id=\"wbf-block-button-title-" + WebPartUniqueID + "-" + index + "\" " + ((String.IsNullOrEmpty(title)) ? " style=\" display: none;\"" : "") + ">" + title + "</div>\n";
                html += "<div class=\"block-button-extra-text " + CSSExtraClass + "\" id=\"wbf-block-button-extra-text-" + WebPartUniqueID + "-" + index + "\" " + ((String.IsNullOrEmpty(extraText)) ? " style=\" display: none;\"" : "") + ">" + extraText + "</div> \n";
                html += "</div></div></a></td>";

                index++;
            }

            BlockButtons.Text = html;

            if ((SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                  || (webPartManager.DisplayMode == WebPartManager.EditDisplayMode))
            {
                String[] detailsToEdit = new String[5];

                detailsToEdit[0] = WBUtils.ReplaceDelimiterCharacters(webPart.BlockButtonsWidth);
                detailsToEdit[1] = WBUtils.ReplaceDelimiterCharacters(webPart.BlockButtonsHeight);
                detailsToEdit[2] = WBUtils.ReplaceDelimiterCharacters(webPart.BlockButtonsDetails.WBxTrim());
                detailsToEdit[3] = WBUtils.ReplaceDelimiterCharacters(webPart.CssExtraClass);
                detailsToEdit[4] = WBUtils.ReplaceDelimiterCharacters(webPart.CssExtraStyles);

                String currentDetails = String.Join(",", detailsToEdit);

                // currentDetails = HttpUtility.UrlEncode(currentDetails);

                BlockButtonsDetails.Value = currentDetails;

                InEditMode = true;
                EditBlockButtonsButton.OnClientClick = "WBF_editBlockButtons(WBF_EditDialogCallback" + WebPartUniqueID + ", \"" + BlockButtonsDetails.ClientID + "\"); return false;";
            }
            else
            {
                EditBlockButtonsButton.OnClientClick = "";
            }


            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpdateBlockButtons" + WebPartUniqueID, String.Format("WBF_checkBlockButtonsHeights('{0}', '{1}');", WebPartUniqueID, SetHeight), true);
        }

        private String ButtonToBorderColor(string buttonColorString)
        {
            Color buttonColor = System.Drawing.ColorTranslator.FromHtml(buttonColorString);

            int red = 32 + (int)(((int)buttonColor.R) * 1.5);
            int green = 32 + (int)(((int)buttonColor.G) * 1.5);
            int blue = 32 + (int)(((int)buttonColor.B) * 1.5);

            if (red > 255) red = 255;
            if (green > 255) green = 255;
            if (blue > 255) blue = 255;

            if (red < 0) red = 0;
            if (green < 0) green = 0;
            if (blue < 0) blue = 0;

            Color borderColor = Color.FromArgb(red, green, blue);

            return "#" + borderColor.R.ToString("X2") + borderColor.G.ToString("X2") + borderColor.B.ToString("X2"); ;
        }

        private String ButtonToTextColor(string buttonColorString)
        {
            Color buttonColor = System.Drawing.ColorTranslator.FromHtml(buttonColorString);

            double multiplier = 1.25;
            int constant = 48 + (((3 * 255) - buttonColor.R - buttonColor.G - buttonColor.B) / (5)); ;

            // If the button colour is lightish then we'll go darker with the text:
            //if (buttonColor.R > 160 || buttonColor.G > 160 || buttonColor.B > 160)
            if ((buttonColor.R + buttonColor.G + buttonColor.B > 400) || (buttonColor.G > 216))
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

            return "#" + textColor.R.ToString("X2") + textColor.G.ToString("X2") + textColor.B.ToString("X2"); ;
        }

    }
}
