namespace CRM.Core.Geocoding.Domain
{
    public class GeographicCoordinate(Double dblLat, Double dblLng)
    {
        public Double Latitude { get; } = dblLat;
        public Double Longitude { get; } = dblLng;

        /// <summary>
        /// Formats as "lat, lng"
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"{Latitude}, {Longitude}";
        }

        /// <summary>
        /// Converts to OS Grid Coordinate
        /// </summary>
        /// <returns></returns>
        public OsGridCoordinate ToOSGridCoordinate()
        {
            return new OsGridCoordinate(Latitude, Longitude);
        }
    }
}
