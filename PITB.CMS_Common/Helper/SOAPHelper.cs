using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
using PITB.CMS_Common.Helper.Extensions;

namespace PITB.CMS_Common.Helper
{
    public class SOAPHelper
    {
        public static string HttpGetProtocolConsumeWebRequest(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                req.Accept = "text/xml";
                req.Method = "GET";
                using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    string result = responseReader.ReadToEnd();
                    XDocument ResultXML = XDocument.Parse(result);
                    string ResultString = result;
                    return ResultString;
                }
            }
            catch (Exception ex)
            {
                Utility.WriteFile(@"C:\Apps\Crm\", "HtmlOrignal.txt", ex.StackTrace.ToString()+"------Inner exception = "+ex.InnerException.ObjToStr()+"-----Orignal Exception = "+ex.ToString());
            }
            return null;
        }
        public static string HttpGetProtocolGetUrlAsString(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html";
            req.Method = "GET";
            using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                string result = responseReader.ReadToEnd();
                return result;
            }
        }
        public static string HttpPostProtocolConsumeWebRequest(string url,string actionName,Dictionary<string,object> paramDict)
        {
            var request = CreateSOAPWebRequest(url, actionName);
            var soapreqbody = GetRequestBody(url, actionName, paramDict);
            string xml = GetWebRequest(request, soapreqbody);
            return xml;
        }
        private static HttpWebRequest CreateSOAPWebRequest(string url,string actionName)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Headers.Add(@"SOAPACTION:http://tempuri.org/"+actionName);
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";
            return req;
        }
        private static XmlDocument GetRequestBody(string url,string actionName,Dictionary<string,object> parameters)
        {
            XmlDocument SOAPReqBody = new XmlDocument();
            StringBuilder builder = new StringBuilder();
            string paramsStr = "";
            foreach(var d in parameters){
                builder.AppendFormat("<{0}>{1}</{0}>",d.Key,d.Value.ToString());
            }
            paramsStr = builder.ToString();
            SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-   instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">  
             <soap:Body>  
                <"+actionName+ @" xmlns=""http://tempuri.org/"">  
                  " + paramsStr + @" 
                </"+actionName+@">  
              </soap:Body>  
            </soap:Envelope>");
            return SOAPReqBody;
        }
        private static string GetWebRequest(HttpWebRequest request, XmlDocument SOAPReqBody)
        {
            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    SOAPReqBody.Save(stream);
                }
                using (WebResponse service = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(service.GetResponseStream()))
                    {
                        var serviceResult = rd.ReadToEnd();
                        return serviceResult;
                    }
                }
            }catch(Exception ex)
            {
                string obj = JsonConvert.SerializeObject(ex);
            }
            return string.Empty;
        }
    }
}