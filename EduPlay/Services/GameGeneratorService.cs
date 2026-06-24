using EduPlay.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EduPlay.Services;

public class GameGeneratorService
{
    // ── Replace with your Alibaba Function Compute URL after deployment ──
    // For local dev, use your machine's LAN IP, e.g. http://192.168.1.42:3001
    private const string BackendBaseUrl = "http://YOUR_BACKEND_IP:3001";

    private readonly HttpClient _http;

    public GameGeneratorService()
    {
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(3),
        };
    }

    /// <summary>
    /// Streams generation progress events from the backend SSE endpoint.
    /// Yields GenerationProgress items while generating, then a final
    /// GeneratedGame when complete. Uses IAsyncEnumerable for clean streaming.
    /// </summary>
    public async IAsyncEnumerable<(GenerationProgress? Progress, GeneratedGame? Result, string? Error)>
        GenerateAsync(
            string userId,
            string prompt,
            string category,
            string ageGroup,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            userId,
            prompt,
            category,
            ageGroup,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{BackendBaseUrl}/generate")
        {
            Content = JsonContent.Create(payload),
        };

        // SSE requires streaming response
        using var response = await _http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            yield return (null, null, $"Server error: {response.StatusCode}");
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: "))
                continue;

            var json = line["data: ".Length..];

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch
            {
                continue;
            }

            using (doc)
            {
                var type = doc.RootElement.GetProperty("type").GetString();

                switch (type)
                {
                    case "progress":
                        var step = doc.RootElement.GetProperty("step").GetInt32();
                        var label = doc.RootElement.GetProperty("label").GetString() ?? "";
                        yield return (new GenerationProgress(step, label), null, null);
                        break;

                    case "complete":
                        var game = new GeneratedGame
                        {
                            GameId = doc.RootElement.GetProperty("gameId").GetString() ?? "",
                            GameUrl = doc.RootElement.GetProperty("gameUrl").GetString() ?? "",
                            Category = category,
                            AgeGroup = ageGroup,
                            Prompt = prompt,
                            MechanicName = doc.RootElement
                                .TryGetProperty("gameSpec", out var spec)
                                ? spec.TryGetProperty("mechanicName", out var mn)
                                    ? mn.GetString() ?? ""
                                    : ""
                                : "",
                            CreatedAt = DateTime.UtcNow,
                        };
                        yield return (null, game, null);
                        yield break;

                    case "error":
                        var msg = doc.RootElement.GetProperty("message").GetString();
                        yield return (null, null, msg);
                        yield break;
                }
            }
        }
    }
}
