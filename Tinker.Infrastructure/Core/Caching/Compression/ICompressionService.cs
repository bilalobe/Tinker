namespace Tinker.Infrastructure.Core.Caching.Compression;

public interface ICompressionService
{
    Task<byte[]?> CompressAsync(string   data, CancellationToken cancellationToken = default);
    Task<string?> DecompressAsync(byte[] data, CancellationToken cancellationToken = default);
    bool ShouldCompress(string           data, int               threshold         = 1024);
}