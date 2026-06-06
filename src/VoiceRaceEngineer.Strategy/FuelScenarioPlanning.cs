using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy;

public static class FuelScenarioPlanning
{
    public static FuelScenarioPlanningResult CalculateKLapSaving(
        Liters fuelRemaining,
        Liters reserve,
        Laps normalRemainingDistance,
        Laps extraDistance,
        LitersPerLap planningConsumption,
        Laps savingDistance)
    {
        EnsureNonNegative(fuelRemaining.Value, nameof(fuelRemaining));
        EnsureNonNegative(reserve.Value, nameof(reserve));
        EnsureNonNegative(normalRemainingDistance.Value, nameof(normalRemainingDistance));
        EnsureNonNegative(extraDistance.Value, nameof(extraDistance));
        EnsurePositive(planningConsumption.Value, nameof(planningConsumption));
        EnsurePositive(savingDistance.Value, nameof(savingDistance));

        decimal availableForBurn = fuelRemaining.Value - reserve.Value;
        var targetDistance = new Laps(normalRemainingDistance.Value + extraDistance.Value);
        EnsurePositive(targetDistance.Value, nameof(normalRemainingDistance));
        var effectiveSavingDistance = new Laps(decimal.Min(savingDistance.Value, targetDistance.Value));
        var normalConsumptionDistance = new Laps(targetDistance.Value - effectiveSavingDistance.Value);
        decimal fuelNeededAtNormalConsumption = normalConsumptionDistance.Value * planningConsumption.Value;
        decimal fuelForSavingDistance = availableForBurn - fuelNeededAtNormalConsumption;
        bool isImpossible = targetDistance.Value > 0m
            && (availableForBurn <= 0m || fuelForSavingDistance <= 0m);

        var targetConsumption = new LitersPerLap(
            isImpossible
                ? 0m
                : CalculateTargetConsumption(planningConsumption.Value, effectiveSavingDistance.Value, fuelForSavingDistance));
        var savingPerLap = new LitersPerLap(decimal.Max(0m, planningConsumption.Value - targetConsumption.Value));
        var savingPercentage = new Percentage(savingPerLap.Value / planningConsumption.Value * 100m);
        var status = DetermineStatus(isImpossible, savingPerLap.Value);

        return new FuelScenarioPlanningResult(
            targetDistance,
            savingDistance,
            effectiveSavingDistance,
            normalConsumptionDistance,
            targetConsumption,
            savingPerLap,
            savingPercentage,
            status,
            !isImpossible);
    }

    private static decimal CalculateTargetConsumption(
        decimal planningConsumption,
        decimal effectiveSavingDistance,
        decimal fuelForSavingDistance)
    {
        return effectiveSavingDistance == 0m
            ? planningConsumption
            : fuelForSavingDistance / effectiveSavingDistance;
    }

    private static FuelScenarioPlanningStatus DetermineStatus(bool isImpossible, decimal savingPerLap)
    {
        return isImpossible
            ? FuelScenarioPlanningStatus.Impossible
            : savingPerLap == 0m
                ? FuelScenarioPlanningStatus.AlreadyAchieved
                : FuelScenarioPlanningStatus.Possible;
    }

    private static void EnsureNonNegative(decimal value, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);
    }

    private static void EnsurePositive(decimal value, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, parameterName);
    }
}

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

public enum FuelScenarioPlanningStatus
{
    AlreadyAchieved,
    Possible,
    Impossible
}
