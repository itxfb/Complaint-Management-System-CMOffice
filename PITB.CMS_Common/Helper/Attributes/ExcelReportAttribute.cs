using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = true)]
    public class ExcelReportAttribute:Attribute
    {
        public string ColumnName { get; set; }
        public string ReportName { get; set; }
        public ExcelReportAttribute() {
            ColumnName = null;
            ReportName = null;
        }
        public ExcelReportAttribute(string colName,string reportName)
        {
            ColumnName = colName;
            ReportName = reportName;
        }
    }
}