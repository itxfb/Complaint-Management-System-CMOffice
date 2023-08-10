using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using BridgeDTO.Form.DynamicForm;
//using BridgeDTO.Form.FormAjaxParams;
//using BridgeDTO.Form.Model;
//using BridgeDTO.Form.FormAjaxParams;
using Newtonsoft.Json;

namespace BridgeClassLib.Form
{
    [FileName("FormInitializer.js")]
    public class FormInitializer
    {
        //[Ready]
        public static void Main(string formParams)
        {
            //HTMLDivElement pramsDiv = (HTMLDivElement)Document.GetElementById("FormParams");
            HTMLDivElement pramsDiv = (HTMLDivElement)Document.GetElementById(formParams);
            
            FormAjaxParams formAjaxParams = JsonConvert.DeserializeObject<FormAjaxParams>(pramsDiv.InnerHTML);
             
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = formAjaxParams.Url,
                    Cache = false,
                    Success = delegate(object data, string str, jqXHR jqxhr)
                    {
                        //string str2 = JsonConvert.SerializeObject(data);
                        //DynamicForm dynamicForm = JsonConvert.DeserializeObject<DynamicForm>(str2);

                        //object sd = dynamicForm;
                        //DynamicForm asd2 = (DynamicForm)sd;
                        DynamicForm dynamicForm = new DynamicForm(data);
                        FormBuilder formBuilder = new FormBuilder();
                        formBuilder.CreateForm(jQuery.Select("#" + formAjaxParams.FormId), formAjaxParams, dynamicForm);
                    }
                }
                );
        }
    }
}
