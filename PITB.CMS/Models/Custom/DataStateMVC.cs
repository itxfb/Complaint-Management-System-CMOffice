using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.Custom
{
    public class DataStateMVC
    {
        private static int PoolMaxId = Config.DataPoolMaxLength;
        private static int CurrId { get; set; }

        private static List<DataStateModel> ListDataStateModel = new List<DataStateModel>();

        public static int AddInPool(object data,string fileName = "",string startDate = "",string endDate = "")
        {
            CurrId = (CurrId + 1) % PoolMaxId;
            DataStateModel dataStateModel = new DataStateModel(CurrId, data,fileName,startDate,endDate);
            ListDataStateModel.Add(dataStateModel);
            return CurrId;
        }

        public static void RemoveFromPool (int id)
        {
            ListDataStateModel.Remove(ListDataStateModel.Single(n=>n.Id==id));
        }

        public static object GetDataFromPool(int id)
        {
            if (ListDataStateModel != null && ListDataStateModel.Count > 0)
            {
                return ListDataStateModel.Where(n => n.Id == id).FirstOrDefault().Data;
            }
            return null;
        }
        public static DataStateModel GetStoredObjectFromPool(int id)
        {
            return ListDataStateModel.Where(n => n.Id == id).FirstOrDefault();
        }
    }

    public class DataStateModel
    {
        public int Id { get; set; }
        public object Data { get; set; }

        public string FileName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public DataStateModel (int id, object data,string fileName,string startDate,string endDate)
        {
            this.Id = id;
            this.Data = data;
            this.FileName = fileName;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
    }
}