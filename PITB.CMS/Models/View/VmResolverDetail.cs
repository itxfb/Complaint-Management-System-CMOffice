using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Models.View
{
    public class VmResolverDetail
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public string Cnic { get; set; }

        //[StringLength(50)]
        public string Email { get; set; }

        public string Address { get; set; }

        public string Designation { get; set; }

        public string DesignationAbbr { get; set; }

        public string ProvinceName { get; set; }

        public string DistrictName { get; set; }

        public string DivisionName { get; set; }

        public string TehsilName { get; set; }

        public string UnionCouncilName { get; set; }

        public string WardName { get; set; }


        public VmResolverDetail (int resolverId)
        {
            DbUsers dbUser = DbUsers.GetUser(resolverId);
            this.Name = Utility.GetStrForDisplay(dbUser.Name);
            this.Phone = Utility.GetStrForDisplay(dbUser.Phone);
            this.Email = Utility.GetStrForDisplay(dbUser.Email);
            this.Cnic = Utility.GetStrForDisplay(dbUser.Cnic);
            this.Address = Utility.GetStrForDisplay(dbUser.Address);
            this.DesignationAbbr = Utility.GetStrForDisplay(dbUser.Designation_abbr);
            int provinceId = Convert.ToInt32(dbUser.Province_Id);
            int districtId = Convert.ToInt32(dbUser.District_Id);
            int tehsilId = Convert.ToInt32(dbUser.Tehsil_Id);
            int ucId = Convert.ToInt32(dbUser.UnionCouncil_Id);

            this.ProvinceName = (provinceId!=0) ? DbProvince.GetById(provinceId).Province_Name : Config.None;
            this.DistrictName = (districtId != 0) ? DbDistrict.GetById(districtId).District_Name : Config.None;
            this.TehsilName = (tehsilId != 0) ? DbTehsil.GetById(tehsilId).Tehsil_Name : Config.None;
            this.UnionCouncilName = (ucId != 0) ? DbUnionCouncils.GetById(ucId).Councils_Name : Config.None;
        }
    
    }

  
}