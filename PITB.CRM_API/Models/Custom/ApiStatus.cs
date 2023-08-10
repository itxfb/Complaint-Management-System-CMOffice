using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Authentication;

namespace PITB.CRM_API.Models.Custom
{
    public class ApiStatus
    {
        public string Status { get; set; }

        public string Message { get; set; }

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
        }
        public void SetSuccess()
        {
            Status = "Success";
            Message = Config.ResponseType.Success.ToString();
        }

        public void SetAuthenticationError()
        {
            Status = "Authentication Error";
            Message = Config.ResponseType.Failure.ToString();
        }

        public void SetStatus(AuthenticationModel authModel)
        {
            Status = authModel.Status;
            Message = authModel.Message;
            AppUpdateUrl = authModel.AppUpdateUrl;
        }
    }
}