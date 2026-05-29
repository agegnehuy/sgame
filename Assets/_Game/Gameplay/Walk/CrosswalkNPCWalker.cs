using System;
using System.Collections.Generic;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// NPC that walks a closed loop of waypoints. Some waypoints are flagged as
    /// "wait for cars red" — at those, the NPC pauses until the traffic light
    /// shows red for cars before continuing. This demonstrates safe crossing
    /// behaviour for the player to imitate.
    ///
    /// Movement is kinematic (direct Transform writes — no physics) and the
    /// NPC bobs vertically while walking, just like the kid.
    /// </summary>
    public class CrosswalkNPCWalker : MonoBehaviour
    {
        [Serializable]
        public class Waypoint
        {
            public Vector3 position;
            public bool waitForCarsRed;
            public float pauseSeconds;
        }

        [SerializeField] private List<Waypoint> waypoints = new List<Waypoint>();
        [SerializeField] private float walkSpeed = 1.3f;
        [SerializeField] private float turnSpeedDegPerSec = 360f;
        [SerializeField] private TrafficLight3D trafficLight;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float bobAmplitude = 0.05f;
        [SerializeField] private float bobFrequency = 7f;
        [SerializeField] private HumanoidLimbAnimator limbAnimator;

        private int _currentIdx;
        private float _pauseUntil;
        private float _bobPhase;
        private Vector3 _bobBase;
        private bool _moving;

        public void Configure(List<Waypoint> path, float speed,
            TrafficLight3D light, Transform visual, HumanoidLimbAnimator animator = null)
        {
            waypoints = path;
            walkSpeed = speed;
            trafficLight = light;
            visualRoot = visual;
            limbAnimator = animator;
            if (visualRoot != null) _bobBase = visualRoot.localPosition;

            _currentIdx = 0;
            if (waypoints != null && waypoints.Count > 0)
            {
                transform.position = waypoints[0].position;
            }
        }

        private void Update()
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                UpdateBob(false);
                return;
            }

            if (Time.time < _pauseUntil)
            {
                UpdateBob(false);
                return;
            }

            var target = waypoints[_currentIdx];

            // Wait at the curb while cars are going (light GREEN); cross only
            // once cars are stopped (light RED).
            if (target.waitForCarsRed && trafficLight != null && trafficLight.IsGreen)
            {
                _moving = false;
                UpdateBob(false);
                return;
            }

            Vector3 delta = target.position - transform.position;
            delta.y = 0f;
            float dist = delta.magnitude;

            if (dist < 0.06f)
            {
                _pauseUntil = Time.time + Mathf.Max(0f, target.pauseSeconds);
                _currentIdx = (_currentIdx + 1) % waypoints.Count;
                _moving = false;
                UpdateBob(false);
                return;
            }

            Vector3 dir = delta / dist;
            transform.position += dir * Mathf.Min(dist, walkSpeed * Time.deltaTime);

            Quaternion want = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, want, turnSpeedDegPerSec * Time.deltaTime);

            _moving = true;
            UpdateBob(true);
        }

        private void UpdateBob(bool moving)
        {
            if (limbAnimator != null) limbAnimator.SetMoving(moving);
            if (visualRoot == null) return;
            if (moving)
            {
                _bobPhase += Time.deltaTime * bobFrequency;
                float y = Mathf.Abs(Mathf.Sin(_bobPhase)) * bobAmplitude;
                visualRoot.localPosition = _bobBase + new Vector3(0f, y, 0f);
            }
            else
            {
                visualRoot.localPosition = Vector3.Lerp(
                    visualRoot.localPosition, _bobBase, Time.deltaTime * 8f);
            }
        }
    }
}
