namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("PITB.Dynamic_ComplaintFields")]
    public partial class DbDynamicComplaintFields
    {
        public int Id { get; set; }

        public int? ComplaintId { get; set; }

        public int? SaveTypeId { get; set; }

        public int? CategoryHierarchyId { get; set; }

        public int? CategoryTypeId { get; set; }

        public int? ControlId { get; set; }

        [StringLength(2000)]
        public string FieldName { get; set; }

        [StringLength(2000)]
        public string FieldValue { get; set; }

        public string Filter1Name { get; set; }

        public int? Filter1 { get; set; }

        public string Filter2Name { get; set; }

        public int? Filter2 { get; set; }

        public string Filter3Name { get; set; }

        public int? Filter3 { get; set; }

        [StringLength(200)]
        public string TagId { get; set; }

        public static List<DbDynamicComplaintFields> GetByComplaintId(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicComplaintFields.AsNoTracking().Where(m => m.ComplaintId == complaintId).OrderBy(m => m.ComplaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicComplaintFields> GetBy(int complaintId, string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicComplaintFields.AsNoTracking().Where(m => m.ComplaintId == complaintId && m.TagId==tagId).OrderBy(m => m.ComplaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicComplaintFields> GetBy(int complaintId, int filter1)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicComplaintFields.AsNoTracking().Where(m => m.ComplaintId == complaintId && m.Filter1 == filter1).OrderBy(m => m.ComplaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        

        public static void Save(List<DbDynamicComplaintFields> listDbDynamicComplaintFields)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    foreach (DbDynamicComplaintFields dbDynamicComplaintField in listDbDynamicComplaintFields)
                    {
                        db.DbDynamicComplaintFields.Add(dbDynamicComplaintField);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
