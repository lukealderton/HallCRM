using CRM.Core.Geocoding.Domain;

namespace CRM.Core.Geocoding.Abstraction
{
    public interface IGeocodingService
    {
        /// <summary>
        /// Returns a link to a map for use in the browser.
        /// </summary>
        /// <param name="strPostcode">The target postcode.</param>
        /// <param name="strLatLng">The target lat long.</param>
        /// <param name="strCountry">The target country code.</param>
        /// <returns>A link to Google maps.</returns>
        String GetMapLink(String strPostcode = "", String strLatLng = "", String strCountry = "UK");

        /// <summary>
        /// Gets a formatted address using from the specified lat lng.
        /// </summary>
        /// <param name="fltLat">The location latitude.</param>
        /// <param name="fltLng">The location longitude.</param>
        /// <returns>A formatted address string.</returns>
        Task<String> RetrieveFormatedAddressAsync(float fltLat, float fltLng);

        /// <summary>
        /// Gets latitude and logitude values for a placename.
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="blnAppendUK">Will add ', UK' to the end of the search query.</param>
        /// <param name="objCancellationToken">Token to cancel search.</param>
        /// <returns></returns>
        Task<LocateResult?> LocateAsync(String strQuery, Boolean blnAppendUK = true, CancellationToken objCancellationToken = default);
    }
}
