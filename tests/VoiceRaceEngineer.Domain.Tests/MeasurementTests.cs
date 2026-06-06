namespace VoiceRaceEngineer.Domain.Tests;

public sealed class MeasurementTests
{
    public static TheoryData<Func<decimal, object>> NonNegativeFactories =>
    [
        value => new Liters(value),
        value => new LitersPerLap(value),
        value => new Laps(value),
        value => new Percentage(value),
    ];

    [Theory]
    [MemberData(nameof(NonNegativeFactories))]
    public void MeasurementConstructorsRejectNegativeValues(Func<decimal, object> createMeasurement)
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => createMeasurement(-0.01m));
    }

    [Fact]
    public void ZeroValuesAreValid()
    {
        Assert.Equal(0m, Liters.Zero.Value);
        Assert.Equal(0m, LitersDelta.Zero.Value);
        Assert.Equal(0m, LitersPerLap.Zero.Value);
        Assert.Equal(0m, Laps.Zero.Value);
        Assert.Equal(0m, Percentage.Zero.Value);
    }

    [Fact]
    public void LitersArithmeticUsesOnlyLitersOperands()
    {
        var fuelRemaining = new Liters(10m);
        var fuelAdded = new Liters(2m);
        var fuelBurned = new Liters(3m);

        var sum = fuelRemaining + fuelAdded;
        var difference = fuelRemaining - fuelBurned;

        Assert.Equal(new Liters(12m), sum);
        Assert.Equal(new LitersDelta(7m), difference);
    }

    [Fact]
    public void LitersDeltaAllowsSignedFuelDifference()
    {
        Assert.Equal(-2m, new LitersDelta(-2m).Value);
        Assert.Equal(2m, new LitersDelta(2m).Value);
    }

    [Fact]
    public void UnitOperatorsDoNotExposeCrossUnitArithmetic()
    {
        var measurementTypes = new[] { typeof(Liters), typeof(LitersDelta), typeof(LitersPerLap), typeof(Laps), typeof(Percentage) };

        var publicOperators = measurementTypes
            .SelectMany(type => type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            .Where(method => method.IsSpecialName && method.Name is "op_Addition" or "op_Subtraction");

        foreach (var publicOperator in publicOperators)
        {
            var parameterTypes = publicOperator.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

            Assert.Equal(parameterTypes[0], parameterTypes[1]);
            Assert.Contains(publicOperator.ReturnType, measurementTypes);

            if (parameterTypes[0] == typeof(Liters) && publicOperator.Name == "op_Subtraction")
            {
                Assert.Equal(typeof(LitersDelta), publicOperator.ReturnType);
            }
            else
            {
                Assert.Equal(parameterTypes[0], publicOperator.ReturnType);
            }
        }
    }
}
