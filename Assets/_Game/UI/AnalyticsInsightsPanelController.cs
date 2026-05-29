using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class AnalyticsInsightsPanelController : MonoBehaviour
    {
        [SerializeField] private Text totalDecisionsText;
        [SerializeField] private Text safeRatioText;
        [SerializeField] private Text topMissionText;
        [SerializeField] private Text challengeEventsText;
        [SerializeField] private Text missionCountText;
        [SerializeField] private Button refreshButton;

        [Header("Localization keys")]
        [SerializeField] private string totalDecisionsPrefixKey = "ui.analytics.total_decisions";
        [SerializeField] private string safeRatioPrefixKey = "ui.analytics.safe_ratio";
        [SerializeField] private string topMissionPrefixKey = "ui.analytics.top_mission";
        [SerializeField] private string challengeEventsPrefixKey = "ui.analytics.challenge_events";
        [SerializeField] private string missionCountPrefixKey = "ui.analytics.mission_count";
        [SerializeField] private string noDataLabelKey = "ui.analytics.no_data";

        [Header("Fallback labels")]
        [SerializeField] private string totalDecisionsPrefixDefault = "Total decisions: ";
        [SerializeField] private string safeRatioPrefixDefault = "Safe ratio: ";
        [SerializeField] private string topMissionPrefixDefault = "Top mission: ";
        [SerializeField] private string challengeEventsPrefixDefault = "Challenge events: ";
        [SerializeField] private string missionCountPrefixDefault = "Missions played: ";
        [SerializeField] private string noDataLabelDefault = "No data";

        private void Awake()
        {
            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(Refresh);
            }
        }

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

        private void OnDestroy()
        {
            if (refreshButton != null)
            {
                refreshButton.onClick.RemoveListener(Refresh);
            }
        }

        public void Refresh()
        {
            var profileId = PlayerSession.ActiveProfileId;
            var insights = AnalyticsInsightsService.BuildProfileInsights(profileId);
            var safeRatioPct = Mathf.RoundToInt(insights.safeRatio01 * 100f);
            var topMissionValue = string.IsNullOrWhiteSpace(insights.topMissionId)
                ? GetLocalized(noDataLabelKey, noDataLabelDefault)
                : $"{insights.topMissionId} ({insights.topMissionBestScore})";

            if (totalDecisionsText != null)
            {
                totalDecisionsText.text = $"{GetLocalized(totalDecisionsPrefixKey, totalDecisionsPrefixDefault)}{insights.totalDecisionCount}";
            }

            if (safeRatioText != null)
            {
                safeRatioText.text = $"{GetLocalized(safeRatioPrefixKey, safeRatioPrefixDefault)}{safeRatioPct}%";
            }

            if (topMissionText != null)
            {
                topMissionText.text = $"{GetLocalized(topMissionPrefixKey, topMissionPrefixDefault)}{topMissionValue}";
            }

            if (challengeEventsText != null)
            {
                challengeEventsText.text = $"{GetLocalized(challengeEventsPrefixKey, challengeEventsPrefixDefault)}{insights.totalChallengeEvents}";
            }

            if (missionCountText != null)
            {
                missionCountText.text = $"{GetLocalized(missionCountPrefixKey, missionCountPrefixDefault)}{insights.missionCount}";
            }
        }

        private void OnProgressChanged(string profileId)
        {
            if (profileId == PlayerSession.ActiveProfileId)
            {
                Refresh();
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
