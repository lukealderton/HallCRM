using CRM.Primitives.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Payments.Domain
{
    public enum PaymentMethod
    {
        [Display(Name = "Bank transfer")]
        [UI(
            IconName = "account_balance",
            ColorHex = "#2563eb",
            ColorClass = "primary")]
        BankTransfer = 0,

        [Display(Name = "Cash")]
        [UI(
            IconName = "payments",
            ColorHex = "#15803d",
            ColorClass = "success")]
        Cash = 1,

        [Display(Name = "Card")]
        [UI(
            IconName = "credit_card",
            ColorHex = "#7c3aed",
            ColorClass = "purple")]
        Card = 2,

        [Display(Name = "Cheque")]
        [UI(
            IconName = "receipt_long",
            ColorHex = "#b45309",
            ColorClass = "warning")]
        Cheque = 3,

        [Display(Name = "Other")]
        [UI(
            IconName = "more_horiz",
            ColorHex = "#64748b",
            ColorClass = "secondary")]
        Other = 100
    }
}