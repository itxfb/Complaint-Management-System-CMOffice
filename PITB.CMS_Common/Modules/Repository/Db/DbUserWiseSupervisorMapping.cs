using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using PITB.CMS_Common;
using PITB.CMS_Common.Helper.Database;

namespace PITB.CMS_Common.Models
{

    public partial class DbUserWiseSupervisorMapping
    {
        public static bool DeleteEntries(int campaignId)
        {
            try
            {
                string sqlQuery = @"DELETE FROM PITB.User_Wise_Supervisor_Mapping
                                    WHERE Campaign_Id = @Campaign_Id";
                Dictionary<string, object> paramsDict = new Dictionary<string, object>();
                paramsDict.Add("@Campaign_Id", campaignId.ToDbObj());
                DBHelper.GetDataSetByQueryString(sqlQuery, paramsDict);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        public static bool BulkMerge(List<DbUserWiseSupervisorMapping> listToMerge, SqlConnection con = null)
        {
            try
            {


                if (con == null)
                {
                    con = new SqlConnection(Config.ConnectionString);
                }

                BulkOperation<DbUserWiseSupervisorMapping> bulkOp = new BulkOperation<DbUserWiseSupervisorMapping>(con);
                bulkOp.BatchSize = 1000;
                bulkOp.ColumnInputExpression = c => new
                {
                    c.UserIdFk,
                    c.UserSupervisorIdFk,
                    c.Username,
                    c.Password,
                    c.Campaign_Id,
                    c.Province_Id,
                    c.District_Id,
                    c.Division_Id,
                    c.UnionCouncil_Id,
                    c.Role_Id,
                    c.SubRole_Id,
                    c.Created_Date,
                    c.Created_By,
                    c.IsActive,
                    c.Updated_Date,
                    c.Password_Updated,
                    c.LastLoginDate,
                    c.LastOpenDate,
                    c.SignOutDate,
                    c.IsMultipleLoginsAllowed,
                    c.IsLoggedIn,
                    c.Name,
                    c.Phone,
                    c.Email,
                    c.Address,
                    c.Campaigns,
                    c.Hierarchy_Id,
                    c.Categories,
                    c.Ward_Id,
                    c.Imei_No,
                    c.User_Hierarchy_Id,
                    c.Cnic,
                    c.Designation,
                    c.Ward_Ids,
                    c.UserCategoryId1,
                    c.UserCategoryId2,
                    c.Designation_abbr,






                    //c.Username,
                    //c.Password,
                    //c.Campaign_Id,
                    //c.Campaigns,
                    //c.Name,
                    //c.Cnic,
                    //c.Phone,
                    //c.Designation,
                    //c.Designation_abbr,

                    //c.Province_Id,
                    //c.Division_Id,
                    //c.District_Id,
                    //c.Tehsil_Id,
                    //c.UnionCouncil_Id,


                    //c.Hierarchy_Id,
                    //c.User_Hierarchy_Id,
                    //c.UserCategoryId1,
                    //c.UserCategoryId2,

                    //c.Categories,
                    //c.Role_Id,
                    //c.IsLoggedIn,
                    //c.IsMultipleLoginsAllowed,
                    //c.IsActive,
                    //c.Created_Date,
                    //c.Created_By
                };
                bulkOp.DestinationTableName = "PITB.User_Wise_Supervisor_Mapping";
                bulkOp.ColumnOutputExpression = c => c.Id;
                bulkOp.ColumnPrimaryKeyExpression = c => c.Id;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                bulkOp.BulkMerge(listToMerge);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}