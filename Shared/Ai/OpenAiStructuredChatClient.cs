using GrailJobApi.Shared.Configuration;
using GrailJobApi.Shared.Json;
using Microsoft.Extensions.Options;

namespace GrailJobApi.Shared.Ai;

public sealed class OpenAiStructuredChatClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiOptions _options;
    private readonly string _apiKey;

    public OpenAiStructuredChatClient(HttpClient httpClient, IOptions<OpenAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _apiKey = _options.ResolveApiKey();
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);
    public bool UseMockFallback => _options.UseMockWhenApiKeyMissing;

    public async Task<T> GetStructuredResponseAsync<T>(string schemaName, object schema, string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("OpenAI is not configured. Provide OpenAi:ApiKeyFile or enable the mock fallback in development.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/chat/completions");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = BuildPayload(schemaName, schema, systemPrompt, userPrompt);

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonDefaults.SerializerOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"OpenAI request failed with status {(int)response.StatusCode} ({response.StatusCode}). Body: {errorBody}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var content = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("OpenAI returned an empty content payload.");
        }

        var result = JsonSerializer.Deserialize<T>(content, JsonDefaults.SerializerOptions);
        if (result is null)
        {
            throw new InvalidOperationException("OpenAI response could not be deserialized.");
        }

        return result;
    }

    private object BuildPayload(string schemaName, object schema, string systemPrompt, string userPrompt)
    {
        return _options.Model switch
        {
            OpenAiModelKind.Gpt54 => BuildJsonSchemaPayload(schemaName, schema, systemPrompt, userPrompt),
            OpenAiModelKind.Gpt5Mini => BuildJsonObjectPayload(schema, systemPrompt, userPrompt),
            OpenAiModelKind.O4Mini => BuildJsonObjectPayload(schema, systemPrompt, userPrompt),
            _ => BuildJsonSchemaPayload(schemaName, schema, systemPrompt, userPrompt)
        };
    }

    private object BuildJsonSchemaPayload(string schemaName, object schema, string systemPrompt, string userPrompt) =>
        new
        {
            model = _options.ResolveModelId(),
            temperature = _options.Temperature,
            max_completion_tokens = _options.MaxOutputTokens,
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            response_format = new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = schemaName,
                    strict = true,
                    schema
                }
            }
        };

    private object BuildJsonObjectPayload(object schema, string systemPrompt, string userPrompt) =>
        new
        {
            model = _options.ResolveModelId(),
            max_completion_tokens = _options.MaxOutputTokens,
            messages = new object[]
            {
                new { role = "system", content = BuildJsonObjectSystemPrompt(systemPrompt, schema) },
                new { role = "user", content = userPrompt }
            },
            response_format = new
            {
                type = "json_object"
            }
        };

    private static string BuildJsonObjectSystemPrompt(string systemPrompt, object schema)
    {
        var serializedSchema = JsonSerializer.Serialize(schema, JsonDefaults.SerializerOptions);

        return $$"""
{{systemPrompt}}

Contraintes supplémentaires :
- Retourner uniquement un objet JSON valide.
- Ne jamais retourner de markdown.
- Le JSON retourné doit respecter strictement ce schéma :
{{serializedSchema}}
""";
    }
}