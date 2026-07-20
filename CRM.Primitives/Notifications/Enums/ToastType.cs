using CRM.Primitives.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace CRM.Primitives.Notifications.Enums
{
    /// <summary>
	/// Type of warning/message box
	/// </summary>
	public enum ToastType : Int32
    {
        [Display(Name = "Info")]
        [UI(ColorClass = "info")]
        Info = 1,
        [Display(Name = "Warning")]
        [UI(ColorClass = "warning")]
        Warning = 2,
        [Display(Name = "Error")]
        [UI(ColorClass = "error")]
        Error = 4,
        [Display(Name = "Success")]
        [UI(ColorClass = "success")]
        Success = 8
    }
}
