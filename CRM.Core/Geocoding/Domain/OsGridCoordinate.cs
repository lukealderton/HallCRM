namespace CRM.Core.Geocoding.Domain
{
    /// <summary>
    /// OS National Grid coordinate (immutable).
    /// </summary>
    public class OsGridCoordinate
    {
        public Int32 Northing { get; }
        public Int32 Easting { get; }
        public Double Latitude { get; }
        public Double Longitude { get; }

        /// <summary>
        /// Creates an OS coordinate from a lat and lng then generates the north east from it
        /// </summary>
        /// <param name="dblLat"></param>
        /// <param name="dblLng"></param>
        public OsGridCoordinate(Double dblLat, Double dblLng)
        {
            Latitude = dblLat;
            Longitude = dblLng;

            if (dblLat != 0 || dblLng != 0)
            {
                CalculateFromLatLng(
                    dblLat,
                    dblLng,
                    out Int32 intNorthing,
                    out Int32 intEasting);

                Northing = intNorthing;
                Easting = intEasting;
            }
            else
            {
                Northing = 0;
                Easting = 0;
            }
        }

        /// <summary>
        /// Creates an OS coordinate with the specified values
        /// </summary>
        /// <param name="dblLat"></param>
        /// <param name="dblLng"></param>
        /// <param name="intNorth"></param>
        /// <param name="intEast"></param>
        public OsGridCoordinate(Double dblLat, Double dblLng, Int32 intNorth, Int32 intEast)
        {
            Latitude = dblLat;
            Longitude = dblLng;
            Northing = intNorth;
            Easting = intEast;
        }

        /// <summary>
        /// Performs WGS84 → OSGB36 conversion.
        /// </summary>
        private static void CalculateFromLatLng(
            Double dblLat,
            Double dblLng,
            out Int32 intNorth,
            out Int32 intEast)
        {
            Double lat = dblLat * Math.PI / 180;
            Double lon = dblLng * Math.PI / 180;

            Double a = 6377563.396;
            Double b = 6356256.910;
            Double F0 = 0.9996012717;
            Double lat0 = 49 * Math.PI / 180;
            Double lon0 = -2 * Math.PI / 180;
            Double N0 = -100000;
            Double E0 = 400000;
            Double e2 = 1 - (b * b) / (a * a);
            Double n = (a - b) / (a + b), n2 = n * n, n3 = n * n * n;

            Double cosLat = Math.Cos(lat), sinLat = Math.Sin(lat);
            Double nu = a * F0 / Math.Sqrt(1 - e2 * sinLat * sinLat);
            Double rho = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinLat * sinLat, 1.5);
            Double eta2 = nu / rho - 1;

            Double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (lat - lat0);
            Double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(lat - lat0) * Math.Cos(lat + lat0);
            Double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (lat - lat0)) * Math.Cos(2 * (lat + lat0));
            Double Md = (35 / 24) * n3 * Math.Sin(3 * (lat - lat0)) * Math.Cos(3 * (lat + lat0));
            Double M = b * F0 * (Ma - Mb + Mc - Md);

            Double cos3lat = cosLat * cosLat * cosLat;
            Double cos5lat = cos3lat * cosLat * cosLat;
            Double tan2lat = Math.Tan(lat) * Math.Tan(lat);
            Double tan4lat = tan2lat * tan2lat;

            Double I = M + N0;
            Double II = (nu / 2) * sinLat * cosLat;
            Double III = (nu / 24) * sinLat * cos3lat * (5 - tan2lat + 9 * eta2);
            Double IIIA = (nu / 720) * sinLat * cos5lat * (61 - 58 * tan2lat + tan4lat);
            Double IV = nu * cosLat;
            Double V = (nu / 6) * cos3lat * (nu / rho - tan2lat);
            Double VI = (nu / 120) * cos5lat * (5 - 18 * tan2lat + tan4lat + 14 * eta2 - 58 * tan2lat * eta2);

            Double dLon = lon - lon0;
            Double dLon2 = dLon * dLon, dLon3 = dLon2 * dLon, dLon4 = dLon3 * dLon, dLon5 = dLon4 * dLon, dLon6 = dLon5 * dLon;

            intNorth = (Int32)(I + II * dLon2 + III * dLon4 + IIIA * dLon6);
            intEast = (Int32)(E0 + IV * dLon + V * dLon3 + VI * dLon5);
        }

        /// <summary>
        /// Formats the northing and easting values into a somma separated string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return String.Format("{0}, {1}", Northing, Easting);
        }
    }
}