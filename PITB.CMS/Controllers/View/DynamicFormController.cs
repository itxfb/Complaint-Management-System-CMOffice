using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authorization;
//using BridgeDTO.Form.Model;
using Newtonsoft.Json;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
//using BridgeDTO.Form.Model;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class DynamicFormController : Controller
    {
        // GET: DynamicForm
        public ActionResult Index()
        {

            //"string".IndexOf()
          

            //FormAjaxParams formAjaxParams = new FormAjaxParams()
            //{
            //    Url = "/DynamicForm/GetDynamicFormData",
            //    FormContainer = "#form1",
            //    UrlAfterPost = "/DynamicForm/Index"
            //};

            return View("~/Views/Complaint/AddDynamic2.cshtml");
            //return View();
        }


        public string MunnaKaka(string str)
        {
            return str + "asdasdasdasdsdfsdfsdf";
        }


        //public ActionResult AddDynamicChild(string paramsStr)
        //{
        //    Dictionary<string, string> dict = Utility.ConvertCollonFormatToDict(paramsStr);
        //    FormAjaxParams formAjaxParams = new FormAjaxParams()
        //    {
        //        Url = "/DynamicForm/GetDynamicFormData?paramStr=" + paramsStr,
        //        FormId =  dict["FormId"],
        //        FormTag = "Add_Complaint_Caller",
        //        UrlAfterPost = "/DynamicForm/Index"
        //    };

        //    return PartialView("~/Views/Complaint/AddDynamicChild.cshtml", formAjaxParams);
        //    //return View();
        //}

        //public ActionResult AddDynamicChild2(string campaignId)
        //{
            
           
        //    FormAjaxParams formAjaxParams = new FormAjaxParams()
        //    {
        //        Url = "/DynamicForm/GetDynamicFormData",
        //        FormId = "#form1",
        //        UrlAfterPost = "/DynamicForm/Index"
        //    };

        //    return PartialView("~/Views/Complaint/AddDynamicChild.cshtml", formAjaxParams);
        //    //return View();
        //}



        //[HttpPost]
        //public string GetDynamicCategories()
        //{
        //    Request.InputStream.Seek(0, SeekOrigin.Begin);
        //    string jsonData = new StreamReader(Request.InputStream).ReadToEnd();
        //    Dictionary<string,string> dict = Utility.ConvertCollonFormatToDict(jsonData);
        //    List<Option> listOption = new List<Option>();
        //    List<DbCategoryMapping> listDbCategory = DbCategoryMapping.GetBy(Convert.ToInt32(dict["Campaign_Id"]), dict["Tag_Id"],
        //               Convert.ToInt32(dict["Parent_Id"]));
        //    Response.ContentType = "application/json";
        //    listOption = DbCategoryMapping.GetListOption(listDbCategory);
        //    string strToRet = JsonConvert.SerializeObject(listOption);
        //    strToRet = strToRet.Replace("\n", "");
        //    strToRet = strToRet.Replace("\r", "");
        //    return strToRet;
        //}

        //[HttpGet]
        //public string GetDynamicFormData(string paramStr)
        //{
        //    try
        //    {

            
        //    MethodInfo method = typeof(string).GetMethod("IndexOf", new Type[] { typeof(char) });


        //    DynamicFormController asd = new DynamicFormController();
        //    MethodInfo method2 = typeof(DynamicFormController).GetMethod("MunnaKaka", new Type[] { typeof(string) });

        //    Func<string, string> converted2 = (Func<string, string>)
        //        Delegate.CreateDelegate(typeof(Func<string, string>), asd, method2);


        //    Func<char, int> converted = (Func<char, int>)
        //        Delegate.CreateDelegate(typeof(Func<char, int>), "Hello", method);




        //    //Debug.WriteLine(converted('l'));
        //    //Debug.WriteLine(converted('o'));
        //    //Debug.WriteLine(converted('x'));

        //    Debug.WriteLine(converted2("papappa "));
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }


        //    // end junk code


        //    //return "kaka";
        //    //Dictionary<string, string> dict = Utility.ConvertCollonFormatToDict(paramStr);
        //    DynamicForm dynamicForm = BlDynamicForm.GetDynamicForm(paramStr);
        //    Response.ContentType = "application/json";

        //    return JsonConvert.SerializeObject(dynamicForm);
            
            
        //    return JsonConvert.SerializeObject(new DynamicForm()
        //    {
        //        Fields =
        //        {
        //            new TextFormField()
        //            {
        //                Id = "name",
        //                Label = "Full Name",
        //                PlaceHolder = "Full Name",
        //                Required = true
        //            },

        //            new TextFormField()
        //            {
        //                Id = "email",
        //                Label = "Email",
        //                Required = true,
        //                PlaceHolder = "abc@gmail.com"
        //            },

        //            new OptionsFormField()
        //            {
        //                Id = "list1",
        //                Label = "Select Province",
        //                Options = new List<Option>()
        //                {
        //                    new Option(){Value = "1", Text = "Prov1"},
        //                    new Option(){Value = "2", Text = "Prov2"},
        //                    new Option(){Value = "3", Text = "Prov3"},
        //                    new Option(){Value = "4", Text = "Prov4"},
        //                    new Option(){Value = "5", Text = "Prov5"},
        //                },
        //                ListPermissions = new List<FormPermission>()
        //                {
        //                    new FormPermission(){Key = "ApiOnChange",Value = "/Home/GetDropDownData"},
        //                    new FormPermission(){Key = "NextDropDownToPopulateId",Value = "list2"},
        //                    new FormPermission(){Key = "DropDownsToBeEmptied",Value = "list3,list4"},
        //                }
                        
        //            },
        //            new OptionsFormField()
        //            {
        //                Id = "list2",
        //                Label = "Select Division",
        //                Options = new List<Option>()
        //                {
        //                    new Option(){Value = "1", Text = "Div1"},
        //                    new Option(){Value = "2", Text = "Div2"},
        //                    new Option(){Value = "3", Text = "Div3"},
        //                    new Option(){Value = "4", Text = "Div4"},
        //                    new Option(){Value = "5", Text = "Div5"},
        //                },
        //                ListPermissions = new List<FormPermission>()
        //                {
        //                    new FormPermission(){Key = "ApiOnChange",Value = "/Home/GetDropDownData"},
        //                    new FormPermission(){Key = "NextDropDownToPopulateId",Value = "list3"},
        //                    new FormPermission(){Key = "DropDownsToBeEmptied",Value = "list4"},
        //                }
        //            },
        //            new OptionsFormField()
        //            {
        //                Id = "list3",
        //                Label = "Select District",
        //                Options = new List<Option>()
        //                {
        //                    new Option(){Value = "1", Text = "DIS1"},
        //                    new Option(){Value = "2", Text = "DIS2"},
        //                    new Option(){Value = "3", Text = "DIS3"},
        //                    new Option(){Value = "4", Text = "DIS4"},
        //                    new Option(){Value = "5", Text = "DIS5"},
        //                },
        //                ListPermissions = new List<FormPermission>()
        //                {
        //                    new FormPermission(){Key = "ApiOnChange",Value = "/Home/GetDropDownData"},
        //                    new FormPermission(){Key = "NextDropDownToPopulateId",Value = "list4"},
        //                    new FormPermission(){Key = "DropDownsToBeEmptied",Value = ""},
        //                }
        //            },
        //            new OptionsFormField()
        //            {
        //                Id = "list4",
        //                Label = "Select Tehsil",
        //                Options = new List<Option>()
        //                {
        //                    new Option(){Value = "1", Text = "TEH1"},
        //                    new Option(){Value = "2", Text = "TEH2"},
        //                    new Option(){Value = "3", Text = "TEH3"},
        //                    new Option(){Value = "4", Text = "TEH4"},
        //                    new Option(){Value = "5", Text = "TEH5"},
        //                }
        //            },

        //            new RadioFormField()
        //            {
        //                Id = "q1",
        //                Label = "Favorite Browser",
        //                Options = new List<Option>()
        //                {
        //                    new Option(){Value = "1", Text = "IE1"},
        //                    new Option(){Value = "2", Text = "IE2"},
        //                    new Option(){Value = "3", Text = "IE3"},
        //                    new Option(){Value = "4", Text = "IE4"},
        //                    new Option(){Value = "5", Text = "IE5"},
        //                }
        //                //Options = "IE,Firefox,Chrome,Safari".Split(',')
        //            },

                  

        //            new RadioFormField()
        //            {
        //                Id = "q2",
        //                Label = "Favorite Language",
        //                Options = new List<Option>()
        //                {
        //                    new Option(){Value = "1", Text = "Kaka 1"},
        //                    new Option(){Value = "2", Text = "Kaka 2"},
        //                    new Option(){Value = "3", Text = "Kaka 3"},
        //                    new Option(){Value = "4", Text = "Kaka 4"},
        //                    new Option(){Value = "5", Text = "Kaka 5"},
        //                }
        //            }
        //        }
        //    });
        //    //Form 
        //    //return null;
        //}
    }
}