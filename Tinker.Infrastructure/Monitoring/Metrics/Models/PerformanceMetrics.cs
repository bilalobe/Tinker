namespace Tinker.Infrastructure.Monitoring.Metrics.Models;

public static class PerformanceMetrics
{
    public const string ResponseTime = "response_time";
    public const string DatabaseQueryTime = "db_query_time";
    public const string CacheHitRate = "cache_hit_rate";
    public const string MemoryUsage = "memory_usage";
    public const string CpuUsage = "cpu_usage";
    public const string ActiveConnections = "active_connections";
    public const string RequestRate = "request_rate";
    public const string ErrorRate = "error_rate";
    public const string ManagedMemory = "managed_memory";
    public const string ThreadCount = "thread_count";
    public const string HandleCount = "handle_count";
    public const string GcGen0 = "gc_gen0";
    public const string GcGen1 = "gc_gen1";
    public const string GcGen2 = "gc_gen2";
    public const string DatabaseConnections = "db_connections";
    public const string DatabaseTransactions = "db_transactions";
    public const string HttpRequestRate = "http_request_rate";
    public const string HttpErrorRate = "http_error_rate";
    public const string ApiLatency = "api_latency";
    public const string CacheSize = "cache_size";
    public const string BackgroundJobDuration = "job_duration";
    public const string QueueLength = "queue_length";
    public const string ActiveSessions = "active_sessions";
    public const string DomainEventLatency = "domain_event_latency";
}