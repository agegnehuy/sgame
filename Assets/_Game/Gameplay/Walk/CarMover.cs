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
        private bool _loggedFirstUpdate;
        private float _nextHeartbeatAt;
        private float _lastLoggedX;

        // Cached wheel transforms — spun every frame so even from far away
        // (where the body slide is hard to perceive) the rotating wheels make
        // it obvious the car is moving. Wheel diameter is ~0.45m → circumference
        // 2πr ≈ 1.41m, so wheels rotate 360°/1.41 ≈ 255° per meter traveled.
        private Transform[] _wheels;
        private float _wheelSpinDegPerMeter = 255f;

        // Impact-mode state — when an accident triggers, the car starts from
        // its CURRENT position (so the player visually sees this specific car
        // arriving) and brakes to rest on top of the kid over a short
        // distance-proportional interval. The duration is clamped so it's
        // always long enough to read but never feels sluggish.
        private bool _impactActive;
        private Vector3 _impactStartPos;
        private Vector3 _impactTargetPos;
        private float _impactStartTime;
        private float _impactDurationSeconds;
        private const float ImpactMinSeconds = 0.30f;
        private const float ImpactMaxSeconds = 1.20f;
        // If the car ends up "too far" from the kid when the accident fires
        // (only happens in the fallback path), we still snap it closer so the
        // approach doesn't take an absurd amount of time.
        private const float MaxFallbackApproachDistance = 9f;

        public float LaneZ => laneZ;
        public bool DriveWest => driveWest;
        public bool InImpactMode => _impactActive;
        // How long the scripted impact brake lasts (game-time seconds). Valid
        // only after EnterImpactMode has been called.
        public float ImpactDurationSeconds => _impactDurationSeconds;

        /// <summary>
        /// Releases the car from impact mode and re-syncs its progress to
        /// wherever it currently sits on the lane, so it resumes driving
        /// along the loop instead of staying parked on the kid. Called by
        /// CrosswalkZone after the failure cinematic so the road keeps
        /// flowing visibly even while the fail panel is up.
        /// </summary>
        public void ExitImpactMode()
        {
            if (!_impactActive) return;
            _impactActive = false;
            // Snap to the lane and re-derive _progress from current x so the
            // next Update advances smoothly from where the car ended up.
            var p = transform.position;
            transform.position = new Vector3(p.x, laneY, laneZ);
            _progress = XToProgress(p.x);
            ClearExhaustTrail();
        }

        /// <summary>
        /// Forces this car to dramatically drive into <paramref name="kidWorldPos"/>
        /// over a short interval, then come to rest on top of the kid. Used by
        /// <see cref="CrosswalkZone"/> to visualize an accident — the player
        /// sees the same car that "killed" them stopped right on the zebra.
        /// </summary>
        public void EnterImpactMode(Vector3 kidWorldPos)
        {
            _impactActive = true;
            _impactStartTime = Time.time;

            // Start from the car's CURRENT world position — no teleport. The
            // player has already been watching this specific car drive into
            // the kid; we just take over the last few meters with a scripted
            // brake to make the contact dramatic.
            Vector3 currentPos = transform.position;

            // If the car is implausibly far (fallback path with no nearby
            // traffic) snap it onto the lane line, but not closer than the
            // MaxFallbackApproachDistance threshold so we never literally
            // appear on top of her with no animation.
            float dx = Mathf.Abs(currentPos.x - kidWorldPos.x);
            if (dx > MaxFallbackApproachDistance)
            {
                float approachSign = driveWest ? +1f : -1f;
                currentPos = new Vector3(
                    kidWorldPos.x + approachSign * MaxFallbackApproachDistance,
                    laneY, laneZ);
                dx = MaxFallbackApproachDistance;
            }

            _impactStartPos  = new Vector3(currentPos.x,    laneY, laneZ);
            _impactTargetPos = new Vector3(kidWorldPos.x,   laneY, laneZ);
            // Duration scales with distance — close cars hit fast (short brake),
            // far cars get a longer scripted approach.
            _impactDurationSeconds = Mathf.Clamp(dx / 8f, ImpactMinSeconds, ImpactMaxSeconds);

            transform.position = _impactStartPos;
            transform.rotation = Quaternion.LookRotation(
                driveWest ? Vector3.left : Vector3.right, Vector3.up);
            ClearExhaustTrail();
        }

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

        // Runs strictly before any Update() — guarantees _progress and
        // _loopLength are valid before the very first frame.
        private void Awake()
        {
            // Defensive guards — only fire when fields are truly broken
            // (deserialized as 0 from an older script version). Do NOT
            // overwrite valid values that come from the scene file.
            if (endX <= startX || Mathf.Abs(endX - startX) < 1f)
            {
                startX = -22f; endX = 22f;
            }
            if (speed < 0.5f) speed = 6f;
            if (brakingDistance < 0.5f) brakingDistance = 8f;
            if (Mathf.Abs(eastStopLineX - westStopLineX) < 1f)
            {
                eastStopLineX = -5.5f;
                westStopLineX =  5.5f;
            }
            _loopLength = Mathf.Max(0.01f, endX - startX);

            // _progress is private (not serialized) so it would always
            // deserialize as 0 — which means every car would teleport to the
            // far loop edge on frame 1, all bunched together, then slowly
            // drift back into view. Instead, recover progress from the saved
            // transform.position the editor placed them at so the runtime
            // line-up matches what you see in the scene.
            _progress = XToProgress(transform.position.x);

            // Snap to the lane (Y & Z were also serialized but we re-write them
            // every frame, so this just ensures the very first frame is clean).
            var p = transform.position;
            transform.position = new Vector3(p.x, laneY, laneZ);
        }

        private void Start()
        {
            // Editor-time Configure already wired trafficLight, but if a
            // scene was hand-tweaked we re-discover it so cars never fail
            // their CarsRed() check silently.
            if (trafficLight == null)
            {
                trafficLight = UnityEngine.Object.FindObjectOfType<TrafficLight3D>();
            }

            // Cache child wheel + hubcap transforms so we can spin them in Update().
            // Wheels were built by WalkSceneBuilder.AddWheels as children named
            // "WheelFL", "WheelFR", "WheelRL", "WheelRR"; hubcaps as "Hub_FL" etc.
            // We're tolerant: ANY child whose name starts with "Wheel" or "Hub"
            // qualifies, so future builds with extra wheels still spin correctly.
            var found = new System.Collections.Generic.List<Transform>();
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                if (t == transform) continue;
                if (t.name.StartsWith("Wheel") || t.name.StartsWith("Hub")) found.Add(t);
            }
            _wheels = found.ToArray();

            // Wheel diameter is read off the FIRST wheel's local scale.x (the
            // cylinder is rotated so its "diameter" axis is local Y by default,
            // but in WalkSceneBuilder the wheel is created as a cylinder mesh
            // scaled with x = thickness, y = diameter/2, so localScale.y * 2
            // gives the diameter).
            if (_wheels.Length > 0)
            {
                float diameter = Mathf.Max(0.05f, _wheels[0].localScale.y * 2f);
                float circumference = Mathf.PI * diameter;
                _wheelSpinDegPerMeter = 360f / Mathf.Max(0.01f, circumference);
            }
        }

        private void Update()
        {
            if (!_loggedFirstUpdate)
            {
                _loggedFirstUpdate = true;
                _lastLoggedX = transform.position.x;
                _nextHeartbeatAt = Time.time + 2f;
                bool tlGreen = trafficLight != null && trafficLight.IsGreen;
                Debug.Log($"[CarMover:{name}] First Update: speed={speed}, loopLen={_loopLength}, " +
                          $"progress={_progress:F2}, x={transform.position.x:F2}, driveWest={driveWest}, " +
                          $"trafficLight={(trafficLight == null ? "NULL" : "OK")}, lightGreen={tlGreen}, " +
                          $"timeScale={Time.timeScale}, wheels={(_wheels == null ? 0 : _wheels.Length)}");
            }

            // Accident takeover: the car ignores the loop & the traffic light
            // until it has finished its scripted screech into the kid.
            if (_impactActive)
            {
                float dur = Mathf.Max(0.01f, _impactDurationSeconds);
                float t = Mathf.Clamp01((Time.time - _impactStartTime) / dur);
                // Ease-out cubic so it looks like the brakes are biting at the end.
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                Vector3 prev = transform.position;
                transform.position = Vector3.Lerp(_impactStartPos, _impactTargetPos, eased);
                SpinWheels(Mathf.Abs(transform.position.x - prev.x));
                return;
            }

            // Tentatively advance progress this frame, then see if we'd cross the
            // stop line while cars are red. If so, clamp to the stop line.
            float dt = Time.deltaTime;
            float natural = _progress + speed * dt;
            bool wrappedLoop = false;
            if (natural >= _loopLength) { natural -= _loopLength; wrappedLoop = true; }
            if (wrappedLoop) ClearExhaustTrail();

            float naturalX = ProgressToX(natural);
            float currentX = ProgressToX(_progress);

            float distanceMoved = 0f;

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
                        HeartbeatLog();
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
                    SpinWheels(step);
                    HeartbeatLog();
                    return;
                }
            }

            distanceMoved = speed * dt;
            _progress = natural;
            transform.position = ProgressToWorldPosition(_progress);
            SpinWheels(distanceMoved);
            HeartbeatLog();
        }

        private void SpinWheels(float metersTraveled)
        {
            if (_wheels == null || _wheels.Length == 0 || metersTraveled <= 0f) return;
            // Wheels rotate around their local X axis. East-bound cars (driveWest=false)
            // are facing +X, so the wheels need to spin in one direction; west-bound
            // cars face -X and need the opposite direction. Local X of each wheel
            // points outward from the car's centerline regardless of car heading,
            // so we use a constant sign and trust the local rotation to be along X.
            float deg = metersTraveled * _wheelSpinDegPerMeter;
            // Direction trick: the wheel's local X is roughly +/-1 depending on
            // which side of the car it's on. We use Rotate with the wheel's
            // RIGHT axis so all four wheels spin the same way relative to the
            // direction of travel.
            for (int i = 0; i < _wheels.Length; i++)
            {
                var w = _wheels[i];
                if (w == null) continue;
                w.Rotate(Vector3.right, deg, Space.Self);
            }
        }

        private void HeartbeatLog()
        {
            if (Time.time < _nextHeartbeatAt) return;
            _nextHeartbeatAt = Time.time + 2f;
            float dx = transform.position.x - _lastLoggedX;
            _lastLoggedX = transform.position.x;
            bool tlGreen = trafficLight != null && trafficLight.IsGreen;
            Debug.Log($"[CarMover:{name}] HB  x={transform.position.x:F2}  dx(last 2s)={dx:+0.00;-0.00}  " +
                      $"green={tlGreen}  impact={_impactActive}  timeScale={Time.timeScale}");
        }

        private bool CarsRed()
        {
            // TrafficLight3D is the CARS' light: IsGreen=true → cars go.
            // Cars must stop only when the light is RED.
            // If trafficLight is null (not wired), cars always move (fallback).
            if (trafficLight == null) return false;
            
            // If the light exists but hasn't cycled in 10 seconds, assume it's stuck
            // and force cars to move as a safety fallback.
            if (Time.time > 10f && !trafficLight.IsGreen && _loggedFirstUpdate)
            {
                // Light has been red for too long - force movement
                return false;
            }
            
            return !trafficLight.IsGreen;
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

        // Resets any child TrailRenderer (the exhaust streak) so a loop wrap
        // doesn't draw a 44m streak across the screen as the car teleports
        // back to its starting X.
        private TrailRenderer _cachedTrail;
        private bool _trailLookupDone;
        private void ClearExhaustTrail()
        {
            if (!_trailLookupDone)
            {
                _cachedTrail = GetComponentInChildren<TrailRenderer>();
                _trailLookupDone = true;
            }
            if (_cachedTrail != null) _cachedTrail.Clear();
        }
    }
}
