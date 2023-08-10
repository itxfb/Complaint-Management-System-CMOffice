using System.Threading;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PITB.CMS.Handler.Business;
using PITB.CMS.Handler.Messages;
using PITB.CMS.Models.Custom;
using WebGrease.Css.Ast;
using Z.BulkOperations;

namespace PITB.CMS.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity;
    using PITB.CMS.Handler.StakeHolder;
    using PITB.CMS.Models.API.Response;
    using System.Data.SqlClient;
    using PITB.CMS.Helper.Extensions;
    using System.Data;

    [Table("PITB.User_Wise_Supervisor_Mapping")]
    public class DbUserWiseSupervisorMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        //[NotMapped]
        //public int Id { get; set; }

        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(20)]
        public string Password { get; set; }

        public int? Campaign_Id { get; set; }

        public string Province_Id { get; set; }

        public string District_Id { get; set; }

        public string Division_Id { get; set; }

        public string Tehsil_Id { get; set; }

        public string UnionCouncil_Id { get; set; }

        public string Ward_Id { get; set; }

        public Config.Roles Role_Id { get; set; }

        public Config.SubRoles? SubRole_Id { get; set; }

        public Config.Hierarchy? Hierarchy_Id { get; set; }

        public int? User_Hierarchy_Id { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Created_By { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Updated_Date { get; set; }

        public DateTime? Password_Updated { get; set; }

        public bool IsMultipleLoginsAllowed { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastOpenDate { get; set; }

        public DateTime? SignOutDate { get; set; }

        public bool IsLoggedIn { get; set; }

        //[StringLength(50)]
        public string Name { get; set; }

        public string Cnic { get; set; }

        //[StringLength(20)]
        public string Phone { get; set; }

        //[StringLength(50)]
        public string Email { get; set; }

        public string Address { get; set; }
        public string Campaigns { get; set; }
        public string Categories { get; set; }


        public string Imei_No { get; set; }

        public string Ward_Ids { get; set; }
        //public virtual List<DbPermissionsAssignment> ListUserPermissions {get; set;}

        public int? UserCategoryId1 { get; set; }

        public int? UserCategoryId2 { get; set; }

        public string Designation { get; set; }

        public string Designation_abbr { get; set; }

        public int UserIdFk { get; set; }

        public int? UserSupervisorIdFk { get; set; }



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