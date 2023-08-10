using PITB.CMS_Common.Models.Public_Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.PublicWeb_Handlers
{
    public class ListingLogicPublic
    {
        private static string ColumnsInSelect = @"a.Id,a.Compaign_Id,a.Complaint_Remarks,a.Complaint_Computed_Status,a.Complaint_Computed_Status_Id,a.Complaint_Category_Name,
			                a.District_Name,a.Tehsil_Name,a.UnionCouncil_Name,a.Created_Date,a.StatusChangedDate_Time,a.StatusChangedComments,b.Name as StatusChangedByName,
                            a.Latitude,a.Longitude";

        public static string GetQuery(VmFilters filters)
        {
            string finalQuery = string.Empty;
            string whereFilter = string.Format("WHERE a.Compaign_Id={0}", (int)filters.CampaignId);
            string selectColumns = string.Format("{0},ROW_NUMBER() OVER (ORDER BY {1} {2}) AS RowNum", ColumnsInSelect, filters.Paging.OrderByField, filters.Paging.OrderByDirection);


            if (filters.Filterable)
            {
                if (filters.DistrictIds!= null && filters.DistrictIds[0] != 0)
                {
                    whereFilter += string.Format(@" AND EXISTS(SELECT 1 FROM dbo.SplitString('{0}',',') X WHERE X.Item=a.District_Id)",
                            filters.DistrictIds.ToCommaSepratedString());
                }
                if (filters.TownTehsilId!= null && filters.TownTehsilId[0] != 0)
                {
                    whereFilter += string.Format(@" AND EXISTS(SELECT 1 FROM dbo.SplitString('{0}',',') X WHERE X.Item=a.Tehsil_Id)",
                            filters.TownTehsilId.ToCommaSepratedString());
                }
                if (filters.Uc!= null && filters.Uc[0] != 0)
                {
                    whereFilter += string.Format(@" AND EXISTS(SELECT 1 FROM dbo.SplitString('{0}',',') X WHERE X.Item=a.UnionCouncil_Id)",
                            filters.Uc.ToCommaSepratedString());
                }
                if (filters.Categories!= null && filters.Categories[0] != 0)
                {
                    whereFilter += string.Format(@" AND EXISTS(SELECT 1 FROM dbo.SplitString('{0}',',') X WHERE X.Item=a.Complaint_SubCategory)",
                            filters.Categories.ToCommaSepratedString());
                }
                if (filters.StatusId!= null && filters.StatusId.Count > 0)
                {
                    whereFilter += string.Format(@" AND EXISTS(SELECT 1 FROM dbo.SplitString('{0}',',') X WHERE X.Item=a.Complaint_Computed_Status_Id)",
                            filters.StatusId.ToCommaSepratedString());
                }
            }
            else
            {

            }
            finalQuery = string.Format(@"SELECT * FROM (

                                        SELECT {0} FROM PITB.Complaints a LEFT JOIN pitb.Users b ON a.Status_ChangedBy=b.Id
                                        {1}) as tbl
                                        WHERE tbl.RowNum BETWEEN {2} AND {3}"
                , selectColumns
                , whereFilter
                , filters.Paging.FromPage
                , filters.Paging.ToPage);





            return finalQuery;
        }
    }
}