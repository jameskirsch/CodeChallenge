using System.IO;
using Newtonsoft.Json;

namespace CodeChallenge.Tests.Integration.Helpers;

public class JsonSerialization
{
    private readonly JsonSerializer _serializer = JsonSerializer.CreateDefault();

    public string ToJson<T>(T obj)
    {
        var json = string.Empty;

        if (obj == null) return json;

        using var sw = new StringWriter();
        using var jtw = new JsonTextWriter(sw);
        _serializer.Serialize(jtw, obj);
        json = sw.ToString();

        return json;
    }
}