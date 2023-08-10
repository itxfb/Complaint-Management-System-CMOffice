using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
//using System.Web.Helpers;
using PITB.CRM_API.Handlers.Business;
using PITB.CRM_API.Models.API.Authentication;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using PITB.CRM_API.Helper.Database;
    using System.Linq;

    [Table("PITB.ApiRequestLogs")]
    public class DbApiRequestLogs
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public string RequestUrl { get; set; }

        public string RequestHeaders { get; set; }

        //[StringLength(4000)]
        public string JsonString { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [StringLength(100)]
        public string IpAddress { get; set; }

        [StringLength(1000)]
        public string Exception { get; set; }

        public bool? IsValid { get; set; }

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
            try { 
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