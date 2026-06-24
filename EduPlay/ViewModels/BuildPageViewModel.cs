using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlay.Models;
using EduPlay.Services;
using EduPlay.Views;

namespace EduPlay.ViewModels;

public partial class BuildPageViewModel : BaseViewModel
{
    private readonly GameGeneratorService _generatorService;
    private readonly GameLibraryService _libraryService;

    private CancellationTokenSource? _cancellationTokenSource;

    // ── Categories & Age Groups ──────────────────────────────────────────

    public List<GameCategory> Categories { get; } = GameCategory.AllCategory;
    public List<AgeGroup> AgeGroups { get; } = AgeGroup.AllAgeGroup;

    [ObservableProperty]
    private GameCategory _selectedCategory = GameCategory.AllCategory[0];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExamplePrompts))]
    private GameCategory _selectedCategoryForExamples = GameCategory.AllCategory[0];

    [ObservableProperty]
    private AgeGroup _selectedAgeGroup = AgeGroup.AllAgeGroup[1]; // Default: 7-9

    // ── Prompt ───────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _prompt = string.Empty;

    public List<string> ExamplePrompts => _examplePromptMap
        .GetValueOrDefault(SelectedCategory?.Id ?? "literacy", []);

    private readonly Dictionary<string, List<string>> _examplePromptMap = new()
    {
        ["literacy"] =
        [
            "Spelling game using ocean animal words",
            "Vocabulary matching for ESL learners — fruit and vegetables",
            "Phonics game for the letters B, C, D and F with animal sounds",
        ],
        ["numbers"] =
        [
            "Times tables practice — 3, 4, and 5 times tables",
            "Counting and sorting farm animals into groups of 5",
            "Missing number sequences up to 100",
        ],
        ["memory"] =
        [
            "Matching pairs game with parts of the human body",
            "Label the water cycle diagram",
            "Sequence memory game with colours and shapes",
        ],
    };

    // ── Generation state ─────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotGenerating))]
    private bool _isGenerating;

    public bool IsNotGenerating => !IsGenerating;

    [ObservableProperty]
    private string _progressLabel = string.Empty;

    [ObservableProperty]
    private int _progressStep;

    [ObservableProperty]
    private string? _errorMessage;

    // ── Constructor ──────────────────────────────────────────────────────

    public BuildPageViewModel(
        GameGeneratorService generatorService,
        GameLibraryService libraryService)
    {
        _generatorService = generatorService;
        _libraryService = libraryService;
        Title = "Build a Game";
    }

    // ── Commands ─────────────────────────────────────────────────────────

    [RelayCommand]
    private void SelectCategory(GameCategory category)
    {
        SelectedCategory = category;
        SelectedCategoryForExamples = category;
        OnPropertyChanged(nameof(ExamplePrompts));
    }

    [RelayCommand]
    private void SelectAgeGroup(AgeGroup ageGroup)
    {
        SelectedAgeGroup = ageGroup;
    }

    [RelayCommand]
    private void UseExamplePrompt(string example)
    {
        Prompt = example;
    }

    [RelayCommand]
    private async Task GenerateAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt))
        {
            await Shell.Current.DisplayAlertAsync(
                "Add a description",
                "Tell us what kind of game you want to build.",
                "OK");
            return;
        }

        ErrorMessage = null;
        IsGenerating = true;
        ProgressStep = 1;
        ProgressLabel = "Starting...";

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            GeneratedGame? result = null;

            await foreach (var (progress, game, error) in
                _generatorService.GenerateAsync(
                    userId: "user-001",  // Replace with real auth
                    prompt: Prompt,
                    category: SelectedCategory.Id,
                    ageGroup: SelectedAgeGroup.Id,
                    cancellationToken: _cancellationTokenSource.Token))
            {
                if (progress is not null)
                {
                    ProgressStep = progress.Step;
                    ProgressLabel = progress.Label;
                }

                if (error is not null)
                {
                    ErrorMessage = error;
                    break;
                }

                if (game is not null)
                {
                    result = game;
                    break;
                }
            }

            if (result is not null)
            {
                // Save to local library
                await _libraryService.SaveGameAsync(result);

                // Navigate to game screen
                await Shell.Current.GoToAsync(
                    $"{nameof(GamePage)}",
                    new Dictionary<string, object>
                    {
                        ["Game"] = result,
                    });
            }
        }
        catch (OperationCanceledException)
        {
            // User cancelled — no error to show
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Generation failed: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
            ProgressStep = 0;
            ProgressLabel = string.Empty;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand]
    private void CancelGeneration()
    {
        _cancellationTokenSource?.Cancel();
    }
}
