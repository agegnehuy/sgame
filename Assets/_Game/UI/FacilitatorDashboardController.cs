using SGame.Localization;
using SGame.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class FacilitatorDashboardController : MonoBehaviour
    {
        [SerializeField] private GameObject dashboardPanel;
        [SerializeField] private Button toggleButton;
        [SerializeField] private Text toggleButtonText;
        [SerializeField] private Text dashboardTitleText;
        [SerializeField] private AnalyticsInsightsPanelController analyticsInsightsPanelController;
        [SerializeField] private bool startOpen;
        [Header("Optional PIN lock")]
        [SerializeField] private bool requirePinToOpen = true;
        [SerializeField] private string facilitatorPin = "2580";
        [SerializeField] private GameObject pinPanel;
        [SerializeField] private InputField pinInputField;
        [SerializeField] private Button pinSubmitButton;
        [SerializeField] private Button pinCancelButton;
        [SerializeField] private Text pinStatusText;
        [SerializeField] private Text pinSecurityStateText;
        [SerializeField] private float lockoutDurationSeconds = 20f;
        [SerializeField] private int maxFailedAttempts = 3;
        [SerializeField] private float failedAttemptCooldownSeconds = 2f;
        [Header("Optional QA security controls")]
        [SerializeField] private bool enableSecurityTestControls;
        [SerializeField] private Button simulateCooldownButton;
        [SerializeField] private Button simulateLockoutButton;
        [SerializeField] private Button clearSecurityBlockButton;
        [SerializeField] private float qaSimulatedCooldownSeconds = 5f;
        [SerializeField] private float qaSimulatedLockoutSeconds = 20f;

        [Header("Localization keys")]
        [SerializeField] private string openLabelKey = "ui.facilitator.open";
        [SerializeField] private string closeLabelKey = "ui.facilitator.close";
        [SerializeField] private string titleKey = "ui.facilitator.title";
        [SerializeField] private string pinPromptKey = "ui.facilitator.pin_prompt";
        [SerializeField] private string pinInvalidKey = "ui.facilitator.pin_invalid";
        [SerializeField] private string pinLockedKey = "ui.facilitator.pin_locked";
        [SerializeField] private string pinAttemptsLeftKey = "ui.facilitator.pin_attempts_left";
        [SerializeField] private string pinCooldownKey = "ui.facilitator.pin_cooldown";
        [SerializeField] private string pinStateReadyKey = "ui.facilitator.pin_state_ready";
        [SerializeField] private string pinStateCooldownKey = "ui.facilitator.pin_state_cooldown";
        [SerializeField] private string pinStateLockoutKey = "ui.facilitator.pin_state_lockout";

        [Header("Fallback labels")]
        [SerializeField] private string openLabelDefault = "Open Facilitator Panel";
        [SerializeField] private string closeLabelDefault = "Close Facilitator Panel";
        [SerializeField] private string titleDefault = "Facilitator Dashboard";
        [SerializeField] private string pinPromptDefault = "Enter facilitator PIN";
        [SerializeField] private string pinInvalidDefault = "Incorrect PIN";
        [SerializeField] private string pinLockedDefault = "Too many attempts. Try again in {0}s";
        [SerializeField] private string pinAttemptsLeftDefault = "Incorrect PIN. Attempts left: {0}";
        [SerializeField] private string pinCooldownDefault = "Please wait {0}s before trying again";
        [SerializeField] private string pinStateReadyDefault = "Security: Ready";
        [SerializeField] private string pinStateCooldownDefault = "Security: Cooldown ({0}s)";
        [SerializeField] private string pinStateLockoutDefault = "Security: Locked ({0}s)";

        private bool _isOpen;
        private int _failedAttempts;
        private float _lockedUntilSeconds;
        private float _attemptCooldownUntilSeconds;
        private bool _wasEntryBlockedLastFrame;
        private int _lastBlockedSecondsShown = -1;

        private void Awake()
        {
            if (toggleButton != null)
            {
                toggleButton.onClick.AddListener(TogglePanel);
            }

            if (pinSubmitButton != null)
            {
                pinSubmitButton.onClick.AddListener(OnPinSubmit);
            }

            if (pinCancelButton != null)
            {
                pinCancelButton.onClick.AddListener(OnPinCancel);
            }

            if (enableSecurityTestControls && simulateCooldownButton != null)
            {
                simulateCooldownButton.onClick.AddListener(OnSimulateCooldown);
            }

            if (enableSecurityTestControls && simulateLockoutButton != null)
            {
                simulateLockoutButton.onClick.AddListener(OnSimulateLockout);
            }

            if (enableSecurityTestControls && clearSecurityBlockButton != null)
            {
                clearSecurityBlockButton.onClick.AddListener(OnClearSecurityBlock);
            }
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += OnLanguageChanged;
            SetPinPanelVisible(false);
            SetOpen(startOpen, refreshInsights: true);
            RefreshLabels();
            _wasEntryBlockedLastFrame = IsEntryBlocked();
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnDestroy()
        {
            if (toggleButton != null)
            {
                toggleButton.onClick.RemoveListener(TogglePanel);
            }

            if (pinSubmitButton != null)
            {
                pinSubmitButton.onClick.RemoveListener(OnPinSubmit);
            }

            if (pinCancelButton != null)
            {
                pinCancelButton.onClick.RemoveListener(OnPinCancel);
            }

            if (simulateCooldownButton != null)
            {
                simulateCooldownButton.onClick.RemoveListener(OnSimulateCooldown);
            }

            if (simulateLockoutButton != null)
            {
                simulateLockoutButton.onClick.RemoveListener(OnSimulateLockout);
            }

            if (clearSecurityBlockButton != null)
            {
                clearSecurityBlockButton.onClick.RemoveListener(OnClearSecurityBlock);
            }
        }

        private void Update()
        {
            var isBlockedNow = IsEntryBlocked();
            if (isBlockedNow)
            {
                RefreshBlockedMessageIfVisible();
            }
            RefreshSecurityStateIfVisible();

            if (_wasEntryBlockedLastFrame && !isBlockedNow)
            {
                RestorePinEntryIfVisible();
            }

            _wasEntryBlockedLastFrame = isBlockedNow;
        }

        public void TogglePanel()
        {
            if (_isOpen)
            {
                SetOpen(false, refreshInsights: false);
                SetPinPanelVisible(false);
                return;
            }

            if (requirePinToOpen)
            {
                if (IsEntryBlocked())
                {
                    ShowBlockedMessage();
                    return;
                }

                ShowPinPrompt();
                return;
            }

            SetOpen(true, refreshInsights: true);
        }

        public void SetOpen(bool open, bool refreshInsights)
        {
            _isOpen = open;

            if (dashboardPanel != null)
            {
                dashboardPanel.SetActive(_isOpen);
            }

            if (_isOpen && refreshInsights)
            {
                analyticsInsightsPanelController?.Refresh();
            }

            RefreshLabels();
        }

        private void OnLanguageChanged(string _)
        {
            RefreshLabels();
            RefreshPinPromptIfVisible();
            RefreshSecurityStateIfVisible();
        }

        private void RefreshLabels()
        {
            if (toggleButtonText != null)
            {
                toggleButtonText.text = _isOpen
                    ? GetLocalized(closeLabelKey, closeLabelDefault)
                    : GetLocalized(openLabelKey, openLabelDefault);
            }

            if (dashboardTitleText != null)
            {
                dashboardTitleText.text = GetLocalized(titleKey, titleDefault);
            }
        }

        private void OnPinSubmit()
        {
            if (IsEntryBlocked())
            {
                ShowBlockedMessage();
                return;
            }

            if (pinInputField == null)
            {
                SetOpen(true, refreshInsights: true);
                SetPinPanelVisible(false);
                return;
            }

            var enteredPin = pinInputField.text != null ? pinInputField.text.Trim() : string.Empty;
            if (enteredPin == facilitatorPin)
            {
                _failedAttempts = 0;
                FacilitatorPinAuditService.LogPinSuccess(PlayerSession.ActiveProfileId);
                ClearPinInput();
                SetPinPanelVisible(false);
                SetOpen(true, refreshInsights: true);
                return;
            }

            _failedAttempts += 1;
            var attemptsLeft = Mathf.Max(0, maxFailedAttempts - _failedAttempts);
            _attemptCooldownUntilSeconds = Time.unscaledTime + Mathf.Max(0f, failedAttemptCooldownSeconds);
            FacilitatorPinAuditService.LogPinFailed(
                PlayerSession.ActiveProfileId,
                _failedAttempts,
                Mathf.Max(1, maxFailedAttempts),
                Mathf.Max(0f, failedAttemptCooldownSeconds));

            if (pinStatusText != null)
            {
                pinStatusText.text = attemptsLeft > 0
                    ? string.Format(GetLocalized(pinAttemptsLeftKey, pinAttemptsLeftDefault), attemptsLeft)
                    : GetLocalized(pinInvalidKey, pinInvalidDefault);
            }

            if (_failedAttempts >= Mathf.Max(1, maxFailedAttempts))
            {
                _lockedUntilSeconds = Time.unscaledTime + Mathf.Max(1f, lockoutDurationSeconds);
                _attemptCooldownUntilSeconds = 0f;
                FacilitatorPinAuditService.LogLockoutStarted(
                    PlayerSession.ActiveProfileId,
                    Mathf.Max(1, maxFailedAttempts),
                    Mathf.Max(1f, lockoutDurationSeconds));
                _failedAttempts = 0;
                ShowBlockedMessage();
                return;
            }

            ShowBlockedMessage();
        }

        private void OnPinCancel()
        {
            ClearPinInput();
            _lastBlockedSecondsShown = -1;
            SetPinPanelVisible(false);
        }

        private void ShowPinPrompt()
        {
            SetPinPanelVisible(true);
            if (pinStatusText != null)
            {
                pinStatusText.text = GetLocalized(pinPromptKey, pinPromptDefault);
            }

            if (pinInputField != null)
            {
                pinInputField.text = string.Empty;
                pinInputField.interactable = true;
                pinInputField.ActivateInputField();
            }

            if (pinSubmitButton != null)
            {
                pinSubmitButton.interactable = true;
            }

            RefreshSecurityStateIfVisible();
        }

        private void RefreshPinPromptIfVisible()
        {
            if (pinPanel == null || !pinPanel.activeSelf || pinStatusText == null)
            {
                return;
            }

            if (IsEntryBlocked())
            {
                ShowBlockedMessage();
                return;
            }

            pinStatusText.text = GetLocalized(pinPromptKey, pinPromptDefault);
            RefreshSecurityStateIfVisible();
        }

        private void SetPinPanelVisible(bool visible)
        {
            if (pinPanel != null)
            {
                pinPanel.SetActive(visible);
            }
        }

        private void ClearPinInput()
        {
            if (pinInputField != null)
            {
                pinInputField.text = string.Empty;
            }
        }

        private void RestorePinEntryIfVisible()
        {
            if (pinPanel == null || !pinPanel.activeSelf)
            {
                return;
            }

            if (pinInputField != null)
            {
                pinInputField.interactable = true;
                pinInputField.ActivateInputField();
            }

            if (pinSubmitButton != null)
            {
                pinSubmitButton.interactable = true;
            }

            if (pinStatusText != null)
            {
                pinStatusText.text = GetLocalized(pinPromptKey, pinPromptDefault);
            }
            RefreshSecurityStateIfVisible();

            _lastBlockedSecondsShown = -1;
        }

        private bool IsPinLocked()
        {
            return Time.unscaledTime < _lockedUntilSeconds;
        }

        private bool IsAttemptCoolingDown()
        {
            return Time.unscaledTime < _attemptCooldownUntilSeconds;
        }

        private bool IsEntryBlocked()
        {
            return IsPinLocked() || IsAttemptCoolingDown();
        }

        private int GetBlockedSecondsLeft()
        {
            if (IsPinLocked())
            {
                return Mathf.Max(1, Mathf.CeilToInt(_lockedUntilSeconds - Time.unscaledTime));
            }

            if (IsAttemptCoolingDown())
            {
                return Mathf.Max(1, Mathf.CeilToInt(_attemptCooldownUntilSeconds - Time.unscaledTime));
            }

            return 0;
        }

        private void ShowBlockedMessage()
        {
            SetPinPanelVisible(true);
            var secondsLeft = GetBlockedSecondsLeft();
            if (IsPinLocked())
            {
                FacilitatorPinAuditService.LogLockoutBlocked(PlayerSession.ActiveProfileId, secondsLeft);
            }
            else if (IsAttemptCoolingDown())
            {
                FacilitatorPinAuditService.LogCooldownBlocked(PlayerSession.ActiveProfileId, secondsLeft);
            }

            if (pinStatusText != null)
            {
                var messageTemplate = IsPinLocked()
                    ? GetLocalized(pinLockedKey, pinLockedDefault)
                    : GetLocalized(pinCooldownKey, pinCooldownDefault);
                pinStatusText.text = string.Format(messageTemplate, secondsLeft);
            }
            _lastBlockedSecondsShown = secondsLeft;

            if (pinInputField != null)
            {
                pinInputField.text = string.Empty;
                pinInputField.interactable = false;
            }

            if (pinSubmitButton != null)
            {
                pinSubmitButton.interactable = false;
            }

            RefreshSecurityStateIfVisible();
        }

        private void RefreshBlockedMessageIfVisible()
        {
            if (pinPanel == null || !pinPanel.activeSelf || pinStatusText == null)
            {
                return;
            }

            var secondsLeft = GetBlockedSecondsLeft();
            if (secondsLeft <= 0 || secondsLeft == _lastBlockedSecondsShown)
            {
                return;
            }

            var messageTemplate = IsPinLocked()
                ? GetLocalized(pinLockedKey, pinLockedDefault)
                : GetLocalized(pinCooldownKey, pinCooldownDefault);
            pinStatusText.text = string.Format(messageTemplate, secondsLeft);
            _lastBlockedSecondsShown = secondsLeft;
            RefreshSecurityStateIfVisible();
        }

        private void RefreshSecurityStateIfVisible()
        {
            if (pinPanel == null || !pinPanel.activeSelf || pinSecurityStateText == null)
            {
                return;
            }

            if (IsPinLocked())
            {
                pinSecurityStateText.text = string.Format(
                    GetLocalized(pinStateLockoutKey, pinStateLockoutDefault),
                    GetBlockedSecondsLeft());
                return;
            }

            if (IsAttemptCoolingDown())
            {
                pinSecurityStateText.text = string.Format(
                    GetLocalized(pinStateCooldownKey, pinStateCooldownDefault),
                    GetBlockedSecondsLeft());
                return;
            }

            pinSecurityStateText.text = GetLocalized(pinStateReadyKey, pinStateReadyDefault);
        }

        private void OnSimulateCooldown()
        {
            _lockedUntilSeconds = 0f;
            _attemptCooldownUntilSeconds = Time.unscaledTime + Mathf.Max(1f, qaSimulatedCooldownSeconds);
            ShowBlockedMessage();
        }

        private void OnSimulateLockout()
        {
            _attemptCooldownUntilSeconds = 0f;
            _lockedUntilSeconds = Time.unscaledTime + Mathf.Max(1f, qaSimulatedLockoutSeconds);
            ShowBlockedMessage();
        }

        private void OnClearSecurityBlock()
        {
            _failedAttempts = 0;
            _attemptCooldownUntilSeconds = 0f;
            _lockedUntilSeconds = 0f;
            RestorePinEntryIfVisible();
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
