using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace PITB.CMS_Common.Models
{

    public partial class DbClientSystem
    {
        public static DbClientSystem GetBy(string systemName, string username, string password)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbClientSystem.Where(n => n.SystemName == systemName && n.SystemUserName==username && n.Password==password).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
