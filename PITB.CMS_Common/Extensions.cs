using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common
{
    public static class Extensions
    {
        public static bool PropertyExists(dynamic dynamic, string property)
        {
            Type objType = dynamic.GetType();

            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)dynamic).ContainsKey(property);
            }

            return objType.GetProperty(property) != null;
        }

        public static string ToCommaSepratedString(this int[] arrayOfIntegers)
        {
            return string.Join(",", arrayOfIntegers);
        }
        public static string ToCommaSepratedString(this List<int> arrayOfIntegers)
        {
            return string.Join(",", arrayOfIntegers);
        }
    }
}
