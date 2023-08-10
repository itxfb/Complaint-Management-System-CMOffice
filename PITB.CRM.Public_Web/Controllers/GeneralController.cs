using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CRM.Public_Web.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PITB.CRM.Public_Web.Controllers
{
    public class GeneralController : Controller
    {
        private const int CampaignId = 49;
        // GET: General
        [HttpPost]
        public JsonResult Districts(int id) // Province Id
        {
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.District);
            districts = DbDistrict.GetDistrictByProvinceAndGroup(id, groupId);
            var jsonToReturn = from d in districts
                               select new { Value = d.District_Id, Text = d.District_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DistrictByDivisionId(int id) // Division Id
        {
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.District);
            //var data = DbDistrict.GetDistrictList(provinceId);
            var jsonToReturn = from d in DbDistrict.GetByDivisionAndGroupId(id, groupId)
                               select new { Value = d.District_Id, Text = d.District_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Divisions(int id) // Division Id
        {
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.Division);
            //var data = DbDistrict.GetDistrictList(provinceId);
            var jsonToReturn = from d in DbDivision.GetByProvinceAndGroupId(id, groupId)
                               select new { Value = d.Division_Id, Text = d.Division_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TownTehsils(int id) // DistrictId
        {
            List<DbTehsil> tehsils = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.Tehsil);

            tehsils = DbTehsil.GetByDistrictAndGroupId(id, groupId);

            var jsonToReturn = from t in tehsils
                               select new { Value = t.Tehsil_Id, Text = t.Tehsil_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UnionCouncils(int id) // TehsilId
        {
            //int? campaignId = Convert.ToInt32(cookie.Campaigns);

            List<DbUnionCouncils> unionCouncils = null;

            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.UnionCouncil);

            //if (cookie.Role == Config.Roles.Agent && campaignId == 35) // for smart lahore
            //{
            //campaignId = Convert.ToInt32(cookie.Campaigns);
            unionCouncils = DbUnionCouncils.GetUnionCouncilListByCouncilAndCampaign(id, groupId);
            // }
            //else
            //{
            // unionCouncils = DbUnionCouncils.GetUnionCouncilList(id, groupId);
            //}

            unionCouncils.Insert(0, new DbUnionCouncils { Councils_Name = "-Dont know-", UnionCouncil_Id = 0 });
            var jsonToReturn = (from u in unionCouncils
                                select new { Value = u.UnionCouncil_Id, Text = u.Councils_Name });


            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Wards(int id) // Union Council
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbWards> tehsils = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.Ward);

            var unionCouncils = DbWards.GetByUnionCouncilAndGroupId(id, groupId);


            unionCouncils.Insert(0, new DbWards { Wards_Name = "-Dont know-", Ward_Id = 0 });
            var jsonToReturn = (from u in unionCouncils
                                select new { Value = u.Ward_Id, Text = u.Wards_Name });


            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ComplaintTypes(int id) // complaintType
        {
            var jsonToReturn = from sb in DbComplaintType.GetByCampaignId(id)
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        /*
        [System.Web.Mvc.HttpPost]
        public JsonResult ComplaintTypesByDepartmentId(int id) // complaintType
        {
            var jsonToReturn = from sb in DbComplaintType.GetByDepartmentId(id)
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        */
        [HttpPost]
        public JsonResult ComplaintTypesByDepartmentId(int id, int complaintType) // complaintType
        {
            List<DbComplaintType> unionCouncils = null;
            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(CampaignId, (Config.ComplaintType)complaintType);

            var jsonToReturn = from sb in DbComplaintType.GetByDepartmentAndGroupId(id, groupId)
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ComplaintSubType(int id, int complaintType) // complaintType
        {
            var jsonToReturn = from sb in DbComplaintSubType.GetByComplaintType(id)
                               select new { Value = sb.Complaint_SubCategory, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        /*
        [System.Web.Mvc.HttpPost]
        public JsonResult DepartmentCategory(int id) // Department SubCategory
        {
            var jsonToReturn = from sb in DbDepartmentSubCategory.GetByCategoryId(id)
                               select new { Value = sb.Id, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        */


        public ActionResult LoadDistricts(int id=1)
        {
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.District);
            districts = DbDistrict.GetDistrictByProvinceAndGroup(id, groupId);
            var model = from d in districts
                               select new { Value = d.District_Id, Text = d.District_Name };
            VmDropdown viewModel = new VmDropdown();

            foreach (var m in model)
            {
                viewModel.DropdownValuesList.Add(new VmDropdown.DropdownValues()
                {
                    Text = m.Text,
                    Value = m.Value.ToString()
                });
            }
            return PartialView("Dropdowns/_DistrictsDDL", viewModel);
        }

        [HttpPost]
        public ActionResult LoadTehsils(int SelectedValue)
        {
            List<DbTehsil> tehsils = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.Tehsil);

            tehsils = DbTehsil.GetByDistrictAndGroupId(SelectedValue, groupId);

            var model = from t in tehsils
                        select new { Value = t.Tehsil_Id, Text = t.Tehsil_Name };
            VmDropdown viewModel = new VmDropdown();

            foreach (var m in model)
            {
                viewModel.DropdownValuesList.Add(new VmDropdown.DropdownValues()
                {
                    Text = m.Text,
                    Value = m.Value.ToString()
                });
            }
            return PartialView("Dropdowns/_TehsilDDL", viewModel);
        }
        public ActionResult LoadUcs(int SelectedValue)
        {
            List<DbUnionCouncils> unionCouncils = null;

            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(CampaignId, Config.Hierarchy.UnionCouncil);
            unionCouncils = DbUnionCouncils.GetUnionCouncilListByCouncilAndCampaign(SelectedValue, groupId);
            var model = from t in unionCouncils
                        select new { Value = t.UnionCouncil_Id, Text = string.Format("{1}-{0}",t.UcNo,t.Councils_Name)};
            VmDropdown viewModel = new VmDropdown();

            foreach (var m in model)
            {
                viewModel.DropdownValuesList.Add(new VmDropdown.DropdownValues()
                {
                    Text = m.Text,
                    Value = m.Value.ToString()
                });
            }

            return PartialView("Dropdowns/UcsDDL", viewModel);
        }

    }
}