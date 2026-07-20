using Microsoft.AspNetCore.Components;

namespace CRM.Web.State
{
    public sealed class PageHeaderState
    {
        public event Action? OnChange;

        public String Title { get; private set; } = "Dashboard";
        public String? Subtitle { get; private set; }
        public RenderFragment? Actions { get; private set; }

        public void SetHeader(String strTitle, String? strSubtitle = null, RenderFragment? objActions = null)
        {
            Title = strTitle;
            Subtitle = strSubtitle;
            Actions = objActions;

            NotifyStateChanged();
        }

        public void SetActions(RenderFragment? objActions)
        {
            Actions = objActions;

            NotifyStateChanged();
        }

        public void Reset()
        {
            Title = "Dashboard";
            Subtitle = "Overview of your CRM activity";
            Actions = null;

            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}