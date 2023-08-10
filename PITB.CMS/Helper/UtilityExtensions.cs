using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Dynamic;
using System.Text.RegularExpressions;
using PITB.CMS.Models.DB;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Xml;

namespace PITB.CMS.Helper
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



        public static List<T> GetMatchedFieldResults<T>(this List<T> listT, T t, string fieldsToComp) where T : new()
        {
            List<string> listFieldsToComp = fieldsToComp.Split(',').ToList();
            //const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            //List<PropertyInfo> listPropertyInfo = typeof (T).GetProperties(flags).ToList();

            List<T> listToReturn = new List<T>();

            int matchedCount = 0;

            foreach (T element in listT)
            {
                matchedCount = 0;
                for (int i = 0; i < listFieldsToComp.Count; i++)
                {
                    if (t.GetType().GetProperty(listFieldsToComp[i]) != null)
                    {
                        //var c1 = Convert.ChangeType(element.GetType().GetProperty(listFieldsToComp[i]).GetValue(element, null), element.GetType().GetProperty(listFieldsToComp[i]).PropertyType);
                        //var c2 = Convert.ChangeType(t.GetType().GetProperty(listFieldsToComp[i]).GetValue(t, null), t.GetType().GetProperty(listFieldsToComp[i]).PropertyType);

                        var c1 = element.GetType().GetProperty(listFieldsToComp[i]).GetValue(element);
                        var c2 = t.GetType().GetProperty(listFieldsToComp[i]).GetValue(t);

                        if ((c1 != null && c1.Equals(c2)) || (c1 == null && c2 == null))
                        {
                            matchedCount++;

                            if (matchedCount == listFieldsToComp.Count)
                            {
                                listToReturn.Add(element);
                            }
                        }

                    }
                }
            }
            return listToReturn;
        }
        /*
        public static List<T> GetMatchedFieldResults<T>(this List<T> listT, T t, string fieldsToComp) where T : new()
        {
            List<string> listFieldsToComp = fieldsToComp.Split(',').ToList();
            //const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            //List<PropertyInfo> listPropertyInfo = typeof (T).GetProperties(flags).ToList();

            List<T> listToReturn = new List<T>();

            int matchedCount = 0;

            foreach (T element in listT)
            {
                matchedCount = 0;
                for (int i = 0; i < listFieldsToComp.Count; i++)
                {
                    if (t.GetType().GetProperty(listFieldsToComp[i]) != null)
                    {
                        //var c1 = Convert.ChangeType(element.GetType().GetProperty(listFieldsToComp[i]).GetValue(element, null), element.GetType().GetProperty(listFieldsToComp[i]).PropertyType);
                        //var c2 = Convert.ChangeType(t.GetType().GetProperty(listFieldsToComp[i]).GetValue(t, null), t.GetType().GetProperty(listFieldsToComp[i]).PropertyType);

                        var c1 = element.GetType().GetProperty(listFieldsToComp[i]).GetValue(element);
                        var c2 = t.GetType().GetProperty(listFieldsToComp[i]).GetValue(t);

                        if ((c1 != null && c1.Equals(c2)) || (c1 == null && c2 == null))
                        {
                            matchedCount++;

                            if (matchedCount == listFieldsToComp.Count)
                            {
                                listToReturn.Add(element);
                            }
                        }

                    }
                }
            }
            return listToReturn;
        }*/


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
            if (o == null)
            {
                return DBNull.Value;
            }
            return o;
        }

        public static List<SelectListItem> GetDummySelectList()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            for (int i = 0; i < 10; i++)
            {
                listItems.Add(new SelectListItem() { Text = "Dummy_" + i, Value = i.ToString() });

            }
            return listItems;
        }
        private static object lockObject = new object();
        public static string GetXMLFromWebUrl(Dictionary<string, object> Props, string URL)
        {
            StringBuilder urlParameters = new StringBuilder();
            foreach (var prop in Props)
            {
                urlParameters.AppendFormat("{0}={1}&",prop.Key,prop.Value);
            }
            string urlTemps = urlParameters.ToString().TrimEnd('&');
            URL = URL + "?" + urlTemps;
            WebRequest req;
            WebResponse res;
            Stream stream;
            string xmlText = "";
            try
            {
                req = WebRequest.Create(URL);
                res = req.GetResponse();
                stream = res.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                xmlText = reader.ReadToEnd();
            }
            catch (Exception ex)
            {

            }
            return xmlText;
        }
        public static byte[] GetImageByteArrayFromWebUrl(string requestUriString, bool isCompressed)
        {
            WebRequest req = null;
            WebResponse res = null;
            Stream stream = null;
            byte[] buffer = null;
            try
            {
                lock (lockObject)
                {
                    req = WebRequest.Create(requestUriString);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    res = req.GetResponse();
                    stream = res.GetResponseStream();
                }
                if (isCompressed)
                {
                    buffer = GetCompressedImageByteArrayFromStream(stream, 100, 100);
                }
                else
                {
                    buffer = GetByteArrayFromStream(stream);
                }
                res.Close();
                res.Dispose();
                stream.Close();
                stream.Dispose();

            }
            catch (WebException we)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("WebException downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", we.GetType().Name));
                if (we.Response != null)
                {
                    string message = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                    builder.AppendLine(string.Format("Response Messgae: {0}", message));
                }
                builder.AppendLine(string.Format("Exception Messgae: {0}", we.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", we.StackTrace));
                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (IOException io)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("IO Exception downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", io.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", io.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", io.StackTrace));
                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (ArgumentNullException ar)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("ArgumentNull downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ar.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ar.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ar.StackTrace));

                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (UriFormatException ur)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("UriFormat downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ur.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ur.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ur.StackTrace));

                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (System.Security.SecurityException se)
            {

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Security Error downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Action: {0}", se.Action.GetDescription()));
                builder.AppendLine(string.Format("Exception Demanded: {0}", se.Demanded.ToString()));
                builder.AppendLine(string.Format("Exception Url: {0}", se.Url));
                builder.AppendLine(string.Format("Exception Type: {0}", se.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", se.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", se.StackTrace));

                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (NotSupportedException ns)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("NotSupported Error downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ns.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ns.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ns.StackTrace));

                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (NotImplementedException ni)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("NotImplemented Error downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ni.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ni.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ni.StackTrace));

                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("General Error downloading data");
                //builder.AppendLine(string.Format("Complaint Id: {0}", complaintId));
                //builder.AppendLine(string.Format("Reference Type: {0}", referenceType));
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ex.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ex.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ex.StackTrace));

                Debug.Write(builder.ToString());
                //WriteToFile(builder.ToString(), "E:\\error.txt");
            }
            return buffer;
        }
        public static void WriteToFile(string data, string filePath)
        {
            object obj = new object();
            lock (obj)
            {
                using (StreamWriter wr = File.AppendText(filePath))
                {
                    wr.WriteLine(data);
                    wr.WriteLine("Time : " + DateTime.Now.ToShortTimeString());
                }
            }
        }
        public static byte[] GetByteArrayFromImage(Image img)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                buffer = ms.ToArray();
            }
            return buffer;
        }
        public static byte[] GetByteArrayFromStream(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static byte[] GetCompressedImageByteArrayFromImage(Bitmap img, int customHeight, int customWidth)
        {
            byte[] buffer;
            try
            {
                customWidth = (customWidth == 0) ? img.Width : customWidth;
                customHeight = (customHeight == 0) ? img.Height : customHeight;
                Bitmap newBitmap = new Bitmap(customWidth, customHeight, PixelFormat.Format24bppRgb);
                newBitmap = img;
                newBitmap.SetResolution(80, 80);
                Image compressedImage = newBitmap.GetThumbnailImage(customWidth, customHeight, null, IntPtr.Zero);
                buffer = GetByteArrayFromImage(compressedImage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }
        public static byte[] GetCompressedImageByteArrayFromStream(Stream stream, int customHeight, int customWidth)
        {
            byte[] buffer;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    buffer = ms.ToArray();
                    using (MemoryStream ms2 = new MemoryStream(buffer))
                    {
                        Bitmap img = new Bitmap(ms2);
                        customWidth = (customWidth == 0) ? img.Width : customWidth;
                        customHeight = (customHeight == 0) ? img.Height : customHeight;
                        Bitmap newBitmap = new Bitmap(customWidth, customHeight, PixelFormat.Format24bppRgb);
                        newBitmap = img;
                        newBitmap.SetResolution(80, 80);
                        Image compressedImage = newBitmap.GetThumbnailImage(customWidth, customHeight, null, IntPtr.Zero);
                        buffer = GetByteArrayFromImage(compressedImage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }

        public static byte[] GetByteArrayFromWebUrl(string requestUriString)
        {
            WebRequest req = null;
            WebResponse res = null;
            Stream stream = null;
            byte[] buffer = null;
            try
            {

                req = WebRequest.Create(requestUriString);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                res = req.GetResponse();
                stream = res.GetResponseStream();
                buffer = GetByteArrayFromStream(stream);
                res.Close();
                res.Dispose();
                stream.Close();
                stream.Dispose();
            }
            catch (ArgumentNullException ar)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("ArgumentNull downloading data");
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ar.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ar.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ar.StackTrace));

                Debug.Write(builder.ToString());
            }
            catch (UriFormatException ur)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("UriFormat downloading data");
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ur.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ur.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ur.StackTrace));

                Debug.Write(builder.ToString());
            }
            catch (System.Security.SecurityException se)
            {

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Security Error downloading data");
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Action: {0}", se.Action.GetDescription()));
                builder.AppendLine(string.Format("Exception Demanded: {0}", se.Demanded.ToString()));
                builder.AppendLine(string.Format("Exception Url: {0}", se.Url));
                builder.AppendLine(string.Format("Exception Type: {0}", se.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", se.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", se.StackTrace));

                Debug.Write(builder.ToString());
            }
            catch (NotSupportedException ns)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("NotSupported Error downloading data");
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ns.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ns.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ns.StackTrace));

                Debug.Write(builder.ToString());
            }
            catch (NotImplementedException ni)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("NotImplemented Error downloading data");
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ni.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ni.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ni.StackTrace));

                Debug.Write(builder.ToString());
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("General Error downloading data");
                builder.AppendLine(string.Format("Url: {0}", requestUriString));
                builder.AppendLine(string.Format("Exception Type: {0}", ex.GetType().Name));
                builder.AppendLine(string.Format("Exception Messgae: {0}", ex.Message));
                builder.AppendLine(string.Format("Exception StackTrace: {0}", ex.StackTrace));

                Debug.Write(builder.ToString());
            }
            return buffer;
        }
        public static KeyValuePair<int, int> GetCampaignAndComplaintId(string complaintId)
        {
            string[] strArr = complaintId.Split('-');
            return new KeyValuePair<int, int>(Convert.ToInt32(strArr[0]), Convert.ToInt32(strArr[1]));
        }

        public static Config.CommandStatus GetStatus(string status)
        {
            return (Config.CommandStatus)Enum.Parse(typeof(Config.CommandStatus), status);
        }

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

        public static T CastObj<T>(this object input)
        {
            Type t = typeof(T);
            if (input==null || (input is string && string.IsNullOrEmpty((string)input)))
            {
                if (t == typeof (bool))
                {
                    return (T) (false as object);
                }
                else if (t == typeof(bool?))
                {
                    return (T)(null as object);
                }
                else if (t == typeof(int))
                {
                    return (T)(0 as object);
                }
                else if (t == typeof (int?))
                {
                    return (T)(null as object);
                }
            }
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (input == null || DBNull.Value.Equals(input))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(input, t);
            }
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

        public static int EnumToInt(Config.Roles role)
        {
            return Convert.ToInt32(role);
        }

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
            return encryptedString.Replace("=", "");
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
        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }
        public static bool PasswordCharactersCheckAny(this string str, List<Config.PasswordProperty> listPasswordProperty = null, Config.PasswordProperty passwordProperty = Config.PasswordProperty.All)
        {
            if (listPasswordProperty == null)
            {
                listPasswordProperty = new List<Config.PasswordProperty> { passwordProperty };
            }
            char[] alphabetsLowerCase = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] alphabetsUpperCase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] characters = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '{', '[', '}', ']', ':', ';', '<', '>', '?', '/' };

            int index = -1;
            if (listPasswordProperty.Count > 0)
            {
                for (int i = 0; i < listPasswordProperty.Count; i++)
                {
                    index = (int)listPasswordProperty[i];
                    if (index == (int)Config.PasswordProperty.All)
                    {
                        for (int j = 0; j < alphabetsLowerCase.Length; j++)
                        {
                            if (str.Contains(alphabetsLowerCase[j]))
                            {
                                return true;
                            }
                        }
                        for (int j = 0; j < alphabetsUpperCase.Length; j++)
                        {
                            if (str.Contains(alphabetsUpperCase[j]))
                            {
                                return true;
                            }
                        }
                        for (int j = 0; j < numbers.Length; j++)
                        {
                            if (str.Contains(numbers[j]))
                            {
                                return true;
                            }
                        }
                        for (int j = 0; j < characters.Length; j++)
                        {
                            if (str.Contains(characters[j]))
                            {
                                return true;
                            }
                        }
                    }
                    else if (index == (int)Config.PasswordProperty.AlphabetsLowerCase)
                    {
                        for (int j = 0; j < alphabetsLowerCase.Length; j++)
                        {
                            if (str.Contains(alphabetsLowerCase[j]))
                            {
                                return true;
                            }
                        }
                    }
                    else if (index == (int)Config.PasswordProperty.AlphabetsUpperCase)
                    {
                        for (int j = 0; j < alphabetsUpperCase.Length; j++)
                        {
                            if (str.Contains(alphabetsUpperCase[j]))
                            {
                                return true;
                            }
                        }
                    }
                    else if (index == (int)Config.PasswordProperty.Characters)
                    {
                        for (int j = 0; j < characters.Length; j++)
                        {
                            if (str.Contains(characters[j]))
                            {
                                return true;
                            }
                        }
                    }
                    else if (index == (int)Config.PasswordProperty.Numbers)
                    {
                        for (int j = 0; j < numbers.Length; j++)
                        {
                            if (str.Contains(numbers[j]))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static IEnumerable<DbUsers> PasswordCharactersCheck(this IEnumerable<DbUsers> container, List<Config.PasswordProperty> listPasswordProperty = null, Config.PasswordProperty passwordProperty = Config.PasswordProperty.All)
        {
            if (listPasswordProperty == null)
            {
                listPasswordProperty = new List<Config.PasswordProperty> { passwordProperty };
            }
            foreach (var item in container)
            {
                if (item.Password.PasswordCharactersCheckAny(listPasswordProperty))
                {
                    yield return item;
                }
            }
        }
    }
}