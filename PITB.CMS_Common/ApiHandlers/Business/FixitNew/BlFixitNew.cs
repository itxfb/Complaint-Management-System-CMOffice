using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using PITB.CMS_Common.ApiModels.API.FixitNew.CFNP;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Helper.Database;

namespace PITB.CMS_Common.ApiHandlers.Business.FixitNew
{
    public class BlFixitNew
    {
        public static ResponseCNFPComplaintsModel GetCnfpComplaints(RequestCNFPComplaintsModel reqCnfpModel)
            //string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            ResponseCNFPComplaintsModel respCnfpModel = new ResponseCNFPComplaintsModel();

            reqCnfpModel.FromDate = DateTime.ParseExact(reqCnfpModel.from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            reqCnfpModel.ToDate = DateTime.ParseExact(reqCnfpModel.to, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            reqCnfpModel.ToDate = reqCnfpModel.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);


            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                List<int?> listNewFixit = Utility.GetNullableIntList(Config.ListNewFixitCampaings);
                
                //DBContextHelperLinq db = new DBContextHelperLinq();
                List<ComplaintCNFPDbModel> complaintCNFPModel= db.DbComplaints.Where(m => m.StatusChangedDate_Time >= reqCnfpModel.FromDate 
                    && m.StatusChangedDate_Time <= reqCnfpModel.ToDate &&
                    listNewFixit.Contains(m.Compaign_Id) && 
                    m.Complaint_Computed_Status_Id == Config.NewFixitResolvedStatusId)
                    .Select(s=> new ComplaintCNFPDbModel()
                    {
                        complaint_resolve_date = s.StatusChangedDate_Time,
                        complaint_number = s.Id,
                        first_name = s.Person_Name,
                        last_name = "",
                        phone_num = s.Person_Contact,
                        
                        district_Id = s.District_Id,
                        district = s.District_Name,
                        
                        tehsil_Id = s.Tehsil_Id,
                        tehsil = s.Tehsil_Name,

                        uc_Id = s.UnionCouncil_Id,
                        uc_no = s.UnionCouncil_Name,

                        service_Id = s.Compaign_Id,
                        service = s.Campaign_Name,

                        sub_Service_Id = s.Complaint_Category,
                        sub_service = s.Complaint_Category_Name

                    }).ToList();

                respCnfpModel.ListResult = complaintCNFPModel;
            }

            return respCnfpModel;

        }

        public static ApiStatus SetCnfpComplainantFeedback (RequestCNFPComplaintFeedbackModel reqCnfpFeedbackModel)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            foreach (CNFPComplaintFeedbackModel cnfpFeedbackModel in reqCnfpFeedbackModel.listFeedBack)
            {
                paramDict.Add("@Feedback_Id", cnfpFeedbackModel.feedbackId.ToDbObj());

                paramDict.Add("@Feedback_Value", cnfpFeedbackModel.feedbackVal);

                paramDict.Add("@Complaint_Id", cnfpFeedbackModel.complaintId);
                paramDict.Add("@Feedback_Comments", cnfpFeedbackModel.feedbackCommments);
                paramDict.Add("@Feedback_DateTime", DateTime.Now);
                DBHelper.CrudOperation("[PITB].[Update_CNFP_Feedback]", paramDict);
            }

            return new ApiStatus(Config.ResponseType.Success.ToString(),"Feedback Submitted");
        }


    }

}