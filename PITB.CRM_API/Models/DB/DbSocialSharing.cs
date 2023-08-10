using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PITB.CRM_API.Helper.Database;


namespace PITB.CRM_API.Models.DB
{
    [Table("Complaints_Social_Sharing",Schema = "PITB")]
    public class DbSocialSharing
    {
        public int Id   { get; set; }
        public long Complaint_Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string User_Id { get; set; }
        public string Post_Id { get; set; }
        public string Provider { get; set; }
        public DateTime Created_DateTime { get; set; }
        //public bool? Is_Reply_Submitted { get; set; }
        //public string Comment_Id { get; set; }




        public static DbSocialSharing GetByComplaintId(int complaintId)
        {
            using (DBContextHelperLinq db= new DBContextHelperLinq())
            {
                return db.DbSocialSharings.FirstOrDefault(m => m.Complaint_Id == complaintId);
            }
        }
    }
}