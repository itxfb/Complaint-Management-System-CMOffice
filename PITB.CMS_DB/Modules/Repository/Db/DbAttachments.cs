using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PITB.CMS_DB.Models
{

    public partial class DbAttachments
    {

        public static List<DbAttachments> GetByComplaintAndAttachmentRef(int complaintId, int attachmentRefType, int attachmentRefTypeId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbAttachments.AsNoTracking().Where(m => m.Complaint_Id == complaintId && m.ReferenceType == attachmentRefType && m.ReferenceTypeId == attachmentRefTypeId).ToList();
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