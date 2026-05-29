using UnityEngine;

namespace SGame.Gameplay
{
    public class VehicleDistanceSimulator : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private float minDistanceMeters = 4f;
        [SerializeField] private float maxDistanceMeters = 30f;
        [SerializeField] private float cycleSpeed = 1.2f;
        [SerializeField] private bool runOnPlay = true;

        private bool _running;
        private float _timeOffset;
        public float MinDistanceMeters => minDistanceMeters;
        public float MaxDistanceMeters => maxDistanceMeters;
        public float CycleSpeed => cycleSpeed;

        private void Start()
        {
            _timeOffset = Random.Range(0f, 10f);
            _running = runOnPlay;
        }

        private void Update()
        {
            if (!_running || missionController == null)
            {
                return;
            }

            var t = (Mathf.Sin((Time.time + _timeOffset) * cycleSpeed) + 1f) * 0.5f;
            var distance = Mathf.Lerp(minDistanceMeters, maxDistanceMeters, t);
            missionController.UpdateNearestVehicleDistance(distance);
        }

        public void SetRunning(bool running)
        {
            _running = running;
        }

        public void Configure(float minDistance, float maxDistance, float speed)
        {
            minDistanceMeters = Mathf.Max(0.5f, minDistance);
            maxDistanceMeters = Mathf.Max(minDistanceMeters + 0.5f, maxDistance);
            cycleSpeed = Mathf.Max(0.1f, speed);
        }
    }
}
