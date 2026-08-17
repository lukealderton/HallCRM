using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Entities.Domain
{
    public enum PredefinedEntityType
    {
        [Display(Name = "Company")]
        Company = 1,
        [Display(Name = "Contact")]
        Contact = 2,
        [Display(Name = "Job")]
        Job = 3,
        [Display(Name = "Ticket")]
        Ticket = 4,
        [Display(Name = "Activity")]
        Activity = 5,
        [Display(Name = "Service")]
        Service = 6,

        [Display(Name = "Custom Record")]
        CustomRecord = 100
    }
}
