using CRM.Contracts.Results;
using CRM.Core.Common.Configuration;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Mailing.Abstraction;
using CRM.Core.Mailing.Domain;
using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;

namespace CRM.Infrastructure.Mailing.Services
{
    public sealed class SmtpMailService : IMailService
    {
        private readonly CRMConfiguration _configuration;
        private readonly ILogService _logService;

        public SmtpMailService(
            IOptions<CRMConfiguration> objConfig,
            ILogService objLogService)
        {
            _configuration = objConfig.Value;
            _logService = objLogService;
        }

        /// <summary>
        /// Sends an email using the mail server set up in appSettings.
        /// </summary>
        /// <param name="colFromAddresses">The address this email should be sent from.</param>
        /// <param name="colToAddresses">The address this email should be sent to.</param>
        /// <param name="strSubject">The title of this email.</param>
        /// <param name="strBody">The body of this email.</param>
        /// <param name="blnIsHtml">Mark this email as containing an HTML body.</param>
        /// <param name="colReplyToAddresses">The address this email should reply to. If left empty, will default to from address.</param>
        /// <param name="colCCs">People to copy this email to.</param>
        /// <param name="colBCCs">People to blind copy this email to.</param>
        public async Task<BasicResult> SendEmailAsync
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
        )
        {
            BasicResult objResult = new();

            if (String.IsNullOrWhiteSpace(_configuration.Mail.Host)
                || String.IsNullOrWhiteSpace(_configuration.Mail.Username)
                || String.IsNullOrWhiteSpace(_configuration.Mail.Password))
            {
                await _logService.LogErrorAsync(new Exception("Mail configuration is incomplete"), LogArea.Mailing, null, null, ItemType.None, "Failed to send email", objToken);
                return new BasicResult() { Message = "Mail configuration is incomplete" };
            }

            try
            {
                MimeMessage objMessage = new();
                objMessage.From.AddRange(colFromAddresses.Select(x => x.ToMailboxAddress()));
                objMessage.To.AddRange(colToAddresses.Select(x => x.ToMailboxAddress()));
                objMessage.Subject = strSubject;
                if (colCCs != null)
                {
                    objMessage.Cc.AddRange(colCCs.Select(x => x.ToMailboxAddress()));
                }
                if (colBCCs != null)
                {
                    objMessage.Bcc.AddRange(colBCCs.Select(x => x.ToMailboxAddress()));
                }
                if (colReplyToAddresses != null)
                {
                    objMessage.ReplyTo.AddRange(colReplyToAddresses.Select(x => x.ToMailboxAddress()));
                }
                objMessage.Body = new TextPart(blnIsHtml ? "html" : "plain")
                {
                    Text = strBody
                };

                using (SmtpClient objClient = new())
                {
                    await objClient.ConnectAsync(_configuration.Mail.Host, _configuration.Mail.Port, false, objToken);
                    await objClient.AuthenticateAsync(_configuration.Mail.Username, _configuration.Mail.Password, objToken);
                    await objClient.SendAsync(objMessage, objToken);
                    await objClient.DisconnectAsync(true, objToken);
                }

                objResult.Success = true;
            }
            catch (TaskCanceledException) { }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(objError, LogArea.Mailing, null, null, ItemType.None, "Failed to send email", objToken);
                objResult.Message = objResult.Message;
            }

