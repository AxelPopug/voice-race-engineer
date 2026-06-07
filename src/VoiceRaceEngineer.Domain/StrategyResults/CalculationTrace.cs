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

public sealed class CalculationTrace : IEquatable<CalculationTrace>
{
    public CalculationTrace(IReadOnlyList<CalculationStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        foreach (var step in steps)
        {
            if (step is null)
            {
                throw new ArgumentException("Steps cannot contain null values.", nameof(steps));
            }
        }

        Steps = [.. steps];
    }

    public IReadOnlyList<CalculationStep> Steps { get; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CalculationTrace);
    }

    public bool Equals(CalculationTrace? other)
    {
        return other is not null &&
            Steps.Count == other.Steps.Count &&
            Steps.SequenceEqual(other.Steps);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var step in Steps)
        {
            hashCode.Add(step);
        }

        return hashCode.ToHashCode();
    }

    public static CalculationTrace Empty { get; } = new CalculationTrace([]);
}
