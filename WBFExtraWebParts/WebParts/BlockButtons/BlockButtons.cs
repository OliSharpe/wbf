using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WBFExtraWebParts.BlockButtons
{
    [ToolboxItemAttribute(false)]
    public class BlockButtons : WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("CSS Extra Class")]
        [WebDescription("An extra CSS class to add to all elements")]
        [System.ComponentModel.Category("Configuration")]
        public String CssExtraClass { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("CSS Extra Styles")]
        [WebDescription("Custom CSS styles to add to defaults")]
        [System.ComponentModel.Category("Configuration")]
        public String CssExtraStyles { get; set; }
        
        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Buttons Width")]
        [WebDescription("Width for all block buttons.")]
        [System.ComponentModel.Category("Configuration")]
        public String BlockButtonsWidth { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Buttons Height")]
        [WebDescription("Height for all block buttons.")]
        [System.ComponentModel.Category("Configuration")]
        public String BlockButtonsHeight { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Buttons Details")]
        [WebDescription("Details for all buttons.")]
        [System.ComponentModel.Category("Configuration")]
        public String BlockButtonsDetails { get; set; }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/WBFExtraWebParts/BlockButtons/BlockButtonsUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
