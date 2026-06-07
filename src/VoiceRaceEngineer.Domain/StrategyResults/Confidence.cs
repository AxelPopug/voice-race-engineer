namespace VoiceRaceEngineer.Domain.StrategyResults;

public enum ConfidenceLevel
{
    Unknown,
    Low,
    Medium,
    High
}

public sealed record Confidence
{
    public Confidence(ConfidenceLevel level, decimal value)
    {
        if (value is < 0m or > 1m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Confidence value must be between 0 and 1.");
        }

        Level = level;
        Value = value;
    }

    public ConfidenceLevel Level { get; }

    public decimal Value { get; }

    public static Confidence UnknownResult { get; } = new Confidence(ConfidenceLevel.Unknown, 0m);
}
