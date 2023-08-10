using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using BridgeDTO.Form.DynamicForm;

namespace BridgeClassLib.Form
{
    [FileName("FormValidationHandler.js")]
    public class FormValidationHandler
    {
        //------ For attribute Validation ---------
        public bool RegisterValidation(string formId)
        {
            //List<HTMLInputElement> listInputElement = null;
            //List<HTMLSelectElement> listSelectElement = null;
            FormEventHandler formEventHandler = new FormEventHandler();
            string jqFormIdStr = string.Format("#{0}", formId);

            HTMLFormElement htmlFormElement = Document.GetElementById<HTMLFormElement>(formId);
            htmlFormElement.AddEventListener(EventType.Submit, formEventHandler.OnFormSubmit);

            string formTag = jQuery.Select(jqFormIdStr).Attr(FormConfig.Att_FormTag);

            //string jqSelectStr = string.Format("select[{0}={1}]", FormConfig.Attribute_FormTag, formTag);
            //string jqInputStr = string.Format("input[{0}={1}]", FormConfig.Attribute_FormTag, formTag);

            string jqSelectStr = FormCommonMethods.GetJqueryStr(FormConfig.Element_Select, FormConfig.Att_FormTag,formTag);
            string jqInputStr = FormCommonMethods.GetJqueryStr(FormConfig.Element_Input, FormConfig.Att_FormTag, formTag);

            //string formTag = jQuery.Select("#" + formId).Attr(FormConfig.Attribute_FormTagin


            List<HTMLInputElement> listInputElement = new List<HTMLInputElement>();
            jQuery.Select(jqSelectStr).Each(
               delegate(int id, Element element)
               {
                   HTMLInputElement inputElement = (HTMLInputElement) element;
                   listInputElement.Add(inputElement);
                   if (inputElement.Type == InputType.Text)
                   {
                       FormCommonMethods.ApplyCommonValidationSpan(inputElement);
                       FormCommonMethods.SaveAllAttributes(inputElement);
                       inputElement.AddEventListener(EventType.Input, formEventHandler.OnTextInputFromAttribute);
                   }
               });

            List<HTMLSelectElement> listSelectElement = new List<HTMLSelectElement>();
            jQuery.Select(jqInputStr).Each(
               delegate(int id, Element element)
               {
                   FormCommonMethods.ApplyCommonValidationSpan(element);
                   FormCommonMethods.SaveAllAttributes(element);
                   listSelectElement.Add((HTMLSelectElement)element);
                   //element.AddEventListener(EventType.Change, formEventHandler.OnOptionListChange2);
               });
            
            bool isValid = true;

            return isValid;
        }

