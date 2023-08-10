using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Helper;
using AutoMapper;
using System.Threading;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Models.DB;
using PITB.CMS_Handlers.Complaint;
using PITB.CMS_Common;
using PITB.CMS_Handlers.DB.Repository;
using PITB.CMS_Handlers.DataTableJquery;
using PITB.CMS_Models.Custom;
using PITB.CMS_Models.View;

namespace PITB.CMS_Handlers.Business
{
    public class BlSchool
    {
        public static DbSchoolsMapping GetSchoolAgainstSchoolSearch(string searchStr)
        {
            string idStr = searchStr.Split(new string[] { Config.Separator }, StringSplitOptions.None)[0];
            DbSchoolsMapping dbSchoolsMapping = DbSchoolsMapping.GetById(Convert.ToInt32(idStr));
            /*if (dbSchoolsMapping.School_Type == (int) Config.SchoolType.Elementary)
            {
                dbSchoolsMapping.School_Type_Str = "Elementary";
            }
            else
            {
                dbSchoolsMapping.School_Type_Str = "Secondary";
            }*/
            return dbSchoolsMapping;
        }

        public static VmSEStakeholderComplaintDetail GetStakeholderComplaintDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            List<DbComplaint> listComplaint = DbComplaint.GetListByComplaintId(complaintId);
            List<DbDynamicComplaintFields> listDynamicFields = DbDynamicComplaintFields.GetByComplaintId(complaintId);
            DbDynamicComplaintFields teachingQualityField =
                listDynamicFields.Where(n => n.ControlId == 19).FirstOrDefault();
            if (teachingQualityField != null)
            {
                listDynamicFields.Remove(teachingQualityField);
            }
            listDynamicFields.RemoveAll(n => n.FieldName == "Tehsil" || n.FieldName == "Markaz");
            VmStakeholderComplaintDetail vmComplaintDetail = new VmStakeholderComplaintDetail();
            vmComplaintDetail.GetComplaintDetail(listComplaint.First(), listDynamicFields, detailType);

            vmComplaintDetail.VmStatusChange.vmFileModel = FileHandler.GetVmFileModel(complaintId, (int)Config.AttachmentReferenceType.Add, complaintId);
            vmComplaintDetail.hasStatusHistory = (StatusHandler.GetComplaintStatusChangeHistoryTableList(complaintId).Count > 0) ? true : false;
            vmComplaintDetail.hasTransferHistory = TransferHandler.GetTransferedHistoryStatus(complaintId);
            //vmComplaintDetail.currDetailType = detailType;

