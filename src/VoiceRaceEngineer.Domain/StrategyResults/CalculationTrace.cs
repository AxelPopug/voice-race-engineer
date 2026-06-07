namespace VoiceRaceEngineer.Domain.StrategyResults;

public sealed record CalculationStep
{
    public CalculationStep(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        Name = name;
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }
}

public sealed record CalculationTrace
{
    public CalculationTrace(IEnumerable<CalculationStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var orderedSteps = steps as IReadOnlyList<CalculationStep> ?? [.. steps];

        foreach (var step in orderedSteps)
        {
            if (step is null)
            {
                throw new ArgumentException("Steps cannot contain null values.", nameof(steps));
            }
        }

        Steps = orderedSteps;
    }

    public IReadOnlyList<CalculationStep> Steps { get; }

    public static CalculationTrace Empty { get; } = new CalculationTrace([]);
}
