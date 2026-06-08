using System;
using System.Collections;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Watches the kid's presence on the zebra and continuously evaluates
    /// crossing safety against the CARS' traffic light + actual oncoming cars.
    ///
    /// Failure conditions while the kid is on the road and the cars' light is
    /// GREEN (cars going):
    ///   1. ANY car physically overlaps the kid's position (close in X, in the
    ///      kid's current lane in Z) — that specific car becomes the impact car
    ///      so the player sees the actual car that hit her.
    ///   2. If no car reaches her after a few seconds (e.g., she's standing
    ///      still mid-road with no traffic nearby), a fallback accident still
    ///      fires using the closest oncoming car — so she can't simply chill
    ///      on the zebra during a green light.
    ///
    /// If the kid reaches the far-side trigger before either fires, forward
    /// to MissionController.OnCrossAttempt for scoring and lock in a safe cross.
    ///
    /// The STOP / GO buttons on the HUD pause / resume the kid's auto-walk
    /// at any time — they no longer gate progress at the curb.
    /// </summary>
    public class CrosswalkZone : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private TrafficLight3D trafficLight;
        [SerializeField] private KidPlayer kidPlayer;

        private bool _kidOnRoad;
        private bool _safeCrossed;
        private bool _isAccident;
        private bool _atCurb;

        // Tracks how long the kid has been on the road during a green light
        // without being physically reached by a car. Once it exceeds
        // FallbackAccidentSeconds we still trigger an accident (otherwise she
        // could chill on the road if no car happens to be near).
        private float _greenOnRoadSeconds;

        // Cache of cars in the scene, refreshed occasionally so we don't
        // FindObjectsByType every frame.
        private CarMover[] _cars;
        private float _carCacheRefreshAt;

        // ── Tuning ────────────────────────────────────────────────────────
        // Half-length the car occupies along X, plus a small kid-radius
        // margin. If a car's X is within this range of the kid's X, they
        // overlap longitudinally.
        private const float CarHalfLengthX = 2.5f;
        // Half-width the car occupies along Z (lane width), plus a kid-radius
        // margin. If the kid's Z is within this range of the car's lane Z,
        // she's in the car's path.
        private const float CarHalfWidthZ = 1.8f;
        // How often (real seconds) to re-scan for car objects in the scene.
        private const float CarCacheRefreshSeconds = 2.0f;
        // If the kid stands on a green-lit road for this long without any car
        // physically reaching her, force the fallback teleport-accident so
        // she can't game the system by waiting in the middle of the road.
        private const float FallbackAccidentSeconds = 6.0f;

        public event Action MissionFailed;
        public event Action SafelyCrossed;

        public bool AtCurb => _atCurb;
        public bool CrossResolved => _safeCrossed || _isAccident;
        public bool FarSideReached => _safeCrossed;
        public TrafficLight3D Light => trafficLight;
        public KidPlayer Kid => kidPlayer;

        public void Configure(MissionController controller, TrafficLight3D light, KidPlayer kid)
        {
            missionController = controller;
            trafficLight = light;
            kidPlayer = kid;
        }

        private void Update()
        {
            if (_isAccident || _safeCrossed || !_kidOnRoad) { _greenOnRoadSeconds = 0f; return; }
            if (trafficLight == null || !trafficLight.IsGreen) { _greenOnRoadSeconds = 0f; return; }
            if (kidPlayer == null) return;

            // Refresh car cache occasionally.
            if (_cars == null || Time.time >= _carCacheRefreshAt)
            {
                _cars = UnityEngine.Object.FindObjectsByType<CarMover>(FindObjectsSortMode.None);
                _carCacheRefreshAt = Time.time + CarCacheRefreshSeconds;
            }

            // PRIMARY check: is any oncoming car physically on top of the kid?
            Vector3 kidPos = kidPlayer.transform.position;
            CarMover hitter = FindCarOverlappingKid(kidPos);
            if (hitter != null)
            {
                TriggerAccidentWith(hitter);
                return;
            }

            // FALLBACK: she's been on a green-lit road too long without any
            // car reaching her. Force an accident with the closest oncoming car.
            _greenOnRoadSeconds += Time.deltaTime;
            if (_greenOnRoadSeconds >= FallbackAccidentSeconds)
            {
                TriggerAccidentWith(FindClosestApproachingCar(kidPos));
            }
        }

        // True when a car physically overlaps the kid (close in X AND in her
        // current lane in Z). Used to trigger an accident with the SPECIFIC
        // car the player sees bearing down on her.
        private CarMover FindCarOverlappingKid(Vector3 kidPos)
        {
            if (_cars == null) return null;
            CarMover best = null;
            float bestDist = float.MaxValue;
            foreach (var c in _cars)
            {
                if (c == null || c.InImpactMode) continue;
                float dx = Mathf.Abs(c.transform.position.x - kidPos.x);
                if (dx > CarHalfLengthX) continue;
                float dz = Mathf.Abs(c.LaneZ - kidPos.z);
                if (dz > CarHalfWidthZ) continue;
                // Within the overlap box — pick the one whose front nose is
                // closest to the kid (most "this is the one hitting her").
                if (dx < bestDist) { bestDist = dx; best = c; }
            }
            return best;
        }

        // Used by the fallback. Returns the closest car currently APPROACHING
        // the kid (kid is in front of the car along its travel direction).
        private CarMover FindClosestApproachingCar(Vector3 kidPos)
        {
            if (_cars == null) return null;
            CarMover best = null;
            float bestDist = float.MaxValue;
            foreach (var c in _cars)
            {
                if (c == null || c.InImpactMode) continue;
                float signedDx = kidPos.x - c.transform.position.x;
                bool approaching = c.DriveWest ? (signedDx < 0f) : (signedDx > 0f);
                if (!approaching) continue;
                float dist = Mathf.Abs(signedDx);
                if (dist < bestDist) { bestDist = dist; best = c; }
            }
            return best;
        }

        // On the return leg the kid approaches the zebra from the OPPOSITE
        // side (school → home). The physical trigger volumes don't change,
        // but the SEMANTICS do: what used to be "FarSide" is now the
        // entry-side curb, and what used to be "Curb" is now the finish.
        // _reverseFlow flips the mapping between trigger and behaviour.
        private bool _reverseFlow;

        public void NotifyCurbEnter()
        {
            if (_reverseFlow) HandleFinishEnter();
            else HandleStartCurbEnter();
        }

        public void NotifyCurbExit()
        {
            if (_reverseFlow) { /* finish trigger has no exit logic */ return; }
            HandleStartCurbExit();
        }

        public void NotifyRoadEnter()
        {
            if (_safeCrossed || _isAccident) return;

            _kidOnRoad = true;
            _greenOnRoadSeconds = 0f;
            if (missionController != null) missionController.SetCrosswalkUsage(true);
            // No instant accident here — Update() will catch her if any car
            // physically reaches her while she's on a green-lit road.
        }

        public void NotifyRoadExit()
        {
            _kidOnRoad = false;
        }

        public void NotifyFarSideEnter()
        {
            if (_reverseFlow) HandleStartCurbEnter();
            else HandleFinishEnter();
        }

        public void NotifyFarSideExit()
        {
            if (_reverseFlow) HandleStartCurbExit();
            // On the forward leg the kid never re-enters the FarSide trigger
            // after exiting, so we don't need to clear anything here.
        }

        private void HandleStartCurbEnter()
        {
            _atCurb = true;
            if (missionController != null) missionController.SetCrosswalkUsage(true);
        }

        private void HandleStartCurbExit()
        {
            _atCurb = false;
            if (missionController != null) missionController.SetCrosswalkUsage(false);
        }

        private void HandleFinishEnter()
        {
            if (_isAccident || _safeCrossed) return;

            // Reached the far side without being hit — safe cross.
            _safeCrossed = true;
            _kidOnRoad = false;
            if (missionController != null) missionController.OnCrossAttempt();
            SafelyCrossed?.Invoke();
        }

        public void RequestCross()
        {
            if (kidPlayer != null) kidPlayer.SetWalkingAllowed(true);
        }

        /// <summary>
        /// Resets the zone state so the player can cross the road a second time
        /// (used for the return-home leg of the level). Clears the safely-crossed
        /// and accident flags so future crossings are re-evaluated cleanly.
        /// </summary>
        public void ResetForReturnTrip()
        {
            _kidOnRoad = false;
            _safeCrossed = false;
            _isAccident = false;
            _atCurb = false;
            _greenOnRoadSeconds = 0f;
            // From now on, what used to be "FarSide" is the START curb and
            // what used to be "Curb" is the FINISH line.
            _reverseFlow = true;
        }

        public void RequestWait()
        {
            if (kidPlayer != null) kidPlayer.SetWalkingAllowed(false);
            if (_atCurb && !CrossResolved && missionController != null)
            {
                missionController.OnWaitAction();
            }
        }

        // Cinematic timing.
        //  - Impact delay uses SCALED time so it matches CarMover's impact lerp
        //    (which also uses Time.time). The actual duration is taken from
        //    the car (CarMover.ImpactDurationSeconds) so close-range hits feel
        //    snappy while fallback teleport-ins still have a visible approach.
        //  - The post-impact hold uses REAL time so the slow-mo doesn't make
        //    the player wait too long for the failure overlay.
        private const float CinematicSlowMoScale  = 0.35f;
        private const float DefaultImpactArrival  = 1.10f;  // used when no car is available
        private const float PostImpactRealSec     = 1.60f;  // hold after the hit
        private const float CinematicCameraSlack  = 0.80f;  // extra real seconds the camera stays pinned

        private void TriggerAccidentWith(CarMover hitCar)
        {
            if (_isAccident) return;
            _isAccident = true;
            StartCoroutine(AccidentCinematic(hitCar));
        }

        private IEnumerator AccidentCinematic(CarMover hitCar)
        {
            Vector3 kidPos = kidPlayer != null
                ? kidPlayer.transform.position
                : transform.position;

            // ── Trigger the car's impact sequence ──────────────────────────
            Vector3 carDir = Vector3.right;
            float laneZ = kidPos.z;
            float impactArrivalGameSec = DefaultImpactArrival;
            if (hitCar != null)
            {
                hitCar.EnterImpactMode(kidPos);
                carDir = hitCar.DriveWest ? Vector3.left : Vector3.right;
                laneZ = hitCar.LaneZ;
                impactArrivalGameSec = hitCar.ImpactDurationSeconds;
            }

            // ── Tire screech right as the car commits to the impact ────────
            // Parent to the car so the screech sweeps with it instead of
            // sitting 11m away where the car teleported in.
            if (hitCar != null)
            {
                AccidentEffects.PlayScreech(hitCar.transform, volume: 0.95f);
            }

            // ── Frame the impact from the side, with a slow dolly-in ───────
            // Camera sits on the OPPOSITE Z side from the lane the car is in
            // (so we don't see through the car), starts wide, ends close on
            // the impact point.
            var cam = Camera.main;
            FollowCamera follow = cam != null ? cam.GetComponent<FollowCamera>() : null;
            // Real seconds for the whole cinematic = full-speed approach +
            // slow-mo post-impact hold + a bit of slack.
            float cinematicRealSec =
                impactArrivalGameSec + PostImpactRealSec + CinematicCameraSlack;

            if (follow != null)
            {
                float sideZSign = -Mathf.Sign(laneZ + 0.001f);
                Vector3 camStart = new Vector3(
                    kidPos.x - carDir.x * 1.8f,
                    3.4f,
                    laneZ + sideZSign * 9.0f);
                Vector3 camEnd = new Vector3(
                    kidPos.x - carDir.x * 0.4f,
                    2.1f,
                    laneZ + sideZSign * 6.0f);
                Vector3 lookAt = new Vector3(kidPos.x, 1.05f, kidPos.z);
                follow.SetCinematic(camStart, camEnd, lookAt, cinematicRealSec);
            }

            // ── Let the car drive in at FULL speed so the player sees the
            //     specific car bear down on her in real-time. We slow time
            //     only AT the moment of impact for dramatic emphasis.
            float savedTimeScale = Time.timeScale;
            yield return new WaitForSeconds(impactArrivalGameSec);

            // IMPACT MOMENT — snap to slow-mo and fire everything.
            Time.timeScale = CinematicSlowMoScale;
            Vector3 knock = carDir * 3.4f;
            if (kidPlayer != null) kidPlayer.TriggerAccident(knock);
            if (follow != null) follow.Shake(amplitude: 0.85f, duration: 0.55f);
            AccidentEffects.PlayThud(kidPos, volume: 1f);
            StartCoroutine(AccidentEffects.RedFlash(this, duration: 0.55f, peakAlpha: 0.7f));
            AccidentEffects.SpawnDust(kidPos);
            if (hitCar != null) AccidentEffects.PaintSkidMarks(hitCar);

            // Hold the slow-mo aftermath so the player sees the body knocked
            // and the car parked on top. REAL seconds so we don't wait 5+
            // wall-clock seconds at 0.35× timescale.
            yield return new WaitForSecondsRealtime(PostImpactRealSec);

            // ── Smooth ramp back to normal speed ───────────────────────────
            yield return RampTimeScale(Time.timeScale, savedTimeScale, 0.15f);
            Time.timeScale = savedTimeScale;

            // Release the hit car so it doesn't sit parked on the kid forever
            // — without this the most visible car on screen looks frozen and
            // the player concludes "no cars are moving" even though the rest
            // of the traffic loop is still cycling normally.
            if (hitCar != null) hitCar.ExitImpactMode();

            if (missionController != null)
            {
                missionController.EmitGuidance(
                    "feedback.fail.red_light", positiveTone: false, guidanceType: "accident");
            }

            MissionFailed?.Invoke();
        }

        // Tween Time.timeScale over real-time seconds with a smoothstep curve.
        private static IEnumerator RampTimeScale(float from, float to, float realSeconds)
        {
            if (realSeconds <= 0f) { Time.timeScale = to; yield break; }
            float startReal = Time.unscaledTime;
            while (Time.unscaledTime - startReal < realSeconds)
            {
                float t = (Time.unscaledTime - startReal) / realSeconds;
                Time.timeScale = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            Time.timeScale = to;
        }

    }
}
