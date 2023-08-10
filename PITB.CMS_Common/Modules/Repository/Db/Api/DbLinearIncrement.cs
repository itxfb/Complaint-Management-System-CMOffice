using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{
    public partial class DbLinearIncrement
    {
        public static int GetMaxCount(int typeId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbLinearIncrement.Where(n => n.Type == typeId).FirstOrDefault().Incremental_Value;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static void IncrementValue(DBContextHelperLinq db, int typeId, int incrementValue)
        {
            try
            {
                DbLinearIncrement dbLinearIncrement = db.DbLinearIncrement.Where(n => n.Type == typeId).FirstOrDefault();
                dbLinearIncrement.Incremental_Value = dbLinearIncrement.Incremental_Value + incrementValue;
                db.DbLinearIncrement.Attach(dbLinearIncrement);
                db.Entry(dbLinearIncrement).Property(n => n.Incremental_Value).IsModified = true;
                //return db.DbLinearIncrement.Where(n => n.Type == typeId).FirstOrDefault().Incremental_Value;
            }
            catch (Exception ex)
            {
                //return -1;
            }
        }

    }
}