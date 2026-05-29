using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class LanguageToggleController : MonoBehaviour
    {
        [SerializeField] private LocalizationBootstrap localizationBootstrap;
        [SerializeField] private Button englishButton;
        [SerializeField] private Button amharicButton;
        [SerializeField] private Text currentLanguageText;
        [SerializeField] private string englishLabelDefault = "Language: English";
        [SerializeField] private string amharicLabelDefault = "Language: Amharic";
        [SerializeField] private string englishLabelKey = "ui.language.current.en";
        [SerializeField] private string amharicLabelKey = "ui.language.current.am";

        private void Awake()
        {
            if (englishButton != null)
            {
                englishButton.onClick.AddListener(SetEnglish);
            }

            if (amharicButton != null)
            {
                amharicButton.onClick.AddListener(SetAmharic);
            }
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += OnLanguageChanged;
            RefreshLanguageIndicator();
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnDestroy()
        {
            if (englishButton != null)
            {
                englishButton.onClick.RemoveListener(SetEnglish);
            }

            if (amharicButton != null)
            {
                amharicButton.onClick.RemoveListener(SetAmharic);
            }
        }

        public void SetEnglish()
        {
            var bootstrap = ResolveBootstrap();
            if (bootstrap != null)
            {
                bootstrap.LoadEnglish();
            }
        }

        public void SetAmharic()
        {
            var bootstrap = ResolveBootstrap();
            if (bootstrap != null)
            {
                bootstrap.LoadAmharic();
            }
        }

        private void OnLanguageChanged(string _)
        {
            RefreshLanguageIndicator();
        }

        private void RefreshLanguageIndicator()
        {
            if (currentLanguageText == null)
            {
                return;
            }

            var isAmharic = LocalizationService.CurrentLanguage == "am";
            currentLanguageText.text = isAmharic
                ? GetLocalized(amharicLabelKey, amharicLabelDefault)
                : GetLocalized(englishLabelKey, englishLabelDefault);
        }

        private LocalizationBootstrap ResolveBootstrap()
        {
            return localizationBootstrap != null ? localizationBootstrap : LocalizationBootstrap.Instance;
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
