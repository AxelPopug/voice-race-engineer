namespace VoiceRaceEngineer.Domain;

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
