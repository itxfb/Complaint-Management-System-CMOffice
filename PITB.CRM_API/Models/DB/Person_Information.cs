namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Person_Information")]
    public partial class Person_Information
    {
        [Key]
        public int Person_id { get; set; }

        [StringLength(100)]
        public string Person_Name { get; set; }

        [StringLength(100)]
        public string Person_Father_Name { get; set; }

        [StringLength(16)]
        public string Cnic_No { get; set; }

        public byte? Gender { get; set; }

        [StringLength(50)]
        public string Mobile_No { get; set; }

        [StringLength(15)]
        public string Secondary_Mobile_No { get; set; }

        [StringLength(10)]
        public string LandLine_No { get; set; }

        [StringLength(250)]
        public string Person_Address { get; set; }

        [StringLength(30)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Nearest_Place { get; set; }

        public int? Province_Id { get; set; }

        public int? Division_Id { get; set; }

        public int? District_Id { get; set; }

        public int? Tehsil_Id { get; set; }

        public int? Town_Id { get; set; }

        public int? Uc_Id { get; set; }

        [StringLength(50)]
        public string Ward_Id { get; set; }

        public DateTime? Created_Date { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Created_By { get; set; }

        public int? Updated_By { get; set; }

        [StringLength(50)]
        public string Imei_No { get; set; }

        public int? RegisterType { get; set; }

        public bool? IsEditable { get; set; }

        [StringLength(100)]
        public string Category_of_Seats { get; set; }

        [StringLength(50)]
        public string Party_Affiliation { get; set; }
    }
}
