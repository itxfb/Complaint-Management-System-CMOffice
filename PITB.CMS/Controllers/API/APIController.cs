using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System;
using OfficeOpenXml;
using System.IO;
using System.Diagnostics;
using System.Text;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View.Message;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.View.Select2;
using PITB.CMS_Common.Models.View.Table;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Handler.Notification;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.Data_Representation;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.View.Reports;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Handler.ExportFileHandler;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using PITB.CMS_Common.Handler.CustomJsonConverter;

namespace PITB.CMS.Controllers
{
    public class GeneralApiController : Controller
    {

        #region ListingServerSide

        [System.Web.Mvc.HttpPost]
        public JsonResult GetAgentComplaints([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] int complaintType, [FromBody] int listingType)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);

            commaSeperatedCampaigns = string.IsNullOrEmpty(commaSeperatedCampaigns) ? "-1" : commaSeperatedCampaigns;
            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt = BlAgent.GetComplaintListings(dtModel, from, to, commaSeperatedCampaigns, complaintType, "Listing", listingType);
            dt = Utility.GetAlterStatusDataTable(dt, "Campaign_Id", "Status", Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            //dt = BlAgent.GetAlterDataTable(dt);
            List<VmAgentComplaintListing> dataTable = dt.ToList<VmAgentComplaintListing>();
            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult GetPublicUserComplaints([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] int complaintType, [FromBody] int listingType)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);

