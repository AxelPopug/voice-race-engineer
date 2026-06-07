namespace VoiceRaceEngineer.Domain.StrategyResults;

public enum StrategyWarningSeverity
{
    Low,
    Medium,
    High
}

public sealed record StrategyWarning
{
    public StrategyWarning(string code, string message, StrategyWarningSeverity severity)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required.", nameof(message));
        }

        Code = code;
        Message = message;
        Severity = severity;
    }

    public string Code { get; }

    public string Message { get; }

    public StrategyWarningSeverity Severity { get; }
}
