using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Smooth third-person follow camera. Sits behind and above the target,
    /// looks at a head-height offset. No collision avoidance in MVP.
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 localOffset = new Vector3(0f, 4.5f, -6f);
        [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.2f, 0f);
        [SerializeField] private float smoothTime = 0.12f;

        private Vector3 _vel;

        public void Configure(Transform follow, Vector3 offset, Vector3 look)
        {
            target = follow; localOffset = offset; lookOffset = look;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + localOffset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _vel, smoothTime);
            transform.LookAt(target.position + lookOffset);
        }
    }
}
