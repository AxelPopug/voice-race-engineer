using System.Text.Json;
using VoiceRaceEngineer.Domain.StrategyResults;

namespace VoiceRaceEngineer.Domain.Tests;

public sealed class StrategyResultContractsTests
{
    private sealed record TestValue(int Count);

    [Fact]
    public void StrategyResultRejectsInvalidInputs()
    {
        var assumptions = new List<Assumption>();
        var warnings = new List<StrategyWarning>();
        CalculationTrace trace = new([]);

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StrategyResult<TestValue>(-1, new TestValue(1), new Confidence(ConfidenceLevel.High, 0.9m), assumptions, warnings, trace));

        _ = Assert.Throws<ArgumentNullException>(() =>
            new StrategyResult<TestValue>(1, null!, new Confidence(ConfidenceLevel.High, 0.9m), assumptions, warnings, trace));

        _ = Assert.Throws<ArgumentNullException>(() =>
            new StrategyResult<TestValue>(1, new TestValue(1), null!, assumptions, warnings, trace));

        _ = Assert.Throws<ArgumentNullException>(() =>
            new StrategyResult<TestValue>(1, new TestValue(1), new Confidence(ConfidenceLevel.High, 0.9m), null!, warnings, trace));

        _ = Assert.Throws<ArgumentNullException>(() =>
            new StrategyResult<TestValue>(1, new TestValue(1), new Confidence(ConfidenceLevel.High, 0.9m), assumptions, null!, trace));

        _ = Assert.Throws<ArgumentNullException>(() =>
            new StrategyResult<TestValue>(1, new TestValue(1), new Confidence(ConfidenceLevel.High, 0.9m), assumptions, warnings, null!));
    }

    [Fact]
    public void ConfidenceValidatesRange()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Confidence(ConfidenceLevel.High, -0.01m));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Confidence(ConfidenceLevel.High, 1.01m));

        var confidence = new Confidence(ConfidenceLevel.Medium, 0.75m);
        Assert.Equal(0.75m, confidence.Value);
    }

    [Fact]
    public void StrategyResultSupportsEqualityAndRoundTripSerialization()
    {
        var payload = new TestValue(7);
        List<Assumption> assumptions =
        [
            new Assumption("fuel_source", "Using running lap average")
        ];
        List<StrategyWarning> warnings =
        [
            new StrategyWarning("stale_snapshot", "Using stale data", StrategyWarningSeverity.Low)
        ];
        CalculationTrace calculationTrace = new(
            [
                new CalculationStep("remaining_fuel", "12.5"),
            ]);

        var source = new StrategyResult<TestValue>(
            123,
            payload,
            new Confidence(ConfidenceLevel.High, 0.93m),
            assumptions,
            warnings,
            calculationTrace);

        string json = JsonSerializer.Serialize(source);
        var restored = JsonSerializer.Deserialize<StrategyResult<TestValue>>(json);

        Assert.NotNull(restored);
        Assert.Equal(source, restored);
    }

    [Fact]
    public void CalculationTraceRejectsNullSteps()
    {
        _ = Assert.Throws<ArgumentException>(() => new CalculationStep(null!, "value"));
        _ = Assert.Throws<ArgumentException>(() => new CalculationStep("name", null!));
        _ = Assert.Throws<ArgumentNullException>(() => new CalculationTrace(null!));
    }

    [Theory]
    [InlineData("", "desc")]
    [InlineData("code", "")]
    public void AssumptionRequiresRequiredFields(string code, string description)
    {
        _ = Assert.Throws<ArgumentException>(() => new Assumption(code, description));
    }

    [Theory]
    [InlineData("", "message", StrategyWarningSeverity.Low)]
    [InlineData("code", "", StrategyWarningSeverity.Low)]
    public void WarningRequiresRequiredFields(string code, string message, StrategyWarningSeverity severity)
    {
        _ = Assert.Throws<ArgumentException>(() => new StrategyWarning(code, message, severity));
    }
}
