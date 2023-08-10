using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Users")]
    public class DbUsers
    {
        [Key]
        [Column("Id")]
        public int User_Id { get; set; }

        [NotMapped]
        public int Id { get; set; }

        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public int? Campaign_Id { get; set; }

        public string Province_Id { get; set; }

        public string District_Id { get; set; }

        public string Division_Id { get; set; }

        public string Tehsil_Id { get; set; }

        public string UnionCouncil_Id { get; set; }

        public string Ward_Id { get; set; }

        public Config.Roles Role_Id { get; set; }

        public Config.SubRoles? SubRole_Id { get; set; }

        public Config.Hierarchy? Hierarchy_Id { get; set; }

        public int? User_Hierarchy_Id { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Created_By { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Updated_Date { get; set; }

        public DateTime? Password_Updated { get; set; }

        public bool IsMultipleLoginsAllowed { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastOpenDate { get; set; }

        public DateTime? SignOutDate { get; set; }

        public bool IsLoggedIn { get; set; }

        //[StringLength(50)]
        public string Name { get; set; }

        public string Cnic { get; set; }

        //[StringLength(20)]
        public string Phone { get; set; }

        //[StringLength(50)]
        public string Email { get; set; }
        public string Imei_No { get; set; }
        public string Address { get; set; }
        public string Campaigns { get; set; }
        public string Categories { get; set; }
        //public virtual List<DbPermissionsAssignment> ListUserPermissions {get; set;}

        public int? UserCategoryId1 { get; set; }

        public int? UserCategoryId2 { get; set; }

        public string Designation { get; set; }

        public string Designation_abbr { get; set; }
        [Column("Verification_code")]
        public string Verification_code { get; set; }
        [Column("Verification_Code_Sent_Date")]
        public DateTime? VerificationCodeSentDate { get; set; }
        [Column("Password_Changed_Date")]
        public DateTime? PasswordChangedDate { get; set; }


        public virtual List<DbUserWiseDevices> ListDbUserWiseDevices { get; set; }
        public virtual List<DbUserCategory> ListDbUserCategory { get; set; }

        [NotMapped]
        public static Dictionary<int, string> DictHierarchyWiseColumnName = new Dictionary<int, string>
        {
            {1,"Province_Id"},
            {2,"Division_Id"},
            {3,"District_Id"},
            {4,"Tehsil_Id"},
            {5,"UnionCouncil_Id"},
            {6,"Ward_Id"},
        };
    }
}
