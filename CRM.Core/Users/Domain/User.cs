namespace CRM.Core.Users.Domain
{
    public class User
    {
        public Guid Id { get; set; }

        public String? Username { get; set; }
        public String? Email { get; set; }

        public String? Forename { get; set; }
        public String? Surname { get; set; }

        //public UserAccessLevel AccessLevel { get; set; }

        public Boolean EmailConfirmed { get; set; }
        public Boolean IsLockedOut { get; set; }
        public Boolean Enabled { get; set; }

        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset? UpdatedUtc { get; set; }
        public DateTimeOffset? LastLoginUtc { get; set; }

        public String DisplayName
        {
            get
            {
                String strName = $"{Forename} {Surname}".Trim();

                if (!String.IsNullOrWhiteSpace(strName))
                {
                    return strName;
                }

                return Email ?? Username ?? "Unknown user";
            }
        }
    }
}