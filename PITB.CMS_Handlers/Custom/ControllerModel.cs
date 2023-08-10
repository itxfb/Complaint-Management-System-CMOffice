using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common;
using PITB.CMS_Models.View;

namespace PITB.CMS_Handlers.Custom
{
    public class ControllerModel
    {
        public class Response
        {

            public string StatusMsg { get; set; }

            //public string ResponseType { get; set; }

            //public string ResponsePermissionData { get; set; }

            public Controller ControllerContext { get; set; }

            public string RedirectUrl { get; set; }

            public List<PartialViewData> ListPartialView { get; set; }

            //public string StrListPartialViewToLoadAfterRedirect { get; set; }

            public List<PartialViewData> ListPartialViewToLoadAfterRedirect { get; set; }

            
            public Response()
            {
                StatusMsg = "Success";
                ListPartialView = new List<PartialViewData>();
                ListPartialViewToLoadAfterRedirect = new List<PartialViewData>();
            }

            public Response(Controller ctrlContext)
            {
                StatusMsg = "Success";
                ControllerContext = ctrlContext;
                ListPartialView = new List<PartialViewData>();
                ListPartialViewToLoadAfterRedirect = new List<PartialViewData>();
            }

            public void AddMessagePartialView(Controller ctrController, List<PartialViewData> listPartialView,Config.CommandMessage commandMessage/*,VmMessage vmMessage*/)
            {
                VmMessage vmMessage = ctrController.StatusMessage(commandMessage.Status, commandMessage.Value);
                string partialViewHtml = ctrController.RenderViewToString(ctrController.ControllerContext, "~/Views/Shared/ViewUserControls/_MessageBox.cshtml", vmMessage, true);
                AddPartialView(listPartialView, "#PopupDiv2", partialViewHtml);
            }

            //public void AddMessagePartialView(List<PartialViewData> listPartialView, Config.CommandMessage commandMessage/*,VmMessage vmMessage*/)
            //{
            //    VmMessage vmMessage = ControllerContext.StatusMessage(commandMessage.Status, commandMessage.Value);
            //    string partialViewHtml = ControllerContext.RenderViewToString(ControllerContext.ControllerContext, "~/Views/Shared/ViewUserControls/_MessageBox.cshtml", vmMessage, true);
            //    AddPartialView(listPartialView, "#PopupDiv2", partialViewHtml);
            //}

            private void AddPartialView(List<PartialViewData> listPartialView, string selectorId, string htmlStr)
            {
                PartialViewData partialViewData = new PartialViewData();
                partialViewData.SelectorId = selectorId;
                partialViewData.HtmlString = htmlStr;
                listPartialView.Add(partialViewData);
            }

            public class PartialViewData
            {
                public string SelectorId { get; set; }
                public string HtmlString { get; set; }

            }

        }
    }
}