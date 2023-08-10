using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_WindowService
{
    public class Utility
    {
        public static string GetCommaSepStrFromList(List<int> listInt)
        {
            return string.Join(",", listInt.Select(n => n.ToString()).ToArray());
        }

        public static int GetIntByCommaSepStr(string str)
        {
            return Convert.ToInt32(str.Split(',').Select(int.Parse).ToList()[0]);
        }

        public static List<int> GetIntList(string str)
        {
            return str.Split(',').Select(int.Parse).ToList();
        }

        public static string GetValueAgainstIdStr(string idStr, string name)
        {
            if (idStr != null)
            {
                int id = CMS.Utility.GetIntByCommaSepStr(idStr);
                if (id != 0)
                {
                    return name;
                }
            }

            return null;
        }

       

        public static List<int?> GetNullableIntList(string str)
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
}
