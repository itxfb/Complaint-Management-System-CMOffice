using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Amazon.S3;
using Amazon.S3.Model;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;
using pk.gov.punjab.pws.sdk;
using pk.gov.punjab.pws.sdk.Security;

namespace PITB.CMS_Common.ApiHandlers.FileUpload
{
    public class FileUploadHandler
    {
        #region Amazon Web Services
        //public static void StartUploadUtility(string base64String, string filePrefix, string contentType, string fileExtension, int campaignId, int complaintId, int referenceTypeId, Config.AttachmentReferenceType attachmentRefType, Int64 apiRequestId)
        //{
        //    Config.AmazonConfig amazonConfig = Utility.GetAmazonConfigModel();
        //    AmazonS3Client client = Utility.GetS3Client(amazonConfig.AmazonKeyId, amazonConfig.AmazonSecretKey);
        //    string fileName = Utility.GetUniqueFileName(filePrefix, campaignId + "-" + complaintId, fileExtension);
        //    //int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
        //    UploadAmazon(client, base64String, contentType, fileExtension, amazonConfig.AmazonBucket, fileName, complaintId, amazonConfig.AmazonUrlPrefix + fileName, referenceTypeId, attachmentRefType, apiRequestId);

        //}

        //private static void UploadAmazon(AmazonS3Client client, string base64String, string contentType, string fileExtension, string bucket, string filename, int complaintId, string completeUrl, int referenceTypeId, Config.AttachmentReferenceType attachmentRefType, Int64 apiRequestId)
        //{
        //    try
        //    {
        //        // Initialization.
        //        string path = string.Empty;
        //        //AmazonS3Client client = GetS3Client();

        //        // Loading image.
        //        byte[] bytes = Convert.FromBase64String(base64String);

        //        // Setting.
        //        PutObjectRequest request = new PutObjectRequest
        //        {
        //            BucketName = bucket,
        //            CannedACL = S3CannedACL.PublicRead,
        //            Key = string.Format("{0}", filename),
        //            ContentType = contentType
        //        };


        //        // Setting.
        //        //request.AddHeader(AmazonSettings.AMAZON_PUBLIC_HEADER, AmazonSettings.AMAZON_PUBLIC_VALUE);

        //        // Uploading.
        //        using (var ms = new MemoryStream(bytes))
        //        {
        //            request.InputStream = ms;
        //            client.PutObject(request);
        //        }
        //        StoreImageUrlInDb(completeUrl, complaintId, apiRequestId, contentType, fileExtension, referenceTypeId, attachmentRefType);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Info.
        //        throw ex;
        //    }
        //}

        #endregion
     

        #region PITB Web Services
        public static void StartUploadUtilityPWS(FileUploadModel fileUploadModel/*string base64String, string filePrefix, string contentType, string fileExtension, int campaignId, int complaintId, int referenceTypeId, Config.AttachmentReferenceType attachmentRefType, Int64 apiRequestId*/)
        {

            Config.PWSConfig pwsConfig = Utility.GetPwsConfig();
            PWSClient client = new PWSClient(new PWSCredential(pwsConfig.AccessKeyId, pwsConfig.AccessKeySecret));
            string fileName = Utility.GetUniqueFileName(fileUploadModel.FilePrefix, fileUploadModel.CampaignId + "-" + fileUploadModel.ComplaintId, fileUploadModel.FileExtension);
            fileUploadModel.FileName = fileName;
            fileUploadModel.Client = client;
            fileUploadModel.CompleteUrl = pwsConfig.UrlPrefix + fileName;
            fileUploadModel.PwsConfig = pwsConfig;
            UploadPWS(/*client,*/ fileUploadModel/*, fileUploadModel.Base64String, fileUploadModel.ContentType, fileUploadModel.FileExtension, pwsConfig.Bucket, fileName, fileUploadModel.ComplaintId, pwsConfig.UrlPrefix + fileName, fileUploadModel.ReferenceTypeId, fileUploadModel.AttachmentRefType, fileUploadModel.ApiRequestId*/);


        }

        private static void UploadPWS(/*PWSClient client,*/ FileUploadModel fileUploadModel/*, string base64String, string contentType, string fileExtension, string bucket, string filename, int complaintId, string completeUrl, int referenceTypeId, Config.AttachmentReferenceType attachmentRefType, Int64 apiRequestId*/)
        {
            try
            {

                // Loading image.
                byte[] bytes = Convert.FromBase64String(fileUploadModel.Base64String);

                PWSStorage storageObj = new PWSStorage(fileUploadModel.PwsConfig.Bucket)
                {
                    ContentType = fileUploadModel.ContentType,
                    Key = string.Format("{0}", fileUploadModel.FileName),
                    Timeout = new TimeSpan(0, 5, 0),
                };

                // Setting.
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = fileUploadModel.PwsConfig.Bucket,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = string.Format("{0}", fileUploadModel.FileName),
                    ContentType = fileUploadModel.ContentType
                };



                // Uploading.
                using (var ms = new MemoryStream(bytes))
                {
                    storageObj.InputStream = ms;
                    PWSTask putObjectTask = fileUploadModel.Client.CreateTask(PWSAction.PutObject, storageObj);
                    fileUploadModel.Client.PerformTask(putObjectTask);
                    if (putObjectTask.Status != PStatus.OK)
                    {
                        throw new Exception(string.Format("Unable to upload content reason :{0}", putObjectTask.Status));
                    }
                }
                StoreImageUrlInDb(fileUploadModel/*completeUrl, complaintId, apiRequestId, contentType, fileExtension, referenceTypeId, attachmentRefType*/);
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }
        }

        #endregion

        private static void StoreImageUrlInDb(FileUploadModel fileUploadModel/*string url, int complaintId, Int64 apiRequestId, string contentType, string fileExtension, int referenceTypeId, Config.AttachmentReferenceType attachmentRefType*/)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbAttachments dbAttachments = new DbAttachments();
                dbAttachments.Source_Id = fileUploadModel.SourceId; //(int)Config.AttachmentSaveType.WebServer;
                dbAttachments.Source_Url = fileUploadModel.CompleteUrl;//url;
                dbAttachments.Complaint_Id = fileUploadModel.ComplaintId; // complaintId;
                dbAttachments.ApiRequestId = fileUploadModel.ApiRequestId; // apiRequestId;
                dbAttachments.ReferenceType = (int) fileUploadModel.AttachmentRefType;//(int)attachmentRefType;
                dbAttachments.ReferenceTypeId = fileUploadModel.ReferenceTypeId;//referenceTypeId;
                dbAttachments.FileType = fileUploadModel.FileType;//(int)fileType;
                dbAttachments.FileName = fileUploadModel.FileName;//Config.FileName;
                dbAttachments.FileExtension = fileUploadModel.FileExtension; //fileExtension;
                dbAttachments.FileContentType = fileUploadModel.ContentType;//contentType;
                db.DbAttachments.Add(dbAttachments);
                db.SaveChanges();
                //db.db
            }
            catch (Exception ex)
            {
                return;
            }
        }


        
    }
}