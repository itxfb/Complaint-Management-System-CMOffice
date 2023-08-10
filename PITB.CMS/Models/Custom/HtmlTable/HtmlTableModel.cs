using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using PITB.CMS.Helper.Extensions;
using System.Diagnostics;

namespace PITB.CMS.Models.Custom.HtmlTable
{
    public class HtmlTableModel
    {
        //public List<HeaderModel> ListHeaderModel { get; set; }

        //public int CurrRowPointer { get; set; }

        public string Html { get; set; }

        public Dictionary<string, string> DictSetting { get; set; }

        //public List<dynamic> ListCell { get; set; }

        public List<string> ListColumns { get; set; }

        public List<CellModel> CurrRow { get; set; } 

        public List<CellModel> NewRow 
        {
            get
            {
                CurrRow = new List<CellModel>();
                ListRowModel.Add(CurrRow);
                return CurrRow;
            } 
            //set; 
        }

        //public List<List<CellModel>> ListHeaderModel { get; set; }
        public List<List<CellModel>> ListRowModel { get; set; }




        public HtmlTableModel(Dictionary<string,string> dictSetting=null)
        {
            //ListHeaderModel = new List<HeaderModel>();
            //ListCell = new List<dynamic>();
            ListColumns = new List<string>();
            if (dictSetting == null)
            {
                DictSetting = new Dictionary<string, string>();
                DictSetting.Add("HtmlHead", "<thead>");
                DictSetting.Add("HtmlHeadEnd", "</thead>");

                DictSetting.Add("HtmlBody", "<tbody>");
                DictSetting.Add("HtmlBodyEnd", "</tbody>");

                DictSetting.Add("HtmlFooter", "<tfoot>");
                DictSetting.Add("HtmlFooterEnd", "</tfoot>");

                DictSetting.Add("HtmlRow", "<tr>");
                DictSetting.Add("HtmlColTh", "<th>");
                DictSetting.Add("HtmlColTd", "<td>");
            }
            else
            {
                DictSetting = dictSetting;
            }

            ListRowModel = new List<List<CellModel>>();
        }

        public void /*List<string>*/ SetColumns()
        {
            //List<string> listColumns = new List<string>();
            if (ListRowModel != null)
            {
                foreach (List<CellModel> listCellModel in ListRowModel)
                {
                    this.ListColumns.AddRange(listCellModel.Where(n => n.ColumnId != null).Select(n => n.ColumnId).ToList());
                }
            }
            //return listColumns;
        }

        public CellModel GetCellModel(CellModel templateCell)
        {
            CellModel cellModel = new CellModel()
            {
                InnerHtml = templateCell.InnerHtml,
                ColumnId = templateCell.ColumnId,
                RowId = templateCell.RowId,
                Type = templateCell.Type,
                NextHtml = templateCell.NextHtml,
                PreviousHtml = templateCell.PreviousHtml,
                DictSetting = templateCell.DictSetting,
                DictAttributes = templateCell.DictAttributes
            };
            return cellModel;
        }

        public void BindHtmlCell(List<dynamic> listDynamic/*, CellModel templateCell*/, Dictionary<string,CellModel> dictTempCell /*int type, Dictionary<string,string> dictAttributes, Dictionary<string,string> dictSettings*/)
        {
            dynamic d = null;
            IDictionary<string, object> dDictionary = null;

            List<string> listDictKeys = null;
            CellModel templateCell = null;
            string key = null;
            
            for (int i = 0; i < listDynamic.Count; i++)
            {
                d = listDynamic[i];
                dDictionary = d;
                //foreach (KeyValuePair<string,object> keyVal in dDictionary)
                listDictKeys = dDictionary.Keys.ToList();
                for(int j=0; j < listDictKeys.Count; j++)
                {
                    key = listDictKeys[j];
                    if (dictTempCell.ContainsKey(key))
                    {
                        templateCell = dictTempCell[key];
                    }
                    else
                    {
                        templateCell = dictTempCell["*"];
                    }
                    BindHtmlCell(dDictionary, key, GetCellModel(templateCell));
                }
            }
        }

