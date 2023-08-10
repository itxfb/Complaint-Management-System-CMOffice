using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Common.Models
{
    public partial class DbVersion
    {
        #region HelperMethods
        public static int GetDbVersion(Config.VersionType versionType, Config.AppID appId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (int)db.DbVersion.AsNoTracking().Where(n => n.Version_Type == (int)versionType && n.App_Id == (int)appId).FirstOrDefault().Version_Id;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion
    }
}