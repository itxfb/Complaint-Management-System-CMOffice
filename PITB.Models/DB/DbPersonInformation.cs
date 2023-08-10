using PITB.CMS_Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Person_Information")]
    public class DbPersonInformation
    {
        [Key]
        public int Person_id { get; set; }

        [StringLength(100)]
        public string Person_Name { get; set; }

        [StringLength(100)]
        public string Person_Father_Name { get; set; }

        [StringLength(16)]
        public string Cnic_No { get; set; }

        [StringLength(20)]
        public string Account_No { get; set; }

        public Config.Gender? Gender { get; set; }

        [StringLength(15)]
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

        public bool? Is_Cnic_Present { get; set; }

        public virtual DbDistrict District { get; set; }
        public virtual DbProvince Province { get; set; }

        public bool? Is_Active { get; set; }
    }
}
