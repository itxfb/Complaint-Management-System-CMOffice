using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System;
using System.Dynamic;
using System.Text;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Authorization;
using PITB.CMS_Common;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Authentication;

namespace PITB.CMS.Controllers.View.LWMC
{
    [AuthorizePermission]
    public class LwmcController : GeneralApiController
    {
        // GET: LWMC
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [System.Web.Mvc.HttpPost]
        public  ActionResult GetStakeholderComplaintsServerSide2([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1)
        {
            HttpResponseMessage responseMessage =new HttpResponseMessage(HttpStatusCode.OK);
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt = BlLwmc.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType) listingType,
                "Listing",
                userId);

           // List<VmStakeholderComplaintListing> dataTable = dt.ToList<VmStakeholderComplaintListing>();

            List<dynamic> listDynamic = dt.ToDynamicList();
            //List<dynamic> listDynamic2 = dataTable.Cast<dynamic>().ToList();

            if (campaign.Contains(Config.Campaign.FixItLwmc.ToInt().ToString()))
            {
                foreach (dynamic listing in listDynamic)
                {
                    int complaintId = listing.ComplaintId;
                    List<DbAttachments> attachments = BlComplaints.GetComplaintAttachments(complaintId);
                    foreach (var row in attachments)
                    {
                        if (row.Source_Url != null)
                        {
                            if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.Add)
                            {
                                //listing.BeforePicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                                listing.BeforePictureSrc = row.Source_Url;
                            }
                            else if (row.ReferenceType != null && (int)row.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus)
                            {
                                //listing.AfterPicture = UtilityExtensions.GetImageByteArrayFromWebUrl(row.Source_Url, true);
                                listing.AfterPictureSrc = row.Source_Url;
                            }
                        }
                    }
                }
            }

            int totalRows = listDynamic.Count == 0 ? 0 : listDynamic.First().Total_Rows;
            try
            {
                dynamic dyn = new ExpandoObject();
                dyn.data = listDynamic;
                dyn.draw = dtModel.Draw;
                dyn.recordsTotal = totalRows;
                dyn.recordsFiltered = totalRows;
                
                //JsonResult result = Json(new
                //{
                //    data = listDynamic,
                //    draw = dtModel.Draw,
                //    recordsTotal = totalRows, //dtModel.Length,
                //    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                //}, JsonRequestBehavior.AllowGet);
                //string jsonStr = result.Data.ToString();
                string jsonStr = JsonConvert.SerializeObject(dyn);
                StringContent content=new StringContent(jsonStr,Encoding.UTF8,"application/json");
                responseMessage.Content = content;
                return Content(jsonStr, "application/json");
                //return jsonStr;
            }
            catch (Exception ex)
            {
                
            }

            return null;
            //return Json(new
            //{
            //    data = listDynamic,
            //    draw = dtModel.Draw,
            //    recordsTotal = totalRows,//dtModel.Length,
            //    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            //}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId,cookie.UserId, detailType);
            return PartialView("~/Views/Stakeholder/LWMC/LwmcDetail.cshtml", vmComplaintDetail);
        }
    }
}