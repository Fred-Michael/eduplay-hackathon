using EduPlay.Models;
using System.Text.Json;

namespace EduPlay.Services;

/// <summary>
/// Persists the user's game library to local device storage.
/// Games are saved as JSON files in the app's data directory.
/// </summary>
public class GameLibraryService
{
    private readonly string _libraryFilePath;
    private List<GeneratedGame> _cache = [];

    public GameLibraryService()
    {
        _libraryFilePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "game_library.json");
    }

    public async Task<List<GeneratedGame>> GetAllAsync()
    {
        if (_cache.Count > 0) return _cache;

        if (!File.Exists(_libraryFilePath))
            return [];

        var json = await File.ReadAllTextAsync(_libraryFilePath);
        _cache = JsonSerializer.Deserialize<List<GeneratedGame>>(json) ?? [];
        return _cache;
    }

    public async Task SaveGameAsync(GeneratedGame game)
    {
        var library = await GetAllAsync();

        // Insert at top (most recent first)
        library.Insert(0, game);

        // Keep last 50 games
        if (library.Count > 50)
            library = library.Take(50).ToList();

        _cache = library;

        var json = JsonSerializer.Serialize(library, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        await File.WriteAllTextAsync(_libraryFilePath, json);
    }

    public async Task DeleteGameAsync(string gameId)
    {
        var library = await GetAllAsync();
        library.RemoveAll(g => g.GameId == gameId);
        _cache = library;

        var json = JsonSerializer.Serialize(library);
        await File.WriteAllTextAsync(_libraryFilePath, json);
    }
}
