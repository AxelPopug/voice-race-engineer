using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy.Tests;

public sealed class FuelPlanningBoundaryTests
{
    [Fact]
    public void FuelToFinishFuelRequiredIsMonotonicWithDistance()
    {
        decimal[] fuelCases = [0m, 1m, 5m, 20m];
        decimal[] consumptionCases = [0.1m, 1m, 2.5m, 4m];
        decimal[] reserveCases = [0m, 0.5m, 3m];
        decimal[] distanceCases = [0m, 0.25m, 1m, 3.5m, 8m];

        foreach (decimal fuelRemaining in fuelCases)
        {
            foreach (decimal planningConsumption in consumptionCases)
            {
                foreach (decimal reserve in reserveCases)
                {
                    var results = distanceCases
                        .Select(distance =>
                            FuelPlanning.CalculateFuelToFinish(
                                new Liters(fuelRemaining),
                                new Laps(distance),
                                new LitersPerLap(planningConsumption),
                                new Liters(reserve)))
                        .ToList();

                    AssertMonotonic(results, (previous, current) =>
                        current.FuelRequired.Value >= previous.FuelRequired.Value
                        && current.FuelMargin.Value <= previous.FuelMargin.Value
                        && current.FuelToAdd.Value >= previous.FuelToAdd.Value);
                }
            }
        }
    }

    [Fact]
    public void FuelToFinishMarginIsMonotonicWithFuelAndReserve()
    {
        decimal[] fuelCases = [0m, 1m, 4m, 12m, 25m];
        decimal[] reserveCases = [0m, 0.5m, 2m, 6m];

        var fuelResults = fuelCases
            .Select(fuelRemaining =>
                FuelPlanning.CalculateFuelToFinish(
                    new Liters(fuelRemaining),
                    new Laps(4m),
                    new LitersPerLap(2.5m),
                    new Liters(1m)))
            .ToList();

        AssertMonotonic(fuelResults,
            (previous, current) => current.FuelMargin.Value >= previous.FuelMargin.Value
                && current.FuelToAdd.Value <= previous.FuelToAdd.Value);

        var reserveResults = reserveCases
            .Select(reserve =>
                FuelPlanning.CalculateFuelToFinish(
                    new Liters(12m),
                    new Laps(4m),
                    new LitersPerLap(2.5m),
                    new Liters(reserve)))
            .ToList();

        AssertMonotonic(reserveResults,
            (previous, current) => current.FuelMargin.Value <= previous.FuelMargin.Value
                && current.FuelToAdd.Value >= previous.FuelToAdd.Value);
    }

    [Fact]
    public void FuelToFinishHandlesZeroDistance()
    {
        var result = FuelPlanning.CalculateFuelToFinish(
            new Liters(3m),
            Laps.Zero,
            new LitersPerLap(2.5m),
            new Liters(1m));

        Assert.True(result.CanFinish);
        Assert.Equal(Laps.Zero, result.RemainingDistance);
        Assert.Equal(Liters.Zero, result.PredictedBurn);
        Assert.Equal(new Liters(1m), result.FuelRequired);
        Assert.Equal(new LitersDelta(2m), result.FuelMargin);
        Assert.Equal(Liters.Zero, result.FuelToAdd);
    }

    [Fact]
    public void FuelToFinishHandlesExactFuel()
    {
        var result = FuelPlanning.CalculateFuelToFinish(
            new Liters(13m),
            new Laps(4m),
            new LitersPerLap(3m),
            new Liters(1m));

        Assert.True(result.CanFinish);
        Assert.Equal(new Liters(12m), result.PredictedBurn);
        Assert.Equal(new Liters(13m), result.FuelRequired);
        Assert.Equal(LitersDelta.Zero, result.FuelMargin);
        Assert.Equal(Liters.Zero, result.FuelToAdd);
    }

