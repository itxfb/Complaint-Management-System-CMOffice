using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using PITB.CMS_Common.Models.Custom;
using System.Net;

namespace PITB.CMS_Common.Handler.API
{
    public static class APIHelper
    {
        public static string ResponsePath = "";
        public static List<T1> HttpClientGetResponseList<T1,T2>(string url, T2 postObj, Pair<string,string> auth=null) where T1 : class, new()
        {
            try
            {
                string strPost = JsonConvert.SerializeObject(postObj);
                using (var client = new HttpClient())
                {
                    if (auth != null)
                    {
                        var byteArray = Encoding.ASCII.GetBytes(auth.Item1 + ":" + auth.Item2);
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                                Convert.ToBase64String(byteArray));
                    }
                    var content = new StringContent(strPost, Encoding.UTF8, "application/json");
                    string response = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<T1>>(response);
                }
            }
            catch (Exception ex)
            {
                return new List<T1>();
            }
            return new List<T1>();
        }

        public static T1 HttpClientGetResponse<T1, T2>(string url, T2 postObj, Dictionary<string, string> authDict = null) where T1 : class//, new()
        {
            try
            {
                string strPost = JsonConvert.SerializeObject(postObj,Utility.GetJsonSettings());
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(1, 2, 0, 30, 0);
                    if (authDict != null)
                    {
                        //var byteArray = Encoding.ASCII.GetBytes(auth.Item1 + ":" + auth.Item2);
                        //client.DefaultRequestHeaders.Authorization =
                        //    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                        //        Convert.ToBase64String(byteArray));
                        foreach (KeyValuePair<string, string> auth in authDict)
                        {
                            // do something with entry.Value or entry.Key
                            client.DefaultRequestHeaders.TryAddWithoutValidation(auth.Key, auth.Value);
                            //requestMessage.Headers.Add(auth.Key,auth.Value);
                        }
                        
                    }
                    var content = new StringContent(strPost, Encoding.UTF8, "application/json");
                    string response = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                    //System.IO.File.AppendAllText(ResponsePath, DateTime.Now.ToShortTimeString());  
                    //System.IO.File.AppendAllText(ResponsePath, response);
                    return JsonConvert.DeserializeObject<T1>(response, Utility.GetJsonSettings());
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<T1>(null);
                //return new T1();
            }
            return JsonConvert.DeserializeObject<T1>(null);
            //return new T1();
        }

        public static List<T1> HttpWebReqtGetResponse<T1, T2>(string url, T2 postObj, Pair<string, string> auth = null) where T1 : class, new()
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            string credidential = auth.Item1 + ":" + auth.Item2;
            string authStr = Convert.ToBase64String(Encoding.Default.GetBytes(credidential));
            request.Headers["Authorization"] = "Basic " + authStr;

            string postDataStr = JsonConvert.SerializeObject(postObj);
            var data = Encoding.ASCII.GetBytes(postDataStr);

            request.Method = "POST";
            request.ContentType = "application/json";//"application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject<List<T1>>(responseString);
            return null;
        }
    }
}