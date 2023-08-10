using PITB.CMS_Common.Helper.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Modules.Query.Template
{
    public class QueryParamsTemplateProvider
    {
        public static Dictionary<string,object> GetListingParamsProvider(Dictionary<string,object> dictParam /*dynamic dParam*/)
        {
            // Populate Start and End Date
            if (dictParam.ContainsKey("@fromDate")){
                dictParam["@fromDate"] = dictParam["@fromDate"] + " 00:00:00";
            }
            if (dictParam.ContainsKey("@toDate"))
            {
                dictParam["@toDate"] = dictParam["@toDate"] + " 23:59:59";
            }

            List<string> listKeys = dictParam.Keys.ToList();
            SqlParameter sqlParam = null;
            foreach (var key in listKeys)
            {
                if(key.Contains("table"))
                {
                    sqlParam = DBHelper.GetSqlParamTableAgainstList(key, (List<int>)dictParam[key]);
                    dictParam[key] = sqlParam;
                }
            }
            
            return dictParam;
        }



        //public static string ReplaceStringWithDict(string strInput, Dictionary<string, string> dict)
        //{
        //    List<string> listKeys = dict.Keys.ToList();
        //    SqlParameter sqlParam = null;
        //    foreach (var key in listKeys)
        //    {
        //        if (key.Contains("table"))
        //        {
        //            string strToInject = string.Format(@"SELECT
        //                                        id
        //                                        FROM
        //                        {0}","@"+dict[key]);

        //            strInput = strInput.Replace(key, strToInject );
        //        }
        //    }
        //    return strInput;
        //}
    }
}
