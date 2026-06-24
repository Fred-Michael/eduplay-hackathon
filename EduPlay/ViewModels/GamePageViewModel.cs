using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlay.Models;

namespace EduPlay.ViewModels;

[QueryProperty(nameof(Game), "Game")]
public partial class GamePageViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GameUrl))]
    private GeneratedGame? _game;

    [ObservableProperty]
    private bool _isWebViewLoading = true;

    public string? GameUrl => Game?.GameUrl;

    public GamePageViewModel()
    {
        Title = "Your Game";
    }

    [RelayCommand]
    private async Task ShareGameAsync()
    {
        if (Game is null) return;

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Play this EduPlay game!",
            Text = $"Here's a learning game I built:\n\n{Game.GameUrl}",
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}