//using System.Web.Script.Serialization;

//using System.Web.Script.Serialization;

using System.Collections.Generic;
using System.Linq;
//using System.Text.RegularExpressions;
using Bridge;
using Bridge.Html5;
//using BridgeClassLibrary.Form;
using BridgeDTO;
using BridgeDTO.Form.DynamicForm;
//using BridgeDTO.Form.Model;
using BridgeDTO.Form.DynamicForm;
using Newtonsoft.Json;
using Bridge.jQuery2;
using System;
//using System.Text.RegularExpressions;
//using System.Web.Script.Serialization;
//using PortableClasses.Form;
//using PortableClasses.Form;
//using PortableClasses.Form;

namespace BridgeClassLib.Form
{
    //[Module("FormBuilder")]
    [FileName("FormBuilder.js")]
    public class FormBuilder
    {
        private FormEventHandler formEventHandler { get; set; }
        private FormCommonMethods formCommonMethods { get; set; }
        //public const string FormDataUrl = "/DynamicForm/GetDynamicFormData";
        //public const string FORM_CONTAINER = "#form1";
       // public const string SUBMIT_URL = "/DynamicForm/Index";
        [Ready]
        public static void Main()
        {
            //jQuery.Ajax(
            //    new AjaxOptions()
            //    {
            //        Url = FormBuilder.FormDataUrl,
            //        Cache = false,
            //        Success = delegate(object data, string str, jqXHR jqxhr)
            //        {
            //            //string str2 = JsonConvert.SerializeObject(data);
            //            //DynamicForm dynamicForm = JsonConvert.DeserializeObject<DynamicForm>(str2);

            //            //object sd = dynamicForm;
            //            //DynamicForm asd2 = (DynamicForm)sd;
            //            DynamicForm template = new DynamicForm(data);
            //            FormBuilder.CreateForm(jQuery.Select(FORM_CONTAINER), template);
            //        }
            //    }
            //    );
        }

        public  void CreateForm(jQuery container, FormAjaxParams formParams, DynamicForm dynamicForm)
        {
            formEventHandler = new FormEventHandler();
            formCommonMethods = new FormCommonMethods();
            foreach (FormField field in dynamicForm.Fields)
            {
                //container.Append(FormBuilder.CreateFormField(field));
                container.Append(CreateFormField(container, field, formParams));
            }
            HTMLFormElement htmlFormElement = (HTMLFormElement)Document.GetElementById(formParams.FormId);
            htmlFormElement.AddEventListener(EventType.Submit, formEventHandler.OnFormSubmit);

            jQuery formJq = new jQuery(htmlFormElement);
            formJq.Data("data", formParams);

            // Create Submit Button
            List<FormField> listSubmitFormField = dynamicForm.Fields.Where(n => n.Kind == FormFieldType.ButtonSubmit).ToList();
            List<HTMLInputElement> listHtmlFormSubmitBtn = new List<HTMLInputElement>(); 
            HTMLInputElement htmlFormSubmitBtn = null;
            if (listSubmitFormField.Count==0)
            {
                htmlFormSubmitBtn = new HTMLInputElement
                {
                    Type = InputType.Submit,
                    Value = "Submit",
                    ClassName = "btn btn-primary",
                    FormAction = formParams.UrlAfterPost, //SUBMIT_URL,
                    FormMethod = "POST"
                };
                listHtmlFormSubmitBtn.Add(htmlFormSubmitBtn);
            }
            else
            {
                foreach (FormField submitFormField in listSubmitFormField)
                {
                    htmlFormSubmitBtn = new HTMLInputElement
                    {
                        Type = InputType.Submit,
                        Value = submitFormField.GetStrPermissionValue("Name"),
                        ClassName = submitFormField.GetStrPermissionValue("Class"),
                        FormAction = submitFormField.GetStrPermissionValue("Url_To_Navigate"), //SUBMIT_URL,
                        FormMethod = submitFormField.GetStrPermissionValue("POST"), //SUBMIT_URL,
                    };
                    htmlFormSubmitBtn.SetAttribute("Tags-To-Post", submitFormField.GetStrPermissionValue("Tags_To_Post"));
                    listHtmlFormSubmitBtn.Add(htmlFormSubmitBtn);
                }
               
            }



            container.Append(new jQuery("<div>")
                .AddClass("form-group")
                .Append(new jQuery("<div>")
                    .AddClass("col-sm-offset-2 col-sm-10")
                    .Append(new jQuery(listHtmlFormSubmitBtn.ToArray())
                )
            ));
            //Window.Alert("asdfasdf");
        }



