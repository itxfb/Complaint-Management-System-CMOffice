using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
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

        public Decimal? ApiRequestId { get; set; }

        public int? ReferenceType { get; set; }
        
        public int? ReferenceTypeId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FileContentType { get; set; }

        public int? FileType { get; set; }

        public string TagId { get; set; }

        public static List<DbAttachments> GetByComplaintAndAttachmentRef(int complaintId, int attachmentRefType, int attachmentRefTypeId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbAttachments.AsNoTracking().Where(m => m.Complaint_Id == complaintId && m.ReferenceType == attachmentRefType && m.ReferenceTypeId==attachmentRefTypeId).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public static List<DbAttachments> GetByRefAndComplaintIdAndFileType(int complaintId, Config.AttachmentReferenceType refType, int refTypeId, Config.AttachmentType fileType)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    IQueryable<DbAttachments> sw = (db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType && n.ReferenceTypeId == refTypeId && n.FileType == (int)fileType));
                    string sq = sw.ToString();
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType && n.ReferenceTypeId == refTypeId && n.FileType == (int)fileType).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}