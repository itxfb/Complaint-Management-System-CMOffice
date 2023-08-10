using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Mvc;
using Newtonsoft.Json;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
using System.Dynamic;


namespace PITB.CRM_API.Helper
{
    public static class UtilityExtensions
    {
        //public static List<SelectListItem> GetYesNoList()
        //{
        //        List<SelectListItem> listItems = new List<SelectListItem>();
               
        //         listItems.Add(new SelectListItem() { Text = "Yes" , Value = "Yes" });
        //         listItems.Add(new SelectListItem() { Text = "No", Value = "No" });
                
        //        return listItems;
        //}
        public static List<dynamic> ToDynamic(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }
        public static object ObjectToDbObject(object o)
        {
            if(o==null)
            {
                return DBNull.Value;
            }
            return o;
        }

        //public static List<SelectListItem> GetDummySelectList()
        //{
        //    List<SelectListItem> listItems= new List<SelectListItem>();
        //    for (int i = 0; i < 10; i++)
        //    {
        //        listItems.Add(new SelectListItem() { Text = "Dummy_"+i, Value = i.ToString() });
                
        //    }
        //    return listItems;
        //}

        public static KeyValuePair<int,int> GetCampaignAndComplaintId(string complaintId)
        {
            string[] strArr = complaintId.Split('-');
            return new KeyValuePair<int, int>(Convert.ToInt32(strArr[0]),Convert.ToInt32(strArr[1]));
        }

        /*
        public static Config.CommandStatus GetStatus(string status)
        {
            return (Config.CommandStatus)Enum.Parse(typeof(Config.CommandStatus), status);
        }*/

        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
                return 0;
            if (value == null)
                return 0;
            return value;
        }

        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;
            return value;
        }

        public static object ReturnFalseIfNull(this object value)
        {
            if (value == DBNull.Value)
                return false;
            if (value == null)
                return false;
            return value;
        }

        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
                return DateTime.MinValue;
            if (value == null)
                return DateTime.MinValue;
            return value;
        }

        public static object ReturnNullIfDbNull(this object value)
        {
            if (value == DBNull.Value)
                return '\0';
            if (value == null)
                return '\0';
            return value;
        }

        public static List<int> ToIntList(this string[] array)
        {
            return array.Select(Int32.Parse).ToList();
        }

        public static List<int> ToIntList(this List<string> strList)
        {
            return strList.Select(int.Parse).ToList();
        }

        public static string ToCommaSepratedString(this string[] arrayStrings)
        {
            return string.Join(",", arrayStrings);
        }
        public static string ToCommaSepratedString(this List<string> arrayStrings)
        {
            return string.Join(",", arrayStrings);
        }
        public static string ToNewLineSepratedString(this List<string> arrayStrings)
        {
            return string.Join(Environment.NewLine, arrayStrings);
        }
        public static string ToJointString(this List<string> arrayStrings,string delimeter)
        {
            return string.Join(delimeter, arrayStrings);
        }
        public static string ToJson(this List<string> arrayStrings)
        {
            return JsonConvert.SerializeObject(arrayStrings);
        }
        /// <summary>
        /// Convert DateTime to string
        /// </summary>
        /// <param name="datetTime"></param>
        /// <param name="excludeHoursAndMinutes">if true it will execlude time from datetime string. Default is false</param>
        /// <returns></returns>
        public static string ConvertDate(this DateTime datetTime, bool excludeHoursAndMinutes = false)
        {
            if (datetTime != DateTime.MinValue)
            {
                if (excludeHoursAndMinutes)
                    return datetTime.ToString("yyyy-MM-dd");
                return datetTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return null;
        }
        /*
        public static int EnumToInt(Config.Roles role)
        {
            return Convert.ToInt32(role);
        }
        */
        public static string ToAaData(this DataTable dt)
        {
            object objectData = new { aaData = dt };
            return JsonConvert.SerializeObject(objectData);
        }
       
            //public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
            //    where TEnum : struct, IComparable, IFormattable, IConvertible
            //{
            //    var values = from TEnum e in Enum.GetValues(typeof(TEnum))
            //                 select new { Id = e, Name = e.ToString() };
            //    return new SelectList(values, "Id", "Name", enumObj);
            //}
        public static string Encrypt(string id)
        {
            string encryptedString = string.Empty;
            foreach (char c in id)
            {
                encryptedString = encryptedString + (Convert.ToBase64String(BitConverter.GetBytes(c)));
            }
            return encryptedString.Replace("=","");
        }
        public static string Decrypt(string encrypted)
        {
            string decryptedString = string.Empty;
            foreach (char c in encrypted)
            {
                decryptedString = decryptedString + (char)(Convert.ToInt32(c));
            }
            return decryptedString;
        }
        
        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }

        public static byte ToByte(this Enum enumValue)
        {
            return Convert.ToByte(enumValue);
        }

    }       
}