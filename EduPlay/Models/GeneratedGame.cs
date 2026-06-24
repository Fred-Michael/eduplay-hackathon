namespace EduPlay.Models;

public class GeneratedGame
{
    public string GameId { get; set; } = string.Empty;
    public string GameUrl { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public string MechanicName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
