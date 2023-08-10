using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System.Diagnostics;
using System.Dynamic;
using System.Net;
using System.IO;
using System.Text;

namespace PITB.CMS_Common.Models.Custom.CustomForm
{
    public class CustomForm
    {
        public enum RequestBodyType
        {
            FormData = 1,
            Json = 2
        }

        public class Post 
        {
            public Dictionary<string, string> DictQueryParams { get; set; }
            //public HttpFileCollectionBase Files { get; set; }

            public Dictionary<string, string> DictHeaders { get; set; }

            public RequestBodyType RequestBodyType { get; set; }
           
            public string RequestJsonBody { get; set; }

            public HttpRequest Request { get; set; }

            public PostModel.File postedFiles { get; set; }

            //public ControllerModel.Response ControllerResponse { get; set; }


            //public List<HttpPostedFileBase> ListPostedFile { get; set; }
            public List<ElementData> ListElementData { get; set; }

            public dynamic CustomData { get; set; }

            public Post()
            {
                
            }

            public Post(/*HttpRequestBase request*/HttpRequest request, dynamic customData = null)
            {
                try
                {
                    Request = request;//HttpContext.Current.Request;
                    CustomData = customData;
                    this.DictQueryParams = request.QueryString.Keys.Cast<string>().ToDictionary(k => k, v => request.QueryString[v]);
                    //this.Files = request.Files;
                    this.postedFiles = new PostModel.File(request.Files);
                    NameValueCollection headerCollection = request.Headers;
                    NameValueCollection formCollection = request.Form;
                    this.ListElementData = new List<ElementData>();
                    this.DictHeaders = new Dictionary<string, string>();
                    ElementData elementData = null;
                    Dictionary<string, dynamic> tempDict = null;
                    //string asd = formCollection[key];
                    //List<KeyValuePair<string, string>> asdff;
                    
                    if(Request.ContentType.Contains("multipart/form-data"))
                    {
                        this.RequestBodyType = RequestBodyType.FormData;
                    }
                    else if(Request.ContentType.Contains("application/json"))
                    {
                        this.RequestBodyType = RequestBodyType.Json;
                        using (Stream receiveStream = request.InputStream)
                        {
                            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                            {
                                RequestJsonBody = readStream.ReadToEnd();
                            }
                        }
                    }

                    // populating headers
                    foreach (var key in headerCollection.AllKeys)
                    {
                        DictHeaders.Add(key, headerCollection[key]);
                    }


                    foreach (var key in formCollection.AllKeys)
                    {
                        if (!key.Contains("file-"))
                        {
                            if (!Utility.IsValidJson(formCollection[key]))
                            {
                                elementData = new ElementData();
                                elementData.Key = key;
                                elementData.Value = formCollection[key];
                                elementData.DictAttributes = new Dictionary<string, string>();
                            }
                            else
                            {
                                try 
                                {
                                    tempDict =
                                        JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(formCollection[key]);
                                }
                                catch(Exception ex)
                                {
                                    elementData = new ElementData();
                                    elementData.Key = key;
                                    elementData.Value = formCollection[key];
                                    elementData.DictAttributes = new Dictionary<string, string>();
                                    ListElementData.Add(elementData);
                                    continue;
                                }
                                elementData = new ElementData();
                                elementData.Key = key;
                                if (tempDict.ContainsKey("Value"))
                                {
                                    elementData.Value = ((tempDict != null) && tempDict["Value"] != null) ? WebUtility.HtmlEncode(tempDict["Value"].ToString()) : null;
                                }
                                else
                                {
                                    elementData.Value = formCollection[key];
                                }

                                //string attKey="";
                                //string attVal="";
                                //foreach (var item in attJArray.Children())
                                //{
                                //    JEnumerable<JProperty> itemProperties = item.Children<JProperty>();
                                //    //you could do a foreach or a linq here depending on what you need to do exactly with the value
                                //    attKey = itemProperties.FirstOrDefault(x => x.Name == "key").Value.ToString();
                                //    attVal = itemProperties.FirstOrDefault(x => x.Name == "value").Value.ToString();
                                //    elementData.DictAttributes.Add(attKey,attVal);
                                //    //var myElementValue = myElement.Value; ////This is a JValue type
                                //}
                                if (tempDict.ContainsKey("Attributes"))
                                {
                                    Newtonsoft.Json.Linq.JArray attJArray = tempDict["Attributes"];
                                    elementData.DictAttributes =
                                        attJArray.ToDictionary(
                                            k => ((JObject)k).Properties().First(x => x.Name == "key").Value.ToString(),
                                            v => ((JObject)v).Properties().First(x => x.Name == "value").Value.ToString());

                                    if (elementData.DictAttributes.ContainsKey("data-val-config"))
                                    {
                                        elementData.DictConfiguration =
                                            Utility.ConvertCollonFormatToDict(elementData.DictAttributes["data-val-config"]);
                                    }
                                    else
                                    {
                                        elementData.DictConfiguration = new Dictionary<string, string>();
                                    }
                                }
                                else {
                                    elementData.DictConfiguration = new Dictionary<string, string>();
                                }
                            }
                            ListElementData.Add(elementData);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            public static string GetForgeryKeyFromRequest()
            {
                List<string> listHeadersToIgnore = new List<string> { "Referer" };
                //string 
                return null;
            }

            public bool IsFormAuthentic()
            {
                Dictionary<string,string> dict = new Dictionary<string, string>();
                foreach (var key in Request.Headers.AllKeys)
                {
                    dict.Add(key, Request.Headers[key]); 
                }
                return true;
            }

            public bool IsFormValid()
            {
                foreach (ElementData elementData in ListElementData)
                {
                    //elementData.
                }
                return true;
            }

            public ElementData GetElement(string key)
            {
                return ListElementData.Where(n => n.Key == key).FirstOrDefault();
            }

            public string GetElementValue(string key)
            {
                ElementData elementData = ListElementData.Where(n => n.Key == key).FirstOrDefault();
                if (elementData != null)
                {
                    return elementData.Value;    
                }
                return null;
            }

            public void PrintForm()
            {
                Debug.WriteLine("-------- Printing form data ----------");
                ListElementData = ListElementData = ListElementData.OrderBy(n => n.Key).ToList();
                foreach (ElementData elData in ListElementData)
                {
                    Debug.WriteLine(string.Format("key= {0}, value= {1}", elData.Key, elData.Value)); 
                }
            }

            public dynamic GetDynamicForm()
            {
                dynamic d = new ExpandoObject();
                ListElementData = ListElementData = ListElementData.OrderBy(n => n.Key).ToList();
                IDictionary<string, object> dict = (IDictionary<string, object>) d;
                foreach (ElementData elData in ListElementData)
                {
                    dict.Add(elData.Key, elData.Value);
                }
                return d;
            }

            public PostModel.File.Single GetFile(string key)
            {
                //return Files.Get(key);
                return postedFiles.GetSingle(key);
            }
        }

        public class ElementData
        {
            public string Key { get; set; }

            public string Value { get; set; }

            public Dictionary<string, string> DictAttributes { get; set; }

            public Dictionary<string, string> DictConfiguration { get; set; }

            public ElementData()
            {
                this.DictAttributes = new Dictionary<string, string>();
            }

        }
    }
}