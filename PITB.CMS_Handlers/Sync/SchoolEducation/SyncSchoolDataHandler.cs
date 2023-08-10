using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PITB.CMS_Common.Helper.Database;
using System.Net.Http;
using System.Data;
using PITB.CMS_Models.ApiModels.Request;
using PITB.CMS_Models.DB;
using PITB.CMS_Common;
using PITB.CMS_Handlers.DB.Repository;
using PITB.CMS_Models.ApiModels.Response;
using PITB.CMS_Models.Custom;
using Antlr.Runtime.Misc;
using Newtonsoft.Json;

namespace PITB.CMS_Handlers.Sync.SchoolEducation
{
    public class SyncSchoolDataHandler
    {
        public static string conStr = Config.ConnectionString;
        //public SqlConnection connection;//= new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
        public static DateTime CurrDateTime;

        public static void SynRegionAndShoolsDataMain(DateTime from, DateTime to)
        {
            try
            {
                ////--------------------Sync Region and School Data main-------------------------
                //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 2", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 2", Config.ServiceType.SyncSchoolDataMain);
                ////---------------------------------------------
                CurrDateTime = DateTime.Now;
            
                List<Config.SyncApiRequiredData> listDataRequired = new List<Config.SyncApiRequiredData> { Config.SyncApiRequiredData.districts, 
                    Config.SyncApiRequiredData.tehsils, 
                    Config.SyncApiRequiredData.markazes, 
                    Config.SyncApiRequiredData.school };

                ApiReqSyncSEModel.SyncSEData syncData = new ApiReqSyncSEModel.SyncSEData();

                syncData.sec_key = "pitbasdf12345";
                syncData.system_type = "crm";
            
                syncData.start_date = from.ToString("yyyy-MM-dd");
                syncData.end_date = to.ToString("yyyy-MM-dd");

                //DBContextHelperLinq db = new DBContextHelperLinq();

                List<DbCrmIdsMappingToOtherSystem> listCrmIdsMapping = RepoDbCrmIdsMappingToOtherSystem.GetAll();

                List<DbProvince> listProvince = RepoDbProvince.AllProvincesList();

                List<DbDivision> listDivision = RepoDbDivision.GetByProvinceIdsStrAndGroupId(Utility.GetCommaSepStrFromList(listProvince.Select(n=>n.Province_Id).ToList()),null);

                List<DbDistrict> listDistricts = RepoDbDistrict.GetAll();

                int? groupId = RepoDbHierarchyCampaignGroupMapping.GetModelByCampaignId((int) Config.Campaign.SchoolEducationEnhanced, Config.Hierarchy.Tehsil);
                List<DbTehsil> listTehsils = RepoDbTehsil.GetByDistrictIdList(Utility.GetNullableIntList(listDistricts.Select(n => n.District_Id).ToList()), groupId);

                groupId = RepoDbHierarchyCampaignGroupMapping.GetModelByCampaignId((int)Config.Campaign.SchoolEducationEnhanced, Config.Hierarchy.UnionCouncil);
                List<DbUnionCouncils> listUnionCouncils = RepoDbUnionCouncils.GetUnionCouncilList(Utility.GetNullableIntList(listTehsils.Select(n => n.Tehsil_Id).ToList()), groupId);

                List<DbSchoolsMapping> lisDbSchoolMapping = RepoDbSchoolsMapping.GeAll();
            
                int crmModuleId = (int) Config.CrmModule.Hierarchy;
                int otsSystemId = (int) Config.OtherSystemId.SchoolEducation;
                int crmModuleCat1 = 0;

                ////--------------------Sync Region and School Data main-------------------------
                //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 3", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 3", Config.ServiceType.SyncSchoolDataMain);
                ////---------------------------------------------

                int maxIterationCount = 15;
                int totalIteration = 0;

                for (int i=1; i < listDataRequired.Count; i++)
                {
                    try
                    {
                        syncData.data_required = listDataRequired[i].ToString();
                        if (listDataRequired[i] == Config.SyncApiRequiredData.districts)
                        {
                            crmModuleCat1 = (int)Config.Hierarchy.District;
                            SynDistricts(listCrmIdsMapping.Where(n => n.Crm_Module_Id == crmModuleId
                                && n.OTS_System_Id == otsSystemId
                                && n.Crm_Module_Id == crmModuleId
                                && n.Crm_Module_Cat1 == crmModuleCat1
                                ).ToList(), listDistricts, (ApiResponseSyncSEModel.ResponseSEDataDistrict)GetSyncSeDataApiResponse(syncData));
                        }
                        else if (listDataRequired[i] == Config.SyncApiRequiredData.tehsils)
                        {
                            ////--------------------Sync Region and School Data main-------------------------
                            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 4", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 4", Config.ServiceType.SyncSchoolDataMain);
                            ////---------------------------------------------

                            crmModuleCat1 = (int)Config.Hierarchy.Tehsil;
                            SynTehsils(listCrmIdsMapping.Where(n => n.Crm_Module_Id == crmModuleId
                                && n.OTS_System_Id == otsSystemId
                                && n.Crm_Module_Id == crmModuleId
                                && n.Crm_Module_Cat1 == crmModuleCat1
                                ).ToList(), listTehsils, (ApiResponseSyncSEModel.ResponseSEDataTehsil)GetSyncSeDataApiResponse(syncData));

                            //--------------------Sync Region and School Data main-------------------------
                            RepoDbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 5", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 5", Config.ServiceType.SyncSchoolDataMain);
                            //---------------------------------------------

                        }
                        else if (listDataRequired[i] == Config.SyncApiRequiredData.markazes)
                        {
                            ////--------------------Sync Region and School Data main-------------------------
                            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 6", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 6", Config.ServiceType.SyncSchoolDataMain);
                            ////---------------------------------------------

                            crmModuleCat1 = (int)Config.Hierarchy.UnionCouncil;
                            SynUnionCouncils(listCrmIdsMapping.Where(n => n.Crm_Module_Id == crmModuleId
                                && n.OTS_System_Id == otsSystemId
                                && n.Crm_Module_Id == crmModuleId
                                && n.Crm_Module_Cat1 == crmModuleCat1
                                ).ToList(), listUnionCouncils, (ApiResponseSyncSEModel.ResponseSEDataMarkaz)GetSyncSeDataApiResponse(syncData));

                            ////--------------------Sync Region and School Data main-------------------------
                            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 7", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 7", Config.ServiceType.SyncSchoolDataMain);
                            ////---------------------------------------------
                        }
                        else if (listDataRequired[i] == Config.SyncApiRequiredData.school)
                        {
                            ////--------------------Sync Region and School Data main-------------------------
                            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 8", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 8", Config.ServiceType.SyncSchoolDataMain);
                            ////---------------------------------------------
                    
