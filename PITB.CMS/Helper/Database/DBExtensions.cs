using System.ComponentModel;
using System.Dynamic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using PITB.CMS.Helper.Attributes;

namespace PITB.CMS.Helper.Database
{
    public static class DBExtensions
    {

        

        /// <summary>
        /// Converts datatable to list<T> dynamically
        /// </summary>
        /// <typeparam name="T">Class name</typeparam>
        /// <param name="dataTable">data table to convert</param>
        /// <returns>List<T></returns>
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var dd = typeof(T).GetProperties().ToList();

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {
                        if (!DBNull.Value.Equals(dataRow[dtField.Name]))
                        {
                            if (propertyInfos.PropertyType == typeof (DateTime) ||
                                propertyInfos.PropertyType == typeof (DateTime?))
                            {
                                propertyInfos.SetValue
                                    (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                            }
                            else if (propertyInfos.PropertyType == typeof (int) ||
                                     propertyInfos.PropertyType == typeof (int?))
                            {
                                propertyInfos.SetValue
                                    (classObj, ConvertToInt(dataRow[dtField.Name]), null);

                            }
                            else if (propertyInfos.PropertyType == typeof (long) ||
                                     propertyInfos.PropertyType == typeof (long?))
                            {
                                propertyInfos.SetValue
                                    (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                            }
                            else if (propertyInfos.PropertyType == typeof (decimal) ||
                                     propertyInfos.PropertyType == typeof (decimal?))
                            {
                                propertyInfos.SetValue
                                    (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                            }
                            else if (propertyInfos.PropertyType == typeof (decimal) ||
                                     propertyInfos.PropertyType == typeof (decimal?))
                            {
                                propertyInfos.SetValue
                                    (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                            }
                            else if (propertyInfos.PropertyType == typeof (String) ||
                                     propertyInfos.PropertyType == typeof (string))
                            {
                                if (dataRow[dtField.Name].GetType() == typeof (DateTime))
                                {
                                    propertyInfos.SetValue
                                        (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                                }
                                else
                                {
                                    propertyInfos.SetValue
                                        (classObj, ConvertToString(dataRow[dtField.Name]), null);
                                }
                            }
                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        

        public static List<dynamic> ToDynamicList(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    if (row[column] == DBNull.Value)
                    {
                        dic[column.ColumnName] = null;
                    }
                    else
                    {
                        dic[column.ColumnName] = row[column];
                    }
                    
                }
            }
            return dynamicDt;
        }


        //public static List<dynamic> ToDynamicObjectList(this DataTable dt)
        //{
        //    var dynamicDt = new List<dynamic>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        dynamic dyn = new ExpandoObject() as object;
        //        dynamicDt.Add(dyn);
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            //var dic = (IDictionary<string, object>)dyn;
        //            //dyn. = row[column];
        //        }
        //    }
        //    return dynamicDt;
        //}


        public static object ToDbObj(this object obj) 
        {
            if(obj==null)
            {
                return DBNull.Value;
            }
            return obj;
        }



        private static string ConvertToDateString(object date)
        {
            if (date == null)
                return string.Empty;

            //return (Convert.ToDateTime(date)).ToLongDateString();
            return date.ToString();
        }

        private static string ConvertToString(object value)
        {
            return Convert.ToString(UtilityExtensions.ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(UtilityExtensions.ReturnZeroIfNull(value));
        }

        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(UtilityExtensions.ReturnZeroIfNull(value));
        }

        private static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(UtilityExtensions.ReturnZeroIfNull(value));
        }

        private static DateTime convertToDateTime(object date)
        {
            return Convert.ToDateTime(UtilityExtensions.ReturnDateTimeMinIfNull(date));
        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        public static DataTable ToDataTableForReport<T>(this IList<T> data,string reportName)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            Dictionary<PropertyInfo,string> PropertiesExport = new Dictionary<PropertyInfo,string>();
            if (!string.IsNullOrEmpty(reportName))
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    IEnumerable<Attribute> attributes = properties[i].GetCustomAttributes(typeof(ExcelReportAttribute));
                    if (attributes != null && attributes.Count() > 0)
                    {
                        foreach (var t in attributes)
                        {
                            PropertyInfo reportProp = t.GetType().GetProperty("ReportName");
                            if (reportProp != null)
                            {
                                if (reportProp.GetValue(t, null).ToString() == reportName)
                                {
                                    PropertyInfo columnProp = t.GetType().GetProperty("ColumnName");
                                    if (columnProp != null)
                                    {
                                        PropertiesExport.Add(properties[i], columnProp.GetValue(t, null).ToString());
                                    }
                                }
                            }
                        }
                    }
                } 
            }
            DataTable table = new DataTable();
            for (int i = 0; i < PropertiesExport.Count; i++)
            {
                PropertyInfo prop = PropertiesExport.ElementAt(i).Key;
                if (prop.PropertyType == typeof(System.Byte[]))
                {
                    table.Columns.Add(PropertiesExport.ElementAt(i).Value, typeof(System.Byte[]));
                }
                else
                {
                    table.Columns.Add(PropertiesExport.ElementAt(i).Value, prop.PropertyType);
                }
            }
            object[] values = new object[PropertiesExport.Count];
            for (int j = 0; j < data.Count;j++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = PropertiesExport.ElementAt(i).Key.GetValue(data[j]);
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }
}