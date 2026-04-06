using GrailJobApi.Shared.Configuration;
using GrailJobApi.Shared.Json;
using Microsoft.Extensions.Options;

namespace GrailJobApi.Shared.Ai;

public sealed class OpenAiStructuredChatClient(HttpClient httpClient, IOptions<OpenAiOptions> options)
{
    private readonly OpenAiOptions _options = options.Value;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);
    public bool UseMockFallback => _options.UseMockWhenApiKeyMissing;

    public async Task<T> GetStructuredResponseAsync<T>(string schemaName, object schema, string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("OpenAI is not configured. Provide an API key or enable the mock fallback in development.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/chat/completions");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var payload = new
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

        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonDefaults.SerializerOptions), Encoding.UTF8, "application/json");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

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
}
