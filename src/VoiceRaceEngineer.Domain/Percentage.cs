namespace VoiceRaceEngineer.Domain;

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