        public bool ValidateForm(List<HTMLInputElement> listInputElement, List<HTMLSelectElement> listSelectElement )
        {
            bool isValid = true;

            HTMLInputElement inputElement = null;
            for (int i = 0; i < listInputElement.Count; i++)
            {
                inputElement = listInputElement[i];
                if (inputElement.Type == InputType.Text)
                {
                    if (!OnTextBoxValidate(inputElement, (TextFormField) inputElement["FormField"], true))
                    {
                        isValid = false;
                    }
                }
            }

            HTMLSelectElement selectElement = null;
            for (int i = 0; i < listSelectElement.Count; i++)
            {
                selectElement = listSelectElement[i];
                if (!OnOptionValidate(selectElement, (OptionsFormField) selectElement["FormField"], true))
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public bool OnOptionValidate(HTMLSelectElement selectElement, OptionsFormField fieldData, bool isFormValidation = false)
        {
            HTMLSpanElement spanElement = (HTMLSpanElement)selectElement["htmlValidationSpanElement"];
            bool isValid = false;
            if (isFormValidation)
            {
                // If is required element is empty
                if (fieldData.GetStrPermissionValue("Validation_Is_Required").Equals("True"))
                {
                    if (selectElement.Value == "" || selectElement.Value == "-1")
                    {
                        spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                        spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                        isValid = false;
                    }
                    else
                    {
                        spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                        spanElement.InnerHTML = "";
                        isValid = true;
                    }
                }

            }
            return isValid;
        }

        public bool OnOptionValidateFromAttribute(HTMLSelectElement selectElement, bool isFormValidation = false)
        {
            
            //HTMLSpanElement spanElement = (HTMLSpanElement)selectElement["htmlValidationSpanElement"];
            HTMLSpanElement spanError = (HTMLSpanElement)selectElement[FormConfig.Jq_Data_HtmlSpanError];
            HTMLSpanElement spanErrorContainer = (HTMLSpanElement)selectElement[FormConfig.Jq_Data_HtmlSpanError];
            bool isValid = false;
            if (isFormValidation)
            {
                // If is required element is empty
                if (FormCommonMethods.GetAttributeValueString(selectElement, FormConfig.Att_Validation_IsRequired).Equals("True"))
                {
                    if (selectElement.Value == "" || selectElement.Value == "-1")
                    {
                        //spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                        //spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                        spanError.InnerHTML = FormCommonMethods.GetAttributeValueString(selectElement, FormConfig.Att_Validation_IsRequired_Message);
                        spanErrorContainer.AppendChild(spanError);
                        isValid = false;
                    }
                    else
                    {
                        spanErrorContainer.RemoveChild(spanError);
                        isValid = true;
                    }
                }

            }
            return isValid;
        }



        public bool OnTextBoxValidateFromAttribute(HTMLInputElement inputElement, bool isFormValidation = false)
        {
            bool isValid = true;
            HTMLSpanElement spanError = (HTMLSpanElement)inputElement[FormConfig.Jq_Data_HtmlSpanError];
            HTMLSpanElement spanErrorContainer = (HTMLSpanElement)inputElement[FormConfig.Jq_Data_HtmlSpanError];
            
            int minChar = FormCommonMethods.GetAttributeValueInt(inputElement, FormConfig.Att_Validation_Input_Min_Char);
            int maxChar = FormCommonMethods.GetAttributeValueInt(inputElement, FormConfig.Att_Validation_Input_Max_Char);

            // For From Validation
            if (isFormValidation)
            {
                if (FormCommonMethods.GetAttributeValueString(inputElement,FormConfig.Att_Validation_IsRequired).Equals("True") &&
                    inputElement.Value.Length == 0)
                {
                    //spanElement.ClassName = FormCommonMethods.GetAttributeValueString(FormConfig.attr); fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    //spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                    spanError.InnerHTML = FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Validation_IsRequired_Message);
                    spanErrorContainer.AppendChild(spanError);
                    isValid = false;
                }
                else if (minChar != -1 && inputElement.Value.Length < minChar)
                {
                    //spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    //spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Input_Min_Char_Message");
                    spanError.InnerHTML = FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Validation_Input_Min_Char_message);
                    spanErrorContainer.AppendChild(spanError);
                    
                    isValid = false;
                }
                return isValid;
            }


            // Lower and Upper Case
            //if (fieldData.GetStrPermissionValue("Input_Case").Equals("Upper"))
            if (FormCommonMethods.GetAttributeValueString(inputElement,FormConfig.Att_Input_Case).Equals(FormConfig.Val_Validation_Input_Case_Upper))
            {
                inputElement.Value = inputElement.Value.ToUpper();
            }
            else if (FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Input_Case).Equals(FormConfig.Val_Validation_Input_Case_Lower))
            {
                inputElement.Value = inputElement.Value.ToLower();
            }

            // For Only Numeric characters
            //if (fieldData.GetStrPermissionValue("Input_Format").Equals("Numeric"))
            if (FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Input_Format).Equals(FormConfig.Val_Validation_Input_Numeric))
            {
                inputElement.Value = new string(inputElement.Value.Where(c => char.IsDigit(c)).ToArray());
            }

            // For validation is required
            //if (fieldData.GetStrPermissionValue("Validation_Is_Required").Equals("True"))
            if (FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Validation_IsRequired).Equals(FormConfig.Val_True))
            {
                if (inputElement.Value.Length > 0)
                {
                    //spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                    //spanElement.InnerHTML = "";
                    spanErrorContainer.RemoveChild(spanError);
                    isValid = true;
                }
                else
                {
                    //spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    //spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                    spanError.InnerHTML = FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Validation_IsRequired_Message);
                    spanErrorContainer.AppendChild(spanError);
                    isValid = false;
                }
            }


