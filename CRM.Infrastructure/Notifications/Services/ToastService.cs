using CRM.Core.Notifications.Abstractions;
using CRM.Core.Notifications.Domain;
using CRM.Primitives.Notifications.Enums;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace CRM.Infrastructure.Notifications.Services
{
    public sealed class ToastService : IToastService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<Toast> Toasts { get; } = [];

        public event Action? OnChange;

        private readonly TimeSpan _ts;
        private readonly System.Timers.Timer _timer;

        public ToastService(IHttpContextAccessor objHttpContextAccessor)
        {
            _httpContextAccessor = objHttpContextAccessor;

            _ts = TimeSpan.FromSeconds(1);
            _timer = new System.Timers.Timer(_ts);

            _timer.Elapsed += (o, e) => Cleanup();
        }

        ///<inheritdoc/>
        private void Cleanup()
        {
            foreach (Toast objToast in Toasts.ToList()) // ToList is a cheat so we don't get 'collection modified' error
            {
                if (objToast.ToastLength != ToastLength.Infinite)
                {
                    // Expires different times based on toast length
                    DateTime dtmExpires = DateTime.UtcNow.AddSeconds(-(objToast.ToastLength == ToastLength.Short ? 12 : 60));

                    if (objToast.CreatedAt < dtmExpires)
                    {
                        RemoveToast(objToast);
                    }
                }
            }
        }

        ///<inheritdoc/>
        public void Toast(ToastType enmAlertType, String? strMessage, String? strTitle = null, ToastLength enmToastLength = ToastLength.Short)
        {
            String strCssClass;

            switch (enmAlertType)
            {
                default:
                    strTitle ??= "Info";
                    strCssClass = "";
                    break;
                case ToastType.Warning:
                    strTitle ??= "Warning";
                    strCssClass = "bg-warning";
                    break;
                case ToastType.Error:
                    strTitle ??= "Error";
                    strCssClass = "bg-danger text-white";
                    break;
                case ToastType.Success:
                    strTitle ??= "Success";
                    strCssClass = "bg-success text-white";
                    break;
            }

            Toast objAlert = new(strCssClass, strTitle, strMessage ?? "");

            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session != null)
            {
                Toasts.Add(objAlert);
                OnChange?.Invoke();
            }

            _timer.Start();
        }

        ///<inheritdoc/>
        public void ToastException(Exception objError, String? strMessage = null, Boolean blnEmail = true, Boolean blnLog = true, [CallerMemberName] String callerMember = "")
        {
            // Alert error with specified text.
            Toast(ToastType.Error, !String.IsNullOrWhiteSpace(strMessage) ? strMessage : objError.Message, enmToastLength: ToastLength.Infinite);

            // strMessage ??= "Failed to " + callerMember;

            //if (blnLog)
            //{
            //    //_logService.LogError(objError, LogEventId.UpdateProfile, strMessage + objError.Message, blnEmail);
            //}
            //else if (blnEmail)
            //{
            //    //Helpers.Mail.SendExceptionEmail(objError, null, strMessage);
            //}
        }

        ///<inheritdoc/>
        public void RemoveToast(Toast objToast)
        {
            Toasts.Remove(objToast);

            OnChange?.Invoke();

            if (Toasts.Count == 0)
            {
                _timer.Stop();
            }
        }
    }
}
