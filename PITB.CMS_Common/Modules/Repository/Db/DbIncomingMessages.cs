using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    public partial class DbIncomingMessages
    {
        public static DbIncomingMessages GetByMessageId(int id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.Where(n => n.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbIncomingMessages> GetMessagesByPhoneNo(string phoneNo)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.Where(n => n.Caller_No == phoneNo).OrderBy(n => n.Message_Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbIncomingMessages> GetAllMessagesGroupByPhoneNoOrderByDateTime()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.OrderByDescending(n => n.Message_Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbIncomingMessages> GetByIncomingMessageIdsList(List<int> listIncomingMessageIds)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.Where(n => listIncomingMessageIds.Contains(n.Id)).OrderBy(n => n.Message_Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<string> GetUniquePhoneNoList()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.GroupBy(n => n.Caller_No).Select(n => n.Key).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
