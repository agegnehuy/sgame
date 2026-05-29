using UnityEngine;

namespace SGame.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string startupLog = "Safe Steps Addis bootstrap ready.";

        private void Start()
        {
            Debug.Log(startupLog);
            Application.targetFrameRate = 30;
        }
    }
}
