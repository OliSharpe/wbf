using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class EditSubjectTagRouting : WorkBoxDialogPageBase
    {
        public WBTaxonomy subjectTagsTaxonomy;
        WBFarm farm;
        public WBSubjectTagsRecordsRoutings routings;

        protected void Page_Load(object sender, EventArgs e)
        {
            subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(SPContext.Current.Site);
            subjectTagsTaxonomy.InitialiseTaxonomyControl(SubjectTag, "Subject Tag", false);

            farm = WBFarm.Local;
            routings = farm.SubjectTagsRecordsRoutings(subjectTagsTaxonomy);

            if (!IsPostBack)
            {
                RouteIndex.Value = Request.QueryString["RouteIndex"];

                int index;
                if (Int32.TryParse(RouteIndex.Value, out index))
                {
                    WBSubjectTagRecordsRoutings routing = routings[index];

                    if (routing != null)
                    {
                        SubjectTag.Text = routing.SubjectTag.UIControlValue;
                        PublicDocumentsLibrary.Text = routing.PublicDocumentsLibrary;
                        ExtranetDocumentsLibrary.Text = routing.ExtranetDocumentsLibrary;
                    }
                }    
            }


        }

        protected void saveButton_OnClick(object sender, EventArgs e)
        {
            WBSubjectTagRecordsRoutings newRouting = new WBSubjectTagRecordsRoutings(subjectTagsTaxonomy, SubjectTag.Text, PublicDocumentsLibrary.Text, ExtranetDocumentsLibrary.Text);

            int index;
            if (Int32.TryParse(RouteIndex.Value, out index))
            {
                routings[index] = newRouting;
            }
            else
            {
                routings.Add(newRouting);
            }

            // Now save the new routing information:
            farm.SubjectTagsRecordsRoutingsString = routings.ToString();
            farm.Update();

            CloseDialogAndRefresh();
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithCancel();
        }

    }
}
