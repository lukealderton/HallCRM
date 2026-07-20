using System.ComponentModel.DataAnnotations;

namespace CRM.Primitives.Logging.Enums
{
    /// <summary>
    /// Area of the system or application in either front or back end
    /// </summary>
    public enum LogArea : Int32
    {
        [Display(Name = "None")]
        None = 0,

        [Display(Name = "Profile")]
        Profile = 10,
        [Display(Name = "Api")]
        Api = 17,
        [Display(Name = "Geocoding")]
        Geocoding = 18,

        [Display(Name = "Tickets")]
        Tickets = 19,

        [Display(Name = "Finished")]
        Finished = 100,

        [Display(Name = "Mailing")]
        Mailing = 250,

        [Display(Name = "Logs")]
        Logs = 500,
    }
}