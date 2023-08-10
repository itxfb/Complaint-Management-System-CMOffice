using System.Collections.Specialized;
using System.Data;
using System.Dynamic;
using System.Net;
using System.Reflection;
using System.Web.Security;
using Amazon.DynamoDBv2;
using Amazon.KeyManagementService.Model;
using Amazon.Runtime;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using System.Text;
using PITB.CMS.Models.View;
using System.IO;
using System.Web.Configuration;
using System.Security.Cryptography;
using pk.gov.punjab.pws.sdk;
using pk.gov.punjab.pws.sdk.Security;
using System.Globalization;
using System.Collections;

namespace PITB.CMS
{
    public class Utility
    {
        public static string RemoveSlashes(string str)
        {
            char[] adr = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
            }
            str = str.Replace("\\\"", "\"");
            return str;
            //str.Replace("\\\")
        }
        public static Dictionary<string, string> ConvertNewtonsoftDictionaryResponse(Dictionary<string, object> dict)
        {
            Dictionary<string, string> dictToRet = new Dictionary<string, string>();

            foreach (KeyValuePair<string, object> keyVal in dict)
            {
                string typeStr = keyVal.Value.GetType().ToString();
                if (typeStr.Contains("JArray"))
                {
                    //List<string> listStr =((JArray) keyVal.Value).ToObject<string>();
                    dictToRet.Add(keyVal.Key, string.Join(",", ((JArray)keyVal.Value).ToObject<List<string>>()));//keyVal.Value.ToString()
                }
                else
                {
                    dictToRet.Add(keyVal.Key, keyVal.Value.ToString());
                }
            }
            return dictToRet;
        }

        public static string GetUrl(string url)
        {
            if (Config.CURRENT_SERVER_TYPE == Config.ServerType.Local)
            {
                url = "http://localhost:2826/" + url;
            }
            else if (Config.CURRENT_SERVER_TYPE == Config.ServerType.Production)
            {
                url = "https://crm.punjab.gov.pk/rest/"+ url;
            }
            return url;
        }

        public static Dictionary<string, Dictionary<string, string>> ConvertCollonFormatToMultipleDict(string str)
        {
            Dictionary<string,string> dict = ConvertCollonFormatToDict(str);
            Dictionary<string, Dictionary<string, string>> dictMultiple = new Dictionary<string, Dictionary<string, string>>();
            foreach (KeyValuePair<string, string> keyVal in dict)
            {
                
                string[] val1Arr = keyVal.Value.Split(new string[] { "||" }, StringSplitOptions.None);
                var dictTemp = new Dictionary<string, string>();
                foreach (string v1 in val1Arr)
                {
                    Dictionary<string, string> dictMultipledictTemp = new Dictionary<string, string>();
                    string[] val2Arr = v1.Split(new string[] { ":" }, StringSplitOptions.None);
                    dictTemp.Add(val2Arr[0].Split(new string[] { "=" }, StringSplitOptions.None)[1], val2Arr[1].Split(new string[] { "=" }, StringSplitOptions.None)[1]);
                }
                dictMultiple.Add(keyVal.Key,dictTemp);
            }
            return dictMultiple;
        }


