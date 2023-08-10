using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.Custom;

namespace PITB.CMS.Handler.Complaint
{
    public class AgentListingLogic
    {
        public static string GetListingQuery(ListingParamsAgent listingModelAgent)
        {
            string RoleCheck = "";
            //string UserDesignationHierarchyCheck;
            string FinalSQL = "";
            string MulticolumnSearchQuery;

            MulticolumnSearchQuery = listingModelAgent.WhereOfMultiSearch + listingModelAgent.WhereLogic;
            if (listingModelAgent.RoleId == (int) Config.Roles.Agent && listingModelAgent.ListingType==(int)Config.AgentComplaintListingType.AddedByMe)
            {
                RoleCheck =  "complaints.Created_By="+listingModelAgent.UserId+" and ";
            }

            string ComplaintTypeInnerjoinCondition = "";
            string ComplaintTypeSelection = "";

            if (listingModelAgent.ComplaintType == (int) Config.ComplaintType.Complaint) //
            {
                ComplaintTypeInnerjoinCondition = " INNER JOIN pitb.Statuses Statuses ON Statuses.Id=complaints.Complaint_Computed_Status_Id ";
                ComplaintTypeSelection = " ,Statuses.[Status]";

            }
            else if (listingModelAgent.ComplaintType == (int)Config.ComplaintType.Suggestion ||
                    listingModelAgent.ComplaintType == (int)Config.ComplaintType.Inquiry)
            {
                ComplaintTypeInnerjoinCondition = " INNER JOIN pitb.Statuses Statuses ON Statuses.Id=complaints.Complaint_Computed_Status_Id ";
                ComplaintTypeSelection = " ,Statuses.[Status]";
            }

            DateTime currDate = DateTime.Now;
            


            if (listingModelAgent.SpType == "Listing")
            {
                FinalSQL = @"
		                    SELECT * from (
		                   SELECT " + listingModelAgent.SelectionFields +
                           @"complaints.Id as ComplaintNo,complaints.Compaign_Id as Campaign_Id,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, 
                            campaign.Campaign_Name Campaign_Name,personalInfo.Person_Name Person_Name,CONVERT(VARCHAR(10),
                            complaints.Created_Date,120) Created_Date,complaints.Complaint_Remarks As [Complaint_Remarks],complaints.Department_Name As [Department_Name],complaints.Complaint_Category_Name As [Complaint_Category_Name],complaints.Complaint_SubCategory_Name As [Complaint_SubCategory_Name]" + ComplaintTypeSelection + @",
		                    count(*)  OVER() AS Total_Rows,
		                    ROW_NUMBER() OVER (ORDER BY " + listingModelAgent.OrderByColumnName + " " + listingModelAgent.OrderByDirection +
                            @" ) AS RowNum
		                    FROM pitb.Complaints complaints
		                    INNER JOIN pitb.Campaign campaign ON complaints.Compaign_Id=campaign.Id
		                    INNER JOIN pitb.Complaints_Type complaintType ON complaints.Complaint_Category=complaintType.Id 
		                    INNER JOIN pitb.Person_Information personalInfo ON personalInfo.Person_id=complaints.Person_Id
		                    --INNER JOIN pitb.Statuses Statuses ON Statuses.Id=complaints.Complaint_Computed_Status_Id
		                    " + ComplaintTypeInnerjoinCondition + @"
		                    WHERE " + RoleCheck + @"
		                    CONVERT(DATE,complaints.Created_Date,120) BETWEEN CONVERT(DATE,'" + listingModelAgent.From + "') AND CONVERT(DATE,'" + listingModelAgent.To + @"') 
		                    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelAgent.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
		                    AND complaints.Complaint_Type = " + listingModelAgent.ComplaintType +
                           @"
		                     " + MulticolumnSearchQuery + @" ) as tbl
		                    WHERE tbl.RowNum BETWEEN " + listingModelAgent.StartRow + @" AND " + listingModelAgent.EndRow + @"
		                    ";
            }
            else if (listingModelAgent.SpType == "Export")
            {
                FinalSQL = @"
		                    SELECT * from (
		                    SELECT " + listingModelAgent.SelectionFields +
                           @"complaints.Id as ComplaintNo,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, campaign.Campaign_Name Campaign_Name,
                            complaints.District_Name,complaints.Tehsil_Name,                            
                            personalInfo.Person_Name Person_Name,complaints.Complaint_Remarks As [Complaint_Remarks],
                            CONVERT(VARCHAR(10),complaints.Created_Date,120) Created_Date,complaints.Department_Name As [Department_Name],
                            complaints.Complaint_Category_Name As [Complaint_Category_Name],complaints.Complaint_SubCategory_Name As [Complaint_SubCategory_Name],
		                    CASE 
                                WHEN complaints.ComplaintSrc = 1 THEN (SELECT NAME FROM PITB.USERS WHERE Id = complaints.Created_By)
                                WHEN complaints.ComplaintSrc = 2 THEN 'Mobile'
                                WHEN complaints.ComplaintSrc = 3 THEN 'Public'
                                WHEN complaints.ComplaintSrc = 4 THEN 'WhatsApp'
                            END AS Created_By" + ComplaintTypeSelection + @"
		                    FROM pitb.Complaints complaints
		                    INNER JOIN pitb.Campaign campaign ON complaints.Compaign_Id=campaign.Id
		                    INNER JOIN pitb.Complaints_Type complaintType ON complaints.Complaint_Category=complaintType.Id 
		                    INNER JOIN pitb.Person_Information personalInfo ON personalInfo.Person_id=complaints.Person_Id
		                    --INNER JOIN pitb.Statuses Statuses ON Statuses.Id=complaints.Complaint_Computed_Status_Id
		                    " + ComplaintTypeInnerjoinCondition + @"
		                    WHERE " + RoleCheck + @"
		                    (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelAgent.From + @"' AND '" +
                           listingModelAgent.To + @"' ) 
		                    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelAgent.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
		                    AND complaints.Complaint_Type = " + listingModelAgent.ComplaintType +
                           @"
		                     " + MulticolumnSearchQuery + @" ) as tbl
		                   
		                    ";
            }
           
            return FinalSQL;

        }
    }
}