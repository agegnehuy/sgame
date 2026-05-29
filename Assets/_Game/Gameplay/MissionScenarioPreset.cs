using UnityEngine;

namespace SGame.Gameplay
{
    [CreateAssetMenu(menuName = "SGame/Mission Scenario Preset", fileName = "MissionScenarioPreset")]
    public class MissionScenarioPreset : ScriptableObject
    {
        [Header("Identity")]
        public string missionId = "P1-M1";
        public string missionSceneName = "P1_M1";
        public string missionTitleKey = "mission.p1m1.title";
        public string missionBriefingKey = "mission.p1m1.briefing";

        [Header("Traffic signal timing")]
        public float redDurationSeconds = 5f;
        public float greenDurationSeconds = 4f;

        [Header("Vehicle distance simulator")]
        public float minVehicleDistanceMeters = 4f;
        public float maxVehicleDistanceMeters = 30f;
        public float vehicleCycleSpeed = 1.2f;

        [Header("Mission rules")]
        public float safeVehicleDistanceMeters = 12f;
        public int requiredSafeCrosses = 1;
        public bool allowCrossOnlyOnGreen = true;
        [Range(0f, 1f)] public float timingDiscipline01 = 0.8f;

        [Header("Week 3 challenge events")]
        public bool enableDistractionEvent;
        public float distractionDelaySeconds = 3f;
        public string distractionMessageKey = "feedback.info.distraction";

        public bool enablePressureEvent;
        public float pressureStartDelaySeconds = 6f;
        public float pressureDurationSeconds = 6f;
        public float pressureMinVehicleDistanceMeters = 3f;
        public float pressureMaxVehicleDistanceMeters = 16f;
        public float pressureVehicleCycleSpeed = 2.2f;
        public string pressureStartMessageKey = "feedback.info.pressure_start";
        public string pressureEndMessageKey = "feedback.info.pressure_end";
    }
}
