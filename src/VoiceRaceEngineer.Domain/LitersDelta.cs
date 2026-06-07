namespace VoiceRaceEngineer.Domain;

public readonly record struct LitersDelta(decimal Value)
{
    public static LitersDelta Zero => new(0m);

    public static LitersDelta operator +(LitersDelta left, LitersDelta right) => new(left.Value + right.Value);

    public static LitersDelta operator -(LitersDelta left, LitersDelta right) => new(left.Value - right.Value);
}
