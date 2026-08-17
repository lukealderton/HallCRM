using CRM.Primitives.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Invoices.Domain
{
    public enum InvoiceStatus
    {
        [Display(Name = "Draft")]
        [UI(
            IconName = "draft",
            ColorHex = "#64748b",
            ColorClass = "secondary")]
        Draft = 0,

        [Display(Name = "Issued")]
        [UI(
            IconName = "send",
            ColorHex = "#2563eb",
            ColorClass = "primary")]
        Issued = 1,

        [Display(Name = "Part paid")]
        [UI(
            IconName = "payments",
            ColorHex = "#b45309",
            ColorClass = "warning")]
        PartPaid = 2,

        [Display(Name = "Paid")]
        [UI(
            IconName = "check_circle",
            ColorHex = "#15803d",
            ColorClass = "success")]
        Paid = 3,

        [Display(Name = "Void")]
        [UI(
            IconName = "block",
            ColorHex = "#b91c1c",
            ColorClass = "danger")]
        Void = 4
    }
}