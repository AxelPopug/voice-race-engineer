namespace VoiceRaceEngineer.Domain.StrategyResults;

public sealed record Assumption
{
    public Assumption(string code, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        Code = code;
        Description = description;
    }

    public string Code { get; }

    public string Description { get; }
}
