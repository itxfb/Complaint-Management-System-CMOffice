using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common;

namespace PITB.CMS_Common.Controllers
{
   
        public class Controller : System.Web.Mvc.Controller
        {
            protected override void HandleUnknownAction(string actionName)
            {
                string str = this.ControllerContext.ToString();
                RedirectToAction("Index").ExecuteResult(ControllerContext);
            }

            protected override void OnActionExecuting(ActionExecutingContext filterContext)
            {
               ViewBag.Message= TempData["Message"] ;
            }

            public VmMessage StatusMessage(Config.CommandStatus status,string successMessage, string failureMessage, string exceptionMessage)
            {
                VmMessage vmMessage= new VmMessage();
                switch (status)
                {
                        case Config.CommandStatus.Success:
                        vmMessage.MessageText = successMessage;
                        vmMessage.Type=Config.MessageType.success;
                        break;
                    case Config.CommandStatus.Failure:
                        vmMessage.MessageText = failureMessage;
                        vmMessage.Type=Config.MessageType.warning;
                        break;
                    case Config.CommandStatus.Exception:
                        vmMessage.MessageText = exceptionMessage;
                        vmMessage.Type=Config.MessageType.error;
                        break;
                }
                return vmMessage;
            }

            public VmMessage StatusMessage(Config.CommandStatus status, string message)
            {
                VmMessage vmMessage = new VmMessage();
                switch (status)
                {
                    case Config.CommandStatus.Success:
                        vmMessage.MessageText = message;
                        vmMessage.Type = Config.MessageType.success;
                        break;
                    case Config.CommandStatus.Failure:
                        vmMessage.MessageText = message;
                        vmMessage.Type = Config.MessageType.warning;
                        break;
                    case Config.CommandStatus.Exception:
                        vmMessage.MessageText = message;
                        vmMessage.Type = Config.MessageType.error;
                        break;
                }
                return vmMessage;
            }


            public ActionResult ReturnView(string path, object model=null)
            {
                if (model == null)
                {
                    return View(path);
                }
                else
                {
                    return View(path, model);    
                }
               
            }

            public string RenderViewToString(ControllerContext context,
                           string viewPath,
                           object model = null,
                           bool partial = false)
            {
                // first find the ViewEngine for this view
                ViewEngineResult viewEngineResult = null;
                if (partial)
                    viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
                else
                    viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

                if (viewEngineResult == null)
                    throw new FileNotFoundException("View cannot be found.");

                // get the view and attach the model to view data
                var view = viewEngineResult.View;
                context.Controller.ViewData.Model = model;

                string result = null;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(context, view,
                                                context.Controller.ViewData,
                                                context.Controller.TempData,
                                                sw);
                    view.Render(ctx, sw);
                    result = sw.ToString();
                }

                return result;
            }


            public string RenderActionResultToString(ActionResult result)
            {
                // Create memory writer.
                var sb = new StringBuilder();
                var memWriter = new StringWriter(sb);

                // Create fake http context to render the view.
                var fakeResponse = new HttpResponse(memWriter);
                var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request,
                    fakeResponse);
                var fakeControllerContext = new ControllerContext(
                    new HttpContextWrapper(fakeContext),
                    this.ControllerContext.RouteData,
                    this.ControllerContext.Controller);
                var oldContext = System.Web.HttpContext.Current;
                System.Web.HttpContext.Current = fakeContext;

                // Render the view.
                result.ExecuteResult(fakeControllerContext);

                // Restore old context.
                System.Web.HttpContext.Current = oldContext;

                // Flush memory and return output.
                memWriter.Flush();
                return sb.ToString();
            }


        }


        


        public class XmlResult : ActionResult
        {
            private object objectToSerialize;
            public XmlResult(object objectToSerialize)
            {
                this.objectToSerialize = objectToSerialize;
            }
            public object ObjectToSerialize
            {
                get { return this.objectToSerialize; }
            }
            public override void ExecuteResult(ControllerContext context)
            {
                if (this.objectToSerialize != null)
                {
                    context.HttpContext.Response.Clear();
                    var xs = new System.Xml.Serialization.XmlSerializer(this.objectToSerialize.GetType());
                    context.HttpContext.Response.ContentType = "text/xml";
                    xs.Serialize(context.HttpContext.Response.Output, this.objectToSerialize);
                }
            }
        }

    
    
}