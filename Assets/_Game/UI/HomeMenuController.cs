using SGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class HomeMenuController : MonoBehaviour
    {
        [SerializeField] private SceneFlowManager sceneFlowManager;
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlay);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuit);
            }
        }

        private void OnPlay()
        {
            sceneFlowManager?.LoadSelectedMissionOrDefault();
        }

        private void OnQuit()
        {
            sceneFlowManager?.QuitGame();
        }
    }
}
