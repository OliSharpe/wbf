﻿using System;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Publishing;
using WBFWebParts.PickRelatedDocuments;
using WorkBoxFramework;

namespace WBFWebParts.ControlTemplates.WBFWebParts
{
    public partial class FindWhereRecordIsBeingUsed : UserControl
    {

        SPSite _recordsSite = null;
        SPWeb _recordsWeb = null;
        SPList _recordsLibrary = null;

        public String recordID;
        String _itemId;
        public SPListItem recordItem = null;
        public String recordURL = "";
        String recordURLToSearchFor = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            recordID = Page.Request.QueryString["RecordID"];

            //            _listId = Page.Request.QueryString["ListId"];
            _itemId = Page.Request.QueryString["ItemId"];


            if (String.IsNullOrEmpty(recordID) && String.IsNullOrEmpty(_itemId))
            {
                recordItem = null;
                return;
            }


            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                String publicRecordsLibraryURL = WBFWebPartsUtils.GetRecordsLibraryURL(SPContext.Current.Site);
               // String publicExtranetRecordsLibraryURL = WBFWebPartsUtils.GetPublicExtranetLibraryURL(SPContext.Current);
              //  String protectedRecordsLibraryURL = WBFarm.Local.ProtectedRecordsLibraryUrl;

                using (_recordsSite = new SPSite(publicRecordsLibraryURL))
                using (_recordsWeb = _recordsSite.OpenWeb())
                {
                    _recordsLibrary = _recordsWeb.GetList(publicRecordsLibraryURL);

                    if (String.IsNullOrEmpty(_itemId))
                    {
                        recordItem = WBFWebPartsUtils.GetRecord(_recordsSite, _recordsWeb, _recordsLibrary, "", recordID);
                    }
                    else
                    {
                        recordItem = _recordsLibrary.GetItemById(Convert.ToInt32(_itemId));
                        recordID = recordItem.WBxGetAsString(WBColumn.RecordID);
                    }

                    recordURL = _recordsWeb.ServerRelativeUrl + "/" + recordItem.Url;
                    recordURLToSearchFor = recordItem.Url.Substring(recordItem.ParentList.RootFolder.Url.Length);
                }
            });

        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            if (recordItem == null || String.IsNullOrEmpty(recordID) && String.IsNullOrEmpty(_itemId))
            {
                writer.Write("<i>You have to pass in a RecordID or ItemId parameter for this page to work properly</i>");
                return;
            }

            // We're doing this to prevent IE from buffering the subsequent outputs to the browser.
            Page.Response.BufferOutput = false;
            writer.Flush();
            Response.Flush();

            Response.Write("<!--");
            Response.Write(new string('*', 256));
            Response.Write("-->");
            Response.Flush();

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                // This is a horrible hack - but this feature is only experimental at the moment until further
                // feedback has confirmed that the feature is going in right direction
