using System;
using System.IO;
using UnityEngine;

namespace SGame.Data
{
    [Serializable]
    public class FacilitatorPinAuditRecord
    {
        public string tsUtc;
        public string profileId;
        public string eventType;
        public int failedAttempts = -1;
        public int maxAttempts = -1;
        public float lockSeconds = -1f;
        public int secondsRemaining = -1;
    }

    public static class FacilitatorPinAuditService
    {
        private static string AuditDirectoryPath => Path.Combine(Application.persistentDataPath, "security");
        private static string AuditFilePath => Path.Combine(AuditDirectoryPath, $"pin_audit_{DateTime.UtcNow:yyyy_MM_dd}.jsonl");

        public static void LogPinSuccess(string profileId)
        {
            Write(new FacilitatorPinAuditRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                eventType = "pin_success"
            });
        }

        public static void LogPinFailed(string profileId, int failedAttempts, int maxAttempts, float lockSeconds)
        {
            Write(new FacilitatorPinAuditRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                eventType = "pin_failed",
                failedAttempts = failedAttempts,
                maxAttempts = maxAttempts,
                lockSeconds = lockSeconds
            });
        }

        public static void LogCooldownBlocked(string profileId, int secondsRemaining)
        {
            Write(new FacilitatorPinAuditRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                eventType = "pin_cooldown_blocked",
                secondsRemaining = secondsRemaining
            });
        }

        public static void LogLockoutStarted(string profileId, int maxAttempts, float lockSeconds)
        {
            Write(new FacilitatorPinAuditRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                eventType = "pin_lockout_started",
                maxAttempts = maxAttempts,
                lockSeconds = lockSeconds
            });
        }

        public static void LogLockoutBlocked(string profileId, int secondsRemaining)
        {
            Write(new FacilitatorPinAuditRecord
            {
                tsUtc = DateTime.UtcNow.ToString("O"),
                profileId = profileId,
                eventType = "pin_lockout_blocked",
                secondsRemaining = secondsRemaining
            });
        }

        public static int ClearAllPinAuditLogs()
        {
            if (!Directory.Exists(AuditDirectoryPath))
            {
                return 0;
            }

            var files = Directory.GetFiles(AuditDirectoryPath, "pin_audit_*.jsonl");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            return files.Length;
        }

        private static void Write(FacilitatorPinAuditRecord record)
        {
            try
            {
                if (!Directory.Exists(AuditDirectoryPath))
                {
                    Directory.CreateDirectory(AuditDirectoryPath);
                }

                var json = JsonUtility.ToJson(record);
                File.AppendAllText(AuditFilePath, json + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"PIN audit write skipped: {ex.Message}");
            }
        }
    }
}
