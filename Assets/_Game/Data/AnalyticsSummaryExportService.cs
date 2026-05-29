using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SGame.Data
{
    [Serializable]
    public class MissionAnalyticsAggregate
    {
        public string missionId;
        public int decisionCount;
        public int safeDecisionCount;
        public int unsafeDecisionCount;
        public int missionSummaryCount;
        public int bestScore;
        public float averageScore;
        public int totalEarnedCoins;
        public int distractionEvents;
        public int pressureStartEvents;
        public int pressureEndEvents;
        public float averageDurationSeconds;
    }

    [Serializable]
    public class ProfileAnalyticsAggregate
    {
        public string profileId;
        public int totalDecisionCount;
        public int totalSafeDecisions;
        public int totalUnsafeDecisions;
        public List<MissionAnalyticsAggregate> missions = new();
    }

    [Serializable]
    public class AnalyticsSummaryReport
    {
        public string generatedAtUtc;
        public int profileCount;
        public int sourceFileCount;
        public List<ProfileAnalyticsAggregate> profiles = new();
    }

    public static class AnalyticsSummaryExportService
    {
        public static string ExportSummary()
        {
            var report = BuildSummarySnapshot();
            var json = JsonUtility.ToJson(report, true);

            var exportsDir = Path.Combine(Application.persistentDataPath, "exports");
            if (!Directory.Exists(exportsDir))
            {
                Directory.CreateDirectory(exportsDir);
            }

            var fileName = $"analytics_summary_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var fullPath = Path.Combine(exportsDir, fileName);
            File.WriteAllText(fullPath, json);
            return fullPath;
        }

        public static AnalyticsSummaryReport BuildSummarySnapshot()
        {
            var report = new AnalyticsSummaryReport
            {
                generatedAtUtc = DateTime.UtcNow.ToString("O")
            };

            var analyticsDir = Path.Combine(Application.persistentDataPath, "analytics");
            if (!Directory.Exists(analyticsDir))
            {
                return report;
            }

            var files = Directory.GetFiles(analyticsDir, "events_*.jsonl");
            report.sourceFileCount = files.Length;

            var profileMap = new Dictionary<string, ProfileAnalyticsAggregate>();
            var missionMap = new Dictionary<string, MissionAnalyticsAggregate>();

            foreach (var file in files)
            {
                foreach (var line in File.ReadLines(file))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var ev = JsonUtility.FromJson<AnalyticsEventRecord>(line);
                    if (ev == null || string.IsNullOrWhiteSpace(ev.profileId) || string.IsNullOrWhiteSpace(ev.missionId))
                    {
                        continue;
                    }

                    if (!profileMap.TryGetValue(ev.profileId, out var profile))
                    {
                        profile = new ProfileAnalyticsAggregate { profileId = ev.profileId };
                        profileMap[ev.profileId] = profile;
                    }

                    var missionKey = $"{ev.profileId}::{ev.missionId}";
                    if (!missionMap.TryGetValue(missionKey, out var mission))
                    {
                        mission = new MissionAnalyticsAggregate
                        {
                            missionId = ev.missionId,
                            bestScore = 0
                        };
                        missionMap[missionKey] = mission;
                        profile.missions.Add(mission);
                    }

                    Accumulate(profile, mission, ev);
                }
            }

            report.profiles.AddRange(profileMap.Values);
            report.profileCount = report.profiles.Count;
            return report;
        }

        private static void Accumulate(ProfileAnalyticsAggregate profile, MissionAnalyticsAggregate mission, AnalyticsEventRecord ev)
        {
            switch (ev.eventType)
            {
                case "decision":
                    mission.decisionCount += 1;
                    profile.totalDecisionCount += 1;
                    if (ev.isSafe == 1)
                    {
                        mission.safeDecisionCount += 1;
                        profile.totalSafeDecisions += 1;
                    }
                    else if (ev.isSafe == 0)
                    {
                        mission.unsafeDecisionCount += 1;
                        profile.totalUnsafeDecisions += 1;
                    }
                    break;

                case "challenge_event":
                    if (ev.tag == "distraction") mission.distractionEvents += 1;
                    if (ev.tag == "pressure_start") mission.pressureStartEvents += 1;
                    if (ev.tag == "pressure_end") mission.pressureEndEvents += 1;
                    break;

                case "mission_summary":
                    mission.missionSummaryCount += 1;
                    if (ev.score >= 0)
                    {
                        mission.bestScore = Math.Max(mission.bestScore, ev.score);
                        mission.averageScore = RunningAverage(mission.averageScore, mission.missionSummaryCount, ev.score);
                    }
                    if (ev.earnedCoins >= 0)
                    {
                        mission.totalEarnedCoins += ev.earnedCoins;
                    }
                    if (ev.elapsedSeconds > 0f)
                    {
                        mission.averageDurationSeconds = RunningAverage(mission.averageDurationSeconds, mission.missionSummaryCount, ev.elapsedSeconds);
                    }
                    break;
            }
        }

        private static float RunningAverage(float currentAverage, int countAfterIncrement, float newValue)
        {
            if (countAfterIncrement <= 1)
            {
                return newValue;
            }

            var previousTotal = currentAverage * (countAfterIncrement - 1);
            return (previousTotal + newValue) / countAfterIncrement;
        }
    }
}
