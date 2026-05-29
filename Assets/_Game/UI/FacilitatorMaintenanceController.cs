using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class FacilitatorMaintenanceController : MonoBehaviour
    {
        [SerializeField] private Button clearAnalyticsButton;
        [SerializeField] private Button clearExportsButton;
        [SerializeField] private Button clearPinAuditButton;
        [SerializeField] private Button resetProfileProgressButton;
        [SerializeField] private Text statusText;
        [SerializeField] private Image statusBackground;
        [SerializeField] private GameObject dangerZoneIndicator;
        [SerializeField] private float resetArmDurationSeconds = 5f;

        [Header("Status colors")]
        [SerializeField] private Color neutralTextColor = Color.white;
        [SerializeField] private Color warningTextColor = new Color(1f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color successTextColor = new Color(0.5f, 1f, 0.5f, 1f);
        [SerializeField] private Color neutralBackgroundColor = new Color(0f, 0f, 0f, 0.3f);
        [SerializeField] private Color warningBackgroundColor = new Color(0.5f, 0.2f, 0.1f, 0.45f);
        [SerializeField] private Color successBackgroundColor = new Color(0.1f, 0.45f, 0.2f, 0.45f);

        [Header("Localization keys")]
        [SerializeField] private string clearedAnalyticsKey = "ui.maintenance.cleared_analytics";
        [SerializeField] private string clearedExportsKey = "ui.maintenance.cleared_exports";
        [SerializeField] private string clearedPinAuditKey = "ui.maintenance.cleared_pin_audit";
        [SerializeField] private string resetProfileDoneKey = "ui.maintenance.reset_done";
        [SerializeField] private string resetConfirmKey = "ui.maintenance.reset_confirm";
        [SerializeField] private string resetTimeoutKey = "ui.maintenance.reset_timeout";

        [Header("Fallback labels")]
        [SerializeField] private string clearedAnalyticsDefault = "Analytics logs cleared: {0}";
        [SerializeField] private string clearedExportsDefault = "Export files cleared: {0}";
        [SerializeField] private string clearedPinAuditDefault = "PIN audit logs cleared: {0}";
        [SerializeField] private string resetProfileDoneDefault = "Active profile progress reset.";
        [SerializeField] private string resetConfirmDefault = "Press reset again to confirm.";
        [SerializeField] private string resetTimeoutDefault = "Reset confirmation expired.";

        private bool _resetArmed;
        private float _resetArmedAtSeconds;

        private void Awake()
        {
            if (clearAnalyticsButton != null)
            {
                clearAnalyticsButton.onClick.AddListener(OnClearAnalytics);
            }

            if (clearExportsButton != null)
            {
                clearExportsButton.onClick.AddListener(OnClearExports);
            }

            if (clearPinAuditButton != null)
            {
                clearPinAuditButton.onClick.AddListener(OnClearPinAudit);
            }

            if (resetProfileProgressButton != null)
            {
                resetProfileProgressButton.onClick.AddListener(OnResetProfileProgress);
            }
        }

        private void OnDestroy()
        {
            if (clearAnalyticsButton != null)
            {
                clearAnalyticsButton.onClick.RemoveListener(OnClearAnalytics);
            }

            if (clearExportsButton != null)
            {
                clearExportsButton.onClick.RemoveListener(OnClearExports);
            }

            if (clearPinAuditButton != null)
            {
                clearPinAuditButton.onClick.RemoveListener(OnClearPinAudit);
            }

            if (resetProfileProgressButton != null)
            {
                resetProfileProgressButton.onClick.RemoveListener(OnResetProfileProgress);
            }
        }

        private void Update()
        {
            if (!_resetArmed)
            {
                return;
            }

            var age = Time.unscaledTime - _resetArmedAtSeconds;
            if (age < Mathf.Max(1f, resetArmDurationSeconds))
            {
                return;
            }

            DisarmReset();
            SetStatus(GetLocalized(resetTimeoutKey, resetTimeoutDefault), neutralTextColor, neutralBackgroundColor);
        }

        private void OnClearAnalytics()
        {
            DisarmReset();
            var count = FacilitatorMaintenanceService.ClearAnalyticsLogs();
            SetStatus(
                string.Format(GetLocalized(clearedAnalyticsKey, clearedAnalyticsDefault), count),
                successTextColor,
                successBackgroundColor);
        }

        private void OnClearExports()
        {
            DisarmReset();
            var count = FacilitatorMaintenanceService.ClearExportFiles();
            SetStatus(
                string.Format(GetLocalized(clearedExportsKey, clearedExportsDefault), count),
                successTextColor,
                successBackgroundColor);
        }

        private void OnClearPinAudit()
        {
            DisarmReset();
            var count = FacilitatorMaintenanceService.ClearPinAuditLogs();
            SetStatus(
                string.Format(GetLocalized(clearedPinAuditKey, clearedPinAuditDefault), count),
                successTextColor,
                successBackgroundColor);
        }

        private void OnResetProfileProgress()
        {
            if (!_resetArmed)
            {
                _resetArmed = true;
                _resetArmedAtSeconds = Time.unscaledTime;
                SetDangerVisuals(true);
                SetStatus(GetLocalized(resetConfirmKey, resetConfirmDefault), warningTextColor, warningBackgroundColor);
                return;
            }

            DisarmReset();
            FacilitatorMaintenanceService.ResetActiveProfileProgress();
            SetStatus(GetLocalized(resetProfileDoneKey, resetProfileDoneDefault), successTextColor, successBackgroundColor);
        }

        private void SetStatus(string text, Color textColor, Color backgroundColor)
        {
            if (statusText != null)
            {
                statusText.text = text;
                statusText.color = textColor;
            }

            if (statusBackground != null)
            {
                statusBackground.color = backgroundColor;
            }
        }

        private void DisarmReset()
        {
            _resetArmed = false;
            SetDangerVisuals(false);
        }

        private void SetDangerVisuals(bool enabled)
        {
            if (dangerZoneIndicator != null)
            {
                dangerZoneIndicator.SetActive(enabled);
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
