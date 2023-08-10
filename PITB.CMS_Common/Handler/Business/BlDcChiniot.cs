using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlDcChiniot
    {
        public static dynamic GetPersonInfo(int personId)
        {
            DbPersonInformation dbPersonInfo = BlCommon.GetPersonInfo(personId);
            dynamic d = new ExpandoObject();
            if (dbPersonInfo != null)
            {
                d.personName = dbPersonInfo.Person_Name;
                d.cnicNo = dbPersonInfo.Cnic_No;
                d.isCnicPresent = dbPersonInfo.Is_Cnic_Present;
                d.mobileNo = dbPersonInfo.Mobile_No;
                d.secondaryMobileNo = dbPersonInfo.Secondary_Mobile_No;
                d.address = dbPersonInfo.Person_Address;
                d.personId = dbPersonInfo.Person_id;

                d.gender = (int)dbPersonInfo.Gender;
                d.listGender = new List<object>()
                {
                   new { id = "0", name = "Female"},
                   new { id = "1", name = "Male"}
                };

                d.provinceId = dbPersonInfo.Province_Id;
                d.listProvince = DbProvince.AllProvincesList();

                d.districtId = dbPersonInfo.District_Id;
                d.listDistrict = DbDistrict.GetByGroupId(null);
                d.isPresent = true;
            }
            else
            {
                d.personName = null;
                d.cnicNo = null;
                d.isCnicPresent = null;
                d.mobileNo = null;
                d.secondaryMobileNo = null;
                d.address = null;
                d.personId = null;
                

                d.gender = -1;
                d.listGender = new List<object>()
                {
                   new { id = "0", name = "Female"},
                   new { id = "1", name = "Male"}
                };

                d.provinceId = 1;
                d.listProvince = DbProvince.AllProvincesList();

                d.districtId = -1;
                d.listDistrict  = DbDistrict.GetByGroupId(null);
                d.isPresent = false;
            }
            return d;
        }

        public static dynamic GetComplaintInfo(int campaignId)
        {
            dynamic d = new ExpandoObject();

            List<DbDynamicFormControls> listDbDynamicFc = DynamicFieldsHandler.GetFormControls(campaignId);
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(campaignId, Config.Hierarchy.District);

            d.provinceId = 1;
            d.listProvince = DbProvince.AllProvincesList();

            d.districtId = 36;
            d.listDistrict = DbDistrict.GetDistrictByProvinceAndGroup(1, groupId);

            d.tehsilId = -1;
            d.listTehsil = DbTehsil.GetTehsil(d.districtId, campaignId);

            d.listDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            d.listCategory = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);
            d.listDynamicControl = DynamicFieldsHandler.GetFormControls(campaignId);
            d.hasDepartment = (d.listDepartment != null && d.listDepartment.Count > 0) ? true : false;
            d.campaignId = campaignId;
            return d;
        }


        public static dynamic GetModelFromComplaintAddView(CustomForm.Post form)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            string moduleSep = null;
            dynamic d = new ExpandoObject();

            d.srctag = "src::web__module::agent";
            #region personInfo
                d.personId = form.GetElementValue("personId").CastObj<int?>();
                d.personName = form.GetElementValue("personName").CastObj<string>();
                d.personGender = form.GetElementValue("personGender").CastObj<int>();
                d.personCnic = form.GetElementValue("personCnic").CastObj<string>();
                d.personIsCnicPresent = !Convert.ToBoolean(form.GetElementValue("personIsCnicNotPresent").CastObj<string>());
                d.personIsProfileEditing = d.personId == null?false:true;

                d.personMobileNo = form.GetElementValue("personMobileNo").CastObj<string>();
                d.personSecondaryNo = form.GetElementValue("personSecondaryNo").CastObj<string>();
                d.personAddress = form.GetElementValue("personAddress").CastObj<string>();

                d.personProvinceId = form.GetElementValue("personProvince").CastObj<int?>();
                d.personDistrictId = form.GetElementValue("personDistrict").CastObj<int?>();
                d.personDivisionId = ((DbDistrict)DbDistrict.GetById(d.personDistrictId)).Division_Id;
            #endregion


            #region complaintSection
            d.campaignId = form.GetElementValue(string.Format("campaignId")).CastObj<int>();
                d.complaintType = form.GetElementValue(string.Format("complaintType")).CastObj<string>().ToLower();
                moduleSep = d.complaintType + "__";
                if (d.complaintType == "complaint")
                {
                    d.complaintType = (int)Config.ComplaintType.Complaint;
                }
                else if (d.complaintType == "suggestion")
                {
                    d.complaintType = (int)Config.ComplaintType.Suggestion;
                }
                else if (d.complaintType == "inquiry")
                {
                    d.complaintType = (int)Config.ComplaintType.Inquiry;
                }


                d.categoryId = form.GetElementValue(string.Format("{0}category", moduleSep)).CastObj<int>();
                d.subcategoryId = form.GetElementValue(string.Format("{0}subcategory", moduleSep)).CastObj<int>();

                d.provinceId = form.GetElementValue(string.Format("{0}province", moduleSep)).CastObj<int>();
                d.districtId = form.GetElementValue(string.Format("{0}district", moduleSep)).CastObj<int>();
                d.divisionId = ((DbDistrict)DbDistrict.GetById(d.districtId)).Division_Id;
                d.tehsilId = form.GetElementValue(string.Format("{0}tehsil", moduleSep)).CastObj<int>();

                d.agentComments = form.GetElementValue(string.Format("{0}agentComments", moduleSep)).CastObj<string>();
                d.complaintDetail = form.GetElementValue(string.Format("{0}detail", moduleSep)).CastObj<string>();
            #endregion

            d.complaintCreatedBy = cookie.UserId;
            d.complaintSrc = (int) Config.ComplaintSource.Agent;

            return d;
        }

        public static dynamic PostTimeChange(dynamic dParam)
        {
            CustomForm.Post cForm = dParam.postedForm;
            dynamic dForm = cForm.GetDynamicForm();
            return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint time changed Id = " + dForm.complaintId);
        }

        public static dynamic PostStatusChange(dynamic dParam)
        {
            dynamic dForm = null;
            DbPermissionsAssignment dbPermissionAssignment = null;
            CustomForm.Post cForm = dParam.postedForm;
            if (dParam.srcTag == "src::web__module::agent")
            {
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                dbPermissionAssignment = cmsCookie.ListPermissions.Where(n => n.Permission_Id == (int)Config.Permissions.AssignmentMatrixTagOnStatusChange).FirstOrDefault();
                dForm = cForm.GetDynamicForm();
                dForm.userId = cmsCookie.UserId;
                dForm.statusId = Utility.GetPropertyValueFromDynamic(dForm, "VmStatusChange.statusID");
                dForm.statusComments = Utility.GetPropertyValueFromDynamic(dForm, "VmStatusChange.statusChangeComments");
                dForm.assignmentMatrixTag = dbPermissionAssignment?.Permission_Value;
                dForm.postedFiles = cForm.postedFiles;
                //dForm.
                dForm.complaintId = int.Parse(dForm.complaintId);
                dForm.statusId = int.Parse(dForm.statusId);
            }
            else if (dParam.srcTag == "src::mobile__module::publicUser")
            {
                dForm = dParam.statusChangeModel;
                //dForm.userId = ;
                //dForm.statusId = Utility.GetPropertyValueFromDynamic(dForm, "VmStatusChange.statusID");
                //dForm.statusComments = Utility.GetPropertyValueFromDynamic(dForm, "VmStatusChange.statusChangeComments");
                dbPermissionAssignment = ((List<DbPermissionsAssignment>) DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                    (int)Config.PermissionsType.User, dForm.userId, (int)Config.Permissions.AssignmentMatrixTagOnStatusChange)).FirstOrDefault();
                dForm.assignmentMatrixTag = dbPermissionAssignment?.Permission_Value;
                //dForm.postedFiles = cForm.postedFiles;
                //dForm.
                //dForm.complaintId = int.Parse(dForm.complaintId);
                //dForm.statusId = int.Parse(dForm.statusId);
            }

            if (dForm.statusId == (int)Config.ComplaintStatus.InapplicableVerfied)
            {
                dForm.assignmentMatrixTag = "user::dc";
            }
            else
            {
                dForm.assignmentMatrixTag = PermissionHandler.GetUserPermissionValue(Config.Permissions.AssignmentMatrixTagOnStatusChange, dForm.userId);
            }
            StatusHandler.ChangeStatusDynamic(dForm);

            if (dParam.srcTag == "src::web__module::agent")
            {
                return Utility.GetDynamicSuccessResponse("Status changed successfully, ComplaintId =" + dForm.complaintId);
            }
            else if (dParam.srcTag == "src::mobile__module::publicUser")
            {
                return Utility.GetInitializedDynamic(new { complaintId = dParam.statusChangeModel.complaintIdStr, message = "Complaint status changed successfully Id = " + dParam.statusChangeModel.complaintIdStr });
            }
            return null;
            //return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint status changed Id = " + dForm.complaintId);
        }

        //public static dynamic PostCategoryChange(dynamic dParam)
        //{
        //    CustomForm.Post cForm = dParam.postedForm;
        //    dynamic dForm = cForm.GetDynamicForm();
        //    return new Config.CommandMessage(Config.CommandStatus.Success, "Category changed Id = " + dForm.complaintId);
        //}

        public static dynamic AddComplaint2(dynamic d)
        {
            dynamic dAddComplaintModel;
            string complaintIdStr = null;
            if (d.srcTag == "src::web__module::agent") // if complaint added from web
            {
                dAddComplaintModel = GetModelFromComplaintAddView(d.postedForm);
                Utility.PrintDynamic(dAddComplaintModel);
                dynamic dSpModel = BlComplaints.GetAddComplaintSpModel(dAddComplaintModel);
                Dictionary<string, object> dictParamsAddComplaint = BlComplaints.GetAddComplaintSpParamsDict(dSpModel);

                DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", dictParamsAddComplaint);
                complaintIdStr = dt.Rows[0][1].ToString();
                int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);

                PostModel.File postedFiles = d.postedForm.postedFiles;
                FileUploadHandler.UploadMultipleFiles(postedFiles, Config.AttachmentReferenceType.Add, complaintIdStr, complaintId, Config.TAG_COMPLAINT_ADD);
                DynamicFieldsHandler.SaveDyamicFieldsInDb(d.postedForm, null, complaintId);
                return Utility.GetDynamicSuccessResponse("Complaint added successfully Id = " + complaintIdStr);
            }
            else if(d.srcTag == "src::mobile__module::publicUser")
            {
                Utility.PrintDynamic(d.addComplaintModel);
                dynamic dSpModel = BlComplaints.GetAddComplaintSpModel(d.addComplaintModel);
                Dictionary<string, object> dictParamsAddComplaint = BlComplaints.GetAddComplaintSpParamsDict(dSpModel);

                DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", dictParamsAddComplaint);
                complaintIdStr = dt.Rows[0][1].ToString();
                int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);

                PostModel.File postedFiles = d.postedForm.postedFiles;
                FileUploadHandler.UploadMultipleFiles(postedFiles, Config.AttachmentReferenceType.Add, complaintIdStr, complaintId, Config.TAG_COMPLAINT_ADD);
                DynamicFieldsHandler.SaveDyamicFieldsInDb(d.postedForm, null, complaintId);
                return Utility.GetInitializedDynamic( new { complaintId = complaintIdStr, message = "Complaint added successfully Id = " + complaintIdStr });
            }
            return null;
            //return Utility.GetDynamicSuccessResponse("Complaint added successfully Id = " + complaintIdStr);
            //return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint added successfully Id = " + complaintIdStr);
            //return null;


            //Dictionary<string, object> paramDict = new Dictionary<string, object>();
            //paramDict.Add("@Id", -1);
            //paramDict.Add("@Person_Id", d.personId);
            //paramDict.Add("@DepartmentId", d.departmentId);
            //paramDict.Add("@Complaint_Type", d.complaintType);
            //paramDict.Add("@Complaint_Category", d.categoryId.ToDbObj());
            //paramDict.Add("@Complaint_SubCategory", d.subcategoryId.ToDbObj());
            //paramDict.Add("@Compaign_Id", d.campaignId.ToDbObj());
            //paramDict.Add("@Province_Id", d.provinceId.ToDbObj());
            //paramDict.Add("@Division_Id", d.divisionId.ToDbObj());
            //paramDict.Add("@District_Id", d.districtId.ToDbObj());
            //paramDict.Add("@Tehsil_Id", d.tehsilId.ToDbObj());
            //paramDict.Add("@UnionCouncil_Id", d.ucId ?? 0);
            //paramDict.Add("@Ward_Id", d.wardId ?? 0);
            //paramDict.Add("@Complaint_Remarks", d.complaintDetail.ToDbObj());
            //paramDict.Add("@Agent_Comments", d.agentComments.ToDbObj());

            //paramDict.Add("@Agent_Id", d.agentId.ToDbObj());
            //paramDict.Add("@Complaint_Address", d.complaintAddress.ToDbObj());
            //paramDict.Add("@Business_Address", d.businessAddress.ToDbObj());

            //paramDict.Add("@Complaint_Status_Id", d.complaintStatusId.ToDbObj());//If complaint is adding then set complaint status to 1 (Pending(Fresh) 
            //paramDict.Add("@Created_Date", complaintCreatedDate.ToDbObj());
            //paramDict.Add("@Created_By", ComplaintCreatedBy.ToDbObj());
            //paramDict.Add("@Complaint_Assigned_Date", complaintAssignedDate.ToDbObj());
            //paramDict.Add("@Completed_Date", complaintCompletedDate.ToDbObj());
            ////paramDict.Add("@Updated_Date", (null as object).ToDbObj());
            //paramDict.Add("@Updated_By", complaintUpdatedBy.ToDbObj());
            //paramDict.Add("@Is_Deleted", complaintIsDeleted.ToDbObj());
            //paramDict.Add("@Date_Deleted", complaintDeletedDate.ToDbObj());
            //paramDict.Add("@Deleted_By", complaintDeletedBy.ToDbObj());
            //paramDict.Add("@ComplaintSrc", complaintSrc.ToDbObj());
            //paramDict.Add("@IsComplaintEditing", isComplaintEditing.ToDbObj());

            ////Personal Information
            //paramDict.Add("@p_Person_id", personId.ToDbObj());
            //paramDict.Add("@Person_Name", personName.ToDbObj());
            //paramDict.Add("@Person_Father_Name", personFatherName.ToDbObj());
            //paramDict.Add("@Is_Cnic_Present", personIsCnicPresent.ToDbObj());
            //paramDict.Add("@Cnic_No", personCnic.ToDbObj());
            //paramDict.Add("@Gender", personGender.ToDbObj());
            //paramDict.Add("@Mobile_No", personMobileNo.ToDbObj());
            //paramDict.Add("@Secondary_Mobile_No", personSecondaryNo.ToDbObj());
            //paramDict.Add("@LandLine_No", personLandlineNo.ToDbObj());
            //paramDict.Add("@Person_Address", personAddress.ToDbObj());
            //paramDict.Add("@Email", personEmail.ToDbObj());
            //paramDict.Add("@Nearest_Place", personNearestPlace.ToDbObj());
            //paramDict.Add("@p_Province_Id", personProvinceId.ToDbObj());
            //paramDict.Add("@p_Division_Id", personDivisionId.ToDbObj());
            //paramDict.Add("@p_District_Id", personDistrictId.ToDbObj());
            //paramDict.Add("@p_Tehsil_Id", personTehsilId.ToDbObj());
            //paramDict.Add("@p_Town_Id", personTehsilId.ToDbObj());
            //paramDict.Add("@p_Uc_Id", personUcId.ToDbObj());
            //paramDict.Add("@p_Created_By", personCreatedBy.ToDbObj());
            //paramDict.Add("@p_Updated_By", personUpdatedBy.ToDbObj());
            //paramDict.Add("@IsProfileEditing", isProfileEditing.ToDbObj());
            //for (int i = 0; i < 10; i++)
            //{
            //    if (i < assignmentModelList.Count)
            //    {
            //        paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
            //        paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
            //        paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
            //    }
            //    else
            //    {
            //        paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
            //        paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
            //        paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
            //    }
            //}

            //paramDict.Add("@MaxLevel", assignmentModelList.Count);

            //paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList).ToDbObj());
            //paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList).ToDbObj());

            //paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList).ToDbObj());
            //paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList).ToDbObj());

            //if (!string.IsNullOrEmpty(form.GetElementValue("RadioInPerson")))
            //{
            //    paramDict.Add("@RefField1",
            //        form.GetElementValue("RadioInPerson")
            //            .Split(new string[] { Config.Separator }, StringSplitOptions.None)[1].CastObj<string>());
            //}
            //if (!string.IsNullOrEmpty(form.GetElementValue("ComplaintVm.ListDynamicDropDown[2].SelectedItemId")))
            //{
            //    paramDict.Add("@RefField2",
            //        form.GetElementValue("ComplaintVm.ListDynamicDropDown[2].SelectedItemId")
            //            .Split(new string[] { Config.Separator }, StringSplitOptions.None)[1].CastObj<string>());
            //}
            //paramDict.Add("@RefField3", form.GetElementValue("DynamicComplaintVmCnicInPerson").CastObj<string>());
            //paramDict.Add("@RefField4", form.GetElementValue("DynamicComplaintVmMobileNoInPerson").CastObj<string>());
            //paramDict.Add("@RefField5", form.GetElementValue("DynamicComplaintVmNameInPerson").CastObj<string>());


            // Add ComplaintSp
            //DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
        }


        /// <summary>
        /// currentStatusId = 1;
        /// List(DbPermissionsAssignment) listDbPermissionAssignment;
        /// List(DbStatus)listDbStatuses = d.listDbStatuses;
        /// VmStakeholderComplaintDetail.DetailType detailType;
        /// int campaignId;
        /// string srcTag='web'/ 'mobile';
        /// DbUsers dbUser;
        /// </summary>
        public static dynamic GetComplaintDetail(dynamic dParam)
        {
            dynamic dComplaint = dParam.dComplaint;
            //if (complaintId == 678307)
            //{
            //    complaintId = complaintId;
            //}
            int currentStatusId = dParam.currentStatusId;
            dynamic resp = new ExpandoObject();
            resp.canChangeCategory = false;
            resp.canChangeStatus = true;
            VmStakeholderComplaintDetail.DetailType detailType = dParam.detailType;

            int userId = -1;
            int hierarchyId = -1;
            int userHierarchyId = -1;
            

            if (dParam.srcTag == "web")
            {
                CMSCookie cmsCookie = dParam.cmsCookie;
                userId = cmsCookie.UserId;
                hierarchyId = (int)cmsCookie.Hierarchy_Id;
                userHierarchyId = (int)cmsCookie.User_Hierarchy_Id;
            }
            else
            {
                DbUsers dbUser = dParam.dbUser;
                userId = dbUser.Id;
                hierarchyId = (int)dbUser.Hierarchy_Id;
                userHierarchyId = (int)dbUser.User_Hierarchy_Id;
            }

            resp.listStatuses = VmStakeholderComplaintDetail.GetListDbStatusesAgainstComplaintDetail(dParam);

            if (detailType == VmStakeholderComplaintDetail.DetailType.AssignedToMe)
            {
                DbUsers dbUser = dParam.dbUser;
                if(hierarchyId == 3 && userHierarchyId ==200 ) //CCI
                {
                    if(currentStatusId==33 || currentStatusId == 34)
                    {
                        resp.canChangeCategory = false;
                    }
                    else
                    {
                        resp.canChangeCategory = true;
                    }

                    if (resp.canChangeStatus)
                    {
                        List<int> listAssignedStatuses = null;
                        if (dComplaint.Complaint_Status_Id == 13) // if inapplicable then 
                        {
                            listAssignedStatuses = new List<int> { 7, 14 };
                        }
                        else if (dComplaint.Complaint_Status_Id == 32) // if inapplicable then 
                        {
                            listAssignedStatuses = new List<int> { 14 };
                        }
                        else if (dComplaint.Complaint_Status_Id == 12) // if resolved unverified
                        {
                            listAssignedStatuses = new List<int> { 3, 7 };
                        }
                        else if (dComplaint.Complaint_Status_Id == 33) // if resolved unverified
                        {
                            listAssignedStatuses = new List<int>();
                        }

                        resp.listStatuses = (((List<DbStatus>)dParam.listDbStatuses).Where(n => listAssignedStatuses.Contains(n.Complaint_Status_Id)).ToList());

                        //vmStakeholderComplaintDetail.VmStatusChange.ListStatus = UiUtility.GetSelectList(
                        //        DbStatus.GetByStatusIds(listAssignedStatuses),
                        //        "Complaint_Status_Id",
                        //        "Status",
                        //        "-- Select--",
                        //        null);
                    }
                    //else
                    //{
                    //    resp.listStatuses = VmStakeholderComplaintDetail.GetListDbStatusesAgainstComplaintDetail(dParam);
                    //}
                
                }
            }


            //// new code
            //if (resp.canChangeStatus && hierarchyId == 3 && userHierarchyId == 200) // CCI
            //{
            //    //13  Inapplicable
            //    //14  Inapplicable(Verfied)
            //    List<int> listAssignedStatuses = null;
            //    if (dComplaint.Complaint_Status_Id == 13) // if inapplicable then 
            //    {
            //        listAssignedStatuses = new List<int> { 7, 14 };
            //    }
            //    else if (dComplaint.Complaint_Status_Id == 32) // if inapplicable then 
            //    {
            //        listAssignedStatuses = new List<int> { 14 };
            //    }
            //    else if (dComplaint.Complaint_Status_Id == 12) // if resolved unverified
            //    {
            //        listAssignedStatuses = new List<int> { 3, 7 };
            //    }
            //    else if (dComplaint.Complaint_Status_Id == 33) // if resolved unverified
            //    {
            //        listAssignedStatuses = new List<int>();
            //    }

            //    resp.listStatuses = (((List<DbStatus>) dParam.listDbStatuses).Where(n=>listAssignedStatuses.Contains(n.Complaint_Status_Id)).ToList());

            //    //vmStakeholderComplaintDetail.VmStatusChange.ListStatus = UiUtility.GetSelectList(
            //    //        DbStatus.GetByStatusIds(listAssignedStatuses),
            //    //        "Complaint_Status_Id",
            //    //        "Status",
            //    //        "-- Select--",
            //    //        null);
            //}
            //else
            //{
            //    resp.listStatuses = VmStakeholderComplaintDetail.GetListDbStatusesAgainstComplaintDetail(dParam);
            //}
            //// end new code
            
            return resp;
        }

        public static VmStakeholderComplaintDetail GetComplaintDetail(int complaintId, int userId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            dynamic d = new ExpandoObject();
            DbComplaint dbComplaint = DbComplaint.GetBy(complaintId);
            DbUsers dbUser = DbUsers.GetActiveUser(userId);
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmStakeholderComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId, userId, detailType);

            if (detailType == VmStakeholderComplaintDetail.DetailType.AssignedToMe)
            {
                vmStakeholderComplaintDetail.CanChangeStatus = true;
                vmStakeholderComplaintDetail.CanChangeCategory = true;
                // if last status changed is by same person
                if (dbComplaint.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.UnsatisfactoryClosed
                    || dbComplaint.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.PendingReopened)
                {

                    if (dbComplaint.StatusChangedBy_HierarchyId == (int)dbUser.Hierarchy_Id
                        && dbComplaint.StatusChangedBy_User_HierarchyId > dbUser.User_Hierarchy_Id)
                    {
                        //if (cookie.User_Hierarchy_Id >= 200)
                        {
                            vmStakeholderComplaintDetail.CanChangeStatus = true;
                            vmStakeholderComplaintDetail.CanChangeCategory = true;
                        }
                    }
                    else
                    {
                        vmStakeholderComplaintDetail.CanChangeStatus = false;
                        vmStakeholderComplaintDetail.CanChangeCategory = false;
                    }
                }
                //else
                //{
                //    vmStakeholderComplaintDetail.CanChangeStatus = true;
                //    vmStakeholderComplaintDetail.CanChangeCategory = true;
                //}
            }
            if (vmStakeholderComplaintDetail.CanChangeStatus && (int)dbUser.Hierarchy_Id ==3 && dbUser.User_Hierarchy_Id==200) // CCI
            {
                //13  Inapplicable
                //14  Inapplicable(Verfied)
                List<int> listAssignedStatuses = null;
                if (dbComplaint.Complaint_Status_Id==13) // if inapplicable then 
                {
                    listAssignedStatuses = new List<int> { 7, 14 };
                }
                else if (dbComplaint.Complaint_Status_Id == 32) // if inapplicable then 
                {
                    listAssignedStatuses = new List<int> { 14 };
                }
                else if(dbComplaint.Complaint_Status_Id == 12) // if resolved unverified
                {
                    listAssignedStatuses = new List<int> { 3, 7 };  
                }
                else if (dbComplaint.Complaint_Status_Id == 33) // if resolved unverified
                {
                    listAssignedStatuses = new List<int> ();
                }

                vmStakeholderComplaintDetail.VmStatusChange.ListStatus = UiUtility.GetSelectList(
                        DbStatus.GetByStatusIds(listAssignedStatuses),
                        "Complaint_Status_Id",
                        "Status",
                        "-- Select--",
                        null);
            }
           // vmStakeholderComplaintDetail.VmStatusChange.ListStatus = 
            return vmStakeholderComplaintDetail;
        }

    //    public static Config.CommandMessage AddComplaint(CustomForm.Post form)
    //    {
    //        PostModel.File postedFiles = form.postedFiles;

    //        CMSCookie cmsCookie;

    //        DateTime? nowDate, minSrcIdDate, maxSrcIdDate,
    //             complaintCreatedDate, complaintAssignedDate, complaintCompletedDate, complaintDeletedDate;

    //        bool isProfileEditing, isComplaintEditing, isFormValid, personIsCnicPresent = false, complaintIsDeleted;
    //        int? complaintProvinceId, complaintDistrictId, complaintDivisionId, complaintTehsilId, complaintUcId = 0, complaintWardId = 0,
    //            departmentId, categoryId = -1, subCategoryId = -1, agentId,
    //            complaintStatusId, userId, ComplaintCreatedBy, complaintUpdatedBy, complaintDeletedBy,
    //            personId, personGender, personProvinceId, personDivisionId, personDistrictId, personTehsilId, personUcId,
    //            personCreatedBy, personUpdatedBy,
    //            maxLevel, minSrcId, maxSrcId;
    //        int complaintType = -1, campaignId, complaintSrc;
    //        string currentTab, modelName = null, complaintVm = null, personVm = null, complaintRemarks = null, complaintAgentComments = null, complaintAddress = null,
    //            complaintBusinessAddress = null, personCnic = null, personName = null, personFatherName = null, personMobileNo = null, personSecondaryNo = null,
    //            personLandlineNo = null, personAddress = null, personEmail = null, personNearestPlace = null;

    //        float catRetainingHours = 0;
    //        float? subcatRetainingHours = 0;

    //        isFormValid = form.IsFormValid();

    //        if (!isFormValid)
    //        {
    //            Config.CommandMessage exceptionMsg = new Config.CommandMessage(Config.CommandStatus.Exception, "An error has occured");
    //            return exceptionMsg;
    //        }
    //        FileUploadHandler.FileValidationStatus validationStatus = FileUploadHandler.GetFileValidationStatus(postedFiles);

    //        //form.IsFormAuthentic();
    //        //int asd = Convert.ToInt32("hahaha");

    //        personVm = "PersonalInfoVm";
    //        complaintVm = "ComplaintVm";

    //        nowDate = DateTime.Now;
    //        cmsCookie = AuthenticationHandler.GetCookie();
    //        isProfileEditing = form.GetElementValue(string.Format("{0}.Person_id", personVm)).CastObj<int>() > 0;//(complaintModel.PersonalInfoVm.Person_id > 0); //form.GetElementValue("ComplaintVm.IsProfileEditing").CastObj<bool>();
    //        isComplaintEditing = form.GetElementValue(string.Format("{0}.Id", complaintVm)).CastObj<int>() != 0;//(complaintModel.ComplaintVm.Id != 0); //form.GetElementValue("ComplaintVm.IsComplaintEditing").CastObj<bool>();
    //        personCnic = form.GetElementValue(string.Format("{0}.Cnic_No", personVm)).CastObj<string>();
    //        campaignId = form.GetElementValue(string.Format("{0}.Compaign_Id", complaintVm)).CastObj<int>();
    //        personIsCnicPresent = form.GetElementValue(string.Format("{0}.IsCnicPresent", personVm)).CastObj<bool>();

    //        // Current Tab
    //        currentTab = form.GetElementValue("currentComplaintTypeTab").CastObj<string>();

    //        if (currentTab == VmAddComplaint.TabComplaint)
    //        {
    //            modelName = "ComplaintVm";
    //            complaintType = (int)Config.ComplaintType.Complaint;
    //        }
    //        else if (currentTab == VmAddComplaint.TabSuggestion)
    //        {
    //            modelName = "SuggestionVm";
    //            complaintType = (int)Config.ComplaintType.Suggestion;
    //        }
    //        else if (currentTab == VmAddComplaint.TabInquiry)
    //        {
    //            modelName = "InquiryVm";
    //            complaintType = (int)Config.ComplaintType.Inquiry;
    //        }

    //        // Person Cnic
    //        personIsCnicPresent = true;
    //        if (string.IsNullOrEmpty(personCnic))
    //        {
    //            personIsCnicPresent = false;
    //        }
    //        if (!personIsCnicPresent && !isProfileEditing)
    //        {
    //            decimal cnic = DbUniqueIncrementor.GetUniqueValue(Config.UniqueIncrementorTag.Cnic);
    //            string cnicStr = cnic.ToString();
    //            cnicStr = Utility.PaddLeft(cnicStr, Config.CnicLength, '0');
    //            personCnic = cnicStr;
    //        }

    //        // Complaint Status
    //        if (isComplaintEditing)
    //        {
    //            complaintStatusId = form.GetElementValue(string.Format("{0}.Complaint_Status_Id", complaintVm)).CastObj<int?>();
    //        }
    //        else
    //        {
    //            List<string> lisKeys = new List<string>()
    //            {
    //                string.Format("Type::Config___Module::ComplaintLaunchStatus___CampaignId::{0}",campaignId),
    //                "Type::Config___Module::ComplaintLaunchStatus"
    //            };
    //            complaintStatusId = int.Parse(ConfigurationHandler.GetConfiguration(lisKeys,
    //                complaintType.ToString()));
    //        }


    //        departmentId = form.GetElementValue(string.Format("{0}.departmentId", modelName)).CastObj<int?>();
    //        categoryId = form.GetElementValue(string.Format("{0}.Complaint_Category", modelName)).CastObj<int?>();
    //        subCategoryId = form.GetElementValue(string.Format("{0}.Complaint_SubCategory", modelName)).CastObj<int?>();


    //        complaintProvinceId = form.GetElementValue(string.Format("{0}.Province_Id", modelName)).CastObj<int?>();
    //        complaintDistrictId = form.GetElementValue(string.Format("{0}.District_Id", modelName)).CastObj<int?>();
    //        complaintDivisionId = DbDistrict.GetById((int)complaintDistrictId).Division_Id;
    //        complaintTehsilId = form.GetElementValue(string.Format("{0}.Tehsil_Id", modelName)).CastObj<int?>();
    //        complaintUcId = form.GetElementValue(string.Format("{0}.UnionCouncil_Id", modelName)).CastObj<int?>();
    //        complaintWardId = form.GetElementValue(string.Format("{0}.Ward_Id", modelName)).CastObj<int?>();



    //        // Assignment Matrix

    //        //int categoryId = -1;
    //        //Config.CategoryType cateogryType = Config.CategoryType.Main;

    //        //AssignmentMatrix
    //        List<AssignmentModel> assignmentModelList = null;
    //        if (currentTab == VmAddComplaint.TabComplaint) // when there is complaint populate assignment matrix
    //        {
    //            subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)subCategoryId);
    //            if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
    //            {
    //                catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)categoryId);
    //                //cateogryType = Config.CategoryType.Main;
    //            }
    //            else
    //            {
    //                catRetainingHours = (float)subcatRetainingHours;
    //                //cateogryType = Config.CategoryType.Sub;
    //            }

    //            List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
    //            {
    //                new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)complaintProvinceId),
    //                new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)complaintDivisionId),
    //                new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)complaintDistrictId),
    //                new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, (int?)complaintTehsilId),
    //                new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, (int?)complaintUcId),
    //                new Pair<int?, int?>((int?)Config.Hierarchy.Ward, (int?)complaintWardId)
    //            };


    //            assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment((DateTime)nowDate,
    //            DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)campaignId, (int)categoryId, (int)subCategoryId, null, null, listHierarchyPair), catRetainingHours) /* nowDate,
				//DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours*/);
    //        }
    //        else
    //        {
    //            assignmentModelList = new List<AssignmentModel>();
    //        }




    //        complaintRemarks = form.GetElementValue(string.Format("{0}.Complaint_Remarks", modelName)).CastObj<string>();
    //        complaintAgentComments = form.GetElementValue(string.Format("{0}.Agent_Comments", modelName)).CastObj<string>();

    //        complaintAddress = form.GetElementValue(string.Format("{0}.Complaint_Address", modelName)).CastObj<string>();
    //        complaintBusinessAddress = form.GetElementValue(string.Format("{0}.Business_Address", modelName)).CastObj<string>();

    //        userId = cmsCookie.UserId;
    //        agentId = userId;

    //        complaintStatusId = complaintStatusId;
    //        complaintCreatedDate = nowDate;
    //        ComplaintCreatedBy = userId;
    //        complaintAssignedDate = null;
    //        complaintCompletedDate = null;
    //        complaintUpdatedBy = userId;
    //        complaintIsDeleted = false;
    //        complaintDeletedDate = null;
    //        complaintDeletedBy = null;
    //        complaintSrc = (int)Config.ComplaintSource.Agent;

    //        personId = form.GetElementValue(string.Format("{0}.Person_id", personVm)).CastObj<int?>();
    //        personName = form.GetElementValue(string.Format("{0}.Person_Name", personVm)).CastObj<string>();
    //        personFatherName = form.GetElementValue(string.Format("{0}.Person_Father_Name", personVm)).CastObj<string>();
    //        //personIsCnicPresent = personIsCnicPresent;
    //        //personCnic = personCnic;
    //        personGender = form.GetElementValue(string.Format("{0}.Gender", personVm)).CastObj<int?>();
    //        personMobileNo = form.GetElementValue(string.Format("{0}.Mobile_No", personVm)).CastObj<string>();
    //        personSecondaryNo = form.GetElementValue(string.Format("{0}.Secondary_Mobile_No", personVm)).CastObj<string>();
    //        personLandlineNo = form.GetElementValue(string.Format("{0}.LandLine_No", personVm)).CastObj<string>();
    //        personAddress = form.GetElementValue(string.Format("{0}.Person_Address", personVm)).CastObj<string>();
    //        personEmail = form.GetElementValue(string.Format("{0}.Email", personVm)).CastObj<string>();
    //        personNearestPlace = form.GetElementValue(string.Format("{0}.Nearest_Place", personVm)).CastObj<string>();
    //        personProvinceId = form.GetElementValue(string.Format("{0}.Province_Id", personVm)).CastObj<int?>();
    //        personDivisionId = form.GetElementValue(string.Format("{0}.Division_Id", personVm)).CastObj<int?>();
    //        personDistrictId = form.GetElementValue(string.Format("{0}.District_Id", personVm)).CastObj<int?>();
    //        personTehsilId = form.GetElementValue(string.Format("{0}.Tehsil_Id", personVm)).CastObj<int?>();
    //        personTehsilId = form.GetElementValue(string.Format("{0}.Town_Id", personVm)).CastObj<int?>();
    //        personUcId = form.GetElementValue(string.Format("{0}.Uc_Id", personVm)).CastObj<int?>();
    //        personCreatedBy = userId;
    //        personUpdatedBy = userId;


    //        Dictionary<string, object> paramDict = new Dictionary<string, object>();
    //        paramDict.Add("@Id", -1);
    //        paramDict.Add("@Person_Id", personId.ToDbObj());
    //        paramDict.Add("@DepartmentId", departmentId.ToDbObj());
    //        paramDict.Add("@Complaint_Type", complaintType);
    //        paramDict.Add("@Complaint_Category", categoryId.ToDbObj());
    //        paramDict.Add("@Complaint_SubCategory", subCategoryId.ToDbObj());
    //        paramDict.Add("@Compaign_Id", campaignId.ToDbObj());
    //        paramDict.Add("@Province_Id", complaintProvinceId.ToDbObj());
    //        paramDict.Add("@Division_Id", complaintDivisionId.ToDbObj());
    //        paramDict.Add("@District_Id", complaintDistrictId.ToDbObj());
    //        paramDict.Add("@Tehsil_Id", complaintTehsilId.ToDbObj());
    //        paramDict.Add("@UnionCouncil_Id", complaintUcId ?? 0);
    //        paramDict.Add("@Ward_Id", complaintWardId ?? 0);
    //        paramDict.Add("@Complaint_Remarks", complaintRemarks.ToDbObj());
    //        paramDict.Add("@Agent_Comments", complaintAgentComments.ToDbObj());

    //        paramDict.Add("@Agent_Id", agentId.ToDbObj());
    //        paramDict.Add("@Complaint_Address", complaintAddress.ToDbObj());
    //        paramDict.Add("@Business_Address", complaintBusinessAddress.ToDbObj());

    //        paramDict.Add("@Complaint_Status_Id", complaintStatusId.ToDbObj());//If complaint is adding then set complaint status to 1 (Pending(Fresh) 
    //        paramDict.Add("@Created_Date", complaintCreatedDate.ToDbObj());
    //        paramDict.Add("@Created_By", ComplaintCreatedBy.ToDbObj());
    //        paramDict.Add("@Complaint_Assigned_Date", complaintAssignedDate.ToDbObj());
    //        paramDict.Add("@Completed_Date", complaintCompletedDate.ToDbObj());
    //        //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
    //        paramDict.Add("@Updated_By", complaintUpdatedBy.ToDbObj());
    //        paramDict.Add("@Is_Deleted", complaintIsDeleted.ToDbObj());
    //        paramDict.Add("@Date_Deleted", complaintDeletedDate.ToDbObj());
    //        paramDict.Add("@Deleted_By", complaintDeletedBy.ToDbObj());
    //        paramDict.Add("@ComplaintSrc", complaintSrc.ToDbObj());
    //        paramDict.Add("@IsComplaintEditing", isComplaintEditing.ToDbObj());

    //        //Personal Information
    //        paramDict.Add("@p_Person_id", personId.ToDbObj());
    //        paramDict.Add("@Person_Name", personName.ToDbObj());
    //        paramDict.Add("@Person_Father_Name", personFatherName.ToDbObj());
    //        paramDict.Add("@Is_Cnic_Present", personIsCnicPresent.ToDbObj());
    //        paramDict.Add("@Cnic_No", personCnic.ToDbObj());
    //        paramDict.Add("@Gender", personGender.ToDbObj());
    //        paramDict.Add("@Mobile_No", personMobileNo.ToDbObj());
    //        paramDict.Add("@Secondary_Mobile_No", personSecondaryNo.ToDbObj());
    //        paramDict.Add("@LandLine_No", personLandlineNo.ToDbObj());
    //        paramDict.Add("@Person_Address", personAddress.ToDbObj());
    //        paramDict.Add("@Email", personEmail.ToDbObj());
    //        paramDict.Add("@Nearest_Place", personNearestPlace.ToDbObj());
    //        paramDict.Add("@p_Province_Id", personProvinceId.ToDbObj());
    //        paramDict.Add("@p_Division_Id", personDivisionId.ToDbObj());
    //        paramDict.Add("@p_District_Id", personDistrictId.ToDbObj());
    //        paramDict.Add("@p_Tehsil_Id", personTehsilId.ToDbObj());
    //        paramDict.Add("@p_Town_Id", personTehsilId.ToDbObj());
    //        paramDict.Add("@p_Uc_Id", personUcId.ToDbObj());
    //        paramDict.Add("@p_Created_By", personCreatedBy.ToDbObj());
    //        paramDict.Add("@p_Updated_By", personUpdatedBy.ToDbObj());
    //        paramDict.Add("@IsProfileEditing", isProfileEditing.ToDbObj());
    //        for (int i = 0; i < 10; i++)
    //        {
    //            if (i < assignmentModelList.Count)
    //            {
    //                paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
    //                paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
    //                paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
    //            }
    //            else
    //            {
    //                paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
    //                paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
    //                paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
    //            }
    //        }

    //        paramDict.Add("@MaxLevel", assignmentModelList.Count);

    //        paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList).ToDbObj());
    //        paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList).ToDbObj());

    //        paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList).ToDbObj());
    //        paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList).ToDbObj());

    //        if (!string.IsNullOrEmpty(form.GetElementValue("RadioInPerson")))
    //        {
    //            paramDict.Add("@RefField1",
    //                form.GetElementValue("RadioInPerson")
    //                    .Split(new string[] { Config.Separator }, StringSplitOptions.None)[1].CastObj<string>());
    //        }
    //        if (!string.IsNullOrEmpty(form.GetElementValue("ComplaintVm.ListDynamicDropDown[2].SelectedItemId")))
    //        {
    //            paramDict.Add("@RefField2",
    //                form.GetElementValue("ComplaintVm.ListDynamicDropDown[2].SelectedItemId")
    //                    .Split(new string[] { Config.Separator }, StringSplitOptions.None)[1].CastObj<string>());
    //        }
    //        paramDict.Add("@RefField3", form.GetElementValue("DynamicComplaintVmCnicInPerson").CastObj<string>());
    //        paramDict.Add("@RefField4", form.GetElementValue("DynamicComplaintVmMobileNoInPerson").CastObj<string>());
    //        paramDict.Add("@RefField5", form.GetElementValue("DynamicComplaintVmNameInPerson").CastObj<string>());


    //        // Add ComplaintSp
    //        DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
    //        string complaintIdStr = dt.Rows[0][1].ToString();
    //        int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
    //        DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
    //        Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());

    //        FileUploadHandler.UploadMultipleFiles(postedFiles, Config.AttachmentReferenceType.Add, complaintIdStr, complaintId, Config.TAG_COMPLAINT_ADD);

    //        DynamicFieldsHandler.SaveDyamicFieldsInDb(form, currentTab, complaintId);


    //        if (cm.Status == Config.CommandStatus.Success && currentTab == VmAddComplaint.TabComplaint) // send message on complaint launch
    //        {

    //            if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
    //            {
    //                TextMessageHandler.SendMessageOnComplaintLaunch(personMobileNo,
    //                    (int)campaignId, Convert.ToInt32(cm.Value.Split('-')[1]),
    //                    (int)categoryId, personName);
    //                string msgToText = null;

    //                TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint.GetByComplaintId(complaintId), msgToText);
    //            }
    //        }

    //        return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint action added successfully Id = " + campaignId + "-" + complaintId);

    //    }
    }
}
