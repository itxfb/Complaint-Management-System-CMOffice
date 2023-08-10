using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Helper;
using System;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.LWMC.ResolverEnd
{
    public class LwmcPostOnSocialSiteController : ApiController
    {
       
        [HttpPost]
        public IHttpActionResult Post(PostOnSocialSiteRequestModel model)
        {
            try
            {
                
                if (ModelState.IsValid)
                {

                    return Ok(BlLwmcApiHandler.PostToFacebook(model));
                }
                return Ok(new ApiStatus("Validation Errors", Utility.ExtractAllValidationMessagesFromModelState(ModelState).ToJointString(" ")));

            }
            catch (Exception ex)
            {

                return Ok(new ApiStatus("Server Error",ex.Message));

            }
          

            
        }
    }
}