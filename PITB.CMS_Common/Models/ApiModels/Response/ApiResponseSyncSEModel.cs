using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Amazon.S3.Model;

namespace PITB.CMS_Common.Models.ApiModels.Response
{
    public class ApiResponseSyncSEModel
    {
        public class SyncSEData
        {
            public string status { get; set; }
        }

        public class SynInnerModelBase
        {
            public string status { get; set; }

            public bool is_Active
            {
                get
                {
                    if (status == "0")
                    {
                        return false;
                    }
                    else if (status == "1")
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public class ResponseSEDataDistrict : SyncSEData
        {
            public List<District> data { get; set; }
            public class District : SynInnerModelBase
            {

                public string district_id { get; set; }

                public string district_name { get; set; }



                public DateTime? created_at { get; set; }

                public DateTime? updated_at { get; set; }
            }
        }

        public class ResponseSEDataTehsil : SyncSEData
        {
            public List<Tehsil> data { get; set; }
            public class Tehsil : SynInnerModelBase
            {
                public string tehsil_id { get; set; }

                public string tehsil_name { get; set; }

                public string district_id_Fk { get; set; }

                //public string status { get; set; }

                public DateTime? created_at { get; set; }

                public DateTime? updated_at { get; set; }
            }
        }


        public class ResponseSEDataMarkaz : SyncSEData
        {
            public List<Markaz> data { get; set; }
            public class Markaz : SynInnerModelBase
            {
                public string markaz_id { get; set; }

                public string markaz_name { get; set; }

                public string markaz_gender { get; set; }

                public string district_id_Fk { get; set; }

                public string tehsil_id_Fk { get; set; }

                //public string status { get; set; }

                //public string status { get; set; }

                public DateTime? created_at { get; set; }

                public DateTime? updated_at { get; set; }
            }
        }

        public class ResponseSEDataSchool : SyncSEData
        {
            public List<School> data { get; set; }
            public class School : SynInnerModelBase
            {
                public string school_id { get; set; }

                public string school_emis_code { get; set; }

                public string school_name { get; set; }

                public string school_district { get; set; }

                public string school_tehsil { get; set; }


                public string school_markaz { get; set; }


                public string school_level { get; set; }

                //---- new fields


                public string school_head_name { get; set; }

                public string school_head_designation { get; set; }

                public string school_head_phone { get; set; }

                public string school_gender { get; set; }
                //---- 

                //public string status { get; set; }

                //public string status { get; set; }

                public DateTime? created_at { get; set; }

                public DateTime? updated_at { get; set; }

                public bool gender
                {
                    get
                    {
                        if (school_gender == "Male")
                        {
                            return true;
                        }
                        else if (school_gender == "Female")
                        {
                            return false;
                        }

                        return false;
                    }
                }
            }
        }

        public class ResponseSEDataUsers
        {
            public List<Users> data { get; set; }

            public class Users 
            {
                public string crmu_id { get; set; }
                public string crmu_province_id_Fk { get; set; }
                public string crmu_division_id_Fk { get; set; }
                public string crmu_district_id_Fk { get; set; }
                public string crmu_tehsil_id_Fk { get; set; }
                public string crmu_markaz_id_Fk { get; set; }
                public string crmu_name { get; set; }
                public string crmu_cnic { get; set; }
                public string crmu_phone { get; set; }
                public string crmu_email { get; set; }
                public string crmu_type { get; set; }
                public string crmu_designation { get; set; }
                public string crmu_designation_abbreviation { get; set; }
                public string crmu_hierarchy_id { get; set; }
                public string crmu_user_hierarchy_id { get; set; }
                public string crmu_user_category_id1 { get; set; }
                public string crmu_user_category_id2 { get; set; }
                public string crmu_status { get; set; }
                public DateTime? crmu_updated_at { get; set; }
                public string crmu_created_by_Fk { get; set; }
                public DateTime? crmu_created_at { get; set; }
                public string crmu_updated_by_Fk { get; set; }
             
                public bool is_Active
                {
                    get
                    {
                        if (crmu_status == "0")
                        {
                            return false;
                        }
                        else if (crmu_status == "1")
                        {
                            return true;
                        }

                        return false;
                    }
                }
        
            }
        }


    }
}