//                string[] publishingSites = { "http://uatstagingweb/", "http://uatizzi/", "http://uatevidencehub/" };
                string[] publishingSites = { "http://izzi/", "http://stagingweb/", "http://evidencehub.stagingweb/" };

                bool originalCatchValue = SPSecurity.CatchAccessDeniedException;
                SPSecurity.CatchAccessDeniedException = false;

                try
                {
                    foreach (string publishingSiteURL in publishingSites)
                    {

                        using (SPSite site = new SPSite(publishingSiteURL))
                        using (SPWeb rootWeb = site.RootWeb)
                        {

                            AddResultsForSPWeb(site, rootWeb);

                            Response.Flush();
                        }
                    }
                }
                finally
                {
                    SPSecurity.CatchAccessDeniedException = originalCatchValue;
                }

                Response.Write("<script type=\"text/javascript\">finishedSearch();</script>\n");
                Response.Flush();

            });

        }

        private void AddResultsForSPWeb(SPSite site, SPWeb web)
        {
            WBLogging.RecordsTypes.Verbose("In AddResultsForSPWeb(): looking at SPWeb: " + web.Url);

            
            try
            {
                PublishingWeb pubWeb = PublishingWeb.GetPublishingWeb(web);

                foreach (PublishingPage page in pubWeb.GetPublishingPages())
                {
                    WBLogging.RecordsTypes.Verbose("In AddResultsForSPWeb(): page = " + page.WBxToString());

                    if (page != null)
                    {
                        AddResultsForPublishingPage(page);

                        WBLogging.RecordsTypes.Verbose("InAddResultsForSPWeb() just finished for AddResultsForPublishingPage(): page = " + page.Name);

                    }
                }

            }
            catch (Exception e)
            {
                WBLogging.RecordsTypes.Verbose("In FindWhereRecordIsBeingUsed.AddResultsForSPWeb(): This SPWeb is probably not a publishing site: " + web.Url);

                StringBuilder command = new StringBuilder();
                command.Append("<script type=\"text/javascript\">errorProcessingPage(\"").Append(web.Url).Append("\", \"An exception occured at an SPWeb level: ").Append(e.Message).Append("\");</script>\n");
                command.Append("<!-- Full exception stack using ToString(): ");
                command.Append(e.ToString()).Append("-->\n\n");

                command.Append("<!-- Full exception stack using FlattenException(): ");
                command.Append(FlattenException(e)).Append("-->\n\n");

                Response.Write(command.ToString());
                Response.Flush();
            }

            WBLogging.RecordsTypes.Verbose("InAddResultsForSPWeb() about to look at sub webs for SPWeb: " + web.Url);

            foreach (SPWeb subWeb in web.Webs)
            {
                if (subWeb != null)
                {
                    WBLogging.RecordsTypes.Verbose("InAddResultsForSPWeb() for a sub SPWeb: " + subWeb.Url);

                    AddResultsForSPWeb(site, subWeb);

                    WBLogging.RecordsTypes.Verbose("InAddResultsForSPWeb() returned from AddResultsForSPWeb for:" + subWeb.Url);

                    subWeb.Dispose();
                }
                else
                {
                    WBLogging.RecordsTypes.Verbose("InAddResultsForSPWeb() subweb was null:");

                }
            }
        

        }

        private void AddResultsForPublishingPage(PublishingPage page)
        {
            String errorString = "";
            bool inWebPart = false;
            bool inPageContent = false;

            StringBuilder command = new StringBuilder();
            String pageURL = page.PublishingWeb.Web.Url + "/" + page.Url;

            WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL);

            
            // If this 'page' is actually not an aspx page at all but something else (e.g. an image file) then just ignore it:
            if (!pageURL.ToLower().EndsWith(".aspx")) return;

            WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL + " About to try");


            try
            {
                SPLimitedWebPartManager webPartManager = page.ListItem.File.GetLimitedWebPartManager(PersonalizationScope.Shared); // web.GetLimitedWebPartManager(page.Uri.ToString(), PersonalizationScope.Shared);

                if (webPartManager != null)
                {

                    WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL + " got a non-null webPartManager");

                    foreach (System.Web.UI.WebControls.WebParts.WebPart existingWebPart in webPartManager.WebParts)
                    {
                        string pickedDocumentsDetails = null;

                        if (existingWebPart.GetType() == typeof(PickRelatedDocuments.PickRelatedDocuments))
                        {
                            PickRelatedDocuments.PickRelatedDocuments relatedDocumentsWebPart = (PickRelatedDocuments.PickRelatedDocuments)existingWebPart;

                            pickedDocumentsDetails = relatedDocumentsWebPart.PickedDocumentsDetails;
                        }

                        if (existingWebPart.GetType() == typeof(PickedDocumentsGroup.PickedDocumentsGroup))
                        {
                            PickedDocumentsGroup.PickedDocumentsGroup documentsGroupWebPart = (PickedDocumentsGroup.PickedDocumentsGroup)existingWebPart;

                            pickedDocumentsDetails = documentsGroupWebPart.PickedDocumentsDetails;
                        }

                        // If there are no listed documents then we can just skip this web part.
                        if (String.IsNullOrEmpty(pickedDocumentsDetails)) break;

                        WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL + " we've got some non-empty picked document details: " +pickedDocumentsDetails);


                        string[] documentsDetailsArray = pickedDocumentsDetails.Split(';');
                        foreach (string documentDetails in documentsDetailsArray)
                        {
                            string[] documentDetailsArray = documentDetails.Split('|');

                            if (documentDetailsArray.Length != 4)
                            {
                                errorString += "Badly formatted document details in PickRelatedDocuments web part: " + documentDetails + " - Ignoring these details";
                                continue;
                            }

                            string webPartItemRecordID = documentDetailsArray[1];

                            if (webPartItemRecordID == recordID)
                            {
                                inWebPart = true;
                                break;
                            }
                        }

                        if (inWebPart) break;
                    }
                }
                else
                {
                    WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL + " got a NULL webPartManager");
                }

                WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL + " About to do page content search");

                
                String content = page.ListItem.WBxGetColumnAsString("Page Content");
                if (content != null && content.Contains(recordURLToSearchFor)) inPageContent = true;

                if (inWebPart || inPageContent)
                {
                    command.Append("<script type=\"text/javascript\">foundUsage(\"").Append(pageURL).Append("\", \"").Append(inWebPart).Append("\", \"").Append(inPageContent).Append("\");</script>\n");
                }
                else
                {
                    if (String.IsNullOrEmpty(errorString))
                    {
                        command.Append("<script type=\"text/javascript\">justSearched(\"").Append(pageURL).Append("\");</script>\n");
                    }
                    else
                    {
                        command.Append("<script type=\"text/javascript\">errorProcessingPage(\"").Append(pageURL).Append("\", \"").Append(errorString).Append("\");</script>\n");
                    }
                }

                WBLogging.RecordsTypes.Verbose("In AddResultsForPublishingPage(): pageURL= " + pageURL + " Command = " + command.ToString());

            }
            catch (Exception e)
            {
                command.Append("<script type=\"text/javascript\">errorProcessingPage(\"").Append(pageURL).Append("\", \"An exception occured: ").Append(e.Message).Append("\");</script>\n");
                command.Append("<!-- Full exception stack using ToString(): ");
                command.Append(e.ToString()).Append("-->\n\n");

                command.Append("<!-- Full exception stack using FlattenException(): ");
                command.Append(FlattenException(e)).Append("-->\n\n");
            }

            Response.Write(command.ToString());
            Response.Flush();

        }

        private static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
                if (exception != null)
                {
                    stringBuilder.AppendLine("    ---- Inner Exception: ----");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
