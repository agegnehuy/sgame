using SGame.Data;
using SGame.Core;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class MissionMapController : MonoBehaviour
    {
        [SerializeField] private string profileId = "";
        [SerializeField] private string missionId = "P1-M1";
        [SerializeField] private Button missionButton;
        [SerializeField] private GameObject lockOverlay;
        [SerializeField] private Text missionLabel;
        [SerializeField] private string unlockedLabelDefault = "Mission";
        [SerializeField] private string lockedLabelDefault = "Locked";
        [SerializeField] private string unlockedLabelKey = "ui.mission.unlocked";
        [SerializeField] private string lockedLabelKey = "ui.mission.locked";
        [SerializeField] private SceneFlowManager sceneFlowManager;
        [SerializeField] private string missionSceneName = "P1_M1";

        private bool _isUnlocked;

        private void Start()
        {
            if (missionButton != null)
            {
                missionButton.onClick.AddListener(OnMissionClicked);
            }

            Refresh();
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            if (missionButton != null)
            {
                missionButton.onClick.RemoveListener(OnMissionClicked);
            }

            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        public void Refresh()
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                profileId = PlayerSession.ActiveProfileId;
            }

            _isUnlocked = ProgressionService.IsMissionUnlocked(profileId, missionId);

            if (missionButton != null)
            {
                missionButton.interactable = _isUnlocked;
            }

            if (lockOverlay != null)
            {
                lockOverlay.SetActive(!_isUnlocked);
            }

            if (missionLabel != null)
            {
                missionLabel.text = _isUnlocked
                    ? GetLocalized(unlockedLabelKey, unlockedLabelDefault)
                    : GetLocalized(lockedLabelKey, lockedLabelDefault);
            }
        }

        private void OnMissionClicked()
        {
            if (!_isUnlocked)
            {
                return;
            }

            PlayerSession.SetSelectedMission(missionId, missionSceneName);

            if (sceneFlowManager != null)
            {
                sceneFlowManager.LoadMissionByName(missionSceneName);
            }
        }

        private void OnLanguageChanged(string _)
        {
            Refresh();
        }

        private static string GetLocalized(string key, string fallback)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return fallback;
            }

            var value = LocalizationService.Get(key);
            return value == key ? fallback : value;
        }
    }
}
