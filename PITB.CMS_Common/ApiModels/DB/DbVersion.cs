namespace PITB.CRM_API.Models.DB
{
    using Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Version")]
    public class DbVersion
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? Version_Type { get; set; }

        public int? App_Id { get; set; }

        public int? Version_Id { get; set; }

        #region HelperMethods
        public static int GetDbVersion(Config.VersionType versionType, Config.AppID appId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (int)db.DbVersion.AsNoTracking().Where(n=>n.Version_Type== (int) versionType && n.App_Id == (int) appId).FirstOrDefault().Version_Id;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion
    }
}