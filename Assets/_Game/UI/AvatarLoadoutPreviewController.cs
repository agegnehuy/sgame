using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class AvatarLoadoutPreviewController : MonoBehaviour
    {
        [SerializeField] private Text profileNameText;
        [SerializeField] private Text avatarIdText;
        [SerializeField] private Text clothesText;
        [SerializeField] private Text shoesText;
        [SerializeField] private Text accessoryText;
        [SerializeField] private string noneLabelDefault = "None";
        [SerializeField] private string noneLabelKey = "ui.common.none";

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
            var profile = SaveService.GetProfile(profileId);

            if (profileNameText != null)
            {
                profileNameText.text = profile != null ? profile.displayName : profileId;
            }

            if (avatarIdText != null)
            {
                avatarIdText.text = profile != null ? profile.avatarId : "boy_01";
            }

            if (clothesText != null)
            {
                clothesText.text = AsLabel(SaveService.GetEquippedItemId(profileId, "clothes"));
            }

            if (shoesText != null)
            {
                shoesText.text = AsLabel(SaveService.GetEquippedItemId(profileId, "shoes"));
            }

            if (accessoryText != null)
            {
                accessoryText.text = AsLabel(SaveService.GetEquippedItemId(profileId, "accessory"));
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

        private string AsLabel(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? GetLocalized(noneLabelKey, noneLabelDefault)
                : value;
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
