namespace EduPlay.Models;

public class AgeGroup
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string SubLabel { get; init; } = string.Empty;

    public static readonly List<AgeGroup> AllAgeGroup =
    [
        new()
        { 
            Id = "4-6",
            Label = "Ages 4-6",
            SubLabel = "Early learners"
        },
        new()
        {
            Id = "7-9",
            Label = "Ages 7-9",
            SubLabel = "Primary school"
        },
        new()
        {
            Id = "10-12",
            Label = "Ages 10-12",
            SubLabel = "Upper primary"
        }
    ];
}