        public void BindHtmlCell(dynamic d, string key, CellModel cellModel)
        {
            IDictionary<string, object> dDictionary = d;
            if (key != null /*&& !dDictionary.ContainsKey(key + Config.Separator + "CellModel")*/)
            {
                dDictionary[key + Config.Separator + "CellModel"] = cellModel;
                cellModel.InnerHtml = dDictionary[key].ToString();

                if (dDictionary.ContainsKey(key + Config.Separator + "ColumnId"))
                {
                    cellModel.ColumnId = dDictionary[key + Config.Separator + "ColumnId"].ToString();
                }
                if (dDictionary.ContainsKey(key + Config.Separator + "RowId"))
                {
                    cellModel.RowId = dDictionary[key + Config.Separator + "RowId"].ToString();
                }

                if (cellModel.DictSetting == null)
                {
                    cellModel.DictSetting = this.DictSetting;
                }
            }
        }

        public void AddTableBody2(List<dynamic> listRow, List<string> listColumns, int wrapRows, Dictionary<string, string> wrapRowsConfig /*, Dictionary<string, string> dictSetting*/)
        {
            //int i = 0;
            string columnId = null;
            string beforeHtml = null, afterHtml = null;
            CellModel cellModel = new CellModel();
            IDictionary<string, object> dictRow = null;

            if (wrapRows==1) // head
            {
                beforeHtml = (wrapRowsConfig != null) ? wrapRowsConfig["HtmlHead"] : DictSetting["HtmlHead"];
                afterHtml = (wrapRowsConfig != null) ? (wrapRowsConfig.ContainsKey("HtmlHeadEnd") ? wrapRowsConfig["HtmlHeadEnd"] : DictSetting["HtmlHeadEnd"]) : DictSetting["HtmlHeadEnd"];
            }
            else if (wrapRows == 2) // body
            {
                beforeHtml = (wrapRowsConfig != null) ? wrapRowsConfig["HtmlBody"] : DictSetting["HtmlBody"];
                afterHtml = (wrapRowsConfig != null) ? (wrapRowsConfig.ContainsKey("HtmlBodyEnd") ? wrapRowsConfig["HtmlBodyEnd"] : DictSetting["HtmlBodyEnd"]) : DictSetting["HtmlBodyEnd"];
            }
            else if (wrapRows == 3) // footer
            {
                beforeHtml = (wrapRowsConfig != null) ? wrapRowsConfig["HtmlFooter"] : DictSetting["HtmlFooter"];
                afterHtml = (wrapRowsConfig != null) ? (wrapRowsConfig.ContainsKey("HtmlFooterEnd") ? wrapRowsConfig["HtmlFooterEnd"] : DictSetting["HtmlFooterEnd"]) : DictSetting["HtmlFooterEnd"];
            }

            for (int i = 0; i < listRow.Count; i++)
            {
                this.SetNewRow();
                dynamic rowModel = listRow[i];
                dictRow = (IDictionary<string, object>)rowModel;

                for (int j = 0; j < listColumns.Count; j++)
                {
                    //string key = columnId + Config.Separator + "CellModel";
                    
                    columnId = listColumns[j];
                    string key = columnId + Config.Separator + "CellModel";
                    if (dictRow.ContainsKey(key))
                    {
                        cellModel = (CellModel)dictRow[key];

                        if (i == 0 && j == 0) // first cell
                        {
                            cellModel.BeforeRowHtml = beforeHtml;
                        }
                        else if (i == listRow.Count - 1 && j == listColumns.Count - 1) // last cell
                        {
                            cellModel.AfterRowHtml = afterHtml;
                        }

                        this.CurrRow.Add(cellModel);
                    }
                }
            }

            
        }

        public void AddTableBody(List<dynamic> listRow, List<string> listColumns, Dictionary<string,string> dictSetting)
        {
            //if (dictSetting == null)
            //{
            //    dictSetting = //MergeDict();
            //}
            if (listRow!=null)
            {
                int count = 0;
                foreach (dynamic d in listRow)
                {
                    AddRow(d, count, listRow.Count, listColumns, dictSetting);
                    count++;
                }
            }
        }

