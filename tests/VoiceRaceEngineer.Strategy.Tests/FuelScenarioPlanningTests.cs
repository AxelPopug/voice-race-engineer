using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy.Tests;

public sealed class FuelScenarioPlanningTests
{
    [Fact]
    public void KLapSavingAppliesOnlyToRequestedSavingWindow()
    {
        var result = FuelScenarioPlanning.CalculateKLapSaving(
            new Liters(18m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m),
            new Laps(2m));

        Assert.True(result.IsMathematicallyPossible);
        Assert.Equal(FuelScenarioPlanningStatus.Possible, result.Status);
        Assert.Equal(new Laps(6m), result.TargetDistance);
        Assert.Equal(new Laps(2m), result.EffectiveSavingDistance);
        Assert.Equal(new Laps(4m), result.NormalConsumptionDistance);
        Assert.Equal(new LitersPerLap(2.5m), result.TargetConsumption);
        Assert.Equal(new LitersPerLap(0.5m), result.SavingPerLap);
        Assert.Equal(new Percentage(0.5m / 3m * 100m), result.SavingPercentage);
    }

    [Fact]
    public void KLapSavingReturnsAlreadyAchievedWhenNormalConsumptionExactlyFits()
    {
        var result = FuelScenarioPlanning.CalculateKLapSaving(
            new Liters(19m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m),
            new Laps(2m));

        Assert.True(result.IsMathematicallyPossible);
        Assert.Equal(FuelScenarioPlanningStatus.AlreadyAchieved, result.Status);
        Assert.Equal(new LitersPerLap(3m), result.TargetConsumption);
        Assert.Equal(LitersPerLap.Zero, result.SavingPerLap);
        Assert.Equal(Percentage.Zero, result.SavingPercentage);
    }

    [Fact]
    public void KLapSavingReturnsImpossibleWhenNormalDistanceConsumesAvailableFuel()
    {
        var result = FuelScenarioPlanning.CalculateKLapSaving(
            new Liters(12m),
            new Liters(1m),
            new Laps(5m),
            new Laps(1m),
            new LitersPerLap(3m),
            new Laps(2m));

        Assert.False(result.IsMathematicallyPossible);
        Assert.Equal(FuelScenarioPlanningStatus.Impossible, result.Status);
        Assert.Equal(LitersPerLap.Zero, result.TargetConsumption);
        Assert.Equal(new LitersPerLap(3m), result.SavingPerLap);
        Assert.Equal(new Percentage(100m), result.SavingPercentage);
    }

    [Fact]
    public void KLapSavingReturnsImpossibleWhenReserveExceedsFuel()
    {
        var result = FuelScenarioPlanning.CalculateKLapSaving(
            new Liters(1m),
            new Liters(2m),
            new Laps(1m),
            new Laps(1m),
            new LitersPerLap(3m),
            new Laps(1m));

        Assert.False(result.IsMathematicallyPossible);
        Assert.Equal(FuelScenarioPlanningStatus.Impossible, result.Status);
        Assert.Equal(LitersPerLap.Zero, result.TargetConsumption);
    }

    [Fact]
    public void KLapSavingUsesWholeTargetDistanceWhenSavingWindowExceedsTargetDistance()
    {
        var result = FuelScenarioPlanning.CalculateKLapSaving(
            new Liters(7m),
            new Liters(1m),
            new Laps(2m),
            new Laps(1m),
            new LitersPerLap(3m),
            new Laps(5m));

        Assert.True(result.IsMathematicallyPossible);
        Assert.Equal(new Laps(3m), result.TargetDistance);
        Assert.Equal(new Laps(5m), result.RequestedSavingDistance);
        Assert.Equal(new Laps(3m), result.EffectiveSavingDistance);
        Assert.Equal(Laps.Zero, result.NormalConsumptionDistance);
        Assert.Equal(new LitersPerLap(2m), result.TargetConsumption);
        Assert.Equal(new LitersPerLap(1m), result.SavingPerLap);
    }

    [Theory]
    [InlineData(0)]
    public void KLapSavingRejectsNonPositiveSavingWindow(decimal savingLaps)
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            FuelScenarioPlanning.CalculateKLapSaving(
                new Liters(7m),
                new Liters(1m),
                new Laps(2m),
                new Laps(1m),
                new LitersPerLap(3m),
                new Laps(savingLaps)));
    }

    [Fact]
    public void KLapSavingRejectsZeroTargetDistance()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            FuelScenarioPlanning.CalculateKLapSaving(
                new Liters(5m),
                new Liters(1m),
                Laps.Zero,
                Laps.Zero,
                new LitersPerLap(3m),
                new Laps(2m)));
    }
}
