using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.Custom.DataTable
{
    public class DataTableParamsModel
    {
        public List<Columns> ListColumns { get; set; }
        public List<Order> ListOrder { get; set; }
        public int Draw { get; set; }

        private int _Start = 0;
        public int Start 
        { 
            get
            {
                return _Start + 1;
            }
            set
            {
                _Start = value;
            } 
        }

        public int Length
        {
            get;
            set;
        }

        public int End { 
            get
            {
                return _Start + Length;
            }
        }
        public string Search { get; set; }
        public bool SearchRegX { get; set; }


        public Dictionary<string, string> filterDict;

        public string WhereOfMultiSearch { get; set; }

        public string WhereOfMultiSearchParametrized { get; set; }

        public Dictionary<string, object> dtQueryParams { get; set; }
    }

    public class Columns
    {
        public string data { get; set; }
        public string name { get; set; }

        public bool searchable { get; set; }
        public bool orderable { get; set; }

        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }

        public bool regex { get; set; }
    }

    public class OrderParam
    {
        public int column { get; set; }

       // public string columnName { get; set; }

        public string dir { get; set; }
     }

    public class Order
    {
        public int columnId { get; set; }
        public string columnName { get; set; }
        public string sortingDirectionStr { get; set; }
        public bool isAscending { get; set; }

        public Order(OrderParam o, List<Columns> listColumn)
        {
            this.columnId = o.column;
            this.columnName = listColumn[this.columnId].data;
            this.isAscending = (o.dir == "asc");

            if (this.isAscending) sortingDirectionStr = "";
            else sortingDirectionStr = "DESC";
            
        }

        public Order()
        {

        }
    }
}