        private void AddRow(dynamic rowModel, int index, int totalRows, List<string> listColumns, Dictionary<string,string> dictSetting )
        {
            IDictionary<string, object> dictProperties = (IDictionary<string, object>)rowModel;
            this.SetNewRow();
            //int i = 0;
            string columnId = null;
            string beforeHtml = null, afterHtml=null;

           
            for (int i = 0; i < listColumns.Count; i++)
            {
                columnId = listColumns[i];
                if (dictProperties.ContainsKey(columnId)) // if it contains key
                {
                    if (index == 0)
                    {
                        beforeHtml = dictSetting!=null && dictSetting.ContainsKey("HtmlBody") ? dictSetting["HtmlBody"] : DictSetting["HtmlBody"];
                    }
                    else if (index == totalRows-1)
                    {
                        afterHtml =  Utility.GetEndingTag(dictSetting!=null && dictSetting.ContainsKey("HtmlBody") ? dictSetting["HtmlBody"] : DictSetting["HtmlBody"]);
                    }
                    else
                    {
                        beforeHtml = null;
                        afterHtml = null;
                    }

                    this.CurrRow.Add(new HtmlTableModel.CellModel()
                    {
                        InnerHtml = dictProperties[columnId].ToString(),
                        ColumnId = columnId,
                        RowId = i.ToString(),
                        Type = 2,
                        BeforeRowHtml = beforeHtml,
                        AfterRowHtml = afterHtml
                    });
                    if (dictSetting != null)
                    {
                        this.CurrRow[this.CurrRow.Count - 1].DictSetting = dictSetting;
                        //MergeDict(this.CurrRow[this.CurrRow.Count - 1]); 
                    }
                    //dictProperties.Add();
                }
                //foreach (KeyValuePair<string, object> keyVal in dictProperties)
                //{
                //    if (i == 0)
                //    {
                //        this.CurrRow.Add(new HtmlTableModel.CellModel()
                //        {
                //            InnerHtml = keyVal.Key,
                //            ColumnId = "Total" + "_" + "Resolved",
                //            Type = 2,
                //            BeforeRowHtml = Utility.GetEndingTag(DictSetting["HtmlBody"])
                //        });
                //    }

                //    i++;
                //}
            }

            
            //ListCell.Add(d);
        }


        public void ConvertTableToHtml()
        {
            string html = "";
            string colTag = "";
            CellModel cellModel = null;
            //CellModel cellPrev = null;
            //CellModel cellNext = null;
            int c = 0;
            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < ListRowModel.Count /*&& i<500*/; i++) // row
            {
                //html = html + DictSetting["HtmlRow"];
                Debug.WriteLine("Elapsed Time ="+stopWatch.ElapsedMilliseconds);
                for (int j = 0; j < ListRowModel[i].Count; j++) // col
                {
                    cellModel = ListRowModel[i][j];
                    MergeDict(cellModel);

                    //if First cell
                    if (j == 0) 
                    {
                        html = html + cellModel.BeforeRowHtml.NullToEmptyStr() + cellModel.DictSetting["HtmlRow"];
                    }

                    html = html+ cellModel.PreviousHtml.NullToEmptyStr() + ConvertToHtml(cellModel) + cellModel.NextHtml.NullToEmptyStr();

                    //if Last Cell
                    if (j == ListRowModel[i].Count - 1) 
                    {
                        html = html + Utility.GetEndingTag(cellModel.DictSetting["HtmlRow"]) + cellModel.AfterRowHtml.NullToEmptyStr();
                    }
                    c++;
                }
                //html = html + GetEndingTag(DictSetting["HtmlRow"]);
            }

            this.Html = html;
        }

        //private void PopulateHeadFootTags(List<CellModel> ListCells, string html, int type, bool isStarting)
        //{
        //    string tag = "";
        //    int index = -1;
        //    if (ListCells.Count >= 0) 
        //    {
        //        if (isStarting)
        //        {
        //            index = 0;
        //        }
        //        else
        //        {
        //            index = ListCells.Count-1;
        //        }


