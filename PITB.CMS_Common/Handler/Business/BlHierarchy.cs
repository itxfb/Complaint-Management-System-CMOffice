using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlHierarchy
    {
        public static string GetRegionValueAgainstHierarchy(Config.Hierarchy hierarchy, string hierarchyValue)
        {
            switch (hierarchy)
            {
                case Config.Hierarchy.Province:
                    return DbProvince.GetByProvinceIdsStr(hierarchyValue);
                    break;
                case Config.Hierarchy.Division:
                    return DbDivision.GetByDivisionIdsStr(hierarchyValue);
                    break;
                case Config.Hierarchy.District:
                    return DbDistrict.GetByDistrictIdsStr(hierarchyValue);
                    break;
                case Config.Hierarchy.Tehsil:
                    return DbTehsil.GetByTehsilIdsStr(hierarchyValue);
                    break;
                case Config.Hierarchy.UnionCouncil:
                    return DbUnionCouncils.GetByUcIdsStr(hierarchyValue);
                    break;
            }
            return "";
        }
    }
}