            commaSeperatedCampaigns = string.IsNullOrEmpty(commaSeperatedCampaigns) ? "-1" : commaSeperatedCampaigns;
            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt = BlPublicUser.GetComplaintListings(dtModel, from, to, commaSeperatedCampaigns, complaintType, "Listing", listingType);
            // dt = Utility.GetAlterStatusDataTable(dt, "Campaign_Id", "Status", Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            //dt = BlAgent.GetAlterDataTable(dt);
            List<VmAgentComplaintListing> dataTable = dt.ToList<VmAgentComplaintListing>();
            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult GetTaggingListing([FromBody] string aoData)
        {

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

            List<VmAgentTagListing> dataTable = BlTag.GetTagListVmAgentData(dtModel);
            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetMessagingListing([FromBody] string aoData)
        {

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

            List<VmStakeholderMessageListing> dataTable = BlMessages.GetMessagingListDataTable(dtModel);
            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }



        [System.Web.Mvc.HttpPost]
        public JsonResult GetStakeholderComplaints([FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedStatuses = string.Join(",", statuses);
            var dataTable = BlComplaints.GetComplaintsOfStakeholder(from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses);

            return Json(new { aaData = dataTable }, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        public virtual JsonResult GetStakeholderComplaintsServerSide([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

            List<VmStakeholderComplaintListing> dataTable = BlComplaints.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "Listing",
                userId).ToList<VmStakeholderComplaintListing>();

            if (campaign.Contains(Config.Campaign.FixItLwmc.ToInt().ToString()))
            {
                foreach (VmStakeholderComplaintListing listing in dataTable)
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

            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }



        /*
            [System.Web.Mvc.HttpPost]
            public JsonResult GetStakeholderSuggestionServerSide([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries)
            {
                string commaSeperatedCampaigns = string.Join(",", campaign);
                string commaSeperatedCategories = string.Join(",", cateogries);
                //string commaSeperatedStatuses = string.Join(",", statuses);

                DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

                List<VmStakeholderComplaintListing> dataTable = BlComplaints.GetStakeHolderServerSideListDenormalized(from, to, dtModel, commaSeperatedCampaigns, commaSeperatedCategories, "", Config.ComplaintType.Suggestion);

                int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

                return Json(new
                {
                    data = dataTable,
                    draw = dtModel.Draw,
                    recordsTotal = totalRows,//dtModel.Length,
                    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                }, JsonRequestBehavior.AllowGet);
            }


            [System.Web.Mvc.HttpPost]
            public JsonResult GetStakeholderInquiryServerSide([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries)
            {
                string commaSeperatedCampaigns = string.Join(",", campaign);
                string commaSeperatedCategories = string.Join(",", cateogries);
                //string commaSeperatedStatuses = string.Join(",", statuses);

                DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

                List<VmStakeholderComplaintListing> dataTable = BlComplaints.GetStakeHolderServerSideListDenormalized(from, to, dtModel, commaSeperatedCampaigns, commaSeperatedCategories, "", Config.ComplaintType.Inquiry);

                int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

                return Json(new
                {
                    data = dataTable,
                    draw = dtModel.Draw,
                    recordsTotal = totalRows,//dtModel.Length,
                    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                }, JsonRequestBehavior.AllowGet);
            }*/
        #endregion

        [System.Web.Mvc.HttpPost]
        public JsonResult Districts([FromBody] int id, int campaignId) // Province Id
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.District);
            districts = DbDistrict.GetDistrictByProvinceAndGroup(id, groupId);

            //var data = DbDistrict.GetDistrictList(provinceId);
            var jsonToReturn = from d in districts
                               select new { Value = d.District_Id, Text = d.District_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult ProvinceByCampaignId(int campaignId)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbProvince> province = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.Province);
            province = DbProvince.AllProvincesList();

            //var data = DbDistrict.GetDistrictList(provinceId);
            var jsonToReturn = from d in province
                               select new { Value = d.Province_Id, Text = d.Province_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult DistrictByDivisionId([FromBody] int id, int campaignId) // Division Id
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.District);
            //var data = DbDistrict.GetDistrictList(provinceId);
            var jsonToReturn = from d in DbDistrict.GetByDivisionAndGroupId(id, groupId)
                               select new { Value = d.District_Id, Text = d.District_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult Divisions([FromBody] int id, int campaignId) // Division Id
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbDistrict> districts = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.Division);
            //var data = DbDistrict.GetDistrictList(provinceId);
            var jsonToReturn = from d in DbDivision.GetByProvinceAndGroupId(id, groupId)
                               select new { Value = d.Division_Id, Text = d.Division_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult TownTehsils(int id, int campaignId) // DistrictId
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbTehsil> tehsils = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.Tehsil);

            tehsils = DbTehsil.GetByDistrictAndGroupId(id, groupId);

            var jsonToReturn = from t in tehsils
                               select new { Value = t.Tehsil_Id, Text = t.Tehsil_Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult SearchCampaign(string searchParam)
        {

            var listOfVmCampaigns = BlCampaignDepartments.GetCampaignTags(searchParam, AuthenticationHandler.GetCookie().UserId);

            return Json(listOfVmCampaigns, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult UnionCouncils([FromBody] int id, int campaignId) // TehsilId
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            //int? campaignId = Convert.ToInt32(cookie.Campaigns);

            List<DbUnionCouncils> unionCouncils = null;

            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.UnionCouncil);

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

        [System.Web.Mvc.HttpPost]
        public JsonResult UnionCouncilsMendatory([FromBody] int id, int campaignId) // TehsilId
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            //int? campaignId = Convert.ToInt32(cookie.Campaigns);

            List<DbUnionCouncils> unionCouncils = null;

            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.UnionCouncil);

            //if (cookie.Role == Config.Roles.Agent && campaignId == 35) // for smart lahore
            //{
            //campaignId = Convert.ToInt32(cookie.Campaigns);
            unionCouncils = DbUnionCouncils.GetUnionCouncilListByCouncilAndCampaign(id, groupId);
            // }
            //else
            //{
            // unionCouncils = DbUnionCouncils.GetUnionCouncilList(id, groupId);
            //}

            //unionCouncils.Insert(0, new DbUnionCouncils { Councils_Name = "-Dont know-", UnionCouncil_Id = 0 });
            var jsonToReturn = (from u in unionCouncils
                                select new { Value = u.UnionCouncil_Id, Text = u.Councils_Name });


            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetHTMLOfURL(string url)
        {
            string response = SOAPHelper.HttpGetProtocolGetUrlAsString(url);
            return Content(response);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult Wards(int id, int campaignId) // Union Council
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbWards> tehsils = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.Ward);

            var unionCouncils = DbWards.GetByUnionCouncilAndGroupId(id, groupId);


            unionCouncils.Insert(0, new DbWards { Wards_Name = "-Dont know-", Ward_Id = 0 });
            var jsonToReturn = (from u in unionCouncils
                                select new { Value = u.Ward_Id, Text = u.Wards_Name });


            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult WardsByDistrictId(int id, int campaignId) // Union Council
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbWards> tehsils = null;
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.Ward);

            var unionCouncils = DbWards.GetByDistrictAndGroup(id, groupId);


            //unionCouncils.Insert(0, new DbWards { Wards_Name = "-Dont know-", Ward_Id = 0 });
            var jsonToReturn = (from u in unionCouncils
                                select new { Value = u.Ward_Id, Text = u.Wards_Name });


            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult ComplaintTypes(int id) // complaintType
        {
            var jsonToReturn = from sb in DbComplaintType.GetByCampaignId(id)
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Mvc.HttpGet]
        public ActionResult GetComplaintsSubCategoryListForCategory(int categoryId)
        {
            var jsonToResult = from s in DbComplaintSubType.GetByComplaintType(categoryId)
                               select new { Value = s.Complaint_SubCategory, Text = s.Name };
            return Json(jsonToResult, JsonRequestBehavior.AllowGet);

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

        [System.Web.Mvc.HttpPost]
        public JsonResult ComplaintDepartment(string dataStr /*int campaignId, int complaintType, string tagId*/) // complaintType
        {
            List<DbDepartment> listDbDepartment = null;
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(dataStr);
            List<DbComplaintType> unionCouncils = null;
            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId((int)data.campaignId, (Config.ComplaintType)data.complaintType);

            //string tagId = Utility.GetSerializedJsonWithoutSpaces(data.tagData);
            //string tagVal = DbTagLookup.GetTag(tagId);

            string tagIdKey = Utility.GetSerializedJsonWithoutSpaces(data.tagData);
            DbConfiguration_Assignment dbConfigAssignment = DbConfiguration_Assignment.GetByKey(tagIdKey);
            
            if(dbConfigAssignment!=null)
            {
                dynamic dConfigVal = JsonConvert.DeserializeObject(dbConfigAssignment.Value, typeof(ExpandoObject), new DynamicJsonConverter());
                if(Utility.PropertyExists(dConfigVal, "listDepartments"))
                {
                    List<int> listDepIdsToLoad = dConfigVal.listDepartments;
                    listDbDepartment = DbDepartment.GetByDepartmentIds(listDepIdsToLoad);
                }
            }
            else if(listDbDepartment == null)
            {
                listDbDepartment = DbDepartment.GetByCampaignAndGroupId((int)data.campaignId, groupId);
            }

            var jsonToReturn = from sb in listDbDepartment
                               select new { Value = sb.Id, Text = sb.Name};

            //var jsonToReturn = from sb in DbComplaintType.GetByDepartmentAndGroupId(id, groupId)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult ComplaintTypesByDataStr(string dataStr) // complaintType
        {
            List<DbComplaintType> listComplaintTypes = null;
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(dataStr);
            string tagIdKey = Utility.GetSerializedJsonWithoutSpaces(data.tagData);
            DbConfiguration_Assignment dbConfigAssignment = DbConfiguration_Assignment.GetByKey(tagIdKey);

            if (dbConfigAssignment != null)
            {
                dynamic dConfigVal = JsonConvert.DeserializeObject(dbConfigAssignment.Value, typeof(ExpandoObject), new DynamicJsonConverter());
                if (Utility.PropertyExists(dConfigVal, "listCategories"))
                {
                    List<int> listCategories = dConfigVal.listCategories;
                    listComplaintTypes = DbComplaintType.GetByTypeIds(listCategories);
                }
            }
            else if (listComplaintTypes == null)
            {
                int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId((int)data.campaignId, (Config.ComplaintType) data.complaintType);
                data.departmentId = string.IsNullOrEmpty(data.departmentId) ? 0 : data.departmentId;
                listComplaintTypes = DbComplaintType.GetByDepartmentAndGroupId(Convert.ToInt32(data.departmentId), groupId);
               
            }
            var jsonToReturn = from sb in listComplaintTypes
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
            //----------------------------------------
            //return null;

            //int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, (Config.ComplaintType)complaintType);

            //var jsonToReturn = from sb in DbComplaintType.GetByDepartmentAndGroupId(id, groupId)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };
            //return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }


        [System.Web.Mvc.HttpPost]
        public JsonResult ComplaintTypesByDepartmentId(int id, int campaignId, int complaintType) // complaintType
        {
            List<DbComplaintType> unionCouncils = null;
            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, (Config.ComplaintType)complaintType);

            var jsonToReturn = from sb in DbComplaintType.GetByDepartmentAndGroupId(id, groupId)
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult ComplaintSubType(int id, int? campaignId, int? complaintType) // complaintType
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

        [System.Web.Mvc.HttpGet]
        public JsonResult GetDynamicOptionListAgainstSearch(int campaignId, int categoryId, string searchStr, int page) // Division Id
        {
            //var data = DbDistrict.GetDistrictList(provinceId);
            /*
            VmSelect2ServerSideDropDownLIst vmDDL = new VmSelect2ServerSideDropDownLIst();
            vmDDL.ListItems = BlDynamicCategories.GetServerSideSearchCategories(campaignId, categoryId, searchStr)
                              .Select(n=>new Select2ListItem{id = n.Id,text = n.Name}).ToList();
            
            vmDDL

            var objToReturn = 
            */

            VmSelect2ServerSideDropDownLIst vmServerSideDropDownList = BlDynamicCategories.GetDropDownListModel(campaignId, categoryId, searchStr, page);
            return Json(vmServerSideDropDownList, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetDynamicSchoolsListAgainstSearch(int districtId, string searchStr, int page, int schoolCategory) // Division Id
        {

            VmSelect2ServerSideDropDownLIst vmServerSideDropDownList = BlDynamicCategories.GetDropDownSchoolListModel(districtId, schoolCategory, searchStr, page);
            return Json(vmServerSideDropDownList, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetSchoolAgainstSchoolSearchStr(string schoolSearchResult) // Division Id
        {

            try
            {
                DbSchoolsMapping dbSchoolsMapping = BlSchool.GetSchoolAgainstSchoolSearch(schoolSearchResult);
                return Json(dbSchoolsMapping, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult DepartmentSubCategory(int id) // Department SubCategory
        {
            var jsonToReturn = from sb in DbDepartmentSubCategory.GetByCategoryId(id)
                               select new { Value = sb.Id, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetAgentsOfCampaign(string[] id) // complaintType
        {
            var jsonToReturn = from sb in DbUsers.GetUsersAgainstCampaigns(id.ToIntList()).OrderBy(m => m.Name)
                               select new { Value = sb.User_Id, Text = sb.Name };
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        //DbComplaintType.GetByCampaignIdAndCategoryId(campaignId);
        [System.Web.Mvc.HttpPost]
        public JsonResult GetStatusesOfCampaign(string[] id) // complaintType
        {
            //var jsonToReturn = from sb in DbComplaintType.GetByCampaignIds(id.ToIntList()).OrderBy(m => m.Name)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };
            var jsonToReturn = BlCommon.GetStatusListByCampaignIds(new List<string>(id).Select(int.Parse).ToList(), Config.Permissions.StatusesForComplaintListing);
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetCategoriesOfCampaignOfUser(string[] id) // complaintType
        {
            //var jsonToReturn = from sb in DbComplaintType.GetByCampaignIds(id.ToIntList()).OrderBy(m => m.Name)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };
            var jsonToReturn = BlComplaintType.GetUserCategoriesAgainstCampaign(id);
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Mvc.HttpPost]
        public JsonResult GetCategoriesOfCampaignOfUserWithDepartment(string[] campaignIds, string[] departmentIds) // complaintType
        {
            //var jsonToReturn = from sb in DbComplaintType.GetByCampaignIds(id.ToIntList()).OrderBy(m => m.Name)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };
            var jsonToReturn = BlComplaintType.GetUserCategoriesAgainstCampaignWithDepartment(campaignIds, departmentIds);
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetDepartmentsOfCampaignOfUser(string[] id)
        {
            var jsonToReturn = BlDepartment.GetDepartmentsOfCampaignOfUser(id);
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);
        }
        #region Export Excel
        //[System.Web.Mvc.HttpGet]
        [System.Web.Mvc.HttpPost]
        public JsonResult /*HttpResponseBase*/ ExportStakeHolderData(/*[FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1*/)
        {
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonData = new StreamReader(Request.InputStream).ReadToEnd();
            int dataId = BlCommon.Export(jsonData);
            return Json(dataId, JsonRequestBehavior.AllowGet);
            //Response.

            /*
            if (status == null) status = "";
            DataTable data = BlComplaints.GetStakeholderComplaintsForExport(
                startDate,
                endDate,
                campaign,
                category,
                status,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType);
            */
            /*
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable data = BlComplaints.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "ExcelReport",
                userId);
            
            int rowCount = data.Rows.Count;
            //return FileHandler.GetFile(Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData");
            //return FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");
            
            //HttpResponseBase responseBase = FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");
            //return Json(responseBase, JsonRequestBehavior.AllowGet);
            ExcelPackage excelPack = FileHandler.ExportToExcel(data, "Complaint Listing Data");
            int dataId = DataStateMVC.AddInPool(excelPack);

            //string asd = "sfdsdf";
            return Json(dataId, JsonRequestBehavior.AllowGet);
            */
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetDashboardLabelsStakeholderData([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] string dashboardType, [FromBody] int userId = -1)
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
            List<VmStakeholderComplaintDashboard> data = BlComplaints.GetStakeHolderServerSideListDenormalized(
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

        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportStakeHolderData(int dataId)
        {

            ExcelPackage data = (ExcelPackage)DataStateMVC.GetDataFromPool(dataId);
            string fileName = DataStateMVC.GetStoredObjectFromPool(dataId).FileName;
            string startDate = DataStateMVC.GetStoredObjectFromPool(dataId).StartDate;
            string endDate = DataStateMVC.GetStoredObjectFromPool(dataId).EndDate;
            string fileNameFull = string.Format("{0} {1} {2} {3}", fileName, startDate, endDate, "ComplaintsListingData.xlsx");
            DataStateMVC.RemoveFromPool(dataId);
            return FileHandler.Generate(Response, data, fileNameFull);
        }

        //[FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType
        /*[System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportStakeHolderDataLowerHierarchy(string startDate, string endDate, string campaign, string category, string status, int complaintType, int listingType)
         {
            //Response.
            if (status == null) status = "";
            DataTable data  = BlComplaints.GetStakeholderComplaintsForExport(
                startDate,
                endDate,
                campaign,
                category,
                status,
                complaintType,
                (Config.StakeholderComplaintListingType) listingType);
            int rowCount = data.Rows.Count;
            //return FileHandler.GetFile(Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData");
             return FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");
         }
        */
        /*
         [System.Web.Mvc.HttpGet]
         public FileContentResult ExportSuggestionData(string startDate, string endDate, string campaign, string category )
         {
             DataTable data = BlComplaints.GetStakeholderComplaintsForExport(
                 startDate,
                 endDate,
                 campaign,
                 category,
                 "",
                 Config.ComplaintType.Suggestion);
             int rowCount = data.Rows.Count;
             return FileHandler.GetFile(Config.FileType.Excel, data, "CRM Suggestion");
         GetDynamicOptionListAgainstSearch
         [System.Web.Mvc.HttpGet]
         public FileContentResult ExportInquiryData(string startDate, string endDate, string campaign, string category )
         {
             DataTable data = BlComplaints.GetStakeholderComplaintsForExport(
                 startDate,
                 endDate,
                 campaign,
                 category,
                 "",
                 Config.ComplaintType.Inquiry);
             int rowCount = data.Rows.Count;
             return FileHandler.GetFile(Config.FileType.Excel, data, "CRM Inquiry");
         }
        */
        /* 
        [System.Web.Mvc.HttpGet]
         public FileContentResult ExportAgentData(string startDate, string endDate, string campaign, int complaintType)
         {
             DataTable data = BlComplaints.GetAgentComplaintsForExport(
                 startDate,
                 endDate,
                 campaign,
                 complaintType);
             int rowCount = data.Rows.Count;
             return FileHandler.GetFile(Config.FileType.Excel, data, "CRM Complaints");
         }*/

        [System.Web.Mvc.HttpPost]
        public JsonResult ExportAgentData([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] int complaintType, [FromBody] int listingType)
        {
            //DataTable data = BlComplaints.GetAgentComplaintsForExport(
            //    startDate,
            //    endDate,
            //    campaign,
            //    complaintType);
            //int rowCount = data.Rows.Count;
            //return FileHandler.GetFile(Config.FileType.Excel, data, "CRM Complaints");

            string commaSeperatedCampaigns = string.Join(",", campaign);

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt = BlAgent.GetComplaintListings(dtModel, from, to, commaSeperatedCampaigns, complaintType, "Export", listingType);
            dt = Utility.GetAlterStatusDataTable(dt, "Campaign_Name", "Status", Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            //dt = BlAgent.GetAlterDataTable(dt);
            //List<VmAgentComplaintListing> dataTable = dt.ToList<VmAgentComplaintListing>();
            int rowCount = dt.Rows.Count;
            string sheetName = string.Format("{0} Listing Data", ((Config.ComplaintType)complaintType).GetPluralName());
            ExcelPackage excelPack = FileHandler.ExportToExcel(dt, sheetName);
            int dataId = DataStateMVC.AddInPool(excelPack);

            //string asd = "sfdsdf";
            return Json(dataId, JsonRequestBehavior.AllowGet);
            //return FileHandler.GetFile(Config.FileType.Excel, dt, "CRM Complaints");

            //return Json(new
            //{
            //    data = dataTable,
            //    draw = dtModel.Draw,
            //    recordsTotal = totalRows,//dtModel.Length,
            //    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            //}, JsonRequestBehavior.AllowGet);

        }


        [System.Web.Mvc.HttpPost]
        public JsonResult ExportPublicUserData([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] int complaintType, [FromBody] int listingType)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt = BlPublicUser.GetComplaintListings(dtModel, from, to, commaSeperatedCampaigns, complaintType, "Export", listingType);
            //dt = Utility.GetAlterStatusDataTable(dt, "Campaign_Name", "Status", Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            int rowCount = dt.Rows.Count;
            string sheetName = string.Format("{0} Listing Data", ((Config.ComplaintType)complaintType).GetPluralName());
            ExcelPackage excelPack = FileHandler.ExportToExcel(dt, sheetName);
            int dataId = DataStateMVC.AddInPool(excelPack);

            return Json(dataId, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpGet]
        public FileContentResult ExportTaggingData(string dtData)
        {
            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(dtData);

            DataTable data = BlTag.GetTaggingListDataTable(dtModel);
            int rowCount = data.Rows.Count;

            return FileHandler.GetFile(Config.FileType.Excel, data, "CRM Tagging Data");
        }

        [System.Web.Mvc.HttpGet]
        public FileContentResult ExportMessageThreadData()
        {
            //DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(dtData);

            DataTable data = BlMessages.GetAllThreadListingForExport();
            //int rowCount = data.Rows.Count;

            return FileHandler.GetFile(Config.FileType.Excel, data, "CRM Messages Data");
        }

        #endregion

        [System.Web.Mvc.HttpGet]
        public JsonResult GetComlaintsTransferedHistoryTable(int complaintId)
        {
            List<VmTableTransferHistory> listTableAssignedTo = TransferHandler.GetComplaintHistoryTableList(complaintId);

            return Json(new
            {
                //aaData = listComplaints
                aaData = listTableAssignedTo
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetStakeholderExpiringComplaintsList([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1)
        {
            try
            {
                string spType = "ListingExpiring";//"ListingExpiring";

                string commaSeperatedCampaigns = string.Join(",", campaign);
                string commaSeperatedCategories = string.Join(",", cateogries);
                string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
                string commaSeperatedStatuses = "";
                if (statuses != null)
                {
                    commaSeperatedStatuses = string.Join(",", statuses);
                }

                DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

                List<VmStakeholderComplaintListingSchoolEducation> dataTable = BlComplaints.GetStakeHolderServerSideListDenormalized(
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
                    userId).ToList<VmStakeholderComplaintListingSchoolEducation>();

                int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

                return Json(new
                {
                    data = dataTable,
                    draw = dtModel.Draw,
                    recordsTotal = totalRows,//dtModel.Length,
                    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [System.Web.Mvc.HttpPost]
        public JsonResult GetPLRAComplaintsServerSide([FromBody] string from, [FromBody] string to, [FromBody] string aoData)
        {
            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            string filePath = Server.MapPath("~/ComplaintsXML");
            List<VmStakeholderComplaintListingPLRA> dataTable = BlPLRA.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel, filePath
               ).ToList<VmStakeholderComplaintListingPLRA>();

            int totalRows = dataTable.Count == 0 ? 0 : dataTable[0].Total_Rows;
            var jsonResult = Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,
                recordsFiltered = totalRows,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetNotifcationCount(int notificationType, int campaignId, int notificationStatus)
        {
            int count = NotificationHandler.GetNotificationCount((Config.NotificationType)notificationType, campaignId, (Config.NotificationStatus)notificationStatus).Count;
            Notification notification = new Notification();
            notification.Count = count;
            return Json(new
            {
                //aaData = listComplaints
                Data = notification
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetNotifcationList(int notificationType, int campaignId, int notificationStatus)
        {
            //int count = NotificationHandler.GetNotificationCount((Config.NotificationType)notificationType, campaignId, (Config.NotificationStatus)notificationStatus).Count;
            Notification notification = new Notification();
            List<DbNotificationLogs> listDbNotificationLogs = NotificationHandler.GetNotificationCount((Config.NotificationType)notificationType, campaignId,
                (Config.NotificationStatus)notificationStatus);
            notification.ListNotification = listDbNotificationLogs;
            notification.Count = listDbNotificationLogs.Count;
            return Json(new
            {
                //aaData = listComplaints
                Data = notification
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetComlaintsStatusChangeHistoryTable(int complaintId)
        {
            List<VmTableStatusHistory> listTableStatusChangeHistory = StatusHandler.GetComplaintStatusChangeHistoryTableList(complaintId);

            return Json(new
            {
                //aaData = listComplaints
                aaData = listTableStatusChangeHistory
            }, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpGet]
        public ContentResult GetHierarchyReponseTime(int hierarchyId, string campaignId, int escalationId, string startDate, string endDate)
        {
            try
            {
                DateTime d1 = new DateTime();

                bool date1 = DateTime.TryParseExact(startDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d1);
                if (date1 == true)
                {
                    startDate = d1.ToString("dd/MM/yyyy");
                }

                DateTime d2 = new DateTime();
                bool date2 = DateTime.TryParseExact(endDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d2);
                if (date2 == true)
                {
                    endDate = d2.ToString("dd/MM/yyyy");
                }

                DateTime startDateDT = DateTime.Parse(startDate);

                DateTime endDateDT = DateTime.Parse(endDate);

                BlDataRepresentation.filePath = Server.MapPath(@"~/Exception.txt");
                string xml = BlDataRepresentation.GetHierarchyReponseTimeData(hierarchyId, campaignId, escalationId, startDateDT, endDateDT);
                return this.Content(xml, "text/xml", System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                //string filePath = Server.MapPath(@"~/Response.txt");
                //System.IO.File.AppendAllText(filePath, ex.Message);
                //System.IO.File.AppendAllText(filePath, "Exception over");
            }
            return null;
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetRegionWiseResolvedComplaintCampaings() // Province Id
        {
            string campaigns = ConfigurationHandler.GetConfiguration("RegionWiseResolvedComplaint_Campaigns", Config.ConfigType.Config);
            List<DbCampaign> listDbCampaign = DbCampaign.GetByIds(Utility.GetIntList(campaigns));
            var jsonToReturn = from c in listDbCampaign select new { Value = c.Id, Text = c.Campaign_Name }
            ;
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetHierarchies(int campaignId)
        {
            string hierarchies = ConfigurationHandler.GetConfiguration("CampaignHierarchies_Campaign=" + campaignId, Config.ConfigType.Config);
            List<DbHierarchy> listDbHierarchy = DbHierarchy.Get(Utility.GetIntList(hierarchies));
            var jsonToReturn = from h in listDbHierarchy select new { Value = h.Id, Text = h.HierarchyName }
            ;
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetCategories()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();
            CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
            //var jsonToReturn = from sb in DbComplaintType.GetByCampaignId(id)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };
            string campaignId = postForm.GetElementValue("campaignId").ToString().Split(new string[] { "___" }, StringSplitOptions.None)[0];
            int categoryLevel = int.Parse(postForm.GetElementValue("categoryLevel"));
            string hierarchies = ConfigurationHandler.GetConfiguration("CampaignHierarchies_Campaign=" + 47, Config.ConfigType.Config);
            List<DbHierarchy> listDbHierarchy = DbHierarchy.Get(Utility.GetIntList(hierarchies));
            dynamic result = BlCommon.GetCategories(postForm);
            List<dynamic> listData = (List<dynamic>)result.data;

            var jsonToReturn = listData.Select(n => new { Value = n.key, Text = n.value }).ToList();

            //var jsonToReturn = from h in listDbHierarchy select new { Value = h.Id, Text = h.HierarchyName }
            //;
            return Json(jsonToReturn, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetHierarchyWiseReportData(int hierarchyId, string campaignIds, string statusIds, string startDate, string endDate)
        {
            List<VmHealthDistrictWiseReport> vm = BlReports.GetHierarchyWiseReportData(hierarchyId, campaignIds, statusIds, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

            return Json(new
            {
                data = vm
            }, JsonRequestBehavior.AllowGet);
        }
        public class HtmlPost
        {
            public int id { get; set; }

            public string htmlStr { get; set; }
        }

        [ValidateInput(false)]
        [System.Web.Mvc.HttpPost]
        public JsonResult ExportPdfWithHtml([FromBody] HtmlPost htmlData, [FromBody] int orientation = 2)
        {
            try
            {
                //string links =  GetCSSLinksString();
                //string html = RemoveHtmlTags(htmlData.htmlStr, new List<string>() { "link" });
                //string purgedHtml = new StringBuilder().AppendLine(html).AppendLine(links).ToString();        
                //var result = PreMailer.Net.PreMailer.MoveCssInline(purgedHtml);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.Orientation = (NReco.PdfGenerator.PageOrientation)orientation;
                htmlToPdf.CustomWkHtmlArgs = "-q --margin-top 5mm --margin-bottom 4mm --margin-right 10mm --margin-left 10mm";

                //htmlToPdf.CustomWkHtmlPageArgs = "--disable-smart-shrinking --viewport-size 1280x1024 ";
                //htmlToPdf.CustomWkHtmlTocArgs = "--disable-dotted-lines ";
                htmlToPdf.Size = NReco.PdfGenerator.PageSize.A3;
                htmlToPdf.PageFooterHtml = "<div style='text-align:right;'>Page <span class='page'></span> of <span class='topage'></span> </div>";
                //var pdfBytes = htmlToPdf.GeneratePdf(result.Html);
                var pdfBytes = htmlToPdf.GeneratePdf(htmlData.htmlStr);

                //var pdfBytes = ConvertToPdf(actualJson, Path.Combine(Server.MapPath("~/Libraries"), "wkhtmltopdf.exe"));      
                int dataId = DataStateMVC.AddInPool(pdfBytes);
                return Json(dataId, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        [ValidateInput(false)]
        [System.Web.Mvc.HttpPost]
        public JsonResult ExportPdfWithHtmlDisabledShrinking([FromBody] HtmlPost htmlData, [FromBody] int orientation = 2)
        {
            try
            {
                //string links = GetCSSLinksString();
                //links = "<!--StartLinks-->" + links + "<!--EndLinks-->";
                //string html = RemoveHtmlTags(htmlData.htmlStr, new List<string>() { "link" });
                //string purgedHtml = new StringBuilder().AppendLine(html).AppendLine(links).ToString();
                //var result = PreMailer.Net.PreMailer.MoveCssInline(purgedHtml);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.Orientation = (NReco.PdfGenerator.PageOrientation)orientation;
                htmlToPdf.CustomWkHtmlArgs = "-q --print-media-type --margin-top 10mm --margin-bottom 10mm --margin-right 10mm --margin-left 10mm ";

                htmlToPdf.CustomWkHtmlPageArgs = "--disable-smart-shrinking --viewport-size 1280x1024 ";
                //htmlToPdf.CustomWkHtmlPageArgs = "--print-media-type";

                htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
                htmlToPdf.PageFooterHtml = "<div style='text-align:right;'>Page <span class='page'></span> of <span class='topage'></span> </div>";

                int startingIndex = htmlData.htmlStr.IndexOf("<div id=\"changeStatus")==-1?htmlData.htmlStr.IndexOf("  <div class=\"box box-primary\" style=\"box-shadow: none;\">\n                    <div class=\"box-header-blue with-border btn-header\">\n                        <h4 class=\"modal-title\">Change Category</h4>") : htmlData.htmlStr.IndexOf("<div id=\"changeStatus");
                int endingIndex = htmlData.htmlStr.Length - 35;

                if (endingIndex > startingIndex && (startingIndex != -1 && endingIndex != -1))
                    htmlData.htmlStr = htmlData.htmlStr.Remove(startingIndex, endingIndex - startingIndex);

                htmlData.htmlStr = htmlData.htmlStr.Replace("textarea", "p");
                htmlData.htmlStr = htmlData.htmlStr.Replace("<button class=\"btn btn-primary btn-block\" type=\"submit\">Save</button>", " ");

                //Utility.WriteFile(@"C:\Apps\Crm\", "HtmlOrignal.txt", htmlData.htmlStr);
                //Utility.WriteFile(@"C:\Apps\Crm\", "HtmlPdf.txt", result.Html);
                //htmlToPdf.Zoom = 0.55f;
               
               

                
                //string htmlStr = result.Html;


                //var pdfBytes = htmlToPdf.GeneratePdf(result.Html);
                var pdfBytes = htmlToPdf.GeneratePdf(htmlData.htmlStr);
                //var pdfBytes = ConvertToPdf(actualJson, Path.Combine(Server.MapPath("~/Libraries"), "wkhtmltopdf.exe"));      
                int dataId = DataStateMVC.AddInPool(pdfBytes);
                return Json(dataId, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        [System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportPdfWithHtml(int dataId, string filename = "MainSummaryReport.pdf")
        {
            byte[] data = (byte[])DataStateMVC.GetDataFromPool(dataId);
            if (data != null)
            {
                DataStateMVC.RemoveFromPool(dataId);
                return FileHandler.Generate(Response, data, Config.ContentTypePdf, filename);
            }
            return null;
        }

        private string GetCSSLinksString()
        {
            StringBuilder strBuild = new StringBuilder();
            if (Directory.Exists(Server.MapPath("~/Content")))
            {
                strBuild.AppendLine(GetAllFilesLinks(Server.MapPath("~/Content")));
                IEnumerable<string> dirList = Directory.EnumerateDirectories(Server.MapPath("~/Content"), "*", SearchOption.AllDirectories);
                foreach (var path in dirList)
                {
                    strBuild.AppendLine(GetAllFilesLinks(path));
                }
            }
            if (Directory.Exists(Server.MapPath("~/css")))
            {
                strBuild.AppendLine(GetAllFilesLinks(Server.MapPath("~/css")));
                IEnumerable<string> dirList = Directory.EnumerateDirectories(Server.MapPath("~/css"), "*", SearchOption.AllDirectories);
                foreach (var path in dirList)
                {
                    strBuild.AppendLine(GetAllFilesLinks(path));
                }
            }
            return strBuild.ToString();
        }
        private string GetAllFilesLinks(string dirpath)
        {
            StringBuilder strBuild = new StringBuilder();
            IEnumerable<string> filesList = Directory.EnumerateFiles(dirpath, "*.css", SearchOption.AllDirectories);
            foreach (var filename in filesList)
            {
                strBuild.AppendFormat("<link rel='stylesheet' type='text/css' href='{0}' >", filename);
                strBuild.AppendLine();
            }
            return strBuild.ToString();
        }
        private string RemoveHtmlTags(string html, IEnumerable<string> tagsName)
        {
            foreach (string tag in tagsName)
            {
                for (int i = 0; i < html.Length; i++)
                {
                    if (html.ElementAt(i) == '<')
                    {
                        if (html.Substring(i + 1, tag.Length).Equals(tag))
                        {
                            for (int j = i; j < html.Length; j++)
                            {
                                if (html.ElementAt(j) == '>')
                                {
                                    html = RemoveIndexesFromHtml(html, i, j);
                                    i = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return html;
        }
        private string RemoveIndexesFromHtml(string html, int startIndex, int endIndex)
        {
            return html.Remove(startIndex, endIndex - startIndex + 1);
        }



        public byte[] ConvertToPdf(string htmlCode, string wkhtmlToPdfExePath)
        {
            Process process;
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = wkhtmlToPdfExePath;
            processStartInfo.WorkingDirectory = @"C:\";

            // run the conversion utility
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            // note: that we tell wkhtmltopdf to be quiet and not run scripts
            string switches = "-q -n ";
            switches += "--print-media-type ";
            switches += "--margin-top 10mm --margin-bottom 10mm --margin-right 10mm --margin-left 10mm ";
            switches += "--footer-right [page]/[topage] ";
            switches += "--page-size A4 - -";
            string args = "-q -n - -";
            //args += "--disable-smart-shrinking ";
            //args += "";
            //args += "--outline-depth 0 ";
            //args += "--grayscale ";
            //args += " - -";

            processStartInfo.Arguments = switches;

            process = Process.Start(processStartInfo);

            using (StreamWriter stramWriter = process.StandardInput)
            {
                stramWriter.AutoFlush = true;
                stramWriter.Write(htmlCode);
            }

            //read output
            byte[] buffer = new byte[32768];
            byte[] file;
            using (var memoryStream = new MemoryStream())
            {
                while (true)
                {
                    int read = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        break;
                    memoryStream.Write(buffer, 0, read);
                }
                file = memoryStream.ToArray();
            }

            process.StandardOutput.Close();
            // wait or exit
            process.WaitForExit(60000);

            // read the exit code, close process
            int returnCode = process.ExitCode;
            process.Close();

            process.Dispose();
            if (returnCode == 0 || returnCode == 1)
            {
                return file;
            }
            else
            {
                throw new Exception(string.Format("Could not create PDF, returnCode:{0}", returnCode));
            }

        }


        [System.Web.Mvc.HttpGet]
        public JsonResult GetTagsbyCampaign(int campaignId)
        {
            string[] Tags = BlCampaignDepartments.GetTagsbyCampaignId(Convert.ToInt32(campaignId));

            return new JsonResult() { Data = Tags, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult UpdateTagsbyCampaign(string TagsbyCampaign,int campaignId)
        {
            try
            {
                //string TagsbyCampaign = data["TagsbyCampaign"].ToString();
                //int campaignId = Convert.ToInt32(data["campaignId"]);
                BlCampaignDepartments.UpdateTagsbyCampaign(TagsbyCampaign, campaignId, AuthenticationHandler.GetCookie().UserId);

                return new JsonResult() { Data = "success", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {

                return new JsonResult() { Data = "Error", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }
    }
}