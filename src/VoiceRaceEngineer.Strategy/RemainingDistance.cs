using VoiceRaceEngineer.Domain;

namespace VoiceRaceEngineer.Strategy;

public static class RemainingDistance
{
    public static LapLimitedRemainingDistanceResult CalculateLapLimited(
        Laps completedLaps,
        Laps currentLapProgress,
        Laps scheduledLaps)
    {
        EnsureNonNegative(completedLaps.Value, nameof(completedLaps));
        EnsureWholeLapCount(completedLaps.Value, nameof(completedLaps));
        EnsureNormalizedLapProgress(currentLapProgress.Value, nameof(currentLapProgress));
        EnsureNonNegative(scheduledLaps.Value, nameof(scheduledLaps));
        EnsureWholeLapCount(scheduledLaps.Value, nameof(scheduledLaps));

        decimal playerProgress = completedLaps.Value + currentLapProgress.Value;
        decimal remainingEquivalentDistance = decimal.Max(0m, scheduledLaps.Value - playerProgress);
        var remainingDistance = new Laps(remainingEquivalentDistance);

        return new LapLimitedRemainingDistanceResult(
            remainingDistance,
            ToFinishLineCrossings(remainingEquivalentDistance),
            remainingEquivalentDistance == 0m);
    }

    private static FinishLineCrossings ToFinishLineCrossings(decimal remainingEquivalentDistance)
    {
        decimal crossings = decimal.Ceiling(remainingEquivalentDistance);

        return crossings <= int.MaxValue
            ? new FinishLineCrossings(decimal.ToInt32(crossings))
            : throw new ArgumentOutOfRangeException(
                nameof(remainingEquivalentDistance),
                remainingEquivalentDistance,
                "Remaining distance is too large to represent as finish-line crossings.");
    }

    private static void EnsureNonNegative(decimal value, string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);
    }

    private static void EnsureWholeLapCount(decimal value, string parameterName)
    {
        if (decimal.Truncate(value) != value)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "Lap count must be a whole number.");
        }
    }

    private static void EnsureNormalizedLapProgress(decimal value, string parameterName)
    {
        EnsureNonNegative(value, parameterName);

        if (value > 1m)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Lap progress must be between 0 and 1.");
        }
    }
}
