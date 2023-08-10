using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.Custom
{
    public class MapData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int dataValue { get; set; }
        public int dataMin { get; set; }
        public int dataMax { get; set; }
    }
}