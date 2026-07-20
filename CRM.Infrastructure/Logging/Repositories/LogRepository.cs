using CRM.Core.Common.Configuration;
using CRM.Core.Common.Domain;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Logging.Domain;
using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;

namespace CRM.Infrastructure.Logging.Repositories
{
    public sealed class SqlLogRepository : ILogRepository
    {
        private readonly CRMConfiguration _configuration;

        public SqlLogRepository(IOptions<CRMConfiguration> objConfiguration)
        {
            _configuration = objConfiguration.Value;
        }

        private static Log GetLogFromReader(SqlDataReader objReader)
        {
            Log objLog = new();

            objLog.Id        = objReader.GetGuid("logId");
            objLog.Timestamp = objReader.GetDateTime("logTimestamp");
            objLog.LogType   = (LogType)objReader.GetInt32("logType");
            objLog.LogArea   = (LogArea)objReader.GetInt32("logArea");
            objLog.MemberId  = !objReader.IsDBNull("logMemberId") ? objReader.GetGuid("logMemberId") : null;
            objLog.RelId     = !objReader.IsDBNull("logRelId") ? objReader.GetGuid("logRelId") : null;
            objLog.RelType   = !objReader.IsDBNull("logRelType") ? (ItemType)objReader.GetInt32("logRelType") : ItemType.None;
            objLog.Text      = !objReader.IsDBNull("logText") ? objReader.GetString("logText") : "";

            return objLog;
        }

        public async Task<Log?> GetLogAsync(Guid objLogId, CancellationToken objToken = default)
        {
            if (objToken.IsCancellationRequested)
            {
                return null;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery = "SELECT * FROM T_Log WHERE logId = @logId";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = objLogId;

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (await objReader.ReadAsync(objToken))
                            {
                                return GetLogFromReader(objReader);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogAsync failed] logId={objLogId} err={objError}");
            }

            return null;
        }

