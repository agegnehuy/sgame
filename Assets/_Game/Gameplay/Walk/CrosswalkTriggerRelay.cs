using UnityEngine;

namespace SGame.Gameplay.Walk
{
    public enum CrosswalkTriggerKind
    {
        Curb,
        Road,
        FarSide
    }

    /// <summary>
    /// Attached to each of the three child trigger volumes. Forwards OnTriggerEnter/Exit
    /// from the player to the parent CrosswalkZone, distinguishing which zone it is.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CrosswalkTriggerRelay : MonoBehaviour
    {
        [SerializeField] private CrosswalkZone zone;
        [SerializeField] private CrosswalkTriggerKind kind;
        [SerializeField] private string playerTag = "Player";

        public void Configure(CrosswalkZone owner, CrosswalkTriggerKind triggerKind)
        {
            zone = owner;
            kind = triggerKind;
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (zone == null || !other.CompareTag(playerTag)) return;
            switch (kind)
            {
                case CrosswalkTriggerKind.Curb: zone.NotifyCurbEnter(); break;
                case CrosswalkTriggerKind.Road: zone.NotifyRoadEnter(); break;
                case CrosswalkTriggerKind.FarSide: zone.NotifyFarSideEnter(); break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (zone == null || !other.CompareTag(playerTag)) return;
            switch (kind)
            {
                case CrosswalkTriggerKind.Curb: zone.NotifyCurbExit(); break;
                case CrosswalkTriggerKind.Road: zone.NotifyRoadExit(); break;
            }
        }
    }
}
