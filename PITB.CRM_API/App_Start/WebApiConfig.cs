using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PITB.CRM_API
{
    public static class WebApiConfig
    {
        public static Dictionary<string, string> dictRouteMapping = new Dictionary<string, string>()
        {
            {"api/PrivateSchool/SubmitComplaint", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.SubmitPrivateComplaint"},
            {"api/PrivateSchool/GetComplaints", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.GetPrivateComplaints"},

            {"api/AeoApp/SubmitComplaint", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.SubmitAeoAppComplaint"},
            {"api/AeoApp/GetComplaints", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.GetAeoAppComplaints"},

            {"api/eTransferSIS/GetCategories", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.GetSchoolCategories"},
            {"api/eTransferSIS/SubmitComplaint", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.SubmitETransferComplaint"},
            {"api/eTransferSIS/GetComplaints", "PITB.CMS_Common::ApiHandlers.Business.SchoolEducation.BlSchoolEducation.GetETransferComplaints"},
            //{"api/CombinedCampaign/SubmitComplaint2", "PITB.CMS::Handler.Business.BlCombinedCampaign.SubmitComplaint" },
            //{"api/SISGetUsersInfo2","PITB.CRM_API::Handlers.Business.SchoolEducation.BlSchoolEducationResolver.GetUsers" }
            {"api/PublicUser/Login", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.Login"},
            {"api/PublicUser/LoadForm", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.LoadForm"},
            {"api/PublicUser/AppStart", "PITB.CMS_Common::ApiHandlers.Business.BlApiDynamicForm.AppStart"},
            { "api/PublicUser/GetComplaints", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.PostDynamicData"},
            { "api/PublicUser/PostSignupForm", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.PostDynamicData"},
            //{ "api/PublicUser/GetUserSignupOTP", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.PostDynamicData"},
            { "api/PublicUser/PostComplaint", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.PostDynamicData"},
            { "api/PublicUser/PostStatus", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.PostDynamicData"},
            { "api/PublicUser/PostChangeTimeAndCategory", "PITB.CMS_Common::ApiHandlers.Business.PublicUser.BlApiPublicUser.PostDynamicData"}
        };
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //  config.SuppressDefaultHostAuthentication();
            //    config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes

            FunctionInvoker.RegisterURI(dictRouteMapping);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var formatter = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatter.JsonFormatter;
            /*JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };*/
            var settings = jsonFormatter.SerializerSettings;//jsonSettings;//jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();



            //var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Adding JSON type web api formatting.
            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);
            //config.Formatters.JsonFormatter.SerializerSettings = settings;
        }
    }
}
