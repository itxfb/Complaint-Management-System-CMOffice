using PITB.CMS_Common.ApiHandlers.Translation;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.ApiHandlers.Business
{
    public class BlApiStakeholderHander
    {
        public static StakeholderComplaintsResponse GetStakeHolderComplaintsServerSideByUserName(string userName, string statuses, int startingRowIndex, Config.Language language, Config.PlatformID platformId)
        {
            try
            {
                /*
                if (platformId == Config.PlatformID.IOS)
                {
                    userName = "10000000000";
                }*/

                DbUsers dbUser = DbUsers.GetUserAgainstUserName(userName);

                int campaignId = Convert.ToInt32(dbUser.Campaign_Id);
                //List<DbStatuses> listStatuses = DbStatuses.GetByCampaignId(campaignId);
                //string statusesStr = String.Join(",", listStatuses.Select(n => n.Id));

                if (dbUser != null && dbUser.Role_Id ==  Config.Roles.Stakeholder)
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

                    List<StakeholderComplaint> listStakeholderComplaints = dt.ToList<StakeholderComplaint>();
                    int totalRows = (listStakeholderComplaints != null && listStakeholderComplaints.Count > 0)
                        ? listStakeholderComplaints[0].Total_Rows : 0;
                    foreach (StakeholderComplaint complaint in listStakeholderComplaints)
                    {
                        complaint.ListAttachments = DbAttachments.GetByRefAndComplaintId(complaint.Complaint_Id, Config.AttachmentReferenceType.Add, complaint.Complaint_Id);
                    }
                    StakeholderComplaintsResponse complaintResponse = new StakeholderComplaintsResponse();
                    complaintResponse.ListStakeholderComplaint = listStakeholderComplaints;
                    
                    Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
                    complaintResponse.ListStakeholderComplaint.GetTranslatedList<StakeholderComplaint>(@"Complaint_Category_Name,Complaint_SubCategory_Name,Complaint_Computed_Status,Campaign_Name", translationDict, language);
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
    }
}