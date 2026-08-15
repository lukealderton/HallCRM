using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Activities.Domain
{
    public enum ActivityType
    {
        [Display(Name = "Task")]
        Task = 0,

        [Display(Name = "Call")]
        Call = 1,

        [Display(Name = "Meeting")]
        Meeting = 2,

        [Display(Name = "Site Visit")]
        SiteVisit = 3,

        [Display(Name = "Follow Up")]
        FollowUp = 4
    }
}