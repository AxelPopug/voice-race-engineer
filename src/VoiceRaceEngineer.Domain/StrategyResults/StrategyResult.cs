namespace VoiceRaceEngineer.Domain.StrategyResults;

public sealed class StrategyResult<T> : IEquatable<StrategyResult<T>>
    where T : notnull
{
    public StrategyResult(
        long snapshotId,
        T value,
        Confidence confidence,
        IReadOnlyList<Assumption> assumptions,
        IReadOnlyList<StrategyWarning> warnings,
        CalculationTrace trace)
    {
        if (snapshotId < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshotId),
                snapshotId,
                "Snapshot id must be non-negative.");
        }

        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(confidence);
        ArgumentNullException.ThrowIfNull(assumptions);
        ArgumentNullException.ThrowIfNull(warnings);
        ArgumentNullException.ThrowIfNull(trace);

        SnapshotId = snapshotId;
        Value = value;
        Confidence = confidence;
        Assumptions = assumptions;
        Warnings = warnings;
        Trace = trace;
    }

    public long SnapshotId { get; }

    public T Value { get; }

    public Confidence Confidence { get; }

    public IReadOnlyList<Assumption> Assumptions { get; }

    public IReadOnlyList<StrategyWarning> Warnings { get; }

    public CalculationTrace Trace { get; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as StrategyResult<T>);
    }

    public bool Equals(StrategyResult<T>? other)
    {
        return other is not null &&
            SnapshotId == other.SnapshotId &&
            Value.Equals(other.Value) &&
            Confidence.Equals(other.Confidence) &&
            Assumptions.SequenceEqual(other.Assumptions) &&
            Warnings.SequenceEqual(other.Warnings) &&
            Trace.Equals(other.Trace);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(SnapshotId);
        hash.Add(Value);
        hash.Add(Confidence);
        foreach (var assumption in Assumptions)
        {
            hash.Add(assumption);
        }

        foreach (var warning in Warnings)
        {
            hash.Add(warning);
        }

        hash.Add(Trace);
        return hash.ToHashCode();
    }
}
