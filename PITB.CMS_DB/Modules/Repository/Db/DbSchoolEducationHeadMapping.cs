using System.Data.SqlClient;
using System.Linq;
using Z.BulkOperations;
using System;
using System.Collections.Generic;
using PITB.CMS_Common;

namespace PITB.CMS_DB.Models
{

    public partial class DbSchoolEducationHeadMapping
    {

        public static DbSchoolEducationHeadMapping GetByEmisCode(string emisCode)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbSchoolEducationHeadMapping.AsNoTracking().Where(n => n.school_emis_code == emisCode).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<DbSchoolEducationHeadMapping> GetAll(DBContextHelperLinq db)
        {
            try
            {
                //using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbSchoolEducationHeadMapping.Select(n => n).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<DbSchoolEducationHeadMapping> GetAll()
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbSchoolEducationHeadMapping.AsNoTracking().Select(n => n).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static bool BulkMerge(List<DbSchoolEducationHeadMapping> listToMerge, SqlConnection con, int currentRetryCount = 0, int totalRetryCount = Config.FunctionRetryCount)
        {
            try
            {
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
                //con.Open();
                BulkOperation<DbSchoolEducationHeadMapping> bulkOp = new BulkOperation<DbSchoolEducationHeadMapping>(con);
                bulkOp.BatchSize = 1000;
                bulkOp.ColumnInputExpression = c => new
                {
                    c.school_emis_code,
                    c.School_Head_Name,
                    c.School_Head_Designation,
                    c.School_Head_PhoneNo
                };
                bulkOp.DestinationTableName = "dbo.SchoolEducationHeadMapping";
                bulkOp.ColumnOutputExpression = c => c.school_emis_code;
                bulkOp.ColumnPrimaryKeyExpression = c => c.school_emis_code;
                bulkOp.BulkMerge(listToMerge);
                return true;
            }
            catch (Exception ex)
            {
                if (currentRetryCount != totalRetryCount)
                {
                    BulkMerge(listToMerge, con, currentRetryCount + 1, totalRetryCount);
                }
            }
            return false;
        }


    }
}