        public static Dictionary<string, string> ConvertCollonFormatToDict(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str.Trim();
                Dictionary<string, string> dictToRet = new Dictionary<string, string>();
                string[] strArrKeyVal = str.Split(new string[] {"__"}, StringSplitOptions.None);
                string[] tempKeyVal = null;
                foreach (string keyVal in strArrKeyVal)
                {
                    tempKeyVal = keyVal.Split(new string[] {"::"}, StringSplitOptions.None);
                    if (!dictToRet.ContainsKey(tempKeyVal[0]))
                    {
                        dictToRet.Add(tempKeyVal[0], tempKeyVal[1]);
                    }
                    else
                    {
                        string tempVal = dictToRet.First(x => x.Key.Equals(tempKeyVal[0])).Value + "," + tempKeyVal[1];
                        dictToRet.Remove(tempKeyVal[0]);
                        dictToRet.Add(tempKeyVal[0], tempVal);
                    }
                }
                return dictToRet;
            }
            return new Dictionary<string, string>();
        }

        public static Dictionary<string,string> GetDictParamsFromUrl(string url)
        {
            Dictionary<string, string> dictToRet = new Dictionary<string, string>();
            
            string[] str = url.Split('?');
            if(str.Length>1)
            {
                str = str[1].Split('&');
                foreach(string s in str)
                {
                    string[] t = s.Split('=');
                    if (t.Length > 0)
                    {
                        if (t.Length == 1)
                        {
                            t[1] = null;
                        }
                        dictToRet.Add(t[0], t[1]);
                    }
                }
                
            }
            else
            {
                return dictToRet;
            }
            return dictToRet;
        }

        public static List<int> ConvertStringListToIntList(List<string> listStr)
        {
            return listStr.Select(int.Parse).ToList();
        }

        public static List<int?> ConvertStringListToNullableIntList(List<string> listStr)
        {
            return GetNullableIntList(listStr.Select(int.Parse).ToList());
        }

        public static string ConvertDictToCollonFormat(Dictionary<string, string> dict)
        {
            string strToRet = strToRet = string.Join("__", dict.Select(x => x.Key + "::" + x.Value).ToArray());
            return strToRet;
        }

        public static Pair<string, string> GenerateForgeryKeyAgainstRequest(HttpRequestBase request)
        {
            List<string> listHeaders = new List<string>() { "Host"/*, "Referer"*/, "User-Agent", "Accept-Language", "Connection" };
            NameValueCollection headers = request.Headers;
            string headersValue = "";
            List<string> listHeadersForKeyGeneration = headers.AllKeys.Intersect(listHeaders).ToList();
            foreach (string key in listHeadersForKeyGeneration)
            {
                headersValue = key +"="+ headersValue + headers[key] + Config.Separator;
            }
            string strToEncrypt = headersValue +"Hash="+ GetUniqueToken(50, Config.EncryptionKey1);
            //Pair<string, string> pairValues = new Pair<string, string>(GetEncryptedString(strToEncrypt, Config.EncryptionKey2), GetEncryptedString(strToEncrypt, Config.EncryptionKey3));
            //headersValue = GetEncryptedString(headersValue) + GetUniqueToken(20, Config.EncryptionKey1);
            //Pair<string, string> decryptedValues = new Pair<string, string>(GetEncryptedString(strToEncrypt, Config.EncryptionKey2), GetEncryptedString(strToEncrypt, Config.EncryptionKey3));
            Pair<string, string> pairValues2 = new Pair<string, string>(GetEncryptedString(strToEncrypt, Config.EncryptionKey2), GetEncryptedString(strToEncrypt, Config.EncryptionKey3));
            //Pair<string, string> pairValues3 = new Pair<string, string>(GetDecryptedString(pairValues2.Item1, Config.EncryptionKey2), GetDecryptedString(pairValues2.Item2, Config.EncryptionKey3));
            return pairValues2;
        }

        //static readonly string PasswordHash = "P@@Sw0rd";
        
        public static string GetEncryptedString(string plainText, string key)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(Config.SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(Config.VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }



        public static string GetDecryptedString(string encryptedText, string key)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(Config.SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(Config.VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        //public static string GetEncryptedString(string input, string key)
        //{
        //    try
        //    {
        //        byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
        //        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        //        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        //        tripleDES.Mode = CipherMode.ECB;
        //        tripleDES.Padding = PaddingMode.PKCS7;
        //        ICryptoTransform cTransform = tripleDES.CreateEncryptor();
        //        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        //        tripleDES.Clear();
        //        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static string GetDecryptedString(string input, string key)
        //{
        //    byte[] inputArray = Convert.FromBase64String(input);
        //    TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        //    tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        //    tripleDES.Mode = CipherMode.ECB;
        //    tripleDES.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = tripleDES.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        //    tripleDES.Clear();
        //    return UTF8Encoding.UTF8.GetString(resultArray);
        //}

        public static string GetEncryptedString(string str)
        {
            try
            {
                //return str;
                //return Convert.ToBase64String(MachineKey.Protect(GetBytesFromString(str), "enc"));
                //return GetEncryptedString(str, Config.EncryptionKey1);
                return Encrypt(str,true);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static string GetDecryptedString(string str)
        {
            try
            {
                //return str;
                int strLength = str.Length;
               // return GetStringFromBytes(MachineKey.Decode(str, MachineKeyProtection.Encryption));
                //return GetStringFromBytes(MachineKey.Unprotect(GetBytesFromString(str), "enc"));
                //return GetDecryptedString(str, Config.EncryptionKey1);
                return Decrypt(str, true);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static byte[] GetBytesFromString(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetStringFromBytes(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        //Sami

        public static string Encrypt(string secureUserData, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(secureUserData);
            string key = string.Empty;
            byte[] resultArray;

            key = "S@mII198911";//ConfigurationManager.AppSettings.Get("SecurityKey");

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            byte[] resultArray;
            string key = string.Empty;

            key = "S@mII198911";//ConfigurationManager.AppSettings.Get("SecurityKey");  // Get the key from Web.Config file

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        //End Sami
        public static string GetUniqueToken(int length, string chars /*= "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_"*/)
        {
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];

                // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
                // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
                byte[] buffer = null;

                // Maximum random number that can be used without introducing a bias
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                crypto.GetBytes(data);

                char[] result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];

                    while (value > maxRandom)
                    {
                        if (buffer == null)
                        {
                            buffer = new byte[1];
                        }

                        crypto.GetBytes(buffer);
                        value = buffer[0];
                    }

                    result[i] = chars[value % chars.Length];
                }

                return new string(result);
            }
        }

        public static List<HttpPostedFileBase> GetListHttpPostedFileBase(HttpFileCollectionBase fileCollectionBase)
        {
            List<HttpPostedFileBase> listPostedFile = new List<HttpPostedFileBase>();
            foreach (string fileName in fileCollectionBase)
            {
                HttpPostedFileBase file = fileCollectionBase[fileName];
                listPostedFile.Add(fileCollectionBase[fileName]);
            }
            return listPostedFile;
        }

        public static JsonSerializerSettings GetJsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Error = (serializer, err) => err.ErrorContext.Handled = true;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return settings;
        }
        public static bool PropertyExists(dynamic dynamic, string property)
        {
            Type objType = dynamic.GetType();

            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)dynamic).ContainsKey(property);
            }

            return objType.GetProperty(property) != null;
        }

        public static dynamic GetDynamic(string jsonStr)
        {
            return (dynamic)JsonConvert.DeserializeObject(jsonStr, typeof(ExpandoObject)); 
        }

        public static List<dynamic> GetDynamicList(string jsonStr)
        {
            return (List<dynamic>)JsonConvert.DeserializeObject(jsonStr, typeof(List<ExpandoObject>));
        }


        public static dynamic GetApiResponse(bool isSuccessful = true, string Message = null, string status = null, dynamic d = null)
        {
            d = (d) ?? new ExpandoObject();
            if (isSuccessful)
            {
                d.Status = (status) ?? Config.ResponseType.Success.ToString();
                d.Message = (Message) ?? Config.ResponseType.Success.ToString();
            }
            else
            {
                d.Status = (status) ?? Config.ResponseType.Failure.ToString();
                d.Message = (Message) ?? "Server Error";
            }
            return d;
        }

        //public static dynamic SetApiResponse(dynamic d, bool isSuccessful)
        //{
        //    d.Status = 
        //    return d;
        //}

        private static bool IsJsonArray(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<string, string> GetDictionary(NameValueCollection nameValueCollection)
        {
            Dictionary<string,string> dict = new Dictionary<string, string>();
            foreach (var k in nameValueCollection.AllKeys)
            {
                dict.Add(k, nameValueCollection[k]);
            }
            return dict;
        }

        public static string GetTranslatedValue(Dictionary<string, dynamic> dict, string key, string language)
        {
            dynamic d = null;
            if (dict.TryGetValue(key, out d))
            {
                if (d != null)
                {
                    var dictV = (IDictionary<string, object>)d;
                    if (language == Config.Language.English.ToString())
                    {
                        return key;
                    }
                    else
                    {
                        return (string) dictV[language];
                    }

                }
                return null;
            }
            return null;
        }

        public static string GetUrlEncodedBytes(Dictionary<string, string> paramsDict)
        {
            StringBuilder paramz = new StringBuilder("");
            foreach (var parameter in paramsDict)
            {
                paramz.Append(parameter.Key);
                paramz.Append("=");
                //paramz.Append(parameter.Value);

                //paramz.Append(HttpUtility.UrlEncode(parameter.Value));
                paramz.Append(parameter.Value);
                paramz.Append("&");
            }
            paramz = paramz.Remove(paramz.Length - 1, 1);
            return paramz.ToString();
        }
        public static int[] GetIntArrayFromStringArray(string[] arr)
        {
            if (arr == null)
                return new int[0];
            if (arr.Length == 0)
                return new int[0];

            int length = arr.Length;
            int[] resArr = new int[length];
            for(int i = 0;i < length; i++)
            {
                int result = int.MinValue;
                bool isValid = int.TryParse(arr[i],out result);
                if (isValid)
                    resArr[i] = result;
                else
                    resArr[i] = -1;
            }
            return resArr;
        }
        public static string GetUrlEncodedBytes(Dictionary<string, object> paramsDict)
        {
            StringBuilder paramz = new StringBuilder("");
            foreach (var parameter in paramsDict)
            {
                paramz.Append(parameter.Key);
                paramz.Append("=");
                //paramz.Append(parameter.Value);

                //paramz.Append(HttpUtility.UrlEncode(parameter.Value));
                paramz.Append(parameter.Value);
                paramz.Append("&");
            }
            paramz = paramz.Remove(paramz.Length - 1, 1);
            return paramz.ToString();
        }

        public static string GetRandomKey(int bytelength)
        {
            byte[] buff = new byte[bytelength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buff);
            StringBuilder sb = new StringBuilder(bytelength * 2);
            for (int i = 0; i < buff.Length; i++)
                sb.Append(string.Format("{0:X2}", buff[i]));
            return sb.ToString();
        }
        public static int GetRandomNumber(int NumberOfdigits)
        {
            if (NumberOfdigits > 0)
            {
                Random generator = new Random();
                int r = generator.Next((int)(Math.Pow(10, NumberOfdigits - 1) - 1), (int)Math.Pow(10, NumberOfdigits));
                return r;
            }
            return 0;
        }
        public static string GetAutoGeneratedPassword(int length, List<Config.PasswordProperty> listPasswordProperty = null, Config.PasswordProperty passwordProperty = Config.PasswordProperty.All)
        {
            Random rnd = new Random();
            if (listPasswordProperty == null)
            {
                listPasswordProperty = new List<Config.PasswordProperty> { passwordProperty };
            }
            StringBuilder lStrPasswordBuilder = new StringBuilder();
            char[] alphabetsLowerCase = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] alphabetsUpperCase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] characters = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '{', '[', '}', ']', ':', ';', '<', '>', '?', '/' };
            if (length == 0 || length < 0)
            {
                return null;
            }
            int lastIndex = -1;
            char lastCharacter = '\0';
            for (int i = 0; i < length; i++)
            {
                int index = -1;

                if (listPasswordProperty != null && listPasswordProperty.Count > 0)
                {
                    if (listPasswordProperty[0] == Config.PasswordProperty.All)
                    {
                        if (lastIndex == 4 && lastCharacter == '<')
                        {
                            index = rnd.Next(3, 5);
                        }
                        else
                        {
                            index = rnd.Next(1, 5);
                        }
                    }
                    else
                    {
                        if (lastIndex == 4 && lastCharacter == '<')
                        {
                            index = rnd.Next(2, listPasswordProperty.Count);
                            index = (int)listPasswordProperty[index];
                        }
                        else
                        {
                            index = rnd.Next(0, listPasswordProperty.Count);
                            index = (int)listPasswordProperty[index];
                        }
                    }
                }



                if (index == (int)Config.PasswordProperty.AlphabetsLowerCase)
                {
                    lStrPasswordBuilder.Append(alphabetsLowerCase[rnd.Next(0, 26)]);
                }
                else if (index == (int)Config.PasswordProperty.AlphabetsUpperCase)
                {
                    lStrPasswordBuilder.Append(alphabetsUpperCase[rnd.Next(0, 26)]);
                }
                else if (index == (int)Config.PasswordProperty.Numbers)
                {
                    lStrPasswordBuilder.Append(numbers[rnd.Next(0, 10)]);
                }
                else if (index == (int)Config.PasswordProperty.Characters)
                {
                    lStrPasswordBuilder.Append(characters[rnd.Next(0, 25)]);
                }
                lastIndex = index;
                lastCharacter = lStrPasswordBuilder.ToString().ElementAt(lStrPasswordBuilder.ToString().Length - 1);
            }
            return (lStrPasswordBuilder.ToString().Length == length ? lStrPasswordBuilder.ToString() : null);
        }

        //Function ... you can create it in a different class  
        public static PITB.CMS.Config.CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = "6LcMtFgUAAAAACU_HF8T4USzDNoYsa09Dp2aJPfs";//System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<PITB.CMS.Config.CaptchaResponse>(jsonResult.ToString());
        }
        // GET: /CRUD/Edit/5  



        public static string ConvertDateTo_DD_MMMM_YY_H_MM_tt(DateTime dateTime)
        {
            return dateTime.ToString("dd MMMM yyyy h:mm tt");
        }

        public static string GetDateTimeStr(/*string currformat,*/ string dtStr, string returnedFormat=Config.SqlDateFormat)
        {
            //DateTime dt = DateTime.Parse(dtStr, currformat, System.Globalization.CultureInfo.InvariantCulture);
            DateTime dt = DateTime.Parse(dtStr, System.Globalization.CultureInfo.InvariantCulture);
            return dt.ToString(returnedFormat);
        }

        public static string GetMaximumTimeForDate(string date)
        {
            return date + " 23:59:59.999";
        }

        public static string GetSQLDateTimeFormat(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }


        public static DateTime GetDateTime(/*string currformat,*/ string dtStr, string returnedFormat = Config.SqlDateFormat)
        {
            //DateTime dt = DateTime.Parse(dtStr, currformat, System.Globalization.CultureInfo.InvariantCulture);
            DateTime dt = DateTime.Parse(dtStr, System.Globalization.CultureInfo.InvariantCulture);
            return dt;
        }

        public static string GetDateTimeStr(/*string currformat,*/ DateTime dt, string returnedFormat)
        {
            //DateTime dt = DateTime.Parse(dtStr, currformat, System.Globalization.CultureInfo.InvariantCulture);
            //string str = DateTime.Parse(returnedFormat);
            return dt.ToString(returnedFormat); //DateTime.Parse(dtStr, System.Globalization.CultureInfo.InvariantCulture);
            //return dtToRet.ToString(returnedFormat);
        }


        public static List<DateTime> GetDateDifference(DateTime startDate, DateTime endDate, string format)
        {
            DateTime iterator;
            DateTime limit;
            List<DateTime> listDateTime = new List<DateTime>();

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                //yield return Tuple.Create(
                //    dateTimeFormat.GetMonthName(iterator.Month),
                //    iterator.Year);
                
                listDateTime.Add(iterator);
                if (format == "day")
                {
                    iterator = iterator.AddDays(1);
                }
                else if (format == "month")
                {
                    iterator = iterator.AddMonths(1);
                }
                else if (format == "year")
                {
                    iterator = iterator.AddYears(1);
                }
            }
            return listDateTime;
        }

        public static string GetYesNoFromBool(bool isTrue)
        {
            if (isTrue) return "Yes";
            else return "No";
        }

        public static string GetN_A(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Config.None;
            }
            return str;
        }

        public static KeyValuePair<string,string> GetKeyVal(string str)
        {
            string[] strArr = Utility.Split(str, Config.Separator);
            return new KeyValuePair<string, string>(strArr[0],strArr[1]);
        }

        public static string GetKey(string str)
        {
            string[] strArr = Utility.Split(str, Config.Separator);
            return strArr[0];
        }


        public static int GetIntFromYesNo(string str)
        {
            if (str.ToLower().Contains("yes"))
            {
                return 1;
            }
            else if (str.ToLower().Contains("no"))
            {
                return 2;
            }
            return -1;
        }

        public static string[] Split(string strToSplit, string splitSeq)
        {
            return strToSplit.Split(new string[] { splitSeq }, StringSplitOptions.None);
        }

        public static string GetCommaSepStrFromList(List<int> listInt)
        {
            return string.Join(",", listInt.Select(n => n.ToString()).ToArray());
        }

        public static string GetCommaSepStrFromList(List<string> listStr)
        {
            return string.Join(",", listStr.Select(n => n.ToString()).ToArray());
        }

        public static string GetCommaSepStrFromList(List<int?> listInt)
        {
            return string.Join(",", listInt.Select(n => n.ToString()).ToArray());
        }

        public static int GetIntByCommaSepStr(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return Convert.ToInt32(str.Split(',').Select(int.Parse).ToList()[0]);
            }
            else return -1;
        }

        public static List<int?> GetNullableIntList(List<int> listInt)
        {
            //List<int> listInt = str.Split(',').Select(int.Parse).ToList();
            List<int?> listNullableInt = new List<int?>(listInt.Count); // Allocate enough memory for all items
            foreach (var i in listInt)
            {
                listNullableInt.Add(i);
            }
            return listNullableInt;
        }

        public static List<int> GetIntList(List<int?> listInt)
        {
            //List<int> listInt = str.Split(',').Select(int.Parse).ToList();
            List<int> listNullableInt = new List<int>(listInt.Count); // Allocate enough memory for all items
            foreach (var i in listInt)
            {
                listNullableInt.Add(Convert.ToInt32(i));
            }
            return listNullableInt;
        }

        public static List<int> GetIntList(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    return str.Split(',') != null && str.Split(',').Length > 0 ? str.Split(',').Select(int.Parse).ToList() : new List<int>();
                }
                catch (FormatException fe)
                {
                    return new List<int>();
                }
            }
            else
            {
                return new List<int>();
            }
        }


        public static List<int?> GetNullableIntList(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<int?>();
            }
            else
            {
                List<int> listInt = str.Split(',').Select(int.Parse).ToList();
                List<int?> listNullableInt = new List<int?>(listInt.Count); // Allocate enough memory for all items
                foreach (var i in listInt)
                {
                    listNullableInt.Add(i);
                }
                return listNullableInt;
            }
        }

        public static string GetDBSearchQueryOnIndividualWords(string fieldName, string searchStr)
        {
            string searchQuery = "";
            List<string> listWords = GetIndividualStrList(searchStr);

            int count = 0;
            foreach (string word in listWords)
            {


                searchQuery = searchQuery + fieldName + " like " + "'%" + word + "%'";

                if (count < listWords.Count - 1)
                {
                    searchQuery = searchQuery + " AND ";
                }
                count++;
            }
            return searchQuery;
        }

        public static string GetConvertedString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = "0";
            }
            return str;
        }

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public static int? GetConvertedNullInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = "0";
            }
            int number = Int32.MinValue;
            Int32.TryParse(str, out number);
            if (number == 0)
            {
                str = "0";
            }
            return Convert.ToInt32(str);
        }

        public static string GetConvertedString(int? val)
        {
            string str = null;
            if (val == null)
            {
                str = "0";
            }
            else
            {
                str = val.ToString();
            }
            return str;
        }

        public static List<string> GetIndividualStrList(string searchStr)
        {
            List<string> wordsList = new List<string>();
            string word = "";
            bool isValid = false;

            for (int i = 0; i < searchStr.Length; i++)
            {
                isValid = IsCharValidAndNotSpecialChar(searchStr[i]);
                if (isValid)
                {
                    word = word + searchStr[i];
                    if (i == searchStr.Length - 1)
                    {
                        wordsList.Add(word);
                    }
                }
                else
                {
                    if (word != "")
                    {
                        wordsList.Add(word);
                    }
                    word = "";
                }
            }

            return wordsList;
        }

        public static Tuple<int, int> GetRowRangeFromPageIndex(int pageIndex)
        {
            int from = ((pageIndex - 1) * Config.ServerSideDropDownListSize) + 1;
            int to = ((pageIndex - 1) * Config.ServerSideDropDownListSize) + Config.ServerSideDropDownListSize;
            return new Tuple<int, int>(from, to);
        }

        public static bool IsCharValidAndNotSpecialChar(char c)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public static string GetMergedStrList(string seperator, List<string> listStr )
        //{
        //    for (int i = 0; i < listStr.Count; i++)
        //    {

        //    }
        //    return campaignId + "-" + complaintId;
        //}

        public static string GetComplaintIdStr(int campaignId, int complaintId)
        {
            return campaignId + "-" + complaintId;
        }

        public static string GetComplaintIdStr(DbComplaint dbComplaint)
        {
            return dbComplaint.Compaign_Id + "-" + dbComplaint.Id;
        }

        public static object GetPropertyThroughReflection(object obj, string propertyName)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop)
            {
                return prop.GetValue(obj, null);
            }
            return null;
        }

        public static bool IsPropertyPresent(object obj, string propertyName)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop)
            {
                return true;
            }
            return false;
        }

        public static bool IsPropertyAndNotNull(object obj, string propertyName)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop)
            {
                object returnObj = prop.GetValue(obj, null);
                if (returnObj != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetPropertyThroughReflection(object obj, string propertyName, int? propertyValue)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(obj, propertyValue, null);
            }
        }

        public static void SetPropertyThroughReflection(object obj, string propertyName, object propertyValue)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(obj, propertyValue, null);
            }
        }

        public static void SetPropertyThroughReflection(object obj, string propertyName, DateTime? propertyValue)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(obj, propertyValue, null);
            }
        }

        //public static object HasChanged(object obj1, object obj2)
        //{
        //    //------- copied code
        //    //object copiedObject = Activator.CreateInstance(objToCopy.GetType());
        //    // Get all FieldInfo. 

        //    if (obj1 == null)
        //        return null;
        //    Type type = obj1.GetType();
        //    if (type.IsClass)
        //    {
        //        return obj;
        //    }

        //    FieldInfo[] fields1 = obj1.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    FieldInfo[] fields1 = obj1.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    foreach (FieldInfo field in fields1)
        //    {
        //        //object fieldValue = field.GetValue(objToCopy);
        //        //if (fieldValue != null)
        //        {
        //            var value = field.GetValue(objToCopy);
        //            field.SetValue(objToCopy, value);
        //        }


        //    }
        //    return copiedObject;
        //}

        //public static object CreateNewInstance(object objToCopy)
        //{
        //    //------- copied code
        //    object copiedObject = Activator.CreateInstance(objToCopy.GetType());
        //    // Get all FieldInfo. 
        //    FieldInfo[] fields = copiedObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    foreach (FieldInfo field in fields)
        //    {
        //        //object fieldValue = field.GetValue(objToCopy);
        //        //if (fieldValue != null)
        //        {
        //            var value = field.GetValue(objToCopy);
        //            field.SetValue(objToCopy, value);
        //        }


        //    }
        //    return copiedObject;
        //}

        public static void SetPropertyThroughReflection(object obj, string propertyName, string propertyValue)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(obj, propertyValue, null);
            }
        }

        public static Config.AmazonConfig GetAmazonConfigModel()
        {
            //string connectionStr = ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString;
            string value = ConfigurationManager.AppSettings["AmazonConnectionType"];
            //if (connectionStr.Contains("10.50.16.35") || connectionStr.Contains("119.159.228.5"))
            switch (value)
            {
                case "Local":
                    return new Config.AmazonConfig(Config.AmazonConfigDevKeyId, Config.AmazonConfigDevSecretKey, Config.AmazonDevBucket, Config.AmazonDevUrlPrefix);
                    break;
                case "Production":
                    return new Config.AmazonConfig(Config.AmazonConfigProdKeyId, Config.AmazonConfigProdSecretKey, Config.AmazonProdBucket, Config.AmazonProdUrlPrefix);
                    break;
            }
            return null;
        }

        public static Config.AmazonConfig GetPWSConfigModel()
        {
            //string connectionStr = ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString;
            string value = ConfigurationManager.AppSettings["AmazonConnectionType"];

            switch (value)
            {
                case "Local":
                    return new Config.AmazonConfig(Config.AmazonConfigDevKeyId, Config.AmazonConfigDevSecretKey, Config.AmazonDevBucket, Config.AmazonDevUrlPrefix);
                    break;
                case "Production":
                    return new Config.AmazonConfig(Config.AmazonConfigProdKeyId, Config.AmazonConfigProdSecretKey, Config.AmazonProdBucket, Config.AmazonProdUrlPrefix);
                    break;
            }
            return null;
        }

        public static Config.PWSConfig GetPwsClient()
        {
            Config.PWSConfig config = new Config.PWSConfig();
            string value = ConfigurationManager.AppSettings["AmazonConnectionType"];

            switch (value)
            {
                case "Local":
                    return  new Config.PWSConfig(Config.PWSConfigDevKeyId, Config.PWSConfigDevSecretKey, Config.PWSDevBucket, Config.PWSDevUrlPrefix);
                    break;
                case "Production":
                    return new Config.PWSConfig(Config.PWSConfigProdKeyId, Config.PWSConfigProdSecretKey, Config.PWSProdBucket, Config.PWSProdUrlPrefix);
                    break;
            }


            return null;
        }

        public static string GetTranslation(string str)
        {
            string strRet = null;
            Config.DictTranslationMap.TryGetValue(str, out strRet);
            if (strRet == null)
            {
                return str;
            }
            return strRet;
        }

        public static AmazonS3Client GetS3Client(string amazonKey, string amazonSecretKey)
        {

            //NameValueCollection appConfig = ConfigurationManager.AppSettings;
            AWSCredentials credentials = new BasicAWSCredentials(amazonKey, amazonSecretKey);
            AmazonS3Client client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest1);

            return client;
        }

        public static string GetUniqueFileName(string fileName, string complaintId, string fileExt)
        {
            return (DateTime.Now.Year + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "--" + complaintId + "--" + fileName + "--" + Guid.NewGuid().ToString("N") + fileExt);
        }

        public static List<SelectListItem> GetBinarySelectedListItem()
        {
            List<SelectListItem> listBinary = new List<SelectListItem>();
            listBinary.Add(new SelectListItem() { Value = "1", Text = "Yes" });
            listBinary.Add(new SelectListItem() { Value = "0", Text = "No" });
            return listBinary;
        }

        public static int GetFirstFromCommaSeperatedString(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                List<int> listIds = str.Split(',').Select(int.Parse).ToList();
                return listIds[0];
            }
            return -1;
        }

        public static bool IsArrayElementPresentInString(string str, List<int> list)
        {
            List<int> listInt = Utility.GetIntList(str);
            foreach (int el in list)
            {
                if (listInt.Contains(el))
                {
                    return true;
                }
            }
            return false;
        }



        public static bool IsElementPresentInString(string str, int id)
        {
            return (Utility.GetIntList(str).Contains(id));
        }
        public static bool IsAnyMultipleElementsPresentInString(string str1, string str2)
        {
            List<int> str2Values = Utility.GetIntList(str2);
            bool flag = false;
            foreach (int value in str2Values)
            {
                if (Utility.GetIntList(str1).Contains(value))
                    flag = true;
            }
            return flag;
        }
        public static bool IsAllMultipleElementsPresentInString(string str1, string str2)
        {
            List<int> str2Values = Utility.GetIntList(str2);
            bool[] flag = new bool[str2Values.Count];
            for (int i = 0; i < flag.Length; i++)
                flag[i] = false;
            for (int j = 0; j < str2Values.Count; j++)
            {
                if (Utility.GetIntList(str1).Contains(str2Values[j]))
                    flag[j] = true;
            } 
            return flag.All(x=> x== true);
        }
        public static string ReplaceStringWithParams(string query, Dictionary<string, string> paramDictionary)
        {
            foreach (KeyValuePair<string, string> param in paramDictionary)
            {
                query.Replace(param.Key, param.Value);
            }
            return query;
        }

        public static DbUsers GetUserFromCookie()
        {
            DbUsers dbUser = DbUsers.GetActiveUser(new AuthenticationHandler().CmsCookie.UserId);
            return dbUser;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') /*|| c == '.' || c == '_'*/)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }



        public static List<Tuple<int, string>> GetHierarchyMappingListByUser(DbUsers dbUsers, int hierarchyId, bool desc = true)
        {
            List<Tuple<int, string>> listHierarchyMapping = new List<Tuple<int, string>>();

            string hierarchyValue = "";
            if (desc)
            {
                for (int i = hierarchyId; i > 0; i--)
                {
                    hierarchyValue = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUsers, (Config.Hierarchy)i);
                    listHierarchyMapping.Add(new Tuple<int, string>(i, hierarchyValue));
                }
            }
            else
            {
                for (int i = 1; i <= hierarchyId; i++)
                {
                    hierarchyValue = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUsers, (Config.Hierarchy)i);
                    listHierarchyMapping.Add(new Tuple<int, string>(i, hierarchyValue));
                }
            }

            return listHierarchyMapping;
        }

        public static List<Tuple<int, string>> GetHierarchyMappingListByComplaint(DbComplaint dbComplaint, int hierarchyId)
        {
            List<Tuple<int, string>> listHierarchyMapping = new List<Tuple<int, string>>();

            string hierarchyValue = "";
            for (int i = hierarchyId; i > 0; i--)
            {
                hierarchyValue = DbComplaint.GetHierarchyIdValueAgainstHierarchyId((Config.Hierarchy)i, dbComplaint).ToString();
                listHierarchyMapping.Add(new Tuple<int, string>(i, hierarchyValue));
            }

            return listHierarchyMapping;
        }


        public static string GetHierarchyValueName(Config.Hierarchy hierarchyId, CMSCookie cookie)
        {
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    return DbProvince.GetById(Utility.GetIntByCommaSepStr(cookie.ProvinceId)).Province_Name;
                    break;

                case Config.Hierarchy.Division:
                    return DbDivision.GetById(Utility.GetIntByCommaSepStr(cookie.DivisionId)).Division_Name;
                    break;

                case Config.Hierarchy.District:
                    return DbDistrict.GetById(Utility.GetIntByCommaSepStr(cookie.DistrictId)).District_Name;
                    break;

                case Config.Hierarchy.Tehsil:
                    return DbTehsil.GetById(Utility.GetIntByCommaSepStr(cookie.TehsilId)).Tehsil_Name;
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return DbUnionCouncils.GetById(Utility.GetIntByCommaSepStr(cookie.UcId)).Councils_Name;
                    break;

                case Config.Hierarchy.Ward:
                    return DbWards.GetByWardId(Utility.GetIntByCommaSepStr(cookie.WardId)).Wards_Name;
                    break;
            }
            return "";
        }

        public static List<int> GetHierarchyValue(Config.Hierarchy hierarchyId, CMSCookie cookie)
        {
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    return Utility.GetIntList(cookie.ProvinceId);
                    break;

                case Config.Hierarchy.Division:
                    return Utility.GetIntList(cookie.DivisionId);
                    break;

                case Config.Hierarchy.District:
                    return Utility.GetIntList(cookie.DistrictId);
                    break;

                case Config.Hierarchy.Tehsil:
                    return Utility.GetIntList(cookie.TehsilId);
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return Utility.GetIntList(cookie.UcId);
                    break;

                case Config.Hierarchy.Ward:
                    return Utility.GetIntList(cookie.WardId);
                    break;
            }
            return new List<int>(){-1};
        }

        public static string GetHierarchyValueName(Config.Hierarchy hierarchyId, int hierarchyVal)
        {
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    return DbProvince.GetById(hierarchyVal).Province_Name;
                    break;

                case Config.Hierarchy.Division:
                    return DbDivision.GetById(hierarchyVal).Division_Name;
                    break;

                case Config.Hierarchy.District:
                    return DbDistrict.GetById(hierarchyVal).District_Name;
                    break;

                case Config.Hierarchy.Tehsil:
                    return DbTehsil.GetById(hierarchyVal).Tehsil_Name;
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return DbUnionCouncils.GetById(hierarchyVal).Councils_Name;
                    break;

                case Config.Hierarchy.Ward:
                    return DbWards.GetByWardId(hierarchyVal).Wards_Name;
                    break;
            }
            return "";
        }
        public static string GetHierarchyColumnNameByHierarchyIdComplaintsTable(int hierarchyId)
        {
            string hierarchyColumnName = null;
            switch (hierarchyId)
            {
                case (int)Config.Hierarchy.Province:
                    hierarchyColumnName = "Province_Id,Province_Name";
                    break;
                case (int)Config.Hierarchy.Division:
                    hierarchyColumnName = "Division_Id,Division_Name";
                    break;
                case (int)Config.Hierarchy.District:
                    hierarchyColumnName = "District_Id,District_Name";
                    break;
                case (int)Config.Hierarchy.Tehsil:
                    hierarchyColumnName = "Tehsil_Id,Tehsil_Name";
                    break;
                case (int)Config.Hierarchy.UnionCouncil:
                    hierarchyColumnName = "UnionCouncil_Id,UnionCouncil_Name";
                    break;
                case (int)Config.Hierarchy.Ward:
                    hierarchyColumnName = "Ward_Id,Ward_Name";
                    break;
                case (int)Config.Hierarchy.None:
                    hierarchyColumnName = null;
                    break;
                default:
                    hierarchyColumnName = string.Empty;
                    break;
            }
            return hierarchyColumnName;
        }

      
        public static string GetHierachyNameByHierarchyId(int hierarchyId)
        {
            string hierarchyName = null;
            switch (hierarchyId)
            {
                case (int)Config.Hierarchy.Province:
                    hierarchyName = "Province";
                    break;
                case (int)Config.Hierarchy.Division:
                    hierarchyName = "Division";
                    break;
                case (int)Config.Hierarchy.District:
                    hierarchyName = "District";
                    break;
                case (int)Config.Hierarchy.Tehsil:
                    hierarchyName = "Tehsil";
                    break;
                case (int)Config.Hierarchy.UnionCouncil:
                    hierarchyName = "UnionCouncil";
                    break;
                case (int)Config.Hierarchy.Ward:
                    hierarchyName = "Ward";
                    break;
                case (int)Config.Hierarchy.None:
                    hierarchyName = "None";
                    break;
                default:
                    hierarchyName = string.Empty;
                    break;
            }
            return hierarchyName;
        }
        public static List<int> GetHierarchyValueUnderHierarchy(Config.Hierarchy hierarchyId, string hierarchyVal, int campaignId)
        {
            int hierarchyIdForGroupMapping = (int)hierarchyId;
            hierarchyIdForGroupMapping++;
            if (hierarchyIdForGroupMapping == (int)Config.Hierarchy.Division)
            {
                hierarchyIdForGroupMapping++;
            }
            List<int?> listValues = Utility.GetNullableIntList(hierarchyVal);

            List<DbHierarchyCampaignGroupMapping> listHierarchyCampaignGroupMappings =
                DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId);

            int? groupId = null;
            DbHierarchyCampaignGroupMapping hierarchyGroupMapping = listHierarchyCampaignGroupMappings.Where(n => n.Hierarchy_Id == (int)hierarchyIdForGroupMapping)
                            .FirstOrDefault();

            if (hierarchyGroupMapping != null)
            {
                groupId = hierarchyGroupMapping.Group_Id;
            }

            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    //return DbDistrict.GetByProvinceIdsList(listValues, groupId).Where(n=> new List<int>(){1,8,34,35}.Contains(n.District_Id)).Select(n => n.District_Id).ToList();
                    return DbDistrict.GetByProvinceIdsList(listValues, groupId).Select(n => n.District_Id).ToList();
                    break;

                case Config.Hierarchy.Division:
                    //return DbDivision.GetById(hierarchyVal).Division_Name;
                    break;

                case Config.Hierarchy.District:
                    return DbTehsil.GetByDistrictIdList(listValues, groupId).Select(n => n.Tehsil_Id).ToList();
                    //return DbDistrict.GetById(hierarchyVal).District_Name;
                    break;

                case Config.Hierarchy.Tehsil:
                    return DbUnionCouncils.GetUnionCouncilList(listValues, groupId).Select(n => n.UnionCouncil_Id).ToList();
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return DbWards.GetByUc(listValues, groupId).Select(n => n.Ward_Id).ToList();
                    break;

                case Config.Hierarchy.Ward:
                    //return DbWards.GetByUnionCouncilId(hierarchyVal).FirstOrDefault().Wards_Name;
                    return new List<int>();
                    break;
            }
            return new List<int>();
        }

        public static TimeSpan GetTimeSpanFromString(string remainingTime)
        {
            TimeSpan span = new TimeSpan();
            if (!string.IsNullOrEmpty(remainingTime) && remainingTime != "None")
            {
                string[] timeArr = remainingTime.Split(':');

                int days = 0, hrs = 0, mins = 0, sec = 0;
                for (int i = 0; i < timeArr.Length; i++)
                {
                    if (timeArr[i].Contains("days"))
                    {
                        timeArr[i] = timeArr[i].Replace("days", "");
                        timeArr[i] = timeArr[i].Replace(" ", "");
                        days = Convert.ToInt32(timeArr[i]);
                    }
                    else if (timeArr[i].Contains("hr"))
                    {
                        timeArr[i] = timeArr[i].Replace("hr", "");
                        timeArr[i] = timeArr[i].Replace(" ", "");
                        hrs = Convert.ToInt32(timeArr[i]);
                    }
                    else if (timeArr[i].Contains("mins"))
                    {
                        timeArr[i] = timeArr[i].Replace("mins", "");
                        timeArr[i] = timeArr[i].Replace(" ", "");
                        mins = Convert.ToInt32(timeArr[i]);
                    }
                    else if (timeArr[i].Contains("sec"))
                    {
                        timeArr[i] = timeArr[i].Replace("sec", "");
                        timeArr[i] = timeArr[i].Replace(" ", "");
                        sec = Convert.ToInt32(timeArr[i]);
                    }
                }
                span = new TimeSpan(days, hrs, mins, sec);
            }
            return span;
        }

        public static int GetUserHierarchyIdValue()
        {
            int userId = AuthenticationHandler.GetCookie().UserId;
            Config.Hierarchy hierarchyId = AuthenticationHandler.GetCookie().Hierarchy_Id;
            DbUsers dbUser = DbUsers.GetActiveUser(AuthenticationHandler.GetCookie().UserId);
            string hierarchyIdVal = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser, hierarchyId);
            int hierarchyIdValInt = Convert.ToInt32(hierarchyIdVal.Split(',')[0]);
            return hierarchyIdValInt;
        }

        public static string GetUserHierarchyValueString()
        {
            int userId = AuthenticationHandler.GetCookie().UserId;
            Config.Hierarchy hierarchyId = AuthenticationHandler.GetCookie().Hierarchy_Id;
            DbUsers dbUser = DbUsers.GetActiveUser(AuthenticationHandler.GetCookie().UserId);
            string hierarchyIdVal = DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser, hierarchyId);
            int hierarchyIdValInt = Convert.ToInt32(hierarchyIdVal.Split(',')[0]);
            string hierarchyIdValString = Utility.GetHierarchyValueName(hierarchyId, hierarchyIdValInt);
            return hierarchyIdValString;
        }

        public static void MergeLists(List<VmStatusCount> listDestination, List<VmStatusCount> listSource)
        {
            int count = 0;
            foreach (VmStatusCount vmStatusCount in listSource)
            {
                count = listDestination.Where(n => n.StatusId == vmStatusCount.StatusId).Count();
                if (count == 0)
                {
                    listDestination.Add(vmStatusCount);
                }
            }
        }

        public static string GetStrForDisplay(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Config.None;
            }
            return str.Trim();
        }

        public static string PaddLeft(string str, int maxChars, char padChar)
        {
            str = str.PadLeft((maxChars - str.Length), padChar);
            return str.Trim();
        }




        public static string GetAlteredStatus(string statusStr, string matchedStr, string strtoReturn)
        {
            if (statusStr == matchedStr)
            {
                return strtoReturn;
            }
            return statusStr;
        }

        public static string GetAlteredStatus(int campaignId, string statusStr)
        {
            if (campaignId == (int)Config.Campaign.SchoolEducationEnhanced)
            {
                if (statusStr == Config.UnsatisfactoryClosedStatus)
                {
                    return Config.SchoolEducationUnsatisfactoryStatus;
                }
            }
            return statusStr;
        }

        public static List<SelectListItem> GetAlteredStatus(List<SelectListItem> listCampaignStatuses, string matchedStr, string strtoReturn)
        {
            SelectListItem selectedListItem = listCampaignStatuses.FirstOrDefault(n => n.Text == matchedStr);
            if (selectedListItem != null)
            {
                selectedListItem.Text = strtoReturn;
            }
            return listCampaignStatuses;
        }

        public static DataTable GetAlterStatusDataTable(DataTable dt, string columnName, string matchedStr, string strtoReturn)
        {
            foreach (DataRow dtRow in dt.Rows)
            {
                // On all tables' columns
                //if (Convert.ToInt32(dtRow["Campaign_Id"]) == (int)Config.Campaign.SchoolEducationEnhanced)
                if (dt.Columns.Contains(columnName))
                {
                    dtRow[columnName] = Utility.GetAlteredStatus(dtRow[columnName].ToString(), matchedStr, strtoReturn);
                }
            }

            return dt;
        }

        public static DataTable GetAlterStatusDataTable(DataTable dt, string campaignColumnName, string statusColumnName, string matchedStr, string strtoReturn)
        {
            foreach (DataRow dtRow in dt.Rows)
            {
                // On all tables' columns
                //if (Convert.ToInt32(dtRow["Campaign_Id"]) == (int)Config.Campaign.SchoolEducationEnhanced)
                if (dt.Columns.Contains(campaignColumnName))
                {
                    if (dtRow[campaignColumnName].ToString().ToLower() == Config.CampaignSchoolEducation.ToLower()
                        || dtRow[campaignColumnName].ToString().ToLower() == ((int)Config.Campaign.SchoolEducationEnhanced).ToString())
                    {
                        dtRow[statusColumnName] = Utility.GetAlteredStatus(dtRow[statusColumnName].ToString(), matchedStr,
                            strtoReturn);
                    }
                }
            }

            return dt;
        }

        public static List<SelectListItem> GetSortedSelectedListItem(List<SelectListItem> listItems, List<int> listSortedList)
        {
            return listItems.OrderBy(n => listSortedList.IndexOf(Convert.ToInt32(n.Value))).ToList();
        }


        public static List<SelectListItem> GetSortedSelectedListItem(List<SelectListItem> listItems, string commaSepStr)
        {
            List<int> listSortedList = GetIntList(commaSepStr);
            return listItems.OrderBy(n => listSortedList.IndexOf(Convert.ToInt32(n.Value))).ToList();
        }

        public static void WriteFile(string path, string fileName, string txt)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
             File.WriteAllText(path + fileName, txt);
            
        }
        public static void writeLogFile(Exception exeption)
        {

            string path = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["Logs"].ToString());
            path += "\\" + DateTime.Now.ToString("MMyyyy");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter writer = new StreamWriter(path + "\\" + DateTime.Now.ToString("ddMMyyyy") + ".txt", true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Message :" + exeption.Message);
                writer.WriteLine("StackTrace :" + exeption.StackTrace);
                writer.WriteLine("Date :" + DateTime.Now.ToString());
                writer.WriteLine("Data :" + exeption.Data);
                writer.WriteLine("TargetSite :" + exeption.TargetSite);
                writer.WriteLine("HelpLink :" + exeption.HelpLink);
                writer.WriteLine("-----------------------------------------------------------------------------" + Environment.NewLine);
            }
            //file.Close();
        }
        public static void writeLogFile(string errorMessage, string errorDescription)
        {

            string path = System.Web.Hosting.HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["Logs"].ToString());
            path += "\\" + DateTime.Now.ToString("MMyyyy");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter writer = new StreamWriter(path + "\\" + DateTime.Now.ToString("ddMMyyyy") + ".txt", true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Message :" + errorMessage);
                writer.WriteLine("Description :" + errorDescription);
                writer.WriteLine("-----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
            //file.Close();
        }
        public static int GetFileLineNumber()
        {
            return (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber();
        }
        public static bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        public static bool IsNumericLetter(char c)
        {
            return (c >= '0' && c <= '9') ;
        }
        public static List<Config.CampaignConfig> GetCampaignConfigList(List<Config.AppConfig> listAppCampaignConfigurations, Config.AppID appId)
        {
            Config.AppConfig appConfig = listAppCampaignConfigurations.FirstOrDefault(n => n.AppId == appId);
            if (appConfig != null)
            {
                return appConfig.ListCampaigns;
            }
            return null;
        }

        public static string GetEndingTag(string tag)
        {
            int index = tag.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);
            string endingTag = tag.Insert(index + 1, "/");
            return endingTag;
        }

        public static T GetDynamicValue<T>(dynamic d, string property) where T : class
        {
            IDictionary<string, object> dict = d;
            if (dict.ContainsKey(property))
            {
                return (T) dict[property];
            }
            else
            {
                return null;
            }
        }

        public static T GetDictValue<T>(IDictionary<string, object> dict, string property) where T : class
        {
            //IDictionary<string, object> dict = d;
            if (dict.ContainsKey(property))
            {
                return (T)dict[property];
            }
            else
            {
                return null;
            }
        }

        //public static T GetValueInt<T>(dynamic d, string property) where T :struct, System.Enum
        //{
        //    IDictionary<string, object> dict = d;
        //    if (dict.ContainsKey(property))
        //    {
        //        return (T)dict[property];
        //    }
        //    else
        //    {
        //        return -1;
        //    }
        //}

    }



    public class CampaignComparer : IComparer<VmCampaign>
    {
        public int Compare(VmCampaign x, VmCampaign y)
        {
            int[] order = new int[3] { 1, 70, 47 };
            if ((x.Id == 1 && y.Id == 70) || (x.Id == 1 && y.Id == 47) || (x.Id == 70 && y.Id == 47))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }


}