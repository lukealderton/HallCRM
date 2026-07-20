using CRM.Core.Common.Configuration;
using CRM.Core.Geocoding.Abstraction;
using CRM.Core.Geocoding.Domain;
using CRM.Core.Logging.Abstraction;
using CRM.Primitives.Logging.Enums;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Xml.Linq;

namespace CRM.Infrastructure.Geocoding.Services
{
    public class GoogleGeocodingService : IGeocodingService
    {
        public const String HttpClientName = "GeocodingApi";

        private readonly HttpClient _httpClient;
        private readonly ILogService _logService;
        private readonly CRMConfiguration _configuration;

        public GoogleGeocodingService(
            IOptions<CRMConfiguration> objConfiguration,
            IHttpClientFactory objHttpClientFactory,
            ILogService objLogService)
        {
            _configuration  = objConfiguration.Value;
            _httpClient     =  objHttpClientFactory.CreateClient(HttpClientName);
            _logService     = objLogService;
        }

        /// <summary>
        /// This uri is used as the address to contact when a string location is required from a lat long.
        /// </summary>
        /// <param name="strEndpoint">Maps endpoint to call</param>
        /// <returns></returns>
        private String GetAPIAddress(String strEndpoint)
        {
            return $"https://maps.googleapis.com/maps/api/geocode/xml?key={_configuration.GoogleGeocoding.PublicApiKey}&sensor=false{strEndpoint}";
        }

        /// <inheritdoc/>
        public String GetMapLink(String strPostcode = "", String strLatLng = "", String strCountry = "UK")
        {
            String strBaseFormat = "https://www.google.co.uk/maps/dir//";

            if (!String.IsNullOrEmpty(strLatLng))
            {
                return strBaseFormat + strLatLng.Replace(" ", "+");
            }

            return strBaseFormat + strPostcode.Replace(" ", "+") + "," + strCountry;
        }

        /// <inheritdoc/>
        public async Task<String> RetrieveFormatedAddressAsync(float fltLat, float fltLng)
        {
            String strRequestUri = GetAPIAddress($"&latlng={fltLat},{fltLng}");

            try
            {
                using (HttpResponseMessage objResponse = await _httpClient.GetAsync(strRequestUri))
                {
                    String strResponse = await objResponse.Content.ReadAsStringAsync();

                    XElement objXmlElm = XElement.Parse(strResponse);
                    XElement? objStatus = (from objElm in objXmlElm.Descendants()
                                           where objElm.Name == "status"
                                           select objElm).FirstOrDefault();

                    if (objStatus != null && objStatus.Value.Equals("ok", StringComparison.CurrentCultureIgnoreCase))
                    {
                        XElement? objRes = (from objElm in objXmlElm.Descendants()
                                            where objElm.Name == "formatted_address"
                                            select objElm).FirstOrDefault();

                        return objRes?.Value ?? "";
                    }
                }
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(objError, LogArea.Geocoding, strMessage: "Failed to RetrieveFormatedAddress");
            }

            return "";
        }

        /// <inheritdoc/>
        public async Task<LocateResult?> LocateAsync(String strQuery, Boolean blnAppendUK = true, CancellationToken objCancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(strQuery))
            {
                return null;
            }

            // Look it up
            try
            {
                // Only append UK to search if search isn't already 'UK'.
                if (blnAppendUK && !strQuery.Trim().Equals("uk", StringComparison.CurrentCultureIgnoreCase))
                {
                    strQuery += ", UK";
                }

                String strRequestUri = GetAPIAddress("&address=" + strQuery);

                using (HttpResponseMessage objResponse = await _httpClient.GetAsync(strRequestUri, objCancellationToken))
                {
                    using (Stream objStream = objResponse.Content.ReadAsStream(objCancellationToken))
                    {
                        XDocument objDocument = await XDocument.LoadAsync(objStream, LoadOptions.None, objCancellationToken);

                        if (objDocument == null)
                        {
                            return null;
                        }

                        XElement? objLongitudeElement = objDocument.Descendants("lng").FirstOrDefault();
                        XElement? objLatitudeElement  = objDocument.Descendants("lat").FirstOrDefault();

                        if (objLongitudeElement == null || objLatitudeElement == null)
                        {
                            return null;
                        }

                        Double dblLatitude  = Double.Parse(objLatitudeElement.Value,  CultureInfo.InvariantCulture);
                        Double dblLongitude = Double.Parse(objLongitudeElement.Value, CultureInfo.InvariantCulture);
                        String? strPlaceName = null;
                        
                        try
                        {
                            XElement? objFirstResult = objDocument.Element("GeocodeResponse")?.Element("result");
                            IEnumerable<XElement>? colResultTypes = objFirstResult?.Elements("type");

                            if (colResultTypes != null 
                                && colResultTypes.Any(x => x.Value == "locality" || x.Value == "sublocality" || x.Value == "administrative_area_level_2") 
                                && colResultTypes.Any(x => x.Value == "political"))
                            {
                                // Must be city or town yeah?
                                // https://developers.google.com/maps/documentation/geocoding/requests-geocoding#Types

                                strPlaceName = objFirstResult?.Element("address_component")?.Element("long_name")?.Value ?? "";
                            }
                        }
                        catch (Exception objError)
                        {
                            await _logService.LogErrorAsync(objError, LogArea.Geocoding, strMessage: "Failed to get placename in LocateAsync", objToken: CancellationToken.None);
                        }

                        LocateResult objResult = new(dblLatitude, dblLongitude, strPlaceName);

                        return objResult;
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(objError, LogArea.Geocoding, strMessage: "Failed to LocateAsync", objToken: CancellationToken.None);
            }

            return null;
        }
    }
}
