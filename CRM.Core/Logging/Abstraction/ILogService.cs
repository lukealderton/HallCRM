using CRM.Contracts.Results;
using CRM.Core.Common.Domain;
using CRM.Core.Logging.Domain;
using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;

namespace CRM.Core.Logging.Abstraction
{
    /// <summary>
    /// Logging service interface for managing logs in the system.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Gets a single specific log
        /// </summary>
        /// <param name="objLogId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Log?> GetLogAsync(Guid objLogId, CancellationToken objToken = default);

        /// <summary>
        /// Gets logs between the specified dates with optional filtering by keyword, type, and area.
        /// </summary>
        /// <param name="strKeyword"></param>
        /// <param name="objStartDate"></param>
        /// <param name="objEndDate"></param>
        /// <param name="enmType"></param>
        /// <param name="enmArea"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intSkip"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<SearchResult<Log>> GetLogsAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            LogType enmType,
            LogArea enmArea,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default);

        /// <summary>
        /// Counts total logs with specified types
        /// </summary>
        /// <param name="enmType"></param>
        /// <param name="enmArea"></param>
        /// <param name="dtmStart">Optional</param>
        /// <param name="dtmEnd">Optional</param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Int32> CountLogsOfTypeAsync(LogType enmType = 0, LogArea enmArea = 0, DateTime? dtmStart = null, DateTime? dtmEnd = null, CancellationToken objToken = default);

        /// <summary>
        /// Gets log entries of the specified type that happened after the specified date.
        /// </summary>
        /// <param name="objStartDate"></param>
        /// <param name="objEndDate"></param>
        /// <param name="enmType"></param>
        /// <param name="enmArea"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intSkip"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<SearchResult<Log>> GetLogsByTypeAsync(String? strKeyword, DateTime objStartDate, DateTime objEndDate, LogType enmType, LogArea enmArea, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default);

        /// <summary>
        /// Gets logs of the specified type.
        /// </summary>
        /// <param name="enmType"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intSkip"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<SearchResult<Log>> GetLogsByTypeAsync(LogType enmType, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default);

        /// <summary>
        /// Gets log entries of the specified area that happened after the specified date.
        /// </summary>
        /// <param name="objStartDate"></param>
        /// <param name="enmArea"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<SearchResult<Log>> GetLogsByAreaAsync(DateTime objStartDate, LogArea enmArea, CancellationToken objToken = default);

        /// <summary>
        /// Gets all logs between the specified dates.
        /// </summary>
        /// <param name="objStartDate"></param>
        /// <param name="objEndDate"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intSkip"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<SearchResult<Log>> GetLogsAsync(String? strKeyword, DateTime objStartDate, DateTime objEndDate, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default);

        /// <summary>
        /// Get latest logs for member
        /// </summary>
        /// <param name="objMemberId"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intSkip"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<SearchResult<Log>> GetLogsForMemberAsync(Guid objMemberId, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default);

        /// <summary>
        /// Logs an error with the specified details.
        /// </summary>
        /// <param name="objError"></param>
        /// <param name="enmArea"></param>
        /// <param name="objMemberId"></param>
        /// <param name="objRelId"></param>
        /// <param name="enmRelType"></param>
        /// <param name="strMessage"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> LogErrorAsync(Exception objError, LogArea enmArea = LogArea.None, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, String? strMessage = null, CancellationToken objToken = default);

        /// <summary>
        /// Logs an error with the specified details.
        /// </summary>
        /// <param name="enmType"></param>
        /// <param name="enmArea"></param>
        /// <param name="strLogText"></param>
        /// <param name="objMemberId"></param>
        /// <param name="objRelId"></param>
        /// <param name="enmRelType"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> LogAsync(LogType enmType, LogArea enmArea, String? strLogText = null, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, CancellationToken objToken = default);

        /// <summary>
        /// Logs an action with the specified details.
        /// </summary>
        /// <param name="enmType"></param>
        /// <param name="enmArea"></param>
        /// <param name="dtmLogTimestamp"></param>
        /// <param name="strLogText"></param>
        /// <param name="objMemberId"></param>
        /// <param name="objRelId"></param>
        /// <param name="enmRelType"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> LogAsync(LogType enmType, LogArea enmArea, DateTime dtmLogTimestamp, String? strLogText = null, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, CancellationToken objToken = default);

        /// <summary>
        /// Logs a debug message with the specified details.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="enmArea"></param>
        /// <param name="objMemberId"></param>
        /// <param name="objRelId"></param>
        /// <param name="enmRelType"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> LogDebugAsync(String strMessage, LogArea enmArea = LogArea.None, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, CancellationToken objToken = default);

        /// <summary>
        /// Cleans up logs.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<BasicResult> CleanLogsAsync(CancellationToken objToken);

        Task<Boolean> LogObjectAsync<T>(
            LogType enmType,
            LogArea enmArea,
            T objValue,
            Guid? objMemberId = null,
            Guid? objRelId = null,
            ItemType enmRelType = ItemType.None,
            CancellationToken objToken = default);
    }
}
