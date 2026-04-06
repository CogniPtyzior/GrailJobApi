namespace GrailJobApi.Shared.Json;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
