using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bridge;
using Bridge.Html5;
//using BridgeClassLibrary.Form;
using BridgeDTO.Form.DynamicForm;
//using BridgeDTO.Form.Model;
using Newtonsoft.Json;
using Bridge.jQuery2;
using System;

[assembly: Reflectable]
namespace BridgeClassLib.Form
{

    [FileName("FormEventHandler.js")]
    public class FormEventHandler
    {
        FormValidationHandler formVHandler = new FormValidationHandler();
        FormCommonMethods formCommonMethods = new FormCommonMethods();


        public void OnFormSubmit2(Event e)
        {
            //Window.Alert("Munna kaka shuru hoa");
            try
            {
                //typeof(System.Console).GetMethod("WriteLine", new[] { typeof(string) }).Invoke(null, new[] { "Hello" });
                HTMLFormElement htmlFormElement = (HTMLFormElement)e.CurrentTarget;
                jQuery jqFormElement = new jQuery(htmlFormElement);
                FormAjaxParams formParams = (FormAjaxParams)jqFormElement.Data("data");
                //FormEventHandler asd = new FormEventHandler();
                //MethodInfo method2 = typeof(FormEventHandler).GetMethod("MunnaKaka", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public/*, new Type[] { typeof(string) }*/);

                //Func<string, string> converted3 = (Func<string, string>)
                //    Delegate.CreateDelegate(typeof(Func<string, string>), asd, method2);
                //Console.WriteLine(converted3("papappa "));

                //HTMLFormElement formElement = e.CurrentTarget.As<HTMLFormElement>();

                //MethodInfo mI = typeof (FormEventHandler).GetMethod("ProcessFormInput", new Type[] {typeof (jQuery)});
                //Func<jQuery, string> converted2 = (Func<jQuery, string>)Delegate.CreateDelegate(typeof(Func<jQuery, string>), this, mI);
                //jQuery.Select("form#" + formElement.Id + " :input").Each(
                //   delegate (int id, Element element){

                //    });
                List<HTMLInputElement> listElement = new List<HTMLInputElement>();
                jQuery.Select("input[data-form-tag='" + formParams.FormTag + "']").Each(
                   delegate(int id, Element element)
                   {
                       listElement.Add((HTMLInputElement)element);
                   });

                List<HTMLSelectElement> listSelectElement = new List<HTMLSelectElement>();
                jQuery.Select("select[data-form-tag='" + formParams.FormTag + "']").Each(
                   delegate(int id, Element element)
                   {
                       listSelectElement.Add((HTMLSelectElement)element);
                   });

                FormValidationHandler formValidationHandler = new FormValidationHandler();
                bool isFormValid = formValidationHandler.ValidateForm(listElement, listSelectElement);
                //object[] objArr = jQuery.Select("#" + htmlFormElement.Id).SerializeArray();
                if (!isFormValid)
                {
                    e.PreventDefault();
                }
                //return isFormValid;
                //Window.Alert("Munna kaka");
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public void OnFormSubmit(Event e)
        {
            //Window.Alert("Munna kaka shuru hoa");
            try
            {
                typeof(System.Console).GetMethod("WriteLine", new[] { typeof(string) }).Invoke(null, new[] { "Hello" });
                HTMLFormElement htmlFormElement =(HTMLFormElement) e.CurrentTarget;
                jQuery jqFormElement = new jQuery(htmlFormElement);
                FormAjaxParams formParams = (FormAjaxParams)jqFormElement.Data("data");
                //FormEventHandler asd = new FormEventHandler();
                //MethodInfo method2 = typeof(FormEventHandler).GetMethod("MunnaKaka", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public/*, new Type[] { typeof(string) }*/);

                //Func<string, string> converted3 = (Func<string, string>)
                //    Delegate.CreateDelegate(typeof(Func<string, string>), asd, method2);
                //Console.WriteLine(converted3("papappa "));

                //HTMLFormElement formElement = e.CurrentTarget.As<HTMLFormElement>();

                //MethodInfo mI = typeof (FormEventHandler).GetMethod("ProcessFormInput", new Type[] {typeof (jQuery)});
                //Func<jQuery, string> converted2 = (Func<jQuery, string>)Delegate.CreateDelegate(typeof(Func<jQuery, string>), this, mI);
                //jQuery.Select("form#" + formElement.Id + " :input").Each(
                //   delegate (int id, Element element){
                    
                //    });
                List<HTMLInputElement> listElement = new List<HTMLInputElement>();
                jQuery.Select("input[data-form-tag='" + formParams.FormTag + "']").Each(
                   delegate(int id, Element element)
                   {
                       listElement.Add((HTMLInputElement)element);
                   });

                List<HTMLSelectElement> listSelectElement = new List<HTMLSelectElement>();
                jQuery.Select("select[data-form-tag='" + formParams.FormTag + "']").Each(
                   delegate(int id, Element element)
                   {
                       listSelectElement.Add((HTMLSelectElement)element);
                   });
                
                FormValidationHandler formValidationHandler = new FormValidationHandler();
                bool isFormValid = formValidationHandler.ValidateForm(listElement, listSelectElement);
                //object[] objArr = jQuery.Select("#" + htmlFormElement.Id).SerializeArray();
                if (!isFormValid)
                {
                    e.PreventDefault();
                }
                //return isFormValid;
                //Window.Alert("Munna kaka");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string ProcessFormInput(jQuery jq)
        {
            return "asdasd";
        }

        public string MunnaKaka()
        {
            return "asdaiiii ---- " + "asdasdasdasdsdfsdfsdf";
        }

        public void OnOptionListChange(Event e)
        {
            HTMLSelectElement selectElement = e.CurrentTarget.As<HTMLSelectElement>();
            OptionsFormField optionFormField = (OptionsFormField) selectElement["FormField"];
            string parentOf = optionFormField.GetStrPermissionValue("Parent_Of");
            if (!string.IsNullOrEmpty(parentOf))
            {
                string[] childIdArr = parentOf.Split(',');
                foreach (string childId in childIdArr)
                {
                    HTMLSelectElement childElement = Document.GetElementById<HTMLSelectElement>(childId);
                    OptionsFormField childFF = (OptionsFormField) childElement["FormField"];
                    string apiParams = childFF.GetStrPermissionValue("Api_Params").Replace("@val",selectElement.Value);
                    PouplateDropdownList("/DynamicForm/GetDynamicCategories",apiParams, childElement);
                    formVHandler.OnOptionValidate(childElement, childFF);
                    if (childFF.GetBoolPermissionValue("Is_Multiselect"))
                    {
                        childElement.SetAttribute("multiple", "multiple");
                        Script.Write("this.formCommonMethods.EnableMultiselect(childFF.Id)");
                    }
                }
                
            }
           // Window.Alert(selectElement.Value);
        }

        public void OnOptionListChange2(Event e)
        {
            HTMLSelectElement selectElement = e.CurrentTarget.As<HTMLSelectElement>();
            OptionsFormField optionFormField = (OptionsFormField)selectElement["FormField"];
            string parentOf = optionFormField.GetStrPermissionValue("Parent_Of");
            if (!string.IsNullOrEmpty(parentOf))
            {
                string[] childIdArr = parentOf.Split(',');
                foreach (string childId in childIdArr)
                {
                    HTMLSelectElement childElement = Document.GetElementById<HTMLSelectElement>(childId);
                    OptionsFormField childFF = (OptionsFormField)childElement["FormField"];
                    string apiParams = childFF.GetStrPermissionValue("Api_Params").Replace("@val", selectElement.Value);
                    PouplateDropdownList("/DynamicForm/GetDynamicCategories", apiParams, childElement);
                    formVHandler.OnOptionValidate(childElement, childFF);
                    if (childFF.GetBoolPermissionValue("Is_Multiselect"))
                    {
                        childElement.SetAttribute("multiple", "multiple");
                        Script.Write("this.formCommonMethods.EnableMultiselect(childFF.Id)");
                    }
                }

            }
            // Window.Alert(selectElement.Value);
        }

        public void PouplateDropdownList(string url, string urlParams, HTMLSelectElement elementToPopulate)
        {
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = url,
                    Type = "Post",
                    Cache = false,
                    Data = urlParams,
                    Success = delegate(object data, string str, jqXHR jqxhr)
                    {
                        //Window.Alert(data.ToString());
                        string str2 = JsonConvert.SerializeObject(data);
                        List<Option> listOption = JsonConvert.DeserializeObject<List<Option>>(str2);
                        //elementToPopulate.InnerHTML = "";
                        jQuery.Select(elementToPopulate).Empty();
                        for (int i = 0; i < listOption.Count; i++)
                        {
                            HTMLOptionElement htmlInputElement = new HTMLOptionElement
                            {
                                Value = listOption[i].Value,
                                InnerHTML = listOption[i].Text
                            };
                            elementToPopulate.AppendChild(htmlInputElement);
                        }
                    }
                }
            );
        }

        

        public void OnTextInput(Event e)
        {
            KeyboardEvent kEvent = e.As<KeyboardEvent>();
            HTMLInputElement inputElement = kEvent.CurrentTarget.As<HTMLInputElement>();

            HTMLSpanElement spanElement = (HTMLSpanElement)inputElement["htmlValidationSpanElement"];
            TextFormField formField = (TextFormField)inputElement["FormField"];

            //FormValidationHandler formVHandler = new FormValidationHandler();
            formVHandler.OnTextBoxValidate(inputElement,formField);
        }


        public void OnTextInputFromAttribute(Event e)
        {
            KeyboardEvent kEvent = e.As<KeyboardEvent>();
            HTMLInputElement inputElement = kEvent.CurrentTarget.As<HTMLInputElement>();

            //HTMLSpanElement spanElement = (HTMLSpanElement)inputElement["htmlValidationSpanElement"];
            //TextFormField formField = (TextFormField)inputElement["FormField"];

            //FormValidationHandler formVHandler = new FormValidationHandler();
            formVHandler.OnTextBoxValidateFromAttribute(inputElement/*, formField*/);
        }

    }
}
