using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Continuously drives a vehicle along the X axis in one direction, looping
    /// off-screen, and respects the cars' traffic light: when the light is RED
    /// (TrafficLight3D.IsGreen == false) the car decelerates to a stop line
    /// just before the crosswalk. When the light is GREEN (cars go) the car
    /// accelerates again. Each car owns its own progress so stops persist.
    /// </summary>
    public class CarMover : MonoBehaviour
    {
        [Header("Loop range (world X)")]
        [SerializeField] private float startX = -32f;
        [SerializeField] private float endX = 32f;

        [Header("Lane")]
        [SerializeField] private float laneZ = -1.8f;
        [SerializeField] private float laneY = 0.02f;

        [Header("Motion")]
        [SerializeField] private bool driveWest;
        [SerializeField] private float speed = 6f;
        [SerializeField] private float phaseOffsetSeconds;

        [Header("Traffic light")]
        [SerializeField] private TrafficLight3D trafficLight;
        [Tooltip("World X at which an eastbound car must stop (the west edge of the crosswalk).")]
        [SerializeField] private float eastStopLineX = -5.5f;
        [Tooltip("World X at which a westbound car must stop (the east edge of the crosswalk).")]
        [SerializeField] private float westStopLineX = 5.5f;
        [Tooltip("How early before the stop line the car starts to brake.")]
        [SerializeField] private float brakingDistance = 8f;

        // Internal state — distance traveled along the loop since spawn.
        private float _progress;
        private float _loopLength;

        public void Configure(float xStart, float xEnd, float z, float y, bool west,
            float metersPerSecond, float phaseSeconds, TrafficLight3D light = null)
        {
            startX = xStart;
            endX = xEnd;
            laneZ = z;
            laneY = y;
            driveWest = west;
            speed = metersPerSecond;
            phaseOffsetSeconds = phaseSeconds;
            trafficLight = light;
            _loopLength = Mathf.Max(0.01f, endX - startX);

            transform.rotation = Quaternion.LookRotation(
                west ? Vector3.left : Vector3.right, Vector3.up);

            _progress = Mathf.Repeat(phaseSeconds * speed, _loopLength);
            transform.position = ProgressToWorldPosition(_progress);
        }

        private void Start()
        {
            if (_loopLength <= 0f) _loopLength = Mathf.Max(0.01f, endX - startX);
        }

        private void Update()
        {
            // Tentatively advance progress this frame, then see if we'd cross the
            // stop line while cars are red. If so, clamp to the stop line.
            float dt = Time.deltaTime;
            float natural = _progress + speed * dt;
            if (natural >= _loopLength) natural -= _loopLength;

            float naturalX = ProgressToX(natural);
            float currentX = ProgressToX(_progress);

            if (CarsRed())
            {
                float stopX = driveWest ? westStopLineX : eastStopLineX;

                // Are we in the braking zone approaching the stop line?
                bool approaching = driveWest
                    ? (currentX >= stopX && currentX <= stopX + brakingDistance)
                    : (currentX <= stopX && currentX >= stopX - brakingDistance);

                bool aboutToCrossStopLine = driveWest
                    ? (currentX > stopX && naturalX <= stopX)
                    : (currentX < stopX && naturalX >= stopX);

                if (approaching || aboutToCrossStopLine)
                {
                    // Decelerate smoothly: pick the target position as the stop
                    // line, but interpolate from currentX so it eases in.
                    float remaining = driveWest ? (currentX - stopX) : (stopX - currentX);
                    if (remaining <= 0.02f)
                    {
                        // At the line — freeze.
                        _progress = XToProgress(stopX);
                        transform.position = ProgressToWorldPosition(_progress);
                        return;
                    }

                    // Speed scales linearly with remaining distance, min 0.4 m/s
                    // so the car doesn't crawl forever — it will then snap to the
                    // stop line on the next frame.
                    float brakingSpeed = Mathf.Max(0.4f, speed * (remaining / brakingDistance));
                    float step = brakingSpeed * dt;
                    if (step > remaining) step = remaining;

                    _progress += step;
                    if (_progress >= _loopLength) _progress -= _loopLength;
                    transform.position = ProgressToWorldPosition(_progress);
                    return;
                }
            }

            _progress = natural;
            transform.position = ProgressToWorldPosition(_progress);
        }

        private bool CarsRed()
        {
            // TrafficLight3D is the CARS' light: IsGreen=true → cars go.
            // Cars must stop only when the light is RED.
            return trafficLight != null && !trafficLight.IsGreen;
        }

        private float ProgressToX(float progress)
        {
            float p = Mathf.Repeat(progress, _loopLength);
            return driveWest ? (endX - p) : (startX + p);
        }

        private float XToProgress(float x)
        {
            float p = driveWest ? (endX - x) : (x - startX);
            return Mathf.Repeat(p, _loopLength);
        }

        private Vector3 ProgressToWorldPosition(float progress)
        {
            return new Vector3(ProgressToX(progress), laneY, laneZ);
        }
    }
}
