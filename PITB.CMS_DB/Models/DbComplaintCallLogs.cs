using System.Data;
using System.Linq;
using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{


    [Table("PITB.Complaint_Call_Logs")]
    public partial class DbComplaintCallLogs
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

    }
}
