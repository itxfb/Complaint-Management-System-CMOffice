using PITB.CMS_Common.ApiModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.Custom
{
    public class ApiStatus
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public int Code { get; set; }

        public string AppUpdateUrl { get; set; }

        public ApiStatus()
        {
            
        }
        public ApiStatus(string status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        public void SetFailure()
        {
            Status = "Server Error";
            Message = Config.ResponseType.Failure.ToString();
            Code = 500;
        }
        public void SetSuccess()
        {
            Status = "Success";
            Message = Config.ResponseType.Success.ToString();
            Code = 200;
        }

        public void SetAuthenticationError()
        {
            Status = "Authentication Error";
            Message = Config.ResponseType.Failure.ToString();
            Code = 401;
        }

        public void SetStatus(AuthenticationModel authModel)
        {
            Status = authModel.Status;
            Message = authModel.Message;
            AppUpdateUrl = authModel.AppUpdateUrl;
        }
    }
}