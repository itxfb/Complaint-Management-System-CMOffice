using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web.Helpers.Database
{
    public class DBHelper
    {
        private SqlConnection sqlConnection;

        private DBHelper()
        {
            sqlConnection = new SqlConnection(Utility.GetDecryptedString(ConfigurationManager.ConnectionStrings["PITB.CMSPublic"].ConnectionString));
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(Utility.GetDecryptedString(ConfigurationManager.ConnectionStrings["PITB.CMSPublic"].ConnectionString));

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