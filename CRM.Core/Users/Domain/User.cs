//using System.Text.Json.Serialization;

//namespace CRM.Core.Users.Domain
//{
//    public class User : UserBasic
//    {
//        /// <summary>
//        /// Gets or sets the normalized email address for the user.
//        /// </summary>
//        /// <remarks>The normalized email address is typically stored in a standardized format, such as
//        /// all lowercase, to facilitate consistent comparisons and lookups. This property is useful for ensuring that
//        /// email address comparisons are case-insensitive and free of extraneous whitespace.</remarks>
//        public String? EmailNormalized { get; set; }
//        /// <summary>
//        /// Hashed password of the member
//        /// </summary>
//        public String?  PasswordHashed { get; set; }
//        /// <summary>
//        /// The date the password was last set
//        /// </summary>
//        public DateTimeOffset PwdLastSetDate { get; set; }
//        /// <summary>
//        /// Gets or sets the security stamp used to validate the user's credentials.
//        /// </summary>
//        /// <remarks>The security stamp is typically updated whenever a user's credentials change, such as
//        /// when the user changes their password. This helps in ensuring that any existing authentication tokens are
//        /// invalidated, enhancing security.</remarks>
//        public String? SecurityStamp { get; set; }
//        /// <summary>
//        /// Is the member a debug member or not
//        /// </summary>
//        public Boolean  Debug { get; set; }
//        /// <summary>
//        /// Number of invalid login attemots since last success
//        /// </summary>
//        public Int32    InvalidLoginAttempts { get; set; }
//        /// <summary>
//        /// Date the mmeber was last edited
//        /// </summary>
//        public DateTimeOffset LastEditDate { get; set; }
//        /// <summary>
//        /// Is the member account locked out
//        /// </summary>
//        public Boolean  LockedOut { get; set; }
//        /// <summary>
//        /// Is the member account enabled
//        /// </summary>
//        public Boolean  Enabled { get; set; }
//        /// <summary>
//        /// Date of birth of the member
//        /// </summary>
//        public DateTime DOB { get; set; }
//        /// <summary>
//        /// 
//        /// </summary>
//        public String? Bio { get; set; }
//        /// <summary>
//        /// 
//        /// </summary>
//        public String? PhoneNumber { get; set; }

//        /// <summary>
//        /// Calculkated age of the member in years
//        /// </summary>
//        [JsonIgnore]
//        public Int32 AgeYears
//        {
//            get
//            {
//                return (Int32)Math.Floor((DateTime.UtcNow - DOB.ToUniversalTime().Date).TotalDays / 365.25);
//            }
//        }
//    }
//}