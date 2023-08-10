using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.S3.Model;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.ApiModels.API.SchoolEducation
{
    public class SISApiModel
    {
        public class Request
        {
            public class SEStakeholderComplaintsRequestModel
            {
                public string UserName { get; set; }
                public string Statuses { get; set; }
                public int StartRowIndex { get; set; }
                //public string StartDate { get; set; }
                //public string EndDate { get; set; }
            }

            public class SubmitSchoolEducationStatusModel
            {
                public string UserName { get; set; }
                public string ComplaintId { get; set; }
                public int StatusId { get; set; }
                public string StatusComments { get; set; }

                public List<Picture> PicturesList { get; set; }
            }

            public class SubmitSEStakeholderLogin
            {
                public string Password { get; set; }
                public string Username { get; set; }
                public string ImeiNo { get; set; }
            }

            public class SEGetComplaintAgainstEmisCodes
            {
                public List<SchoolModel> ListSchoolModel { get; set; }
            }

            public class SchoolModel
            {
                public string EmisCode { get; set; }
            }

            public class SISGetUsers
            {
                public string Password { get; set; }
                public string Username { get; set; }
            }
        }

        public class Response
        {
            public class SEStakeholderStatusesModel : ApiStatus
            {
                public List<DbStatus> ListFilterStatus { get; set; }

                public List<DbStatus> ListAssignableStatus { get; set; }
            }


            public class SEStakeholderComplaintResponseModel : ApiStatus
            {
                public List<SEStakeholderComplaint> ListStakeholderComplaint { get; set; }

                public int Total_Rows { get; set; }
            }

            public class SEGetComplaintAgainstEmisCodes : ApiStatus
            {
                public List<ComplaintModel> ListComplaint { get; set; }

                public List<SchoolStatusWiseCount> ListSchoolStatusWiseCount { get; set; }

                public SEGetComplaintAgainstEmisCodes()
                {
                    this.ListSchoolStatusWiseCount = new List<SchoolStatusWiseCount>();
                }
            }

            public class ComplaintModel
            {
                public int Id { get; set; }

                public string Category { get; set; }

                public string SubCategory { get; set; }

                public string Detail { get; set; }
            }

            public class SchoolStatusWiseCount
            {
                public string SchoolEmisCode { get; set; }

                public int PendingFresh { get; set; }

                public int Overdue { get; set; }

                public int Reopened { get; set; }
            }

            public class SEStakeholderComplaint
            {
                public int Complaint_Id { get; set; }

                public string Campaign_Name { get; set; }

                public string Person_Name { get; set; }

                public string Person_Cnic { get; set; }

                public string Person_Contact { get; set; }

                public string District_Name { get; set; }

                public string Tehsil_Name { get; set; }

                public string UnionCouncil_Name { get; set; }

                public string Created_Date { get; set; }

                public string Complaint_Category_Name { get; set; }

                public string Complaint_SubCategory_Name { get; set; }

                public string Complaint_Computed_Status_Id { get; set; }

                public string Complaint_Computed_Status { get; set; }

                public string Complaint_Computed_Hierarchy { get; set; }

                public string Complaint_Remarks { get; set; }

                public int Complainant_Remark_Id { get; set; }

                public string Complainant_Remark_Str { get; set; }

                public string computed_Remaining_Time_To_Escalate { get; set; }

                public int Total_Rows { get; set; }


                public string Stakeholder_Comments { get; set; }
                //public string Complaint_Status_Id { get; set; }

                //public string Complaint_Status { get; set; }


                // School Education Fields
                public string SchoolEmsiCode { get; set; }

                public string SchoolName { get; set; }
                public string SchoolLevel { get; set; }
                public string SchoolType { get; set; }
                public string SchoolGender { get; set; }

                public string SchoolMarkazName { get; set; }

                public List<DbAttachments> ListAttachments { get; set; }

                public StatusHistory StatusHistory { get; set; }


            }

            public class SISGetUsers : ApiStatus
            {
                public List<User> ListUsers { get; set; }

                public SISGetUsers(List<DbUsers> listDbUsers, ApiStatus apiStatus)
                {

                    //List<PITB.CMS.Models.DB.DbCrmIdsMappingToOtherSystem> listDbCrmIdsMappingToOtherSystems = PITB.CMS.Models.DB.DbCrmIdsMappingToOtherSystem.Get()

                    ListUsers = new List<User>();
                    if (listDbUsers != null)
                    {
                        User loginInfo = null;

                        foreach (DbUsers dbUser in listDbUsers)
                        {
                            //List<int> listDTMSIds = new List<int>(dbUser.Province_Id, dbUser.Tehsil_Id);
                            //List<PITB.CMS.Models.DB.DbCrmIdsMappingToOtherSystem> listDbCrmIdsMappingToOtherSystems = PITB.CMS.Models.DB.DbCrmIdsMappingToOtherSystem.Get()

                            loginInfo = new User(dbUser);
                            ListUsers.Add(loginInfo);
                        }
                        this.Message = apiStatus.Message;
                        this.Status = apiStatus.Status;
                    }
                    else
                    {
                        this.Message = apiStatus.Message;
                        this.Status = apiStatus.Status;
                    }
                }

                public class User
                {
                    public string Username { get; set; }

                    public string Info { get; set; }

                    public int Role { get; set; }

                    public string PhoneNo { get; set; }

                    public string Name { get; set; }

                    public string Designation { get; set; }

                    public string DesignationAbbr { get; set; }

                    public string ProvinceId { get; set; }
                    public string DivisionId { get; set; }

                    public string DistrictId { get; set; }

                    public string TehsilId { get; set; }

                    public string UcId { get; set; }

                    public List<UserCategory> ListUserCategory { get; set; }

                    public class UserCategory
                    {
                        //public string SchoolType { get; set; }
                        //public string Gender { get; set; }

                        public int? SchoolType { get; set; }

                        public int? Gender { get; set; }

                        public UserCategory()
                        {

                        }

                        public UserCategory(DbUserCategory dbUserCategory)
                        {
                            if (dbUserCategory != null)
                            {
                                SchoolType = dbUserCategory.Parent_Category_Id;
                                Gender = dbUserCategory.Child_Category_Id;
                                //if (dbUserCategory.Parent_Category_Id == null)
                                //{
                                //    this.SchoolType = "All";
                                //}
                                //else if(dbUserCategory.Parent_Category_Id ==(int) PITB.CMS.Config.SchoolType.Primary)
                                //{
                                //    this.SchoolType = PITB.CMS.Config.SchoolType.Primary.ToString();
                                //}
                                //else if (dbUserCategory.Parent_Category_Id == (int)PITB.CMS.Config.SchoolType.Secondary)
                                //{
                                //    this.SchoolType = PITB.CMS.Config.SchoolType.Secondary.ToString();
                                //}
                                //else if (dbUserCategory.Parent_Category_Id == (int)PITB.CMS.Config.SchoolType.Elementary)
                                //{
                                //    this.SchoolType = PITB.CMS.Config.SchoolType.Elementary.ToString();
                                //}

                                //if (dbUserCategory.Child_Category_Id == null)
                                //{
                                //    this.Gender = "All";
                                //}
                                //else if (dbUserCategory.Child_Category_Id == (int)PITB.CMS.Config.Gender.Male)
                                //{
                                //    this.Gender = PITB.CMS.Config.Gender.Male.ToString();
                                //}
                                //else if (dbUserCategory.Child_Category_Id == (int)PITB.CMS.Config.Gender.Female)
                                //{
                                //    this.Gender = PITB.CMS.Config.Gender.Female.ToString();
                                //}


                                //this.Category1 = dbUserCategory.Parent_Category_Id;
                                //this.Category2 = dbUserCategory.Child_Category_Id;
                            }
                        }

                    }

                    public User(DbUsers dbUser)
                    {
                        string infoStr = "";
                        this.Username = dbUser.Username;
                        //infoStr = infoStr + DbCampaign.GetById(Convert.ToInt32(dbUser.Campaigns)).Campaign_Name+"_";
                        //infoStr = infoStr + ((Config.Hierarchy)dbUser.Hierarchy_Id).GetDisplayName() + "_";
                        //infoStr = infoStr + dbUser.Designation;
                        infoStr = infoStr +
                                Utility.GetHierarchyValueName((Config.Hierarchy)dbUser.Hierarchy_Id,
                                      Utility.GetIntByCommaSepStr(
                                          (DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser))));
                        PhoneNo = dbUser.Phone.Trim();
                        Name = dbUser.Name.Trim();
                        Designation = dbUser.Designation;


                        List<DbCrmIdsMappingToOtherSystem> listDbCrmIdsMappingToOtherSystems =
                            DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, 1,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy, Utility.GetNullableIntList(dbUser.Province_Id));
                        ProvinceId =
                            Utility.GetCommaSepStrFromList(
                                listDbCrmIdsMappingToOtherSystems.Select(n => n.OTS_Id).ToList());// dbUser.Province_Id;


                        listDbCrmIdsMappingToOtherSystems =
                            DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, 2,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy, Utility.GetNullableIntList(dbUser.Division_Id));
                        DivisionId = Utility.GetCommaSepStrFromList(
                                listDbCrmIdsMappingToOtherSystems.Select(n => n.OTS_Id).ToList()); //dbUser.Division_Id;


                        listDbCrmIdsMappingToOtherSystems =
                            DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, 3,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy, Utility.GetNullableIntList(dbUser.District_Id));
                        DistrictId = Utility.GetCommaSepStrFromList(
                                listDbCrmIdsMappingToOtherSystems.Select(n => n.OTS_Id).ToList());//User.District_Id;


                        listDbCrmIdsMappingToOtherSystems =
                            DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, 4,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy, Utility.GetNullableIntList(dbUser.Tehsil_Id));
                        TehsilId = Utility.GetCommaSepStrFromList(
                                listDbCrmIdsMappingToOtherSystems.Select(n => n.OTS_Id).ToList());//dbUser.Tehsil_Id;

                        listDbCrmIdsMappingToOtherSystems =
                            DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, 5,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy, Utility.GetNullableIntList(dbUser.UnionCouncil_Id));
                        UcId = Utility.GetCommaSepStrFromList(
                                listDbCrmIdsMappingToOtherSystems.Select(n => n.OTS_Id).ToList()); //dbUser.UnionCouncil_Id;


                        DesignationAbbr = dbUser.Designation_abbr;
                        ListUserCategory = new List<UserCategory>();
                        foreach (DbUserCategory dbUserCategory in dbUser.ListDbUserCategory)
                        {
                            ListUserCategory.Add(new UserCategory(dbUserCategory));
                        }

                        this.Info = infoStr;
                    }
                }
            }

        }
    }
}