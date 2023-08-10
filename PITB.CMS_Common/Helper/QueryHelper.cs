using System.Collections.Generic;
using System.Linq;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Helper.Extensions;

namespace PITB.CMS_Common.Helper
{
    public class QueryHelper
    {
        public static string GetFinalQuery(string module, Config.ConfigType configType, Dictionary<string, object> dictQueryParams = null)
        {
            Dictionary<string, string> dictParams = new Dictionary<string, string>() { { "Module", module } };
            string query = ConfigurationHandler.GetConfiguration(dictParams, configType);
            if (query != null)
            {
                return GetParamsReplacedQuery(query, dictQueryParams);
            }
            return null;
        }

        //public static string GetFinalQueryOrDefault(string module, string defaultModule, Config.ConfigType configType, Dictionary<string, object> dictQueryParams = null)
        //{
        //    Dictionary<string, string> dictParams = new Dictionary<string, string>() { { "Module", module } };
        //    GetConfiguration
        //    string query = ConfigurationHandler.GetConfiguration(dictParams, configType);
        //    if (query != null)
        //    {
        //        return GetParamsReplacedQuery(query, dictQueryParams);
        //    }
        //    return null;
        //}

        public static string GetFinalQuery(string module, string campaign, Config.ConfigType configType, Dictionary<string, object> dictQueryParams)
        {
            Dictionary<string, string> dictParams = new Dictionary<string, string>() { { "Module", module }, { "Campaign", campaign } };
            string query = ConfigurationHandler.GetConfiguration(dictParams, configType);
            if (query != null)
            {
                return GetParamsReplacedQuery(query, dictQueryParams);
            }
            return null;
        }
        public static string GetParamsReplacedQuery(string query, Dictionary<string, object> dictQueryParams)
        {
            //             query = @"
            //					    SELECT * from (SELECT  listingModelBase.SelectionFields complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name,complaints.Province_Id Province_Id,complaints.Province_Name Province_Name,complaints.Division_Id Division_Id,complaints.Division_Name Division_Name,complaints.District_Id District_Id, complaints.District_Name District_Name, complaints.Tehsil_Id Tehsil_Id,Ref_Complaint_Id, complaints.Tehsil_Name Tehsil_Name, complaints.UnionCouncil_Id UnionCouncil_Id,complaints.UnionCouncil_Name UnionCouncil_Name,
            //				    CONVERT(VARCHAR(10),complaints.Created_Date,120)+RIGHT(CONVERT(VARCHAR, complaints.Created_Date, 100), 7) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name
            //                          ComplaintTypeSelection , count(*)  OVER() AS Total_Rows,
            //				    ROW_NUMBER() OVER (ORDER BY listingModelBase.OrderByColumnName  listingModelBase.OrderByDirection 
            //                            ) AS RowNum
            //				    FROM pitb.Complaints complaints  listingModelBase.InnerJoinLogic 
            //			
            //				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id
            //
            //				    WHERE complaints.Complaint_Type =  listingModelBase.ComplaintType 
            //                           and EXISTS(SELECT 1 FROM dbo.SplitString('listingModelBase.Campaign',',') X WHERE X.Item=complaints.Compaign_Id)
            //				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN 'listingModelBase.From' AND 'listingModelBase.To' ) 
            //				    AND EXISTS(SELECT 1 FROM dbo.SplitString('listingModelBase.Category',',') X WHERE X.Item=complaints.Complaint_Category)
            //				
            //				    usercategoryCheck  ComplaintTypeHierarchyCheck
            //                            MulticolumnSearchQuery 
            //				    -- My code
            //			
            //				    --and complaints.Province_Id = @ProvinceId) as tbl
            //				      HierarchyCheck ) as tbl
            //			
            //				    WHERE tbl.RowNum BETWEEN  listingModelBase.StartRow  AND  listingModelBase.EndRow 
            //		        ";

            if (dictQueryParams == null) return query;
            int startIndex = 0, endIndex = 0, matchedCharCount = 0;
            bool hasFound = false;//, hasFirstCharMatched = false;
            foreach (KeyValuePair<string, object> keyVal in dictQueryParams)
            {
                string varToReplace = keyVal.Key.ObjToStr();
                for (int i = 0; i < query.Length; i++)
                {
                    startIndex = i;
                    endIndex = startIndex;
                    matchedCharCount = 0;
                    //hasFirstCharMatched = false;
                    for (int j = 0; j < varToReplace.Length; j++)
                    {
                        if (varToReplace[j] == query[endIndex]) // if letter matches the substring char
                        {
                            endIndex++;
                            matchedCharCount++;
                        }
                        else
                        {
                            matchedCharCount = 0;
                            endIndex = startIndex;
                            break;
                        }
                    }

                    if (matchedCharCount == varToReplace.Length)
                    {
                        //int indexAfterMatch = endIndex;
                        if (endIndex < query.Length) // if next char is present after match
                        {
                            if (query[endIndex] != '_' && !Utility.IsEnglishLetter(query[endIndex]) && !Utility.IsNumericLetter(query[endIndex]))
                            // if next char is not english letter and _
                            {
                                hasFound = true;
                            }
                            else
                            {
                                hasFound = false;
                            }
                        }
                        else
                        {
                            hasFound = true;
                        }

                        if (hasFound)
                        {
                            query = query.Remove(startIndex, varToReplace.Count());
                            query = query.Insert(startIndex, keyVal.Value.ObjToStr());
                        }
                    }

                }

                //query = query.Replace(keyVal.Key,keyVal.Value.ObjToStr());
            }
            return query;
            //string finalQuery = query.Replace()
        }
    }
}