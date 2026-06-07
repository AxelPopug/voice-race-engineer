using VoiceRaceEngineer.Domain.StrategyQueries;
namespace VoiceRaceEngineer.Domain.Tests;

public sealed class StrategyQueryContractsTests
{
    [Fact]
    public void QueryContractsStoreSnapshotAndFreshnessPolicy()
    {
        var query = new GetFuelStatusQuery(
            42,
            SnapshotFreshnessPolicy.AllowStale,
            new Liters(12.5m),
            new Liters(1.2m),
            new LitersPerLap(2.3m),
            new Laps(34m));

        Assert.Equal(42, query.SnapshotId);
        Assert.Equal(SnapshotFreshnessPolicy.AllowStale, query.SnapshotFreshnessPolicy);
    }

    [Theory]
    [InlineData(0)]
    public void StrategyQueryRequiresPositiveSnapshotId(int snapshotId)
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new GetFuelStatusQuery(
                snapshotId,
                SnapshotFreshnessPolicy.RequireFresh,
                new Liters(12.5m),
                new Liters(1.2m),
                new LitersPerLap(2.3m),
                new Laps(34m)));
    }

    [Fact]
    public void QueryRecordsAreValueComparable()
    {
        var one = new GetFuelToFinishQuery(
            99,
            SnapshotFreshnessPolicy.RequireFresh,
            new Liters(20m),
            new Liters(1m),
            new Laps(40m),
            new LitersPerLap(2.2m));
        var two = new GetFuelToFinishQuery(
            99,
            SnapshotFreshnessPolicy.RequireFresh,
            new Liters(20m),
            new Liters(1m),
            new Laps(40m),
            new LitersPerLap(2.2m));

        Assert.Equal(one, two);
    }
}
