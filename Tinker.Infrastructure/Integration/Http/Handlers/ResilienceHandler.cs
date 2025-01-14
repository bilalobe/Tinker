namespace Tinker.Infrastructure.Integration.Http.Handlers;

public class ResilienceHandler : DelegatingHandler
{
    private readonly IHttpResiliencePipeline _pipeline;

    public ResilienceHandler(IHttpResiliencePipeline pipeline)
    {
        _pipeline = pipeline;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(ct => base.SendAsync(request, ct));
    }
}