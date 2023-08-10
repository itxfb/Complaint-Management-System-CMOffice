using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;
using System;
using System.Web.Http;

namespace PITB.CRM_API.Controllers
{
    public class LwmcGetStakeholderStatusesController : ApiController
    {
        public object Get(string userName, int appId, int languageId = 0, int platformId = 0, int appVersionId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                try
                {
                    return BlLwmcApiHandler.GetStakeholderValidStatuses(userName, (Config.Language)languageId, (Config.AppID)appId, (Config.PlatformID)platformId, appVersionId);
                }
                catch (Exception exception)
                {
                    return null;
                }
            }
            else
            {
                StatusList resp = new StatusList();
                //resp.SetAuthenticationError();
                resp.SetStatus(authModel);
                return resp;
            }

        }
       

      
    }
}