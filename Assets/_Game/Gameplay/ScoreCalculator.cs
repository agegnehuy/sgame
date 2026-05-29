using UnityEngine;

namespace SGame.Gameplay
{
    public readonly struct MissionScoreInput
    {
        public MissionScoreInput(int safeDecisions, int unsafeDecisions, float timingDiscipline01)
        {
            SafeDecisions = safeDecisions;
            UnsafeDecisions = unsafeDecisions;
            TimingDiscipline01 = Mathf.Clamp01(timingDiscipline01);
        }

        public int SafeDecisions { get; }
        public int UnsafeDecisions { get; }
        public float TimingDiscipline01 { get; }
    }

    public readonly struct MissionScoreResult
    {
        public MissionScoreResult(int finalScore, int stars)
        {
            FinalScore = finalScore;
            Stars = stars;
        }

        public int FinalScore { get; }
        public int Stars { get; }
    }

    public static class ScoreCalculator
    {
        public static MissionScoreResult Calculate(MissionScoreInput input, MissionDefinition mission)
        {
            var totalAttempts = Mathf.Max(1, input.SafeDecisions + input.UnsafeDecisions);
            var accuracy01 = (float)input.SafeDecisions / totalAttempts;

            var safety01 = Mathf.Clamp01(1f - (input.UnsafeDecisions / (float)totalAttempts));
            var weightedScore =
                (safety01 * mission.safetyWeight) +
                (accuracy01 * mission.accuracyWeight) +
                (input.TimingDiscipline01 * mission.timingWeight);

            var score = Mathf.RoundToInt(weightedScore);
            var stars = score >= 85 ? 3 : score >= 70 ? 2 : score >= 55 ? 1 : 0;

            return new MissionScoreResult(score, stars);
        }
    }
}
