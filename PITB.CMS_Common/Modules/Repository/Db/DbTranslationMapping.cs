using System.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.Models
{
    public partial class DbTranslationMapping
    {
        #region HelperMethods
        public static List<DbTranslationMapping> GetMappedTranslation(int parentTypeId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbTranslationMapping> listDbTranslationMapping = db.DbTranslationMapping.Where(n => n.Parent_Type_Id == parentTypeId && n.Is_Active).ToList();
                return listDbTranslationMapping;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbTranslationMapping> GetAllTranslation()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbTranslationMapping> listDbTranslationMapping = db.DbTranslationMapping.Where(n => n.Is_Active).ToList();
                return listDbTranslationMapping;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, dynamic> GetTranslationDictionaryFromTranslationMapping(List<DbTranslationMapping> listDbTranslationMapping)
        {
            try
            {
                Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

                DbTranslationMapping translationMapping = null;
                dynamic translationModel = null;

                List<string> groupOrignalStr = listDbTranslationMapping.GroupBy(n => n.OrignalString).Select(n => n.Key).ToList();
                string strAssign = "";
                foreach (string str in groupOrignalStr)
                {
                    translationMapping = listDbTranslationMapping.Where(n => n.OrignalString == str).ToList().First();
                    translationModel = new ExpandoObject();
                    translationModel.Urdu = translationMapping.UrduMappedString;
                    //strAssign = str.Replace("/n", "");
                    //strAssign = strAssign.Replace("/r", "");
                    strAssign = str.Trim();

                    dict.Add(strAssign, translationModel);
                }
                return dict;
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //List<DbTranslationMapping> listDbTranslationMapping = db.DbTranslationMapping.Where(n => n.Parent_Type_Id == parentTypeId).ToList();
                //return listDbTranslationMapping;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }

    #region From API
    public partial class DbTranslationMapping
    {
        public static Dictionary<string, TranslatedModel> GetTranslationDictionaryFromTranslationMapping_API(List<DbTranslationMapping> listDbTranslationMapping)
        {
            try
            {
                Dictionary<string, TranslatedModel> dict = new Dictionary<string, TranslatedModel>();

                DbTranslationMapping translationMapping = null;
                TranslatedModel translationModel = null;

                List<string> groupOrignalStr = listDbTranslationMapping.GroupBy(n => n.OrignalString).Select(n => n.Key).ToList();
                string strAssign = "";
                foreach (string str in groupOrignalStr)
                {
                    translationMapping = listDbTranslationMapping.Where(n => n.OrignalString == str).ToList().First();
                    translationModel = new TranslatedModel();
                    translationModel.UrduMappedString = translationMapping.UrduMappedString;
                    strAssign = str.Replace("/n", "");
                    strAssign = strAssign.Replace("/r", "");
                    strAssign = str.Trim();
                    dict.Add(strAssign, translationModel);
                }
                return dict;
                //    DBContextHelperLinq db = new DBContextHelperLinq();
                //    List<DbTranslationMapping> listDbTranslationMapping = db.DbTranslationMapping.Where(n => n.Parent_Type_Id == parentTypeId).ToList();
                //    return listDbTranslationMapping;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    #endregion
}