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
        ValidateInputs(fuelRemaining, reserve, normalRemainingDistance, extraDistance, planningConsumption, savingDistance);

        decimal availableForBurn = CalculateAvailableForBurn(fuelRemaining, reserve);
        var targetDistance = CalculateTargetDistance(normalRemainingDistance, extraDistance);
        var effectiveSavingDistance = CalculateEffectiveSavingDistance(savingDistance, targetDistance);
        var normalConsumptionDistance = CalculateNormalConsumptionDistance(targetDistance, effectiveSavingDistance);
        decimal fuelForSavingDistance = CalculateFuelForSavingDistance(availableForBurn, normalConsumptionDistance, planningConsumption);
        bool isImpossible = IsScenarioImpossible(targetDistance, availableForBurn, fuelForSavingDistance);

        var targetConsumption = new LitersPerLap(
            CalculateTargetConsumption(planningConsumption, effectiveSavingDistance, fuelForSavingDistance, isImpossible));
        var savingPerLap = CalculateSavingPerLap(planningConsumption, targetConsumption);
        var savingPercentage = CalculateSavingPercentage(savingPerLap, planningConsumption);
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

    private static void ValidateInputs(
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
    }

    private static decimal CalculateAvailableForBurn(Liters fuelRemaining, Liters reserve)
    {
        return fuelRemaining.Value - reserve.Value;
    }

    private static Laps CalculateTargetDistance(Laps normalRemainingDistance, Laps extraDistance)
    {
        var targetDistance = new Laps(normalRemainingDistance.Value + extraDistance.Value);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetDistance.Value, nameof(normalRemainingDistance));
        return targetDistance;
    }

    private static Laps CalculateEffectiveSavingDistance(Laps savingDistance, Laps targetDistance)
    {
        return new Laps(decimal.Min(savingDistance.Value, targetDistance.Value));
    }

    private static Laps CalculateNormalConsumptionDistance(
        Laps targetDistance,
        Laps effectiveSavingDistance)
    {
        return new Laps(targetDistance.Value - effectiveSavingDistance.Value);
    }

    private static decimal CalculateFuelForSavingDistance(
        decimal availableForBurn,
        Laps normalConsumptionDistance,
        LitersPerLap planningConsumption)
    {
        decimal fuelNeededAtNormalConsumption = normalConsumptionDistance.Value * planningConsumption.Value;
        return availableForBurn - fuelNeededAtNormalConsumption;
    }

    private static bool IsScenarioImpossible(
        Laps targetDistance,
        decimal availableForBurn,
        decimal fuelForSavingDistance)
    {
        return targetDistance.Value > 0m
            && (availableForBurn <= 0m || fuelForSavingDistance <= 0m);
    }

    private static decimal CalculateTargetConsumption(
        LitersPerLap planningConsumption,
        Laps effectiveSavingDistance,
        decimal fuelForSavingDistance,
        bool isImpossible)
    {
        return isImpossible
            ? 0m
            : effectiveSavingDistance.Value == 0m
                ? planningConsumption.Value
                : fuelForSavingDistance / effectiveSavingDistance.Value;
    }

    private static LitersPerLap CalculateSavingPerLap(
        LitersPerLap planningConsumption,
        LitersPerLap targetConsumption)
    {
        return new LitersPerLap(Math.Max(0m, planningConsumption.Value - targetConsumption.Value));
    }

    private static Percentage CalculateSavingPercentage(
        LitersPerLap savingPerLap,
        LitersPerLap planningConsumption)
    {
        return new Percentage(savingPerLap.Value / planningConsumption.Value * 100m);
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
