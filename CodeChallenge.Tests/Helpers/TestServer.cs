using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CodeChallenge.Tests.Integration.Helpers;

public class TestServer : IDisposable, IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _applicationFactory = new();

    public HttpClient NewClient()
    {
        return _applicationFactory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        await ((IAsyncDisposable)_applicationFactory).DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        ((IDisposable)_applicationFactory).Dispose();
        GC.SuppressFinalize(this);
    }
}