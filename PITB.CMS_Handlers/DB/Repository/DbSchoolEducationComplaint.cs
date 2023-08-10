using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Handlers.DB.Repository
{
    public class RepoDbSchoolEducationComplaint//: DbComplaint
    {
        public bool SetUpdated(/*DbSchoolsMapping dbSchoolsMapping,*/ List<DbCrmIdsMappingToOtherSystem> listHierarchyMapping, List<DbProvince> listProvince, List<DbDivision> listDivision, List<DbDistrict> listDistrict, List<DbTehsil> listTehsil, List<DbUnionCouncils> listUc)
        {
            try
            {


                //---- Assign Region ------
                if (DbSchoolMapping.System_Markaz_Id != DbComplaint.UnionCouncil_Id || DbSchoolMapping.School_Type != Config.SchoolTypeDict[DbComplaint.RefField4] ||
                    DbSchoolMapping.System_School_Gender != Convert.ToBoolean(Config.SchoolGenderDict[DbComplaint.RefField5])) // if markaz doesnt match
                {
                    DbComplaint.UnionCouncil_Id = DbSchoolMapping.System_Markaz_Id;
                    DbComplaint.UnionCouncil_Name = listUc.Where(n => n.UnionCouncil_Id == DbComplaint.UnionCouncil_Id).FirstOrDefault().Councils_Name;

                    if (DbSchoolMapping.System_Tehsil_Id != DbComplaint.Tehsil_Id) // if tehsil doesnt match
                    {
                        DbComplaint.Tehsil_Id = DbSchoolMapping.System_Tehsil_Id;
                        DbComplaint.Tehsil_Name =
                            listTehsil.Where(n => n.Tehsil_Id == DbComplaint.Tehsil_Id).FirstOrDefault().Tehsil_Name;

                        if (DbSchoolMapping.System_District_Id != DbComplaint.District_Id) // if district doesnt match
                        {
                            DbComplaint.District_Id = DbSchoolMapping.System_District_Id;

                            DbDistrict dbDistrict =
                                listDistrict.Where(n => n.District_Id == DbComplaint.District_Id).FirstOrDefault();


                            DbComplaint.District_Name = dbDistrict.District_Name;

                            if (dbDistrict.Division_Id != DbComplaint.Division_Id) // if division doesnt match
                            {
                                DbComplaint.Division_Id = dbDistrict.Division_Id;

                                DbDivision dbDivision = listDivision.Where(n => n.Division_Id == DbComplaint.Division_Id).FirstOrDefault();

                                DbComplaint.Division_Name = dbDivision.Division_Name;

                                if (dbDivision.Province_Id != DbComplaint.Province_Id) // if province doesnt match
                                {
                                    DbComplaint.Province_Id = dbDivision.Province_Id;

                                    DbProvince dbProvince = listProvince.Where(n => n.Province_Id == DbComplaint.Province_Id).FirstOrDefault();

                                    DbComplaint.Province_Name = dbProvince.Province_Name;
                                }
                            }
                        }


                    }
                    if (DbComplaint.Complaint_Type == Config.ComplaintType.Complaint)
                    {
                        BlSchool.ReEvaluateEscallation(DbComplaint, DbSchoolMapping);
                        //---- Origin complaints assignment done
                        using (DBContextHelperLinq db = new DBContextHelperLinq())
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            BlSchool.SaveOrignalHierarchyLogInDb(db, DbComplaint);
                            db.SaveChanges();
                            db.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    /*
                    BlSchool.ReEvaluateEscallation(this, dbSchoolsMapping);

                    if (dbSchoolsMapping.School_Type != Config.SchoolTypeDict[this.RefField4] ||
                    dbSchoolsMapping.System_School_Gender != Convert.ToBoolean(Config.SchoolGenderDict[this.RefField5]))
                    {
                        BlSchool.ReEvaluateEscallation(this, dbSchoolsMapping);
                    }
                    else
                    {
                        return false;
                    }*/
                }

                // If school Info Has been changed
                //if(!this.RefField1.Equals(dbSchoolsMapping.school_emis_code) 
                //    || !this.RefField2.Equals(dbSchoolsMapping.school_name)
                //    || !this.RefField3.Equals(dbSchoolsMapping.School_Type)
                //    || !this.RefField5.Equals(dbSchoolsMapping.school_gender)
                //{
                //    Convert.ToInt16(Config.SchoolTypeMapDict[dbSchoolsMapping.school_level])
                //}
                DbComplaint.RefField1 = DbSchoolMapping.school_emis_code;
                DbComplaint.RefField2 = DbSchoolMapping.school_name;
                DbComplaint.RefField3 = DbSchoolMapping.school_level;
                DbComplaint.RefField4 = Config.SchoolTypeDict.FirstOrDefault(x => x.Value == DbSchoolMapping.School_Type).Key;
                DbComplaint.RefField5 = Config.SchoolGenderDict.FirstOrDefault(x => x.Value == Convert.ToInt32(DbSchoolMapping.System_School_Gender)).Key;
                DbComplaint.RefField6 = DbSchoolMapping.markaz_name;

            }
            catch (Exception)
            {

                throw;
            }
            // If schoolType and gender has been changed


            return false;
        }
    }
}