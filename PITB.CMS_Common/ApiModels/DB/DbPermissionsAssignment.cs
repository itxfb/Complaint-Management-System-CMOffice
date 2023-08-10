using System.Collections.Generic;
//using System.Web.Razor.Generator;
using System.Linq;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System;

    [Table("PITB.Permissions_Assignment")]
    public class DbPermissionsAssignment
    {
        [Key]
        public int Id { get; set; }

        public int? Type { get; set; }

        public int? Type_Id { get; set; }

        public int? Permission_Id { get; set; }

        public string Permission_Value { get; set; }

        //public virtual DbPermissions DbPermissions { get; set; }




        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, int typeId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && typeId ==m.Type_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissionsByTypeAndPermissionId(int type, int permissonId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && permissonId == m.Permission_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissionsByTypeTypeIdAndPermissionId(int type, int typeId, int permissonId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && m.Type_Id==typeId && permissonId == m.Permission_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissionsByTypeTypeIdListAndPermissionId(int type, List<int> listTypeId, int permissonId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && listTypeId.Any(s=>s==m.Type_Id) && permissonId == m.Permission_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, List<int?> listTypeIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && listTypeIds.Contains(m.Type_Id)).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPermissionsAssignment> GetListOfPermissions(DBContextHelperLinq db, int type, int typeId, int permissionId)
        {
            try
            {
                
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && m.Type_Id == typeId && m.Permission_Id==permissionId).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
            
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, int typeId, int permissionId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    List<DbPermissionsAssignment> listDbPermission =
                        db.DbPermissionsAssignments.AsNoTracking()
                            .Where(m => m.Type == type && m.Type_Id == typeId && m.Permission_Id == permissionId)
                            .ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, List<int?> listTypeId, int permissionId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    List<DbPermissionsAssignment> listDbPermission =
                        db.DbPermissionsAssignments.AsNoTracking()
                            .Where(m => m.Type == type && listTypeId.Contains(m.Type_Id) && m.Permission_Id == permissionId)
                            .ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
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
