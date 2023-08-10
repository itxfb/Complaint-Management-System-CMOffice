using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.ComplaintFileHandler;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Wasa;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common.Helper.Attributes;
using PITB.CMS_Common.Models.View.PublicUser;

namespace PITB.CMS.Controllers.View.PublicUser
{
    public class CmccPublicController : PublicUserController
    {

        public override ActionResult SignUpPage()
        {
            ViewBag.Provinces = DbProvince.AllProvincesList();
            ViewBag.CampaignController = "CmccPublic";
            return PartialView("~/Views/PublicUser/_PartialPublicUserSignUp.cshtml");
        }

        [AjaxRequestOnly]
        [ValidateJsonAntiForgeryToken]
        public override JsonResult SignUpUser(VmSignUp data)
        {
            dynamic message = new ExpandoObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    message = "notValid";
                }
                else
                {
                    dynamic d = new ExpandoObject();
                    d.UserName = data.UserName.Trim();
                    d.UserCnic = data.UserCnic.Trim();
                    d.UserMobileNo = data.UserMobileNo.Trim();
                    d.UserEmail = data.UserEmail.Trim();
                    d.UserPassword = data.UserPassword.Trim();
                    d.ConfirmPassword = data.ConfirmPassword.Trim();
                    d.UserGender = data.UserGender;
                    d.UserProvince = data.UserProvince;
                    d.UserDistrict = data.UserDistrict;
                    d.UserAddress = data.UserAddress.Trim();
                    d.Campaign_Id = Config.Campaign.CMCC;
                    d.Campaigns = new string[]{
                        ((byte)Config.Campaign.CMCC).ToString(),
                        ((byte)Config.Campaign.SchoolEducationEnhanced).ToString(),
                        ((byte)Config.Campaign.DcoOffice).ToString(),
                            ((byte)Config.Campaign.Police).ToString(),

                            ((byte)Config.Campaign.Health).ToString(),
                            ((byte)Config.Campaign.LGCDCMCC).ToString(),

                            


                    };
                    d.OTP = data.OTP;
                    d.src = "web";

                    dynamic result = BlPublicUser.SavePublicUser(d);
                    message = result.message;
                }
            }
            catch (Exception ex)
            {
                message = "error";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}