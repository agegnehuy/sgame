using System.Collections.Generic;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Lightweight registry that tracks every walking character (player + NPCs)
    /// so they can keep personal space and avoid overlapping each other.
    ///
    /// Each character adds one of these as a sibling component. Movement
    /// systems (NPCWalker, CrosswalkNPCWalker) query <see cref="IsBlockedAhead"/>
    /// and pause briefly when someone else is in front of them. The kid's
    /// CharacterController collides physically via the CapsuleCollider that
    /// scripts attach alongside this component, so she can never run THROUGH
    /// an NPC — she slides around them naturally.
    /// </summary>
    public class CharacterPresence : MonoBehaviour
    {
        [Tooltip("Personal-space radius. Other characters within (myRadius + otherRadius) are considered overlapping.")]
        [SerializeField] private float radius = 0.45f;

        public float Radius => radius;
        public void SetRadius(float r) => radius = Mathf.Max(0.05f, r);

        public static readonly List<CharacterPresence> All = new List<CharacterPresence>();

        private void OnEnable()  { if (!All.Contains(this)) All.Add(this); }
        private void OnDisable() { All.Remove(this); }

        /// <summary>
        /// Returns true if any other registered character is within
        /// <paramref name="lookAhead"/> meters along <paramref name="forward"/>
        /// AND in a forward arc of <paramref name="halfAngleDeg"/> degrees.
        /// Used by NPCs to pause when someone is in front of them.
        /// </summary>
        public bool IsBlockedAhead(Vector3 forward, float lookAhead, float halfAngleDeg = 50f)
        {
            Vector3 myPos = transform.position;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) return false;
            forward.Normalize();
            float cosLimit = Mathf.Cos(halfAngleDeg * Mathf.Deg2Rad);

            for (int i = 0; i < All.Count; i++)
            {
                var other = All[i];
                if (other == null || other == this) continue;
                Vector3 delta = other.transform.position - myPos;
                delta.y = 0f;
                float dist = delta.magnitude;
                if (dist < 0.001f) continue;
                float reach = lookAhead + radius + other.radius;
                if (dist > reach) continue;
                Vector3 dir = delta / dist;
                if (Vector3.Dot(dir, forward) >= cosLimit) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a separation vector that pushes this character away from
        /// any other characters whose personal-space spheres overlap. Used
        /// for gentle nudging — apply it to position with a small weight.
        /// </summary>
        public Vector3 GetSeparation(float padding = 0.25f)
        {
            Vector3 sep = Vector3.zero;
            Vector3 myPos = transform.position;
            for (int i = 0; i < All.Count; i++)
            {
                var other = All[i];
                if (other == null || other == this) continue;
                Vector3 delta = myPos - other.transform.position;
                delta.y = 0f;
                float d = delta.magnitude;
                if (d < 0.0001f) continue;
                float r = radius + other.radius + padding;
                if (d >= r) continue;
                sep += delta.normalized * ((r - d) / r);
            }
            return sep;
        }
    }
}
