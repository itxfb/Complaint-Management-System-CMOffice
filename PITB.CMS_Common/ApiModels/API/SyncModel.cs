using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace PITB.CMS_Common.ApiModels.API
{
    public class SyncModel: ApiStatus
    {
        public List<DbProvince> ListProvince { get; set; }

        public List<DbDistrict> ListDistrict { get; set; }

        public List<DbTehsil> ListTehsil { get; set; }

        public List<DbUnionCouncils> ListUnionCouncils { get; set; }

        public List<DbWards> ListWards { get; set; }

        public List<DbComplaintType> ListCategory { get; set; }

        public List<DbComplaintSubType> ListSubCategory { get; set; }

        public string Cnic { get; set; }

        public int DbVersionId { get; set; }

       // public List<Picture> ListImages { get; set; }

        public SyncModel()
        {
            this.ListProvince = new List<DbProvince>();
            this.ListDistrict = new List<DbDistrict>();
            this.ListTehsil = new List<DbTehsil>();
            this.ListUnionCouncils = new List<DbUnionCouncils>();
            this.ListWards = new List<DbWards>();
            this.ListCategory = new List<DbComplaintType>();
            this.ListSubCategory = new List<DbComplaintSubType>();
        }
    }
}