using UnityEngine;

namespace SGame.Data
{
    public class AnalyticsProfileInsights
    {
        public string profileId;
        public int totalDecisionCount;
        public int safeDecisionCount;
        public int unsafeDecisionCount;
        public float safeRatio01;
        public int totalChallengeEvents;
        public int missionCount;
        public string topMissionId;
        public int topMissionBestScore;
    }

    public static class AnalyticsInsightsService
    {
        public static AnalyticsProfileInsights BuildProfileInsights(string profileId)
        {
            var insights = new AnalyticsProfileInsights
            {
                profileId = profileId,
                topMissionId = string.Empty,
                topMissionBestScore = -1
            };

            if (string.IsNullOrWhiteSpace(profileId))
            {
                return insights;
            }

            var summary = AnalyticsSummaryExportService.BuildSummarySnapshot();
            foreach (var profile in summary.profiles)
            {
                if (profile.profileId != profileId)
                {
                    continue;
                }

                insights.totalDecisionCount = profile.totalDecisionCount;
                insights.safeDecisionCount = profile.totalSafeDecisions;
                insights.unsafeDecisionCount = profile.totalUnsafeDecisions;
                insights.missionCount = profile.missions != null ? profile.missions.Count : 0;

                if (insights.totalDecisionCount > 0)
                {
                    insights.safeRatio01 = Mathf.Clamp01(insights.safeDecisionCount / (float)insights.totalDecisionCount);
                }

                if (profile.missions == null)
                {
                    break;
                }

                foreach (var mission in profile.missions)
                {
                    if (mission == null)
                    {
                        continue;
                    }

                    insights.totalChallengeEvents += mission.distractionEvents + mission.pressureStartEvents + mission.pressureEndEvents;

                    if (mission.bestScore > insights.topMissionBestScore)
                    {
                        insights.topMissionBestScore = mission.bestScore;
                        insights.topMissionId = mission.missionId;
                    }
                }

                break;
            }

            if (insights.topMissionBestScore < 0)
            {
                insights.topMissionBestScore = 0;
            }

            return insights;
        }
    }
}
