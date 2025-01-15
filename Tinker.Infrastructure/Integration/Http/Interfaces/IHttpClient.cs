namespace Tinker.Infrastructure.Integration.Http.Interfaces;

public interface IHttpClient<TDto> where TDto : class
{
    Task<TDto?> GetAsync(string endpoint, object queryParams);
    Task<TDto?> GetAsync(string endpoint);
    Task<TDto> PutAsync(string endpoint, TDto data);
    Task DeleteAsync(string endpoint);


    Task<IEnumerable<TDto>> GetListAsync(string endpoint, Dictionary<string, string>? queryParams = null);
    Task<IEnumerable<TDto>> GetListAsync(string endpoint, object queryParams);

    Task<TResponse> PostAsync<TResponse>(string endpoint, TDto data) where TResponse : class;

    Task<Stream> GetStreamAsync(string endpoint);
    Task<byte[]> GetByteArrayAsync(string endpoint);
}