using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SGame.Data
{
    [Serializable]
    public class FacilitatorMissionReport
    {
        public string missionId;
        public int bestScore;
        public int stars;
        public int attempts;
    }

    [Serializable]
    public class FacilitatorProfileReport
    {
        public string profileId;
        public string displayName;
        public string avatarId;
        public int coins;
        public List<FacilitatorMissionReport> missions = new();
    }

    [Serializable]
    public class FacilitatorReport
    {
        public string generatedAtUtc;
        public int profileCount;
        public List<FacilitatorProfileReport> profiles = new();
    }

    public static class FacilitatorReportService
    {
        public static string ExportAllProfilesReport()
        {
            var report = BuildReport();
            var json = JsonUtility.ToJson(report, true);

            var exportsDir = Path.Combine(Application.persistentDataPath, "exports");
            if (!Directory.Exists(exportsDir))
            {
                Directory.CreateDirectory(exportsDir);
            }

            var fileName = $"facilitator_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var fullPath = Path.Combine(exportsDir, fileName);
            File.WriteAllText(fullPath, json);
            return fullPath;
        }

        private static FacilitatorReport BuildReport()
        {
            var profiles = SaveService.LoadProfiles().profiles;
            var report = new FacilitatorReport
            {
                generatedAtUtc = DateTime.UtcNow.ToString("O"),
                profileCount = profiles.Count
            };

            foreach (var profile in profiles)
            {
                if (profile == null || string.IsNullOrWhiteSpace(profile.profileId))
                {
                    continue;
                }

                var snapshot = SaveService.GetProgressSnapshot(profile.profileId);
                var profileReport = new FacilitatorProfileReport
                {
                    profileId = profile.profileId,
                    displayName = profile.displayName,
                    avatarId = profile.avatarId,
                    coins = snapshot.coins
                };

                foreach (var mission in snapshot.missions)
                {
                    profileReport.missions.Add(new FacilitatorMissionReport
                    {
                        missionId = mission.missionId,
                        bestScore = mission.bestScore,
                        stars = mission.stars,
                        attempts = mission.attempts
                    });
                }

                report.profiles.Add(profileReport);
            }

            report.profileCount = report.profiles.Count;
            return report;
        }
    }
}