        public jQuery CreateFormField(jQuery container, FormField template, FormAjaxParams formParams)
        {
            switch (template.Kind)
            {
                case FormFieldType.RadioList: 
                    return CreateRadioInput( template.Id, template.Label, ((RadioFormField)template).Options);

                case FormFieldType.OptionList:
                    return CreateOptionList((OptionsFormField)template, formParams, container);
                    //return CreateOptionList(template.Id, template.Label, ((OptionsFormField)template).Options);

                case FormFieldType.Checkbox:
                    return CreateCheckboxInput((CheckboxFormField)template, formParams);

                case FormFieldType.TextField:
                    return CreateTextInput((TextFormField)template, formParams, container);

                default:
                    return new jQuery("");
            }
        }

        public jQuery CreateSubmitButton(FormField template, FormAjaxParams formParams)
        {
            return null;
        }

        public jQuery CreateCheckboxInput(CheckboxFormField formField, FormAjaxParams formParams)
        {
            //jQuery divOptionList = new jQuery("<div>");
            //divOptionList.AddClass("col-sm-" + formField.GetStrPermissionValue("Field_Column"));

            HTMLInputElement htmlInputElement = new HTMLInputElement
            {
                Type = InputType.Checkbox,
                Id = formField.Id,
                Name = formField.Label,
                //Required = formField.GetBoolPermissionValue("Validation_Is_Required"),
                //ClassName = "",//"form-control",
                Placeholder = formField.GetStrPermissionValue("Placeholder")
            };

            jQuery jq = ApplyCommonFeatures(htmlInputElement, formField);

            return jq;
            //return new jQuery("<div>")
            //    .AddClass("form-group")
            //    .Append(new HTMLLabelElement
            //    {
            //        ClassName = "control-label col-sm-" + formField.GetStrPermissionValue("Label_Column"),
            //        HtmlFor = formField.Id,
            //        InnerHTML = formField.Label
            //    })
            //    .Append(new jQuery("<div>")
            //        .AddClass("col-sm-" + formField.GetStrPermissionValue("Field_Column"))
            //        .Append(htmlInputElement).Append(htmlValidationSpanElement)
            //    );
        }

        public  jQuery CreateRadioInput(string id, string label, List<Option> listOptions)
        {
            jQuery divRadio = new jQuery("<div>");
            divRadio.AddClass("col-sm-10");

            for (int i = 0; i < listOptions.Count; i++)
            {
                divRadio
                    .Append(new jQuery("<label>")
                        .AddClass("radio-inline")
                        .Append(new HTMLInputElement
                        {
                            Type = InputType.Radio,
                            Id = listOptions[i].Value,//id + "-" + i,
                            Name = id,
                            Value = listOptions[i].Text
                        })
                        .Append(listOptions[i].Text)
                    );
            }

            return new jQuery("<div>")
                .AddClass("form-group")
                .Append(new HTMLLabelElement
                {
                    ClassName = "control-label col-sm-2",
                    HtmlFor = id,
                    InnerHTML = label + ":"
                })
                .Append(divRadio);
        }


