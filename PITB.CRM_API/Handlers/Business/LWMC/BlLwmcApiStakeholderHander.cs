using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PITB.CMS;
using PITB.CMS.Handler.Complaint;
using PITB.CMS.Handler.Complaint.Status;
using PITB.CRM_API.Handlers.Translation;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;
using PITB.CMS.Models.Custom;

namespace PITB.CRM_API.Handlers.Business.LWMC
{
    public class BlLwmcApiStakeholderHander
    {
        public static LwmcStakeholderComplaintResponse GetStakeHolderComplaintsServerSideByUserName(string userName, string statuses, int startingRowIndex, Config.Language language, Config.PlatformID platformId)
        {
            try
            {
                /*
                if (platformId == Config.PlatformID.IOS)
                {
                    userName = "10000000000";
                }*/

                DbUsers dbUser = DbUsers.GetByUserName(userName);

                int campaignId = Convert.ToInt32(dbUser.Campaign_Id);
                //List<DbStatuses> listStatuses = DbStatuses.GetByCampaignId(campaignId);
                //string statusesStr = String.Join(",", listStatuses.Select(n => n.Id));

                if (dbUser != null && dbUser.Role_Id == Config.Roles.Stakeholder)
                {
                    Dictionary<string, object> paramDict = new Dictionary<string, object>();
                    paramDict.Add("@StartRow", (startingRowIndex).ToDbObj());
                    paramDict.Add("@Campaign", dbUser.Campaigns.ToDbObj());
                    paramDict.Add("@Category", dbUser.Categories.ToDbObj());
                    paramDict.Add("@Status", statuses.ToDbObj());
                    paramDict.Add("@ComplaintType", (Convert.ToInt32(Config.ComplaintType.Complaint)).ToDbObj());
                    paramDict.Add("@UserHierarchyId", Convert.ToInt32(dbUser.Hierarchy_Id).ToDbObj());
                    paramDict.Add("@UserDesignationHierarchyId", Convert.ToInt32(dbUser.User_Hierarchy_Id).ToDbObj());
                    paramDict.Add("@ProvinceId", (dbUser.Province_Id).ToDbObj());
                    paramDict.Add("@DivisionId", (dbUser.Division_Id).ToDbObj());
                    paramDict.Add("@DistrictId", (dbUser.District_Id).ToDbObj());

                    paramDict.Add("@Tehsil", (dbUser.Tehsil_Id).ToDbObj());
                    paramDict.Add("@UcId", (dbUser.UnionCouncil_Id).ToDbObj());
                    paramDict.Add("@WardId", (dbUser.Ward_Id).ToDbObj());

                    // paramDict.Add("@UserId", dbUser.Id.ToDbObj());
                    List<DbAttachments> listAttachments = null;
                    DataTable dt =
                        DBHelper.GetDataTableByStoredProcedure(
                            "[PITB].[Get_Stakeholder_Complaints_Service_ServerSide]", paramDict);

                    List<LwmcStakeholderComplaint> listStakeholderComplaints = dt.ToList<LwmcStakeholderComplaint>();
                    int totalRows = (listStakeholderComplaints != null && listStakeholderComplaints.Count > 0)
                        ? listStakeholderComplaints[0].Total_Rows : 0;
                    foreach (LwmcStakeholderComplaint complaint in listStakeholderComplaints)
                    {
                        complaint.Resolver.Stakeholder_Comments = complaint.Stakeholder_Comments;
                        complaint.Resolver.Complaint_Status_Id = complaint.Complaint_Computed_Status_Id;
                        complaint.Resolver.Complaint_Status = complaint.Complaint_Computed_Status;

                        complaint.ListAttachments = DbAttachments.GetByRefAndComplaintId(complaint.Complaint_Id, Config.AttachmentReferenceType.Add, complaint.Complaint_Id);
                        complaint.Resolver.ListAttachments = DbAttachments.GetByRefAndComplaintId(complaint.Complaint_Id, Config.AttachmentReferenceType.ChangeStatus);
                        var dbSocialSharing = DbSocialSharing.GetByComplaintId(complaint.Complaint_Id);
                        if (dbSocialSharing != null)
                        {
                            complaint.Facebook_PostID = dbSocialSharing.Post_Id;
                        }

                    }
                    LwmcStakeholderComplaintResponse complaintResponse = new LwmcStakeholderComplaintResponse();
                    complaintResponse.ListStakeholderComplaint = listStakeholderComplaints;
                    
                    Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
                    complaintResponse.ListStakeholderComplaint.GetTranslatedList<LwmcStakeholderComplaint>(@"Complaint_Category_Name,Complaint_SubCategory_Name,Complaint_Computed_Status,Campaign_Name", translationDict, language);
                    //complaintResponse.ListStakeholderComplaint.GetTranslatedList<StakeholderComplaint>("Complaint_Category_Name", translationDict, language);

                    complaintResponse.Total_Rows = totalRows;
                    return complaintResponse;
                }
            }

            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public static LwmcStakeholderComplaintResponse GetStakeHolderComplaintsServerSideByUserNameDynamic(string fcmKey, string userName, string statuses, int startingRowIndex, string from, string to, Config.Language language, Config.PlatformID platformId) //string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            try
            {
                DbUsers dbUser = DbUsers.GetByUserName(userName);
                if (fcmKey != null)
                {
                    // Register Device Id
                    //CMS.Models.DB.DbUsers dbUsers = CMS.Models.DB.DbUsers.GetUser(submitStakeHolderLogin.Username, submitStakeHolderLogin.Cnic);
                    string updateCommand = @"UPDATE PITB.User_Wise_Devices
                                        SET Is_Active = 0
                                        WHERE USER_ID = @UserId
                                        
                                        INSERT INTO PITB.User_Wise_Devices
                                        ( User_Id ,
                                          Platform_Id ,
                                          Tag_Id ,
                                          Device_Id ,
                                          Is_Active
                                        )
                                    VALUES  ( @UserId , -- User_Id - int
                                          @Platform_Id , -- Platform_Id - int
                                          @Tag_Id , -- Tag_Id - nvarchar(max)
                                          @Device_Id , -- Device_Id - nvarchar(max)
                                          @Is_Active  -- Is_Active - bit
                                        )

                                        ";
                    Dictionary<string, object> dictParams = new Dictionary<string, object>();
                    dictParams.Add("@UserId", dbUser.Id);
                    dictParams.Add("@Platform_Id", Config.PlatformID.Android.ToDbObj());
                    dictParams.Add("@Tag_Id", "Campaign::47__Type::User__Platform::Android");
                    dictParams.Add("@Device_Id", fcmKey);
                    dictParams.Add("@Is_Active", 1);
                    //dictParams.Add("@UserId", dbUsers.User_Id);
                    CMS.Helper.Database.DBHelper.GetDataTableByQueryString(updateCommand, dictParams);

                }


                ListingParamsModelBase paramsSchoolEducation = null;//= SetStakeholderListingParams(dbUser, startingRowIndex, from, to, dbUser.Campaigns, dbUser.Categories, statuses,"1,0", Config.ComplaintType.Complaint, PITB.CMS.Config.StakeholderComplaintListingType.AssignedToMe, "MobileListingWithDateFilters");

                if (dbUser.User_Hierarchy_Id > 30)
                {
                    paramsSchoolEducation = SetStakeholderListingParams(dbUser, startingRowIndex, from, to, dbUser.Campaigns, dbUser.Categories, statuses, "1,0", Config.ComplaintType.Complaint, PITB.CMS.Config.StakeholderComplaintListingType.UptilMyHierarchy, "MobileListingWithDateFilters");
                }
                else
                {
                    paramsSchoolEducation = SetStakeholderListingParams(dbUser, startingRowIndex, from, to, dbUser.Campaigns, dbUser.Categories, statuses, "1,0", Config.ComplaintType.Complaint, PITB.CMS.Config.StakeholderComplaintListingType.AssignedToMe, "MobileListingWithDateFilters");
                }
                

                /*
                if (platformId == Config.PlatformID.IOS)
                {
                    userName = "10000000000";
                }*/

                //DbUsers dbUser = DbUsers.GetByUserName(userName);

                int campaignId = Convert.ToInt32(dbUser.Campaign_Id);
                //List<DbStatuses> listStatuses = DbStatuses.GetByCampaignId(campaignId);
                //string statusesStr = String.Join(",", listStatuses.Select(n => n.Id));

                if (dbUser != null && dbUser.Role_Id == Config.Roles.Stakeholder)
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
                    List<LwmcStakeholderComplaint> listStakeholderComplaints = dt.ToList<LwmcStakeholderComplaint>();
                    int totalRows = (listStakeholderComplaints != null && listStakeholderComplaints.Count > 0)
                        ? listStakeholderComplaints[0].Total_Rows : 0;

                    // Get status History
                    List<DbComplaintStatusChangeLog> listStatusChangeLogs = DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintIds(Utility.GetNullableIntList(listStakeholderComplaints.Select(n => n.Complaint_Id).ToList()));
                    
                    // End Get status History

                    foreach (LwmcStakeholderComplaint complaint in listStakeholderComplaints)
                    {
                        List<DbComplaintStatusChangeLog> listComplaintStatusChangeLogs = listStatusChangeLogs.Where(n => n.Complaint_Id == complaint.Complaint_Id).ToList();
                        complaint.ReopenedCount =
                            listComplaintStatusChangeLogs.Where(n => n.StatusId == (int) Config.ComplaintStatus.Resolved)
                                .Count();
                        // Populate History
                        complaint.ListLogHistory = new List<LogHistory>();
                        foreach (DbComplaintStatusChangeLog dbStatusLog in listComplaintStatusChangeLogs)
                        {
                            
                            LogHistory logHistory = new LogHistory();
                            logHistory.ComplaintId = Convert.ToInt32(dbStatusLog.Complaint_Id);
                        
                            logHistory.StatusChangeComments = dbStatusLog.Comments;
                            logHistory.StatusChangeDateTime = dbStatusLog.StatusChangeDateTime.ToString("dd/MM/yyyy hh:mm tt");
                            logHistory.StatusId = Convert.ToByte(dbStatusLog.StatusId);
                            logHistory.StatusChangedByUserName = dbStatusLog.ChangedBy.Name;
                            logHistory.StatusChangedByUserContact = dbStatusLog.ChangedBy.Phone;
                            logHistory.StatusChangedByUserHierarchy = ((Config.LwmcResolverHirarchyId)Convert.ToByte(dbStatusLog.ChangedBy.User_Hierarchy_Id)).ToString();
                            logHistory.Status = ((Config.ComplaintStatus)logHistory.StatusId).GetDisplayName();
                            logHistory.ListAttachments = dbStatusLog.ListDbAttachments;
                            complaint.ListLogHistory.Add(logHistory);

                        }
                        // End Populate History

                        // Change created Date Format
                        //Convert.ToInt32(BlApiHandler.StoreApiRequestInDb("test123", "119.160.101.172", false, complaint.Created_Date));
                        complaint.Created_Date = CMS.Utility.GetDateTimeStr(complaint.Created_Date, "dd/MM/yyyy");

                        complaint.Resolver.Stakeholder_Comments = complaint.Stakeholder_Comments;
                        complaint.Resolver.Complaint_Status_Id = complaint.Complaint_Computed_Status_Id;
                        complaint.Resolver.Complaint_Status = complaint.Complaint_Computed_Status;

                        complaint.ListAttachments = DbAttachments.GetByRefAndComplaintId(complaint.Complaint_Id, Config.AttachmentReferenceType.Add, complaint.Complaint_Id);
                        complaint.Resolver.ListAttachments = DbAttachments.GetByRefAndComplaintId(complaint.Complaint_Id, Config.AttachmentReferenceType.ChangeStatus);
                        var dbSocialSharing = DbSocialSharing.GetByComplaintId(complaint.Complaint_Id);
                        if (dbSocialSharing != null)
                        {
                            complaint.Facebook_PostID = dbSocialSharing.Post_Id;
                        }
                        complaint.CanUpdateStatus = false;
                        complaint.CanUpdateStatus = Int32.Parse(complaint.Complaint_Computed_Status_Id) == 12 ? true : false;
                        if (Int32.Parse(complaint.Complaint_Status_Id) == 12 && Int32.Parse(complaint.Complaint_Computed_Status_Id) == 6)
                        {
                            complaint.CanUpdateStatus = true;
                        }
                    }
                    LwmcStakeholderComplaintResponse complaintResponse = new LwmcStakeholderComplaintResponse();
                    complaintResponse.ListStakeholderComplaint = listStakeholderComplaints;

                    Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
                    complaintResponse.ListStakeholderComplaint.GetTranslatedList<LwmcStakeholderComplaint>(@"Complaint_Category_Name,Complaint_SubCategory_Name,Complaint_Computed_Status,Campaign_Name", translationDict, language);
                    //complaintResponse.ListStakeholderComplaint.GetTranslatedList<StakeholderComplaint>("Complaint_Category_Name", translationDict, language);

                    complaintResponse.Total_Rows = totalRows;
                    complaintResponse.SetSuccess();
                    return complaintResponse;
                }
            }

            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public static ListingParamsModelBase SetStakeholderListingParams(DbUsers dbUser, int startRow, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, Config.ComplaintType complaintType, PITB.CMS.Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = "complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status,Latitude,Longitude,LocationArea,Computed_Remaining_Total_Time,";
            
            /*
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            */
            ListingParamsModelBase paramsModel = new ListingParamsModelBase();
            paramsModel.StartRow = startRow;
            paramsModel.EndRow = startRow + (Config.PageSizeMobileLWMC-1);
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

            paramsModel.UserId = dbUser.Id;
            paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            paramsModel.CheckIfExistInSrcId = 0;
            paramsModel.CheckIfExistInUserSrcId = 0;
            paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;
            paramsModel.ListUserCategory = new List<UserCategoryModel>();
            return paramsModel;
        }

