using System.Net;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Infrastructure.Http;

namespace StreamsMS.Tests.Fake{
    public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseGenerator;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseGenerator)
    {
        _responseGenerator = responseGenerator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_responseGenerator(request));
    }
}
}