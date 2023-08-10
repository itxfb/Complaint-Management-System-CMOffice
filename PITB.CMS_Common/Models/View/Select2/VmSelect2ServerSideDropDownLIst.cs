using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Select2
{
    public class VmSelect2ServerSideDropDownLIst
    {
        public List<Select2ListItem> ListItems { get; set; }

        public int TotalCount { get; set; }
    }

    public class Select2ListItem
    {
        public string id { get; set; }

        public string text { get; set; }
    }
}