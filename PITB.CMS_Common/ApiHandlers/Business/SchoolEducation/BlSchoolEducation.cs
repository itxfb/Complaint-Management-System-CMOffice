using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
//using System.Web.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Data;
using System.Dynamic;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Handler.StakeHolder;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Dynamic;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.ApiModels.API.SchoolEducation.Sync.Response;
using PITB.CMS_Common.ApiModels.API.SchoolEducation;
using static PITB.CMS_Common.ApiModels.API.SchoolEducation.SISApiModel.Request;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Messages;
using System.IO;
using System.Text;

namespace PITB.CMS_Common.ApiHandlers.Business.SchoolEducation
{
    public class BlSchoolEducation
    {
        public static dynamic SubmitStakeholderLogin(dynamic d)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbUsers dbUser = DbUsers.GetUserAgainstUserNameAndPassword(d.username, d.password);
            bool isUsernamePresent = (dbUser != null);
            DbUsers dbUserTemp = null;
            dynamic response = new ExpandoObject();
            response.user = null;
            if (isUsernamePresent) // username and cnic present
            {
                response.user = new ExpandoObject();
                response.user.name = dbUser.Name;
                response.user.cnic = dbUser.Cnic;
                response.user.phoneNo = dbUser.Phone;
                response.user.districtId = DbCrmIdsMappingToOtherSystem.Get(1, 3, 1, 1, new List<int?> { int.Parse(dbUser.District_Id) }).FirstOrDefault().OTS_Id;
                response = Utility.GetApiResponse(true, "User present", null, response);
                return response;
            }
            else // if username not present
            {
                response = Utility.GetApiResponse(true, "User not found", null, response);
                return response;
            }

        }


        public static GetSchoolEducationComplaintModel GetSchoolComplaintsForMea(SchoolApiModel schoolApiModel)
        {
            List<string> listEmisCode = schoolApiModel.ListSchoolModel.Select(n => n.EmisCode).ToList();
            List<DbSchoolsMapping> listDbSchools = DbSchoolsMapping.GetByEmisCode(listEmisCode);

            List<DbComplaint> listDbComplaints = DbComplaint.GetByTableRef(Config.TableRef.SchoolEducation, Utility.GetNullableIntList(listDbSchools.Select(n => n.Id).ToList()));
            var catGroup = listDbComplaints.GroupBy(n => n.Complaint_SubCategory);
            List<int?> listCatGroup = new List<int?>();
            foreach (var cat in catGroup)
            {
                listCatGroup.Add(cat.Key);
            }
            List<DbSchoolCategoryUserMapping> listDbSchoolCategoryUserMappings =
                DbSchoolCategoryUserMapping.Get(Config.Campaign.SchoolEducationEnhanced, Config.Categories.Third,
                    listCatGroup, Config.SchoolEducationAssignedTo.MEA);

            List<int?> listCategoryIds = listDbSchoolCategoryUserMappings.Select(n => n.Category_Id).ToList();

            listDbComplaints = listDbComplaints.Where(n => listCategoryIds.Contains(n.Complaint_SubCategory) && n.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.ResolvedUnverified).ToList();


            // Start
            GetSchoolEducationComplaintModel getComlaintsModel = new GetSchoolEducationComplaintModel();
            getComlaintsModel.ListComplaint = new List<SchoolModelWrapper>();

            List<Picture> listStatusPicturesUrl = new List<Picture>();
            List<Picture> listComplaintPicturesUrl = new List<Picture>();
            List<Video> listComplaintVideoUrl = new List<Video>();
            List<int> listChangeStatusLogIds = null;

            List<DbAttachments> listDbAttachment = new List<DbAttachments>();

            getComlaintsModel.TotalComplaints = listDbComplaints.Count;
            foreach (DbComplaint dbComplaint in listDbComplaints)
            {
                //listAttachments.Clear();
                listChangeStatusLogIds = DbComplaintStatusChangeLog.GetActiveStatusChangeLogIdsListAgainstComplaintId(dbComplaint.Id);
                foreach (int changeLogId in listChangeStatusLogIds) //  get all attachment against change log ids
                {
                    listDbAttachment.AddRange(DbAttachments.GetByRefAndComplaintId(dbComplaint.Id, Config.AttachmentReferenceType.ChangeStatus, changeLogId));
                }

                if (listDbAttachment != null)
                {
                    listStatusPicturesUrl = listDbAttachment.Select(n => new Picture()
                    {
                        picture = n.Source_Url
                    }).ToList();
                }


                listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.AttachmentType.File);

                if (listDbAttachment != null)
                {
                    listComplaintPicturesUrl = listDbAttachment.Select(n => new Picture()
                    {
                        picture = n.Source_Url
                    }).ToList();
                }

                listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.AttachmentType.Video);

                if (listDbAttachment != null)
                {
                    listComplaintVideoUrl = listDbAttachment.Select(n => new Video()
                    {
                        video = n.Source_Url
                    }).ToList();
                }


                SchoolComplaintModel complaintModel = new SchoolComplaintModel()
                {
                    complaintID = dbComplaint.Id,
                    campaignID = Convert.ToInt32(dbComplaint.Compaign_Id),
                    categoryID = Convert.ToInt32(dbComplaint.Complaint_Category),

                    cnic = dbComplaint.Person_Cnic,
                    comment = dbComplaint.Complaint_Remarks,
                    date = Convert.ToDateTime(dbComplaint.Created_Date).ToString("MM/dd/yyyy hh:mm tt"),
                    departmentId = Convert.ToInt32(dbComplaint.Department_Id),
                    districtID = Convert.ToInt32(dbComplaint.District_Id),

                    ListPicturesComplaintsUrl = listComplaintPicturesUrl,
                    ListPicturesStatusUrl = listStatusPicturesUrl,
                    ListVideoComplaintsUrl = listComplaintVideoUrl,
                    personContactNumber = dbComplaint.Person_Contact,
                    personName = dbComplaint.Person_Name,
                    provinceID = Convert.ToInt32(dbComplaint.Province_Id),

                    subCategoryID = Convert.ToInt32(dbComplaint.Complaint_SubCategory),

                    tehsilID = Convert.ToInt32(dbComplaint.Tehsil_Id),

                    ucID = Convert.ToInt32(dbComplaint.UnionCouncil_Id),
                    wardID = Convert.ToInt32(dbComplaint.Ward_Id),
                    statusId = Convert.ToInt32(dbComplaint.Complaint_Computed_Status_Id),
                    statusStr = dbComplaint.Complaint_Computed_Status,
                    statusChangeRemarks = dbComplaint.StatusChangedComments,
                    statusChangeDateTime = dbComplaint.StatusChangedDate_Time.ToString()
                };
                string emisCode = listDbSchools.FirstOrDefault(n => n.Id == dbComplaint.TableRowRefId).school_emis_code;
                SchoolModelWrapper schoolModelWrapper = getComlaintsModel.ListComplaint.FirstOrDefault(m => m.emisCode == emisCode);
                if (schoolModelWrapper == null) // if null create new
                {
                    schoolModelWrapper = new SchoolModelWrapper();
                    schoolModelWrapper.emisCode = emisCode;
                    getComlaintsModel.ListComplaint.Add(schoolModelWrapper);
                }
                else
                {
                    schoolModelWrapper.emisCode = emisCode;
                }

