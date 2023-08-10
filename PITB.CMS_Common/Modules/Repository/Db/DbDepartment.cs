using System.Linq;
using System;
using System.Collections.Generic;

namespace PITB.CMS_Common.Models
{

    public partial class DbDepartment
    {

        public static List<DbDepartment> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDepartment.AsNoTracking().Where(n => n.Campaign_Id == campaignId && n.Is_Active).ToList();
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
                DbDepartment dbDepartment = db2.DbDepartment.Where(x => x.Id == departmentId && x.Is_Active).FirstOrDefault();
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

        public static List<DbDepartment> GetByCampaignAndGroupId(int campaignId, int? groupId, string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDepartment.AsNoTracking().Where(n => n.Campaign_Id == campaignId && n.Group_Id == groupId && n.Tag_Id==tagId && n.Is_Active).ToList();
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
