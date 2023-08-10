using System.Linq;
using AutoMapper;
using PITB.CMS_Common.Models.View;
using System.Collections.Generic;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.Business
{
    public class BlPersonInformation
    {
        public static VmPersonalInfo GetPersonalInfoByCnic(string cnicNo)
        {
           
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            return Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetListByCnic((string.IsNullOrEmpty(cnicNo)) ? "0" : cnicNo).FirstOrDefault());

            //AutoMapper.Mapper.CreateMap<DbCampaign, VmCampaign>();
            //var list = AutoMapper.Mapper.Map<List<VmCampaign>>(DbCampaign.All());
        }
        public static VmPersonalInfo GetPersonalInfoByCellNumer(string cellNumber)
        {
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            return Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetListByMobileNo((string.IsNullOrEmpty(cellNumber)) ? "0" : cellNumber).FirstOrDefault());
        }

        public static VmPersonalInfo GetPersonalInfoByComplaintNo(string complaintNo)
        {
            
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            return Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetListByComplaintId((string.IsNullOrEmpty(complaintNo)) ? "0" : complaintNo).FirstOrDefault());
        }
        public static List<VmPersonalInfo> GetPersonalInfoListByCellNumer(string cellNumber)
        {
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            return Mapper.Map<List<VmPersonalInfo>>(DbPersonInformation.GetListByMobileNo((string.IsNullOrEmpty(cellNumber)) ? "0" : cellNumber));
        }
    }
}