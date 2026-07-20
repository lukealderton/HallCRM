using CRM.Core.Common.Domain;
using CRM.Core.Logging.Domain;
using CRM.Primitives.Logging.Enums;

namespace CRM.Core.Logging.Abstraction
{
    /// <summary>
    /// Persistence and query abstraction for logs.
    /// Implementations live in Infrastructure (e.g. SQL Server).
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// Get a single log entry by its id.
        /// </summary>
        Task<Log?> GetLogAsync(
            Guid objLogId,
            CancellationToken objToken = default);

        /// <summary>
        /// Get logs with optional keyword, date range, type and area filtering (paged).
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
        /// Get logs by type with optional keyword, date range and area filtering.
        /// </summary>
        Task<SearchResult<Log>> GetLogsByTypeAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            LogType enmType,
            LogArea enmArea,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default);

        /// <summary>
        /// Get logs by type only (paged).
        /// </summary>
        Task<SearchResult<Log>> GetLogsByTypeAsync(
            LogType enmType,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default);

        /// <summary>
        /// Get logs by area since a given date.
        /// </summary>
        Task<SearchResult<Log>> GetLogsByAreaAsync(
            DateTime objStartDate,
            LogArea enmArea,
            CancellationToken objToken = default);

        /// <summary>
        /// Get logs in a date range with optional keyword filtering.
        /// </summary>
        Task<SearchResult<Log>> GetLogsAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default);

        /// <summary>
        /// Get logs for a specific member (paged).
        /// </summary>
        Task<SearchResult<Log>> GetLogsForMemberAsync(
            Guid objMemberId,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default);

        /// <summary>
        /// Count logs with optional type, area and date filtering.
        /// </summary>
        Task<Int32> CountLogsOfTypeAsync(
            LogType enmType = 0,
            LogArea enmArea = 0,
            DateTime? dtmStart = null,
            DateTime? dtmEnd = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Insert a new log entry.
        /// </summary>
        Task<Boolean> InsertLogAsync(
            Log objLog,
            CancellationToken objToken = default);

        /// <summary>
        /// Delete logs of a specific type older than the cutoff date (UTC).
        /// Used by higher-level cleanup policies.
        /// </summary>
        Task<Int32> DeleteLogsOlderThanAsync(
            LogType enmType,
            DateTime dtmCutoffUtc,
            CancellationToken objToken = default);

        /// <summary>
        /// Delete logs of a specific area older than the cutoff date (UTC).
        /// Used by higher-level cleanup policies.
        /// </summary>
        Task<Int32> DeleteLogsOlderThanAsync(
            LogArea enmArea,
            DateTime dtmCutoffUtc,
            CancellationToken objToken = default);
    }
}