        public async Task<SearchResult<Log>> GetLogsAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            LogType enmType,
            LogArea enmArea,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default)
        {
            SearchResult<Log> objResult = new();

            if (objToken.IsCancellationRequested)
            {
                return objResult;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    StringBuilder objSql = new();

                    objSql.AppendLine("SELECT T_Log.*, TotalRows = COUNT(*) OVER()");
                    objSql.AppendLine("FROM T_Log");
                    objSql.AppendLine("WHERE logTimestamp >= @startDate");
                    objSql.AppendLine("AND logTimestamp < @endDate");

                    if ((Int32)enmType != 0)
                    {
                        objSql.AppendLine("AND logType = @logType");
                    }

                    if ((Int32)enmArea != 0)
                    {
                        objSql.AppendLine("AND logArea = @logArea");
                    }

                    if (!String.IsNullOrWhiteSpace(strKeyword))
                    {
                        objSql.AppendLine("AND logText LIKE '%' + @keyword + '%'");
                    }

                    objSql.AppendLine("ORDER BY logTimestamp DESC");
                    objSql.AppendLine("OFFSET @skip ROWS");
                    objSql.AppendLine("FETCH NEXT @take ROWS ONLY");

                    using (SqlCommand objCmd = new(objSql.ToString(), objConnection))
                    {
                        objCmd.Parameters.Add("@startDate", SqlDbType.DateTime2).Value = objStartDate.ToUniversalTime();
                        objCmd.Parameters.Add("@endDate", SqlDbType.DateTime2).Value = objEndDate.ToUniversalTime();
                        objCmd.Parameters.Add("@skip", SqlDbType.Int).Value = intSkip;
                        objCmd.Parameters.Add("@take", SqlDbType.Int).Value = intPageSize;

                        if ((Int32)enmType != 0)
                        {
                            objCmd.Parameters.Add("@logType", SqlDbType.Int).Value = (Int32)enmType;
                        }

                        if ((Int32)enmArea != 0)
                        {
                            objCmd.Parameters.Add("@logArea", SqlDbType.Int).Value = (Int32)enmArea;
                        }

                        if (!String.IsNullOrWhiteSpace(strKeyword))
                        {
                            objCmd.Parameters.Add("@keyword", SqlDbType.NVarChar, 4000).Value = strKeyword.Trim();
                        }

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (!objReader.HasRows)
                            {
                                objResult.TotalCount = 0;
                                return objResult;
                            }

                            while (await objReader.ReadAsync(objToken))
                            {
                                objResult.Items.Add(GetLogFromReader(objReader));

                                if (objResult.TotalCount == 0)
                                {
                                    objResult.TotalCount = Convert.ToInt32(objReader["TotalRows"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogsAsync filtered failed] type={enmType} area={enmArea} err={objError}");
            }

            return objResult;
        }

        public async Task<SearchResult<Log>> GetLogsByTypeAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            LogType enmType,
            LogArea enmArea,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default)
        {
            SearchResult<Log> objResult = new();

            if (objToken.IsCancellationRequested)
            {
                return objResult;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery =
                        "SELECT T_Log.*, TotalRows = count(*) OVER() " +
                        "FROM T_Log " +
                        "WHERE logType = @logType AND logTimestamp > @startDate AND logTimestamp < @endDate ";

                    if (enmArea != LogArea.None)
                    {
                        strQuery += "AND logArea = @logArea ";
                    }

                    if (!String.IsNullOrWhiteSpace(strKeyword))
                    {
                        strQuery += "AND logText LIKE '%' + @keyword + '%' ";
                    }

                    strQuery +=
                        "ORDER BY logTimestamp DESC " +
                        "OFFSET @skip ROWS " +
                        "FETCH NEXT @take ROWS ONLY ";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logType", SqlDbType.Int).Value = (Int32)enmType;
                        objCmd.Parameters.Add("@startDate", SqlDbType.DateTime2).Value = objStartDate.ToUniversalTime();
                        objCmd.Parameters.Add("@endDate", SqlDbType.DateTime2).Value = objEndDate.ToUniversalTime();
                        objCmd.Parameters.Add("@skip", SqlDbType.Int).Value = intSkip;
                        objCmd.Parameters.Add("@take", SqlDbType.Int).Value = intPageSize;

                        if (enmArea != LogArea.None)
                        {
                            objCmd.Parameters.Add("@logArea", SqlDbType.Int).Value = (Int32)enmArea;
                        }

                        if (!String.IsNullOrWhiteSpace(strKeyword))
                        {
                            objCmd.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = strKeyword;
                        }

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (!objReader.HasRows)
                            {
                                objResult.TotalCount = 0;
                                return objResult;
                            }

                            while (await objReader.ReadAsync(objToken))
                            {
                                objResult.Items.Add(GetLogFromReader(objReader));

                                if (objResult.TotalCount == 0)
                                {
                                    objResult.TotalCount = Convert.ToInt32(objReader["TotalRows"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogsByTypeAsync failed] type={enmType} area={enmArea} err={objError}");
            }

            return objResult;
        }

        public async Task<SearchResult<Log>> GetLogsByTypeAsync(LogType enmType, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default)
        {
            SearchResult<Log> objResult = new();

            if (objToken.IsCancellationRequested)
            {
                return objResult;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery =
                        "SELECT T_Log.*, TotalRows = count(*) OVER() " +
                        "FROM T_Log " +
                        "WHERE logType = @logType " +
                        "ORDER BY logTimestamp DESC " +
                        "OFFSET @skip ROWS " +
                        "FETCH NEXT @take ROWS ONLY";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logType", SqlDbType.Int).Value = (Int32)enmType;
                        objCmd.Parameters.Add("@skip", SqlDbType.Int).Value = intSkip;
                        objCmd.Parameters.Add("@take", SqlDbType.Int).Value = intPageSize;

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (!objReader.HasRows)
                            {
                                objResult.TotalCount = 0;
                                return objResult;
                            }

                            while (await objReader.ReadAsync(objToken))
                            {
                                objResult.Items.Add(GetLogFromReader(objReader));

                                if (objResult.TotalCount == 0)
                                {
                                    objResult.TotalCount = Convert.ToInt32(objReader["TotalRows"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogsByTypeAsync (simple) failed] type={enmType} err={objError}");
            }

            return objResult;
        }

        public async Task<SearchResult<Log>> GetLogsByAreaAsync(DateTime objStartDate, LogArea enmArea, CancellationToken objToken = default)
        {
            SearchResult<Log> objResult = new();

            if (objToken.IsCancellationRequested)
            {
                return objResult;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery =
                        "SELECT T_Log.*, TotalRows = count(*) OVER() " +
                        "FROM T_Log " +
                        "WHERE logArea = @logArea AND logTimestamp > @startDate " +
                        "ORDER BY logTimestamp DESC ";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@startDate", SqlDbType.DateTime2).Value = objStartDate.ToUniversalTime();
                        objCmd.Parameters.Add("@logArea", SqlDbType.Int).Value = (Int32)enmArea;

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (!objReader.HasRows)
                            {
                                objResult.TotalCount = 0;
                                return objResult;
                            }

                            while (await objReader.ReadAsync(objToken))
                            {
                                objResult.Items.Add(GetLogFromReader(objReader));

                                if (objResult.TotalCount == 0)
                                {
                                    objResult.TotalCount = Convert.ToInt32(objReader["TotalRows"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogsByAreaAsync failed] area={enmArea} err={objError}");
            }

            return objResult;
        }

        public async Task<SearchResult<Log>> GetLogsAsync(
            String? strKeyword,
            DateTime objStartDate,
            DateTime objEndDate,
            Int32 intPageSize,
            Int32 intSkip,
            CancellationToken objToken = default)
        {
            SearchResult<Log> objResult = new();

            if (objToken.IsCancellationRequested)
            {
                return objResult;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery =
                        @"SELECT T_Log.*, TotalRows = count(*) OVER()
                          FROM T_Log
                          WHERE logTimestamp > @startDate AND logTimestamp < @endDate ";

                    if (!String.IsNullOrWhiteSpace(strKeyword))
                    {
                        strQuery += "AND logText LIKE '%' + @keyword + '%' ";
                    }

                    strQuery +=
                        @"ORDER BY logTimestamp DESC
                          OFFSET @skip ROWS
                          FETCH NEXT @take ROWS ONLY ";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@startDate", SqlDbType.DateTime2).Value = objStartDate.ToUniversalTime();
                        objCmd.Parameters.Add("@endDate", SqlDbType.DateTime2).Value = objEndDate.ToUniversalTime();
                        objCmd.Parameters.Add("@skip", SqlDbType.Int).Value = intSkip;
                        objCmd.Parameters.Add("@take", SqlDbType.Int).Value = intPageSize;

                        if (!String.IsNullOrWhiteSpace(strKeyword))
                        {
                            objCmd.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = strKeyword;
                        }

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (!objReader.HasRows)
                            {
                                objResult.TotalCount = 0;
                                return objResult;
                            }

                            while (await objReader.ReadAsync(objToken))
                            {
                                objResult.Items.Add(GetLogFromReader(objReader));

                                if (objResult.TotalCount == 0)
                                {
                                    objResult.TotalCount = Convert.ToInt32(objReader["TotalRows"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogsAsync failed] err={objError}");
            }

            return objResult;
        }

        public async Task<SearchResult<Log>> GetLogsForMemberAsync(Guid objMemberId, Int32 intPageSize, Int32 intSkip, CancellationToken objToken = default)
        {
            SearchResult<Log> objResult = new();

            if (objToken.IsCancellationRequested)
            {
                return objResult;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery =
                        @"SELECT T_Log.*, TotalRows = count(*) OVER()
                          FROM T_Log
                          WHERE logMemberId = @logMemberId
                          ORDER BY logTimestamp DESC
                          OFFSET @skip ROWS
                          FETCH NEXT @take ROWS ONLY ";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logMemberId", SqlDbType.UniqueIdentifier).Value = objMemberId;
                        objCmd.Parameters.Add("@skip", SqlDbType.Int).Value = intSkip;
                        objCmd.Parameters.Add("@take", SqlDbType.Int).Value = intPageSize;

                        await objConnection.OpenAsync(objToken);

                        using (SqlDataReader objReader = await objCmd.ExecuteReaderAsync(objToken))
                        {
                            if (!objReader.HasRows)
                            {
                                objResult.TotalCount = 0;
                                return objResult;
                            }

                            while (await objReader.ReadAsync(objToken))
                            {
                                objResult.Items.Add(GetLogFromReader(objReader));

                                if (objResult.TotalCount == 0)
                                {
                                    objResult.TotalCount = Convert.ToInt32(objReader["TotalRows"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.GetLogsForMemberAsync failed] memberId={objMemberId} err={objError}");
            }

            return objResult;
        }

        public async Task<Int32> CountLogsOfTypeAsync(LogType enmType = 0, LogArea enmArea = 0, DateTime? dtmStart = null, DateTime? dtmEnd = null, CancellationToken objToken = default)
        {
            if (objToken.IsCancellationRequested)
            {
                return 0;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    StringBuilder objSb = new("SELECT COUNT(*) FROM T_Log WHERE (@logType = 0 OR logType = @logType) AND (@logArea = 0 OR logArea = @logArea)");
                    if (dtmStart.HasValue) objSb.Append(" AND logTimestamp >= @startDate");
                    if (dtmEnd.HasValue) objSb.Append(" AND logTimestamp <= @endDate");

                    using (SqlCommand objCmd = new(objSb.ToString(), objConnection))
                    {
                        objCmd.Parameters.Add("@logType", SqlDbType.Int).Value = (Int32)enmType;
                        objCmd.Parameters.Add("@logArea", SqlDbType.Int).Value = (Int32)enmArea;

                        if (dtmStart.HasValue)
                        {
                            objCmd.Parameters.Add("@startDate", SqlDbType.DateTime2).Value = dtmStart.Value.ToUniversalTime();
                        }

                        if (dtmEnd.HasValue)
                        {
                            objCmd.Parameters.Add("@endDate", SqlDbType.DateTime2).Value = dtmEnd.Value.ToUniversalTime();
                        }

                        await objConnection.OpenAsync(objToken);

                        Object? objScalar = await objCmd.ExecuteScalarAsync(objToken);
                        return Convert.ToInt32(objScalar);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.CountLogsOfTypeAsync failed] type={enmType} area={enmArea} err={objError}");
            }

            return 0;
        }

        public async Task<Boolean> InsertLogAsync(Log objLog, CancellationToken objToken = default)
        {
            if (objToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    String strQuery = @"
                        INSERT INTO T_Log (
                            logId,logTimestamp,logType,logArea,logMemberId,logRelId,logRelType,logText
	                    )
                        VALUES (
	                        @logId,@logTimestamp,@logType,@logArea,@logMemberId,@logRelId,@logRelType,@logText
	                    )";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = objLog.Id;
                        objCmd.Parameters.Add("@logTimestamp", SqlDbType.DateTime2).Value = objLog.Timestamp;
                        objCmd.Parameters.Add("@logMemberId", SqlDbType.UniqueIdentifier).Value = objLog.MemberId != null ? objLog.MemberId : DBNull.Value;
                        objCmd.Parameters.Add("@logRelId", SqlDbType.UniqueIdentifier).Value = objLog.RelId != null ? objLog.RelId : DBNull.Value;
                        objCmd.Parameters.Add("@logRelType", SqlDbType.Int).Value = (Int32)objLog.RelType;
                        objCmd.Parameters.Add("@logType", SqlDbType.Int).Value = (Int32)objLog.LogType;
                        objCmd.Parameters.Add("@logArea", SqlDbType.Int).Value = (Int32)objLog.LogArea;
                        objCmd.Parameters.Add("@logText", SqlDbType.NVarChar, -1).Value = !String.IsNullOrWhiteSpace(objLog.Text) ? objLog.Text : DBNull.Value;

                        await objConnection.OpenAsync(objToken);
                        return await objCmd.ExecuteNonQueryAsync(objToken) > 0;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.InsertLogAsync failed] err={objError}");
            }

            return false;
        }

        public async Task<Int32> DeleteLogsOlderThanAsync(LogType enmType, DateTime dtmCutoffUtc, CancellationToken objToken = default)
        {
            if (objToken.IsCancellationRequested)
            {
                return 0;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    await objConnection.OpenAsync(objToken);

                    String strQuery = @"
                        DELETE FROM T_Log
                        WHERE logType = @logType
                        AND logTimestamp < @cutoffDate";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logType", SqlDbType.Int).Value = (Int32)enmType;
                        objCmd.Parameters.Add("@cutoffDate", SqlDbType.DateTime2).Value = dtmCutoffUtc;

                        return await objCmd.ExecuteNonQueryAsync(objToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.DeleteLogsOlderThanAsync failed] type={enmType} cutoff={dtmCutoffUtc:O} err={objError}");
            }

            return 0;
        }

        public async Task<Int32> DeleteLogsOlderThanAsync(LogArea enmArea, DateTime dtmCutoffUtc, CancellationToken objToken = default)
        {
            if (objToken.IsCancellationRequested)
            {
                return 0;
            }

            try
            {
                using (SqlConnection objConnection = new(_configuration.DataConnectionString))
                {
                    await objConnection.OpenAsync(objToken);

                    String strQuery = @"
                        DELETE FROM T_Log
                        WHERE logArea = @logArea
                        AND logTimestamp < @cutoffDate";

                    using (SqlCommand objCmd = new(strQuery, objConnection))
                    {
                        objCmd.Parameters.Add("@logArea", SqlDbType.Int).Value = (Int32)enmArea;
                        objCmd.Parameters.Add("@cutoffDate", SqlDbType.DateTime2).Value = dtmCutoffUtc;

                        return await objCmd.ExecuteNonQueryAsync(objToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
            catch (Exception objError)
            {
                Console.Error.WriteLine($"[SqlLogRepository.DeleteLogsOlderThanAsync failed] area={enmArea} cutoff={dtmCutoffUtc:O} err={objError}");
            }

            return 0;
        }
    }
}
