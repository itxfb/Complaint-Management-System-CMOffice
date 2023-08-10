using System.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Handlers.Translation
{
    public static class TranslationHandler
    {
        public static List<T> GetTranslatedList<T>(this List<T> lst, string fieldToReplace, Dictionary<string, TranslatedModel> translationDict, Config.Language language) //where T : new()
        {
            PropertyInfo prop = null;
            string key = "";
            string[] fieldNameArr = fieldToReplace.Split(',');
            TranslatedModel translationModel;
            foreach (T obj in lst)
            {
                foreach (string fieldName in fieldNameArr)
                {
                    prop = obj.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        //prop.GetValue(obj).ToString()
                        //string s = ;
                        key = prop.GetValue(obj).ToString();
                        if (translationDict.TryGetValue(key, out translationModel))
                        {
                            prop.SetValue(obj, GetTranslation(translationDict, key, language), null);
                        }
                    }
                }
            }
            return lst;
        }

        public static List<T> GetTranslatedList<T>(this List<T> lst, string fieldToReplace,string fieldFromReplace, Dictionary<string, TranslatedModel> translationDict, Config.Language language) //where T : new()
        {
            PropertyInfo propFromReplace = null;
            PropertyInfo propToReplace = null;
            string key = "";
            string[] fieldNameFromReplaceArr = fieldFromReplace.Split(',');
            string[] fieldToReplaceArr = fieldToReplace.Split(',');
            TranslatedModel translationModel;
            int i = 0;
            foreach (T obj in lst)
            {
                i = 0;
                foreach (string fieldNameFromReplace in fieldNameFromReplaceArr)
                {
                    propFromReplace = obj.GetType().GetProperty(fieldNameFromReplace, BindingFlags.Public | BindingFlags.Instance);
                    propToReplace = obj.GetType().GetProperty(fieldToReplaceArr[i], BindingFlags.Public | BindingFlags.Instance);

                    if (null != propFromReplace && propFromReplace.CanWrite)
                    {
                        //prop.GetValue(obj).ToString()
                        //string s = ;
                        key = propFromReplace.GetValue(obj).ToString();
                        if (translationDict.TryGetValue(key, out translationModel))
                        {
                            propToReplace.SetValue(obj, GetTranslation(translationDict, key, language), null);
                        }
                    }
                    i++;
                }
            }
            return lst;
        }

       


        public static T GetTranslatedModel<T>(this T model, string fieldToReplace, Dictionary<string, TranslatedModel> translationDict, Config.Language language) //where T : new()
        {
            PropertyInfo prop = null;
            string key = "";
            string[] fieldNameArr = fieldToReplace.Split(',');
            TranslatedModel translationModel;
            foreach (string fieldName in fieldNameArr)
            {
                prop = model.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    //prop.GetValue(obj).ToString()
                    //string s = ;
                    key = prop.GetValue(model).ToString();
                    if (translationDict.TryGetValue(key, out translationModel))
                    {
                        prop.SetValue(model, GetTranslation(translationDict, key, language), null);
                    }
                }
            }
            return model;
        }

        private static string GetTranslation(Dictionary<string, TranslatedModel> translationDict, string key, Config.Language language)
        {
            string translationStr = "";
            switch (language)
            {
                case   Config.Language.Default:
                    translationStr = key;
                    break;

                case Config.Language.English:
                    translationStr = key;
                    break;

                case Config.Language.Urdu:
                    translationStr = translationDict[key].UrduMappedString;
                    break;
            }

            return translationStr;
        }
    }
}