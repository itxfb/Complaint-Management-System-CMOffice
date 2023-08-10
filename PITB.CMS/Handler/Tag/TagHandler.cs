using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Handler.Tag
{
    public class TagHandler
    {
        public static string GetMatchedTag(string tagToMatch, List<string> listTag)
        {
            if (listTag != null && listTag.Count > 0)
            {
                List<dynamic> listDyn = GetMatchedTagScoreList(tagToMatch, listTag);
                dynamic d = listDyn.FirstOrDefault();
                if (d != null && d.matchedScore > 0)
                {
                    return d.tag;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static List<dynamic> GetMatchedTagScoreList(string tagToMatch, List<string> listTagStr)
        {
            List<dynamic> listDynamic = new List<dynamic>();
            //d.matchedCount = 0;
            //d.notMatchedCount = 0;
            Dictionary<string, string> dictTagToMatch = Utility.ConvertCollonFormatToDict(tagToMatch);
            //Dictionary<string, string> dictTagSrc = ConvertCollonFormatToDict(tSrc);
            foreach (string tagSrcStr in listTagStr)
            {
                Dictionary<string, string> dictTagSrc = Utility.ConvertCollonFormatToDict(tagSrcStr);
                dynamic d = GetTagMatchedScore(tagToMatch, tagSrcStr, dictTagToMatch);
                listDynamic.Add(d);
            }
            listDynamic = listDynamic.OrderByDescending(n => n.matchedCount).ToList();
            return listDynamic;
        }

        private static dynamic GetTagMatchedScore(string tagToMatch, string tagSrcStr, Dictionary<string, string> dictTagToMatch=null)
        {
            dictTagToMatch = dictTagToMatch == null ? Utility.ConvertCollonFormatToDict(tagToMatch) : dictTagToMatch;
            Dictionary<string, string> dictTagSrc = Utility.ConvertCollonFormatToDict(tagSrcStr);
            dynamic d = new ExpandoObject();
            d.tag = tagSrcStr;
            d.matchedScore = 0.0f;
            d.matchedCount = 0;
            bool isMatched = true;
            for (int i = 0; i < dictTagSrc.Count && isMatched; i++)
            {
                KeyValuePair<string, string> keyVal = dictTagSrc.ElementAt(i);
                string val = null;

                if (dictTagToMatch.TryGetValue(keyVal.Key, out val)) // if key matches
                {
                    if (val == keyVal.Value) // if value matches
                    {
                        d.matchedCount += 1;
                    }
                    else // if value doesnt match
                    {
                        d.matchedCount = 0;
                        isMatched = false;
                    }
                }

            }

            d.matchedScore = (float)(dictTagSrc.Count * d.matchedCount) / dictTagToMatch.Count;
            return d;
        }

       
    }
}