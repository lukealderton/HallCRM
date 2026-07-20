using System.ComponentModel.DataAnnotations;

namespace CRM.Primitives.Notifications.Enums
{
    /// <summary>
	/// How long to display a toast for
	/// </summary>
	public enum ToastLength : Int32
    {
        [Display(Name = "Short")]
        Short = 1,
        [Display(Name = "Long")]
        Long = 2,
        [Display(Name = "Infinite")]
        Infinite = 3,
    }

}