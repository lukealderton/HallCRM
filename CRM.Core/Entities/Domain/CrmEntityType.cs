namespace CRM.Core.Entities.Domain
{
    public sealed class CrmEntityType
    {
        public Int32 Id { get; set; }

        public String Name { get; set; } = String.Empty;
        public String Alias { get; set; } = String.Empty;

        public Boolean IsSystem { get; set; }
        public Boolean IsCustom { get; set; }

        public DateTime CreatedUtc { get; set; }

        public ICollection<CrmEntity> Entities { get; set; } = [];
    }
}
