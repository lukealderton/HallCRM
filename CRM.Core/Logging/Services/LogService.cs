using CRM.Contracts.Results;
using CRM.Core.Common.Domain;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Logging.Domain;
using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;

namespace CRM.Core.Logging.Services
{
    public sealed class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository objLogRepository)
        {
            _logRepository = objLogRepository;
        }

        ///<inheritdoc/>
        public Task<Log?> GetLogAsync(Guid objLogId, CancellationToken objToken = default)
            => _logRepository.GetLogAsync(objLogId, objToken);

        ///<inheritdoc/>
        public Task<SearchResult<Log>> GetLogsAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            LogType enmType,
            LogArea enmArea,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default)
            => _logRepository.GetLogsAsync(strKeyword, objStartDate, objEndDate, enmType, enmArea, intPageSize, intSkip, objToken);

        ///<inheritdoc/>
        public Task<SearchResult<Log>> GetLogsByTypeAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            LogType enmType,
            LogArea enmArea,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default)
            => _logRepository.GetLogsByTypeAsync(strKeyword, objStartDate, objEndDate, enmType, enmArea, intPageSize, intSkip, objToken);

        ///<inheritdoc/>
        public Task<SearchResult<Log>> GetLogsByTypeAsync(LogType enmType, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default)
            => _logRepository.GetLogsByTypeAsync(enmType, intPageSize, intSkip, objToken);

        ///<inheritdoc/>
        public Task<SearchResult<Log>> GetLogsByAreaAsync(DateTime objStartDate, LogArea enmArea, CancellationToken objToken = default)
            => _logRepository.GetLogsByAreaAsync(objStartDate, enmArea, objToken);

        ///<inheritdoc/>
        public Task<SearchResult<Log>> GetLogsAsync(String? strKeyword, DateTime objStartDate, DateTime objEndDate, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default)
            => _logRepository.GetLogsAsync(strKeyword, objStartDate, objEndDate, intPageSize, intSkip, objToken);

        ///<inheritdoc/>
        public Task<SearchResult<Log>> GetLogsForMemberAsync(Guid objMemberId, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default)
            => _logRepository.GetLogsForMemberAsync(objMemberId, intPageSize, intSkip, objToken);

        ///<inheritdoc/>
        public Task<Int32> CountLogsOfTypeAsync(LogType enmType = 0, LogArea enmArea = 0, DateTime? dtmStart = null, DateTime? dtmEnd = null, CancellationToken objToken = default)
            => _logRepository.CountLogsOfTypeAsync(enmType, enmArea, dtmStart, dtmEnd, objToken);

        ///<inheritdoc/>
        public Task<Boolean> LogAsync(LogType enmType, LogArea enmArea, String? strLogText = null, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, CancellationToken objToken = default)
            => LogAsync(enmType, enmArea, DateTime.UtcNow, strLogText, objMemberId, objRelId, enmRelType, objToken);

        ///<inheritdoc/>
        public async Task<Boolean> LogAsync(LogType enmType, LogArea enmArea, DateTime dtmLogTimestamp, String? strLogText = null, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, CancellationToken objToken = default)
        {
            if (objToken.IsCancellationRequested)
            {
                return false;
            }

            Log objLog = new()
            {
                Id = Guid.NewGuid(),
                Timestamp = dtmLogTimestamp,
                LogType = enmType,
                LogArea = enmArea,
                MemberId = objMemberId,
                RelId = objRelId,
                RelType = enmRelType,
                Text = strLogText ?? String.Empty
            };

            try
            {
                return await _logRepository.InsertLogAsync(objLog, objToken);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                // last resort: NEVER call LogErrorAsync here (can recurse if SQL is down)
                Console.Error.WriteLine($"[Logging failed] {objError}");
                return false;
            }
        }

        ///<inheritdoc/>
        public Task<Boolean> LogErrorAsync(
            Exception objError,
            LogArea enmArea = LogArea.None,
            Guid? objMemberId = null,
            Guid? objRelId = null,
            ItemType enmRelType = ItemType.None,
            String? strMessage = null,
            CancellationToken objToken = default)
        {
            String strLogMessage = String.Empty;

            if (!String.IsNullOrWhiteSpace(strMessage))
            {
                strLogMessage += "Message: " + strMessage + Environment.NewLine;
            }

            strLogMessage += "Exception Type: " + objError.GetType().FullName + Environment.NewLine;
            strLogMessage += "Exception: " + objError.Message + Environment.NewLine;
            strLogMessage += "Stack Trace: " + objError.StackTrace;

            if (objError.InnerException != null)
            {
                strLogMessage += Environment.NewLine;
                strLogMessage += "Inner Exception Type: " + objError.InnerException.GetType().FullName + Environment.NewLine;
                strLogMessage += "Inner Exception: " + objError.InnerException.Message + Environment.NewLine;
                strLogMessage += "Inner Stack Trace: " + objError.InnerException.StackTrace;
            }

            return LogAsync(LogType.Error, enmArea, strLogMessage, objMemberId, objRelId, enmRelType, objToken);
        }

        ///<inheritdoc/>
        public Task<Boolean> LogDebugAsync(String strMessage, LogArea enmArea = LogArea.None, Guid? objMemberId = null, Guid? objRelId = null, ItemType enmRelType = ItemType.None, CancellationToken objToken = default)
            => LogAsync(LogType.Debug, enmArea, strMessage, objMemberId, objRelId, enmRelType, objToken);

        ///<inheritdoc/>
        public async Task<BasicResult> CleanLogsAsync(CancellationToken objToken)
        {
            BasicResult objResult = new();

            if (objToken.IsCancellationRequested)
            {
                objResult.Message = "Log cleanup was cancelled.";
                return objResult;
            }

            try
            {
                Dictionary<LogType, Int32> colRetentionDaysByType = new()
                {
                    { LogType.Debug, 60 },
                    { LogType.System, 60 },
                    { LogType.Activity, 360 }
                };

                Int32 intTotalDeleted = 0;

                foreach (KeyValuePair<LogType, Int32> objRetentionRecord in colRetentionDaysByType)
                {
                    DateTime dtmCutoffUtc = DateTime.UtcNow.AddDays(-objRetentionRecord.Value);

                    Int32 intDeleted = await _logRepository.DeleteLogsOlderThanAsync(objRetentionRecord.Key, dtmCutoffUtc, objToken);
                    intTotalDeleted += intDeleted;

                    _ = await LogAsync(
                        LogType.System,
                        LogArea.Logs,
                        $"Deleted {intDeleted} '{objRetentionRecord.Key}' logs older than {objRetentionRecord.Value} days (before {dtmCutoffUtc:yyyy-MM-dd}).",
                        objToken: objToken);
                }

                Dictionary<LogArea, Int32> colRetentionDaysByArea = new()
                {
                    //{ LogArea.Finished, Finished.Constants.CleanupFinishedAfterXDays }
                };

                foreach (KeyValuePair<LogArea, Int32> objRetentionRecord in colRetentionDaysByArea)
                {
                    DateTime dtmCutoffUtc = DateTime.UtcNow.AddDays(-objRetentionRecord.Value);

                    Int32 intDeleted = await _logRepository.DeleteLogsOlderThanAsync(objRetentionRecord.Key, dtmCutoffUtc, objToken);
                    intTotalDeleted += intDeleted;

                    _ = await LogAsync(
                        LogType.System,
                        LogArea.Logs,
                        $"Deleted {intDeleted} '{objRetentionRecord.Key}' logs older than {objRetentionRecord.Value} days (before {dtmCutoffUtc:yyyy-MM-dd}).",
                        objToken: objToken);
                }

                objResult.Success = true;
                objResult.Message = $"{intTotalDeleted} logs cleaned up.";
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                _ = await LogErrorAsync(objError, LogArea.Logs, null, null, ItemType.None, "Failed to CleanLogsAsync", objToken);
                objResult.Message = "Failed to clean logs: " + objError.Message;
            }

            return objResult;
        }

        ///<inheritdoc/>
        public Task<Boolean> LogObjectAsync<T>(
            LogType enmType,
            LogArea enmArea,
            T objValue,
            Guid? objMemberId = null,
            Guid? objRelId = null,
            ItemType enmRelType = ItemType.None,
            CancellationToken objToken = default)
        {
            String strLogText = System.Text.Json.JsonSerializer.Serialize(objValue);

            return LogAsync(
                enmType,
                enmArea,
                strLogText,
                objMemberId,
                objRelId,
                enmRelType,
                objToken);
        }
    }
}