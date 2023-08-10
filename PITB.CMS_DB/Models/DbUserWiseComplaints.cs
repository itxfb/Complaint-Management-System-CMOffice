using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace PITB.CMS_DB.Models
{
    [Table("PITB.User_Wise_Complaints")]
    public partial class DbUserWiseComplaints
    {
        [Key]

        public Int64 Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? User_Id { get; set; }

        //public int? User_Role { get; set; }

        public int? Complaint_Id { get; set; }

        public int? Complaint_Type { get; set; }

        public int? Complaint_Subtype { get; set; }
    }
}