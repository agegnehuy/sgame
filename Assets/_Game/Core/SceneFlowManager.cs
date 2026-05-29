using UnityEngine;
using UnityEngine.SceneManagement;
using SGame.Data;

namespace SGame.Core
{
    public class SceneFlowManager : MonoBehaviour
    {
        [SerializeField] private string homeSceneName = "HomeScene";
        [SerializeField] private string missionSceneName = "P1_M1";

        public void LoadHome()
        {
            SceneManager.LoadScene(homeSceneName);
        }

        public void LoadMission()
        {
            SceneManager.LoadScene(missionSceneName);
        }

        public void LoadSelectedMissionOrDefault()
        {
            if (!string.IsNullOrWhiteSpace(PlayerSession.SelectedMissionSceneName))
            {
                SceneManager.LoadScene(PlayerSession.SelectedMissionSceneName);
                return;
            }

            LoadMission();
        }

        public void LoadMissionByName(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                LoadMission();
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
