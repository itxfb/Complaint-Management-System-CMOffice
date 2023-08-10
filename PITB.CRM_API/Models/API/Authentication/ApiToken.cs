using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Authentication;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API.Authentication
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