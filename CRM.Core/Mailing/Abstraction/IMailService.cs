using CRM.Contracts.Results;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Mailing.Domain;
using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;
using System.Net.Mail;
using System.Text;

namespace CRM.Core.Mailing.Abstraction
{
    public interface IMailService
    {
        /// <summary>
        /// Sends an email using the mail server set up in appSettings.
        /// </summary>
        /// <param name="colFromAddresses"></param>
        /// <param name="colToAddresses"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <param name="blnIsHtml"></param>
        /// <param name="colReplyToAddresses"></param>
        /// <param name="colCCs"></param>
        /// <param name="colBCCs"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<BasicResult> SendEmailAsync
        (
            IEnumerable<EmailAddress> colFromAddresses,
            IEnumerable<EmailAddress> colToAddresses,
            String strSubject,
            String strBody,
            Boolean blnIsHtml = false,
            IEnumerable<EmailAddress>? colReplyToAddresses = null,
            IEnumerable<EmailAddress>? colCCs = null,
            IEnumerable<EmailAddress>? colBCCs = null,
            CancellationToken objToken = default
        );

        /// <summary>
        /// Sends an email using the mail server set up in appSettings.
        /// </summary>
        /// <param name="objToAddress">The address this email should be sent from.</param>
        /// <param name="strSubject">The title of this email.</param>
        /// <param name="strBody">The body of this email.</param>
        /// <param name="blnIsHtml">Mark this email as containing an HTML body.</param>
        /// <param name="colReplyToAddresses">The address this email should reply to. If left empty, will default to from address.</param>
        /// <param name="colCCs">People to copy this email to.</param>
        /// <param name="colBCCs">People to blind copy this email to.</param>
        public Task<BasicResult> SendEmailAsync
        (
            EmailAddress objToAddress,
            String strSubject,
            String strBody,
            Boolean blnIsHtml = false,
            IEnumerable<EmailAddress>? colReplyToAddresses = null,
            IEnumerable<EmailAddress>? colCCs = null,
            IEnumerable<EmailAddress>? colBCCs = null,
            CancellationToken objToken = default
        );

        /// <summary>
        /// Sends an email using the mail server set up in appSettings.
        /// </summary>
        /// <param name="objToAddress">The address this email should be sent from.</param>
        /// <param name="objFromAddress">The address this email should be sent to.</param>
        /// <param name="strSubject">The title of this email.</param>
        /// <param name="strBody">The body of this email.</param>
        /// <param name="blnIsHtml">Mark this email as containing an HTML body.</param>
        /// <param name="colReplyToAddresses">The address this email should reply to. If left empty, will default to from address.</param>
        /// <param name="colCCs">People to copy this email to.</param>
        /// <param name="colBCCs">People to blind copy this email to.</param>
        Task<BasicResult> SendEmailAsync
        (
            EmailAddress objToAddress,
            EmailAddress objFromAddress,
            String strSubject,
            String strBody,
            Boolean blnIsHtml = false,
            IEnumerable<EmailAddress>? colReplyToAddresses = null,
            IEnumerable<EmailAddress>? colCCs = null,
            IEnumerable<EmailAddress>? colBCCs = null,
            CancellationToken objToken = default
        );

        /// <summary>
        /// Sends an exception email to webmaster of exception details
        /// </summary>
        /// <param name="objException"></param>
        /// <param name="objMailDetail"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<BasicResult> SendExceptionEmailAsync(
            Exception objException, 
            ExceptionMailDetail? objMailDetail, 
            CancellationToken objToken = default);

        /// <summary>
        /// Sends email to webmaster of exception details
        /// </summary>
        /// <param name="objException"></param>
        /// <param name="objMailDetail"></param>
        Task<BasicResult> SendExceptionEmailAsync(
            Exception objException, 
            ExceptionMailDetail? objMailDetail = null, 
            String? strMessage = null, 
            String? strCCName = null, 
            String? strCCAddress = null, 
            CancellationToken objToken = default);

        /// <summary>
        /// Sends email to the webmaster with debug as the default title.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="strTitle"></param>
        /// <param name="blnIsHtml"></param>
        /// <param name="strCCName"></param>
        /// <param name="strCCAddress"></param>
        Task<BasicResult> SendDebugEmailAsync(
            String strMessage, 
            String? strTitle = null, 
            Boolean blnIsHtml = false, 
            String? strCCName = null, 
            String? strCCAddress = null, 
            CancellationToken objToken = default);
    }
}
