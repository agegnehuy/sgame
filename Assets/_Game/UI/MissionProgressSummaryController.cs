using System;
using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    [Serializable]
    public class MissionSummaryBinding
    {
        public string missionId = "P1-M1";
        public string missionTitleKey = "";
        public Text missionTitleText;
        public Text bestScoreText;
        public Text starsText;
        public Text attemptsText;
        public Text unlockStatusText;
    }

    public class MissionProgressSummaryController : MonoBehaviour
    {
        [SerializeField] private MissionSummaryBinding[] missionBindings;
        [SerializeField] private string bestScorePrefixDefault = "Best: ";
        [SerializeField] private string starsPrefixDefault = "Stars: ";
        [SerializeField] private string attemptsPrefixDefault = "Attempts: ";
        [SerializeField] private string unlockedLabelDefault = "Unlocked";
        [SerializeField] private string lockedLabelDefault = "Locked";
        [Header("Localization keys")]
        [SerializeField] private string bestScorePrefixKey = "ui.progress.best_prefix";
        [SerializeField] private string starsPrefixKey = "ui.progress.stars_prefix";
        [SerializeField] private string attemptsPrefixKey = "ui.progress.attempts_prefix";
        [SerializeField] private string unlockedLabelKey = "ui.progress.unlocked";
        [SerializeField] private string lockedLabelKey = "ui.progress.locked";

        private void OnEnable()
        {
            SaveService.ProgressChanged += OnProgressChanged;
            LocalizationService.LanguageChanged += OnLanguageChanged;
            Refresh();
        }

        private void OnDisable()
        {
            SaveService.ProgressChanged -= OnProgressChanged;
            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        public void Refresh()
        {
            var profileId = PlayerSession.ActiveProfileId;
            if (missionBindings == null)
            {
                return;
            }

            foreach (var binding in missionBindings)
            {
                if (binding == null || string.IsNullOrWhiteSpace(binding.missionId))
                {
                    continue;
                }

                var missionRecord = SaveService.GetMissionResult(profileId, binding.missionId);
                var isUnlocked = ProgressionService.IsMissionUnlocked(profileId, binding.missionId);

                if (binding.missionTitleText != null)
                {
                    if (!string.IsNullOrWhiteSpace(binding.missionTitleKey))
                    {
                        var titleValue = LocalizationService.Get(binding.missionTitleKey);
                        binding.missionTitleText.text = titleValue != binding.missionTitleKey ? titleValue : binding.missionId;
                    }
                    else
                    {
                        binding.missionTitleText.text = binding.missionId;
                    }
                }

                if (binding.bestScoreText != null)
                {
                    var value = missionRecord != null ? missionRecord.bestScore.ToString() : "-";
                    binding.bestScoreText.text = $"{GetLocalized(bestScorePrefixKey, bestScorePrefixDefault)}{value}";
                }

                if (binding.starsText != null)
                {
                    var value = missionRecord != null ? missionRecord.stars.ToString() : "0";
                    binding.starsText.text = $"{GetLocalized(starsPrefixKey, starsPrefixDefault)}{value}";
                }

                if (binding.attemptsText != null)
                {
                    var value = missionRecord != null ? missionRecord.attempts.ToString() : "0";
                    binding.attemptsText.text = $"{GetLocalized(attemptsPrefixKey, attemptsPrefixDefault)}{value}";
                }

                if (binding.unlockStatusText != null)
                {
                    binding.unlockStatusText.text = isUnlocked
                        ? GetLocalized(unlockedLabelKey, unlockedLabelDefault)
                        : GetLocalized(lockedLabelKey, lockedLabelDefault);
                }
            }
        }

        private void OnLanguageChanged(string _)
        {
            Refresh();
        }

        private void OnProgressChanged(string profileId)
        {
            if (profileId == PlayerSession.ActiveProfileId)
            {
                Refresh();
            }
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