                            listDistricts = RepoDbDistrict.GetAll();

                            groupId = RepoDbHierarchyCampaignGroupMapping.GetModelByCampaignId((int)Config.Campaign.SchoolEducationEnhanced, Config.Hierarchy.Tehsil);
                            listTehsils = RepoDbTehsil.GetByDistrictIdList(Utility.GetNullableIntList(listDistricts.Select(n => n.District_Id).ToList()), groupId);

                            groupId = RepoDbHierarchyCampaignGroupMapping.GetModelByCampaignId((int)Config.Campaign.SchoolEducationEnhanced, Config.Hierarchy.UnionCouncil);
                            listUnionCouncils = RepoDbUnionCouncils.GetUnionCouncilList(Utility.GetNullableIntList(listTehsils.Select(n => n.Tehsil_Id).ToList()), groupId);

                            List<DbComplaint> listDbComplaint = RepoDbComplaint.GetBy((int)Config.Campaign.SchoolEducationEnhanced, new List<int> { (int)Config.ComplaintStatus.PendingFresh, (int)Config.ComplaintStatus.PendingReopened, (int)Config.ComplaintStatus.UnsatisfactoryClosed });
                            //List<int> listStatus = new List<int> { (int)CMS.Config.ComplaintStatus.PendingFresh, (int)CMS.Config.ComplaintStatus.PendingReopened, (int)CMS.Config.ComplaintStatus.UnsatisfactoryClosed };
                            //List<DbComplaint> listDbComplaint = DbComplaint.GetBy((int)CMS.Config.Campaign.SchoolEducationEnhanced, listStatus);
                            SynSchools( listCrmIdsMapping.Where(n => n.OTS_System_Id == otsSystemId).ToList(), lisDbSchoolMapping, (ApiResponseSyncSEModel.ResponseSEDataSchool)GetSyncSeDataApiResponse(syncData), listDbComplaint, listProvince, listDivision, listDistricts, listTehsils, listUnionCouncils);

