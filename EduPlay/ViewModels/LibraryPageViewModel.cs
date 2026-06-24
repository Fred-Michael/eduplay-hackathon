using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlay.Models;
using EduPlay.Services;
using EduPlay.Views;
using System.Collections.ObjectModel;

namespace EduPlay.ViewModels;

public partial class LibraryPageViewModel : BaseViewModel
{
    private readonly GameLibraryService _libraryService;

    public ObservableCollection<GeneratedGame> Games { get; } = [];

    [ObservableProperty]
    private bool _isEmpty;

    public LibraryPageViewModel(GameLibraryService libraryService)
    {
        _libraryService = libraryService;
        Title = "My Games";
    }

    [RelayCommand]
    private async Task LoadGamesAsync()
    {
        IsBusy = true;
        try
        {
            var games = await _libraryService.GetAllAsync();
            Games.Clear();
            foreach (var game in games)
                Games.Add(game);

            IsEmpty = Games.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenGameAsync(GeneratedGame game)
    {
        await Shell.Current.GoToAsync(
            nameof(GamePage),
            new Dictionary<string, object> { ["Game"] = game });
    }

    [RelayCommand]
    private async Task DeleteGameAsync(GeneratedGame game)
    {
        bool confirmed = await Shell.Current.DisplayAlertAsync(
            "Delete game",
            $"Remove '{game.Prompt.Truncate(40)}' from your library?",
            "Delete", "Cancel");

        if (!confirmed) return;

        await _libraryService.DeleteGameAsync(game.GameId);
        Games.Remove(game);
        IsEmpty = Games.Count == 0;
    }
}

// String extension helper
public static class StringExtensions
{
    public static string Truncate(this string s, int maxLength) =>
        s.Length <= maxLength ? s : s[..maxLength] + "...";
}