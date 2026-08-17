using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Tickets.Domain
{
    public enum TicketStatus
    {
        [Display(Name = "")]
        New = 0,
        [Display(Name = "")]
        Triage = 1,
        [Display(Name = "In Progress")]
        InProgress = 2,
        [Display(Name = "Waiting On Client")]
        WaitingOnClient = 3,
        [Display(Name = "Waiting On Internal")]
        WaitingOnInternal = 4,
        [Display(Name = "Ready To Invoice")]
        ReadyToInvoice = 5,
        [Display(Name = "Complete")]
        Complete = 6,
        [Display(Name = "Cancelled")]
        Cancelled = 7
    }
}