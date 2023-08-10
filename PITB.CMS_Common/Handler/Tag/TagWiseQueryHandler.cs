using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Handler.Tag
{
    public class TagWiseQueryHandler
    {

        public static string GetTagWiseQuery2(string tagId, string tagKey, Dictionary<string, object> dictParam)
        {
            string strQuery = TagWiseBestMatchHandler.GetBestMatch(tagId, tagKey);
            if (strQuery != null)
            {
                return QueryHelper.GetParamsReplacedQuery(strQuery, dictParam);
            }
            else
            {
                return null;
            }

        }
        public static string GetTagWiseQuery(string tagId, string key)
        {
            List<DbConfiguration_Assignment> listDbConfigAssign = DbConfiguration_Assignment.GetBy(tagId, 2);
            if (listDbConfigAssign.FirstOrDefault() != null)
            {
                string matchedQuery = string.Empty;
                //tagToMatch += string.Format("__UserId::{0}", dbUser.User_Id);
                string matchedKeyTag = TagHandler.GetMatchedTag(key, listDbConfigAssign.Select(n => n.Key).ToList());
                var tag = listDbConfigAssign.Where(n => n.Key == matchedKeyTag).FirstOrDefault();
                if (tag != null)
                {
                    matchedQuery = tag.Value;
                    return matchedQuery;
                }
                else return null;
            }
            return null;
        }

        public static string GetTagWiseQuery(string tagKeyToMatch, string queryTag, Dictionary<string, object> dictParam)
        {
            //----- New configuration assignment implementation----
            //string tagToMatch = string.Format("RoleId::{0}{1}CampaignId::{2}{3}ModuleName::{4}{5}UserId::{6}", (int)dbUser.Role_Id, "__", dbUser.Campaign_Id, "__", "Export", "__", dbUser.User_Id);
            //string tagToMatch = string.Format("RoleId::{0}{1}CampaignId::{2}{3}ModuleName::{4}", (int)dbUser.Role_Id, "__", dbUser.Campaign_Id, "__", "Export");
            List<DbConfiguration_Assignment> listDbConfigAssign = DbConfiguration_Assignment.GetBy(queryTag, 2);
            if (listDbConfigAssign.FirstOrDefault() != null)
            {
                string matchedQuery = string.Empty;
                //tagToMatch += string.Format("__UserId::{0}", dbUser.User_Id);
                string matchedKeyTag = TagHandler.GetMatchedTag(tagKeyToMatch, listDbConfigAssign.Select(n => n.Key).ToList());
                if (matchedKeyTag == null) return matchedKeyTag;

                var tag = listDbConfigAssign.Where(n => n.Key == matchedKeyTag).FirstOrDefault();
                if (tag != null)
                {
                    matchedQuery = tag.Value;
                    return QueryHelper.GetParamsReplacedQuery(matchedQuery, dictParam);
                }
                else return null;
                //DbConfiguration_Assignment dbConfigAssign = listDbConfigAssign.Where(n => n.Key == matchedKey).FirstOrDefault();
                //permDictToRet = JsonConvert.DeserializeObject<Dictionary<string, string>>(dbConfigAssign.Value);
                //return permDictToRet;
            }
            return null;
        }
    }
}