                //SchoolModelWrapper schoolModelWrapper = new SchoolModelWrapper();
                //schoolModelWrapper.emisCode =
                //    listDbSchools.FirstOrDefault(n => n.Id == dbComplaint.TableRowRefId).school_emis_code;
                schoolModelWrapper.listSchoolComplaintModel.Add(complaintModel);
                schoolModelWrapper.totalComplaints = schoolModelWrapper.listSchoolComplaintModel.Count;

                //complaintModel.categoryName = DbComplaintType.GetById(complaintModel.categoryID);



                complaintModel.categoryName = dbComplaint.Complaint_Category_Name;//(complaintModel.categoryID != 0) ? DbComplaintType.GetById(complaintModel.categoryID).Name : "";
                complaintModel.subCategoryName = dbComplaint.Complaint_SubCategory_Name;//(complaintModel.subCategoryID != 0) ? DbComplaintSubType.GetById(complaintModel.subCategoryID).Name : "";

                //Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("categoryName", translationDict, language);
                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("subCategoryName", translationDict, language);

                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("statusStr", translationDict, language);


                complaintModel.provinceName = dbComplaint.Province_Name;//(complaintModel.provinceID != 0) ? DbProvince.GetById(complaintModel.provinceID).Province_Name : "";
                complaintModel.districtName = dbComplaint.District_Name;//(complaintModel.districtID != 0) ? DbDistrict.GetById(complaintModel.districtID).District_Name : "";
                complaintModel.tehsilName = dbComplaint.Tehsil_Name; //(complaintModel.tehsilID != 0) ? DbTehsil.GetById(complaintModel.tehsilID).Tehsil_Name : "";
                complaintModel.ucName = dbComplaint.UnionCouncil_Name;

