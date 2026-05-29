using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Auto-starts the mission when Play begins so the builder-generated scene
    /// is runnable without needing to wire a Start button.
    /// </summary>
    public class WalkMissionBootstrapper : MonoBehaviour
    {
        [SerializeField] private MissionController mission;

        public void Bind(MissionController controller)
        {
            mission = controller;
        }

        private void Start()
        {
            if (mission != null)
            {
                mission.StartMission();
            }
        }
    }
}
