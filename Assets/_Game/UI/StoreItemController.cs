using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class StoreItemController : MonoBehaviour
    {
        [SerializeField] private string itemId = "shirt_blue_01";
        [SerializeField] private string slot = "clothes";
        [SerializeField] private int cost = 25;

        [Header("UI refs")]
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text costText;
        [SerializeField] private Text statusText;
        [SerializeField] private Button actionButton;
        [SerializeField] private CoinBalanceLabel coinBalanceLabel;
        [Header("Localization keys")]
        [SerializeField] private string ownedKey = "ui.store.owned";
        [SerializeField] private string costPrefixKey = "ui.store.cost_prefix";
        [SerializeField] private string tapToEquipKey = "ui.store.tap_to_equip";
        [SerializeField] private string tapToBuyKey = "ui.store.tap_to_buy";
        [SerializeField] private string notEnoughCoinsKey = "ui.store.not_enough";
        [SerializeField] private string purchasedLeftKey = "ui.store.purchased_left";
        [SerializeField] private string equippedKey = "ui.store.equipped";
        [SerializeField] private string ownedDefault = "Owned";
        [SerializeField] private string costPrefixDefault = "Cost: ";
        [SerializeField] private string tapToEquipDefault = "Tap to Equip";
        [SerializeField] private string tapToBuyDefault = "Tap to Buy";
        [SerializeField] private string notEnoughCoinsDefault = "Not enough coins";
        [SerializeField] private string purchasedLeftDefault = "Purchased ({0} left)";
        [SerializeField] private string equippedDefault = "Equipped";

        private string _profileId;

        private void Awake()
        {
            if (actionButton != null)
            {
                actionButton.onClick.AddListener(OnActionButtonPressed);
            }
        }

        private void OnEnable()
        {
            _profileId = PlayerSession.ActiveProfileId;
            LocalizationService.LanguageChanged += OnLanguageChanged;
            Refresh();
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        public void Refresh()
        {
            _profileId = PlayerSession.ActiveProfileId;
            var ownsItem = SaveService.OwnsItem(_profileId, itemId);

            if (itemNameText != null && string.IsNullOrWhiteSpace(itemNameText.text))
            {
                itemNameText.text = itemId;
            }

            if (costText != null)
            {
                costText.text = ownsItem
                    ? GetLocalized(ownedKey, ownedDefault)
                    : $"{GetLocalized(costPrefixKey, costPrefixDefault)}{cost}";
            }

            if (statusText != null)
            {
                statusText.text = ownsItem
                    ? GetLocalized(tapToEquipKey, tapToEquipDefault)
                    : GetLocalized(tapToBuyKey, tapToBuyDefault);
            }
        }

        private void OnActionButtonPressed()
        {
            var ownsItem = SaveService.OwnsItem(_profileId, itemId);
            if (!ownsItem)
            {
                var purchased = SaveService.PurchaseItem(_profileId, itemId, cost, out var remainingCoins);
                if (!purchased)
                {
                    if (statusText != null) statusText.text = GetLocalized(notEnoughCoinsKey, notEnoughCoinsDefault);
                    if (costText != null) costText.text = $"{GetLocalized(costPrefixKey, costPrefixDefault)}{cost}";
                    coinBalanceLabel?.Refresh();
                    return;
                }

                if (statusText != null)
                {
                    var template = GetLocalized(purchasedLeftKey, purchasedLeftDefault);
                    statusText.text = string.Format(template, remainingCoins);
                }
            }

            var equipped = SaveService.EquipItem(_profileId, itemId, slot);
            if (equipped && statusText != null)
            {
                statusText.text = GetLocalized(equippedKey, equippedDefault);
            }

            Refresh();
            coinBalanceLabel?.Refresh();
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