            if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStakeholderEscalationInDetail))
            {
                vmComplaintDetail.ListEscalationModel =
                    EscalationHandler.GetEscalationListOfComplaint(listComplaint.First());
            }
            //DbStatus.GetByCampaignIdAndCategoryId()

            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            vmComplaintDetail.vmPersonlInfo = Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetPersonInformationByPersonId((int)listComplaint.FirstOrDefault().Person_Id));
            vmComplaintDetail.Complaint_SubCategory = listComplaint.First().Complaint_SubCategory;
            vmComplaintDetail.currentStatusStr = Utility.GetAlteredStatus(vmComplaintDetail.currentStatusStr, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);

            VmSEStakeholderComplaintDetail vmSeComplaintDetail = new VmSEStakeholderComplaintDetail(vmComplaintDetail);
            // Assign SchoolHead
            DbSchoolEducationHeadMapping dbSeHeadMapping = DbSchoolEducationHeadMapping.GetByEmisCode(vmSeComplaintDetail.ListDynamicComplaintFields[0].FieldValue);
            if (dbSeHeadMapping != null)
            {
                vmSeComplaintDetail.SchoolHeadName = dbSeHeadMapping.School_Head_Name;
                vmSeComplaintDetail.SchoolHeadPhoneNo = dbSeHeadMapping.School_Head_PhoneNo;
            }
            else
            {
                vmSeComplaintDetail.SchoolHeadName = "None";
                vmSeComplaintDetail.SchoolHeadPhoneNo = "None";
            }
            // Complaint Responsible Person Information code
            //**********************************************//
            //List<DbUserWiseComplaints> listUserWiseComplaintsCurrent = DbUserWiseComplaints.GetUserWiseComplaints((int)vmSeComplaintDetail.Compaign_Id,
            //    new List<int?> { (int)vmSeComplaintDetail.ComplaintId }, (int)CMS.Config.ComplaintType.Complaint, (int)CMS.Config.StakeholderComplaintListingType.AssignedToMe);
            //if (listUserWiseComplaintsCurrent != null && listUserWiseComplaintsCurrent.Count > 0)
            {
                //DbUsers user = DbUsers.GetUser((int)listUserWiseComplaintsCurrent.First().User_Id);
                vmSeComplaintDetail.VmComplaintResponsiblePerson.Name = vmComplaintDetail.dbUserStakeholder.Name;//user.Name;
                vmSeComplaintDetail.VmComplaintResponsiblePerson.MobileNo = vmComplaintDetail.dbUserStakeholder.Phone;//user.Phone;
                vmSeComplaintDetail.VmComplaintResponsiblePerson.Designation = vmComplaintDetail.dbUserStakeholder.Designation;//user.Designation;
                vmSeComplaintDetail.VmComplaintResponsiblePerson.UserId = vmComplaintDetail.dbUserStakeholder.User_Id;//user.Id;
            }
            //*********************************************//
            return vmSeComplaintDetail;
        }

        public static List<VmServerSideDropDownList> GetServerSideSearchSchools(int districtId, int schoolCategory, string searchStr, int from, int to)
        {
            List<VmServerSideDropDownList> listDynamic = null;
            string searchQuery = Utility.GetDBSearchQueryOnIndividualWords("school_name", searchStr);
            if (searchQuery != "")
            {
                searchQuery = searchQuery + " OR ";
            }
            searchQuery = searchQuery + Utility.GetDBSearchQueryOnIndividualWords("school_emis_Code", searchStr);


            if (searchQuery != "")
            {
                searchQuery = searchQuery.Insert(0, "(");
                searchQuery = searchQuery.Insert(searchQuery.Length, ")");
                string fullQuery = @"select * FROM (
                                        select Id AS Id, ('['+ school_emis_code +'] ' + school_name) AS Text,ROW_NUMBER() OVER (ORDER BY school_name ) AS RowNum, count(*)  OVER() AS TotalRows 
                                        from dbo.Schools_Mapping where is_active = 1 and School_Category = "+ schoolCategory + " and System_District_Id=" + districtId + " and " + searchQuery + ") as a " +
                                        "where RowNum BETWEEN " + from + " AND " + to;
                listDynamic = DBHelper.GetDataTableByQueryString(fullQuery, null).ToList<VmServerSideDropDownList>();
            }
            else
            {
                listDynamic = new List<VmServerSideDropDownList>();
            }
            return listDynamic;
        }

        public static Config.CommandMessage AddComplaint(VmAddComplaint vm, bool isProfileEditing, bool isComplaintEditing)
        {
            #region Add Complaint Section

            DateTime nowDate = DateTime.Now;

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();

            VmPersonalInfo vmPersonalInfo = vm.PersonalInfoVm;
            VmComplaint vmComplaint = vm.ComplaintVm;
            VmSuggestion vmSuggestion = vm.SuggestionVm;
            VmInquiry vmInquiry = vm.InquiryVm;



            Dictionary<string, object> paramDict = new Dictionary<string, object>();


            //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

            // complaint info
            //int categoryId = -1;
            //int subCategoryId = -2;
            int categoryId = 0;
            int subCategoryId = 0;
            paramDict.Add("@Id", -1);
            paramDict.Add("@Person_Id", vmPersonalInfo.Person_id.ToDbObj());
            int schoolId = -1;
            DbSchoolsMapping dbSchools = null;

            if (!vmPersonalInfo.IsCnicPresent && !isProfileEditing)
            {
                decimal cnic = DbUniqueIncrementor.GetUniqueValue(Config.UniqueIncrementorTag.Cnic);
                string cnicStr = cnic.ToString();
                cnicStr = Utility.PaddLeft(cnicStr, Config.CnicLength, '0');
                vmPersonalInfo.Cnic_No = cnicStr;
            }

            switch (vm.currentComplaintTypeTab)
            {
                case VmAddComplaint.TabComplaint:
                    schoolId = Convert.ToInt32(vmComplaint.ListDynamicDropDownServerSide[0].SelectedItemId.Split(new string[] { Config.Separator }, StringSplitOptions.None)[0]);
                    dbSchools = DbSchoolsMapping.GetById(schoolId);
                    vmComplaint.Tehsil_Id = dbSchools.System_Tehsil_Id;
                    vmComplaint.UnionCouncil_Id = dbSchools.System_Markaz_Id;

                    paramDict.Add("@DepartmentId", vmComplaint.departmentId.ToDbObj());
                    paramDict.Add("@Complaint_Type", Config.ComplaintType.Complaint);
                    paramDict.Add("@Complaint_Category", vmComplaint.Complaint_Category.ToDbObj());
                    paramDict.Add("@Complaint_SubCategory", vmComplaint.Complaint_SubCategory.ToDbObj());
                    paramDict.Add("@Compaign_Id", vmComplaint.Compaign_Id.ToDbObj());
                    paramDict.Add("@Province_Id", vmComplaint.Province_Id.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", vmComplaint.District_Id.ToDbObj());
                    paramDict.Add("@Tehsil_Id", vmComplaint.Tehsil_Id.ToDbObj());
                    paramDict.Add("@UnionCouncil_Id", vmComplaint.UnionCouncil_Id ?? 0);
                    paramDict.Add("@Ward_Id", vmComplaint.Ward_Id ?? 0);
                    paramDict.Add("@Complaint_Remarks", vmComplaint.Complaint_Remarks.ToDbObj());
                    paramDict.Add("@Agent_Comments", vmComplaint.Agent_Comments.ToDbObj());

                    categoryId = Convert.ToInt32(vmComplaint.Complaint_Category);
                    subCategoryId = Convert.ToInt32(vmComplaint.Complaint_SubCategory);
                    break;
                case VmAddComplaint.TabSuggestion:
                    if (vmSuggestion.ListDynamicDropDownServerSide[0].SelectedItemId != null)
                    {
                        schoolId =
                            Convert.ToInt32(
                                vmSuggestion.ListDynamicDropDownServerSide[0].SelectedItemId.Split(
                                    new string[] { Config.Separator }, StringSplitOptions.None)[0]);

                        dbSchools = DbSchoolsMapping.GetById(schoolId);

                        vmSuggestion.Tehsil_Id = dbSchools.System_Tehsil_Id;
                        vmSuggestion.UnionCouncil_Id = dbSchools.System_Markaz_Id;
                    }
                    paramDict.Add("@DepartmentId", vmSuggestion.departmentId.ToDbObj());
                    paramDict.Add("@Complaint_Type", Config.ComplaintType.Suggestion);
                    paramDict.Add("@Complaint_Category", vmSuggestion.Complaint_Category.ToDbObj());
                    paramDict.Add("@Complaint_SubCategory", vmSuggestion.Complaint_SubCategory.ToDbObj());
                    paramDict.Add("@Compaign_Id", vmComplaint.Compaign_Id.ToDbObj());
                    paramDict.Add("@Province_Id", vmSuggestion.Province_Id.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)vmSuggestion.District_Id).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", vmSuggestion.District_Id.ToDbObj());
                    paramDict.Add("@Tehsil_Id", vmSuggestion.Tehsil_Id.ToDbObj());
                    paramDict.Add("@UnionCouncil_Id", vmSuggestion.UnionCouncil_Id ?? 0);
                    paramDict.Add("@Ward_Id", vmSuggestion.Ward_Id ?? 0);
                    paramDict.Add("@Complaint_Remarks", vmSuggestion.Complaint_Remarks.ToDbObj());
                    paramDict.Add("@Agent_Comments", vmSuggestion.Agent_Comments.ToDbObj());

                    categoryId = Convert.ToInt32(vmSuggestion.Complaint_Category);
                    subCategoryId = Convert.ToInt32(vmSuggestion.Complaint_SubCategory);
                    break;
                case VmAddComplaint.TabInquiryVm:
                    if (vmInquiry.ListDynamicDropDownServerSide[0].SelectedItemId != null)
                    {
                        schoolId =
                            Convert.ToInt32(
                                vmInquiry.ListDynamicDropDownServerSide[0].SelectedItemId.Split(
                                    new string[] { Config.Separator }, StringSplitOptions.None)[0]);

                        dbSchools = DbSchoolsMapping.GetById(schoolId);
                        vmInquiry.Tehsil_Id = dbSchools.System_Tehsil_Id;
                        vmInquiry.UnionCouncil_Id = dbSchools.System_Markaz_Id;
                    }


                    paramDict.Add("@DepartmentId", vmInquiry.departmentId.ToDbObj());
                    paramDict.Add("@Complaint_Type", Config.ComplaintType.Inquiry);
                    paramDict.Add("@Complaint_Category", vmInquiry.Complaint_Category.ToDbObj());
                    paramDict.Add("@Complaint_SubCategory", vmInquiry.Complaint_SubCategory.ToDbObj());
                    paramDict.Add("@Compaign_Id", vmComplaint.Compaign_Id.ToDbObj());
                    paramDict.Add("@Province_Id", vmInquiry.Province_Id.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)vmInquiry.District_Id).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", vmInquiry.District_Id.ToDbObj());
                    paramDict.Add("@Tehsil_Id", vmInquiry.Tehsil_Id.ToDbObj());
                    paramDict.Add("@UnionCouncil_Id", vmInquiry.UnionCouncil_Id ?? 0);
                    paramDict.Add("@Ward_Id", vmInquiry.Ward_Id ?? 0);
                    paramDict.Add("@Complaint_Remarks", vmInquiry.Complaint_Remarks.ToDbObj());
                    paramDict.Add("@Agent_Comments", vmInquiry.Agent_Comments.ToDbObj());

                    categoryId = Convert.ToInt32(vmInquiry.Complaint_Category);
                    subCategoryId = Convert.ToInt32(vmInquiry.Complaint_SubCategory);
                    break;
            }


            paramDict.Add("@Agent_Id", cmsCookie.UserId.ToDbObj());
            paramDict.Add("@Complaint_Address", vmComplaint.Complaint_Address.ToDbObj());
            paramDict.Add("@Business_Address", vmComplaint.Business_Address.ToDbObj());

            paramDict.Add("@Complaint_Status_Id", (isComplaintEditing) ? vmComplaint.Complaint_Status_Id.ToDbObj() : Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
            paramDict.Add("@Created_Date", nowDate.ToDbObj());
            paramDict.Add("@Created_By", cmsCookie.UserId.ToDbObj());
            paramDict.Add("@Complaint_Assigned_Date", (null as object).ToDbObj());
            paramDict.Add("@Completed_Date", (null as object).ToDbObj());
            //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
            paramDict.Add("@Updated_By", cmsCookie.UserId.ToDbObj());
            paramDict.Add("@Is_Deleted", false);
            paramDict.Add("@Date_Deleted", (null as object).ToDbObj());
            paramDict.Add("@Deleted_By", (null as object).ToDbObj());
            paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.Agent).ToDbObj());

            paramDict.Add("@IsComplaintEditing", isComplaintEditing);



            //Personal Information
            paramDict.Add("@p_Person_id", vmPersonalInfo.Person_id.ToDbObj());
            paramDict.Add("@Person_Name", vmPersonalInfo.Person_Name.ToDbObj());
            paramDict.Add("@Person_Father_Name", vmPersonalInfo.Person_Father_Name.ToDbObj());
            paramDict.Add("@Is_Cnic_Present", vmPersonalInfo.IsCnicPresent.ToDbObj());
            paramDict.Add("@Cnic_No", vmPersonalInfo.Cnic_No.ToDbObj());
            paramDict.Add("@Gender", vmPersonalInfo.Gender.ToDbObj());
            paramDict.Add("@Mobile_No", vmPersonalInfo.Mobile_No.ToDbObj());
            paramDict.Add("@Secondary_Mobile_No", vmPersonalInfo.Secondary_Mobile_No.ToDbObj());
            paramDict.Add("@LandLine_No", vmPersonalInfo.LandLine_No.ToDbObj());
            paramDict.Add("@Person_Address", vmPersonalInfo.Person_Address.ToDbObj());
            paramDict.Add("@Email", vmPersonalInfo.Email.ToDbObj());
            paramDict.Add("@Nearest_Place", vmPersonalInfo.Nearest_Place.ToDbObj());
            paramDict.Add("@p_Province_Id", vmPersonalInfo.Province_Id.ToDbObj());
            paramDict.Add("@p_Division_Id", vmPersonalInfo.Division_Id.ToDbObj());
            paramDict.Add("@p_District_Id", vmPersonalInfo.District_Id.ToDbObj());
            paramDict.Add("@p_Tehsil_Id", vmPersonalInfo.Tehsil_Id.ToDbObj());
            paramDict.Add("@p_Town_Id", vmPersonalInfo.Town_Id.ToDbObj());
            paramDict.Add("@p_Uc_Id", vmPersonalInfo.Uc_Id ?? 0);
            paramDict.Add("@p_Created_By", cmsCookie.UserId);
            paramDict.Add("@p_Updated_By", cmsCookie.UserId);

            paramDict.Add("@IsProfileEditing", isProfileEditing);


            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;
            //int categoryId = -1;
            //Config.CategoryType cateogryType = Config.CategoryType.Main;

            //AssignmentMatrix
            //List<AssignmentModel> assignmentModelList = null;
            //if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint) // when there is complaint populate assignment matrix
            //{
            //    subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)vmComplaint.Complaint_SubCategory);
            //    if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
            //    {
            //        catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)vmComplaint.Complaint_Category);
            //        //cateogryType = Config.CategoryType.Main;
            //    }
            //    else
            //    {
            //        catRetainingHours = (float)subcatRetainingHours;
            //        //cateogryType = Config.CategoryType.Sub;
            //    }
            //    assignmentModelList = AssignmentHandler.GetAssignmentModel(nowDate,
            //        DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory), catRetainingHours);
            //}
            //else
            //{
            //    assignmentModelList = new List<AssignmentModel>();
            //}

            int? userCategoryId1 = null;
            int? userCategoryId2 = null;
            int? userCategoryId3 = null;

            List<AssignmentModel> assignmentModelList = null;
            if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint)
            {
                string assignmentModelTag = "SchoolCategory::" + dbSchools.School_Category;
                assignmentModelList =
                    AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory((int)vmComplaint.Compaign_Id,
                        (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, true, dbSchools.School_Type, null, assignmentModelTag);

                //------ Custom Code -------

                List<DbUsers> listDbUsers = UsersHandler.GetUsersHierarchyMapping(Convert.ToInt32(vmComplaint.Compaign_Id));
                Config.Hierarchy hierarchyId = (Config.Hierarchy)assignmentModelList[0].SrcId;
                int? userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);

                OriginHierarchy originHierarchy = EvaluateAssignmentMartix(vmComplaint, listDbUsers, assignmentModelList, dbSchools, hierarchyId, userHierarchyVal, ref userCategoryId1, ref userCategoryId2, ref userCategoryId3, 0, null);
                paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
                paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
                paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
                paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
                paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);
                //--------------------------
            }
            else if (dbSchools != null)
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
            }




            //------ Commenting Code --------
            /*for (int i = 0; i < 10; i++)
            {
                if (dbSchools!=null && i < assignmentModelList.Count)
                {
                    if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Secondary) // no male female diff
                    {
                        if (assignmentModelList[i].SrcId > (int) Config.Hierarchy.District) // means tehsil or uc
                        {
                            assignmentModelList[i].SrcId = (int) Config.Hierarchy.District;
                            assignmentModelList[i].UserSrcId = 10;
                        }

                        if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId <= 20 || assignmentModelList[i].UserSrcId == null))
                            // if under district and under lower district
                        {
                            userCategoryId1 = (int) Config.SchoolType.Secondary;
                            userCategoryId2 = null;
                            //userCategoryId2 = (bool)dbSchools.System_School_Gender
                            //    ? (int)Config.Gender.Male
                            //    : (int)Config.Gender.Female;
                        }
                    }
                    else if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Primary)
                    {
                        if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
                            // if under district and under lower district
                        {
                            userCategoryId1 = (int) Config.SchoolType.Primary;
                            userCategoryId2 = (bool) dbSchools.System_School_Gender
                                ? (int) Config.Gender.Male
                                : (int) Config.Gender.Female;
                        }

                    }
                    else if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Elementary)
                    {
                        if (assignmentModelList[i].SrcId > (int)Config.Hierarchy.Tehsil) // means uc
                        {
                            assignmentModelList[i].SrcId = (int)Config.Hierarchy.Tehsil;
                            //assignmentModelList[i].UserSrcId = 10;
                        }

                        if (assignmentModelList[i].SrcId >= (int)Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
                        // if under district and under lower district
                        {
                            userCategoryId1 = (int)Config.SchoolType.Elementary;
                            userCategoryId2 = (bool)dbSchools.System_School_Gender
                                ? (int)Config.Gender.Male
                                : (int)Config.Gender.Female;
                            //userCategoryId2 = (bool)dbSchools.System_School_Gender
                            //    ? (int)Config.Gender.Male
                            //    : (int)Config.Gender.Female;
                        }

                    }





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
            }*/
            //--------------------------

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



            paramDict.Add("@MaxLevel", assignmentModelList.Count);

            paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList));
            paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList));

            paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList));
            paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList));

            paramDict.Add("@MinUserSrcId", AssignmentHandler.GetMinUserSrcId(assignmentModelList));
            paramDict.Add("@MaxUserSrcId", AssignmentHandler.GetMaxUserSrcId(assignmentModelList));

            paramDict.Add("@TableRefId", (int)Config.TableRef.SchoolEducation);
            paramDict.Add("@TableRowRefId", schoolId);

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
                paramDict.Add("@RefField7_Int", dbSchools.School_Category);
                paramDict.Add("@RefField7", dbSchools.School_Category == 2 ? "private" : "public");
                paramDict.Add("@RefField1_Int", dbSchools.School_Category == 2 ? dbSchools.PMIU_School_Id : null); 
            }

            #endregion
            DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
            string complaintStr = dt.Rows[0][1].ToString();
            int complaintId = Convert.ToInt32(complaintStr.Split('-')[1]);
            int campaignId = Convert.ToInt32(complaintStr.Split('-')[0]);

            Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
            if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.ComplaintVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }
            else if (vm.currentComplaintTypeTab == VmAddComplaint.TabSuggestion)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.SuggestionVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }
            else if (vm.currentComplaintTypeTab == VmAddComplaint.TabInquiryVm)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.InquiryVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }

            if (cm.Status == Config.CommandStatus.Success && vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint) // send message on complaint launch
            {
                if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
                {
                    //TextMessageHandler.SendMessageOnComplaintLaunch(vmPersonalInfo.Mobile_No,
                    //    (int)vmComplaint.Compaign_Id, Convert.ToInt32(cm.Value.Split('-')[1]),
                    //    (int)vmComplaint.Complaint_Category);

                    new Thread(delegate()
                    {
                        SendMessageToComplainant(vmPersonalInfo.Mobile_No, complaintStr, campaignId, (int)vmComplaint.Complaint_Category, vmPersonalInfo.Person_Name, vmComplaint);
                    }).Start();

                    new Thread(delegate()
                    {
                        SendMessageToStakeholder(DbComplaint.GetByComplaintId(complaintId), dbSchools);
                    }).Start();
                }
            }

            //db.DbDynamicComplaintFields.Add();
            //SendMessage(Convert.ToInt32(cm.Value.Split('-')[1]), (int)Config.ComplaintStatus.PendingFresh);
            return cm;
        }


        public static void AssignValuesAgainstAssignmentMatrix(List<DbUsers> listDbUsers,
            List<AssignmentModel> assignmentModelList, DbSchoolsMapping dbSchools, ref Config.Hierarchy hierarchyId,
            ref int? userHierarchyId, ref int? userCategoryId1, ref int? userCategoryId2, ref int? userCategoryId3)
        {
            for (int i = 0; i < assignmentModelList.Count; i++)
            {
                if (dbSchools != null)
                {

                    //--------------------- New Secondary Code ---------------------
                    if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Secondary)
                    // no male female diff
                    {
                        if (assignmentModelList[i].SrcId == (int)Config.Hierarchy.Tehsil) // for tehsil route to district for uc dont due to headmaster
                        {
                            assignmentModelList[i].SrcId = (int)Config.Hierarchy.District;
                            assignmentModelList[i].UserSrcId = 10;
                            userHierarchyId = 10;
                            hierarchyId = Config.Hierarchy.District;
                        }
                        if (assignmentModelList[i].SrcId == (int)Config.Hierarchy.UnionCouncil) // for uc assign headmaster
                        {
                            userCategoryId1 = (int)Config.SchoolType.Secondary;
                            userCategoryId2 = Convert.ToInt32(dbSchools.school_emis_code);
                            //userCategoryId3 = (bool)dbSchools.System_School_Gender ? (int)Config.Gender.Male : (int)Config.Gender.Female;
                        }

                        else if (assignmentModelList[i].SrcId >= (int)Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null)) // if under district and under lower district
                        {
                            userCategoryId1 = (int)Config.SchoolType.Secondary;
                            userCategoryId2 = null;
                            //userCategoryId2 = (bool)dbSchools.System_School_Gender
                            //    ? (int)Config.Gender.Male
                            //    : (int)Config.Gender.Female;
                        }
                    }
                    //------------------- End -----------------------

                    //--------------------- Old Secondary Code ---------------------
                    /*if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Secondary)
                        // no male female diff
                    {
                        if (assignmentModelList[i].SrcId > (int) Config.Hierarchy.District) // means tehsil or uc
                        {
                            assignmentModelList[i].SrcId = (int) Config.Hierarchy.District;
                            assignmentModelList[i].UserSrcId = 10;
                        }

                        if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId <= 20 || assignmentModelList[i].UserSrcId == null))
                            // if under district and under lower district
                        {
                            userCategoryId1 = (int) Config.SchoolType.Secondary;
                            userCategoryId2 = null;
                            //userCategoryId2 = (bool)dbSchools.System_School_Gender
                            //    ? (int)Config.Gender.Male
                            //    : (int)Config.Gender.Female;
                        }
                    }*/
                    //------------------- End -----------------------


                    //------------------- New Secondary Code ---------------
                    /*if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Secondary)
                    {
                        if (callCount == 0)
                        {
                            
                        }
                    }*/
                    //------------------- End New Secondary Code -----------
                    else if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Primary)
                    {
                        if (assignmentModelList[i].SrcId >= (int)Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
                        // if under district and under lower district
                        {
                            userCategoryId1 = (int)Config.SchoolType.Primary;
                            userCategoryId2 = (bool)dbSchools.System_School_Gender
                                ? (int)Config.Gender.Male
                                : (int)Config.Gender.Female;
                        }

                    }
                    else if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Elementary)
                    {
                        if (assignmentModelList[i].SrcId >= (int)Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
                        // if under district and under lower district
                        {
                            userCategoryId1 = (int)Config.SchoolType.Elementary;
                            userCategoryId2 = (bool)dbSchools.System_School_Gender
                                ? (int)Config.Gender.Male
                                : (int)Config.Gender.Female;
                        }

                    }
                    /*else if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Elementary)
                    {
                        if (assignmentModelList[i].SrcId > (int) Config.Hierarchy.Tehsil) // means uc
                        {
                            assignmentModelList[i].SrcId = (int) Config.Hierarchy.Tehsil;
                            //assignmentModelList[i].UserSrcId = 10;
                        }

                        if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
                            (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
                            // if under district and under lower district
                        {
                            userCategoryId1 = (int) Config.SchoolType.Elementary;
                            userCategoryId2 = (bool) dbSchools.System_School_Gender
                                ? (int) Config.Gender.Male
                                : (int) Config.Gender.Female;
                            //userCategoryId2 = (bool)dbSchools.System_School_Gender
                            //    ? (int)Config.Gender.Male
                            //    : (int)Config.Gender.Female;
                        }

                    }*/

                }
            }
        }

        public static ReEvaluationAssignmentModel ReEvaluateEscallation(DbComplaint dbComplaint, DbSchoolsMapping dbSchool)
        {
            ReEvaluationAssignmentModel reEvaluationAssignmentModel = new ReEvaluationAssignmentModel();

            Mapper.CreateMap<DbComplaint, DbComplaint>();
            reEvaluationAssignmentModel.PrevDbComplaint = Mapper.Map<DbComplaint>(dbComplaint);

            List<AssignmentModel> assignmentModelList = null;
            assignmentModelList =
                    AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory((int)dbComplaint.Compaign_Id,
                        (int)dbComplaint.Complaint_Category, (int)dbComplaint.Complaint_SubCategory, true, dbSchool.School_Type, null);

            //------ Custom Code -------

            List<DbUsers> listDbUsers = UsersHandler.GetUsersHierarchyMapping(Convert.ToInt32(dbComplaint.Compaign_Id));
            Config.Hierarchy hierarchyId = (Config.Hierarchy)assignmentModelList[0].SrcId;
            int? userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);


            VmComplaint vmComplaint = new VmComplaint();
            vmComplaint.Province_Id = dbComplaint.Province_Id;
            vmComplaint.Division_Id = dbComplaint.Division_Id;
            vmComplaint.District_Id = dbComplaint.District_Id;
            vmComplaint.Tehsil_Id = dbComplaint.Tehsil_Id;
            vmComplaint.UnionCouncil_Id = dbComplaint.UnionCouncil_Id;


            int? userCategoryId1 = null;
            int? userCategoryId2 = null;
            int? userCategoryId3 = null;

            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            OriginHierarchy originHierarchy = EvaluateAssignmentMartix(vmComplaint, listDbUsers, assignmentModelList, dbSchool, hierarchyId, userHierarchyVal, ref userCategoryId1, ref userCategoryId2, ref userCategoryId3, 0, null);
            dbComplaint.Origin_HierarchyId = originHierarchy.OriginHierarchyId;
            dbComplaint.Origin_UserHierarchyId = originHierarchy.OriginUserHierarchyId;
            dbComplaint.Origin_UserCategoryId1 = originHierarchy.OriginUserCategoryId1;
            dbComplaint.Origin_UserCategoryId2 = originHierarchy.OriginUserCategoryId2;
            dbComplaint.Is_AssignedToOrigin = originHierarchy.IsAssignedToOrigin;

            dbComplaint.MaxLevel = assignmentModelList.Count;

            dbComplaint.MinSrcId = AssignmentHandler.GetMinSrcId(assignmentModelList);
            dbComplaint.MaxSrcId = AssignmentHandler.GetMaxSrcId(assignmentModelList);

            dbComplaint.MinUserSrcId = AssignmentHandler.GetMinUserSrcId(assignmentModelList);
            dbComplaint.MaxUserSrcId = AssignmentHandler.GetMaxUserSrcId(assignmentModelList);

            dbComplaint.MinSrcIdDate = AssignmentHandler.GetMinDate(assignmentModelList);
            dbComplaint.MaxSrcIdDate = AssignmentHandler.GetMaxDate(assignmentModelList);

            dbComplaint.UserCategoryId1 = userCategoryId1;
            dbComplaint.UserCategoryId2 = userCategoryId2;


            for (int i = 0; i < 10; i++)
            {
                if (i < assignmentModelList.Count)
                {
                    Utility.SetPropertyThroughReflection(dbComplaint, "Dt" + (i + 1), assignmentModelList[i].Dt);
                    Utility.SetPropertyThroughReflection(dbComplaint, "SrcId" + (i + 1), assignmentModelList[i].SrcId);
                    Utility.SetPropertyThroughReflection(dbComplaint, "UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId);
                }
                else
                {
                    Utility.SetPropertyThroughReflection(dbComplaint, "Dt" + (i + 1), (DateTime?)null);
                    Utility.SetPropertyThroughReflection(dbComplaint, "SrcId" + (i + 1), (int?)null);
                    Utility.SetPropertyThroughReflection(dbComplaint, "UserSrcId" + (i + 1), (int?)null);
                }

            }

            Mapper.CreateMap<DbComplaint, DbComplaint>();
            reEvaluationAssignmentModel.CurrDbComplaint = Mapper.Map<DbComplaint>(dbComplaint);

            return reEvaluationAssignmentModel;

            //paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
            //paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
            //paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
            //paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
            //paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);
        }

        public static OriginHierarchy EvaluateAssignmentMartix(VmComplaint vmComplaint, List<DbUsers> listDbUsers, List<AssignmentModel> assignmentModelList, DbSchoolsMapping dbSchools, Config.Hierarchy hierarchyId, int? userHierarchyId, ref int? userCategoryId1, ref int? userCategoryId2, ref int? userCategoryId3, int callCount, OriginHierarchy originHierarchy)
        {
            bool hasResponsibleUser = false;

            AssignValuesAgainstAssignmentMatrix(listDbUsers, assignmentModelList, dbSchools, ref hierarchyId,
               ref userHierarchyId, ref userCategoryId1, ref userCategoryId2, ref userCategoryId3);
            //List<DbUsers> listDbUsers = UsersHandler.GetUsersHierarchyMapping(Convert.ToInt32(vmComplaint.Compaign_Id));

            //List<AssignmentModel> assignmentModelList = null;

            //-------- Start Commenting ------------------------
            //for (int i = 0; i < assignmentModelList.Count; i++)
            //{
            //    if (dbSchools != null)
            //    {

            //        //--------------------- New Secondary Code ---------------------
            //        if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Secondary)
            //        // no male female diff
            //        {
            //            if (assignmentModelList[i].SrcId == (int)Config.Hierarchy.Tehsil) // for tehsil route to district for uc dont due to headmaster
            //            {
            //                assignmentModelList[i].SrcId = (int)Config.Hierarchy.District;
            //                assignmentModelList[i].UserSrcId = 10;
            //                userHierarchyId = 10;
            //                hierarchyId = Config.Hierarchy.District;
            //            }
            //            if (assignmentModelList[i].SrcId == (int) Config.Hierarchy.UnionCouncil) // for uc assign headmaster
            //            {
            //                userCategoryId1 = (int)Config.SchoolType.Secondary;
            //                userCategoryId2 = Convert.ToInt32(dbSchools.school_emis_code);
            //                //userCategoryId3 = (bool)dbSchools.System_School_Gender ? (int)Config.Gender.Male : (int)Config.Gender.Female;
            //            }

            //            else if (assignmentModelList[i].SrcId >= (int)Config.Hierarchy.District &&
            //                (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null)) // if under district and under lower district
            //            {
            //                userCategoryId1 = (int)Config.SchoolType.Secondary;
            //                userCategoryId2 = null;
            //                //userCategoryId2 = (bool)dbSchools.System_School_Gender
            //                //    ? (int)Config.Gender.Male
            //                //    : (int)Config.Gender.Female;
            //            }
            //        }
            //        //------------------- End -----------------------

            //        //--------------------- Old Secondary Code ---------------------
            //        /*if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Secondary)
            //            // no male female diff
            //        {
            //            if (assignmentModelList[i].SrcId > (int) Config.Hierarchy.District) // means tehsil or uc
            //            {
            //                assignmentModelList[i].SrcId = (int) Config.Hierarchy.District;
            //                assignmentModelList[i].UserSrcId = 10;
            //            }

            //            if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
            //                (assignmentModelList[i].UserSrcId <= 20 || assignmentModelList[i].UserSrcId == null))
            //                // if under district and under lower district
            //            {
            //                userCategoryId1 = (int) Config.SchoolType.Secondary;
            //                userCategoryId2 = null;
            //                //userCategoryId2 = (bool)dbSchools.System_School_Gender
            //                //    ? (int)Config.Gender.Male
            //                //    : (int)Config.Gender.Female;
            //            }
            //        }*/
            //        //------------------- End -----------------------


            //        //------------------- New Secondary Code ---------------
            //        /*if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Secondary)
            //        {
            //            if (callCount == 0)
            //            {

            //            }
            //        }*/
            //        //------------------- End New Secondary Code -----------
            //        else if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Primary)
            //        {
            //            if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
            //                (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
            //                // if under district and under lower district
            //            {
            //                userCategoryId1 = (int) Config.SchoolType.Primary;
            //                userCategoryId2 = (bool) dbSchools.System_School_Gender
            //                    ? (int) Config.Gender.Male
            //                    : (int) Config.Gender.Female;
            //            }

            //        }
            //        else if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Elementary)
            //        {
            //            if (assignmentModelList[i].SrcId >= (int)Config.Hierarchy.District &&
            //                (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
            //            // if under district and under lower district
            //            {
            //                userCategoryId1 = (int)Config.SchoolType.Elementary;
            //                userCategoryId2 = (bool)dbSchools.System_School_Gender
            //                    ? (int)Config.Gender.Male
            //                    : (int)Config.Gender.Female;
            //            }

            //        }
            //        /*else if (Convert.ToInt32(dbSchools.School_Type) == (int) Config.SchoolType.Elementary)
            //        {
            //            if (assignmentModelList[i].SrcId > (int) Config.Hierarchy.Tehsil) // means uc
            //            {
            //                assignmentModelList[i].SrcId = (int) Config.Hierarchy.Tehsil;
            //                //assignmentModelList[i].UserSrcId = 10;
            //            }

            //            if (assignmentModelList[i].SrcId >= (int) Config.Hierarchy.District &&
            //                (assignmentModelList[i].UserSrcId < 20 || assignmentModelList[i].UserSrcId == null))
            //                // if under district and under lower district
            //            {
            //                userCategoryId1 = (int) Config.SchoolType.Elementary;
            //                userCategoryId2 = (bool) dbSchools.System_School_Gender
            //                    ? (int) Config.Gender.Male
            //                    : (int) Config.Gender.Female;
            //                //userCategoryId2 = (bool)dbSchools.System_School_Gender
            //                //    ? (int)Config.Gender.Male
            //                //    : (int)Config.Gender.Female;
            //            }

            //        }*/

            //    }
            //}
            //----------- End Commenting ---------------------------------------
            List<Tuple<Config.Hierarchy?, int>> listHierarchyMapping = new List<Tuple<Config.Hierarchy?, int>>();
            listHierarchyMapping.Add(new Tuple<Config.Hierarchy?, int>(Config.Hierarchy.Province, Convert.ToInt32(vmComplaint.Province_Id)));
            listHierarchyMapping.Add(new Tuple<Config.Hierarchy?, int>(Config.Hierarchy.Division, Convert.ToInt32(vmComplaint.Division_Id)));
            listHierarchyMapping.Add(new Tuple<Config.Hierarchy?, int>(Config.Hierarchy.District, Convert.ToInt32(vmComplaint.District_Id)));
            listHierarchyMapping.Add(new Tuple<Config.Hierarchy?, int>(Config.Hierarchy.Tehsil, Convert.ToInt32(vmComplaint.Tehsil_Id)));
            listHierarchyMapping.Add(new Tuple<Config.Hierarchy?, int>(Config.Hierarchy.UnionCouncil, Convert.ToInt32(vmComplaint.UnionCouncil_Id)));
            listHierarchyMapping.Add(new Tuple<Config.Hierarchy?, int>(Config.Hierarchy.Ward, Convert.ToInt32(vmComplaint.Ward_Id)));

            //Config.Hierarchy hierarchyId = (Config.Hierarchy)assignmentModelList[0].SrcId;
            int hierarchyVal = vmComplaint.GetHierarchyVal(hierarchyId);
            //int? userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);

            List<DbUsers> listCurrentDbUsers = UsersHandler.GetUsersPresentForCurrentHierarchy2(listDbUsers,
                hierarchyId, vmComplaint.GetHierarchyVal(hierarchyId),
                userHierarchyId, userCategoryId1, userCategoryId2);

            if (callCount == 0 && originHierarchy == null)
            {
                originHierarchy = new OriginHierarchy();
                originHierarchy.OriginHierarchyId = assignmentModelList[0].SrcId;
                originHierarchy.OriginUserHierarchyId = assignmentModelList[0].UserSrcId;
                originHierarchy.OriginUserCategoryId1 = userCategoryId1;
                originHierarchy.OriginUserCategoryId2 = userCategoryId2;
                if (listCurrentDbUsers != null)
                {
                    if (listCurrentDbUsers.Count == 0) // if origin is not present
                    {
                        originHierarchy.IsAssignedToOrigin = false;
                    }
                    else // origin present
                    {
                        originHierarchy.IsAssignedToOrigin = true;
                    }
                }
                else // origin present
                {
                    originHierarchy.IsAssignedToOrigin = false;
                }

                //originHierarchy.ListAssignmentMatrixInitial = new List<AssignmentModel>();
                originHierarchy.DeepCopyAssignMatrix(assignmentModelList);
                if (listCurrentDbUsers != null && listCurrentDbUsers.Count > 0)
                {
                    return originHierarchy;
                }
            }

            //if (listCurrentDbUsers != null && listCurrentDbUsers.Count == 0) // if current user has not found
            //{
            //    /*
            //       DbUsers dbUser = listCurrentDbUsers[0];
            //       int hierarchyIdToAssign = Convert.ToInt32(dbUser.Hierarchy_Id);


            //       for (int i = 0; i < assignmentModelList.Count; i++)
            //       {
            //           if (i == 0)
            //           {
            //               assignmentModelList[i].UserSrcId = (dbUser.User_Hierarchy_Id == 0) ? null : dbUser.User_Hierarchy_Id;
            //               assignmentModelList[i].SrcId = hierarchyIdToAssign;
            //           }
            //           else
            //           {
            //               assignmentModelList[i].SrcId = assignmentModelList[i].SrcId - 1;
            //           }
            //           //hierarchyIdToAssign--;
            //       }
            //   */

            //    if (callCount == 0)
            //    {
            //        originHierarchy.IsAssignedToOrigin = false;
            //    }

            //    return originHierarchy;
            //}
            if (listCurrentDbUsers == null || listCurrentDbUsers.Count == 0) // if no user has found
            {
                listCurrentDbUsers = UsersHandler.GetUsersPresentForUpperUserHierarchy(listHierarchyMapping, listDbUsers,
                    hierarchyId,
                    userHierarchyId);

                if (listCurrentDbUsers.Count > 0) // If Users Have Found
                {
                    DbUsers dbUser = listCurrentDbUsers[0];
                    Config.Hierarchy hierarchyIdToAssign = (Config.Hierarchy)dbUser.Hierarchy_Id;
                    //int hierarchyIdToAssign = Convert.ToInt32(dbUser.Hierarchy_Id);

                    int? prevSrcId = null;
                    int? prevUserSrcId = null;
                    bool canBreakLoop = false;

                    for (int i = 0; i < assignmentModelList.Count && !canBreakLoop; i++)
                    {
                        if (i == 0)
                        {
                            assignmentModelList[i].UserSrcId = (dbUser.User_Hierarchy_Id == 0)
                                ? null
                                : dbUser.User_Hierarchy_Id;
                            assignmentModelList[i].SrcId = (int)hierarchyIdToAssign;

                            //initialSrcId = assignmentModelList[i].SrcId;
                            //initialUserSrcId = assignmentModelList[i].UserSrcId;
                        }
                        else // assignment Matrix Second Value
                        {
                            if (assignmentModelList[i].SrcId > prevSrcId) // eg if next escallation is lesser than current one..... eg after tehsil there is UC
                            {
                                assignmentModelList.RemoveAt(i);
                                i--;
                                canBreakLoop = true;
                            }
                            else if (assignmentModelList[i].SrcId == prevSrcId && assignmentModelList[i].UserSrcId <= prevUserSrcId)
                            {
                                assignmentModelList.RemoveAt(i);
                                i--;
                                canBreakLoop = true;
                            }
                            //assignmentModelList[i].SrcId = assignmentModelList[i].SrcId - 1;
                        }
                        if (!canBreakLoop)
                        {
                            prevSrcId = assignmentModelList[i].SrcId;
                            prevUserSrcId = assignmentModelList[i].UserSrcId;
                            //hierarchyIdToAssign--;
                        }
                    }

                    //DbUsers dbUser = listCurrentDbUsers[0];
                    //Config.Hierarchy hierarchyIdToAssign = (Config.Hierarchy)dbUser.Hierarchy_Id;
                    int? userHierarchyVal = dbUser.User_Hierarchy_Id;
                    userCategoryId1 = null;
                    userCategoryId2 = null;
                    //Convert.ToInt32(assignmentModelList[0].UserSrcId);
                    callCount++;
                    EvaluateAssignmentMartix(vmComplaint, listDbUsers, assignmentModelList, dbSchools,
                        hierarchyIdToAssign, userHierarchyVal, ref userCategoryId1,
                        ref userCategoryId2, ref userCategoryId3, callCount, originHierarchy);
                }
                else if (listCurrentDbUsers.Count == 0) // if no user has found in upper hierarchy then assign orignal assignment matrix
                {
                    // Assign initial Assignment matrix to the returning one
                    for (int i = 0; i < originHierarchy.ListAssignmentMatrixInitial.Count; i++)
                    {
                        assignmentModelList[i].SrcId = originHierarchy.ListAssignmentMatrixInitial[i].SrcId;
                        assignmentModelList[i].Dt = originHierarchy.ListAssignmentMatrixInitial[i].Dt;
                        assignmentModelList[i].UserSrcId = originHierarchy.ListAssignmentMatrixInitial[i].UserSrcId;
                    }
                    userCategoryId1 = originHierarchy.OriginUserCategoryId1;
                    userCategoryId2 = originHierarchy.OriginUserCategoryId2;
                }

            }

            return originHierarchy;

        }



        private static void SendMessageToComplainant(string phoneNo, string complaintId, int campaignId, int categoryId, string personName, VmComplaint vmComplaint)
        {
            int selectedComplaintSrc = Convert.ToInt32(vmComplaint.ListDynamicDropDown[0].SelectedItemId.Split(new string[] { Config.Separator }, StringSplitOptions.None)[0]);

            if (selectedComplaintSrc == 6123) // Rabta line
            {
                DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
                DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
                //string sms = "Your complaint has been successfully registered in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
                //             dbComplaintType.Name + "\nComplaint Id : " + campaignId + "-" + complaintId;

                string sms = "Dear " + personName + ", We have received your complaint for '" + dbComplaintType.Name + "'. The complaint no. is '" + complaintId + "' for your reference. Concerned authority will respond to your complaint.\n\nThank you for calling Cheif Minister's Complaint Center, 0800-02345";
                TextMessageHandler.SendMessageToPhoneNo(phoneNo, sms);
            }

                //string msg =
            //    "Your feedback for School Education Department has been successfully registered and shared with relevant official. \n";
            //msg = msg + "Complaint ID: " + complaintId + "";
            else
            {
                string msg = "Thank you for contacting the School Education Helpline (042-111-11-20-20). \n" +

                             "Your feedback has been registered and shared with relevant officials. \n" +

                             "Complaint #: ";

                msg = msg + complaintId + "";

                TextMessageHandler.SendMessageToPhoneNo(phoneNo, msg);
            }
        }

        public static void SendMessageToStakeholder(/*int complaintId, int campaignId, int categoryId, int subCategoryId*/DbComplaint dbComplaint, DbSchoolsMapping dbSchools)
        {
            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            DbComplaintType dbComplaintType = DbComplaintType.GetById((int)dbComplaint.Complaint_Category);
            DbComplaintSubType dbComplaintSubType = DbComplaintSubType.GetById((int)dbComplaint.Complaint_SubCategory);
            TimeSpan span = Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
            DateTime dueDate = DateTime.Now.Add(span);
            List<DbDynamicComplaintFields> callSourceList = DbDynamicComplaintFields.GetByComplaintId(dbComplaint.Id);
            string text = "";
            if (callSourceList != null)
            {
                if (callSourceList.Any(x => x.ControlId == 20 && x.CategoryTypeId == 6123))
                {
                    text = "Chief Minister's Complaint Center Alert\n\n" +
                           "Dear Concerned, Complaint no.{0} has been assigned to you on {1} at {2}.\n" +
                           "Category: {3}\n" +
                            "Subcategory: {4} at {5}, {6}\n" +
                            "Please resolve by {7}\n" +
                            "To view details, please visit: https://www.crm.punjab.gov.pk";
                    text = string.Format(text, dbComplaint.Id, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), dbComplaintType.Name, dbComplaintSubType.Name, dbSchools.school_emis_code, dbSchools.school_name, dueDate.ToString());
                }
                else if (callSourceList.Any(x => x.ControlId == 20 && x.CategoryTypeId == 6124))
                {
                    text = "New Complaint: \n" + dbComplaintType.Name + ":" + dbComplaintSubType.Name + " at " + dbSchools.school_emis_code + " " + dbSchools.school_name + "" + "\n" +
                        "Please resolve by " + dueDate.ToString() + "\n" +
                        "To view details, please visit: https://www.crm.punjab.gov.pk";
                }
            }            
            List<DbUsers> listDbUsers = TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(dbComplaint, text);
            //List<DbUsers> listDbUsers = new List<DbUsers>();
            /*if (listDbUsers.Count == 0)
            {
                DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
                DbUsers currUser = null;
                int hierarchyIdValue = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
                listDbUsers = UsersHandler.FindUserUpperThanCurrentHierarchy(null, dbComplaint, campaignId, (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id,
                    hierarchyIdValue, true);

                TextMessageHandler.SendMessageToUsersList(listDbUsers, msg);
            }*/

            //PrepareMessageToStakeholderSupervisor(dbComplaint, (int)dbComplaint.Compaign_Id, dbComplaintType, dbComplaintSubType,
            //    dbSchools);
        }

        private static void PrepareMessageToStakeholderSupervisor(DbComplaint dbComplaint, int campaignId, DbComplaintType dbComplaintType, DbComplaintSubType dbComplaintSubType, DbSchoolsMapping dbSchools)
        {
            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            DbUsers currUser = null;
            int hierarchyIdValue = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
            List<DbUsers> listDbUsers = UsersHandler.FindUserUpperThanCurrentHierarchy2(null, dbComplaint, campaignId, (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id,
                hierarchyIdValue, true);


            if (listDbUsers.Count > 0)
            {
                string designation = "", hierarchyValue;
                DbUsers dbUser = listDbUsers.FirstOrDefault();
                //int hierarchyVal = Convert.ToInt32(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser).Split(',')[0]);
                //string hierarchyString = Utility.GetHierarchyValue((Config.Hierarchy)dbUser.Hierarchy_Id, hierarchyVal);

                int hierarchyVal = Convert.ToInt32(DbComplaint.GetHierarchyIdValueAgainstHierarchyId((Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id, dbComplaint));
                string hierarchyString = Utility.GetHierarchyValueName((Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id, hierarchyVal);


                TimeSpan span = Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
                DateTime dueDate = DateTime.Now.Add(span);

                designation = "" + dbUser.Designation_abbr + "-" + hierarchyString + "";
                string msg = "Please direct " + designation + " to resolve: \n" + dbComplaintType.Name + ":" + dbComplaintSubType.Name + " at " +
                             dbSchools.school_emis_code + " " + dbSchools.school_name + " by " + dueDate.ToShortDateString() + "\n" +
                             "To view details, please visit: www.crm.punjab.gov.pk";

                TextMessageHandler.SendMessageToUsersList(listDbUsers, msg);
            }
        }

        public static VmAddComplaint GetVmAddComplaintMerged(VmAddComplaint viewModel, int profileId = 0, int campaignId = 0)
        {
            viewModel.ComplaintVm.Compaign_Id = campaignId;
            viewModel.PersonalInfoVm.Person_id = profileId;

            viewModel.ComplaintVm.ListOfProvinces = DbProvince.AllProvincesList();

            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Complaint);
            viewModel.ComplaintVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.ComplaintVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);

            //viewModel.ComplaintVm.ListOfDepartment = DbDepartment.GetByCampaignId(campaignId);
            viewModel.ComplaintVm.hasDepartment = (viewModel.ComplaintVm.ListOfDepartment != null &&
                                                   viewModel.ComplaintVm.ListOfDepartment.Count > 0) ? true : false;
            //viewModel.ComplaintVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignId(campaignId);

            viewModel.ComplaintVm.DynamicFieldsCount = 0;
            //viewModel.ComplaintVm.ListDynamicFields = DynamicFieldsHandler.GetDynamicFieldsAgainstCampaignId(campaignId);

            List<VmDynamicField> listVmDynamic = DynamicFieldsHandler.GetDynamicFieldsAgainstCampaignId(campaignId).OrderBy(n => n.Priority).ToList();
            if (listVmDynamic != null && listVmDynamic.Count > 0)
            {
                viewModel.ComplaintVm.DynamicFieldsCount = listVmDynamic.Count;
                viewModel.ComplaintVm.MinDynamicFormPriority = listVmDynamic.First().Priority;
                viewModel.ComplaintVm.MaxDynamicFormPriority = listVmDynamic.Last().Priority;


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
                viewModel.ComplaintVm.ListDynamicTextBox = listDynamicTextBox;
                viewModel.ComplaintVm.ListDynamicLabel = listDynamicLabel;
                viewModel.ComplaintVm.ListDynamicDropDown = listDynamicDropdown;
                viewModel.ComplaintVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;

                //viewModel.SuggestionVm.ListDynamicTextBox = listDynamicTextBox;
                //viewModel.SuggestionVm.ListDynamicDropDown = listDynamicDropdown;
                //viewModel.SuggestionVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;

                //viewModel.InquiryVm.ListDynamicTextBox = listDynamicTextBox;
                //viewModel.InquiryVm.ListDynamicDropDown = listDynamicDropdown;
                //viewModel.InquiryVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;
            }

            viewModel.HardCopyComplaintIntoSuggestionAndInquiry();
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            viewModel.PersonalInfoVm = Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetPersonInformationByPersonId(profileId));

            // Copy complaint into suggestion
            Mapper.CreateMap<VmComplaint, VmSuggestion>();
            viewModel.SuggestionVm = Mapper.Map<VmSuggestion>(viewModel.ComplaintVm);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Suggestion);
            viewModel.SuggestionVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.SuggestionVm.ListOfComplaintTypes = DbComplaintType.GetByDepartmentAndGroupId(campaignId, groupId);
            //viewModel.SuggestionVm.ListOfDepartment = DbDepartment(campaignId);


            // Copy complaint into inquiry
            Mapper.CreateMap<VmComplaint, VmInquiry>();
            viewModel.InquiryVm = Mapper.Map<VmInquiry>(viewModel.ComplaintVm);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Inquiry);
            viewModel.InquiryVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.InquiryVm.ListOfComplaintTypes = DbComplaintType.GetByDepartmentAndGroupId(campaignId, groupId);
            MakeFieldsNonMendatory(viewModel);

            //viewModel.SuggestionVm.

            return viewModel;
        }

        private static void MakeFieldsNonMendatory(VmAddComplaint viewModel)
        {
            viewModel.SuggestionVm.ListDynamicDropDownServerSide[0].IsRequired = false;
            viewModel.InquiryVm.ListDynamicDropDownServerSide[0].IsRequired = false;

            viewModel.SuggestionVm.ListDynamicDropDown.Where(n => n.ControlId == 19).First().IsRequired = false;
            viewModel.InquiryVm.ListDynamicDropDown.Where(n => n.ControlId == 19).First().IsRequired = false;
        }


        public static List<DbComplaint> GetComplaintsAgainstUser(DbUsers dbUser, Config.StakeholderComplaintListingType listingType, Config.ComplaintType complaintType, Config.SelectionType selectionType)
        {
            string commaSeperatedCampaigns = dbUser.Campaigns;
            string commaSeperatedCategories = dbUser.Categories;


            Config.Permissions StatusPermission = (listingType == Config.StakeholderComplaintListingType.AssignedToMe)
                ? Config.Permissions.StatusesForComplaintListing
                : Config.Permissions.StatusesForComplaintListingAll;

            List<DbPermissionsAssignment> listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, dbUser.User_Id, (int)StatusPermission);

            List<DbStatus> listDbStatuses = BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.User_Id,
                        listDbPermissionsAssignment, StatusPermission);

            string commaSeperatedStatuses = Utility.GetCommaSepStrFromList(listDbStatuses.Select(n => n.Complaint_Status_Id).ToList());
            string commaSeperatedTransferedStatus = "0,1";
            // Config.ComplaintType complaintType = 
            ListingParamsModelBase paramsSchoolEducation = SetParamsSchoolEducation(dbUser, null, null, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, null, complaintType, listingType, "UserComplaintsList");
            if (selectionType == Config.SelectionType.All)
            {
                paramsSchoolEducation.SelectionFields = "complaints.*";
            }
            else
            {
                paramsSchoolEducation.SelectionFields = "complaints.Id";
            }
            //paramsSchoolEducation.SelectionFields = 
            string queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
            return DBHelper.GetDataTableByQueryString(queryStr, null).ToList<DbComplaint>();
        }

        public static DataTable GetStakeHolderServerSideListDenormalized(string from, string to, DataTableParamsModel dtModel, string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            DbUsers dbUser = null;
            if (userId == -1)
            {
                dbUser = Utility.GetUserFromCookie();
            }
            else
            {
                dbUser = RepoDbUsers.GetActiveUser(userId);
            }

            if (dtModel != null)
            {

                Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
                //dictOrderQuery.Add("Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate))");
                //List<string> prefixStrList = new List<string> { "a", "a", "a", "a", "a", "a", "a", "a", "a" };
                Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();

                List<string> prefixStrList = null;
                if (dbUser.SubRole_Id == Config.SubRoles.SDU)
                {
                    prefixStrList = new List<string>
                    {
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "schoolMap",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints"
                    };
                    dictFilterQuery.Add("complaints.StatusChangedDate_Time",
                        "CONVERT(VARCHAR(10),complaints.StatusChangedDate_Time,120) Like '%_Value_%'");
                }
                else
                {
                    prefixStrList = new List<string>
                    {
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints"
                    };
                }
                //List<string> prefixStrList = new List<string>();
                DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList, dictOrderQuery);


                //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
                dictFilterQuery.Add("complaints.Created_Date",
                    "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

                // for joins
                //dictFilterQuery.Add("complaintType.Complaint_Category_Name", "complaintType.name Like '%_Value_%'");
                //dictFilterQuery.Add("Statuses.Complaint_Computed_Status", "Statuses.[Status] Like '%_Value_%'");
                DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixStrList,
                    dictFilterQuery);
                //return GetComplaintsOfStakeholderServerSide(from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
                //DbUsers dbUser = DbUsers.GetActiveUser(new AuthenticationHandler().CmsCookie.UserId);
            }
            //dbUser.User_Id
            ListingParamsModelBase paramsSchoolEducation = SetParamsSchoolEducation(dbUser, from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
            string queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }

        public static ListingParamsModelBase SetParamsSchoolEducation(DbUsers dbUser, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = " complaints.Department_Name, complaints.Complaint_SubCategory_Name, complaints.RefField1, complaints.RefField2, complaints.RefField3, complaints.RefField4, complaints.RefField5, complaints.RefField6, complaints.Person_Cnic, complaints.Computed_Overdue_Days, complaints.Complaint_Remarks, ";

            //CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsModelBase paramsModel = new ListingParamsModelBase();
            paramsModel.SelectionFields = "";
            paramsModel.InnerJoinLogic = "";
            if (spType == "ExcelReport")
            {
                paramsModel.SelectionFields = @"CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10)) AS [Complaint No],
                    complaints.Complaint_Computed_Status as Complaint_Status,Computed_Remaining_Total_Time, 
                    complaints.Complaint_Computed_Hierarchy as [Escalation Level],
                    complaints.Computed_Overdue_Days,
                    complaints.FollowupCount,
                    -- C.Person_Name as [Person Name],
                    -- C.Cnic_No as [Cnic No],
                    -- CASE C.Gender WHEN 1 THEN 'MALE' ELSE 'FEMALE' END AS Gender,
                    -- complaints.Person_District_Name as [Caller District],
                    -- CONVERT(VARCHAR(10),complaints.Created_Date,120) Date,
                    -- C.Mobile_No as [Mobile No],
                    -- C.Person_Address as [Person Address],
                    D.District_Name [Complaint District],
                    B.Name Category  ,
                    F.Name as [Sub Category],
                    complaints.Complaint_Remarks as [Complaint Remarks],
                    --complaints.Agent_Comments as [Agent Comments],
                    --P.[Status],
                    
                    complaints.Created_Date as [Created Date]--,
                    --df.FieldName,df.FieldValue";

                paramsModel.InnerJoinLogic = @"INNER JOIN PITB.Complaints_Type B ON complaints.Complaint_Category=B.Id
                    INNER JOIN PITB.Complaints_SubType F ON complaints.Complaint_SubCategory=F.Id
                    INNER JOIN PITB.Person_Information C ON complaints.Person_Id=C.Person_id
                    INNER JOIN PITB.Districts D ON complaints.District_Id=D.id
                    INNER JOIN PITB.Statuses P ON p.Id=complaints.Complaint_Computed_Status_Id
                    --LEFT JOIN pitb.Dynamic_ComplaintFields df ON df.ComplaintId = complaints.Id
                    ";
            }


            if (dtParams != null)
            {
                paramsModel.StartRow = dtParams.Start;
                paramsModel.EndRow = dtParams.End;
                paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
                paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
                paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;
            }

            paramsModel.From = fromDate;
            paramsModel.To = toDate;
            paramsModel.Campaign = campaign;
            paramsModel.Category = category;
            paramsModel.Status = complaintStatuses;
            paramsModel.TransferedStatus = commaSeperatedTransferedStatus;
            paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
            paramsModel.UserHierarchyId = Convert.ToInt32(dbUser.Hierarchy_Id);
            paramsModel.UserDesignationHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);
            paramsModel.ListingType = Convert.ToInt32(listingType);
            paramsModel.ProvinceId = dbUser.Province_Id;
            paramsModel.DivisionId = dbUser.Division_Id;
            paramsModel.DistrictId = dbUser.District_Id;

            paramsModel.Tehsil = dbUser.Tehsil_Id;
            paramsModel.UcId = dbUser.UnionCouncil_Id;
            paramsModel.WardId = dbUser.Ward_Id;

            paramsModel.UserId = dbUser.User_Id;
            paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            paramsModel.ListUserCategory = UserCategoryModel.GetListUserCategoryModel(dbUser.ListDbUserCategory);
            paramsModel.CheckIfExistInSrcId = 1;
            paramsModel.CheckIfExistInUserSrcId = 1;
            paramsModel.SelectionFields = paramsModel.SelectionFields + extraSelection;
            paramsModel.SpType = spType;


            if (dbUser.SubRole_Id == Config.SubRoles.SDU && (listingType == Config.StakeholderComplaintListingType.AssignedToMe || listingType == Config.StakeholderComplaintListingType.UptilMyHierarchy))
            {
                paramsModel.IgnoreComputedHierarchyCheck = true;
                paramsModel.SelectionFields = @"complaints.StatusReopenedCount, complaints.Callback_Count, complaints.Callback_Status, complaints.Callback_Comment, complaints.Person_Contact, schoolMap.Assigned_To, schoolMap.Assigned_To_Name,CONVERT(VARCHAR(10),complaints.StatusChangedDate_Time,120) StatusChangedDate_Time ," + paramsModel.SelectionFields;
                paramsModel.InnerJoinLogic = paramsModel.InnerJoinLogic + @" INNER JOIN pitb.School_Category_User_Mapping schoolMap
                    ON complaints.Complaint_SubCategory=schoolMap.Category_Id ";
                paramsModel.WhereLogic = " AND (schoolMap.Assigned_To = " + (int)Config.SchoolEducationUserSubRoles.SDU + " OR (complaints.Status_ChangedBy=" + Config.MEALoginId + "AND complaints.Complaint_Status_Id=" + (int)Config.ComplaintStatus.ResolvedVerified + ") ) ";
            }
            else if (dbUser.User_Id == Config.MEALoginId && listingType == Config.StakeholderComplaintListingType.AssignedToMe)
            {
                paramsModel.UserHierarchyId = (int)Config.Hierarchy.None;
                paramsModel.InnerJoinLogic = paramsModel.InnerJoinLogic + @" INNER JOIN pitb.School_Category_User_Mapping schoolMap
                    ON complaints.Complaint_SubCategory=schoolMap.Category_Id ";
                paramsModel.WhereLogic = " AND (schoolMap.Assigned_To = " + (int)Config.SchoolEducationUserSubRoles.MEA + " AND complaints.Complaint_Computed_Status_Id=" + (int)Config.ComplaintStatus.ResolvedUnverified + ") ";
                paramsModel.IgnoreComputedHierarchyCheck = true;
                paramsModel.UserCategoryId1 = null;
                paramsModel.UserCategoryId2 = null;
                paramsModel.CheckIfExistInUserSrcId = 0;
            }
            else if (dbUser.User_Id == Config.DashBoardSpecific1LoginId)
            {
                List<DbPermissionsAssignment> permList = RepoDbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.User, dbUser.User_Id, (int)Config.Permissions.ViewableSchoolsToUser);

                if (permList != null && permList.Count > 0)
                {
                    var selectedPerm = permList[0];
                    var value = selectedPerm.Permission_Value;
                    paramsModel.WhereLogic = " AND RefField1 IN (" + value + ") ";
                }
            }
            else
            {
                paramsModel.IgnoreComputedHierarchyCheck = false;
            }

            return paramsModel;
        }
        /*
        public static List<SelectListItem> GetAlteredStatus(List<SelectListItem> listCampaignStatuses)
        {
            SelectListItem selectedListItem = listCampaignStatuses.FirstOrDefault(n => n.Value == ((int)Config.ComplaintStatus.UnsatisfactoryClosed).ToString());
            if (selectedListItem != null)
            {
                selectedListItem.Text = Config.SchoolEducationUnsatisfactoryStatus;
            }
            return listCampaignStatuses;
        }

        public static string GetAlteredStatus(string statusStr)
        {
            if (statusStr == Config.UnsatisfactoryClosedStatus)
            {
                return Config.SchoolEducationUnsatisfactoryStatus; 
            }
            return statusStr;
        }

        public static string GetAlteredStatus(int campaignId, string statusStr)
        {
            if (campaignId == (int) Config.Campaign.SchoolEducationEnhanced)
            {
                if (statusStr == Config.UnsatisfactoryClosedStatus)
                {
                    return Config.SchoolEducationUnsatisfactoryStatus;
                }
            }
            return statusStr;
        }*/

        #region Dashboard


        public static VmStatusWiseComplaintsData GetAgingReportData(string dateFirst, string dateSecond, int campaignId, Config.AgingReportType agingReportType)
        {
            VmStatusWiseComplaintsData statusWiseComplaintData = new VmStatusWiseComplaintsData();
            statusWiseComplaintData.ListUserWiseData = new List<VmUserWiseStatus>();

            for (int i = 0; i < Config.ListAgingReportXDistribution.Count; i++)
            {
                statusWiseComplaintData.ListUserWiseData.Add(new VmUserWiseStatus { UserId = -1, Name = Config.ListAgingReportXDistribution[i], ListVmStatusWiseCount = new List<VmStatusCount>() });
            }

            List<AgingReportListingData> listAgingReportData = RepoDbComplaint.GetAgingReportListingData(campaignId, Config.ComplaintStatus.ResolvedUnverified);

            DateTime dtFirst = DateTime.Now;
            DateTime dtSecond = DateTime.Now;
            List<Tuple<int, DateTime>> listComparableDates = null;
            //List<int> listComparableDateIds = null;
            VmStatusCount vmStatusCount = null;
            int count = 0;
            switch (agingReportType)
            {
                case Config.AgingReportType.Monthly:
                    dtFirst = DateTime.ParseExact(dateFirst, "MM-yyyy", CultureInfo.InvariantCulture);
                    dtSecond = DateTime.ParseExact(dateSecond, "MM-yyyy", CultureInfo.InvariantCulture);
                    listComparableDates = new List<Tuple<int, DateTime>> { new Tuple<int, DateTime>(100, dtFirst), new Tuple<int, DateTime>(101, dtSecond) };
                    //listComparableDateIds = new List<int> { 100, 101 }; 
                    listAgingReportData = listAgingReportData.Where(
                    n => (n.ResolvedDateTime.Month == dtFirst.Month && n.ResolvedDateTime.Year == dtFirst.Year) ||
                         (n.ResolvedDateTime.Month == dtSecond.Month && n.ResolvedDateTime.Year == dtSecond.Year))
                    .ToList();
                    //float f1 = listAgingReportData[0].ResolvedTimePercentage;
                    //float f2 = listAgingReportData[1].ResolvedTimePercentage;
                    //float f3 = listAgingReportData[2].ResolvedTimePercentage;
                    //float f4 = listAgingReportData[3].ResolvedTimePercentage;
                    //float f5 = listAgingReportData[4].ResolvedTimePercentage;

                    for (int i = 0; i < Config.ListPercentageDistribution.Count - 1; i++)
                    {
                        statusWiseComplaintData.ListUserWiseData[i].ListVmStatusWiseCount = new List<VmStatusCount>();
                        for (int j = 0; j < listComparableDates.Count; j++)
                        {
                            vmStatusCount = new VmStatusCount();
                            vmStatusCount.StatusId = listComparableDates[j].Item1;
                            vmStatusCount.StatusString =
                                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(listComparableDates[j].Item2.Month);//Config.ListAgingReportXDistribution[i];
                            //vmStatusCount.
                            if (Config.ListPercentageDistribution[i] > 0)
                            {
                                vmStatusCount.Count = listAgingReportData.Where(
                                   n => (n.ResolvedDateTime.Month == listComparableDates[j].Item2.Month && n.ResolvedDateTime.Year == listComparableDates[j].Item2.Year)
                                        && n.ResolvedTimePercentage <= Config.ListPercentageDistribution[i]
                                        && n.ResolvedTimePercentage >= Config.ListPercentageDistribution[i + 1])
                                   .ToList().Count;
                            }
                            else if (Config.ListPercentageDistribution[i] <= 0)
                            {
                                vmStatusCount.Count = listAgingReportData.Where(
                                    n => (n.ResolvedDateTime.Month == listComparableDates[j].Item2.Month && n.ResolvedDateTime.Year == listComparableDates[j].Item2.Year)
                                         && n.ResolvedTimePercentage <= Config.ListPercentageDistribution[i]
                                        && n.ResolvedTimePercentage >= Config.ListPercentageDistribution[i + 1])
                                   .ToList().Count;
                            }
                            statusWiseComplaintData.ListUserWiseData[i].ListVmStatusWiseCount.Add(vmStatusCount);
                        }
                    }

                    break;

                case Config.AgingReportType.Quarterly:
                    int dtQuarter1 = Convert.ToInt32(dateFirst.Split('_')[1]);
                    int dtQuarter2 = Convert.ToInt32(dateSecond.Split('_')[1]);

                    int quarter1StartMonth = ((dtQuarter1 * 3) - 3) + 1;
                    int quarter1EndMonth = (dtQuarter1 * 3);

                    int quarter2StartMonth = ((dtQuarter2 * 3) - 3) + 1;
                    int quarter2EndMonth = (dtQuarter2 * 3);


                    dtFirst = DateTime.ParseExact(dateFirst.Split('_')[0], "yyyy", CultureInfo.InvariantCulture);
                    dtSecond = DateTime.ParseExact(dateSecond.Split('_')[0], "yyyy", CultureInfo.InvariantCulture);

                    List<Tuple<int, DateTime, int, int, int>> listComparableDatesQuarterly = new List<Tuple<int, DateTime, int, int, int>>
                    {
                        new Tuple<int, DateTime,int,int,int>(100, dtFirst, dtQuarter1, quarter1StartMonth, quarter1EndMonth), 
                        new Tuple<int, DateTime,int,int,int>(101, dtSecond, dtQuarter2, quarter2StartMonth, quarter2EndMonth)
                    };
                    //listComparableDateIds = new List<int> { 100, 101 }; 
                    listAgingReportData = listAgingReportData.Where(
                    n => (n.ResolvedDateTime.Year == dtFirst.Year && n.ResolvedDateTime.Month >= quarter1StartMonth && n.ResolvedDateTime.Month <= quarter1EndMonth) ||
                         (n.ResolvedDateTime.Year == dtSecond.Year && n.ResolvedDateTime.Month >= quarter2StartMonth && n.ResolvedDateTime.Month <= quarter2EndMonth))
                    .ToList();


                    for (int i = 0; i < Config.ListPercentageDistribution.Count - 1; i++)
                    {
                        statusWiseComplaintData.ListUserWiseData[i].ListVmStatusWiseCount = new List<VmStatusCount>();
                        for (int j = 0; j < listComparableDatesQuarterly.Count; j++)
                        {
                            vmStatusCount = new VmStatusCount();
                            vmStatusCount.StatusId = listComparableDatesQuarterly[j].Item1;
                            vmStatusCount.StatusString = listComparableDatesQuarterly[j].Item2.Year.ToString() + "_Q (" + listComparableDatesQuarterly[j].Item3 + ")";//Config.ListAgingReportXDistribution[i];
                            //vmStatusCount.
                            if (Config.ListPercentageDistribution[i] > 0)
                            {
                                vmStatusCount.Count = listAgingReportData.Where(
                                   n => (n.ResolvedDateTime.Year == listComparableDatesQuarterly[j].Item2.Year && n.ResolvedDateTime.Month >= listComparableDatesQuarterly[j].Item4 && n.ResolvedDateTime.Month <= listComparableDatesQuarterly[j].Item5)
                                        && n.ResolvedTimePercentage <= Config.ListPercentageDistribution[i]
                                        && n.ResolvedTimePercentage >= Config.ListPercentageDistribution[i + 1])
                                   .ToList().Count;
                            }
                            else if (Config.ListPercentageDistribution[i] <= 0)
                            {
                                vmStatusCount.Count = listAgingReportData.Where(
                                    n => (n.ResolvedDateTime.Year == listComparableDatesQuarterly[j].Item2.Year && n.ResolvedDateTime.Month >= listComparableDatesQuarterly[j].Item4 && n.ResolvedDateTime.Month <= listComparableDatesQuarterly[j].Item5)
                                         && n.ResolvedTimePercentage <= Config.ListPercentageDistribution[i]
                                        && n.ResolvedTimePercentage >= Config.ListPercentageDistribution[i + 1])
                                   .ToList().Count;
                            }
                            statusWiseComplaintData.ListUserWiseData[i].ListVmStatusWiseCount.Add(vmStatusCount);
                        }
                    }

                    break;

                case Config.AgingReportType.Yearly:
                    dtFirst = DateTime.ParseExact(dateFirst, "yyyy", CultureInfo.InvariantCulture);
                    dtSecond = DateTime.ParseExact(dateSecond, "yyyy", CultureInfo.InvariantCulture);
                    listComparableDates = new List<Tuple<int, DateTime>> { new Tuple<int, DateTime>(100, dtFirst), new Tuple<int, DateTime>(101, dtSecond) };
                    //listComparableDateIds = new List<int> { 100, 101 }; 
                    listAgingReportData = listAgingReportData.Where(
                    n => (n.ResolvedDateTime.Year == dtFirst.Year) ||
                         (n.ResolvedDateTime.Year == dtSecond.Year))
                    .ToList();
                    //float f1 = listAgingReportData[0].ResolvedTimePercentage;
                    //float f2 = listAgingReportData[1].ResolvedTimePercentage;
                    //float f3 = listAgingReportData[2].ResolvedTimePercentage;
                    //float f4 = listAgingReportData[3].ResolvedTimePercentage;
                    //float f5 = listAgingReportData[4].ResolvedTimePercentage;

                    for (int i = 0; i < Config.ListPercentageDistribution.Count - 1; i++)
                    {
                        statusWiseComplaintData.ListUserWiseData[i].ListVmStatusWiseCount = new List<VmStatusCount>();
                        for (int j = 0; j < listComparableDates.Count; j++)
                        {
                            vmStatusCount = new VmStatusCount();
                            vmStatusCount.StatusId = listComparableDates[j].Item1;
                            vmStatusCount.StatusString = listComparableDates[j].Item2.Year.ToString();//Config.ListAgingReportXDistribution[i];
                            //vmStatusCount.
                            if (Config.ListPercentageDistribution[i] > 0)
                            {
                                vmStatusCount.Count = listAgingReportData.Where(
                                   n => (n.ResolvedDateTime.Year == listComparableDates[j].Item2.Year)
                                        && n.ResolvedTimePercentage <= Config.ListPercentageDistribution[i]
                                        && n.ResolvedTimePercentage >= Config.ListPercentageDistribution[i + 1])
                                   .ToList().Count;
                            }
                            else if (Config.ListPercentageDistribution[i] <= 0)
                            {
                                vmStatusCount.Count = listAgingReportData.Where(
                                    n => (n.ResolvedDateTime.Year == listComparableDates[j].Item2.Year)
                                         && n.ResolvedTimePercentage <= Config.ListPercentageDistribution[i]
                                        && n.ResolvedTimePercentage >= Config.ListPercentageDistribution[i + 1])
                                   .ToList().Count;
                            }
                            statusWiseComplaintData.ListUserWiseData[i].ListVmStatusWiseCount.Add(vmStatusCount);
                        }
                    }

                    break;
            }


            return statusWiseComplaintData;
        }


        public static VmStatusWiseComplaintsData GetTeachingQualityDataForSchoolEducationDashboard(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph, int hierarchyId = -1)
        {
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            List<DbStatus> listDbStatuses = null;
            string userStatuses = null;
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            string queryStr = "";
            DataSet ds;
            VmStatusWiseComplaintsData statusWiseComplaintData;
            List<VmUserWiseStatus> listVmUserWiseStatus;

            switch (userWiseGraph)
            {
                case Config.UserWiseGraphType.MyOwn:
                    //dbUser = DbUsers.GetActiveUser(userId);
                    //listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                    //    (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

                    //listDbStatuses =
                    //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                    //        listDbPermissionsAssignment);

                    //userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());
                    dbUser = DbUsers.GetActiveUser(userId);
                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, null, null,
                        null, null, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.None, "DynamicFieldsWiseCounts");


                    string regionId = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser);
                    string regionValue = Utility.GetHierarchyValueName((Config.Hierarchy)dbUser.Hierarchy_Id,
                        Convert.ToInt32(regionId.Split(',')[0])); //DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser);

                    List<DbDynamicCategories> listDbDynamicCategories =
                        DbDynamicCategories.GetByCampaignAndCategoryId((int)Config.Campaign.SchoolEducationEnhanced, (int)Config.DynamicCategoryTypeId.SchoolEducationTeachingQuality);
                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyTeachingQualityModel(new KeyValuePair<string, string>(regionId, regionValue), listDbDynamicCategories));
                    paramsSchoolEducation.DynamicFieldsControlId = ((int)Config.DynamicFieldControlId.SchoolEducationTeachingQuality).ToString();
                    paramsSchoolEducation.regionTypeId = Convert.ToInt32(regionId.Split(',')[0]);

                    SetListingLogicParamsAgainstHierarchy(paramsSchoolEducation, (int)dbUser.Hierarchy_Id, regionId);

                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetTeachingQualityComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);
                        return statusWiseComplaintData;
                    }
                    return null;
                    break;


                case Config.UserWiseGraphType.LowerIndividual:
                    //return GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
                    dbUser = Utility.GetUserFromCookie();
                    //dbUser = DbUsers.GetActiveUser(userId);
                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, null, null,
                        null, null, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.None, "DynamicFieldsWiseCounts");

                    //hierarchyId = (hierarchyId - 1);


                    //regionId = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser, (Config.Hierarchy)hierarchyId);
                    regionId = userId.ToString();
                    List<int> listRegionIds = Utility.GetHierarchyValueUnderHierarchy((Config.Hierarchy)hierarchyId, regionId, (int)Config.Campaign.SchoolEducationEnhanced);


                    listDbDynamicCategories = DbDynamicCategories.GetByCampaignAndCategoryId((int)Config.Campaign.SchoolEducationEnhanced, (int)Config.DynamicCategoryTypeId.SchoolEducationTeachingQuality);
                    int lowerHierarchy = (hierarchyId + 1) <= (int)Config.Hierarchy.Ward ? (hierarchyId + 1) : -1;
                    if (lowerHierarchy == 2)
                    {
                        lowerHierarchy++;
                    }
                    if (lowerHierarchy != -1)
                    {
                        int i = 0;
                        listVmUserWiseStatus = new List<VmUserWiseStatus>();
                        foreach (int rId in listRegionIds)
                        {
                            //List<DbDynamicCategories> listDbDynamicCategories =
                            regionValue = Utility.GetHierarchyValueName((Config.Hierarchy)lowerHierarchy,
                                rId); //DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser);

                            listVmUserWiseStatus.Add(
                                MakeEmptyTeachingQualityModel(new KeyValuePair<string, string>(rId.ToString(), regionValue),
                                    listDbDynamicCategories));
                            paramsSchoolEducation.DynamicFieldsControlId =
                                ((int)Config.DynamicFieldControlId.SchoolEducationTeachingQuality).ToString();
                            paramsSchoolEducation.regionTypeId = rId;
                            i++;
                        }
                        string selectionCheck = "";
                        string groupByFields = "";
                        string regionIds = Utility.GetCommaSepStrFromList(listRegionIds);

                        SetListingLogicParamsAgainstHierarchy(paramsSchoolEducation, lowerHierarchy, regionIds);

                        queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                        if (!string.IsNullOrEmpty(queryStr))
                        {
                            ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                            statusWiseComplaintData = GetTeachingQualityComplaintData(ds, listVmUserWiseStatus);
                            VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                                statusWiseComplaintData, false);

                            statusWiseComplaintData.ListUserWiseData =
                            statusWiseComplaintData.ListUserWiseData.OrderByDescending(
                                n => n.TotalStatusWiseCount).ToList();

                            return statusWiseComplaintData;
                        }
                    }
                    return null;
                    break;

                case Config.UserWiseGraphType.CumulationOfLower:
                    // start custom
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

                    listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);
                    //BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                    //    listDbPermissionsAssignment);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);
                        return statusWiseComplaintData;
                    }
                    return null;
                    // end custom

                    //statusWiseComplaintData = GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
                    //return GetCumulatedStatusWiseComplaintData(statusWiseComplaintData, userId);
                    break;
            }
            return new VmStatusWiseComplaintsData();
        }

        //public static VmStatusWiseComplaintsData GetTeachingQualityDataForSchoolEducationDashboard(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph, int hierarchyId = -1)
        //{
        //    DbUsers dbUser = null;
        //    List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
        //    List<DbStatus> listDbStatuses = null;
        //    string userStatuses = null;
        //    DataTableParamsModel dtParams = null;
        //    ListingParamsModelBase paramsSchoolEducation = null;

        //    string queryStr = "";
        //    DataSet ds;
        //    VmStatusWiseComplaintsData statusWiseComplaintData;
        //    List<VmUserWiseStatus> listVmUserWiseStatus;

        //    switch (userWiseGraph)
        //    {
        //        case Config.UserWiseGraphType.MyOwn:
        //            //dbUser = DbUsers.GetActiveUser(userId);
        //            //listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
        //            //    (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

        //            //listDbStatuses =
        //            //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
        //            //        listDbPermissionsAssignment);

        //            //userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());
        //            dbUser = DbUsers.GetActiveUser(userId);
        //            paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, null, null,
        //                null, null, Config.ComplaintType.Complaint,
        //                Config.StakeholderComplaintListingType.None, "DynamicFieldsWiseCounts");


        //            string regionId = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser);
        //            string regionValue = Utility.GetHierarchyValue((Config.Hierarchy)dbUser.Hierarchy_Id,
        //                Convert.ToInt32(regionId.Split(',')[0])); //DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser);

        //            List<DbDynamicCategories> listDbDynamicCategories =
        //                DbDynamicCategories.GetByCampaignAndCategoryId((int)Config.Campaign.SchoolEducationEnhanced, (int)Config.DynamicCategoryTypeId.SchoolEducationTeachingQuality);
        //            listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //            listVmUserWiseStatus.Add(MakeEmptyTeachingQualityModel(new KeyValuePair<string, string>(regionId, regionValue), listDbDynamicCategories));
        //            paramsSchoolEducation.DynamicFieldsControlId = ((int)Config.DynamicFieldControlId.SchoolEducationTeachingQuality).ToString();
        //            paramsSchoolEducation.regionTypeId = Convert.ToInt32(regionId.Split(',')[0]);

        //            SetListingLogicParamsAgainstHierarchy(paramsSchoolEducation, (int)dbUser.Hierarchy_Id, regionId);

        //            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

        //            if (!string.IsNullOrEmpty(queryStr))
        //            {
        //                ds = DBHelper.GetDataSetByQueryString(queryStr, null);

        //                statusWiseComplaintData = GetTeachingQualityComplaintData(ds, listVmUserWiseStatus);
        //                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                    statusWiseComplaintData, false);
        //                return statusWiseComplaintData;
        //            }
        //            return null;
        //            break;


        //        case Config.UserWiseGraphType.LowerIndividual:
        //            //return GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
        //            dbUser = Utility.GetUserFromCookie();
        //            //dbUser = DbUsers.GetActiveUser(userId);
        //            paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, null, null,
        //                null, null, Config.ComplaintType.Complaint,
        //                Config.StakeholderComplaintListingType.None, "DynamicFieldsWiseCounts");

        //            //hierarchyId = (hierarchyId - 1);


        //            //regionId = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser, (Config.Hierarchy)hierarchyId);
        //            regionId = userId.ToString();
        //            List<int> listRegionIds = Utility.GetHierarchyValueUnderHierarchy((Config.Hierarchy)hierarchyId, regionId, (int)Config.Campaign.SchoolEducationEnhanced);


        //            listDbDynamicCategories = DbDynamicCategories.GetByCampaignAndCategoryId((int)Config.Campaign.SchoolEducationEnhanced, (int)Config.DynamicCategoryTypeId.SchoolEducationTeachingQuality);
        //            int lowerHierarchy = (hierarchyId + 1) <= (int)Config.Hierarchy.Ward ? (hierarchyId + 1) : -1;
        //            if (lowerHierarchy == 2)
        //            {
        //                lowerHierarchy++;
        //            }
        //            if (lowerHierarchy != -1)
        //            {
        //                int i = 0;
        //                listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //                foreach (int rId in listRegionIds)
        //                {
        //                    //List<DbDynamicCategories> listDbDynamicCategories =
        //                    regionValue = Utility.GetHierarchyValue((Config.Hierarchy)lowerHierarchy,
        //                        rId); //DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser);

        //                    listVmUserWiseStatus.Add(
        //                        MakeEmptyTeachingQualityModel(new KeyValuePair<string, string>(rId.ToString(), regionValue),
        //                            listDbDynamicCategories));
        //                    paramsSchoolEducation.DynamicFieldsControlId =
        //                        ((int)Config.DynamicFieldControlId.SchoolEducationTeachingQuality).ToString();
        //                    paramsSchoolEducation.regionTypeId = rId;
        //                    i++;
        //                }
        //                string selectionCheck = "";
        //                string groupByFields = "";
        //                string regionIds = Utility.GetCommaSepStrFromList(listRegionIds);

        //                SetListingLogicParamsAgainstHierarchy(paramsSchoolEducation, lowerHierarchy, regionIds);

        //                queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

        //                if (!string.IsNullOrEmpty(queryStr))
        //                {
        //                    ds = DBHelper.GetDataSetByQueryString(queryStr, null);

        //                    statusWiseComplaintData = GetTeachingQualityComplaintData(ds, listVmUserWiseStatus);
        //                    VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                        statusWiseComplaintData, false);

        //                    statusWiseComplaintData.ListUserWiseData =
        //                    statusWiseComplaintData.ListUserWiseData.OrderByDescending(
        //                        n => n.TotalStatusWiseCount).ToList();

        //                    return statusWiseComplaintData;
        //                }
        //            }
        //            return null;
        //            break;

        //        case Config.UserWiseGraphType.CumulationOfLower:
        //            // start custom
        //            dbUser = DbUsers.GetActiveUser(userId);
        //            listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
        //                (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

        //            listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);
        //            //BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
        //            //    listDbPermissionsAssignment);

        //            userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

        //            paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
        //                "1,0", dtParams, Config.ComplaintType.Complaint,
        //                Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

        //            listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //            listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
        //            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

        //            if (!string.IsNullOrEmpty(queryStr))
        //            {
        //                ds = DBHelper.GetDataSetByQueryString(queryStr, null);

        //                statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
        //                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                    statusWiseComplaintData, false);
        //                return statusWiseComplaintData;
        //            }
        //            return null;
        //            // end custom

        //            //statusWiseComplaintData = GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
        //            //return GetCumulatedStatusWiseComplaintData(statusWiseComplaintData, userId);
        //            break;
        //    }
        //    return new VmStatusWiseComplaintsData();
        //}
        private static void SetListingLogicParamsAgainstHierarchy(ListingParamsModelBase paramsSchoolEducation, int lowerHierarchy, string regionIds)
        {
            switch (lowerHierarchy)
            {
                case (int)Config.Hierarchy.Province:
                    paramsSchoolEducation.SelectionFields = "complaints.Province_Id as RegionId";
                    paramsSchoolEducation.WhereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + regionIds + @"',',') X WHERE X.Item=complaints.Province_Id)";
                    paramsSchoolEducation.GroupByLogic = "complaints.Province_Id";
                    break;

                case (int)Config.Hierarchy.Division:
                    paramsSchoolEducation.SelectionFields = "complaints.Division_Id as RegionId";
                    paramsSchoolEducation.WhereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + regionIds + @"',',') X WHERE X.Item=complaints.Division_Id)";
                    paramsSchoolEducation.GroupByLogic = "complaints.Division_Id";
                    break;

                case (int)Config.Hierarchy.District:
                    paramsSchoolEducation.SelectionFields = "complaints.District_Id as RegionId";
                    paramsSchoolEducation.WhereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + regionIds + @"',',') X WHERE X.Item=complaints.District_Id)";
                    paramsSchoolEducation.GroupByLogic = "complaints.District_Id";
                    break;

                case (int)Config.Hierarchy.Tehsil:
                    paramsSchoolEducation.SelectionFields = "complaints.Tehsil_Id as RegionId";
                    paramsSchoolEducation.WhereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + regionIds + @"',',') X WHERE X.Item=complaints.Tehsil_Id)";
                    paramsSchoolEducation.GroupByLogic = "complaints.Tehsil_Id";
                    break;

                case (int)Config.Hierarchy.UnionCouncil:
                    paramsSchoolEducation.SelectionFields = "complaints.UnionCouncil_Id as RegionId";
                    paramsSchoolEducation.WhereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + regionIds + @"',',') X WHERE X.Item=complaints.UnionCouncil_Id)";
                    paramsSchoolEducation.GroupByLogic = "complaints.UnionCouncil_Id";
                    break;

                case (int)Config.Hierarchy.Ward:
                    paramsSchoolEducation.SelectionFields = "complaints.Ward_Id as RegionId";
                    paramsSchoolEducation.WhereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + regionIds + @"',',') X WHERE X.Item=complaints.Ward_Id)";
                    paramsSchoolEducation.GroupByLogic = "complaints.Ward_Id";
                    break;
            }
        }
        public static VmStatusWiseComplaintsData GetUsersDashboardData(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph)
        {
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            List<DbStatus> listDbStatuses = null;
            string userStatuses = null;
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            string queryStr = "";
            DataSet ds;
            VmStatusWiseComplaintsData statusWiseComplaintData;
            List<VmUserWiseStatus> listVmUserWiseStatus;

            switch (userWiseGraph)
            {
                case Config.UserWiseGraphType.MyOwn:
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

                    listDbStatuses =
                        BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                            listDbPermissionsAssignment);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.AssignedToMe, "StatusWiseUserComplaints");

                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);
                        return statusWiseComplaintData;
                    }
                    return null;
                    break;


                case Config.UserWiseGraphType.LowerIndividual:
                    return GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
                    //dbUser = DbUsers.GetActiveUser(userId);
                    //List<DbUsers> listDbUsers = UsersHandler.FindUserLowerThanCurrentHierarchy(userId, Utility.GetIntByCommaSepStr(dbUser.Campaigns),
                    //    (Config.Hierarchy)dbUser.Hierarchy_Id,Utility.GetIntByCommaSepStr( DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)));

                    //listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdListAndPermissionId(
                    //    (int)Config.PermissionsType.User, listDbUsers.Select(n=>n.User_Id).ToList(), (int)Config.Permissions.StatusesForComplaintListing);


                    //listVmUserWiseStatus = new List<VmUserWiseStatus>(); 
                    //queryStr = "";
                    //foreach (DbUsers user in listDbUsers)
                    //{

                    //    listDbStatuses =
                    //        BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), user.User_Id,
                    //            listDbPermissionsAssignment);

                    //    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    //    paramsSchoolEducation = SetParamsSchoolEducation(user, startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                    //        "1,0", dtParams, Config.ComplaintType.Complaint,
                    //        Config.StakeholderComplaintListingType.AssignedToMe, "StatusWiseUserComplaints");

                    //    queryStr = queryStr + ListingLogic.GetListingQuery(paramsSchoolEducation);
                    //    listVmUserWiseStatus.Add(MakeEmptyStatusModel(user,listDbStatuses));
                    //}

                    //ds = DBHelper.GetDataSetByQueryString(queryStr, null);
                    //statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                    //return statusWiseComplaintData;
                    break;

                case Config.UserWiseGraphType.CumulationOfLower:
                    // start custom
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListingAll);

                    //listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);


                    listDbStatuses = BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                        listDbPermissionsAssignment, Config.Permissions.StatusesForComplaintListingAll);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);
                        return statusWiseComplaintData;
                    }
                    return null;
                    // end custom

                    //statusWiseComplaintData = GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
                    //return GetCumulatedStatusWiseComplaintData(statusWiseComplaintData, userId);
                    break;
            }
            return new VmStatusWiseComplaintsData();
        }

        public static List<VmComplaintTypeWiseCount> GetComplaintsTypeWiseTotalCount(int userId, string startDate, string endDate, int campaignId)
        {
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            string queryStr = "";
            DataSet ds;
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            string categories = BlComplaintType.GetCommaSepCategoriesForAllComplaintTypesofCampaign(cookie.Campaigns);
            List<VmComplaintTypeWiseCount> listVmComplaintTypeWiseCount = new List<VmComplaintTypeWiseCount>();
            dbUser = DbUsers.GetActiveUser(userId);
            if (dbUser != null)
            {
                listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                (int)Config.PermissionsType.User, userId, (int)Config.Permissions.ViewComplaintTypeWiseData);
                if (listDbPermissionsAssignment != null && listDbPermissionsAssignment.Count > 0)
                {
                    queryStr = "SELECT COUNT(Complaint_Type) AS Complaint_Type_Count ,Complaint_Type FROM PITB.Complaints WHERE Compaign_Id = " + campaignId + " AND (CONVERT(DATE,complaints.Created_Date,120)) BETWEEN '" + startDate + "' AND '" + endDate + "' AND Complaint_Category IN(" + categories + ")  GROUP BY Complaint_Type";
                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);
                        if (ds != null)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    VmComplaintTypeWiseCount vm = new VmComplaintTypeWiseCount();
                                    vm.Id = Convert.ToInt32(row["Complaint_Type"]);
                                    vm.y = Convert.ToInt32(row["Complaint_Type_Count"]);
                                    vm.name = GetComplaintTypePluralName(vm.Id);
                                    listVmComplaintTypeWiseCount.Add(vm);
                                }

                            }
                        }

                        return listVmComplaintTypeWiseCount;
                    }
                }
            }
            return new List<VmComplaintTypeWiseCount>();
        }

        public static List<VmComplaintTypeWiseCount> GetComplaintsCategoryWiseTotalCount(int userId, string startDate, string endDate, int campaignId)
        {
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            string queryStr = "";
            DataSet ds;
            List<VmComplaintTypeWiseCount> listVmComplaintTypeWiseCount = new List<VmComplaintTypeWiseCount>();
            dbUser = DbUsers.GetActiveUser(userId);
            if (dbUser != null)
            {
                listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                (int)Config.PermissionsType.User, userId, (int)Config.Permissions.ViewComplaintTypeWiseData);
                if (listDbPermissionsAssignment != null && listDbPermissionsAssignment.Count > 0)
                {
                    //queryStr = "SELECT COUNT(Complaint_Type) AS Complaint_Type_Count ,Complaint_Type FROM PITB.Complaints WHERE Compaign_Id = " + campaignId + " AND Created_Date BETWEEN '" + startDate + "' AND '" + endDate + "' GROUP BY Complaint_Type";
                    queryStr = @"SELECT COUNT(Complaint_SubCategory) AS Complaint_SubCategory_Count ,Complaint_Category,Complaint_Category_Name,Complaint_SubCategory,Complaint_SubCategory_Name FROM PITB.Complaints WHERE Compaign_Id = " + campaignId + " AND Complaint_Type = 1 AND Created_Date BETWEEN '" + startDate + "' AND '" + endDate + "' AND   Complaint_Computed_Status_Id = 1 GROUP BY Complaint_Category,Complaint_Category_Name,Complaint_SubCategory,Complaint_SubCategory_Name ORDER BY Complaint_SubCategory_Count DESC";
                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);
                        if (ds != null)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    VmComplaintTypeWiseCount vm = new VmComplaintTypeWiseCount();
                                    vm.Id = Convert.ToInt32(dt.Rows[i]["Complaint_SubCategory"]);
                                    vm.y = Convert.ToInt32(dt.Rows[i]["Complaint_SubCategory_Count"]);
                                    string value = dt.Rows[i]["Complaint_SubCategory_Name"].ToString();
                                    string[] splits = value.Split(' ');
                                    string newValue = "";
                                    for (int j = 0; j < splits.Length; j++)
                                    {
                                        if (j % 3 == 0 && j != 0)
                                            newValue = newValue + "<br>";
                                        newValue = newValue +" "+ splits[j];
                                    }
                                    vm.name = dt.Rows[i]["Complaint_Category_Name"].ToString() + ":<br>" + newValue;
                                    listVmComplaintTypeWiseCount.Add(vm);
                                }
                            }
                        }

                        return listVmComplaintTypeWiseCount;
                    }
                }
            }
            return new List<VmComplaintTypeWiseCount>();
        }
        public static string GetComplaintTypePluralName(int id)
        {
            Config.ComplaintType type;
            switch (id)
            {
                case 0:
                    type = Config.ComplaintType.None;
                    break;
                case 1:
                    type = Config.ComplaintType.Complaint;
                    break;
                case 2:
                    type = Config.ComplaintType.Suggestion;
                    break;
                case 3:
                    type = Config.ComplaintType.Inquiry;
                    break;
                default:
                    type = Config.ComplaintType.None;
                    break;
            }
            return EnumExtension.GetPluralName(type);
        }
        private static VmStatusWiseComplaintsData GetCumulatedStatusWiseComplaintData(VmStatusWiseComplaintsData complaintData, int userId)
        {
            if (complaintData == null)
            {
                return null;
            }
            VmStatusWiseComplaintsData vmStatusWiseDataCumulation = new VmStatusWiseComplaintsData();
            vmStatusWiseDataCumulation.ListUserWiseData = new List<VmUserWiseStatus>();

            // Assign Cumulation User
            VmUserWiseStatus vmUserWiseStatusCumulation = new VmUserWiseStatus();
            vmUserWiseStatusCumulation.Name = "Cumulation Hierarchy";
            vmUserWiseStatusCumulation.UserId = userId;
            vmUserWiseStatusCumulation.ListVmStatusWiseCount = new List<VmStatusCount>();
            vmStatusWiseDataCumulation.ListUserWiseData.Add(vmUserWiseStatusCumulation);


            VmStatusCount vmStatusCountTemp = null;

            foreach (VmUserWiseStatus vmUserWiseStatuse in complaintData.ListUserWiseData)
            {
                foreach (VmStatusCount vmStatusCount in vmUserWiseStatuse.ListVmStatusWiseCount)
                {
                    vmStatusCountTemp =
                        vmUserWiseStatusCumulation.ListVmStatusWiseCount.Where(n => n.StatusId == vmStatusCount.StatusId)
                            .FirstOrDefault();

                    if (vmStatusCountTemp == null)
                    {
                        vmUserWiseStatusCumulation.ListVmStatusWiseCount.Add(new VmStatusCount(vmStatusCount));
                    }
                    else
                    {
                        vmStatusCountTemp.Count = vmStatusCountTemp.Count + vmStatusCount.Count;
                    }
                }

                //vmStatusCount = new VmStatusCount();
                //vmStatusCount.
            }
            return vmStatusWiseDataCumulation;
        }
        /*
        public static VmStatusWiseComplaintsData GetLowerAccumulation(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph, bool canDiscartHierarchyLowerThanImmediateOne)
        {
             dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int) Config.PermissionsType.User, userId, (int) Config.Permissions.StatusesForComplaintListing);

                    listDbStatuses =
                        BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                            listDbPermissionsAssignment);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.AssignedToMe, "StatusWiseUserComplaints");

                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
                    queryStr = ListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        return statusWiseComplaintData;
                    }


        }*/



        private static VmStatusWiseComplaintsData GetLowerIndividual(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph, bool canDiscartHierarchyLowerThanImmediateOne)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            List<DbStatus> listDbStatuses = null;
            string userStatuses = null;
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            string queryStr = null;
            DataSet ds;
            VmStatusWiseComplaintsData statusWiseComplaintData;
            List<VmUserWiseStatus> listVmUserWiseStatus;

            dbUser = DbUsers.GetActiveUser(userId);
            List<DbUsers> listDbUsers = UsersHandler.FindUserLowerThanCurrentHierarchy2(userId, Utility.GetIntByCommaSepStr(dbUser.Campaigns),
                (Config.Hierarchy)dbUser.Hierarchy_Id, Utility.GetIntByCommaSepStr(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)), canDiscartHierarchyLowerThanImmediateOne, null, null);

            listDbUsers = listDbUsers.Where(n => !Config.ListSchoolUsersToDiscartDashboard.Contains(n.User_Id)).ToList();

            listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdListAndPermissionId(
                (int)Config.PermissionsType.User, listDbUsers.Select(n => n.User_Id).ToList(), (int)Config.Permissions.StatusesForComplaintListing);


            listVmUserWiseStatus = new List<VmUserWiseStatus>();
            queryStr = "";
            foreach (DbUsers user in listDbUsers)
            {

                listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);
                //BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), user.User_Id,
                //    listDbPermissionsAssignment);

                userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                paramsSchoolEducation = SetParamsSchoolEducation(user, startDate, endDate, /*dbUser.Campaigns*/cookie.Campaigns, Utility.GetCommaSepStrFromList(cookie.CategoryIds) /*dbUser.Categories*/, userStatuses,
                    "1,0", dtParams, Config.ComplaintType.Complaint,
                    Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

                queryStr = queryStr + StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
                listVmUserWiseStatus.Add(MakeEmptyStatusModel(user, listDbStatuses));
            }
            if (!string.IsNullOrEmpty(queryStr))
            {
                ds = DBHelper.GetDataSetByQueryString(queryStr, null);
                statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, true);


                statusWiseComplaintData.ListUserWiseData =
                    statusWiseComplaintData.ListUserWiseData.OrderByDescending(
                        n => n.TotalStatusWiseCount).ToList();

                return statusWiseComplaintData;
            }
            return null;
        }


        //private static VmStatusWiseComplaintsData GetLowerIndividualTeachingQuality(int userId, string startDate, string endDate/*, Config.UserWiseGraphType userWiseGraph, bool canDiscartHierarchyLowerThanImmediateOne*/, int hierarchyId)
        //{
        //    DbUsers dbUser = null;
        //    List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
        //    List<DbStatus> listDbStatuses = null;
        //    string userStatuses = null;
        //    DataTableParamsModel dtParams = null;
        //    ListingParamsModelBase paramsSchoolEducation = null;

        //    string queryStr = null;
        //    DataSet ds;
        //    VmStatusWiseComplaintsData statusWiseComplaintData;
        //    List<VmUserWiseStatus> listVmUserWiseStatus;

        //    dbUser = DbUsers.GetActiveUser(userId);
        //    List<DbUsers> listDbUsers = UsersHandler.FindUserLowerThanCurrentHierarchy(userId, Utility.GetIntByCommaSepStr(dbUser.Campaigns),
        //        (Config.Hierarchy)dbUser.Hierarchy_Id, Utility.GetIntByCommaSepStr(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)), canDiscartHierarchyLowerThanImmediateOne);

        //    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdListAndPermissionId(
        //        (int)Config.PermissionsType.User, listDbUsers.Select(n => n.User_Id).ToList(), (int)Config.Permissions.StatusesForComplaintListing);


        //    listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //    queryStr = "";
        //    foreach (DbUsers user in listDbUsers)
        //    {

        //        listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);
        //        //BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), user.User_Id,
        //        //    listDbPermissionsAssignment);

        //        userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

        //        paramsSchoolEducation = SetParamsSchoolEducation(user, startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
        //            "1,0", dtParams, Config.ComplaintType.Complaint,
        //            Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

        //        queryStr = queryStr + StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
        //        listVmUserWiseStatus.Add(MakeEmptyStatusModel(user, listDbStatuses));
        //    }
        //    if (!string.IsNullOrEmpty(queryStr))
        //    {
        //        ds = DBHelper.GetDataSetByQueryString(queryStr, null);
        //        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
        //        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                    statusWiseComplaintData, true);
        //        return statusWiseComplaintData;
        //    }
        //    return null;
        //}

        public static VmUserWiseStatus MakeEmptyStatusModel(DbUsers dbUser, List<DbStatus> listStatus)
        {
            VmUserWiseStatus vmUserWiseStatus = new VmUserWiseStatus();
            vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();
            VmStatusCount vmStatusCount = null;


            vmUserWiseStatus.UserId = dbUser.User_Id;
            //vmUserWiseStatus.Name = (dbUser.Name!=null) ? dbUser.Name : dbUser.Username;
            //vmUserWiseStatus.Name = (dbUser.Designation_abbr != null)
            //    ? vmUserWiseStatus.Name.Trim() + " [" + dbUser.Designation_abbr.Trim() + "]"
            //    : vmUserWiseStatus.Name.Trim();

            //-------- Adding new code -------
            string hierarchyVal = DbUsers.GetHierarchyVal(dbUser);
            vmUserWiseStatus.Name = (dbUser.Designation_abbr != null)
               ? hierarchyVal + " [" + dbUser.Designation_abbr.Trim() + "]"
               : hierarchyVal;
            //--------------------------------

            foreach (DbStatus dbStatus in listStatus)
            {
                vmStatusCount = new VmStatusCount();
                vmStatusCount.StatusId = dbStatus.Complaint_Status_Id;
                vmStatusCount.StatusString = Utility.GetAlteredStatus(dbStatus.Status, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                vmStatusCount.Count = 0;
                vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
            }
            return vmUserWiseStatus;
        }

        private static VmUserWiseStatus MakeEmptyTeachingQualityModel(KeyValuePair<string, string> regionKeyValuePair, List<DbDynamicCategories> listDynamicCategories)
        {
            VmUserWiseStatus vmUserWiseStatus = new VmUserWiseStatus();
            vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();
            VmStatusCount vmStatusCount = null;

            vmUserWiseStatus.UserId = Convert.ToInt32(regionKeyValuePair.Key.Split(',')[0]);
            vmUserWiseStatus.Name = regionKeyValuePair.Value;

            foreach (DbDynamicCategories dbDynamicCategories in listDynamicCategories)
            {
                vmStatusCount = new VmStatusCount();
                vmStatusCount.StatusId = dbDynamicCategories.Id;
                vmStatusCount.StatusString = dbDynamicCategories.Name; //GetAlteredStatus(dbStatus.Status);
                vmStatusCount.Count = 0;
                vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
            }
            return vmUserWiseStatus;
        }

        public static VmStatusWiseComplaintsData GetUserStatusWiseComplaintData(DataSet dataSet, List<VmUserWiseStatus> listVmUserWiseStatus)
        {
            VmStatusWiseComplaintsData vmStatusWiseComplaintData = new VmStatusWiseComplaintsData();
            vmStatusWiseComplaintData.ListUserWiseData = new List<VmUserWiseStatus>();
            VmUserWiseStatus vmUserWiseStatus = null;
            VmStatusCount vmStatusCount = null;
            VmUserWiseStatus vmUserWiseStatusToMerge = null;

            bool isUserPresent = false;
            int i = 0;
            foreach (DataTable dt in dataSet.Tables)
            {
                isUserPresent = false;
                vmUserWiseStatusToMerge = listVmUserWiseStatus[i];
                //vmStatusWiseComplaintData.ListUserWiseData = new List<VmUserWiseStatus>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!isUserPresent)
                        {
                            vmUserWiseStatus = new VmUserWiseStatus();
                            vmUserWiseStatus.UserId = Convert.ToInt32(row["User_Id"]);
                            vmUserWiseStatus.Name = vmUserWiseStatusToMerge.Name;
                            vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();

                            vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatus);
                            isUserPresent = true;
                        }

                        vmStatusCount = new VmStatusCount();
                        vmStatusCount.StatusId = Convert.ToInt32(row["Complaint_Computed_Status_Id"]);
                        vmStatusCount.StatusString = Utility.GetAlteredStatus(row["Complaint_Computed_Status"].ToString(), Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                        vmStatusCount.Count = Convert.ToInt32(row["Count"]);
                        vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
                    }
                    Utility.MergeLists(vmUserWiseStatus.ListVmStatusWiseCount, vmUserWiseStatusToMerge.ListVmStatusWiseCount);
                }
                else
                {
                    vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatusToMerge);
                }
                i++;
            }
            return vmStatusWiseComplaintData;
        }


        private static VmStatusWiseComplaintsData GetTeachingQualityComplaintData(DataSet dataSet, List<VmUserWiseStatus> listVmUserWiseStatus)
        {
            VmStatusWiseComplaintsData vmStatusWiseComplaintData = new VmStatusWiseComplaintsData();
            vmStatusWiseComplaintData.ListUserWiseData = listVmUserWiseStatus;
            //VmUserWiseStatus vmUserWiseStatus = null;
            VmStatusCount vmStatusCount = null;
            //VmUserWiseStatus vmUserWiseStatusToMerge = null;

            bool isUserPresent = false;
            int userId = -1, statusId = -1;
            //int i = 0;
            foreach (DataTable dt in dataSet.Tables)
            {
                //isUserPresent = false;
                //vmUserWiseStatusToMerge = listVmUserWiseStatus[i];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        //if (!isUserPresent)
                        {
                            //vmUserWiseStatus = new VmUserWiseStatus();
                            userId = Convert.ToInt32(row["RegionId"]);
                            statusId = Convert.ToInt32(row["CategoryTypeId"]);
                            if (statusId != -1)
                            {
                                vmStatusCount =
                                    vmStatusWiseComplaintData.ListUserWiseData.Where(n => n.UserId == userId)
                                        .FirstOrDefault()
                                        .ListVmStatusWiseCount.Where(m => m.StatusId == statusId)
                                        .FirstOrDefault();
                                vmStatusCount.Count = Convert.ToInt32(row["Count"]);
                            }
                            //vmUserWiseStatus.UserId = Convert.ToInt32(row["RegionId"]);
                            //vmUserWiseStatus.Name = vmUserWiseStatusToMerge.Name;
                            //vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();

                            //vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatus);
                            //isUserPresent = true;
                        }

                        //vmStatusCount = new VmStatusCount();
                        //vmStatusCount.StatusId = Convert.ToInt32(row["CategoryTypeId"]);
                        //vmStatusCount.StatusString = GetAlteredStatus(row["FieldValue"].ToString());
                        //vmStatusCount.Count = Convert.ToInt32(row["Count"]);
                        //vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
                    }
                    //MergeLists(vmUserWiseStatus.ListVmStatusWiseCount, vmUserWiseStatusToMerge.ListVmStatusWiseCount);
                }
                //else
                //{
                //    //MergeLists(vmUserWiseStatus.ListVmStatusWiseCount, vmUserWiseStatusToMerge.ListVmStatusWiseCount);
                //    vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatusToMerge);
                //}
                //i++;
            }
            return vmStatusWiseComplaintData;
        }

        //-------- One Time Functions ----------

        public static Config.CommandMessage AssignComplaintsToUsers(VmAddComplaint complaintModel, bool isProfileEditing, bool isComplaintEditing)
        {
            Config.CommandMessage commandMessage = new Config.CommandMessage();
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                //------ Commenting code ----------
                List<DbUsers> listDbUsers =
                    db.DbUsers.Where(m => m.Campaign_Id == (int)Config.Campaign.SchoolEducationEnhanced &&
                                         m.Cnic != null && m.IsActive && m.Username == m.Cnic).OrderBy(n => n.User_Id).ToList();


                //---------------------------------

                //List<DbUsers> listDbUsers = db.DbUsers.Where(n => listUserIdsToSendMessage.Contains(n.User_Id)).ToList();

                int? userCat1;
                bool userCat2;
                DbUsers dbUser = null;

                List<int> listUsersToRegisterComplaint = new List<int>();
                for (int i = 0; i < listDbUsers.Count; i++)
                {
                    dbUser = listDbUsers[i];
                    complaintModel.ComplaintVm.departmentId = 2;
                    complaintModel.ComplaintVm.Complaint_Category = 272;
                    complaintModel.ComplaintVm.Complaint_SubCategory = 1046;
                    //complaintModel.ComplaintVm.Complaint_Type = Config.ComplaintType.Complaint;
                    complaintModel.ComplaintVm.Province_Id = Utility.GetIntByCommaSepStr(dbUser.Province_Id);
                    complaintModel.ComplaintVm.Division_Id = Utility.GetIntByCommaSepStr(dbUser.Division_Id);
                    complaintModel.ComplaintVm.District_Id = Utility.GetIntByCommaSepStr(dbUser.District_Id);

                    userCat1 = dbUser.UserCategoryId1;
                    userCat2 = dbUser.UserCategoryId2 == 1 ? true : false;

                    DbSchoolsMapping dbSchoolMapping = DbSchoolsMapping.GetById(userCat1, userCat2, Utility.GetIntByCommaSepStr(dbUser.UnionCouncil_Id));
                    VmDynamicLabel vmDynamicLabel = null;
                    if (dbSchoolMapping != null)
                    {
                        complaintModel.ComplaintVm.ListDynamicDropDownServerSide[0].SelectedItemId = dbSchoolMapping.Id.ToString() + Config.Separator + "[" + dbSchoolMapping.school_emis_code + "] " + dbSchoolMapping.school_name;

                        vmDynamicLabel = complaintModel.ComplaintVm.ListDynamicLabel[0];
                        vmDynamicLabel.FieldValue = DbTehsil.GetById((int)dbSchoolMapping.System_Tehsil_Id).Tehsil_Name;

                        vmDynamicLabel = complaintModel.ComplaintVm.ListDynamicLabel[1];
                        vmDynamicLabel.FieldValue = DbUnionCouncils.GetById((int)dbSchoolMapping.System_Markaz_Id).Councils_Name;

                        vmDynamicLabel = complaintModel.ComplaintVm.ListDynamicLabel[2];
                        vmDynamicLabel.FieldValue = dbSchoolMapping.school_emis_code;

                        vmDynamicLabel = complaintModel.ComplaintVm.ListDynamicLabel[3];
                        vmDynamicLabel.FieldValue = Convert.ToBoolean(dbSchoolMapping.System_School_Gender) ? "Male" : "Female";

                        vmDynamicLabel = complaintModel.ComplaintVm.ListDynamicLabel[4];
                        vmDynamicLabel.FieldValue = dbSchoolMapping.school_level;

                        vmDynamicLabel = complaintModel.ComplaintVm.ListDynamicLabel[5];
                        if (Convert.ToInt32(dbSchoolMapping.School_Type) == (int)Config.SchoolType.Elementary)
                        {
                            vmDynamicLabel.FieldValue = "Elementary";
                        }
                        else if (Convert.ToInt32(dbSchoolMapping.School_Type) == (int)Config.SchoolType.Primary)
                        {
                            vmDynamicLabel.FieldValue = "Primary";
                        }
                        else if (Convert.ToInt32(dbSchoolMapping.School_Type) == (int)Config.SchoolType.Secondary)
                        {
                            vmDynamicLabel.FieldValue = "Secondary";
                        }
                        //schoolId = Convert.ToInt32(complaintModel.ComplaintVm.ListDynamicDropDownServerSide[0].SelectedItemId.Split(new string[] { Config.Separator }, StringSplitOptions.None)[0]);

                        commandMessage = BlSchool.AddComplaint(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0), (complaintModel.ComplaintVm.Id != 0));

                        SmsModel smsModel = null;
                        string messageStr = "Dear Sir/Madam, \n" +
                       "Govt. of Punjab is launching an Education Hotline (042-111-11-20-20) for parents. Complaints related to schools coming under your Markaz will be assigned to you. \n " +
                       "As a test case, a complaint has been assigned to you. Please download the application from “https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver” and resolve the complaint using your login ID/password (already provided). In case of technical issues, " +
                       "please email at hotline@sed.punjab.gov.pk. \n" +
                       "School Education Department, Government of the Punjab";

                        smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, dbUser.Phone, messageStr,
                       (int)Config.MsgType.ToStakeholder,
                       (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
                        TextMessageHandler.SendMessageToPhoneNo(dbUser.Phone, messageStr, true, smsModel);

                        listUsersToRegisterComplaint.Add(dbUser.User_Id);
                    }

                    //complaintModel.ComplaintVm.
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return commandMessage;
        }

        public static void SendMessageToSEAllUsers()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                //------ Commenting code ----------
                List<DbUsers> listDbUsers = db.DbUsers.Where(m => m.Campaign_Id == (int)Config.Campaign.SchoolEducationEnhanced &&
                m.Cnic != null && m.IsActive && m.Username == m.Cnic).ToList();
                //---------------------------------

                //List<DbUsers> listDbUsers = db.DbUsers.Where(n => listUserIdsToSendMessage.Contains(n.User_Id)).ToList();

                string messageStr = "Dear Sir/Madam, \n" +
                "This is to inform you that SED Hotline user login has been updated to your CNIC number (Without dashes). Password remains unchanged. \n " +
                "https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver";

                SmsModel smsModel = null;
                for (int i = 0; i < listDbUsers.Count; i++)
                {
                    //listDbUsers[i].Phone = "0331-4714621";
                    //listDbUsers[i].Phone = "0300-4449123";
                    //listDbUsers[i].Phone = "0321-4226005";
                    //listDbUsers[i].Phone = "0321-5123483";
                    smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, listDbUsers[i].Phone, messageStr,
                    (int)Config.MsgType.ToStakeholder,
                    (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
                    TextMessageHandler.SendMessageToPhoneNo(listDbUsers[i].Phone, messageStr, true, smsModel);

                }

            }
            catch (Exception ex)
            {
                return;
            }
        }

        //        UPDATE PITB.Complaints
        //SET SrcId1 = Origin_HierarchyId,
        //UserSrcId1 = Origin_UserHierarchyId,
        //UserCategoryId1 = Origin_UserCategoryId1,
        //UserCategoryId2 = Origin_UserCategoryId2,
        //MaxSrcId = Origin_HierarchyId,
        //MinSrcId = Origin_HierarchyId
        //WHERE id = 198716


        private static List<int> listUserIdsToSendMessage = new List<int>()
        {
            10819,
            10820,
            10821,
            10822,
            10823,
            10824,
            10825,
            10826,
            10827,
            10828,
            10829,
            10830,
            10831,
            10832,
            10833,
            10834,
            10835,
            10836,
            10837,
            10838,
            10839,
            10840,
            10841,
            10842,
            10843,
            10844,
            10845,
            10846,
            10847,
            10848,
            10849,
            10850,
            10851,
            10852,
            10853,
            10854,
            10855,
            10856,
            10857,
            10858
        };


        //--------------------------------------  
        #endregion

        //public static DataTable GetComplaintsOfStakeholderServerSide(string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        //{
        //    string extraSelection = "complaints.RefField1, complaints.RefField2, complaints.RefField3, complaints.RefField4, complaints.RefField5, complaints.RefField6, complaints.Person_Cnic,";

        //    CMSCookie cookie = new AuthenticationHandler().CmsCookie;
        //    Dictionary<string, object> paramDict = new Dictionary<string, object>();
        //    paramDict.Add("@StartRow", (dtParams.Start).ToDbObj());
        //    paramDict.Add("@EndRow", (dtParams.End).ToDbObj());
        //    paramDict.Add("@OrderByColumnName", dtParams.ListOrder[0].columnName.ToDbObj());
        //    paramDict.Add("@OrderByDirection", dtParams.ListOrder[0].sortingDirectionStr.ToDbObj());

        //    paramDict.Add("@WhereOfMultiSearch", dtParams.WhereOfMultiSearch.ToDbObj());

        //    paramDict.Add("@From", fromDate.ToDbObj());
        //    paramDict.Add("@To", toDate.ToDbObj());
        //    paramDict.Add("@Campaign", campaign.ToDbObj());
        //    paramDict.Add("@Category", category.ToDbObj());
        //    paramDict.Add("@Status", complaintStatuses.ToDbObj());
        //    paramDict.Add("@TransferedStatus", commaSeperatedTransferedStatus.ToDbObj());
        //    paramDict.Add("@ComplaintType", (Convert.ToInt32(complaintType)).ToDbObj());
        //    paramDict.Add("@UserHierarchyId", Convert.ToInt32(cookie.Hierarchy_Id).ToDbObj());
        //    paramDict.Add("@UserDesignationHierarchyId", Convert.ToInt32(cookie.User_Hierarchy_Id).ToDbObj());
        //    paramDict.Add("@ListingType", Convert.ToInt32(listingType).ToDbObj());
        //    paramDict.Add("@ProvinceId", (cookie.ProvinceId).ToDbObj());
        //    paramDict.Add("@DivisionId", (cookie.DivisionId).ToDbObj());
        //    paramDict.Add("@DistrictId", (cookie.DistrictId).ToDbObj());

        //    paramDict.Add("@Tehsil", (cookie.TehsilId).ToDbObj());
        //    paramDict.Add("@UcId", (cookie.UcId).ToDbObj());
        //    paramDict.Add("@WardId", (cookie.WardId).ToDbObj());

        //    paramDict.Add("@UserId", cookie.UserId.ToDbObj());
        //    paramDict.Add("@UserCategoryId1", cookie.UserCategoryId1.ToDbObj());
        //    paramDict.Add("@UserCategoryId2", cookie.UserCategoryId2.ToDbObj());
        //    paramDict.Add("@CheckIfExistInSrcId", 1);
        //    paramDict.Add("@CheckIfExistInUserSrcId", 1);
        //    paramDict.Add("@SelectionFields", extraSelection);
        //    paramDict.Add("@SpType", spType);

        //    return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints_ServerSide]", paramDict);
        //}
        public static void SendUsernameAndPasswordInSMSToNewlyCreatedUsers()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                IEnumerable<DbUsers> list = db.DbUsers.Where(x => x.User_Id >= 35238);
                if (list != null)
                {
                    if (list.Count() > 0)
                    {
                        foreach (DbUsers user in list)
                        {
                            user.Password = Utility.GetAutoGeneratedPassword(8, new List<Config.PasswordProperty>() { Config.PasswordProperty.Numbers});
                            user.Password_Updated = DateTime.Now;
                            user.PasswordChangedDate = DateTime.Now.Date;
                            if (!String.IsNullOrEmpty(user.Phone) && !String.IsNullOrEmpty(user.Password))
                            {
                                string smsText = @"Respected Sir/Madam, \n\nPlease resolve complaints received on 
                                School Education Hotline (042-111-11-2020) by logging on crm.punjab.gov.pk using 
                                following credentials: \n\nUsername: {0}\nPassword: {1}\nYou should change your 
                                password on logging in, and keep it safe with you.\n\nSchool Education Department\n
                                Government of Punjab";
                                smsText = String.Format(smsText, user.Username, user.Password);
                                string messageStr = "Respected Sir/Madam, \n" + "\n" +
                                "Please resolve complaints received on School Education Hotline (042-111-11-2020) by logging on crm.punjab.gov.pk using following credentials: " + "\n" + "\n" +
                                "Username: Your CNIC Number (without dashes)" + "\n" +
                                "Password: " + user.Password + "\n" +
                                "You should change your password on logging in, and keep it safe with you." + "\n" + "\n" +
                                "School Education Department" + "\n" +
                                "Government of Punjab";
                                SmsModel sms = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, user.Phone, smsText, (int)Config.MsgType.ToStakeholder, (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameAndPasswordNotification);
                                System.Threading.Thread childThread = new Thread(()=> TextMessageHandler.SendMessageToPhoneNo(user.Phone,smsText));
                                childThread.Start();
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void ChangePassWordAndSendSMSToUsers()
        {
            try
            {

                DBContextHelperLinq db = new DBContextHelperLinq();
                IEnumerable<DbUsers> lstUsers = db.DbUsers.Where(x => x.Campaign_Id == (int)Config.Campaign.SchoolEducationEnhanced && x.IsActive == true);
                if (lstUsers != null)
                {
                    //IEnumerable<DbUsers> lstSortedUsers = lstUsers.Where(x => x.Password.Equals("1234", StringComparison.OrdinalIgnoreCase));
                    IEnumerable<DbUsers> lstSortedUsers = lstUsers.Where(x => (int?)x.Hierarchy_Id != 5 && (int?)x.User_Hierarchy_Id != 0 && !x.Designation_abbr.Equals("AEO"));
                    var itemLst = from element in lstSortedUsers
                                  group element by element.Username
                                      into groups
                                      select groups.OrderBy(p => p.User_Id).Last();
                    //lstSortedUsers = lstSortedUsers.PasswordCharactersCheck(new List<Config.PasswordProperty> { Config.PasswordProperty.Characters });
                    string lastPassword = "";
                    foreach (DbUsers user in itemLst)
                    {
                        string path = @"d:\Passwords Folder\PasswordsList.txt";
                        string password = null;//Utility.GetAutoGeneratedPassword(10, new List<Config.PasswordProperty> { Config.PasswordProperty.Numbers});
                        string UserIdPath = @"d:\Passwords Folder\IdsList.txt";
                        //user.Password = password;
                        //user.Password_Updated = DateTime.Now;

                        if (!System.IO.File.Exists(UserIdPath))
                        {
                            using (System.IO.StreamWriter sw = System.IO.File.CreateText(UserIdPath))
                            {
                                sw.WriteLine("User ID : " + user.User_Id);

                            }

                        }
                        else
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(UserIdPath, true))
                            {
                                sw.WriteLine("User ID : " + user.User_Id);

                            }
                        }


                        if (lastPassword.Equals(password))
                        {
                            if (!System.IO.File.Exists(path))
                            {
                                using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine("Same Passwords created");
                                    sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                                }

                            }
                            else
                            {
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                                {
                                    sw.WriteLine("Same Passwords created");
                                    sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                                }
                            }
                        }


                        if (!System.IO.File.Exists(path))
                        {
                            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
                            {
                                sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                            }

                        }
                        else
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                            {

                                sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                            }
                        }
                        if (!String.IsNullOrEmpty(user.Phone) && !String.IsNullOrEmpty(user.Password))
                        {
                            string messageStr = "Respected Sir/Madam, \n" + "\n" +
                            "Please resolve complaints received on School Education Hotline (042-111-11-2020) by logging on crm.punjab.gov.pk using following credentials: " + "\n" + "\n" +
                            "Username: Your CNIC Number (without dashes)" + "\n" +
                            "Password: " + user.Password + "\n" +
                            "You should change your password on logging in, and keep it safe with you." + "\n" + "\n" +
                            "School Education Department" + "\n" +
                            "Government of Punjab";

                            SmsModel smsModel = null;
                            smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, user.Phone, messageStr,
                            (int)Config.MsgType.ToStakeholder,
                            (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
                            new Thread(delegate()
                            {
                                TextMessageHandler.SendMessageToPhoneNo(user.Phone, messageStr, true, smsModel);

                            }).Start();
                        }
                    }
                }
                int q = 0;
                //db.SaveChanges();
            }
            catch (Exception ex)
            {
                return;
            }
            return;
        }
        public static void SendPasswordToUserPhone()
        {
            //Mahmood Saab Phone number : 03419001001
            //Waqas Muzaffar Phone number : 03424011710

            string[] phone = new string[] { "03419001001" };
            string password = "0923424011710";
            string messageStr = "Dear Sir/Madam, \n" +
                            "This is to inform you that your SED Hotline user password is " + password + " \n " +
                            "https://crm.punjab.gov.pk" + "\n" +
                            "https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver";

            string messageStr2 = "Dear Sir/Madam, \n" +
                            "Please resolve complaints received on School Education Hotline (042-111-11-2020) by logging on https://www.crm.punjab.gov.pk  using following credentials: " + "\n" +
                            "Username: Your CNIC number (without dashes)" + "\n" +
                            "Password: 0923424011710" + "\n" +
                            "School Education Department" + "\n" +
                            "Government of Punjab";

            string messageStr3 = "Dear Sir/Madam, \n" +
                            "Please resolve complaints received on School Education Hotline (042-111-11-2020) by logging in SIS App using following credentials: " + "\n" +
                            "Username: Your CNIC number (without dashes)" + "\n" +
                            "Password: 0923424011710" + "\n" +
                            "School Education Department" + "\n" +
                            "Government of Punjab";
            string messageStr4 = "Respected AEO, \n" + "\n" +
                            "Please resolve complaints received on School Education Hotline (042-111-11-2020) by logging in SIS App using following credentials: " + "\n" + "\n" +
                            "Username: Your CNIC Number (without dashes)" + "\n" +
                            "Password: 0923424011710" + "\n" +
                            "App link:" + "\n" +
                            "https://play.google.com/store/apps/details?id=pk.gov.pitb.sis" + "\n" +
                            "You should change your password on logging in, and keep it safe with you." + "\n" + "\n" +
                            "School Education Department" + "\n" +
                            "Government of Punjab";
            string messageStr5 = "Respected Sir/Madam, \n" + "\n" +
                            "Please resolve complaints received on School Education Hotline (042-111-11-2020) by logging on crm.punjab.gov.pk using following credentials: " + "\n" + "\n" +
                            "Username: Your CNIC Number (without dashes)" + "\n" +
                            "Password: " + "0902938383838" + "\n" +
                            "You should change your password on logging in, and keep it safe with you." + "\n" + "\n" +
                            "School Education Department" + "\n" +
                            "Government of Punjab";
            string[] msg = new string[] { messageStr5 };
            //for(int i=0; i < msg.Length-1; i++){
            //    for (int j = 0; j < phone.Length-1; j++)
            //    {
            //        SmsModel smsModel = null;
            //        smsModel = new SmsModel((int)CMS.Config.Campaign.SchoolEducationEnhanced, phone[j], msg[i],
            //        (int)Config.MsgType.ToStakeholder,
            //        (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
            //        new Thread(delegate()
            //        {
            //            TextMessageHandler.SendMessageToPhoneNo(phone[j], msg[i], true, smsModel);

            //        }).Start();
            //    }

            //}                        
            int i = 0;
            int j = 0;
            SmsModel smsModel = null;
            smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, phone[j], msg[i],
            (int)Config.MsgType.ToStakeholder,
            (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
            new Thread(delegate()
            {
                TextMessageHandler.SendMessageToPhoneNo(phone[j], msg[i], true, smsModel);

            }).Start();
        }
        public static void CreateUserAndSendSMSToUsers()
        {
            try
            {

                DBContextHelperLinq db = new DBContextHelperLinq();
                IEnumerable<DbUsers> lstUsers = db.DbUsers.Where(x => x.Campaign_Id == (int)Config.Campaign.SchoolEducationEnhanced && x.IsActive == true);
                if (lstUsers != null)
                {
                    //IEnumerable<DbUsers> lstSortedUsers = lstUsers.Where(x => x.Password.Equals("1234", StringComparison.OrdinalIgnoreCase));
                    IEnumerable<DbUsers> lstSortedUsers = lstUsers.Where(x => x.User_Id == 31098 || x.User_Id == 31099 || x.User_Id == 19605);
                    string lastPassword = "";
                    foreach (DbUsers user in lstSortedUsers)
                    {
                        string path = @"d:\Passwords Folder\PasswordsList.txt";
                        string password = Utility.GetAutoGeneratedPassword(10, new List<Config.PasswordProperty> { Config.PasswordProperty.Numbers, Config.PasswordProperty.AlphabetsLowerCase, Config.PasswordProperty.AlphabetsUpperCase });
                        user.Password = password;
                        user.Password_Updated = DateTime.Now;
                        if (lastPassword.Equals(password))
                        {
                            if (!System.IO.File.Exists(path))
                            {
                                using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine("Same Passwords created");
                                    sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                                }

                            }
                            else
                            {
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                                {
                                    sw.WriteLine("Same Passwords created");
                                    sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                                }
                            }
                        }


                        if (!System.IO.File.Exists(path))
                        {
                            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
                            {
                                sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                            }

                        }
                        else
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                            {

                                sw.WriteLine("Username : " + user.Username + " ,New Password : " + user.Password);
                            }
                        }
                        if (!String.IsNullOrEmpty(user.Phone) && !String.IsNullOrEmpty(user.Password))
                        {
                            string messageStrWithoutUsername = "Dear Sir/Madam,\n" +
                            "This is to inform you that your SED Hotline user has been created.\nUsername: CNIC without dashes\n" +
                            "Password: " + user.Password + "\nWebsite: http://crm.punjab.gov.pk \nAndroidApp: https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver";

                            string messageStrWithUsername = "Dear Sir/Madam,\n" +
                            "This is to inform you that your SED Hotline user has been created.\nUsername: " + user.Username + "\n" +
                            "Password: " + user.Password + "\nWebsite: http://crm.punjab.gov.pk \nAndroidApp: https://play.google.com/store/apps/details?id=pk.gov.pitb.schooleducationresolver";

                            SmsModel smsModel = null;
                            smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, user.Phone, messageStrWithoutUsername,
                            (int)Config.MsgType.ToStakeholder,
                            (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholderUsernameChangeReminder);
                            new Thread(delegate()
                            {
                                TextMessageHandler.SendMessageToPhoneNo(user.Phone, messageStrWithoutUsername, true, smsModel);

                            }).Start();
                        }
                    }
                }
                int q = 0;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return;
            }
            return;
        }
        #region Assignemnt

        public static void AssignComplaintToOrignalHierarchy(DBContextHelperLinq db, DbComplaint dbComplaint)
        {
            //DbComplaintsOriginLog dbComplaintOriginLog = null;

            //db.Entry(dbComplaint).State = EntityState.Modified;

            dbComplaint.SrcId1 = dbComplaint.Origin_HierarchyId;
            db.Entry(dbComplaint).Property(n => n.SrcId1).IsModified = true;

            dbComplaint.UserSrcId1 = dbComplaint.Origin_UserHierarchyId;
            db.Entry(dbComplaint).Property(n => n.UserSrcId1).IsModified = true;

            dbComplaint.UserCategoryId1 = dbComplaint.Origin_UserCategoryId1;
            db.Entry(dbComplaint).Property(n => n.UserCategoryId1).IsModified = true;

            dbComplaint.UserCategoryId2 = dbComplaint.Origin_UserCategoryId2;
            db.Entry(dbComplaint).Property(n => n.UserCategoryId2).IsModified = true;

            dbComplaint.MaxSrcId = dbComplaint.Origin_HierarchyId;
            db.Entry(dbComplaint).Property(n => n.MaxSrcId).IsModified = true;

            dbComplaint.MinSrcId = dbComplaint.Origin_HierarchyId;
            db.Entry(dbComplaint).Property(n => n.MinSrcId).IsModified = true;

            dbComplaint.Is_AssignedToOrigin = true;
            db.Entry(dbComplaint).Property(n => n.Is_AssignedToOrigin).IsModified = true;

            SaveOrignalHierarchyLogInDb(db, dbComplaint);

        }

        public static void SaveOrignalHierarchyLogInDb(DBContextHelperLinq db, DbComplaint dbComplaint)
        {
            DbComplaintsOriginLog dbComplaintOriginLog = new DbComplaintsOriginLog();
            dbComplaintOriginLog.Complaint_Id = dbComplaint.Id;
            dbComplaintOriginLog.Origin_HierarchyId = dbComplaint.Origin_HierarchyId;
            dbComplaintOriginLog.Origin_UserHierarchyId = dbComplaint.Origin_UserHierarchyId;
            dbComplaintOriginLog.Origin_UserCategoryId1 = dbComplaint.Origin_UserCategoryId1;
            dbComplaintOriginLog.Origin_UserCategoryId2 = dbComplaint.Origin_UserCategoryId2;
            dbComplaintOriginLog.Source_Id = (int)Config.SourceType.Service;

            dbComplaintOriginLog.Event_Id = (int)Config.Events.ComplaintAssignToOrigin;
            dbComplaintOriginLog.Created_DateTime = DateTime.Now;
            dbComplaintOriginLog.Is_AssignedToOrigin = dbComplaint.Is_AssignedToOrigin;
            dbComplaintOriginLog.IsCurrentlyActive = true;

            DbComplaintsOriginLog.SaveComplaintsOriginHistoryLog(dbComplaintOriginLog, db);
        }




        #endregion

    }


}