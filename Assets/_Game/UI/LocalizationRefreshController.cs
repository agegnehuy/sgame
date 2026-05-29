using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class LocalizationRefreshController : MonoBehaviour
    {
        [SerializeField] private Button refreshButton;
        [SerializeField] private Text statusText;
        [SerializeField] private string doneDefault = "Localized UI refreshed";
        [SerializeField] private string doneKey = "ui.debug.localization_refreshed";

        private void Awake()
        {
            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(RefreshAll);
            }
        }

        private void OnDestroy()
        {
            if (refreshButton != null)
            {
                refreshButton.onClick.RemoveListener(RefreshAll);
            }
        }

        public void RefreshAll()
        {
            var localizedTexts = FindObjectsOfType<LocalizedText>(true);
            foreach (var localizedText in localizedTexts)
            {
                localizedText.Refresh();
            }

            var progressControllers = FindObjectsOfType<MissionProgressSummaryController>(true);
            foreach (var controller in progressControllers)
            {
                controller.Refresh();
            }

            var coinLabels = FindObjectsOfType<CoinBalanceLabel>(true);
            foreach (var label in coinLabels)
            {
                label.Refresh();
            }

            var storeItems = FindObjectsOfType<StoreItemController>(true);
            foreach (var storeItem in storeItems)
            {
                storeItem.Refresh();
            }

            var loadoutPreviews = FindObjectsOfType<AvatarLoadoutPreviewController>(true);
            foreach (var preview in loadoutPreviews)
            {
                preview.Refresh();
            }

            if (statusText != null)
            {
                statusText.text = GetLocalized(doneKey, doneDefault);
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
