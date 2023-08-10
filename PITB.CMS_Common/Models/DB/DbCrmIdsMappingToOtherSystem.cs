using System.Data.SqlClient;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Z.BulkOperations;


namespace PITB.CMS_Common.Models
{

    [Table("PITB.CrmIdsMappingToOtherSystems")]
    public partial class DbCrmIdsMappingToOtherSystem
    {
        public int Id { get; set; }

        public int? Crm_Module_Id { get; set; }

        public string Crm_Module_Tag { get; set; }

        public int? Crm_Module_Cat1 { get; set; }

        public int? Crm_Module_Cat2 { get; set; }

        public int? Crm_Module_Cat3 { get; set; }

        public int? Crm_Id { get; set; }

        public int? OTS_System_Id { get; set; }

        public int? OTS_Module_Id { get; set; }

        public int? OTS_Id { get; set; }

        public bool? Is_Active { get; set; }

        public DateTime? Created_Date { get; set; }

        public DateTime? Updated_Date { get; set; }
    }
}
