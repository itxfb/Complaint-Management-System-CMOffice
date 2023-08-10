using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Complaints_SubType")]
    public partial class DbComplaintSubType
    {
        [Key]
        [Column("Id")]
        public int Complaint_SubCategory { get; set; }

        //[StringLength(100)]
        public string Name { get; set; }

        [Column("Ideal_Action")]
        public string Ideal_Action { get; set; }
        public int? Complaint_Type_Id { get; set; }

        public double? Retaining_Hours { get; set; }

        public string Color_Code { get; set; }
        public string Url_Urdu { get; set; }

        [NotMapped]
        public string SubCategory_UrduName { get; set; }

        public bool Is_Active { get; set; }

        public string TagId { get; set; }

        #region from API

        [NotMapped]
        public string Picture { get; set; }
        public string Url { get; set; }
        #endregion
    }
}
