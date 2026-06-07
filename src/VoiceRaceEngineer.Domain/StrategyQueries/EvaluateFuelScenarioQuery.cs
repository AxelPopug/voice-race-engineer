namespace VoiceRaceEngineer.Domain.StrategyQueries;

public sealed record EvaluateFuelScenarioQuery : StrategyQuery
{
    public EvaluateFuelScenarioQuery(
        long snapshotId,
        SnapshotFreshnessPolicy snapshotFreshnessPolicy,
        Liters fuelRemaining,
        Liters reserve,
        Laps normalRemainingDistance,
        Laps extraDistance,
        Laps savingLaps,
        LitersPerLap planningConsumption)
        : base(snapshotId, snapshotFreshnessPolicy)
    {
        FuelRemaining = fuelRemaining;
        Reserve = reserve;
        NormalRemainingDistance = normalRemainingDistance;
        ExtraDistance = extraDistance;
        SavingLaps = savingLaps;
        PlanningConsumption = planningConsumption;
    }

    public Liters FuelRemaining { get; }

    public Liters Reserve { get; }

    public Laps NormalRemainingDistance { get; }

    public Laps ExtraDistance { get; }

    public Laps SavingLaps { get; }

    public LitersPerLap PlanningConsumption { get; }
}
