using SGame.Gameplay;
using SGame.Core;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class ResultPanelController : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private Text resultTitleText;
        [SerializeField] private Text resultDetailText;
        [SerializeField] private Text starsText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Button backHomeButton;
        [SerializeField] private SceneFlowManager sceneFlowManager;

        [Header("Localization keys")]
        [SerializeField] private string passTitleKey = "result.pass";
        [SerializeField] private string retryTitleKey = "result.retry";
        [SerializeField] private string scorePrefixKey = "ui.score.prefix";
        [SerializeField] private string starsPrefixKey = "ui.score.stars_prefix_short";
        [SerializeField] private string coinsPrefixKey = "ui.coins.prefix";
        [SerializeField] private string totalPrefixKey = "ui.total.prefix";
        [SerializeField] private string passTitleDefault = "Mission Complete";
        [SerializeField] private string retryTitleDefault = "Try Again";
        [SerializeField] private string scorePrefixDefault = "Score: ";
        [SerializeField] private string starsPrefixDefault = "Stars: ";
        [SerializeField] private string coinsPrefixDefault = "Coins: ";
        [SerializeField] private string totalPrefixDefault = "Total: ";

        private void Awake()
        {
            if (backHomeButton != null)
            {
                backHomeButton.onClick.AddListener(OnBackHome);
            }
        }

        private void OnEnable()
        {
            if (missionController != null)
            {
                missionController.MissionCompleted += OnMissionCompleted;
            }
        }

        private void OnDisable()
        {
            if (missionController != null)
            {
                missionController.MissionCompleted -= OnMissionCompleted;
            }
        }

        private void OnMissionCompleted(int score, int stars, int earnedCoins, int totalCoins)
        {
            if (resultTitleText != null)
            {
                resultTitleText.text = stars > 0
                    ? GetLocalized(passTitleKey, passTitleDefault)
                    : GetLocalized(retryTitleKey, retryTitleDefault);
            }

            if (resultDetailText != null)
            {
                resultDetailText.text = $"{GetLocalized(scorePrefixKey, scorePrefixDefault)}{score}";
            }

            if (starsText != null)
            {
                starsText.text = $"{GetLocalized(starsPrefixKey, starsPrefixDefault)}{stars}";
            }

            if (coinsText != null)
            {
                coinsText.text =
                    $"{GetLocalized(coinsPrefixKey, coinsPrefixDefault)}+{earnedCoins} " +
                    $"({GetLocalized(totalPrefixKey, totalPrefixDefault)}{totalCoins})";
            }
        }

        private void OnBackHome()
        {
            sceneFlowManager?.LoadHome();
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