        //        if (ListCells[index].Type == 1) // head
        //        {
        //            tag = isStarting ? DictSetting["HtmlHead"] : GetEndingTag(DictSetting["HtmlHead"]);
        //        }
        //        else if (ListCells[index].Type == 2) // body
        //        {
        //            tag = isStarting ? DictSetting["HtmlBody"] : GetEndingTag(DictSetting["HtmlBody"]);
        //        }
        //        else if (ListCells[index].Type == 3) // footer
        //        {
        //            tag = isStarting ? DictSetting["HtmlFooter"] : GetEndingTag(DictSetting["HtmlFooter"]);
        //        }

        //        html = html + tag;
        //    }
        //}

        private string ConvertToHtml(CellModel cellModel)
        {
            string html = "";
            string colHtml = "";
            //MergeDict(DictSetting, cellModel.DictSetting);
            //if (cellModel.DictSetting == null)
            //{
            //    cellModel.DictSetting = DictSetting;
            //}


            if (cellModel.Type == 1 || cellModel.Type==3) // if header or footer
            {
                colHtml = cellModel.DictSetting["HtmlColTh"];
            }
            else
            {
                colHtml = cellModel.DictSetting["HtmlColTd"];
            }
            html += colHtml;
            html = AddAttributes(html,cellModel);
            html += cellModel.InnerHtml;
            html += Utility.GetEndingTag(colHtml);
            return html;
        }

        private void MergeDict(CellModel cellModel)
        {
            if (cellModel.DictSetting == null)
            {
                cellModel.DictSetting = DictSetting;
            }
            else
            {
                Dictionary<string, string> dictSetting = DictSetting.ToDictionary(p => p.Key,
                   p => p.Value);

                foreach (KeyValuePair<string, string> keyVal in cellModel.DictSetting)
                {
                    if (dictSetting.ContainsKey(keyVal.Key))
                    {
                        dictSetting[keyVal.Key] = keyVal.Value;
                    }
                    else
                    {
                        dictSetting.Add(keyVal.Key, keyVal.Value);
                    }
                }
                cellModel.DictSetting = dictSetting;
            }

        }

        public void SetNewRow()
        {
            CurrRow = this.NewRow;
            //ListRowModel.Add(CurrRow);
            //return CurrRow;
        }

        //private void MergeDict(Dictionary<string, string> dict, Dictionary<string, string> dictToMerge)
        //{
        //    Dictionary<string, string> finalDict = new Dictionary<string, string>();
        //    if (dictToMerge == null || dictToMerge.Count == 0)
        //    {
        //        return;
        //    }
        //    int count = dict.Keys.Count;
        //    //Listdict.Keys.ToList();
        //    for (int i=0; i<count; i++)
        //    {
        //        KeyValuePair<string, string> keyVal = dict.ElementAt(i);
        //        if (keyVal.Key == dictToMerge[keyVal.Key]) // if key has found replace value
        //        {
        //            finalDict.Add(keyVal.Key, dictToMerge[keyVal.Key]);
        //            //dict[keyVal.Key] = dictToMerge[keyVal.Key];
        //        }
        //        else 
        //        {
        //            dict.Add(keyVal.Key, keyVal.Value);
        //        }
        //    }
        //}

        private string AddAttributes(string htmlStr, CellModel cellModel)
        {
            string attString = " ";
            int insertAt = htmlStr.LastIndexOf('>');
            if (cellModel.DictAttributes != null)
            {
                foreach (KeyValuePair<string, object> keyVal in cellModel.DictAttributes)
                {
                    attString = attString + String.Format("{0}='{1}'", keyVal.Key, keyVal.Value)+" ";
                    
                    //if (keyVal.Value is int)
                    //{
                    //    attString = attString + String.Format("{0}='{1}'", keyVal.Key, keyVal.Value);
                    //    htmlStr += String.Format("{0}='{1}'", keyVal.Key, keyVal.Value);
                    //}
                    //else
                    //{
                    //    htmlStr += String.Format("{0} ='{1}'", keyVal.Key, keyVal.Value);
                    //}

                }
                htmlStr =  htmlStr.Insert(insertAt, attString);
            }
            return htmlStr;
        }

        //private string GetEndingTag(string tag)
        //{
        //    int index = tag.IndexOf("<",StringComparison.InvariantCultureIgnoreCase);
        //    string endingTag = tag.Insert(index+1, "/");
        //    return endingTag;
        //}

