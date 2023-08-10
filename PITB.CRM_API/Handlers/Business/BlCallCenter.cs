using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using PITB.CRM_API.Models.API.CallCenter;

namespace PITB.CRM_API.Handlers.Business
{
    public class BlCallCenter
    {
        public static CallCenterModel.Response.SubmitLandedCallLogs SubmitCallLandedStats(CallCenterModel.Request.SubmitLandedCallLogs request)
        {
            DbCampaignWiseCallLogs dbCampaignWiseCallLogs;
            DBContextHelperLinq db = new DBContextHelperLinq();
            List<CallCenterModel.Request.SubmitLandedCallLogs.Log> listCallLog = request.ListLogs;
            Dictionary<string,string> dictSessionIds = DbCampaignWiseCallLogs.GetSessionIdsDict();
            CallCenterModel.Request.SubmitLandedCallLogs.Log log;

            string value = null;
            int updatedRecords = 0;
            for (int i = 0; i < request.ListLogs.Count; i++)
            {
                log = request.ListLogs[i];
                if (!dictSessionIds.TryGetValue(log.Session_id, out value))
                {
                    dictSessionIds.Add(log.Session_id, log.PhoneNo);
                    
                    dbCampaignWiseCallLogs = new DbCampaignWiseCallLogs();


                    dbCampaignWiseCallLogs.Campaign_Id = log.CampaignId;
                    dbCampaignWiseCallLogs.Phone_No = log.PhoneNo;
                    dbCampaignWiseCallLogs.Call_DateTime = log.LandedDateTime;
                    dbCampaignWiseCallLogs.Created_DateTime = DateTime.Now;
                    dbCampaignWiseCallLogs.Session_Id = log.Session_id;
                    db.DbCampaignWiseCallLogs.Add(dbCampaignWiseCallLogs);
                    updatedRecords++;
                }
            }
            db.SaveChanges();
            CallCenterModel.Response.SubmitLandedCallLogs resp = new CallCenterModel.Response.SubmitLandedCallLogs();
            resp.UpdatedRecords = updatedRecords;
            resp.SetSuccess();
            return resp;
        }
    }
}