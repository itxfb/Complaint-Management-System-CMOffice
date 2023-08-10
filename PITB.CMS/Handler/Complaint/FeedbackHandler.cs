using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace PITB.CMS.Handler.Complaint
{
    public class FeedbackHandler
    {
        public static IEnumerable<FeedbackCategoryWiseCount> GetFeedbackCategoryWiseCounts(int campaignId,string startDate,string endDate,out int feedbackTotalCount)
        {
            Array arrFeedbackEnum = Enum.GetValues(typeof(ComplaintFeedbackCategory));
            int NoOfCategories = arrFeedbackEnum.GetLength(0);
            FeedbackCategoryWiseCount[] arrfeedback = new FeedbackCategoryWiseCount[NoOfCategories];
            for(int i = 0; i < NoOfCategories; i++)
            {
                arrfeedback[i] = new FeedbackCategoryWiseCount();
                int feedbackIndex = (int)(arrFeedbackEnum.Cast<ComplaintFeedbackCategory>().ElementAt(i));
                arrfeedback[i].CategoryName = Enum.GetName(typeof(ComplaintFeedbackCategory), feedbackIndex);
            }
            DbPermissionsAssignment permissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, campaignId, (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap).FirstOrDefault();
            string statusIds = "8";
            if (permissionsAssignment != null)
            {
                var statuses = Utility.ConvertCollonFormatToDict(permissionsAssignment.Permission_Value);
                string resolved = statuses.First(x => x.Key.Equals("8")).Value.ToString();
                statusIds = resolved;
            }
            startDate = DateTime.ParseExact(startDate, "MM/dd/yyyy",new CultureInfo("en-US",true)).ToString("yyyy-MM-dd");
            endDate = DateTime.ParseExact(endDate, "MM/dd/yyyy", new CultureInfo("en-US", true)).ToString("yyyy-MM-dd");
            
            
            //------ New code 
            string query2 = string.Format("SELECT CNFP_Feedback_Id FeedbackId, COUNT(1) Count  FROM PITB.Complaints WHERE Complaint_Type = 1 AND Compaign_Id = {0} AND CONVERT(DATE,Created_Date,120) BETWEEN CONVERT(DATE,'{1}',120) AND CONVERT(DATE,'{2}',120) AND CNFP_Feedback_Id IN (1,2) GROUP BY CNFP_Feedback_Id", campaignId, startDate, endDate);
            DataTable dt2 = DBHelper.GetDataTableByQueryString(query2, null);
            List<dynamic> listFeedbackData = dt2.ToDynamicList();
            List<FeedbackCategoryWiseCount> listfeedbackCount = new List<FeedbackCategoryWiseCount>();

            listfeedbackCount.Add(new FeedbackCategoryWiseCount(0,(int)ComplaintFeedbackCategory.Satisfied, ComplaintFeedbackCategory.Satisfied.ToString(),0.0));
            listfeedbackCount.Add(new FeedbackCategoryWiseCount(0,(int)ComplaintFeedbackCategory.NotSatisfied, ComplaintFeedbackCategory.NotSatisfied.ToString(),0.0));
            FeedbackCategoryWiseCount tempFeedback;
            int totalFeedbackComplaintCount = listFeedbackData.Sum(n => n.Count);
            feedbackTotalCount = totalFeedbackComplaintCount;
            foreach (dynamic d in listFeedbackData)
            {
                tempFeedback = listfeedbackCount.Where(n=>n.CategoryId==d.FeedbackId).FirstOrDefault();
                tempFeedback.CategoryCount = d.Count;
                tempFeedback.CategoryRelativePercentage = Math.Round((d.Count*1.0/totalFeedbackComplaintCount)*100,1);
            }
           
            return listfeedbackCount;
            //------ End new code
            
            
            string query = string.Format("SELECT * FROM PITB.Complaints WHERE Complaint_Type = 1 AND Compaign_Id = {0} AND CONVERT(DATE,Created_Date,120) BETWEEN CONVERT(DATE,'{1}',120) AND CONVERT(DATE,'{2}',120) ", campaignId, startDate, endDate);
            //string query = string.Format("SELECT * FROM PITB.Complaints WHERE Complaint_Type = 1 AND Compaign_Id = {0}",campaignId);
            DataTable dt = DBHelper.GetDataTableByQueryString(query, null);
            var complaints = dt.ToList<DbComplaint>();
            feedbackTotalCount = 0;
            if (complaints != null && complaints.Count > 0)
            {
                int totalCount = complaints.Count(x => x.CNFP_Feedback_Id != null && x.CNFP_Feedback_Id != -1);
                feedbackTotalCount = totalCount;
                for (int i = 0; i < NoOfCategories; i++)
                {
                    int feedbackIndex = (int)(arrFeedbackEnum.Cast<ComplaintFeedbackCategory>().ElementAt(i));
                    int feedbackCount = 0;
                    foreach (var item in complaints)
                    {
                        if ((int)ComplaintFeedbackCategory.FeedbackPending == feedbackIndex)
                        {
                            if (item.CNFP_Feedback_Id == null)
                            {
                                feedbackCount++;
                            }
                        }
                        else
                        {
                            if (item.CNFP_Feedback_Id is int)
                            {
                                if (feedbackIndex == (int)ComplaintFeedbackCategory.FeedbackNotReceived)
                                {
                                    if (item.CNFP_Feedback_Id != (int)ComplaintFeedbackCategory.Satisfied && item.CNFP_Feedback_Id != (int)ComplaintFeedbackCategory.NotSatisfied)
                                    {
                                        feedbackCount++;
                                    }
                                }
                                else
                                {
                                    if (item.CNFP_Feedback_Id == feedbackIndex)
                                    {
                                        feedbackCount++;
                                    }
                                }
                            }
                        }
                    }
                    arrfeedback[i].CategoryCount = feedbackCount;
                    if(totalCount != 0)
                    {
                        arrfeedback[i].CategoryRelativePercentage = Math.Round((feedbackCount * 100 * 1.0) / totalCount, 1);
                    }
                    else
                    {
                        arrfeedback[i].CategoryRelativePercentage = 0.0;
                    }   
                }
            }
            return arrfeedback;
        }
    }
    public class FeedbackCategoryWiseCount
    {
        public int CategoryCount;
        public int CategoryId;
        public string CategoryName;
        public double CategoryRelativePercentage;
        public FeedbackCategoryWiseCount(int count, string name, double percentage)
        {
            CategoryCount = count;
            CategoryName = name;
            CategoryRelativePercentage = percentage;
        }

        public FeedbackCategoryWiseCount(int count, int id, string name, double percentage)
        {
            CategoryCount = count;
            CategoryId = id;
            CategoryName = name;
            CategoryRelativePercentage = percentage;
        }
        public FeedbackCategoryWiseCount()
        {
            CategoryCount = 0;
            CategoryName = null;
            CategoryRelativePercentage = 0.0;
        }
    }
    public struct FeedbackStatusWiseCount
    {
        public int StatusId;
        public string StatusName;
        public int StatusCount;
        public float StatusRelativePercentage;
        public FeedbackStatusWiseCount(int id, string name, int count, float percentage)
        {
            StatusId = id;
            StatusName = name;
            StatusCount = count;
            StatusRelativePercentage = percentage;
        }
    }
    public enum ComplaintFeedbackCategory
    {
        Satisfied = 1,
        NotSatisfied = 2,
        FeedbackNotReceived = 3,
        FeedbackPending = 4,
    }

}