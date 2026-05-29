using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private bool refreshOnEnable = true;

        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += OnLanguageChanged;
            if (refreshOnEnable)
            {
                Refresh();
            }
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        public void Refresh()
        {
            if (_text == null)
            {
                _text = GetComponent<Text>();
            }

            _text.text = LocalizationService.Get(key);
        }

        private void OnLanguageChanged(string _)
        {
            Refresh();
        }
    }
}
