using SGame.Data;
using SGame.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class FacilitatorReportExportController : MonoBehaviour
    {
        private enum ExportStatusState
        {
            None,
            Success,
            Failure
        }

        [SerializeField] private Button exportButton;
        [SerializeField] private Text statusText;
        [SerializeField] private string successPrefixDefault = "Report saved:";
        [SerializeField] private string failMessageDefault = "Failed to export report.";
        [Header("Localization keys")]
        [SerializeField] private string successPrefixKey = "ui.export.report_saved";
        [SerializeField] private string failMessageKey = "ui.export.report_failed";

        private ExportStatusState _lastStatusState;
        private string _lastExportPath = string.Empty;

        private void Awake()
        {
            if (exportButton != null)
            {
                exportButton.onClick.AddListener(OnExportPressed);
            }
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += OnLanguageChanged;
            RefreshStatusText();
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnDestroy()
        {
            if (exportButton != null)
            {
                exportButton.onClick.RemoveListener(OnExportPressed);
            }
        }

        private void OnExportPressed()
        {
            try
            {
                _lastExportPath = FacilitatorReportService.ExportAllProfilesReport();
                _lastStatusState = ExportStatusState.Success;
            }
            catch
            {
                _lastStatusState = ExportStatusState.Failure;
            }

            RefreshStatusText();
        }

        private void OnLanguageChanged(string _)
        {
            RefreshStatusText();
        }

        private void RefreshStatusText()
        {
            if (statusText == null)
            {
                return;
            }

            switch (_lastStatusState)
            {
                case ExportStatusState.Success:
                    statusText.text = $"{GetLocalized(successPrefixKey, successPrefixDefault)}\n{_lastExportPath}";
                    break;
                case ExportStatusState.Failure:
                    statusText.text = GetLocalized(failMessageKey, failMessageDefault);
                    break;
                default:
                    break;
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
