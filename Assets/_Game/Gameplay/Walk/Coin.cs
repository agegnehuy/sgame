using System;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Spinning collectible coin. Sits in the air with a slow bob and continuous
    /// spin until the player walks through its trigger, then fires the static
    /// <c>Collected</c> event with its value and destroys itself.
    ///
    /// IMPORTANT: Coins do NOT persist to the wallet on pickup. They are tallied
    /// per-run by the HUD and only committed to the save file when the mission
    /// succeeds (kid reaches school). On accident the run's coins are lost.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int value = 1;
        [SerializeField] private float spinDegPerSec = 180f;
        [SerializeField] private float bobAmplitude = 0.12f;
        [SerializeField] private float bobFrequency = 1.6f;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private Transform visualRoot;

        /// <summary>Fired with the coin's value when it is picked up.</summary>
        public static event Action<int> Collected;

        private Vector3 _basePos;
        private bool _collected;
        private float _phase;

        public void Configure(int coinValue, Transform visual)
        {
            value = Mathf.Max(1, coinValue);
            visualRoot = visual;
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void Awake()
        {
            _basePos = transform.position;
            _phase = UnityEngine.Random.value * 6.28f;
        }

        private void Update()
        {
            if (_collected) return;

            _phase += Time.deltaTime;
            float y = _basePos.y + Mathf.Sin(_phase * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
            transform.position = new Vector3(_basePos.x, y, _basePos.z);

            if (visualRoot != null)
            {
                visualRoot.Rotate(Vector3.up, spinDegPerSec * Time.deltaTime, Space.Self);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_collected) return;
            if (!other.CompareTag(playerTag)) return;

            _collected = true;
            Collected?.Invoke(value);
            Debug.Log($"[Coin] Picked up (+{value} this run, not yet saved).");

            if (visualRoot != null)
            {
                visualRoot.localScale *= 1.4f;
            }
            Destroy(gameObject, 0.08f);
        }
    }
}
