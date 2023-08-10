using System.Data;
using System.Web.Configuration;
using Amazon.DataPipeline.Model;
using Newtonsoft.Json;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Handlers.Location
{

    public class LocationHandler
    {
        private const double EarthRadius = 3959;//3437.670013352;

        private static Dictionary<int, List<LocationMapping>> _hirerchyAndTheirMappings = new Dictionary<int, List<LocationMapping>>();
        public static List<LocationMapping> GetAllAboveHirerchyByHirerchyIdAndType(LocationMapping location)
        {
            List<LocationMapping> listMappings = new List<LocationMapping>();
            listMappings.Add(location);
            var allUpperHierarchies = Enum.GetValues(typeof(Config.Hierarchy))
                .Cast<Config.Hierarchy>()
                .Where(e => (int)e < (int)location.Hierarchy).OrderByDescending(e => (int)e)
                .ToList();


            int childElementId = 0;
            for (int i = 0; i < allUpperHierarchies.Count; i++)
            {
                //  if (childElementId == 0)
                childElementId = listMappings[i].HirerchyTypeID;
                //  else
                //     childElementId = listMappings[i-1].HirerchyTypeID;


                listMappings.Add(GetParentLocationMappingByHierarchy(allUpperHierarchies[i], childElementId));

            }

            //Array a = Enum.GetValues(typeof (Config.Hierarchy));
            /*  
              foreach (Config.Hierarchy s in a)
              {
                  listMappings.Add(new LocationMapping()
                  {
                      Hierarchy = s,
                      HirerchyTypeID = 1
                  });
              }
              */
            return listMappings;
        }

        private static LocationMapping GetParentLocationMappingByHierarchy(Config.Hierarchy hierarchy, int childHierarchyId)
        {



            LocationMapping mapping = new LocationMapping();
            Dictionary<string, object> paramz = new Dictionary<string, object>()
            {
             
                {"@Hierarchy_Type",Convert.ToInt32(hierarchy)},
                {"@ChildHierarchyId",Convert.ToInt32(childHierarchyId)}

            };
            DataSet locationsDataSet = DBHelper.GetDataSetByStoredProcedure("[PITB].[Get_HirerchyInformation]", paramz);
            if (locationsDataSet.Tables.Count > 0)
                if (locationsDataSet.Tables[0].Rows.Count > 0)
                {
                    mapping.Hierarchy = hierarchy;
                    mapping.HirerchyTypeID = Convert.ToInt32(locationsDataSet.Tables[0].Rows[0]["HierarchyTypeID"]);
                }
            return mapping;
        }

        public static bool IsLocationExistInPolygon(LocationCoordinate coordinate, out LocationMapping mapping)
        {
            bool isExist = false;
            LocationMapping matchedMapping = null;
            var allHierarchies = Enum.GetValues(typeof(Config.Hierarchy))
                                     .Cast<Config.Hierarchy>()
                                     .OrderByDescending(e => (int)e)
                                     .ToList();
            if (_hirerchyAndTheirMappings.Count <= 0)
            {
                //Initialize 
                foreach (Config.Hierarchy allUpperHierarchy in allHierarchies)
                {
                    LoadLocationCoordinatesBoundries(allUpperHierarchy);
                }
            }

            foreach (var dict in _hirerchyAndTheirMappings.OrderByDescending(m => m.Key))
            {
                if (IsLocationExistInPolygon(coordinate, (Config.Hierarchy)dict.Key, out matchedMapping))
                {
                    isExist = true;
                    break;
                }
            }
            mapping = matchedMapping;
            return isExist;
        }

        public static bool IsLocationExistInPolygon(LocationCoordinate coordinate, Config.Hierarchy hierarchyType, out LocationMapping mapping)
        {
            bool isExist = false;
            LocationMapping matchedMapping = null;
            List<LocationMapping> locationBoundriesOfHirerchy = LoadLocationCoordinatesBoundries(hierarchyType);
            if (locationBoundriesOfHirerchy.Count > 0)
                foreach (LocationMapping boundry in locationBoundriesOfHirerchy)
                {
                    if (IsPointInPolygon(boundry.LocationCoordinates, coordinate))
                    {
                        FindHierarchyTypeIdByFidAndHierarchyType(boundry);
                        matchedMapping = boundry;
                        isExist = true;
                        break;
                    }
                }
            mapping = matchedMapping;
            return isExist;
        }

        private static void FindHierarchyTypeIdByFidAndHierarchyType(LocationMapping mapping)
        {
            Dictionary<string, object> paramz = new Dictionary<string, object>()
            {
             
                {"@Hierarchy_Type",Convert.ToInt32(mapping.Hierarchy)},
                {"@FID",(mapping.KML_FID)}

            };
            DataSet locationsDataSet = DBHelper.GetDataSetByStoredProcedure("[PITB].[Get_HirerchyInformation_By_KMLFID]", paramz);
            mapping.HirerchyTypeID = Convert.ToInt32(locationsDataSet.Tables[0].Rows[0][0]);

        }
        private static List<LocationMapping> LoadLocationCoordinatesBoundries(Config.Hierarchy hierarchy)
        {
            List<LocationMapping> mappingList;

            if (_hirerchyAndTheirMappings.TryGetValue(Convert.ToInt32(hierarchy), out mappingList))
            {
                return mappingList;

            }

            mappingList = new List<LocationMapping>();
            LocationMapping mapping;


            Dictionary<string, object> paramz = new Dictionary<string, object>()
            {
                {"@HirerchyID",Convert.ToInt32(hierarchy)}
            };
            DataSet locationsDataSet = DBHelper.GetDataSetByStoredProcedure("[PITB].[Get_Boundries_By_HirerchyID]", paramz);
            if (locationsDataSet.Tables.Count > 0)
                if (locationsDataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < locationsDataSet.Tables[0].Rows.Count; i++)
                    {
                        mapping = new LocationMapping()
                        {
                            Hierarchy = (Config.Hierarchy)Convert.ToInt32(locationsDataSet.Tables[0].Rows[i]["Hirerchy_ID"]),
                            KML_FID = Convert.ToInt32(locationsDataSet.Tables[0].Rows[i]["Boundry_KML_FID"]),
                            LocationCoordinates = JsonToLocationCoordinate(locationsDataSet.Tables[0].Rows[i]["Boundry_Collection"].ToString())
                        };
                        mappingList.Add(mapping);
                    }
                }
            if (mappingList.Count > 0)
            {
                _hirerchyAndTheirMappings.Add(Convert.ToInt32(hierarchy), mappingList);

            }
            return mappingList;
        }

        private static List<LocationCoordinate> JsonToLocationCoordinate(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<List<LocationCoordinate>>(json);
            }
            return null;
        }
        private static bool IsPointInPolygon(List<LocationCoordinate> boundries, LocationCoordinate point)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = boundries.Count - 1; i < boundries.Count; j = i++)
            {
                if ((((boundries[i].Lt <= point.Lt) && (point.Lt < boundries[j].Lt))
                     || ((boundries[j].Lt <= point.Lt) && (point.Lt < boundries[i].Lt)))
                    &&
                    (point.Lg < (boundries[j].Lg - boundries[i].Lg) * (point.Lt - boundries[i].Lt)
                     / (boundries[j].Lt - boundries[i].Lt) + boundries[i].Lg))

                    c = !c;
            }
            return c;
        }


        public static List<LocationCoordinate> GetRadiusPoints(double lat, double lon, double radius)
        {
            List<LocationCoordinate> coords =new  List<LocationCoordinate>();

            lat = ConvertToRadians(lat);
            lon = ConvertToRadians(lon);
            double d = radius / EarthRadius;
            for (int x = 0; x <= 360; x++)
            {
                double brng = ConvertToRadians(x);
                double latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                double lngRadians = lon + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));
                coords.Add(new LocationCoordinate(ConvertToLocationCoordinate(latRadians), ConvertToLocationCoordinate(lngRadians)));
            }
            return coords;
        }
        public static double ConvertToRadians(double angle)
        {
            return (angle * Math.PI) / 180;
        }
        public static double ConvertToLocationCoordinate(double radian)
        {
            return (radian * 180) / Math.PI;
        }
    }
}