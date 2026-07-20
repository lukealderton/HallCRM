//using CRM.Core.Users.Abstraction;

//namespace CRM.Core.Users.Domain
//{
//    public class UserBasic : IUser
//    {
//        /// <summary>
//        /// Id of the member in the database
//        /// </summary>
//        public Guid     Id { get; set; }
//        public String   DisplayName { get; set; } = "";
//        /// <summary>
//        /// Users forename
//        /// </summary>
//        public String   Forename { get; set; } = "";
//        /// <summary>
//        /// Users surname
//        /// </summary>
//        public String   Surname { get; set; } = "";
//        /// <summary>
//        /// Users email
//        /// </summary>
//        public String   Email { get; set; } = "";
//        /// <summary>
//        /// Users email is verified
//        /// </summary>
//        public Boolean  EmailVerified { get; set; }
//        /// <summary>
//        /// Users register date
//        /// </summary>
//        public DateTimeOffset RegisterDate { get; set; }
//        /// <summary>
//        /// Users last login date
//        /// </summary>
//        public DateTimeOffset LastLoginDate { get; set; }

//        /// <summary>
//        /// Users profil eimage url
//        /// </summary>
//        public String   ProfileImage { get; set; } = "";
//        /// <summary>
//        /// Used for date and time display
//        /// </summary>
//        public String? TimeZone { get; set; }

//        public String GetInitials()
//        {
//            String strForename = String.IsNullOrWhiteSpace(Forename) ? "" : Forename.Trim()[0].ToString();
//            String strSurname = String.IsNullOrWhiteSpace(Surname) ? "" : Surname.Trim()[0].ToString();

//            String strInitials = (strForename + strSurname).ToUpperInvariant();

//            return String.IsNullOrWhiteSpace(strInitials) ? "?" : strInitials;
//        }

//        public String GetDisplayName()
//        {
//            if (!String.IsNullOrWhiteSpace(DisplayName))
//            {
//                return DisplayName;
//            }

//            return Forename;
//        }
//    }
//}
