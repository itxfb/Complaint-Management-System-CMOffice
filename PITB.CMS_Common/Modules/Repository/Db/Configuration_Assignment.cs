using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{
    public partial class DbConfiguration_Assignment
    {
        public static DbConfiguration_Assignment Get(string key)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Key == key && n.Is_Active).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbConfiguration_Assignment> Get(List<string> listKey)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => listKey.Contains(n.Key) && n.Is_Active).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbConfiguration_Assignment> GetByTagId(string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Tag_Id == tagId && n.Is_Active).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static DbConfiguration_Assignment GetByKey(string key)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Key == key && n.Is_Active).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbConfiguration_Assignment> GetBy(string tagId, int type1)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbConfiguration_Assignment.Where(n => n.Tag_Id == tagId && n.Type_1==type1 && n.Is_Active).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
