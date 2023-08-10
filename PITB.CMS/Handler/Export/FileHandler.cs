using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using PITB.CMS.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web.UI;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using PITB.CMS.Models.Custom;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Helper.Database;
using System.Web.Http;
using System.Drawing;
using OfficeOpenXml.Drawing;

namespace PITB.CMS.Handler.Export
{
    public class FileHandler
    {
        public static FileContentResult GetFile(Config.FileType type, DataTable data, string headingText, string downloadName = null)
        {
            List<byte> arrayList = new List<byte>();
            string contentType = string.Empty;
            switch (type)
            {
                case Config.FileType.Excel:
                    arrayList.AddRange(ExportToExcel(data, headingText).GetAsByteArray());
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
            }

            FileContentResult fileContentResult = new FileContentResult(arrayList.ToArray(), contentType);

            if (downloadName != null)
            {
                fileContentResult.FileDownloadName = downloadName;
            }
            return new FileContentResult(arrayList.ToArray(), contentType);
        }


        public static HttpResponseBase Generate(HttpResponseBase response, Config.FileType type, DataTable data, string headingText, string downloadName = null)
        {


            response.ClearContent();
            response.Buffer = true;


            response.ClearContent();
            response.Buffer = true;
            response.AddHeader("content-disposition", "attachment; filename=" + downloadName);
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //response.Output.Write(sw.ToString());
            response.BinaryWrite(ExportToExcel(data, headingText).GetAsByteArray());
            response.Flush();
            response.End();

            return response;


            /*
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ExportToExcel(data, headingText).GetAsByteArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "downloadName.xlsx"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            return result;
             */
        }


        public static HttpResponseBase Generate(HttpResponseBase response, ExcelPackage data, string downloadName = null)
        {


            response.ClearContent();
            response.Buffer = true;


            response.ClearContent();
            response.Buffer = true;
            response.AddHeader("content-disposition", "attachment; filename=" + downloadName);
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //response.Output.Write(sw.ToString());
            response.BinaryWrite(data.GetAsByteArray());
            response.Flush();
            response.End();

            return response;
        }

        public static HttpResponseBase Generate(HttpResponseBase response, byte[] data, string contentType, string downloadName = null)
        {
            response.ClearContent();
            response.Buffer = true;


            response.ClearContent();
            response.Buffer = true;
            response.AddHeader("content-disposition", "attachment; filename=" + downloadName);
            response.ContentType = contentType;
            //response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //response.Output.Write(sw.ToString());
            response.BinaryWrite(data);
            response.Flush();
            response.End();

            return response;
        }

        public static ExcelPackage ExportToExcel(DataTable dataTableToExport, string sheetName)
        {
            ExcelPackage excelSheet = new ExcelPackage();

            try
            {

                ExcelWorksheet ws = excelSheet.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;

                //Merging cells and create a center heading for out table

                //Font size of columns headings etc
                const float headingFontSize = 13;
                const float subHeadingFontSize = headingFontSize - 1;
                const float cellFontSize = subHeadingFontSize - 1;
                //Number of columns in current data
                int numberOfColumns = dataTableToExport.Columns.Count;

                //
                const int currRow = 2;

                #region Heading
                ws.Cells[1, 1].Value = sheetName; // Heading Name
                ws.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);



                ws.Cells[1, 1, 1, numberOfColumns].Merge = true; //Merge columns start and end range
                ws.Cells[1, 1, 1, numberOfColumns].Style.Font.Bold = true; //Font should be bold
                ws.Cells[1, 1, 1, numberOfColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Aligmnet is center
                var fill = ws.Cells[1, 1].Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);

                #region Main Table Headings of Columns


                for (int i = 0; i < dataTableToExport.Columns.Count; i++)
                {

                    ws.Cells[currRow, i + 1].Value = dataTableToExport.Columns[i].ColumnName;
                    ws.Cells[currRow, i + 1].Style.Font.Bold = true;

                }

                #endregion
                #endregion


                #region Cell Style

                var border = ws.Cells.Style.Border;

                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                #endregion
                #region Data
                bool flag = false;
                for (int i = 0; i < (dataTableToExport.Rows.Count); i++)
                {
                    for (int j = 0; j < dataTableToExport.Columns.Count; j++)
                    {
                        if (dataTableToExport.Columns[j].DataType == typeof(System.Byte[]))
                        {
                            if (dataTableToExport.Rows[i][j] != System.DBNull.Value && dataTableToExport.Rows[i][j].GetType() == typeof(Byte[]))
                            {
                                using (var ms = new MemoryStream((System.Byte[])dataTableToExport.Rows[i][j]))
                                {
                                    ExcelPicture picture = ws.Drawings.AddPicture(string.Format("Image_{0}_{1}", i, j), Image.FromStream(ms));
                                    picture.From.Column = j;
                                    picture.From.Row = i + currRow;
                                    picture.SetSize(100, 100);
                                    picture.From.ColumnOff = 2 * 9525;
                                    picture.From.RowOff = 2 * 9525;
                                    ws.Column(j + 1).Width = (100 - 12 + 5) / 7d + 1;
                                    flag = true;
                                }
                            }
                        }
                        else
                        {
                            ws.Cells[i + currRow + 1, j + 1].Value = dataTableToExport.Rows[i][j].ToString();
                        }
                    }
                }
                for (int i = 0; i < (dataTableToExport.Rows.Count); i++)
                {
                    if (flag)
                    {
                        ws.Row(i + currRow).Height = 100 * 72 / 96d;
                    }
                }
                #endregion

                ws.Cells.Style.Font.Size = cellFontSize;
                ws.Cells[1, 1].Style.Font.Size = headingFontSize;
            }
            catch (Exception ex)
            {
                UtilityExtensions.WriteToFile(ex.Message, "E:\\error.txt");
            }

