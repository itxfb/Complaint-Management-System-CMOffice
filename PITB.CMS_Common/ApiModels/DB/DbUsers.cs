using System.Linq;
using PITB.CRM_API.Helper.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CRM_API.Models.DB
{
  

    [Table("PITB.Users")]
    public partial class DbUsers
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(20)]
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

        public string Imei_No { get; set; }


        public DateTime? Created_Date { get; set; }

        public int? Created_By { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Updated_Date { get; set; }

        public DateTime? Password_Updated { get; set; }

        public bool IsMultipleLoginsAllowed { get; set; }

        public bool IsLoggedIn { get; set; }

        //[StringLength(50)]
        public string Name { get; set; }

        //[StringLength(20)]
        public string Phone { get; set; }

        public string Cnic { get; set; }

        //[StringLength(50)]
        public string Email { get; set; }

        public string Address { get; set; }
        public string Campaigns { get; set; }
        public string Categories { get; set; }

        public int? UserCategoryId1 { get; set; }

        public int? UserCategoryId2 { get; set; }

        public string Designation { get; set; }

        public string Designation_abbr { get; set; }

        #region HelperMethods

        public static DbUsers GetByUserName(string userName)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == userName && n.IsActive);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static DbUsers GetByUserName(string userName, DBContextHelperLinq db)
        {
            try
            {
                return db.DbUsers.Where(n => n.Username == userName).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbUsers> GetByCnic(string cnic, DBContextHelperLinq db)
        {
            try
            {
                return
                    db.DbUsers.Where(n => n.Cnic == cnic && n.IsActive)
                        .ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbUsers GetByUsernameAndImeiNo(string userName, string imeiNo)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return
                        db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == userName && n.Imei_No == imeiNo);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbUsers GetByPhoneNoCnicAndImeiNo(string phoneNo, string cnic, string imeiNo)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return
                        db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Phone == phoneNo && n.Cnic==cnic && n.Imei_No == imeiNo);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetImeiAgainstUsername(string username)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbUsers dbUser = db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == username);
                    if (dbUser != null)
                    {
                        return dbUser.Imei_No;
                    }

                    return "";

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetImeiAgainstPhoneNoAndCnic(string phoneNo, string cnic)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbUsers dbUser = db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Phone == phoneNo && n.Cnic==cnic);
                    if (dbUser != null)
                    {
                        return dbUser.Imei_No;
                    }

                    return "";

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsImeiPresent(string imeiNo)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    if (db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Imei_No == imeiNo) != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsUsernamePresent(string username)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    if (db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == username) != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsUsernameAndCnicPresent(string username, string cnic)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    if (
                        db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == username && n.Cnic == cnic) !=
                        null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsPhoneNoAndCnicPresent(string phoneNo, string cnic)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    if (
                        db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Phone == phoneNo && n.Cnic == cnic) !=
                        null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsUsernameAndPasswordPresent(string username, string password)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    if (
                        db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == username && n.Password == password) !=
                        null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DbUsers GetByUsernameAndPasswordPresent(string username, string password)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbUsers.AsNoTracking().FirstOrDefault(n => n.Username == username && n.Password == password);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static List<DbUsers> GetByUsernameAndPasswordPresent(string username, string password, DBContextHelperLinq db)
        {
            try
            {
                //using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //return db.DbUsers.ToList();
                    return db.DbUsers.Where(n => n.Username == username && n.Password == password && n.IsActive).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetHierarchyIdValueAgainstHierarchyId(DbUsers dbUsers)
        {
            Config.Hierarchy hierarchyValue = (Config.Hierarchy)dbUsers.Hierarchy_Id;
            switch (hierarchyValue)
            {
                case Config.Hierarchy.Division:
                    return dbUsers.Division_Id;
                    break;

                case Config.Hierarchy.District:
                    return dbUsers.District_Id;
                    break;

                case Config.Hierarchy.Province:
                    return dbUsers.Province_Id;
                    break;

                case Config.Hierarchy.Tehsil:
                    return dbUsers.Tehsil_Id;
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return dbUsers.UnionCouncil_Id;
                    break;

                case Config.Hierarchy.Ward:
                    return dbUsers.Ward_Id;
                    break;
            }
            return "-1";
        }

        public static List<DbUsers> GetByCampIdH_IdUserH_Id(int campaignId, Config.Hierarchy hierarchyId,
            int hierarchyIdVal, int? userHierarchyId, int categoryId)
        {
            try
            {
                string campaignIdStr = campaignId.ToString();
                userHierarchyId = (userHierarchyId == Config.UserMaxHierarchy) ? null : userHierarchyId;
                DBContextHelperLinq db = new DBContextHelperLinq();
                switch (hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        return
                            db.DbUsers.Where(
                                m =>
                                    m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId &&
                                    m.User_Hierarchy_Id == userHierarchyId && m.IsActive).ToList()
                                .Where(
                                    n =>
                                        n.Province_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .Where(
                                    n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                                .ToList();
                        break;

                    case Config.Hierarchy.Division:
                        return
                            db.DbUsers.Where(
                                m =>
                                    m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId &&
                                    m.User_Hierarchy_Id == userHierarchyId && m.IsActive).ToList()
                                .Where(
                                    n =>
                                        n.Division_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .Where(
                                    n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                                .ToList();
                        break;

                    case Config.Hierarchy.District:
                        return
                            db.DbUsers.Where(
                                m =>
                                    m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId &&
                                    m.User_Hierarchy_Id == userHierarchyId && m.IsActive).ToList()
                                .Where(
                                    n =>
                                        n.District_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .Where(
                                    n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                                .ToList().ToList();
                        break;

                    case Config.Hierarchy.Tehsil:
                        return
                            db.DbUsers.Where(
                                m =>
                                    m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId &&
                                    m.User_Hierarchy_Id == userHierarchyId && m.IsActive).ToList()
                                .Where(
                                    n =>
                                        n.Tehsil_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .Where(
                                    n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                                .ToList().ToList();
                        break;

                    case Config.Hierarchy.UnionCouncil:
                        return
                            db.DbUsers.Where(
                                m =>
                                    m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId &&
                                    m.User_Hierarchy_Id == userHierarchyId && m.IsActive).ToList()
                                .Where(
                                    n =>
                                        n.UnionCouncil_Id.Split(',')
                                            .Select(i => int.Parse(i))
                                            .ToList()
                                            .Contains(hierarchyIdVal))
                                .Where(
                                    n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                                .ToList().ToList();
                        break;

                    case Config.Hierarchy.Ward:
                        return
                            db.DbUsers.Where(
                                m =>
                                    m.Campaigns == campaignIdStr && m.Hierarchy_Id == hierarchyId &&
                                    m.User_Hierarchy_Id == userHierarchyId && m.IsActive).ToList()
                                .Where(
                                    n =>
                                        n.Ward_Id.Split(',').Select(i => int.Parse(i)).ToList().Contains(hierarchyIdVal))
                                .Where(
                                    n => n.Categories.Split(',').Select(i => int.Parse(i)).ToList().Contains(categoryId))
                                .ToList().ToList();
                        break;
                }
                return new List<DbUsers>();
            }
            catch (Exception ex)
            {
                return null;
            }

        #endregion
        }
    }
}
