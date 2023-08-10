using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using PITB.CRM_API.Helper.Database;
    using System.Linq;

    [Table("PITB.Attachments")]
    public class DbAttachments
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? Complaint_Id { get; set; }
        
        public int? Source_Id { get; set; }

        public string Source_Url { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ApiRequestId { get; set; }

        public int? ReferenceType { get; set; }

        public int? ReferenceTypeId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FileContentType { get; set; }

        public int? FileType { get; set; }

        //public virtual DbComplaintStatusChangeLog DbComplaintStatusChangeLog { get; set; }


        #region HelperMethods

        public static List<DbAttachments> GetByComplaintId(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbAttachments> GetByRefAndComplaintId(int complaintId, Config.AttachmentReferenceType refType)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType).OrderByDescending(m=>m.Id).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbAttachments> GetByRefAndComplaintId(int complaintId, Config.AttachmentReferenceType refType, int refTypeId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType && n.ReferenceTypeId == refTypeId).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbAttachments> GetByRefAndComplaintIdAndFileType(int complaintId, Config.AttachmentReferenceType refType, int refTypeId, Config.FileType fileType)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType && n.ReferenceTypeId == refTypeId && n.FileType==(int)fileType).ToList();
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