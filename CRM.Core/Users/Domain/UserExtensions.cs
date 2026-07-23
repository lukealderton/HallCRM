namespace CRM.Core.Users.Domain
{
    public static class UserExtensions
    {
        public static String GetInitials(this User objUser)
        {
            String? strFullName = objUser.Forename + " " + objUser.Surname;

            if (String.IsNullOrWhiteSpace(strFullName))
            {
                return "?";
            }

            String[] arrNameParts = strFullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (arrNameParts.Length == 1)
            {
                return arrNameParts[0][0].ToString().ToUpperInvariant();
            }

            return $"{arrNameParts[0][0]}{arrNameParts[^1][0]}".ToUpperInvariant();
        }

        public static String GetFullName(this User objUser)
        {
            return $"{objUser.Forename} {objUser.Surname}".Trim();
        }
    }
}