        //public static jQuery CreateOptionList(string id, string label, List<Option> listOptions)
        public jQuery CreateOptionList(/*jQuery container,*/ OptionsFormField optionsFormField, FormAjaxParams formParams, jQuery container)
        {
            
            //jQuery divOptionList = new jQuery("<div>");
            //divOptionList.AddClass("col-sm-" + optionsFormField.GetStrPermissionValue("Field_Column"));
            
            HTMLSelectElement selectElement = new HTMLSelectElement();
            selectElement.Id = optionsFormField.Id;
            selectElement.ClassName = "form-control";
            for (int i = 0; i < optionsFormField.Options.Count; i++)
            {

                HTMLOptionElement htmlInputElement = new HTMLOptionElement
                {
                    Value = optionsFormField.Options[i].Value,
                    InnerHTML = optionsFormField.Options[i].Text
                };
                selectElement.AppendChild(htmlInputElement);
            }
            jQuery jq = ApplyCommonFeatures(selectElement, optionsFormField, container);
            //divOptionList.Append(selectElement).Append(htmlValidationSpanElement);
            selectElement.AddEventListener(EventType.Change, formEventHandler.OnOptionListChange);
            
           
            /*selectElement.SetAttribute("data-is-valid", "false");
            selectElement.SetAttribute("data-form-tag", formParams.FormTag);
            HTMLSpanElement htmlValidationSpanElement = null;
            if (optionsFormField.IsPermissionSubStringPresent("Validation"))
            {
                htmlValidationSpanElement = new HTMLSpanElement()
                {
                    Id = "Validate__" + optionsFormField.Id,
                    ClassName = optionsFormField.GetStrPermissionValue("Validation_Span_Class_Valid")
                };
                selectElement["htmlValidationSpanElement"] = htmlValidationSpanElement;
            }
            */


            selectElement["FormField"] = optionsFormField;

            //jQuery jq = new jQuery("<div>")
            //    .AddClass("form-group")
            //    .Append(new HTMLLabelElement
            //    {
            //        ClassName = "control-label col-sm-" + optionsFormField.GetStrPermissionValue("Label_Column"),
            //        HtmlFor = optionsFormField.Id,
            //        InnerHTML = optionsFormField.Label 
            //    })
            //    .Append(divOptionList);
            //container.Append(jq);
            if (optionsFormField.GetBoolPermissionValue("Is_Multiselect"))
            {
                selectElement.SetAttribute("multiple", "multiple");
                Script.Write("this.formCommonMethods.EnableMultiselect(optionsFormField.Id)");
                //Script.Write("this.formCommonMethods.EnableMultiselectOfElement(selectElement)");
            }
            return jq;
        }

        public  jQuery CreateTextInput(TextFormField formField, FormAjaxParams formParams,jQuery container)
        {
            HTMLInputElement htmlInputElement = new HTMLInputElement
            {
                Type = InputType.Text,
                Id = formField.Id,
                Name = formField.Label,
                //Required = formField.GetBoolPermissionValue("Validation_Is_Required"),
                //ClassName = "form-control",
                Placeholder = formField.GetStrPermissionValue("Placeholder")
            };

            jQuery jq = ApplyCommonFeatures(htmlInputElement, formField, container);

      
            if (formField.IsPermissionSubStringPresent("Input"))
            {
                //jQueryEvent
                htmlInputElement["FormField"] = formField;
                htmlInputElement.AddEventListener(EventType.Input, formEventHandler.OnTextInput);
            }

            return jq;
        }


        private jQuery ApplyCommonFeatures(HTMLElement htmlElement, FormField formField, jQuery container=null)
        {
            string htmlStr = "";

            htmlElement.ClassName = formField.GetStrPermissionValue("Class");
            if (formField.IsPermissionSubStringPresent("style"))
            {
                htmlElement.SetAttribute("style", formField.GetStrPermissionValue("style"));
            }
            htmlElement.SetAttribute("data-is-valid", "false");
            htmlElement.SetAttribute("data-form-tag", formField.TagId);
            HTMLSpanElement htmlValidationSpanElement = null;
            if (formField.IsPermissionSubStringPresent("Validation"))
            {
                htmlValidationSpanElement = new HTMLSpanElement()
                {
                    Id = "Validate__" + formField.Id,
                    ClassName = formField.GetStrPermissionValue("Validation_Span_Class_Valid")
                };
                htmlElement["htmlValidationSpanElement"] = htmlValidationSpanElement;
            }

            List<jQuery> listJq = new List<jQuery>()
            {
                new jQuery(htmlElement),
                new jQuery(htmlValidationSpanElement)
            };
            //List<jQuery>listJqNext = GetJqList(formField.GetStrPermissionValue("Next_Html"));
            //List<jQuery> listJqNext = GetJqList(formField.GetStrPermissionValue("Html"));
            //jQuery jq6 = AppendJq(GetJqList(formField.GetStrPermissionValue("Previous_Html")), listJq);
            jQuery jq = new jQuery(formField.GetStrPermissionValue("Html"));
            container.Append(jq);
            jq = jQuery.Select("#Element_To_Add");
            jq.ReplaceWith(listJq.ToArray());
            return new jQuery("");
        }

