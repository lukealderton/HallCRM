using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Tickets.Domain
{
    public enum TicketType
    {
        [Display(Name = "General")]
        General = 0,
        [Display(Name = "Support")]
        Support = 1,
        [Display(Name = "Bug")]
        Bug = 2,
        [Display(Name = "Change Request")]
        ChangeRequest = 3,
        [Display(Name = "Development")]
        Development = 4,
        [Display(Name = "Design")]
        Design = 5,
        [Display(Name = "Content")]
        Content = 6,
        [Display(Name = "Hosting")]
        Hosting = 7,
        [Display(Name = "Maintenance")]
        Maintenance = 8,
        [Display(Name = "Admin")]
        Admin = 9
    }
}
