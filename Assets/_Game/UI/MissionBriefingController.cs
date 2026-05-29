using SGame.Gameplay;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class MissionBriefingController : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private Text missionTitleText;
        [SerializeField] private Text missionBriefingText;

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += OnLanguageChanged;
            if (missionController != null)
            {
                missionController.MissionIdentityChanged += OnMissionIdentityChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
            if (missionController != null)
            {
                missionController.MissionIdentityChanged -= OnMissionIdentityChanged;
            }
        }

        public void Refresh()
        {
            if (missionController == null)
            {
                return;
            }

            if (missionTitleText != null)
            {
                missionTitleText.text = LocalizationService.Get(missionController.CurrentMissionTitleKey);
            }

            if (missionBriefingText != null)
            {
                missionBriefingText.text = LocalizationService.Get(missionController.CurrentMissionBriefingKey);
            }
        }

        private void OnLanguageChanged(string _)
        {
            Refresh();
        }

        private void OnMissionIdentityChanged()
        {
            Refresh();
        }
    }
}
