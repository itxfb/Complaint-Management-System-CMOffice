using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PITB.CMS.Handler.Complaint;
using PITB.CMS.Models.DB;
using PITB.CRM_API.Handlers.Translation;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Models.API.Authentication;
using PITB.CRM_API.Models.API.SchoolEducation;
using PITB.CRM_API.Models.API.SchoolEducation.Resolver;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;
using PITB.CMS.Models.Custom;
using PITB.CRM_API.Handler.Messages;
using DbAttachments = PITB.CRM_API.Models.DB.DbAttachments;
using DbPermissionsAssignment = PITB.CRM_API.Models.DB.DbPermissionsAssignment;
using DbUsers = PITB.CRM_API.Models.DB.DbUsers;


namespace PITB.CRM_API.Handlers.Business.SchoolEducation
{
    public class BlSchoolEducationResolver
    {

        public static SISApiModel.Response.SEStakeholderComplaintResponseModel GetStakeHolderComplaintsServerSideByUserNameDynamic(
            string userName, string statuses, int startingRowIndex, string from, string to, Config.Language language
            /*, Config.PlatformID platformId*/)
            //string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            try
            {
                CMS.Models.DB.DbUsers dbUser = CMS.Models.DB.DbUsers.GetUserAgainstUserName(userName);
                ListingParamsModelBase paramsSchoolEducation = SetStakeholderListingParams(dbUser, startingRowIndex,
                    from, to, dbUser.Campaigns, dbUser.Categories, statuses, "1,0", Config.ComplaintType.Complaint,
                    Config.StakeholderComplaintListingType.AssignedToMe, "MobileListingWithDateFilters");
                /*
                if (platformId == Config.PlatformID.IOS)
                {
                    userName = "10000000000";
                }*/

                //DbUsers dbUser = DbUsers.GetByUserName(userName);

                int campaignId = Convert.ToInt32(dbUser.Campaign_Id);
                //List<DbStatuses> listStatuses = DbStatuses.GetByCampaignId(campaignId);
                //string statusesStr = String.Join(",", listStatuses.Select(n => n.Id));

                if (dbUser != null && dbUser.Role_Id == CMS.Config.Roles.Stakeholder)
                {
                    //Dictionary<string, object> paramDict = new Dictionary<string, object>();
                    //paramDict.Add("@StartRow", (startingRowIndex).ToDbObj());
                    //paramDict.Add("@Campaign", dbUser.Campaigns.ToDbObj());
                    //paramDict.Add("@Category", dbUser.Categories.ToDbObj());
                    //paramDict.Add("@Status", statuses.ToDbObj());
                    //paramDict.Add("@ComplaintType", (Convert.ToInt32(Config.ComplaintType.Complaint)).ToDbObj());
                    //paramDict.Add("@UserHierarchyId", Convert.ToInt32(dbUser.Hierarchy_Id).ToDbObj());
                    //paramDict.Add("@UserDesignationHierarchyId", Convert.ToInt32(dbUser.User_Hierarchy_Id).ToDbObj());
                    //paramDict.Add("@ProvinceId", (dbUser.Province_Id).ToDbObj());
                    //paramDict.Add("@DivisionId", (dbUser.Division_Id).ToDbObj());
                    //paramDict.Add("@DistrictId", (dbUser.District_Id).ToDbObj());

                    //paramDict.Add("@Tehsil", (dbUser.Tehsil_Id).ToDbObj());
                    //paramDict.Add("@UcId", (dbUser.UnionCouncil_Id).ToDbObj());
                    //paramDict.Add("@WardId", (dbUser.Ward_Id).ToDbObj());

                    //// paramDict.Add("@UserId", dbUser.Id.ToDbObj());
                    //List<DbAttachments> listAttachments = null;
                    //DataTable dt =
                    //    DBHelper.GetDataTableByStoredProcedure(
                    //        "[PITB].[Get_Stakeholder_Complaints_Service_ServerSide]", paramDict);
                    string queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);



                    DataTable dt = PITB.CMS.Helper.Database.DBHelper.GetDataTableByQueryString(queryStr, null);
                    List<SISApiModel.Response.SEStakeholderComplaint> listStakeholderComplaints = dt.ToList<SISApiModel.Response.SEStakeholderComplaint>();


                    //List<StakeholderComplaint> listStakeholderComplaints = dt.ToList<StakeholderComplaint>();
                    int totalRows = (listStakeholderComplaints != null && listStakeholderComplaints.Count > 0)
                        ? listStakeholderComplaints[0].Total_Rows
                        : 0;
                    foreach (SISApiModel.Response.SEStakeholderComplaint complaint in listStakeholderComplaints)
                    {
                        complaint.ListAttachments = DbAttachments.GetByRefAndComplaintId(complaint.Complaint_Id,
                            Config.AttachmentReferenceType.Add, complaint.Complaint_Id);
                        complaint.StatusHistory = BlCommons.PopulateStatusHistory(complaint.Complaint_Id);

                    }
                    SISApiModel.Response.SEStakeholderComplaintResponseModel complaintResponse = new SISApiModel.Response.SEStakeholderComplaintResponseModel();
                    complaintResponse.ListStakeholderComplaint = listStakeholderComplaints;

                    Dictionary<string, TranslatedModel> translationDict =
                        PITB.CRM_API.Models.DB.DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(
                            PITB.CRM_API.Models.DB.DbTranslationMapping.GetAllTranslation());
                    complaintResponse.ListStakeholderComplaint.GetTranslatedList<SISApiModel.Response.SEStakeholderComplaint>(
                        @"Complaint_Category_Name,Complaint_SubCategory_Name,Complaint_Computed_Status,Campaign_Name",
                        translationDict, language);
                    //complaintResponse.ListStakeholderComplaint.GetTranslatedList<StakeholderComplaint>("Complaint_Category_Name", translationDict, language);

                    complaintResponse.Total_Rows = totalRows;
                    SetAlteredStatus(complaintResponse.ListStakeholderComplaint);
                    return complaintResponse;
                }
            }

            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public static ListingParamsModelBase SetStakeholderListingParams(CMS.Models.DB.DbUsers dbUser, int startRow, string fromDate,
            string toDate, string campaign, string category, string complaintStatuses,
            string commaSeperatedTransferedStatus, Config.ComplaintType complaintType,
            Config.StakeholderComplaintListingType listingType, string spType)
        {
            //string extraSelection = "complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status,Latitude,Longitude,LocationArea, Computed_Remaining_Time_To_Escalate,Computed_Remaining_Total_Time,";
            string extraSelection =
                "complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status, complaints.RefField1 SchoolEmsiCode, complaints.RefField2 SchoolName, complaints.RefField3 SchoolLevel, complaints.RefField4 SchoolType, complaints.RefField5 SchoolGender, complaints.RefField6 SchoolMarkazName,";

            /*
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            */
            ListingParamsModelBase paramsModel = new ListingParamsModelBase();
            paramsModel.StartRow = startRow;
            paramsModel.EndRow = startRow + (Config.PageSizeMobileLWMC - 1);
            //paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
            //paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
            //paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;

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

            //paramsModel.UserId = dbUser.Id;
            //paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            //paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            //paramsModel.CheckIfExistInSrcId = 0;
            //paramsModel.CheckIfExistInUserSrcId = 0;
            //paramsModel.SelectionFields = extraSelection;
            //paramsModel.SpType = spType;


            paramsModel.UserId = dbUser.User_Id;
            paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            paramsModel.ListUserCategory = UserCategoryModel.GetListUserCategoryModel(dbUser.ListDbUserCategory);
            paramsModel.CheckIfExistInSrcId = 1;
            paramsModel.CheckIfExistInUserSrcId = 1;
            paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;

            if (dbUser.SubRole_Id == CMS.Config.SubRoles.SDU &&
                listingType == Config.StakeholderComplaintListingType.AssignedToMe)
            {
                paramsModel.IgnoreComputedHierarchyCheck = true;
                paramsModel.SelectionFields =
                    @" schoolMap.Assigned_To, schoolMap.Assigned_To_Name,CONVERT(VARCHAR(10),complaints.StatusChangedDate_Time,120) StatusChangedDate_Time ," +
                    paramsModel.SelectionFields;
                paramsModel.InnerJoinLogic = @" INNER JOIN pitb.School_Category_User_Mapping schoolMap
					ON complaints.Complaint_SubCategory=schoolMap.Category_Id ";
                paramsModel.WhereLogic = " AND (schoolMap.Assigned_To = " + (int) Config.SchoolEducationUserSubRoles.SDU +
                                         " OR (complaints.Status_ChangedBy=" + Config.MEALoginId +
                                         "AND complaints.Complaint_Status_Id=" +
                                         (int) Config.ComplaintStatus.ResolvedVerified + ") ) ";
            }
            else
            {
                paramsModel.IgnoreComputedHierarchyCheck = false;
            }

            return paramsModel;
        }

