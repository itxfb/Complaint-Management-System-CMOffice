using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PITB.CMS_DB.Models
{
    [Table("PITB.Complaints_Type")]
    public partial class DbComplaintType
    {
        [Key]
        [Column("Id")]
        public int Complaint_Category { get; set; }

        public int? Campaign_Id { get; set; }

        //[StringLength(100)]
        public string Name { get; set; }

        //[NotMapped]
        //public string Name { get; set; }


        public double? RetainingHours { get; set; }

        public int? DepartmentId { get; set; }

        public int? Group_Id { get; set; }

        public bool Is_Active { get; set; }
    }
}
