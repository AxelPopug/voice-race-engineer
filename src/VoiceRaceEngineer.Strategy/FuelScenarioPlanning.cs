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
        ArgumentOutOfRangeException.ThrowIfNegative(fuelRemaining.Value, nameof(fuelRemaining));
        ArgumentOutOfRangeException.ThrowIfNegative(reserve.Value, nameof(reserve));
        ArgumentOutOfRangeException.ThrowIfNegative(normalRemainingDistance.Value, nameof(normalRemainingDistance));
        ArgumentOutOfRangeException.ThrowIfNegative(extraDistance.Value, nameof(extraDistance));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(planningConsumption.Value, nameof(planningConsumption));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(savingDistance.Value, nameof(savingDistance));

        decimal availableForBurn = fuelRemaining.Value - reserve.Value;
        var targetDistance = new Laps(normalRemainingDistance.Value + extraDistance.Value);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetDistance.Value, nameof(normalRemainingDistance));

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
        var savingPerLap = new LitersPerLap(Math.Max(0m, planningConsumption.Value - targetConsumption.Value));
        var savingPercentage = new Percentage(savingPerLap.Value / planningConsumption.Value * 100m);
        var status = DetermineStatus(isImpossible, savingPerLap);

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

    private static FuelScenarioPlanningStatus DetermineStatus(
        bool isImpossible,
        LitersPerLap savingPerLap)
    {
        return (isImpossible, savingPerLap == LitersPerLap.Zero) switch
        {
            (true, _) => FuelScenarioPlanningStatus.Impossible,
            (false, true) => FuelScenarioPlanningStatus.AlreadyAchieved,
            (false, false) => FuelScenarioPlanningStatus.Possible
        };
    }
}
