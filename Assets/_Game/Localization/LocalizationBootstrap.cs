using UnityEngine;

namespace SGame.Localization
{
    public class LocalizationBootstrap : MonoBehaviour
    {
        public static LocalizationBootstrap Instance { get; private set; }

        [SerializeField] private TextAsset englishFile;
        [SerializeField] private TextAsset amharicFile;
        [SerializeField] private string defaultLanguage = "en";

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Awake()
        {
            LoadLanguage(defaultLanguage);
        }

        public void LoadLanguage(string languageCode)
        {
            var code = (languageCode ?? "en").ToLowerInvariant();
            var target = code == "am" ? amharicFile : englishFile;
            LocalizationService.LoadFromJson(target);
        }

        public void LoadEnglish()
        {
            LoadLanguage("en");
        }

        public void LoadAmharic()
        {
            LoadLanguage("am");
        }
    }
}
