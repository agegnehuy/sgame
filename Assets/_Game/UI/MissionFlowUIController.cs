using SGame.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class MissionFlowUIController : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;

        [Header("Panels")]
        [SerializeField] private GameObject briefingPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultPanel;

        [Header("Buttons")]
        [SerializeField] private Button startMissionButton;
        [SerializeField] private Button retryButton;

        private void Awake()
        {
            if (startMissionButton != null)
            {
                startMissionButton.onClick.AddListener(StartMission);
            }

            if (retryButton != null)
            {
                retryButton.onClick.AddListener(StartMission);
            }
        }

        private void OnEnable()
        {
            if (missionController != null)
            {
                missionController.StateChanged += OnStateChanged;
            }

            SetPanels(briefingActive: true, gameplayActive: false, resultActive: false);
        }

        private void OnDisable()
        {
            if (missionController != null)
            {
                missionController.StateChanged -= OnStateChanged;
            }
        }

        public void StartMission()
        {
            missionController?.StartMission();
        }

        private void OnStateChanged(MissionRuntimeState state)
        {
            switch (state)
            {
                case MissionRuntimeState.Briefing:
                    SetPanels(briefingActive: true, gameplayActive: false, resultActive: false);
                    break;
                case MissionRuntimeState.Playing:
                case MissionRuntimeState.Evaluating:
                    SetPanels(briefingActive: false, gameplayActive: true, resultActive: false);
                    break;
                case MissionRuntimeState.Result:
                    SetPanels(briefingActive: false, gameplayActive: false, resultActive: true);
                    break;
                default:
                    break;
            }
        }

        private void SetPanels(bool briefingActive, bool gameplayActive, bool resultActive)
        {
            if (briefingPanel != null) briefingPanel.SetActive(briefingActive);
            if (gameplayPanel != null) gameplayPanel.SetActive(gameplayActive);
            if (resultPanel != null) resultPanel.SetActive(resultActive);
        }
    }
}
