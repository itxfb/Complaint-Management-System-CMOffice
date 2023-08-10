using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_DB.Models
{

    public partial class DbPoliceAction
    {

        public static DbPoliceAction GetAction(int complaintId, bool isActive)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbPoliceAction.Where(n => n.Complaint_Id == complaintId && n.IsActive == isActive).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbPoliceAction GetAction(int complaintId, bool isComplete, bool isActive, DBContextHelperLinq _db = null)
        {
            DBContextHelperLinq db = (_db == null) ? new DBContextHelperLinq() : _db;
            try
            {
                return db.DbPoliceAction.Where(n => n.Complaint_Id == complaintId && n.IsComplete == isComplete && n.IsActive == isActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (_db == null)
                {
                    db.Dispose();
                }
            }
        }

        public static List<DbPoliceAction> GetActions(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbPoliceAction.Where(n => n.Complaint_Id == complaintId).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
