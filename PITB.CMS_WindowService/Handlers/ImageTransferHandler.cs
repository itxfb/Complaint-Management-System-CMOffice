using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using Amazon.Runtime;
using Amazon.S3;
using System.Threading;
using pk.gov.punjab.pws.sdk;
using pk.gov.punjab.pws.sdk.Security;

namespace PITB.CMS_WindowService.Handlers
{
    public class ImageTransferHandler
    {

        //public const string AmazonConfigProdKeyId = "AKIAIZNFP6Z6UXMXRY7Q";
        //public const string AmazonConfigProdSecretKey = "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";
        //public const string AmazonProdBucket = "crm-cms";

        public const string PWSConfigProdKeyId = "C4A6466EEF8D49809DB55882CE6A23ED";
        public const string PWSConfigProdSecretKey = "A23F41B1527241A8AC0A2C052A022B43";
        public const string PWSProdBucket = "crm-cms";
        public const string PWSProdUrlPrefix = "https://storage.punjab.gov.pk/crm-cms/";

        private const string SelectQuery = @"SELECT * FROM PITB.Attachments
WHERE Source_Url ='https://storage.punjab.gov.pk/general/no_image_avail.jpg' AND Source_Url_Unmodified LIKE '%amazon%'";

        private const string UpdateQuery = @"UPDATE PITB.Attachments
                                             SET Source_Url = @Source_Url,
                                             FileExtension = @FileExtension
                                             WHERE Id = @Id";

        //private const int BatchSize = 10;
        private const int NoOfThreads = 50;//10;
        public class AttachmentsModel
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }

            public string FileContentType { get; set; }
            public string Source_Url { get; set; }
            public string Source_Url_Unmodified { get; set; }