                            ////--------------------Sync Region and School Data main-------------------------
                            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 9", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 9", Config.ServiceType.SyncSchoolDataMain);
                            ////---------------------------------------------
                    
                        }

                    }
                    catch (Exception)
                    {
                        i--;
                        
                    }
                    totalIteration++;
                    if (totalIteration == maxIterationCount)
                    {
                        return;
                    }
                }

                //------- Sync Users ---------
                ApiReqSyncSEModel.SyncSEUsersData syncUserData = new ApiReqSyncSEModel.SyncSEUsersData();

                syncUserData.sec_key = syncData.sec_key;
                syncUserData.system_type = syncData.system_type;
            
                syncUserData.start_date = syncData.start_date;
                syncUserData.end_date = syncData.end_date;

                syncUserData.user_type = "all";
                syncUserData.user_id = "-1";

                int campaignId = (int) PITB.CMS_Common.Config.Campaign.SchoolEducationEnhanced;
                List<DbUsers> listDbUsers = RepoDbUsers.GetUsersAgainstCampaign(campaignId);

                SynUsers(listCrmIdsMapping.Where(n => n.OTS_System_Id == otsSystemId).ToList(), listDbUsers, GetSyncSeUsersDataApiResponse(syncUserData),listProvince,listDivision,
                    listDistricts,listTehsils,listUnionCouncils);
            }
            catch (Exception ex)
            {
                //throw;
            }   
        }

        public static bool SynDistricts(/*DBContextHelperLinq db,*/ List<DbCrmIdsMappingToOtherSystem> listCrmIdsMapping, List<DbDistrict> listDbDistricts , ApiResponseSyncSEModel.ResponseSEDataDistrict responseDistrict)
        {
            SqlConnection connection = new SqlConnection(conStr);
            try
            {
            List<Pair<ApiResponseSyncSEModel.ResponseSEDataDistrict.District, DbCrmIdsMappingToOtherSystem, DbDistrict, Config.SyncOperations>> listOverallMapping =
                new List<Pair<ApiResponseSyncSEModel.ResponseSEDataDistrict.District, DbCrmIdsMappingToOtherSystem, DbDistrict, Config.SyncOperations>>();

            DbCrmIdsMappingToOtherSystem dbTempCrmIdsMapping = listCrmIdsMapping.FirstOrDefault();

            //-------- RecordToUpdate -------
            listOverallMapping = (from respDist in responseDistrict.data
                      join map in listCrmIdsMapping on
                          Convert.ToInt32(respDist.district_id) equals map.OTS_Id
                      join dbDistrict in listDbDistricts on
                          map.Crm_Id equals dbDistrict.District_Id
                      select new Pair<ApiResponseSyncSEModel.ResponseSEDataDistrict.District, DbCrmIdsMappingToOtherSystem, DbDistrict, Config.SyncOperations>
                      (
                        respDist,
                        map,
                        dbDistrict,
                        Config.SyncOperations.Update
                      )).ToList();

            //------ RecordsToInsert--------
            listOverallMapping.AddRange(responseDistrict.data.Where(
                n => !listCrmIdsMapping.Select(x => x.OTS_Id).Contains(Convert.ToInt32(n.district_id))).ToList().Select(n => new Pair<ApiResponseSyncSEModel.ResponseSEDataDistrict.District, DbCrmIdsMappingToOtherSystem, DbDistrict, Config.SyncOperations>
                      (
                        n,
                        null,
                        null,
                        Config.SyncOperations.Add
                      )).ToList()); 

            //
            List<Pair<DbDistrict, ApiResponseSyncSEModel.ResponseSEDataDistrict.District>> listModelToAdd = new List<Pair<DbDistrict, ApiResponseSyncSEModel.ResponseSEDataDistrict.District>>();

            bool isUpdated = false;
            foreach (Pair<ApiResponseSyncSEModel.ResponseSEDataDistrict.District, DbCrmIdsMappingToOtherSystem, DbDistrict, Config.SyncOperations> overallMap in listOverallMapping)
            {
                try
                {
                    if (overallMap.Item4 == Config.SyncOperations.Update)
                    {
                        overallMap.Item3.id = overallMap.Item3.District_Id;
                        isUpdated = overallMap.Item3.SetUpdated(overallMap.Item1);

                        if (!isUpdated)
                        {
                            overallMap.Item4 = Config.SyncOperations.NoChange;
                            overallMap.Item2.Updated_Date = CurrDateTime;
                        }
                    }
                    else if (overallMap.Item4 == Config.SyncOperations.Add)
                    {
                        //---- add value in main table
                        DbDistrict dbDistrict = RepoDbDistrict.GetDbDistrict(overallMap.Item1);
                        listModelToAdd.Add(new Pair<DbDistrict, ApiResponseSyncSEModel.ResponseSEDataDistrict.District>(dbDistrict,overallMap.Item1));
                        //---- add value in mapping table

                    }
                }
                catch (Exception)
                {

                }
            }
            
               // SqlConnection connection = new SqlConnection(conStr);
                connection.Open();
                List<DbDistrict> listToMerge = listOverallMapping.Where(n => n.Item4 == Config.SyncOperations.Update).Select(n => n.Item3).ToList();
                listToMerge.AddRange(listModelToAdd.Select(x => x.Item1).ToList());
                RepoDbDistrict.BulkMerge(listToMerge, connection);

                List<DbCrmIdsMappingToOtherSystem> listDbCrmMappingToOtherSystem = new List<DbCrmIdsMappingToOtherSystem>();
                DbCrmIdsMappingToOtherSystem dbCrmIdsMappingToOtherSystem = null;
 

                foreach (Pair<DbDistrict, ApiResponseSyncSEModel.ResponseSEDataDistrict.District> modelToAdd in listModelToAdd)
                {
                    dbCrmIdsMappingToOtherSystem = RepoDbCrmIdsMappingToOtherSystem.GetModel(dbTempCrmIdsMapping,
                        modelToAdd.Item1.id, Convert.ToInt32(modelToAdd.Item2.district_id), modelToAdd.Item2.is_Active, CurrDateTime);
                
                    listDbCrmMappingToOtherSystem.Add(dbCrmIdsMappingToOtherSystem);
                }

                RepoDbCrmIdsMappingToOtherSystem.BulkMerge(listDbCrmMappingToOtherSystem, connection);
                connection.Close();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return false;

        }





        public static bool SynTehsils(/*DBContextHelperLinq db,*/ List<DbCrmIdsMappingToOtherSystem> listCrmIdsMapping, List<DbTehsil> listDbTehsils, ApiResponseSyncSEModel.ResponseSEDataTehsil responseTehsil)
        {
            SqlConnection connection = new SqlConnection(conStr);
            try
            {

                ////--------------------Sync Region and School Data main-------------------------
                //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 9", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 9", Config.ServiceType.SyncSchoolDataMain);
                ////---------------------------------------------

            List<Pair<ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil, DbCrmIdsMappingToOtherSystem, DbTehsil, Config.SyncOperations>> listOverallMapping =
                new List<Pair<ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil, DbCrmIdsMappingToOtherSystem, DbTehsil, Config.SyncOperations>>();



            DbCrmIdsMappingToOtherSystem dbTempCrmIdsMapping = listCrmIdsMapping.FirstOrDefault();

            List<DbCrmIdsMappingToOtherSystem> listCrmDistrictIdsMapping =
                RepoDbCrmIdsMappingToOtherSystem.Get(dbTempCrmIdsMapping.Crm_Module_Id, (int)Config.Hierarchy.District,
                    dbTempCrmIdsMapping.OTS_System_Id, dbTempCrmIdsMapping.OTS_Module_Id);

            ////--------------------Sync Region and School Data main-------------------------
            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 10", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 10", Config.ServiceType.SyncSchoolDataMain);
            ////---------------------------------------------

            //-------- RecordToUpdate -------
            listOverallMapping = (from respTehs in responseTehsil.data
                                  join map in listCrmIdsMapping on
                                      Convert.ToInt32(respTehs.tehsil_id) equals map.OTS_Id
                                  join dbTehsil in listDbTehsils on
                                      map.Crm_Id equals dbTehsil.Tehsil_Id
                                  select new Pair<ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil, DbCrmIdsMappingToOtherSystem, DbTehsil, Config.SyncOperations>
                                  (
                                    respTehs,
                                    map,
                                    dbTehsil,
                                    Config.SyncOperations.Update
                                  )).ToList();

            //------ RecordsToInsert--------
            listOverallMapping.AddRange(responseTehsil.data.Where(
                n => !listCrmIdsMapping.Select(x => x.OTS_Id).Contains(Convert.ToInt32(n.tehsil_id))).ToList().Select(n => new Pair<ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil, DbCrmIdsMappingToOtherSystem, DbTehsil, Config.SyncOperations>
                      (
                        n,
                        null,
                        null,
                        Config.SyncOperations.Add
                      )).ToList());

            //

            ////--------------------Sync Region and School Data main-------------------------
            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 11", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 11", Config.ServiceType.SyncSchoolDataMain);
            ////---------------------------------------------
            List<Pair<DbTehsil, ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil>> listModelToAdd = new List<Pair<DbTehsil, ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil>>();

            bool isUpdated = false;
            foreach (Pair<ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil, DbCrmIdsMappingToOtherSystem, DbTehsil, Config.SyncOperations> overallMap in listOverallMapping)
            {
                try
                {
                    if (overallMap.Item4 == Config.SyncOperations.Update)
                    {
                        overallMap.Item3.Id = overallMap.Item3.Tehsil_Id;
                        isUpdated = overallMap.Item3.SetUpdated(overallMap.Item1,
                            Convert.ToInt32(
                                listCrmDistrictIdsMapping.Where(
                                    n => n.OTS_Id == Convert.ToInt32(overallMap.Item1.district_id_Fk))
                                    .FirstOrDefault()
                                    .Crm_Id));
                        if (!isUpdated)
                        {
                            overallMap.Item4 = Config.SyncOperations.NoChange;
                            overallMap.Item2.Updated_Date = CurrDateTime;
                        }

                    }
                    else if (overallMap.Item4 == Config.SyncOperations.Add)
                    {
                        //---- add value in main table
                        DbTehsil dbTehsil = RepoDbTehsil.GetDbTeshil(overallMap.Item1,
                            listDbTehsils.FirstOrDefault().Group_Id,
                            Convert.ToInt32(
                                listCrmDistrictIdsMapping.Where(
                                    n => n.OTS_Id == Convert.ToInt32(overallMap.Item1.district_id_Fk))
                                    .FirstOrDefault()
                                    .Crm_Id));
                        listModelToAdd.Add(
                            new Pair<DbTehsil, ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil>(dbTehsil,
                                overallMap.Item1));

                    }
                }
                catch (Exception)
                {

                }
            }
            ////--------------------Sync Region and School Data main-------------------------
            //DbWindowServiceError.SaveErrorLog(2, "Start SyncRegionAndSchoolDataMain 12", "Start SchoolEducationHandler.SynRegionAndShoolsDataMain() 12", Config.ServiceType.SyncSchoolDataMain);
            ////---------------------------------------------
                //SqlConnection connection = new SqlConnection(conStr);
                connection.Open();
                List<DbTehsil> listToMerge = listOverallMapping.Where(n => n.Item4 == Config.SyncOperations.Update).Select(n => n.Item3).ToList();
                listToMerge.AddRange(listModelToAdd.Select(x => x.Item1).ToList());
                RepoDbTehsil.BulkMerge(listToMerge, connection);


                List<DbCrmIdsMappingToOtherSystem> listDbCrmMappingToOtherSystem = new List<DbCrmIdsMappingToOtherSystem>();
                DbCrmIdsMappingToOtherSystem dbCrmIdsMappingToOtherSystem = null;
 
                foreach (Pair<DbTehsil, ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil> modelToAdd in listModelToAdd)
                {
                    dbCrmIdsMappingToOtherSystem = RepoDbCrmIdsMappingToOtherSystem.GetModel(dbTempCrmIdsMapping,
                        modelToAdd.Item1.Id, Convert.ToInt32(modelToAdd.Item2.tehsil_id), modelToAdd.Item2.is_Active, CurrDateTime);
                
                    listDbCrmMappingToOtherSystem.Add(dbCrmIdsMappingToOtherSystem);
                }
                RepoDbCrmIdsMappingToOtherSystem.BulkMerge(listDbCrmMappingToOtherSystem, connection);
                connection.Close();
                }
            catch (Exception ex)
            {
                RepoDbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex, Config.ServiceType.SyncSchoolDataMain);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return false;

        }

        


        public static bool SynUnionCouncils(/*DBContextHelperLinq db,*/ List<DbCrmIdsMappingToOtherSystem> listCrmIdsMapping, List<DbUnionCouncils> listDbUc, ApiResponseSyncSEModel.ResponseSEDataMarkaz responseMarkaz)
        {
            SqlConnection connection = new SqlConnection(conStr);
            try
            {
                List
                    <
                        Pair
                            <ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz, DbCrmIdsMappingToOtherSystem,
                                DbUnionCouncils, Config.SyncOperations>> listOverallMapping =
                                    new List
                                        <
                                            Pair
                                                <ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz,
                                                    DbCrmIdsMappingToOtherSystem, DbUnionCouncils, Config.SyncOperations
                                                    >>();



                DbCrmIdsMappingToOtherSystem dbTempCrmIdsMapping = listCrmIdsMapping.FirstOrDefault();

                List<DbCrmIdsMappingToOtherSystem> listCrmTehsilIdsMapping =
                    RepoDbCrmIdsMappingToOtherSystem.Get(dbTempCrmIdsMapping.Crm_Module_Id, (int) Config.Hierarchy.Tehsil,
                        dbTempCrmIdsMapping.OTS_System_Id, dbTempCrmIdsMapping.OTS_Module_Id);



                //-------- RecordToUpdate -------
                listOverallMapping = (from respMark in responseMarkaz.data
                    join map in listCrmIdsMapping on
                        Convert.ToInt32(respMark.markaz_id) equals map.OTS_Id
                    join dbUc in listDbUc on
                        map.Crm_Id equals dbUc.UnionCouncil_Id
                    select
                        new Pair
                            <ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz, DbCrmIdsMappingToOtherSystem,
                                DbUnionCouncils, Config.SyncOperations>
                            (
                            respMark,
                            map,
                            dbUc,
                            Config.SyncOperations.Update
                            )).ToList();

                //------ RecordsToInsert--------
                listOverallMapping.AddRange(responseMarkaz.data.Where(
                    n => !listCrmIdsMapping.Select(x => x.OTS_Id).Contains(Convert.ToInt32(n.markaz_id)))
                    .ToList()
                    .Select(
                        n =>
                            new Pair
                                <ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz, DbCrmIdsMappingToOtherSystem,
                                    DbUnionCouncils, Config.SyncOperations>
                                (
                                n,
                                null,
                                null,
                                Config.SyncOperations.Add
                                )).ToList());

                //
                List<Pair<DbUnionCouncils, ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz>> listModelToAdd =
                    new List<Pair<DbUnionCouncils, ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz>>();
                bool isUpdated = false;

                foreach (
                    Pair
                        <ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz, DbCrmIdsMappingToOtherSystem,
                            DbUnionCouncils, Config.SyncOperations> overallMap in listOverallMapping)
                {
                    try
                    {
                        if (overallMap.Item4 == Config.SyncOperations.Update)
                        {
                            overallMap.Item3.Id = overallMap.Item3.UnionCouncil_Id;
                            isUpdated = overallMap.Item3.SetUpdated(overallMap.Item1,
                                Convert.ToInt32(
                                    listCrmTehsilIdsMapping.Where(
                                        n => n.OTS_Id == Convert.ToInt32(overallMap.Item1.tehsil_id_Fk))
                                        .FirstOrDefault()
                                        .Crm_Id));

                            if (!isUpdated)
                            {
                                overallMap.Item4 = Config.SyncOperations.NoChange;
                                overallMap.Item2.Updated_Date = CurrDateTime;
                            }


                        }
                        else if (overallMap.Item4 == Config.SyncOperations.Add)
                        {
                            //---- add value in main table
                            DbUnionCouncils dbUc = RepoDbUnionCouncils.GetDbMarkaz(overallMap.Item1,
                                listDbUc.FirstOrDefault().Group_Id,
                                Convert.ToInt32(
                                    listCrmTehsilIdsMapping.Where(
                                        n => n.OTS_Id == Convert.ToInt32(overallMap.Item1.tehsil_id_Fk))
                                        .FirstOrDefault()
                                        .Crm_Id));
                            listModelToAdd.Add(
                                new Pair<DbUnionCouncils, ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz>(dbUc,
                                    overallMap.Item1));
                            //---- add value in mapping table

                        }
                    }
                    catch (Exception)
                    {

                    }
                }


                //-- Bulk Merge --------
                List<DbUnionCouncils> listUc =
                    listOverallMapping.Where(n => n.Item4 == Config.SyncOperations.Update).Select(n => n.Item3).ToList();
                listUc.AddRange(listModelToAdd.Select(x => x.Item1).ToList());
                
                connection.Open();
                //var asd = listUc.GroupBy(n => n.Id).Where(n => n.Count() > 1).ToList();
                //foreach (var ss in asd)
                //{

                //}
                RepoDbUnionCouncils.BulkMerge(listUc, connection);

                ////---- End Bulk Insertion --------
                List<DbCrmIdsMappingToOtherSystem> listDbCrmMappingToOtherSystem =
                    new ListStack<DbCrmIdsMappingToOtherSystem>();
                DbCrmIdsMappingToOtherSystem dbCrmIdsMappingToOtherSystem = null;
                foreach (
                    Pair<DbUnionCouncils, ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz> modelToAdd in
                        listModelToAdd)
                {
                    dbCrmIdsMappingToOtherSystem = RepoDbCrmIdsMappingToOtherSystem.GetModel(dbTempCrmIdsMapping,
                        modelToAdd.Item1.Id, Convert.ToInt32(modelToAdd.Item2.markaz_id), modelToAdd.Item2.is_Active,
                        CurrDateTime);

                    listDbCrmMappingToOtherSystem.Add(dbCrmIdsMappingToOtherSystem);
                }
                RepoDbCrmIdsMappingToOtherSystem.BulkMerge(listDbCrmMappingToOtherSystem, connection);
                connection.Close();

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return false;

        }

        

        


        public static bool SynSchools(/*DBContextHelperLinq db,*/ List<DbCrmIdsMappingToOtherSystem> listCrmIdsMapping, List<DbSchoolsMapping> listDbSchoolsMapping, 
            ApiResponseSyncSEModel.ResponseSEDataSchool responseSchoolMap, List<DbComplaint> listDbComplaint,List<DbProvince> listProvinces, List<DbDivision> listDivisions, List<DbDistrict> listDistricts, List<DbTehsil> listTehsils, List<DbUnionCouncils> listUc)
        {
            SqlConnection connection = new SqlConnection(conStr);
            try
            {

            
                List<Pair<ApiResponseSyncSEModel.ResponseSEDataSchool.School, DbCrmIdsMappingToOtherSystem, DbSchoolsMapping, Config.SyncOperations>> listOverallMapping =
                    new List<Pair<ApiResponseSyncSEModel.ResponseSEDataSchool.School, DbCrmIdsMappingToOtherSystem, DbSchoolsMapping, Config.SyncOperations>>();

            

                int crmModuleId = (int)Config.CrmModule.SchoolMapping;
                List<DbCrmIdsMappingToOtherSystem> listSchoolIdsMapping = listCrmIdsMapping.Where(n => n.Crm_Module_Id == crmModuleId).ToList();
                DbCrmIdsMappingToOtherSystem dbTempCrmIdsMapping = listSchoolIdsMapping.FirstOrDefault();

                crmModuleId = (int)Config.CrmModule.Hierarchy;
                List<DbCrmIdsMappingToOtherSystem> listHierarchyIdsMapping = listCrmIdsMapping.Where(n => n.Crm_Module_Id == crmModuleId).ToList();

                //-------- RecordToUpdate -------
                listOverallMapping = (from respSchool in responseSchoolMap.data
                                      join map in listSchoolIdsMapping on
                                          Convert.ToInt32(respSchool.school_id) equals map.OTS_Id
                                      join dbSchoolMapping in listDbSchoolsMapping on
                                          map.Crm_Id equals dbSchoolMapping.Id
                                      select new Pair<ApiResponseSyncSEModel.ResponseSEDataSchool.School, DbCrmIdsMappingToOtherSystem, DbSchoolsMapping, Config.SyncOperations>
                                      (
                                        respSchool,
                                        map,
                                        dbSchoolMapping,
                                        Config.SyncOperations.Update
                                      )).ToList();

                //------ RecordsToInsert--------



                listOverallMapping.AddRange(responseSchoolMap.data.Where(
                     n => !listSchoolIdsMapping.Select(x => x.OTS_Id).Contains(Convert.ToInt32(n.school_id))).ToList().Select(n => new Pair<ApiResponseSyncSEModel.ResponseSEDataSchool.School, DbCrmIdsMappingToOtherSystem, DbSchoolsMapping, Config.SyncOperations>
                           (
                             n,
                             null,
                             null,
                             Config.SyncOperations.Add
                           )).ToList());

                //
                List<DbSchoolEducationHeadMapping> listDbHeadMapping = RepoDbSchoolEducationHeadMapping.GetAll();
                List<Pair<DbSchoolsMapping, ApiResponseSyncSEModel.ResponseSEDataSchool.School>> listModelToAdd = new List<Pair<DbSchoolsMapping, ApiResponseSyncSEModel.ResponseSEDataSchool.School>>();
                bool isUpdated = false;
                DbSchoolsMapping dbSchoolMap = null;

               
                int i = 0;
                foreach (Pair<ApiResponseSyncSEModel.ResponseSEDataSchool.School, DbCrmIdsMappingToOtherSystem, DbSchoolsMapping, Config.SyncOperations> overallMap in listOverallMapping)
                {
                    try
                    {
                        if (overallMap.Item4 == Config.SyncOperations.Update)
                        {

                            try
                            {
                                isUpdated = overallMap.Item3.SetUpdated( listDbHeadMapping, overallMap.Item1,
                                    listHierarchyIdsMapping, listDistricts,
                                    listTehsils, listUc);
                                if (!isUpdated)
                                {
                                    overallMap.Item4 = Config.SyncOperations.NoChange;
                                    overallMap.Item2.Updated_Date = CurrDateTime;
                                }
                            }
                            catch (Exception ex)
                            {
                        
                            }
                        }
                        else if (overallMap.Item4 == Config.SyncOperations.Add)
                        {
                            try
                            {
                                //---- add value in main table
                                dbSchoolMap = RepoDbSchoolsMapping.GetDbSchoolMapping( overallMap.Item1, listHierarchyIdsMapping,
                                    listDistricts,
                                    listTehsils, listUc);

                                if (dbSchoolMap != null)
                                {
                                    listModelToAdd.Add(
                                        new Pair<DbSchoolsMapping, ApiResponseSyncSEModel.ResponseSEDataSchool.School>(
                                            dbSchoolMap, overallMap.Item1));
                                }
                                //---- add value in mapping table
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    i++;
                }
                //SqlConnection connection = new SqlConnection(conStr);
                connection.Open();
                // Bulk Update
                List<DbSchoolsMapping> listSchoolMap = listOverallMapping.Where(n => n.Item4 == Config.SyncOperations.Update).Select(n => n.Item3).ToList().ToList();
                listSchoolMap.AddRange(listModelToAdd.Select(x => x.Item1).ToList());
                List<DbSchoolEducationHeadMapping> listHeadMap = listSchoolMap.Select(n => n.dbHeadMapping).ToList();

                RepoDbSchoolsMapping.BulkMerge(listSchoolMap, connection);
                RepoDbSchoolEducationHeadMapping.BulkMerge(listHeadMap, connection);

            

                List<DbCrmIdsMappingToOtherSystem> listDbCrmMappingToOtherSystem = new ListStack<DbCrmIdsMappingToOtherSystem>();
                DbCrmIdsMappingToOtherSystem dbCrmIdsMappingToOtherSystem = null;
                
                //db.SaveChanges();
                foreach (Pair<DbSchoolsMapping, ApiResponseSyncSEModel.ResponseSEDataSchool.School> modelToAdd in listModelToAdd)
                {
                    dbCrmIdsMappingToOtherSystem = RepoDbCrmIdsMappingToOtherSystem.GetModel(dbTempCrmIdsMapping,
                            modelToAdd.Item1.Id, Convert.ToInt32(modelToAdd.Item2.school_id), modelToAdd.Item2.is_Active, CurrDateTime);

                    listDbCrmMappingToOtherSystem.Add(dbCrmIdsMappingToOtherSystem);
                }
                RepoDbCrmIdsMappingToOtherSystem.BulkMerge(listDbCrmMappingToOtherSystem, connection);
                

                //-------- Sync Complaints-------
                List<DbSchoolEducationComplaint> listDbSchoolEducationComplaints = (from  dbComplaint in listDbComplaint
                                                                                   join dbSchool in listSchoolMap
                                                                                   on dbComplaint.RefField1 equals dbSchool.school_emis_code
                                                                                    select new DbSchoolEducationComplaint
                                                                                    {
                                                                                        DbComplaint = dbComplaint,
                                                                                        DbSchoolMapping = dbSchool
                                                                                    }).ToList();
                
                for (int j=0; j<listDbSchoolEducationComplaints.Count; j++)
                {
                    try
                    {
                        listDbSchoolEducationComplaints[j].SetUpdated(listHierarchyIdsMapping, listProvinces, listDivisions,
                            listDistricts, listTehsils, listUc);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                RepoDbComplaint.BulkMerge(listDbSchoolEducationComplaints.Select(n=>n.DbComplaint).ToList(), connection);
                connection.Close();
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return false;

        }

        public static bool SynUsers(/*DBContextHelperLinq db,*/ List<DbCrmIdsMappingToOtherSystem> listCrmIdsMapping, List<DbUsers> listUsers, ApiResponseSyncSEModel.ResponseSEDataUsers respnseUsers,
            List<DbProvince> listProvinces, List<DbDivision> listDivisions, List<DbDistrict> listDistricts, List<DbTehsil> listTehsils, List<DbUnionCouncils> listUc)
        {
            SqlConnection connection = new SqlConnection(conStr);
            try
            {
                List<Pair<ApiResponseSyncSEModel.ResponseSEDataUsers.Users, DbCrmIdsMappingToOtherSystem, DbUsers, Config.SyncOperations>> listOverallMapping =
                    new List<Pair<ApiResponseSyncSEModel.ResponseSEDataUsers.Users, DbCrmIdsMappingToOtherSystem, DbUsers, Config.SyncOperations>>();



                DbCrmIdsMappingToOtherSystem dbTempCrmIdsMapping = listCrmIdsMapping.FirstOrDefault();

                List<DbCrmIdsMappingToOtherSystem> listHierarchyMap =
                    RepoDbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, dbTempCrmIdsMapping.OTS_System_Id, (int)Config.CrmModule.Hierarchy);

                int crmModuleId = (int)Config.CrmModule.Users;
                List<DbCrmIdsMappingToOtherSystem> listCrmUserIdsMapping = listCrmIdsMapping.Where(n => n.Crm_Module_Id == crmModuleId).ToList();

                //-------- test code ------

                //respnseUsers.data = respnseUsers.data.Where(n => n.crmu_id == "48782").ToList();
                //-------- end test code -----


                //-------- RecordToUpdate -------
                listOverallMapping = (from respUser in respnseUsers.data
                                      join map in listCrmUserIdsMapping on
                                          Convert.ToInt32(respUser.crmu_id) equals map.OTS_Id
                                      join dbUser in listUsers on
                                          map.Crm_Id equals dbUser.User_Id
                                      select new Pair<ApiResponseSyncSEModel.ResponseSEDataUsers.Users, DbCrmIdsMappingToOtherSystem, DbUsers, Config.SyncOperations>
                                      (
                                        respUser,
                                        map,
                                        dbUser,
                                        Config.SyncOperations.Update
                                      )).ToList();

                //------ RecordsToInsert--------
                listOverallMapping.AddRange(respnseUsers.data.Where(
                    n => !listCrmUserIdsMapping.Select(x => x.OTS_Id).Contains(Convert.ToInt32(n.crmu_id))).ToList().Select(n => new Pair<ApiResponseSyncSEModel.ResponseSEDataUsers.Users, DbCrmIdsMappingToOtherSystem, DbUsers, Config.SyncOperations>
                          (
                            n,
                            null,
                            null,
                            Config.SyncOperations.Add
                          )).ToList());

                //
                List<Pair<DbUsers, ApiResponseSyncSEModel.ResponseSEDataUsers.Users>> listModelToAdd = new List<Pair<DbUsers, ApiResponseSyncSEModel.ResponseSEDataUsers.Users>>();

                DbCrmIdsMappingToOtherSystem tempCrmIdsMap = null;
                Dictionary<Config.Hierarchy, string> dictUsersHierarchyPMIUIds = null;


                List<DbComplaintType> listDbComplaintType =
                    RepoDbComplaintType.GetByDepartmentIds(Utility.GetNullableIntList(RepoDbDepartment.GetByCampaignId((int) Config.Campaign.SchoolEducationEnhanced)
                            .Select(n=>n.Id).ToList()));
                //List<DbUserCategory> listDbUserCategory = listOverallMapping.Select(n=>n.Item3.ListDbUserCategory).ToList();
               // int? provinceId, divisionId, districtId, tehsilId, ucId; 
                bool isUpdated = false;
                //listOverallMapping = listOverallMapping.Where(n => n.Item3!=null &&  n.Item3.User_Id == 1918).ToList();
                foreach (Pair<ApiResponseSyncSEModel.ResponseSEDataUsers.Users, DbCrmIdsMappingToOtherSystem, DbUsers, Config.SyncOperations> overallMap in listOverallMapping)
                {
                    try
                    {

                        if (overallMap.Item4 == Config.SyncOperations.Update)
                        {
                            try
                            {
                                dictUsersHierarchyPMIUIds = GetUsersHierarchyPMIUIds(overallMap.Item3, listHierarchyMap);
                                overallMap.Item3.Id = overallMap.Item3.User_Id;
                                isUpdated = overallMap.Item3.SetUpdated(overallMap.Item1, listHierarchyMap, dictUsersHierarchyPMIUIds,listProvinces,listDivisions,listDistricts,listTehsils,listUc);
                                if (!isUpdated)
                                {
                                    overallMap.Item4 = Config.SyncOperations.NoChange;
                                    overallMap.Item2.Updated_Date = CurrDateTime;
                                }
                            }
                            catch (Exception ex)
                            {
                          
                            }

                        }
                        else if (overallMap.Item4 == Config.SyncOperations.Add)
                        {
                            try
                            {


                                //---- add value in main table
                                DbUsers dbUser = RepoDbUsers.GetDbUser(overallMap.Item1, listHierarchyMap,
                                     listProvinces, listDivisions,
                                    listDistricts, listTehsils, listUc, (int) Config.Campaign.SchoolEducationEnhanced,
                                    listDbComplaintType,
                                    listUsers);
                                listModelToAdd.Add(
                                    new Pair<DbUsers, ApiResponseSyncSEModel.ResponseSEDataUsers.Users>(dbUser,
                                        overallMap.Item1));
                                //---- add value in mapping table
                            }
                            catch (Exception ex)
                            {
                            
                            }

                        }
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }


                //-- Bulk Merge --------
                List<DbUsers> listDbUsersToMerge = listOverallMapping.Where(n => n.Item4 == Config.SyncOperations.Update).Select(n => n.Item3).ToList();
                RepoDbUsers.BulkMerge(listDbUsersToMerge, connection, false); // is updating
                
                List<DbUsers> listDbUsersToAdd = listModelToAdd.Select(x => x.Item1).ToList();
                listDbUsersToMerge.AddRange(listDbUsersToAdd);
                RepoDbUsers.BulkMerge(listDbUsersToAdd, connection, true); // is adding
    
                //DbUsers.BulkMerge(listDbUsersToMerge, connection);

                ////---- End Bulk Insertion --------
                dbTempCrmIdsMapping = listCrmUserIdsMapping.FirstOrDefault();
                List<DbCrmIdsMappingToOtherSystem> listDbCrmMappingToOtherSystem = new ListStack<DbCrmIdsMappingToOtherSystem>();
                DbCrmIdsMappingToOtherSystem dbCrmIdsMappingToOtherSystem = null;
                foreach (Pair<DbUsers, ApiResponseSyncSEModel.ResponseSEDataUsers.Users> modelToAdd in listModelToAdd)
                {
                    dbCrmIdsMappingToOtherSystem = RepoDbCrmIdsMappingToOtherSystem.GetModel(dbTempCrmIdsMapping,
                    modelToAdd.Item1.Id, Convert.ToInt32(modelToAdd.Item2.crmu_id), modelToAdd.Item2.is_Active, CurrDateTime);

                    listDbCrmMappingToOtherSystem.Add(dbCrmIdsMappingToOtherSystem);
                }
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                RepoDbCrmIdsMappingToOtherSystem.BulkMerge(listDbCrmMappingToOtherSystem, connection);
                
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            

            }
            catch (Exception)
            {

              
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return false;

        }

        private static Dictionary<Config.Hierarchy, string> GetUsersHierarchyPMIUIds(DbUsers dbUser, List<DbCrmIdsMappingToOtherSystem> listDbCrmIdsMappingToOtherSystems )
        {
            Dictionary<Config.Hierarchy, string> listMapCrmUser = new Dictionary<Config.Hierarchy, string>();
            listMapCrmUser.Add(Config.Hierarchy.Province, dbUser.Province_Id);
            listMapCrmUser.Add(Config.Hierarchy.Division, dbUser.Division_Id);
            listMapCrmUser.Add(Config.Hierarchy.District, dbUser.District_Id);
            listMapCrmUser.Add(Config.Hierarchy.Tehsil, dbUser.Tehsil_Id);
            listMapCrmUser.Add(Config.Hierarchy.UnionCouncil, dbUser.UnionCouncil_Id);
            
            DbCrmIdsMappingToOtherSystem tempCrmIdsMap = null;

            Dictionary<Config.Hierarchy, string> listMapPMIUUser = new Dictionary<Config.Hierarchy, string>();
            
            foreach (KeyValuePair<Config.Hierarchy, string> mapCrmUser in listMapCrmUser)
            {
                if (!string.IsNullOrEmpty(mapCrmUser.Value))
                {
                    tempCrmIdsMap =
                        listDbCrmIdsMappingToOtherSystems.Where(
                            n =>
                                n.Crm_Module_Cat1 == (int) mapCrmUser.Key &&
                                Utility.GetNullableIntList(mapCrmUser.Value).Contains(n.Crm_Id))
                            .FirstOrDefault();

                    if (tempCrmIdsMap != null)
                    {
                        listMapPMIUUser.Add(mapCrmUser.Key, tempCrmIdsMap.OTS_Id.ToString());
                    }
                    else
                    {
                        listMapPMIUUser.Add(mapCrmUser.Key, null);
                    }
                }
                else
                {
                    listMapPMIUUser.Add(mapCrmUser.Key, null);
                }
            }
            return listMapPMIUUser;
        }


        private static ApiResponseSyncSEModel.SyncSEData GetSyncSeDataApiResponse(ApiReqSyncSEModel.SyncSEData syncData)
        {
            try
            {
                var responseString = "";
                //----- new code---
                using (var client = new HttpClient())
                {
                    //client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("sec_key", syncData.sec_key),
                        new KeyValuePair<string, string>("system_type", syncData.system_type),
                        new KeyValuePair<string, string>("data_required", syncData.data_required),
                        new KeyValuePair<string, string>("start_date", syncData.start_date),
                        new KeyValuePair<string, string>("end_date", syncData.end_date)

                    };


                    var content = new FormUrlEncodedContent(pairs);


                    var response =
                        client.PostAsync(
                            Config.PMIU_API_DATA, content);
                    responseString = response.Result.Content.ReadAsStringAsync().Result;

                }

                if (syncData.data_required == Config.SyncApiRequiredData.districts.ToString())
                {
                    return JsonConvert.DeserializeObject<ApiResponseSyncSEModel.ResponseSEDataDistrict>(responseString);
                }
                else if (syncData.data_required == Config.SyncApiRequiredData.tehsils.ToString())
                {
                    return JsonConvert.DeserializeObject<ApiResponseSyncSEModel.ResponseSEDataTehsil>(responseString);
                }
                else if (syncData.data_required == Config.SyncApiRequiredData.markazes.ToString())
                {
                    ApiResponseSyncSEModel.ResponseSEDataMarkaz markazResp = JsonConvert.DeserializeObject<ApiResponseSyncSEModel.ResponseSEDataMarkaz>(responseString);
                    /*var grp = markazResp.data.GroupBy(n => n.markaz_id).Where(n=>n.Count()>1).ToList();
                    foreach (var val in grp)
                    {
                        List<ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz> listMark = markazResp.data.Where(n => n.markaz_id == val.Key).OrderBy(n => n.updated_at).ToList();
                        for (int i = 0; i < listMark.Count-1; i++)
                        {
                            markazResp.data.Remove(listMark[i]);
                        }
                    }*/
                    return markazResp;
                }
                else if (syncData.data_required == Config.SyncApiRequiredData.school.ToString())
                {
                    return JsonConvert.DeserializeObject<ApiResponseSyncSEModel.ResponseSEDataSchool>(responseString);
                }
            }
            catch (Exception ex)
            {

                return null;
            }

            return null;
        }

        private static ApiResponseSyncSEModel.ResponseSEDataUsers GetSyncSeUsersDataApiResponse(ApiReqSyncSEModel.SyncSEUsersData syncData)
        {
            try
            {
                var responseString = "";
                //----- new code---
                using (var client = new HttpClient())
                {
                    client.Timeout = System.TimeSpan.FromSeconds(1000);
                    //client. = 90;
                    //client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("sec_key", syncData.sec_key),
                        new KeyValuePair<string, string>("system_type", syncData.system_type),
                        new KeyValuePair<string, string>("user_type", syncData.user_type),
                        new KeyValuePair<string, string>("user_id", syncData.user_id),
                        new KeyValuePair<string, string>("start_date", syncData.start_date),
                        new KeyValuePair<string, string>("end_date", syncData.end_date)

                    };


                    var content = new FormUrlEncodedContent(pairs);


                    var response =
                        client.PostAsync(
                            Config.PMIU_API_USERS_DATA, content);
                    responseString = response.Result.Content.ReadAsStringAsync().Result;

                }
                return JsonConvert.DeserializeObject<ApiResponseSyncSEModel.ResponseSEDataUsers>(responseString);
               
            }
            catch (Exception ex)
            {

                return null;
            }

            return null;
        }

        public class UserData
        {
            public int UserId { get; set; }
            public string UserName { get; set; }

            public string Password { get; set; }

            public int ProvinceId { get; set; }

            public int DivisionId { get; set; }

            public int DistrictId { get; set; }

            public int TehsilId { get; set; }

            public int UcId { get; set; }

            public bool IsActive { get; set; }
        }

        //public static List<DbUsers> GetUsers(int campaignId, string userType, DateTime from, DateTime to)
        //{
        //    List<DbCrmIdsMappingToOtherSystem> listCrmHierarchyIdsMap = DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy);
            
        //    Config.Hierarchy? configHierarchy = null;
        //    List<int?> listUserCategoryId1 = new List<int?>{(int)Config.SchoolType.Primary, (int) Config.SchoolType.Elementary};

        //    if (userType == "Aeo")
        //    {
        //        configHierarchy = Config.Hierarchy.UnionCouncil;
        //        //listUserCategoryId1 = 
        //    }
        //    List<DbUsers> listDbUsers = DbUsers.GetUser(campaignId, configHierarchy, listUserCategoryId1, from, to);
        //    return listDbUsers;
            
        //}
    }
}