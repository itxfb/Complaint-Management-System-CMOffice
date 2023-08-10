using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Authorization;

namespace PITB.CMS.Controllers.View.ZimmedarShehri
{
    [AuthorizePermission]
    public class ZimmedarShehriController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: ZimmedarShehri
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId, cookie.UserId, detailType);
            return PartialView("~/Views/Stakeholder/ZimmedarShehri/ZimmedarShehriDetail.cshtml", vmComplaintDetail);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult OnStatusChange(VmStatusChange vmStatusChange, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                FileUploadHandler.FileValidationStatus validationStatus = BlZimmedarShehri.ChangeStatus(vmStatusChange, System.Web.HttpContext.Current.Request.Files);
                if (validationStatus.ValidationStatus == Config.AttachmentErrorType.NoError)
                {
                    TempData["Message"] = StatusMessage(Config.CommandStatus.Success, validationStatus.ValidationMessage,
                        Config.Message.UpdateSuccess, Config.Message.Error);
                }
                else
                {
                    TempData["Message"] = StatusMessage(Config.CommandStatus.Failure, "",
                        validationStatus.ValidationMessage, Config.Message.Error);
                }
                //TempData["Message"] = StatusMessage(Config.CommandStatus.Success, "Complaint " + vmStakeholderComplaint.Compaign_Id+"-"+vmStakeholderComplaint.ComplaintId + " status changed successfully!! ", Config.Message.UpdateSuccess, Config.Message.Error);


                //var status = vmStakeholderComplaint.AddComplaint(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0),
                //     (complaintModel.ComplaintVm.Id != 0));
                //TempData["Message"] = StatusMessage(status.Status, "Complaint added successfully , your complaint number : " + status.Value, Config.Message.Failure, Config.Message.Error);
                //return RedirectToAction("Search");

            }
            /*
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            return RedirectToAction("controllerName", "actionName");
            */

            //return StakeholderComplaintsListingServerSide();
            return RedirectToAction("StakeholderComplaintsListingServerSide", "Complaint");


            //ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            //VmAddComplaint vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), (int)complaintModel.PersonalInfoVm.Person_id,
            //     (int)complaintModel.ComplaintVm.Compaign_Id);

            //ViewBag.Campaignname = DbCampaign.GetById((int)complaintModel.ComplaintVm.Compaign_Id).LogoUrl;
            //return View("Add", vmodel);
            //return null;
            //return RedirectToAction("StakeholderComplaints");
            //return Json(true);
        }

        public ActionResult UserWiseComplaintsMain(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            ViewBag.CampaignName = BlView.GetAlteredCampaignName(cmsCookie, dbCamp.Campaign_Name);
            //if (cmsCookie.navigationalData != null && cmsCookie.navigationalData.campaignName != null)
            //    ViewBag.CampaignName = cmsCookie.navigationalData.campaignName;
            //else
            //    ViewBag.CampaignName = dbCamp.Campaign_Name;

            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampignIds = campaignId;
            // user cookie
            //VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(new AuthenticationHandler().CmsCookie.UserId, "2016-01-01", "2018-01-01", Config.UserWiseGraphType.CumulationOfLower);
            // end user cookie
            return View("~/Views/Stakeholder/ZimmedarShehri/UserWiseReportMain.cshtml", BlView.Instance.GetMasterPageFromCookie());


            /*

            DbUsers dbUsers = Utility.GetUserFromCookie();
            ViewBag.CampaignIds = campaignId;
            ViewBag.ProvinceName = Utility.GetHierarchyValue(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            if (dbUsers.Hierarchy_Id == Config.Hierarchy.District)
            {
                List<DbDistrict> listDbDistrict = new List<DbDistrict>();
                listDbDistrict.Add(DbDistrict.GetById(Convert.ToInt32(dbUsers.District_Id)));
                ViewBag.DistrictList = listDbDistrict.ToSelectList("District_Id", "District_Name");
                ViewBag.TehsilList = new List<SelectListItem>();
            }
            
            return View("~/Views/Stakeholder/ZimmedarShehri/UserWiseReportMain.cshtml");
            */
        }
        public ActionResult RegionAndStatusWiseCountSummaryDistrict(string divName, string startDate, string endDate, string provinceid, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.provinceid = Int32.Parse(provinceid);
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.RegionDict[hierarchyId];
            ViewBag.Hierarchy2 = Config.RegionDict[hierarchyId + 1];

            return PartialView("~/Views/Stakeholder/ZimmedarShehri/_RegionStatusWiseCountSummaryDistrict.cshtml");
        }
        public ActionResult RegionAndStatusWiseCountSummaryDivision(string divName, string startDate, string endDate, string provinceid, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.provinceid = Int32.Parse(provinceid);
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.RegionDict[hierarchyId];
            ViewBag.Hierarchy2 = Config.RegionDict[hierarchyId + 1];

            return PartialView("~/Views/Stakeholder/ZimmedarShehri/_RegionStatusWiseCountSummaryDivision.cshtml");
        }
        public ActionResult RegionWiseFeedbackSummary(string startDate, string endDate, string provinceId, int campaignId, int hierarchyId, string divTag, int upperHierarchyId)
        {
            ViewBag.divTag = divTag;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campaignId = campaignId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.provinceId = Int32.Parse(provinceId);
            ViewBag.Hierarchy1 = Config.RegionDict[hierarchyId];
            ViewBag.UpperHierarchyId = upperHierarchyId;
            return PartialView("~/Views/Stakeholder/ZimmedarShehri/_RegionWiseFeedbackSummary.cshtml");
        }
        public ActionResult GetDistrictComplaintsData(int campaignId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.provinceid = dbUsers.Province_Id;
            ViewBag.HierarchyId = 3;// Convert.ToInt32(dbUsers.Hierarchy_Id);
            ViewBag.UserHierarchyId = Convert.ToInt32(dbUsers.User_Hierarchy_Id);
            //ViewBag.commaSepVal = DbUsers.GetHierarchy((Config.Hierarchy)dbUsers.Hierarchy_Id, dbUsers);
            ViewBag.commaSepVal = DbDistrict.GetDistrictsByProvinceId(Convert.ToInt32(dbUsers.Province_Id));
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, Int32.Parse(dbUsers.Province_Id));
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/ZimmedarShehri/GetDistrictComplaintsData.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        public ActionResult GetDivisionComplaintsData(int campaignId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.provinceid = dbUsers.Province_Id;
            ViewBag.HierarchyId = 2;// Convert.ToInt32(dbUsers.Hierarchy_Id);
            ViewBag.UserHierarchyId = Convert.ToInt32(dbUsers.User_Hierarchy_Id);
            //ViewBag.commaSepVal = DbUsers.GetHierarchy((Config.Hierarchy)dbUsers.Hierarchy_Id, dbUsers);
            ViewBag.commaSepVal = DbDistrict.GetDistrictsByProvinceId(Convert.ToInt32(dbUsers.Province_Id));
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, Int32.Parse(dbUsers.Province_Id));
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/ZimmedarShehri/GetDivisionComplaintsData.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetTehsilComplaintsData(int campaignId)
        {
            DbUsers user = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, Int32.Parse(user.Province_Id));
            TempData["HierarchyId"] = user.Hierarchy_Id;
            TempData["ProvinceId"] = user.Province_Id;
            TempData["DivisionId"] = user.Division_Id;
            TempData["DistrictId"] = user.District_Id;
            if ((Config.Hierarchy?)user.Hierarchy_Id == Config.Hierarchy.Province)
            {
                TempData["DistrictList"] = ToSelectList(DbDistrict.GetDistrictsNameAndIDByProvinceId(Convert.ToInt32(user.Province_Id)));
            }
            else if ((Config.Hierarchy?)user.Hierarchy_Id == Config.Hierarchy.Division)
            {
                TempData["DistrictList"] = ToSelectList(DbDistrict.GetDistrictsByProvinceAndDivisionId(Convert.ToInt32(user.Province_Id), Convert.ToInt32(user.Division_Id)));
            }
            else if ((Config.Hierarchy?)user.Hierarchy_Id == Config.Hierarchy.District)
            {
                TempData["DistrictList"] = ToSelectList(new List<DbDistrict>() { DbDistrict.GetById(Int32.Parse(user.District_Id)) }.Select(x => new { DistrictId = x.District_Id, DistrictName = x.District_Name }));
            }
            TempData["Role"] = user.Role_Id;
            TempData["CampaignId"] = campaignId;
            TempData["UserHierarchyId"] = user.User_Hierarchy_Id;
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/ZimmedarShehri/TehsilByDistrict.cshtml", BlView.Instance.GetMasterPageFromCookie());

        }
        [System.Web.Mvc.HttpGet]
        public ActionResult TehsilByDistrictPartialView(string startDate, string endDate, int districtId)
        {
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            if (TempData.ContainsKey("ProvinceId"))
            {
                ViewBag.ProvinceId = Int32.Parse(TempData["ProvinceId"].ToString());
            }
            if (TempData["DivisionId"] != null)
            {
                ViewBag.DivisionId = Int32.Parse(TempData["DivisionId"].ToString());
            }
            ViewBag.DistrictId = districtId;
            ViewBag.UpperHierarchyId = districtId;
            if (TempData.ContainsKey("CampaignId"))
            {
                ViewBag.CampaignId = Int32.Parse(TempData["CampaignId"].ToString());
            }
            if (TempData.ContainsKey("UserHierarchyId"))
            {
                ViewBag.UserHierarchyId = Int32.Parse(TempData["UserHierarchyId"].ToString());
            }
            if (TempData["HierarchyId"] != null)
            {
                ViewBag.HierarchyId = 4;
            }
            if (TempData["CommaSepVal"] != null)
            {
                ViewBag.CommaSepVal = TempData["CommaSepVal"] as string;
            }
            TempData.Keep();
            ViewBag.StatusIds = "1,6,7,8,21,22";
            ViewBag.Hierarchy1 = Config.RegionDict[ViewBag.HierarchyId];
            ViewBag.divTag = 3;
            //PartialViewResult view = PartialView("~/Views/Stakeholder/ZimmedarShehri/_TehsilByDistrictPartialView.cshtml");
            return PartialView("~/Views/Stakeholder/ZimmedarShehri/_TehsilByDistrictPartialView.cshtml");
        }
        public SelectList ToSelectList(IEnumerable<object> lst)
        {
            SelectList response = new SelectList(lst, "DistrictId", "DistrictName");
            return response;
        }
        public ActionResult ProvinceDistrictPartialView(string startDate, string endDate, string provinceId, string campaignId, string districtId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampaignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = 3;
            ViewBag.UserHierarchyId = 0;
            ViewBag.commaSepVal = districtId;
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.GraphName = "Category";
            ViewBag.UserId = Utility.GetUserFromCookie().User_Id;
            ViewBag.CategoryType = Config.CategoryType.Main;
            ViewBag.CategoryId = -1;
            ViewBag.DistrictName = Utility.GetHierarchyValueName((Config.Hierarchy)Config.Hierarchy.District, (int)Convert.ToInt32(ViewBag.commaSepVal));
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.DistrictId = districtId;
            ViewBag.ProvinceId = provinceId;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            return PartialView("~/Views/Stakeholder/SchoolEducation/_ProvinceDistricts.cshtml");
        }



        public ActionResult StakeholderComplaintsListingServerSide(int userId, string from, string to, string statusId, string categories)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            int canShowStatusChangeInDetail = 0;//Convert.ToInt32(userId == cookie.UserId);
            canShowStatusChangeInDetail = Convert.ToInt32((userId == cookie.UserId) &&

                                              !PermissionHandler.IsPermissionAllowedInUser(
                                                  Config.Permissions.HideStatusChangeInComplaintsAssignedToMeStakeholder, cookie.UserId, cookie.Role));
            DbUsers dbUser = DbUsers.GetActiveUser(userId);

            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromDbUser(dbUser);
            ViewBag.LogoUrl =
                DbCampaign.GetLogoUrlByCampaignId(
                    Utility.GetIntByCommaSepStr(dbUser.Campaigns/*AuthenticationHandler.GetCookie().Campaigns*/));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            //  ViewBag.ComplaintTypeList =
            //       BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, dbUser);

            //ViewBag.ComplaintTypeList = DbComplaintType.GetByTypeIds(Utility.GetIntList(categories)).ToSelectList("Complaint_Category", "Name");
            ViewBag.ComplaintTypeList =
                BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, dbUser);
            List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, dbUser.User_Id);
            ViewBag.ListPermissions = listPermissions;
            ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.UserId = dbUser.User_Id;
            ViewBag.CanShowStatusChangeInDetail = canShowStatusChangeInDetail;
            //}
            //int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<SelectListItem> listStatuses = null;

            //if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIds)) // if is school education new campaign
            {
                if (canShowStatusChangeInDetail == 1)
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId)).Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
                    }
                    else
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), dbUser);
                    }
                }
                else
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId))
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                    else
                    {

                        listStatuses =
                            DbStatus.GetByCampaignId((int)Config.Campaign.ZimmedarShehri)
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                }
                listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.UnsatisfactoryClosedStatus);
                ViewBag.StatusList = listStatuses;
                return View("~/Views/Stakeholder/ZimmedarShehri/_ZimmedarShehriComplaintListingsServerSide.cshtml");
            }
            /*else
            {
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList());
                ViewBag.StatusList = listStatuses;
                return View("~/Views/Stakeholder/ComplaintListingsServerSide.cshtml");
            }*/

        }




        /*
        public JsonResult GetDashboardLabelsStakeholderData([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] string dashboardType, [FromBody] int userId = -1)
        {
            string spType = "";
            if (dashboardType == "_DashboardLabelStatus")
            {
                spType = "DashboardLabelsStausWise";
            }
            else if (dashboardType == "_DashboardLabelComplaintSrc")
            {
                spType = "DashboardLabelsComplaintSrc";
            }
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            List<VmStakeholderComplaintDashboard> data = BlSchool.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                spType,
                userId).ToList<VmStakeholderComplaintDashboard>();


            //ExcelPackage data = (ExcelPackage) DataStateMVC.GetDataFromPool(dataId);
            //DataStateMVC.RemoveFromPool(dataId);
            //return FileHandler.Generate(Response, data, "ComplaintsListingData.xlsx");
            return Json(data, JsonRequestBehavior.AllowGet);
            //return Json("aasasd", JsonRequestBehavior.AllowGet);

        }*/
    }
}