        //public List<jQuery> GetJqList(string htmlStr)
        //{
        //    List<string> listhtmlTags = BridgeUtility.GetRegexStr(htmlStr);
        //    List<jQuery> listJQueries = new List<jQuery>();
        //    string htmlTag = "";
        //    jQuery jq = null;
        //    for (int i=0; i<listhtmlTags.Count; i++)
        //    {
        //        htmlTag = listhtmlTags[i];
        //        jq = new jQuery(htmlTag);
        //        listJQueries.Add(jq);
        //    }
        //    return listJQueries;
        //}

        //public jQuery AppendJq(List<jQuery> listJq, List<jQuery> listJqToAppend)
        //{
        //    jQuery jqToRet = null;
        //    for (int i = 0; i < listJq.Count-1; i++)
        //    {
        //        if (jqToRet == null)
        //        {
        //            jqToRet = listJq[i];
        //        }
        //        else
        //        {
        //            jqToRet.Append(listJq[i]);
        //        }
        //    }

        //    jQuery jqToAppend = null;
        //    for (int i = 0; i < listJqToAppend.Count; i++)
        //    {
        //        jqToAppend = listJqToAppend[i];
        //        listJq[listJq.Count - 1].Append(jqToAppend);
        //    }
        //    jqToRet.Append(listJq[listJq.Count - 1]);
        //    return jqToRet;
        //}

        public  void AjaxCallOnDropdownChange(/*string url, string type,*/ OptionsFormField optionFormField)
        {
            /*
            jQuery.Ajax(
                new AjaxOptions()
                {
                    Url = url,
                    Type = type,
                    Data = JsonConvert.SerializeObject(optionFormField),
                    Success = delegate(object data, string str, jqXHR jqxhr)
                    {
                        //Window.Alert("Munnay kakon");
                        //JavaScriptSerializer asd = new JavaScriptSerializer();
                        string str2 = JsonConvert.SerializeObject(data);
                        DynamicForm dynamicForm = JsonConvert.DeserializeObject<DynamicForm>(str2);

                        object sd = dynamicForm;
                        DynamicForm asd2 = (DynamicForm)sd;
                        DynamicForm template = new DynamicForm(dynamicForm);
                        App.CreateForm(jQuery.Select(FORM_CONTAINER), template);
                    }
                });
             */
        }

        public  void OnOptionListChange(Event e)
        {
            //e.CurrentTarget;
            //event kEvent = e.As<KeyboardEvent>();
            HTMLSelectElement selectElement = e.CurrentTarget.As<HTMLSelectElement>();
            //Window.Alert(inputElement.Value);

            // for setting value to upper case
            //inputElement.Value = inputElement.Value.ToUpper();
            Window.Alert(selectElement.Value);
            //AjaxCallOnDropdownChange(selectElement);
        }

        




        /*
        public static void Main()
        {
            //JsFormHandler.Main();

            // Write a message to the Console
            Console.WriteLine("Welcome5345345435 to Munna.NET");
            var button = Document.CreateElement("button");
            button.InnerHTML = "click me";
            button.OnClick = (ev) =>
            {
                Console.WriteLine("welcome to munna kaka");
            };
            Document.Body.AppendChild(button);


            var h2 = Document.CreateElement("h2");
            h2.InnerHTML = "Welcome5345345435 ki heading hy";

            Document.Body.AppendChild(h2);
            Script.Write("alert('zeeshi');");

            Script.Write("alert('zeeshi'.toUpperCase());");




            //----------- eval -----
            var jsCode = "\"" + "asdf" + "\".toUpperCase();";

            // Evaluated string variable is returned
            var evalResult = Script.Eval<string>(jsCode);

            Window.Alert(evalResult);





            //Global.Alert(string.Format("Hello, {0}", "zeeshi"));
            //'alert zeeshi';

            // After building (Ctrl + Shift + B) this project, 
            // browse to the /bin/Debug or /bin/Release folder.

            // A new bridge/ folder has been created and
            // contains your projects JavaScript files. 

            // Open the bridge/index.html file in a browser by
            // Right-Click > Open With..., then choose a
            // web browser from the list

            // This application will then run in the browser.
        }*/
    }
    
}
