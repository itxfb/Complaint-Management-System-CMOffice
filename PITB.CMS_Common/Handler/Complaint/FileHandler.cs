using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.ComplaintFileHandler
{
    public class FileHandler
    {

        public static VmFileModel GetVmFileModel(int complaintId, int attachmentRefType, int attachmentRefTypeId)
        {
            VmFileModel vmFileModel = new VmFileModel();
            vmFileModel.ListFileModel = GetAttachments(complaintId, attachmentRefType, attachmentRefTypeId);
            return vmFileModel;
        }

        public static List<FileModel> GetAttachments(int complaintId, int attachmentRefType, int attachmentRefTypeId)
        {
            List<DbAttachments> listAttachments = DbAttachments.GetByComplaintAndAttachmentRef(complaintId, attachmentRefType, attachmentRefTypeId);
            return GetFileModelFromAttachments(listAttachments);
        }

        private static List<FileModel> GetFileModelFromAttachments(List<DbAttachments> listAttachments)
        {
            List<FileModel> listFileModel = new List<FileModel>();
            FileModel tempFile = null;
            foreach (DbAttachments dbAttachments in listAttachments)
            {
                tempFile = new FileModel();
                tempFile.Url = dbAttachments.Source_Url;
                tempFile.Extension = dbAttachments.FileExtension;
                tempFile.ContentType = dbAttachments.FileContentType;
                tempFile.OrignalFileName = dbAttachments.FileName;
                listFileModel.Add(tempFile);
            }
            return listFileModel;
        }
    }
}