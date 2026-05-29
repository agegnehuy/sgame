namespace SGame.Data
{
    public static class ProgressionService
    {
        public static bool IsMissionUnlocked(string profileId, string missionId)
        {
            if (missionId == "P1-M1")
            {
                return true;
            }

            var progress = SaveService.LoadProgress(profileId);
            return missionId switch
            {
                "P1-M2" => HasMissionClear(progress, "P1-M1"),
                "P1-M3" => HasMissionClear(progress, "P1-M2"),
                "P1-M4" => HasMissionClear(progress, "P1-M3"),
                "P1-M5" => HasMissionClear(progress, "P1-M4"),
                _ => false
            };
        }

        private static bool HasMissionClear(ProfileProgress progress, string missionId)
        {
            var record = progress.missions.Find(m => m.missionId == missionId);
            return record != null && record.bestScore >= 55;
        }
    }
}
