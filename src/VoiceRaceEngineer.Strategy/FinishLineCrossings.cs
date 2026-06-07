namespace VoiceRaceEngineer.Strategy;

public readonly record struct FinishLineCrossings
{
    public FinishLineCrossings(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public int Value { get; }

    public static FinishLineCrossings Zero => new(0);
}
