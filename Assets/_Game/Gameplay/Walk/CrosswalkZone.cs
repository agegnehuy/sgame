using System;
using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Watches the kid's presence on the zebra and continuously evaluates
    /// crossing safety against the CARS' traffic light.
    ///
    ///   - If the kid is on the road AT ANY TIME while the cars' light is
    ///     GREEN (cars going), trigger an accident — even if she stepped onto
    ///     the zebra while the light was red and the light then changed.
    ///   - If the kid reaches the far-side trigger before that happens,
    ///     forward to MissionController.OnCrossAttempt for scoring and lock
    ///     in a safe cross.
    ///
    /// The WAIT / CROSS buttons on the HUD pause / resume the kid's auto-walk
    /// at any time — they no longer gate progress at the curb.
    /// </summary>
    public class CrosswalkZone : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private TrafficLight3D trafficLight;
        [SerializeField] private KidPlayer kidPlayer;

        private bool _kidOnRoad;
        private bool _safeCrossed;
        private bool _isAccident;
        private bool _atCurb;

        public event Action MissionFailed;
        public event Action SafelyCrossed;

        public bool AtCurb => _atCurb;
        public bool CrossResolved => _safeCrossed || _isAccident;
        public bool FarSideReached => _safeCrossed;
        public TrafficLight3D Light => trafficLight;
        public KidPlayer Kid => kidPlayer;

        public void Configure(MissionController controller, TrafficLight3D light, KidPlayer kid)
        {
            missionController = controller;
            trafficLight = light;
            kidPlayer = kid;
        }

        private void Update()
        {
            // Continuous accident check: kid on road + cars' light GREEN +
            // cross not yet resolved → ACCIDENT, regardless of how she got there.
            if (_kidOnRoad && !_safeCrossed && !_isAccident
                && trafficLight != null && trafficLight.IsGreen)
            {
                TriggerAccident();
            }
        }

        public void NotifyCurbEnter()
        {
            _atCurb = true;
            if (missionController != null) missionController.SetCrosswalkUsage(true);
        }

        public void NotifyCurbExit()
        {
            _atCurb = false;
            if (missionController != null) missionController.SetCrosswalkUsage(false);
        }

        public void NotifyRoadEnter()
        {
            if (_safeCrossed || _isAccident) return;

            _kidOnRoad = true;
            if (missionController != null) missionController.SetCrosswalkUsage(true);

            // Stepped onto the road while cars are GREEN → instant accident.
            if (trafficLight != null && trafficLight.IsGreen)
            {
                TriggerAccident();
            }
        }

        public void NotifyRoadExit()
        {
            _kidOnRoad = false;
        }

        public void NotifyFarSideEnter()
        {
            if (_isAccident || _safeCrossed) return;

            // Reached the far side without being hit — safe cross.
            _safeCrossed = true;
            _kidOnRoad = false;
            if (missionController != null) missionController.OnCrossAttempt();
            SafelyCrossed?.Invoke();
        }

        public void RequestCross()
        {
            if (kidPlayer != null) kidPlayer.SetWalkingAllowed(true);
        }

        public void RequestWait()
        {
            if (kidPlayer != null) kidPlayer.SetWalkingAllowed(false);
            if (_atCurb && !CrossResolved && missionController != null)
            {
                missionController.OnWaitAction();
            }
        }

        private void TriggerAccident()
        {
            if (_isAccident) return;
            _isAccident = true;

            // Pick a believable knock direction: backward along the kid's
            // forward, but biased toward the world Z direction the kid is
            // facing so she falls "into" the lane she was crossing toward.
            Vector3 hit = Vector3.back;
            if (kidPlayer != null)
            {
                hit = -kidPlayer.transform.forward;
                kidPlayer.TriggerAccident(hit);
            }

            if (missionController != null)
            {
                missionController.EmitGuidance(
                    "feedback.fail.red_light", positiveTone: false, guidanceType: "accident");
            }
            MissionFailed?.Invoke();
        }
    }
}
