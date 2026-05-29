using SGame.Gameplay;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private Button crossButton;
        [SerializeField] private Button waitButton;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Text scoreText;
        [Header("Localization keys")]
        [SerializeField] private string safeLabelKey = "ui.feedback.safe";
        [SerializeField] private string unsafeLabelKey = "ui.feedback.unsafe";
        [SerializeField] private string safeLabelDefault = "SAFE";
        [SerializeField] private string unsafeLabelDefault = "UNSAFE";
        [SerializeField] private string scorePrefixKey = "ui.score.prefix";
        [SerializeField] private string starsPrefixKey = "ui.score.stars_prefix";
        [SerializeField] private string coinsSuffixKey = "ui.score.coins_suffix";
        [SerializeField] private string scorePrefixDefault = "Score: ";
        [SerializeField] private string starsPrefixDefault = " | Stars: ";
        [SerializeField] private string coinsSuffixDefault = " coins";

        private void Awake()
        {
            if (crossButton != null) crossButton.onClick.AddListener(OnCrossPressed);
            if (waitButton != null) waitButton.onClick.AddListener(OnWaitPressed);
        }

        private void OnEnable()
        {
            if (missionController == null) return;
            missionController.DecisionEvaluated += OnDecisionEvaluated;
            missionController.MissionCompleted += OnMissionCompleted;
        }

        private void OnDisable()
        {
            if (missionController == null) return;
            missionController.DecisionEvaluated -= OnDecisionEvaluated;
            missionController.MissionCompleted -= OnMissionCompleted;
        }

        private void OnCrossPressed()
        {
            missionController?.OnCrossAttempt();
        }

        private void OnWaitPressed()
        {
            missionController?.OnWaitAction();
        }

        private void OnDecisionEvaluated(bool isSafe, string reasonKey)
        {
            if (feedbackText == null) return;
            var stateLabel = isSafe
                ? GetLocalized(safeLabelKey, safeLabelDefault)
                : GetLocalized(unsafeLabelKey, unsafeLabelDefault);
            feedbackText.text = $"{stateLabel} - {LocalizationService.Get(reasonKey)}";
        }

        private void OnMissionCompleted(int score, int stars, int earnedCoins, int totalCoins)
        {
            if (scoreText == null) return;
            scoreText.text =
                $"{GetLocalized(scorePrefixKey, scorePrefixDefault)}{score}" +
                $"{GetLocalized(starsPrefixKey, starsPrefixDefault)}{stars}" +
                $" | +{earnedCoins}{GetLocalized(coinsSuffixKey, coinsSuffixDefault)}";
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
