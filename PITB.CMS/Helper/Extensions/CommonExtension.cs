using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Ajax.Utilities;
using System.Reflection;
using System.Xml.Linq;
using System.Dynamic;

namespace PITB.CMS.Helper.Extensions
{
    public static class CommonExtension
    {

        public static List<SelectListItem> ToSelectList<T>(this List<T> listItems, string valueField, string textField, string defaultValue = null, string defaultText = null, int defualtIndex=0) where T : new()
        {
           
            List<SelectListItem> listSelectList = new List<SelectListItem>();
            foreach (T item in listItems)
            {
                PropertyInfo valueProp = item.GetType().GetProperty(valueField, BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo textProp = item.GetType().GetProperty(textField, BindingFlags.Public | BindingFlags.Instance);

                string str = valueProp.GetValue(item, null).ToString();
                if (str != null)
                {
                    listSelectList.Add(new SelectListItem()
                    {
                        Value = valueProp.GetValue(item, null).ToString(),
                        Text = textProp.GetValue(item, null).ToString()
                    });
                }
            }

            if (defaultValue != null)
            {
                listSelectList.Insert(0, new SelectListItem()
                {
                    Value = defaultValue,
                    Text = defaultText
                });
            }
            if (listSelectList.Count > 0)
            {
                listSelectList[0].Selected = true;
            }
            //SelectList selectlist = new SelectList(listSelectList); 
            
            //if(selectlist.)
            return listSelectList;
        }
        /* private SelectList ToSelectList(IEnumerable<object> districtList)
         {
             List<SelectListItem> list = new List<SelectListItem>();
             foreach (dynamic item in districtList)
             {
                 list.Add(new SelectListItem()
                 {
                     Text = item.DistrictName,
                     Value = Convert.ToString(item.DistrictId)
                 });
             }
             return new SelectList(list, "Value", "Text");
         }*/

        public static object[] ToNullableObjsArr (this object[] inputObjArr, int length)
        {
            object[] objArr = new object[length];
            for (int i=0; i < objArr.Length; i++)
            {
                if(i< inputObjArr.Length)
                {
                    objArr[i] = inputObjArr[i];
                }
                else
                {
                    objArr[i] = null;
                }
            }

            return objArr;
        }

        public static string ObjToStr(this object InputObj)
        {
            if (InputObj == null)
            {
                return "";
            }
            else if(InputObj.GetType()==typeof(DateTime))
            {
                return Utility.GetSQLDateTimeFormat((DateTime)InputObj);
            }
            return  InputObj.ToString();
        }

        public static string NullToEmptyStr(this string strInput)
        {
            if (strInput == null)
            {
                return "";
            }
            return strInput;
        }

        public static string TrimNullable(this string strInput) 
        {
            if (strInput != null)
            {
                return strInput.Trim();
            }
            return null;
        }

        public static bool IsEqualAfterTrim(this string strInput, string strToCompare)
        {

            if (strInput != null && strToCompare!=null)
            {
                if (strInput.Trim().Equals(strToCompare.Trim()))
                {
                    return true;
                }
            }
            else if (strInput == null && strToCompare == null)
            {
                return true;
            }
            return false;
        }

        public static bool EqualsTrimmedStr(this string strInput, string strToCompare)
        {

            if (strInput != null && strToCompare != null)
            {
                if (strInput.Trim()==(strToCompare.Trim()))
                {
                    return true;
                }
            }
            else if (strInput == null && strToCompare == null)
            {
                return true;
            }
            return false;
        }

        public static void AddProperty(this ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }



        //public static string SerializeObjectToXml<T>(this T value)
        //{
        //    if (value == null)
        //    {
        //        return string.Empty;
        //    }
        //    try
        //    {
        //        var xmlserializer = new XmlSerializer(typeof(T));
        //        var stringWriter = new StringWriter();
        //        using (var writer = XmlWriter.Create(stringWriter))
        //        {
        //            xmlserializer.Serialize(writer, value);
        //            return stringWriter.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred", ex);
        //    }
        //}


        
    }
}