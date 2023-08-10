using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Modules.Api.Response
{
    public class ApiResponseHandlerMobile
    {
        public static dynamic SetServerError(dynamic d)
        {
            d = _SetDynamicIfNull(d);
            d.status = false;
            d.message = "ServerError";
            d.code = (int)Config.ResponseCodesMobile.ServerError;
            d.updateUrl = null;
            return d;
        }
        public static dynamic SetSuccess(dynamic d, string msg = null)
        {
            d = _SetDynamicIfNull(d);
            d.status = true;
            d.message = msg ?? Config.ResponseType.Success.ToString();
            d.code = (int)Config.ResponseCodesMobile.Success;
            d.updateUrl = null;
            return d;
        }

        public static dynamic SetNoDataFound(dynamic d, string msg = null)
        {
            d = _SetDynamicIfNull(d);
            d.status = true;
            d.message = msg ?? "No data found";
            d.code = (int)Config.ResponseCodesMobile.NoDataFound;
            d.updateUrl = null;
            return d;
        }

        public static dynamic SetAuthenticationError(dynamic d)
        {
            d = _SetDynamicIfNull(d);
            d.status = false;
            d.message = "Authentication error";//Config.ResponseType.Failure.ToString();
            d.code = (int)Config.ResponseCodesMobile.AuthenticationError;
            d.updateUrl = null;
            return d;
        }

        public static dynamic SetUnauthorizedError(dynamic d)
        {
            d = _SetDynamicIfNull(d);
            d.status = false;
            d.message = "User Unauthorized";
            d.code = (int)Config.ResponseCodesMobile.Unauthorized;
            d.updateUrl = null;
            return d;
        }

        public static dynamic SetAppUpdateUrl(dynamic d, string url)
        {
            d = _SetDynamicIfNull(d);
            d.status = false;
            d.message = "Application update is required"; //Config.ResponseType.Failure.ToString();
            d.code = (int)Config.ResponseCodesMobile.UpdateAppUrl;
            d.updateUrl = url;
            return d;
        }

        private static dynamic _SetDynamicIfNull(dynamic d)
        {
            if (d == null)
            {
                d = d ?? new ExpandoObject();
                d.data = null;
            }
            return d;
        }

        public static dynamic SetData(dynamic data)
        {
            dynamic d = new ExpandoObject();
            d.data = data;
            return d;
        }
    }
}