        public static LwmcStakeholderComplaintResponse GetStakeHolderComplaintsServerSideByUserNameUsingQuery(
            string userName, string statuses, int startingRowIndex, Config.Language language,
            Config.PlatformID platformId)
        {
            return null;
        }
        //public static LwmcStakeholderComplaintResponse GetStakeholderComplaintResponseByUserName(string userName, string statuses, int startingRowIndex, Config.Language language, Config.PlatformID platformId)
        //{
        //    ListingParamsModelBase paramsSchoolEducation = new ListingParamsModelBase();
        //    paramsSchoolEducation.Status = statuses;
        //    paramsSchoolEducation.StartRow = startingRowIndex;
        //    paramsSchoolEducation.SpType = "";

                
        //        //BlComplaints.SetStakeholderListingParams("", "", 
        //        //commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
        //    DbUsers dbUser = DbUsers.GetByUserName(userName);

        //    return null;
        //}

        //private static void SetListingParams(DbUsers user, ListingParamsModelBase model)
        //{
        //    model.Campaign = user.Campaigns;
        //    model.Category = user.Categories;
        //    model.ProvinceId = user.Province_Id;
        //    model.DistrictId = user.District_Id;
        //    model.DivisionId = user.Division_Id;
        //    model.Tehsil = user.Tehsil_Id;
        //    model.UcId = user.UnionCouncil_Id;
        //    model.WardId = user.Ward_Id;
        //    model.UserHierarchyId = (int)user.Hierarchy_Id;
        //    model.UserDesignationHierarchyId = user.User_Hierarchy_Id;
        //    //model.ListingType =Config.

        //}


        public static ApiStatus ChangeStatus(string username, int complaintId, int statusId, string statusComments,
            List<Picture> listPictures, Int64 apiRequestId)
        {
            bool canSendMessage = (statusId == (int) Config.ComplaintStatus.Resolved);

            return PITB.CRM_API.Handlers.Complaint.StatusHandler.ChangeStatus(username, complaintId, statusId, statusComments, listPictures, apiRequestId,
                canSendMessage);
        }

    }


}