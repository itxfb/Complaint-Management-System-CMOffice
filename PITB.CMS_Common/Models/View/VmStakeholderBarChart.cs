using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View
{
    public class VmStakeholderBarChart
    {
        public VmStakeholderBarChart()
        {
            yAxisText = new List<string>();
            ListData = new List<VmBarChartData>();
            ColorList = new List<string>();
        }

        public List<string> yAxisText { get; set; }
        public List<VmBarChartData> ListData { get; set; }
        public List<string> ColorList { get; set; }
    }

    public class VmBarChartData
    {
        public VmBarChartData()
        {
            xAxisText = new List<int>();
        }
        public string LegendText { get; set; }

        public List<int> xAxisText { get; set; }
    }
}