            return objResult;
        }

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
        public async Task<BasicResult> SendEmailAsync
        (
            EmailAddress objToAddress,
            String strSubject,
            String strBody,
            Boolean blnIsHtml = false,
            IEnumerable<EmailAddress>? colReplyToAddresses = null,
            IEnumerable<EmailAddress>? colCCs = null,
            IEnumerable<EmailAddress>? colBCCs = null,
            CancellationToken objToken = default
        )
        {
            try
            { 
                return await SendEmailAsync(
                    objToAddress, 
                    new EmailAddress(_configuration.Mail.FromAddress, _configuration.Mail.FromName), 
                    strSubject, 
                    strBody, 
                    blnIsHtml, 
                    colReplyToAddresses, 
                    colCCs, 
                    colBCCs, 
                    objToken);
            }
            catch (TaskCanceledException) { return new BasicResult() { Message = "Request was canceled" }; }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(objError, LogArea.Mailing, null, null, ItemType.None, "Failed to send email", objToken);

                return new BasicResult() { Message = objError.Message };
            }
        }

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
        public async Task<BasicResult> SendEmailAsync
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
        )
        {
            BasicResult objResult = new();

            // If no from address, use system default
            if (String.IsNullOrWhiteSpace(objFromAddress.Address))
            {
                if (!String.IsNullOrWhiteSpace(_configuration.Mail.FromAddress))
                {
                    objFromAddress = new EmailAddress(_configuration.Mail.FromAddress);
                }
                else
                {
                    _ = _logService.LogAsync(LogType.Error, LogArea.Mailing, "FromAddress in MailSettings has not been configured!", objToken: CancellationToken.None);
                    return new BasicResult() { Message = "From address is required" };
                }
            }
            
            if (String.IsNullOrWhiteSpace(objToAddress.Address))
            {
                // Need address to send to and from
                return new BasicResult() { Message = "To address is required" };
            }

            if (String.IsNullOrWhiteSpace(_configuration.Mail.Host)
                || String.IsNullOrWhiteSpace(_configuration.Mail.Username)
                || String.IsNullOrWhiteSpace(_configuration.Mail.Password))
            {
                await _logService.LogErrorAsync(new Exception("Mail configuration is incomplete"), LogArea.Mailing, null, null, ItemType.None, "Failed to send email", objToken);
                return new BasicResult() { Message = "Mail configuration is incomplete" };
            }

            try
            { 
                MimeMessage objMessage = new();
                objMessage.From.AddRange([objFromAddress.ToMailboxAddress()]);
                objMessage.To.AddRange([objToAddress.ToMailboxAddress()]);
                objMessage.Subject = strSubject;
                if (colCCs != null)
                {
                    objMessage.Cc.AddRange(colCCs.Select(x => x.ToMailboxAddress()));
                }
                if (colBCCs != null)
                {
                    objMessage.Bcc.AddRange(colBCCs.Select(x => x.ToMailboxAddress()));
                }
                if (colReplyToAddresses != null)
                {
                    objMessage.ReplyTo.AddRange(colReplyToAddresses.Select(x => x.ToMailboxAddress()));
                }
                objMessage.Body = new TextPart(blnIsHtml ? "html" : "plain")
                {
                    Text = strBody
                };

                using (SmtpClient objClient = new())
                {
                    await objClient.ConnectAsync(_configuration.Mail.Host, _configuration.Mail.Port, false, objToken);
                    await objClient.AuthenticateAsync(_configuration.Mail.Username, _configuration.Mail.Password, objToken);
                    await objClient.SendAsync(objMessage, objToken);
                    await objClient.DisconnectAsync(true, objToken);
                }

                objResult.Success = true;
            }
            catch (TaskCanceledException) { }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(objError, LogArea.Mailing, null, null, ItemType.None, "Failed to send email", objToken);
                objResult.Message = objError.Message;
            }

            return objResult;
        }

        public async Task<BasicResult> SendExceptionEmailAsync(
            Exception objException, 
            ExceptionMailDetail? objMailDetail, 
            CancellationToken objToken = default)
        {
            try
            { 
                StringBuilder objStringBuilder = new();

                // Write the exception email to StringBuilder
                objStringBuilder.Append("<html>");
                objStringBuilder.Append("<body style=\"font-family: Arial;\">");
                // Error details
                objStringBuilder.Append("<b>Error Details</b>");
                objStringBuilder.Append($"<table><tr><td>Status Code:</td><td>{objMailDetail?.StatusCode}</td></tr></table>");
                objStringBuilder.Append($"<br/><hr/><br/>");
                // Client details
                objStringBuilder.Append("<b>Client Details</b>");
                objStringBuilder.Append($"<table><tr><td>IP Address:</td><td>{objMailDetail?.RemoteIp}</td></tr>");
                objStringBuilder.Append($"<tr><td>User Agent:</td><td>{objMailDetail?.UserAgent}</td></tr>");
                objStringBuilder.Append($"<tr><td>Timestamp:</td><td>{DateTime.UtcNow}</td></tr></table>");
                objStringBuilder.Append($"<br/><hr/><br/>");
                objStringBuilder.Append($"{objException.Message}<br/><br/>");
                objStringBuilder.Append($"{objException.StackTrace?.Replace("\n", "<br/>")}");
                objStringBuilder.Append("</body>");
                objStringBuilder.Append("</html>");

                return await SendEmailAsync
                (
                    [new EmailAddress(_configuration.Mail.FromAddress, _configuration.Mail.FromName)],
                    [new EmailAddress(_configuration.Mail.WebmasterAddress, _configuration.Mail.WebmasterName)],
                    $"{_configuration.ApplicationNameShort} - Error {objMailDetail?.StatusCode}",
                    objStringBuilder.ToString(),
                    true,
                    objToken: objToken
                );
            }
            catch (TaskCanceledException) { return new BasicResult() { Message = "Request was canceled" }; }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(objError, LogArea.Mailing, null, null, ItemType.None, "Failed to send email", objToken);
                return new BasicResult() { Message = objError.Message };
            }
        }

        /// <summary>
        /// Sends email to webmaster of exception details
        /// </summary>
        /// <param name="objException"></param>
        /// <param name="objMailDetail"></param>
        public Task<BasicResult> SendExceptionEmailAsync(
            Exception objException, 
            ExceptionMailDetail? objMailDetail = null, 
            String? strMessage = null, 
            String? strCCName = null, 
            String? strCCAddress = null, 
            CancellationToken objToken = default)
        {
            if (objException == null)
            {
                // There was no error, please continue.
                return Task.FromResult(new BasicResult() { Success = true });
            }

            String? strRequestUrl;
            Int32? intStatusCode;
            String? strRemoteIp;
            String? strAgent;
            String? strTitle;

            // Collect information or build defaults if no context.
            if (objMailDetail != null)
            {
                strRequestUrl = objMailDetail.RequestUrl;
                intStatusCode = objMailDetail.StatusCode;
                strRemoteIp = objMailDetail.RemoteIp;
                strAgent = objMailDetail.UserAgent;
                strTitle = $"{_configuration.ApplicationName} - Error {objMailDetail.StatusCode}: {objException.Message}";
            }
            else
            {
                strRequestUrl = "Unknown";
                intStatusCode = 0;
                strRemoteIp = "0.0.0.0";
                strAgent = "Unknown";
                strTitle = $"{_configuration.ApplicationName} - Exception: {objException.Message}";
            }

            // Build summary of event for email
            StringBuilder objStringBuilder = new();

            // Write the exception email to StringBuilder
            objStringBuilder.Append("<html>");
            objStringBuilder.Append("<body style=\"font-family: Arial;\">");
            // Error details
            objStringBuilder.Append("<b>Error Details</b>");
            objStringBuilder.Append($"<table><tr><td>Status Code:</td><td>{intStatusCode}</td></tr>");
            objStringBuilder.Append($"<tr><td>URI:</td><td><a href=\"{strRequestUrl}\">{strRequestUrl}</a></td></tr></table>");
            objStringBuilder.Append($"<br/><hr/><br/>");

            // Extra message if given
            if (!String.IsNullOrWhiteSpace(strMessage))
            {
                objStringBuilder.Append($"<b>Further details:</b>");
                objStringBuilder.Append(strMessage);
                objStringBuilder.Append($"<br/><hr/><br/>");
            }

            // Client details
            objStringBuilder.Append("<b>Client Details</b>");
            objStringBuilder.Append($"<table><tr><td>IP Address:</td><td>{strRemoteIp}</td></tr>");
            objStringBuilder.Append($"<tr><td>User Agent:</td><td>{strAgent}</td></tr>");
            objStringBuilder.Append($"<tr><td>Timestamp:</td><td>{DateTime.UtcNow}</td></tr></table>");
            objStringBuilder.Append($"<br/><hr/><br/>");
            objStringBuilder.Append($"{objException.Message}<br/><br/>");
            objStringBuilder.Append($"{objException.StackTrace?.Replace("\n", "<br/>")}");
            objStringBuilder.Append("</body>");
            objStringBuilder.Append("</html>");

            return SendDebugEmailAsync(objStringBuilder.ToString(), strTitle, true, strCCName, strCCAddress, objToken);
        }

        /// <summary>
        /// Sends email to the webmaster with debug as the default title.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="strTitle"></param>
        /// <param name="blnIsHtml"></param>
        /// <param name="strCCName"></param>
        /// <param name="strCCAddress"></param>
        public Task<BasicResult> SendDebugEmailAsync(
            String strMessage, 
            String? strTitle = null, 
            Boolean blnIsHtml = false, 
            String? strCCName = null, 
            String? strCCAddress = null, 
            CancellationToken objToken = default)
        {
            if (String.IsNullOrWhiteSpace(strTitle))
            {
                strTitle = $"{_configuration.ApplicationName} - Debug";
            }

            EmailAddress[]? colCCs = null;
            if (!String.IsNullOrWhiteSpace(strCCName) && !String.IsNullOrWhiteSpace(strCCAddress))
            {
                colCCs =
                [
                    new EmailAddress(strCCName, strCCAddress)
                ];
            }

            return SendEmailAsync
            (
                [new EmailAddress(_configuration.Mail.WebmasterAddress,_configuration.Mail.WebmasterName)], // to
                [new EmailAddress(_configuration.Mail.FromAddress, _configuration.Mail.FromName)],          // from
                strTitle,   // title
                strMessage, // message
                blnIsHtml,  // html content
                null,       // Reply to address
                colCCs,     //cc
                null,       // bcc
                objToken
            );
        }
    }
}
