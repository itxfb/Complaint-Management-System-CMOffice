using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.Authentication
{
    public class ApiToken
    {
        public class Request : AuthenticationModel
        {
            
        }

        public class Response : ApiStatus
        {
            public string TokenSecret { get; set; }

            public string TokenValue { get; set; }
        }
    }
}