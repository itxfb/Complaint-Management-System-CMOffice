﻿using System.Linq;
using PITB.CMS.Helper.Database;


namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.User_Wise_Complaints")]
    public class DbUserWiseComplaints
    {
        [Key]

        public Int64 Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? User_Id { get; set; }

        //public int? User_Role { get; set; }

        public int? Complaint_Id { get; set; }

        public int? Complaint_Type { get; set; }

        public int? Complaint_Subtype { get; set; }


        public static void SyncUserWiseComplaints(DBContextHelperLinq db, List<DbUserWiseComplaints> listDbUserWiseComplaintsCurrent , List<DbUserWiseComplaints> listDbUserWiseComplaintsToInsert)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            List<DbUserWiseComplaints> listTempComplaints = listDbUserWiseComplaintsCurrent.Where(
                n => !listDbUserWiseComplaintsToInsert.Any(x => x.Campaign_Id == n.Campaign_Id &&
                                                                x.User_Id == n.User_Id  &&
                                                                x.Complaint_Id == n.Complaint_Id &&
                                                                x.Complaint_Type == n.Complaint_Type &&
                                                                x.Complaint_Subtype == n.Complaint_Subtype)).ToList();

            foreach (DbUserWiseComplaints dbUserWiseComplaint in listTempComplaints)
            {
                db.DbUserWiseComplaints.Remove(dbUserWiseComplaint);
            }

            listTempComplaints = listDbUserWiseComplaintsToInsert.Where(
                n => !listDbUserWiseComplaintsCurrent.Any(x => x.Campaign_Id == n.Campaign_Id &&
                                                                x.User_Id == n.User_Id  &&
                                                                x.Complaint_Id == n.Complaint_Id &&
                                                                x.Complaint_Type == n.Complaint_Type &&
                                                                x.Complaint_Subtype == n.Complaint_Subtype)).ToList();
            foreach (DbUserWiseComplaints dbUserWiseComplaint in listTempComplaints)
            {
                db.DbUserWiseComplaints.Add(dbUserWiseComplaint);
            }
            db.Configuration.AutoDetectChangesEnabled = true;
        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(DBContextHelperLinq db, int campaign_Id)
        {
            return db.DbUserWiseComplaints.Where(n => n.Campaign_Id == campaign_Id).ToList();
        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(int campaign_Id, List<int?> listComplaintIds, int complaintType, int complaintSubType )
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbUserWiseComplaints.Where(n => n.Campaign_Id == campaign_Id && listComplaintIds.Contains(n.Complaint_Id) && n.Complaint_Type==complaintType && n.Complaint_Subtype==complaintSubType).ToList();
            }
        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(DBContextHelperLinq db, int campaign_Id, List<int?> listComplaintType, List<int?> listComplaintSubType )
        {
            return db.DbUserWiseComplaints.Where(n => n.Campaign_Id == campaign_Id && listComplaintType.Contains(n.Complaint_Type) && listComplaintSubType.Contains(n.Complaint_Subtype)).ToList();
        }
    }
}