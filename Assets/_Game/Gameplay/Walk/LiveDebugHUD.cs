using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Silent runtime watchdog that prevents two specific soft-locks that
    /// can otherwise persist across Play sessions:
    ///
    ///   1. <see cref="Time.timeScale"/> stuck below 1.0 (e.g. an accident
    ///      cinematic that crashed mid-coroutine and never ramped time back).
    ///   2. The kid permanently stuck with walkingAllowed = false despite
    ///      not being in an accident or the school-day pause.
    ///
    /// Class is intentionally named <c>LiveDebugHUD</c> so existing scenes
    /// with a GameObject already wired to this MonoBehaviour keep resolving
    /// — but there is NO on-screen overlay any more. To inspect runtime
    /// state, attach a profiler or temporarily re-enable Debug.Log calls.
    /// </summary>
    public class LiveDebugHUD : MonoBehaviour
    {
        private KidPlayer _kid;
        private float _nextScanAt;
        private float _lastForceWalkAt;

        private void Update()
        {
            // Restore time scale if anything left it stuck in slow-mo.
            if (Time.timeScale < 0.99f && Time.timeScale > 0.0001f)
            {
                Time.timeScale = 1f;
            }

            // Re-scan for the kid every 1.5s in case scenes were swapped.
            if (Time.time >= _nextScanAt || _kid == null)
            {
                _nextScanAt = Time.time + 1.5f;
                _kid = Object.FindFirstObjectByType<KidPlayer>();
            }

            if (_kid != null && !_kid.InAccident && !_kid.IsWalkingAllowed)
            {
                // The school-day / home-arrival flow legitimately holds her
                // stopped — those coroutines re-enable walking themselves.
                // This safety net only kicks in if she's been stuck for 8s+.
                if (Time.time - _lastForceWalkAt > 8f)
                {
                    _lastForceWalkAt = Time.time;
                    _kid.SetWalkingAllowed(true);
                }
            }
        }
    }
}
