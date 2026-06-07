namespace VoiceRaceEngineer.Domain.StrategyQueries;

public abstract record StrategyQuery
{
    protected StrategyQuery(long snapshotId, SnapshotFreshnessPolicy snapshotFreshnessPolicy)
    {
        if (snapshotId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshotId),
                snapshotId,
                "Snapshot id must be greater than zero.");
        }

        SnapshotId = snapshotId;
        SnapshotFreshnessPolicy = snapshotFreshnessPolicy;
    }

    public long SnapshotId { get; }

    public SnapshotFreshnessPolicy SnapshotFreshnessPolicy { get; }
}
