using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Tickets.Domain
{
    public enum TicketPriority
    {
        [Display(Name = "Low")]
        Low = 0,
        [Display(Name = "Normal")]
        Normal = 1,
        [Display(Name = "High")]
        High = 2,
        [Display(Name = "Urgent")]
        Urgent = 3
    }
}