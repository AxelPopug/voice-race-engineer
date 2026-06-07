namespace VoiceRaceEngineer.Domain.StrategyQueries;

public sealed record GetExtraLapSavingQuery : StrategyQuery
{
    public GetExtraLapSavingQuery(
        long snapshotId,
        SnapshotFreshnessPolicy snapshotFreshnessPolicy,
        Liters fuelRemaining,
        Liters reserve,
        Laps normalRemainingDistance,
        Laps extraDistance,
        LitersPerLap planningConsumption)
        : base(snapshotId, snapshotFreshnessPolicy)
    {
        FuelRemaining = fuelRemaining;
        Reserve = reserve;
        NormalRemainingDistance = normalRemainingDistance;
        ExtraDistance = extraDistance;
        PlanningConsumption = planningConsumption;
    }

    public Liters FuelRemaining { get; }

    public Liters Reserve { get; }

    public Laps NormalRemainingDistance { get; }

    public Laps ExtraDistance { get; }

    public LitersPerLap PlanningConsumption { get; }
}
