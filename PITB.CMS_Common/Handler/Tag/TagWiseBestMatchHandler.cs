using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Handler.Tag
{
    public class TagWiseBestMatchHandler
    {
        public static string GetBestMatch(string tagId, string tagKey)
        {
            //string statusMaskTag = string.Format("Config::MaskedStatuses__RoleId::{0}", (int)dbUser.Role_Id);
            List<DbConfiguration_Assignment> listDbConfigAssign = DbConfiguration_Assignment.GetByTagId(tagId);
            if (listDbConfigAssign.FirstOrDefault() != null)
            {
                string matchedKey = TagHandler.GetMatchedTag(tagKey, listDbConfigAssign.Select(n => n.Key).ToList());
                DbConfiguration_Assignment dbConfigAssign = listDbConfigAssign.Where(n => n.Key == matchedKey).FirstOrDefault();
                return dbConfigAssign.Value;
            }
            return null;
        }
    }
}
