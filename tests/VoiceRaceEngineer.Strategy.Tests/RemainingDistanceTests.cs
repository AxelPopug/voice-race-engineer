using System.Globalization;
using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy.Tests;

public sealed class RemainingDistanceTests
{
    [Fact]
    public void LapLimitedRaceReturnsEquivalentDistanceForPartialLap()
    {
        var result = RemainingDistance.CalculateLapLimited(
            new Laps(10m),
            new Laps(0.25m),
            new Laps(30m));

        Assert.False(result.IsFinished);
        Assert.Equal(new Laps(19.75m), result.RemainingEquivalentDistance);
        Assert.Equal(new FinishLineCrossings(20), result.RemainingFinishLineCrossings);
    }

    [Fact]
    public void LapLimitedRaceReturnsZeroAtFinishLine()
    {
        var result = RemainingDistance.CalculateLapLimited(
            new Laps(30m),
            Laps.Zero,
            new Laps(30m));

        Assert.True(result.IsFinished);
        Assert.Equal(Laps.Zero, result.RemainingEquivalentDistance);
        Assert.Equal(FinishLineCrossings.Zero, result.RemainingFinishLineCrossings);
    }

    [Theory]
    [InlineData("29", "0", "1")]
    [InlineData("29", "0.01", "1")]
    [InlineData("29", "0.99", "1")]
    public void LapLimitedRaceCountsFinalFinishLineCrossingWithoutOffByOne(
        string completedLapsText,
        string lapFractionText,
        string expectedCrossingsText)
    {
        decimal completedLaps = decimal.Parse(completedLapsText, CultureInfo.InvariantCulture);
        decimal lapFraction = decimal.Parse(lapFractionText, CultureInfo.InvariantCulture);
        int expectedCrossings = int.Parse(expectedCrossingsText, CultureInfo.InvariantCulture);

        var result = RemainingDistance.CalculateLapLimited(
            new Laps(completedLaps),
            new Laps(lapFraction),
            new Laps(30m));

        Assert.Equal(new FinishLineCrossings(expectedCrossings), result.RemainingFinishLineCrossings);
    }

    [Fact]
    public void LapLimitedRaceClampsAlreadyFinishedDistanceToZero()
    {
        var result = RemainingDistance.CalculateLapLimited(
            new Laps(31m),
            Laps.Zero,
            new Laps(30m));

        Assert.True(result.IsFinished);
        Assert.Equal(Laps.Zero, result.RemainingEquivalentDistance);
        Assert.Equal(FinishLineCrossings.Zero, result.RemainingFinishLineCrossings);
    }

    [Theory]
    [InlineData("29.5", "0", "30")]
    [InlineData("29", "0", "30.5")]
    [InlineData("1", "1.1", "30")]
    public void LapLimitedRaceRejectsInvalidInputs(
        string completedLapsText,
        string lapFractionText,
        string scheduledLapsText)
    {
        decimal completedLaps = decimal.Parse(completedLapsText, CultureInfo.InvariantCulture);
        decimal lapFraction = decimal.Parse(lapFractionText, CultureInfo.InvariantCulture);
        decimal scheduledLaps = decimal.Parse(scheduledLapsText, CultureInfo.InvariantCulture);

        _ = Assert.Throws<ArgumentOutOfRangeException>(
            () => RemainingDistance.CalculateLapLimited(
                new Laps(completedLaps),
                new Laps(lapFraction),
                new Laps(scheduledLaps)));
    }

    [Fact]
    public void FinishLineCrossingsRejectsNegativeValues()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new FinishLineCrossings(-1));
    }
}
