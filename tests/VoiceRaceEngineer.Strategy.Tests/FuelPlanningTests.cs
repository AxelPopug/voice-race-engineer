using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy.Tests;

public sealed class FuelPlanningTests
{
    [Fact]
    public void FuelToFinishReturnsMarginWhenFuelIsSufficient()
    {
        var result = FuelPlanning.CalculateFuelToFinish(
            new Liters(20m),
            new Laps(5m),
            new LitersPerLap(3m),
            new Liters(1m));

        Assert.True(result.CanFinish);
        Assert.Equal(new Liters(16m), result.FuelRequired);
        Assert.Equal(new Liters(4m), result.FuelMargin);
        Assert.Equal(Liters.Zero, result.FuelToAdd);
    }

    [Fact]
    public void FuelToFinishReturnsFuelToAddWhenFuelIsInsufficient()
    {
        var result = FuelPlanning.CalculateFuelToFinish(
            new Liters(14m),
            new Laps(5m),
            new LitersPerLap(3m),
            new Liters(1m));

        Assert.False(result.CanFinish);
        Assert.Equal(new Liters(-2m), result.FuelMargin);
        Assert.Equal(new Liters(2m), result.FuelToAdd);
    }

    [Fact]
    public void ExtraLapSavingReturnsRequiredSaving()
    {
        var result = FuelPlanning.CalculateExtraLapSaving(
            new Liters(18m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m));

        Assert.True(result.IsMathematicallyPossible);
        Assert.Equal(new LitersPerLap(17m / 6m), result.TargetConsumption);
        Assert.Equal(new LitersPerLap(1m / 6m), result.SavingPerLap);
        Assert.Equal(new Percentage(1m / 18m * 100m), result.SavingPercentage);
    }

    [Fact]
    public void MoreFuelNeverIncreasesRequiredSaving()
    {
        var lowerFuel = FuelPlanning.CalculateExtraLapSaving(
            new Liters(17m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m));
        var higherFuel = FuelPlanning.CalculateExtraLapSaving(
            new Liters(18m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m));

        Assert.True(higherFuel.SavingPerLap.Value <= lowerFuel.SavingPerLap.Value);
    }
}
