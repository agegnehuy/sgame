using System;
using System.Collections;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Orchestrates the round-trip level structure:
    ///   1. Kid runs from home → reaches the school front door (SchoolGoal).
    ///   2. SchoolDayStarted is fired so the HUD can show a "Inside school..."
    ///      overlay. A short timer simulates the school day.
    ///   3. SchoolDayEnded fires; the kid is teleported to the BACK door of the
    ///      school facing south, the crosswalk state is reset, and the
    ///      ReturnTripStarted event fires.
    ///   4. The kid auto-runs in her forward direction; RoundTripController
    ///      steers her around the east side of the school, west to the zebra,
    ///      then straight south to home.
    ///   5. HomeGoal arms when the return trip starts; when it fires we emit
    ///      RoundTripCompleted so the HUD can show the final result panel.
    /// </summary>
    public class RoundTripController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SchoolGoal schoolGoal;
        [SerializeField] private HomeGoal homeGoal;
        [SerializeField] private CrosswalkZone crosswalk;
        [SerializeField] private KidPlayer kid;
        [SerializeField] private ObjectiveArrow objectiveArrow;
        [SerializeField] private Transform schoolFrontTarget;
        [SerializeField] private Transform schoolBackTarget;
        [SerializeField] private Transform homeTarget;

        [Header("Return trip")]
        [Tooltip("World position the kid is teleported to when she comes out the back of the school. " +
                 "Should be slightly behind (positive Z of) and east of the school building.")]
        [SerializeField] private Vector3 backExitWorldPos = new Vector3(4.5f, 1f, 31f);

        [Tooltip("Ordered waypoints the kid steers through on her way home. The first " +
                 "should be near the back exit; the last should be at/near home.")]
        [SerializeField] private Vector3[] returnWaypoints;

        [Tooltip("How close (m) the kid has to get to a waypoint before advancing to the next.")]
        [SerializeField] private float waypointReachDistance = 1.6f;

        [Tooltip("How fast (deg/sec) the kid is steered toward the current waypoint.")]
        [SerializeField] private float steeringDegPerSec = 360f;

        [Tooltip("Exponential ease factor on steering — higher = quicker response. Combined " +
                 "with steeringDegPerSec as a hard cap to avoid whip-around turns.")]
        [SerializeField] private float steeringEaseSpeed = 4.5f;

        [Tooltip("If the kid hasn't gotten meaningfully closer to the current waypoint " +
                 "after this many seconds, skip to the next one (anti-stuck safety net).")]
        [SerializeField] private float stuckTimeoutSeconds = 4.0f;

        [Tooltip("Seconds the in-school overlay is shown before the return trip starts. " +
                 "Kept VERY short so the kid is visibly running again almost immediately " +
                 "— a long pause was reading as 'the kid stopped' from the player's POV.")]
        [SerializeField] private float schoolDaySeconds = 1.2f;

        [Tooltip("World-space anchor inside the classroom diorama. The kid + camera are teleported here for the school day.")]
        [SerializeField] private Transform classroomAnchor;

        public void SetClassroom(Transform anchor)
        {
            classroomAnchor = anchor;
        }

        [Header("Transition")]
        [Tooltip("Seconds to fade to black after the kid reaches the school door.")]
        [SerializeField] private float fadeInSeconds = 0.55f;
        [Tooltip("Seconds to fade back from black after the teleport to the back door.")]
        [SerializeField] private float fadeOutSeconds = 0.65f;
        [Tooltip("Extra time to hold the screen black on top of schoolDaySeconds, so the " +
                 "in-school overlay has room to read.")]
        [SerializeField] private float extraBlackHoldSeconds = 0.6f;

        public event Action SchoolDayStarted;
        public event Action SchoolDayEnded;      // fires AFTER the overlay; before the kid teleports
        public event Action ReturnTripStarted;   // fires once the kid is at the back door & walking
        public event Action RoundTripCompleted;

        private bool _schoolReached;
        private bool _completed;
        private bool _returnActive;
        private int  _currentWaypoint;
        private float _lastWaypointProgressTime;
        private float _lastDistanceSqr;

        public void Configure(SchoolGoal school, HomeGoal home, CrosswalkZone crossing, KidPlayer kidPlayer)
        {
            schoolGoal = school;
            homeGoal = home;
            crosswalk = crossing;
            kid = kidPlayer;
        }

        public void ConfigureObjectiveArrow(ObjectiveArrow arrow, Transform front, Transform back, Transform home)
        {
            objectiveArrow = arrow;
            schoolFrontTarget = front;
            schoolBackTarget = back;
            homeTarget = home;
        }

        public void SetBackExit(Vector3 worldPos, float schoolDurationSeconds)
        {
            backExitWorldPos = worldPos;
            schoolDaySeconds = schoolDurationSeconds;
        }

        public void SetReturnPath(Vector3[] waypoints)
        {
            returnWaypoints = waypoints;
        }

        private void OnEnable()
        {
            if (schoolGoal != null) schoolGoal.GoalReached += OnSchoolReached;
            if (homeGoal != null) homeGoal.GoalReached += OnHomeReached;
        }

        private void Start()
        {
            // Point the arrow at the school front door at the start of the level.
            if (objectiveArrow != null && schoolFrontTarget != null)
            {
                objectiveArrow.SetTarget(schoolFrontTarget);
            }
        }

        private void OnDisable()
        {
            if (schoolGoal != null) schoolGoal.GoalReached -= OnSchoolReached;
            if (homeGoal != null) homeGoal.GoalReached -= OnHomeReached;
        }

        private void OnSchoolReached()
        {
            if (_schoolReached || _completed) return;
            _schoolReached = true;
            StartCoroutine(RunSchoolThenReturn());
        }

        private IEnumerator RunSchoolThenReturn()
        {
            // Stop the kid at the school door.
            if (kid != null) kid.SetWalkingAllowed(false);

            // Hide the arrow during the school overlay.
            if (objectiveArrow != null) objectiveArrow.Hide();

            SchoolDayStarted?.Invoke();

            // Fade to black so the teleport is invisible to the player. This
            // also covers the camera-snap that would otherwise look jarring.
            var fade = FadeOverlay.Get();
            yield return fade.FadeTo(1f, fadeInSeconds);

            // ── Teleport the kid into the classroom diorama and fade IN so
            //    the player actually SEES the school session.
            var follow = UnityEngine.Object.FindFirstObjectByType<FollowCamera>();
            Vector3 doorPos = Vector3.zero;
            Quaternion doorRot = Quaternion.identity;
            bool haveClassroom = classroomAnchor != null && kid != null;
            if (haveClassroom)
            {
                doorPos = kid.transform.position;
                doorRot = kid.transform.rotation;
                kid.TeleportTo(classroomAnchor.position, classroomAnchor.rotation, resumeWalking: false);
                if (follow != null) follow.SnapToTarget();

                // Fade IN — the classroom is now visible to the player.
                yield return fade.FadeTo(0f, fadeOutSeconds);

                // Hold the classroom view for the duration of the school day.
                yield return new WaitForSecondsRealtime(schoolDaySeconds);

                // Fade OUT before the next teleport.
                yield return fade.FadeTo(1f, fadeInSeconds);
            }
            else
            {
                // Fallback: just hold the black screen with the HUD overlay.
                yield return fade.Hold(schoolDaySeconds + extraBlackHoldSeconds);
            }

            SchoolDayEnded?.Invoke();

            // Reset the crosswalk so the next cross is evaluated fresh (and so
            // the trigger semantics are reversed for the south-bound leg).
            if (crosswalk != null) crosswalk.ResetForReturnTrip();

            // Teleport the kid to the back of the school, facing south (toward
            // home), and resume auto-running.
            if (kid != null)
            {
                kid.TeleportTo(
                    backExitWorldPos,
                    Quaternion.LookRotation(Vector3.back, Vector3.up),
                    resumeWalking: true);

                // Snap the follow camera so it doesn't slide across the
                // entire map catching up to the teleport.
                if (follow == null) follow = UnityEngine.Object.FindFirstObjectByType<FollowCamera>();
                if (follow != null) follow.SnapToTarget();
            }

            // Arm the home trigger now that the return trip has started.
            if (homeGoal != null) homeGoal.Arm();

            _currentWaypoint = 0;
            _lastWaypointProgressTime = Time.time;
            _lastDistanceSqr = float.PositiveInfinity;
            _returnActive = true;

            // Point the arrow at the HOME door for the return leg.
            if (objectiveArrow != null && homeTarget != null)
            {
                objectiveArrow.SetTarget(homeTarget);
            }

            ReturnTripStarted?.Invoke();

            // Fade back in — the player sees the kid emerging from the back door.
            yield return fade.FadeTo(0f, fadeOutSeconds);
        }

        private void Update()
        {
            if (!_returnActive || _completed || kid == null) return;
            if (returnWaypoints == null || returnWaypoints.Length == 0) return;
            if (_currentWaypoint >= returnWaypoints.Length) return;

            Vector3 kidPos = kid.transform.position;
            Vector3 wpPos  = returnWaypoints[_currentWaypoint];
            Vector3 toWp   = wpPos - kidPos;
            toWp.y = 0f;

            float dSqr = toWp.sqrMagnitude;

            // Reached this waypoint — advance to the next one.
            if (dSqr < waypointReachDistance * waypointReachDistance)
            {
                AdvanceWaypoint();
                return;
            }

            // Stuck-detection: if she stops getting closer to the waypoint
            // for stuckTimeoutSeconds, skip ahead so the level can't hang.
            if (dSqr < _lastDistanceSqr - 0.05f)
            {
                _lastDistanceSqr = dSqr;
                _lastWaypointProgressTime = Time.time;
            }
            else if (Time.time - _lastWaypointProgressTime > stuckTimeoutSeconds)
            {
                Debug.LogWarning("[RoundTrip] Stuck on waypoint " + _currentWaypoint + " — skipping.");
                AdvanceWaypoint();
                return;
            }

            // Steer the kid's facing toward the waypoint with the same eased+
            // capped rotation profile the player uses. KidPlayer.Update will
            // auto-walk in her forward direction.
            Quaternion targetRot = Quaternion.LookRotation(toWp.normalized, Vector3.up);
            float easeT = 1f - Mathf.Exp(-steeringEaseSpeed * Time.deltaTime);
            Quaternion eased = Quaternion.Slerp(kid.transform.rotation, targetRot, easeT);
            kid.transform.rotation = Quaternion.RotateTowards(
                kid.transform.rotation, eased, steeringDegPerSec * Time.deltaTime);
        }

        private void AdvanceWaypoint()
        {
            _currentWaypoint++;
            _lastWaypointProgressTime = Time.time;
            _lastDistanceSqr = float.PositiveInfinity;
        }

        private void OnHomeReached()
        {
            if (_completed) return;
            _completed = true;
            _returnActive = false;
            if (kid != null) kid.SetWalkingAllowed(false);
            if (objectiveArrow != null) objectiveArrow.Hide();
            RoundTripCompleted?.Invoke();
        }
    }
}
