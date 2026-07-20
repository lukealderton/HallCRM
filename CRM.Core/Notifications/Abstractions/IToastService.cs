using CRM.Core.Notifications.Domain;
using CRM.Primitives.Notifications.Enums;
using System.Runtime.CompilerServices;

namespace CRM.Core.Notifications.Abstractions
{
    public interface IToastService
    {
        /// <summary>
        /// List of <see cref="Domain.Toast">Toast</see>s currently active
        /// </summary>
        List<Toast> Toasts { get; }

        /// <summary>
        /// Event handler, called when a toast is added or removed
        /// </summary>
        event Action? OnChange;

        // --------------- Toasts --------------- //

        /// <summary>
        /// Displays a message on screen to the user.
        /// </summary>
        /// <param name="enmAlertType"></param>
        /// <param name="strMessage"></param>
        /// <param name="strTitle"></param>
        /// <param name="enmToastLength"></param>
        public void Toast(ToastType enmAlertType, String? strMessage, String? strTitle = null, ToastLength enmToastLength = ToastLength.Short);

        /// <summary>
        /// Toast to the current user that an error has occured and optionally logs/emails sysadmin.
        /// </summary>
        /// <param name="objError">The error object.</param>
        /// <param name="strMessage">Main message body text.</param>
        /// <param name="blnEmail">If set, administrator will be notified by email of the error and stack trace.</param>
        /// <param name="blnLog">Should log or not</param>
        /// <param name="callerMember">The method that called this</param>
        public void ToastException(Exception objError, String? strMessage = null, Boolean blnEmail = true, Boolean blnLog = true, [CallerMemberName] String callerMember = "");

        /// <summary>
        /// Removes a toast and notifies OnChanged handler
        /// </summary>
        /// <param name="objToast">The toast object from the Toasts array to remove</param>
        void RemoveToast(Toast objToast);
    }
}
