using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.CustomJsonConverter;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Tag;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Modules.Api.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace PITB.CMS_Common.ApiHandlers.Business.PublicUser
{
    public partial class BlApiPublicUser
    {
        public static dynamic Login(dynamic dParam/*, HttpRequest request*/)
        {
            CustomForm.Post customForm = dParam;
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(customForm.GetElementValue("data"));

            dynamic response = new ExpandoObject();
            var username = (string)data.username;
            var password = (string)data.password;
            DbUsers dbUser = BlPublicUser.GetUserAgainstUsernameAndPassword(username, (string)password);

            if (dbUser == null)
            {
                response.status = false;
                response.message = "Username or password is incorrect";
                return ApiResponseHandlerMobile.SetNoDataFound(ApiResponseHandlerMobile.SetData(response), "Username or password is incorrect");
            }

            response = new ExpandoObject();
            response.name = dbUser.Name;
            response.username = dbUser.Username;
            response.email = dbUser.Email;
            response.cnic = dbUser.Cnic;
            response.contactNo = dbUser.Phone;
            response.designation = dbUser.Designation;
            response.designationAbbr = dbUser.Designation_abbr;
            response.formPermission = new ExpandoObject();
            dynamic formPermission = response.formPermission;
            if (dbUser.Role_Id == Config.Roles.Stakeholder)
            {
                formPermission.Listing = true;
                formPermission.PublicUserDCChiniotComplaintAddForm = false;
            }
            else if (dbUser.Role_Id == Config.Roles.PublicUser)
            {
                formPermission.Listing = true;
                formPermission.PublicUserDCChiniotComplaintAddForm = true;
            }
            // prepare hashtoken for user
            response.hashToken = DbUsers.GetUserHashString(dbUser);
            response.status = true;
            response.message = "User found successfully";
            response = ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(response), "User found successfully");
            return response;
        }

        public static dynamic PostDynamicData(dynamic dParam)
        {
            dynamic response = new ExpandoObject();
            dynamic dParamToPass = new ExpandoObject();
            CustomForm.Post customForm = dParam;
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(customForm.GetElementValue("data"));
            dynamic appData = JsonConvert.DeserializeObject<ExpandoObject>(customForm.GetElementValue("appData"));



            //string formTag = appData.formTag;
            //List<object> listFields = data.listFields;
            dParamToPass.postedForm = customForm;
            dParamToPass.data = data;
            dParamToPass.appData = appData;
            dParamToPass.formTag = data.formTag;
            dParamToPass.fields = data.fields;




            if (dParamToPass.formTag != "PublicUserSignupForm")
            {
                if (!string.IsNullOrEmpty(data.userToken))
                {
                    int userId = DbUsers.GetUserIdFromHashString(data.userToken);
                    DbUsers dbUser = DbUsers.GetActiveUser(userId);
                    if (dbUser.IsAllowed != null && dbUser.IsAllowed == false)
                    {
                        response.data = null;
                        response.status = false;
                        response.message = "User Unauthorized";
                        return ApiResponseHandlerMobile.SetUnauthorizedError(response);
                    }
                }
            }




            switch (dParamToPass.formTag)
            {
                case "PublicUserComplaintListing":
                    return PostComplaintListing(dParamToPass);
                    //return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicUserComplaintListing(dParamToPass)), null);
                    break;

                case "PublicUserDCChiniotComplaintAddForm":
                    return PostAddForm(dParamToPass);
                    //return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicUserDCChiniotComplaintAddForm(dParamToPass)), null);
                    break;

                case "complaintStatusChangeForm":
                    return PostStatusChange(dParamToPass);
                    //return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicUserDCChiniotComplaintAddForm(dParamToPass)), null);
                    break;
                case "complaintCategoryChangeForm":
                    return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(ChangeComplaintTimeandCategory(dParamToPass)), null);
                    break;

                //case "PublicUserSignupOTP":
                //    return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicUserSignupOTP(dParamToPass)), null);
                //    //return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicUserComplaintListing(dParamToPass)), null);
                //    break;
                case "PublicUserSignupForm":
                    return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicSignupFormPost(dParamToPass)), null);
                    //return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PublicUserComplaintListing(dParamToPass)), null);
                    break;

                case "PostComplaint":
                    return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(PostComplaintListing(dParamToPass)), null);
                    break;
            }
            return response = ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(response), null);

        }

        public static dynamic PostStatusChange(dynamic dParam)
        {
            List<dynamic> listObj = dParam.fields;
            dynamic statusChangeModel = new ExpandoObject();
            //statusChangeModel.userId = DbUsers.GetUserIdFromHashString(listObj.Where(n => n.key == "userToken").FirstOrDefault().value);
            statusChangeModel.userId = DbUsers.GetUserIdFromHashString(dParam.data.userToken);
            string complaintIdStr = (string)listObj.Where(n => n.key == "complaintId").FirstOrDefault().value;
            string[] strArr = complaintIdStr.Split('-');
            statusChangeModel.complaintIdStr = complaintIdStr;
            statusChangeModel.complaintId = Convert.ToInt32(strArr[1]);
            statusChangeModel.statusId = Convert.ToInt32(listObj.Where(n => n.key == "statusChangeDropDown").FirstOrDefault().value);
            statusChangeModel.statusComments = listObj.Where(n => n.key == "statusChangeComments").FirstOrDefault().value;
            statusChangeModel.postedFiles = dParam.postedForm.postedFiles;
            //dParam.
            dParam.srcTag = "src::mobile__module::publicUser";
            dParam.statusChangeModel = statusChangeModel;
            //dynamic resp = BlDcChiniot.AddComplaint2(dComplaintAdd);
            dynamic resp = BlDcChiniot.PostStatusChange(dParam);
            resp.status = true;
            return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(resp), resp.message);
            //return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(resp), resp.message);
        }

        public static dynamic PostAddForm(dynamic dParam)
        {
            List<dynamic> listObj = dParam.fields;
            dynamic dAddComp = new ExpandoObject();
            dAddComp.personId = DbUsers.GetUserIdFromHashString(dParam.data.userToken);
            //dAddComp.personId = DbUsers.GetUserIdFromHashString(listObj.Where(n => n.key == "userToken").FirstOrDefault().value);

            DbUsers dbUser = DbUsers.GetActiveUser(dAddComp.personId);
            DbPersonInformation dbPersonInfo = DbPersonInformation.GetPersonInformationByPersonId((int)dbUser.PersonalInfo_Id);


            if (dbUser == null)
            {
                Exception ex = new Exception("Current user id not active");
                throw ex;
            }
            //dAddComp.srctag = "src::mobile__module::publicUser";
            dAddComp.personId = dbPersonInfo.Person_id;
            dAddComp.personName = dbPersonInfo.Person_Name;
            dAddComp.personGender = (int)dbPersonInfo.Gender;
            dAddComp.personCnic = dbPersonInfo.Cnic_No;
            dAddComp.personIsCnicPresent = Convert.ToBoolean(dbPersonInfo.Is_Cnic_Present);
            dAddComp.personIsProfileEditing = true;
            dAddComp.personMobileNo = dbPersonInfo.Mobile_No;
            dAddComp.personSecondaryNo = dbPersonInfo.Secondary_Mobile_No;
            dAddComp.personAddress = dbPersonInfo.Person_Address;
            dAddComp.personProvinceId = dbPersonInfo.Province_Id;
            dAddComp.personDivisionId = dbPersonInfo.Division_Id;
            dAddComp.personDistrictId = dbPersonInfo.District_Id;

            dAddComp.campaignId = (int)Config.Campaign.DcChiniot;
            dAddComp.complaintType = (int)Config.ComplaintType.Complaint;
            dAddComp.categoryId = Convert.ToInt32(listObj.Where(n => n.key == "complaintCategory").FirstOrDefault().value);
            dAddComp.subcategoryId = Convert.ToInt32(listObj.Where(n => n.key == "complaintSubCategory").FirstOrDefault().value);
            dAddComp.provinceId = Convert.ToInt32(listObj.Where(n => n.key == "complaintProvince").FirstOrDefault().value);
            //dAddComp.divisionId = DbUsers.GetUserIdFromHashString(listObj.Where(n => n.key == "complaintProvince").FirstOrDefault().value);
            dAddComp.districtId = Convert.ToInt32(listObj.Where(n => n.key == "complaintDistrict").FirstOrDefault().value);
            dAddComp.divisionId = ((DbDistrict)DbDistrict.GetById(dAddComp.districtId)).Division_Id;
            dAddComp.tehsilId = Convert.ToInt32(listObj.Where(n => n.key == "complaintTehsil").FirstOrDefault().value);
            dAddComp.agentComments = listObj.Where(n => n.key == "complaintComment").FirstOrDefault().value;
            dAddComp.complaintDetail = listObj.Where(n => n.key == "complaintDetail").FirstOrDefault().value;
            dAddComp.complaintCreatedBy = dbUser.User_Id;
            dAddComp.complaintSrc = (int)Config.ComplaintSource.Public;

            dynamic dComplaintAdd = new ExpandoObject();
            dComplaintAdd.postedForm = dParam.postedForm;
            dComplaintAdd.addComplaintModel = dAddComp;
            dComplaintAdd.srcTag = "src::mobile__module::publicUser";
            dynamic resp = BlDcChiniot.AddComplaint2(dComplaintAdd);
            resp.status = true;
            return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(resp), resp.message);
            //DbUsers dbUser = DbUsers.GetUser(userId);
        }

        public static dynamic PostComplaintListing(dynamic dParam)
        {
            int campaignId = 99;
            List<DbComplaintType> listDbComplaintTypes = DbComplaintType.GetByCampaignId(99);
            Config.StakeholderComplaintListingType stakeholderListingType = Config.StakeholderComplaintListingType.None;
            List<DbStatus> listDbStatuses = DbStatus.GetAll();
            //dynamic dStatusMask = null;
            dynamic data = new ExpandoObject();
            data.status = true;
            data.message = null;
            List<dynamic> listDForm = new List<dynamic>();
            List<dynamic> listField = dParam.fields;
            dynamic statusChangeModel = new ExpandoObject();
            //int userId = DbUsers.GetUserIdFromHashString((string)GetFieldValue(listField, "userToken"));
            int userId = DbUsers.GetUserIdFromHashString(dParam.data.userToken);
            DbUsers dbUser = DbUsers.GetActiveUser(userId);
            List<dynamic> listUserComplaints = null;

            //-----Populate campaign permission-----
            string campaignPermission = TagWiseBestMatchHandler.GetBestMatch("Config::CampaignPermission", "CampaignId::{dbUser.Campaign_Id}");
            dynamic dCampPerm = JsonConvert.DeserializeObject(campaignPermission, typeof(ExpandoObject), new DynamicJsonConverter());
            Dictionary<int, string> dictIconMap = BlStatus.GetStatusIconMappingDict(dCampPerm.statusIconMap);
            dynamic dStatusMask = null;
            //--------------------------------------


            if (dbUser.Role_Id == Config.Roles.PublicUser)
            {

                //// Create status mask tag for public user
                //string tagKey = string.Format("RoleId::{0}__CampaignId::{1}", (int)dbUser.Role_Id, dbUser.Campaign_Id);
                //string tagId = string.Format("Config::MaskedStatuses__RoleId::{0}", (int)dbUser.Role_Id);

                //string tagValue = TagWiseBestMatchHandler.GetBestMatch(tagId, tagKey);
                //if (tagValue != null)
                //{
                //    dStatusMask = BlStatus.GetStatusMaskingModel(tagValue, listDbStatuses);
                //    //return permDictToRet;
                //}
                //// end Create mask for tag
                dStatusMask = BlStatus.GetStatusMaskingModel(dCampPerm.roleId_9.statusMask, listDbStatuses);
                dParam.statusMask = dStatusMask;
                listUserComplaints = _GetPublicUserComplaints(dParam);
            }
            else if (dbUser.Role_Id == Config.Roles.Stakeholder)
            {
                string from = (string)GetFieldValue(listField, "listingStartDate");
                string to = (string)GetFieldValue(listField, "listingEndDate");
                int start = Convert.ToInt32(GetFieldValue(listField, "fromRow"));
                int end = Convert.ToInt32(GetFieldValue(listField, "toRow"));
                string complaintListingType = (string)GetFieldValue(listField, "listingType");
                string commaSepStatuses = (string)GetFieldValue(listField, "listingStatus");
                List<int> listStatuses = Utility.GetIntList(commaSepStatuses);
                string commaSeperatedStatuses = string.Join(",", listStatuses.Select(n => n.ToString()).ToArray());
                string commaSeperatedCategories = dbUser.Categories;
                string commaSeperatedCampaigns = dbUser.Campaigns;
                string commaSeperatedTransferedStatus = "1,0";
                stakeholderListingType = (Config.StakeholderComplaintListingType)Convert.ToInt32(complaintListingType); //Config.StakeholderComplaintListingType.AssignedToMe;
                string spType = "Listing";

                DataTableParamsModel dtParamModel = DataTableHandler.GetDatatableModel(start - 1, end - start + 1, "complaints.Id", "desc", null);

                ListingParamsModelBase paramsModel = BlComplaints.SetStakeholderListingParams(dbUser, from, to,
                    commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses,
                    commaSeperatedTransferedStatus, dtParamModel, (Config.ComplaintType.Complaint), stakeholderListingType, spType);
                string queryStr = StakeholderListingLogic.GetListingQuery(paramsModel);
                queryStr = queryStr.Remove(0, queryStr.IndexOf("RowNum"));
                queryStr = queryStr.Insert(0, @"SELECT * FROM( SELECT *,
                             COUNT(*) OVER() AS Total_Rows,
                             ROW_NUMBER() OVER ( ORDER BY
                             complaints.Id DESC ) AS ");
                listUserComplaints = DBHelper.GetDynamicListByQueryString(queryStr, null);

            }

            // if list has no element then return no record found
            if (listUserComplaints.Count == 0)
            {
                data.listing = listDForm;
                data.status = false;
                data.message = "no record found";
                return ApiResponseHandlerMobile.SetNoDataFound(ApiResponseHandlerMobile.SetData(data), "no record found");
            }


            // Prepare data 

            //#region get campaign Settings 
            ////string campaignPermission = TagWiseBestMatchHandler.GetBestMatch("Config::CampaignPermission", $"CampaignId::{dbUser.Campaign_Id}");
            ////dynamic dynamicCampaignPermission = JsonConvert.DeserializeObject(campaignPermission, typeof(ExpandoObject), new DynamicJsonConverter());
            ////Dictionary<int, string> dictIconMap = BlStatus.GetStatusIconMappingDict(dCampPerm.statusIconMap);
            //#endregion


            List<DbComplaintType> listCatogory = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, null);
            List<DbComplaintSubType> listSubcategory = DbComplaintSubType.GetByComplaintTypes(listCatogory.Select(n => n.Complaint_Category).ToList());

            //List<dynamic> listUserComplaints = _GetPublicUserComplaints(dParam);
            List<object> listComplaintIds = listUserComplaints.Select(n => Convert.ToInt32(n.Id)).ToList();
            List<dynamic> listAttachments = DBHelper.GetDynamicListByQueryString(
                string.Format(@"SELECT Source_Url,ReferenceType,ReferenceTypeId,FileName,FileExtension,FileContentType 
                                FROM PITB.Attachments
                                WHERE Complaint_Id IN({0})", string.Join(",", listComplaintIds.Select(n => n.ToString()).ToArray())), null);

            List<dynamic> listStatusLogs = DBHelper.GetDynamicListByQueryString(
                string.Format(@"SELECT * 
                                FROM PITB.Complaints_Status_Change_Log
                                WHERE Complaint_Id IN ({0})
                                ORDER BY Complaint_Id, StatusChangeDateTime", string.Join(",", listComplaintIds.Select(n => n.ToString()).ToArray())), null);


            List<DbPermissionsAssignment> listDbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, dbUser.User_Id);









            Utility.PrintDynamic(listUserComplaints);


            dynamic dTemp = null;

            //dynamic response = new ExpandoObject();
            dynamic dCell = new ExpandoObject();
            dynamic dField;
            int count = 0;
            foreach (dynamic d in listUserComplaints)
            {

                dCell = new ExpandoObject();
                dCell.cellForm = new ExpandoObject();
                dCell.complaintDetailForm = new ExpandoObject();
                dCell.complaintChangeCategoryForm = null;
                dCell.complaintStatusHistory = null;
                dCell.complaintStatusChangeForm = null;




                //---Populate cellForm
                count = 1;
                dTemp = dCell.cellForm;
                dTemp.formTag = "cellForm";
                dTemp.fields = new List<dynamic>();
                dField = dTemp.fields;
                SetLabel(dField, "complaintId", "Complaint Id", d.Compaign_Id + "-" + d.Id, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "campaignName", "Campaign", d.Campaign_Name, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "categoryName", "Category", d.Complaint_Category_Name, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                string statusName = (dbUser.Role_Id == Config.Roles.PublicUser) ? dStatusMask.dictStatusStrMap[d.Complaint_Computed_Status] : d.Complaint_Computed_Status;
                SetLabel(dField, "statusName", "Status", statusName, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                dField[dField.Count - 1].field_icon = (dbUser.Role_Id == Config.Roles.PublicUser) ? dictIconMap[dStatusMask.dictStatusIdsMap[Convert.ToInt32(d.Complaint_Computed_Status_Id)]] : dictIconMap[Convert.ToInt32(d.Complaint_Computed_Status_Id)];
                //SetLabel(dField, "statusIcon", "Status Icon", dictIconMap[Convert.ToInt32(d.Complaint_Computed_Status_Id)], Utility.GetEnumDisplayName(Config.DynamicControlType.URL), count++, dTemp.formTag);
                SetLabel(dField, "createdDateTime", "Date", ((DateTime)d.Created_Date).ToString(), Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "comment", "Comments", d.Agent_Comments, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "complaintRemarks", "Details", d.Complaint_Remarks, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);


                //---Populate complaintDetailForm
                //VmStakeholderComplaintDetail vmComplaintDetail = null;
                dynamic stakeholderDetailModel = null;
                if (dbUser.Role_Id == Config.Roles.Stakeholder)
                {
                    dynamic dComplaintDetailParam = new ExpandoObject();
                    dComplaintDetailParam.postedForm = dParam.postedForm;

                    dComplaintDetailParam.currentStatusId = d.Complaint_Computed_Status_Id;
                    dComplaintDetailParam.listDbPermissionAssignment = listDbPermissionAssignment;
                    dComplaintDetailParam.listDbStatuses = listDbStatuses;
                    dComplaintDetailParam.detailType = stakeholderListingType == Config.StakeholderComplaintListingType.AssignedToMe ? VmStakeholderComplaintDetail.DetailType.AssignedToMe : VmStakeholderComplaintDetail.DetailType.All;
                    dComplaintDetailParam.campaignId = dbUser.Campaign_Id;
                    dComplaintDetailParam.dbUser = dbUser;
                    dComplaintDetailParam.dComplaint = d;
                    dComplaintDetailParam.srcTag = "mobile";
                    stakeholderDetailModel = BlDcChiniot.GetComplaintDetail(dComplaintDetailParam);
                    stakeholderDetailModel.canChangeStatus = stakeholderListingType == Config.StakeholderComplaintListingType.AssignedToMe ? stakeholderDetailModel.canChangeStatus : false;

                }

                count = 1;
                dTemp = dCell.complaintDetailForm;
                dTemp.formTag = "complaintDetailForm";
                dTemp.fields = new List<dynamic>();
                dField = dTemp.fields;
                SetLabel(dField, "personName", "Name", d.Person_Name, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "personPhoneNo", "Phone No", d.Person_Contact, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "personCnic", "CNIC", d.Person_Cnic, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "complaintId", "Complaint Id", d.Compaign_Id + "-" + d.Id, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "campaignName", "Campaign", d.Campaign_Name, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "categoryName", "Category", d.Complaint_Category_Name, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "subCategoryName", "Subcategory", d.Complaint_SubCategory_Name, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "createdDateTime", "Created Date", ((DateTime)d.Created_Date).ToString(), Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "comment", "Comments", d.Agent_Comments, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "complaintRemarks", "Details", d.Complaint_Remarks, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);

                // adding attachment
                List<dynamic> listDAttachment = listAttachments.Where(n => n.ReferenceType == 1 && n.ReferenceTypeId == d.Id).ToList();
                //int attCount = 1;
                List<dynamic> listAttToSend = new List<dynamic>();
                int attCount = 1;
                foreach (dynamic dAtt in listDAttachment)
                {
                    SetAttachment(dField, dAtt.Source_Url, dAtt.FileName, dAtt.FileExtension, dAtt.FileContentType, "attachment_" + attCount,
                        "attachment_" + attCount, dAtt.Source_Url, Utility.GetEnumDisplayName(Config.DynamicControlType.FileAttachment), count++, dTemp.formTag);
                    attCount++;
                }
                SetLabel(dField, "statusName", "Status", statusName /*d.Complaint_Computed_Status*/, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                dField[dField.Count - 1].field_icon = (dbUser.Role_Id == Config.Roles.PublicUser) ? dictIconMap[dStatusMask.dictStatusIdsMap[Convert.ToInt32(d.Complaint_Computed_Status_Id)]] : dictIconMap[Convert.ToInt32(d.Complaint_Computed_Status_Id)];
                SetLabel(dField, "statusDateTime", "Status Date", d.StatusChangedDate_Time == null ? null : (d.StatusChangedDate_Time).ToString(), Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                SetLabel(dField, "statusComments", "Status Comments", d.StatusChangedComments, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);


                //---Populate category change 
                if (dbUser.Role_Id == Config.Roles.Stakeholder)
                {
                    count = 1;
                    if (stakeholderDetailModel.canChangeCategory)
                    {
                        dCell.complaintChangeCategoryForm = new ExpandoObject();
                        dTemp = dCell.complaintChangeCategoryForm;
                        dTemp.formTag = "complaintCategoryChangeForm";
                        dTemp.fields = new List<dynamic>();
                        dTemp.data = null;


                        SetHidden(dTemp.fields, "complaintId", d.Compaign_Id + "-" + d.Id, Utility.GetEnumDisplayName(Config.DynamicControlType.Hidden), dTemp.formTag);


                        IEnumerable<object> listObj = listCatogory.Select(n => new
                        {
                            value = n.Complaint_Category,
                            text = n.Name,
                            parentId = (object)null,
                            selected = n.Complaint_Category.ToString() == d.Complaint_Category.ToString() ? true : false
                        }).ToList();

                        //listStatusOptions = Utility.GetInitializedDynamicListGeneric<object>(listObj);
                        List<dynamic> listCategoryOptions = Utility.GetInitializedDynamicListGeneric<object>(listObj);
                        SetDropdownField(dTemp.fields, -1, "complaintCategory", "Complaint Category", d.Complaint_Category.ToString(),
                            Utility.GetEnumDisplayName(Config.DynamicControlType.DropDownList), 1, 1, 1, count++, 1, null,
                            JsonConvert.SerializeObject(listCategoryOptions),
                        "set category", true, true, true, dTemp.formTag);

                        listObj = listSubcategory.Select(n => new
                        {
                            value = n.Complaint_SubCategory,
                            text = n.Name,
                            parentId = n.Complaint_Type_Id,
                            selected = n.Complaint_SubCategory.ToString() == d.Complaint_SubCategory.ToString() ? true : false
                        }).ToList();
                        List<dynamic> listSubcategoryOptions = Utility.GetInitializedDynamicListGeneric<object>(listObj);
                        SetDropdownField(dTemp.fields, -1, "complaintSubCategory", "Complaint Subcategory", d.Complaint_SubCategory.ToString(), Utility.GetEnumDisplayName(Config.DynamicControlType.DropDownList), 1, 2, 2, count++, 1, null, JsonConvert.SerializeObject(listSubcategoryOptions),
                        "set Subcategory", true, true, true, dTemp.formTag);

                        if (d.Dt1 == null)
                        {
                            SetLabel(dTemp.fields, "previousTime", "Current escalation time",
                                listDbComplaintTypes.Where(w => w.Complaint_Category == Convert.ToInt32(d.Complaint_Category)).FirstOrDefault().RetainingHours.ToString(),
                                Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                        }
                        else
                        {
                            SetLabel(dTemp.fields, "previousTime", "Current escalation time", Math.Abs(((TimeSpan)(Convert.ToDateTime(d.Dt1) - Convert.ToDateTime(d.Created_Date))).Hours).ToString(), Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                        }
                        SetTextBoxField(dTemp.fields, "newTime", "New Time", Utility.GetEnumDisplayName(Config.DynamicControlType.TextBox), "number", count++, "None", null, "Set new time",
                        true, true, false, 5, 1, dTemp.formTag);
                    }
                }


                //---Populate complaint status history
                List<dynamic> listComplaintStatusLogs = listStatusLogs.Where(n => n.Complaint_Id == d.Id).ToList();
                if (listComplaintStatusLogs.Count > 0)
                {
                    count = 1;
                    dCell.complaintStatusHistory = new ExpandoObject();
                    dTemp = dCell.complaintStatusHistory;
                    dTemp.formTag = "complaintStatusHistory";
                    dTemp.fields = null; //new List<List<dynamic>>();
                    dTemp.data = new List<dynamic>();

                    //List<dynamic> listStatusHistory = new List<dynamic>();
                    foreach (dynamic dStatusLog in listComplaintStatusLogs)
                    {
                        dynamic dataObj = new ExpandoObject();
                        dataObj.fields = new List<dynamic>();
                        dField = dataObj.fields;
                        //dField = new List<dynamic>();
                        SetLabel(dField, "complaintId", "Complaint Id", d.Compaign_Id + "-" + d.Id, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                        //statusName = (dbUser.Role_Id == Config.Roles.PublicUser) ? dStatusMask.dictStatusStrMap[dStatusLog.StatusId] : listDbStatuses.Where(n => n.Complaint_Status_Id == dStatusLog.StatusId).FirstOrDefault().Status;
                        SetLabel(dField, "statusName", "Status name", listDbStatuses.Where(n => n.Complaint_Status_Id == dStatusLog.StatusId).FirstOrDefault().Status, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                        dField[dField.Count - 1].field_icon = (dbUser.Role_Id == Config.Roles.PublicUser) ? dictIconMap[dStatusMask.dictStatusIdsMap[Convert.ToInt32(dStatusLog.StatusId)]] : dictIconMap[Convert.ToInt32(dStatusLog.StatusId)];
                        //dField[dField.Count - 1].field_icon = dictIconMap[Convert.ToInt32(dStatusLog.StatusId)];
                        SetLabel(dField, "statusDateTime", "Status change date", dStatusLog.StatusChangeDateTime.ToString(), Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);
                        SetLabel(dField, "statusComments", "Status comments", dStatusLog.Comments, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);

                        //SetLabel(dTemp.fields, "statusComments", "Status change date", dStatusLog.Comments, Utility.GetEnumDisplayName(Config.DynamicControlType.Label), count++, dTemp.formTag);

                        //dynamic dStatusToSend = new ExpandoObject();
                        //dStatusToSend.statusName = listDbStatuses.Where(n => n.Complaint_Status_Id == dStatusLog.StatusId).FirstOrDefault().Status;
                        //dStatusToSend.statusDateTime = dStatusLog.StatusChangeDateTime;
                        //dStatusToSend.statusComments = dStatusLog.Comments;
                        //dStatusToSend.listAttachments = new List<dynamic>();
                        attCount = 1;
                        foreach (dynamic dAtt in listAttachments.Where(n => n.ReferenceTypeId == dStatusLog.Id).ToList())
                        {
                            SetAttachment(dField, dAtt.Source_Url, dAtt.FileName, dAtt.FileExtension, dAtt.FileContentType, "attachment_" + attCount,
                            "attachment_" + attCount, dAtt.Source_Url, Utility.GetEnumDisplayName(Config.DynamicControlType.FileAttachment), count++, dTemp.formTag);
                            attCount++;
                            //dAtt.
                            //dynamic dAttToSend = new ExpandoObject();
                            //dAttToSend.url = dAtt.Source_Url;
                            //dAttToSend.fileName = dAtt.FileName;
                            //dAttToSend.fileExtension = dAtt.FileExtension;
                            //dAttToSend.fileContentType = dAtt.FileContentType;
                            //dStatusToSend.listAttachments.Add(dAttToSend);

                            //attCount++;
                        }
                        dTemp.data.Add(dataObj);
                        //dTemp.fields.Add(dField);
                        //dTemp.data.Add(dStatusToSend);
                    }
                }


                //---Populate change Status Form
                count = 1;


                if (dbUser.Role_Id == Config.Roles.PublicUser)
                {
                    if (d.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.ResolvedVerified)
                    {
                        List<dynamic> listStatusOptions = null;
                        dCell.complaintStatusChangeForm = new ExpandoObject();
                        dTemp = dCell.complaintStatusChangeForm;
                        List<DbStatus> listDbStatusTemp = listDbStatuses.Where(n => n.Complaint_Status_Id == 34 || n.Complaint_Status_Id == 35).ToList();
                        IEnumerable<object> listObj = listDbStatusTemp.Select(n => new { value = n.Complaint_Status_Id, text = n.Status, parentId = (object)null }).ToList();
                        listStatusOptions = Utility.GetInitializedDynamicListGeneric<object>(listObj);

                        dTemp.formTag = "complaintStatusChangeForm";
                        dTemp.fields = new List<dynamic>();
                        dField = dTemp.fields;

                        SetHidden(dTemp.fields, "complaintId", d.Compaign_Id + "-" + d.Id, Utility.GetEnumDisplayName(Config.DynamicControlType.Hidden), dTemp.formTag);

                        SetDropdownField(dTemp.fields, -1, "statusChangeDropDown", "Feedback", null, Utility.GetEnumDisplayName(Config.DynamicControlType.DropDownList), 1, 1, 1, count++, 1, null, JsonConvert.SerializeObject(listStatusOptions),
                            "Mark your statisfaction", true, true, true, "complaintStatusChangeForm");

                        SetTextBoxField(dTemp.fields/*, -1*/, "statusChangeComments", "Status comments", Utility.GetEnumDisplayName(Config.DynamicControlType.TextBox), "text", count++, "None", null, "Add status comments",
                        true, true, true, 4000, 10, "complaintStatusChangeForm");
                        for (int i = 1; i <= 5; i++)
                        {
                            SetAttachment(dField, null, null, null, null, "attachment_" + i,
                                "attachment_" + i, null, Utility.GetEnumDisplayName(Config.DynamicControlType.FileAttachment), i, dTemp.formTag);

                        }
                    }
                }
                else if (dbUser.Role_Id == Config.Roles.Stakeholder)
                {
                    if (stakeholderDetailModel.canChangeStatus)
                    {
                        List<dynamic> listStatusOptions = null;
                        dCell.complaintStatusChangeForm = new ExpandoObject();
                        dTemp = dCell.complaintStatusChangeForm;
                        //List<DbStatus> listDbStatusTemp = listDbStatuses.Where(n => n.Complaint_Status_Id == 34 || n.Complaint_Status_Id == 35).ToList();
                        List<DbStatus> listDdStatuses = stakeholderDetailModel.listStatuses;
                        IEnumerable<object> listObj = listDdStatuses.Select(n => new { value = n.Complaint_Status_Id, text = n.Status, parentId = (object)null }).ToList();
                        listStatusOptions = Utility.GetInitializedDynamicListGeneric<object>(listObj);

                        //listStatusOptions = Utility.GetInitializedDynamicList(vmComplaintDetail.VmStatusChange.ListStatus.Select(n => new { value = Convert.ToInt32(n.Value), text = n.Text, parentId = (object)null }).ToList());

                        dTemp.formTag = "complaintStatusChangeForm";
                        dTemp.fields = new List<dynamic>();
                        dField = dTemp.fields;

                        SetHidden(dTemp.fields, "complaintId", d.Compaign_Id + "-" + d.Id, Utility.GetEnumDisplayName(Config.DynamicControlType.Hidden), dTemp.formTag);

                        SetDropdownField(dTemp.fields, -1, "statusChangeDropDown", "Status", null, Utility.GetEnumDisplayName(Config.DynamicControlType.DropDownList), 1, 1, 1, count++, 1, null, JsonConvert.SerializeObject(listStatusOptions),
                            "Mark your statisfaction", true, true, true, "complaintStatusChangeForm");

                        SetTextBoxField(dTemp.fields/*, -1*/, "statusChangeComments", "Status comments", Utility.GetEnumDisplayName(Config.DynamicControlType.TextBox), "text", count++, "None", null, "Add status comments",
                        true, true, true, 4000, 10, "complaintStatusChangeForm");
                        for (int i = 1; i <= 5; i++)
                        {
                            SetAttachment(dField, null, null, null, null, "attachment_" + i,
                                "attachment_" + i, null, Utility.GetEnumDisplayName(Config.DynamicControlType.FileAttachment), i, dTemp.formTag);

                        }
                    }
                }



                dCell.cellForm = Utility.SerializeDynamic(dCell.cellForm);
                dCell.complaintDetailForm = Utility.SerializeDynamic(dCell.complaintDetailForm);
                dCell.complaintChangeCategoryForm = dCell.complaintChangeCategoryForm != null ? Utility.SerializeDynamic(dCell.complaintChangeCategoryForm) : null;
                dCell.complaintStatusHistory = dCell.complaintStatusHistory != null ? Utility.SerializeDynamic(dCell.complaintStatusHistory) : null;
                //dCell.complaintStatusHistory = dCell.complaintStatusHistory != null ? dCell.complaintStatusHistory : null;

                dCell.complaintStatusChangeForm = dCell.complaintStatusChangeForm != null ? Utility.SerializeDynamic(dCell.complaintStatusChangeForm) : null;

                listDForm.Add(dCell);
            }
            data.listing = listDForm;
            data.status = true;
            data.message = string.Format("total no of records = {0}", listDForm.Count);
            return ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(data), null);
            //return listDForm;
            //return dCell = ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(listDForm), null);
        }

        private static void GetListOfFiles(List<dynamic> listAttachment, int referenceType, int referenceTypeId)
        {

        }

        private static void SetHidden(List<dynamic> listFields, string field_key, string field_value, string field_type, string formTag)
        {
            dynamic d = new ExpandoObject();
            d.field_key = field_key;
            d.field_value = field_value;
            d.field_type = field_type;
            d.formTag = formTag;
            listFields.Add(d);
        }

        private static object GetFieldValue(List<dynamic> listObj, string key)
        {
            dynamic obj = listObj.Where(n => n.key == key).FirstOrDefault();
            if (obj != null)
            {
                return obj.value;
            }
            return null;
        }

        private static void SetAttachment(List<dynamic> listFields, string field_url, string field_file_name, string field_file_extension, string field_content_type
            , string field_key, string field_text, string field_value, string field_type, int field_order_in_form, string formTag)
        {
            dynamic d = new ExpandoObject();

            d.field_url = field_url;
            d.field_file_name = field_file_name;
            d.field_file_extension = field_file_extension;
            d.field_content_type = field_content_type;

            d.field_key = field_key;
            d.field_type = field_type;
            d.field_text = field_text;
            d.field_value = field_value;
            //d.field_order_in_form = field_order_in_form;
            d.formTag = formTag;
            listFields.Add(d);
        }


        private static void SetLabel(List<dynamic> listFields, string field_key, string field_text, string field_value, string field_type, int field_order_in_form, string formTag)
        {
            dynamic d = new ExpandoObject();
            d.field_key = field_key;
            d.field_text = field_text;
            d.field_value = field_value;
            d.field_type = field_type;
            //d.field_order_in_form = field_order_in_form;
            d.formTag = formTag;
            listFields.Add(d);
        }

        public static void SetDropdownField(List<dynamic> listFields, int field_id, string field_key, string field_text, string field_value, string field_type, int field_group, int field_hirarchy, int field_position_in_group, int field_order_in_form, int field_dropDown_Hierarchy,
            int? field_dependent_On_Id, string field_options, string field_hint, bool field_isEditable, bool field_isFocusable, bool field_isRequired, string formTag)
        {
            dynamic d = new ExpandoObject();
            //d.field_id = field_id;
            d.field_key = field_key;
            d.field_text = field_text;
            d.field_value = field_value;
            d.field_type = field_type;
            d.field_group = field_group;
            d.field_hirarchy = field_hirarchy;
            d.field_position_in_group = field_position_in_group;
            //d.field_order_in_form = field_order_in_form;
            d.field_hirarchy = field_dropDown_Hierarchy;
            //d.field_dependent_On_Id = field_dependent_On_Id;
            d.field_options = field_options;
            d.field_hint = field_hint;
            d.field_editable = field_isEditable;
            d.field_focusable = field_isFocusable;
            d.field_required = field_isRequired;

            d.formTag = formTag;
            listFields.Add(d);
        }

        public static void SetTextBoxField(List<dynamic> listFields/*, int field_id*/, string field_key, string field_text, string field_type, string field_input_type, int field_order_in_form, string field_special_case,
            string field_value, string field_hint, bool field_isEditable, bool field_isFocusable, bool field_isRequired, int field_length, int field_lines, string formTag)
        {
            dynamic d = new ExpandoObject();
            //d.field_id = field_id;
            d.field_key = field_key;
            d.field_text = field_text;
            d.field_type = field_type;
            d.field_input_type = field_input_type;
            //d.field_order_in_form = field_order_in_form;
            d.field_special_case = field_special_case;
            d.field_value = field_value;
            d.field_hint = field_hint;
            d.field_editable = field_isEditable;
            d.field_focusable = field_isFocusable;
            d.field_required = field_isRequired;
            d.text_length = field_length;
            d.text_lines = field_lines;
            d.formTag = formTag;

            listFields.Add(d);
        }


        private static List<dynamic> _GetPublicUserComplaints(dynamic dParam)
        {
            List<dynamic> listObj = dParam.fields;
            //int userId = DbUsers.GetUserIdFromHashString(listObj.Where(n => n.key == "userToken").FirstOrDefault().value);
            int userId = DbUsers.GetUserIdFromHashString(dParam.data.userToken);
            DbUsers dbUser = DbUsers.GetUser(userId);
            //List<object> arr = listObj.Where(n => n.key == "listingStatus").FirstOrDefault().value;

            //string jsonData = @"{
            //    'FirstName':'Jignesh',
            //    'LastName':'Trivedi'
            //    }";

            //var details = JObject.Parse(jsonData);
            string commaSepStatuses = listObj.Where(n => n.key == "listingStatus").FirstOrDefault().value;
            List<int> listStatuses = Utility.GetIntList(commaSepStatuses); //listObj.Where(n => n.key == "listingStatus").FirstOrDefault().value;
            Dictionary<int, List<int>> dictFilterStatuses = dParam.statusMask.dictFilterStatuses;
            List<int> listMappedStatuses = new List<int>();
            for (int i = 0; i < listStatuses.Count; i++)
            {
                listMappedStatuses.AddRange(dictFilterStatuses[listStatuses[i]]);
            }
            listStatuses = listMappedStatuses;

            DataTable tableStatusIds = DBHelper.GetIdsDataTable(listStatuses);


            Dictionary<string, object> dictQueryParam = new Dictionary<string, object>();
            dictQueryParam.Add("@fromDate", listObj.Where(n => n.key == "listingStartDate").FirstOrDefault().value + " 00:00:00");
            dictQueryParam.Add("@toDate", listObj.Where(n => n.key == "listingEndDate").FirstOrDefault().value + " 23:59:59");
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@tableStatusIds";
            sqlParam.SqlDbType = SqlDbType.Structured;
            sqlParam.TypeName = "pitb.IdsTable";
            sqlParam.Value = tableStatusIds;
            dictQueryParam.Add("@tableStatusIds", sqlParam);
            dictQueryParam.Add("@fromRow", listObj.Where(n => n.key == "fromRow").FirstOrDefault().value);
            dictQueryParam.Add("@toRow", listObj.Where(n => n.key == "toRow").FirstOrDefault().value);
            dictQueryParam.Add("@createdBy", userId);
            dictQueryParam.Add("@campaignIds", dbUser.Campaigns);

            string queryStr = QueryHelper.GetFinalQuery("Agent_ComplaintsListing_Mine_Mobile", Config.ConfigType.Query, //dictQueryParam
                new Dictionary<string, object>() {
                    { "@campaignIds", dbUser.Campaigns }
                }
                );
            List<dynamic> listDynamic = DBHelper.GetDynamicListByQueryString(queryStr, dictQueryParam);
            return listDynamic;
        }


        public static dynamic LoadForm(dynamic d, HttpRequest request)
        {

            dynamic response = new ExpandoObject();

            response.message = "Success";
            response.code = 200;
            response.status = true;
            response.data = new ExpandoObject();
            response.data.fields = new List<ExpandoObject>();




            dynamic field1 = new ExpandoObject();
            field1.type = "edittext";
            field1.input_type = "text";
            field1.special_case = "cnic";
            field1.text = "text here";
            field1.hint = "hint here";
            field1.editable = "true";
            field1.focusable = "true";
            field1.value = "value here";
            field1.required = "true";
            field1.length = "15";
            field1.lines = "1";
            field1.options = "json string of key value pairs of items";
            field1.group = "1";
            field1.hirarchy = "1";
            field1.position_in_group = "1";

            dynamic field2 = new ExpandoObject();
            field2.type = "edittext";
            field2.input_type = "text";
            field2.special_case = "cnic";
            field2.text = "text here";
            field2.hint = "hint here";
            field2.editable = "true";
            field2.focusable = "true";
            field2.value = "value here";
            field2.required = "true";
            field2.length = "15";
            field2.lines = "1";
            field2.options = "json string of key value pairs of items";
            field2.group = "1";
            field2.hirarchy = "1";
            field2.position_in_group = "1";


            response.data.fields.Add(field1);
            response.data.fields.Add(field2);


            response = Utility.GetApiResponse(true, "Form Loaded", null, response);
            return response;
        }
    }
}
