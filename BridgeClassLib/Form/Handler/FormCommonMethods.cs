using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace BridgeClassLib.Form
{
    [FileName("FormCommonMethods.js")]
    public class FormCommonMethods
    {

        public static void ApplyCommonValidationSpan(Element htmlElement)
        {
            string elementId = htmlElement.Id;
            string str = htmlElement.GetAttribute(FormConfig.Att_Validation_For_Id);
            string jqSpanStr = FormCommonMethods.GetJqueryStr(FormConfig.Element_Span,FormConfig.Att_Validation_For_Id,htmlElement.Id) ;
            HTMLSpanElement spanContainer = null;
            jQuery.Select(jqSpanStr).Each(
                delegate(int id, Element element)
                {
                    HTMLElement el = (HTMLElement) element;
                    if (elementId.Equals(el.Id)) // if span consists with element id
                    {
                        spanContainer = (HTMLSpanElement) el;
                    }
                }
            );

            HTMLSpanElement spanError = null;
            if (spanContainer != null)
            {
                spanError = new HTMLSpanElement();
                spanError.Id = FormConfig.Element_Span+FormConfig.Seperator+htmlElement.Id;
                htmlElement[FormConfig.Jq_Data_HtmlSpanError] = spanError;
                htmlElement[FormConfig.Jq_Data_HtmlSpanErrorContainer] = spanContainer;
            }
        }

        public static void SaveAllAttributes(Element htmlElement)
        {
            Dictionary<string,string> dictAttr = new Dictionary<string, string>();
            NamedNodeMap htmlAttr = htmlElement.Attributes;
            Node node = null;
            for (int i = 0; i < htmlAttr.Length; i++)
            {
                node = htmlAttr[i];
                dictAttr.Add(node.NodeName,node.NodeValue);
            }
            htmlElement[FormConfig.Jq_Data_Attributes] = dictAttr;
        }


        public static string GetJqueryStr(string elementName, string attribute, string attributeValue)
        {
            string jqStr = string.Format("{0}[{1}={2}]", elementName, attribute, attributeValue);
            return jqStr;
        }

        public static bool IsAttributeSubStringPresent(HTMLElement htmlElement, string permissionStr)
        {
            NamedNodeMap htmlAttr = htmlElement.Attributes;
            Node node = null;
            for (int i=0; i<htmlAttr.Length; i++)
            {
                node = htmlAttr[i];
                if (node.NodeName.Contains(permissionStr))
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetAttributeValueInt(HTMLElement htmlElement, string permissionStr)
        {
            Dictionary<string,string> dictAtt = (Dictionary<string,string>) htmlElement[FormConfig.Jq_Data_Attributes];
            //FormPermission fp = ListPermissions.FirstOrDefault(n => n.Key.Equals(str));
            string attrVal = dictAtt[permissionStr];//htmlElement.GetAttribute(permissionStr);
            if (!string.IsNullOrEmpty(attrVal))
            {
                return Convert.ToInt32(attrVal);
            }
            return -1;
        }

        public static string GetAttributeValueString(HTMLElement htmlElement, string permissionStr)
        {
            Dictionary<string, string> dictAtt = (Dictionary<string, string>)htmlElement[FormConfig.Jq_Data_Attributes];
            //FormPermission fp = ListPermissions.FirstOrDefault(n => n.Key.Equals(str));
            string attrVal = dictAtt[permissionStr];//htmlElement.GetAttribute(permissionStr);
            if (!string.IsNullOrEmpty(attrVal))
            {
                return attrVal;
            }
            return attrVal;
        }

        public static bool GetAttributeValueBool(HTMLElement htmlElement, string permissionStr)
        {
            Dictionary<string, string> dictAtt = (Dictionary<string, string>)htmlElement[FormConfig.Jq_Data_Attributes];
            //FormPermission fp = ListPermissions.FirstOrDefault(n => n.Key.Equals(str));
            string attrVal = dictAtt[permissionStr];//htmlElement.GetAttribute(permissionStr);
            if (!string.IsNullOrEmpty(attrVal))
            {
                return Convert.ToBoolean(attrVal);
            }
            return false;
        }

        public void EnableMultiselect(object element)
        {
            Script.Write(@" 
                    $('#' + element).multiselect('destroy');
        
                    $('#' + element).multiselect({
                        includeSelectAllOption: true,
                        //checkboxName: 'multiselect[]',
                        enableCaseInsensitiveFiltering: true,
                        maxHeight: 200,
                        enableFiltering: true,
                        allSelectedText: 'All selected',
                        buttonWidth: '100%'

                    });
        
                    $('#' + element).multiselect('selectAll', false);
                    $('#' + element).multiselect('updateButtonText');
                ");
        }

        public void EnableMultiselectOfElement(HTMLSelectElement element)
        {
            Script.Write(@" 
                    element.multiselect('destroy');
        
                   element.multiselect({
                        includeSelectAllOption: true,
                        //checkboxName: 'multiselect[]',
                        enableCaseInsensitiveFiltering: true,
                        maxHeight: 200,
                        enableFiltering: true,
                        allSelectedText: 'All selected',
                        buttonWidth: '100%'

                    });
        
                    element.multiselect('selectAll', false);
                    element.multiselect('updateButtonText');
                ");
        }
    }
}
