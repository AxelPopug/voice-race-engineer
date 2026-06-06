namespace VoiceRaceEngineer.Domain;

public readonly record struct Liters(decimal Value)
{
    public static Liters Zero => new(0m);

    public static Liters operator +(Liters left, Liters right) => new(left.Value + right.Value);

    public static Liters operator -(Liters left, Liters right) => new(left.Value - right.Value);
}

public readonly record struct LitersPerLap(decimal Value)
{
    public static LitersPerLap Zero => new(0m);
}

public readonly record struct Laps(decimal Value)
{
    public static Laps Zero => new(0m);
}

public readonly record struct Percentage(decimal Value)
{
    public static Percentage Zero => new(0m);
}
