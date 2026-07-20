namespace CRM.Core.Geocoding.Domain
{
    public sealed class LocateResult
    {
        public LocateResult(double dblLat, double dblLong, String? strName)
        {
            Name        = strName;
            Coordinate  = new GeographicCoordinate(dblLat, dblLong);
        }

        public String?              Name        { get; set; }
        public GeographicCoordinate Coordinate  { get; set; }
    }
}
