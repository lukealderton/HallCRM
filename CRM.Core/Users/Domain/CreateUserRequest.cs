namespace CRM.Core.Users.Domain
{
    public sealed class CreateUserRequest
    {
        public String Forename { get; set; } = String.Empty;
        public String Surname { get; set; } = String.Empty;
        public String Email { get; set; } = String.Empty;

        public Boolean Enabled { get; set; } = true;

        public String? Password { get; set; }
    }
}