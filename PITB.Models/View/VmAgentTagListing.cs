using System;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace PITB.CMS_Models.View
{
    public class VmAgentTagListing
    {
        public string ID { get; set; }
        public string Campaign_ID { get; set; }

        public string Start_Time { get; set; }
        public string End_Time { get; set; }
        public string Duration { get; set; }
        public string Caller_Name { get; set; }
        public string DepartmentCategoryName { get; set; }

        public string DepartmentSubCategoryName { get; set; }

        public string Recording_ID { get; set; }

        public string Agent_ID { get; set; }

        public string Agent_Name { get; set; }

        public int Total_Rows { get; set; }

    }
}