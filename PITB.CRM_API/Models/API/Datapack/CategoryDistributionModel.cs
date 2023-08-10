using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API.Datapack
{
    public class CategoryDistributionModel
    {
        public string DistrictName { get; set; }
        public List<dataModel> data { get; set; }

    }
    public struct dataModel{
        public int DistrictId { get; set; }
        public int DepartmentId { get; set; }
        public string DistrictName { get; set; }
        public string DepartmentName { get; set; }
        public string MonthName { get; set; }
        public string MonthPercentage { get; set; }
    }
}