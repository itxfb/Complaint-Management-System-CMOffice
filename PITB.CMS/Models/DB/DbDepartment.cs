using System.Data.Entity;
using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Department")]
    public partial class DbDepartment
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? Group_Id { get; set; }

        public bool Is_Active { get; set; }

        //[NotMapped]
       // public virtual List<DbComplaintType> ListComplaintType { get; set; }


        public static List<DbDepartment> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDepartment.AsNoTracking().Where(n=>n.Campaign_Id==campaignId && n.Is_Active).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static List<DbDepartment> GetByDepartmentIds(List<int> departmentIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDepartment.Where(x => departmentIds.Contains(x.Id) && x.Is_Active).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbDepartment GetByDepartmentId(int departmentId, DBContextHelperLinq db = null)
        {
            try
            {
                DBContextHelperLinq db2 = (db) ?? new DBContextHelperLinq();
                DbDepartment dbDepartment = db2.DbDepartment.Where(x => x.Id==departmentId && x.Is_Active).FirstOrDefault();
                if (db == null) { db2.Dispose(); }
                return dbDepartment;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<DbDepartment> GetByCampaignIds(List<int> campaignIdsList, int? groupId = null)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDepartment.AsNoTracking().Where(m => campaignIdsList.Contains((int)m.Campaign_Id) && m.Group_Id == groupId && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbDepartment> GetByCampaignAndGroupId(int campaignId, int? groupId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDepartment.AsNoTracking().Where(n => n.Campaign_Id == campaignId && n.Group_Id == groupId && n.Is_Active).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool DoesCampaignHasDepartment(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return (db.DbDepartment.AsNoTracking().Where(n => n.Campaign_Id == campaignId && n.Is_Active).FirstOrDefault() == null);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
