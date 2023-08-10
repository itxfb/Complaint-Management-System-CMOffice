using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http.ModelBinding;
using Amazon.ElasticMapReduce.Model;
using Amazon.Runtime;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using HashidsNet;
using Newtonsoft.Json;
using PITB.CRM_API.Models.Custom;
using Image = Amazon.EC2.Model.Image;
using PITB.CMS.Models.View;

namespace PITB.CRM_API
{
    public class Utility
    {
        public static string GetHeadersString(HttpRequest request)
        {
            //string value = null;
            Dictionary<string,string> dictKeyVal = new Dictionary<string, string>();
            foreach (string key in request.Headers.AllKeys)
            {
                foreach (string value in request.Headers.GetValues(key))
                {
                    dictKeyVal.Add(key,value);
                }
                
            }

            return JsonConvert.SerializeObject(dictKeyVal);
        }

        public static List<int> GetIntList(string str)
        {
            return str.Split(',').Select(int.Parse).ToList();
        }

        public static string GetClientIpAddress(HttpRequest request)
        {
            string ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetBase64FromUrl(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    byte[] imageBytes = mem.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }

            }
        }

        public static List<Config.CampaignConfig> GetCampaignConfigList(List<Config.AppConfig> listAppCampaignConfigurations, Config.AppID appId)
        {
            Config.AppConfig appConfig = listAppCampaignConfigurations.FirstOrDefault(n => n.AppId == appId);
            if (appConfig != null)
            {
                return appConfig.ListCampaigns;
            }
            return null;
        }

        public static string GetStatusJsonString(string status, string exceptionMessage)
        {
            ApiStatus apiStatus = new ApiStatus(status, exceptionMessage);
            return JsonConvert.SerializeObject(apiStatus);
        }

        public static ApiStatus GetStatus(string status, string exceptionMessage)
        {
            ApiStatus apiStatus = new ApiStatus(status, exceptionMessage);
            return apiStatus;
        }

        public static string GetUniqueFileName(string fileName, string complaintId, string fileExtension)
        {
            return (DateTime.Now.Year + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "--" + complaintId + "--" + fileName + "--" + Guid.NewGuid().ToString("N") + fileExtension);
        }

        //public static Config.AmazonConfig GetAmazonConfigModel0()
        //{
        //    Config.AmazonConfig amazonConfig = null;
        //    string value = ConfigurationManager.AppSettings["AmazonConnectionType"];
        //    switch (value)
        //    {
        //        case "Local":
        //            amazonConfig = new Config.AmazonConfig(Config.AmazonConfigDevKeyId, Config.AmazonConfigDevSecretKey, Config.AmazonDevBucket, Config.AmazonDevUrlPrefix);
        //            break;
        //        case "Production":
        //            amazonConfig = new Config.AmazonConfig(Config.AmazonConfigProdKeyId, Config.AmazonConfigProdSecretKey, Config.AmazonProdBucket, Config.AmazonProdUrlPrefix);
        //            break;
        //    }
        //    return amazonConfig;
        //}
        public static Config.PWSConfig GetPwsConfig()
        {
            Config.PWSConfig config = new Config.PWSConfig();
            string value = ConfigurationManager.AppSettings["AmazonConnectionType"];

            switch (value)
            {
                case "Local":
                    return new Config.PWSConfig(Config.PWSConfigDevKeyId, Config.PWSConfigDevSecretKey, Config.PWSDevBucket, Config.PWSDevUrlPrefix);
                    break;
                case "Production":
                    return new Config.PWSConfig(Config.PWSConfigProdKeyId, Config.PWSConfigProdSecretKey, Config.PWSProdBucket, Config.PWSProdUrlPrefix);
                    break;
            }


            return null;
        }
        public static AmazonS3Client GetS3Client(string amazonKey, string amazonSecretKey)
        {

            AWSCredentials credentials = new BasicAWSCredentials(amazonKey, amazonSecretKey);
            AmazonS3Client client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest1);

            return client;
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static List<int?> GetNullableIntList(List<int> listInt )
        {
            List<int?> nullableList = listInt.Select(i => (int?)i).ToList();
            return nullableList;
        }

        public static List<int> GetIntListFromNullableIntList(List<int?> listInt)
        {
            List<int> lst = listInt.Where(x => x != null).Select(x => x.Value).ToList();
            return lst;
        }

        public static List<string> ExtractAllValidationMessagesFromModelState(ModelStateDictionary modelState)
        {
            List<string> messages = new List<string>();
            var errors = modelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0)
                .ToList();
            foreach (var errorCollection in errors)
            {
                messages.AddRange(errorCollection.Select(e => e.ErrorMessage));
            }
            return messages;
        }

        public static string EncrpytComplaintId(int complaintId)
        {
            Hashids hashids = new Hashids("IKTZ");
            return hashids.EncodeLong(complaintId);
        }
        public static Int64 DecrpytComplaintId(string encryptedComplaintId)
        {
            var hashids = new Hashids("IKTZ");
            return hashids.DecodeLong(encryptedComplaintId)[0];
        }

        public static bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

    }

}