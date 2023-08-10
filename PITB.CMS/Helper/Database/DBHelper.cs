using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web.Razor.Generator;
using Amazon.CognitoSync.Model;
using AngleSharp.Dom.Events;


namespace PITB.CMS.Helper.Database
{
    public class DBHelper
    {
        private SqlConnection sqlConnection;

        private DBHelper()
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
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
                if (paramDict!=null)
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



        public static DataTable GetDataTableByQueryString(string sqlCommand, Dictionary<string, object> paramDict)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    //con.ConnectionTimeout = 100000;
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, con) { CommandType = CommandType.Text })
                    {
                        cmd.CommandTimeout = 1000;  
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
                        dic[column.ColumnName] = row[column];
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

        //private dynamic GetDynamicData(SqlDataReader reader)
        //{
        //    var expandoObject = new ExpandoObject() as IDictionary<string, object>;
        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        expandoObject.Add(reader.GetName(i), reader[i]);
        //    }
        //    return expandoObject;
        //}

        public static DataSet GetDataSetByQueryString(string sqlCommand, Dictionary<string, object> paramDict)
        {
            try
            {
                using (SqlConnection con = new DBHelper().sqlConnection)
                {
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, con) { CommandType = CommandType.Text })
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
    }
}