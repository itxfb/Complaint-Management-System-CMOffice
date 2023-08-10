using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS.Models.Custom.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Handler.DataTableJquery
{
    public class DataTableHandler
    {
        public static DataTableParamsModel ConvertaoDataToModel(string aoDataStr)
        {
            //JObject jObject = JObject.Parse(aoDataStr);
            List<Dictionary<string, object>> values = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(aoDataStr);
            return DataTableModelFromDictionary(values);
        }

        private static DataTableParamsModel DataTableModelFromDictionary (List<Dictionary<string, object>> listDataDict)
        {
            //int count = 0;
            string str;
            DataTableParamsModel dtModel = new DataTableParamsModel();
            List<OrderParam> listOrderParam = null;
            List<Order> listOrder = null;
            foreach(Dictionary<string,object> dict in listDataDict)
            {
                if (dict["name"].ToString() == "draw")
                {
                    dtModel.Draw = Convert.ToInt32(dict["value"]);
                }
                if (dict["name"].ToString() == "start")
                {
                    dtModel.Start = Convert.ToInt32(dict["value"]);
                }
                if (dict["name"].ToString() == "length")
                {
                    dtModel.Length = Convert.ToInt32(dict["value"]);
                }
                if (dict["name"].ToString() == "search")
                {
                    str = dict["value"].ToString();
                    Dictionary<string, string> dictSearchVals = JsonConvert.DeserializeObject<Dictionary<string, string>>(dict["value"].ToString());
                    dtModel.Search = dictSearchVals["value"];
                    dtModel.SearchRegX = Convert.ToBoolean(dictSearchVals["regex"]);
                    //dtModel.Length = Convert.ToInt32(keyVal.Value);
                }
                if (dict["name"].ToString() == "columns")
                {
                    str = dict["value"].ToString();
                    dtModel.ListColumns = JsonConvert.DeserializeObject<List<Columns>>(dict["value"].ToString());
                }

                if (dict["name"].ToString() == "order")
                {
                    str = dict["value"].ToString();
                    listOrderParam = JsonConvert.DeserializeObject<List<OrderParam>>(dict["value"].ToString());
                }
            }

            if(listOrderParam!=null)
            {
                listOrder = new List<Order>();
                foreach (OrderParam o in listOrderParam)
                {
                    listOrder.Add(new Order(o, dtModel.ListColumns));
                }
                dtModel.ListOrder = listOrder;
            }

            

            return dtModel;
        }

        public static void ApplyColumnOrderPrefix(DataTableParamsModel dtParam, List<string> prefixArray = null, Dictionary<string, string> dictOrderQuery = null)
        {
            List<Columns> listColumns = dtParam.ListColumns;
            int colIndex = 0;
            string query = "";
            if (prefixArray != null)
            {
                for (int i = 0; i < dtParam.ListOrder.Count; i++)
                {
                    if (dictOrderQuery != null && dictOrderQuery.TryGetValue(dtParam.ListOrder[i].columnName, out query))
                    {
                        dtParam.ListOrder[i].columnName = query;
                    }
                    else
                    {
                        colIndex = listColumns.FindIndex(n => n.data == dtParam.ListOrder[i].columnName);
                        if (colIndex < prefixArray.Count)
                        {
                            dtParam.ListOrder[i].columnName = prefixArray[colIndex] + "." + dtParam.ListOrder[i].columnName;
                        }
                        
                    }
                }
            }
        }

        public static void ApplyColumnFilters(DataTableParamsModel dtParam, List<string> columnsToIgnore, List<string> prefixArray, Dictionary<string, string> filterQueryDict = null)
        {
            string fiterReplaceVal = "_Value_";
            dtParam.WhereOfMultiSearch = " ";
            bool isGlobalSearch = !string.IsNullOrEmpty(dtParam.Search);

            if (dtParam.ListColumns != null)
            {
                dtParam.filterDict = new Dictionary<string, string>();
                int i = 0;
                foreach (Columns c in dtParam.ListColumns)
                {
                    if (isGlobalSearch && !columnsToIgnore.Contains(c.data))
                    {
                        if (prefixArray.Count != 0)
                        {
                            if (!dtParam.filterDict.ContainsKey(prefixArray[i] + "." + c.data))
                            {
                                dtParam.filterDict.Add(prefixArray[i] + "." + c.data, dtParam.Search);
                            }
                        }
                        else
                        {
                            dtParam.filterDict.Add(c.data, dtParam.Search);
                        }
                        i++;
                    }
                    else
                    {
                        if (c.search != null && !string.IsNullOrEmpty(c.search.value) && !columnsToIgnore.Contains(c.data))
                        {
                            if (prefixArray.Count != 0 && i< prefixArray.Count)
                            {
                                dtParam.filterDict.Add(prefixArray[i] + "." + c.data, c.search.value);
                            }
                            else
                            {
                                dtParam.filterDict.Add(c.data, c.search.value);
                            }
                        }
                        i++;
                    }
                }
                int count = 0;
                string query = "";
                if (dtParam.filterDict.Count != 0)
                {
                    dtParam.WhereOfMultiSearch = " and (";
                }
                foreach (KeyValuePair<string, string> keyVal in dtParam.filterDict)
                {
                    if (filterQueryDict != null && filterQueryDict.TryGetValue(keyVal.Key, out query)) // column query override
                    {
                        dtParam.WhereOfMultiSearch = dtParam.WhereOfMultiSearch + query;
                        dtParam.WhereOfMultiSearch = dtParam.WhereOfMultiSearch.Replace(fiterReplaceVal, keyVal.Value);
                    }
                    else
                    {
                        dtParam.WhereOfMultiSearch = dtParam.WhereOfMultiSearch + keyVal.Key + " Like '%" + keyVal.Value + "%' ";
                    }
                    if (count < dtParam.filterDict.Count - 1)
                    {
                        if (isGlobalSearch)
                        {
                            dtParam.WhereOfMultiSearch = dtParam.WhereOfMultiSearch + " or ";
                        }
                        else
                        {
                            dtParam.WhereOfMultiSearch = dtParam.WhereOfMultiSearch + " and ";
                        }
                    }
                    else
                    {
                        dtParam.WhereOfMultiSearch = dtParam.WhereOfMultiSearch + ")";
                    }
                    count++;
                }


            }
        }

    }
}