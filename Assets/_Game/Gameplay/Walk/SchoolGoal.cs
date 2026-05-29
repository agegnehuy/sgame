using System;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Trigger volume placed in front of the school door. Fires GoalReached
    /// the first time the player crosses the trigger AFTER having safely
    /// reached the far side of the crosswalk.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SchoolGoal : MonoBehaviour
    {
        [SerializeField] private CrosswalkZone crosswalk;
        [SerializeField] private string playerTag = "Player";

        private bool _fired;

        public bool Reached => _fired;
        public event Action GoalReached;

        public void Configure(CrosswalkZone crossing)
        {
            crosswalk = crossing;
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_fired) return;
            if (!other.CompareTag(playerTag)) return;
            if (crosswalk == null || !crosswalk.FarSideReached) return;

            _fired = true;
            Debug.Log("[SchoolGoal] Player reached school.");
            GoalReached?.Invoke();
        }
    }
}
