using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Helper.Database;
using System.Data;
using PITB.CMS_Common.Models.Custom;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlMaps
    {
        public static List<MapData> GetComplaintsStatusData(int campaignId,int statusId)
        {
            string query = @"SELECT District_Id As [Id],District_Name As 'Name',Count(*) AS [Count] FROM             PITB.Complaints where Compaign_Id = {0}
                    AND Complaint_Computed_Status_Id = {1}
                    Group By District_Id,District_Name
                    Order By District_Id";
            string queryStr = string.Format(query, campaignId, statusId);
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr,null);
            List<MapData> data = dt.ToList<MapData>();
            return data;
        }
        public static List<MapData> GetComplaintsStatusData(string campaignId, string statusId,string startDate,string endDate)
        {
            string query = @"SELECT District_Id As [Id],District_Name As 'Name',Count(*) AS [dataValue] FROM             PITB.Complaints where Compaign_Id IN ({0})
                    AND Complaint_Computed_Status_Id IN ({1})
                    AND (CONVERT(DATE,Created_Date,120)) BETWEEN '{2}' AND '{3}'
                    Group By District_Id,District_Name
                    Order By District_Id";
            string queryStr = string.Format(query, campaignId, statusId, startDate, endDate);
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null); 
            List<MapData> data = dt.ToList<MapData>();
            return data;
        }
    }
}