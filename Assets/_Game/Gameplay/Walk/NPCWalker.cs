using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Decorative NPC pedestrian. Walks between two world-space points at a
    /// constant speed with a small pause at each endpoint, and gently bobs
    /// up and down while moving so it doesn't look like it's gliding.
    /// No collision, no interaction with the kid — purely visual life.
    /// </summary>
    public class NPCWalker : MonoBehaviour
    {
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB;
        [SerializeField] private float speed = 1.4f;
        [SerializeField] private float pauseAtEndsSeconds = 0.8f;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private float bobAmplitude = 0.04f;
        [SerializeField] private float bobFrequency = 7f;
        [SerializeField] private HumanoidLimbAnimator limbAnimator;

        private Vector3 _target;
        private float _resumeAt;
        private float _bobPhase;
        private Vector3 _bodyBase;

        public void Configure(Vector3 a, Vector3 b, float metersPerSecond, Transform body,
            HumanoidLimbAnimator animator = null)
        {
            pointA = a;
            pointB = b;
            speed = metersPerSecond;
            bodyTransform = body;
            limbAnimator = animator;
            if (bodyTransform != null) _bodyBase = bodyTransform.localPosition;

            transform.position = pointA;
            _target = pointB;
            FaceTarget();
        }

        private void Start()
        {
            if (bodyTransform != null) _bodyBase = bodyTransform.localPosition;
            if (_target == Vector3.zero) _target = pointB;
        }

        private void Update()
        {
            bool moving = Time.time >= _resumeAt;
            if (moving)
            {
                Vector3 pos = transform.position;
                Vector3 toTarget = _target - pos;
                float dist = toTarget.magnitude;
                if (dist < 0.05f)
                {
                    _target = (_target - pointB).sqrMagnitude < 0.01f ? pointA : pointB;
                    _resumeAt = Time.time + pauseAtEndsSeconds;
                    FaceTarget();
                    moving = false;
                }
                else
                {
                    Vector3 step = toTarget.normalized * speed * Time.deltaTime;
                    if (step.magnitude > dist) step = toTarget;
                    transform.position = pos + step;
                }
            }

            // Walking bob
            if (bodyTransform != null)
            {
                if (moving)
                {
                    _bobPhase += Time.deltaTime * bobFrequency;
                    float y = Mathf.Abs(Mathf.Sin(_bobPhase)) * bobAmplitude;
                    bodyTransform.localPosition = _bodyBase + new Vector3(0f, y, 0f);
                }
                else
                {
                    bodyTransform.localPosition = Vector3.Lerp(
                        bodyTransform.localPosition, _bodyBase, Time.deltaTime * 8f);
                }
            }

            if (limbAnimator != null) limbAnimator.SetMoving(moving);
        }

        private void FaceTarget()
        {
            Vector3 dir = _target - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }
        }
    }
}