        public static SISApiModel.Response.SEStakeholderStatusesModel GetStakeholderValidStatuses(string userName, Config.Language language,
            Config.AppID appId, Config.PlatformID platformId, int appVersionId)
        {
            SISApiModel.Response.SEStakeholderStatusesModel shStatusModel = new SISApiModel.Response.SEStakeholderStatusesModel();


            DbUsers dbUser = DbUsers.GetByUserName(userName);
            if (dbUser != null)
            {
                //DbUsersVersionMapping.Update_AddVersion(Config.UserType.Resolver, dbUser.Id, (int)platformId, (int)appId, appVersionId);

                //List<DbStatuses> listDbStatuses = DbStatuses.GetByCampaignId(Convert.ToInt32(dbUser.Campaigns));

                // Status filter 
                List<DbPermissionsAssignment> listDbPermissionsAssignment = DbPermissionsAssignment
                    .GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int) Config.PermissionsType.User, dbUser.Id,
                        (int) Config.Permissions.StatusesForComplaintListing);

                List<DbStatuses> listDbStatuses =
                    GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.Id,
                        listDbPermissionsAssignment);

                Dictionary<string, TranslatedModel> translationDict =
                    PITB.CRM_API.Models.DB.DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(
                        PITB.CRM_API.Models.DB.DbTranslationMapping.GetAllTranslation());
                listDbStatuses.GetTranslatedList<DbStatuses>("Status", translationDict, language);

                //StatusList statusList = new StatusList(listDbStatuses);

                shStatusModel.ListFilterStatus = listDbStatuses;
                //return statusList;


                // Status compaint assigned to me
                DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                    ((int)Config.PermissionsType.User, dbUser.Id, (int)Config.Permissions.StakeholderStatusesOnStatusChangeView
                    ).FirstOrDefault();
                //DbPermissions dbPermission =
                //DbPermissions.GetPermissionsByPermissionAndType(
                // (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
                List<CRM_API.Models.DB.DbStatuses> listDbStatus = CRM_API.Models.DB.DbStatuses.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
                shStatusModel.ListAssignableStatus = listDbStatus;
                SetAlteredStatus(shStatusModel.ListAssignableStatus);
                SetAlteredStatus(shStatusModel.ListFilterStatus);
            }
            shStatusModel.SetSuccess();
            return shStatusModel;
        }

        public static void SetAlteredStatus(List<SISApiModel.Response.SEStakeholderComplaint> ListStakeholderComplaint )
        {
            foreach (SISApiModel.Response.SEStakeholderComplaint shComplaint in ListStakeholderComplaint)
            {
                shComplaint.Complaint_Computed_Status = GetAlterStatusStr(shComplaint.Complaint_Computed_Status);
                //shComplaint.StatusHistory = GetAlterStatusStr(shComplaint.Complaint_Computed_Status);
            }

        }

        public static void SetAlteredStatus(List<DbStatuses> listDbStatuses)
        {
            foreach (DbStatuses dbStatus in listDbStatuses)
            {
                dbStatus.Status = GetAlterStatusStr(dbStatus.Status);
            }
           
        }

        public static string GetAlterStatusStr(string statusStr)
        {
            if (statusStr == Config.UnsatisfactoryClosedStatus)
            {
                return Config.SchoolEducationUnsatisfactoryStatus;
            }
            return statusStr;
        }
        public static List<DbStatuses> GetStatusListByCampaignIdsAndPermissions(List<int> listCampaignIds, int userId, List<DbPermissionsAssignment> listPermissions)
        {
            DbPermissionsAssignment dbPermission = listPermissions
                                    .FirstOrDefault(
                                        n =>
                                            n.Type == (int)Config.PermissionsType.User &&
                                            n.Type_Id == userId &&
                                            n.Permission_Id == (int)Config.Permissions.StatusesForComplaintListing);

            List<DbStatuses> listDbStatuses = null;

            if (dbPermission == null)
            {
                listDbStatuses = DbStatuses.GetByCampaignIds(listCampaignIds);
            }
            else
            {
                List<int> listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                List<Config.ComplaintStatus> statusList = listStatuses.Select(status => (Config.ComplaintStatus)(status)).ToList();
                listDbStatuses = DbStatuses.GetByStatusIds(statusList);
            }

            return listDbStatuses;
        }

        public static SEResponseStakeholderLogin SubmitStakeholderLogin(SISApiModel.Request.SubmitSEStakeholderLogin submitStakeHolderLogin, Config.PlatformID platformId)
        {

            DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            DbUsers dbUserTemp = null;

            // For IOS platform
            /*if (platformId == Config.PlatformID.IOS)
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbUsers> listDbUser = DbUsers.GetByCnic("3520256571439", db);
                foreach (DbUsers dbUser in listDbUser)
                {
                    dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                    db.DbUsers.Attach(dbUser);
                    db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    dbUserTemp = dbUser;
                }
                if (dbUserTemp != null)
                {
                    TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                }

                return new ResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
            }
             */
            // End for IOS platform


            if (isUsernamePresent) // username and cnic present
            {
                string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLogin.Username);
                if (imeiNo == null) // if imei not registered then register it
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //listDbUser.Add(user);
                    //---- end old code ----

                    List<DbUsers> listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    foreach (DbUsers dbUser in listDbUser)
                    {
                        dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                        db.DbUsers.Attach(dbUser);
                        db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                        db.SaveChanges();
                        dbUserTemp = dbUser;
                    }
                    if (dbUserTemp != null)
                    {
                        TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                    }

                    return new SEResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //---- end old code ----
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    return new SEResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "This user is already registered on another device"));
                }
            }
            else // if username not present
            {
                return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
            }

        }

        public static SEResponseStakeholderLogin SubmitStakeholderLoginImeiNoRestriction(SISApiModel.Request.SubmitSEStakeholderLogin submitStakeHolderLogin, Config.PlatformID platformId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            //DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password, db).FirstOrDefault();
            DbUsers user = DbUsers.GetByUserName(submitStakeHolderLogin.Username);
            bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            DbUsers dbUserTemp = null;

            // For IOS platform
            /*if (platformId == Config.PlatformID.IOS)
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbUsers> listDbUser = DbUsers.GetByCnic("3520256571439", db);
                foreach (DbUsers dbUser in listDbUser)
                {
                    dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                    db.DbUsers.Attach(dbUser);
                    db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    dbUserTemp = dbUser;
                }
                if (dbUserTemp != null)
                {
                    TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                }

                return new ResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
            }
             */
            // End for IOS platform


            if (isUsernamePresent) // username and cnic present
            {
                string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLogin.Username);
                //if (imeiNo == null) // if imei not registered then register it
                {
                    
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //listDbUser.Add(user);
                    //---- end old code ----

                    List<DbUsers> listDbUser = null;//DbUsers.GetByCnic(user.Cnic, db);
                    if(user.Cnic==null)
                    {
                        listDbUser = new List<DbUsers>();
                        listDbUser.Add(user);
                        //listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    }
                    else
                    {
                        listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    }
                    //listDbUser.Add(user);

                    for (int i = 0; i < listDbUser.Count; i++)
                    {
                        listDbUser[i].Imei_No = submitStakeHolderLogin.ImeiNo;
                        db.DbUsers.Attach(listDbUser[i]);
                        db.Entry(listDbUser[i]).Property(x => x.Imei_No).IsModified = true;

                        dbUserTemp = listDbUser[i];
                    }
                    db.SaveChanges();
                    if (dbUserTemp != null)
                    {
                        TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                    }

                    return new SEResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                /*else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //---- end old code ----
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    return new SEResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "This user is already registered on another device"));
                }*/
            }
            else // if username not present
            {
                return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
            }

        }

        public static SISApiModel.Response.SISGetUsers GetUsers(SISApiModel.Request.SISGetUsers submitStakeHolderLogin, Config.PlatformID platformId)
        {
            PITB.CMS.Helper.Database.DBContextHelperLinq db = new PITB.CMS.Helper.Database.DBContextHelperLinq();
            //DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password, db).FirstOrDefault();
            PITB.CMS.Models.DB.DbUsers user = PITB.CMS.Models.DB.DbUsers.GetUserAgainstUserNameAndPassword(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
            DbUsers dbUserTemp = null;

            if (isUsernamePresent) // username and cnic present
            {
                //string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLogin.Username);
              
                //---- old code ----
                //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                //listDbUser.Add(user);
                //---- end old code ----

                List<PITB.CMS.Models.DB.DbUsers> listDbUser = null;//DbUsers.GetByCnic(user.Cnic, db);
                if (user.Cnic == null)
                {
                    listDbUser = new List<PITB.CMS.Models.DB.DbUsers>();
                    listDbUser.Add(user);
                    //listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                }
                else
                {
                    listDbUser = PITB.CMS.Models.DB.DbUsers.GetByCnic(user.Cnic, db);
                }
                //listDbUser.Add(user);

                //for (int i = 0; i < listDbUser.Count; i++)
                //{
                //    db.DbUsers.Attach(listDbUser[i]);
                //    db.Entry(listDbUser[i]).Property(x => x.Imei_No).IsModified = true;

                //    dbUserTemp = listDbUser[i];
                //}
                //db.SaveChanges();
                //if (dbUserTemp != null)
                //{
                //    TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                //}

                return new SISApiModel.Response.SISGetUsers(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logged in."));
            }
            else // if username not present
            {
                return new SISApiModel.Response.SISGetUsers(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
            }

        }
    }
}