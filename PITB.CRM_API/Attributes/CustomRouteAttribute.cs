using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PITB.CRM_API.Attributes
{
    public class CustomRouteAttribute : ActionFilterAttribute
    {
        public CustomRouteAttribute()
        {

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //Trace.WriteLine(string.Format("Action Method {0} executing at {1}", actionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), "Web API Logs");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Trace.WriteLine(string.Format("Action Method {0} executed at {1}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), "Web API Logs");
        }
    }


}