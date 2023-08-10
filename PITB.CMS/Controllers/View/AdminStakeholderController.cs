using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Handler.Authorization;
using PITB.CMS_Common.Handler.Authentication;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class AdminStakeholderController : Controller
    {
        // GET: AdminStakeholder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddEditUser()
        {
            VmAddStakeholderUser vms = BIStakeholderAdmin.GetVmAddStakeholderUser();
            
            return View("~/Views/AdminStakeholder/AddEditUser.cshtml", AuthenticationHandler.GetCookie().MasterPage, vms);
        }
        public ActionResult AddCampaignTag()
        {
            VmAddStakeholderUser vms = BIStakeholderAdmin.GetVmAddCampaigns();
            ViewBag.TagsList = "asad,javed";
            return View("~/Views/AdminStakeholder/AddCampaignTag.cshtml", AuthenticationHandler.GetCookie().MasterPage, vms);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult OnAddUserSubmit(VmAddStakeholderUser vms)
        {
            ResponseData response = new ResponseData();
            
            if (ModelState.IsValid)
            {
                bool isduplicate = BIStakeholderAdmin.IsUserPresentWithUsername(vms.UserName);
                if (isduplicate)
                {
                    response.messageText = "Username already exists.";
                    response.css_cls = "error";
                    response.messageTitle = "Error";
                    response.statusId = 2;
                }
                else
                {
                    bool user = BIStakeholderAdmin.AddUser(vms);
                    if (user == false)
                    {
                        response.messageText = "User data is invalid.";
                        response.css_cls = "error";
                        response.messageTitle = "Error";
                        response.statusId = 2;
                    }
                    else
                    {
                        response.messageText = "User successfully added.";
                        response.css_cls = "success";
                        response.messageTitle = "Success";
                        response.statusId = 1;
                    }
                }
            }
                
               
            return Json(response);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult OnEditUserSubmit(VmEditStakeholderUser vms)
        {
            ResponseData response = new ResponseData();
            if (ModelState.IsValid)
            {
                var user = BIStakeholderAdmin.EditUserSubmit(vms);
                if (user != null)
                {
                    response.messageText = "User successfully edited.";
                    response.css_cls = "success";
                    response.messageTitle = "Success";
                    response.statusId = 1;
                }
            }
            else
            {
                response.messageText = "Error in saving data.";
                response.css_cls = "error";
                response.messageTitle = "Error";
                response.statusId = 2;
            }
           
            return Json(response);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult AddUser()
        {
            VmAddStakeholderUser vms = BIStakeholderAdmin.GetAddUser();

            return PartialView("~/Views/AdminStakeholder/_AddForm.cshtml", vms);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult EditUser(int UserID)
        {
            VmEditStakeholderUser vms = BIStakeholderAdmin.GetEditUser(UserID);

            return PartialView("~/Views/AdminStakeholder/_EditUserForm.cshtml", vms);
        }
        [System.Web.Mvc.HttpGet]
        public JsonResult GetProvinceList(int campaignId)
        {
            List<SelectListItem> selectLists = DbProvince.AllProvincesList().Select(x=> new SelectListItem(){Text = x.Province_Name,Value=x.Province_Id.ToString() }).ToList();
            //selectLists.Insert(0, new SelectListItem { Text = "--Select--", Value = (-1).ToString() });
            return Json(selectLists, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetDivisionList(int provinceId, int campaignId)
        {
            IEnumerable<SelectListItem> SelectList = DbDivision.GetSelectListofDivisionForCampaignAndProvinceIds(campaignId, provinceId);
            return Json(SelectList, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetDistrictList(int divisionId, int campaignId)
        {
            IEnumerable<SelectListItem> SelectList = DbDistrict.GetSelectListofDistrictForCampaignAndDivisionIds(campaignId, divisionId);
            return Json(SelectList, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetTehsilList(int districtId, int campaignId)
        {
            IEnumerable<SelectListItem> SelectList = DbTehsil.GetSelectListofTehsilForCampaignAndDistrictIds(campaignId, districtId);
            return Json(SelectList, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetUnionCouncilList(string tehsilId, int campaignId)
        {
            IEnumerable<SelectListItem> SelectList = DbUnionCouncils.GetSelectListofUnionCouncilForCampaignAndTehsilIds(campaignId, tehsilId);
            return Json(SelectList, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetWardList(int unioncouncilId, int campaignId)
        {
            IEnumerable<SelectListItem> SelectList = DbWards.GetSelectListofWardForCampaignAndUnionCouncilIds(campaignId, unioncouncilId);
            return Json(SelectList, JsonRequestBehavior.AllowGet);
        }
        private struct ResponseData
        {
            public int statusId;
            public string messageText;
            public string css_cls;
            public string messageTitle;
            public ResponseData(int id,string text,string title,string css)
            {
                statusId = id;
                messageText = text;
                messageTitle = title;
                css_cls = css;
            }
        }

        public ActionResult BlockUser()
        {
            //VmAddStakeholderUser vms = BIStakeholderAdmin.GetVmAddStakeholderUser();

            return View("~/Views/AdminStakeholder/BlockUser.cshtml", BlView.Instance.GetMasterPageFromCookie() /*BlView.Instance.SetMasterPageInCookie("~/Views/Shared/_AdminStakeholderLayout.cshtml")*/);
        }


        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public FileResult ExportTable(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "Grid.xls");
        }




        [System.Web.Mvc.HttpPost]
        public string GetBlockList()
        {
            NameValueCollection formCollection = Request.Form;
            //string aoData2 = formCollection["aoData"];
            string aoData = Request["aoData"];

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt= BIStakeholderAdmin.GetUserListings(dtModel);
            List<dynamic> listDynamic = dt.ToDynamic();
            int totalRecords=0, filteredRecords=0;
            if (listDynamic != null && listDynamic.Count > 0)
            {
                totalRecords = listDynamic[0].Total_Rows;
                filteredRecords = totalRecords;
            }
           
            //string str = JsonConvert.SerializeObject(listDynamic);
            //return Json(dt, JsonRequestBehavior.AllowGet);
            try
            {
                var jr = new 
                {
                    Data = new
                    {
                        aaData = listDynamic,
                        draw = dtModel.Draw,
                        recordsTotal = totalRecords,//dtModel.Length,
                        recordsFiltered = totalRecords, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return JsonConvert.SerializeObject(jr);
            }
            catch (Exception ex)
            {

            }
            

            return null;
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetUserActiveState(int userId, int isActive)
        {
            ResponseData rsp = new ResponseData();

            try
            {
                BIStakeholderAdmin.UpdatePersonActiveInactive(userId, isActive);
                rsp.statusId = 1;
                rsp.css_cls = "success";
                rsp.messageText = "User has " + (isActive == 1 ? "deactivated" : "activated") + " Successfully";
            }
            catch (Exception ex)
            {
                rsp.statusId = -1;
                rsp.css_cls = "error";
                rsp.messageText = "Exception =>" + ex.Message;
            }


            return Json(rsp, JsonRequestBehavior.AllowGet);
        }
    }
}