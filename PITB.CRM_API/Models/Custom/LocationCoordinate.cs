using System.Collections.Generic;
using System.ComponentModel;

namespace PITB.CRM_API.Models.Custom
{
   
    public class LocationCoordinate
    {
        /// <summary>
        /// Latitude property
        /// </summary>
        public double Lt { get; set; }
        
        
        /// <summary>
        /// Longitude property
        /// </summary>
        public double Lg { get; set; }


        /// <summary>
        /// Creates a new location based on Latitude and longitude
        /// </summary>
        /// <param name="latitude"> latitude is a geographic coordinate that specifies the north–south position of a point on the Earth's surface</param>
        /// <param name="longitude">longitude is a geographic coordinate that specifies the east-west position of a point on the Earth's surface</param>
        public LocationCoordinate(double latitude, double longitude)
        {
            Lt = latitude;
            Lg = longitude;
        }
    }

    public class LocationMapping
    {
        public Config.Hierarchy Hierarchy { get; set; }
        public int HirerchyTypeID { get; set; }
        public int KML_FID { get; set; }

        public List<LocationCoordinate> LocationCoordinates { get; set; }
    }
}