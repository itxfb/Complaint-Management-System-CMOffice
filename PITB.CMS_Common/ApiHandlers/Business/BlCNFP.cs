using PITB.CMS_Common.ApiModels.API.CNFP;
using PITB.CMS_Common.Helper.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiHandlers.Business
{
    public class BlCNFP
    {
        public static CNFPApiModel.Response.PostFeedback SetCnfpComplainantFeedback(CNFPApiModel.Request.PostFeedback reqPostFeedback)
        {
            CNFPApiModel.Response.PostFeedback respPostFeedback = new CNFPApiModel.Response.PostFeedback();
            Dictionary<string, object> paramDict = new Dictionary<string, object>();


            CNFPApiModel.Response.PostFeedback.PostFeedbackResponse respFeedback = null;
            if (reqPostFeedback.listFeedBack != null)
            {
                foreach (CNFPApiModel.Request.PostFeedback.Feedback cnfpFeedbackModel in reqPostFeedback.listFeedBack)
                {
                    try
                    {
                        paramDict.Clear();
                        paramDict.Add("@Feedback_Id", cnfpFeedbackModel.feedbackId.ToDbObj());
                        paramDict.Add("@Feedback_Value", cnfpFeedbackModel.feedbackVal);
                        paramDict.Add("@Complaint_Id", cnfpFeedbackModel.complaintId);
                        paramDict.Add("@Feedback_Comments", cnfpFeedbackModel.feedbackCommments);
                        paramDict.Add("@Feedback_DateTime",
                            Utility.GetDateTimeStr(cnfpFeedbackModel.feedbackDateTime, "yyyy-MM-dd HH:mm") /*DateTime.Now*/);
                        DBHelper.CrudOperation("[PITB].[Update_CNFP_Feedback]", paramDict);
                        respFeedback =
                            new CNFPApiModel.Response.PostFeedback.PostFeedbackResponse(cnfpFeedbackModel.complaintId);
                        respPostFeedback.listFeedBackResponse.Add(respFeedback);
                    }
                    catch (Exception ex)
                    {
                        respFeedback.SetFailure();
                        respPostFeedback.listFeedBackResponse.Add(respFeedback);
                        respPostFeedback.SetFailure();
                        //throw;
                    }
                }
            }
            return respPostFeedback;
        }
    }
}