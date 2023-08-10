using System.Linq;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Complaints_Type")]
    public class DbComplaintType
    {
        [Key]
        [Column("Id")]
        public int Complaint_Category { get; set; }

        public int? Campaign_Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public double? RetainingHours { get; set; }

        [NotMapped]
        public string Category_UrduName { get; set; }

        public bool? Is_Active { get; set; }



        #region Helpers

        public static DbComplaintType GetById(int categoryId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.Where(n => n.Complaint_Category == categoryId).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> All()
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => m.Is_Active == true).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public static List<DbComplaintType> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Is_Active == true).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> GetByCampaignIds(List<int> campaignIdsList)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => campaignIdsList.Contains((int)m.Campaign_Id) && m.Is_Active == true).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static float GetRetainingHoursByTypeId(int typeId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return (float)db.DbComplaints_Type.AsNoTracking().Where(m => m.Complaint_Category == typeId && m.Is_Active == true).First().RetainingHours;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}
