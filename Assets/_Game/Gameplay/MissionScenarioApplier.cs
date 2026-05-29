using SGame.Data;
using UnityEngine;

namespace SGame.Gameplay
{
    public class MissionScenarioApplier : MonoBehaviour
    {
        [SerializeField] private MissionScenarioPreset[] presets;
        [SerializeField] private MissionController missionController;
        [SerializeField] private TrafficLightController trafficLightController;
        [SerializeField] private VehicleDistanceSimulator vehicleDistanceSimulator;
        public MissionScenarioPreset CurrentPreset { get; private set; }

        private void Awake()
        {
            ApplyForSelectedMission();
        }

        public void ApplyForSelectedMission()
        {
            var missionId = PlayerSession.SelectedMissionId;
            if (string.IsNullOrWhiteSpace(missionId))
            {
                return;
            }

            var preset = FindPresetByMissionId(missionId);
            if (preset == null)
            {
                return;
            }
            CurrentPreset = preset;

            missionController?.ConfigureRuntime(
                preset.safeVehicleDistanceMeters,
                preset.requiredSafeCrosses,
                preset.allowCrossOnlyOnGreen,
                preset.timingDiscipline01);
            missionController?.ConfigureMissionIdentity(
                preset.missionId,
                preset.missionTitleKey,
                preset.missionBriefingKey);

            trafficLightController?.ConfigureTimings(preset.redDurationSeconds, preset.greenDurationSeconds);
            vehicleDistanceSimulator?.Configure(
                preset.minVehicleDistanceMeters,
                preset.maxVehicleDistanceMeters,
                preset.vehicleCycleSpeed);
        }

        private MissionScenarioPreset FindPresetByMissionId(string missionId)
        {
            if (presets == null)
            {
                return null;
            }

            foreach (var preset in presets)
            {
                if (preset != null && preset.missionId == missionId)
                {
                    return preset;
                }
            }

            return null;
        }
    }
}
