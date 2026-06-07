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

    private static readonly Type[] _unitTypes =
    [
        typeof(Liters),
        typeof(LitersDelta),
        typeof(LitersPerLap),
        typeof(Laps),
        typeof(Percentage),
    ];

    private static readonly (Type ownerType, string operatorName, Type operandType, Type returnType)[] _expectedBinaryOperators =
    [
        (typeof(Liters), "op_Addition", typeof(Liters), typeof(Liters)),
        (typeof(Liters), "op_Subtraction", typeof(Liters), typeof(LitersDelta)),
        (typeof(LitersDelta), "op_Addition", typeof(LitersDelta), typeof(LitersDelta)),
        (typeof(LitersDelta), "op_Subtraction", typeof(LitersDelta), typeof(LitersDelta)),
    ];

    public static TheoryData<Type, string, Type, Type> PublicBinaryOperatorContractData => new()
    {
        { typeof(Liters), "op_Addition", typeof(Liters), typeof(Liters) },
        { typeof(Liters), "op_Subtraction", typeof(Liters), typeof(LitersDelta) },
        { typeof(LitersDelta), "op_Addition", typeof(LitersDelta), typeof(LitersDelta) },
        { typeof(LitersDelta), "op_Subtraction", typeof(LitersDelta), typeof(LitersDelta) },
    };

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

    [Theory]
    [MemberData(nameof(PublicBinaryOperatorContractData))]
    public void PublicBinaryUnitOperatorsUseExpectedSignatures(
        Type ownerType,
        string operatorName,
        Type operandType,
        Type returnType)
    {
        var publicOperator = ownerType.GetMethod(
            operatorName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            null,
            [operandType, operandType],
            null);

        Assert.NotNull(publicOperator);
        Assert.Equal(operandType, publicOperator!.GetParameters()[0].ParameterType);
        Assert.Equal(operandType, publicOperator.GetParameters()[1].ParameterType);
        Assert.Equal(returnType, publicOperator.ReturnType);
        Assert.Contains(publicOperator.ReturnType, _unitTypes);
    }

    [Fact]
    public void UnitTypesExposeOnlyExpectedPublicBinaryOperators()
    {
        var expectedOperatorSignatures = _expectedBinaryOperators
            .Select(operatorInfo => (operatorInfo.ownerType, operatorInfo.operatorName, operatorInfo.operandType, operatorInfo.operandType, operatorInfo.returnType))
            .ToHashSet();

        var publicOperatorSignatures = _unitTypes
            .SelectMany(type => type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            .Where(method => method.IsSpecialName && method.Name is "op_Addition" or "op_Subtraction")
            .Select(method =>
            {
                var parameters = method.GetParameters();
                return (
                    method.DeclaringType!,
                    method.Name,
                    parameters[0].ParameterType,
                    parameters[1].ParameterType,
                    method.ReturnType);
            })
            .ToHashSet();

        Assert.Equal(expectedOperatorSignatures.Count, publicOperatorSignatures.Count);
        Assert.All(expectedOperatorSignatures, expected => Assert.Contains(expected, publicOperatorSignatures));
    }
}
