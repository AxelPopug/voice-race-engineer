using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy;

public static class FuelPlanning
{
    public static FuelToFinishResult CalculateFuelToFinish(
        Liters fuelRemaining,
        Laps remainingDistance,
        LitersPerLap planningConsumption,
        Liters reserve)
    {
        EnsureNonNegative(fuelRemaining.Value, nameof(fuelRemaining));
        EnsureNonNegative(remainingDistance.Value, nameof(remainingDistance));
        EnsurePositive(planningConsumption.Value, nameof(planningConsumption));
        EnsureNonNegative(reserve.Value, nameof(reserve));

        var predictedBurn = new Liters(remainingDistance.Value * planningConsumption.Value);
        var fuelRequired = predictedBurn + reserve;
        var fuelMargin = fuelRemaining - fuelRequired;
        var fuelToAdd = new Liters(decimal.Max(0m, -fuelMargin.Value));

        return new FuelToFinishResult(
            remainingDistance,
            planningConsumption,
            predictedBurn,
            reserve,
            fuelRequired,
            fuelMargin,
            fuelToAdd,
            fuelMargin.Value >= 0m);
    }

    public static ExtraLapSavingResult CalculateExtraLapSaving(
        Liters fuelRemaining,
        Liters reserve,
        Laps normalRemainingDistance,
        Laps extraDistance,
        LitersPerLap planningConsumption)
    {
        EnsureNonNegative(fuelRemaining.Value, nameof(fuelRemaining));
        EnsureNonNegative(reserve.Value, nameof(reserve));
        EnsureNonNegative(normalRemainingDistance.Value, nameof(normalRemainingDistance));
        EnsurePositive(extraDistance.Value, nameof(extraDistance));
        EnsurePositive(planningConsumption.Value, nameof(planningConsumption));

        var availableForBurn = fuelRemaining - reserve;
        var targetDistance = new Laps(normalRemainingDistance.Value + extraDistance.Value);

        if (availableForBurn.Value <= 0m)
        {
            return new ExtraLapSavingResult(
                targetDistance,
                LitersPerLap.Zero,
                planningConsumption,
                new Percentage(100m),
                false);
        }

        var targetConsumption = new LitersPerLap(availableForBurn.Value / targetDistance.Value);
        var savingPerLap = new LitersPerLap(
            decimal.Max(0m, planningConsumption.Value - targetConsumption.Value));
        var savingPercentage = new Percentage(
            savingPerLap.Value / planningConsumption.Value * 100m);

        return new ExtraLapSavingResult(
            targetDistance,
            targetConsumption,
            savingPerLap,
            savingPercentage,
            targetConsumption.Value > 0m);
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

public sealed record FuelToFinishResult(
    Laps RemainingDistance,
    LitersPerLap PlanningConsumption,
    Liters PredictedBurn,
    Liters Reserve,
    Liters FuelRequired,
    Liters FuelMargin,
    Liters FuelToAdd,
    bool CanFinish);

public sealed record ExtraLapSavingResult(
    Laps TargetDistance,
    LitersPerLap TargetConsumption,
    LitersPerLap SavingPerLap,
    Percentage SavingPercentage,
    bool IsMathematicallyPossible);