            // For Maximum No of Chars
            if (maxChar != -1)
            {
                //int maxChar = Convert.ToInt32(fieldData.GetStrPermissionValue("Validation_Input_Max_Char"));
                inputElement.Value = inputElement.Value.Substring(0, maxChar);
            }

            // For minimun no of Chars
            if (minChar != -1)
            {
                //int minChar = Convert.ToInt32(fieldData.GetStrPermissionValue("Validation_Input_Min_Char"));
                if (/*inputElement.Value.Length == 0 ||*/ inputElement.Value.Length == minChar)
                {
                    //spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                    //spanElement.InnerHTML = "";
                    spanErrorContainer.RemoveChild(spanError);
                    isValid = true;
                }
                else if (inputElement.Value.Length > 0 && inputElement.Value.Length < minChar)
                {
                    //spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    //spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Input_Min_Char_Message");
                    spanError.InnerHTML = FormCommonMethods.GetAttributeValueString(inputElement, FormConfig.Att_Validation_Input_Min_Char_message);
                    spanErrorContainer.AppendChild(spanError);
                    isValid = false;
                }
            }

            //inputElement.SetAttribute("data-is-valid", isValid.ToString());
            inputElement[FormConfig.Jq_Data_IsValid] = isValid;
            return isValid;
        }








        public bool OnTextBoxValidate(HTMLInputElement inputElement, TextFormField fieldData, bool isFormValidation=false)
        {
            bool isValid = true;
            HTMLSpanElement spanElement = (HTMLSpanElement)inputElement["htmlValidationSpanElement"];
            int minChar = fieldData.GetPermissionValueInt("Validation_Input_Min_Char");
            int maxChar = fieldData.GetPermissionValueInt("Validation_Input_Max_Char"); 

            // For From Validation
            if (isFormValidation)
            {
                if (fieldData.GetStrPermissionValue("Validation_Is_Required").Equals("True") &&
                    inputElement.Value.Length == 0)
                {
                    spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                    isValid = false;
                }
                else if (minChar != -1 && inputElement.Value.Length < minChar)
                {
                    spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Input_Min_Char_Message");
                    isValid = false;
                }
                return isValid;
            }


            // Lower and Upper Case
            if (fieldData.GetStrPermissionValue("Input_Case").Equals("Upper"))
            {
                inputElement.Value = inputElement.Value.ToUpper();
            }
            else if (fieldData.GetStrPermissionValue("Input_Case").Equals("Lower"))
            {
                inputElement.Value = inputElement.Value.ToLower();
            }

            // For Only Numeric characters
            if (fieldData.GetStrPermissionValue("Input_Format").Equals("Numeric"))
            {
                inputElement.Value = new string(inputElement.Value.Where(c => char.IsDigit(c)).ToArray());
            }

            // For validation is required
            if (fieldData.GetStrPermissionValue("Validation_Is_Required").Equals("True"))
            {
                if (inputElement.Value.Length > 0)
                {
                    spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                    spanElement.InnerHTML = "";
                    isValid = true;
                }
                else
                {
                    spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                    isValid = false;
                }
            }


            // For Maximum No of Chars
            if (maxChar!=-1)
            {
                //int maxChar = Convert.ToInt32(fieldData.GetStrPermissionValue("Validation_Input_Max_Char"));
                inputElement.Value = inputElement.Value.Substring(0, maxChar);
            }

            // For minimun no of Chars
            if (minChar!=-1)
            {
                //int minChar = Convert.ToInt32(fieldData.GetStrPermissionValue("Validation_Input_Min_Char"));
                if (/*inputElement.Value.Length == 0 ||*/ inputElement.Value.Length == minChar)
                {
                    spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                    spanElement.InnerHTML = "";
                    isValid = true;
                }
                else if (inputElement.Value.Length>0 && inputElement.Value.Length < minChar)
                {
                    spanElement.ClassName = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                    spanElement.InnerHTML = fieldData.GetStrPermissionValue("Validation_Input_Min_Char_Message");
                    isValid = false;
                }
            }

            inputElement.SetAttribute("data-is-valid", isValid.ToString());
            return isValid;
        }

    }
}
