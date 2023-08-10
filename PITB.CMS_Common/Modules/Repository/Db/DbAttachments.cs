using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PITB.CMS_Common.Models
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

    #region #region From API
    public partial class DbAttachments
    {
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
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType).OrderByDescending(m => m.Id).ToList();
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
                    return db.DbAttachments.AsNoTracking().Where(n => n.Complaint_Id == complaintId && n.ReferenceType == (int)refType && n.ReferenceTypeId == refTypeId && n.FileType == (int)fileType).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    #endregion
}