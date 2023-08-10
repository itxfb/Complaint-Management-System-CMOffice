using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;
//using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{
    public partial class DbApiRequestLogs
    {
        public DbApiRequestLogs()
        {

        }
        public DbApiRequestLogs(HttpRequest httpRequest, HttpRequestMessage Request, bool isValid, string exception, bool removePictureList = true)
        {
            try
            {
                string jsonStr = null;
                using (StreamReader streamReader = new StreamReader(httpRequest.InputStream, Encoding.UTF8))
                {
                    jsonStr = streamReader.ReadToEnd();
                }

                //jsonStr = RemoveValueIfPresentFromJsonStr(jsonStr, "video");

                //DBContextHelperLinq db = new DBContextHelperLinq();
                //DbApiRequestLogs dbApiRequest = new DbApiRequestLogs();
                //string reqUrl = Request.GetRequestContext().VirtualPathRoot;//Request.RequestUri..AbsoluteUristUri;
                //string requestHeaders = Request.Headers.ToString();

                RequestUrl = Request.RequestUri.OriginalString;
                RequestHeaders = Request.Headers.ToString();
                JsonString = jsonStr;
                IpAddress = Utility.GetClientIpAddress(httpRequest);
                CreatedDateTime = DateTime.Now;
                IsValid = isValid;
                Exception = exception;
                //db.DbApiRequestLogs.Add(dbApiRequest);
                //db.SaveChanges();
                //return dbApiRequest.Id;
            }
            catch (Exception ex)
            {
                //return null;
            }
        }

        public static decimal Save(DbApiRequestLogs dbApiRequestLog)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    db.DbApiRequestLogs.Add(dbApiRequestLog);
                    db.SaveChanges();
                    return dbApiRequestLog.Id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}