using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pk.gov.punjab.pws.sdk;

namespace PITB.CMS_Common.ApiModels.Custom
{
    public class FileUploadModel
    {
        public string Base64String { get; set; }

        public int SourceId { get; set; }
        public string FileName { get; set; }
        public string FilePrefix { get; set; }
        public string ContentType { get; set; }
        public string FileExtension { get; set; }
        public int CampaignId { get; set; }
        public int ComplaintId { get; set; }
        public int ReferenceTypeId { get; set; }

        public int? FileType { get; set; }

        //public Config.FileType FileType { get; set; }

        //public Config.FileType File { get; set; }
        public Config.AttachmentReferenceType AttachmentRefType { get; set; }
        public Int64 ApiRequestId { get; set; }

        // Used for inner function
        public string CompleteUrl { get; set; }

        public PWSClient Client { get; set; }

        public Config.PWSConfig PwsConfig { get; set; }


        public FileUploadModel()
        {
            
        }

        public FileUploadModel(string base64String,Config.AttachmentSaveType attachmentSaveType, string filePrefix, string contentType, string fileExtension, int campaignId, int complaintId, int referenceTypeId, Config.AttachmentReferenceType attachmentRefType, int? fileType, Int64 apiRequestId)
        {
            this.Base64String = base64String;
            //this.FileName = Config.FileName;
            this.SourceId = (int)attachmentSaveType;
            this.FilePrefix = filePrefix;
            this.ContentType = contentType;
            this.FileExtension = fileExtension;
            this.CampaignId = campaignId;
            this.ComplaintId = complaintId;
            this.ReferenceTypeId = referenceTypeId;
            this.AttachmentRefType = attachmentRefType;
            this.FileType = fileType;
            this.ApiRequestId = apiRequestId;
        }

        //public FileUploadModel(string base64String, string filePrefix, string contentType, string fileExtension, int campaignId, int complaintId, Config.FileType fileType, Int64 apiRequestId)
        //{
        //    this.Base64String = base64String;
        //    this.FilePrefix = filePrefix;
        //    this.ContentType = contentType;
        //    this.FileExtension = fileExtension;
        //    this.CampaignId = campaignId;
        //    this.ComplaintId = complaintId;
        //    this.ReferenceTypeId = referenceTypeId;
        //    this.AttachmentRefType = attachmentRefType;
        //    this.ApiRequestId = apiRequestId;
        //}
    }
}