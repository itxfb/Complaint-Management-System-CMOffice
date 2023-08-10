using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace PITB.CMS_Common.Helper.Database
{
    public class DBHelper
    {
        private SqlConnection sqlConnection;

        private DBHelper()
        {
            sqlConnection = new SqlConnection(Config.ConnectionString
                /*Utility.GetDecryptedString(Config.ConnectionStringConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString)*/);
        }

        public static DataSet GetDataSetByStoredProcedure(string spName, Dictionary<string, object> paramDict)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure })
                    {
                        con.Open();
                        if (paramDict != null && paramDict.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> param in paramDict)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            con.Close();
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static string GetSpString(string spName, Dictionary<string, object> paramDict)
        {
            string spTxt = "";
            using (SqlConnection sqlConnection = new DBHelper().sqlConnection)
            {
                //sqlConnection.ConnectionString = yourConnectionStringHere;
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("sys.sp_helptext", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@objname", spName);
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlDataAdapter.Fill(ds);
                DataTable dt = ds.Tables[0];
                for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    for (int j = 0; j <= ds.Tables[0].Columns.Count - 1; j++)
                    {
                        spTxt = spTxt + dt.Rows[i][j].ToString();

                    }
                    //ds.Tables[0].Rows[i].ItemArray[0] + " -- " + ds.Tables[0].Rows[i].ItemArray[1]);
                }
                if (paramDict != null)
                {
                    int start = spTxt.IndexOf("begin", StringComparison.CurrentCultureIgnoreCase);
                    int end = spTxt.LastIndexOf("end", StringComparison.CurrentCultureIgnoreCase);
                    spTxt = spTxt.Substring(start, end - start);
                    foreach (KeyValuePair<string, object> keyVal in paramDict)
                    {
                        // do something with entry.Value or entry.Key
                        if (keyVal.Value == null)
                        {
                            //spTxt.ind
                            spTxt = spTxt.Replace(keyVal.Key, "NULL");
                        }
                        else
                        {
                            spTxt = spTxt.Replace(keyVal.Key, keyVal.Value.ToString());
                        }

                    }
                }

                return spTxt;
            }

            //var spCode = "";

            //using (SqlConnection con = new DBHelper().sqlConnection)
            //{

            //    using (SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure })
            //    {
            //        if (paramDict != null && paramDict.Count > 0)
            //        {
            //            foreach (KeyValuePair<string, object> param in paramDict)
            //            {
            //                cmd.Parameters.AddWithValue(param.Key, param.Value);
            //            }
            //        }
            //        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            //        {
            //            con.Open();
            //            using (SqlDataReader myReader = cmd.ExecuteReader())
            //            {
            //                if (myReader.HasRows)
            //                {
            //                    myReader.Read();
            //                    if (myReader["ROUTINE_DEFINITION"] != null)
            //                    {
            //                        spCode = myReader["ROUTINE_DEFINITION"].ToString();
            //                        //litSPOut.Text = spCode.Replace(Environment.NewLine, "<br />");
            //                    }
            //                }
            //            };
            //            //return ds.Tables[0];
            //            return spCode;
            //        }
            //    }
            //}


            return spTxt;

        }


        public static Tuple<int, int, DataTable> GetDataTableAndSizeByStoredProcedure(string spName, Dictionary<string, object> paramDict)
        {
            int totalRows = 0, totalFilteredRows = 0;
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure })
                    {
                        if (paramDict != null && paramDict.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> param in paramDict)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        var totalRowsCount = new SqlParameter("TotalRowsCount", 0) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(totalRowsCount);
                        var filteredRowsCount = new SqlParameter("FilteredRowsCount", 0) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(filteredRowsCount);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            totalRows = (totalRowsCount.Value == null) ? 0 : Convert.ToInt32(totalRowsCount.Value);
                            totalFilteredRows = (filteredRowsCount.Value == null) ? 0 : Convert.ToInt32(filteredRowsCount.Value);

                            con.Close();
                            return new Tuple<int, int, DataTable>(totalRows, totalFilteredRows, ds.Tables[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public static DataTable GetDataTableByStoredProcedure(string spName, Dictionary<string, object> paramDict)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure })
                    {
                        if (paramDict != null && paramDict.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> param in paramDict)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            con.Close();
                            return ds.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static DataTable GetDataTableByStoredProcedure(string spName)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure })
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            con.Close();
                            return ds.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public static DataTable GetDataTableByQueryString(string sqlCommand, Dictionary<string, object> paramDict, int commandTimeout = 1000)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    //con.ConnectionTimeout = 100000;
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, con) { CommandType = CommandType.Text })
                    {
                        cmd.CommandTimeout = commandTimeout;
                        if (paramDict != null && paramDict.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> param in paramDict)
                            {
                                if (param.Value is SqlParameter)
                                {
                                    cmd.Parameters.Add(param.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                                }
                            }
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            con.Close();
                            if (ds.Tables.Count > 0)
                            {
                                return ds.Tables[0];
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public static List<dynamic> GetDynamicListByQueryString(string sqlCommand, Dictionary<string, object> paramDict)
        {
            try
            {
                DataTable dt = GetDataTableByQueryString(sqlCommand, paramDict);

                var dynamicDt = new List<dynamic>();
                foreach (DataRow row in dt.Rows)
                {
                    dynamic dyn = new ExpandoObject();
                    dynamicDt.Add(dyn);
                    foreach (DataColumn column in dt.Columns)
                    {
                        var dic = (IDictionary<string, object>)dyn;
                        dic[column.ColumnName] = (row[column] is DBNull) ? null : row[column];
                    }
                }
                return dynamicDt;

                //foreach (DataRow row in dt.Rows)
                //{
                //    TextBox1.Text = row["ImagePath"].ToString();
                //}
                //return dt.AsEnumerable().ToList<object>();

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static DataTable GetIdsDataTable<T>(List<T> listInt)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(Int32));
            foreach (object val in listInt)
            {
                dt.Rows.Add(val);
            }
            return dt;
            //return 
        }


        //public static DataTable GetIdsDataTable<T>(List<T> listInt)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Id", typeof(T));
        //    foreach (T val in listInt)
        //    {
        //        dt.Rows.Add(val);
        //    }
        //    return dt;
        //    //return 
        //}

        //public static DataTable GetIdsDataTable(List<string> listStr)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Id", typeof(string));
        //    foreach (string val in listStr)
        //    {
        //        dt.Rows.Add(val);
        //    }
        //    return dt;
        //    //return 
        //}

        #region GetSqlParamTableAgainstList

        public static SqlParameter GetSqlParamTableAgainstList(string paramName, List<int> listInt)
        {
            DataTable tableStatusIds = DBHelper.GetIdsDataTable(listInt);
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = paramName;
            sqlParam.SqlDbType = SqlDbType.Structured;
            sqlParam.TypeName = "pitb.IdsTable";
            sqlParam.Value = tableStatusIds;
            return sqlParam;
        }
        #endregion


        public static string InjectParameter(Dictionary<string,object> dictParam, string query, string paramName, object paramValue)
        {
            paramName += dictParam.Count;
            query = string.Format(query, paramName);
            dictParam.Add(paramName, paramValue);
            return query;
        }


        #region GetDynamicParameterizedQuery

        //public static dynamic GetDynamicParameterizedQuery2(string strQuery)
        //{
        //    dynamic dParam = new ExpandoObject();
        //    dParam.currWord = "";
        //    dParam.currWordDataType = "None";
        //    dParam.dictQueryParam = new Dictionary<string, object>();
        //    dParam.keyWordsSettings = new Dictionary<string, ExpandoObject>();

        //    dynamic settings = new ExpandoObject();
        //    settings.beforeAllowableChar = new List<char>() { ' ' };
        //    settings.afterAllowableChar = new List<char>() { ' ' };
        //    dParam.keyWordsSettings.Add("=", settings);

        //    settings = new ExpandoObject();
        //    settings.beforeAllowableChar = new List<char>() { ' ' };
        //    settings.afterAllowableChar = new List<char>() { ' ' };
        //    dParam.keyWordsSettings.Add("like", settings);


        //    dParam.hasStartQuoteBegun = false;

        //    dParam.appendedStr = "";
        //    dParam.appendedStrStartIndex = -1;
        //    dParam.appendedStrEndIndex = -1;

        //    dParam.canParameterize = false;
        //    dParam.orignalQuery = strQuery;

        //    dParam.queryCurrIndex = 0;
        //    dParam.queryStartIndex = 0;
        //    dParam.queryEndIndex = 0;
        //    _ComputeDynamicParameterizedQuery(dParam);
        //    return dParam;
        //}

        //private static dynamic _ComputeDynamicParameterizedQuery(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
        //    //int orignalQueryIndex = d.orignalQueryIndex;
        //    if (orignalQuery[d.queryCurrIndex] == '\'') //urr
        //    {
        //        d.hasStartQuoteBegun = !d.hasStartQuoteBegun;
        //    }
        //    d.queryCurrIndex++;

        //    if (!d.hasStartQuoteBegun)
        //    {
        //        d.currWord = "";
        //        d.currWord = _GetCurrWord2(d);
        //    }

        //    return d;
        //}

        //private static string _GetCurrWord2(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
        //    while(d.queryCurrIndex < orignalQuery.Length)
        //    {
        //        if (orignalQuery[d.queryCurrIndex] == ' ')
        //        {
        //            d.currWord = "";
        //        }
        //        if (orignalQuery[d.queryCurrIndex] == '=')
        //        {
        //            d.currWord = "=";
                    
                    
        //            return d.currWord;
        //        }
        //        if (d.queryCurrIndex + " like ".Length < orignalQuery.Length &&
        //            orignalQuery.Substring(d.queryCurrIndex, " like".Length) == " like ")
        //        {
        //            d.currWord = "like";
        //            d.queryCurrIndex = d.queryCurrIndex + " like".Length;
        //            return d.currWord;
        //        }
        //        else if (d.queryCurrIndex + " in".Length < orignalQuery.Length &&
        //            orignalQuery.Substring(d.queryCurrIndex, " in".Length) == " in")
        //        {
        //            d.currWord = "in";
        //            d.queryCurrIndex = d.queryCurrIndex + " in".Length;
        //            return d.currWord;
        //        }

        //        else if (d.queryCurrIndex + " between".Length < orignalQuery.Length &&
        //            orignalQuery.Substring(d.queryCurrIndex, " between".Length) == " between")
        //        {
        //            if (orignalQuery[d.queryCurrIndex + 1] == '@' && char.IsLetter(orignalQuery[d.queryCurrIndex]))
        //            {
        //                d.currWord = "";
        //            }
        //            else
        //            {
        //                d.currWord = "between";
        //                d.queryCurrIndex = d.queryCurrIndex + " between".Length;
        //            }
        //            return d.currWord;
        //        }
        //        else
        //        {
        //            d.queryCurrIndex++;
        //        }
        //    }
        //    return d.currWord;
        //}

        //private static void _ComputeEqual(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
        //    Dictionary<string, object> dictQueryParam = d.dictQueryParam;
        //    _SetDataType(d);
        //    if (d.dataTypeAfterKeyword == "int")
        //    {
        //        _SetIntParam(d);
        //    }
        //}

        //private static void _SetIntParam(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
        //    Dictionary<string, object> dictQueryParam = d.dictQueryParam;
        //    int paramsCount = dictQueryParam.Keys.Count;
        //    string intStr = "";
        //    int startIndex= d.queryCurrIndex, endIndex=-1;
        //    while (d.queryCurrIndex < orignalQuery.Length)
        //    {
        //        if (Utility.IsNumericLetter(orignalQuery[d.queryCurrIndex]))
        //        {
        //            intStr += orignalQuery[d.queryCurrIndex];
        //        }
        //        else
        //        {
        //            endIndex = d.queryCurrIndex;
        //            break;
        //        }
        //        d.queryCurrIndex++;
        //    }
        //    dictQueryParam.Add(string.Format("@param_{0}", paramsCount), Int32.Parse(intStr)); //dictParams
        //    orignalQuery = orignalQuery.Remove(startIndex, endIndex - startIndex);
        //    orignalQuery = orignalQuery.Insert(startIndex, string.Format("@param_{0}", paramsCount));
        //}

        //private static void _SetStrParam(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
        //    Dictionary<string, object> dictQueryParam = d.dictQueryParam;
        //    int paramsCount = dictQueryParam.Keys.Count;
        //    string intStr = "";
        //    int startIndex = d.queryCurrIndex, endIndex = -1;
        //    while (d.queryCurrIndex < orignalQuery.Length)
        //    {
        //        if (Utility.IsNumericLetter(orignalQuery[d.queryCurrIndex]))
        //        {
        //            intStr += orignalQuery[d.queryCurrIndex];
        //        }
        //        else
        //        {
        //            endIndex = d.queryCurrIndex;
        //            break;
        //        }
        //        d.queryCurrIndex++;
        //    }
        //    dictQueryParam.Add(string.Format("@param_{0}", paramsCount), Int32.Parse(intStr)); //dictParams
        //    orignalQuery = orignalQuery.Remove(startIndex, endIndex - startIndex);
        //    orignalQuery = orignalQuery.Insert(startIndex, string.Format("@param_{0}", paramsCount));
        //}

        //private static void _SetDataType(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
        //    while (d.queryCurrIndex < orignalQuery.Length)
        //    {
        //        if (orignalQuery[d.queryCurrIndex] == '\'')
        //        {
        //            d.dataTypeAfterKeyword = "string";
        //            break;
        //        }
        //        else if (Utility.IsNumericLetter(orignalQuery[d.queryCurrIndex]))
        //        {
        //            d.dataTypeAfterKeyword = "int";
        //            break;
        //        }
        //        else if (orignalQuery[d.queryCurrIndex] == '@')
        //        {
        //            d.dataTypeAfterKeyword = "param";
        //            break;
        //        }
        //        else if (Utility.IsEnglishLetter(orignalQuery[d.queryCurrIndex]))
        //        {
        //            d.dataTypeAfterKeyword = "query";
        //            break;
        //        }
        //        d.queryCurrIndex++;
        //    }
        //}




        //private static string _GetCurrWord(dynamic d)
        //{
        //    string orignalQuery = d.orignalQuery;
            
        //    if(orignalQuery[d.queryCurrIndex] == ' ')
        //    {
        //        d.currWord = "";
        //        //d.queryCurrIndex++;
        //        //return _GetCurrWord(d);
        //    }
        //    if (orignalQuery[d.queryCurrIndex] == '=')
        //    {
        //        d.currWord = "=";
        //        return d.currWord;
        //    }
        //    if (d.queryCurrIndex + " like ".Length < orignalQuery.Length && 
        //        orignalQuery.Substring(d.queryCurrIndex, " like".Length)==" like ")
        //    {
        //        d.currWord = "like";
        //        d.queryCurrIndex = d.queryCurrIndex + " like ".Length;
        //        return d.currWord;
        //    }
        //    else if (d.queryCurrIndex + " in".Length < orignalQuery.Length &&
        //        orignalQuery.Substring(d.queryCurrIndex, " in".Length) == " in")
        //    {
        //        d.currWord = "in";
        //        d.queryCurrIndex = d.queryCurrIndex + " in".Length;
        //        return d.currWord;
        //    }

        //    else if (d.queryCurrIndex + " between".Length < orignalQuery.Length &&
        //        orignalQuery.Substring(d.queryCurrIndex, " between".Length) == " between")
        //    {
        //        if(orignalQuery[d.queryCurrIndex+1]=='@' && char.IsLetter(orignalQuery[d.queryCurrIndex]))
        //        {
        //            d.currWord = "";
        //        }
        //        else
        //        {
        //            d.currWord = "between";
        //            d.queryCurrIndex = d.queryCurrIndex + " between".Length;
        //        }
        //        return d.currWord;
        //    }

        //    if (d.queryCurrIndex != orignalQuery.Length - 1)
        //    {
        //        d.queryCurrIndex++;
        //        return _GetCurrWord(d);
        //    }
        //    else
        //    {
        //        d.currWord = d.orignalQuery;
        //        //return d.currWord;
        //    }
           
          
        


            
        //    return d.currWord;
        //}


        //public static dynamic GetDynamicParameterizedQuery(string strQuery)
        //{
        //    dynamic dToRet = new ExpandoObject();
        //    //int paramsCount = 1;
        //    Dictionary<string, object> dictParams = new Dictionary<string, object>();
        //    string finalQuery = "";
        //    List<string> listStr = Utility.GetWordsList(strQuery);
        //    string strTemp = null;
        //    for(int i=0; i<listStr.Count; i++)
        //    {
        //        strTemp = listStr[i].ToLower();

        //        if (strTemp == "like")
        //        {
        //            _DoOperationIfLike(listStr, i+1, dictParams);
        //        }
        //        if (strTemp == "between")
        //        {
        //            _DoOperationIfNumeric(listStr, i+1, dictParams);
        //            _DoOperationIfNumeric(listStr, i+3, dictParams);

        //            _DoOperationIfString(listStr, i+1, dictParams);
        //            _DoOperationIfString(listStr, i+3, dictParams);
        //        }
        //        if (strTemp == "in")
        //        {
        //            _DoOperationIfIn(listStr, i+1, dictParams);
        //        }
        //        if (strTemp == "=")
        //        {
        //            // if next characted is numeric or start of single quote
        //            _DoOperationIfNumeric(listStr, i+1, dictParams);

        //            // if is quotes 
        //            _DoOperationIfString(listStr, i+1, dictParams);
        //        }
        //    }
        //    finalQuery = string.Join(" ", listStr.Select(x=>x));
        //    dToRet.dictParams = dictParams;
        //    dToRet.finalQuery = finalQuery;
        //    return dToRet;
        //}



        //private static void _DoOperationIfNumeric(List<string> listStr, int i, Dictionary<string,object> dictParams)
        //{
        //    if (Utility.IsNumericLetter(listStr[i][0]))
        //    {
        //        int paramsCount = dictParams.Count;
        //        dictParams.Add(string.Format("@param_{0}", paramsCount), Convert.ToInt32(listStr[i])); //dictParams
        //        listStr[i] = string.Format("@param_{0}", paramsCount);
        //    }
        //}

        //private static void _DoOperationIfString(List<string> listStr, int i, Dictionary<string, object> dictParams)
        //{
        //    if (listStr[i][0] == '\'')
        //    {
        //        int paramsCount = dictParams.Count;
        //        listStr[i] = listStr[i].Remove(0, 1);
        //        listStr[i] = listStr[i].Remove(listStr[i].Length - 1, 1);
        //        dictParams.Add(string.Format("@param_{0}", paramsCount), listStr[i]); //dictParams
        //        listStr[i] = string.Format("@param_{0}", paramsCount);
        //    }
        //}
        //#region DoOperationIfIn
        //private static void _DoOperationIfIn(List<string> listStr, int i, Dictionary<string, object> dictParams)
        //{
        //    int paramsCount = dictParams.Count;
        //    dynamic dParam = new ExpandoObject();
        //    dParam.appendedStr = "";
        //    dParam.canParameterize = false;
        //    dParam.listStr = listStr;

        //    //List<string> listStateString = new List<string>();
        //    dParam.listStateString = new List<string>();
        //    dParam.hasStartQuoteBegun = false;
        //    dParam.hasStartBracketBegun = false;

        //    dParam.isStringContinuing = false;
        //    dParam.isIntegerContinuing = false;
        //    dParam.isBracketContinuing = false;
        //    dParam.isParameterContinuing = false;
        //    dParam.isQueryContinuing = false;

        //    dParam.i = i;
        //    dParam.charIndex = 0;
        //    dParam.stringToSplit = "";
        //    dParam.dataTypeBetweenIn = "none";// none=-1, int=1, string=2, @param=3, innerQuery=4

        //    dynamic dReturned = _ComputeInStatement(dParam);
        //    if (dReturned.canParameterize)
        //    {
        //        string paramsKey = string.Format("@tableParam_{0}", paramsCount);
        //        SqlParameter sqlParam = DBHelper.GetSqlParamTableAgainstList(paramsKey, dReturned.list);
        //        dictParams.Add(paramsKey, sqlParam); //dictParams
        //        listStr[i] = string.Format(paramsKey, paramsCount);
        //    }
        //}

        //private static dynamic _ComputeInStatement(dynamic dToRet)
        //{

        //    if (dToRet.listStr[dToRet.i][dToRet.charIndex] != '(')
        //    {
        //        dToRet.hasStartBracketBegun = true;
        //        _ComputeInStatement(dToRet);
        //    }
        //    if(dToRet.hasStartBracketBegun)
        //    {
        //        if (dToRet.listStr[dToRet.i][dToRet.charIndex] != '\'') // split if is string
        //        {
        //            dToRet.canParameterize = true;
        //            dToRet.appendedStr = "select  id from ";
        //            //dToRet.listStr[dToRet.i] = ((string)dToRet.lisouttStr[dToRet.i]).Replace(" ", "");
        //            dToRet.listStr[dToRet.i] = ((string)dToRet.listStr[dToRet.i]).Replace("\'", "");
        //            dToRet.listStr[dToRet.i] = ((string)dToRet.listStr[dToRet.i]).Replace("(", "");
        //            dToRet.listStr[dToRet.i] = ((string)dToRet.listStr[dToRet.i]).Replace(")", "");
        //            List<string> list = ((string)dToRet.listStr[dToRet.i]).Split(',').ToList().Select(n=>n.Trim()).ToList();
        //            dToRet.list = list;
        //            dToRet.dataType = "string";
        //            return dToRet;
        //            //ComputeInStatement(dToRet);
        //        }
        //        else if (Utility.IsNumericLetter(dToRet.listStr[dToRet.i][dToRet.charIndex])) // split if numberic
        //        {
        //            dToRet.canParameterize = true;
        //            dToRet.appendedStr = "select  id from ";
        //            //dToRet.listStr[dToRet.i] = ((string)dToRet.listStr[dToRet.i]).Replace(" ", "");
        //            dToRet.listStr[dToRet.i] = ((string)dToRet.listStr[dToRet.i]).Replace("(", "");
        //            dToRet.listStr[dToRet.i] = ((string)dToRet.listStr[dToRet.i]).Replace(")", "");
        //            List<int> list = ((string)dToRet.listStr[dToRet.i]).Split(',').ToList().Select(n=>int.Parse(n.Trim())).ToList();
        //            dToRet.list = list;
        //            dToRet.dataType = "int";
        //            return dToRet;
        //        }
        //        else if (dToRet.listStr[dToRet.i][dToRet.charIndex] != '@')
        //        {
        //            dToRet.canParameterize = false;
        //            return dToRet;
        //        }
        //        else if (Utility.IsEnglishLetter(dToRet.listStr[dToRet.i][dToRet.charIndex]))
        //        {
        //            dToRet.canParameterize = false;
        //            return dToRet;
        //        }
        //    }
        //    return dToRet;            
        //}
        //#endregion

        //#region DoOperationIfLike
        //private static void _DoOperationIfLike(List<string> listStr, int i, Dictionary<string, object> dictParams)
        //{
        //    int paramsCount = dictParams.Count;
        //    dynamic dParam = new ExpandoObject();
        //    dParam.appendedStr = "";
        //    dParam.canParameterize = false;
        //    dParam.listStr = listStr;
        //    dParam.hasStartQuoteBegun = false;
        //    dParam.i = i;
        //    dParam.charIndex = 0;
        //    dynamic dReturned = _ComputeLikeStatement(dParam);
        //    // ComputeLikeStatement(listStr, false, i, 0, "")
        //    if (dReturned.canParameterize)
        //    {
        //        dictParams.Add(string.Format("@param_{0}", paramsCount), dParam.appendedStr); //dictParams
        //        listStr[i] = string.Format("@param_{0}", paramsCount);
        //    }
        //}

        //private static dynamic _ComputeLikeStatement(/*List<string> listStr, bool hasStartQuoteBegun, int i, int charIndex,*/ dynamic dToRet)
        //{
        //    if (dToRet.charIndex == dToRet.listStr[dToRet.i].Length)
        //    {
        //        return dToRet;
        //    }
            
        //    if(!dToRet.hasStartQuoteBegun)
        //    {
        //        if(Utility.IsEnglishLetter(dToRet.listStr[dToRet.i][0]))
        //        {
        //            return dToRet;
        //        }
        //    }

        //    // for @parameter check
        //    if(!dToRet.hasStartQuoteBegun)
        //    {
        //        if(dToRet.listStr[dToRet.i][dToRet.charIndex] == '@')
        //        {
        //            dToRet.canParameterize = false;
        //            return dToRet;
        //        }
        //    }
        //    if(dToRet.listStr[dToRet.i][dToRet.charIndex]!='\'') // if single quote doesnt exist
        //    {
                
        //        if(dToRet.hasStartQuoteBegun)
        //        {
        //            dToRet.appendedStr += dToRet.listStr[dToRet.i][dToRet.charIndex];
        //            dToRet.charIndex = dToRet.charIndex + 1;
        //            dToRet.canParameterize = true;
        //            return _ComputeLikeStatement(/*dToRet.listStr, dToRet.hasStartQuoteBegun, dToRet.i, dToRet.charIndex +1,*/ dToRet);
        //        }
        //    }
        //    else if(dToRet.listStr[dToRet.i][dToRet.charIndex] == '\'')
        //    {
        //        dToRet.hasStartQuoteBegun = !dToRet.hasStartQuoteBegun;
        //        dToRet.charIndex = dToRet.charIndex + 1;
        //        return _ComputeLikeStatement(/*dToRet.listStr, dToRet.hasStartQuoteBegun, dToRet.i, dToRet.charIndex +1,*/ dToRet);
        //    }
        //    return dToRet;
        //}
        //#endregion
        #endregion


        public static dynamic GetDynamicList2D(string sqlCommand, Dictionary<string, object> paramDict, int timeout = -1)
        {
            dynamic dToRet = new List<List<dynamic>>();
            DataSet ds = GetDataSetByQueryString(sqlCommand, paramDict, timeout = -1);
            foreach(DataTable dt in ds.Tables)
            {
                List<dynamic> listD = new List<dynamic>();
                foreach (DataRow row in dt.Rows)
                {
                    dynamic dyn = new ExpandoObject();
                    var dic = (IDictionary<string, object>)dyn;
                    foreach (DataColumn column in dt.Columns)
                    {
                        dic[column.ColumnName] = (row[column] is DBNull) ? null : row[column];
                    }
                    listD.Add(dyn);
                }
                dToRet.Add(listD);
                //return dynamicDt;
            }
            return dToRet;
        }

        public static DataSet GetDataSetByQueryString(string sqlCommand, Dictionary<string, object> paramDict, int timeout = -1)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, con) { CommandType = CommandType.Text })
                    {
                        if (timeout != -1)
                        {
                            cmd.CommandTimeout = timeout;
                        }
                        if (paramDict != null && paramDict.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> param in paramDict)
                            {
                                if(param.Value.GetType()==typeof(SqlParameter))
                                {
                                    cmd.Parameters.Add(param.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                                }
                                
                            }
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static void CrudOperation(string spName, Dictionary<string, object> paramDict)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure })
                    {

                        if (paramDict != null && paramDict.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> param in paramDict)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #region from Public web
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(Config.ConnectionString/*Utility.GetDecryptedString(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString)*/);

        }
        #endregion
    }
}