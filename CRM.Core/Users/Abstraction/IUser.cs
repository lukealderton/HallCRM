using CRM.Primitives.Common.Abstraction;

namespace CRM.Core.Users.Abstraction
{
    public interface IUser : IDbItem
    {
        String Forename { get; set; }
        String Surname { get; set; }
        String Email { get; set; }
        DateTimeOffset RegisterDate { get; set; }
        DateTimeOffset LastLoginDate { get; set; }
        String ProfileImage { get; set; }
    }
}
