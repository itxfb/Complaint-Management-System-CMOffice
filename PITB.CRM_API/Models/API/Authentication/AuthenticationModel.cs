using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.API.Authentication;

namespace PITB.CRM_API.Models.Authentication
{
    public class AuthenticationModel
    {
        public string SystemName { get; set; }// for json

        public string SystemUserName { get; set; } // for json

        public string Ip { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ClientKey { get; set; }

        public string BodyStr { get; set; }

        public bool IsAuthenticated { get; set; }

        public bool IsIpAllowed { get; set; }

        public string HttpSchemePermission { get; set; }

        public string AppVersion { get; set; }

        public string AppUpdateUrl { get; set; }

        public string AppUpdateMessage { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public AuthenticationModel()
        {
            this.ClientKey = "-1";
            this.AppVersion = "-1";
            this.Status = "Success";
            this.Message = Config.ResponseType.Success.ToString();
        }

        public AuthenticationModel(string systemName, string userName, string password)
        {
            this.SystemName = systemName;
            this.Username = userName;
            this.Password = password;
            this.ClientKey = "-1";
            this.AppVersion = "-1";
            this.Status = "Success";
            this.Message = Config.ResponseType.Success.ToString();
        }

        public AuthenticationModel(string systemName, string ip, string userName, string password)
        {
            this.SystemName = systemName;
            this.Ip = ip;
            this.Username = userName;
            this.Password = password;
            this.ClientKey = "-1";
            this.AppVersion = "-1";
            this.Status = "Success";
            this.Message = Config.ResponseType.Success.ToString();
        }

        public void SetSuccess()
        {
            Status = "Success";
            Message = Config.ResponseType.Success.ToString();
        }

        public void SetAppUpdateStatus(AuthenticationModel authModel)
        {
            Status = "Update required";
            Message = authModel.AppUpdateMessage;
            AppUpdateUrl = authModel.AppUpdateUrl;
        }

        public void SetAuthenticationError()
        {
            Status = "Authentication Error";
            Message = Config.ResponseType.Failure.ToString();
        }
    }
}