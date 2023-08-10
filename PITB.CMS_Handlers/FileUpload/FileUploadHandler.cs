using Amazon.S3;
using Amazon.S3.Model;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using pk.gov.punjab.pws.sdk;
using pk.gov.punjab.pws.sdk.Security;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.FileUpload
{
    public class FileUploadHandler
    {

        public static List<DbAttachments> UploadMultipleFiles(HttpFileCollectionBase files, Config.AttachmentReferenceType refType, string complaintIdStr, int imageRefTypeId, string tagId)
        {
           /* List<HttpPostedFileBase> listPostedFile = new List<HttpPostedFileBase>();
            foreach (string fileName in files)
            {
                HttpPostedFileBase file = files[fileName];
                listPostedFile.Add(files[fileName]);
            }*/
            //new code
            PostModel.File postFileModel = new PostModel.File(files);
            return UploadMultipleFiles(postFileModel, refType, complaintIdStr, imageRefTypeId, tagId);
            //------ end new code
            //return UploadMultipleFiles(listPostedFile, refType, complaintIdStr, imageRefTypeId, tagId);
            
            
            //int count = files.Count;
            //foreach (string upload in files)
            //{
            //    string fileName = Path.GetFileName(files[upload].FileName);
            //    string fileExtention = Path.GetExtension(files[upload].FileName);

            //    if (!string.IsNullOrEmpty(fileName))
            //    {
            //        string contentType = files[upload].ContentType;
            //        Stream fileStream = files[upload].InputStream;

            //        int fileLength = files[upload].ContentLength;
            //        byte[] fileData = new byte[fileLength];
            //        fileStream.Read(fileData, 0, fileLength);
            //        //tring imageInBase64 = Convert.ToBase64String(fileData);
            //        StartUploadUtility(fileData, refType, fileName, fileExtention, contentType, complaintIdStr, null, imageRefTypeId);
            //    }
            //}
        }


        public static List<DbAttachments> UploadMultipleFiles(PostModel.File fileModel, Config.AttachmentReferenceType refType, string complaintIdStr, int imageRefTypeId, string tagId)
        {
            //int count = files.Count;
            List<DbAttachments> listAttachments = new List<DbAttachments>();
            foreach (PostModel.File.Single file in fileModel.ListFiles)
            {
                listAttachments.Add(StartUploadUtilityPWS(file, refType, complaintIdStr, null, imageRefTypeId, tagId, file.AttachmentType));
            }
            return listAttachments;
        }

        //public static List<DbAttachments> UploadMultipleFiles(List<HttpPostedFileBase> listFiles, Config.AttachmentReferenceType refType, string complaintIdStr, int imageRefTypeId, string tagId)
        //{
        //    //int count = files.Count;
        //    List<DbAttachments> listAttachments = new List<DbAttachments>();
        //    foreach (HttpPostedFileBase file in listFiles)
        //    {
        //        string fileName = Path.GetFileName(file.FileName);
        //        string fileExtention = Path.GetExtension(file.FileName);

        //        if (!string.IsNullOrEmpty(fileName))
        //        {
        //            string contentType = file.ContentType;
        //            Stream fileStream = file.InputStream;

        //            int fileLength = file.ContentLength;
        //            byte[] fileData = new byte[fileLength];
        //            fileStream.Read(fileData, 0, fileLength);
        //            //tring imageInBase64 = Convert.ToBase64String(fileData);
        //            listAttachments.Add(StartUploadUtilityPWS(fileData, refType, fileName, fileExtention, contentType, complaintIdStr, null, imageRefTypeId, tagId));
        //        }
        //    }
        //    return listAttachments;
        //}

        //private static DbAttachments StartUploadUtility(byte[] fileData, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string complaintIdStr, Int64? apiRequestId, int imageRefTypeId, string tagId)
        //{
        //    Config.AmazonConfig amazonConfig = Utility.GetAmazonConfigModel();

        //    AmazonS3Client client = Utility.GetS3Client(amazonConfig.AmazonKeyId, amazonConfig.AmazonSecretKey);
        //    string fileName = Utility.GetUniqueFileName("Image", complaintIdStr, fileExtension);
        //    int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
        //    return UploadAmazon(client, fileData, refType, actualFileName, fileExtension, contentType, amazonConfig.AmazonBucket, fileName, complaintId, amazonConfig.AmazonUrlPrefix + fileName, apiRequestId, imageRefTypeId, tagId);
        //}

        //private static DbAttachments StartUploadUtilityPWS(byte[] fileData, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string complaintIdStr, Int64? apiRequestId, int imageRefTypeId, string tagId)
        //{

        //    Config.PWSConfig config = Utility.GetPwsClient();
        //    PWSClient client = new PWSClient(new PWSCredential(config.AccessKeyId, config.AccessKeySecret));

        //    string fileName = Utility.GetUniqueFileName("Image", complaintIdStr, fileExtension);
        //    int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
        //    return UploadPWS(client, fileData, refType, actualFileName, fileExtension, contentType, config.Bucket, fileName, complaintId, config.UrlPrefix + fileName, apiRequestId, imageRefTypeId, tagId);
        //}


        private static DbAttachments StartUploadUtilityPWS(PostModel.File.Single postedFile, Config.AttachmentReferenceType refType,/* string actualFileName, string fileExtension, string contentType,*/ string complaintIdStr, Int64? apiRequestId, int imageRefTypeId, string tagId, Config.AttachmentType attachmentType)
        {

            Config.PWSConfig config = Utility.GetPwsClient();
            PWSClient client = new PWSClient(new PWSCredential(config.AccessKeyId, config.AccessKeySecret));

            string fileName = Utility.GetUniqueFileName("Image", complaintIdStr, postedFile.Extention);
            int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
            return UploadPWS(client, postedFile.FileBytes, refType, postedFile.Name, postedFile.Extention, postedFile.ContentType, config.Bucket, fileName, complaintId, config.UrlPrefix + fileName, apiRequestId, imageRefTypeId, tagId, attachmentType);
        }
        private static DbAttachments UploadPWS(PWSClient client, byte[] fileData, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string bucket, string filename, int complaintId, string completeUrl, Int64? apiRequestId, int imageRefTypeId, string tagId, Config.AttachmentType attachmentType)
        {
            try
            {
                // Loading image.
                byte[] bytes = fileData;

                // Setting.
                PWSStorage storageObject = new PWSStorage(bucket)
                {
                    ContentType = contentType,
                    Key = string.Format("{0}", filename),
                    Timeout = new TimeSpan(0, 5, 0)

                };

                // Uploading.
                using (var ms = new MemoryStream(bytes))
                {
                    storageObject.InputStream = ms;
                    PWSTask putObjectTask = client.CreateTask(PWSAction.PutObject, storageObject);
                    client.PerformTask(putObjectTask);
                    if (putObjectTask.Status != PStatus.OK)
                    {
                        throw new Exception(string.Format("Unable to upload content reason :{0}", putObjectTask.Status));
                    }

                }
                return StoreImageUrlInDb(completeUrl, actualFileName, refType, fileExtension, contentType, complaintId, apiRequestId, imageRefTypeId, tagId,attachmentType);
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }
        }

        private static DbAttachments UploadAmazon(AmazonS3Client client, byte[] fileData, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string bucket, string filename, int complaintId, string completeUrl, Int64? apiRequestId, int imageRefTypeId, string tagId, Config.AttachmentType attachmentType)
        {
            try
            {
                // Initialization.
                string path = string.Empty;
                //AmazonS3Client client = GetS3Client();

                // Loading image.
                byte[] bytes = fileData;
                //byte[] bytes = Convert.FromBase64String(base64String);

                // Setting.
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = bucket,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = string.Format("{0}", filename),
                    ContentType = contentType,
                    //ContentType = "image/jpeg"
                };


                // Setting.
                //request.AddHeader(AmazonSettings.AMAZON_PUBLIC_HEADER, AmazonSettings.AMAZON_PUBLIC_VALUE);

                // Uploading.
                using (var ms = new MemoryStream(bytes))
                {
                    request.InputStream = ms;
                    client.PutObject(request);
                }
                return StoreImageUrlInDb(completeUrl, actualFileName, refType, fileExtension, contentType, complaintId, apiRequestId, imageRefTypeId, tagId,attachmentType);
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }
        }
        public static DbAttachments StoreImageUrlInDb(string url, string actualFileName, Config.AttachmentReferenceType refType, string fileExtension, string contentType, int complaintId, Int64? apiRequestId, int imageRefTypeId, string tagId, Config.AttachmentType attachmentType)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbAttachments dbAttachments = new DbAttachments();
            dbAttachments.Source_Id = (int)Config.AttachmentSaveType.WebServer;
            dbAttachments.Source_Url = url;
            dbAttachments.Complaint_Id = complaintId;
            dbAttachments.ApiRequestId = apiRequestId;
            dbAttachments.ReferenceType = (int)refType; //(int) Config.AttachmentReferenceType.ChangeStatus;
            dbAttachments.ReferenceTypeId = imageRefTypeId;
            dbAttachments.FileName = actualFileName;
            dbAttachments.FileType = (int?) attachmentType;
            dbAttachments.FileExtension = fileExtension;
            dbAttachments.FileContentType = contentType;
            dbAttachments.TagId = tagId;
            db.DbAttachments.Add(dbAttachments);
            db.SaveChanges();
            return dbAttachments;
            //db.db
        }

        public class FileValidationStatus
        {
            public Config.AttachmentErrorType ValidationStatus { get; set; }
            public string ValidationMessage { get; set; }

            public FileValidationStatus(Config.AttachmentErrorType validationStatus, string validationMessage)
            {
                this.ValidationStatus = validationStatus;
                this.ValidationMessage = validationMessage;
            }

        }

        public static FileValidationStatus GetFileValidationStatus(HttpFileCollectionBase files)
        {
            //return new FileValidationStatus(Config.AttachmentErrorType.NoError, "");
            string fileName;
            string fileExtension;

            string contentType;
            int byteCount;
            
            foreach (string upload in files)
            {
                fileName = Path.GetFileName(files[upload].FileName);
                fileExtension = Path.GetExtension(files[upload].FileName);

                if (!string.IsNullOrEmpty(fileName))
                {
                    contentType = files[upload].ContentType;
                    byteCount = files[upload].ContentLength;
                    if (!ExtensionsHandler.IsContentTypeValid(contentType))
                    {
                        return new FileValidationStatus(Config.AttachmentErrorType.InvalidExtension, "Invalid file attached. Only image, pdf, excel and word are acceptable");
                    }
                    else if (byteCount > Config.MaxFileSize)
                    {
                        return new FileValidationStatus(Config.AttachmentErrorType.FileSizeExceeded, "File size exceeded from " + (Config.MaxFileSize / 1000000) + " Mb");
                    }
                }
            }

            return new FileValidationStatus(Config.AttachmentErrorType.NoError, "");
        }

        public static FileValidationStatus GetFileValidationStatus(PostModel.File postFileModel)
        {
            string fileName;
            string fileExtension;

            string contentType;
            int byteCount;

            foreach (PostModel.File.Single file in postFileModel.ListFiles)
            {

                if (!string.IsNullOrEmpty(file.Name))
                {
                    contentType = file.ContentType;
                    byteCount = file.ContentLength;
                    if (!ExtensionsHandler.IsContentTypeValid(contentType))
                    {
                        return new FileValidationStatus(Config.AttachmentErrorType.InvalidExtension, "Invalid file attached. Only image, pdf, excel and word are acceptable");
                    }
                    else if (byteCount > Config.MaxFileSize)
                    {
                        return new FileValidationStatus(Config.AttachmentErrorType.FileSizeExceeded, "File size exceeded from " + (Config.MaxFileSize / 1000000) + " Mb");
                    }
                }
            }

            return new FileValidationStatus(Config.AttachmentErrorType.NoError, "");
        }


        public static int GetAttachedFileCount(HttpFileCollectionBase files)
        {
            int attachedFilesCount = 0;
            int count = files.Count;
            foreach (string upload in files)
            {
                string fileName = Path.GetFileName(files[upload].FileName);
                string fileExtention = Path.GetExtension(files[upload].FileName);

                if (!string.IsNullOrEmpty(fileName))
                {
                    attachedFilesCount++;
                }
            }
            return attachedFilesCount;
        }

    }
}