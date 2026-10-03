using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contractors.Domain;

public sealed class Contractor
{
    public Guid Id { get; set; }

    [Required, StringLength(200)]
    public String Name { get; set; } = String.Empty;

    [StringLength(200)]
    public String? CompanyName { get; set; }

    [EmailAddress, StringLength(254)]
    public String? Email { get; set; }

    [StringLength(50)]
    public String? Phone { get; set; }

    [StringLength(4000)]
    public String? Notes { get; set; }

    public Boolean Enabled { get; set; } = true;
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }

    public String DisplayName => Name;

    public String GetInitials() => String.Concat(Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Take(2).Select(strPart => Char.ToUpperInvariant(strPart[0])));
}
