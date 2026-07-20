using CRM.Core.Common.Extensions;
using CRM.Primitives.Notifications.Enums;

namespace CRM.Core.Notifications.Domain
{
    /// <summary>
    /// A basic Bootstrap alert object.
    /// </summary>
    public class Toast
    {
        public Toast() { }
        public Toast(String strElementClass, string? strTitle, string strMessage, ToastLength enmToastLength = ToastLength.Short)
        {
            ElementClass    = strElementClass;
            Title           = strTitle;
            Message         = strMessage;
            ToastLength     = enmToastLength;
        }

        /// <summary>
        /// Styles to apply to the toast
        /// </summary>
        public string? ElementClass { get; set; }

        /// <summary>
        /// The bold text, displayed first within the alert.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// The text that gets displayed after the title.
        /// </summary>
        public string? Message { get; set; }

        public string TimeSinceStr
        {
            get
            {
                return CreatedAt.GetRelativeTime();
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ToastLength ToastLength { get; set; }
    }
}

