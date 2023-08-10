using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
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





        public static DbPermissions GetPermissionsByPermissionAndType(int permissionId, int permissionType)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbPermissions dbPermission = db.DbPermissions.AsNoTracking().Where(m => m.Permission_Id == permissionId && m.Permission_Type == permissionType).ToList().FirstOrDefault();
                    return dbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }


}
