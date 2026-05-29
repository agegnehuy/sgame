using System.Collections;
using UnityEngine;

namespace SGame.Gameplay
{
    public class MissionChallengeEventController : MonoBehaviour
    {
        [SerializeField] private MissionController missionController;
        [SerializeField] private MissionScenarioApplier scenarioApplier;
        [SerializeField] private VehicleDistanceSimulator vehicleDistanceSimulator;

        private Coroutine _distractionCoroutine;
        private Coroutine _pressureCoroutine;
        private bool _eventsStarted;
        private bool _pressureOverrideActive;
        private float _originalMinDistance;
        private float _originalMaxDistance;
        private float _originalCycleSpeed;

        private void OnEnable()
        {
            if (missionController != null)
            {
                missionController.StateChanged += OnMissionStateChanged;
            }
        }

        private void OnDisable()
        {
            if (missionController != null)
            {
                missionController.StateChanged -= OnMissionStateChanged;
            }

            StopAllEventCoroutines();
        }

        private void OnMissionStateChanged(MissionRuntimeState state)
        {
            if (state == MissionRuntimeState.Briefing)
            {
                _eventsStarted = false;
                StopAllEventCoroutines();
                return;
            }

            if (state == MissionRuntimeState.Result)
            {
                StopAllEventCoroutines();
                return;
            }

            if (state == MissionRuntimeState.Playing && !_eventsStarted)
            {
                _eventsStarted = true;
                StartConfiguredEvents();
            }
        }

        private void StartConfiguredEvents()
        {
            var preset = scenarioApplier != null ? scenarioApplier.CurrentPreset : null;
            if (preset == null)
            {
                return;
            }

            if (preset.enableDistractionEvent)
            {
                _distractionCoroutine = StartCoroutine(DistractionRoutine(preset));
            }

            if (preset.enablePressureEvent)
            {
                _pressureCoroutine = StartCoroutine(PressureRoutine(preset));
            }
        }

        private IEnumerator DistractionRoutine(MissionScenarioPreset preset)
        {
            yield return new WaitForSeconds(Mathf.Max(0f, preset.distractionDelaySeconds));
            missionController?.EmitGuidance(preset.distractionMessageKey, true, "distraction");
        }

        private IEnumerator PressureRoutine(MissionScenarioPreset preset)
        {
            if (vehicleDistanceSimulator == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(Mathf.Max(0f, preset.pressureStartDelaySeconds));
            missionController?.EmitGuidance(preset.pressureStartMessageKey, false, "pressure_start");

            _originalMinDistance = vehicleDistanceSimulator.MinDistanceMeters;
            _originalMaxDistance = vehicleDistanceSimulator.MaxDistanceMeters;
            _originalCycleSpeed = vehicleDistanceSimulator.CycleSpeed;
            _pressureOverrideActive = true;

            vehicleDistanceSimulator.Configure(
                preset.pressureMinVehicleDistanceMeters,
                preset.pressureMaxVehicleDistanceMeters,
                preset.pressureVehicleCycleSpeed);

            yield return new WaitForSeconds(Mathf.Max(0.5f, preset.pressureDurationSeconds));

            RestorePressureOverridesIfNeeded();
            missionController?.EmitGuidance(preset.pressureEndMessageKey, true, "pressure_end");
        }

        private void StopAllEventCoroutines()
        {
            if (_distractionCoroutine != null)
            {
                StopCoroutine(_distractionCoroutine);
                _distractionCoroutine = null;
            }

            if (_pressureCoroutine != null)
            {
                StopCoroutine(_pressureCoroutine);
                _pressureCoroutine = null;
            }

            RestorePressureOverridesIfNeeded();
        }

        private void RestorePressureOverridesIfNeeded()
        {
            if (!_pressureOverrideActive || vehicleDistanceSimulator == null)
            {
                return;
            }

            vehicleDistanceSimulator.Configure(_originalMinDistance, _originalMaxDistance, _originalCycleSpeed);
            _pressureOverrideActive = false;
        }
    }
}
