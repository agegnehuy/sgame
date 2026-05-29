using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Auto-walks the kid forward (toward +Z) at a constant speed when allowed.
    /// External systems (CrosswalkZone, SchoolGoal, etc.) call SetWalkingAllowed
    /// to gate movement — the player taps the CROSS button on the HUD to flip
    /// this back on at the curb. Manual WASD steering still works as a debug
    /// aid in the editor, but the primary loop is auto-walk + tap-to-cross.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class KidPlayer : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Walking speed in m/s. ~2.5 is a moderate child pace for level 1.")]
        [SerializeField] private float walkSpeed = 2.5f;
        [SerializeField] private float turnSpeedDegPerSec = 540f;
        [SerializeField] private float gravity = -15f;

        [Header("State")]
        [Tooltip("Kid starts walking automatically when the scene loads. WAIT pauses her; CROSS resumes.")]
        [SerializeField] private bool walkingAllowed = true;

        [Header("Debug controls (editor only)")]
        [SerializeField] private bool allowKeyboardInput = true;

        [Header("Walking bob (procedural animation)")]
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private float bobAmplitude = 0.05f;
        [SerializeField] private float bobFrequency = 8f;
        [SerializeField] private HumanoidLimbAnimator limbAnimator;

        [Header("Accident state")]
        [SerializeField] private bool inAccident;
        [SerializeField] private float accidentKnockback = 1.2f;

        private CharacterController _cc;
        private float _verticalVelocity;
        private float _bobPhase;
        private Vector3 _bodyBase;
        private Quaternion _bodyBaseRotation;
        private float _accidentStartTime;
        private Vector3 _accidentDirection;

        public bool IsWalkingAllowed => walkingAllowed;
        public bool IsMoving { get; private set; }

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (bodyTransform != null)
            {
                _bodyBase = bodyTransform.localPosition;
                _bodyBaseRotation = bodyTransform.localRotation;
            }
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
        }

        /// <summary>
        /// Trigger accident state: kid falls over, gets knocked backward.
        /// </summary>
        public void TriggerAccident(Vector3 hitDirection)
        {
            if (inAccident) return;
            inAccident = true;
            walkingAllowed = false;
            _accidentStartTime = Time.time;
            _accidentDirection = hitDirection.sqrMagnitude > 0.001f
                ? hitDirection.normalized
                : -transform.forward;
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

            // Auto-walk forward when allowed and no manual input.
            if (walkingAllowed && inp.sqrMagnitude < 0.01f)
            {
                inp = new Vector2(0f, 1f);
            }

            if (!walkingAllowed)
            {
                if (inp.y > 0f) inp.y = 0f;
            }

            Vector3 move = new Vector3(inp.x, 0f, inp.y);
            if (move.sqrMagnitude > 1f) move.Normalize();

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, target, turnSpeedDegPerSec * Time.deltaTime);
            }

            if (_cc.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = move * walkSpeed;
            velocity.y = _verticalVelocity;
            _cc.Move(velocity * Time.deltaTime);

            IsMoving = move.sqrMagnitude > 0.001f;
            UpdateBob();
            if (limbAnimator != null) limbAnimator.SetMoving(IsMoving);
        }

        private void UpdateAccident()
        {
            // Knockback for 0.4s, then settle on the ground.
            float t = Time.time - _accidentStartTime;
            if (t < 0.4f)
            {
                float push = (1f - t / 0.4f) * accidentKnockback;
                Vector3 vel = _accidentDirection * push + Vector3.up * (push * 0.6f);
                _verticalVelocity += gravity * Time.deltaTime;
                vel.y += _verticalVelocity;
                _cc.Move(vel * Time.deltaTime);
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
                if (_cc.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
                _cc.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f));
            }

            // Rotate the body to "fall over" — pivot 80° on its X axis facing the
            // hit direction. Smoothly interpolate so it reads as a tumble.
            if (bodyTransform != null)
            {
                Quaternion target = _bodyBaseRotation * Quaternion.Euler(80f, 0f, 0f);
                bodyTransform.localRotation = Quaternion.RotateTowards(
                    bodyTransform.localRotation, target, 360f * Time.deltaTime);
            }

            IsMoving = false;
        }

        private void UpdateBob()
        {
            if (bodyTransform == null) return;
            if (IsMoving)
            {
                _bobPhase += Time.deltaTime * bobFrequency;
                float y = Mathf.Abs(Mathf.Sin(_bobPhase)) * bobAmplitude;
                bodyTransform.localPosition = _bodyBase + new Vector3(0f, y, 0f);
            }
            else
            {
                bodyTransform.localPosition = Vector3.Lerp(
                    bodyTransform.localPosition, _bodyBase, Time.deltaTime * 10f);
            }
        }
    }
}
