using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Auto-walks the kid forward (toward +Z) at a constant speed when allowed.
    /// External systems (CrosswalkZone, SchoolGoal, etc.) call SetWalkingAllowed
    /// to gate movement — the player taps the GO button on the HUD to flip
    /// this back on at the curb. Manual WASD steering still works as a debug
    /// aid in the editor, but the primary loop is auto-walk + tap-to-cross.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class KidPlayer : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Running speed in m/s. ~7 is a fast, obvious kid's run (matches a real jog).")]
        [SerializeField] private float walkSpeed = 7f;
        [Tooltip("Acceleration in m/s² while ramping UP to walkSpeed. Higher = snappier start.")]
        [SerializeField] private float acceleration = 22f;
        [Tooltip("Deceleration in m/s² while ramping DOWN to a stop. Higher = harder stop.")]
        [SerializeField] private float deceleration = 40f;
        [SerializeField] private float turnSpeedDegPerSec = 540f;
        [Tooltip("Higher = faster rotation easing on top of turnSpeedDegPerSec. 8-14 is good.")]
        [SerializeField] private float rotationEaseSpeed = 11f;
        [SerializeField] private float gravity = -15f;

        [Header("State")]
        [Tooltip("Whether the kid is currently running. Starts TRUE — she auto-starts " +
                 "running as soon as the scene loads. STOP pauses her; GO resumes.")]
        [SerializeField] private bool walkingAllowed = true;

        [Header("Debug controls (editor only)")]
        [SerializeField] private bool allowKeyboardInput = true;

        [Header("Run bob (procedural animation)")]
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private float bobAmplitude = 0.12f;
        [SerializeField] private float bobFrequency = 13f;
        [SerializeField] private HumanoidLimbAnimator limbAnimator;

        [Header("Accident state")]
        [SerializeField] private bool inAccident;
        [SerializeField] private float accidentKnockback = 1.2f;
        [Tooltip("How long the knockback velocity is applied before the kid settles.")]
        [SerializeField] private float accidentKnockDuration = 0.7f;
        [Tooltip("How far the body tips over while ragdolling (degrees).")]
        [SerializeField] private float accidentTumbleDegrees = 95f;
        [Tooltip("Local-Y of the kid's feet relative to the body pivot. The body " +
                 "rotates around this point during the accident so the feet stay " +
                 "anchored to the ground instead of swinging up.")]
        [SerializeField] private float accidentFootPivotY = -0.71f;

        private CharacterController _cc;
        private float _verticalVelocity;
        private float _bobPhase;
        private Vector3 _bodyBase;
        private Quaternion _bodyBaseRotation;
        private float _accidentStartTime;
        private Vector3 _accidentDirection;
        private float _accidentForce;
        private float _currentSpeed;       // m/s, smoothly ramped toward target
        private Vector3 _facing = Vector3.forward;  // smoothed heading

        public bool IsWalkingAllowed => walkingAllowed;
        public bool IsMoving { get; private set; }
        /// <summary>Current horizontal speed in m/s — used by anim/HUD systems.</summary>
        public float CurrentSpeed => _currentSpeed;
        /// <summary>Top speed in m/s (the value she ramps toward when running).</summary>
        public float MaxSpeed => walkSpeed;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (bodyTransform != null)
            {
                _bodyBase = bodyTransform.localPosition;
                _bodyBaseRotation = bodyTransform.localRotation;
            }
            // Force auto-start every time the scene loads, regardless of any
            // stale serialized value in the scene file. STOP/GO still work
            // after start because they're set by user input AFTER Awake runs.
            walkingAllowed = true;

            // Defensive guards — if the scene was last built with an older
            // version of this script that didn't have these fields, Unity
            // deserialized them as 0 which silently kills movement. Reset
            // them to sane defaults so a "not running" bug can't survive.
            // Also force-upgrade any old serialized walkSpeed that's slower
            // than a brisk run — the user explicitly wants the kid running
            // visibly, not walking.
            if (walkSpeed   <  6f)    walkSpeed = 7f;
            if (acceleration <= 0.1f) acceleration = 22f;
            if (deceleration <= 0.1f) deceleration = 26f;
            if (turnSpeedDegPerSec <= 1f) turnSpeedDegPerSec = 540f;
            if (rotationEaseSpeed <= 0.1f) rotationEaseSpeed = 11f;
            if (gravity > -0.1f) gravity = -15f;

            // Skip the accel ramp at scene start so the player is visibly
            // running from frame 1 instead of standing still for a moment.
            _currentSpeed = walkSpeed * 0.7f;
        }

        public bool InAccident => inAccident;

        public void SetWalkingAllowed(bool allowed)
        {
            if (inAccident) return;
            walkingAllowed = allowed;
        }

        public void ConfigureBody(Transform body)
        {
            bodyTransform = body;
            if (bodyTransform != null)
            {
                _bodyBase = bodyTransform.localPosition;
                _bodyBaseRotation = bodyTransform.localRotation;
            }
        }

        public void ConfigureLimbAnimator(HumanoidLimbAnimator animator)
        {
            limbAnimator = animator;
            if (limbAnimator != null) limbAnimator.SetRunGait();
        }

        /// <summary>
        /// Teleport the kid to a new world position and facing. The
        /// CharacterController has to be temporarily disabled so it doesn't
        /// fight the position change. Resets the accident/walking flags.
        /// </summary>
        public void TeleportTo(Vector3 worldPos, Quaternion worldRot, bool resumeWalking)
        {
            bool wasEnabled = _cc != null && _cc.enabled;
            if (_cc != null) _cc.enabled = false;
            transform.SetPositionAndRotation(worldPos, worldRot);
            if (_cc != null) _cc.enabled = wasEnabled;

            inAccident = false;
            _verticalVelocity = -2f;
            walkingAllowed = resumeWalking;

            // Reset the visual body rotation so the kid stands upright again
            // (in case she was mid-tumble from a previous accident).
            if (bodyTransform != null)
            {
                bodyTransform.localRotation = _bodyBaseRotation;
                bodyTransform.localPosition = _bodyBase;
            }
        }

        /// <summary>
        /// Trigger accident state: kid falls over, gets knocked along the
        /// hit direction. The MAGNITUDE of <paramref name="hitDirection"/>
        /// scales the knockback force (so callers can pass a stronger push
        /// for a car-impact vs. a gentle bump).
        /// </summary>
        public void TriggerAccident(Vector3 hitDirection)
        {
            if (inAccident) return;
            inAccident = true;
            walkingAllowed = false;
            _accidentStartTime = Time.time;
            float mag = hitDirection.magnitude;
            _accidentDirection = mag > 0.001f
                ? hitDirection / mag
                : -transform.forward;
            // Hit magnitude scales the punch (clamped so it can't go crazy).
            _accidentForce = Mathf.Clamp(mag, 1f, 4.5f);
        }

        private void Update()
        {
            if (inAccident)
            {
                UpdateAccident();
                return;
            }

            // Manual keyboard input (debug only). On mobile this stays zero.
            Vector2 inp = Vector2.zero;
            if (allowKeyboardInput)
            {
                inp.x = Input.GetAxisRaw("Horizontal");
                inp.y = Input.GetAxisRaw("Vertical");
            }

            if (!walkingAllowed && inp.y > 0f) inp.y = 0f;

            bool wantsToMove;
            Vector3 desiredDir;
            if (inp.sqrMagnitude < 0.01f && walkingAllowed)
            {
                // No manual input — auto-run in the kid's CURRENT facing
                // direction. This lets us turn her 180° during the return
                // trip and have her run south instead of always world-north.
                desiredDir = transform.forward;
                desiredDir.y = 0f;
                if (desiredDir.sqrMagnitude > 0.001f) desiredDir.Normalize();
                wantsToMove = true;
            }
            else if (inp.sqrMagnitude > 0.01f)
            {
                desiredDir = new Vector3(inp.x, 0f, inp.y);
                if (desiredDir.sqrMagnitude > 1f) desiredDir.Normalize();
                wantsToMove = walkingAllowed;
            }
            else
            {
                // walkingAllowed is FALSE and no manual input — stop.
                desiredDir = transform.forward;
                desiredDir.y = 0f;
                if (desiredDir.sqrMagnitude > 0.001f) desiredDir.Normalize();
                wantsToMove = false;
            }

            // Smooth heading turn: hard ceiling at turnSpeedDegPerSec, but
            // exponential ease underneath so the kid eases into corners
            // instead of snapping.
            if (desiredDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(desiredDir, Vector3.up);
                float easeT = 1f - Mathf.Exp(-rotationEaseSpeed * Time.deltaTime);
                Quaternion eased = Quaternion.Slerp(transform.rotation, targetRot, easeT);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, eased, turnSpeedDegPerSec * Time.deltaTime);
            }

            // Ramp speed smoothly toward target (walkSpeed when running, 0 when stopping).
            float targetSpeed = wantsToMove ? walkSpeed : 0f;
            float rate = wantsToMove ? acceleration : deceleration;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.deltaTime);

            if (_cc.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 moveDir = transform.forward;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 0.001f) moveDir.Normalize();
            Vector3 velocity = moveDir * _currentSpeed;
            velocity.y = _verticalVelocity;
            _cc.Move(velocity * Time.deltaTime);

            IsMoving = _currentSpeed > 0.05f;
            UpdateBob();
            if (limbAnimator != null) limbAnimator.SetMoving(IsMoving, _currentSpeed / Mathf.Max(0.01f, walkSpeed));
        }

        private void UpdateAccident()
        {
            // Knockback for accidentKnockDuration, then settle on the ground.
            // Horizontal only — the kid SLIDES along the road, she does NOT
            // get launched into the air. Gravity keeps her stuck to the ground.
            float t = Time.time - _accidentStartTime;
            if (t < accidentKnockDuration)
            {
                float u = t / accidentKnockDuration;
                float decay = 1f - Mathf.SmoothStep(0f, 1f, u);
                float push = decay * accidentKnockback * _accidentForce;
                Vector3 horizontal = _accidentDirection;
                horizontal.y = 0f;
                if (horizontal.sqrMagnitude > 0.001f) horizontal.Normalize();
                Vector3 vel = horizontal * push;
                _verticalVelocity += gravity * Time.deltaTime;
                if (_cc.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
                vel.y = _verticalVelocity;
                _cc.Move(vel * Time.deltaTime);
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
                if (_cc.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
                _cc.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f));
            }

            // Tumble + face-plant. We pivot around the FEET (not the hips) so
            // the feet stay anchored to the ground instead of swinging up
            // into the air. After applying the rotation, we translate the
            // visualRoot down + forward so the foot's local position is the
            // same as before — effectively rotating around that anchor point.
            if (bodyTransform != null)
            {
                Quaternion target = _bodyBaseRotation
                    * Quaternion.Euler(accidentTumbleDegrees, 0f, 25f);
                bodyTransform.localRotation = Quaternion.RotateTowards(
                    bodyTransform.localRotation, target, 380f * Time.deltaTime);

                Vector3 footLocal = new Vector3(0f, accidentFootPivotY, 0f);
                Vector3 rotatedFoot = bodyTransform.localRotation * footLocal;
                Vector3 newPos = _bodyBase;
                newPos.y += footLocal.y - rotatedFoot.y;
                newPos.z += footLocal.z - rotatedFoot.z;
                bodyTransform.localPosition = newPos;
            }

            IsMoving = false;
            if (limbAnimator != null) limbAnimator.SetMoving(false);
        }

        private void UpdateBob()
        {
            if (bodyTransform == null) return;
            // Bob amplitude scales with current speed so starting/stopping
            // is a smooth ramp instead of a hard switch.
            float speedT = Mathf.Clamp01(_currentSpeed / Mathf.Max(0.01f, walkSpeed));
            float amp = bobAmplitude * speedT;
            _bobPhase += Time.deltaTime * bobFrequency * Mathf.Max(0.25f, speedT);
            float y = Mathf.Abs(Mathf.Sin(_bobPhase)) * amp;
            Vector3 desired = _bodyBase + new Vector3(0f, y, 0f);
            bodyTransform.localPosition = Vector3.Lerp(
                bodyTransform.localPosition, desired, 1f - Mathf.Exp(-15f * Time.deltaTime));
        }
    }
}
