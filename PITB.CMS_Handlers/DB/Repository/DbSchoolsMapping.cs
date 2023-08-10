using System.Data.Entity;
using System.Linq;
using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using PITB.CMS_Models.ApiModels.Response;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbSchoolsMapping
    {
        #region HelperMethods
        public static DbSchoolsMapping GetById(int id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => m.Id == id && m.Is_Active == true).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DbSchoolsMapping GetSchoolById(int id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbSchoolsMapping> GetListByIds(List<int> listId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => listId.Contains(m.Id)).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbSchoolsMapping> GeAll()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbSchoolsMapping.AsNoTracking().Include(n => n.dbHeadMapping).Select(m => m).ToList();
                    //return db.DbSchoolsMapping.AsNoTracking().Select(m => m).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbSchoolsMapping> GetByEmisCode(List<string> listEmisCode)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => listEmisCode.Contains(m.school_emis_code)).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbSchoolsMapping GetByEmisCode(string emisCode)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => m.school_emis_code == emisCode).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbSchoolsMapping> GeAll(DBContextHelperLinq db)
        {
            try
            {

                return db.DbSchoolsMapping.Include(n => n.dbHeadMapping).Select(m => m).ToList();


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static DbSchoolsMapping GetBy(int district int otherSystemSchoolId, int schoolCategory)
        //{
        //    try
        //    {
        //        using (var db = new DBContextHelperLinq())
        //        {
        //            return db.DbSchoolsMapping.AsNoTracking().Where(m => m.school_emis_code == "331103720" && m.School_Category == schoolCategory).FirstOrDefault();
        //            //return db.DbSchoolsMapping.AsNoTracking().Where(m => m.PMIU_School_Id == otherSystemSchoolId && m.School_Category == schoolCategory && m.Is_Active==true).FirstOrDefault();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public static DbSchoolsMapping GetByOtherSystemSchoolId(int districtId, int tehsilId, int otherSystemSchoolId, int schoolCategory)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    //return db.DbSchoolsMapping.AsNoTracking().Where(m => m.school_emis_code == "331103720" && m.School_Category== schoolCategory).FirstOrDefault();
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => m.System_Tehsil_Id == tehsilId && m.System_District_Id == districtId && m.PMIU_School_Id == otherSystemSchoolId && m.School_Category == schoolCategory && m.Is_Active == true).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbSchoolsMapping GetById(int? userCat1, bool userCat2, int? markazId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => m.School_Type == userCat1 && m.System_School_Gender == userCat2 && m.System_Markaz_Id == markazId).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SetUpdated(List<DbSchoolEducationHeadMapping> listSchoolEducationHeadMappings, ApiResponseSyncSEModel.ResponseSEDataSchool.School syncSchool, List<DbCrmIdsMappingToOtherSystem> listHierarchyMapping, List<DbDistrict> listDistricts, List<DbTehsil> listTehsils, List<DbUnionCouncils> listUc)
        {
            bool isUpdated = false;

            if (syncSchool.school_name != this.school_name
                || Convert.ToInt32(syncSchool.school_district) != this.District_Id
                || Convert.ToInt32(syncSchool.school_tehsil) != this.Tehsil_Id
                || Convert.ToInt32(syncSchool.school_markaz) != this.UnionCouncil_Id
                || syncSchool.school_emis_code != this.school_emis_code
                || syncSchool.gender != this.System_School_Gender
                || syncSchool.school_level != this.school_level
                || syncSchool.is_Active != this.Is_Active
                )
            {
                //db.DbSchoolsMapping.Attach(this);

                if (Convert.ToInt32(syncSchool.school_district) != this.District_Id)
                {
                    this.District_Id = Convert.ToInt32(syncSchool.school_district);

                    this.System_District_Id = listHierarchyMapping.Where(
                        n => n.Crm_Module_Cat1 == (int)Config.Hierarchy.District && n.OTS_Id == this.District_Id)
                        .FirstOrDefault()
                        .Crm_Id;

                    this.district_name = listDistricts.Where(n => n.District_Id == this.System_District_Id).FirstOrDefault().District_Name;

                    //db.Entry(this).Property(n => n.District_Id).IsModified = true;
                    //db.Entry(this).Property(n => n.System_District_Id).IsModified = true;
                    //db.Entry(this).Property(n => n.district_name).IsModified = true;

                }

                if (Convert.ToInt32(syncSchool.school_tehsil) != this.Tehsil_Id)
                {
                    this.Tehsil_Id = Convert.ToInt32(syncSchool.school_tehsil);

                    this.System_Tehsil_Id = listHierarchyMapping.Where(
                        n => n.Crm_Module_Cat1 == (int)Config.Hierarchy.Tehsil && n.OTS_Id == this.Tehsil_Id)
                        .FirstOrDefault()
                        .Crm_Id;

                    this.tehsil_name = listTehsils.Where(n => n.Tehsil_Id == this.System_Tehsil_Id).FirstOrDefault().Tehsil_Name;

                    //db.Entry(this).Property(n => n.Tehsil_Id).IsModified = true;
                    //db.Entry(this).Property(n => n.System_Tehsil_Id).IsModified = true;
                    //db.Entry(this).Property(n => n.tehsil_name).IsModified = true;
                }

                if (Convert.ToInt32(syncSchool.school_markaz) != this.UnionCouncil_Id)
                {
                    this.UnionCouncil_Id = Convert.ToInt32(syncSchool.school_markaz);

                    this.System_Markaz_Id = listHierarchyMapping.Where(
                        n => n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil && n.OTS_Id == this.UnionCouncil_Id)
                        .FirstOrDefault()
                        .Crm_Id;

                    this.markaz_name = listUc.Where(n => n.UnionCouncil_Id == this.System_Markaz_Id).FirstOrDefault().Councils_Name;

                    //db.Entry(this).Property(n => n.UnionCouncil_Id).IsModified = true;
                    //db.Entry(this).Property(n => n.System_Markaz_Id).IsModified = true;
                    //db.Entry(this).Property(n => n.markaz_name).IsModified = true;
                }


                if (syncSchool.school_name != this.school_name)
                {
                    this.school_name = syncSchool.school_name;
                }

                if (syncSchool.school_emis_code != this.school_emis_code)
                {
                    this.school_emis_code = syncSchool.school_emis_code;
                    //db.Entry(this).Property(n => n.school_emis_code).IsModified = true;
                }



                if (syncSchool.school_level != this.school_level)
                {
                    this.school_level = syncSchool.school_level;
                    this.School_Type = Convert.ToInt16(Config.SchoolTypeMapDict[this.school_level]);
                    //db.Entry(this).Property(n => n.school_level).IsModified = true;
                    //db.Entry(this).Property(n => n.School_Type).IsModified = true;
                }
                if (syncSchool.gender != this.System_School_Gender)
                {
                    this.System_School_Gender = syncSchool.gender;
                    //db.Entry(this).Property(n => n.System_School_Gender).IsModified = true;
                }
                if (syncSchool.is_Active != this.Is_Active)
                {
                    this.Is_Active = syncSchool.is_Active;
                    //db.Entry(this).Property(n => n.Is_Active).IsModified = true;
                }

                isUpdated = true;

            }

            if (this.dbHeadMapping == null || (syncSchool.school_head_name != this.dbHeadMapping.School_Head_Name
                || syncSchool.school_head_designation != this.dbHeadMapping.School_Head_Designation
                || syncSchool.school_head_phone != this.dbHeadMapping.School_Head_PhoneNo))
            {
                DbSchoolEducationHeadMapping dbSchoolHeadMapping = (this.dbHeadMapping == null) ? null : listSchoolEducationHeadMappings.Where(n => n.school_emis_code == this.school_emis_code).FirstOrDefault();
                if (dbSchoolHeadMapping == null)
                {
                    dbSchoolHeadMapping = new DbSchoolEducationHeadMapping();
                    dbSchoolHeadMapping.school_emis_code = this.school_emis_code;
                    dbSchoolHeadMapping.School_Head_Name = syncSchool.school_head_name;
                    dbSchoolHeadMapping.School_Head_PhoneNo = syncSchool.school_head_phone;
                    dbSchoolHeadMapping.School_Head_Designation = syncSchool.school_head_designation;
                    this.dbHeadMapping = dbSchoolHeadMapping;
                    //db.DbSchoolEducationHeadMapping.Add(dbSchoolHeadMapping);
                }
                else
                {
                    if (syncSchool.school_head_name != this.dbHeadMapping.School_Head_Name)
                    {
                        this.dbHeadMapping.School_Head_Name = syncSchool.school_head_name;
                        dbSchoolHeadMapping.School_Head_Name = this.dbHeadMapping.School_Head_Name;
                        //db.Entry(dbSchoolHeadMapping).Property(n => n.School_Head_Name).IsModified = true;
                    }

                    if (syncSchool.school_head_designation != this.dbHeadMapping.School_Head_Designation)
                    {
                        this.dbHeadMapping.School_Head_Designation = syncSchool.school_head_designation;
                        dbSchoolHeadMapping.School_Head_Designation = this.dbHeadMapping.School_Head_Designation;
                        //db.Entry(dbSchoolHeadMapping).Property(n => n.School_Head_Designation).IsModified = true;
                    }

                    if (syncSchool.school_head_phone != this.dbHeadMapping.School_Head_PhoneNo)
                    {
                        this.dbHeadMapping.School_Head_PhoneNo = syncSchool.school_head_phone;
                        dbSchoolHeadMapping.School_Head_PhoneNo = this.dbHeadMapping.School_Head_PhoneNo;
                        //db.Entry(dbSchoolHeadMapping).Property(n => n.School_Head_PhoneNo).IsModified = true;
                    }
                }
                isUpdated = true;
            }
            return isUpdated;
        }

        public static DbSchoolsMapping GetDbSchoolMapping(/*DBContextHelperLinq db,*/ ApiResponseSyncSEModel.ResponseSEDataSchool.School syncSchool, List<DbCrmIdsMappingToOtherSystem> listHierarchyMapping, List<DbDistrict> listDistricts, List<DbTehsil> listTehsils, List<DbUnionCouncils> listUc)
        {
            if (syncSchool.school_markaz != "0" && syncSchool.school_tehsil != "0" && syncSchool.school_district != "0")
            {

                DbSchoolsMapping dbSchoolMap = new DbSchoolsMapping();

                DbSchoolEducationHeadMapping dbSchoolHeadMapping = new DbSchoolEducationHeadMapping();

                dbSchoolHeadMapping.school_emis_code = syncSchool.school_emis_code;

                dbSchoolHeadMapping.School_Head_Name = syncSchool.school_head_name;
                dbSchoolHeadMapping.School_Head_Designation = syncSchool.school_head_designation;
                if (syncSchool.school_head_phone != null)
                {
                    dbSchoolHeadMapping.School_Head_PhoneNo = syncSchool.school_head_phone; //.Trim();
                }

                dbSchoolMap.dbHeadMapping = dbSchoolHeadMapping;


                dbSchoolMap.school_emis_code = syncSchool.school_emis_code;

                dbSchoolMap.school_name = syncSchool.school_name;


                dbSchoolMap.school_level = syncSchool.school_level;

                dbSchoolMap.school_gender = syncSchool.school_gender;

                //---------- Update District -----------
                dbSchoolMap.District_Id = Convert.ToInt32(syncSchool.school_district);

                dbSchoolMap.System_District_Id = listHierarchyMapping.Where(
                    n => n.Crm_Module_Cat1 == (int)Config.Hierarchy.District && n.OTS_Id == dbSchoolMap.District_Id)
                    .FirstOrDefault()
                    .Crm_Id;

                dbSchoolMap.district_name =
                    listDistricts.Where(n => n.District_Id == dbSchoolMap.System_District_Id)
                        .FirstOrDefault()
                        .District_Name;


                //--------- Update Tehsil -------------
                dbSchoolMap.Tehsil_Id = Convert.ToInt32(syncSchool.school_tehsil);

                dbSchoolMap.System_Tehsil_Id = listHierarchyMapping.Where(
                    n => n.Crm_Module_Cat1 == (int)Config.Hierarchy.Tehsil && n.OTS_Id == dbSchoolMap.Tehsil_Id)
                    .FirstOrDefault()
                    .Crm_Id;

                dbSchoolMap.tehsil_name =
                    listTehsils.Where(n => n.Tehsil_Id == dbSchoolMap.System_Tehsil_Id).FirstOrDefault().Tehsil_Name;


                //-------- Update Uc -----------------
                dbSchoolMap.UnionCouncil_Id = Convert.ToInt32(syncSchool.school_markaz);

                dbSchoolMap.System_Markaz_Id = listHierarchyMapping.Where(
                    n =>
                        n.Crm_Module_Cat1 == (int)Config.Hierarchy.UnionCouncil &&
                        n.OTS_Id == dbSchoolMap.UnionCouncil_Id)
                    .FirstOrDefault()
                    .Crm_Id;

                dbSchoolMap.markaz_name =
                    listUc.Where(n => n.UnionCouncil_Id == dbSchoolMap.System_Markaz_Id).FirstOrDefault().Councils_Name;


                dbSchoolMap.System_School_Gender = syncSchool.gender;

                dbSchoolMap.School_Type = Convert.ToInt16(Config.SchoolTypeMapDict[dbSchoolMap.school_level]);


                dbSchoolMap.PMIU_School_Id = Convert.ToInt32(syncSchool.school_id);

                dbSchoolMap.Is_Active = syncSchool.is_Active;

                return dbSchoolMap;
            }
            else
            {
                return null;
            }
        }


        public static bool BulkMerge(List<DbSchoolsMapping> listToMerge, SqlConnection con)
        {
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
            //con.Open();
            BulkOperation<DbSchoolsMapping> bulkOp = new BulkOperation<DbSchoolsMapping>(con);
            bulkOp.BatchSize = 1000;
            bulkOp.ColumnInputExpression = c => new
            {
                c.school_emis_code,
                c.school_name,
                c.school_level,
                c.school_gender,
                c.District_Id,
                c.district_name,
                c.Tehsil_Id,
                c.tehsil_name,
                c.UnionCouncil_Id,
                c.markaz_name,
                c.System_District_Id,
                c.System_Tehsil_Id,
                c.System_Markaz_Id,
                c.System_School_Gender,
                c.School_Type,
                c.Is_Active
            };
            bulkOp.DestinationTableName = "dbo.Schools_Mapping";
            bulkOp.ColumnOutputExpression = c => c.Id;
            bulkOp.ColumnPrimaryKeyExpression = c => c.Id;
            bulkOp.BulkMerge(listToMerge);
            return true;
        }


        #endregion
    }
}
