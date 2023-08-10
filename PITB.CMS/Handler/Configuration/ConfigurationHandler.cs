using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PITB.CMS.Helper;
using PITB.CMS.Helper.Extensions;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Handler.Configuration
{
    public class ConfigurationHandler
    {
        public static string GetConfiguration(string module, Config.ConfigType configType)
        {
            Dictionary<string, string> dictParams = new Dictionary<string, string>() { { "Module", module } };
            string key = GetConfiguration(dictParams, configType);
            return key;
            //DbConfiguration_Assignment dbConfiguration = DbConfiguration_Assignment.Get(key);

            //if (dbConfiguration != null)
            //{
            //    return dbConfiguration.Value;
            //}
            //else
            //{
            //    return null;
            //}
        }

        //public static string GetConfiguration(string module, string condition, Config.ConfigType configType)
        //{
        //    Dictionary<string, string> dictParams = new Dictionary<string, string>() { { "Module", module }, { "RoleId", roleId.ToString() }, { "Campaign", campaign } };
        //    string key = GetConfiguration(dictParams, configType);
        //    return key;
           
        //}

        /*public static string GetConfiguration(string module, int roleId, string campaign, Config.ConfigType configType)
        {
            Dictionary<string, string> dictParams = new Dictionary<string, string>() { { "Module", module }, { "RoleId", roleId.ToString() }, { "Campaign", campaign } };
            string key = GetConfiguration(dictParams, configType);
            return key;
           
        }*/

        public static string GetConfiguration(List<string> listKeys, string moduleSpecificValue = null)
        {
            string moduleVal = null;
            string moduleSVal = null;
            foreach (string key in listKeys)
            {
                moduleVal = ConfigurationHandler.GetConfiguration(key);
                if (moduleVal != null)
                {
                    if (moduleSpecificValue != null)
                    {
                        Utility.ConvertCollonFormatToDict(moduleVal).TryGetValue(moduleSpecificValue, out moduleSVal);
                        return moduleSVal;
                    }
                    else
                    {
                        return moduleVal;
                    }
                }
            }
            return moduleVal;
        }

        public static string GetConfiguration(string configKey)
        {
            DbConfiguration_Assignment dbConfiguration = DbConfiguration_Assignment.Get(configKey);

            if (dbConfiguration != null)
            {
                return dbConfiguration.Value;
            }
            else
            {
                return null;
            }
        }

        public static string GetConfiguration(Dictionary<string,string> dictParams, Config.ConfigType configType )
        {
            string key = GetConfigurationKey(dictParams, configType);
            DbConfiguration_Assignment dbConfiguration = DbConfiguration_Assignment.Get(key);
            
            if (dbConfiguration != null)
            {
                return dbConfiguration.Value;
            }
            else
            {
                return null;
            }
        }

        

        private static string GetConfigurationKey(Dictionary<string, string> dictParams, Config.ConfigType configType)
        {
            string module, campaign;
            string key = "Type::" + Config.DictConfigType[configType];
            foreach (KeyValuePair<string, string> keyVal in dictParams)
            {
                if (keyVal.Value != null)
                {
                    key = key + Config.Separator + keyVal.Key.ObjToStr() + "::" + keyVal.Value.ObjToStr();
                }
            }
            //if (dictParams.TryGetValue("Module", out module))
            //{
            //    module = Config.Separator + "Module::" + module;
            //}
            //if (dictParams.TryGetValue("Campaign", out campaign))
            //{
            //    campaign = Config.Separator + "Campaign::" + campaign;
            //}
            //dictParams.TryGetValue("campaign", out campaign);
            //string key = string.Format("Type::" + Config.DictConfigType[configType] + module.ObjToStr() + campaign.ObjToStr());
            return key;
        }
    }

}