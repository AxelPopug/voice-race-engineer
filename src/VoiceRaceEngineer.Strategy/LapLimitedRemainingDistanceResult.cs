using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy;

public sealed record LapLimitedRemainingDistanceResult(
    Laps RemainingEquivalentDistance,
    FinishLineCrossings RemainingFinishLineCrossings,
    bool IsFinished);
