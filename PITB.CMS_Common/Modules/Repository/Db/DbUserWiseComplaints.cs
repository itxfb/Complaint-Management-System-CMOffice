using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Z.BulkOperations;
using System.Data.SqlClient;
using System.Data;

namespace PITB.CMS_Common.Models
{

    public partial class DbUserWiseComplaints
    {


        public static void SyncUserWiseComplaints(DBContextHelperLinq db, List<DbUserWiseComplaints> listDbUserWiseComplaintsCurrent, List<DbUserWiseComplaints> listDbUserWiseComplaintsToInsert)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            List<DbUserWiseComplaints> listTempComplaints = listDbUserWiseComplaintsCurrent.Where(
                n => !listDbUserWiseComplaintsToInsert.Any(x => x.Campaign_Id == n.Campaign_Id &&
                                                                x.User_Id == n.User_Id &&
                                                                x.Complaint_Id == n.Complaint_Id &&
                                                                x.Complaint_Type == n.Complaint_Type &&
                                                                x.Complaint_Subtype == n.Complaint_Subtype)).ToList();

            foreach (DbUserWiseComplaints dbUserWiseComplaint in listTempComplaints)
            {
                db.DbUserWiseComplaints.Remove(dbUserWiseComplaint);
            }

            listTempComplaints = listDbUserWiseComplaintsToInsert.Where(
                n => !listDbUserWiseComplaintsCurrent.Any(x => x.Campaign_Id == n.Campaign_Id &&
                                                                x.User_Id == n.User_Id &&
                                                                x.Complaint_Id == n.Complaint_Id &&
                                                                x.Complaint_Type == n.Complaint_Type &&
                                                                x.Complaint_Subtype == n.Complaint_Subtype)).ToList();
            foreach (DbUserWiseComplaints dbUserWiseComplaint in listTempComplaints)
            {
                db.DbUserWiseComplaints.Add(dbUserWiseComplaint);
            }
            db.Configuration.AutoDetectChangesEnabled = true;
        }

        public static void SyncUserWiseComplaints_BulkMerge(DBContextHelperLinq db, List<DbUserWiseComplaints> listDbUserWiseComplaintsCurrent, List<DbUserWiseComplaints> listDbUserWiseComplaintsToInsert)
        {
            //db.Configuration.AutoDetectChangesEnabled = false;
            //List<DbUserWiseComplaints> listTempComplaints = listDbUserWiseComplaintsCurrent.Where(
            //    n => !listDbUserWiseComplaintsToInsert.Any(x => x.Campaign_Id == n.Campaign_Id &&
            //                                                    x.User_Id == n.User_Id &&
            //                                                    x.Complaint_Id == n.Complaint_Id &&
            //                                                    x.Complaint_Type == n.Complaint_Type &&
            //                                                    x.Complaint_Subtype == n.Complaint_Subtype)).ToList();

            //foreach (DbUserWiseComplaints dbUserWiseComplaint in listTempComplaints)
            //{
            //    db.DbUserWiseComplaints.Remove(dbUserWiseComplaint);
            //}

            var list = listDbUserWiseComplaintsToInsert.Where(x => listDbUserWiseComplaintsCurrent.Where(xx => xx.Complaint_Id == x.Complaint_Id).FirstOrDefault() != null ? true : false).ToList();
            var listTempComplaints = listDbUserWiseComplaintsToInsert.Where(
                n => !listDbUserWiseComplaintsCurrent.Any(x => x.Campaign_Id == n.Campaign_Id &&
                                                                x.User_Id == n.User_Id &&
                                                                x.Complaint_Id == n.Complaint_Id &&
                                                                x.Complaint_Type == n.Complaint_Type &&
                                                                x.Complaint_Subtype == n.Complaint_Subtype)).ToList();

            BulkOperation<DbUserWiseComplaints> bulkOp = new BulkOperation<DbUserWiseComplaints>(db.Database.Connection);
            bulkOp.BatchSize = 1000;
            bulkOp.ColumnInputExpression = c => new
            {
                c.Id,
                c.Campaign_Id,
                c.User_Id,
                c.Complaint_Id,
                c.Complaint_Type,
                c.Complaint_Subtype,
            };
            bulkOp.DestinationTableName = "PITB.User_Wise_Complaints";
            bulkOp.ColumnOutputExpression = c => c.Id;
            bulkOp.ColumnPrimaryKeyExpression = c => c.Id;


            bulkOp.BulkMerge(listTempComplaints);

        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(DBContextHelperLinq db, int campaign_Id)
        {
            return db.DbUserWiseComplaints.Where(n => n.Campaign_Id == campaign_Id).ToList();
        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(int campaign_Id, List<int?> listComplaintIds, int complaintType, int complaintSubType)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbUserWiseComplaints.Where(n => n.Campaign_Id == campaign_Id && listComplaintIds.Contains(n.Complaint_Id) && n.Complaint_Type == complaintType && n.Complaint_Subtype == complaintSubType).ToList();
            }
        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(DBContextHelperLinq db, int campaign_Id, List<int?> listComplaintType, List<int?> listComplaintSubType)
        {
            return db.DbUserWiseComplaints.Where(n => n.Campaign_Id == campaign_Id && listComplaintType.Contains(n.Complaint_Type) && listComplaintSubType.Contains(n.Complaint_Subtype)).ToList();
        }
    }
}