    [Fact]
    public void ExtraLapSavingRequiredSavingIsMonotonicWithFuelReserveAndExtraDistance()
    {
        decimal[] fuelCases = [0m, 1m, 5m, 10m, 20m];
        decimal[] reserveCases = [0m, 0.5m, 2m, 6m];
        decimal[] extraDistanceCases = [0.25m, 0.5m, 1m, 2m, 4m];

        var fuelResults = fuelCases
            .Select(fuelRemaining =>
                FuelPlanning.CalculateExtraLapSaving(
                    new Liters(fuelRemaining),
                    new Liters(1m),
                    new Laps(5m),
                    new Laps(1m),
                    new LitersPerLap(3m)))
            .ToList();

        AssertMonotonic(fuelResults,
            (previous, current) => current.SavingPerLap.Value <= previous.SavingPerLap.Value
                && current.SavingPercentage.Value <= previous.SavingPercentage.Value);

        var reserveResults = reserveCases
            .Select(reserve =>
                FuelPlanning.CalculateExtraLapSaving(
                    new Liters(18m),
                    new Liters(reserve),
                    new Laps(5m),
                    new Laps(1m),
                    new LitersPerLap(3m)))
            .ToList();

        AssertMonotonic(reserveResults,
            (previous, current) => current.SavingPerLap.Value >= previous.SavingPerLap.Value
                && current.SavingPercentage.Value >= previous.SavingPercentage.Value);

        var extraDistanceResults = extraDistanceCases
            .Select(extraDistance =>
                FuelPlanning.CalculateExtraLapSaving(
                    new Liters(18m),
                    new Liters(1m),
                    new Laps(5m),
                    new Laps(extraDistance),
                    new LitersPerLap(3m)))
            .ToList();

        AssertMonotonic(extraDistanceResults,
            (previous, current) => current.SavingPerLap.Value >= previous.SavingPerLap.Value
                && current.SavingPercentage.Value >= previous.SavingPercentage.Value);
    }

    [Fact]
    public void ExtraLapSavingHandlesZeroNormalDistance()
    {
        var result = FuelPlanning.CalculateExtraLapSaving(
            new Liters(2m),
            Liters.Zero,
            Laps.Zero,
            new Laps(1m),
            new LitersPerLap(3m));

        Assert.True(result.IsMathematicallyPossible);
        Assert.Equal(new Laps(1m), result.TargetDistance);
        Assert.Equal(new LitersPerLap(2m), result.TargetConsumption);
        Assert.Equal(new LitersPerLap(1m), result.SavingPerLap);
        Assert.Equal(new Percentage(1m / 3m * 100m), result.SavingPercentage);
    }

    [Fact]
    public void ExtraLapSavingHandlesExactFuel()
    {
        var result = FuelPlanning.CalculateExtraLapSaving(
            new Liters(19m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m));

        Assert.True(result.IsMathematicallyPossible);
        Assert.Equal(new Laps(6m), result.TargetDistance);
        Assert.Equal(new LitersPerLap(3m), result.TargetConsumption);
        Assert.Equal(LitersPerLap.Zero, result.SavingPerLap);
        Assert.Equal(Percentage.Zero, result.SavingPercentage);
    }

    [Fact]
    public void FuelToFinishRejectsInvalidInputs()
    {
        AssertThrowsArgumentOutOfRange(() => FuelPlanning.CalculateFuelToFinish(
            Liters.Zero,
            new Laps(1m),
            LitersPerLap.Zero,
            Liters.Zero));
    }

    [Fact]
    public void ExtraLapSavingRejectsInvalidInputs()
    {
        AssertThrowsArgumentOutOfRange(() => FuelPlanning.CalculateExtraLapSaving(
            Liters.Zero,
            Liters.Zero,
            Laps.Zero,
            Laps.Zero,
            new LitersPerLap(1m)));
        AssertThrowsArgumentOutOfRange(() => FuelPlanning.CalculateExtraLapSaving(
            Liters.Zero,
            Liters.Zero,
            Laps.Zero,
            new Laps(1m),
            LitersPerLap.Zero));
    }

    private static void AssertMonotonic<T>(IReadOnlyList<T> values, Func<T, T, bool> isMonotonic)
        where T : notnull
    {
        for (int index = 1; index < values.Count; index++)
        {
            var previous = values[index - 1];
            var current = values[index];
            Assert.True(isMonotonic(previous, current));
        }
    }

    private static void AssertThrowsArgumentOutOfRange(Action testCode)
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(testCode);
    }
}
