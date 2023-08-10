using System.Linq;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.DB
{
    using PITB.CRM_API.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Translation_Mapping")]
    public partial class DbTranslationMapping
    {
        public int Id { get; set; }

        public int? Parent_Type_Id { get; set; }

        public int? Type_Id { get; set; }

        public int? SubType_Id { get; set; }

        [StringLength(400)]
        public string OrignalString { get; set; }

        [StringLength(400)]
        public string UrduMappedString { get; set; }

        public bool Is_Active { get; set; }


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
                    List<DbTranslationMapping> listDbTranslationMapping = db.DbTranslationMapping.Where(n=> n.Is_Active).ToList();
                    return listDbTranslationMapping;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static Dictionary<string,TranslatedModel> GetTranslationDictionaryFromTranslationMapping(List<DbTranslationMapping> listDbTranslationMapping)
            {
                try
                {
                    Dictionary<string, TranslatedModel> dict= new Dictionary<string, TranslatedModel>();
                    
                    DbTranslationMapping translationMapping = null;
                    TranslatedModel translationModel = null;

                    List<string> groupOrignalStr = listDbTranslationMapping.GroupBy(n => n.OrignalString).Select(n=>n.Key).ToList();
                    string strAssign="";
                    foreach (string str in groupOrignalStr)
                    {
                        translationMapping = listDbTranslationMapping.Where(n => n.OrignalString == str).ToList().First();
                        translationModel = new TranslatedModel();
                        translationModel.UrduMappedString = translationMapping.UrduMappedString;
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
}
