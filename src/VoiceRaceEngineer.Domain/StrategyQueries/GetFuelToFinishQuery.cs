namespace VoiceRaceEngineer.Domain.StrategyQueries;

public sealed record GetFuelToFinishQuery : StrategyQuery
{
    public GetFuelToFinishQuery(
        long snapshotId,
        SnapshotFreshnessPolicy snapshotFreshnessPolicy,
        Liters fuelRemaining,
        Liters reserve,
        Laps remainingDistance,
        LitersPerLap planningConsumption)
        : base(snapshotId, snapshotFreshnessPolicy)
    {
        FuelRemaining = fuelRemaining;
        Reserve = reserve;
        RemainingDistance = remainingDistance;
        PlanningConsumption = planningConsumption;
    }

    public Liters FuelRemaining { get; }

    public Liters Reserve { get; }

    public Laps RemainingDistance { get; }

    public LitersPerLap PlanningConsumption { get; }
}