            public byte[] fileData { get; set; }
        }

        public static void Start()
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
            List<AttachmentsModel> listAttachments = DBHelper.GetDataTableByQueryString(SelectQuery, null).ToList<AttachmentsModel>();
            StartProcess(listAttachments);
        }


        private static void StartProcess(List<AttachmentsModel> listAttachmentsModel)
        {
            try
            {
                int offset = 0;
                int batchSize = listAttachmentsModel.Count/NoOfThreads;
                for (int t = 0; (/*t < NoOfThreads ||*/ offset < listAttachmentsModel.Count); t++)
                {
                    batchSize = ((offset + batchSize) > listAttachmentsModel.Count) ? listAttachmentsModel.Count % NoOfThreads : batchSize;
                    List<AttachmentsModel> listAttachmentBatch = listAttachmentsModel.GetRange(offset, batchSize);
                    new Thread(delegate()
                    {
                        StartBatchTransfer(listAttachmentBatch);
                     
                    }).Start();
                    //StartBatchTransfer(listAttachmentBatch);
                    offset = offset + batchSize;
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            //return -1;
        }

        private static int StartBatchTransfer(List<AttachmentsModel> listAttachmentsModels)
        {
            //return -1;
            try
            {
                for (int i = 0; i < listAttachmentsModels.Count; i++)
                {
                    //Debug.Print(); GC.GetTotalMemory(true);
                    Debug.WriteLine("---------------- 0 = " + GC.GetTotalMemory(true));
                    StartUploadUtilityPWS(listAttachmentsModels[i]);
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            return -1;
        }

        private static int StartUploadUtilityPWS(AttachmentsModel attachmentModel/*, PostModel.File.Single postedFile, Config.AttachmentReferenceType refType,*//* string actualFileName, string fileExtension, string contentType,*/ /*string complaintIdStr, Int64? apiRequestId, int imageRefTypeId, string tagId*/)
        {
            try
            {
                //Config.PWSConfig config = Utility.GetPwsClient();
                string splitOn = "com/";
                int startIdex = attachmentModel.Source_Url_Unmodified.IndexOf(splitOn) + splitOn.Length;
                string fileRelPath = attachmentModel.Source_Url_Unmodified.Substring(startIdex);

                //CMS.Config.PWSConfig config = CMS.Utility.GetPwsClient();
                CMS.Config.PWSConfig config = new CMS.Config.PWSConfig(PWSConfigProdKeyId, PWSConfigProdSecretKey, PWSProdBucket, PWSProdUrlPrefix);
                PWSClient client = new PWSClient(new PWSCredential(PWSConfigProdKeyId, PWSConfigProdSecretKey));

                //string fileName = GetUniqueFileName("Image", complaintIdStr, postedFile.Extention);
                //int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
                using (var webClient = new WebClient())
                {
                    Debug.WriteLine("---------------- 1 = " + GC.GetTotalMemory(true));
                    byte[] fileBytes = webClient.DownloadData(attachmentModel.Source_Url_Unmodified);
                    attachmentModel.fileData = fileBytes;
                    return UploadPWS(client, config, attachmentModel, fileRelPath/*fileBytes, fileRelPath, attachmentModel.FileContentType*//*, refType, postedFile.Name, postedFile.Extention, postedFile.ContentType, config.Bucket, fileName, complaintId, config.UrlPrefix + fileName, apiRequestId, imageRefTypeId, tagId*/);
                }
                
                
            }
            catch (Exception ex)
            {

                //throw;
            }
            return -1;
        }
        private static int UploadPWS(PWSClient client, CMS.Config.PWSConfig pwsConfig, AttachmentsModel attachModel, string fileName /*, byte[] fileData, string fileName, string fileContentType*//*, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string bucket, string filename, int complaintId, string completeUrl, Int64? apiRequestId, int imageRefTypeId, string tagId*/)
        {
            byte[] bytes = attachModel.fileData;//fileData;
            try
            {
                if (string.IsNullOrEmpty(attachModel.FileExtension))
                {
                    fileName = fileName + ".jpg";
                    attachModel.FileExtension = attachModel.FileExtension + ".jpg";
                }
                // Loading image.
                

                // Setting.
                PWSStorage storageObject = new PWSStorage(PWSProdBucket)
                {
                    ContentType = attachModel.FileContentType,
                    Key = string.Format("{0}", fileName),
                    Timeout = new TimeSpan(0, 5, 0)

                };
                Debug.WriteLine("---------------- 2 = " + GC.GetTotalMemory(true));
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
                    else
                    {
                        string newUrl = pwsConfig.UrlPrefix + fileName;
                        Dictionary<string,object> dictParams = new Dictionary<string, object>();
                        dictParams.Add("@Source_Url", newUrl );
                        dictParams.Add("@Id", attachModel.Id);
                        dictParams.Add("@FileExtension",attachModel.FileExtension);

                        DBHelper.GetDataTableByQueryString(UpdateQuery, dictParams);
                    }

                }
                Debug.WriteLine("---------------- 3 = " + GC.GetTotalMemory(true));
                //return StoreImageUrlInDb(completeUrl, actualFileName, refType, fileExtension, contentType, complaintId, apiRequestId, imageRefTypeId, tagId);
            }
            catch (Exception ex)
            {
                // Info.
                //throw ex;
            }
            bytes = null;
            attachModel.fileData = null;
            return 1;
        }
        public static string GetUniqueFileName(string fileName, string complaintId, string fileExt)
        {
            return (DateTime.Now.Year + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "--" + complaintId + "--" + fileName + "--" + Guid.NewGuid().ToString("N") + fileExt);
        }


        //private static DbAttachments StartUploadUtility(byte[] fileData, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string complaintIdStr, Int64? apiRequestId, int imageRefTypeId, string tagId)
        //{
        //    //Config.AmazonConfig amazonConfig = Utility.GetAmazonConfigModel();

        //    AmazonS3Client client = GetS3Client(AmazonConfigProdKeyId, AmazonConfigProdSecretKey);
        //    string fileName = Utility.GetUniqueFileName("Image", complaintIdStr, fileExtension);
        //    int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
        //    return UploadAmazon(client, fileData, refType, actualFileName, fileExtension, contentType, amazonConfig.AmazonBucket, fileName, complaintId, amazonConfig.AmazonUrlPrefix + fileName, apiRequestId, imageRefTypeId, tagId);
        //}

        //public static AmazonS3Client GetS3Client(string amazonKey, string amazonSecretKey)
        //{

        //    //NameValueCollection appConfig = ConfigurationManager.AppSettings;
        //    AWSCredentials credentials = new BasicAWSCredentials(amazonKey, amazonSecretKey);
        //    AmazonS3Client client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest1);

        //    return client;
        //}

        //private static DbAttachments StartUploadUtilityPWS(byte[] fileData, Config.AttachmentReferenceType refType, string actualFileName, string fileExtension, string contentType, string complaintIdStr, Int64? apiRequestId, int imageRefTypeId, string tagId)
        //{

        //    Config.PWSConfig config = Utility.GetPwsClient();
        //    PWSClient client = new PWSClient(new PWSCredential(config.AccessKeyId, config.AccessKeySecret));

        //    string fileName = Utility.GetUniqueFileName("Image", complaintIdStr, fileExtension);
        //    int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
        //    return UploadPWS(client, fileData, refType, actualFileName, fileExtension, contentType, config.Bucket, fileName, complaintId, config.UrlPrefix + fileName, apiRequestId, imageRefTypeId, tagId);
        //}

        
    }
}
