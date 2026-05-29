using System;
using System.IO;
using UnityEngine;

namespace SGame.Data
{
    [Serializable]
    public class AnalyticsEventRecord
    {
        public string tsUtc;
        public string profileId;
        public string missionId;
        public string eventType;
        public int isSafe = -1;
        public string reasonKey;
        public float elapsedSeconds;
        public int score = -1;
        public int stars = -1;
        public int earnedCoins = -1;
        public int safeDecisions = -1;
        public int unsafeDecisions = -1;
        public string tag;
    }

    public static class AnalyticsEventService
    {
        private static string AnalyticsDirectoryPath => Path.Combine(Application.persistentDataPath, "analytics");

        public static void LogDecision(string profileId, string missionId, bool isSafe, string reasonKey, float elapsedSeconds)
        {
            Write(new AnalyticsEventRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                missionId = missionId,
                eventType = "decision",
                isSafe = isSafe ? 1 : 0,
                reasonKey = reasonKey,
                elapsedSeconds = Mathf.Max(0f, elapsedSeconds)
            });
        }

        public static void LogMissionSummary(
            string profileId,
            string missionId,
            int safeDecisions,
            int unsafeDecisions,
            int score,
            int stars,
            int earnedCoins,
            float elapsedSeconds)
        {
            Write(new AnalyticsEventRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                missionId = missionId,
                eventType = "mission_summary",
                safeDecisions = safeDecisions,
                unsafeDecisions = unsafeDecisions,
                score = score,
                stars = stars,
                earnedCoins = earnedCoins,
                elapsedSeconds = Mathf.Max(0f, elapsedSeconds)
            });
        }

        public static void LogChallengeEvent(string profileId, string missionId, string challengeType, string reasonKey, float elapsedSeconds)
        {
            Write(new AnalyticsEventRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                missionId = missionId,
                eventType = "challenge_event",
                tag = challengeType,
                reasonKey = reasonKey,
                elapsedSeconds = Mathf.Max(0f, elapsedSeconds)
            });
        }

        private static void Write(AnalyticsEventRecord record)
        {
            try
            {
                if (!Directory.Exists(AnalyticsDirectoryPath))
                {
                    Directory.CreateDirectory(AnalyticsDirectoryPath);
                }

                var filePath = Path.Combine(AnalyticsDirectoryPath, $"events_{DateTime.UtcNow:yyyy_MM_dd}.jsonl");
                var json = JsonUtility.ToJson(record);
                File.AppendAllText(filePath, json + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Analytics write skipped: {ex.Message}");
            }
        }

        public static int ClearAllAnalyticsLogs()
        {
            if (!Directory.Exists(AnalyticsDirectoryPath))
            {
                return 0;
            }

            var files = Directory.GetFiles(AnalyticsDirectoryPath, "events_*.jsonl");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            return files.Length;
        }
    }
}
