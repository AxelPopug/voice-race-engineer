namespace VoiceRaceEngineer.Domain.StrategyResults;

public sealed record StrategyResult<T>
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
}
