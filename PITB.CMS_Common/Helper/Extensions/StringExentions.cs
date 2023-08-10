using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Helper.Extensions
{
    public static class StringExentions
    {
        public static string FirstLetterUpperCase(this string param)
        {
            if (string.IsNullOrEmpty(param))
                return null;
            char[] destination = new char[param.Length];
            param.CopyTo(1, destination, 1, param.Length - 1);
            destination[0] = param.ElementAt(0).ToString().ToUpper().ToCharArray().First();
            return string.Concat(destination);
        }
    }
}