namespace VoiceRaceEngineer.Domain.StrategyQueries;

public sealed record GetFuelStatusQuery : StrategyQuery
{
    public GetFuelStatusQuery(
        long snapshotId,
        SnapshotFreshnessPolicy snapshotFreshnessPolicy,
        Liters fuelRemaining,
        Liters reserve,
        LitersPerLap planningConsumption,
        Laps remainingDistance)
        : base(snapshotId, snapshotFreshnessPolicy)
    {
        FuelRemaining = fuelRemaining;
        Reserve = reserve;
        PlanningConsumption = planningConsumption;
        RemainingDistance = remainingDistance;
    }

    public Liters FuelRemaining { get; }

    public Liters Reserve { get; }

    public LitersPerLap PlanningConsumption { get; }

    public Laps RemainingDistance { get; }
}
