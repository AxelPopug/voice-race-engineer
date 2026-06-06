namespace VoiceRaceEngineer.Domain;

public readonly record struct Liters
{
    public Liters(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public decimal Value { get; }

    public static Liters Zero => new(0m);

    public static Liters operator +(Liters left, Liters right) => new(left.Value + right.Value);

    public static LitersDelta operator -(Liters left, Liters right) => new(left.Value - right.Value);
}

public readonly record struct LitersDelta(decimal Value)
{
    public static LitersDelta Zero => new(0m);

    public static LitersDelta operator +(LitersDelta left, LitersDelta right) => new(left.Value + right.Value);

    public static LitersDelta operator -(LitersDelta left, LitersDelta right) => new(left.Value - right.Value);
}

public readonly record struct LitersPerLap
{
    public LitersPerLap(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public decimal Value { get; }

    public static LitersPerLap Zero => new(0m);
}

public readonly record struct Laps
{
    public Laps(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public decimal Value { get; }

    public static Laps Zero => new(0m);
}

public readonly record struct Percentage
{
    public Percentage(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public decimal Value { get; }

    public static Percentage Zero => new(0m);
}