        //public List<CellModel> GetNewRow()
        //{
        //    List<CellModel> row = new List<CellModel>();
        //    ListRowModel.Add(row);
        //    return row;
        //}

        //public List<CellModel> GetCurrRowId()
        //{
        //    List<CellModel> row = new List<CellModel>();
        //    ListRowModel.Add(row);
        //    return row;
        //}

        //public void OrderCells()
        //{
        //    List<string> listColumnIds = ListHeaderModel.Select(n => n.ColumnId).ToList();

        //    for (int i = 0; i < ListRowModel.Count; i++)
        //    {
        //        List<CellModel> listCell = ListRowModel[i];
        //        ListRowModel[i] = listCell.OrderBy(d => listColumnIds.IndexOf(d.ColumnId)).ToList();
        //    }
        //}

        public class BaseCellModel
        {
            public string BeforeRowHtml { get; set; }

            public string AfterRowHtml { get; set; }
            public string PreviousHtml { get; set; }

            public string NextHtml { get; set; }

            public string ColumnId { get; set; }

            public string RowId { get; set; }

            public int Type { get; set; } // 1 = header, 2 = cell, 3 = footer


            //public string Attributes { get; set; }

            public Dictionary<string,object> DictAttributes { get; set; }

            private CssProperties _css;
            // Css fields
            public CssProperties Css
            {
                get
                {
                    if (_css == null)
                    {
                        _css = new CssProperties();
                    }
                    return _css;
                }
                set { _css = value; }
            }
            public string ClassName { get; set; }

            public string InnerHtml { get; set; }

            public int RowSpan { get; set; }

            public int ColSpan { get; set; }

            //public Dictionary<string,string> _dictAttributes { get; set; }

            //public Dictionary<string, string> DictAttributes
            //{
            //    get
            //    {
            //        if (_dictAttributes == null)
            //        {
            //            _dictAttributes = new Dictionary<string, string>();
            //        }
            //        return _dictAttributes;
            //    }
            //    set { _dictAttributes = value; }
            //}

        }

        public class CssProperties
        {

            private string _style;
            public string Style
            {
                get { return _style; }
            }

            private string _bgColor;
            public string BgColor
            {
                get { return _bgColor; }
                set
                {
                    AddStyleAttribute("background-color", value);
                    _bgColor = value;
                }
            }

            private string _fgColor;
            public string FgColor
            {
                get { return _fgColor; }
                set
                {
                    AddStyleAttribute("color", value);
                    _fgColor = value;
                }
            }

            private string _width;
            public string Width
            {
                get { return _width; }
                set
                {
                    AddStyleAttribute("width", value);
                    _width = value;
                }
            }

            private string _minWidth;
            public string MinWidth
            {
                get { return _minWidth; }
                set
                {
                    AddStyleAttribute("min-width", value);
                    _minWidth = value;
                }
            }

            private string _height;
            public string Height
            {
                get { return _height; }
                set
                {
                    AddStyleAttribute("height", value);
                    _height = value;
                }
            }

            private void AddStyleAttribute(string key, string value)
            {
                if (_style == null)
                {
                    _style = "";
                }
                _style += key + ":" + value + ";";
            }
        }

        //public class HeaderModel : BaseCellModel
        //{

        //}

        public class CellModel : BaseCellModel
        {
            public Dictionary<string, string> DictSetting { get; set; }
            //public object Data { get; set; }
            public dynamic Data { get; set; }

            public CellModel()
            {
                
            }

            public CellModel(dynamic d /*string innerHtml, string columnId, int typeId, Dictionary<string,string> dictAttr,*/ )
            {
                IDictionary<string, object> dict = d;
                this.InnerHtml = Utility.GetDictValue<string>(d,"InnerHtml");
                this.ColumnId = Utility.GetDictValue<string>(d, "ColumnId");
                this.Type = Utility.GetDictValue<string>(d, "Type");
                this.DictAttributes = Utility.GetDictValue<string>(d, "DictAttributes");
                this.DictSetting =  Utility.GetDictValue<string>(d, "DictSetting");
                //this.DictSetting
            }

        }
    }
}