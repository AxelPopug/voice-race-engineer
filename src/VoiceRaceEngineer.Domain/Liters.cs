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
