using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Jobs.Domain
{
    public enum JobStage
    {
        [Display(Name = "New")]
        New = 0,
        [Display(Name = "Quoted")]
        Quoted = 1,
        [Display(Name = "To Do")]
        ToDo = 2,
        [Display(Name = "In Progress")]
        InProgress = 2,
        [Display(Name = "Invoiced")]
        Invoiced = 3,
        [Display(Name = "Paid")]
        Paid = 4,
        [Display(Name = "Lost")]
        Lost = 5
    }
}