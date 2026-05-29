namespace SGame.Data
{
    public static class PlayerSession
    {
        private static string _activeProfileId;
        private static string _selectedMissionId = "P1-M1";
        private static string _selectedMissionSceneName = "P1_M1";

        public static string ActiveProfileId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_activeProfileId))
                {
                    _activeProfileId = SaveService.GetActiveProfileId();
                }

                return _activeProfileId;
            }
        }

        public static void SetActiveProfileId(string profileId)
        {
            if (!string.IsNullOrWhiteSpace(profileId))
            {
                _activeProfileId = profileId;
            }
        }

        public static string SelectedMissionId => _selectedMissionId;
        public static string SelectedMissionSceneName => _selectedMissionSceneName;

        public static void SetSelectedMission(string missionId, string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(missionId))
            {
                _selectedMissionId = missionId;
            }

            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                _selectedMissionSceneName = sceneName;
            }
        }
    }
}
