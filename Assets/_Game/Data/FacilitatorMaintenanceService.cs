using System.IO;
using UnityEngine;

namespace SGame.Data
{
    public static class FacilitatorMaintenanceService
    {
        public static int ClearExportFiles()
        {
            var exportsDir = Path.Combine(Application.persistentDataPath, "exports");
            if (!Directory.Exists(exportsDir))
            {
                return 0;
            }

            var files = Directory.GetFiles(exportsDir, "*.json");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            return files.Length;
        }

        public static int ClearAnalyticsLogs()
        {
            return AnalyticsEventService.ClearAllAnalyticsLogs();
        }

        public static int ClearPinAuditLogs()
        {
            return FacilitatorPinAuditService.ClearAllPinAuditLogs();
        }

        public static void ResetActiveProfileProgress()
        {
            var profileId = PlayerSession.ActiveProfileId;
            SaveService.ResetProfileProgress(profileId);
        }
    }
}
