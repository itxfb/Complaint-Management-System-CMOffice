using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using System.Web.Mvc;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.View.ClientMesages;
using System.Data.Entity;
using PITB.CMS.Handler.DataTableJquery;
using PITB.CMS.Models.Custom.DataTable;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Models.Custom;

namespace PITB.CMS.Handler.Business
{
    public class BlTag
    {
        public static VmTagEdit GetTagEditVm(int recordId)
        {
            VmTagEdit vmTagEdit = new VmTagEdit();
            vmTagEdit.RecordId = recordId;

            DbCallTagging dbcallTagging = DbCallTagging.GetByRecordId(recordId);
            if (string.IsNullOrEmpty(dbcallTagging.Caller_Name) || dbcallTagging.Department_ID==null ||
                dbcallTagging.PPMRP_Service_ID == null) // has not already added
            {
                vmTagEdit.HasAlreadyAdded = false;

                vmTagEdit.CallerName = "";
                vmTagEdit.DepartmentList = DbDepartmentCategory.All().Select(n => new SelectListItem() { Text = n.Name, Value = n.Id.ToString() }).ToList();
                vmTagEdit.PpmrpServiceList = new List<SelectListItem>();
                
                
            }
            else // has already added 
            {
                vmTagEdit.HasAlreadyAdded = true;

                vmTagEdit.CallerName = dbcallTagging.Caller_Name;

                vmTagEdit.DepartmentList = DbDepartmentCategory.All().Select(n => new SelectListItem() { Text = n.Name, Value = n.Id.ToString() }).ToList();
                vmTagEdit.DepartmentId = dbcallTagging.Department_ID;

                vmTagEdit.PpmrpServiceList = DbDepartmentSubCategory.GetByCategoryId(Convert.ToInt32(dbcallTagging.Department_ID)).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id.ToString() }).ToList();
                vmTagEdit.PpmrpServiceId = dbcallTagging.PPMRP_Service_ID;
            }  
            
            return vmTagEdit;
        }

        public static string OnEditTag(VmTagEdit vmTagEdit)
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();

            DBContextHelperLinq db = new DBContextHelperLinq();

            DbCallTagging dbcallTagging = DbCallTagging.GetByRecordId(vmTagEdit.RecordId, db);

            dbcallTagging.Caller_Name = vmTagEdit.CallerName;
            dbcallTagging.Department_ID = vmTagEdit.DepartmentId;
            dbcallTagging.PPMRP_Service_ID = vmTagEdit.PpmrpServiceId;
            dbcallTagging.Agent_ID = cmsCookie.UserId;
            dbcallTagging.Agent_Name = cmsCookie.UserName;
            db.Entry(dbcallTagging).State = EntityState.Modified;
            db.SaveChanges();
            //return new StatusMessage(Config.CommandStatus.Success, "Record Id = " + vmTagEdit.RecordId + " has been edited successfully.", Config.Message.Failure, Config.Message.Error);
            return "Record Id = " + vmTagEdit.RecordId+" has been edited successfully.";
        }

        public static List<VmAgentTagListing> GetTagListVmAgentData(DataTableParamsModel dtModel)
        {
            return GetTaggingListDataTable(dtModel, 1).ToList<VmAgentTagListing>();
        }

        public static DataTable GetTaggingListDataTable(DataTableParamsModel dtModel, int spType=2)
        {
            List<string> prefixStrList = new List<string> { "CallTagging", "CallTagging", "CallTagging", "CallTagging", "CallTagging", "CallTagging", "DepartmentCategory", "DepartmentSubCategory", "CallTagging", "CallTagging", "CallTagging" };
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("CallTagging.Start_Time", "CONVERT(VARCHAR(10),CallTagging.Start_Time,120) Like '%_Value_%'");
            dictFilterQuery.Add("CallTagging.End_Time", "CONVERT(VARCHAR(10),CallTagging.End_Time,120) Like '%_Value_%'");
            dictFilterQuery.Add("DepartmentCategory.DepartmentCategoryName", "DepartmentCategory.Name Like '%_Value_%'");
            dictFilterQuery.Add("DepartmentSubCategory.DepartmentSubCategoryName", "DepartmentSubCategory.Name Like '%_Value_%'");

            //dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>(), prefixStrList, dictFilterQuery);
            //List<VmAgentTagListing> listOfTagging = GetTaggingList(dtModel);

            // call api
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@SpType", (spType).ToDbObj());
            paramDict.Add("@StartRow", (dtModel.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtModel.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", dtModel.ListOrder[0].columnName.ToDbObj());
            paramDict.Add("@OrderByDirection", dtModel.ListOrder[0].sortingDirectionStr.ToDbObj());
            paramDict.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch.ToDbObj());

            //DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Tagging_List]", paramDict);
            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Tagging_List]", paramDict);//.ToList<VmAgentTagListing>();


            //return listOfTagging;
        }

        /*
        public static List<VmAgentTagListing> GetTaggingList(DataTableParamsModel dtParams)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@StartRow", (dtParams.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtParams.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", dtParams.ListOrder[0].columnName.ToDbObj());
            paramDict.Add("@OrderByDirection", dtParams.ListOrder[0].sortingDirectionStr.ToDbObj());
            paramDict.Add("@WhereOfMultiSearch", dtParams.WhereOfMultiSearch.ToDbObj());

            //DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Tagging_List]", paramDict);
            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Tagging_List]", paramDict).ToList<VmAgentTagListing>();
        }*/
    }
}