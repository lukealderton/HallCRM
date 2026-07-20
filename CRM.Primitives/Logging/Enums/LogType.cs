using System.ComponentModel.DataAnnotations;

namespace CRM.Primitives.Logging.Enums
{
    [Flags]
    public enum LogType : Int32
    {
        /// <summary>No specific type.</summary>
        [Display(Name = "None", Description = "")]
        None = 0,

        /// <summary>A tenant status change, e.g. pending to live, registration etc.</summary>
        [Display(Name = "Status", Description = "A tenant status change, e.g. pending to live, registration etc.")]
        Status = 1 << 0,
        /// <summary>Account expiration, payment notice etc.</summary>
        [Display(Name = "Alert", Description = "Account expiration, payment notice etc.")]
        Alert = 1 << 1,
        /// <summary>Update account, target information, login or logout etc. (General site usage.)</summary>
        [Display(Name = "Activity", Description = "Update account, target information, login or logout etc. (General site usage.)")]
        Activity = 1 << 2,
        /// <summary>Admin only.</summary>
        [Display(Name = "Note", Description = "Admin only.")]
        Note = 1 << 3,
        /// <summary>Exception occurs anywhere.</summary>
        [Display(Name = "Error", Description = "Exception occurs anywhere.")]
        Error = 1 << 4,
        /// <summary>Debug purposes, shorter life than normal logs.</summary>
        [Display(Name = "Debug", Description = "Debug purposes, shorter life than normal logs.")]
        Debug = 1 << 5,
        /// <summary>Logging CraftyClick usage.</summary>
        [Display(Name = "CraftyClick", Description = "Logging CraftyClick usage.")]
        CraftyClick = 1 << 6,
        /// <summary>Logging API usage.</summary>
        [Display(Name = "API", Description = "Logging API usage.")]
        API = 1 << 7,
        /// <summary>
        /// Automated tasks, system messages and verbose logging.
        /// </summary>
        [Display(Name = "System")]
        System = 1 << 8,
        /// <summary>
        /// Logs security issues
        /// </summary>
        [Display(Name = "Security")]
        Security = 1 << 9,
        /// <summary>
        /// Logs flagged content moderation issues
        /// </summary>
        [Display(Name = "Content Moderation Result")]
        Moderation = 1 << 10
    }
}
