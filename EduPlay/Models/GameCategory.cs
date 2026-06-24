namespace EduPlay.Models;

public class GameCategory
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Emoji { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Color Color { get; init; } = Colors.White;
    public Color BackgroundColor { get; init; } = Colors.Transparent;

    public static readonly List<GameCategory> AllCategory =
    [
        new()
        {
            Id = "literacy",
            Label = "Language & Literacy",
            Emoji = "📖",
            Description = "Spelling, words, reading",
            Color = Color.FromArgb("#FF6B9D"),
            BackgroundColor = Colors.LightBlue
        },
        new()
        {
            Id = "numbers",
            Label = "Numbers & Logic",
            Emoji = "🔢",
            Description = "Maths, counting, patterns",
            Color = Color.FromArgb("#7C5CFF"),
            BackgroundColor = Color.FromArgb("#227C5CFF"),
        },
        new()
        {
            Id = "memory",
            Label = "Memory & Attention",
            Emoji = "🧠",
            Description = "Matching, sequences, focus",
            Color = Color.FromArgb("#4ECDC4"),
            BackgroundColor = Color.FromArgb("#224ECDC4"),
        }
    ];
}
