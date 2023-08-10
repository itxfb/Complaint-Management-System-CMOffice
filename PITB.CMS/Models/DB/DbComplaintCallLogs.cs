using System.Data;
using System.Linq;
using Amazon.DataPipeline.Model;
using Amazon.Lambda.Model;
using PITB.CMS.Handler.Business;
using PITB.CMS.Handler.Complaint.Assignment;
using PITB.CMS.Helper.Database;
using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using PITB.CMS.Models.Custom;
using System.Data.SqlClient;

namespace PITB.CMS.Models.DB
{


    [Table("PITB.Complaint_Call_Logs")]
    public class DbComplaintCallLogs
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        public int call_by_userid { get; set; }
        public int complaint_id { get; set; }

        public int status_Id { get; set; }

        public DateTime call_time { get; set; }

        public string comments { get; set; }
        public Boolean is_currently_active { get; set; }

        public static List<DbComplaintCallLogs> GetByComplaintId(int complaintId, DBContextHelperLinq db)
        {
            try
            {               
                    return db.DbComplaint_Call_Logs.Where(m => m.complaint_id == complaintId).ToList();              
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
