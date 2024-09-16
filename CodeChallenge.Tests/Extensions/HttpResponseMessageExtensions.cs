using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace CodeChallenge.Tests.Integration.Extensions;

public static class HttpResponseMessageExtensions
{
    public static T DeserializeContent<T>(this HttpResponseMessage message)
    {
        var responseObject = default(T);
        if (message == null) return responseObject;

        var responseJson = message.Content.ReadAsStringAsync().Result;
        responseObject = JsonSerializer.CreateDefault().Deserialize<T>(
            new JsonTextReader(new StringReader(responseJson)));

        return responseObject;
    }
}