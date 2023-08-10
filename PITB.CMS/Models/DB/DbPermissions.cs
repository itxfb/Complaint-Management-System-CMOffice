using System.Collections.Generic;
using System.Web.Razor.Generator;
using System.Linq;
using PITB.CMS.Helper.Database;


namespace PITB.CMS.Models.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System;

    [Table("PITB.Permissions")]
    public class DbPermissions
    {
        [Key]
        [Column("Id")]
        public int Permission_Id { get; set; }

        public int Permission_Type { get; set; }

        [StringLength(200)]
        public string Permissions_Name { get; set; }

        public string Permissions_Value { get; set; }





        public static List<DbPermissions> GetPermissionsByPermissionAndType(int permissionId, int permissionType)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissions> listDbPermission = db.DbPermissions.AsNoTracking().Where(m => m.Permission_Id == permissionId && m.Permission_Type == permissionType).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }


}