            return excelSheet;
        }
        /// <summary>
        /// This function gets all the reports in system that is to be shown on Export (index) Report View
        /// </summary>
        /// <returns></returns>
        public static List<ExportReportObject> GetReportsToBeExportedList()
        {
            List<ExportReportObject> expReportList = new List<ExportReportObject>();
            ExportReportObject lObjReport1 = new ExportReportObject(1, "Assignee Data", "Assignee Data report required weekly", "SP_1");
            ExportReportObject lObjReport2 = new ExportReportObject(2, "Zavor-e-Taleem", "Zavor-e-Taleem Complaints Data.", "SP_2");
            expReportList.Add(lObjReport1);
            expReportList.Add(lObjReport2);
            return expReportList;
        }
        public static List<ExportReportObject> GetReportsToBeExportedListForPriviledgedUser()
        {
            List<ExportReportObject> expReportList = new List<ExportReportObject>();
            ExportReportObject lObjReport1 = new ExportReportObject(1, "Assignee Data", "Assignee Data report required weekly", "SP_1");
            ExportReportObject lObjReport2 = new ExportReportObject(2, "Zavor-e-Taleem", "Zavor-e-Taleem Complaints Data.", "SP_2");
            expReportList.Add(lObjReport1);
            expReportList.Add(lObjReport2);
            return expReportList;
        }
        /// <summary>
        /// This function calls the required stored procedure identified by procedureId
        /// </summary>
        /// <param name="procedureID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public static DataTable GetExportReportDataFromDatabase([FromBody] string reportName, [FromBody]string fromDate, [FromBody]string toDate)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            // Assignee Data Stored Procedure Call
            if (String.Equals(reportName, "Assignee Data"))
            {
                paramDict.Add("@From", fromDate.ToDbObj());
                paramDict.Add("@To", toDate.ToDbObj());
                return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Assignee_Data]", paramDict);
            }
            // Zavor-e-Taleem Stored Procedure Call
            else if (String.Equals(reportName, "Zavor-e-Taleem"))
            {
                paramDict.Add("@From", fromDate.ToDbObj());
                paramDict.Add("@To", toDate.ToDbObj());
                paramDict.Add("@ComplaintCategoryId", 324);
                return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Complaint_Against_Params]", paramDict);
            }

            return null;
        }
    }
    /// <summary>
    /// This class stores the report display information objects
    /// </summary>
    public class ExportReportObject
    {
        private int id = -1;
        private string name = null;
        private string description = null;
        private string storedProcedureName = null;
        private Dictionary<string, object> paramDictionary = null;
        public ExportReportObject(int id, string _name, string _description, string _procedureName)
        {
            this.id = id;
            name = _name;
            description = _description;
            storedProcedureName = _procedureName;
            paramDictionary = new Dictionary<string, object>();
        }
        public string Name { get { return name; } set { name = value; } }
        public int ID { get { return id; } set { id = value; } }
        public string Description { get { return description; } set { description = value; } }
        public string StoredProcedure { get { return storedProcedureName; } set { storedProcedureName = value; } }
        public Boolean AddParameter(KeyValuePair<string, object> _param)
        {
            try
            {
                if (!paramDictionary.ContainsKey(_param.Key))
                {
                    paramDictionary.Add(_param.Key, _param.Value);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An element with key = {0} already exists.", _param.Key);
                throw ex;
            }
        }
        public Boolean RemoveParameter(string key)
        {
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    if (paramDictionary.ContainsKey(key))
                    {
                        paramDictionary.Remove(key);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}