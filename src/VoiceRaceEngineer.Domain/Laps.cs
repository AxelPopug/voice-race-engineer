namespace VoiceRaceEngineer.Domain;

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
