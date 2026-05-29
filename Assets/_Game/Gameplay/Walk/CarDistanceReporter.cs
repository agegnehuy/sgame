using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Each frame, finds the closest CarMover to the crosswalk center and reports
    /// its distance to the MissionController. One of these per scene.
    /// </summary>
    public class CarDistanceReporter : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private Transform crosswalkCenter;
        [SerializeField] private CarMover[] cars;

        [Tooltip("Distance reported when no cars are present.")]
        [SerializeField] private float fallbackDistanceMeters = 999f;

        public void Configure(MissionController controller, Transform crosswalk, CarMover[] carRefs)
        {
            missionController = controller;
            crosswalkCenter = crosswalk;
            cars = carRefs;
        }

        private void Update()
        {
            if (missionController == null || crosswalkCenter == null || cars == null || cars.Length == 0)
            {
                return;
            }

            float closest = fallbackDistanceMeters;
            Vector3 cx = crosswalkCenter.position;
            for (int i = 0; i < cars.Length; i++)
            {
                var c = cars[i];
                if (c == null) continue;
                float d = Vector3.Distance(c.transform.position, cx);
                if (d < closest) closest = d;
            }

            missionController.UpdateNearestVehicleDistance(closest);
        }
    }
}
