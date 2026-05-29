using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class CoinBalanceLabel : MonoBehaviour
    {
        [SerializeField] private Text coinText;
        [SerializeField] private string prefixDefault = "Coins: ";
        [SerializeField] private string prefixKey = "ui.coins.prefix";

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
            var coins = SaveService.GetCoins(PlayerSession.ActiveProfileId);
            if (coinText != null)
            {
                coinText.text = $"{GetLocalized(prefixKey, prefixDefault)}{coins}";
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
