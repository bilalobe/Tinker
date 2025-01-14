using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Monitoring.Health.Models;

namespace Tinker.Infrastructure.Monitoring.Health.Checks;

public class DatabaseHealthCheck(
    IApplicationDbContext        context,
    ILogger<DatabaseHealthCheck> logger,
    HealthCheckOptions           options)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context1,
        CancellationToken  cancellationToken = default)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            using var cts = new CancellationTokenSource(options.DatabaseTimeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            // Test database connectivity
            await context.Database.CanConnectAsync(linkedCts.Token);

            var connectionStats = await GetConnectionPoolStatsAsync(linkedCts.Token);
            sw.Stop();

            return new HealthCheckResult(
                HealthStatus.Healthy,
                "Database is healthy",
                data: new Dictionary<string, object>
                {
                    { "ResponseTime", sw.ElapsedMilliseconds },
                    { "OpenConnections", connectionStats.OpenConnections },
                    { "MaxPoolSize", connectionStats.MaxPoolSize },
                    { "ActiveConnections", connectionStats.ActiveConnections }
                });
        }
        catch (OperationCanceledException)
        {
            return new HealthCheckResult(
                context1.Registration.FailureStatus,
                "Database health check timed out");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database health check failed");
            return new HealthCheckResult(
                context1.Registration.FailureStatus,
                "Database is unhealthy",
                ex);
        }
    }

    private async Task<(int OpenConnections, int MaxPoolSize, int ActiveConnections)>
        GetConnectionPoolStatsAsync(CancellationToken cancellationToken)
    {
        await using var conn = context.Database.GetDbConnection();
        var poolCounters = await GetPoolCountersAsync(conn);

        return (
            poolCounters.OpenConnections,
            poolCounters.MaxPoolSize,
            poolCounters.ActiveConnections
        );
    }

    private async Task<dynamic> GetPoolCountersAsync(DbConnection connection)
    {
        // This is SQL Server specific - adapt for other databases
        const string sql = @"
            SELECT 
                COUNT(*) as OpenConnections,
                @@MAX_CONNECTIONS as MaxPoolSize,
                (SELECT COUNT(*) FROM sys.dm_exec_sessions) as ActiveConnections";

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new
        {
            OpenConnections = reader.GetInt32(0),
            MaxPoolSize = reader.GetInt32(1),
            ActiveConnections = reader.GetInt32(2)
        };
    }
}