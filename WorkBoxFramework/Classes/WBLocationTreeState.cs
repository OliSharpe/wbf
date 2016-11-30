using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBLocationTreeState
    {
        public String ViewMode;
        public String MinimumProtectiveZone;
        public SPWeb Web;

        public WBLocationTreeState(SPWeb web, String viewMode, String minimumProtectiveZone)
        {
            Web = web;
            ViewMode = viewMode;
            MinimumProtectiveZone = minimumProtectiveZone;
        }
    }
}
