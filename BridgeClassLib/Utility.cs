using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;
using BridgeDTO.Form.DynamicForm;

namespace BridgeClassLib
{
    [FileName("Form.js")]
    public class Utility
    {
        public static Dictionary<string, string> ConvertCollonFormatToDict(string str)
        {
            Dictionary<string, string> dictToRet = new Dictionary<string, string>();
            string[] strArrKeyVal = str.Split(new string[] { "__" }, StringSplitOptions.None);
            string[] tempKeyVal = null;
            foreach (string keyVal in strArrKeyVal)
            {
                tempKeyVal = keyVal.Split(new string[] { "::" }, StringSplitOptions.None);
                dictToRet.Add(tempKeyVal[0], tempKeyVal[1]);
            }
            return dictToRet;
        }
        public static bool IsSubstringPresent(List<FormPermission> listPermissions, string str)
        {
            return (listPermissions.FirstOrDefault(n => n.Key.Contains(str)) != null);
        }

        public static bool IsStringPresent(List<FormPermission> listPermissions, string str)
        {
            return (listPermissions.FirstOrDefault(n => n.Key.Equals(str)) != null);
        }
    }
}
