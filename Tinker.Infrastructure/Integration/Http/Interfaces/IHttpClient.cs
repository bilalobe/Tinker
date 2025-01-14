namespace Tinker.Infrastructure.Integration.Http.Interfaces;

public interface IHttpClient<TDto> where TDto : class
{
    Task<TDto?> GetAsync(string                 endpoint);
    Task<IEnumerable<TDto>> GetListAsync(string endpoint);
    Task<TDto> PostAsync(string                 endpoint, TDto data);
    Task<TDto> PutAsync(string                  endpoint, TDto data);
    Task DeleteAsync(string                     endpoint);

    Task<TDto?> GetAsync(string endpoint, Dictionary<string, string>? queryParams = null);
    Task<TDto?> GetAsync(string endpoint, object                      queryParams);

    Task<IEnumerable<TDto>> GetListAsync(string endpoint, Dictionary<string, string>? queryParams = null);
    Task<IEnumerable<TDto>> GetListAsync(string endpoint, object                      queryParams);

    Task<TDto> PostAsync(string                 endpoint, TDto data, Dictionary<string, string>? headers = null);
    Task<TResponse> PostAsync<TResponse>(string endpoint, TDto data) where TResponse : class;

    Task<Stream> GetStreamAsync(string    endpoint);
    Task<byte[]> GetByteArrayAsync(string endpoint);
}