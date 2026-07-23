namespace CRM.Core.Users.Domain
{
    public sealed class UpdateUserRequest
    {
        public Guid Id { get; set; }

        public String? Email { get; set; }
        public String? Forename { get; set; }
        public String? Surname { get; set; }

        public Boolean Enabled { get; set; }

        //public UserAccessLevel AccessLevel { get; set; }

        public String? NewPassword { get; set; }
    }
}