                //complaintModel.campaignName = DbCampaign.GetById(complaintModel.campaignID).Campaign_Name;
                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("campaignName", translationDict, language);


            }
            getComlaintsModel.Status = Config.ResponseType.Success.ToString();
            getComlaintsModel.Message = Config.ResponseType.Success.ToString();
            return getComlaintsModel;
        }
        #region PrivateComplaints
        public static dynamic SubmitPrivateComplaint(dynamic complaintReq, HttpRequest request)
        {
            SubmitSEComplaintModel submitComplaintModel = new SubmitSEComplaintModel();
            submitComplaintModel.schoolId = complaintReq.schoolId;
            submitComplaintModel.departmentId = complaintReq.departmentId;
            submitComplaintModel.categoryID = complaintReq.categoryId;
            submitComplaintModel.subCategoryID = complaintReq.subCategoryId;
            submitComplaintModel.schoolEmisCode = complaintReq.schoolEmisCode;
            submitComplaintModel.complaintDistrictId = complaintReq.complaintDistrictId;
            submitComplaintModel.complaintTehsilId = complaintReq.complaintTehsilId;

            submitComplaintModel.comment = complaintReq.comment;
            submitComplaintModel.remarks = complaintReq.remarks;
            submitComplaintModel.cnic = complaintReq.cnic;
            submitComplaintModel.personDistrictId = complaintReq.personDistrictId;

            submitComplaintModel.personName = complaintReq.personName;
            submitComplaintModel.personContactNumber = complaintReq.personContactNumber;
            submitComplaintModel.personFatherName = complaintReq.personFatherName;
            submitComplaintModel.personAddress = complaintReq.personAddress;

            submitComplaintModel.complaintSrc = (int)Config.ComplaintSource.OtherSystem;
            //complaintReq.personEmailAddress = submitComplaintModel.personEmailAddress;
            //Newtonsoft.Json.Linq.JArray listD = complaintReq.listAttachmentUrl;
            submitComplaintModel.listAttachmentUrl = complaintReq.listAttachmentUrl.ToObject<List<string>>();
            string ipAddress = Utility.GetClientIpAddress(request);
            Int64 apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(complaintReq.actualJson, ipAddress, true, null));
            submitComplaintModel.tagId = "PrivateSchools";
            ComplaintSubmitResponseSE response = SubmitComplaint(submitComplaintModel, apiRequestId, 1);
            return response;
        }
        

        public static dynamic GetPrivateComplaints(dynamic reqBody, HttpRequest request)
        {
            dynamic resp = new ExpandoObject();
            try
            {
                DateTime fromDate = reqBody.fromDate;
                DateTime toDate = reqBody.toDate;
                List<DbComplaint> listDbComplaints = DbComplaint.GetByDateRange((int)Config.Campaign.SchoolEducationEnhanced, 2, fromDate, toDate);
                var listComplaintsToRet = listDbComplaints.Select(n => new
                {
                    complaintId = n.Id,
                    complaintCat1Id = n.Department_Id,
                    complaintCat2Id = n.Complaint_Category,
                    complaintCat3Id = n.Complaint_SubCategory,
                    complaintAddComments = n.Complaint_Remarks,
                    schoolId = n.RefField1_Int,
                    personName = n.Person_Name,
                    personCnic = n.Person_Cnic,
                    personContact = n.Person_Contact,
                    personDistrictId = n.Person_District_Id,
                    complaintDistrictId = n.District_Id,
                    complaintTehsilId = n.Tehsil_Id,

                    statusId = n.Complaint_Computed_Status_Id,
                    statusStr = n.Complaint_Computed_Status,
                    statusComments = n.StatusChangedComments,
                    statusDateTime = n.StatusChangedDate_Time,

                    complaintCreatedDateTime = n.Created_Date,
                    complaintAddedRemarks = n.Complaint_Remarks

                }).ToList();
                //listDbComplaints.Select(n => n)
                resp.listComplaints = listComplaintsToRet;
                resp = Utility.GetApiResponse(true, null, null, resp);
            }
            catch (Exception ex)
            {
                resp = Utility.GetApiResponse(false, null, null, resp);
            }
            return resp;
        }
        #endregion


        #region AeoAppComplaints
        public static dynamic SubmitAeoAppComplaint(dynamic complaintReq, HttpRequest request)
        {
            SubmitSEComplaintModel submitComplaintModel = new SubmitSEComplaintModel();
            submitComplaintModel.schoolEmisCode = complaintReq.schoolEmisCode;
            submitComplaintModel.departmentId = complaintReq.departmentId;
            submitComplaintModel.categoryID = complaintReq.categoryId;
            submitComplaintModel.subCategoryID = complaintReq.subCategoryId;
            //submitComplaintModel.schoolEmisCode = complaintReq.schoolEmisCode;
            submitComplaintModel.complaintDistrictId = complaintReq.complaintDistrictId;
            submitComplaintModel.complaintTehsilId = complaintReq.complaintTehsilId;

            submitComplaintModel.comment = complaintReq.comment;
            submitComplaintModel.remarks = complaintReq.remarks;
            submitComplaintModel.cnic = complaintReq.cnic;
            submitComplaintModel.personDistrictId = complaintReq.personDistrictId;

            submitComplaintModel.personName = complaintReq.personName;
            submitComplaintModel.personContactNumber = complaintReq.personContactNumber;
            submitComplaintModel.personFatherName = complaintReq.personFatherName;
            submitComplaintModel.personAddress = complaintReq.personAddress;

            submitComplaintModel.complaintSrc = (int)Config.ComplaintSource.OtherSystem;
            //complaintReq.personEmailAddress = submitComplaintModel.personEmailAddress;
            //Newtonsoft.Json.Linq.JArray listD = complaintReq.listAttachmentUrl;
            submitComplaintModel.listAttachmentUrl = complaintReq.listAttachmentUrl.ToObject<List<string>>();
            string ipAddress = Utility.GetClientIpAddress(request);
            Int64 apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(complaintReq.actualJson, ipAddress, true, null));
            submitComplaintModel.tagId = "AeoApp";
            ComplaintSubmitResponseSE response = SubmitComplaint(submitComplaintModel, apiRequestId, 1);
            return response;
        }

        public static dynamic GetAeoAppComplaints(dynamic reqBody, HttpRequest request)
        {
            dynamic resp = new ExpandoObject();
            try
            {
                DateTime fromDate = reqBody.fromDate;
                DateTime toDate = reqBody.toDate;
                List<DbComplaint> listDbComplaints = DbComplaint.GetByDateRange((int)Config.Campaign.SchoolEducationEnhanced, 2, fromDate, toDate);
                var listComplaintsToRet = listDbComplaints.Select(n => new
                {
                    complaintId = n.Id,
                    complaintCat1Id = n.Department_Id,
                    complaintCat2Id = n.Complaint_Category,
                    complaintCat3Id = n.Complaint_SubCategory,
                    complaintAddComments = n.Complaint_Remarks,
                    schoolId = n.RefField1_Int,
                    personName = n.Person_Name,
                    personCnic = n.Person_Cnic,
                    personContact = n.Person_Contact,
                    personDistrictId = n.Person_District_Id,
                    complaintDistrictId = n.District_Id,
                    complaintTehsilId = n.Tehsil_Id,

                    statusId = n.Complaint_Computed_Status_Id,
                    statusStr = n.Complaint_Computed_Status,
                    statusComments = n.StatusChangedComments,
                    statusDateTime = n.StatusChangedDate_Time,

                    complaintCreatedDateTime = n.Created_Date,
                    complaintAddedRemarks = n.Complaint_Remarks

                }).ToList();
                //listDbComplaints.Select(n => n)
                resp.listComplaints = listComplaintsToRet;
                resp = Utility.GetApiResponse(true, null, null, resp);
            }
            catch (Exception ex)
            {
                resp = Utility.GetApiResponse(false, null, null, resp);
            }
            return resp;
        }

        #endregion

        #region ETransferSIS
        public static dynamic SubmitETransferComplaint(dynamic complaintReq, HttpRequest request)
        {
            SubmitSEComplaintModel submitComplaintModel = new SubmitSEComplaintModel();
            submitComplaintModel.schoolEmisCode = complaintReq.schoolEmisCode;
            submitComplaintModel.departmentId = complaintReq.departmentId;
            submitComplaintModel.categoryID = complaintReq.categoryId;
            submitComplaintModel.subCategoryID = complaintReq.subCategoryId;
            submitComplaintModel.schoolEmisCode = complaintReq.schoolEmisCode;
            submitComplaintModel.complaintDistrictId = complaintReq.complaintDistrictId;
            submitComplaintModel.complaintTehsilId = complaintReq.complaintTehsilId;

            submitComplaintModel.comment = complaintReq.comment;
            submitComplaintModel.remarks = complaintReq.remarks;
            submitComplaintModel.cnic = complaintReq.cnic;
            submitComplaintModel.personDistrictId = complaintReq.personDistrictId;

            submitComplaintModel.personName = complaintReq.personName;
            submitComplaintModel.personContactNumber = complaintReq.personContactNumber;
            submitComplaintModel.personFatherName = complaintReq.personFatherName;
            submitComplaintModel.personAddress = complaintReq.personAddress;

            submitComplaintModel.complaintSrc = (int)Config.ComplaintSource.OtherSystem;
            //complaintReq.personEmailAddress = submitComplaintModel.personEmailAddress;
            //Newtonsoft.Json.Linq.JArray listD = complaintReq.listAttachmentUrl;
            submitComplaintModel.listAttachmentUrl = complaintReq.listAttachmentUrl.ToObject<List<string>>();

            string jsonStr;
            using (Stream receiveStream = request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    jsonStr = readStream.ReadToEnd();
                }
            }

            submitComplaintModel.tagId = "PrivateSchools";
            ComplaintSubmitResponseSE response = SubmitComplaint(submitComplaintModel, -1, 1);
            string ipAddress = Utility.GetClientIpAddress(request);
            Int64 apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(request.Url.ToString(), JsonConvert.SerializeObject(request.Headers), jsonStr, ipAddress, true, null, JsonConvert.SerializeObject(response)));
            return response;


            //string ipAddress = Utility.GetClientIpAddress(request);
            //Int64 apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(complaintReq.actualJson, ipAddress, true, null));
            //submitComplaintModel.tagId = "eTransferSIS";
            //ComplaintSubmitResponseSE response = SubmitComplaint(submitComplaintModel, apiRequestId, 1);
            //return response;
        }

        public static dynamic GetETransferComplaints(dynamic reqBody, HttpRequest request)
        {
            dynamic resp = new ExpandoObject();
            try
            {
                DateTime? fromDate = reqBody.fromDate;
                DateTime? toDate = reqBody.toDate;
                string cnic = reqBody.cnic;
                List<DbComplaint> listDbComplaints = null;
                if (fromDate != null && toDate != null && !string.IsNullOrEmpty(cnic)) // if from to date and cnic exist
                {
                    listDbComplaints = DbComplaint.GetByDateRangeAndNotOfRefField((int)Config.Campaign.SchoolEducationEnhanced, Config.ComplaintType.Complaint, 2, (DateTime)fromDate, (DateTime)toDate, cnic);
                }
                else if (fromDate != null && toDate != null && string.IsNullOrEmpty(cnic)) // if only date exist
                {
                    listDbComplaints = DbComplaint.GetByDateRangeAndNotOfRefField((int)Config.Campaign.SchoolEducationEnhanced, Config.ComplaintType.Complaint, 2, (DateTime)fromDate, (DateTime)toDate);
                }
                else if (fromDate == null && toDate == null && !string.IsNullOrEmpty(cnic)) // if only cnic exist
                {
                    listDbComplaints = DbComplaint.GetByCnicAndNotOfRefField((int)Config.Campaign.SchoolEducationEnhanced, Config.ComplaintType.Complaint, 2, cnic);
                }

                List<DbCrmIdsMappingToOtherSystem> listDistrictMap = DbCrmIdsMappingToOtherSystem.Get(1, (int)Config.Hierarchy.District,
                   1, 1);

                List<DbCrmIdsMappingToOtherSystem> listTehsilMap = DbCrmIdsMappingToOtherSystem.Get(1, (int)Config.Hierarchy.Tehsil,
                   1, 1);

                //int x = 0;
                //foreach(var n in listDbComplaints)
                //{
                //    if(x==7)
                //    {

                //    }
                //    int? personDistrictId = n.Person_District_Id != null ? listDistrictMap.Where(s => s.Crm_Id == n.Person_District_Id).First().OTS_Id : null;
                //    int? complaintDistrictId = n.District_Id != null ? listDistrictMap.Where(s => s.Crm_Id == n.District_Id).First().OTS_Id : null; //n.District_Id,
                //    int? complaintTehsilId = n.Tehsil_Id != null ? listTehsilMap.Where(s => s.Crm_Id == n.Tehsil_Id).First().OTS_Id : null; //n.Tehsil_Id,
                //    x++;
                //}

                var listComplaintsToRet = listDbComplaints.Select(n => new
                {
                    complaintId = n.Id,
                    complaintCat1Id = n.Department_Id,
                    complaintCat2Id = n.Complaint_Category,
                    complaintCat3Id = n.Complaint_SubCategory,
                    complaintAddComments = n.Complaint_Remarks,
                    schoolEmisCode = n.RefField1,
                    personName = n.Person_Name,
                    personCnic = n.Person_Cnic,
                    personContact = n.Person_Contact,
                    personDistrictId = n.Person_District_Id != null ? listDistrictMap.Where(s => s.Crm_Id == n.Person_District_Id).First().OTS_Id : null,
                    complaintDistrictId = n.District_Id != null ? listDistrictMap.Where(s => s.Crm_Id == n.District_Id).First().OTS_Id : null, //n.District_Id,
                    complaintTehsilId = n.Tehsil_Id != null ? listTehsilMap.Where(s => s.Crm_Id == n.Tehsil_Id).First().OTS_Id : null, //n.Tehsil_Id,

                    statusId = n.Complaint_Computed_Status_Id,
                    statusStr = n.Complaint_Computed_Status,
                    statusComments = n.StatusChangedComments,
                    statusDateTime = n.StatusChangedDate_Time,

                    complaintCreatedDateTime = n.Created_Date,
                    complaintAddedRemarks = n.Complaint_Remarks

                }).ToList();
                //listDbComplaints.Select(n => n)

                resp.listComplaints = listComplaintsToRet;
                resp = Utility.GetApiResponse(true, null, null, resp);
            }
            catch (Exception ex)
            {
                resp = Utility.GetApiResponse(false, null, null, resp);
            }
            return resp;
        }
        #endregion

        public static dynamic GetSchoolCategories(dynamic reqBody, HttpRequest request)
        {
            dynamic resp = new ExpandoObject();

            try
            {
                string queryStr = @"SELECT Id, Name 
                                    FROM PITB.Department
                                    WHERE Campaign_Id = 47  AND Group_Id is NULL AND Is_Active = 1";
                DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    resp.listDepartment = dt.ToDynamicList();
                }

                queryStr = @"SELECT Id, Name,DepartmentId
                            FROM PITB.Complaints_Type
                            WHERE Campaign_Id = 47  AND Is_Active =1";
                dt = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    resp.listType = dt.ToDynamicList();

                }

                queryStr = @"SELECT st.id Id, st.Name Name, st.Complaint_Type_Id TypeId  
                            FROM PITB.Complaints_Type t INNER JOIN PITB.Complaints_SubType st 
                            ON t.id = st.Complaint_Type_Id
                            WHERE t.Campaign_Id = 47  AND t.Is_Active = 1 AND st.Is_Active = 1";
                dt = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    resp.listSubType = dt.ToDynamicList();

                }
            }
            catch (Exception ex)
            {
                resp = Utility.GetApiResponse(false, null, null, resp);
            }
            resp = Utility.GetApiResponse(true, null, null, resp);
            return resp;
        }


        public static ComplaintSubmitResponseSE SubmitComplaint(SubmitSEComplaintModel submitComplaintModel, Int64 apiRequestId, int appId)
        {
            try
            {
                bool isPmiuRegion = false;
                dynamic d = new ExpandoObject();
                d.sTag = new ExpandoObject();
                d.sTag.t1 = "PublicSchools"; d.sTag.t2 = "eTransferSIS"; d.sTag.t3 = "PrivateSchools"; d.sTag.t4 = "AeoApp";
                d.sbTag = new ExpandoObject();
                d.sbTag.t1 = new List<string> { d.sTag.t1, d.sTag.t2 }; // tags to get by emis code
                d.sbTag.t2 = new List<string> { d.sTag.t3 }; // tags for private complaints
                d.attTag = new List<string> { d.sTag.t2, d.sTag.t3 };

                List<DbCrmIdsMappingToOtherSystem> listDistrictMap = null;
                List<DbCrmIdsMappingToOtherSystem> listTehsilMap = null;
                if (submitComplaintModel.tagId == d.sTag.t2 || submitComplaintModel.tagId == d.sTag.t4) //if 
                {
                    isPmiuRegion = true;
                    listDistrictMap = DbCrmIdsMappingToOtherSystem.Get(1, (int)Config.Hierarchy.District, 1, 1);
                    listTehsilMap = DbCrmIdsMappingToOtherSystem.Get(1, (int)Config.Hierarchy.Tehsil, 1, 1);
                }

                int asd = 0;
                int campaignId = (int)Config.Campaign.SchoolEducationEnhanced;
                int? provinceId = 0, divisionId = 0, districtId = 0, tehsilId = 0, ucId = 0, wardId = 0;
                Config.AppID app = (Config.AppID)appId;
                DateTime nowDate = DateTime.Now;

                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

                DbPersonInformation dbPersonInfo = DbPersonInformation.GetByCnic(submitComplaintModel.cnic);

                int? personId = 0;
                if (dbPersonInfo != null) personId = dbPersonInfo.Person_id;

                // complaint Info
                paramDict.Add("@Id", -1);
                paramDict.Add("@Person_Id", personId.ToDbObj());



                // Complaint Info
                paramDict.Add("@DepartmentId", submitComplaintModel.departmentId.ToDbObj());
                paramDict.Add("@Complaint_Type", Config.ComplaintType.Complaint);
                paramDict.Add("@Complaint_Category", submitComplaintModel.categoryID.ToDbObj());
                paramDict.Add("@Complaint_SubCategory", submitComplaintModel.subCategoryID.ToDbObj());
                paramDict.Add("@Compaign_Id", (campaignId).ToDbObj());
                /*if (app == Config.AppID.Awazekhalq)  // Hardcoding for Bahawalpur
                {
                    provinceId = 1;
                    divisionId = 6;
                    districtId = 8;

                    paramDict.Add("@Province_Id", 1); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", 6);
                    //DbDistrict.GetById((int) submitComplaintModel.districtID).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", 8); //submitComplaintModel.districtID.ToDbObj());
                }
                else if(app == Config.AppID.SmartLahore)
                {

                    provinceId = submitComplaintModel.provinceID;
                    divisionId = Convert.ToInt32(DbDistrict.GetById((int)submitComplaintModel.districtID).Division_Id);
                    districtId = submitComplaintModel.districtID;

                    paramDict.Add("@Province_Id", submitComplaintModel.provinceID.ToDbObj()); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)submitComplaintModel.districtID).Division_Id.ToDbObj());

                    paramDict.Add("@District_Id", submitComplaintModel.districtID.ToDbObj()); //submitComplaintModel.districtID.ToDbObj());
                }*/
                //tehsilId = submitComplaintModel.tehsilID;
                //ucId = submitComplaintModel.ucID;
                //wardId = submitComplaintModel.wardID;



                DbSchoolsMapping dbSchools = null;

                if (d.sbTag.t1.Contains(submitComplaintModel.tagId))
                {
                    dbSchools = DbSchoolsMapping.GetByEmisCode(submitComplaintModel.schoolEmisCode);
                }
                else if (d.sbTag.t2.Contains(submitComplaintModel.tagId))
                {
                    dbSchools = DbSchoolsMapping.GetByOtherSystemSchoolId(submitComplaintModel.complaintDistrictId, submitComplaintModel.complaintTehsilId, submitComplaintModel.schoolId, 2);
                }
                else if (d.sTag.t4 == submitComplaintModel.tagId)
                {
                    dbSchools = DbSchoolsMapping.GetByOtherSystemSchoolId(submitComplaintModel.complaintDistrictId, submitComplaintModel.complaintTehsilId, -1, 3);
                }

                Dictionary<int?, string> dictSchoolCategory = new Dictionary<int?, string>()
                {
                    { -1, "public"},
                    { 1, "public"},
                    { 2, "private"},
                    { 3, "aeoApp"}
                };
                divisionId = DbDistrict.GetById(Convert.ToInt32(dbSchools.System_District_Id)).Division_Id;
                provinceId = DbDivision.GetById(Convert.ToInt32(divisionId)).Province_Id;

                if (isPmiuRegion)
                {
                    districtId = listDistrictMap.Where(s => s.OTS_Id == submitComplaintModel.complaintDistrictId).First().Crm_Id;
                    tehsilId = listTehsilMap.Where(s => s.OTS_Id == submitComplaintModel.complaintTehsilId).First().Crm_Id;
                }
                else if (d.sbTag.t1.Contains(submitComplaintModel.tagId))
                {
                    districtId = dbSchools.System_District_Id;
                    tehsilId = dbSchools.System_Tehsil_Id;
                }
                else if (d.sbTag.t2.Contains(submitComplaintModel.tagId))
                {
                    districtId = submitComplaintModel.complaintDistrictId;
                    tehsilId = submitComplaintModel.complaintTehsilId;
                }



                ucId = dbSchools.System_Markaz_Id;

                paramDict.Add("@Province_Id", provinceId.ToDbObj()); //submitComplaintModel.provinceID.ToDbObj());
                paramDict.Add("@Division_Id", divisionId.ToDbObj());
                paramDict.Add("@District_Id", districtId.ToDbObj()); //submitComplaintModel.districtID.ToDbObj());
                paramDict.Add("@Tehsil_Id", tehsilId.ToDbObj());
                paramDict.Add("@UnionCouncil_Id", ucId.ToDbObj() ?? 0);
                paramDict.Add("@Ward_Id", (0).ToDbObj());
                paramDict.Add("@Complaint_Remarks", submitComplaintModel.comment.ToDbObj());
                paramDict.Add("@Agent_Comments", (null as object).ToDbObj());
                if (d.sTag.t1 == submitComplaintModel.tagId)
                {
                    paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.Public).ToDbObj());
                }
                else if (d.sTag.t2 == submitComplaintModel.tagId)
                {
                    paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.OtherSystem).ToDbObj());
                    paramDict.Add("@Ref_Complaint_Src", Config.OtherSystemId.eTransferSIS);
                }
                else if (d.sTag.t3 == submitComplaintModel.tagId)
                {
                    paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.OtherSystem).ToDbObj());
                    paramDict.Add("@Ref_Complaint_Src", Config.OtherSystemId.PrivateSchoolEducation);
                }
                else if (d.sTag.t4 == submitComplaintModel.tagId)
                {
                    paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.OtherSystem).ToDbObj());
                    paramDict.Add("@Ref_Complaint_Src", Config.OtherSystemId.AeoApp);
                }


                paramDict.Add("@Agent_Id", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Address", (null as object).ToDbObj());
                paramDict.Add("@Business_Address", (null as object).ToDbObj());

                paramDict.Add("@Complaint_Status_Id", Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
                paramDict.Add("@Created_Date", nowDate.ToDbObj());
                paramDict.Add("@Created_By", (-1).ToDbObj());
                paramDict.Add("@Complaint_Assigned_Date", (null as object).ToDbObj());
                paramDict.Add("@Completed_Date", (null as object).ToDbObj());
                //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
                paramDict.Add("@Updated_By", (null as object).ToDbObj());
                paramDict.Add("@Is_Deleted", false);
                paramDict.Add("@Date_Deleted", (null as object).ToDbObj());
                paramDict.Add("@Deleted_By", (null as object).ToDbObj());

                paramDict.Add("@IsComplaintEditing", false);




                // Personal Info
                /*
                paramDict.Add("@p_Person_id", personId.ToDbObj());
                paramDict.Add("@Person_Name", submitComplaintModel.personName.ToDbObj());
                paramDict.Add("@Person_Father_Name", (null as object).ToDbObj());
                paramDict.Add("@Cnic_No", submitComplaintModel.cnic.ToDbObj());
                paramDict.Add("@Gender", (1).ToDbObj());
                paramDict.Add("@Mobile_No", submitComplaintModel.personContactNumber.ToDbObj());
                paramDict.Add("@Secondary_Mobile_No", (null as object).ToDbObj());
                paramDict.Add("@LandLine_No", (null as object).ToDbObj());
                paramDict.Add("@Person_Address", (null as object).ToDbObj());
                paramDict.Add("@Email", (null as object).ToDbObj());
                paramDict.Add("@Nearest_Place", (null as object).ToDbObj());
                paramDict.Add("@p_Province_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Division_Id", (null as object).ToDbObj());
                paramDict.Add("@p_District_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Tehsil_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Town_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Uc_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Created_By", (null as object).ToDbObj());
                paramDict.Add("@p_Updated_By", (null as object).ToDbObj());
                */
                if (isPmiuRegion)
                {
                    int personDistId = (int)listDistrictMap.Where(s => s.OTS_Id == submitComplaintModel.personDistrictId).First().Crm_Id;
                    divisionId = DbDistrict.GetById(Convert.ToInt32(personDistId)).Division_Id;
                    provinceId = DbDivision.GetById(Convert.ToInt32(divisionId)).Province_Id;
                    districtId = personDistId;
                }
                else
                {
                    divisionId = DbDistrict.GetById(Convert.ToInt32(submitComplaintModel.personDistrictId)).Division_Id;
                    provinceId = DbDivision.GetById(Convert.ToInt32(divisionId)).Province_Id;
                    districtId = submitComplaintModel.personDistrictId;
                }



                paramDict.Add("@p_Person_id", personId.ToDbObj());
                paramDict.Add("@Person_Name", submitComplaintModel.personName.ToDbObj());
                paramDict.Add("@Person_Father_Name", submitComplaintModel.personName.ToDbObj());
                paramDict.Add("@Cnic_No", submitComplaintModel.cnic.ToDbObj());
                paramDict.Add("@Gender", (1).ToDbObj());
                paramDict.Add("@Mobile_No", submitComplaintModel.personContactNumber.ToDbObj());
                paramDict.Add("@Secondary_Mobile_No", (null as object).ToDbObj());
                paramDict.Add("@LandLine_No", (null as object).ToDbObj());
                paramDict.Add("@Person_Address", (submitComplaintModel.personAddress).ToDbObj());
                paramDict.Add("@Email", (submitComplaintModel.personEmailAddress).ToDbObj());
                paramDict.Add("@Nearest_Place", (null as object).ToDbObj());
                paramDict.Add("@p_Province_Id", (provinceId).ToDbObj());
                paramDict.Add("@p_Division_Id", (divisionId).ToDbObj());
                paramDict.Add("@p_District_Id", (districtId).ToDbObj());
                paramDict.Add("@p_Tehsil_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Town_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Uc_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Created_By", (null as object).ToDbObj());
                paramDict.Add("@p_Updated_By", (null as object).ToDbObj());

                //paramDict.Add("@IsProfileEditing", false);

                paramDict.Add("@IsProfileEditing", personId != 0);



                // -------- Start adding code for school education


                float catRetainingHours = 0;
                float? subcatRetainingHours = 0;


                int? userCategoryId1 = null;
                int? userCategoryId2 = null;
                int? userCategoryId3 = null;

                List<Models.Custom.AssignmentModel> assignmentModelList = null;
                //if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint)
                //{
                string assignmentModelTag = "SchoolCategory::" + dbSchools.School_Category;
                assignmentModelList =
                        AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory(campaignId,
                            (int)submitComplaintModel.categoryID, (int)submitComplaintModel.subCategoryID, true, dbSchools.School_Type, null, assignmentModelTag);

                //------ Custom Code -------

                List<DbUsers> listDbUsers = UsersHandler.GetUsersHierarchyMapping(campaignId);
                Config.Hierarchy hierarchyId = (Config.Hierarchy)assignmentModelList[0].SrcId;
                int? userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);

                VmComplaint vmComplaint = new VmComplaint();
                vmComplaint.Province_Id = provinceId;
                vmComplaint.Division_Id = divisionId;
                vmComplaint.District_Id = districtId;
                vmComplaint.Tehsil_Id = tehsilId;
                vmComplaint.UnionCouncil_Id = ucId;



                OriginHierarchy originHierarchy = BlSchool.EvaluateAssignmentMartix(vmComplaint, listDbUsers, assignmentModelList, dbSchools, hierarchyId, userHierarchyVal, ref userCategoryId1, ref userCategoryId2, ref userCategoryId3, 0, null);
                paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
                paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
                paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
                paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
                paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);
                //--------------------------
                //}
                /*else if (dbSchools != null)
                {
                    assignmentModelList = new List<AssignmentModel>();
                    userCategoryId1 = dbSchools.School_Type;
                    if (userCategoryId1 == (int)Config.SchoolType.Primary || userCategoryId1 == (int)Config.SchoolType.Elementary)
                    {
                        userCategoryId2 = (bool)dbSchools.System_School_Gender
                            ? (int)Config.Gender.Male
                            : (int)Config.Gender.Female;
                    }
                    else
                    {
                        userCategoryId2 = null;

                    }
                }
                else
                {
                    assignmentModelList = new List<AssignmentModel>();
                }*/




                //-------- Assign dt ----------

                for (int i = 0; i < 10; i++)
                {
                    if (dbSchools != null && i < assignmentModelList.Count)
                    {
                        paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
                        paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                        paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
                    }
                    else
                    {
                        paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                        paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                        paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                    }
                }
                //-----------------------------



                // ------ adding supporting params for escalation params -----------------

                paramDict.Add("@MaxLevel", assignmentModelList.Count);

                paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList));
                paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList));

                paramDict.Add("@MinUserSrcId", AssignmentHandler.GetMinUserSrcId(assignmentModelList));
                paramDict.Add("@MaxUserSrcId", AssignmentHandler.GetMaxUserSrcId(assignmentModelList));


                paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList));
                paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList));



                // ----------- end adding custom params --------

                paramDict.Add("@TableRefId", (int)Config.TableRef.SchoolEducation);
                paramDict.Add("@TableRowRefId", dbSchools.Id);

                paramDict.Add("@UserCategoryId1", userCategoryId1);
                paramDict.Add("@UserCategoryId2", userCategoryId2);
                if (dbSchools != null)
                {
                    paramDict.Add("@RefField1", dbSchools.school_emis_code);
                    paramDict.Add("@RefField2", dbSchools.school_name);
                    paramDict.Add("@RefField3", dbSchools.school_level);
                    paramDict.Add("@RefField4", ((Config.SchoolType)dbSchools.School_Type).ToString());
                    paramDict.Add("@RefField5", dbSchools.school_gender);
                    paramDict.Add("@RefField6", dbSchools.markaz_name);
                    //paramDict.Add("@RefField7", dbSchools.School_Category == 2 ? "private" : "public");
                    paramDict.Add("@RefField7", dictSchoolCategory[dbSchools.School_Category==null?-1: dbSchools.School_Category]);
                    paramDict.Add("@RefField7_Int", dbSchools.School_Category);
                    paramDict.Add("@RefField1_Int", dbSchools.PMIU_School_Id);
                    //paramDict.Add("@RefField1_Int", dbSchools.School_Category == 2 ? dbSchools.PMIU_School_Id : null);
                }



                // creating dynamic form
                List<VmDynamicField> listVmDynamic = DynamicFieldsHandler.GetDynamicFieldsAgainstCampaignId(campaignId).OrderBy(n => n.Priority).ToList();
                if (listVmDynamic != null && listVmDynamic.Count > 0)
                {
                    vmComplaint.DynamicFieldsCount = listVmDynamic.Count;
                    vmComplaint.MinDynamicFormPriority = listVmDynamic.First().Priority;
                    vmComplaint.MaxDynamicFormPriority = listVmDynamic.Last().Priority;


                    List<VmDynamicTextbox> listDynamicTextBox = new List<VmDynamicTextbox>();
                    List<VmDynamicLabel> listDynamicLabel = new List<VmDynamicLabel>();
                    List<VmDynamicDropDownList> listDynamicDropdown = new List<VmDynamicDropDownList>();
                    List<VmDynamicDropDownListServerSide> listDynamicDropdownServerSide = new List<VmDynamicDropDownListServerSide>();
                    foreach (VmDynamicField dfField in listVmDynamic)
                    {
                        switch (dfField.ControlType)
                        {
                            case Config.DynamicControlType.TextBox:
                                listDynamicTextBox.Add(dfField as VmDynamicTextbox);
                                break;

                            case Config.DynamicControlType.Label:
                                listDynamicLabel.Add(dfField as VmDynamicLabel);
                                break;

                            case Config.DynamicControlType.DropDownList:
                                listDynamicDropdown.Add(dfField as VmDynamicDropDownList);
                                break;

                            case Config.DynamicControlType.DropDownListServerSideSearchable:
                                listDynamicDropdownServerSide.Add(dfField as VmDynamicDropDownListServerSide);
                                break;
                        }
                    }
                    vmComplaint.ListDynamicTextBox = listDynamicTextBox;
                    vmComplaint.ListDynamicLabel = listDynamicLabel;
                    vmComplaint.ListDynamicDropDown = listDynamicDropdown;
                    vmComplaint.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;

                    for (int i = 0; i < vmComplaint.ListDynamicLabel.Count; i++)
                    {
                        if (vmComplaint.ListDynamicLabel[i].ControlId == 12)// Tehsil
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.tehsil_name;
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 13)// Markaz
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.markaz_name;
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 14)// Emis Code
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.school_emis_code;
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 15)// School Gender
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.school_gender;
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 16)// School Level
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.school_level;
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 17)// School Type
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = ((Config.SchoolType)dbSchools.School_Type).ToString();
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 105)// Markaz
                        {
                            //vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.School_Category == 2 ? "private" : "public";
                            vmComplaint.ListDynamicLabel[i].FieldValue = dictSchoolCategory[dbSchools.School_Category == null ? -1 : dbSchools.School_Category];
                        }
                        else if (vmComplaint.ListDynamicLabel[i].ControlId == 106)// SchoolId
                        {
                            vmComplaint.ListDynamicLabel[i].FieldValue = dbSchools.PMIU_School_Id.ToString();
                        }
                    }

                    for (int i = 0; i < vmComplaint.ListDynamicDropDownServerSide.Count; i++)
                    {
                        vmComplaint.ListDynamicDropDownServerSide[i].SelectedItemId = dbSchools.Id + Config.Separator + "[" + dbSchools.school_emis_code + "] " + dbSchools.school_name;
                        /*{
                            Value = vmComplaint.ListDynamicDropDownServerSide[i].FieldName,
                            Text = dbSchools.Id+Config.Separator+"[" + dbSchools.school_emis_code + "] " + dbSchools.school_name,
                            Selected = true
                        });*/
                    }

                    for (int i = 0; i < vmComplaint.ListDynamicDropDown.Count; i++)
                    {
                        /*vmComplaint.ListDynamicDropDown[i].FieldValue.Add(new SelectListItem()
                        {
                            Value = vmComplaint.ListDynamicDropDown[i].FieldName,
                            Text = null
                        });*/
                    }

                }

                //end creating dynamic form











                //---------- End adding code for school education





                DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
                //Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
                int complaintId = Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]);
                string complaintIdStr = dt.Rows[0][1].ToString();

                //SaveMobileRequest(submitComplaintModel, Convert.ToInt32(complaintIdStr.Split('-')[1]), apiRequestId);
                /* if (submitComplaintModel.PicturesList != null)
                 {
                     foreach (Picture picture in submitComplaintModel.PicturesList)
                     {
                         BlApiHandler.StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", complaintIdStr, Config.FileType.File, apiRequestId);
                     }
                 }
                 if (!string.IsNullOrEmpty(submitComplaintModel.video))
                 {
                     BlApiHandler.StartUploadUtility(submitComplaintModel.video, "Video", "application/octet-stream",
                         submitComplaintModel.videoFileExtension, complaintIdStr, Config.FileType.Video, apiRequestId);
                 }*/
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString());
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(),  dt.Rows[0][1].ToString());
                //return "Your Complaint Id = " + dt.Rows[0][1].ToString();
                // Send message


                DynamicFieldsHandler.SaveDyamicFieldsInDb(vmComplaint, complaintId);

                if (d.attTag.Contains(submitComplaintModel.tagId))
                {
                    if (submitComplaintModel.listAttachmentUrl != null)
                    {
                        foreach (string attachment in submitComplaintModel.listAttachmentUrl)
                        {
                            FileUploadHandler.StoreImageUrlInDb(attachment, "attachment", Config.AttachmentReferenceType.Add, ".jpg", "image/jpeg", complaintId, null, complaintId, Config.TAG_COMPLAINT_ADD, Config.AttachmentType.File);
                        }
                    }
                }



                new Thread(delegate ()
                {
                    TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    campaignId, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    submitComplaintModel.categoryID, submitComplaintModel.personName, listVmDynamic);
                }).Start();

                new Thread(delegate ()
                {
                    BlSchool.SendMessageToStakeholder(DbComplaint.GetByComplaintId(complaintId), dbSchools);
                    //TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(complaintId, submitComplaintModel.personContactNumber);
                }).Start();

                // Commenting send message done
                /*TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    submitComplaintModel.categoryID);

                TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(complaintId);
                */
                return new ComplaintSubmitResponseSE(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString(), dt.Rows[0][1].ToString().Split('-')[1]);
            }
            catch (Exception ex)
            {
                //return Utility.GetStatus(Config.ResponseType.Failure.ToString(), ex.Message);
                //return Config.ResponseType.Failure.ToString()+" Exception = "+ex.Message;
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(JsonConvert.SerializeObject(submitComplaintModel), "", false, ex.Message));

                return new ComplaintSubmitResponseSE(Config.ResponseType.Failure.ToString(), "Server Error", "");
            }
        }



        public static GetComplaintStatuses GetMeaStatuses()
        {
            GetComplaintStatuses getComplaintStatuses = new GetComplaintStatuses();
            getComplaintStatuses.listDbStatuses = DbStatus.GetByStatusIds(Config.ListMeaStatuses);
            getComplaintStatuses.Status = Config.ResponseType.Success.ToString();
            getComplaintStatuses.Message = Config.ResponseType.Success.ToString();
            return getComplaintStatuses;
        }


        public static List<SyncResponse.UserData> GetUsersData(int campaignId, string userType, DateTime from, DateTime to)
        {
            List<int?> listUserCategoryId1 = new List<int?> { (int)Config.SchoolType.Primary, (int)Config.SchoolType.Elementary };

            List<DbCrmIdsMappingToOtherSystem> listCrmUsersIdsMap = DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Users, (int)Config.Hierarchy.UnionCouncil, 0, listUserCategoryId1, (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Users);

            List<DbCrmIdsMappingToOtherSystem> listCrmHierarchyIdsMap = DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy);

            Config.Hierarchy? configHierarchy = null;


            if (userType == "Aeo")
            {
                configHierarchy = Config.Hierarchy.UnionCouncil;
                //listCrmHierarchyIdsMap = listCrmHierarchyIdsMap.Where(n=>n.Crm_Module_Cat1=Config.Hierarchy.)
                //listUserCategoryId1 = 
            }
            List<DbUsers> listDbUsers = DbUsers.GetUser(campaignId, configHierarchy, listUserCategoryId1, from, to);

            List<Pair<DbUsers, DbCrmIdsMappingToOtherSystem>> listUserMapPair = new List<Pair<DbUsers, DbCrmIdsMappingToOtherSystem>>();

            listUserMapPair = (from map in listCrmUsersIdsMap
                               join user in listDbUsers on
                                   Convert.ToInt32(map.Crm_Id) equals user.User_Id

                               select new Pair<DbUsers, DbCrmIdsMappingToOtherSystem>
                               (
                                 user,
                                 map
                               )).ToList();

            listDbUsers = listUserMapPair.Select(n => n.Item1).ToList();
            /*
            List<SyncResponse.UserData> listUserData = listDbUsers.Select(n => new SyncResponse.UserData
            {
                UserId = n.User_Id,
                UserName = n.Username,
                Password = n.Password,

                ProvinceId =
                    Convert.ToInt32(
                        listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int) Config.Hierarchy.Province
                                                          && x.Crm_Id == Convert.ToInt32(n.Province_Id))
                            .FirstOrDefault()
                            .OTS_Id),

                DivisionId =
                    Convert.ToInt32(
                        listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int) Config.Hierarchy.Division 
                                                          && x.Crm_Id == Convert.ToInt32(n.Division_Id))
                            .FirstOrDefault()
                            .OTS_Id),

                DistrictId =
                    Convert.ToInt32(
                        listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int) Config.Hierarchy.District
                                                          && x.Crm_Id == Convert.ToInt32(n.District_Id))
                            .FirstOrDefault()
                            .OTS_Id),

                TehsilId =
                    Convert.ToInt32(listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int) Config.Hierarchy.Tehsil
                                                                      && x.Crm_Id == Convert.ToInt32(n.Tehsil_Id))
                        .FirstOrDefault()
                        .OTS_Id),

                UcId =
                    Convert.ToInt32(
                        listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int) Config.Hierarchy.UnionCouncil
                                                          && x.Crm_Id == Convert.ToInt32(n.Tehsil_Id))
                            .FirstOrDefault()
                            .OTS_Id),

                IsActive = n.IsActive

            }).ToList();
            */


            DbUsers dbUser = null;
            SyncResponse.UserData userData = null;
            List<SyncResponse.UserData> listUserData = new List<SyncResponse.UserData>();

            for (int i = 0; i < listDbUsers.Count; i++)
            {
                try
                {


                    dbUser = listDbUsers[i];
                    userData = new SyncResponse.UserData();

                    userData.UserId = dbUser.User_Id;
                    userData.UserName = dbUser.Username;
                    userData.Password = dbUser.Password;

                    userData.ProvinceId =
                        DbCrmIdsMappingToOtherSystem.GetOtsId(
                            listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int)Config.Hierarchy.Province
                                                              && x.Crm_Id == Convert.ToInt32(dbUser.Province_Id))
                                .FirstOrDefault());

                    userData.DivisionId =
                        DbCrmIdsMappingToOtherSystem.GetOtsId(
                            listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int)Config.Hierarchy.Division
                                                              && x.Crm_Id == Convert.ToInt32(dbUser.Division_Id))
                                .FirstOrDefault());

                    userData.DistrictId =
                        DbCrmIdsMappingToOtherSystem.GetOtsId(
                            listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int)Config.Hierarchy.District
                                                              && x.Crm_Id == Convert.ToInt32(dbUser.District_Id))
                                .FirstOrDefault());

                    //Debug.WriteLine("dbUserId = "+dbUser.User_Id + "  dbUser.Tehsil_Id" + dbUser.Tehsil_Id);
                    //if (dbUser.Tehsil_Id == "-1")
                    //{

                    //}
                    userData.TehsilId =
                        DbCrmIdsMappingToOtherSystem.GetOtsId(listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int)Config.Hierarchy.Tehsil
                                                                          && x.Crm_Id == Convert.ToInt32(dbUser.Tehsil_Id))
                            .FirstOrDefault());

                    userData.UcId =
                        DbCrmIdsMappingToOtherSystem.GetOtsId(
                            listCrmHierarchyIdsMap.Where(x => x.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil
                                                              && x.Crm_Id == Convert.ToInt32(dbUser.UnionCouncil_Id))
                                .FirstOrDefault());

                    userData.IsActive = dbUser.IsActive;
                    listUserData.Add(userData);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            return listUserData;
            //return listUserData;
        }

        public static SISApiModel.Response.SEGetComplaintAgainstEmisCodes GetComplaintsAgainstEmisCode(SISApiModel.Request.SEGetComplaintAgainstEmisCodes reqEmisCode)
        {
            SISApiModel.Response.SEGetComplaintAgainstEmisCodes response = new SISApiModel.Response.SEGetComplaintAgainstEmisCodes();
            List<DbComplaint> listDbComplaint = DbComplaint.GetBy((int)Config.Campaign.SchoolEducationEnhanced,
                    Config.ComplaintType.Complaint, reqEmisCode.ListSchoolModel.Select(n => n.EmisCode).ToList());

            response.ListComplaint =
                listDbComplaint.Select(
                    n => new SISApiModel.Response.ComplaintModel
                    {
                        Id = n.Id,
                        Category = n.Complaint_Category_Name,
                        SubCategory = n.Complaint_SubCategory_Name,
                        Detail = n.Complaint_Remarks
                    }).ToList();

            List<DbComplaint> tempListDbComplaint = null;
            SISApiModel.Response.SchoolStatusWiseCount statusWiseCount = null;
            //List<PITB.DbComplaint> tempListDbComplaint2 = null;
            var groupBy = listDbComplaint.GroupBy(n => new { n.RefField1 }).ToList();
            foreach (var groupVal in groupBy)
            {
                //response.ListSchoolStatusWiseCount = new List<SISApiModel.Response.SchoolStatusWiseCount>();
                statusWiseCount = new SISApiModel.Response.SchoolStatusWiseCount();
                tempListDbComplaint = listDbComplaint.Where(n => n.RefField1 == groupVal.Key.RefField1).ToList();
                statusWiseCount.SchoolEmisCode = groupVal.Key.RefField1;
                //for (int i = 0; i < tempListDbComplaint.Count; i++)
                //{
                statusWiseCount.PendingFresh = tempListDbComplaint.Where(
                    n => n.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.PendingFresh).Count();

                statusWiseCount.Overdue = tempListDbComplaint.Where(
                    n => n.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.UnsatisfactoryClosed).Count();

                statusWiseCount.Reopened = tempListDbComplaint.Where(
                    n => n.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.PendingReopened).Count();
                //}
                response.ListSchoolStatusWiseCount.Add(statusWiseCount);
            }
            response.SetSuccess();
            return response;
        }
    }
}