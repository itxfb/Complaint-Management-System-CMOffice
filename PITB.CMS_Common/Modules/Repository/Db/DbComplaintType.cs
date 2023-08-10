using System.Linq;
using System;
using System.Collections.Generic;

namespace PITB.CMS_Common.Models
{

    public partial class DbComplaintType
    {
        #region Helpers


        public static DbComplaintType GetById(int categoryId, DBContextHelperLinq db = null)
        {
            try
            {
                DBContextHelperLinq db2 = (db) ?? new DBContextHelperLinq();
                DbComplaintType dbComplaintType = db2.DbComplaints_Type.Where(n => n.Complaint_Category == categoryId && n.Is_Active).FirstOrDefault();
                if (db == null) { db2.Dispose(); }
                return dbComplaintType;
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
                    return db.DbComplaints_Type.AsNoTracking().Where(n => n.Is_Active).OrderBy(m => m.Name).ToList();
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
                    return db.DbComplaints_Type.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> GetByCampaignIdAndGroupId(int campaignId, int? groupId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Is_Active && m.Group_Id == groupId).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static List<DbComplaintType> GetByDepartmentId(int departmentId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => m.DepartmentId == departmentId && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> GetByDepartmentIds(List<int?> listDepartmentIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => listDepartmentIds.Contains(m.DepartmentId) && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> GetByDepartmentAndGroupId(int departmentId, int? groupId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => m.DepartmentId == departmentId && m.Is_Active && m.Group_Id == groupId).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> GetByCampaignIdsAndDepartmentIds(List<int> campaignIdsList, List<int> departmentIdsList)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => campaignIdsList.Contains((int)m.Campaign_Id) && m.Is_Active && departmentIdsList.Contains((int)m.DepartmentId)).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbComplaintType> GetByCampaignIds(List<int> campaignIdsList, int? groupId = null)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => campaignIdsList.Contains((int)m.Campaign_Id) && m.Is_Active && m.Group_Id == groupId).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbComplaintType> GetCategoriesByCampaignIdsNotGroupId(List<int> campaignIdsList)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => campaignIdsList.Contains((int)m.Campaign_Id) && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbComplaintType> GetByTypeIds(List<int> listTypeIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => listTypeIds.Contains((int)m.Complaint_Category) && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintType> GetByCampaignIdsAndGroupId(List<int> campaignIdsList, int? groupId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_Type.AsNoTracking().Where(m => campaignIdsList.Contains((int)m.Campaign_Id) && m.Is_Active && m.Group_Id == groupId).OrderBy(m => m.Name).ToList();
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
                    return (float)db.DbComplaints_Type.AsNoTracking().Where(m => m.Complaint_Category == typeId && m.Is_Active).First().RetainingHours;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion
    }
}
