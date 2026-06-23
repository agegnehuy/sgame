using System;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Trigger volume placed at the home doorway. Used as the final goal of
    /// the round-trip level — the kid leaves the school via the back door,
    /// walks south, crosses the road again, and arrives back at the home
    /// trigger. Stays disarmed until <see cref="Arm"/> is called so the kid
    /// doesn't trigger it just by spawning at home.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HomeGoal : MonoBehaviour
    {
        [SerializeField] private CrosswalkZone crosswalk;
        [SerializeField] private string playerTag = "Player";
        [Tooltip("If true the trigger fires on contact; if false it ignores triggers " +
                 "until Arm() is called externally.")]
        [SerializeField] private bool armed;

        private bool _fired;

        public bool Reached => _fired;
        public bool IsArmed => armed;
        public event Action GoalReached;

        public void Configure(CrosswalkZone crossing)
        {
            crosswalk = crossing;
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        /// <summary>Enable the trigger so the kid arriving fires GoalReached.</summary>
        public void Arm()
        {
            armed = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!armed) return;
            if (_fired) return;
            if (!other.CompareTag(playerTag)) return;
            // Require a safe return-cross before completing — otherwise the
            // kid could walk around the school without crossing the road.
            if (crosswalk == null || !crosswalk.FarSideReached) return;

            _fired = true;
            Debug.Log("[HomeGoal] Player reached home (round trip complete).");
            GoalReached?.Invoke();
        }
    }
}
