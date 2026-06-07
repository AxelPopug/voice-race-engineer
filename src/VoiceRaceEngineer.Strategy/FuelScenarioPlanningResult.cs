using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy;

public sealed record FuelScenarioPlanningResult(
    Laps TargetDistance,
    Laps RequestedSavingDistance,
    Laps EffectiveSavingDistance,
    Laps NormalConsumptionDistance,
    LitersPerLap TargetConsumption,
    LitersPerLap SavingPerLap,
    Percentage SavingPercentage,
    FuelScenarioPlanningStatus Status,
    bool IsMathematicallyPossible);
