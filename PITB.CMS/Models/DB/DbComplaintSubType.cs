using System.Linq;
using System.Web.Caching;
using Amazon.DataPipeline.Model;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom.DataTable;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Complaints_SubType")]
    public class DbComplaintSubType
    {
        [Key]
        [Column("Id")]
        public int Complaint_SubCategory { get; set; }

        //[StringLength(100)]
        public string Name { get; set; }
        
        [Column("Ideal_Action")]
        public string Ideal_Action { get; set; }
        public int? Complaint_Type_Id { get; set; }

        public double? Retaining_Hours { get; set; }

        public bool Is_Active { get; set; }

        public string TagId { get; set; }

        #region Helpers
        public static List<DbComplaintSubType> All()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().Where(m=> m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintSubType> AllSubTypesByCampaignId(int campaignId, string tagId=null)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return (from subtype in db.DbComplaints_SubType.AsNoTracking()
                            join types in db.DbComplaints_Type.AsNoTracking()
                                  on subtype.Complaint_Type_Id equals types.Complaint_Category
                            where types.Campaign_Id == campaignId && types.Is_Active && subtype.TagId==tagId && subtype.Is_Active
                            orderby subtype.Name ascending
                            select subtype).ToList();


                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static List<DbComplaintSubType> GetAllSubTypesByDepartmentAndGroup(int campaignId, int? groupId, string tagId = null)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return (from subtype in db.DbComplaints_SubType.AsNoTracking()
                            join types in db.DbComplaints_Type.AsNoTracking()
                                  on subtype.Complaint_Type_Id equals types.Complaint_Category
                                  join dep in db.DbDepartment on types.DepartmentId equals dep.Id
                            where dep.Campaign_Id == campaignId && dep.Group_Id == groupId && types.Is_Active && subtype.TagId == tagId && subtype.Is_Active
                            orderby subtype.Name ascending
                            select subtype).ToList();


                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbComplaintSubType> GetByComplaintType(int complaintType, string tagId = null)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().Where(m => m.Complaint_Type_Id == complaintType && m.TagId == tagId && m.Is_Active).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static DbComplaintSubType GetById(int complaintSubType, DBContextHelperLinq db = null)
        {
            try
            {
                DBContextHelperLinq db2 = (db) ?? new DBContextHelperLinq();
                DbComplaintSubType dbComplaintSubType = db2.DbComplaints_SubType.AsNoTracking().FirstOrDefault(m => m.Complaint_SubCategory == complaintSubType && m.Is_Active);
                if (db == null) { db2.Dispose(); }
                return dbComplaintSubType;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbComplaintSubType> GetByIds(List<int> listComplaintSubType)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().Where(m => listComplaintSubType.Contains(m.Complaint_SubCategory) && m.Is_Active).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static float? GetRetainingByComplaintSubTypeId(int complaintSubType, string tagId = null)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    double? retainingHours = db.DbComplaints_SubType.AsNoTracking().Where(m => m.Complaint_SubCategory == complaintSubType && m.TagId==tagId && m.Is_Active).FirstOrDefault().Retaining_Hours;
                    if (retainingHours != null)
                    {
                        return (float?)retainingHours;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static List<DbComplaintSubType> GetByComplaintTypes(List<int> listComplaintTypes)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbComplaints_SubType.AsNoTracking().Where(m => listComplaintTypes.Contains((int)m.Complaint_Type_Id) && m.Is_Active).OrderBy(m => m.Name).ToList();
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
