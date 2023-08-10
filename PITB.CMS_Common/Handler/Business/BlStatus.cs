using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common.Handler.CustomJsonConverter;
using PITB.CMS_Common.Models;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlStatus
    {
        public static dynamic GetStatusMaskingModel(dynamic dStatusMask/*string jsonStatusMask*/, List<DbStatus> listDbStatus)
        {
            dynamic dToRet = new ExpandoObject();
            //dynamic d = JsonConvert.DeserializeObject(jsonStatusMask, typeof(ExpandoObject), new DynamicJsonConverter());

            Dictionary<int, int> dictStatusIdsMap = new Dictionary<int, int>();
            Dictionary<string, string> dictStatusStrMap = new Dictionary<string, string>();
            Dictionary<int,List<int>> dictFilterStatuses = new Dictionary<int, List<int>>();
            foreach(var v in dStatusMask.map)
            {
                dictStatusIdsMap.Add(v.key, v.value);

                dictStatusStrMap.Add(listDbStatus.Where(n => n.Complaint_Status_Id == v.key).First().Status,
                    listDbStatus.Where(n => n.Complaint_Status_Id == v.value).First().Status);

                if(dictFilterStatuses.ContainsKey(v.value))
                {
                    dictFilterStatuses[v.value].Add(v.key);
                }
                else
                {
                    dictFilterStatuses.Add(v.value, new List<int>() {v.key});
                }
                
            }

            //dictFilterStatuses = d.statusMask.filterStatuses;
            dToRet.dictStatusIdsMap = dictStatusIdsMap;
            dToRet.dictStatusStrMap = dictStatusStrMap;
            dToRet.dictFilterStatuses = dictFilterStatuses;
            dToRet.listFilterStatuses = dStatusMask.filterStatuses;
            return dToRet;
            //Dictionary<string,string> dict = new Dictionary<string, string>()
        }
        
        public static dynamic GetStatusIconMappingDict(dynamic dParam)
        {
            Dictionary<int, string> dictIconMap = new Dictionary<int, string>();
            foreach(var v in dParam)
            {
                dictIconMap.Add(v.id, v.url);
            }
            return dictIconMap;
        }

        public static List<DbStatus> GetFilteredStatusList(List<DbStatus> listDbStatus, int statusToRemove)
        {
            //SelectListItem item = null;
            //List<SelectListItem> listStatus = new List<SelectListItem>();

            listDbStatus.RemoveAll(n => n.Complaint_Status_Id == statusToRemove || (n.Complaint_Status_Id == Convert.ToInt32(Config.ComplaintStatus.PendingFresh) || n.Complaint_Status_Id == Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed)));
            return listDbStatus;
            //foreach (DbStatus dbStatus in listDbStatus)
            //{
            //    if (statusToRemove != dbStatus.Complaint_Status_Id && Convert.ToInt32(Config.ComplaintStatus.PendingFresh) != dbStatus.Complaint_Status_Id && Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed) != dbStatus.Complaint_Status_Id)
            //    {
            //        item = new SelectListItem();
            //        item.Value = dbStatus.Complaint_Status_Id.ToString();
            //        item.Text = dbStatus.Status;
            //        listStatus.Add(item);
            //    }
            //}
            //return listStatus;
        }
    }
}